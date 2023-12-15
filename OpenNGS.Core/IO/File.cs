using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.IO
{
    public static class File
    {
        public static byte[] ReadAllBytes(string path)
        {
            return FileSystem.Read(path);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            return FileSystem.ReadAllText(path, encoding);
        }

        public static string ReadAllText(string path)
        {
            return FileSystem.ReadAllText(path, Encoding.UTF8);
        }

    }
}
