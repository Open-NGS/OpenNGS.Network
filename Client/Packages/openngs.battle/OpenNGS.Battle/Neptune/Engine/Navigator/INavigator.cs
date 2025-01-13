using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune
{
    /// <summary>
    /// Navigator Interface
    /// implement to custom navigation
    /// </summary>
    public interface INavigator
    {
        NavAgent CreateAgent(Entity element, int areaMask);
        void LoadNavGridData(byte[] data);
        void Clear();
    }
}