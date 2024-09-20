using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Neptune.Assets;

// 
// Data Management Module V1.0 - mailto:Ray@RayMix.net
// 

/// <summary>
/// DataGetterBase
/// Data getter base class
/// </summary>
public abstract class DataGetterBase
{
    protected string resource;
    public static int MaxCount;
    public static int LoadedCount;
    protected bool loading;
    protected bool loaded = false;
    private float startloadtime = 0;
    public event UnityAction onComplated;

    public virtual bool isLoaded { get { return loaded; } }
    public bool Ready { get { return this.loaded; } }

    public DataGetterBase(string resource)
    {
        this.resource = resource;
        MaxCount++;
    }

    public virtual void Load()
    {
        if (!this.isLoaded)
        {
            //avoid reentry
            if (!loading)
                loading = true;
            else
            {
                return;
            }
            Load(this.resource);
        }
        else
        {
            if (onComplated != null)
                onComplated();
        }
    }
    /// <summary>
    /// 重新加载
    /// </summary>
    public virtual void Reload()
    {
        if (!loaded)
        {
            return;
        }
        //avoid reentry
        loading = false;
        //清理
        this.loaded = false;
        //清空后计数减少
        LoadedCount--;
        Load(this.resource);
    }
    protected virtual void LoadComplate()
    {
        this.loaded = true;
        LoadedCount++;
        if (onComplated != null)
            onComplated();

        Debug.LogFormat("LoadComplatee:{0} Elapsed:{1}", resource, Time.realtimeSinceStartup - startloadtime);
    }

    public IEnumerator Wait()
    {
        while (!this.loaded)
            yield return null;
    }

    public IEnumerator LoadAndWait()
    {
        this.Load();
        while (!this.loaded)
            yield return null;
    }

    protected virtual void onLoadJSON(string json)
    {
    }

    protected virtual void onLoadBSON(byte[] bson)
    {
    }

    private void Load(string name)
    {
        Debug.Log("DataGetterBase :: Load:" + name);
        startloadtime = Time.realtimeSinceStartup;
#if SERVER
        string path = System.Environment.GetEnvironmentVariable("CLIENT_DATA_PATH"); //System.AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrEmpty(path)) path = ".";
        string json = System.IO.File.ReadAllText(path + "/" + name + ".txt");
        onLoadJSON(json);
#else
#if UNITY_EDITOR || UNITY_STANDALONE
        string file = name + ".txt";
        if (AssetLoader.RawMode)
        {
            if (Application.isEditor)
            {
                file = "Assets/Game/BuildAssets/" + file;
            }
            float st = Time.realtimeSinceStartup;
            if (System.IO.File.Exists(file))
            {
                string json = System.IO.File.ReadAllText(file, Encoding.UTF8);
                try
                {
                    Debug.LogFormat("Load Data Done:{0} Elapsed:{1}", name, Time.realtimeSinceStartup - st);
                    onLoadJSON(json);
                    Debug.LogFormat("Parse Data Done:{0} Elapsed:{1}", name, Time.realtimeSinceStartup - st);
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception("Json deserialize error " + name, ex);
                }
            }
           
        }

#endif
        Debug.LogFormat("Load Data:{0}", name);

#if ENABLE_BSON
        AssetLoader.Load(name, onLoadBSON);
#else
        AssetLoader.Load(name, onLoadJSON); 
#endif

#endif
    }
}


public class MultiDataGetter
{
    DataGetterBase[] getters;
    int loaded = 0;
    UnityAction onLoad = null;
    public MultiDataGetter(DataGetterBase[] getter)
    {
        getters = getter;
    }

    void onAllLoaded()
    {
        loaded++;
        //Debug.LogFormat("MultiDataGetter Loaded {0}/{1}", loaded, getters.Length);
        if (loaded == getters.Length && onLoad != null)
            onLoad();
    }

    public void Load(UnityAction onLoad)
    {
        loaded = 0;
        this.onLoad = onLoad;
        for (int i = 0; i < getters.Length; i++)
        {
            getters[i].onComplated += onAllLoaded;
            getters[i].Load();
        }
    }

    public IEnumerator LoadCoroutine(UnityAction onLoad = null)
    {
        for (int i = 0; i < getters.Length; i++)
        {
            getters[i].Load();
            while (!getters[i].isLoaded)
                yield return null;
        }
        if (onLoad != null)
            onLoad();
    }
}

/// <summary>
/// DictDataGetter
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DictDataGetter<TKey,TValue> : DataGetterBase
{
    public event UnityAction<DelayDictionary<TKey, TValue>> onloaded;
    DelayDictionary<TKey, TValue> datas;

    public DictDataGetter(string resource) :base(resource)
    {
    }

    public DelayDictionary<TKey, TValue> Value
    {
        get
        {
            if (!this.loaded)
            {
                this.Load();
            }
            return datas;
        }
    }
    /// <summary>
    /// 重新加载
    /// </summary>
    public virtual void Reload()
    {
        datas = null;
        base.Reload();
    }
    public void SafeUse(UnityAction<DelayDictionary<TKey, TValue>> action)
    {
        if (this.loaded)
            action(this.Value);
        else
        {
            this.onloaded += action;
            this.Load();
        }
    }
    protected override void onLoadBSON(byte[] bson)
    {
        if (bson != null)
        {
            Debug.LogFormat("Bson Data loaded:{0} - {1}", this.resource, bson.Length);
            try
            {
                var obj = fastBinaryJSON.BJSON.Parse(bson);
                Dictionary<string, object> dict = (Dictionary<string, object>)obj;
                this.datas = new DelayDictionary<TKey, TValue>(dict);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("BSON Parse {0} Failed:{1}.", this.resource, ex.ToString());
            }
        }
        else
        {
            Debug.LogErrorFormat("{0} not existed.", this.resource);
        }
        this.LoadComplate();
    }

    protected override void onLoadJSON(string json)
    {
        if (json == null)
        {
            Debug.LogErrorFormat("{0} not existed", this.resource);
        }
        else
            Debug.LogFormat("Json Data loaded:{0} - {1}", this.resource, json.Length);

        try
        {
            Dictionary<TKey, TValue> datas = fastJSON.JSON.ToObject<Dictionary<TKey, TValue>>(json);
            this.datas = new DelayDictionary<TKey, TValue>(datas);
            this.LoadComplate();
        }
        catch(Exception ex)
        {
            Debug.LogErrorFormat("onLoadJSON:{0}\n{1}\n{2}", this.resource, json, ex.ToString()); ;
        }

    }

    protected override void LoadComplate()
    {
        if (this.onloaded != null)
        {
            onloaded(this.Value);
            this.onloaded = null;
            Debug.LogFormat("Json Data LoadComplate Notify:{0}", this.resource);
        }
        base.LoadComplate();
    }

}

public class DataGetter<T> : DataGetterBase
{
    protected T datas;
    public event UnityAction<T> onloaded;

    public virtual T Value
    {
        get
        {
            if (!this.loaded && datas == null)
            {
                this.Load();
            }
            return datas;
        }
    }

    public DataGetter(string resource) : base(resource)
    {
    }

    /// <summary>
    /// 重新加载
    /// </summary>
    public virtual void Reload()
    {
        datas = default(T);
        base.Reload();
    }
    public void SafeUse(UnityAction<T> action)
    {
        if (this.loaded)
            action(this.Value);
        else
        {
            this.onloaded += action;
            this.Load();
        }
    }


    protected override void onLoadBSON(byte[] bson)
    {
        if (bson != null)
        {
            Debug.LogFormat("Bson Data loaded:{0} - {1}", this.resource, bson.Length);
            try
            {
                this.datas = fastBinaryJSON.BJSON.ToObject<T>(bson);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("BSON Parse {0} Failed:{1}.", this.resource, ex.ToString());
            }
        }
        else
        {
            Debug.LogErrorFormat("{0} not existed.", this.resource);
        }
        this.LoadComplate();
    }

    protected override void onLoadJSON(string json)
    {
        if (json == null)
        {
            Debug.LogErrorFormat("{0} not existed", this.resource);
        }
        Debug.LogFormat("Json Data loaded:{0} - {1}", this.resource, json.Length);
        this.loaded = true;
        this.datas = fastJSON.JSON.ToObject<T>(json);
        this.LoadComplate();
    }

    protected override void LoadComplate()
    {
        if (this.onloaded != null)
        {
            onloaded(this.Value);
            this.onloaded = null;
        }
        base.LoadComplate();
    }
}

