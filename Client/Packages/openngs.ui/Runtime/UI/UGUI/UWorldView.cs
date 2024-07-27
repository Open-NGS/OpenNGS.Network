using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace OpenNGS.UI
{
    public class NGSWorldViewParam : OpenNGSViewParam
    {
        public Transform Parent;
        public Vector3 LocalPos;
        public Quaternion LocalRot;
    }
    public class UWorldView<TModel> : UView<TModel> where TModel : IViewModel, INotifyPropertyChanged,  new()
    {
        public void SetParent(Transform parent, Vector3 localPos, Quaternion localRot)
        {
            var trans = transform;
            var scale = trans.localScale;
            
            trans.SetParent(parent);
            trans.localPosition = localPos;
            trans.localRotation = localRot;
            trans.localScale = scale;
        }
    }
}
