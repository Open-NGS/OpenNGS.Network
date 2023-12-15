using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI.DataBinding
{
    public enum BindingMode
    {
        TwoWay,         // view、view model 双向绑定 
        OneWay,         // view model 修改 view 单向绑定
        OneTime,        // view model 初始化 view 控件，绑定时只赋值一次
        OneWayToSource  // view 修改 view model 单向绑定与 OneWay 相反 
    }
}