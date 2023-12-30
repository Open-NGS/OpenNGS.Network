using System.Collections.Generic;
/// <summary>
/// 对Dictionary的扩展
/// </summary>
public static class MissQDictionaryExtension
{

    /// <summary>
    /// 尝试根据Key得到value,得到了的话直接返回value,没有得到直接返回null
    /// this Dictionary<Tkey,Tvalue> dict 这个字典表示我们要获取值得字典
    /// </summary>
    public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }
}
