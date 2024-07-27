using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);
    }
}
