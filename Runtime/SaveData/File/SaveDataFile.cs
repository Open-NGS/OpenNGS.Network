using OpenNGS.IO;

namespace OpenNGS.SaveData.File
{
    public class SaveDataFile
    {
        protected SaveData savedata;

        protected string filename;

        public SaveDataFile(SaveData save, string filename) 
        {
            this.savedata = save;
            this.filename = filename;
            save.Files.Add(this);
        }

        protected virtual void Init() { }

        internal bool Write(IFileSystem fsSave, string folder)
        {
            byte[] data = this.GetData();
            //if (SaveDataManager.Instance.Encrypt)
            {
/*              string key = savedata.Magic.ToString("X8");
                buff = AES.Encrypt(data, key, key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(buff.Length + 4);
                ms.Write(BitConverter.GetBytes(buff.Length), 0, 4);
                ms.Write(buff, 0, buff.Length);
                ms.Flush();
                buff = ms.ToArray();
*/
            }

            string basename = Path.Combine(folder, filename);
#if UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_XBOX
            bool result = fsSave.Write(basename, data);
#else
            string rawFile = basename;
            string cacheFile = basename + ".1";
            string backupFile = basename + ".2";
            string tempFile = basename + ".3";

            bool result = fsSave.Write(tempFile, data);

            if (result)
            {
                fsSave.Copy(tempFile, rawFile);
                fsSave.Move(tempFile, cacheFile);
            }
#endif
            return true;
        }


        protected virtual byte[] GetData()
        {
            return null;
        }

        protected virtual void SetData(byte[] data)
        {
        }

        internal SaveDataResult Read(IFileSystem fsSave, string folder)
        {
            string basename = Path.Combine(folder, filename);

            byte[] data = null;
#if false && (UNITY_SWITCH || UNITY_PLAYSTATION || UNITY_XBOX)
            data = fsSave.Read(basename);
            this.SetData(data);
            return SaveDataResult.Success;
#else
            string rawFile = basename;
            string cacheFile = basename + ".1";
            string backupFile = basename + ".2";
            string tempFile = basename + ".3";

            bool rawExist = fsSave.FileExists(rawFile);
            bool cacheExist = fsSave.FileExists(cacheFile);
            bool backupExist = fsSave.FileExists(backupFile);

            if (!rawExist && !cacheExist && !backupExist)
            {
                return SaveDataResult.NotFound;
            }

            SaveDataResult result = SaveDataResult.NotFound;
            if (rawExist)
            {
                data = fsSave.Read(rawFile);
                if (data != null)
                {
                    this.SetData(data);
                    fsSave.Copy(rawFile, backupFile);
                    return SaveDataResult.Success;
                }
                return SaveDataResult.InvalidData;
            }
            if (cacheExist)
            {
                data = fsSave.Read(cacheFile);
                if (data != null)
                {
                    this.SetData(data);
                    fsSave.Copy(cacheFile, rawFile);
                    return SaveDataResult.Recovered;
                }
                return SaveDataResult.InvalidData;
            }
            if (backupExist)
            {
                data = fsSave.Read(backupFile);
                if (data != null)
                {
                    this.SetData(data);
                    fsSave.Copy(backupFile, rawFile);
                    return SaveDataResult.Recovered;
                }
                return SaveDataResult.InvalidData;
            }
            return result;
#endif
        }
    }
}
