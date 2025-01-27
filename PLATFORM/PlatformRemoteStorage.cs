using OpenNGS.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRemoteStorage
{
    public static int GetFileCount()
    {
        var i = Platform.GetRemoteStorage();
        if (i == null) return 0;
        return i.GetFileCount();
    }

    public static bool IsCloudEnabledForApp()
    {
        var i = Platform.GetRemoteStorage();
        if (i == null) return false;
        return Platform.GetRemoteStorage().IsEnabledForApp();
    }

    public static bool FileDelete(string fileName)
    {
        return Platform.GetRemoteStorage().FileDelete(fileName);
    }

    public static string GetFileNameAndSize(int i, out object _)
    {
        return Platform.GetRemoteStorage().GetFileNameAndSize(i, out _);
    }

    public static bool FileWrite(string saveFileName, byte[] fileData, int length)
    {
        throw new NotImplementedException();
    }

    public static int GetFileSize(string fileName)
    {
        throw new NotImplementedException();
    }

    public static int FileRead(string fileName, byte[] fileData, int fileSize)
    {
        throw new NotImplementedException();
    }
}
