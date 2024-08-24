using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// @fangyiliu(刘方毅)
/// 集合类方法扩展
/// </summary>
public static partial class ExtendCollection
{
    public struct Pair<K, V>
    {
        public Pair(K k, V v)
        {
            this.k = k;
            this.v = v;
        }
        public K k;
        public V v;
    }


    public static void Foreach(int max, Action<int> fun)
    {
        for (int index = 0; index < max; index++)
        {
            fun(index);
        }
    }
    public static void Foreach(int xMax, int yMax, Action<int, int> fun)
    {
        for (int x = 0; x < xMax; ++x)
        {
            for (int y = 0; y < yMax; ++y)
            {
                fun(x, y);
            }
        }
    }

    public static T Find<T>(this IEnumerable<T> enu, Predicate<T> match)
    {
        foreach (var it in enu)
        {
            if (match(it))
            {
                return it;
            }
        }
        return default(T);
    }

	public static T Find<T>(this T[] array, Predicate<T> match)
	{
		T it;
		for (int i = 0, len = array.Length; i < len; i++)
		{
			it = array[i];
			if (match(it))
			{
				return it;
			}
		}
		return default(T);
	}

    public static bool Exist<T>(this IEnumerable<T> enu, Predicate<T> match)
    {
        foreach (var it in enu)
        {
            if (match(it))
            {
                return true;
            }
        }
        return false;
    }

	public static bool Exist<T>(this T[] array, Predicate<T> match)
	{
		T it;
		for (int i = 0, len = array.Length; i < len; i++)
		{
			it = array[i];
			if (match(it))
			{
				return true;
			}
		}
		return false;
	}

	public static int Count<T>(this T[] array, Predicate<T> match)
	{
        int result = 0;

		T it;
		for (int i = 0, len = array.Length; i < len; i++)
		{
			it = array[i];
			if (match(it))
			{
                result++;
			}
		}

		return result;
	}

    public static void Foreach<T>(this T[] array, Action<T> action)
    {
		T it;
		for (int i = 0, len = array.Length; i < len; i++)
		{
			it = array[i];
            action(it);
		}
    }

	public static void Foreach<K, V>(this Dictionary<K, V> dict, Action<KeyValuePair<K, V>> fun)
    {
        foreach (var it in dict)
        {
            fun(it);
        }
    }
    public static void ForeachKey<K, V>(this Dictionary<K, V> dict, Action<K> fun)
    {
        foreach (var it in dict)
        {
            fun(it.Key);
        }
    }
    public static void ForeachValue<K, V>(this Dictionary<K, V> dict, Action<V> fun)
    {
        foreach (var it in dict)
        {
            fun(it.Value);
        }
    }
}


/// <summary>
/// 遍历的时候支持 同步删除和添加
/// </summary>
/// <typeparam name="T"></typeparam>
public class ForeachMutableList<T> : ICollection<T>
{
    List<T> Added = new List<T>(), Current = new List<T>(), Removed = new List<T>();
    bool NeedClear = false;
    public int Count { get { _Sync(); return Current.Count;} }

    public bool IsReadOnly { get { return false; } }

    public void Add(T item)
    {
        if (Removed.Contains(item))
        {
            Removed.RemoveAll(t=> t.Equals(item));
        }
        Added.Add(item);
    }

    public void Clear()
    {
        Added.Clear();
        Removed.Clear();
        NeedClear = true;
    }

    public bool Contains(T item)
    {
        if (Removed.Contains(item))
            return false;
        return Current.Contains(item) || Added.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _Sync();
        Current.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        _Sync();
        return Current.GetEnumerator();
    }

    public bool Remove(T item)
    {
        bool removed = false;
        if(Added.Contains(item))
        {
            Added.RemoveAll(t=> t.Equals(item));
            removed = true;
        }
        if(Current.Contains(item))
        {
            removed = true;
            Removed.Add(item);
        }
        return removed;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        _Sync();
        return Current.GetEnumerator();
    }
    void _Sync()
    {
        if (NeedClear)
        {
            Current.Clear();
            NeedClear = false;
        }
        foreach (var it in Removed)
        {
            Current.Remove(it);
        }
        Removed.Clear();
        foreach (var it in Added)
        {
            if (!Current.Contains(it))
            {
                Current.Add(it);
            }
        }
        Added.Clear();
    }
    public T this[int index] { get { return Current[index]; } }
}


public class ArrayList<T> : IEnumerable<T>
{
    private const int DefaultCapacity = 8;
    public T[] innerArray;
    public int Count { get; private set; } //Also the index of the next element to be added
    public int Capacity = DefaultCapacity;
    public bool IsValueType { get; private set; }
    public ArrayList(ArrayList<T> CopyList)
    {
        innerArray = (T[])CopyList.innerArray.Clone();
        Count = innerArray.Length;
        Capacity = innerArray.Length;
    }

    public ArrayList(T[] StartArray)
    {
        innerArray = StartArray;
        Count = innerArray.Length;
        Capacity = innerArray.Length;
    }

    public ArrayList(int StartCapacity)
    {
        Capacity = StartCapacity;
        innerArray = new T[Capacity];

        Initialize();
    }
    public ArrayList()
    {
        innerArray = new T[Capacity];
        Initialize();
    }

    private void Initialize()
    {

        Count = 0;
        this.IsValueType = typeof(T).IsValueType;
    }

    public void Add(T item)
    {
        EnsureCapacity(Count + 1);
        innerArray[Count++] = item;

    }

    public void AddRange(ArrayList<T> items)
    {
        int arrayLength = items.Count;
        EnsureCapacity(Count + arrayLength + 1);
        for (int i = 0; i < arrayLength; i++)
        {
            innerArray[Count++] = items[i];
        }
    }

    public void AddRange(T[] items)
    {
        int arrayLength = items.Length;
        EnsureCapacity(Count + arrayLength + 1);
        for (int i = 0; i < arrayLength; i++)
        {
            innerArray[Count++] = items[i];
        }
    }
    public void AddRange(T[] items, int startIndex, int count)
    {
        EnsureCapacity(Count + count + 1);
        for (int i = 0; i < count; i++)
        {
            innerArray[Count++] = items[i + startIndex];
        }
    }

    public bool Remove(T item)
    {

        int index = Array.IndexOf(innerArray, item, 0, Count);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        Count--;
        innerArray[index] = default(T);
        Array.Copy(innerArray, index + 1, innerArray, index, Count - index);

    }

    public T[] ToArray()
    {
        T[] retArray = new T[Count];
        Array.Copy(innerArray, 0, retArray, 0, Count);
        return retArray;
    }

    public bool Contains(T item)
    {
        return Array.IndexOf(innerArray, item, 0, Count) != -1;
    }

    public void Reverse()
    {
        //Array.Reverse (innerArray,0,Count);
        int highCount = Count / 2;
        int reverseCount = Count - 1;
        for (int i = 0; i < highCount; i++)
        {
            T swapItem = innerArray[i];
            innerArray[i] = innerArray[reverseCount];
            innerArray[reverseCount] = swapItem;

            reverseCount--;
        }
    }

    public void EnsureCapacity(int min)
    {
        if (Capacity < min)
        {
            Capacity *= 2;
            if (Capacity < min)
            {
                Capacity = min;
            }
            Array.Resize(ref innerArray, Capacity);
        }
    }

    public T this[int index]
    {
        get
        {
            return innerArray[index];
        }
        set
        {
            innerArray[index] = value;
        }
    }

    public void Clear()
    {
        if (this.IsValueType)
        {
            FastClear();
        }
        else
        {
            for (int i = 0; i < Capacity; i++)
            {
                innerArray[i] = default(T);
            }
        }
        Count = 0;
    }

    /// <summary>
    /// Marks elements for overwriting. Note: this list will still keep references to objects.
    /// </summary>
    public void FastClear()
    {
        Count = 0;
    }

    public void CopyTo(ArrayList<T> target)
    {
        Array.Copy(innerArray, 0, target.innerArray, 0, Count);
        target.Count = Count;
        target.Capacity = Capacity;
    }

    public T[] TrimmedArray
    {
        get
        {
            T[] ret = new T[Count];
            Array.Copy(innerArray, ret, Count);
            return ret;
        }
    }

    public override string ToString()
    {
        if (Count <= 0)
            return base.ToString();
        string output = string.Empty;
        for (int i = 0; i < Count - 1; i++)
            output += innerArray[i] + ", ";

        return base.ToString() + ": " + output + innerArray[Count - 1];
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < this.Count; i++)
        {
            yield return this.innerArray[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < this.Count; i++)
        {
            yield return this.innerArray[i];
        }
    }

    public void Enumerate(ArrayList<T> output)
    {
        output.FastClear();
        output.AddRange(this);
    }
}

public class SDictionary<K, V> : IDictionary<K,V>
{
    List<K> mKeys = new List<K>(3);
    List<V> mValues = new List<V>(3);

    public void Add(K key, V value)
    {
        for (int i = 0; i < mKeys.Count; i++)
        {
            if (mKeys[i].Equals(key))
            {
                mValues[i] = value;
                return;
            }
        }
        mKeys.Add(key);
        mValues.Add(value);
    }
    public bool RemoveKey(K k)
    {
        for (int i = 0; i < mKeys.Count; i++)
        {
            if (mKeys[i].Equals(k))
            {
                mKeys.RemoveAt(i);
                mValues.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    public bool RemoveValue(V v)
    {
        for (int i = 0; i < mValues.Count; i++)
        {
            if (mValues[i].Equals(v))
            {
                mKeys.RemoveAt(i);
                mValues.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    public void Sort(Comparison<K> comparison)
    {
        mKeys.Sort(comparison);
    }
    public void Clear()
    {
        mKeys.Clear();
        mValues.Clear();
    }
    public bool ContainsKey(K key)
    {
        for (int i = 0; i < mKeys.Count; i++)
        {
            if (mKeys[i].Equals(key))
            {
                return true;
            }
        }
        return false;
    }
    public bool ContainsValue(V value)
    {
        for (int i = 0; i < mValues.Count; i++)
        {
            if (mValues[i].Equals(value))
            {
                return true;
            }
        }
        return false;
    }
    public bool TryGetValue(K key, out V value)
    {
        for (int i = 0; i < mKeys.Count; i++)
        {
            if (mKeys[i].Equals(key))
            {
                value = mValues[i];
                return true;
            }
        }
        value = default(V);
        return false;
    }
    public int Count { get { return mKeys.Count; } }

    public ICollection<K> Keys { get { return mKeys; } }

    public ICollection<V> Values { get { return mValues; } }

    public bool IsReadOnly { get { return false; } }

    public V this[K key] {
        get {
            V value = default(V);
            if(TryGetValue(key, out value))
            {
                return value;
            }
            return value;
        }
        set
        {
            Add(key, value);
        }
    }

    public V this[int index] { get { return mValues[index]; } }
    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public bool Remove(K key)
    {
        return RemoveKey(key);
    }

    public void Add(KeyValuePair<K, V> item)
    {
        Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
        for (int i = 0; i < mValues.Count; i++)
        {
            if (mValues[i].Equals(item.Value) && mKeys[i].Equals(item.Key))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
        for (int i = 0; i < mValues.Count; i++)
        {
            if (mValues[i].Equals(item.Value) && mKeys[i].Equals(item.Key))
            {
                mKeys.Remove(item.Key);
                mValues.Remove(item.Value);
                return true;
            }
        }
        return false;
    }

    IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public struct Enumerator : IEnumerator<KeyValuePair<K, V>>
    {
        SDictionary<K, V> dic;
        int index;
        public Enumerator(SDictionary<K, V> dic)
        {
            this.dic = dic;
            this.index = -1;
        }
        public KeyValuePair<K, V> Current
        {
            get
            {
                return new KeyValuePair<K, V>(dic.mKeys[index], dic.mValues[index]);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return new KeyValuePair<K, V>(dic.mKeys[index], dic.mValues[index]);
            }
        }

        public void Dispose()
        {
        }
        public bool MoveNext()
        {
            return ++index < dic.mKeys.Count;
        }
        public void Reset()
        {
            index = -1;
        }
    }
}
