using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    public interface IUView : IView
    {
        public void Init(string id, int layer, bool cache);
    }

    public abstract class UView : MonoBehaviour, IUView
    {
        public Action<string> Closed { get; set; }
        public string ID { get; set; }
        public int Layer { get; set; }
        public bool Cache { get; set; }
        public bool Visible { get; private set; }

        protected float DismissAnimationDuration = 0f;

        public void Open(OpenNGSViewParam param)
        {
            OpenView(param);
            OnOpen(param);

            PlayOpenAnimation();
            Visible = true;
        }

        public virtual void Init(string id, int layer, bool cache)
        {
            ID = id;
            Layer = layer;
            Cache = cache;

            InitView();
            OnInit();
        }

        protected virtual void OpenView(OpenNGSViewParam param) { }
        protected virtual void InitView() { }
        protected virtual void CloseView() { }
        public virtual bool CanClose()
        {
            return true;
        }
        public bool Close()
        {
            bool bClose = true;
            OnClose();
            PlayDismissAnimation();
            Closed?.Invoke(ID);
            Destroy(gameObject, DismissAnimationDuration);
            CloseView();
            return bClose;
        }
        public void Show()
        {
            gameObject.SetActive(true);
            Visible = true;
            OnShow();
            PlayOpenAnimation();
        }

        public void Hide()
        {
            PlayDismissAnimation();
            Visible = false;
            OnHide();
            if (DismissAnimationDuration == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(DelayHide());
            }
        }

        private IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(DismissAnimationDuration);
            gameObject.SetActive(false);
        }

        // lifecycle functions
        protected virtual void OnInit() { }
        protected virtual void OnOpen(OpenNGSViewParam param) { }
        protected virtual void OnClose() { }
        protected virtual void PlayOpenAnimation() { }
        protected virtual void PlayDismissAnimation() { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected virtual void OnReceiveViewModelNotify(string command, params object[] args) { }
    }

    public class UView<TModel> : UView where TModel : IViewModel, new()
    {
        protected TModel m_ViewModel;

        public override void Init(string id, int layer, bool cache)
        {
            base.Init(id, layer, cache);
        }
        protected override void InitView()
        {
            base.InitView();
            m_ViewModel = new TModel();
            m_ViewModel?.Init();
        }

        protected override void OpenView(OpenNGSViewParam param)
        {
            base.OpenView(param);
            m_ViewModel?.Open();
        }

        protected override void CloseView()
        {
            base.CloseView();
            m_ViewModel?.Close();
        }
    }
}