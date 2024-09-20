using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// UVector3
/// 3D integer vector
/// </summary>
public struct UVector3
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
    /// <summary>
    ///   <para>Z component of the vector.</para>
    /// </summary>
    public int z;

    private int deltaX;

    private int deltaY;

    private int deltaZ;

    const int FACTOR = 1000;
    const int HALF_FACTOR = 500;

    /// <summary>
    /// Gets or sets the value at the specified index in the UVector3. The index can be 0, 1 or 2.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.x;
                case 1:
                    return this.y;
                case 2:
                    return this.z;
                default:
                    throw new IndexOutOfRangeException("Invalid UVector3 index!");
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                case 2:
                    this.z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid UVector3 index!");
            }
        }
    }
    /// <summary>
    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
    /// </summary>
    public Vector3 normalized
    {
        get
        {
            Vector3 result = new UVector3(this.x, this.y,this.z);
            result.Normalize();
            UFloat.Round(ref result);
            return result;
        }
    }
    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public float magnitude
    {
        get
        {
            return Mathf.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }
    }
    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public int sqrMagnitude
    {
        get
        {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }
    }
    /// <summary>
    ///   <para>Shorthand for writing UVector3(0, 0, 0).</para>
    /// </summary>
    public static readonly UVector3 zero = new UVector3(0, 0, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(1, 1, 1).</para>
    /// </summary>
    public static readonly UVector3 one = new UVector3(1, 1, 1);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(0, 0, 1).</para>
    /// </summary>
    public static readonly UVector3 forward = new UVector3(0, 0, 1);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(0, 0, -1).</para>
    /// </summary>
    public static readonly UVector3 back = new UVector3(0, 0, -1);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(0, 1, 0).</para>
    /// </summary>
    public static readonly UVector3 up = new UVector3(0, 1, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(0, -1, 0).</para>
    /// </summary>
    public static readonly UVector3 down = new UVector3(0, -1, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(-1, 0, 0).</para>
    /// </summary>
    public static UVector3 left = new UVector3(-1, 0, 0);
    /// <summary>
    ///   <para>Shorthand for writing UVector3(1, 0, 0).</para>
    /// </summary>
    public static UVector3 right = new UVector3(1, 0, 0);

    /// <summary>
    ///   <para>Creates a new vector with given x, y, z components.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public UVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.deltaX = 0;
        this.deltaY = 0;
        this.deltaZ = 0;
    }

    public UVector3(float x, float y, float z)
    {
        this.x = (int)Math.Floor(x);
        this.y = (int)Math.Floor(y);
        this.z = (int)Math.Floor(z);
        this.deltaX = (int)((x * FACTOR - this.x * FACTOR));
        this.deltaY = (int)((y * FACTOR - this.y * FACTOR));
        this.deltaZ = (int)((z * FACTOR - this.z * FACTOR));
        if (this.deltaX >= HALF_FACTOR)
        {
            this.x++;
            this.deltaX -= FACTOR;
        }
        if (this.deltaY >= HALF_FACTOR)
        {
            this.y++;
            this.deltaY -= FACTOR;
        }
        if (this.deltaZ >= HALF_FACTOR)
        {
            this.z++;
            this.deltaZ -= FACTOR;
        }
    }
    /// <summary>
    ///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public UVector3(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
        this.deltaX = 0;
        this.deltaY = 0;
        this.deltaZ = 0;
    }

    public UVector3(UVector2 v, int z)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = z;
        this.deltaX = 0;
        this.deltaY = 0;
        this.deltaZ = 0;
    }

    ///// <summary>
    /////   <para>Linearly interpolates between two vectors.</para>
    ///// </summary>
    ///// <param name="a"></param>
    ///// <param name="b"></param>
    ///// <param name="t"></param>
    //public static UVector3 Lerp(UVector3 a, UVector3 b, float t)
    //{
    //    t = Mathf.Clamp01(t);
    //    return new UVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    //}
    ///// <summary>
    /////   <para>Linearly interpolates between two vectors.</para>
    ///// </summary>
    ///// <param name="a"></param>
    ///// <param name="b"></param>
    ///// <param name="t"></param>
    //public static UVector3 LerpUnclamped(UVector3 a, UVector3 b, float t)
    //{
    //    return new UVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    //}

    public static UVector3 Slerp(UVector3 a, UVector3 b, float t)
    {
        return Vector3.Slerp(a, b, t);
    }

    /// <summary>
    ///   <para>Spherically interpolates between two vectors.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static UVector3 SlerpUnclamped(UVector3 a, UVector3 b, float t)
    {
        return Vector3.SlerpUnclamped(a, b, t);
    }

    //public static void OrthoNormalize(ref UVector3 normal, ref UVector3 tangent)
    //{
    //    Vector3.OrthoNormalize(ref normal, ref tangent);
    //}
    //public static void OrthoNormalize(ref UVector3 normal, ref UVector3 tangent, ref UVector3 binormal)
    //{
    //    Vector3.OrthoNormalize(ref normal, ref tangent, ref binormal);
    //}
    /// <summary>
    ///   <para>Moves a point current in a straight line towards a target point.</para>
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="maxDistanceDelta"></param>
    public static UVector3 MoveTowards(UVector3 current, UVector3 target, float maxDistanceDelta)
    {
        Vector3 a = target - current;
        float magnitude = a.magnitude;
        if (magnitude <= maxDistanceDelta || magnitude == 0)
        {
            return target;
        }
        return (Vector3)current + a / magnitude * maxDistanceDelta;
    }
    /// <summary>
    ///   <para>Rotates a vector current towards target.</para>
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="maxRadiansDelta"></param>
    /// <param name="maxMagnitudeDelta"></param>
    public static UVector3 RotateTowards(UVector3 current, UVector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
    {
        return Vector3.RotateTowards(current, target, maxRadiansDelta, maxMagnitudeDelta);
    }

    public static UVector3 SmoothDamp(UVector3 current, UVector3 target, ref UVector3 currentVelocity, float smoothTime, float maxSpeed)
    {
        float deltaTime = Time.deltaTime;
        Vector3 velocity = currentVelocity;
        Vector3 result = Vector3.SmoothDamp(current, target, ref velocity, smoothTime, maxSpeed, deltaTime);
        currentVelocity = velocity;
        return result;
    }

    //public static UVector3 SmoothDamp(UVector3 current, UVector3 target, ref UVector3 currentVelocity, float smoothTime)
    //{
    //    float deltaTime = Time.deltaTime;
    //    float maxSpeed = float.PositiveInfinity;
    //    return UVector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    //}
    //public static UVector3 SmoothDamp(UVector3 current, UVector3 target, ref UVector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    //{
    //    smoothTime = Mathf.Max(0.0001f, smoothTime);
    //    float num = 2f / smoothTime;
    //    float num2 = num * deltaTime;
    //    float d = 1 / (1 + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
    //    Vector3 vector = current - target;
    //    Vector3 vector2 = target;
    //    float maxLength = maxSpeed * smoothTime;
    //    vector = UVector3.ClampMagnitude(vector, maxLength);
    //    target = current - vector;
    //    Vector3 vector3 = ((Vector3)currentVelocity + num * vector) * deltaTime;
    //    currentVelocity = (currentVelocity - num * vector3) * d;
    //    UVector3 vector4 = target + (vector + vector3) * d;
    //    if (UVector3.Dot(vector2 - current, vector4 - vector2) > 0)
    //    {
    //        vector4 = vector2;
    //        currentVelocity = (vector4 - vector2) / deltaTime;
    //    }
    //    return vector4;
    //}
    /// <summary>
    ///   <para>Set x, y and z components of an existing UVector3.</para>
    /// </summary>
    /// <param name="new_x"></param>
    /// <param name="new_y"></param>
    /// <param name="new_z"></param>
    public void Set(int new_x, int new_y, int new_z)
    {
        this.x = new_x;
        this.y = new_y;
        this.z = new_z;
    }
    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static UVector3 Scale(UVector3 a, UVector3 b)
    {
        return new UVector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(UVector3 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
        this.z *= scale.z;
    }
    /// <summary>
    ///   <para>Cross Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static UVector3 Cross(UVector3 lhs, UVector3 rhs)
    {
        return new UVector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
    }
    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
    }
    public override bool Equals(object other)
    {
        if (!(other is UVector3))
        {
            return false;
        }
        UVector3 vector = (UVector3)other;
        return this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z);
    }
    /// <summary>
    ///   <para>Reflects a vector off the plane defined by a normal.</para>
    /// </summary>
    /// <param name="inDirection"></param>
    /// <param name="inNormal"></param>
    public static UVector3 Reflect(UVector3 inDirection, UVector3 inNormal)
    {
        return -2 * UVector3.Dot(inNormal, inDirection) * inNormal + inDirection;
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", new object[]
        {
                this.x,
                this.y,
                this.z
        });
    }
    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public string ToString(string format)
    {
        return string.Format("({0}, {1}, {2})", new object[]
        {
                this.x.ToString(format),
                this.y.ToString(format),
                this.z.ToString(format)
        });
    }
    /// <summary>
    ///   <para>Dot Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static int Dot(UVector3 lhs, UVector3 rhs)
    {
        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
    }
    /// <summary>
    ///   <para>Projects a vector onto another vector.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="onNormal"></param>
    public static UVector3 Project(UVector3 vector, UVector3 onNormal)
    {
        int num = UVector3.Dot(onNormal, onNormal);
        if (num == 0)
        {
            return UVector3.zero;
        }
        return onNormal * UVector3.Dot(vector, onNormal) / num;
    }
    /// <summary>
    ///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="planeNormal"></param>
    public static UVector3 ProjectOnPlane(UVector3 vector, UVector3 planeNormal)
    {
        return vector - UVector3.Project(vector, planeNormal);
    }
    [Obsolete("Use UVector3.ProjectOnPlane instead.")]
    public static UVector3 Exclude(UVector3 excludeThis, UVector3 fromThat)
    {
        return fromThat - UVector3.Project(fromThat, excludeThis);
    }
    /// <summary>
    ///   <para>Returns the angle in degrees between from and to.</para>
    /// </summary>
    /// <param name="from">The angle extends round from this vector.</param>
    /// <param name="to">The angle extends round to this vector.</param>
    public static float Angle(UVector3 from, UVector3 to)
    {
        return Mathf.Acos(Mathf.Clamp(UVector3.Dot(from.normalized, to.normalized), -1, 1)) * 57.29578f;
    }
    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float Distance(UVector3 a, UVector3 b)
    {
        UVector3 vector = new UVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }
    /// <summary>
    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="maxLength"></param>
    public static UVector3 ClampMagnitude(UVector3 vector, float maxLength)
    {
        if (vector.sqrMagnitude > maxLength * maxLength)
        {
            return (Vector3)vector.normalized * maxLength;
        }
        return vector;
    }
    public static float Magnitude(UVector3 a)
    {
        return Mathf.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
    }
    public static float SqrMagnitude(UVector3 a)
    {
        return a.x * a.x + a.y * a.y + a.z * a.z;
    }
    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static UVector3 Min(UVector3 lhs, UVector3 rhs)
    {
        return new UVector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
    }
    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static UVector3 Max(UVector3 lhs, UVector3 rhs)
    {
        return new UVector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
    }
    [Obsolete("Use UVector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
    public static float AngleBetween(UVector3 from, UVector3 to)
    {
        return Mathf.Acos(Mathf.Clamp(UVector3.Dot(from.normalized, to.normalized), -1, 1));
    }
    public static UVector3 operator +(UVector3 a, UVector3 b)
    {
        return new UVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static UVector3 operator -(UVector3 a, UVector3 b)
    {
        return new UVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static UVector3 operator -(UVector3 a)
    {
        return new UVector3(-a.x, -a.y, -a.z);
    }
    public static UVector3 operator *(UVector3 a, int d)
    {
        return new UVector3(a.x * d, a.y * d, a.z * d);
    }
    public static UVector3 operator *(int d, UVector3 a)
    {
        return new UVector3(a.x * d, a.y * d, a.z * d);
    }
    public static UVector3 operator /(UVector3 a, int d)
    {
        return new UVector3(a.x / d, a.y / d, a.z / d);
    }
    public static bool operator ==(UVector3 lhs, UVector3 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }
    public static bool operator !=(UVector3 lhs, UVector3 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
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

        if (this.deltaZ > HALF_FACTOR)
            this.z++;
        else if (this.deltaZ < -HALF_FACTOR)
            this.z--;

        this.deltaX = 0;
        this.deltaY = 0;
        this.deltaZ = 0;
    }



    public static implicit operator Vector3(UVector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator UVector3(Vector3 v)
    {
        return new UVector3(UFloat.RoundToInt(v.x), UFloat.RoundToInt(v.y), UFloat.RoundToInt(v.z));
    }

    public static implicit operator UVector2(UVector3 v)
    {
        return new UVector2(v.x, v.y);
    }
}