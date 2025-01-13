using System.IO;
using UnityEngine;
using UnityEditor;

class AssetProcessor : AssetPostprocessor
{

    private const string BuildPath = "Assets/Game/BuildAssets";
    private const string AssetPath = "Assets/Game/Arts";
    private const string AppPath = "Assets/Apps";

    private const string SkeletonDataSuffix = "_SkeletonData";

    public enum BundleMode
    {
        /// <summary>
        /// Single file per bundle
        /// </summary>
        Single,
        /// <summary>
        /// All files in folder per bundle
        /// </summary>
        Folder,

        Clear,
    }

    public enum BuildVariant
    {
        None = 0,
        Hi = 1,
        Low = 2,
    }

    /// <summary>
    /// 为资源meta文件设置AssetBundle信息
    /// </summary>
    [MenuItem("OpenNGS/Build/Setup AssetBundles", priority = 130)]
    public static void SetupBundles()
    {
        EditorUtility.DisplayProgressBar("Setup Asset Bundles", "Prepare...", 0f);
        AssetDatabase.RemoveUnusedAssetBundleNames();
        BuildTool.assetBundleBuildInfo.Clear();
        AssetDatabase.StartAssetEditing();

        SetupAssetBundleFolder(BuildPath + "/UI/View", BundleMode.Folder);

        AssetDatabase.StopAssetEditing();
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// 清空资源meta文件设置的AssetBundle信息
    /// </summary>
    [MenuItem("OpenNGS/Build/Reset AssetBundles", priority = 131)]
    public static void ResetBundles()
    {
        EditorUtility.DisplayProgressBar("Reset Asset Bundles", "Prepare...", 0f);

        AssetDatabase.StartAssetEditing();
        SetupAssetBundleFolder(BuildPath, BundleMode.Clear);
        ClearBuildScenes(); // 清除场景bundle信息
        AssetDatabase.StopAssetEditing();
        AssetDatabase.RemoveUnusedAssetBundleNames();
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
    }
    public static void SetupSceneAssetBundle(string scenePath)
    {
        string bundlePath = "scenes/" + Path.GetFileNameWithoutExtension(scenePath).ToLower() + OpenNGS.Assets.AssetBundleManager.BundleExt;
        var importer = AssetImporter.GetAtPath(scenePath);
        if (importer != null)
        {
            importer.assetBundleName = bundlePath;
        }
        BuildTool.assetBundleBuildInfo.AddAssetToBundle(importer.assetPath, importer.assetBundleName);
    }
    public static void SetupAssetBundleFolder(string folder, BundleMode mode, BuildVariant variant = BuildVariant.None)
    {
        string[] files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
        float total = files.Length;
        float i = 0;
        foreach (string str in files)
        {
            if (Directory.Exists(str)) continue;
            var ext = Path.GetExtension(str).ToLower();
            if (ext == ".meta" || ext == ".cs" || ext == ".unity")
                continue;
            string file = str.Replace("\\", "/");
            SetAssetBundle(file, mode, true, variant);
            i++;
            EditorUtility.DisplayProgressBar("Setup Asset Bundles", file.Replace(BuildPath, ""), i / total);
        }
    }
    /// <summary>
    /// Client需要打成Bundle的场景，排除启动场景和小程序的场景
    /// </summary>
    /// <returns></returns>
    //[MenuItem("OpenNGS/Build/Setup Build Scene AssetBundles")]
    //public static void SetupBuildScenes()
    //{
    //    List<string> buildScenes = new List<string>();
    //    string appPath = AppPath;
    //    for (int i = 1; i < EditorBuildSettings.scenes.Length; i++)
    //    {
    //        var scene = EditorBuildSettings.scenes[i];
    //        if (!scene.enabled)
    //            continue;

    //        if (!string.IsNullOrEmpty(appPath))
    //        {
    //            if (!scene.path.StartsWith(appPath, System.StringComparison.InvariantCultureIgnoreCase))
    //            {
    //                buildScenes.Add(scene.path);
    //            }
    //        }
    //        else
    //        {
    //            buildScenes.Add(scene.path);
    //        }
    //    }

    //    foreach (var scenePath in buildScenes)
    //    {
    //        string bundlePath = "scenes/" + Path.GetFileNameWithoutExtension(scenePath).ToLower() + OpenNGS.Assets.AssetBundleManager.BundleExt;
    //        var importer = AssetImporter.GetAtPath(scenePath);
    //        importer.assetBundleName = bundlePath;
    //    }
    //}
    //[MenuItem("OpenNGS/Build/Clear Build Scene AssetBundles")]
    public static void ClearBuildScenes()
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            var importer = AssetImporter.GetAtPath(scene.path);
            importer.assetBundleName = null;
        }
    }
    static void Cleanup()
    {
        string[] files = Directory.GetFiles(BuildPath + "/Spine", "*.prefab", SearchOption.AllDirectories);
        float total = files.Length;
        float i = 0;
        AssetDatabase.StartAssetEditing();
        foreach (string str in files)
        {
            string file = str.Replace("\\", "/");
            string filename = Path.GetFileNameWithoutExtension(file);
            string src = Path.GetDirectoryName(file.Replace(BuildPath, AssetPath)) + "/" + filename + "/" + filename + SkeletonDataSuffix + ".asset";
            if (!File.Exists(src))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            i++;
            EditorUtility.DisplayProgressBar("Generate Spine Prefabs", file.Replace(AssetPath, ""), i / total);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
    }

    static void SetAssetBundle(string file, BundleMode mode, bool reimport = false, BuildVariant variant = BuildVariant.None)
    {
        string bundlePath = System.IO.Path.GetDirectoryName(file.Replace(BuildPath + "/", "")).ToLower();
        if (mode == BundleMode.Folder)
        {
            bundlePath = bundlePath + OpenNGS.Assets.AssetBundleManager.BundleExt;
        }
        else if (mode == BundleMode.Single)
        {
            bundlePath = bundlePath + "/" + Path.GetFileNameWithoutExtension(file) + OpenNGS.Assets.AssetBundleManager.BundleExt;
        }
        else
        {
            bundlePath = "";
        }
        var importer = AssetImporter.GetAtPath(file);
        if (importer == null) return;

        bool changed = false;
        if (file.EndsWith("String.xml"))
        {
            bundlePath = "";
        }

        bundlePath = bundlePath.Replace("-lo/", "/");
        if (string.IsNullOrEmpty(bundlePath))
        {
            if (!string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = null;
                changed = true;
            }
        }
        else
        {
            if (importer.assetBundleName != bundlePath)
            {
                importer.assetBundleName = bundlePath;
                changed = true;
            }
        }

        string variantName = "";
        if (variant == BuildVariant.Hi)
            variantName = "hi";
        if (variant == BuildVariant.Low)
            variantName = "lo";

        if (importer.assetBundleVariant != variantName)
        {
            importer.assetBundleVariant = variantName;
            changed = true;
        }

        if (reimport && changed)
            importer.SaveAndReimport();

        BuildTool.assetBundleBuildInfo.AddAssetToBundle(importer.assetPath, importer.assetBundleName);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }

    static string GetPrefabPath(string name)
    {
        return Path.GetDirectoryName(name.Replace(AssetPath, BuildPath)) + ".prefab";
    }
}
