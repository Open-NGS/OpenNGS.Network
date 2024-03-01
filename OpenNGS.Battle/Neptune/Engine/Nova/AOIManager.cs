using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;
using UnityEngine;

namespace Neptune
{

    /// <summary>
    /// AOIManager
    /// </summary>
    public class AOIManager
    {
        private TArray<BattleActor>[] m_Areas;
        private int m_Size = 0;
        private int m_Width;
        private int m_Height;
        private int m_CenterX;
        private int m_CenterY;
        private int m_MaxRows;
        private int m_MaxCols;
        private int m_MaxIndex = 0;
        private int m_TempRow = 0;
        private int m_TempCol = 0;

        ObjectStack<AOIEnumerator> EnumeratorStack = new ObjectStack<AOIEnumerator>();
        AOIEnumerable enumerable;

        /// <summary>
        /// AOIManager
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="size">size</param>
        public AOIManager(int width, int height, int size)
        {
            Init(width, height, size);
            EnumeratorStack.Init(10);
            enumerable = new AOIEnumerable(this);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="size">size</param>
        private void Init(int width, int height, int size)
        {
            m_Height = height;
            m_Width = width;
            m_CenterY = height / 2;
            m_CenterX = width / 2;
            m_Size = size;
            m_MaxRows = m_Height / size;
            m_MaxCols = m_Width / size;
            m_MaxIndex = m_MaxRows * m_MaxCols;
            m_Areas = new TArray<BattleActor>[m_MaxIndex];
            for (int i = 0; i < m_MaxIndex; i++)
            {
                m_Areas[i] = new TArray<BattleActor>(200);
            }

        }

        //位置发生变化
        public void Move(BattleActor role)
        {
            int index = GetAreaIndex(role.Position);
            if (index >= 0 && index < m_MaxIndex && index != role.AOIIndex && !role.IsDead)
            {
                if (role.AOIIndex >= 0)
                {
                    m_Areas[role.AOIIndex].Remove(role);
                }
                m_Areas[index].Add(role);
                role.AOIIndex = index;
            }
        }

        public void RemoveRole(BattleActor role)
        {
            if (role.AOIIndex >= 0)
            {
                m_Areas[role.AOIIndex].Remove(role);
                role.AOIIndex = -1;
            }
        }

        private int GetRow(int y)
        {
            return Mathf.Clamp((y + m_CenterY) / m_Size, 0, m_MaxRows - 1);
        }

        private int GetCol(int x)
        {
            return Mathf.Clamp((x + m_CenterX) / m_Size, 0, m_MaxCols - 1);
        }


        private int GetAreaIndex(UVector2 pos)
        {
            m_TempCol = GetCol(pos.x);
            m_TempRow = GetRow(pos.y);
            return m_TempRow * m_MaxRows + m_TempCol;
        }
        private int GetAreaIndex(int col, int row)
        {
            return row * m_MaxCols + col;
        }

        public IEnumerator<BattleEntity> GetSurvivors(BattleEntity self, int range)
        {
            range += EngineConst.EnableRadiusInDistance ? self.Radius : 0;

            int ColL = GetCol(self.Position.x - range);//左
            int ColR = GetCol(self.Position.x + range);//右
            int RowT = GetRow(self.Position.y - range);//上
            int RowB = GetRow(self.Position.y + range);//下

            for (int y = RowT; y <= RowB; y++)
            {
                for (int x = ColL; x <= ColR; x++)
                {
                    int index = GetAreaIndex(x, y);
                    for (int i = 0; i < m_Areas[index].Length; i++)
                    {
                        if (m_Areas[index][i] == null || m_Areas[index][i].IsDead)
                        {
                            continue;
                        }
                        yield return m_Areas[index][i];

                    }
                }
            }
        }
        public AOIEnumerable GetSurvivors(BattleEntity self, RelativeSide side, int range)
        {
            enumerable.Reset(self, side, range);
            return enumerable;
        }
        public AOIEnumerable GetSurvivors(UVector2 pos, RoleSide roleSide, RelativeSide side, int range, int radius)
        {
            enumerable.Reset(pos, roleSide, side, range, radius);
            return enumerable;
        }


        public struct AOIEnumerable : IEnumerable<BattleEntity>
        {
            AOIManager m_AOI;
            AOIEnumerator m_Enumerator;

            public AOIEnumerable(AOIManager aoi)
            {
                m_AOI = aoi;
                m_Enumerator = null;
            }

            public IEnumerator<BattleEntity> GetEnumerator()
            {
                return m_Enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return m_Enumerator;
            }
            public void Reset(BattleEntity self, RelativeSide side, int range)
            {
                m_Enumerator = m_AOI.EnumeratorStack.New();
                m_Enumerator.Reset(m_AOI, self.Position, self.Side, side, range, self.Radius);
            }

            public void Reset(UVector2 pos, RoleSide roleSide, RelativeSide side, int range, int radius)
            {
                m_Enumerator = m_AOI.EnumeratorStack.New();
                m_Enumerator.Reset(m_AOI, pos, roleSide, side, range, radius);
            }
        }

        protected class AOIEnumerator : IEnumerator<BattleEntity>
        {
            int ColL, ColR, RowT, RowB;
            int x, y, i;

            BattleEntity m_Current;

            AOIManager m_AOI;
            RelativeSide m_RelaSide;
            RoleSide m_Side;
            int m_Range;
            int m_Radius;
            UVector2 m_Position;

            public AOIEnumerator(AOIManager aoi, BattleEntity self, RelativeSide side, int range)
            {
                Reset(aoi, self.Position, self.Side, side, range, self.Radius);
            }
            public AOIEnumerator()
            {
                m_Current = null;
                //this.self = null;
                m_Range = 0;
                m_RelaSide = RelativeSide.Both;
                m_Side = RoleSide.All;
                m_Radius = 0;
                m_Position = UVector2.zero;
                x = 0;
                y = 0;
                i = 0;
                ColL = 0;
                ColR = 0;
                RowT = 0;
                RowB = 0;
            }

            public BattleEntity Current
            {
                get
                {
                    return m_Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return m_Current;
                }
            }

            public void Dispose()
            {
                m_AOI.EnumeratorStack.Delete();
            }

            public bool MoveNext()
            {
                for (; x <= ColR; x++)
                {
                    for (; y <= RowB; y++)
                    {
                        int index = m_AOI.GetAreaIndex(x, y);
                        for (; i < m_AOI.m_Areas[index].Length; i++)
                        {
                            if (m_AOI.m_Areas[index][i] == null || m_AOI.m_Areas[index][i].IsDead)
                            {
                                continue;
                            }

                            if (m_RelaSide == RelativeSide.Both || m_AOI.m_Areas[index][i].GetRelation(m_Side) == m_RelaSide)
                            {
                                if (m_AOI.m_Areas[index][i].Distance(m_Position, EngineConst.EnableRadiusInDistance ? m_Radius : 0, EngineConst.EnableRadiusInDistance) <= m_Range)
                                {
                                    m_Current = m_AOI.m_Areas[index][i];
                                    i++;
                                    return true;
                                }
                            }
                        }
                        i = 0;

                    }
                    y = RowT;
                }
                return false;
            }

            public void Reset()
            {
                m_Current = null;
                x = ColL;
                y = RowT;
                i = 0;
            }

            public void Reset(AOIManager aoi, UVector2 position, RoleSide roleSide, RelativeSide relaSide, int range, int radius)
            {
                m_AOI = aoi;
                m_RelaSide = relaSide;
                m_Side = roleSide;
                m_Range = range;
                m_Position = position;
                m_Radius = EngineConst.EnableRadiusInDistance ? radius : 0;
                range += m_Radius;

                ColL = m_AOI.GetCol(position.x - range);//左
                ColR = m_AOI.GetCol(position.x + range);//右
                RowT = m_AOI.GetRow(position.y - range);//上
                RowB = m_AOI.GetRow(position.y + range);//下
                x = ColL;
                y = RowT;
                i = 0;
            }
        }
    }
}