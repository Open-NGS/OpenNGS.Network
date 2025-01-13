using OpenNGS.Assets;
using UnityEngine;
using UnityEngine.Events;

namespace Neptune.Assets
{
//    class AssetLoadAction<T> where T : Object
//    {
//        UnityAction<T> onLoad;
//        private float startTime;
//        private bool autoDestroy;
//        public string error = null;

//        private AssetLoadAction()
//        {
//            float startTime = Time.realtimeSinceStartup;
//        }

//        public AssetLoadAction(UnityAction<T> onLoadBundle, bool autoDestroy) : this()
//        {
//            this.onLoad = onLoadBundle;
//            this.autoDestroy = autoDestroy;
//        }

//        public void OnLoadBundle(AssetBundleInfo bundle)
//        {
//            if (bundle != null && bundle.assetBundle != null)
//            {
//                string assetname = AssetLoader.GetAssetName(bundle.name);
//                //Debug.Log("LoadAsset: bundle loaded :" + resource + ".  DependCount:" + bundle.DependenciesCount);
//#if DEVELOPMENT
//            UFProfiler.UIProfilerData data = null;
//            if (UFProfiler.indent > 0)
//                data = UFProfiler.Start(string.Format("assetBundle.LoadAsset({0})", assetname));
//#endif
                
//                T obj = bundle.AssetBundle.LoadAsset<T>(assetname);
//#if DEVELOPMENT
//            UFProfiler.End(data);
//#endif
//                if (onLoad != null)
//                {
//                    if (obj == null)
//                        Debug.LogError("LoadAsset: asset is null :" + assetname + ". \n");
//                    onLoad(obj);
//                }

//                bundle.Unload();
//                if (autoDestroy && obj is GameObject)
//                {
//                    AssetLoader.DestroyLoadedAssetObject(obj as GameObject);
//                }

//            }
//            else
//            {
//                error = "LoadAsset: bundle is null";
//                //T obj = Resources.Load(resource) as T;
//                if (onLoad != null)
//                {
//                    onLoad(null);
//                }
//            }
//        }
//    }
}