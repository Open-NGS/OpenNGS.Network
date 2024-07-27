using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace OpenNGS.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Web Image", 13)]
    public class WebImage : RawImage
    {
        [SerializeField]
        private string m_Url;


        private bool m_IsDirty = true;

        public string url
        {
            get
            { return m_Url; }
            set
            {
                if (string.IsNullOrEmpty(m_Url))
                {
                    m_Url = value;
                    m_IsDirty = true;
                }
                else if (m_Url != value)
                {
                    m_Url = value;
                    m_IsDirty = true;
                }

                if (this.isActiveAndEnabled)
                {
                    if (!string.IsNullOrEmpty(m_Url))
                        StartCoroutine(DownloadImage());
                }
            }
        }

        [SerializeField]
        private bool m_UseCache;
        public bool useCache
        {
            get { return m_UseCache; }
            set { m_UseCache = value; }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if(m_IsDirty && !string.IsNullOrEmpty(m_Url))
            {
                StartCoroutine(DownloadImage());
            }
        }
        IEnumerator DownloadImage()
        {
            m_IsDirty = false;
            string cachefile = null;
            if (m_UseCache)
            {
                Uri uri = new Uri(m_Url);
                cachefile = Application.temporaryCachePath + uri.AbsolutePath;
                if (System.IO.File.Exists(cachefile))
                {
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(System.IO.File.ReadAllBytes(cachefile));
                    this.texture = tex;
                    yield break;
                }
            }

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(m_Url);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
                Debug.Log(request.error);
            else
            {
                this.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                if (cachefile != null)
                {
                    string dirname = System.IO.Path.GetDirectoryName(cachefile);
                    if (!System.IO.Directory.Exists(dirname))
                    {
                        System.IO.Directory.CreateDirectory(dirname);
                    }
                    System.IO.File.WriteAllBytes(cachefile, request.downloadHandler.data);
                }
            }

        }

    }
}