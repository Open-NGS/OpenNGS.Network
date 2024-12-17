using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNGS.SDK.Auth.Credentials
{

    internal struct CREDENTIAL<T>
    {
        public int Flags;

        public int Type;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;

        public long LastWritten;

        public int CredentialBlobSize;

        public string CredentialBlob;

        public int Persist;

        public int AttributeCount;

        public string Attributes;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;

        public T User;
    }

    public class LocalStore<T>
    {
        static Dictionary<string, CREDENTIAL<T>> credentials = new Dictionary<string, CREDENTIAL<T>>();
        const string CredentialStore = ".ngs/auth/";
        const string CredentialStoreFile = "ng.store";

        private static string StorePath
        {
            get {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), CredentialStore);
            }
        }

        private static string StoreFile
        {
            get
            {
                return Path.Combine(StorePath, CredentialStoreFile);
            }
        }

        public static void Init()
        {
            System.Environment.GetEnvironmentVariable("", EnvironmentVariableTarget.Machine);
            if (!File.Exists(StoreFile))
            {
                Directory.CreateDirectory(StorePath);
            }
            else
            {
                var json = File.ReadAllText(StoreFile);
                credentials = JsonConvert.DeserializeObject<Dictionary<string, CREDENTIAL<T>>>(json);
            }
        }

        static bool Save()
        {
            var json = JsonConvert.SerializeObject(credentials);
            File.WriteAllText(StoreFile, json);
            return true;
        }

        internal static bool CredDelete(StringBuilder target, CredentialType type, int v)
        {
            credentials.Remove(target.ToString());
            return Save();
        }

        internal static bool CredRead(string target, CredentialType type, int v, out CREDENTIAL<T> credentialPtr)
        {
            return credentials.TryGetValue(target, out credentialPtr);
        }

        internal static bool CredWrite(ref CREDENTIAL<T> userCredential, uint v)
        {
            userCredential.LastWritten = DateTime.Now.ToFileTimeUtc();
            credentials[userCredential.TargetName] = userCredential;
            return Save();
        }
    }
}
