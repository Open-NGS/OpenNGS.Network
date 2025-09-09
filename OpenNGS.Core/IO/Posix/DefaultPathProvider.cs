using OpenNGS.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.IO.Posix
{
    class DefaultPathProvider : IPathProvider
    {
        public string CurrentPath => "";

        public string PersistentDataPath => "";

        public string DataPath => "";

        public string StreamingAssetsPath => "";

        public string LogPath => "";

        public string SavedGamePath { get => ""; set => throw new NotImplementedException(); }
    }
}
