using OpenNGS.Numerical;
using System;
using System.Text;

namespace OpenNGS.Core
{
    public partial class AttributesInt32 : global::ProtoBuf.IExtensible, INumerable
    {
        public void Set(long type, int value)
        {
            this.Values[type] = value;
        }

        public int Get(long type)
        {
            int v = 0;
            this.Values.TryGetValue(type, out v);
            return v;
        }

        public void Reset()
        {
            this.Values.Clear();
        }

        public void Add(long type, int value)
        {
            int v = 0;
            this.Values.TryGetValue(type, out v);
            this.Values[type] = v + value;
        }

        public void Add(INumerable b)
        {
            foreach (var k in ((AttributesInt32)b).Values)
            {
                this.Set(k.Key, this.Get(k.Key) + k.Value);
            }
        }

        public void Multiply(INumerable b)
        {

        }
    }

    public partial class AttributesInt64 : global::ProtoBuf.IExtensible, INumerable
    {
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
            foreach (var k in ((AttributesInt64)b).Values)
            {
                this.Set(k.Key, this.Get(k.Key) + k.Value);
            }
        }

        public void Multiply(INumerable b)
        {

        }
    }

    public partial class AttributesSingle : global::ProtoBuf.IExtensible, INumerable
    {
        public void Set(long type, float value)
        {
            this.Values[type] = value;
        }

        public float Get(long type)
        {
            float v = 0;
            this.Values.TryGetValue(type, out v);
            return v;
        }

        public void Reset()
        {
            this.Values.Clear();
        }

        public void Add(long type, long value)
        {
            float v = 0;
            this.Values.TryGetValue(type, out v);
            this.Values[type] = v + value;
        }

        public void Add(INumerable b)
        {
            foreach (var k in ((AttributesSingle)b).Values)
            {
                this.Set(k.Key, this.Get(k.Key) + k.Value);
            }
        }

        public void Multiply(INumerable b)
        {

        }
    }


    public class AttributesSingle<T> where T : struct, Enum
    {
        AttributesSingle Attributes;

        public AttributesSingle(AttributesSingle attributes)
        {
            this.Attributes = attributes;
        }

        public void Set(int type, float value)
        {
            this.Attributes.Set(type, value);
        }

        public float Get(int type)
        {
            return this.Attributes.Get(type);
        }

        public void Reset()
        {
            this.Attributes.Reset();
        }

        public float this[int attr]
        {

            get { return this.Get(attr); }

            set { this.Set(attr, value); }
        }
    }

    public class AttributesInt32<T> where T : struct, Enum
    {
        AttributesInt32 Attributes;

        public AttributesInt32(AttributesInt32 attributes)
        {
            this.Attributes = attributes;
        }

        public void Set(int type, int value)
        {
            this.Attributes.Set(type, value);
        }

        public int Get(int type)
        {
            return this.Attributes.Get(type);
        }

        public int this[int attr]
        {

            get { return this.Get(attr); }

            set { this.Set(attr, value); }
        }
    }
}