using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenNGS.IO.Posix;
using OpenNGS.SaveData;
using OpenNGS.SaveData.File;
using UnityEngine;
using UnityEngine.TestTools;

public class SaveDataTests
{

    [global::ProtoBuf.ProtoContract()]
    public class GameData : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }

        [global::ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int Level { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public PlayerData Player { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public QuestData Quests { get; set; }

    }
    [global::ProtoBuf.ProtoContract()]
    public class PlayerData : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }

        [global::ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
        [global::ProtoBuf.ProtoMember(2)]
        public int Level { get; set; }
    }

    [global::ProtoBuf.ProtoContract()]
    public class QuestData : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        [global::ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }
        [global::ProtoBuf.ProtoMember(2)]
        public int Level { get; set; }
    }

    [global::ProtoBuf.ProtoContract()]
    public class SaveDataGame : SaveData, global::ProtoBuf.IExtensible
    {

        public SaveDataFilePB<GameData> GameData { get; set; }

        public SaveDataFileJson<QuestData> Quest { get; set; }

        public SaveDataGame() : base("SaveData")
        {
            GameData = new SaveDataFilePB<GameData>(this, "savedata.dat");
            Quest = new SaveDataFileJson<QuestData>(this, "quest.dat");
        }

        public SaveDataGame(string name) : base(name)
        {
            GameData = new SaveDataFilePB<GameData>(this, "savedata.dat");
            Quest = new SaveDataFileJson<QuestData>(this, "quest.dat");
        }
    }


    // A Test behaves as an ordinary method
    [Test]
    public void SaveDataTestsSimplePasses()
    {
        PosixFileSystem fileSystem = new PosixFileSystem();

        SaveDataManager.Initialize<SaveDataGame>("savedata",fileSystem, 1, SaveDataMode.Single); // 初始化存档引擎

        // 通过列表获取存档
        List<SaveData> list = SaveDataManager.Instance.Slots;
        Debug.Log($"Slots:{list.Count}");

        for(int i = 0; i < list.Count; i++) 
        {
            var sd = list[i];
            Debug.Log($"    SaveData[{i}] = [{sd.Title} - {sd.SubTitle}] {sd.Time} {sd.Status}");
        }

        // 从列表存档中加载
        //SaveDataGame data1 = list[0] as SaveDataGame;
        //if(!data1.Loaded)
        //    data1.Read();
        //data1.Save();


        SaveDataGame data = new SaveDataGame("savedata"); // 构造传入存档目录名
        data.Title = "This is Data";
        data.Detail = "This is Detail";
        data.SubTitle = "This is Subtitle";

        data.GameData.Value = new GameData();
        data.GameData.Value.Name = "Haha";
        data.GameData.Value.Level = 100;

        SaveDataManager.Instance.Save(data);// 按照结构保存

        //data.Save(); // 使用对象保存接口
        //data.GameData.Save();// 单独保存存档的部分内容。 // 当局内局外分离时可以使用单独保存机制。


        SaveDataManager.Instance.Load(data);// 按照结构加载

        Debug.Log($"Load SaveData[{data.Title} - {data.SubTitle}] {data.Time} {data.Status} GameData:{data.GameData.Value.Name}/{data.GameData.Value.Level}");

        // 保存新存档
        data.DirName = "savedata_new"; // 新的名字，不可重复
        //data.Save();

        SaveDataManager.Instance.Terminate(); // 游戏退出前执行最终清理

        // 使用系统UI的保存
        //SaveDataDialogManager.Instance.Save(data);// 按照结构保存
        //SaveDataDialogManager.Instance.Load(data);// 按照结构加载

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SaveDataTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
