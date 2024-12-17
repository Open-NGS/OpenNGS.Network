using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth.Credentials
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SecureStringHelper
    {
        internal unsafe static SecureString CreateSecureString(string plainString)
        {
            if (string.IsNullOrEmpty(plainString))
            {
                return new SecureString();
            }

            SecureString secureString;
            fixed (char* ptr = plainString)
            {
                char* value = ptr;
                secureString = new SecureString(value, plainString.Length);
                secureString.MakeReadOnly();
            }

            return secureString;
        }

        internal static string CreateString(SecureString secureString)
        {
            IntPtr intPtr = IntPtr.Zero;
            if (secureString == null || secureString.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                intPtr = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(intPtr);
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(intPtr);
                }
            }
        }
    }
}