using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IDirProvider : IModuleProvider
    {
        ServerInfo CurrentServer { get; }

        public Dictionary<int, ZoneInfo> Zones { get; }

        public Dictionary<int, ServerInfo> Servers { get; }

        void Update();
        void Init(string url, UnityEngine.Events.UnityAction refreshCallback, UnityEngine.Events.UnityAction selectCallback);

        void QueryDir(int dirId);
        ServerInfo GetRecommandServer();
        ServerInfo GetServer(int serverID);
        void SetCurrentServer(ServerInfo server);


        int GetZoneCount();
    }
}
