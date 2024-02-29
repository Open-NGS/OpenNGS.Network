using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;



public interface ISafeListElement
{
    BitArray RemoveState
    {
        get; set;
    }
}

public class SafeList<T> : List<T> where T : ISafeListElement
{
    const int MaxBit = 1024;
    private int recursive = 0;
    static int StaticSafeListIdx = 0;

    bool final = false;
    int UniqueIdx;

    public SafeList()
    {
        UniqueIdx = StaticSafeListIdx++;
    }

    public bool IsRemoved(T item)
    {
        return item.RemoveState.Get(this.UniqueIdx);
    }

    public new IEnumerator<T> GetEnumerator()
    {
        recursive++;
        T element;
        int count = base.Count;
        for (int i = 0; i < count; i++)
        {
            element = base[i];
            if (IsRemoved(element))
                continue;
            yield return element;
        }
        recursive--;
        this.Final();
    }

    public new void Add(T item)
    {
        if (item.RemoveState == null)
            item.RemoveState = new BitArray(MaxBit);
        item.RemoveState.Set(this.UniqueIdx, false);
        base.Add(item);
    }


    public new bool Remove(T item)
    {
        item.RemoveState.Set(this.UniqueIdx, true);
        final = true;
        return true;
    }

    public new void RemoveAt(int index)
    {
        base[index].RemoveState.Set(this.UniqueIdx, true);
    }

    public void Final()
    {
        if (recursive == 0 && final)
        {
            final = false;
            base.RemoveAll(RemovePedicate);
        }
    }

    bool RemovePedicate(T e)
    {
        return e.RemoveState.Get(this.UniqueIdx);
    }
}