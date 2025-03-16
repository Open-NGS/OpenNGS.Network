using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Networking;
#endif

namespace OpenNGS.IO.Posix
{
    public class PosixFileSystem : IFileSystem
    {

        public bool Mounted { get { return true; } }

        public string MountName { get; private set; }

        public void Mount(string mountName, bool @readonly)
        {
            this.MountName = mountName;
        }


        public bool MountCacheData(string mountName, bool readOnly)
        {
            this.MountName = mountName;
            return true;
        }

        public bool MountRom(string mountName)
        {
            this.MountName = mountName;
            return true;
        }

        public bool MountSaveData(string mountName, bool readOnly)
        {
            this.MountName = mountName;
            return true;
        }

        public void Unmount()
        {

        }

        public virtual bool FileExists(string filename)
        {
            var fi = System.IO.File.Exists(filename);
            if (fi)
            {
                return true;
            }
#if DEBUG_LOG
            Debug.Log($"[PosixFileSystem]:FileExist: file does not exist file:" + filename);
#endif
            return false;
        }

        public bool DirectoryExists(string dirname)
        {
            return System.IO.Directory.Exists(dirname);
        }

        public virtual byte[] Read(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }

        public virtual bool Write(string name, byte[] data)
        {
            try
            {
                System.IO.File.WriteAllBytes(name, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateDirectory(string dirname)
        {
            System.IO.Directory.CreateDirectory(dirname);
        }

        public void Copy(string source, string destination)
        {
            string src = source;
            string dst = destination;
            ResetAttribute(src);
            ResetAttribute(dst);
            System.IO.File.Copy(src, dst, true);
        }

        private void ResetAttribute(string file)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(file);
            if (fi.Exists)
            {
                fi.Attributes = System.IO.FileAttributes.Normal;
            }
        }

        public void Move(string source, string destination)
        {
            Copy(source, destination);
            System.IO.File.Delete(source);
        }

        public bool Rename(string srcFileName, string destFileName)
        {
            if (FileExists(destFileName))
            {
                return false;
            }

            System.IO.File.Move(srcFileName, destFileName);
            return true;
        }

        public bool Delete(string path)
        {
            try
            {
                System.IO.File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual Stream OpenRead(string path)
        {
            return System.IO.File.OpenRead(path);
        }

        public virtual string ReadAllText(string path, Encoding encoding)
        {
            return System.IO.File.ReadAllText(path, encoding);
        }
    }

#if UNITY_ANDROID
    public class AndroidPosixFileSystem : PosixFileSystem
    {

        public override bool FileExists(string filename)
        {
            if (filename.StartsWithFast(OpenNGS.IO.FileSystem.DataPath))
            {
                if (System.IO.File.Exists(filename))
                    return true;
            }

            using (UnityWebRequest web = UnityWebRequest.Get(filename))
            {
                web.SendWebRequest();
                while (!web.isDone)
                {

                }
                if (web.result == UnityWebRequest.Result.Success) return true;
            }
#if DEBUG_LOG
            Debug.Log($"[AndroidPosixFileSystem]:FileExist: file does not exist file:" + filename);
#endif
            return false;
        }

        public override byte[] Read(string path)
        {
            if (path.StartsWithFast(OpenNGS.IO.FileSystem.DataPath))
            {
                return System.IO.File.ReadAllBytes(path);
            }
            using (UnityWebRequest rq = UnityWebRequest.Get(path))
            {
                rq.SendWebRequest();
                while (!rq.isDone)
                {

                }
                return rq.downloadHandler.data;
            }
        }

        public override bool Write(string name, byte[] data)
        {
            try
            {
                System.IO.File.WriteAllBytes(name, data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override Stream OpenRead(string path)
        {

            var data = this.Read(path);
            return new MemoryStream(data);

        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            var data = this.Read(path);
            if (data == null) return null;
            var str = encoding.GetString(data);
            return str;
        }
    }
#endif
}
