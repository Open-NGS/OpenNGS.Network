using OpenNGS.Platform;

public class PlatformRemoteStorage
{
    public static int GetFileCount()
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return 0;
        return m.GetFileCount();
    }

    public static bool IsCloudEnabledForApp()
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return false;
        return m.IsEnabledForApp();
    }

    public static bool FileDelete(string fileName)
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return false;
        return m.FileDelete(fileName);
    }

    public static string GetFileNameAndSize(int i, out object _)
    {
        var m = Platform.GetRemoteStorage();
        if (m == null)
        {
            _ = 0;
            return null;
        }
        return m.GetFileNameAndSize(i, out _);
    }

    public static bool FileWrite(string saveFileName, byte[] fileData, int length)
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return false;
        return m.FileWrite(saveFileName, fileData, length);
    }

    public static int GetFileSize(string fileName)
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return -1;
        return m.GetFileSize(fileName);
    }

    public static int FileRead(string fileName, byte[] fileData, int fileSize)
    {
        var m = Platform.GetRemoteStorage();
        if (m == null) return -1;
        return m.FileRead(fileName, fileData, fileSize);
    }
}
