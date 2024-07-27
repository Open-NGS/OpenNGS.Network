using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace OpenNGS.Configs
{
    //public interface IAsyncMgr
    //{
    //    void EndAsyncLoad(IData config);
    //    void QueueWorkItem(WaitCallback callback);
    //}

    //public interface IData
    //{
    //    bool Loaded { get; }
    //    void Load();
    //    void LoadAsync(IAsyncMgr mgr);
    //}

    public class DataLoad : Singleton<DataLoad>, IAsyncMgr
    {
        //private List<IData> Configs = new List<IData>();

        private Queue<WaitCallback> Callbacks = new Queue<WaitCallback>();
        private HashSet<IData> AsyncLoadQueue = new HashSet<IData>();
        private int ThreadCount = 2;
        private bool UseThreadPool = false;

        //public void Init()
        //{
        //    //Configs.Clear();
        //    AsyncLoadQueue.Clear();
        //}
        //public int Total
        //{
        //    get { return Configs.Count; }
        //}

        //public int LoadedCount
        //{
        //    get { return Total - AsyncLoadQueue.Count; }
        //}
        public IEnumerator LoadAsync(List<IData> datas)
        {
            foreach (var cfg in datas)
            {
                if (!cfg.Loaded)
                    BeginAsyncLoad(cfg);

                //if (OpenNGS.Wait.shouldWaitNextFrame) yield return null;
            }

            if (this.AsyncLoadQueue.Count == 0) yield break;

            if (!UseThreadPool)
            {
                for (int i = 0; i < ThreadCount; i++)
                {
                    Thread workThread = new Thread(new ThreadStart(WorkThread));
                    workThread.Name = "ConfigThread" + i;
                    workThread.Start();
                }
            }
        }

        private void WorkThread()
        {
            while (true)
            {
                WaitCallback call = null;
                lock (this.Callbacks)
                {
                    if (this.Callbacks.Count == 0)
                        break;
                    call = this.Callbacks.Dequeue();
                }

                if (call != null)
                {
                    call(null);
                }
            }
        }

        public IEnumerator WaitAllComplete()
        {
            while (this.AsyncLoadQueue.Count > 0)
            {
                yield return new WaitForEndOfFrame();
            }
#if DEBUG_LOG
            // Debug.LogFormat("ConfigMgr:{0}:WaitAllAsyncComplete({1}/{2})", Time.realtimeSinceStartup, this.LoadedCount, this.Total);
#endif
        }

        private void BeginAsyncLoad(IData config)
        {
            this.AsyncLoadQueue.Add(config);
            config.LoadAsync(this);
        }

        public void EndAsyncLoad(IData config)
        {
            this.AsyncLoadQueue.Remove(config);
#if DEVELOPMENT && DEBUG_LOG
        //Debug.LogFormat("{0}:EndAsyncLoad:{1}", config.GetType().ToString(), OpenNGS.Time.RealtimeSinceStartup);
#endif
        }

        public void QueueWorkItem(WaitCallback callback)
        {
            if (this.UseThreadPool)
            {
                ThreadPool.QueueUserWorkItem(callback);
            }
            else
                Callbacks.Enqueue(callback);
        }
    }
}