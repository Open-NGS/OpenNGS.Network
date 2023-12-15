using System.Collections;
using System.Collections.Generic;
using OpenNGS.Assets;
using UnityEngine;

namespace OpenNGS.UI.UGUI
{
    public class UIFactory
    {
        public static IUView CreateView(string package, string component)
        {
            IUView viewCtr = null;
            var prefab = OpenNGSResources.Load<GameObject>($"UI/View/{package}/{component}.prefab");
            if (null == prefab)
            {
                prefab = Resources.Load($"{package}/{component}", typeof(GameObject)) as GameObject;
            }
            
            if (prefab == null)
            {
                Debug.LogError($"UGUI resources not found: {package}/{component}");
                return null;
            }
            
            var root = GameObject.FindWithTag("UGUIRoot");
            if (root == null)
            {
                Debug.LogError("UGUI Root not found!");
                return null;
            }
            
            var go = Object.Instantiate(prefab, root.transform);
            
            // atlas late binding on device need rebuild. maybe bug of unity 2021.3
            go.SetActive(false);
            go.SetActive(true);
            
            viewCtr = go.GetComponent<IUView>();
            return viewCtr;
        }
    }
}