using UnityEngine;

// 
// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
// 
namespace Neptune.Assets
{
    /// <summary>
    /// WWWAssetLoader
    /// asset loader by WWW
    /// </summary>
    //public class WWWAssetLoader : IAssetLoader
    //{
    //    private bool isLoadFromCache = false;
    //    private int assetBundleVersion = 0;

    //    public WWWAssetLoader(bool loadFromCache, int bundleVersion)
    //    {
    //        isLoadFromCache = loadFromCache;
    //        assetBundleVersion = bundleVersion;
    //    }

    //    /// <summary>
    //    /// LoadBundle
    //    /// load a bundle 
    //    /// </summary>
    //    /// <param name="path">bundle relative path</param>
    //    /// <param name="type">bundle type</param>
    //    /// <returns></returns>
    //    public AssetBundleInfo LoadBundle(string path, AssetBundleType type)
    //    {
    //        //UnityEngine.Debug.LogFormat("LoadBundleAsync : {0}", path);

    //        WWW req = null;
    //        if (isLoadFromCache && type == AssetBundleType.AssetBundle)
    //        {
    //            // Debug.Log("Load From Cache for Path: " + path + ", Bundle Version: " + assetBundleVersion);
    //            req = WWW.LoadFromCacheOrDownload(path, assetBundleVersion);

    //        }
    //        else
    //        {
    //            req = new WWW(path);
    //        }

    //        AssetBundleInfo bundleInfo = new AssetBundleInfo(req, type);
    //        return bundleInfo;
    //    }
    //}
}