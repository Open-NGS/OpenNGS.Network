using System;
using System.Collections;
using UnityEngine;
namespace OpenNGS.UI
{
    /// <summary>
    /// Abstract Class for creating a Recycling system.
    /// </summary>
    public abstract class RecyclingSystem
    {
        public IRecyclableScrollRectDataSource DataSource;
        public Action<ICell> OnInstantiateCell;
        public Action<RectTransform> OnCreateCell;
        public Action<int> OnScrollCell;

        protected RectTransform Viewport, Content;
        protected RectTransform PrototypeCell;
        protected bool IsGrid;

        protected float MinPoolCoverage = 1.5f; // The recyclable pool must cover (viewPort * _poolCoverage) area.
        protected int MinPoolSize = 10; // Cell pool must have a min size
        protected float RecyclingThreshold = .2f; //Threshold for recycling above and below viewport

        public abstract IEnumerator InitCoroutine(System.Action onInitialized = null);

        public abstract Vector2 OnValueChangedListener(Vector2 direction);
    }
}