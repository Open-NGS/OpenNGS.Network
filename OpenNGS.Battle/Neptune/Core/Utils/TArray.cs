using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//public class TArrayElem
//{
//    public int Index = -1;
//}
public class TArray<T>
{
    public int Length = 0;
    private int num = 0;
    private T[] Data;
    public int Capacity = 0;
    private int enableIndex = 0;


    public T this[int index]
    {
        get {
            return this.Data[index];
        }
        set {
            this.Data[index] = value;
        }
    }

    public TArray(int capacity)
    {
        Capacity = capacity;
        Data = new T[capacity];
    }

    private bool added = false;
    public void Add(T element)
    {
        if (num >= Capacity)
        {
            this.Capacity += this.Capacity / 2;
            //自动扩容
            T[] newData = new T[this.Capacity];
            Array.Copy(this.Data, 0, newData, 0, this.Data.Length);
            this.Data = newData;
        }
        added = false;
        for (int i = enableIndex; i < Capacity; i++)
        {
            if (Data[i] == null)
            {
                if (!added)
                {
                    added = true;
                    Data[i] = element;
                    num++;
                    //element.Index = i;
                    if (Length - 1 < i)
                    {
                        Length = i + 1;
                    }
                }
                else
                {
                    enableIndex = i;
                    return;
                }
            }
        }
    }


    public bool Contains(T element)
    {
        for (int i = 0; i < Length; i++)
        {
            if (Data[i] != null && Data[i].Equals(element))
            {
                return true;
            }
        }
        return false;
    }
    public bool Contains(int index)
    {
        if (index >= 0 && index < Length && Data[index] != null)
        {
            return true;
        }
        return false;
    }
    public bool Remove(int index)
    {
        if (index >=0 && index < Length)
        {
            if (enableIndex > index)
            {
                enableIndex = index;
            }
            Data[index] = default(T);
            num--;
            if (Length-1 == index)
            {
                Length--;
            }
            return true;
        }
        return false;
    }

    public void Sort(IComparer<T> compare)
    {
        Array.Sort(Data, compare);
        if (num != Capacity)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] != null)
                {
                    Length = i + 1;
                    enableIndex = Length;
                }
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < Length; i++)
        {
            Data[i] = default(T);
        }
        Length = 0;
        num = 0;
        enableIndex = 0;
    }

    public void Remove(T element)
    {
        for (int i = 0; i < Length; i++)
        {
            if (Data[i] != null && Data[i].Equals(element))
            {
                Remove(i);
            }
        }
        
    }
}