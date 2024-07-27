using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNGS.Configs
{
    public interface IAsyncMgr
    {
        void EndAsyncLoad(IData config);
        void QueueWorkItem(WaitCallback callback);
    }

    public interface IData
    {
        bool Loaded { get; }
        void LoadAsync(IAsyncMgr mgr);
    }
    public abstract class BaseData<T> : OpenNGS.Singleton<T>, IData where T : BaseData<T>, new()
    {
        public bool Loaded { get; private set; }

        private IAsyncMgr AsyncMgr;
        public void LoadAsync(IAsyncMgr mgr)
        {
            if (this.Loaded) return;
            this.AsyncMgr = mgr;

            this.AsyncMgr.QueueWorkItem((go) =>
            {
                LoadAsyncWork();
            });

        }
        protected virtual void LoadAsyncWork()
        {
            this.Loaded = true;
            this.AsyncMgr.EndAsyncLoad(this);
        }
    }
}
