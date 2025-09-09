using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.IO
{
    public interface IPathProvider
    {
        string CurrentPath { get; }
        /// <summary>
        /// This value is a directory path where you can store data that you want to be kept between runs. When you publish on iOS and Android, persistentDataPath points to a public directory on the device. Files in this location are not erased by app updates. The files can still be erased by users directly.
        /// </summary>
        string PersistentDataPath { get; }
        /// <summary>
        /// Contains the path to the game data folder on the target device (Read Only).
        /// The value depends on which platform you are running on:
        /// </summary>
        string DataPath { get; }

        string StreamingAssetsPath { get; }

        string LogPath { get; }
        string SavedGamePath { get; set; }
    }
}
