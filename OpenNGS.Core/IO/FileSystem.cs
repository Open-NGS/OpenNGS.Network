using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenNGS.IO.Posix;

namespace OpenNGS.IO
{
    public enum FileSystemType
    {
        Rom,
        Cache,
        SaveData,
        Host,
    }

    public static class FileSystem
    {
        private static IFileSystem fileSystem;
        private static IPathProvider pathProvider;

        public static string PersistentDataPath => pathProvider.PersistentDataPath;

        public static string DataPath => pathProvider.DataPath;

        public static string StreamingAssetsPath => pathProvider.StreamingAssetsPath;

        public static string LogPath => pathProvider.LogPath;

        public static string SavedGamePath => pathProvider.SavedGamePath;


        public static void Init(IFileSystem fs, IPathProvider path)
        {
            if (fs == null)
            {
#if UNITY_SWITCH
                fileSystem = new Switch.NXFileSystem();
#elif UNITY_ANDROID
                fileSystem = new AndroidPosixFileSystem();
#else
                fileSystem = new Posix.PosixFileSystem();
#endif
            }
            else
            {
                fileSystem = fs;
            }

            if (path == null)
                pathProvider = new Posix.DefaultPathProvider();
            else
                pathProvider = path;
        }

       
		
        public static bool FileExists(string filename)
        {
            return fileSystem.FileExists(filename);
        }

        public static bool DirectoryExists(string filename)
        {
            return fileSystem.DirectoryExists(filename);
        }

        public static byte[] Read(string path)
        {
            return fileSystem.Read(path);
        }
        
        public static string ReadAllText(string path, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return fileSystem.ReadAllText(path, encoding);
        }

        public static bool Write(string name, byte[] data)
        {
            return fileSystem.Write(name, data);
        }
        public static bool Delete(string path)
        {
            return fileSystem.Delete(path);
        }

        public static void CreateDirectory(string path)
        {
            fileSystem.CreateDirectory(path);
        }
        public static void Copy(string source, string destination)
        {
            fileSystem.Copy(source, destination);
        }
        public static void Move(string source, string destination)
        {
            fileSystem.Move(source, destination);
        }

        public static bool Rename(string srcFileName, string destFileName)
        {
            return fileSystem.Rename(srcFileName, destFileName);
        }

        public static Stream OpenRead(string filepath)
        {
            return fileSystem.OpenRead(filepath);
        }
    }
}
