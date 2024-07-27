using System.Collections.Generic;

namespace OpenNGS.Extension
{
    public static class RandomExtension
    {
        /// <summary>
        ///  Save this texture into JPG format file.
        /// </summary>
        /// <param name="tex">Text texture to convert.</param>
        /// <param name="file">The filename to save</param>
        /// <param name="flags"></param>
        public static T Random<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);
            if (list.Count == 1)
                return list[0];
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        
        public static int RandomIndex<T>(this List<T> list)
        {
            if (list.Count == 0)
                return -1;
            if (list.Count == 1)
                return 0;
            return UnityEngine.Random.Range(0, list.Count);
        }
    }
}
