using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Crypto
{
    internal struct SValue
    {
#if UNITY_STANDALONE
        private long va;
        private long vb;
#else
        private double v;
#endif
        unsafe private double GetValue()
        {
#if UNITY_STANDALONE
            double ret = 0;
            fixed (long* pa = &va)
            {
                long* pRet = (long*)&ret;
                *pRet = *pa ^ vb;
            }
            return ret;
#else
            return v;
#endif
        }

        unsafe private void SetValue(double val)
        {
#if UNITY_STANDALONE
            fixed (long* pa = &va)
            {
                double* pvf = &val;
                long* pv = (long*)pvf;
                vb = long.MaxValue - SRand.GetValue();
                *pa = *pv ^ vb;
            }
#else
            v = val;
#endif
        }

        public static implicit operator SValue(float value)
        {
            var v = new SValue();
            v.SetValue(value);
            return v;
        }

        public static implicit operator float(SValue value)
        {
            return (float)value.GetValue();
        }

        public static implicit operator SValue(double value)
        {
            var v = new SValue();
            v.SetValue(value);
            return v;
        }

        public static implicit operator double(SValue value)
        {
            return value.GetValue();
        }
    }


    internal static class SRand
    {
        static Random random = new Random();

        const int MAX = int.MaxValue;
        const int MIN = -int.MaxValue;
        internal static int GetValue()
        {
            return random.Next(MIN, MAX);
        }
    }
}
