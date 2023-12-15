using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Collections.Generic
{
    public interface IDictionaryEnumerator : IEnumerator
    {
        DictionaryEntry Entry { get; }
        object Key { get; }
        object Value { get; }
    }

    public class NDictValue<T>
    {
        public T Value;
        public bool deleted;
    }


    public class NDictionary<TKey, TValue> : Dictionary<TKey, NDictValue<TValue>>, ISafeCollection
    {
        NDictValue<TValue> reused;
        List<TKey> deleted = new List<TKey>();

        Dictionary<TKey, NDictValue<TValue>> pending = new Dictionary<TKey, NDictValue<TValue>>();

        private int enumatingCount = 0;

        private int count;

        public new int Count { get { return count; } }

        public new struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            internal Dictionary<TKey, NDictValue<TValue>>.Enumerator enumerator;
            internal NDictionary<TKey, TValue> owner;
            private KeyValuePair<TKey, TValue> current;

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }

            internal Enumerator(NDictionary<TKey, TValue> owner ,Dictionary<TKey, NDictValue<TValue>>.Enumerator enumerator)
            {
                this.owner = owner;
                this.enumerator = enumerator;
                current = default(KeyValuePair<TKey, TValue>);
                this.owner.enumatingCount++;
            }

            public bool MoveNext()
            {
                if(this.enumerator.MoveNext())
                {
                    var internalCurrent = this.enumerator.Current;
                    var val = internalCurrent.Value;
                    if (val.deleted)
                        return this.MoveNext();
                    current = new KeyValuePair<TKey, TValue>(internalCurrent.Key, val.Value);
                    return true;
                }
                current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            void IEnumerator.Reset()
            {
                current = default(KeyValuePair<TKey, TValue>);
            }

            public void Dispose()
            {
                this.owner.enumatingCount--;
                this.owner.FinalizModify();
            }
        }

        public new bool ContainsKey(TKey key)
        {
            if(base.TryGetValue(key,out reused))
            {
                return !reused.deleted;
            }
            if (this.pending.TryGetValue(key, out reused))
            {
                return !reused.deleted;
            }
            return false;
        }

        public void Add(TKey key,TValue value)
        {
            NDictValue<TValue> item = new NDictValue<TValue>();
            item.deleted = false;
            item.Value = value;
            if (this.enumatingCount > 0)
            {
                this.pending.Add(key, item);
            }
            else
            {
                base.Add(key, item);
                count++;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (base.TryGetValue(key, out reused))
            {
                if (!reused.deleted)
                {
                    value = reused.Value;
                    return true;
                }
            }
            if (this.pending.TryGetValue(key, out reused))
            {
                if (!reused.deleted)
                {
                    value = reused.Value;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public new bool Remove(TKey key)
        {
            if (this.enumatingCount > 0)
            {
                if (base.TryGetValue(key, out reused))
                {
                    if (reused.deleted)
                        return false;
                    reused.deleted = true;
                    deleted.Add(key);
                    return true;
                }
                return false;
            }
            else
                return base.Remove(key);
        }

        public new Enumerator GetEnumerator()
        {
            return new Enumerator(this,base.GetEnumerator());
        }


        public void FinalizModify()
        {
            if (this.enumatingCount == 0)
            {
                if (deleted.Count > 0)
                {
                    foreach (var key in deleted)
                    {
                        base.Remove(key);
                    }
                    deleted.Clear();
                }
                if (pending.Count > 0)
                {
                    foreach (var kv in pending)
                    {
                        base.Add(kv.Key, kv.Value);
                    }
                    pending.Clear();
                }
                count = base.Count;
            }
        }
    }
}
