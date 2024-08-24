using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using OpenNGS.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public class PackageTool
{


   static Dictionary<string, Dictionary<string, List<string>>> m_Packages = new Dictionary<string, Dictionary<string, List<string>>>();


    class PackageItem
    {

        public string PackageName;
        public string Folder;
        public string Filename;
        public string file;

        public PackageItem(string file)
        {
            //Packages/openngs.ui\Runtime\UI\UGUI\Controls\RecyclableScrollRect\Interfaces\IRecyclableScrollRectDataSource.cs
            this.file = file;
            this.PackageName = Regex.Match(file, "(?<=Packages/).+?(?=\\\\)").Value;
            file = file.Remove(0, this.PackageName.Length + 10);
            file = file.Replace("Runtime\\", "");
            Folder = Path.GetDirectoryName(file);
            this.Filename = Path.GetFileName(file);
        }

        public override string ToString()
        {
            return $"{PackageName},{Folder},{Filename}";
        }
    }

    [MenuItem("OpenNGS/Packages/Package List")]
    // Start is called before the first frame update
    static void ShowPackageList()
    {

        string[] allfiles = Directory.GetFiles("Packages/", "*.cs", SearchOption.AllDirectories);

        List< PackageItem > packages = new List< PackageItem >();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string file in allfiles)
        {
            if (file.Contains("openngs"))
            {
                var pkg = new PackageItem(file);
                Debug.Log(pkg);
                stringBuilder.AppendLine(pkg.ToString());
                AddPackage(pkg);
            }
        }
        var json = JsonConvert.SerializeObject(m_Packages, Formatting.Indented);
        File.WriteAllText("Packages/openngslist.json", json);
        File.WriteAllText("Packages/openngslist.txt", stringBuilder.ToString());
        Debug.Log(json);

    }

    static void AddPackage(PackageItem item)
    {
        Dictionary<string, List<string>> folders;

        if (!m_Packages.TryGetValue(item.PackageName, out folders))
        {
            folders = new Dictionary<string, List<string>>();
            m_Packages[item.PackageName] = folders;
        }

        List<string> files;
        if (!folders.TryGetValue(item.Folder, out files))
        {
            files = new List<string>();
            folders[item.Folder] = files;
        }
        files.Add(item.Filename);
    }
}
