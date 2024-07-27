
using System;
using UnityEngine;

namespace OpenNGS.Platform
{
    public class NRequest<T> where T : PlatformData
    {
        public delegate void Callback(T message);
        private event Callback callback_;

        T data = null;


        public NRequest(T data)
        {
            this.data = data;
            this.data.Result = OPENNGS_PLAT_RESULT.Success;
        }

        public NRequest(ulong requestID) { this.RequestID = requestID; }
        public ulong RequestID { get; set; }

        public NRequest<T> OnComplete(Callback callback)
        {
            callback_ = callback;
            if(this.data!=null)
            {
                HandleMessage(data);
            }
            return this;
        }

        virtual public void HandleMessage(T msg)
        {
            if (callback_ != null)
            {
                callback_(msg);
                return;
            }
        }
    }
}
