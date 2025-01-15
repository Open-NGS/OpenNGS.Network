using System;
namespace OpenNGS.Platform
{
	/// <summary>
	/// MSDK log
	/// </summary>
	class PlatformLog
	{
		/// <summary>
		/// MSDK SDK Log level
		/// </summary>
		public enum Level
		{
			None = 0,
			Log,
			Warning,
			Error
		}

		/// <summary>
		/// The level, Error by default
		/// </summary>
        private static Level level = Level.Log;

		private const string header = "MSDK : ";

		/// <summary>
		/// Sets the level.
		/// </summary>
		/// <param name="l">Level</param>
		public static void SetLevel (Level l)
		{
			level = l;
		}

		/// <summary>
		/// Log the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Log (string message)
		{
			if (level <= Level.Log) {
				UnityEngine.Debug.Log (header + message);
			}
		}

		/// <summary>
		/// Warning the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void LogWarning (string message)
		{
			if (level <= Level.Warning) {
				UnityEngine.Debug.LogWarning (header + message);
			}
		}

		/// <summary>
		/// Error the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void LogError (string message)
		{
			if (level <= Level.Error) {
				UnityEngine.Debug.LogError (header + message);
			}
		}

		public static void FullLog (string message)
		{
			try {
				var counter = 0;
				const int step = 512;
				var all = message.Length;
				while (counter < all) {
					var start = counter;
					var length = start + step > all ? all - start : step;
					//UnityEngine.Debug.LogError(header + "all : " + all + " start : " + start + " length : " +  length);
					UnityEngine.Debug.Log (header + message.Substring (start, length));
					counter += step;
				}
			} catch (Exception e) {
				UnityEngine.Debug.LogWarning (e.Message);
			}
		}
	}
}
