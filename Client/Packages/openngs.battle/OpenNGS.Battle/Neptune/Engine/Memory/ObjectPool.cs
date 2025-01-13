using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ObjectPool<T> where T : PoolObj, new()
{
    public static int Capacity;
    public static int Count;
    static int maxIndex = 0;
    static T[] elements;

    public static int MaxObject
    {
        get { return maxIndex; }
    }

    public ObjectPool()
    {

    }


    public static void Init(int size)
    {
        Capacity = size;
        //if (typeof(T) == typeof(Trap)) Debug.LogFormat("{0} Init {1}", typeof(ObjectPool<T>), size);
        //if (elements == null || elements.Length != size)
        elements = new T[size];
        for (int i = 0; i < size; i++)
        {
            elements[i] = new T();
            elements[i].poolIdx = i;
        }
        Count = 0;
    }

    public static T New()
    {
        Count++;
        T obj;
        if (maxIndex < Capacity)
        {
            obj = elements[maxIndex++];
            obj.IsNull = false;
            //if(typeof(T) == typeof(Trap))Debug.LogFormat("{0} New {1}", typeof(ObjectPool<T>), obj.poolIdx);
            return obj;
        }

        for(int i=0;i< Capacity; i++)
        {
            obj = elements[i];
            if (obj.IsNull)
            {
                obj.IsNull = false;
                //if (typeof(T) == typeof(Trap)) Debug.LogFormat("{0} New {1}", typeof(ObjectPool<T>), i);
                return obj;
            }
        }
        Count--;
        obj = new T();
        obj.IsNull = false;
        return obj;
    }

    /// <summary>
    /// 删除一个元素
    /// 注意：
    /// 只能且必需在 PoolObj 对象的 Delete 重载中调用
    /// 由 PoolObj 派生的一切子类及所有下级子类均需重载 Delete 方法
    /// </summary>
    /// <param name="obj"></param>
    public static void Delete(T obj)
    {
        obj.OnDelete();
        if (obj.poolIdx == -1 || obj.IsNull)
            return;
        obj.IsNull = true;

        //if (typeof(T) == typeof(Trap)) Debug.LogFormat("{0} Delete {1}", typeof(ObjectPool<T>), obj.poolIdx );
        Count--;
        if (obj.poolIdx == maxIndex - 1)
        {
            while (maxIndex > 0)
            {
                maxIndex--;
                if (maxIndex > 0 && !elements[maxIndex - 1].IsNull)
                    break;
            }
        }
    }
}

/// <summary>
/// Pool Object
/// </summary>
public abstract class PoolObj
{
    public bool IsNull = true;
    public int poolIdx = -1;

    /// <summary>
    /// 元素删除通知
    /// Delete时发生
    /// </summary>
    public abstract void OnDelete();

    /// <summary>
    /// 删除元素接口
    /// 此类派生的一切子类及所有下级子类均需重载 Delete 方法
    /// </summary>
    public abstract void Delete();

    //public static bool operator ==(PoolObj a, PoolObj b)
    //{
    //    if (Object.Equals(a, b))
    //        return true;

    //    if (Object.Equals(a, null) && !Object.Equals(b, null) && b.IsNull)
    //        return true;
    //    if (Object.Equals(b, null) && !Object.Equals(a, null) && a.IsNull)
    //        return true;
    //    return false;
    //}

    //public static bool operator !=(PoolObj a, PoolObj b)
    //{
    //    if (Object.Equals(a, b))
    //        return false;

    //    if (Object.Equals(a, null) && !Object.Equals(b, null) && b.IsNull)
    //        return false;
    //    if (Object.Equals(b, null) && !Object.Equals(a, null) && a.IsNull)
    //        return false;
    //    return true;
    //}
}