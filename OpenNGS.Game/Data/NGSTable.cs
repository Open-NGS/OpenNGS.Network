using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Tables;

public interface ITable : IOpenNGSTable
{
    bool IsSeasonTable { get; }
    void Load();
}

public class Table<ITEM, KEY> : OpenNGSTable<ITEM, KEY>, ITable
{
    public bool IsSeasonTable { get; private set; }

    public Table(OpenNGSTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
    {
        this.IsSeasonTable = season;
        DataManager.Instance.AddTable(this);
    }

    public void Load()
    {
        base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
    }
}

public class SettingTable<ITEM, KEY> : OpenNGSTable<ITEM, KEY>, ITable
{
    public bool IsSeasonTable { get; private set; }

    public SettingTable(OpenNGSTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
    {
        this.IsSeasonTable = season;
        DataManager.Instance.AddTable(this);
    }

    public ITEM Value { get { return base.GetItem(0); } }

    public void Load()
    {
        base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
    }
}

public class ListTableBase<ITEM, KEY> : NGSListTable<ITEM, KEY>, ITable
{
    public bool IsSeasonTable { get; private set; }

    public ListTableBase(NGSListTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
    {
        this.IsSeasonTable = season;
        DataManager.Instance.AddTable(this);
    }

    public void Load()
    {
        base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
    }
}


public class Table<ITEM, PK, SK> : NGSTable<ITEM, PK, SK>, ITable
{
    public bool IsSeasonTable { get; private set; }

    public Table(NGSTable<ITEM, PK, SK>.PKeyGetter pkgetter, NGSTable<ITEM, PK, SK>.SKeyGetter skgetter, bool season)
    {
        this.IsSeasonTable = season;
        base.SetKeyGetter(pkgetter, skgetter);
        DataManager.Instance.AddTable(this);
    }

    public void Load()
    {
        base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
    }
}
