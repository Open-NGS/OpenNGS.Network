using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.IO
{
    public class Path
    {

        public static readonly char AltDirectorySeparatorChar = '/';
        public static readonly char DirectorySeparatorChar = '\\';


        public static string Combine(string path1, string path2, string path3 = null)
        {
            if (path1 == null || path2 == null)
            {
                throw new ArgumentNullException((path1 == null) ? "path1" : "path2");
            }
            if (path2.Length == 0)
            {
                return path1;
            }
            if (path1.Length == 0)
            {
                return path2;
            }
            if (System.IO.Path.IsPathRooted(path2))
            {
                return path2;
            }
            char c = path1[path1.Length - 1];
            if (c != System.IO.Path.DirectorySeparatorChar && c != System.IO.Path.AltDirectorySeparatorChar && c != System.IO.Path.VolumeSeparatorChar)
            {
                path1 = path1 + System.IO.Path.AltDirectorySeparatorChar + path2;
            }
            else
                path1 = path1 + path2;

            if (path3 != null && path3.Length > 0)
            {
                c = path1[path1.Length - 1];
                if (c != System.IO.Path.DirectorySeparatorChar && c != System.IO.Path.AltDirectorySeparatorChar && c != System.IO.Path.VolumeSeparatorChar)
                {
                    return path1 + System.IO.Path.AltDirectorySeparatorChar + path3;
                }
            }
            return path1 + path3;
        }
    }
}
