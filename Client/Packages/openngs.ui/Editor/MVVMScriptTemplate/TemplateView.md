using System;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.UI;
using UnityEngine;

namespace UI
{
    public class TemplateView : UView<TemplateViewModel>
    {
        private void Start()
        {

        }

        protected override void OnInit()
        {
            // this method only invoked once during lifecycle
            m_ViewModel.RegisterNotifyHandler(OnReceiveViewModelNotify);   
        }

        protected override void OnOpen(NiViewParam param)
        {

        }

        protected override void OnClose()
        {

        }

        protected override void OnReceiveViewModelNotify(string command, params object[] args)
        {
            switch (command)
            {
                case "notify1":
                    break;
                case "notify2":
                    break;
            }
        }
    }
}