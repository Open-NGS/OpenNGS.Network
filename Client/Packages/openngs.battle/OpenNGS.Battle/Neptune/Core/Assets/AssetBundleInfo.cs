using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
// 
// Asset Bundle Management Module V1.0 - mailto:Ray@RayMix.net
// 


/// <summary>
/// AssetBundleInfo
/// </summary>
public class AssetBundleInfo : CustomYieldInstruction
{
    private AssetBundleType type;
    private AssetBundle m_AssetBundle;
    private byte[] m_Bytes;
    private string m_Text;
    private Texture2D m_Texture;
    private string m_Error;
    private bool m_IsDone;
    private WWW www;
    private AssetBundleCreateRequest req;

    private int m_PrevDownloadBytes;
    private float m_PrevProgress;

    private int m_WatchProgress;
    private int m_WatchTotal;

    int m_RefCount = 0;
    const int CacheTime = 30;
    public float LifeTime = CacheTime;
    public int refCount { get { return m_RefCount; } set { m_RefCount = value; LifeTime = CacheTime; } }

    /// <summary>
    /// if this bundle is persistent
    /// </summary>
    public bool isPersistent;
    /// <summary>
    /// Is the load already finished? (Read Only)
    /// </summary>
    public bool isDone
    {
        get
        {
            if (!m_IsDone)
            {
                if (www != null)
                {
                    m_IsDone = www.isDone;
                    if (m_IsDone)
                    {
                        m_Error = www.error;
                        if (string.IsNullOrEmpty(m_Error))
                        {
                            switch (type)
                            {
                                case AssetBundleType.AssetBundle:
                                    m_AssetBundle = AssetBundle.LoadFromMemory(www.bytes);
                                    break;
                                case AssetBundleType.Bytes:
                                    m_Bytes = www.bytes;
                                    break;
                                case AssetBundleType.Text:
                                    m_Text = www.text;
                                    break;
                                case AssetBundleType.Texture:
                                    m_Texture = www.texture;
                                    break;
                            }
                        }
                        else
                        {
                            Debug.Log("Load Bundle " + this.name + " error: " + www.error);
                            AssetBundleManager.Instance.ClearBundle(this);
                            return true;
                        }
                    }

                    Watchdog(www);
                }
                else if (req != null)
                {
                    m_IsDone = req.isDone;
                    if (m_IsDone)
                    {
                        m_AssetBundle = req.assetBundle;
                    }
                }
            }

            if (m_IsDone && this.Dependencies.Count > 0)
            {
                for (int i = 0; i < this.Dependencies.Count; i++)
                {
                    if (!this.Dependencies[i].isDone)
                        return false;
                }
            }
            return m_IsDone;
        }
    }

    /// <summary>
    /// How far has the load progressed (Read Only).
    /// </summary>
    public float progress
    {
        get
        {
            if (www != null)
                return www.progress;
            else if (req != null)
                return req.progress;
            return 0;
        }
    }

    public string name { get; set; }

    /// <summary>
    ///  Returns the contents of the fetched asset as a string (Read Only).
    /// </summary>
    public string text
    {
        get { return m_Text; }
    }

    /// <summary>
    ///  Returns the contents of the fetched asset as a byte array (Read Only).
    /// </summary>
    public byte[] bytes
    {
        get { return m_Bytes; }
    }

    /// <summary>
    /// Returns a Texture2D generated from the downloaded data (Read Only).
    /// </summary>
    public Texture2D texture
    {
        get { return m_Texture; }
    }

    /// <summary>
    /// Returns an error message if there was an error during the WWW load (Read Only).
    /// </summary>
    public string error
    {
        get { return m_Error; }
    }

    /// <summary>
    /// Streams an AssetBundle that can contain any kind of asset from the project folder.
    /// </summary>
    public AssetBundle assetBundle
    {
        get
        {
            return m_AssetBundle;
        }
        set
        {
            m_AssetBundle = value;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            //Debug.LogFormat("keepWaiting:{0}:{1}",this.isDone, this.name);
            return !this.isDone;
        }
    }

    private bool Watchdog(WWW www)
    {
        if (www.bytesDownloaded == this.m_PrevDownloadBytes && www.progress == this.m_PrevProgress)
        {
            this.m_WatchProgress++;
        }
        else
            this.m_WatchProgress = 0;

        this.m_PrevDownloadBytes = www.bytesDownloaded;
        this.m_PrevProgress = www.progress;

        this.m_WatchTotal++;

        if (this.m_WatchProgress > 300)
        {
            Debug.LogWarningFormat("Watchdog > WWW [{0}] done:{1} {2}/{3}[{4:f1}%] ERR:{5} DEPS:\n{6}", www.url, www.isDone, www.bytesDownloaded, www.size, www.progress * 100f, www.error, string.Join("\n", this.Dependencies.ConvertAll<string>((x) => { return string.Format("{0}[{1}]", x.name, x.isDone); }).ToArray()));
        }
        if (this.m_WatchTotal > 10000)
        {
            Debug.LogErrorFormat("Watchdog > WWW [{0}] done:{1} {2}/{3}[{4:f1}%] ERR:{5} DEPS:\n{6}", www.url, www.isDone, www.bytesDownloaded, www.size, www.progress * 100f, www.error, string.Join("\n", this.Dependencies.ConvertAll<string>((x) => { return string.Format("{0}[{1}]", x.name, x.isDone); }).ToArray()));
        }
        return true;
    }

    public void Unload(bool delay = false)
    {
        refCount--;
        if (refCount == 0)
        {
            if (AssetBundleManager.Instance.Unload(this, delay))
            {
                foreach (AssetBundleInfo assetBundleInfo in Dependencies)
                {
                    assetBundleInfo.Unload(true);
                }
            }
            else
            {
                refCount++;
            }
        }
    }

    public void ImmediateUnload()
    {
        m_Bytes = null;
        m_Texture = null;
        m_Text = "";
        if (assetBundle != null)
        {
            assetBundle.Unload(false);
            assetBundle = null;
        }
        if (www != null)
        {
            www.Dispose();
            www = null;
        }
    }

    public int DependenciesCount
    {
        get { return this.Dependencies.Count; }
    }

    private List<AssetBundleInfo> Dependencies = new List<AssetBundleInfo>();
    private HashSet<string> DependNames = new HashSet<string>();

    /// <summary>
    /// Create a AssetBundleInfo from AssetBundle
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <returns>A new AssetBundleInfo object. the results can be fetched from the returned object.</returns>
    public AssetBundleInfo(AssetBundle assetBundle)
    {
        this.m_AssetBundle = assetBundle;
        this.m_IsDone = true;
        this.name = assetBundle.name;
    }

    /// <summary>
    /// Create a AssetBundleInfo from a WWW request
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <returns>A new AssetBundleInfo object. When it has been loaded, the results can be fetched from the returned object.</returns>
    public AssetBundleInfo(WWW www, AssetBundleType ptype)
    {
        this.type = ptype;
        this.www = www;
        this.name = www.url;
    }

    /// <summary>
    /// Create a AssetBundleInfo from a async file request
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <returns>A new AssetBundleInfo object. When it has been loaded, the results can be fetched from the returned object.</returns>
    public AssetBundleInfo(AssetBundleCreateRequest req)
    {
        this.req = req;
    }
    /// <summary>
    /// Create a AssetBundleInfo from string
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <returns>A new AssetBundleInfo object. the results can be fetched from the returned object.</returns>
    public AssetBundleInfo(string text)
    {
        this.m_IsDone = true;
        this.m_Text = text;
    }
    /// <summary>
    /// Create a AssetBundleInfo from byte array
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <returns>A new AssetBundleInfo object. the results can be fetched from the returned object.</returns>
    public AssetBundleInfo(byte[] data)
    {
        this.m_IsDone = true;
        this.m_Bytes = data;
    }

    public void AddDependencies(AssetBundleInfo bundle)
    {
        if(!this.DependNames.Contains(bundle.name))
        {
            this.DependNames.Add(bundle.name);
            this.Dependencies.Add(bundle);
        }

    }


}