using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Neptune.Assets
{
    /// <summary>
    /// Resources Loader
    /// </summary>
    public class AssetLoader : BehaviourSingleton<AssetLoader>
    {
        /// <summary>
        /// indicate if the loader should load original assets
        /// </summary>
        public static bool RawMode = false;
        /// <summary>
        /// indicate if the loader should load UI prefab assets from Resources
        /// </summary>
        public static bool LoadUIFromResources = false;

        List<string> LoadingBundles = new List<string>();

        IEnumerator Start()
        {
            yield return null;

        }

        void FixedUpdate()
        {
        }

        public static string GetAssetName(string resource)
        {
            return Path.GetFileNameWithoutExtension(resource);
        }

        public static string GetAssetsFilename(string resource, string ext = ".assets")
        {
            string extension = Path.GetExtension(resource);
            if (!string.IsNullOrEmpty(extension))
            {
                ext = extension;
                resource = resource.Replace(ext, "");
            }
            resource = resource.ToLower();
            string assetename = Application.streamingAssetsPath + "/" + resource + ext;
            //Debug.Log("GetAssetsFilename: " + assetename);

#if UNITY_EDITOR
            if (System.IO.File.Exists(assetename))
            {
                assetename = "file://" + assetename;
                return assetename;
            }
            else
            {
                Debug.Log("file not exits :" + assetename);
                return "";
            }
#else
#if !UNITY_ANDROID
       assetename = "file://" + assetename;
#endif
       return assetename;
#endif
        }

        public bool isDone
        {
            get
            {
                return this.LoadingBundles.Count == 0;
            }
        }

        protected void LoadAsset<T>(string resource, UnityAction<T> onLoaded, bool autoDestroy, bool localized = false) where T : Object
        {

#if UNITY_EDITOR
            if (resource.Contains(" "))
            {
                Debug.LogError("Invalid Resource Name: " + resource);
                onLoaded(null);
                return;
            }

            if (RawMode)
            {
                string path = "Assets/Game/BuildAssets/" + resource + ".prefab";
                if (QualityUtil.QualityLevel == QualityLevel.Low)
                {
                    string path_lod = "Assets/Game/BuildAssets/" + resource + "_lod2" + ".prefab";
                    if (File.Exists(path_lod))
                        path = path_lod;
                }
                if (File.Exists(path))
                {
                    T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    if (onLoaded != null)
                    {
                        onLoaded(obj);
                        return;
                    }
                }
                else
                {
                    path = "Assets/Game/BuildAssets/" + resource + ".bytes"; ;
                    if (File.Exists(path))
                    {
                        T obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                        if (onLoaded != null)
                        {
                            onLoaded(obj);
                            return;
                        }
                    }
                    else
                    {
                        path = "Assets/Game/Resources/" + resource + ".prefab";
                        if (File.Exists(path))
                        {
                            T obj = Resources.Load<T>(resource);
                            if (onLoaded != null)
                            {
                                onLoaded(obj);
                                return;
                            }
                        }
                        else
                        {
                            if (onLoaded != null)
                            {
                                onLoaded(null);
                                return;
                            }
                        }
                    }
                }
            }
#endif
            AssetLoadAction<T> action = new AssetLoadAction<T>(onLoaded, autoDestroy);
            LoadBundle(resource, action.OnLoadBundle, localized);
        }

        public void UnLoadBundle(string bundlename)
        {
            AssetBundleManager.Instance.UnloadBundle(bundlename);
        }

        public void LoadBundle(string bundlename, UnityAction<AssetBundleInfo> onloaded, bool localized = false)
        {
            if (RawMode)
            {
                return;
            }
            AssetBundleManager.Instance.LoadBundle(bundlename, onloaded, localized);
        }

        public static void Load<T>(string resource, UnityAction<T> onLoaded, bool autoDestroy = true, bool localized = false) where T : Object
        {
            if (LoadUIFromResources && (resource.StartsWithFast("UI/prefab") || resource.StartsWithFast("UI/Prefab")))
            {
                T obj = Resources.Load<T>(resource);
                if (obj == null)
                    Debug.LogError("Load: asset is null :" + resource + ". \n");
                onLoaded(obj);
            }
            else
                AssetLoader.Instance.LoadAsset<T>(resource, onLoaded, autoDestroy, localized);
        }
        /// <summary>
        /// 加载Sprite 从Resrouces
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Sprite LoadSpriteFromResrouces(string resource)
        {
            Sprite obj = Resources.Load<Sprite>(resource);
            if (obj == null)
                Debug.LogError("Load: asset is null :" + resource + ". \n");
            return obj;
        }
        public static void Load(string resource, UnityAction<string> onLoaded)
        {
            AssetBundleManager.Instance.LoadText(resource, onLoaded);
        }
        public static void Load(string resource, UnityAction<byte[]> onLoaded)
        {
            AssetBundleManager.Instance.LoadBytes(resource, onLoaded);
        }

        public static void LoadTexture(string resource, UnityAction<Texture2D> onLoaded)
        {
            AssetBundleManager.Instance.LoadTexture(resource, onLoaded);
        }

        public static void DestroyLoadedAssetObject(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            bool shouldDestroy = true;
#if UNITY_EDITOR
            if (AssetLoader.Instance != null && AssetLoader.RawMode)
            {
                shouldDestroy = false;
            }
#endif
            if (shouldDestroy)
            {
                DestroyImmediate(obj, true);
            }
        }
    }

}