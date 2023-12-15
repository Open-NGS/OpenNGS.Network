using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS
{
    public interface IProtoExtension
    {
        void Clear();

        void OnRelease();

        void OnSpawn();
    }
}
