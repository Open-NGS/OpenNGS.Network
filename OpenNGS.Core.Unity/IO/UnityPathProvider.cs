using OpenNGS.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityPathProvider : IPathProvider
{
    public string CurrentPath => System.IO.Directory.GetCurrentDirectory();

    public string PersistentDataPath => Application.persistentDataPath;

    public string DataPath => Application.dataPath;

    public string StreamingAssetsPath => Application.streamingAssetsPath;

    public string LogPath => System.IO.Directory.GetCurrentDirectory();
}
public class UnityAndroidPathProvider : IPathProvider
{
    public string CurrentPath => System.IO.Directory.GetCurrentDirectory();

    public string PersistentDataPath => Application.persistentDataPath;

    public string DataPath => Application.persistentDataPath;

    public string StreamingAssetsPath => Application.streamingAssetsPath;

    public string LogPath => Application.persistentDataPath;
}

