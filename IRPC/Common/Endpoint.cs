using System;

namespace OpenNGS.IRPC
{
    public struct Endpoint
    {
        /// <summary>
        /// target endpoint name
        /// </summary>
        public string targetName;

        /// <summary>
        /// target endpoint id
        /// </summary>
        public UInt64 targetID;

        /// <summary>
        /// key for select target
        /// </summary>
        public string routeInfo;
    }
}
