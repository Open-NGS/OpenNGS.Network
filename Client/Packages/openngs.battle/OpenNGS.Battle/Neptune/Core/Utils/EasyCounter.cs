using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EasyCounter: Singleton<EasyCounter>, System.IDisposable
{
    float start;
    string text;

    private static Dictionary<string, float> records = new Dictionary<string, float>();

    public EasyCounter()
    {

    }

    public void Start(string key)
    {
#if DEVELOPMENT
        records[key] = SafeTime.realtimeSinceStartup;
        Debug.LogFormat("EasyCounter:[{0}]Begin :{1}", key, records[key]);
#endif
    }
    public void End(string key)
    {
#if DEVELOPMENT
        if (records.ContainsKey(key))
            Debug.LogFormat("EasyCounter:[{0}]End :{1} - Elapsed :{2}", key, SafeTime.realtimeSinceStartup, SafeTime.realtimeSinceStartup - records[key]);
#endif
    }

    public EasyCounter(string text)
    {
        this.text = text;
        this.Start(text);
    }

    public void Dispose()
    {
        if (this.text != null)
            this.End(this.text);
    }
}
