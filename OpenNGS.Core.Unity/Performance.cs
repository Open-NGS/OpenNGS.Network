using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Performance : MonoBehaviour
{
    private int frameRate = -1;
    public int maxFrameRate = -1;
    private float targetFrameTime = 0;
    private float threshold = 0.001f;
    private float lastFrame = 0;

    public static bool enableFrameLimit = false;

    public static float deltaTime = 0.033f;
    public static float frameTime { get; private set; }
    public static float gpuFrameTime { get; private set; }
    public static float cpuFrameTime { get; private set; }
    public static float fps { get; private set; }

    float frameBegin = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Loop());
    }

    void LateUpdate()
    {
        float time = UnityEngine.Time.realtimeSinceStartup;
        if (frameBegin == 0)
            frameBegin = time * 1000;

        frameTime = time * 1000 - frameBegin;
        cpuFrameTime = frameTime - gpuFrameTime;
        frameBegin = time * 1000;
        fps = 1000f / frameTime;

#if PROFILER && DEBUG_LOG
        if (frameTime > 33f)
        {
            Debug.LogWarningFormat("************ Performance Low: {0} ************", frameTime);
        }
#endif
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return OpenNGS.Wait.WaitForEndOfFrame;
            gpuFrameTime = UnityEngine.Time.realtimeSinceStartup * 1000 - frameBegin;
            deltaTime = Time.realtimeSinceStartup - this.lastFrame;
            if (enableFrameLimit)
            {
                if (this.frameRate != this.maxFrameRate)
                {
                    this.frameRate = this.maxFrameRate;
                    if (this.frameRate > 0)
                        this.targetFrameTime = 1f / this.frameRate;
                    else
                        this.targetFrameTime = 0f;
                }
                if (frameRate > 0)
                {
                    var wait = this.targetFrameTime - deltaTime - threshold;
                    if (wait > threshold)
                    {
                        Thread.Sleep((int)Mathf.Ceil(wait * 1000));
                    }
                }
            }
            this.lastFrame = Time.realtimeSinceStartup;
        }
    }
}
