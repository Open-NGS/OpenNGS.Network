using OpenNGS.Numerical;
using System;
using System.Text;

namespace OpenNGS.Core
{
    public partial class NGSAttributes : global::ProtoBuf.IExtensible, INumerable
    {
        protected int Divisor = 10000;

        public void Set(long type, long value)
        {
            this.Values[type] = value;
        }

        public long Get(long type)
        {
            long v = 0;
            this.Values.TryGetValue(type, out v);
            return v;
        }

        public void Reset()
        {
            this.Values.Clear();
        }

        public void Add(long type, long value)
        {
            long v = 0;
            this.Values.TryGetValue(type, out v);
            this.Values[type] = v + value;
        }

        public void Add(INumerable b)
        {
            foreach (var k in ((NGSAttributes)b).Values)
            {
                this.Set(k.Key, this.Get(k.Key) + k.Value);
            }
        }

        public void Multiply(INumerable b)
        {

        }
    }

    public class NGSAttributes<T> where T : struct
    {
        NGSAttributes Attributes;

        public NGSAttributes(NGSAttributes attributes)
        {
            this.Attributes = attributes;
        }

        public void Set(T type, long value)
        {
            this.Attributes.Set(Convert.ToInt32(type), value);
        }

        public long Get(T type)
        {
            return this.Attributes.Get(Convert.ToInt32(type));
        }


    }
}
