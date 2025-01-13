using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune.Assets
{
    /// <summary>
    /// ILoadLimiter
    /// </summary>
    public interface ILoadLimiter
    {
        /// <summary>
        /// Can Load
        /// </summary>
        bool CanLoad { get; }
    }
}