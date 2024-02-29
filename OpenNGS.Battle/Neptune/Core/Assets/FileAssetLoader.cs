using UnityEngine;
using System.IO;
// 
// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
// 

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
                AssetBundleInfo bundleInfo = new AssetBundleInfo(AssetBundle.LoadFromFile(path));
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
#if UNITY_EDITOR
            if (File.Exists(path))
                text = File.ReadAllText(path);
#else
            byte[] bytes = PlatformTools.GetFileContents(path);
            if (bytes != null)
            {
                float startTime = Time.realtimeSinceStartup;
                text = System.Text.Encoding.UTF8.GetString(bytes);
                //Debug.LogFormat("LoadRawData Done: {0} [{1},{2}] Decoding Text Elapsed:{3}", path, bytes.Length, text.Length, Time.realtimeSinceStartup - startTime);
            }
#endif
            if (text != null)
                return new AssetBundleInfo(text);
        }
        else if (type == AssetBundleType.Bytes)
        {
#if UNITY_EDITOR
            if (File.Exists(path))
                return new AssetBundleInfo(File.ReadAllBytes(path));
#else
            byte[] bytes = PlatformTools.GetFileContents(path);
            if(bytes!=null)
                return new AssetBundleInfo(bytes);
#endif
        }
        return null;
    }
}