using System;
using OpenNGS.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnityFileSystem : IFileSystem
{

    public void Copy(string source, string destination)
    {
        throw new System.NotImplementedException();
    }

    public void CreateDirectory(string slotPath)
    {
        throw new System.NotImplementedException();
    }

    public bool Delete(string path)
    {
        throw new System.NotImplementedException();
    }

    public Stream OpenRead(String path)
    {
        throw new NotImplementedException();
    }

    public string ReadAllText(string path, System.Text.Encoding encoding)
    {
        throw new System.NotImplementedException();
    }

    public bool DirectoryExists(string filename)
    {
        throw new System.NotImplementedException();
    }

    public bool FileExists(string filename)
    {
        throw new System.NotImplementedException();
    }

    public bool MountCacheData(string mountName, bool readOnly)
    {
        throw new System.NotImplementedException();
    }

    public bool MountRom(string mountName)
    {
        throw new System.NotImplementedException();
    }

    public bool MountSaveData(string mountName, bool readOnly)
    {
        throw new System.NotImplementedException();
    }

    public void Move(string source, string destination)
    {
        throw new System.NotImplementedException();
    }

    public byte[] Read(string path)
    {
        throw new System.NotImplementedException();
    }

    public void Unmount()
    {
        throw new System.NotImplementedException();
    }

    public bool Write(string name, byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public bool Rename(string srcFileName, string destFileName)
    {
        throw new System.NotImplementedException();
    }

    public void Mount(string mountName, bool @readonly)
    {
        throw new NotImplementedException();
    }
}
