using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using OpenNGS.Build;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildPipline : IActiveBuildTargetChanged
{   
    public static string AssetBundlePath
    {
        get { return "BuildAssets/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/StreamingAssets"; }
    }
    public int callbackOrder => 0;

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        if (newTarget == BuildTarget.Android)
        {
            ForceASTC();
        }
    }

    /// <summary>
    /// 执行构建目标平台激活
    /// </summary>
    /// <returns></returns>
    public static bool ActiveTarget()
    {
        ForceASTC();
        if (PiplineSettings.BuildTarget != BuildTarget.NoTarget && PiplineSettings.BuildTarget != EditorUserBuildSettings.activeBuildTarget)
        {
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(GetBuildTargetGroupByBuildTarget(PiplineSettings.BuildTarget), PiplineSettings.BuildTarget))
            {
                Debug.LogFormat("Active Target Failed, current:{0}  target:{1}", EditorUserBuildSettings.activeBuildTarget, PiplineSettings.BuildTarget);
                if (Application.isBatchMode)
                    EditorApplication.Exit(5);//切换目标失败 
                return false;
            }
        }
        return true;
    }

    private static void ForceASTC()
    {
#if UNITY_ANDROID
        if (PiplineSettings.ForceASTC &&  Application.isBatchMode && EditorUserBuildSettings.androidBuildSubtarget != MobileTextureSubtarget.ASTC)
        {
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
            AssetDatabase.SaveAssets();
        }
#endif
    }

    [MenuItem("OpenNGS/Build/Build AssetBundles", priority = 111)]
    public static void BuildAssetBundles()
    {
        if (!ActiveTarget())
        {
            return;
        }
        Directory.CreateDirectory(AssetBundlePath);
        BuildPipeline.BuildAssetBundles(AssetBundlePath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);
    }

    #region Pipline Methods
    static void BuildWindows32()
    {
        Debug.Log("BuildWindows32 Start");
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows))
        {
            EditorApplication.Exit(5);//切换目标失败 
            return;
        }
        BuildPlayer();
    }

    static void BuildWindows64()
    {
        Debug.Log("BuildWindows64 Start");
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
        {
            EditorApplication.Exit(5);//切换目标失败 
            return;
        }
        BuildPlayer();
    }

    static void BuildAndroid()
    {
        Debug.Log("BuildWindows64 Start");
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
        {
            EditorApplication.Exit(5);//切换目标失败 
            return;
        }
        BuildPlayer();
    }


    #endregion


    /// <summary>
    /// 通过命令行参数化平台文件,仅用于发版流水线调用，需要通过命令行参数传递全部参数。
    /// <list type="table">
    /// <item><see cref="PiplineSettings.PlatformTemplatePath"/>>.template : 模板配置文件</item>
    /// <item><see cref="PiplineSettings.PlatformTemplatePath"/>>.cs : 目标输出文件</item>
    /// </list>
    /// </summary>
    [MenuItem("OpenNGS/Build/Build Active Target", priority = 101)]
    public static void BuildPlayer()
    {
        Debug.Log("BuildPlayer : " + EditorUserBuildSettings.activeBuildTarget.ToString());

        if (Application.isBatchMode)
        {
            BuildPlayerPipline();
        }
        else
        {
            BuildPlayerInternal(EditorUserBuildSettings.activeBuildTarget);
        }
        Debug.Log("BuildPlayer End");
    }


    private static void BuildPlayerPipline()
    {
        if (!ActiveTarget())
        {
            return;
        }

        if (EditorUserBuildSettings.development != (PiplineSettings.BuildType == BuildType.Debug))
        {
            EditorUserBuildSettings.development = (PiplineSettings.BuildType == BuildType.Debug);
        }

        if (PiplineSettings.PlatformTemplatePath.Length > 0) {
            var templatePath = PiplineSettings.PlatformTemplatePath.Split('.')[0] + ".template";
            var scriptPath = PiplineSettings.PlatformTemplatePath.Split('.')[0] + ".cs";
            if (System.IO.File.Exists(templatePath)) {
                if (!FileParametric.Parametric(templatePath, scriptPath, CommandLine.Arguments)) {
                    EditorApplication.Exit(4);// 流水线参数化模板文件失败
                    return;
                }
            }
            CompilationPipeline.RequestScriptCompilation();
            CompilationPipeline.compilationFinished += BuildPlayerPiplineInitSettings;
        }
        else
        {
            BuildPlayerPiplineInitSettings(null);
        }
    }

    private static void BuildPlayerPiplineInitSettings(object obj)
    {
        Debug.Log("BuildPlayerPiplineInitSettings");

        string bundleVersion = "";
        if (!string.IsNullOrEmpty(PiplineSettings.BundleVersion)) bundleVersion = PiplineSettings.BundleVersion;
        if (!PiplineSettings.BuildVersion.IsZero()) bundleVersion = PiplineSettings.BuildVersion.ToString();
        if (!string.IsNullOrEmpty(bundleVersion)) PlayerSettings.bundleVersion = bundleVersion;

        if (!string.IsNullOrEmpty(PiplineSettings.CompanyName)) PlayerSettings.companyName = PiplineSettings.CompanyName;
        if (!string.IsNullOrEmpty(PiplineSettings.ProductName)) PlayerSettings.productName = PiplineSettings.ProductName;
        if (!string.IsNullOrEmpty(PiplineSettings.AppIdentifier))
        {
            PlayerSettings.applicationIdentifier = PiplineSettings.AppIdentifier;
        }

        Debug.LogFormat("SplitOBB:{0}", PiplineSettings.SplitOBB);
        PlayerSettings.Android.useAPKExpansionFiles = PiplineSettings.SplitOBB;

        if (PiplineSettings.BuildVersionCode > 0)
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                PlayerSettings.Android.bundleVersionCode = PiplineSettings.BuildVersionCode;
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = PiplineSettings.BuildVersionCode.ToString();
            }
        }
        var report = BuildPlayerInternal(EditorUserBuildSettings.activeBuildTarget);
        EditorApplication.Exit((int)report.summary.result - 1);
    }

    /// <summary>
    /// Build Player
    /// </summary>
    static BuildReport BuildPlayerInternal(BuildTarget buildTarget)
    {
        Debug.Log("BuildPlayerInternal Start");
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = buildTarget;

        if (Application.isBatchMode)
        {
            if (!string.IsNullOrEmpty(PiplineSettings.ExtraScriptingDefines))
            {
                options.extraScriptingDefines = PiplineSettings.ExtraScriptingDefines.Split(";".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
            }

            if (!string.IsNullOrEmpty(PiplineSettings.OutputPath))
            {
                options.locationPathName = PiplineSettings.OutputPath;
            }
            else
            {
                options.locationPathName = GetDefaultOutputPath(buildTarget);
            }
        }
        else
        {
            options.locationPathName = GetDefaultOutputPath(buildTarget);
            if (EditorUserBuildSettings.development)
            {
                if (!PlayerSettings.applicationIdentifier.EndsWith(".debug")) PlayerSettings.applicationIdentifier += ".debug";
            }
            else
            {
                if (PlayerSettings.applicationIdentifier.EndsWith(".debug")) PlayerSettings.applicationIdentifier = PlayerSettings.applicationIdentifier.Remove(PlayerSettings.applicationIdentifier.Length - 6);
            }
        }

        // 优先使用流水线传入的内置场景列表，如果没有则使用默认的
        var scenes = GetCustomBuiltinScenes();
        options.scenes = scenes.Length > 0 ? scenes : GetDefaultBuiltinScenes();

        if (EditorUserBuildSettings.development)
        {
            options.options |= BuildOptions.Development;
        }
        if (EditorUserBuildSettings.allowDebugging)
        {
            options.options |= BuildOptions.AllowDebugging;
        }
        if (EditorUserBuildSettings.connectProfiler)
        {
            options.options |= BuildOptions.ConnectWithProfiler;
        }
        if (EditorUserBuildSettings.buildWithDeepProfilingSupport)
        {
            options.options |= BuildOptions.EnableDeepProfilingSupport;
        }

        #if UNITY_2022_2_OR_NEWER
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        NamedBuildTarget _nameBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
        PlayerSettings.SetIl2CppCodeGeneration(_nameBuildTarget, Il2CppCodeGeneration.OptimizeSize);
		#elif UNITY_2021_2_OR_NEWER
		EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        #endif

        Debug.LogFormat("BuildPlayer.locationPathName:{0}", options.locationPathName);
        Debug.LogFormat("BuildPlayer.bundleVersion:{0}", PlayerSettings.bundleVersion);
        return BuildPipeline.BuildPlayer(options);
    }
    /// <summary>
    /// 流水线传入的需要内置的场景
    /// </summary>
    /// <returns></returns>
    static string[] GetCustomBuiltinScenes()
    {
        if (!string.IsNullOrEmpty(PiplineSettings.BuiltinScenes))
        {
            return PiplineSettings.BuiltinScenes.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        }

        return new string[0];
    }
    /// <summary>
    /// 在BuildSetting里为Enable并且没有设置BundleName的场景
    /// </summary>
    /// <returns></returns>
    static string[] GetDefaultBuiltinScenes()
    {
        List<string> scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;

            var importer = AssetImporter.GetAtPath(scene.path);
            if (importer != null)
            {
                if (string.IsNullOrEmpty(importer.assetBundleName))
                {
                    scenes.Add(scene.path);
                }
            }
        }
        return scenes.ToArray();
    }
    static string GetDefaultOutputPath(BuildTarget buildTarget)
    {
        string buildType = EditorUserBuildSettings.development ? "debug" : "release";
        string suffix = EditorUserBuildSettings.development ? "_debug" : "";
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return $"Build/{buildTarget}/{buildType}/Game.exe";
            case BuildTarget.Android:
                return $"Build/{buildTarget}/{buildType}/{PlayerSettings.applicationIdentifier}_{PlayerSettings.bundleVersion}{suffix}.apk";
            case BuildTarget.iOS:
                return $"Build/{buildTarget}/{buildType}/{ PlayerSettings.productName }";
            default:
                return $"Build/{buildTarget}/{buildType}/{ PlayerSettings.productName }";
        }
    }
    public static BuildTargetGroup GetBuildTargetGroupByBuildTarget(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return BuildTargetGroup.Standalone;
            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;
            case BuildTarget.PS5:
                return BuildTargetGroup.PS5;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.GameCoreXboxOne:
                return BuildTargetGroup.GameCoreXboxOne;
            case BuildTarget.GameCoreXboxSeries:
                return BuildTargetGroup.GameCoreXboxSeries;
#if UNITY_2022_2_OR_NEWER
            case BuildTarget.EmbeddedLinux:
                return BuildTargetGroup.EmbeddedLinux;
#else
            case BuildTarget.Lumin:
                return BuildTargetGroup.Lumin;
#endif
#if UNITY_2021_3_OR_NEWER
            case BuildTarget.LinuxHeadlessSimulation:
                return BuildTargetGroup.LinuxHeadlessSimulation;
#endif
            case BuildTarget.Stadia:
                return BuildTargetGroup.Stadia;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;
            default:
                return BuildTargetGroup.Unknown;
        }
    }
}