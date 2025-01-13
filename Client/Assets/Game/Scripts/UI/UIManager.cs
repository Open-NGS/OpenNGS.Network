using OpenNGS;
using OpenNGS.IO;
using OpenNGS.Localize;
using OpenNGS.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.Rendering.ReloadAttribute;

public static class TestStatisticData
{
    public static Table<OpenNGS.UI.Data.UIConfig, string> s_uiCfg = new Table<OpenNGS.UI.Data.UIConfig, string>((item) => { return item.IdOfUI; }, false);

    public static void Init() { }
}
// TODO
// 这部分之后需要放入OpenNGS。
// 还需要实现UISystem的接口。这套接口主要是为不同平台/不同UI框架来定制。
namespace OpenNGS.UI
{
    public class UIManager: Singleton<UIManager>
    {
        private readonly Dictionary<string, IView> m_CachedViews = new Dictionary<string, IView>();
        private readonly List<IView> m_UICtrList = new List<IView>();
        Stack m_cache = new Stack();

        public static void Init()
        {
            UISystem.Register(OpenNGS.UI.Common.UI_SYSTEM.UI_SYSTEM_UGUI, UGUISystem.Instance);
            UISystem.Init();
        }

        public T Open<T>(string id, OpenNGSViewParam param = null) where T : class, IView
        {
            return Open(id, param) as T;
        }

        public IView Open(string id, OpenNGSViewParam param = null)
        {
            var jsOpened = false;

            if (jsOpened)
            {
                Debug.Log($"[UIManager] OpenByTs");
                return null;
            }

            IView viewCtr = GetViewCtrByUID(id);
            if (viewCtr == null)
            {
                viewCtr = LoadViewCtr(id);
                m_CachedViews.Add(id, viewCtr);
            }
            viewCtr.Closed -= OnViewClosed;
            viewCtr.Closed += OnViewClosed;
            viewCtr.Open(param);

            OpenNGS.UI.Data.UIConfig config = this.GetConfig(id);
            if (config.Stack)
            {
                m_UICtrList.Add(viewCtr);
                m_cache.Push(viewCtr);
            }

            PrintStack();
            return viewCtr;
        }

        public void Close(string id, bool clear = false)
        {
            IView viewCtr = null;
            OpenNGS.UI.Data.UIConfig config = this.GetConfig(id);

            if (null == config)
            {
                Debug.LogError($"uiType is null: {id}");
                return;
            }

            viewCtr = GetViewCtrByUID(id);

            if (viewCtr == null)
            {
                return;
            }
            viewCtr.Closed -= OnViewClosed;
            viewCtr.Close();
            if (clear || !config.Cache)
            {
                m_CachedViews.Remove(id);
            }

            if (config.Stack)
            {
                m_UICtrList.Remove(viewCtr);
            }

            PrintStack();
        }

        public OpenNGS.UI.Data.UIConfig GetConfig(string id)
        {
            return TestStatisticData.s_uiCfg.GetItem(id);
            //return null;
        }

        private void OnViewClosed(string id)
        {
            IView viewCtr = null;
            OpenNGS.UI.Data.UIConfig config = this.GetConfig(id);

            if (null == config)
            {
                Debug.LogError($"uiType is null: {id}");
                return;
            }

            viewCtr = GetViewCtrByUID(id);

            if (viewCtr == null)
            {
                return;
            }
            viewCtr.Closed -= OnViewClosed;
            if (!config.Cache)
            {
                m_CachedViews.Remove(id);
            }

            if (config.Stack)
            {
                m_UICtrList.Remove(viewCtr);
            }

            PrintStack();
        }

        private void PrintStack()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("UIStack: ");
            for (int i = m_UICtrList.Count - 1; i >= 0; --i)
            {
                builder.Append(m_UICtrList[i].ID + " ,");
            }

            Debug.LogFormat(builder.ToString());
        }
        private IView LoadViewCtr(string id)
        {
            OpenNGS.UI.Data.UIConfig config = this.GetConfig(id);

            IUISystem _UISystem = UISystem.Get((OpenNGS.UI.Common.UI_SYSTEM)config.Type);
            IView view = null;
            if (_UISystem != null)
            {
                view = _UISystem.CreateView(id, config.Package, config.Component, (int)config.Layer, config.Cache);
            }
            return view;
        }
        public IView GetViewCtrByUID(string id)
        {
            IView viewCtr = null;
            m_CachedViews.TryGetValue(id, out viewCtr);
            return viewCtr;
        }

        public void CloseAll(bool clear = false)
        {
            //JsCloseAll?.Invoke();
            var keys = m_CachedViews.Keys.ToList<string>();
            foreach (var uiid in keys)
            {
                Close((string)uiid, clear);
            }
            m_CachedViews.Clear();
        }
        public void CloseAllListUI(bool ignoreMainHud = false, bool clear = false)
        {
            for (int i = m_UICtrList.Count - 1; i >= 0; --i)
            {
                var viewCtr = m_UICtrList[i];
                //if (ignoreMainHud && (string)viewCtr.ID == string.UI_MAIN_HUD)
                //{
                //    continue;
                //}
                Close((string)viewCtr.ID, clear);
            }
        }

        public void ShowAll()
        {
            foreach (var viewCtr in m_UICtrList)
            {
                Show((string)viewCtr.ID);
            }
        }
        public void HideAll()
        {
            foreach (var viewCtr in m_UICtrList)
            {
                Hide((string)viewCtr.ID);
            }
        }

        public void Show(string id)
        {
            IView viewCtr = GetViewCtrByUID(id);
            if (viewCtr != null)
            {
                viewCtr.Show();
            }
        }
        public void Hide(string id)
        {
            IView viewCtr = GetViewCtrByUID(id);
            if (viewCtr != null)
            {
                viewCtr.Hide();
            }
        }
        public bool IsVisible(string id)
        {
            var viewCtr = GetViewCtrByUID(id);
            if (viewCtr != null && viewCtr.Visible)
            {
                return true;
            }

            return false;
        }
        //protected override void OnClear()
        //{
        //    CloseAll(true);
        //}
        private void SetUIStrings(SystemLanguage lan)
        {
            //string langName = LocalizationSystem.GetLangName(lan);
            //string file = Path.Combine(Application.streamingAssetsPath, string.Format(LocalizationXmlPath, langName));
            //if (!FileSystem.FileExists(file))
            //{
            //    file = Path.Combine(Application.streamingAssetsPath, DefaultLocalizationXmlPath);
            //}
            //// string fileContent = File.ReadAllText(file);
            //Debug.Log("[SetUIString] " + file);
            //string fileContent = System.Text.Encoding.UTF8.GetString(FileSystem.Read(file));
            //Debug.LogError(fileContent);

            //FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(fileContent);
            //UIPackage.SetStringsSource(xml);
        }

        // 是否存在没有关闭的面板
        public bool AllClosed()
        {
            if (m_UICtrList.Count == 0)
            {
                return true;
            }
            return false;
        }

        // 关闭当前层UI
        public void CloseLayer()
        {
            IView viewCtr = m_cache.Pop() as IView;
            viewCtr.Close();
        }

        private void ReloadUI()
        {
            CloseAll(true);
        }
    }

}