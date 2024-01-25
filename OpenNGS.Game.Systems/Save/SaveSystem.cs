using OpenNGS.SaveData;
using OpenNGS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : EntitySystem
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    public override string GetSystemName()
    {
        return "com.openngs.system.save";
    }

    public bool SaveData<T>(T data, string name)
    {
        return false;
    }

    //public T LoadData<T>(string name)
    //{
    //    return data;
    //}
}
