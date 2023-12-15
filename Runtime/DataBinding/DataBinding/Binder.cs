using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI.DataBinding
{
    public abstract class Binder<TSource, TTarget> : DataSource, IBinder
    {
        public IBindable<TSource> source;
        public TTarget target;
    }
}