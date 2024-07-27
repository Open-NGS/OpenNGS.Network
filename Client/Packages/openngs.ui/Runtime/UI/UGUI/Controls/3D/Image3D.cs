using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace OpenNGS.UI
{
    /// <summary>
    /// Image3D is a element used 3d mesh in the UI hierarchy.
    /// </summary>
    
    [RequireComponent(typeof(CanvasRenderer))]
    public class Image3D : Image
    {
        public Sprite3D sprite3D;
        public float thickness = 100f;
        public Color sideColor = Color.gray;

        private Sprite3D m_DefaultSprite3D;

        public enum SpriteType
        {
            Simple,
            Sliced,
        }

        [SerializeField] private SpriteType m_SpriteType = SpriteType.Simple;
        
        public SpriteType spriteType
        {
            get => m_SpriteType;
            set
            {
                m_SpriteType = value;
                
                switch (m_SpriteType)
                {
                    case SpriteType.Simple:
                        type = Type.Simple;
                        break;
                    case SpriteType.Sliced:
                        type = Type.Sliced;
                        break;
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect r = GetPixelAdjustedRect();

            Color32 color32 = color;
            Color32 sideColor32 = sideColor;
            
            if (sprite3D == null)
            {
                // default sprite 3d is rectangle type
                m_DefaultSprite3D = m_DefaultSprite3D ? m_DefaultSprite3D : ScriptableObject.CreateInstance<Sprite3D>();
                m_DefaultSprite3D.GenerateSprite3D(r, thickness, color32);
                vh.AddUIVertexStream(m_DefaultSprite3D.Verts, m_DefaultSprite3D.Indices);
                return;
            }

            if (sprite3D.type == Sprite3D.Type.Sprite2D)
            {
                bool sliced = spriteType == SpriteType.Sliced && sprite3D.sprite2D != null && hasBorder;
                
                Vector4 border = Vector4.zero;
                if (sliced)
                {
                    border = sprite.border;
                    border = GetAdjustedBorders(border / multipliedPixelsPerUnit, r);
                }
                
                sprite3D.GenerateFromSprite(r, thickness, color32, sideColor32, border, sliced);
                
                vh.AddUIVertexStream(sprite3D.Verts, sprite3D.Indices);
                
                if (sliced)
                {
                    GenerateSlicedFrontBackSprites(vh);
                }
            }
            else
            {
                sprite3D.GenerateSprite3D(r, thickness, color32);
                vh.AddUIVertexStream(sprite3D.Verts, sprite3D.Indices);
            }
        }
        
        protected override void OnEnable()
        {
            if (sprite3D != null && sprite3D.sprite2D != null)
            {
                sprite = sprite3D.sprite2D;
            }

            base.OnEnable();
        }

        protected void LateUpdate()
        {
            CheckSpriteChanged();
        }

        void CheckSpriteChanged()
        {
            if (sprite3D != null && sprite3D.type == Sprite3D.Type.Sprite2D && sprite3D.sprite2D != sprite)
            {
                sprite = sprite3D.sprite2D;
            }
        }

        private static readonly Vector2[] s_VertScratch = new Vector2[4];
        private static readonly Vector2[] s_UVScratch = new Vector2[4];

        /// <summary>
        /// Generate vertices for 9-sliced faces both front and back
        /// </summary>
        private void GenerateSlicedFrontBackSprites(VertexHelper toFill)
        {
            if (!hasBorder)
            {
                return;
            }

            Vector4 outer, inner, padding, border;

            if (sprite != null)
            {
                outer = SpritesUtility.GetOuterUV(sprite);
                inner = SpritesUtility.GetInnerUV(sprite);
                padding = SpritesUtility.GetPadding(sprite);
                border = sprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelAdjustedRect();

            Vector4 adjustedBorders = GetAdjustedBorders(border / multipliedPixelsPerUnit, rect);
            padding = padding / multipliedPixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = adjustedBorders.x;
            s_VertScratch[1].y = adjustedBorders.y;

            s_VertScratch[2].x = rect.width - adjustedBorders.z;
            s_VertScratch[2].y = rect.height - adjustedBorders.w;

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    int y2 = y + 1;
                    
                    AddSliceFrontBackQuads(toFill,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        color,
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y));
                }
            }
        }
        
        void AddSliceFrontBackQuads(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
            
            startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, thickness), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, thickness), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, thickness), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, thickness), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex + 2, startIndex + 1, startIndex);
            vertexHelper.AddTriangle(startIndex, startIndex + 3, startIndex + 2);
        }
        
        private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                // The adjusted rect (adjusted for pixel correctness)
                // may be slightly larger than the original rect.
                // Adjust the border to match the adjustedRect to avoid
                // small gaps between borders (case 833201).
                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }
    }
}
