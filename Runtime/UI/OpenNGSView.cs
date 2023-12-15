using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI
{
    public abstract class OpenNGSViewParam
    {
        
    }
    
    public interface IView
    {
        public Action<int> Closed { get; set; }
        public int ID { get; set; }
        public int Layer { get; set; }
        public bool Cache { get; set; }
        public bool Visible { get; }
        public void Open(OpenNGSViewParam param);
        public void Close();
        public void Show();
        public void Hide();
    }
}
