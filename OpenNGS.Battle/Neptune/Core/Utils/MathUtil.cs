using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MathUtil
{

    public static UVector2 SegmentsIntr(UVector2 a, UVector2 b, UVector2 c, UVector2 d)
    {
        UVector2 coress = UVector2.zero;
        // 三角形abc 面积的2倍  
        int area_abc = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);

        // 三角形abd 面积的2倍  
        int area_abd = (a.x - d.x) * (b.y - d.y) - (a.y - d.y) * (b.x - d.x);
        // 面积符号相同则两点在线段同侧,不相交 (对点在线段上的情况,本例当作不相交处理);  
        if (area_abc * area_abd >= 0)
        {
            return coress;
        }

        // 三角形cda 面积的2倍  
        int area_cda = (c.x - a.x) * (d.y - a.y) - (c.y - a.y) * (d.x - a.x);
        // 三角形cdb 面积的2倍  
        // 注意: 这里有一个小优化.不需要再用公式计算面积,而是通过已知的三个面积加减得出.  
        int area_cdb = area_cda + area_abc - area_abd;
        if (area_cda * area_cdb >= 0)
        {
            return coress;
        }

        //计算交点坐标  
        float t = UFloat.Round((float)area_cda / (area_abd - area_abc));

        coress.x = UFloat.RoundToInt(t * (b.x - a.x)) + a.x;
        coress.y = UFloat.RoundToInt(t * (b.y - a.y)) + a.y;
        return coress;

    }

    public static UVector2 SegmentsIntr2(UVector2 a, UVector2 b, UVector2 c, UVector2 d)
    {
        Vector2 cross = UVector2.zero;
        Vector2 ac = a - c;
        Vector2 bc = b - c;
        Vector2 dc = d - c;
        Vector2 ba = b - a;
        Vector2 ca = -ac;
        Vector2 da = d - a;
        float dcac = dc.x * ac.y - dc.y * ac.x;
        float dcbc = dc.x * bc.y - dc.y * bc.x;
        if (dcac * dcbc >= 0)
            return cross;
        float bada = ba.x * da.y - ba.y * da.x;
        float baca = ba.x * ca.y - ba.y * ca.x;
        if (bada * baca >= 0)
            return cross;

        float t = (ca.x * dc.y - ca.y * dc.x) / (ba.x * dc.y - ba.y * dc.x);
        cross = a + ba * t;
        return cross;
    }


    /// <summary>
    /// VectorToAngle 转角度
    /// </summary>
    /// <param name="fromV2"></param>
    /// <param name="toV2"></param>
    /// <returns></returns>
    public static float VectorToAngle(Vector2 from, Vector2 to)
    {
        float angle1 = Vector2.Angle(Vector2.right, from);
        float angle2 = Vector2.Angle(Vector2.right, to);

        if (from.y < 0)
            angle1 = -angle1;
        if (to.y < 0)
            angle2 = -angle2;
        return UFloat.Round(angle1 - angle2);
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        return UFloat.Round(lhs.x * rhs.x) + UFloat.Round(lhs.y * rhs.y);
    }
    /// <summary>
    ///   <para>Returns the angle in degrees between from and to.</para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static float Angle(Vector2 from, Vector2 to)
    {
        return UFloat.Round(Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);
    }

    /// <summary>
    /// VectorToAngleInt 转角度
    /// </summary>
    /// <param name="fromV2"></param>
    /// <param name="toV2"></param>
    /// <returns></returns>
    public static int VectorToAngleInt(Vector2 from, Vector2 to)
    {
        int angle1 = (int)MathUtil.Angle(Vector2.right, from);
        int angle2 = (int)MathUtil.Angle(Vector2.right, to);

        if (from.y < 0)
            angle1 = -angle1;
        if (to.y < 0)
            angle2 = -angle2;
        return angle1 - angle2;
    }

    public static Int64 bits(Int64 num, int start, int count)
    {
        Int64 remain = num >> start;
        Int64 mask = ((Int64)1 << count) - 1; //(Int64)Mathf.Pow(2, count) - 1;
        return remain & mask;
    }

    public static long makeBits(params int[] values)
    {
        int num_args = values.Length;

        long value = 0;
        int b0 = 0;

        for (int i = num_args; i > 1; i = i - 2)
        {
            int b = values[i - 2];
            int v = values[i - 1];

            Int64 v2 = bits(v, 0, b);
            if (v != v2)
            {
                Debug.LogError("makebits value out of bit range : " + v);
                return 0;
            }

            v = v << (int)b0;
            value = value + v;
            b0 = b0 + b;
        }

        return value;
    }

    public static System.Collections.Generic.List<int> splitbits(int value, params int[] values)
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        int num_args = values.Length;
        int b0 = 0;
        for (int i = num_args; i > 1; i = i - 1)
        {
            int b = values[i - 1];
            list.Add((int)bits(value, b0, b));
            b0 = b0 + b;
        }
        list.Reverse();
        return list;
    }

    public static bool FGreat(float numA, float numB)
    {
        //2016年10月25日
        return numA > numB && !(Math.Abs(numA - numB) < 0.00015f);
    }

    public static bool PointInFanShape(Vector2 direction, float range, float fanShapeAngle, Vector2 orientation)
    {
        if (direction.sqrMagnitude > (range * range))
        {
            return false;
        }

        fanShapeAngle = fanShapeAngle % 360;
        float a = direction.magnitude; // Math.Sqrt(Math.Pow(direction.x, 2) + Math.Pow(direction.y, 2));
        float b = orientation.magnitude; //Math.Sqrt(Math.Pow(orientation.x, 2) + Math.Pow(orientation.y, 2));

        float ab = Vector2.Dot(orientation, direction);

        float radian = Mathf.Acos(ab / (a * b));
        float angle = radian * Mathf.Rad2Deg; // / Mathf.Deg2Rad;

        return angle <= fanShapeAngle / 2;
    }

    public static Vector2 Deflection(Vector2 casterOri, float angle)
    {
        //var eAngle = MathUtil.VectorToAngle(Vector2.right, casterOri);
        //var resultAngle = eAngle + angle;

        //var degree = resultAngle * Mathf.Deg2Rad;
        //var castDirection = new Vector2(Mathf.Cos(degree), Mathf.Sin(degree)).normalized;
        //return castDirection;

        return Quaternion.AngleAxis(angle, Vector3.forward) * casterOri;
    }

    public static bool HasApproximatedOrientation(Vector2 v1, Vector2 v2)
    {
        var value = Vector2.Dot(v1.normalized, v2.normalized);
        return value < -.95f;
    }

    public static Vector2 GetPlaneNormal(Vector2 v, bool reverse = false)
    {
        return Quaternion.AngleAxis(reverse ? 90 : -90, Vector3.forward) * v;
    }

    public static Vector2 GetDeflectedVector2(Vector2 v, float radius, float deltaLength, bool flag)
    {

        float deltaAngle = (float)(deltaLength / (2 * Math.PI * radius) * 360);
        float angle;
        if (flag)
        {
            angle = v.y <= 0 ? deltaAngle : 360 - deltaAngle;
        }
        else
        {
            angle = v.y > 0 ? deltaAngle : 360 - deltaAngle;
        }
        return Quaternion.AngleAxis(angle, Vector3.forward) * v;
    }

    public const float RVO_EPSILON = 0.00001f;

    public static long Sqr(long scalar)
    {
        return scalar * scalar;
    }
    public static int Sqr(int scalar)
    {
        return scalar * scalar;
    }

    public static float Sqr(float scalar)
    {
        return UFloat.Round(scalar * scalar);
    }


    public static float Sqrt(float scalar)
    {
        return UFloat.Round(Mathf.Sqrt(scalar));
    }

    /// <summary>
    /// 两个 二维向量 的行列式
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static float Det(Vector2 vector1, Vector2 vector2)
    {
        return UFloat.Round(UFloat.Round(vector1.x * vector2.y) - UFloat.Round(vector1.y * vector2.x));
    }

    public static float LeftOf(Vector2 a, Vector2 b, Vector2 c)
    {
        return Det(a - c, b - a);
    }

    public static int LeftOf(UVector2 a, UVector2 b, UVector2 c)
    {
        return (a.x - c.x) * (b.y - a.y) - (a.y - c.y) * (b.x - a.x);
    }

    public static int Det(UVector2 vector1, UVector2 vector2)
    {
        return vector1.x * vector2.y - vector1.y * vector2.x;
    }

    public static float DistSqPointLineSegment(Vector2 vector1, Vector2 vector2, Vector2 vector3)
    {
        float r = Vector2.Dot(vector3 - vector1, vector2 - vector1) / (vector2 - vector1).sqrMagnitude;

        if (r < .0f)
        {
            return (vector3 - vector1).sqrMagnitude;
        }
        if (r > 1.0f)
        {
            return (vector2 - vector1).sqrMagnitude;
        }

        return (vector3 - (vector1 + r * (vector2 - vector1))).sqrMagnitude;
    }


    public static int DistSqPointLineSegment(UVector2 vector1, UVector2 vector2, UVector2 vector3)
    {
        //vector3 - vector1
        int v1x = vector3.x - vector1.x;
        int v1y = vector3.y - vector1.y;
        //vector2 - vector1
        int v2x = vector2.x - vector1.x;
        int v2y = vector2.y - vector1.y;
        //vector3 - vector2
        int v3x = vector3.x - vector2.x;
        int v3y = vector3.y - vector2.y;

        float r = (float)(v1x * v2x + v1y * v2y) / (v2x * v2x + v2y * v2y);

        if (r < .0f)
        {
            return v1x * v1x + v1y * v1y;
        }
        if (r > 1.0f)
        {
            return v3x * v3x + v3y * v3y;
        }
        int x = v1x - UFloat.RoundToInt(v2x * r);
        int y = v1y - UFloat.RoundToInt(v2y * r);
        return x * x + y * y;
    }

    public static bool PointInRectangle(Vector2 rectanbleCenter, float height, float width, Vector2 rectangleOrientation, Vector2 point, bool consideRadius)
    {
        Vector2 rectangleOrientationNor = rectangleOrientation.normalized;
        Vector2 verticalOrientationNor = (Quaternion.AngleAxis(90.0f, Vector3.right) * rectangleOrientation).normalized;

        Vector2 vertex1 = rectanbleCenter + (rectangleOrientationNor * height + verticalOrientationNor * width) * .5f;
        Vector2 vertex2 = rectanbleCenter + (rectangleOrientationNor * height - verticalOrientationNor * width) * .5f;
        Vector2 vertex3 = rectanbleCenter + (-rectangleOrientationNor * height - verticalOrientationNor * width) * .5f;
        Vector2 vertex4 = rectanbleCenter + (-rectangleOrientationNor * height + verticalOrientationNor * width) * .5f;

        float totalAngle = Vector2.Angle(vertex1 - point, vertex2 - point) + Vector2.Angle(vertex2 - point, vertex3 - point) +
         Vector2.Angle(vertex3 - point, vertex4 - point) + Vector2.Angle(vertex4 - point, vertex1 - point);

        return Mathf.Abs(totalAngle - 360.0f) <= .1f;
    }

    public static List<UVector2> BitConverterToVector2s(Byte[] dataBytes)
    {
        List<UVector2> data = new List<UVector2>();
        int vector2Size = sizeof(float) * 2;
        for (int i = 0; i < dataBytes.Length; i += vector2Size)
        {
            UVector2 pos = new UVector2(BitConverter.ToSingle(dataBytes, i), BitConverter.ToSingle(dataBytes, i + 4));
            data.Add(pos);
        }
        return data;
    }

    public static List<int> BitConverterToInt(Byte[] dataBytes)
    {
        List<int> data = new List<int>();
        int vector2Size = sizeof(int);
        for (int i = 0; i < dataBytes.Length; i += vector2Size)
        {
            int pos = BitConverter.ToInt32(dataBytes, i);
            data.Add(pos);
        }
        return data;
    }

    public static string ChangeRomanNumerals(int num)
    {
        string str = "";
        switch (num)
        {

            case 1:
                str = "I";
                break;
            case 2:
                str = "II";
                break;
            case 3:
                str = "III";
                break;
            case 4:
                str = "IV";
                break;
            case 5:
                str = "V";
                break;
            default:
                break;
        }
        return str;
    }


    public static System.DateTime TransToDateTime(int t)
    {
        System.DateTime dt = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long lTime = long.Parse(t.ToString() + "0000000");
        System.TimeSpan toNow = new System.TimeSpan(lTime);
        return dt.Add(toNow);
    }

    /// <summary>
    /// cross worldup orientation
    /// </summary>
    /// <param name="orientation"></param>
    /// <returns></returns>
    public static Vector2 GetRight(Vector2 orientation)
    {
        Vector3 right = Vector3.Cross(Vector3.up, new Vector3(orientation.x, 0, orientation.y));
        Vector2 curright = new Vector2(right.x, right.z);
        return curright;
    }


    /// <summary>
    /// 角度和半径计算本地坐标系旋转坐标
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static UVector2 GetLocalRotatePositon(int angle, int radius)
    {
        float a = UFloat.Round(Mathf.Deg2Rad * angle);
        int x = UFloat.RoundToInt(Mathf.Cos(a) * radius);
        int z = UFloat.RoundToInt(Mathf.Sin(a) * radius);
        return new UVector2(x, z);
    }
}