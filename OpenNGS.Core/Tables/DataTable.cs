using System;
using OpenNGS.IO;
using OpenNGS.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Tables
{

    public interface IDataTable
    {
        string Name { get; }
        void Load(string filename);
        void Unload();
    }

    public abstract class TableBase<T, ITEM> : OpenNGS.Singleton<T>, IDataTable where T : Singleton<T>, new()
    {
        public List<ITEM> Items { get; private set; }

        private bool loaded = false;

        public string Name
        {
            get { return typeof(ITEM).Name; }
        }

        public void Load(string filename)
        {
            if (this.loaded) return;

            // TODO bin 文件大小写错位问题临时解决方案
#if DEBUG_LOG && PROFILER
            OpenNGS.Profiling.ProfilerLog.Start("Tables.Load", filename);
#endif
            byte[] data = File.ReadAllBytes(filename);
 
            if (data == null) {
#if UNITY_2021_1_OR_NEWER
                var filenameList = filename.Split("/");
                filenameList[^1] = filenameList[^1].ToLower();
                var filenameNew = String.Join("/", filenameList);

#else
                var filenameList = filename.Split('/');
                filenameList[filenameList.Length-1] = filenameList[filenameList.Length - 1].ToLower();
                var filenameNew = String.Join("/", filenameList);

#endif
                data = File.ReadAllBytes(filenameNew);

                if (data == null) {
                    Debug.LogError("[DataTable:Load]can't find " + filename + " and " + filenameNew);
#if DEBUG_LOG && PROFILER
                    OpenNGS.Profiling.ProfilerLog.End("Tables.Load", filename);
#endif
                    return;
                }

            }

            try
            {
                this.Items = DataTable.Serializer.Deserialize<List<ITEM>>(data);
                this.Prepare();
            }
            catch (Exception ex)
            {
                Debug.LogError("[DataTable:Load]" + filename + " Error:" + ex.ToString());
            }
            this.loaded = true;
#if DEBUG_LOG && PROFILER
            OpenNGS.Profiling.ProfilerLog.End("Tables.Load", filename);
#endif
        }

        protected virtual void Prepare() { }

        public ITEM GetItem(int index)
        {
            ITEM item = this.Items[index];
            return item;
        }

        public virtual void Unload()
        {
            this.Items.Clear();
            this.loaded = false;
        }
    }

    public class DataTable
    {
        public static ISerializer Serializer { get; set; }

    }

    public class DataTable<ITEM, KEY> : TableBase<DataTable<ITEM, KEY>, ITEM>, IDataTable
    {
        public static Dictionary<KEY, ITEM> map = new Dictionary<KEY, ITEM>();

        public delegate KEY KeyGetter(ITEM item);

        KeyGetter keyGetter;

        public DataTable()
        {

        }
        public DataTable(KeyGetter keyGetter)
        {
            this.keyGetter = keyGetter;
        }

        protected virtual KEY GetKey(ITEM item) {
            if (this.keyGetter != null)
                return this.keyGetter(item);
            return default(KEY);
        }


        protected override void Prepare()
        {
            foreach(var item in this.Items)
            {
                try
                {
                    map.Add(GetKey(item), item);
                }
                catch(System.Exception ex)
                {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
                    UnityEngine.Debug.LogErrorFormat("{0}:{1}", this.Name, ex.Message);
#else
                    throw ex;
#endif
                }
            }
        }

        public ITEM GetItem(KEY key)
        {
            ITEM item = default(ITEM);
            map.TryGetValue(key, out item);
            return item;
        }

        public override void Unload()
        {
            map.Clear();
            base.Unload();
        }
    }

    public class DataTable<ITEM, PK, SK>  : TableBase<DataTable<ITEM, PK, SK>, ITEM>, IDataTable
    {
        public Dictionary<PK, Dictionary<SK, ITEM>> Map = new Dictionary<PK, Dictionary<SK, ITEM>>();

        public delegate PK PKeyGetter(ITEM item);
        public delegate SK SKeyGetter(ITEM item);

        PKeyGetter pkGetter;
        SKeyGetter skGetter;

        public DataTable<ITEM, PK, SK> SetKeyGetter(PKeyGetter pkgetter , SKeyGetter skgetter)
        {
            pkGetter = pkgetter;
            skGetter = skgetter;
            return this;
        }

        protected override void Prepare()
        {
            foreach (var item in this.Items)
            {
                try
                {
                    this.AddItem(item);
                }
                catch (System.Exception ex)
                {
                    throw new Exception(string.Format("{0}:{1}.{2} \nPrepare Exceotion:\n{3}", this.Name, GetPKey(item), GetSKey(item), ex));
                }
            }
        }

        private void AddItem(ITEM item)
        {
            Dictionary<SK, ITEM> submap = null;
            PK key1 = GetPKey(item);
            if (!this.Map.TryGetValue(key1, out submap))
            {
                submap = new Dictionary<SK, ITEM>();
                this.Map[key1] = submap;
            }
            submap.Add(GetSKey(item), item);
        }

        private PK GetPKey(ITEM item)
        {
            if (this.pkGetter != null)
                return this.pkGetter(item);
            return default(PK);
        }

        private SK GetSKey(ITEM item)
        {
            if (this.skGetter != null)
                return this.skGetter(item);
            return default(SK);
        }

        public ITEM GetItem(PK pk, SK sk)
        {
            ITEM item = default(ITEM);
            Dictionary<SK, ITEM> submap = GetItems(pk);
            if(submap!=null)
            { 
                submap.TryGetValue(sk, out item);
            }
            return item;
        }

        public Dictionary<SK, ITEM> GetItems(PK pk)
        {
            Dictionary<SK, ITEM> items = null;
            this.Map.TryGetValue(pk, out items);
            return items;
        }

        public override void Unload()
        {
            Map.Clear();
            base.Unload();
        }
    }

    public class DataTable<ITEM,PK,SK,TK> : TableBase<DataTable<ITEM, PK, SK,TK>, ITEM>, IDataTable
    {
        public Dictionary<PK, Dictionary<SK, Dictionary<TK,ITEM>>> Map = new Dictionary<PK, Dictionary<SK, Dictionary<TK, ITEM>>>();

        public delegate PK PKeyGetter(ITEM item);
        public delegate SK SKeyGetter(ITEM item);
        public delegate TK TKeyGetter(ITEM item);

        PKeyGetter pkGetter;
        SKeyGetter skGetter;
        TKeyGetter tkGetter;

        public DataTable<ITEM, PK, SK,TK> SetKeyGetter(PKeyGetter pkgetter, SKeyGetter skgetter,TKeyGetter tkgetter)
        {
            pkGetter = pkgetter;
            skGetter = skgetter;
            tkGetter = tkgetter;
            return this;
        }

        protected override void Prepare()
        {
            foreach (var item in this.Items)
            {
                try
                {
                    this.AddItem(item);
                }
                catch (System.Exception ex)
                {
                    throw new Exception(string.Format("{0}:{1}.{2}.{3} \nPrepare Exceotion:\n{4}", this.Name, GetPKey(item), GetSKey(item),
                    GetTKey(item), ex));
                }
            }
        }

        private void AddItem(ITEM item)
        {
            Dictionary<SK, Dictionary<TK, ITEM>> submap = null;
            PK key1 = GetPKey(item);
            SK key2 = GetSKey(item);
            if (!this.Map.TryGetValue(key1, out submap))
            {
                submap = new Dictionary<SK, Dictionary<TK, ITEM>>();
                this.Map[key1] = submap;
            }
            Dictionary<TK,ITEM> submap2 = null;
            if(!submap.TryGetValue(key2,out submap2))
            {
                submap2 = new Dictionary<TK, ITEM>();
                submap[key2] = submap2;
            }
            TK _tk = GetTKey(item);
            submap2[_tk] = item;
            //submap.Add(key2, submap2);
        }

        private PK GetPKey(ITEM item)
        {
            if (this.pkGetter != null)
                return this.pkGetter(item);
            return default(PK);
        }

        private SK GetSKey(ITEM item)
        {
            if (this.skGetter != null)
                return this.skGetter(item);
            return default(SK);
        }

        private TK GetTKey(ITEM item)
        {
            if (this.tkGetter != null)
                return this.tkGetter(item);
            return default(TK);
        }

        public ITEM GetItem(PK pk, SK sk,TK tk)
        {
            ITEM item = default(ITEM);
            Dictionary<SK, Dictionary<TK, ITEM>> submap = GetItems(pk);
            Dictionary<TK, ITEM> submap2 = null;
            if (submap != null)
            {
                submap.TryGetValue(sk, out submap2);
            }
            if(submap2 != null)
            {
                submap2.TryGetValue(tk, out item);
            }
            return item;
        }

        public Dictionary<SK, Dictionary<TK, ITEM>> GetItems(PK pk)
        {
            Dictionary<SK, Dictionary<TK, ITEM>> items = null;
            this.Map.TryGetValue(pk, out items);
            return items;
        }

        public override void Unload()
        {
            Map.Clear();
            base.Unload();
        }
    }


    public class DataCollectionTable<ITEM, KEY> : TableBase<DataTable<ITEM, KEY>, ITEM>, IDataTable
    {
        public Dictionary<KEY, List<ITEM>> Map = new Dictionary<KEY, List<ITEM>>();

        public delegate KEY KeyGetter(ITEM item);

        KeyGetter keyGetter;

        public DataCollectionTable(KeyGetter getter)
        {
            this.keyGetter = getter;
        }

        protected override void Prepare()
        {
            foreach (var item in this.Items)
            {
                try
                {
                    this.AddItem(item);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void AddItem(ITEM item)
        {
            List<ITEM> list = null;
            KEY key1 = GetPKey(item);
            if (!this.Map.TryGetValue(key1, out list))
            {
                list = new List<ITEM>();
                this.Map[key1] = list;
            }
            list.Add(item);
        }

        private KEY GetPKey(ITEM item)
        {
            if (this.keyGetter != null)
                return this.keyGetter(item);
            return default(KEY);
        }

        public ITEM GetItem(KEY pk, int index)
        {
            ITEM item = default(ITEM);
            List<ITEM> list = GetItems(pk);
            if (list != null)
            {
                item = list[index];
            }
            return item;
        }

        public List<ITEM> GetItems(KEY pk)
        {
            List<ITEM> items = null;
            this.Map.TryGetValue(pk, out items);
            return items;
        }

        public override void Unload()
        {
            Map.Clear();
            base.Unload();
        }
    }


    public class DataCollectionTable<ITEM, PK, SK> : TableBase<DataTable<ITEM, PK, SK>, ITEM>, IDataTable
    {
        public Dictionary<PK, Dictionary<SK, List<ITEM>>> Map = new Dictionary<PK, Dictionary<SK, List<ITEM>>>();
        public delegate PK PKeyGetter(ITEM item);
        public delegate SK SKeyGetter(ITEM item);

        PKeyGetter pkGetter;
        SKeyGetter skGetter;

        public DataCollectionTable<ITEM, PK, SK> SetKeyGetter(PKeyGetter pkgetter, SKeyGetter skgetter)
        {
            pkGetter = pkgetter;
            skGetter = skgetter;
            return this;
        }

        protected override void Prepare()
        {
            foreach (var item in this.Items)
            {
                try
                {
                    this.AddItem(item);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void AddItem(ITEM item)
        {
            Dictionary<SK, List<ITEM>> submap = null;
            PK key1 = GetPKey(item);
            if (!this.Map.TryGetValue(key1, out submap))
            {
                submap = new Dictionary<SK, List<ITEM>>();
                this.Map[key1] = submap;
            }
            SK _key2 = GetSKey(item);
            List<ITEM> _lst = null;
            if (!submap.TryGetValue(_key2, out _lst) )
            {
                _lst = new List<ITEM>();
                submap[_key2] = _lst;
            }
            _lst.Add(item);
        }

        private SK GetSKey(ITEM item)
        {
            if (this.skGetter != null)
                return this.skGetter(item);
            return default(SK);
        }
        private PK GetPKey(ITEM item)
        {
            if (this.pkGetter != null)
                return this.pkGetter(item);
            return default(PK);
        }

        public List<ITEM> GetItems(PK pk, SK sk)
        {
            Dictionary<SK, List<ITEM>> _subMap = null;
            List<ITEM> _lstResult = null;
            if ( this.Map.TryGetValue(pk, out _subMap) )
            {
                _subMap.TryGetValue(sk, out _lstResult);
            }
            return _lstResult;
        }

        public override void Unload()
        {
            Map.Clear();
            base.Unload();
        }
    }
}
