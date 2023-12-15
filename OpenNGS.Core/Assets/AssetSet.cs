using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenNGS.Assets
{
    class AssetSet<T> : MonoBehaviour where T : Object
    {
        private static AssetSet<T> instance = null;
        private static Dictionary<string, T> setMap = new Dictionary<string, T>();

        [SerializeField]
        private T[] Set = null;

        public static void Load(string path)
        {
#if !UNITY_PS4
            if (instance==null)
            {
                try
                {
                    instance = Resources.Load<AssetSet<T>>(path);
                }
                catch(System.Exception ex)
                {
                    Debug.LogErrorFormat("AssetSet.Load:{0} Error:{1}", ex.ToString());
                }
            }
            foreach (var v in instance.Set)
            {
                if (v != null)
                    setMap.Add(v.name, v);
            }
#endif
        }

        public static T Get(string name)
        {
            T val;
            if (setMap.TryGetValue(name, out val))
                return val;
            Debug.LogErrorFormat("{0}[{1}] not found in Set", typeof(T), name);
            return default(T);
        }
    }
}
