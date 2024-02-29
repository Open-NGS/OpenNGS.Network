// 
// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
// 
/// <summary>
/// IAssetLoader
/// asset load interface
/// </summary>
public interface IAssetLoader
{
    /// <summary>
    /// LoadBundle
    /// load a bundle 
    /// </summary>
    /// <param name="path">bundle relative path</param>
    /// <param name="type">bundle type</param>
    /// <returns></returns>
    AssetBundleInfo LoadBundle(string path, AssetBundleType type);
}