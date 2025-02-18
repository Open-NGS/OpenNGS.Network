using Newtonsoft.Json;
using OpenNGS.Credits;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UICredits : MonoBehaviour
{
    public TMP_FontAsset font;
    public GameObject allContent;
    public GameObject preb_RootLayout;
    public GameObject preb_ElementLayout;
    public GameObject preb_ElementText;
    public string folderName;
    private GameObject m_curContent;
    private List<string> m_urls = new List<string>();
    //测试使用本地文件
    public string jsonPath;
    public Sprite targetSprite;
    private void Start()
    {
        m_curContent = allContent;
        string jsonContent = "";
        if (jsonPath != null && File.Exists(jsonPath))
        {
            // 读取测试文件内容
            jsonContent = File.ReadAllText(jsonPath);
        }
        else
        {
            //todo 接入SDK
        }

        if (jsonContent != null)
        {
            OpenNGS.Credits.Root root = JsonConvert.DeserializeObject<OpenNGS.Credits.Root>(jsonContent);
            if (root != null)
            {
                foreach (OpenNGS.Credits.Element element in root.Elements)
                {
                    CheckImageType(element);
                }
                StartCoroutine(DownloadAllImages(root));
            }
        }

    }
    IEnumerator DownloadAllImages(OpenNGS.Credits.Root root)
    {
        foreach (string url in m_urls)
        {
            yield return StartCoroutine(DownloadImage(url));
        }

        // 所有图片下载完成后执行下一步操作
        foreach (OpenNGS.Credits.Element element in root.Elements)
        {
            UpdateUI(element);
        }
    }
    private void CheckImageType(OpenNGS.Credits.Element element) 
    {
        if (element.Type == "Image")
        {
            if (element.Title != null && element.Title != "")
            {
                string savePath = GetSavePath(element.Title);
                if (!File.Exists(savePath))
                {
                    if (!m_urls.Contains(element.Title))
                    {
                        m_urls.Add(element.Title);
                    }
                }
            }
        }
        if (element.Content != null)
        {
            foreach (OpenNGS.Credits.Element subElement in element.Content)
            {
                CheckImageType(subElement);
            }
        }
        
    }
    private void UpdateUI(OpenNGS.Credits.Element element)
    {
        if (element.Content == null || element.Content.Count == 0)//不需要嵌套
        {
            if (element.Type == "Text")//文字
            {
                GameObject obj = Instantiate(preb_ElementText, m_curContent.transform);
                UICreditsElementText creditsElement = obj.GetComponent<UICreditsElementText>();
                if (creditsElement != null)
                {
                    creditsElement.SetText(element.Text,font);
                }
            }
            else if (element.Type == "Image")//图片
            {
                GameObject imageObject = new GameObject("DynamicImage");
                Image image = imageObject.AddComponent<Image>();
                imageObject.transform.SetParent(m_curContent.transform, false);
                if (element.Title == null && targetSprite != null)
                {
                    image.sprite = targetSprite;
                    image.type = Image.Type.Simple; // 设置图片显示类型为简单模式
                    image.preserveAspect = true; // 保持图片的长宽比

                    float originalWidth = targetSprite.rect.width;
                    float originalHeight = targetSprite.rect.height;

                    // 设置图片的尺寸为原始尺寸
                    RectTransform rectTransform = image.rectTransform;
                    rectTransform.sizeDelta = new Vector2(originalWidth, originalHeight);
                }
                else
                {
                    if (folderName != null)
                    {
                        LoadAndDisplayImage(GetSavePath(element.Title), image);
                    }
                }
            }
        }
        else//需要嵌套
        {
            if (element.Type == "Block")//嵌套
            {
                if (element.Content != null)
                {
                    GameObject obj;
                    if (element.Content.Exists((item) => item.Type == "Block") || m_curContent.Equals(allContent))//使用外部嵌套
                    {
                        obj = Instantiate(preb_RootLayout, m_curContent.transform);
                    }
                    else//使用内部
                    {
                        obj = Instantiate(preb_ElementLayout, m_curContent.transform);
                    }
                    ;
                    UICreditsElement creditsElement = obj.GetComponent<UICreditsElement>();
                    if (creditsElement != null)
                    {
                        creditsElement.SetTitle(element.Title,font);
                        m_curContent = creditsElement.content;
                    }
                    foreach (OpenNGS.Credits.Element subElement in element.Content)
                    {
                        UpdateUI(subElement);
                    }
                    m_curContent = allContent;
                }
            }

        }

    }
    IEnumerator DownloadImage(string url)
    {
        string savePath = GetSavePath(url);

        // 下载图片
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 获取下载的纹理
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // 将纹理保存为文件
                byte[] imageBytes = texture.EncodeToPNG();
                File.WriteAllBytes(savePath, imageBytes);

                Debug.Log("图片已保存到: " + savePath);
            }
        }
    }
    private void LoadAndDisplayImage(string path,Image image)
    {
        // 读取本地图片
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);

        // 转换为Sprite并显示
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        image.preserveAspect = true;
    }
    private string GetSavePath(string url)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName).Replace("\\", "/");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); // 创建文件夹
        }
        // 图片保存路径
        string fileName = Path.GetFileName(url);
        string savePath = Path.Combine(folderPath, fileName).Replace("\\", "/");
        return savePath;
    }
}
namespace OpenNGS.Credits
{
    public class Element
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public List<Element> Content { get; set; } // 用于嵌套的 Block 类型
        public string Text { get; set; } // 用于 Text 类型
    }

    public class Root
    {
        public List<Element> Elements { get; set; }
    }
}
