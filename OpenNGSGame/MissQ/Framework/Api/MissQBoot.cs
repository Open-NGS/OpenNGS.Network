

namespace MissQ
{
    public class MissQBoot
    {
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static readonly MissQBoot mInstance = new MissQBoot();

        /// <summary>
        /// Initialize Load MissQ AttributeData 
        /// </summary>
        public void Initialize ()
		{
            // 读取动作数据
            //AnimatorManager.mInstance.LoadAttributes();

            // 初始化
            //ServiceLocator.Init();
            //ServiceLocator.SetLogger(new MissQLogger());
        }

        /// <summary>
        /// Gets the miss Q version.
        /// </summary>
        /// <returns>The miss Q version.</returns>
        public string GetMissQVersion ()
		{
			return Version.VersionNumber;
		}

        public string GetMissQResVersion()
        {
            return Version.ResVersionNumber;
        }

		/// <summary>
		/// Gets the version number U int64.
		/// </summary>
		/// <returns>The version number U int64.</returns>
		/// <param name="version">Version.</param>
		public ulong GetMissQVersionNumUInt64 ()
		{
            return Version.GetVersionNumUInt64 (Version.VersionNumber);
		}

        /// <summary>
        /// Gets the version number U int64.
        /// </summary>
        /// <returns>The version number U int64.</returns>
        /// <param name="version">Version.</param>
        public ulong GetMissQResVersionNumberUInt64()
        {
            return Version.GetVersionNumUInt64(Version.ResVersionNumber);
        }
    }
}