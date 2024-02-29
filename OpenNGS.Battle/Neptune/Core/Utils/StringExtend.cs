using UnityEngine;


public static class StringExtend
{
    public static bool EndsWithFast(this string a, string value)
    {
        int ap = a.Length - 1;
        int bp = value.Length - 1;

        while (ap >= 0 && bp >= 0 && a[ap] == value[bp])
        {
            ap--;
            bp--;
        }
        return (bp < 0 && a.Length >= value.Length) ||

                (ap < 0 && value.Length >= a.Length);
    }

    public static bool StartsWithFast(this string a, string value)
    {
        int aLen = a.Length;
        int bLen = value.Length;
        int ap = 0; int bp = 0;

        while (ap < aLen && bp < bLen && a[ap] == value[bp])
        {
            ap++;
            bp++;
        }

        return (bp == bLen && aLen >= bLen) ||

                (ap == aLen && bLen >= aLen);
    }

}


public static class VectorExtend
{
    public static string ToString(this Vector2 value, int precision)
    {
        if (precision > 0)
            return string.Format("({0:f" + precision + "},{1:f" + precision + "})", value.x, value.y);
        else
            return value.ToString();
    }
}