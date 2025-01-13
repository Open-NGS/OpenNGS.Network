using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using OpenNGS.Assets;
using System.Collections.Generic;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

public class BuildTool : MonoBehaviour
{

    #region Public Methods

    /// <summary>
    /// 用于Batch Mode调用，如果编译未报错正常运行，则返回9.
    /// </summary>
    public static void TestCompile()
    {
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(9);
        }
    }

    public static void SwitchAndroidPlatform()
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
    }

    #endregion
    [Serializable]
    public class AssetBundleBuildInfoItem
    {
        [SerializeField]
        public string BundleName;
        [SerializeField]
        public List<string> AssetsName = new List<string>();
        [SerializeField]
        public List<string> Dependencies = new List<string>();
    }
    [Serializable]
    public class AssetBundleBuildInfo
    {
        [SerializeField]
        public List<AssetBundleBuildInfoItem> Bundles = new List<AssetBundleBuildInfoItem>();
        public void Clear()
        {
            Bundles.Clear();
            var file = GetJsonPath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
        public void AddAssetToBundle(string assetName, string bundleName)
        {
            assetName = NormalizePath(assetName);
            bundleName = NormalizePath(bundleName);
            bool bFound = false;
            foreach (var v in Bundles)
            {
                if (v.BundleName == bundleName)
                {
                    bFound = true;
                    v.AssetsName.Add(assetName);
                    break;
                }
            }

            if (!bFound)
            {
                var item = new AssetBundleBuildInfoItem();
                item.BundleName = bundleName;
                item.AssetsName.Add(assetName);
                Bundles.Add(item);
            }
        }
        public void WriteDependencies()
        {
            string manifestBundle = BuildPipline.AssetBundlePath + "/StreamingAssets";
            AssetBundle bundle = AssetBundle.LoadFromFile(manifestBundle);
            if (bundle == null)
                return;

            var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            foreach (var v in Bundles)
            {
                var depend = manifest.GetAllDependencies(v.BundleName);
                v.Dependencies.Clear();
                v.Dependencies.AddRange(depend);
            }
            bundle.Unload(true);
        }
        public void SaveToJson()
        {
            WriteDependencies();
            var json = EditorJsonUtility.ToJson(this, true);
            var file = GetJsonPath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            StreamWriter sw = new StreamWriter(file);
            sw.Write(json);
            sw.Flush();
            sw.Close();
            Bundles.Clear();
        }
        private string GetJsonPath()
        {
            var path = Application.dataPath;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, BuildPipline.AssetBundlePath);
            return path + "/Bundles.json";
        }
    }
    static public AssetBundleBuildInfo assetBundleBuildInfo = new AssetBundleBuildInfo();

    /// <summary>
    /// Update build settings including player settings and build tools.
    /// Command line arguments rule: -sdk SDK_PATH -ndk NDK_PATH -jdk JDK_PATH -gradle GRADLE_PATH]
    /// </summary>
    public static void UpdateBuildSettings()
    {

        PlayerSettings.companyName = "Tencent";

        PlayerSettings.colorSpace = ColorSpace.Linear;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
#if UNITY_ANDROID
        Debug.Log("ndkRoot");
        var commandLineArgs = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < commandLineArgs.Length; i++) {
            var commandLineArg = commandLineArgs[i];
            switch (commandLineArg) {
                case "-sdk": {
                    var pathArgs = commandLineArgs[++i];
                    AndroidExternalToolsSettings.sdkRootPath = pathArgs;
                    break;
                }
                case "-jdk": {
                    var pathArgs = commandLineArgs[++i];
                    AndroidExternalToolsSettings.jdkRootPath = pathArgs;
                    break;
                }
                case "-ndk": {
                    var pathArgs = commandLineArgs[++i];
                    AndroidExternalToolsSettings.ndkRootPath = pathArgs;
                    break;
                }
                case "-gradle": {
                    var pathArgs = commandLineArgs[++i];
                    AndroidExternalToolsSettings.gradlePath = pathArgs;
                    break;
                }
                default: {
                    continue;
                }
            }
        }
#endif

    }
    /// <summary>
    /// 通过命令行参数化编译资源
    /// <para>needClean => true/false 是否删除已构建资源</para>
    /// </summary>
    [MenuItem("OpenNGS/Build/SetupAndBuildAssetsBundle", priority = 113)]
    public static void SetupAndBuildAssetsBundle()
    {
        AssetProcessor.ResetBundles();
        AssetProcessor.SetupBundles();
        BuildPipline.BuildAssetBundles();
    }

    [MenuItem("OpenNGS/Build/Clean AssetBundles", priority = 112)]

    public static void ClearAssetBundles()
    {
        var clientPath = Path.GetDirectoryName(Application.dataPath);
        var bundlePath = Path.Combine(clientPath, BuildPipline.AssetBundlePath).Replace("\\", "/");
        // 删除assetbundle
        string[] bundleDir = { "/apps", "/characters", "/objects", "/projectsettings", "/scenes", "/skybox", "/theme", "/ui", "/audio" };
        foreach (var s in bundleDir)
        {
            var dir = bundlePath + s;
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        // 删除manifest文件
        var manifestFile = bundlePath + "/StreamingAssets";
        if (File.Exists(manifestFile))
        {
            File.Delete(manifestFile);
        }
    }

    #region Obsolete Methods

    [Obsolete("Use BuildPipline instead")]
    public static void SwitchPlatform()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        }
    }

    /// <summary>
    /// Start to build oculus apk, called by extern script.
    /// Command line arguments rule: -scenes {scene1;scene2;...} -target {target_file_path} -bundle-version {VERSION} -build-type {BUILD_TYPE}
    /// </summary>
    [Obsolete("Use BuildPipline instead")]
    public static void StartBuildOculus()
    {

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            target = BuildTarget.Android,
            locationPathName = "build/temp.apk"
        };

        var commandLineArgs = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < commandLineArgs.Length; i++)
        {
            var commandLineArg = commandLineArgs[i];
            switch (commandLineArg)
            {
                case "-scenes":
                    {
                        var sceneArg = commandLineArgs[++i];
                        buildPlayerOptions.scenes = sceneArg.Split(";");
                        break;
                    }
                case "-target":
                    {
                        var fileArg = commandLineArgs[++i];
                        buildPlayerOptions.locationPathName = fileArg;
                        break;
                    }
                case "-bundle-version":
                    {
                        var versionArg = commandLineArgs[++i];
                        PlayerSettings.bundleVersion = versionArg;
                        break;
                    }
                case "-build-type":
                    {
                        var versionArg = commandLineArgs[++i];
                        switch (versionArg)
                        {
                            case "Development":
                                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                                break;
                            case "Debug":
                                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Debug;
                                EditorUserBuildSettings.allowDebugging = true;
                                break;
                            case "Release":
                                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
                                break;
                            default:
                                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Development;
                                EditorUserBuildSettings.allowDebugging = true;
                                break;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;


        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    public static string NormalizePath(string path)
    {
        path = path.Replace("\\", "/").ToLower();
        return path;
    }
    /// <summary>
    /// 用户自定义的AssetBundle构建过程，目前xface使用的
    /// </summary>
    [MenuItem("OpenNGS/Build/NiBuild Custom AssetBundle(xFace etc)")]
    public static void StartUserCustomAssetBundleBuild()
    {
        BuildPipeline.BuildAssetBundles(BuildPipline.AssetBundlePath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    #endregion

}
