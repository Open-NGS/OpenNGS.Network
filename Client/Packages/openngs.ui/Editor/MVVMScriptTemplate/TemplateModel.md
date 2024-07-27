using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public enum TemplateNotify
    {
        OnXxxChange1,
        OnXxxChange2
    }

    public class TemplateModel : ModelBase<TemplateModel, TemplateNotify>
    {
        public void DoSth1()
        {
            PostNotification(TemplateNotify.OnXxxChange1);
        }

        public void DoSth2()
        {
            PostNotification(TemplateNotify.OnXxxChange2, 123);
        }
    }
}
