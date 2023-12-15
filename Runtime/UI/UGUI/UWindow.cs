using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OpenNGS.UI.UGUI
{
    public class UWindow : IUView
    { 
        public Action<int> Closed { get; set; }
        public int ID { get; set; }
        public int Layer { get; set; }
        public bool Cache { get; set; }
        public bool Visible { get; private set; }
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Init(int id, int layer, bool cache)
        {
            throw new NotImplementedException();
        }

        public void Open(OpenNGSViewParam param)
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}