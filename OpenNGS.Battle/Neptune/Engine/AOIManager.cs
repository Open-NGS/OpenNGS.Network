using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;
using Neptune.Datas;
using UnityEngine;

namespace Neptune
{

    /// <summary>
    /// AOIManager
    /// </summary>
    public class AOIManager
    {
        private TArray<Actor>[] m_Areas;
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
            this.m_Height = height;
            this.m_Width = width;
            this.m_CenterY = height / 2;
            this.m_CenterX = width / 2;
            this.m_Size = size;
            m_MaxRows = m_Height / size;
            m_MaxCols = m_Width / size;
            m_MaxIndex = m_MaxRows * m_MaxCols;
            m_Areas = new TArray<Actor>[m_MaxIndex];
            for (int i = 0; i < m_MaxIndex; i++)
            {
                m_Areas[i] = new TArray<Actor>(200);
            }

        }

        //位置发生变化
        public void Move(Actor role)
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

        public void RemoveRole(Actor role)
        {
            if (role.AOIIndex >= 0)
            {
                m_Areas[role.AOIIndex].Remove(role);
                role.AOIIndex = -1;
            }
        }

        private int GetRow(int y)
        {
            return Mathf.Clamp((y + this.m_CenterY) / m_Size, 0, this.m_MaxRows - 1);
        }

        private int GetCol(int x)
        {
            return Mathf.Clamp((x + this.m_CenterX) / m_Size, 0, this.m_MaxCols - 1);
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

        public IEnumerator<Entity> GetSurvivors(Entity self, int range)
        {
            range += NeptuneConst.EnableRadiusInDistance ? self.Radius : 0;

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
        public AOIEnumerable GetSurvivors(Entity self, RelativeSide side, int range)
        {
            this.enumerable.Reset(self, side, range);
            return this.enumerable;
        }
        public AOIEnumerable GetSurvivors(UVector2 pos, RoleSide roleSide, RelativeSide side, int range, int radius)
        {
            this.enumerable.Reset(pos, roleSide, side, range, radius);
            return this.enumerable;
        }


        public struct AOIEnumerable : IEnumerable<Entity>
        {
            AOIManager m_AOI;
            AOIEnumerator m_Enumerator;

            public AOIEnumerable(AOIManager aoi)
            {
                this.m_AOI = aoi;
                this.m_Enumerator = null;
            }

            public IEnumerator<Entity> GetEnumerator()
            {
                return this.m_Enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_Enumerator;
            }
            public void Reset(Entity self, RelativeSide side, int range)
            {
                this.m_Enumerator = this.m_AOI.EnumeratorStack.New();
                this.m_Enumerator.Reset(this.m_AOI, self.Position, self.Side, side, range, self.Radius);
            }

            public void Reset(UVector2 pos, RoleSide roleSide, RelativeSide side, int range, int radius)
            {
                this.m_Enumerator = this.m_AOI.EnumeratorStack.New();
                this.m_Enumerator.Reset(this.m_AOI, pos, roleSide, side, range, radius);
            }
        }

        protected class AOIEnumerator : IEnumerator<Entity>
        {
            int ColL, ColR, RowT, RowB;
            int x, y, i;

            Entity m_Current;

            AOIManager m_AOI;
            RelativeSide m_RelaSide;
            RoleSide m_Side;
            int m_Range;
            int m_Radius;
            UVector2 m_Position;

            public AOIEnumerator(AOIManager aoi, Entity self, RelativeSide side, int range)
            {
                this.Reset(aoi, self.Position, self.Side, side, range, self.Radius);
            }
            public AOIEnumerator()
            {
                this.m_Current = null;
                //this.self = null;
                this.m_Range = 0;
                this.m_RelaSide = RelativeSide.Both;
                this.m_Side = RoleSide.All;
                this.m_Radius = 0;
                this.m_Position = UVector2.zero;
                x = 0;
                y = 0;
                i = 0;
                ColL = 0;
                ColR = 0;
                RowT = 0;
                RowB = 0;
            }

            public Entity Current
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
                this.m_AOI.EnumeratorStack.Delete();
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

                            if (m_RelaSide == RelativeSide.Both || m_AOI.m_Areas[index][i].GetRelation(this.m_Side) == m_RelaSide)
                            {
                                if (m_AOI.m_Areas[index][i].Distance(m_Position, NeptuneConst.EnableRadiusInDistance ? m_Radius : 0, NeptuneConst.EnableRadiusInDistance) <= this.m_Range)
                                {
                                    this.m_Current = m_AOI.m_Areas[index][i];
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
                this.m_AOI = aoi;
                this.m_RelaSide = relaSide;
                this.m_Side = roleSide;
                this.m_Range = range;
                this.m_Position = position;
                this.m_Radius = NeptuneConst.EnableRadiusInDistance ? radius : 0;
                range += this.m_Radius;

                ColL = this.m_AOI.GetCol(position.x - range);//左
                ColR = this.m_AOI.GetCol(position.x + range);//右
                RowT = this.m_AOI.GetRow(position.y - range);//上
                RowB = this.m_AOI.GetRow(position.y + range);//下
                x = ColL;
                y = RowT;
                i = 0;
            }
        }
    }

}