using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Build
{
    public class CommandLine
    {

        private static Dictionary<string, string> arguments = new Dictionary<string, string>();

        public static Dictionary<string, string> Arguments
        {
            get
            {
                if (arguments.Count == 0)
                {
                    ParseArgs();
                }
                return arguments;
            }
        }
        public static void ParseArgs()
        {
            arguments.Clear();
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key.StartsWith("-"))
                {
                    key= key.TrimStart('-');
                    string value = i + 1 < args.Length ? args[i + 1] : "";
                    if (value.StartsWith("-"))
                        arguments[key] = "";
                    else
                    {
                        arguments[key] = value;
                        i++;
                    }
                }
            }
        }

        public static string GetArgument(string name, string defVal = null)
        {
            string value = "";
            if (Arguments.TryGetValue(name, out value))
                return value;
            return defVal;
        }
    }
}
