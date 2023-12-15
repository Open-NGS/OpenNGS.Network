using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformDir
{
    
    public static void Update()
    {
        if (Platform.GetDir() == null) return;
        Platform.GetDir().Update();
    }
   
    public static void Init(string url, UnityAction refreshCallback, UnityAction selectCallback)
    {
        Platform.GetDir().Init(url, refreshCallback, selectCallback);
    }

    public static void QueryDir(int dirId)
    {
        Platform.GetDir().QueryDir(dirId);
    }

    public static ServerInfo GetRecommandServer()
    {
        return Platform.GetDir().GetRecommandServer();
    }

    public static ServerInfo GetServer(int serverID)
    {
        return Platform.GetDir().GetServer(serverID);
    }

    public static void SetCurrentServer(ServerInfo server)
    {
        Platform.GetDir().SetCurrentServer(server);
    }

    public static int ZoneCount
    {
        get
        {
            return Platform.GetDir().GetZoneCount();
        }
    }

    public static ServerInfo CurrentServer
    {
        get
        {
            return Platform.GetDir().CurrentServer;
        }
    }


    public static Dictionary<int, ZoneInfo> Zones
    {
        get
        {
            return Platform.GetDir().Zones;
        }
    }

    public static Dictionary<int, ServerInfo> Servers
    {
        get
        {
            return Platform.GetDir().Servers;
        }
    }
}

public enum NDirFlag
{
    Heavy = 0x10,
    Crown = 0x20,
    Fine = 0x40,
    Unavailable = 0x80,
}

[Flags]
public enum NDirTag
{
    Hot = 0x01,
    Recommend = 0x02,
    New = 0x04,
    Limited = 0x08,
    Experience = 0x10,
}

public class PlatformDirNode
{
    public int Id;//Id of node
    public int ParentId;//Id of parent
    public string Name;//Name of node

    public NDirFlag Flag;
    public NDirTag Tag;

    //Corresponding backend custom value 1
    public int Attr1;
    //Corresponding backend custom value 2
    public int Attr2;
    //Corresponding backend custom data
    public string UserData;

    public bool HasTag(NDirTag tag)
    {
        return (this.Tag & tag) == tag;
    }

}


public partial class ServerInfo : PlatformDirNode
{
    public string Url;
    public string ServerIP { get;  set; }
    public string RealmPort { get;  set; }
    public string GatePort { get; set; }
    public ZoneInfo Zone { get; set; }
    
    public ServerInfo()
    {  
    }
}

public partial class ZoneInfo : PlatformDirNode
{
    public virtual string GetName() { return Name; }

    public Dictionary<int, ServerInfo> ServerList = new Dictionary<int, ServerInfo>();

    public void AddServer(ServerInfo server)
    {
        server.Zone = this;
        this.ServerList.Add(server.Id, server);
    }
}
