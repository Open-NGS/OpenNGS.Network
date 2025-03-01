using Newtonsoft.Json;
using OpenNGS.Credits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class LayoutInfo
{
    public GameObject preb;
    public float parentSpace;
    public float subSpace;
    public float titleFontSize;
}

public class UICredits : MonoBehaviour
{
    public TMP_FontAsset font;
    public GameObject allContent;
    public LayoutInfo info_RootLayout;
    public LayoutInfo info_ElementLayout;
    public LayoutInfo info_ElementText;
    public string folderName;
    public Action<Image,string> getImageSprite;
    private GameObject m_curContent;
    private List<string> m_urls = new List<string>();
    private List<OpenNGS.Credits.Element> m_rootElement = new List<OpenNGS.Credits.Element>();

    //测试使用本地文件
    public string jsonPath;
    public Sprite targetSprite;
    private void Start()
    {
        Debug.Log(string.Format("credit start"));
        m_curContent = allContent;
        string jsonContent = "";
        jsonPath = Application.streamingAssetsPath + "/" + "data." + "json";
        if (jsonPath != null && File.Exists(jsonPath))
        {
            // 读取测试文件内容
            jsonContent = File.ReadAllText(jsonPath);
            Debug.Log("has path " + jsonPath);
        }
        else
        {
            //todo 接入SDK
            Debug.Log("has not path " + jsonPath);
        }

        if (jsonContent != null)
        {
            OpenNGS.Credits.Root root = JsonConvert.DeserializeObject<OpenNGS.Credits.Root>(jsonContent);
            if (root != null)
            {
                m_rootElement = root.Elements;
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
                if(IsURLorPath(element.Title)== "URL")
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
        }
        if (element.Content != null)
        {
            foreach (OpenNGS.Credits.Element subElement in element.Content)
            {
                CheckImageType(subElement);
            }
        }
        
    }
    GameObject m_curRootBlock = null;
    private List<OpenNGS.Credits.Element> _subList = new List<OpenNGS.Credits.Element>();
    private void UpdateUI(OpenNGS.Credits.Element element)
    {
        if (element.Content == null || element.Content.Count == 0)//不需要嵌套
        {
            if (element.Type == "Text")//文字
            {
                GameObject obj = Instantiate(info_ElementText.preb, m_curContent.transform);
                UICreditsElementText creditsElement = obj.GetComponent<UICreditsElementText>();
                if (creditsElement != null)
                {
                    creditsElement.SetText(element.Text, font, info_ElementText.titleFontSize);
                }
            }
            else if (element.Type == "Image")//图片
            {
                GameObject imageObject = new GameObject("DynamicImage");
                Image image = imageObject.AddComponent<Image>();
                imageObject.transform.SetParent(m_curContent.transform, false);
                if (element.Title == "" && targetSprite != null)
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
                    if (IsURLorPath(element.Title) == "URL")
                    {
                        if (folderName != null)
                        {
                            LoadAndDisplayImage(GetSavePath(element.Title), image);
                        }
                    }
                    else if(IsURLorPath(element.Title) == "Path")
                    {
                        getImageSprite?.Invoke(image, element.Title);
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
                        obj = Instantiate(info_RootLayout.preb, m_curContent.transform);
                        _subList = element.Content;
                        m_curRootBlock = obj.GetComponent<UICreditsElement>().content;
                        UICreditsElement creditsElement = obj.GetComponent<UICreditsElement>();
                        if (creditsElement != null)
                        {
                            creditsElement.GetComponent<VerticalLayoutGroup>().spacing = info_RootLayout.parentSpace;
                            creditsElement.content.GetComponent<VerticalLayoutGroup>().spacing = info_RootLayout.subSpace;
                            creditsElement.SetTitle(element.Title, font, info_RootLayout.titleFontSize);
                            m_curContent = creditsElement.content;
                        }
                    }
                    else//使用内部
                    {
                        if (m_curRootBlock != null && _subList.Contains(element))
                        {
                            m_curContent = m_curRootBlock;
                        }
                        obj = Instantiate(info_ElementLayout.preb, m_curContent.transform);
                        UICreditsElement creditsElement = obj.GetComponent<UICreditsElement>();
                        if (creditsElement != null)
                        {
                            creditsElement.GetComponent<VerticalLayoutGroup>().spacing = info_ElementLayout.parentSpace;
                            creditsElement.content.GetComponent<VerticalLayoutGroup>().spacing = info_ElementLayout.subSpace;
                            creditsElement.SetTitle(element.Title, font, info_ElementLayout.titleFontSize);
                            m_curContent = creditsElement.content;
                        }
                    }
                    foreach (OpenNGS.Credits.Element subElement in element.Content)
                    {
                        UpdateUI(subElement);
                    }
                    if (m_rootElement.Contains(element))
                    {
                        m_curContent = allContent;
                    }

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
    private string IsURLorPath(string input)
    {
        // 定义URL的正则表达式
        string urlPattern = @"^(http|https)://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(:[0-9]+)?(/.*)?$";
        Regex urlRegex = new Regex(urlPattern);

        // 定义路径的正则表达式（支持Windows和Unix路径）
        string pathPattern = @"^([a-zA-Z]:\\|/)?([a-zA-Z0-9\-_\.]+[\\/])*([a-zA-Z0-9\-_\.]+)?$";
        Regex pathRegex = new Regex(pathPattern);

        if (urlRegex.IsMatch(input))
        {
            return "URL";
        }
        else if (pathRegex.IsMatch(input))
        {
            return "Path";
        }
        else
        {
            return "Unknown";
        }
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
