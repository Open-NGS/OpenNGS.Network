using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

/// <summary>
/// UFloat
/// </summary>
[ComVisible(true)]
public struct UFloat
{
    public static int count;
    public static int round_count;

    const float L2 = 100;
    const float L3 = 1000;
//    public static readonly UFloat one = 1f;
//    public static readonly UFloat zero = 0f;
//    float value;

//    private UFloat(float internalValue):this(internalValue,true)
//    {
//    }
//
//    private UFloat(float internalValue, bool round)
//    {
//        count++;
//        if (!round)
//            value = internalValue;
//        else
//        {
//            if (double.IsNaN(internalValue) ||
//                double.IsInfinity(internalValue) ||
//                internalValue.Equals(float.MaxValue) ||
//                internalValue.Equals(float.MinValue) ||
//                internalValue.Equals(1) ||
//                internalValue.Equals(-1) ||
//                internalValue.Equals(0)
//                )
//                value = internalValue;
//            else
//            {
//                value = Round(internalValue);
//                //value = Floor(internalValue);
//            }
//        }
//    }

    public static float Round(float value)
    {
        round_count++;
        if (value < 1000 && value > -1000)
            return (float)(RoundInternal(value * L3) / L3);
        else if (value < 10000 && value > -10000)
            return (float)(RoundInternal(value * L2) / L2);
        return (float)RoundInternal(value);
    }

    public static float RoundF(float value)
    {
        round_count++;
        if (value < 1000 && value > -1000)
            return Mathf.Round(value * L3) / L3;
        else if (value < 10000 && value > -10000)
            return Mathf.Round(value * L2) / L2;
        return Mathf.Round(value);
    }

    public static void Round(ref Vector2 value)
    {
        value.x = Round(value.x);
        value.y = Round(value.y);
    }

    public static void Round(ref Vector3 value)
    {
        value.x = Round(value.x);
        value.y = Round(value.y);
        value.z = Round(value.z);
    }

    public static unsafe float Floor(float f)
    {
        uint* vp = (uint*)(&f);
        *vp = (*vp) & 0xFFFFFFF0;
        return f;
    }

    private static float RoundInternal(float x)
    {
        float int_part, dec_part;
        int_part = (float)Math.Floor(x);
        dec_part = x - int_part;
        if (dec_part >= 0.5)
        {
            int_part++;
        }
        return int_part;
    }

    public static int RoundToInt(float x)
    {
        return (int)RoundInternal(x);
    }

//    public TypeCode GetTypeCode()
//    {
//        return TypeCode.Single;
//    }
//
//    public bool ToBoolean(IFormatProvider provider)
//    {
//        return value != 0;
//    }
//
//    public byte ToByte(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public char ToChar(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public DateTime ToDateTime(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public decimal ToDecimal(IFormatProvider provider)
//    {
//        return (decimal)value;
//    }
//
//    public double ToDouble(IFormatProvider provider)
//    {
//        return value;
//    }
//
//    public short ToInt16(IFormatProvider provider)
//    {
//        return (short)(value);
//    }
//
//    public int ToInt32(IFormatProvider provider)
//    {
//        return (int)value;
//    }
//
//    public long ToInt64(IFormatProvider provider)
//    {
//        return (long)value;
//    }
//
//    public sbyte ToSByte(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public float ToSingle(IFormatProvider provider)
//    {
//        return value;
//    }
//
//    public string ToString(IFormatProvider provider)
//    {
//        return value.ToString(provider);
//    }
//
//    public object ToType(Type conversionType, IFormatProvider provider)
//    {
//        UnityEngine.Debug.LogFormat("ToType({0})", conversionType);
//        switch (Type.GetTypeCode(conversionType))
//        {
//            case TypeCode.Object:
//                if (conversionType.IsAssignableFrom(typeof(UFloat)))
//                    return new UFloat();
//                else
//                    throw new InvalidCastException(String.Format("Conversion to a {0} is not supported.", conversionType.Name));
//            case TypeCode.Int32:
//                return ToInt32(null);
//            case TypeCode.Decimal:
//                return ToDecimal(null);
//            case TypeCode.DateTime:
//                return ToDateTime(null);
//            case TypeCode.String:
//                return ToString(null);
//            default:
//                throw new InvalidCastException(String.Format("Conversion to {0} is not supported.", conversionType.Name));
//        }
//    }
//
//    public ushort ToUInt16(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public uint ToUInt32(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public ulong ToUInt64(IFormatProvider provider)
//    {
//        throw new NotImplementedException();
//    }
//
//    public string ToString(string format, IFormatProvider formatProvider)
//    {
//        return value.ToString(format, formatProvider);
//    }

//    public int CompareTo(UFloat other)
//    {
//        if (double.IsPositiveInfinity(this.value) && double.IsPositiveInfinity(other.value))
//        {
//            return 0;
//        }
//        if (double.IsNegativeInfinity(this.value) && double.IsNegativeInfinity(other.value))
//        {
//            return 0;
//        }
//        if (double.IsNaN(other.value))
//        {
//            if (double.IsNaN(this.value))
//            {
//                return 0;
//            }
//            return 1;
//        }
//        else if (double.IsNaN(this.value))
//        {
//            if (double.IsNaN(other.value))
//            {
//                return 0;
//            }
//            return -1;
//        }
//        else
//        {
//            if (this.value.Equals(float.MaxValue) && other.value.Equals(float.MaxValue))
//            {
//                return 0;
//            }
//            if (this.value.Equals(float.MaxValue))
//            {
//                return 1;
//            }
//            if (other.value.Equals(float.MaxValue))
//            {
//                return -1;
//            }
//            if (this.value.Equals(float.MinValue) && other.value.Equals(float.MinValue))
//            {
//                return 0;
//            }
//            if (this.value.Equals(float.MinValue))
//            {
//                return -1;
//            }
//            if (other.value.Equals(float.MinValue))
//            {
//                return 1;
//            }
//
//
//            if (this.value == other.value)
//            {
//                return 0;
//            }
//            if (this.value > other.value)
//            {
//                return 1;
//            }
//            //int thisV = (int)(this.value * L);
//            //int otherV = (int)(other.value * L);
//            //if (thisV == otherV)
//            //{
//            //    return 0;
//            //}
//            //if (thisV > otherV)
//            //{
//            //    return 1;
//            //}
//            return -1;
//        }
//    }
//
//    public int CompareTo(object value)
//    {
//        if (value == null)
//        {
//            return 1;
//        }
//        if (!(value is UFloat))
//        {
//            throw new ArgumentException("Value is not a System.Single.");
//        }
//        UFloat num = (UFloat)value;
//        return this.CompareTo(num);
//    }

    // UFloat & UFloat +-*/
//    public static UFloat operator +(UFloat d1, UFloat d2)
//    {
//        return new UFloat(d1.value + d2.value);
//    }
//    public static UFloat operator -(UFloat d1, UFloat d2)
//    {
//        return new UFloat(d1.value - d2.value);
//    }
//    public static UFloat operator *(UFloat d1, UFloat d2)
//    {
//        return new UFloat(d1.value * d2.value);
//    }
//    public static UFloat operator /(UFloat d1, UFloat d2)
//    {
//        return new UFloat(d1.value / d2.value);
//    }

    // UFloat & float +-*/
//    public static UFloat operator +(UFloat d1, float d2)
//    {
//        return new UFloat(d1.value + d2);
//    }
//    public static UFloat operator -(UFloat d1, float d2)
//    {
//        return new UFloat(d1.value - d2);
//    }
//    public static UFloat operator *(UFloat d1, float d2)
//    {
//        return new UFloat(d1.value * d2);
//    }
//    public static UFloat operator /(UFloat d1, float d2)
//    {
//        return new UFloat(d1.value / d2);
//    }
//    // UFloat & int +-*/
//    public static UFloat operator +(UFloat d1, int d2)
//    {
//        return new UFloat(d1.value + d2);
//    }
//    public static UFloat operator -(UFloat d1, int d2)
//    {
//        return new UFloat(d1.value - d2);
//    }
//    public static UFloat operator *(UFloat d1, int d2)
//    {
//        return new UFloat(d1.value * d2);
//    }
//    public static UFloat operator /(UFloat d1, int d2)
//    {
//        return new UFloat(d1.value / d2);
//    }

    // Vector2 & UFloat * /
//    public static Vector2 operator *(Vector2 d1, UFloat d2)
//    {
//        return new Vector2(Round(d1.x * d2.value), Round(d1.y * d2.value));
//    }
//    public static Vector2 operator /(Vector2 d1, UFloat d2)
//    {
//        return new Vector2(Round(d1.x / d2.value), Round(d1.y / d2.value));
//    }
//    // UFloat & Vector2 * /
//    public static Vector2 operator *(UFloat d1, Vector2 d2)
//    {
//        return new Vector2(Round(d1.value * d2.x), Round(d1.value * d2.y));
//    }
//    public static Vector2 operator /(UFloat d1, Vector2 d2)
//    {
//        return new Vector2(Round(d1.value / d2.x), Round(d1.value / d2.y));
//    }
//
//    public static Vector3 operator *(Vector3 d1, UFloat d2)
//    {
//        return new Vector3(Round(d1.x * d2.value), Round(d1.y * d2.value), Round(d1.z * d2.value));
//    }
//    public static Vector3 operator /(Vector3 d1, UFloat d2)
//    {
//        return new Vector3(Round(d1.x / d2.value), Round(d1.y / d2.value), Round(d1.z / d2.value));
//    }
//
//    public static Vector3 operator *(UFloat d1, Vector3 d2)
//    {
//        return new Vector3(Round(d1.value * d2.x), Round(d1.value * d2.y), Round(d1.value * d2.z));
//    }
//    public static Vector3 operator /(UFloat d1, Vector3 d2)
//    {
//        return new Vector3(Round(d1.value / d2.x), Round(d1.value / d2.y), Round(d1.value / d2.z));
//    }
//
//    public static UVector2 operator *(UVector2 a, UFloat d)
//    {
//        return new UVector2(a.x * d.value, a.y * d.value);
//    }
//    public static UVector2 operator *(UFloat d, UVector2 a)
//    {
//        return new UVector2(a.x * d.value, a.y * d.value);
//    }
//    public static UVector2 operator /(UVector2 a, UFloat d)
//    {
//        return new UVector2(a.x / d.value, a.y / d.value);
//    }

//    public static implicit operator UFloat(int v)
//    {
//        return new UFloat(v);
//    }
//
//    public static explicit operator UFloat(float v)
//    {
//        return new UFloat(v);
//    }
//
//    public static implicit operator UFloat(double v)
//    {
//        return new UFloat((float)v);
//    }
//    //public static implicit operator int(UFloat v)
//    //{
//    //    return v.ToInt32(null);
//    //}
//    public static implicit operator float(UFloat v)
//    {
//        return v.value;
//    }
//
//    public static implicit operator double(UFloat v)
//    {
//        return v.ToDouble(null);
//    }

//    public override string ToString()
//    {
//        return this.ToString("F6", null);
//    }
}

