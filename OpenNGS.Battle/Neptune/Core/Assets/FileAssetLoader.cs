using UnityEngine;
using System.IO;
// 
// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
// 

namespace Neptune.Assets
{

    /// <summary>
    /// FileAssetLoader
    /// asset loader by File
    /// </summary>
    public class FileAssetLoader : IAssetLoader
    {
        private bool asyncMode = false;
        public FileAssetLoader(bool async = false)
        {
            asyncMode = async;
        }

        /// <summary>
        /// LoadBundle
        /// load a bundle 
        /// </summary>
        /// <param name="path">bundle relative path</param>
        /// <param name="type">bundle type</param>
        /// <returns></returns>
        public AssetBundleInfo LoadBundle(string path, AssetBundleType type)
        {
            if (type == AssetBundleType.AssetBundle)
            {
                //Debug.LogFormat("LoadBundle : {0} Async:{1}", path, asyncMode);
                if (asyncMode)
                {
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
                    AssetBundleInfo bundleInfo = new AssetBundleInfo(req);
                    return bundleInfo;
                }
                else
                {
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
#if UNITY_EDITOR
                    if(assetBundle == null)
                    {
                        NgDebug.LogError(string.Format("LoadBundle path = [{0}] failed. Please check dependency", path));
                    }
#endif
                    AssetBundleInfo bundleInfo = new AssetBundleInfo(assetBundle);
                    return bundleInfo;
                }
            }
            else
                return LoadRawData(path, type);
        }

        public AssetBundleInfo LoadRawData(string path, AssetBundleType type)
        {
            //Debug.LogFormat("LoadRawData : {0} Type:{1} Async:{2}", path, type, asyncMode);
            if (type == AssetBundleType.Text)
            {
                string text = null;
                if (File.Exists(path))
                    text = File.ReadAllText(path);

                if (text != null)
                    return new AssetBundleInfo(text);
            }
            else if (type == AssetBundleType.Bytes)
            {
                if (File.Exists(path))
                    return new AssetBundleInfo(File.ReadAllBytes(path));
            }
            return null;
        }
    }
}