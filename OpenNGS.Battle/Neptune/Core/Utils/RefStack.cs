using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// 一个带引用计数的特殊栈
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class RefStack<TKey, TValue>
{
    public string Name;
    /// <summary>
    /// FXID => Index
    /// </summary>
    public Dictionary<TKey, int> KeyMap = new Dictionary<TKey, int>();
    /// <summary>
    /// Index => Count
    /// </summary>
    public Dictionary<int, int> RefMap = new Dictionary<int, int>();

    /// <summary>
    /// Index => Value
    /// </summary>
    public Dictionary<int, TValue> ValueMap = new Dictionary<int, TValue>();


    protected int uniqueId = 0;


    protected int AddRef(TKey key, bool unique)
    {
        if (this.KeyMap.ContainsKey(key))
        {//已经存在效果
            if (unique)
            {//唯一类型，已返回已有对象，不重复创建，但增加引用以延长时间
                int index = this.KeyMap[key];
                this.RefMap[index]++;
                //Debug.LogFormat("EffectHolder[{0}:{1}]::[{2}:-]:AddRef > Index:{3} Ref:{4}", this.Name, this.GetHashCode(), key.ToString(), index, this.RefMap[index]);
                return index;
            }
            else
            {//不唯一类型，新增对象，这类对象不需要维护引用
                this.uniqueId++;
                this.RefMap[this.uniqueId] = 1;
            }
        }
        else
        {
            this.uniqueId++;
            this.KeyMap[key] = this.uniqueId;
            this.RefMap[this.uniqueId] = 1;
            uniqueId = uniqueId == int.MaxValue ? 0 : uniqueId;
        }
        //Debug.LogFormat("EffectHolder[{0}:{1}]::[{2}:-]:AddRef > Index:{3} Ref:{4}", this.Name, this.GetHashCode(), key.ToString(), uniqueId, this.RefMap.ContainsKey(uniqueId) ? this.RefMap[uniqueId] : 0);
        return 0;
    }

    public void Remove(int index)
    {
        if (this.RefMap.ContainsKey(index))
        {
            if (this.RefMap[index] > 1)
            {
                this.RefMap[index]--;
                //Debug.LogFormat("EffectHolder[{0}:{1}]::[{2}:{3}]:Remove > Index:{4} Ref:{5}", this.Name, this.GetHashCode(), this.ValueMap[index].ToString(), this.ValueMap[index].GetHashCode(), index, this.RefMap[index]);
                return;
            }
            this.RefMap.Remove(index);
        }

        TValue val = default(TValue);
        if (this.ValueMap.ContainsKey(index))
        {
            val = this.ValueMap[index];
            this.ValueMap.Remove(index);
        }

        //string name = "null";

        if (index == this.uniqueId)
        {//退栈操作
            this.uniqueId = this.RefMap.Count > 0 ? this.RefMap.Keys.Max() : 0;
        }

        foreach (KeyValuePair<TKey, int> kv in this.KeyMap)
        {
            if (kv.Value == index)
            {
                //name = kv.Key.ToString();
                this.KeyMap.Remove(kv.Key);
                break;
            }
        }

        //Debug.LogFormat("EffectHolder[{0}:{1}]::[{2}:{3}]:Remove > Index:{4} Ref:{5}", this.Name, this.GetHashCode(), name, val==null ? 0 : val.GetHashCode(), index, 0);

        this.OnRemove(index, val);
    }

    public virtual void OnRemove(int index, TValue val)
    {
    }

    public TValue GetValue(int index)
    {
        TValue val = default(TValue);
        if (this.ValueMap.ContainsKey(index))
        {
            val = this.ValueMap[index];
        }
        return val;
    }
}
