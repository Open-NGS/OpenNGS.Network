using System;
using System.Collections;
using System.Collections.Generic;

namespace Neptune
{
    // 
    // Data Management Module V1.0 - mailto:Ray@RayMix.net
    // 

    /// <summary>
    /// DelayDictionary
    /// Delay load dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DelayDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        Dictionary<TKey, TValue> finalData = new Dictionary<TKey, TValue>();
        Dictionary<string, object> baseData = null;

        public TValue this[TKey key]
        {
            get
            {
                TValue val;
                TryGetValue(key, out val);
                return val;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                return finalData.Keys;
            }
        }

        public ValueCollection Values
        {
            get
            {
                return new ValueCollection(this);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return finalData.ContainsKey(key);
        }

        public int Count
        {
            get
            {
                return finalData.Count;
            }
        }

        public DelayDictionary(Dictionary<string, object> data)
        {
            baseData = data;
            foreach (KeyValuePair<string, object> kv in baseData)
            {
                finalData[GetKey(kv.Key)] = default;
            }
        }

        private TKey GetKey(string key)
        {
            TKey newkey = default;
            if (newkey is int)
            {
                return (TKey)(object)int.Parse(key);
            }
            if (newkey is float)
            {
                return (TKey)(object)float.Parse(key);
            }
            else
            {
                return (TKey)(object)key;
            }
        }

        public DelayDictionary(Dictionary<TKey, TValue> data)
        {
            finalData = data;
        }

        public bool TryGetValue(TKey key, out TValue val)
        {
            if (finalData.TryGetValue(key, out val))
            {
                if (val == null)
                {
                    val = GetValue(key.ToString());
                    finalData[key] = val;
                }
                return true;
            }
            return false;
        }

        TValue GetValue(string key)
        {
            try
            {
                TValue val = default;
                object data;
                if (baseData.TryGetValue(key, out data))
                {
                    val = fastBinaryJSON.BJSON.ToObject<TValue>((byte[])data);
                }
                return val;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("DelayDictionary GetItem {0} Failed:{1}.", key, ex.ToString());
                return default;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private DelayDictionary<TKey, TValue> dictionary;
            private KeyValuePair<TKey, TValue> current;
            IDictionaryEnumerator baseEnumerator;

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the <see cref="T:System.Collections.Generic.Dictionary`2" /> at the current position of the enumerator.</returns>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return current;
                }
            }
            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator, as an <see cref="T:System.Object" />.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            object IEnumerator.Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }
            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the dictionary at the current position of the enumerator, as a <see cref="T:System.Collections.DictionaryEntry" />.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    return new DictionaryEntry(current.Key, current.Value);
                }
            }
            /// <summary>Gets the key of the element at the current position of the enumerator.</summary>
            /// <returns>The key of the element in the dictionary at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            object IDictionaryEnumerator.Key
            {
                get
                {
                    return current.Key;
                }
            }
            /// <summary>Gets the value of the element at the current position of the enumerator.</summary>
            /// <returns>The value of the element in the dictionary at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            object IDictionaryEnumerator.Value
            {
                get
                {
                    return current.Value;
                }
            }
            internal Enumerator(DelayDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                if (dictionary.baseData != null)
                    baseEnumerator = dictionary.baseData.GetEnumerator();
                else
                    baseEnumerator = dictionary.finalData.GetEnumerator();
                current = default;
            }
            /// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.Dictionary`2" />.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                bool canmove = baseEnumerator.MoveNext();
                if (canmove)
                {

                    if (dictionary.baseData == null)
                    {
                        current = new KeyValuePair<TKey, TValue>((TKey)baseEnumerator.Key, (TValue)baseEnumerator.Value);
                    }
                    else
                    {
                        TKey key = dictionary.GetKey(baseEnumerator.Key.ToString());
                        current = new KeyValuePair<TKey, TValue>(key, dictionary[key]);
                    }
                }
                else
                    current = default;
                return canmove;
            }
            /// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.Dictionary`2.Enumerator" />.</summary>

            public void Dispose()
            {
            }
            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>

            void IEnumerator.Reset()
            {
                current = default;
            }
        }


        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection
        {
            /// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</summary>
            [Serializable]
            public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
            {
                private DelayDictionary<TKey, TValue>.Enumerator enumerator;
                private TValue currentValue;
                /// <summary>Gets the element at the current position of the enumerator.</summary>
                /// <returns>The element in the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> at the current position of the enumerator.</returns>
                public TValue Current
                {
                    get
                    {
                        return currentValue;
                    }
                }
                /// <summary>Gets the element at the current position of the enumerator.</summary>
                /// <returns>The element in the collection at the current position of the enumerator.</returns>
                /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
                object IEnumerator.Current
                {
                    get
                    {
                        return currentValue;
                    }
                }
                internal Enumerator(DelayDictionary<TKey, TValue> dictionary)
                {
                    currentValue = default;
                    enumerator = (DelayDictionary<TKey, TValue>.Enumerator)dictionary.GetEnumerator();
                }
                /// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection.Enumerator" />.</summary>
                public void Dispose()
                {
                }
                /// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</summary>
                /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                public bool MoveNext()
                {
                    bool move = enumerator.MoveNext();
                    if (move)
                    {
                        currentValue = enumerator.Current.Value;
                    }
                    else
                        currentValue = default;
                    return false;
                }
                /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
                /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                void IEnumerator.Reset()
                {
                    currentValue = default;
                }
            }
            private DelayDictionary<TKey, TValue> dictionary;
            /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</summary>
            /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</returns>
            public int Count
            {
                get
                {
                    return dictionary.Count;
                }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }
            /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
            /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.  In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />, this property always returns false.</returns>
            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
            /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />, this property always returns the current instance.</returns>
            object ICollection.SyncRoot
            {
                get
                {
                    return ((ICollection)dictionary).SyncRoot;
                }
            }
            /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> class that reflects the values in the specified <see cref="T:System.Collections.Generic.Dictionary`2" />.</summary>
            /// <param name="dictionary">The <see cref="T:System.Collections.Generic.Dictionary`2" /> whose values are reflected in the new <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</param>
            /// <exception cref="T:System.ArgumentNullException">
            ///   <paramref name="dictionary" /> is null.</exception>
            public ValueCollection(DelayDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
            }
            /// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</summary>
            /// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection.Enumerator" /> for the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            /// <summary>Copies the <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> elements to an existing one-dimensional <see cref="T:System.Array" />, starting at the specified array index.</summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="T:System.ArgumentNullException">
            ///   <paramref name="array" /> is null.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///   <paramref name="index" /> is less than zero.</exception>
            /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.Dictionary`2.ValueCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentException("array");
                }
                if (index < 0 || index > array.Length)
                {
                    throw new IndexOutOfRangeException("index out of range");
                }
                if (array.Length - index < dictionary.Count)
                {
                    throw new IndexOutOfRangeException("index out of range");
                }
                int count = dictionary.Count;
                int i = 0;
                foreach (KeyValuePair<TKey, TValue> kv in dictionary)
                {
                    array[index++] = kv.Value;
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
                return false;
            }
            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }
            bool ICollection<TValue>.Contains(TValue item)
            {
                throw new NotSupportedException();
                //return this.dictionary.ContainsValue(item);
            }
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            /// <summary>Returns an enumerator that iterates through a collection.</summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(dictionary);
            }
            /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="T:System.ArgumentNullException">
            ///   <paramref name="array" /> is null.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///   <paramref name="index" /> is less than zero.</exception>
            /// <exception cref="T:System.ArgumentException">
            ///   <paramref name="array" /> is multidimensional.-or-<paramref name="array" /> does not have zero-based indexing.-or-The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    throw new ArgumentException("array");
                }
                if (index < 0 || index > array.Length)
                {
                    throw new IndexOutOfRangeException("index out of range");
                }
                if (array.Length - index < dictionary.Count)
                {
                    throw new IndexOutOfRangeException("index out of range");
                }
                TValue[] array2 = array as TValue[];
                if (array2 != null)
                {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null)
                {
                    throw new ArgumentException();
                }
                int count = dictionary.Count;
                int i = 0;
                try
                {
                    foreach (KeyValuePair<TKey, TValue> kv in dictionary)
                    {
                        array3[index++] = kv.Value;
                    }
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw ex;
                }
            }
        }
    }
}