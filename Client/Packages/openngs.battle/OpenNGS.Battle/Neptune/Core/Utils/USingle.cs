using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public struct USingle
{
    public int i;
    public int f;
    const int FACTOR = 1000;

    public USingle(float val)
    {
        this.i = val >= 0 ? (int)Math.Floor(val) : (int)Math.Ceiling(val);
        this.f = UFloat.RoundToInt(val * FACTOR) - this.i * FACTOR;
    }

    private USingle(int i, int f)
    {
        this.i = i;
        this.f = f;
    }

    public static USingle operator *(USingle a, USingle b)
    {
        int i = a.i * b.i;
        int f = a.f * b.i + a.i * b.f + a.f * b.f / FACTOR;
        int mx = f / FACTOR;
        if (mx != 0)
        {
            i += mx;
            f -= mx * FACTOR;
        }

        return new USingle(i, f);
    }


    public override string ToString()
    {
        return string.Format("{0}.{1}", this.i, Math.Abs(this.f).ToString().PadLeft(3, '0'));
    }
}