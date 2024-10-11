using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;
using System.Text;


namespace OpenNGS.SDK.Auth.Credentials
{
    public class Credential : IDisposable
    {
        private static object _lockObject;

        private bool _disposed;

        private static SecurityPermission _unmanagedCodePermission;

        private CredentialType _type;

        private string _target;

        private SecureString _password;

        private string _username;

        private string _description;

        private DateTime _lastWriteTime;

        private PersistanceType _persistanceType;

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public string Password
        {
            get
            {
                return SecureStringHelper.CreateString(SecurePassword);
            }
            set
            {
                CheckNotDisposed();
                SecurePassword = SecureStringHelper.CreateSecureString(string.IsNullOrEmpty(value) ? string.Empty : value);
            }
        }

        public UserInfo User;

        public SecureString SecurePassword
        {
            get
            {
                CheckNotDisposed();
                _unmanagedCodePermission.Demand();
                if (_password != null)
                {
                    return _password.Copy();
                }

                return new SecureString();
            }
            set
            {
                CheckNotDisposed();
                if (_password != null)
                {
                    _password.Clear();
                    _password.Dispose();
                }

                _password = ((value == null) ? new SecureString() : value.Copy());
            }
        }

        public string Target
        {
            get
            {
                CheckNotDisposed();
                return _target;
            }
            set
            {
                CheckNotDisposed();
                _target = value;
            }
        }

        public string Description
        {
            get
            {
                CheckNotDisposed();
                return _description;
            }
            set
            {
                CheckNotDisposed();
                _description = value;
            }
        }

        public DateTime LastWriteTime => LastWriteTimeUtc.ToLocalTime();

        public DateTime LastWriteTimeUtc
        {
            get
            {
                CheckNotDisposed();
                return _lastWriteTime;
            }
            private set
            {
                _lastWriteTime = value;
            }
        }

        public CredentialType Type
        {
            get
            {
                CheckNotDisposed();
                return _type;
            }
            set
            {
                CheckNotDisposed();
                _type = value;
            }
        }

        public PersistanceType PersistanceType
        {
            get
            {
                CheckNotDisposed();
                return _persistanceType;
            }
            set
            {
                CheckNotDisposed();
                _persistanceType = value;
            }
        }

        static Credential()
        {
            _lockObject = new object();
            lock (_lockObject)
            {
                _unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            }
        }

        public Credential()
            : this(null)
        {
        }

        public Credential(string username)
            : this(username, null)
        {
        }

        public Credential(string username, string password)
            : this(username, password, null)
        {
        }

        public Credential(string username, string password, string target)
            : this(username, password, target, CredentialType.Generic)
        {
        }

        public Credential(string username, string password, string target, CredentialType type)
        {
            Username = username;
            Password = password;
            Target = target;
            Type = type;
            PersistanceType = PersistanceType.Session;
            _lastWriteTime = DateTime.MinValue;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~Credential()
        {
            Dispose(disposing: false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                SecurePassword.Clear();
                SecurePassword.Dispose();
            }

            _disposed = true;
        }

        private void CheckNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Credential object is already disposed.");
            }
        }

        public bool Save()
        {
            CheckNotDisposed();
            _unmanagedCodePermission.Demand();
            byte[] bytes = Encoding.Unicode.GetBytes(Password);
            if (Password.Length > 4096)
            {
                throw new ArgumentOutOfRangeException("The password has exceeded 512 bytes.");
            }

            CREDENTIAL userCredential = default(CREDENTIAL);
            userCredential.TargetName = Target;
            userCredential.UserName = Username;
            userCredential.CredentialBlob = Convert.ToBase64String(bytes);
            userCredential.CredentialBlobSize = bytes.Length;
            userCredential.Comment = Description;
            userCredential.Type = (int)Type;
            userCredential.Persist = (int)PersistanceType;
            userCredential.User = User;
            LastWriteTimeUtc = DateTime.UtcNow;
            if (!LocalStore.CredWrite(ref userCredential, 0u))
            {
                return false;
            }
            return true;
        }

        public bool Delete()
        {
            CheckNotDisposed();
            _unmanagedCodePermission.Demand();
            if (string.IsNullOrEmpty(Target))
            {
                throw new InvalidOperationException("Target must be specified to delete a credential.");
            }

            StringBuilder target = (string.IsNullOrEmpty(Target) ? new StringBuilder() : new StringBuilder(Target));
            return LocalStore.CredDelete(target, Type, 0);
        }

        public bool Load()
        {
            CheckNotDisposed();
            _unmanagedCodePermission.Demand();
            if (!LocalStore.CredRead(Target, Type, 0, out CREDENTIAL CredentialPtr))
            {
                return false;
            }
            LoadInternal(CredentialPtr);
            return true;
        }

        public bool Exists()
        {
            CheckNotDisposed();
            _unmanagedCodePermission.Demand();
            if (string.IsNullOrEmpty(Target))
            {
                throw new InvalidOperationException("Target must be specified to check existance of a credential.");
            }

            Credential credential = new Credential();
            credential.Target = Target;
            credential.Type = Type;
            using Credential credential2 = credential;
            return credential2.Load();
        }

        internal void LoadInternal(CREDENTIAL credential)
        {
            Username = credential.UserName;
            if (credential.CredentialBlobSize > 0)
            {
                Password = Encoding.Unicode.GetString(Convert.FromBase64String(credential.CredentialBlob));
            }

            Target = credential.TargetName;
            Type = (CredentialType)credential.Type;
            PersistanceType = (PersistanceType)credential.Persist;
            Description = credential.Comment;
            LastWriteTimeUtc = DateTime.FromFileTimeUtc(credential.LastWritten);
            User = credential.User;
        }
    }
}