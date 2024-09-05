using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Initialization;
using UnityEngine;
using OpenNGS.SaveData.Storage;
using OpenNGS.IO;
using OpenNGS;

using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Search;
using static Unity.SaveData.PS5.Search.Searching;
using static UnityEngine.PS5.PS5Input;
using UnityEngine.PS5;

namespace OpenNGS.SaveData.PS5
{
    class PlayStationSaveDataAPI : ISaveDataAPI
    {
        public int Capacity { get; private set; }

        string iconPathWithinUnity = "Media/StreamingAssets/SaveIcon.png";

        List<SaveData> indexies = new List<SaveData>();

        Dictionary<string, SearchSaveDataItem> Slots = new Dictionary<string, SearchSaveDataItem>();

        private int UserID;

        public void Init(IFileSystem fs, int capacity, SaveDataMode mode)
        {
            this.Capacity = capacity;
            this.Slots.Clear();
            Unity.SaveData.PS5.Main.OnAsyncEvent += OnSaveDataEvent;


            // Set 3D Text to whoever's using the pad
#if UNITY_2017_2_OR_NEWER
            UserID = PS5Input.RefreshUsersDetails(0).userId;
#else
            UserID = PS5Input.PadRefreshUsersDetails(0).userId;
#endif


            try
            {
                InitSettings initSettings = new InitSettings();
                initSettings.Affinity = ThreadAffinity.AllCores;
                var initResult = Unity.SaveData.PS5.Main.Initialize(initSettings);
                if (initResult.Initialized == true)
                {
                    Debug.Log("SaveData Initialized ");
                    Debug.Log("Plugin SDK Version : " + initResult.SceSDKVersion.ToString());
                    Debug.Log("Plugin DLL Version : " + initResult.DllVersion.ToString());
                }
                else
                {
                    Debug.LogError("SaveData not initialized ");
                }
                //if (mode == SaveDataMode.FixedSlot)
                //{//
                //    for (int i = 0; i < this.Capacity; i++)
                //    {
                //        Unity.SaveData.PS5.Info.SaveDataParams slot = new Unity.SaveData.PS5.Info.SaveDataParams();
                //        this.InitSlotParams(i, slot);
                //        this.Slots.Add(slot);
                //    }
                //}
            }
            catch (SaveDataException e)
            {
                Debug.LogError("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                Debug.LogError("Missing DLL Expection : " + e.Message);
                Debug.LogError("The sample APP will not run in the editor.");
            }
#endif
        }

        private void OnSaveDataEvent(SaveDataCallbackEvent callbackEvent)
        {
            switch (callbackEvent.ApiCalled)
            {
                case FunctionTypes.DirNameSearch: // 目录搜索结果事件
                    {
                        OnDirNameSearchReponse(callbackEvent.Response as Searching.DirNameSearchResponse);
                    }
                    break;
            }
        }

        public void Update()
        {
            //Unity.SaveData.PS5.Main.Update();
        }

        public void Close()
        {
            Unity.SaveData.PS5.Main.Terminate();
        }


        private void AddSlot(SearchSaveDataItem slot, bool hasParam, bool hasInfo)
        {
            Debug.Log("AddSlot : " + slot.DirName.Data);

            this.Slots.TryAdd(slot.DirName.Data,slot);
            var sd = SaveDataManager.Instance.NewSaveData(false);
            sd.DirName = slot.DirName.Data;
            if (hasParam)
            {
                sd.Title = slot.Params.Title;
                sd.SubTitle = slot.Params.SubTitle;
                sd.Detail = slot.Params.Detail;
                sd.Time = Time.GetTimestamp(slot.Params.Time);
                sd.Totaltime = slot.Params.UserParam;
            }
            if (hasInfo)
            {
                sd.TotalSize = slot.Info.Blocks;
                sd.FreeSize = slot.Info.FreeBlocks;
            }
        }


        public void LoadIndex(Action onIndexiesLoaded)
        {
            this.indexies.Clear();


            try
            {
                Searching.DirNameSearchRequest request = new Searching.DirNameSearchRequest();

                request.UserId = this.UserID;
                request.Async = false; // 同步/异步
                request.Key = Searching.SearchSortKey.Time;
                request.Order = Searching.SearchSortOrder.Ascending;
                request.IncludeBlockInfo = true;
                request.IncludeParams = true;
                request.MaxDirNameCount = this.Capacity >= 0 ? (uint)this.Capacity : Searching.DirNameSearchRequest.DIR_NAME_MAXSIZE;

                Searching.DirNameSearchResponse response = new Searching.DirNameSearchResponse();

                int requestId = Searching.DirNameSearch(request, response);

                if (!request.Async)
                    OnDirNameSearchReponse(response);

                Debug.Log("DirNameSearch Async : Request Id = " + requestId);
            }
            catch (SaveDataException e)
            {
                Debug.LogError("Exception : " + e.ExtendedMessage);
            }
            onIndexiesLoaded();
        }

        public void OnDirNameSearchReponse(Searching.DirNameSearchResponse response)
        {
            indexies.Clear();//.ClearAllNames();

            Slots.Clear();

            if (response != null)
            {
                bool hasParams = response.HasParams;
                bool hasInfo = response.HasInfo;

                var saveDataItems = response.SaveDataItems;

                Debug.Log("Search Found " + saveDataItems.Length + " saves");

                if (saveDataItems.Length == 0)
                {
                    Debug.Log("Search didn't find any saves for this user.");
                }

                for (int i = 0; i < saveDataItems.Length; i++)
                {
                    var dirName = saveDataItems[i].DirName;

                    this.AddSlot(saveDataItems[i], hasParams, hasInfo);
                }
            }
        }


        private Action<SaveDataResult, SaveData> SaveDataLoadedHandler;
        private Action<SaveDataResult> SaveDataSavedHandler;
        private Action<SaveDataResult> SaveDataDeleteHandler;

        public void LoadData(SaveData saveData, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            //this.Slots[index].fileName = name;
            this.SaveDataLoadedHandler = onSaveDataLoaded;
            //Unity.SaveData.PS5.SaveLoad.LoadGame(this.Slots[index], false);
        }

        public void SaveData(SaveData saveData, Action<SaveDataResult> onDataSaved)
        {
            // this.Slots[index].title = saveData.Title;
            // this.Slots[index].subTitle = saveData.SubTitle;
            // this.Slots[index].detail = saveData.Detail;
            // this.Slots[index].fileName = name;
            this.SaveDataSavedHandler = onDataSaved;
            //byte[] data = Proto.Protobuf.Serialize<SaveData>(saveData);
            //if (data == null)
            //{
            //    if (onDataSaved != null)
            //    {
            //        onDataSaved(SaveDataResult.InvalidData);
            //    }
            //    return;
            //}
            //Debug.LogFormat("PS4.Save.SaveGame:{0}:Size:{1} Hash:{2} LDC:{3}", index, data.Length, Hash.ComputeMd5Hash(data), saveData.Data.levelDatas.Count);
            //Unity.SaveData.PS5.SaveLoad.SaveGame(data, this.Slots[index], false);
        }

        public void DeleteData(string name, Action<SaveDataResult> onDataDeleted)
        {
            this.SaveDataDeleteHandler = onDataDeleted;
            //Unity.SaveData.PS5.SaveLoad.Delete(this.Slots[index], false);
        }


        //private void OnSavedGameDeleted(Messages.PluginMessage msg)
        //{
        //    Debug.LogFormat("PS4.Save.OnSavedGameDeleted:{0}:{1}", msg.type, msg.Text);
        //    if (this.SaveDataDeleteHandler != null)
        //    {
        //        this.SaveDataDeleteHandler(this.GetResult(msg));
        //    }
        //}

        //private void OnSavedGameLoaded(Messages.PluginMessage msg)
        //{
        //    byte[] data = Unity.SaveData.PS5.SaveLoad.GetLoadedGame();
        //    if (this.SaveDataLoadedHandler != null)
        //    {
        //        if (data == null || data.Length == 0)
        //        {
        //            Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:{1}", msg.type, "NoData");
        //            this.SaveDataLoadedHandler(this.GetResult(msg), null);
        //        }
        //        else
        //        {
        //            Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:Size:{1} Hash:{2}", msg.type, data.Length, Hash.ComputeMd5Hash(data));
        //            try
        //            {
        //                SaveData saveData = Proto.Protobuf.Deserialize<SaveData>(data);
        //                this.SaveDataLoadedHandler(this.GetResult(msg), saveData);
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:Deserialize Error:{1}", msg.type, ex.Message);
        //                this.SaveDataLoadedHandler(SaveDataResult.InvalidData, null);
        //            }
        //        }
        //    }
        //}

        //private void OnLoadNoData(Messages.PluginMessage msg)
        //{
        //    Debug.LogFormat("PS4.Save.OnLoadNoData:{0}", msg.Text);
        //    if (this.SaveDataLoadedHandler != null)
        //    {
        //        this.SaveDataLoadedHandler(SaveDataResult.NotFound, null);
        //    }
        //}

        //private void OnLoadError(Messages.PluginMessage msg)
        //{
        //    Debug.LogErrorFormat("PS4.Save.OnLoadError:{0} - {1}", msg.type, msg.Text);
        //    if (this.SaveDataLoadedHandler != null)
        //    {
        //        this.SaveDataLoadedHandler(SaveDataResult.IOError, null);
        //    }
        //}

        //private void OnSavedGameSaved(Messages.PluginMessage msg)
        //{
        //    Debug.LogFormat("PS4.Save.OnSavedGameSaved:{0}", msg.Text);
        //    if (this.SaveDataSavedHandler != null)
        //    {
        //        this.SaveDataSavedHandler(this.GetResult(msg));
        //    }
        //}

        //private void OnSaveError(Messages.PluginMessage msg)
        //{
        //    if (msg.type == Messages.MessageType.kSavedGame_NotSet) return;
        //    int sceResultcode = 0;
        //    if (msg.data != IntPtr.Zero)
        //    {
        //        sceResultcode = Marshal.ReadInt32(msg.data);
        //    }

        //    Debug.LogErrorFormat("PS4.Save.OnSaveError:{0} - 0x{1}:{2}",msg.type, sceResultcode.ToString("X"), msg.Text);
        //    if (this.SaveDataSavedHandler != null)
        //    {
        //        this.SaveDataSavedHandler(SaveDataResult.IOError);
        //    }
        //}

        //private void OnSavedGameCanceled(Messages.PluginMessage msg)
        //{
        //    Debug.LogFormat("PS4.Save.OnSavedGameCanceled:{0}", msg.Text);
        //    if (this.SaveDataSavedHandler != null)
        //    {
        //        this.SaveDataSavedHandler(SaveDataResult.IOError);
        //    }
        //}

        string GetIconPath()
        {
            string fsr = "";

#if UNITY_5_4_OR_NEWER && UNITY_PS4
            // Get the root directory of Unity. Necessary for located the icon location in bootloader operations.
            fsr = UnityEngine.PS4.Utility.GetFileSystemRoot();
#endif

            // WORKAROUND: Currently returns empty if in the root project or a non-bootloader project. This will be fixed.
            if (string.IsNullOrEmpty(fsr))
            {
                fsr = "/app0/";
            }

            return fsr + iconPathWithinUnity;
        }
    }
}
