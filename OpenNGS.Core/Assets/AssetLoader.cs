using OpenNGS.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenNGS.Assets
{
    public class AssetLoader
    {
        public static bool AlwaysRawMode = true;
#if UNITY_EDITOR
        public static bool RawMode = true;  // editor测试bundle模式改为false
        public static string RawResourcePath = "Assets/Game/BuildAssets/";
#endif

        /// <summary>
        /// Loads an asset.
        /// </summary>
        /// <typeparam name="T">Pathname of the target asset</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path, float unloadTime = float.MaxValue) where T : Object
        {
            path = path.ToLowerInvariant();
            T result = default(T);
#if DEBUG_LOG
            OpenNGS.Profiling.ProfilerLog.Start("AssetLoader.Load", path);
#endif

            if(AlwaysRawMode == true)
            {
                result = LoadFromRaw<T>(path);
                NgDebug.Log(string.Format("OpenNgsRes::Load RawMode path [{0}]", path));
            }
#if UNITY_EDITOR
            else
            {
                if (RawMode)
                {
                    result = LoadFromRaw<T>(path);
                    NgDebug.Log(string.Format("OpenNgsRes::Load RawMode path [{0}]",path));
                }
#else
            else
            { 
                result = LoadFromBundle<T>(path);
                NgDebug.Log(string.Format("OpenNgsRes::Load no RawMode path [{0}]", path));
#endif
            }

#if DEBUG_LOG
            OpenNGS.Profiling.ProfilerLog.End("AssetLoader.Load", path);
#endif

            if (!result)
            {
                Debug.LogError($"AssetLoader -- Faild to load asset : {path}");
            }

            return result;
        }
        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
            if(AlwaysRawMode == true)
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            else
            {
#if UNITY_EDITOR
                if (!RawMode)
#endif
                {
                    AssetBundleManager.Instance.LoadBundleBySceneName(sceneName);
                }
                SceneManager.LoadScene(sceneName, mode);
            }
        }
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
#if UNITY_EDITOR
            if (!RawMode)
#endif
            {
                AssetBundleManager.Instance.LoadBundleBySceneName(sceneName);
            }
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        private static T LoadFromBundle<T>(string path) where T : Object
        {
            T asset = default(T);
            path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(path);
#if DEBUG_LOG
            NgDebug.DebugFormat("AssetLoader.LoadFromBundle:assetName:{0} from path:{1}", assetName, path);

            OpenNGS.Profiling.ProfilerLog.Start("AssetLoader.LoadFromBundle:" + path);
#endif
            AssetBundleInfo bundle = AssetBundleManager.Instance.LoadBundleByAsset(path);
            if (bundle != null)
            {
                asset = bundle.LoadAsset<T>(assetName);
            }
#if DEBUG_LOG
            OpenNGS.Profiling.ProfilerLog.End("AssetLoader.LoadFromBundle:" + path);
#endif
            return asset;
        }


#if UNITY_EDITOR
        public static T LoadFromRaw<T>(string path) where T : Object
        {
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(Path.Combine(RawResourcePath, path ));
            return asset;
        }
#endif
    }
}

