using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore;
public class unique_chars : MonoBehaviour
{
    void Start()
    {
        UniqueChars();
    }

    static void UniqueChars()
    {
        string parentDir = Directory.GetParent(Path.GetDirectoryName(Application.dataPath)).FullName;
        string csvFilePath = Path.Combine(parentDir, "Data", "Game", "Localization", "StringTable.csv");
        Debug.Log(csvFilePath);

        string[] lines = File.ReadAllLines(csvFilePath);

        HashSet<char> uniqueChars = new HashSet<char>();


        string additionalCharacters = "%./0123456789:<=>ABCDEFGHIJLNPQRSTUY_abcdefghijklmnoprstuvwxyz…！，？。：《》";
        foreach (char c in additionalCharacters)
        {
            uniqueChars.Add(c);
        }

        foreach (var line in lines)
        {
            string[] columns = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length > 1 && !string.IsNullOrEmpty(columns[1]))
            {
                foreach (char c in columns[1])
                {
                    uniqueChars.Add(c);
                }
            }
        }
        string path = Path.Combine(Application.dataPath, "Game/fontlib/unique_chars.txt");
        Debug.Log(path);
        // 使用 StreamWriter 来按照 UTF-8 编码保存文件
        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.Write(string.Join("", uniqueChars));
        }
    }
}