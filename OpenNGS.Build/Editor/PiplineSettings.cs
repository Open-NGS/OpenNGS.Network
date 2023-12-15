using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace OpenNGS.Build
{
    public static class VersionExtension
    {
        public static bool IsZero(this Version self)
        {
            return (self.Major + self.Minor + self.Build + self.Revision) == 0;
        }
    }

    public enum BuildType
    {
        Release,
        Debug,
    }
    internal class PiplineSettings
    {
        private static BuildTarget buildTarget = 0;
        public static BuildTarget BuildTarget
        {
            get
            {
                if (buildTarget == 0)
                {
                    string target = CommandLine.GetArgument("buildTarget");
                    if (!string.IsNullOrEmpty(target))
                    {
                        if (Enum.TryParse<BuildTarget>(target, out buildTarget))
                        {
                            return buildTarget;
                        }
                        else
                        {
                            throw new ArgumentException("Commandline argument error: " + target + " is invalid buildTarget", "buildTarget");
                        }
                    }
                    buildTarget = BuildTarget.NoTarget;
                }
                return buildTarget;
            }
        }
        public static string Branch { get { return CommandLine.GetArgument("branch"); } }
        public static string OutputPath { get { return CommandLine.GetArgument("outputPath"); } }

        private static Version buildversion = null;
        public static Version BuildVersion
        {
            get
            {
                if (buildversion == null)
                {
                    buildversion = new Version(CommandLine.GetArgument("buildVersion", "0.0.0.0"));
                }
                return buildversion;
            }
        }

        public static string ResVersion { get { return CommandLine.GetArgument("resVersion"); } }
        public static string BundleVersion { get { return CommandLine.GetArgument("bundleVersion"); } }
        public static string CompanyName { get { return CommandLine.GetArgument("companyName"); } }
        public static string ProductName { get { return CommandLine.GetArgument("productName"); } }
        public static int BuildVersionCode { get { return int.Parse(CommandLine.GetArgument("buildVersionCode", "0")); } }
        public static string AppIdentifier { get { return CommandLine.GetArgument("appIdentifier"); } }
        public static string ExtraScriptingDefines { get { return CommandLine.GetArgument("extraScriptingDefines"); } }
        public static BuildType BuildType { 
            get {
                string str = CommandLine.GetArgument("buildType", "Debug");
                if (str == "Release")
                {
                    return BuildType.Release;
                }
                else
                {
                    return BuildType.Debug;
                }
            } 
        }
        public static string ChannelID { get { return CommandLine.GetArgument("channelID"); } }
        public static string Region { get { return CommandLine.GetArgument("region"); } }
        public static string BuildEnvironment { get { return CommandLine.GetArgument("buildEnv"); } }
        public static string PlatformTemplatePath => CommandLine.GetArgument("platformPath", "");
        public static string BuiltinScenes { get { return CommandLine.GetArgument("builtinScenes"); } }

        public static bool ForceASTC { get { return CommandLine.GetArgument("astc") != null; } }

        public static bool SplitOBB { get { return CommandLine.GetArgument("obb") != null; } }
    }
}