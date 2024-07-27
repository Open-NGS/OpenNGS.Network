using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Build
{
    public static class FileParametric
    {

        private const string regexTag = "\\{(?<tag>\\w+?)\\}";
        public static bool Parametric(string template, string target, Dictionary<string, string> parameters)
        {
            string content = File.ReadAllText(template);

            List<string> tags = new List<string>();

            var matchs = System.Text.RegularExpressions.Regex.Matches(content, regexTag);
            foreach (System.Text.RegularExpressions.Match match in matchs)
            {
                if(match.Success && match.Groups.Count > 0)
                {
                    tags.Add(match.Groups["tag"].Value);
                }
            }

            List<string> unspecifiedTas = new List<string>();

            foreach (string tag in tags)
            {
                string val = "";
                if(parameters.TryGetValue(tag,out val))
                {
                    if (string.IsNullOrEmpty(val))
                    {
                        unspecifiedTas.Add(tag);
                        continue;
                    }
                    Console.WriteLine("Parametric {0} = {1}", tag, val);
                    content = content.Replace("{" + tag + "}", val);
                }
                else
                {
                    unspecifiedTas.Add(tag);
                }
            }

            if(unspecifiedTas.Count > 0)
                throw new Exception(string.Format("One or more parameters unspecified or value is null : [{0}]", string.Join(", ", unspecifiedTas)));

            File.WriteAllText(target, content);

            return true;
        }
    }
}
