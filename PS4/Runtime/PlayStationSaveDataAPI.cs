using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sony.PS4.SavedGame;

namespace OpenNGS.SaveData.PS4
{
    class PlayStationSaveDataAPI : ISaveDataAPI
    {
        public int Capacity { get; private set; }

        string iconPathWithinUnity = "Media/StreamingAssets/SaveIcon.png";

        IndexiesData indexies = new IndexiesData();

        List<SaveLoad.SavedGameSlotParams> Slots = new List<SaveLoad.SavedGameSlotParams>();

        public void Init(int capacity, int version)
        {
            this.Capacity = capacity;
            this.Slots.Clear();
            Sony.PS4.SavedGame.Main.OnLog += OnLog;
            Sony.PS4.SavedGame.Main.OnLogWarning += OnLogWarning;
            Sony.PS4.SavedGame.Main.OnLogError += OnLogError;

            Sony.PS4.SavedGame.SaveLoad.OnGameSaved += OnSavedGameSaved;
            Sony.PS4.SavedGame.SaveLoad.OnGameLoaded += OnSavedGameLoaded;
            Sony.PS4.SavedGame.SaveLoad.OnGameDeleted += OnSavedGameDeleted;
            Sony.PS4.SavedGame.SaveLoad.OnCanceled += OnSavedGameCanceled;
            Sony.PS4.SavedGame.SaveLoad.OnSaveError += OnSaveError;
            Sony.PS4.SavedGame.SaveLoad.OnLoadError += OnLoadError;
            Sony.PS4.SavedGame.SaveLoad.OnLoadNoData += OnLoadNoData;

            Sony.PS4.SavedGame.Main.Initialise();

            for (int i = 0; i < this.Capacity; i++)
            {
                SaveLoad.SavedGameSlotParams slot = new Sony.PS4.SavedGame.SaveLoad.SavedGameSlotParams();
                this.InitSlotParams(i, slot);
                this.Slots.Add(slot);
            }
        }

        private void OnLogError(Messages.PluginMessage msg)
        {
            Debug.LogErrorFormat("PS4.Save.OnLogError:{0}:{1}", msg.type, msg.Text);
        }

        private void OnLogWarning(Messages.PluginMessage msg)
        {
            Debug.LogWarningFormat("PS4.Save.OnLogWarning:{0}:{1}", msg.type, msg.Text);
        }

        private void OnLog(Messages.PluginMessage msg)
        {
            Debug.LogFormat("PS4.Save.OnLog:{0}:{1}", msg.type, msg.Text);
        }

        public void Update()
        {
            Sony.PS4.SavedGame.Main.Update();
        }

        public void Close()
        {
            Sony.PS4.SavedGame.Main.Terminate();
        }

        private void  InitSlotParams(int id, SaveLoad.SavedGameSlotParams slot)
        {
            slot.userId = 0;  // by passing a userId of 0 we use the default user that started the title
            slot.titleId = null; // by passing null we use the game's title id from the publishing settings
            slot.dirName = "savedata" + id;
            slot.fileName = "savedata";
            slot.title = "CROWN TRICK";
            slot.newTitle = "[newTitle]PS4 AutoSaveData";
            slot.subTitle = "[SubTitle]CrownTrick SaveData";
            slot.detail = "[detail]The autosave file for the Save Game test project ";
            slot.searchPath = "savedata%%";
            slot.noSpaceSysMsg = Sony.PS4.SavedGame.SaveLoad.DialogSysmsgType.NOSPACE_CONTINUABLE;
            slot.iconPath = GetIconPath();
        }

        public void LoadIndex(Action onIndexiesLoaded)
        {
            this.indexies.Slots.Clear();

            for(int i=0;i<this.Capacity;i++)
            {
                SaveLoad.SaveGameSlotDetails details;
                uint result = SaveLoad.GetDetails(this.Slots[i], out details);
                if(result ==0)
                {
                    Debug.LogFormat("Slot:{0} Title:{1} dirName:{2} fileName:{3} iconPath:{4} titleId:{5}", i, this.Slots[i].title, this.Slots[i].dirName, this.Slots[i].fileName, this.Slots[i].iconPath, this.Slots[i].titleId);
                    SaveSlot slot = new SaveSlot();
                    slot.Index = i;
                    slot.Name = details.title;
                    slot.Detail = details.detail;
                    slot.Status = SaveDataResult.Success;

                    TimeSpan ts = new DateTime((long)details.rtcTicktime * 10, DateTimeKind.Local) - new DateTime(1970, 1, 1);
                    slot.Time = (int)ts.TotalSeconds;
                    this.indexies.Slots.Add(i, slot);
                }
                else if (result == 0x809f0008)
                {
                    Debug.LogFormat("SaveData DOES NOT exist");
                }
                else
                {
                    Debug.LogFormat("SaveData Exists returned 0x{0:X} ", result);
                }
            }
            onIndexiesLoaded(this.indexies);
        }


        private Action<SaveDataResult, SaveData> SaveDataLoadedHandler;
        private Action<SaveDataResult> SaveDataSavedHandler;
        private Action<SaveDataResult> SaveDataDeleteHandler;

        public void LoadData(int index, string name, Action<SaveDataResult, SaveData> onSaveDataLoaded)
        {
            this.Slots[index].fileName = name;
            this.SaveDataLoadedHandler = onSaveDataLoaded;
            Sony.PS4.SavedGame.SaveLoad.LoadGame(this.Slots[index], false);
        }

        public void SaveData(int index, string name, SaveData saveData, Action<SaveDataResult> onDataSaved)
        {
            this.Slots[index].title = saveData.Title;
            this.Slots[index].subTitle = saveData.SubTitle;
            this.Slots[index].detail = saveData.Detail;
            this.Slots[index].fileName = name;
            this.SaveDataSavedHandler = onDataSaved;
            byte[] data = Proto.Protobuf.Serialize<SaveData>(saveData);
            if (data == null)
            {
                if (onDataSaved != null)
                {
                    onDataSaved(SaveDataResult.InvalidData);
                }
                return;
            }
            Debug.LogFormat("PS4.Save.SaveGame:{0}:Size:{1} Hash:{2} LDC:{3}", index, data.Length, Hash.ComputeMd5Hash(data), saveData.Data.levelDatas.Count);
            Sony.PS4.SavedGame.SaveLoad.SaveGame(data, this.Slots[index], false);
        }

        public void DeleteData(int index, string name, Action<SaveDataResult> onDataDeleted)
        {
            this.SaveDataDeleteHandler = onDataDeleted;
            Sony.PS4.SavedGame.SaveLoad.Delete(this.Slots[index], false);
        }

        private SaveDataResult GetResult(Messages.PluginMessage msg)
        {
            switch(msg.type)
            {
                case Messages.MessageType.kSavedGame_GameLoaded:
                    return SaveDataResult.Success;
                case Messages.MessageType.kSavedGame_GameSaved:
                    return SaveDataResult.Success;
                case Messages.MessageType.kSavedGame_GameDeleted:
                    return SaveDataResult.Success;

                default:
                    return SaveDataResult.NotFound;
            }
        }

        private void OnSavedGameDeleted(Messages.PluginMessage msg)
        {
            Debug.LogFormat("PS4.Save.OnSavedGameDeleted:{0}:{1}", msg.type, msg.Text);
            if (this.SaveDataDeleteHandler != null)
            {
                this.SaveDataDeleteHandler(this.GetResult(msg));
            }
        }

        private void OnSavedGameLoaded(Messages.PluginMessage msg)
        {
            byte[] data = Sony.PS4.SavedGame.SaveLoad.GetLoadedGame();
            if (this.SaveDataLoadedHandler != null)
            {
                if (data == null || data.Length == 0)
                {
                    Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:{1}", msg.type, "NoData");
                    this.SaveDataLoadedHandler(this.GetResult(msg), null);
                }
                else
                {
                    Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:Size:{1} Hash:{2}", msg.type, data.Length, Hash.ComputeMd5Hash(data));
                    try
                    {
                        SaveData saveData = Proto.Protobuf.Deserialize<SaveData>(data);
                        this.SaveDataLoadedHandler(this.GetResult(msg), saveData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogFormat("PS4.Save.OnSavedGameLoaded:{0}:Deserialize Error:{1}", msg.type, ex.Message);
                        this.SaveDataLoadedHandler(SaveDataResult.InvalidData, null);
                    }
                }
            }
        }

        private void OnLoadNoData(Messages.PluginMessage msg)
        {
            Debug.LogFormat("PS4.Save.OnLoadNoData:{0}", msg.Text);
            if (this.SaveDataLoadedHandler != null)
            {
                this.SaveDataLoadedHandler(SaveDataResult.NotFound, null);
            }
        }

        private void OnLoadError(Messages.PluginMessage msg)
        {
            Debug.LogErrorFormat("PS4.Save.OnLoadError:{0} - {1}", msg.type, msg.Text);
            if (this.SaveDataLoadedHandler != null)
            {
                this.SaveDataLoadedHandler(SaveDataResult.IOError, null);
            }
        }

        private void OnSavedGameSaved(Messages.PluginMessage msg)
        {
            Debug.LogFormat("PS4.Save.OnSavedGameSaved:{0}", msg.Text);
            if (this.SaveDataSavedHandler != null)
            {
                this.SaveDataSavedHandler(this.GetResult(msg));
            }
        }

        private void OnSaveError(Messages.PluginMessage msg)
        {
            if (msg.type == Messages.MessageType.kSavedGame_NotSet) return;
            int sceResultcode = 0;
            if (msg.data != IntPtr.Zero)
            {
                sceResultcode = Marshal.ReadInt32(msg.data);
            }

            Debug.LogErrorFormat("PS4.Save.OnSaveError:{0} - 0x{1}:{2}",msg.type, sceResultcode.ToString("X"), msg.Text);
            if (this.SaveDataSavedHandler != null)
            {
                this.SaveDataSavedHandler(SaveDataResult.IOError);
            }
        }

        private void OnSavedGameCanceled(Messages.PluginMessage msg)
        {
            Debug.LogFormat("PS4.Save.OnSavedGameCanceled:{0}", msg.Text);
            if (this.SaveDataSavedHandler != null)
            {
                this.SaveDataSavedHandler(SaveDataResult.IOError);
            }
        }

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
