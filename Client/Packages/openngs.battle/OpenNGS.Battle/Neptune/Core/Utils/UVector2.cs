using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// UVector2
/// 2D integer vector
/// </summary>
public struct UVector2
{
    //public const float kEpsilon = 1E-05f;
    /// <summary>
    ///   <para>X component of the vector.</para>
    /// </summary>
    public int x;
    /// <summary>
    ///   <para>Y component of the vector.</para>
    /// </summary>
    public int y;

    private int deltaX;

    private int deltaY;

    const int FACTOR = 1000;
    const int HALF_FACTOR = 500;

    /// <summary>
    /// Gets or sets the value at the specified index in the UVector2. The index can be 0 or 1.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int this[int index]
    {
        get
        {
            if (index == 0)
            {
                return this.x;
            }
            if (index != 1)
            {
                throw new IndexOutOfRangeException("Invalid UVector2 index!");
            }
            return this.y;
        }
        set
        {
            if (index != 0)
            {
                if (index != 1)
                {
                    throw new IndexOutOfRangeException("Invalid UVector2 index!");
                }
                this.y = value;
            }
            else
            {
                this.x = value;
            }
        }
    }
    /// <summary>
    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
    /// </summary>
    public Vector2 normalized
    {
        get
        {
            Vector2 result = new UVector2(this.x, this.y);
            result.Normalize();
            UFloat.Round(ref result);
            return result;
        }
    }

    /// <summary>
    ///   <para>Returns this vector with a magnitude of 1000 (Read Only).</para>
    /// </summary>
    public UVector2 normalizedU
    {
        get
        {
            int length = this.magnitude;
            if (length == 0)
                return UVector2.zero;
            return new UVector2(this.x * FACTOR / length, this.y * FACTOR / length);
        }
    }

    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public int magnitude
    {
        get
        {
            return UFloat.RoundToInt(Mathf.Sqrt(this.x * this.x + this.y * this.y));
        }
    }
    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public int sqrMagnitude
    {
        get
        {
            return this.x * this.x + this.y * this.y;
        }
    }
    /// <summary>
    ///   <para>Shorthand for writing UVector2(0, 0).</para>
    /// </summary>
    public static readonly UVector2 zero = new UVector2(0, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector2(1, 1).</para>
    /// </summary>
    public static readonly UVector2 one = new UVector2(1, 1);

    /// <summary>
    ///   <para>Shorthand for writing UVector2(0, 1).</para>
    /// </summary>
    public static readonly UVector2 up=new UVector2(0, 1);
    /// <summary>
    ///   <para>Shorthand for writing UVector2(0, -1).</para>
    /// </summary>
    public static readonly UVector2 down= new UVector2(0, -1);
    /// <summary>
    ///   <para>Shorthand for writing UVector2(-1, 0).</para>
    /// </summary>
    public static readonly UVector2 left = new UVector2(-1, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector2(1, 0).</para>
    /// </summary>
    public static readonly UVector2 right= new UVector2(1, 0);
    /// <summary>
    ///   <para>Constructs a new vector with given x, y components.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public UVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.deltaX = 0;
        this.deltaY = 0;
    }

    private UVector2(int x, int y, int deltaX, int deltaY)
    {
        this.x = x;
        this.y = y;
        this.deltaX = deltaX;
        this.deltaY = deltaY;
    }

    public UVector2(float x, float y)
    {
        this.x = x >= 0 ? (int)Math.Floor(x) : (int)Math.Ceiling(x);
        this.y = y >= 0 ? (int)Math.Floor(y) : (int)Math.Ceiling(y);
        this.deltaX = UFloat.RoundToInt(x * FACTOR) - this.x * FACTOR;
        this.deltaY = UFloat.RoundToInt(y * FACTOR) - this.y * FACTOR;

        if (this.deltaX >= FACTOR - 1)
        {
            this.x++;
            this.deltaX = 0;
        }
        else if (this.deltaX <= -FACTOR + 1)
        {
            this.x--;
            this.deltaX = 0;
        }

        if (this.deltaY >= FACTOR - 1)
        {
            this.y++;
            this.deltaY = 0;
        }
        else if (this.deltaY < -FACTOR + 1)
        {
            this.y--;
            this.deltaY = 0;
        }
    }
    /// <summary>
    ///   <para>Set x and y components of an existing UVector2.</para>
    /// </summary>
    /// <param name="new_x"></param>
    /// <param name="new_y"></param>
    public void Set(int new_x, int new_y)
    {
        this.x = new_x;
        this.y = new_y;
        this.deltaX = 0;
        this.deltaY = 0;
    }
    /// <summary>
    ///   <para>Linearly interpolates between vectors a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static UVector2 Lerp(UVector2 a, UVector2 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new UVector2(a.x + UFloat.RoundToInt((b.x - a.x) * t), a.y + UFloat.RoundToInt((b.y - a.y) * t));
    }
    /// <summary>
    ///   <para>Linearly interpolates between vectors a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static UVector2 LerpUnclamped(UVector2 a, UVector2 b, float t)
    {
        return new UVector2(a.x + UFloat.RoundToInt((b.x - a.x) * t), a.y + UFloat.RoundToInt((b.y - a.y) * t));
    }
    ///// <summary>
    /////   <para>Moves a point current towards target.</para>
    ///// </summary>
    ///// <param name="current"></param>
    ///// <param name="target"></param>
    ///// <param name="maxDistanceDelta"></param>
    //public static UVector2 MoveTowards(UVector2 current, UVector2 target, int maxDistanceDelta)
    //{
    //    UVector2 a = target - current;
    //    int magnitude = a.magnitude;
    //    if (magnitude <= maxDistanceDelta || magnitude == 0)
    //    {
    //        return target;
    //    }
    //    return current + a / magnitude * maxDistanceDelta;
    //}
    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static UVector2 Scale(UVector2 a, UVector2 b)
    {
        return new UVector2(a.x * b.x, a.y * b.y);
    }
    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(UVector2 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
    }

    public void Round()
    {
        if (this.deltaX > HALF_FACTOR)
            this.x++;
        else if (this.deltaX < -HALF_FACTOR)
            this.x--;

        if (this.deltaY > HALF_FACTOR)
            this.y++;
        else if (this.deltaY < -HALF_FACTOR)
            this.y--;

        this.deltaX = 0;
        this.deltaY = 0;
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public override string ToString()
    {
        return string.Format("({0}.{2}, {1}.{3})", this.x, this.y, this.deltaX, this.deltaY);
    }
    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public string ToString(string format)
    {
        return string.Format("({0}.{2}, {1}.{3})", this.x.ToString(format), this.y.ToString(format), this.deltaX.ToString(format), this.deltaY.ToString(format));
    }
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }
    public override bool Equals(object other)
    {
        if (!(other is UVector2))
        {
            return false;
        }
        UVector2 vector = (UVector2)other;
        return this.x.Equals(vector.x) && this.y.Equals(vector.y);
    }
    /// <summary>
    ///   <para>Reflects a vector off the vector defined by a normal.</para>
    /// </summary>
    /// <param name="inDirection"></param>
    /// <param name="inNormal"></param>
    public static UVector2 Reflect(UVector2 inDirection, UVector2 inNormal)
    {
        return -2 * UVector2.Dot(inNormal, inDirection) * inNormal + inDirection;
    }
    /// <summary>
    ///   <para>Dot Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static int Dot(UVector2 lhs, UVector2 rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y;
    }
    /// <summary>
    ///   <para>Returns the angle in degrees between from and to.</para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static int Angle(UVector2 from, UVector2 to)
    {
        return UFloat.RoundToInt(Mathf.Acos(Mathf.Clamp(Vector2.Dot(from.normalized, to.normalized), -1, 1)) * 57.29578f);
    }
    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static int Distance(UVector2 a, UVector2 b)
    {
        return (a - b).magnitude;
    }
    /// <summary>
    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="maxLength"></param>
    public static UVector2 ClampMagnitude(UVector2 vector, int maxLength)
    {
        if (vector.sqrMagnitude > maxLength * maxLength)
        {
            return vector.normalized * maxLength;
        }
        return vector;
    }
    public static int SqrMagnitude(UVector2 a)
    {
        return a.x * a.x + a.y * a.y;
    }
    public int SqrMagnitude()
    {
        return this.x * this.x + this.y * this.y;
    }
    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static UVector2 Min(UVector2 lhs, UVector2 rhs)
    {
        return new UVector2(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
    }
    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static UVector2 Max(UVector2 lhs, UVector2 rhs)
    {
        return new UVector2(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
    }

    //public static UVector2 SmoothDamp(UVector2 current, UVector2 target, ref UVector2 currentVelocity, float smoothTime, int maxSpeed)
    //{
    //    float deltaTime = Time.deltaTime;
    //    return UVector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    //}

    //public static UVector2 SmoothDamp(UVector2 current, UVector2 target, ref UVector2 currentVelocity, float smoothTime)
    //{
    //    float deltaTime = Time.deltaTime;
    //    int maxSpeed = int.MaxValue;
    //    return UVector2.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    //}
    //public static UVector2 SmoothDamp(UVector2 current, UVector2 target, ref UVector2 currentVelocity, float smoothTime, int maxSpeed,  float deltaTime)
    //{
    //    smoothTime = Mathf.Max(0.0001f, smoothTime);
    //    float num = 2f / smoothTime;
    //    float num2 = num * deltaTime;
    //    float d = 1 / (1 + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
    //    UVector2 vector = current - target;
    //    UVector2 vector2 = target;
    //    int maxLength = (int)(maxSpeed * smoothTime);
    //    vector = UVector2.ClampMagnitude(vector, maxLength);
    //    target = current - vector;
    //    UVector2 vector3 = (currentVelocity + num * vector) * deltaTime;
    //    currentVelocity = (currentVelocity - num * vector3) * d;
    //    UVector2 vector4 = target + (vector + vector3) * d;
    //    if (UVector2.Dot(vector2 - current, vector4 - vector2) > 0)
    //    {
    //        vector4 = vector2;
    //        currentVelocity = (vector4 - vector2) / deltaTime;
    //    }
    //    return vector4;
    //}
    public static UVector2 operator +(UVector2 a, UVector2 b)
    {
        UVector2 result = new UVector2(a.x + b.x, a.y + b.y);
        result.deltaX = a.deltaX + b.deltaX;
        result.deltaY = a.deltaY + b.deltaY;
        int mx = result.deltaX / FACTOR;
        if(mx!=0)
        {
            result.x += mx;
            result.deltaX -= mx * FACTOR;
        }
        int my = result.deltaY / FACTOR;
        if (my != 0)
        {
            result.y += my;
            result.deltaY -= my * FACTOR;
        }
        return result;
    }
    public static UVector2 operator -(UVector2 a, UVector2 b)
    {
        return new UVector2(a.x - b.x, a.y - b.y);
    }
    public static UVector2 operator -(UVector2 a)
    {
        return new UVector2(-a.x, -a.y);
    }
    public static UVector2 operator *(UVector2 a, int d)
    {
        return new UVector2(a.x * d, a.y * d);
    }
    public static UVector2 operator *(int d, UVector2 a)
    {
        return new UVector2(a.x * d, a.y * d);
    }
    public static UVector2 operator /(UVector2 a, int d)
    {
        return new UVector2(a.x / d, a.y / d);
    }

    public static UVector2 operator *(UVector2 a, float d)
    {
        return new UVector2(UFloat.Round(a.x + (float)a.deltaX / FACTOR) * d, UFloat.Round(a.y + (float)a.deltaY / FACTOR) * d);
    }
    public static UVector2 operator /(UVector2 a, float d)
    {
        return new UVector2(UFloat.RoundToInt(a.x / d), UFloat.RoundToInt(a.y / d));
    }

    public static bool operator ==(UVector2 lhs, UVector2 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }
    public static bool operator !=(UVector2 lhs, UVector2 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }
    public static UVector2 operator +(UVector2 a, Vector2 b)
    {
        return new UVector2(a.x + UFloat.RoundToInt(b.x), a.y + UFloat.RoundToInt(b.y));
    }
    public static UVector2 operator -(UVector2 a, Vector2 b)
    {
        return new UVector2(a.x - UFloat.RoundToInt(b.x), a.y - UFloat.RoundToInt(b.y));
    }

    public static implicit operator UVector2(Vector3 v)
    {
        return new UVector2(v.x, v.y);
    }
    public static implicit operator Vector3(UVector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
    public static implicit operator Vector2(UVector2 v)
    {
        return new Vector2(v.x, v.y);
    }
    public static implicit operator UVector2(Vector2 v)
    {
        return new UVector2(v.x, v.y);
    }
    public static implicit operator UVector3(UVector2 v)
    {
        return new UVector3(v.x, v.y, 0);
    }
}