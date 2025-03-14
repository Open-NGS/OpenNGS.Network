using System;
using System.Collections.Generic;
namespace OpenNGS.Platform
{
    public interface IIAPProvider : IModuleProvider
    {
        void InitializePurchasing(Dictionary<string, uint> _dictProducts);
        void Purchase(string productID);
        void Restore();
    }
}