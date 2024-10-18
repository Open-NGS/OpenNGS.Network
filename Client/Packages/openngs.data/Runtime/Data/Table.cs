using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenNGS.Tables;


namespace OpenNGS
{

    public interface ITable : IDataTable
    {
        bool IsSeasonTable { get; }
        void Load();
    }

    public class Table<ITEM, KEY> : DataTable<ITEM, KEY>, ITable
    {
        public bool IsSeasonTable { get; private set; }

        public Table(DataTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
        {
            this.IsSeasonTable = season;
            DataManager.Instance.AddTable(this);
        }

        public void Load()
        {
            base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
        }

        public void ReLoad()
        {
            Unload();
            Load();
        }
    }

    public class SettingTable<ITEM, KEY> : DataTable<ITEM, KEY>, ITable
    {
        public bool IsSeasonTable { get; private set; }

        public SettingTable(DataTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
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

    public class ListTableBase<ITEM, KEY> : DataCollectionTable<ITEM, KEY>, ITable
    {
        public bool IsSeasonTable { get; private set; }

        public ListTableBase(DataCollectionTable<ITEM, KEY>.KeyGetter getter, bool season) : base(getter)
        {
            this.IsSeasonTable = season;
            DataManager.Instance.AddTable(this);
        }

        public void Load()
        {
            base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
        }
    }


    public class ListTableBase<ITEM, PK, SK> : DataCollectionTable<ITEM, PK, SK>, ITable
    {
        public bool IsSeasonTable { get; private set; }
        public ListTableBase(DataCollectionTable<ITEM, PK, SK>.PKeyGetter pkgetter, DataCollectionTable<ITEM, PK, SK>.SKeyGetter skgetter, bool season)
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


    public class Table<ITEM, PK, SK> : DataTable<ITEM, PK, SK>, ITable
    {
        public bool IsSeasonTable { get; private set; }

        public Table(DataTable<ITEM, PK, SK>.PKeyGetter pkgetter, DataTable<ITEM, PK, SK>.SKeyGetter skgetter, bool season)
        {
            this.IsSeasonTable = season;
            base.SetKeyGetter(pkgetter, skgetter);
            DataManager.Instance.AddTable(this);
        }

        public void Load()
        {
            base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
        }

        public void ReLoad()
        {
            Unload();
            Load();
        }
    }

    public class Table<ITEM,PK,SK,TK> :DataTable<ITEM,PK,SK,TK>, ITable
    {
        public bool IsSeasonTable { get; private set; }

        public Table(DataTable<ITEM, PK, SK,TK>.PKeyGetter pkgetter, DataTable<ITEM, PK, SK,TK>.SKeyGetter skgetter, DataTable<ITEM, PK, SK, TK>.TKeyGetter tkgetter, bool season)
        {
            this.IsSeasonTable = season;
            base.SetKeyGetter(pkgetter, skgetter,tkgetter);
            DataManager.Instance.AddTable(this);
        }

        public void Load()
        {
            base.Load(DataManager.Instance.GetDataFile(Name, IsSeasonTable));
        }

        public void ReLoad()
        {
            Unload();
            Load();
        }
    }
}