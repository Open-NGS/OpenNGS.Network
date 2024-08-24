using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OpenNGS.Platform;
//using GCloud;
using UnityEngine.Events;

public partial class GServerInfo : ServerInfo
{
    public GServerInfo(LeafNode node) : base()
    {
        this.Node = node;
        base.Id = node.Id;
        base.ParentId = node.ParentId;
        base.Name = node.Name;
        base.Flag = (NDirFlag)node.Flag;
        base.Tag = (NDirTag)node.Tag;
        base.Attr1 = node.CustomData.Attr1;
        base.Attr2 = node.CustomData.Attr2;
        base.UserData = node.CustomData.UserData;
        this.ServerIP = this.Node.Url;
        this.RealmPort = base.Attr1.ToString();
        this.GatePort = base.Attr2.ToString();
        
    }

    public new GZoneInfo Zone { get { return (GZoneInfo)base.Zone; } }

    public LeafNode Node { get; private set; }

    public string ServerName { get { return string.Format("{0}【{1}】",((GZoneInfo) this.Zone).Name, this.Node.Name); } }

    public string FullName { get { return string.Format("{0}  【{1}】 {2} ", ((GZoneInfo)this.Zone).Name, this.Node.Name, this.Status); } }

    public string Status
    {
        get
        {
            switch (this.Flag)
            {
                case NDirFlag.Fine: return "[color=#66ff33]良好[/color]";
                case NDirFlag.Crown: return "[color=#cc9900]拥挤[/color]";
                case NDirFlag.Heavy: return "[color=#ff0000]火爆[/color]";
                default:
                    return "[color=#666666]维护[/color]";
            }
        }
    }

    public int StatusIndex
    {
        get
        {
            switch (this.Node.Flag)
            {
                case TreeNodeFlag.Heavy: return 0;//繁忙
                case TreeNodeFlag.Crown: return 1;//拥挤
                case TreeNodeFlag.Fine: return 2;//良好
                default: return 3;//维护
            }
        }
    }
}

public partial class GZoneInfo : ZoneInfo
{
    public CategoryNode Node { get; private set; }

    public string GetName()
    {
        return string.Format("{0}区", this.Node.Id);
    }

    public string FullName { get { return string.Format("{0}区 {1}", this.Node.Id, this.Node.Name); } }

    public GZoneInfo(CategoryNode node)
    {
        this.Node = node;
        base.Id = node.Id;
        base.ParentId = node.ParentId;
        base.Name = node.Name;
        base.Tag = (NDirTag)node.Tag;
        base.Attr1 = node.CustomData.Attr1;
        base.Attr2 = node.CustomData.Attr2;
        base.UserData = node.CustomData.UserData;
    }
}


public class GCloudDirProvider : IDirProvider
{
    public Platform_MODULE Module => Platform_MODULE.DIR;

    private Dictionary<int, ZoneInfo> zones = new Dictionary<int, ZoneInfo>();

    private Dictionary<int, ServerInfo> servers = new Dictionary<int, ServerInfo>();


    public Dictionary<int, ZoneInfo> Zones { get { return zones; } }

    public Dictionary<int, ServerInfo> Servers { get { return servers; } }

    ITdir tdir;
    bool inited = false;
    private bool receivedTdirData = false;

    public event UnityAction OnRefresh;

    public event UnityAction OnServerSelected;

    public int GetZoneCount() { return zones.Count; }

    public void Init(string url, UnityAction refreshCallback, UnityAction selectCallback)
    {
        tdir = TdirFactory.CreateInstance();
        TdirInitInfo initInfo = new TdirInitInfo();
        initInfo.Url = url;
        initInfo.OpenID = "";
        initInfo.TDirType = (int)TDirType.UA;

        inited = tdir.Initialize(initInfo);
        tdir.QueryTreeEvent += Tdir_QueryTreeEvent;
        this.OnRefresh = refreshCallback;
        this.OnServerSelected = selectCallback;
    }

    public void QueryDir(int dirId)
    {
        tdir.QueryTree(dirId);
    }

    public void Update()
    {
        if (tdir != null && inited == true && !receivedTdirData)
        {
            tdir.Update();
        }
    }

    /// <summary>
    /// 当前选择的服务器
    /// </summary>
    public ServerInfo CurrentServer { get; private set; }


    private void Tdir_QueryTreeEvent(Result result, TreeInfo nodeList)
    {

        Debug.LogFormat("ServerSystem Tdir_QueryTreeEvent : {0}:{1}", result.ErrorCode, result.Reason);
        this.zones.Clear();

        int rootId = 0;
        receivedTdirData = true;

        if (nodeList != null && nodeList.NodeList != null && nodeList.NodeList.Count > 0)
        {
            for (int i = 0; i < nodeList.NodeList.Count; i++)
            {
                NodeWrapper node = nodeList.NodeList[i];
                if (node != null)
                {
                    if (node.IsRoot())
                    {
                        rootId = node.Category.Id;
                    }
                    else if (node.IsCategory())
                    {
                        this.AddZone(node.Category);
                    }
                    else if (node.IsLeaf())
                    {
                        this.AddServer(node.Leaf.ParentId, node.Leaf);
                    }
                }
            }
        }
        

        if (OnRefresh != null)
            OnRefresh();
    }

    public ServerInfo GetRecommandServer()
    {
        foreach (var server in this.servers)
        {
            if (server.Value.HasTag(NDirTag.Recommend))
                return server.Value;
        }

        return this.servers.First().Value;
    }

    public ServerInfo GetServer(int id)
    {
        ServerInfo server = null;
        this.servers.TryGetValue(id, out server);
        return server;
    }

    private void AddServer(int parentId, LeafNode leaf)
    {
        GServerInfo server = new GServerInfo(leaf);
        this.servers[server.Id]=server;
        ZoneInfo zone;
        if (this.zones.TryGetValue(parentId, out zone))
        {
            zone.AddServer(server);
        }
    }

    void AddZone(CategoryNode zone)
    {
        GZoneInfo zi = new GZoneInfo(zone);
        this.zones[zone.Id] = zi;
    }
    public void SetCurrentServer(ServerInfo data)
    {
        this.CurrentServer = data;
        if (OnServerSelected != null)
        {
            OnServerSelected();
        }
    }
}
