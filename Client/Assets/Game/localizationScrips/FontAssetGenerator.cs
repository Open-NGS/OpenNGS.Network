using TMPro;
using System.IO;
using System.Text;
using UnityEngine;

public class FontAssetGenerator : MonoBehaviour
{
    public static TMP_FontAsset font;

    void Start()
    {
        FontImporter();
    }
    [ContextMenu("test")]
    public void test()
    {
        FontImporter();
    }
    static void FontImporter()
    {
        string fontPath = "Fonts & Materials/SmileySans-Oblique-2 SDF";
        font = Resources.Load<TMP_FontAsset>(fontPath);
        if (font == null)
        {
            Debug.LogError("Font asset is not found at path: " + fontPath);
            return;
        }
        else
        {
            ImportTextCharacters("Assets/Game/fontlib/unique_chars.txt", font);
        }
    }
    public static void ImportTextCharacters(string filePath, TMP_FontAsset font)
    {
        if (font == null)
        {
            Debug.LogError("Font asset is not assigned.");
            return;
        }
        // 清空字体资源数据
        font.ClearFontAssetData();

        if (File.Exists(filePath))
        {
            string allText;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                allText = reader.ReadToEnd();
            }

            bool success = font.TryAddCharacters(allText, includeFontFeatures: true);

            if (success)
            {
                Debug.Log("All characters imported successfully.");
            }
            else
            {
                string missingCharacters;
                font.TryAddCharacters(allText, out missingCharacters, includeFontFeatures: true);
                Debug.LogWarning("Some characters could not be imported: " + missingCharacters);
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}