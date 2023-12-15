using OpenNGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Collections.Generic
{
    public class NList<T> : List<T>, ISafeCollection
    {
        private int enumatingCount = 0;
        List<T> deleted = new List<T>();

        public static NList<T> Empty = new NList<T>();

        public T NullValue { get; private set; }

        public new struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private NList<T> list;
            private int index;
            private T current;
            private int refer;
            public T Current { get { return current; } }

            object IEnumerator.Current
            {
                get
                {
                    if (index == 0 || index == list.Count + 1)
                    {
                        throw new InvalidOperationException();// .ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }

            public Enumerator(NList<T> list)
            {
                list.enumatingCount++;
                this.list = list;
                index = 0;
                refer = 0;
                current = list.NullValue;
            }

            public bool MoveNext()
            {
                uint count = (uint)list.Count;
                if ((uint)index < count)
                {
                    current = list[index];
                    index++;
                    if (current == null || current.Equals(list.NullValue))
                    {
                        return MoveNext();
                    }
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                index = list.Count + 1;
                current = list.NullValue;
                return false;
            }

            public void Reset()
            {
                refer++;
                index = 0;
                current = list.NullValue;
            }

            public void Dispose()
            {
                this.list.enumatingCount--;
                this.list.FinalizModify();
            }
        }


        public NList()
        {
            NullValue = default(T);
        }

        public NList(T nullValue)
        {
            NullValue = nullValue;
        }

        public new void RemoveAt(int index)
        {
            
            base[index] = this.NullValue;
        }

        public new bool Remove(T item)
        {
            int idx = base.IndexOf(item);
            if(idx>=0)
            {
                deleted.Add(base[idx]);
                base[idx] = this.NullValue;
                return true;
            }
            return false;
        }

        public new Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void FinalizModify()
        {
            if (this.enumatingCount == 0)
            {
                if (deleted.Count > 0)
                {
                    this.RemoveAll(RemovePredicate);
                    deleted.Clear();
                }
            }
        }

        private bool RemovePredicate(T v)
        {
            return object.Equals(v, this.NullValue);
        }
    }

}
