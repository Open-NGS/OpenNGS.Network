using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    /// <summary>
    /// Port Class
    /// We can't not use Sprites.DataUtility if the namespace is not UnityEngine.UI
    /// </summary>
    public static class SpritesUtility
    {
        public static Vector4 GetOuterUV(Sprite s)
        {
            return Sprites.DataUtility.GetOuterUV(s);
        }
        
        public static Vector4 GetInnerUV(Sprite s)
        {
            return Sprites.DataUtility.GetInnerUV(s);
        }
        
        public static Vector4 GetPadding(Sprite s)
        {
            return Sprites.DataUtility.GetPadding(s);
        }
        
        public static Vector2 GetMinSize(Sprite sprite)
        {
            Vector2 minSize;
            minSize.x = sprite.border.x + sprite.border.z;
            minSize.y = sprite.border.y + sprite.border.w;
            return minSize;
        }
    }
}