using System;
using UnityEngine;

/// <summary>
/// 同时支持后进先出访问特性和随机删除能力的栈，在栈中删除任何元素时，不影响栈中其他元素的存储位置
/// Note: 内存存储使用从0开始的索引，对外表现的索引值从1开始
/// </summary>
/// 
public class StableStack<TValue>
{
    private TValue[] array;
    private int capacity;   // 当前容量
    private int incrStep;   // 容量增加幅度
    private int currindex;  // 当前栈顶位置

    /// <summary>
    /// 稳定栈的构造函数
    /// </summary>
    /// <param name="initcap">初始栈容量</param>
    /// <param name="increase">栈空间不够时的扩展幅度</param>
    public StableStack(int initcap, int increase = 16)
    {
        this.capacity = initcap;
        this.incrStep = increase;
        this.currindex = 0;
        array = new TValue[initcap];
    }

    /// <summary>
    /// 从栈顶压入一个新的元素。如果当前栈的容量不够，会自动按照构造时传入的增加幅度动态扩展容量
    /// </summary>
    /// <param name="value">被压入的元素值</param>
    public int Push(TValue value)
    {
        if (currindex >= capacity - 1)
        {
            Debug.LogWarning("StableStack Initial Capacity is too small, en-large it!");
            TValue[] newarray = new TValue[capacity + incrStep];
            for (int i = 0; i < capacity; i++)
            {
                newarray[i] = array[i];
            }
            capacity = capacity + incrStep;
            array = newarray;
        }

        array[currindex] = value;
        currindex++;

        return currindex;
    }

    /// <summary>
    /// 删除栈中给定索引的元素，返回被删除的元素值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue Remove(int index)
    {
        if (index <= 0)
        {
            return default(TValue);
        }

        if (index == currindex)
        {
            currindex--;
            for (; currindex > 0; currindex--)
            {
                if (array[currindex - 1] != null && !array[currindex - 1].Equals(default(TValue)))
                {
                    break;
                }
            }
        }

        TValue value = array[index - 1];
        array[index - 1] = default(TValue);
       
        return value;
    }

    /// <summary>
    /// 获取栈顶元素的值
    /// </summary>
    /// <returns></returns>
    public TValue GetLastValue()
    {
        int lastIndex = GetLastKey();
        if (lastIndex > 0)
        {
            return array[lastIndex - 1];
        }
        return default(TValue);
    }

    /// <summary>
    /// 获取栈顶元素的索引位置
    /// </summary>
    /// <returns></returns>
    public int GetLastKey()
    {
        for (int i = currindex; i >= 0; i--)
        {
            if (array[i] != null && !array[i].Equals(default(TValue)))
            {
                return i + 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获取栈底元素
    /// </summary>
    /// <returns></returns>
    public TValue GetFirstValue()
    {
        return array[0];
    }

    /// <summary>
    /// 设置栈底元素
    /// </summary>
    /// <param name="value"></param>
    public void SetFirstValue(TValue value)
    {
        array[0] = value;
        if (currindex == 0)
        {
            currindex++;
        }
    }

    /// <summary>
    /// 检查栈中是否有给定key的元素
    /// </summary>
    /// <param name="key">元素索引</param>
    /// <returns></returns>
    public bool ContainsKey(int key)
    {
        if (key <= 0 || key > currindex || key > capacity)
        {
            return false;
        }

        return (array[key - 1] != null && !array[key - 1].Equals(default(TValue)));
    }

    /// <summary>
    /// 属性。栈中有效元素的数量
    /// </summary>
    public int Count
    {
        get
        {
            int count = 0;
            for (int i = 0; i < currindex; i++)
            {
                if (array[i] != null && !array[i].Equals(default(TValue)))
                {
                    count++;
                }
            }
            return count;
        }
    }

    /// <summary>
    /// 检查索引处是否有值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(int index, out TValue value)
    {
        if (this.ContainsKey(index))
        {
            value = array[index - 1];
            return true;
        }

        value = default(TValue);
        return false;
    }

    /// <summary>
    /// 清空栈内的所有内容
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < currindex; i++)
        {
            array[i] = default(TValue);
        }

        currindex = 0;
    }

}

