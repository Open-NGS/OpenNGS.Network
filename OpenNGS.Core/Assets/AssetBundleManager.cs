using OpenNGS.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OpenNGS.Assets
{
    public class AssetBundleManager : MonoSingleton<AssetBundleManager>, ISingleton
    {
        public const string BundleExt = ".assets";
        static AssetBundleManifest manifest;
        private static long magic = 0;
        private bool inited = false;
        public bool LowRes = false;

        public static long Magic
        {
            get
            {
                return GetMagicNumber();
            }
        }

        Dictionary<string, AssetBundleInfo> bundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, string> dicAssetToBundle = new Dictionary<string, string>();

        public void Init()
        {
            if (!inited) LoadAssetBundleManifest();
            if (SystemInfo.systemMemorySize < 4096)
                this.LowRes = true;

            dicAssetToBundle.Clear();

#if DEBUG_LOG
            Debug.LogFormat("AssetBundleManager.Init: LowRes:{0}", this.LowRes);
#endif
        }

        private void LoadAssetBundleManifest()
        {
            string manifestFile = OpenNGS.IO.Path.Combine(GetStreamingAssetsPath(), "StreamingAssets");
#if UNITY_EDITOR
            if (!File.Exists(manifestFile))
                return;
#endif
            var bundle = AssetBundle.LoadFromFile(manifestFile);
            if (bundle != null)
            {
                manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                bundles.Clear();
                foreach (var bundlename in manifest.GetAllAssetBundles())
                {
                    bundles.Add(bundlename, null);
                }
                bundle.Unload(false);
            }
            bundle = null;
            inited = true;
        }

        public AssetBundleInfo LoadBundleByAsset(string assetPath)
        {
            string bundleName = GetBundleNameByAsset(assetPath);
#if DEBUG_LOG
            OpenNGSDebug.DebugFormat("LoadBundleByAsset:{0} - Try Load:{1}", assetPath, bundleName);
#endif
            AssetBundleInfo bundle = GetBundleInfo(bundleName);
            if (bundle == null)
            {
                OpenNGSDebug.LogErrorFormat("LoadBundleError:{0} - Try Load:{1}", assetPath, bundleName);
#if DEBUG_LOG
                OpenNGSDebug.DebugFormat("LoadBundleByAsset:{0} - Try Load:{1}", assetPath, bundleName);
#endif
            }
            return bundle;
        }
        public AssetBundleInfo LoadBundleBySceneName(string sceneName)
        {
            string bundleName = "scenes/" + sceneName.ToLower() + BundleExt;
            return LoadBundleWithDependencies(bundleName);
        }
        public AssetBundleInfo GetBundleInfo(string bundleName)
        {
            AssetBundleInfo bundleInfo = null;

            bundles.TryGetValue(bundleName, out bundleInfo);
            
            if (bundleInfo == null || bundleInfo.AssetBundle == null)
            {
                return LoadBundleWithDependencies(bundleName);
            }
            return bundleInfo;
        }
        /// <summary>
        /// 存放assetbundle文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetStreamingAssetsPath()
        {
#if UNITY_EDITOR
            return "BuildAssets/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString() + "/StreamingAssets";
#else
            return Application.streamingAssetsPath;
#endif
        }
        private string GetBundleNameByAsset(string assetPath)
        {
            assetPath = assetPath.ToLowerInvariant();
            string bundleName;
            // 避免每次查找，建立缓存
            if (!dicAssetToBundle.TryGetValue(assetPath, out bundleName))
            {
                string ext = Path.GetExtension(assetPath);
                if (string.IsNullOrEmpty(ext))
                    bundleName = assetPath + BundleExt;
                else
                    bundleName = assetPath.Replace(ext, BundleExt);

                // 按照single模式尝试加载bundle文件是否成功，如果不成功，则是folder模式
                if(bundles.ContainsKey(bundleName))
                {
                    dicAssetToBundle.Add(assetPath, bundleName);
                    return bundleName;
                }
                var bundlePath = OpenNGS.IO.Path.Combine(GetStreamingAssetsPath(), bundleName);
                bool bExists = OpenNGS.IO.FileSystem.FileExists(bundlePath);
                if (!bExists)
                {
                    string dir = Path.GetDirectoryName(assetPath);
                    bundleName = dir + BundleExt;
                }
                bundleName = bundleName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                dicAssetToBundle.Add(assetPath, bundleName);
            }
            return bundleName;
        }
        private AssetBundleInfo LoadBundleWithDependencies(string bundleName)
        {
            var dependencies = manifest.GetAllDependencies(bundleName);
            foreach (var v in dependencies)
            {
                AssetBundleInfo info = null;
                bundles.TryGetValue(v, out info);
                if(info == null || info.AssetBundle == null)
                {
                    LoadBundle(v);
                }
            }
            return LoadBundle(bundleName);
        }
        public AssetBundleInfo LoadBundle(string bundleName/*, ref AssetBundleInfo bundle, float unloadTime*/)
        {

#if DEBUG_LOG
                OpenNGSDebug.DebugFormat("LoadBundle:{0}", bundleName);
#endif

            var path = OpenNGS.IO.Path.Combine(GetStreamingAssetsPath(), bundleName);

            var assetbundle = LoadRawBundle(path);
            if (assetbundle == null)
            {
                Debug.LogError("LoadBundleError: " + bundleName);
                return null;
            }
            var bundleInfo = new AssetBundleInfo(assetbundle, float.MaxValue);
            bundles[bundleName] = bundleInfo;

            return bundleInfo;
        }

        private AssetBundle LoadRawBundle(string path)
        {
            AssetBundle assetbundle = AssetBundle.LoadFromFile(path);
            return assetbundle;
        }
        /// <summary>
        /// 加密的bundle，暂时没用到
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetBundle LoadNextBundle(string path)
        {
#if DEBUG_LOG
            OpenNGSDebug.DebugFormat("LoadNextBundle:{0}", path);
#endif
            try
            {
#if DEVELOPMENT || UNITY_SWITCH || UNITY_PS4 || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
                System.IO.FileStream steam = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
#else
                CryptoStream steam = new CryptoStream(path, Magic.ToString("X8"), FileMode.Open);
#endif
                AssetBundle assetbundle = AssetBundle.LoadFromStream(steam);
                return assetbundle;
            }
            catch (Exception ex)
            {
#if DEBUG_LOG
                Debug.LogErrorFormat("LoadNextBundle Decrypt Failed:{0} ,{1}", path, ex);
#endif
                return null;
            }
        }

        static long GetMagicNumber()
        {
            if (magic == 0)
            {
                magic = 0x5685432132698754 | 0x6654219875421325;
            }
            return magic;
        }

        public void Unload(AssetBundleInfo bundle)
        {
            if (bundle != null)
            {
                bundle.ImmediatelyUnload();
            }
        }
        public void OnCreate()
        {
        }
    }
}
