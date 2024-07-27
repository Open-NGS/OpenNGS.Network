using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SLG
{
	 /// <summary>
	 /// Transform的扩展
	 /// </summary>
	public static class TransformExtend
	{
        public static RectTransform rectTransform(this Component cp)
        {
            return cp.transform as RectTransform;
        }

        public static void SetIdentity(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T comp = gameObject.GetComponent<T>();
			if (comp == null)
			{
				comp = gameObject.AddComponent<T>();
			}
			return comp;
		}

        public static T GetOrAddComponent<T>(this GameObject gameObject, Type type) where T : Component
        {
            T comp = gameObject.GetComponent(type) as T;
            if (comp == null)
            {
                comp = gameObject.AddComponent(type) as T;
            }
            return comp;
        }


        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
		{
			return transform.gameObject.GetOrAddComponent<T>();
		}

        public static void RemoveAllChild(this Transform transform)
        {
            if (transform != null)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
	}
}
