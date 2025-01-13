//using System;
//using UnityEngine;
//using UnityEngine.Events;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//// 
//// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
//// 

//namespace Neptune.Assets
//{

//    /// <summary>
//    /// AssetBundleManager
//    /// 管理 Bundle 的创建、缓冲
//    /// </summary>
//    public class AssetBundleManager : BehaviourSingleton<AssetBundleManager>
//    {
//        private static string CurrentLanguage;
//        private static string DefaultLanguage;

//        public static void SetCurrentLanguage(string currengLang, string defaultLang)
//        {
//            CurrentLanguage = currengLang;
//            DefaultLanguage = defaultLang;
//        }

//        public void Init()
//        {
//            // 沒有任何用途，保證可以初始化成功
//        }

//        const string streamingAssetsManifestFile = "StreamingAssets";
//        private AssetBundleManifest assetBundleManifest = null;

//        /// <summary>
//        /// Get AssetBundleManifest
//        /// </summary>
//        public AssetBundleManifest Manifest { get { return assetBundleManifest; } }

//        Dictionary<string, AssetBundleInfo> m_AllBundles = new Dictionary<string, AssetBundleInfo>();

//        List<AssetBundleInfo> m_DelayUnloadBundles = new List<AssetBundleInfo>();

//        public static AssetBundleLoadMode LoadMode;
//        public static bool LoadFromCache = false;
//        public static int BundleVersion = 0;

//        /// <summary>
//        /// Load Limiter
//        /// </summary>
//        public static ILoadLimiter LoadLimiter = null;

//        IAssetLoader bundleLoader;
//        WWWAssetLoader wwwLoader = new WWWAssetLoader(LoadFromCache, BundleVersion);

//        private bool isLoadingManifest = false;
//        public bool isReady = false;

//        private Dictionary<string, bool> RetainBundleList = new Dictionary<string, bool>();

//        public IAssetLoader BundleLoader
//        {
//            get
//            {
//                if (bundleLoader == null)
//                {
//                    switch (LoadMode)
//                    {
//                        case AssetBundleLoadMode.LoadFromFile:
//                            bundleLoader = new FileAssetLoader();
//                            break;
//                        case AssetBundleLoadMode.LoadFromFileAsync:
//                            bundleLoader = new FileAssetLoader(true);
//                            break;
//                        case AssetBundleLoadMode.LoadFromWWW:
//                            bundleLoader = new WWWAssetLoader(LoadFromCache, BundleVersion);
//                            break;
//                    }
//                }
//                return bundleLoader;
//            }
//        }

//        private static string m_streamingAssetsPath;
//        public static string streamingAssetsPath
//        {
//            get
//            {
//                if (m_streamingAssetsPath == null)
//                {
//                    m_streamingAssetsPath = GetStreamingAssetsPath();
//                }
//                return m_streamingAssetsPath;
//            }
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        public Dictionary<string, AssetBundleInfo> AllBundles
//        {
//            get
//            {
//                return m_AllBundles;
//            }

//            set
//            {
//                m_AllBundles = value;
//            }
//        }
//        static AssetBundleManager()
//        {
//            AssetBundleManager.NeedDestroyOnRestart = true;
//        }

//        static string GetStreamingAssetsPath()
//        {
//            string path = Application.streamingAssetsPath + "/";
//#if UNITY_EDITOR || !UNITY_ANDROID
//            if (LoadMode == AssetBundleLoadMode.LoadFromWWW)
//            {
//#if UNITY_IPHONE
//            path = "file://" + path;
//#else
//                path = "file:///" + path;
//#endif
//            }

//#endif
//            return path;
//        }

//        public void UnloadBundles(string[] removeList)
//        {
//            foreach (string bundlerName in removeList)
//            {
//                if (m_AllBundles.ContainsKey(bundlerName) && m_AllBundles[bundlerName] != null)
//                {
//                    if (m_AllBundles[bundlerName].assetBundle != null)
//                    {
//                        m_AllBundles[bundlerName].assetBundle.Unload(true);
//                    }
//                    m_AllBundles[bundlerName] = null;
//                }
//            }
//        }

//        public void UnloadBundle(string bundlerName)
//        {
//            if (m_AllBundles.ContainsKey(bundlerName) && m_AllBundles[bundlerName] != null)
//            {
//                if (m_AllBundles[bundlerName].assetBundle != null)
//                {
//                    m_AllBundles[bundlerName].assetBundle.Unload(true);
//                }
//                m_AllBundles[bundlerName] = null;
//            }
//        }

//        public IEnumerator LoadManifest()
//        {
//            isLoadingManifest = true;
//            // Read the global StreamingAsset dependency info first
//            if (assetBundleManifest == null)
//            {
//#if UNITY_EDITOR
//                string manifestBundleName = "BuildAssets/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString() + "/StreamingAssets/" + streamingAssetsManifestFile;
//#else
//                string manifestBundleName = streamingAssetsPath + streamingAssetsManifestFile;
//#endif
//                Debug.Log("LoadManifest:" + manifestBundleName);
//                if (!string.IsNullOrEmpty(manifestBundleName))
//                {
//                    AssetBundleInfo bundleInfo = BundleLoader.LoadBundle(manifestBundleName, AssetBundleType.AssetBundle);
//                    yield return bundleInfo;

//                    if (bundleInfo.assetBundle != null)
//                    {
//                        AssetBundle assetBundle = bundleInfo.assetBundle;
//                        assetBundleManifest = (AssetBundleManifest)assetBundle.LoadAsset("assetbundlemanifest");
//                        string[] allbundles = assetBundleManifest.GetAllAssetBundles();
//                        for (int i = 0; i < allbundles.Length; i++)
//                        {
//                            this.m_AllBundles[allbundles[i]] = null;
//                        }
//                        assetBundle.Unload(false);
//                    }
//                    else
//                    {
//                        Debug.LogError("Load Manifest Failed : " + manifestBundleName);
//                    }
//                    isReady = true;
//                }
//            }
//            isLoadingManifest = false;
//        }

//        IEnumerator Start()
//        {
//            if (!isLoadingManifest)
//            {
//                yield return LoadManifest();
//            }
//            else
//            {
//                while (isLoadingManifest)
//                {
//                    yield return null;
//                }
//            }
//        }

//        void FixedUpdate()
//        {
//            for (int i = m_DelayUnloadBundles.Count - 1; i >= 0; i--)
//            {
//                AssetBundleInfo bundle = m_DelayUnloadBundles[i];
//                if (bundle.refCount == 0)
//                {
//                    if (bundle.LifeTime <= 0)
//                    {
//                        ImmediateUnload(bundle);
//                        m_DelayUnloadBundles.RemoveAt(i);
//                    }
//                    else
//                    {
//                        bundle.LifeTime -= Time.fixedDeltaTime;
//                    }
//                }
//            }
//        }

//        public void AddRetainBundleName(string bundlename)
//        {
//            if (RetainBundleList.ContainsKey(bundlename))
//            {
//                return;
//            }
//            RetainBundleList.Add(bundlename, true);
//        }

//        public bool Unload(AssetBundleInfo bundle, bool delay)
//        {
//            if (bundle != null && !(RetainBundleList != null && RetainBundleList.ContainsKey(bundle.name)))
//            {
//                //Debug.LogFormat("AssetBundle Unload:{0}", bundle.name);
//                if (delay)
//                {
//                    if (!bundle.name.StartsWithFast("ui/"))
//                        this.m_DelayUnloadBundles.Add(bundle);
//                }
//                else
//                {
//                    ImmediateUnload(bundle);
//                }

//                return true;
//            }
//            return false;
//        }

//        public void ClearBundle(AssetBundleInfo bundle)
//        {
//            ImmediateUnload(bundle);
//        }

//        private void ImmediateUnload(AssetBundleInfo bundle)
//        {
//            //Debug.LogFormat("AssetBundle ImmediateUnload:{0}", bundle.name);
//            this.m_AllBundles[bundle.name] = null;
//            bundle.ImmediateUnload();
//        }

//        public AssetBundleInfo LoadBundle(string name, AssetBundleType type, bool localized = false)
//        {
//#if DEVELOPMENT
//        UFProfiler.UIProfilerData profiler = null;
//        if (UFProfiler.indent > 0)
//            profiler = UFProfiler.Start(string.Format("AssetBundleInfo.LoadBundle({0}; {1})", name, type));
//#endif
//            AssetBundleInfo bundleinfo = null;
//            string[] allDepends = null;
//            string _ext = "";
//            if(type == AssetBundleType.Text)
//            {
//                _ext = ".txt";
//            }
//            else
//            {
//                _ext = ".assets";
//            }
//            string bundleName = GetBundleFilename(name, type, localized,_ext);
//            if (!string.IsNullOrEmpty(bundleName))
//            {
//                if (type == AssetBundleType.Texture)
//                {//Load Texture
//                    AssetBundleInfo bundle = wwwLoader.LoadBundle(streamingAssetsPath + bundleName, type);
//#if DEVELOPMENT
//                UFProfiler.End(profiler);
//#endif
//                    return bundle;
//                }
//                if (type == AssetBundleType.AssetBundle)
//                {

//                    if (!m_AllBundles.TryGetValue(bundleName, out bundleinfo))
//                    {
//#if DEVELOPMENT
//                    UFProfiler.End(profiler);
//#endif
//                        return null;
//                    }
//                    else
//                    {
//                        allDepends = assetBundleManifest.GetAllDependencies(bundleName);
//                    }
//                }
//                if (bundleinfo == null)
//                {
//#if UNITY_EDITOR
//                    string AssetBundlePath = "BuildAssets/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString() + "/StreamingAssets/" + bundleName;
//                    if (File.Exists(AssetBundlePath))
//                    {
//                        bundleinfo = BundleLoader.LoadBundle(AssetBundlePath, type);
//                    }
//#endif
//                    if(bundleinfo==null)
//                        bundleinfo = BundleLoader.LoadBundle(streamingAssetsPath + bundleName, type);

//                    m_AllBundles[bundleName] = bundleinfo;
//                    if (bundleinfo != null)
//                        bundleinfo.name = bundleName;
//                }
//                if (bundleinfo != null)
//                {
//                    if (allDepends != null)
//                    {
//                        for (int i = 0; i < allDepends.Length; i++)
//                        {
//                            AssetBundleInfo dep = LoadBundle(allDepends[i], type);
//                            if (dep != null)
//                                bundleinfo.AddDependencies(dep);
//                        }
//                    }
//                    bundleinfo.refCount++;
//                }
//            }
//#if DEVELOPMENT
//        UFProfiler.End(profiler);
//#endif
//            return bundleinfo;
//        }




//        public void LoadBundle(string name, UnityAction<AssetBundleInfo> onLoadBundle, bool localized = false)
//        {
//            BundleLoadAction action = new BundleLoadAction(onLoadBundle);
//            if (LoadMode == AssetBundleLoadMode.LoadFromFile)
//            {
//                AssetBundleInfo bundleInfo = LoadBundle(name, AssetBundleType.AssetBundle, localized);
//                action.OnLoadBundle(bundleInfo);
//            }
//            else
//                StartCoroutine(LoadBundleRoutine(name, AssetBundleType.AssetBundle, action, localized));
//        }

//        public void LoadText(string name, UnityAction<string> onLoadText)
//        {
//            //Debug.LogFormat("LoadText : {0}", name);
//            BundleLoadAction action = new BundleLoadAction(onLoadText);
//            if (LoadMode == AssetBundleLoadMode.LoadFromFile)
//            {
//                AssetBundleInfo bundleInfo = LoadBundle(name, AssetBundleType.Text);
//                action.OnLoadBundle(bundleInfo);
//            }
//            else
//                StartCoroutine(LoadBundleRoutine(name, AssetBundleType.Text, action));
//        }

//        public void LoadBytes(string name, UnityAction<byte[]> onLoadBytes)
//        {
//            //Debug.LogFormat("LoadBytes : {0}", name);
//            BundleLoadAction action = new BundleLoadAction(onLoadBytes);
//            if (LoadMode == AssetBundleLoadMode.LoadFromFile)
//            {
//                AssetBundleInfo bundleInfo = LoadBundle(name, AssetBundleType.Bytes);
//                action.OnLoadBundle(bundleInfo);
//            }
//            else
//                StartCoroutine(LoadBundleRoutine(name, AssetBundleType.Bytes, action));
//        }

//        public void LoadTexture(string name, UnityAction<Texture2D> onLoadTexture)
//        {
//            //Debug.LogFormat("LoadTexture : {0}", name);
//            BundleLoadAction action = new BundleLoadAction(onLoadTexture);
//            if (LoadMode == AssetBundleLoadMode.LoadFromFile)
//            {
//                AssetBundleInfo bundleInfo = LoadBundle(name, AssetBundleType.Texture);
//                action.OnLoadBundle(bundleInfo);
//            }
//            else
//                StartCoroutine(LoadBundleRoutine(name, AssetBundleType.Texture, action));
//        }

//        private static int RoutineLoadCount = 0;

//        public static int GetRoutineLoadCount()
//        {
//            return RoutineLoadCount;
//        }

//        public static int ResetRoutineLoadCount()
//        {
//            return RoutineLoadCount = 0;
//        }
//        private IEnumerator LoadBundleRoutine(string bundlename, AssetBundleType type, BundleLoadAction action, bool localized = false)
//        {
//            RoutineLoadCount++;
//            AssetBundleInfo bundleInfo = LoadBundle(bundlename, type, localized);
//            if ((type == AssetBundleType.AssetBundle && LoadMode != AssetBundleLoadMode.LoadFromFile) || LoadMode == AssetBundleLoadMode.LoadFromWWW)
//                yield return bundleInfo;
//            while (LoadLimiter != null && !LoadLimiter.CanLoad)
//                yield return null;

//            if (action != null)
//                action.OnLoadBundle(bundleInfo);
//            RoutineLoadCount--;
//        }

//        public static string GetBundleFilename(string resource, AssetBundleType type, bool localized = false, string ext = ".assets")
//        {
//            string extension = Path.GetExtension(resource);
//            if (!string.IsNullOrEmpty(extension))
//            {
//                ext = extension;
//                resource = resource.Replace(ext, "");
//            }
//            resource = resource.ToLower();
//            string bundlename = resource + ext;
//            //Debug.LogFormat("GetBundleFilename:{0} > {1}", resource, bundlename);
//            if (type == AssetBundleType.AssetBundle)
//            {
//                if (localized)
//                {
//                    int delimiter = bundlename.LastIndexOf('/') + 1;
//                    string path = bundlename.Substring(0, delimiter);
//                    string file = bundlename.Substring(delimiter);

//                    string localBundlename = path + CurrentLanguage.ToLower() + "/" + file;
//                    if (AssetBundleManager.Instance.m_AllBundles.ContainsKey(localBundlename))
//                        return localBundlename;

//                    localBundlename = path + DefaultLanguage.ToLower() + "/" + file;
//                    if (AssetBundleManager.Instance.m_AllBundles.ContainsKey(localBundlename))
//                        return localBundlename;
//                }

//                if (QualityUtil.QualityLevel == QualityLevel.Low)
//                {
//                    string bundlename_lod = resource + "_lod2" + ext;
//                    if (AssetBundleManager.Instance.m_AllBundles.ContainsKey(bundlename_lod))
//                        return bundlename_lod;
//                }
//                //if (AssetBundleManager.Instance.m_AllBundles.ContainsKey(bundlename))
//                return bundlename;
//            }
//            return bundlename;
//        }

//        public override void Destroy()
//        {
//            foreach (AssetBundleInfo assetBundleInfo in m_AllBundles.Values)
//            {
//                if (assetBundleInfo != null && assetBundleInfo.assetBundle != null)
//                {
//                    assetBundleInfo.assetBundle.Unload(false);
//                }
//            }
//            assetBundleManifest = null;
//            isReady = false;

//            base.Destroy();
//        }
//    }
//}