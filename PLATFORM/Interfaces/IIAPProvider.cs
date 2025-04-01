using System;
using System.Collections.Generic;
namespace OpenNGS.Platform
{
    public interface IIAPProvider : IModuleProvider
    {
        void InitializePurchasing(Dictionary<string, uint> _dictProducts, bool _testMode);
        void Purchase(string productID);
        void Restore();
        void GetPriceByID(string productID);
    }
}