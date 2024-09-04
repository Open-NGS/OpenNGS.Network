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

namespace OpenNGS.SaveData.PS5
{
    class PlayStationSaveDataAPI : ISaveDataAPI
    {
        public int Capacity { get; private set; }

        string iconPathWithinUnity = "Media/StreamingAssets/SaveIcon.png";

        List<SaveData> indexies = new List<SaveData>();

        List<Unity.SaveData.PS5.Info.SaveDataParams> Slots = new List<Unity.SaveData.PS5.Info.SaveDataParams>();

        public void Init(IFileSystem fs, int capacity, SaveDataMode mode)
        {
            this.Capacity = capacity;
            this.Slots.Clear();
            Unity.SaveData.PS5.Main.OnAsyncEvent += OnSaveDataEvent;

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
                for (int i = 0; i < this.Capacity; i++)
                {
                    Unity.SaveData.PS5.Info.SaveDataParams slot = new Unity.SaveData.PS5.Info.SaveDataParams();
                    this.InitSlotParams(i, slot);
                    this.Slots.Add(slot);
                }
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

        private void OnSaveDataEvent(SaveDataCallbackEvent npEvent)
        {

        }

        public void Update()
        {
            //Unity.SaveData.PS5.Main.Update();
        }

        public void Close()
        {
            Unity.SaveData.PS5.Main.Terminate();
        }


        private void  InitSlotParams(int id, Unity.SaveData.PS5.Info.SaveDataParams slot)
        {
            slot.UserParam = 0;//.userId = 0;  // by passing a userId of 0 we use the default user that started the title
            //slot.titleId = null; // by passing null we use the game's title id from the publishing settings
            //slot.dirName = "savedata" + id;
            //slot.fileName = "savedata";
            slot.Title = "CROWN TRICK";
            //slot.newTitle = "[newTitle]PS4 AutoSaveData";
            slot.SubTitle = "[SubTitle]CrownTrick SaveData";
            slot.Detail = "[detail]The autosave file for the Save Game test project ";
            //slot.searchPath = "savedata%%";
            //slot.noSpaceSysMsg = Unity.SaveData.PS5.SaveLoad.DialogSysmsgType.NOSPACE_CONTINUABLE;
            //slot.iconPath = GetIconPath();
        }

        public void LoadIndex(Action onIndexiesLoaded)
        {
            this.indexies.Clear();

            for(int i=0;i<this.Capacity;i++)
            {
                //SaveLoad.SaveGameSlotDetails details;
                //uint result = SaveLoad.GetDetails(this.Slots[i], out details);
                //if(result ==0)
                //{
                //    Debug.LogFormat("Slot:{0} Title:{1} dirName:{2} fileName:{3} iconPath:{4} titleId:{5}", i, this.Slots[i].title, this.Slots[i].dirName, this.Slots[i].fileName, this.Slots[i].iconPath, this.Slots[i].titleId);
                //    SaveSlot slot = new SaveSlot();
                //    slot.Index = i;
                //    slot.Name = details.title;
                //    slot.Detail = details.detail;
                //    slot.Status = SaveDataResult.Success;

                //    TimeSpan ts = new DateTime((long)details.rtcTicktime * 10, DateTimeKind.Local) - new DateTime(1970, 1, 1);
                //    slot.Time = (int)ts.TotalSeconds;
                //    this.indexies.Slots.Add(i, slot);
                //}
                //else if (result == 0x809f0008)
                //{
                //    Debug.LogFormat("SaveData DOES NOT exist");
                //}
                //else
                //{
                //    Debug.LogFormat("SaveData Exists returned 0x{0:X} ", result);
                //}
            }
            onIndexiesLoaded();
        }


        private Action<SaveDataResult, SaveData> SaveDataLoadedHandler;
        private Action<SaveDataResult> SaveDataSavedHandler;
        private Action<SaveDataResult> SaveDataDeleteHandler;

        public void LoadData(SaveData saveData, string name, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            //this.Slots[index].fileName = name;
            this.SaveDataLoadedHandler = onSaveDataLoaded;
            //Unity.SaveData.PS5.SaveLoad.LoadGame(this.Slots[index], false);
        }

        public void SaveData(SaveData saveData, string name, Action<SaveDataResult> onDataSaved)
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
