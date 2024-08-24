using OpenNGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneManager : MonoSingleton<SceneManager>, ISingleton
{
    void ISingleton.OnCreate()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private readonly Dictionary<string, ILoadingProcessor> m_LoadingDic = new Dictionary<string, ILoadingProcessor>();

    public void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, bool allowSceneActivation, ILoadingProcessor processor)
    {
        if (m_LoadingDic.ContainsKey(sceneName))
        {
            NgDebug.LogError($"NiSceneManager is Loading Same Scene ! SceneName is {sceneName}");
            return;
        }

        m_LoadingDic.Add(sceneName, processor);
        LoadSceneAsync(sceneName, loadSceneMode, allowSceneActivation);
    }

    public static void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, bool allowSceneActivation)
    {
        Instance.StartCoroutine(Instance.DoSceneLoadAsync(sceneName, loadSceneMode, allowSceneActivation));
    }

    public static void LoadSceneAsync(string sceneName)
    {
        LoadSceneAsync(sceneName, LoadSceneMode.Single, false);
    }



    public static void LoadScene(string sceneName)
    {
        LoadScene(sceneName, LoadSceneMode.Single);
    }

    public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode, ILoadingProcessor processor = null)
    {
        Instance.StartCoroutine(Instance.DoSceneLoad(sceneName, loadSceneMode, processor));
    }


    public void UnloadSceneAsync(string sceneName, Action finishCallback = null)
    {
        StartCoroutine(DoSceneUnloadAsync(sceneName, finishCallback));
    }

    private void OnSceneLoaded(string sceneName, Action makeSceneActiveCallback)
    {
        OpenNGS.Profiling.ProfilerLog.Start("NiSceneManager.OnSceneLoaded", sceneName);
        if (m_LoadingDic.TryGetValue(sceneName, out var processor))
        {
            processor.OnUnitySceneLoaded(makeSceneActiveCallback);
        }
        else
        {
            makeSceneActiveCallback?.Invoke();
        }
        OpenNGS.Profiling.ProfilerLog.End("NiSceneManager.OnSceneLoaded", sceneName);
    }

    private void OnSceneActive(string sceneName)
    {
        if (m_LoadingDic.TryGetValue(sceneName, out var processor))
        {
            processor.OnUnitySceneActive();
            m_LoadingDic.Remove(sceneName);
        }
    }

    private void OnSceneLoadingProcess(string sceneName, float process)
    {
        if (m_LoadingDic.TryGetValue(sceneName, out var precessor))
        {
            precessor.OnUnitySceneLoadingProcess(process);
        }
    }

    private IEnumerator DoSceneLoad(string sceneName, LoadSceneMode loadSceneMode, ILoadingProcessor processor = null)
    {
        OpenNGS.Assets.AssetLoader.LoadScene(sceneName, loadSceneMode);
        if (processor != null)
        {
            processor.OnUnitySceneLoaded(null);
            yield return null;
            processor.OnUnitySceneActive();
        }
    }

    private IEnumerator DoSceneLoadAsync(string sceneName, LoadSceneMode loadSceneMode, bool allowSceneActivation)
    {
        OpenNGS.Profiling.ProfilerLog.Start("NiSceneManager.DoSceneLoadAsync", sceneName);
        var asyncOperation = OpenNGS.Assets.AssetLoader.LoadSceneAsync(sceneName, loadSceneMode);
        asyncOperation.allowSceneActivation = allowSceneActivation;
        asyncOperation.completed += (_) =>
        {
            OpenNGS.Profiling.ProfilerLog.Start("NiSceneManager.OnSceneActive", sceneName);
            OnSceneActive(sceneName);
            OpenNGS.Profiling.ProfilerLog.End("NiSceneManager.OnSceneActive", sceneName);
        };

        while (true)
        {
            if (asyncOperation.isDone)
            {
#if DEBUG_LOG
                NgDebug.Log($"NiSceneManager.asyncOperation.isDone {asyncOperation.progress} Done");
#endif
                break;
            }
#if DEBUG_LOG
            NgDebug.Log($"NiSceneManager.asyncOperation.isDone {asyncOperation.progress} Progress");
#endif
            OnSceneLoadingProcess(sceneName, asyncOperation.progress);
            if (asyncOperation.progress >= 0.9f)
            {
                NgDebug.Log("NiSceneManager.OnSceneLoaded");
                OnSceneLoaded(sceneName, MakeSceneActive);
                OpenNGS.Profiling.ProfilerLog.End("NiSceneManager.DoSceneLoadAsync", sceneName);
                yield break;
            }
            yield return null;
#if DEBUG_LOG
            NgDebug.Log($"NiSceneManager.asyncOperation.isDone {asyncOperation.progress} Loop");
#endif
        }
        OpenNGS.Profiling.ProfilerLog.End("NiSceneManager.DoSceneLoadAsync", sceneName);

        void MakeSceneActive()
        {
            if (asyncOperation != null)
            {
                asyncOperation.allowSceneActivation = true;
            }
        }
    }

    private IEnumerator DoSceneUnloadAsync(string sceneName, Action finishCallback)
    {
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        if (asyncOperation == null)
        {
            NgDebug.LogError($"Ð¶ÔØ³¡¾°{sceneName}Ê§°Ü");
            yield break;
        }

        asyncOperation.completed += _ =>
        {
            finishCallback?.Invoke();
        };
        yield return null;
    }

}
