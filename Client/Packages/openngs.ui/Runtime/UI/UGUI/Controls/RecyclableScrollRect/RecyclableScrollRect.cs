using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OpenNGS.UI
{
    /// <summary>
    /// Entry for the recycling system. Extends Unity's inbuilt ScrollRect.
    /// </summary>
    public class RecyclableScrollRect : ScrollRect
    {
        [HideInInspector]
        public IRecyclableScrollRectDataSource DataSource;
        public IRecyclableScrollRectDataSource BindingDataSource
        {
            get => DataSource;
            set => DataSource = value;
        }
        
        [Serializable]
        public class ScrollCellEvent : UnityEvent<int> {}
        
        [SerializeField]
        private ScrollCellEvent m_OnScrollEvent = new ();

        public ScrollCellEvent OnScrollEvent
        {
            get => m_OnScrollEvent;
            set => m_OnScrollEvent = value;
        }

        [Serializable]
        public class CellCreateEvent : UnityEvent<ICell> { }
        
        [SerializeField] 
        private CellCreateEvent m_OnCreateCell = new ();
        public CellCreateEvent OnCellCreate
        {
            get => m_OnCreateCell;
            set => m_OnCreateCell = value;
        }

        public bool IsGrid;
        // Prototype cell can either be a prefab or present as a child to the content(will automatically be disabled in runtime)
        public RectTransform PrototypeCell;
        // If true the intiziation happens at Start. Controller must assign the datasource in Awake.
        // Set to false if self init is not required and use public init API.
        public bool SelfInitialize = true;

        public enum DirectionType
        {
            Vertical,
            Horizontal
        }

        public DirectionType Direction;

        // Segments : coloums for vertical and rows for horizontal.
        public int Segments
        {
            set
            {
                _segments = Math.Max(value, 2);
            }
            get
            {
                return _segments;
            }
        }
        [SerializeField]
        private int _segments;

        private RecyclingSystem _recyclingSystem;
        private Vector2 _prevAnchoredPos;
        
        protected override void Start()
        {
            // defafult(built-in) in scroll rect can have both directions enabled, Recyclable scroll rect can be scrolled in only one direction.
            // setting default as vertical, Initialize() will set this again. 
            vertical = Direction == DirectionType.Vertical;
            horizontal = Direction == DirectionType.Horizontal;

            if (!Application.isPlaying) return;

            if (SelfInitialize) Initialize();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (DataSource != null)
            {
                ReloadData();
            }
        }

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize(Action<RectTransform> onInstantiateCell = null)
        {
            // Construct the recycling system.
            if (Direction == DirectionType.Vertical)
            {
                _recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            else if (Direction == DirectionType.Horizontal)
            {
                _recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            
            if (_recyclingSystem != null && onInstantiateCell != null)
            {
                _recyclingSystem.OnCreateCell = onInstantiateCell;
            }

            if (_recyclingSystem != null)
            {
                _recyclingSystem.OnScrollCell = i => OnScrollEvent.Invoke(i);
                _recyclingSystem.OnInstantiateCell = trans => OnCellCreate.Invoke(trans);

                vertical = Direction == DirectionType.Vertical;
                horizontal = Direction == DirectionType.Horizontal;

                _prevAnchoredPos = content.anchoredPosition;
                onValueChanged.RemoveListener(OnValueChangedListener);
                // Adding listener after pool creation to avoid any unwanted recycling behaviour.(rare scenerio)
                if (gameObject.activeInHierarchy)
                    StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                        onValueChanged.AddListener(OnValueChangedListener)
                    ));
            }
        }

        /// <summary>
        /// public API for Initializing when datasource is not set in controller's Awake. Make sure selfInitalize is set to false. 
        /// </summary>
        public void Initialize(IRecyclableScrollRectDataSource dataSource, Action<RectTransform> onInstantiateCell = null)
        {
            DataSource = dataSource;
            Initialize(onInstantiateCell);
        }

        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="direction">scroll direction</param>
        public void OnValueChangedListener(Vector2 normalizedPos)
        {
            Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }

        /// <summary>
        /// Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData()
        {
            ReloadData(DataSource);
        }

        /// <summary>
        /// Overloaded ReloadData with dataSource param
        /// Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData(IRecyclableScrollRectDataSource dataSource)
        {
            if (_recyclingSystem != null)
            {
                StopMovement();
                onValueChanged.RemoveListener(OnValueChangedListener);
                _recyclingSystem.DataSource = dataSource;
                if (gameObject.activeInHierarchy)
                    StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                                                               onValueChanged.AddListener(OnValueChangedListener)
                                                              ));
                _prevAnchoredPos = content.anchoredPosition;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            CalculateHorizontalPageMove();
        }
        
        private bool m_PreviousPaging = false;
        private bool m_NextPaging = false;
        private int m_TargetCellIndex = 0;

        private void CalculateHorizontalPageMove()
        {
            // todo:
            // support grid mode
            
            if (_recyclingSystem is HorizontalRecyclingSystem system && !IsGrid)
            {
                if (m_PreviousPaging)
                {
                    horizontalNormalizedPosition -= UnityEngine.Time.smoothDeltaTime;
                    var currentIndex = system.MostLeftCellIndexInView;
                    if (currentIndex == m_TargetCellIndex)
                    {
                        m_PreviousPaging = false;
                    }
                }

                if (m_NextPaging)
                {
                    horizontalNormalizedPosition += UnityEngine.Time.smoothDeltaTime;
                    var currentIndex = system.MostLeftCellIndexInView;
                    if (currentIndex == m_TargetCellIndex)
                    {
                        m_NextPaging = false;
                    }
                }
            }
        }
        public void HorizontalPreviousPage()
        {
            if (_recyclingSystem is HorizontalRecyclingSystem && !IsGrid)
            {
                var pageCellCount = (int) (viewport.rect.size.x / PrototypeCell.sizeDelta.x);
                m_PreviousPaging = true;
                m_TargetCellIndex = ((HorizontalRecyclingSystem) _recyclingSystem).MostLeftCellIndexInView - pageCellCount;
                m_TargetCellIndex = Math.Max(0, m_TargetCellIndex);
            }
        }

        public void HorizontalNextPage()
        {
            if (_recyclingSystem is HorizontalRecyclingSystem && !IsGrid)
            {
                var pageCellCount = (int) (viewport.rect.size.x / PrototypeCell.sizeDelta.x);
                m_NextPaging = true;
                m_TargetCellIndex = ((HorizontalRecyclingSystem) _recyclingSystem).MostLeftCellIndexInView + pageCellCount;
                m_TargetCellIndex = Math.Min(m_TargetCellIndex, DataSource.GetItemCount() - pageCellCount);
            }
        }

        /*
        #region Testing
        private void OnDrawGizmos()
        {
            if (_recyclableScrollRect is VerticalRecyclingSystem)
            {
                ((VerticalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

            if (_recyclableScrollRect is HorizontalRecyclingSystem)
            {
                ((HorizontalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

        }
        #endregion
        */
    }
}