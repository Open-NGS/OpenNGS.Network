using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;

namespace Systems
{
    public enum TemplateNotify
    {
        OnXxxChanged,
    }
    public class TemplateSystem : GameSubSystem<TemplateSystem, ViewModelBase, TemplateNotify>
    {
        public void DoSth1()
        {
            PostNotification(TemplateNotify.OnXxxChanged, 123);
        }
    }
}
