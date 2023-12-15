using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TemplateViewModel : ViewModelBase
    {
        public override void Init()
        {

        }

        public override void Open()
        {
            
        }

        public override void Close()
        {

        }

        private void NotifyHandler()
        {
            NotifyView("notify1");
        }
    }
}