using OpenNGS.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.Net
{
    public class ProtoFactory : Singleton<ProtoFactory>
    {
        public T GetProtocol<T>() where T : OpenNGS.IProtoExtension
        {
            return ObjectPool.Get<T>();
        }

        public void RecycleProtocol<T>(T protocol) where T : OpenNGS.IProtoExtension
        {
            if (protocol != null)
            {
                protocol.Clear();
            }
            ObjectPool.Recycle<T>(protocol);
        }
    }
}
