using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;

namespace Neptune
{
    public class EngineUtil
    {
        public static unsafe float XOR(float v)
        {
            uint* vp = (uint*)&v;
            *vp = *vp ^ EngineConst.XOR;
            return v;
        }

        public static unsafe int XOR(int v)
        {
            uint* vp = (uint*)&v;
            *vp = *vp ^ EngineConst.XOR;
            return v;
        }


        static long timeDiff = 0;
        static long unixBaseMillis = new DateTime(1970, 1, 1, 0, 0, 0).ToFileTimeUtc() / 10000;
        public static long gameStartUnixTime = 0;

        public static long GetServerTime()
        {
            long diff = timeDiff;   // 服务器获取的本地时间差 
            long time = GetSystemTime() + diff;
            return time;
        }

        public static long GetSystemTime()
        {
            long seconds = GetUnixTimeStamp(DateTime.Now);
            return seconds;
        }

        public static void SetServerTime(long svr_time)
        {
            timeDiff = svr_time - GetSystemTime();
            //Debug.Log("time diff:" + timeDiff + "\n");
        }

        public static long GetMilliseconds()
        {
            return GetUnixTimeStampMillis(DateTime.Now);
        }

        public static long GetUnixTimeStamp(DateTime d)
        {
            return GetUnixTimeStampMillis(d) / 1000;
        }

        public static long GetUnixTimeStampMillis(DateTime d)
        {
            return d.ToFileTime() / 10000 - unixBaseMillis;
        }

        public static int GetGameStartMilliseconds()
        {
            if (gameStartUnixTime == 0)
            {
                gameStartUnixTime = GetMilliseconds();
            }
            return (int)(GetMilliseconds() - gameStartUnixTime);
        }

        public static long getTodayStartTime()
        {
            DateTime now = DateTime.Now;
            DateTime todayZero = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            long seconds = GetUnixTimeStamp(todayZero) + timeDiff;
            return seconds;

        }






        public static bool HitTest(UVector2 pos, AreaShape shape, UVector2 arg, Vector2 orientation)
        {
            //将坐标转换为本地坐标
            float eAngle = MathUtil.VectorToAngle(orientation, Vector2.right);
            pos = Quaternion.AngleAxis(eAngle, Vector3.back) * pos;

            switch (shape)
            {
                case AreaShape.Circle:
                    return arg.x * arg.x > pos.sqrMagnitude;
                case AreaShape.Rectangle:
                    return pos.x > 0 && arg.x > pos.x && pos.y > -arg.y / 2 && arg.y / 2 > pos.y;
                case AreaShape.Sector:
                case AreaShape.Quadrant:
                case AreaShape.SemiCircle:
                    return pos == UVector2.zero || MathUtil.PointInFanShape(pos, arg.x, arg.y, Vector2.right);
                default:
                    Logger.Log("Area Type: '" + shape + "'");
                    break;
            }
            return false;
        }

        static Vector3[] pos20 = new Vector3[] {
        new Vector3(0.187f, 1.462f, 0),
        new Vector3(4.454f, -1.003f, 0),
        new Vector3(-5.44f - 0.969f, 0),
        new Vector3(-0.663f, 4.522f, 0),
        new Vector3(-0.51f, -4.284f, 0),
        new Vector3(-3.689f, -1.581f, 0),
        new Vector3(3.026f, -2.482f, 0),
        new Vector3(-1.802f, -3.009f, 0),
        new Vector3(-4.165f, 1.513f, 0),
        new Vector3(2.805f, 1.955f, 0),
        new Vector3(-1.632f, 2.567f, 0),
        new Vector3(-1.343f, -5.559f, 0),
        new Vector3(0.102f, -2.38f, 0),
        new Vector3(-3.791f, -4.097f, 0),
        new Vector3(3.587f, 0.731f, 0),
        new Vector3(-2.397f, 0.544f, 0),
        new Vector3(0.765f, 3.094f, 0),
        new Vector3(2.057f, -4.488f, 0),
        new Vector3(1.819f, -0.587f, 0),
        new Vector3(-0.748f, -0.884f, 0),
    };
        public static List<Vector3> GetMultiPositions(int count, float spacing)
        {

            List<Vector3> offset = new List<Vector3>();
            if (count == 2)
            {
                offset.Add(new Vector3(0, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(0, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
            }
            else if (count == 3)
            {
                offset.Add(new Vector3(spacing, 0, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
            }
            else if (count == 4)
            {
                offset.Add(new Vector3(spacing, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(spacing, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
            }
            else if (count == 6)
            {
                offset.Add(new Vector3(0, spacing * 1.5f, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(spacing, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(spacing, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(-spacing, -spacing, 0) * (BattleField.Reverse ? -1f : 1f));
                offset.Add(new Vector3(0, -spacing * 1.5f, 0) * (BattleField.Reverse ? -1f : 1f));
            }
            else if (count == 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    offset.Add(pos20[i] * spacing * (BattleField.Reverse ? -1f : 1f));
                }
            }
            else
            {
                System.Random rnd = new System.Random(0);
                float radius = (float)Math.Sqrt(count);
                for (int i = 0; i < count; i++)
                {
                    float x = rnd.Next((int)(radius * 20)) / 10f - radius;
                    float y = rnd.Next((int)(radius * 20)) / 10f - radius;
                    offset.Add(new Vector3(x * spacing * 2, y * spacing * 2, 0));
                }
            }
            return offset;
        }


        /// <summary>
        /// 生产空气墙的各个点
        /// </summary>
        public static List<UVector2> GenericObstaclePoints(UVector2 centerPos, int radius, int partFactor, bool reverse = false)
        {
            // 限制在4~100等分内
            partFactor = Mathf.Clamp(partFactor, 4, 100);
            float angle = 360f / partFactor;
            List<UVector2> tempList = new List<UVector2>();
            UVector2 tempPos = EngineConst.Vector2Zero;
            float tempAngle = 0;
            for (int i = reverse ? partFactor - 1 : 0; reverse ? i >= 0 : i < partFactor; i += reverse ? -1 : 1)
            {
                tempAngle = angle * i;
                tempPos = new UVector2(UFloat.RoundToInt(radius * Mathf.Sin(tempAngle * Mathf.Deg2Rad)) + centerPos.x, UFloat.RoundToInt(radius * Mathf.Cos(tempAngle * Mathf.Deg2Rad)) + centerPos.y);
                tempList.Add(tempPos);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog && NeptuneBattle.Instance.doneFrameCount >= EngineGlobal.MinLogicframe && NeptuneBattle.Instance.doneFrameCount < EngineGlobal.MaxLogicframe)
                    NeptuneBattle.log("GenericObstaclePoints[{0}] = {1}", i, tempPos);
#endif
            }

            return tempList;
        }

        /// 生成矩形空气墙的各个点
        public static List<UVector2> GenericRectangleObstaclePoints(UVector2 centerPos, float Length, int partFactor)
        {
            // 限制在4~100等分内
            partFactor = Mathf.Clamp(partFactor, 4, 100);
            float length = Length / partFactor;
            List<UVector2> tempList = new List<UVector2>();
            UVector2 tempPos = UVector2.zero;
            float tempAngle = 45;
            for (int i = 0; i < partFactor; ++i)
            {
                tempAngle = length * i;
                if (i < partFactor / 2)
                    tempPos = centerPos + new UVector2(-tempAngle, tempAngle);
                else
                    tempPos = centerPos - new UVector2(-tempAngle, tempAngle);
                tempList.Add(tempPos);
            }
            return tempList;
        }



    }
}