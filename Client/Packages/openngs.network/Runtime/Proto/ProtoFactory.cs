using OpenNGS.Pool;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.Network
{
    public class ProtoFactory : Singleton<ProtoFactory>
    {
        public T GetProtocol<T>() where T : IProtoExtension
        {
            return ObjectPool.Get<T>();
        }

        public void RecycleProtocol<T>(T protocol) where T : IProtoExtension
        {
            if (protocol != null)
            {
                protocol.Clear();
            }
            ObjectPool.Recycle<T>(protocol);
        }
    }
}
