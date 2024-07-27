
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using OpenNGS;


public static class IOUtility
{
    public static string LocalStoragePath
    {
        get
        {
            //  Debug.Log("Application.persistentDataPath:" + Application.persistentDataPath);
#if UNITY_IOS
            return Application.temporaryCachePath+"/Update";

#else
            return Application.persistentDataPath+"/Update";
#endif
        }
    }

    public static byte[] ReadResFile(string pathOnRes, string reletivePath = null, string fileName = null)
    {
        byte[] rawBytes = null;
        if (reletivePath != null && fileName != null)
        {
            rawBytes = ReadFile(reletivePath, fileName);
        }

        if (rawBytes == null)
        {
            TextAsset textAsset = Resources.Load(pathOnRes) as TextAsset;

            if (textAsset != null)
            {
                rawBytes = textAsset.bytes;
                Resources.UnloadAsset(textAsset);
            }
        }

        return rawBytes;
    }

    public static byte[] ReadFile(string reletivePath, string fileName)
    {
#if UNITY_WEBPLAYER
        return null;
#else
        try
        {
            string url = string.Format("{0}/{1}/{2}", LocalStoragePath, reletivePath, fileName);
            if (File.Exists(url))
            {
                return File.ReadAllBytes(url);
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex.ToString());
            return null;
        }
#endif
    }

    public static void ClearDirectory(string pathname)
    {
        string[] files = Directory.GetFiles(pathname);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }

        string[] dirs = System.IO.Directory.GetDirectories(pathname);
        for (int i = 0; i < dirs.Length; i++)
        {
            Directory.Delete(dirs[i], true);
        }
    }

    public static void MakeFileWriteable(string fileName)
    {
        if (File.Exists(fileName))
        {
            if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(fileName, FileAttributes.Normal);
            }
        }
    }

    public static void MakeDirectoryWriteable(string pathName)
    {
        if (Directory.Exists(pathName))
        {
  
            string[] files = Directory.GetFiles(pathName, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                MakeFileWriteable(file);
            }

            DirectoryInfo DirInfo = new DirectoryInfo(pathName)
            {
                Attributes = FileAttributes.Normal & FileAttributes.Directory
            };
        }
    }

    public static void MakeDirectoryReadable(string pathName)
    {
        if (Directory.Exists(pathName))
        {

            string[] files = Directory.GetFiles(pathName, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                MakeFileReadable(file);
            }

            DirectoryInfo DirInfo = new DirectoryInfo(pathName)
            {
                Attributes = FileAttributes.ReadOnly & FileAttributes.Directory
            };
        }
    }

    public static void MakeFileReadable(string fileName)
    {
        if (File.Exists(fileName))
        {
            if ((File.GetAttributes(fileName) & FileAttributes.Normal) == FileAttributes.Normal)
            {
                File.SetAttributes(fileName, FileAttributes.ReadOnly);
            }
        }
    }


    public static void CleanCache()
    {
        //Caching.ClearCache();
        //PlayerPrefs.DeleteAll();

        try
        {
            if (!Directory.Exists(LocalStoragePath))
            {
                Directory.CreateDirectory(LocalStoragePath);
            }

            string[] fileList = Directory.GetFiles(LocalStoragePath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < fileList.Length; i++)
            {
                if (File.Exists(fileList[i]))
                {
                    File.Delete(fileList[i]);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("CleanCache: " + ex.ToString());
        }
    }


}
