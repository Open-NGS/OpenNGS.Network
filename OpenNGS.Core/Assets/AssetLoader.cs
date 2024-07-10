using OpenNGS.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
namespace OpenNGS.Assets
{
    public class AssetLoader
    {
        public static bool RawMode = true;  // editor测试bundle模式改为false
#if UNITY_EDITOR
        public static string RawResourcePath = "Assets/Game/BuildAssets/";
#else
        public static string RawResourcePath = "BuildAssets/";
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

#if UNITY_EDITOR

            if (RawMode)
            {
                result = LoadFromRaw<T>(path);
#if DEBUG_LOG
                NgDebug.Log(string.Format("OpenNgsRes::Load RawMode path [{0}]", path));
#endif
            }
            else
            {

#endif
                result = LoadFromBundle<T>(path);
#if DEBUG_LOG
            OpenNGS.Profiling.ProfilerLog.End("AssetLoader.Load", path);
#endif

#if UNITY_EDITOR
            }
#endif
            if (!result)
            {
#if DEBUG_LOG
                NgDebug.Log($"AssetLoader -- Faild to load asset : {path}");
#endif
            }

            return result;
        }

        static public Sprite LoadIconSprite(string path)
        {
            string spriteName = System.IO.Path.GetFileNameWithoutExtension(path);
            string atlasName = System.IO.Path.GetDirectoryName(path);
            atlasName = atlasName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            SpriteAtlas spriteAtlas = AssetLoader.Load<SpriteAtlas>(atlasName + ".spriteatlas");
            if (spriteAtlas == null)
            {
                Debug.LogErrorFormat("LoadIconSprite {0} failed. Atlas:{1} not existed.", path, atlasName);
                return null;
            }
            return spriteAtlas.GetSprite(spriteName);
        }

        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
#if UNITY_EDITOR
            if (!RawMode)
#endif
            {
                AssetBundleManager.Instance.LoadBundleBySceneName(sceneName);
            }
            SceneManager.LoadScene(sceneName, mode);
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


        public static T LoadFromRaw<T>(string path) where T : Object
        {
            T asset = null;
            string fullPath = path;
            if (RawMode == true)
            {
                fullPath = Path.Combine(RawResourcePath, path);
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
#else
                int firstDotIndex = fullPath.IndexOf('.');
                if (firstDotIndex != -1)
                {
                    fullPath = fullPath.Substring(0, firstDotIndex);
                }
                NgDebug.Log(string.Format("[test:{0}]", fullPath));
                asset = Resources.Load<T>(fullPath);
#endif
            }
            else
            {
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
#endif
            }
            return asset;
        }
    }
}

