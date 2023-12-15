using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.UI
{
    /// <summary>
    /// Sprite3D create mesh for Image3D
    /// </summary>
    public class Sprite3D : ScriptableObject
    {
        /// <summary>
        /// Shape Type
        /// </summary>
        public enum Type
        {
            Rectangle,
            RoundedRectangle,
            Sprite2D,
            CustomMesh,
        }
        
        private enum Quadrant
        {
            I = 0,
            II,
            III,
            IV
        }
        
        public Type type = Type.Rectangle;
        public float roundCornerRadius = 1f;
        
        private readonly List<UIVertex> m_Verts = new List<UIVertex>();
        
        private readonly List<int> m_Indices = new List<int>();

        [SerializeField] private List<int> m_Triangles = new List<int>();
        [SerializeField] private List<Vector3> m_Vertices = new List<Vector3>();
        [SerializeField] private List<Vector3> m_Normals = new List<Vector3>();
        [SerializeField] private List<Vector2> m_Uvs = new List<Vector2>();
        
        // final results
        public List<UIVertex> Verts => m_Verts;
        public List<int> Indices => m_Indices;
        
        // serialized properties
        // cached on asset data
        public List<Vector3> Vertices => m_Vertices;
        public List<Vector3> Normals => m_Normals;
        public List<Vector2> Uvs => m_Uvs;
        public List<int> Triangles => m_Triangles;

        // Cache unit quarter circle trigonometric function calculation results
        private static List<Vector3> unitQuarterCircleVertices;
        
        // quarter circle vertices count
        private const int QuarterSegmentCount = 10;

        public Mesh customMesh;
        
        public Mesh spriteGeneratedMesh;
        public Sprite sprite2D;
        [Range(1, 255)] public int pixelThreshold = 255;
        public bool generateUv = false;
        public bool mergeClosePoints = true;
        public int mergeDistance = 0;

        private readonly Vector2[] m_Quadrant =
        {
            new (1f, 1f), new (1f, -1f), 
            new (-1f, -1f), new (-1f, 1f)
        };

        private readonly int[] m_SliceIndicesIndexes =
        {
            // up 
            11, 0, 9, 0, 11, 1,
            
            // down
            6, 5, 7, 5, 6, 3,

            // left
            10, 6, 8, 6, 10, 9,

            // right
            0, 4, 3, 4, 0, 2,

            // center
            9, 3, 6, 3, 9, 0,
        };

        private readonly int[] m_SliceIndices = new int [30];
        private readonly int[] m_SliceIndexes = new int [12];
         
        public void GenerateSprite3D(Rect rect, float thickness, Color32 color)
        {
            m_Verts.Clear();
            m_Indices.Clear();
            
            switch (type)
            {
                case Type.Rectangle:
                    GenerateRectangle3D(rect, thickness, color);
                    break;
                case Type.RoundedRectangle:
                    GenerateRoundedRectangle3D(rect, thickness, color);
                    break;
                case Type.CustomMesh:
                    GenerateFromMesh(rect, thickness, color);
                    break;
                case Type.Sprite2D:
                    break;
            }
        }

        private void GenerateRectangle3D(Rect rect, float thickness, Color32 color)
        {
            var v = new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
            AddUIVertex(new Vector3(v.x, v.y, 0f), Vector3.back, color);
            AddUIVertex(new Vector3(v.x, v.w, 0f), Vector3.back, color);
            AddUIVertex(new Vector3(v.z,v.w,0f), Vector3.back, color);
            AddUIVertex(new Vector3(v.z, v.y, 0f),Vector3.back, color);
            
            
            m_Indices.Add(0);
            m_Indices.Add(1);
            m_Indices.Add(2);
            
            m_Indices.Add(0);
            m_Indices.Add(2);
            m_Indices.Add(3);
            
            // add back verts & indices
            AddBackVertsAndIndices(thickness, color);
            
            // add connect verts & indices
            AddConnectVertsAndIndices(color);
        }

        
        private void GenerateRoundedRectangle3D(Rect rect, float thickness, Color32 color)
        {
            if (unitQuarterCircleVertices == null)
            {
                InitUnitQuarterCircleVertices();
            }

            if (roundCornerRadius == 0 || 
                roundCornerRadius > rect.width * 0.5f || 
                roundCornerRadius > rect.height * 0.5f)
            {
                GenerateRectangle3D(rect, thickness, color);
                return;
            }
            
            // rounded rectangle draw slice mode in default
            // add four round corners verts
            var v = new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
            var center = new Vector3(v.x + v.z, v.y + v.w) * 0.5f;
            
            for (var quadrant = Quadrant.I; quadrant <= Quadrant.IV; ++quadrant)
            {
                var x = rect.width * 0.5f - roundCornerRadius;
                var y = rect.height * 0.5f - roundCornerRadius;
                var p = m_Quadrant[(int)quadrant];
                var cornerOrigin = center + new Vector3(p.x * x, p.y * y, -thickness * 0f);
                
                AddUIVertex(cornerOrigin, Vector3.back, color);
                
                for (var seg = 0; seg < QuarterSegmentCount; ++seg)
                {
                    Vector3 pos = GetUnitVert(seg, quadrant) * roundCornerRadius;
                    AddUIVertex(cornerOrigin + pos, Vector3.back, color);
                }
            }

            // add quarter circle indices
            for (var quadrant = 0; quadrant < 4; ++quadrant)
            {
                var vertIndex = quadrant + quadrant * QuarterSegmentCount;
                
                for (var i = 1; i < QuarterSegmentCount; ++i)
                {
                    m_Indices.Add(vertIndex);
                    m_Indices.Add(vertIndex + i);
                    m_Indices.Add(vertIndex + i + 1);
                }
            }

            // add slice indices
            for (var quadrant = 0; quadrant < 4; ++quadrant)
            {
                var vertIndex = quadrant + quadrant * QuarterSegmentCount;
                m_SliceIndexes[quadrant * 3] = vertIndex;
                m_SliceIndexes[quadrant * 3 + 1] = vertIndex + 1;
                m_SliceIndexes[quadrant * 3 + 2] = vertIndex + QuarterSegmentCount;
            }

            for (var i = 0; i < m_SliceIndices.Length; ++i)
            {
                m_SliceIndices[i] = m_SliceIndexes[m_SliceIndicesIndexes[i]];
            }
            m_Indices.AddRange(m_SliceIndices);
            
            // add back verts & indices
            AddBackVertsAndIndices(thickness, color);
            
            // add connect verts & indices
            AddRoundedConnectVertsAndIndices(color);
        }

        private void AddBackVertsAndIndices(float thickness, Color color)
        {
            var vertsCount = m_Verts.Count;
            
            for (var i = 0; i < vertsCount; ++i)
            {
                var pos = m_Verts[i].position;
                pos.z = thickness;
                AddUIVertex(pos, Vector3.back, color);
            }
            
            // add back indices
            var indicesCount = m_Indices.Count;
            for (var i = indicesCount - 1; i >= 0; --i)
            {
                m_Indices.Add(m_Indices[i] + vertsCount);
            }
        }

        private void AddRoundedConnectVertsAndIndices(Color color)
        {
            var vertsCount = m_Verts.Count / 2;
            var curVertIndex = m_Verts.Count - 1;
            
            for (var i = 0; i < vertsCount; ++i)
            {
                var index0 = i;
                var index1 = i + 1;
                
                if (index0 == vertsCount - 1)
                    index1 = 1;
                
                if (IsInnerVertex(index0))
                    continue;

                if (IsInnerVertex(index1))
                    index1 += 1;

                var index2 = index0 + vertsCount;
                var index3 = index1 + vertsCount;
                
                curVertIndex += 1;
                
                AddUIVertex(m_Verts[index0].position, m_Verts[index0].position, color);
                AddUIVertex(m_Verts[index1].position, m_Verts[index1].position, color);
                AddUIVertex(m_Verts[index2].position, m_Verts[index2].position, color);
                AddUIVertex(m_Verts[index3].position, m_Verts[index3].position, color);

                AddQuad(curVertIndex, curVertIndex + 1, curVertIndex + 2, curVertIndex + 3);
                curVertIndex += 3;
            }
        }
        
        private void AddConnectVertsAndIndices(Color color)
        {
            var vertsCount = m_Verts.Count / 2;
            var curVertIndex = m_Verts.Count - 1;
            
            for (var i = 0; i < vertsCount; ++i)
            {
                var index0 = i;
                var index1 = i + 1;
                
                if (index0 == vertsCount - 1)
                    index1 = 0;

                var index2 = index0 + vertsCount;
                var index3 = index1 + vertsCount;
                
                curVertIndex += 1;
                
                // calculate vertex normal
                Vector3 dir1 = m_Verts[index0].position - m_Verts[index1].position;
                Vector3 dir2 = m_Verts[index0].position - m_Verts[index3].position;
                Vector3 normal = -Vector3.Cross(dir1, dir2); 
                
                AddUIVertex(m_Verts[index0].position, normal, color);
                AddUIVertex(m_Verts[index1].position, normal, color);
                AddUIVertex(m_Verts[index2].position, normal, color);
                AddUIVertex(m_Verts[index3].position, normal, color);

                AddQuad(curVertIndex, curVertIndex + 1, curVertIndex + 2, curVertIndex + 3);
                curVertIndex += 3;
            }
        }

        private bool IsInnerVertex(int index)
        {
            if (type == Type.RoundedRectangle)
            {
                return index == m_SliceIndexes[0] ||
                       index == m_SliceIndexes[3] ||
                       index == m_SliceIndexes[6] ||
                       index == m_SliceIndexes[9];
            }

            return false;
        }

        private void AddUIVertex(Vector3 pos, Vector3 normal, Color color)
        {
            m_Verts.Add(new UIVertex
            {
                position = pos,
                color = color,
                normal = normal
            });
        }
        private void AddQuad(int index0, int index1, int index2, int index3)
        {
            m_Indices.Add(index0);
            m_Indices.Add(index3);
            m_Indices.Add(index1);
            m_Indices.Add(index3);
            m_Indices.Add(index0);
            m_Indices.Add(index2);
        }

        private static void InitUnitQuarterCircleVertices()
        {
            // Cache unit quarter circle trigonometric function calculation results
            unitQuarterCircleVertices = new List<Vector3>();
            var segmentAngle = 1f / (QuarterSegmentCount - 1) * 90f * Mathf.Deg2Rad;;
            
            for (var i = 0; i < QuarterSegmentCount; ++i)
            {
                var angle = i * segmentAngle;
                var cos = Mathf.Cos(angle);
                var sin = Mathf.Sin(angle);
                unitQuarterCircleVertices.Add(new Vector3(sin, cos, 0f));
            }
        }
        
        private static Vector3 GetUnitVert(int seqIndex, Quadrant quadrant)
        {
            Vector3 ret =  unitQuarterCircleVertices[seqIndex];
            
            switch (quadrant)
            {
                case Quadrant.I:
                    return ret;
                case Quadrant.II:
                    return new Vector3(ret.y, -ret.x, 0f);
                case Quadrant.III:
                    return -ret;
                case Quadrant.IV:
                    return new Vector3(-ret.y, ret.x, 0f);
            }
            return ret;
        }
        
        private void GenerateFromMesh(Rect rect, float thickness, Color32 color)
        {
            if (customMesh == null)
            {
                return;
            }

            var size = customMesh.bounds.size;
            var scale = new Vector3(size.x / rect.width, size.y / rect.height, size.z / thickness); 
                
            for (var i = 0; i < customMesh.vertices.Length; ++i)
            {
                var pos = customMesh.vertices[i];
                pos = new Vector3(pos.x / scale.x, pos.y / scale.y, pos.z / scale.z);
                pos += new Vector3(0f, 0f, thickness * 0.5f);
                m_Verts.Add(new UIVertex
                {
                    position = pos,
                    normal = customMesh.normals[i],
                    tangent = customMesh.tangents[i],
                    uv0 = customMesh.uv[i],
                    color = color,
                });
            }
            
            m_Indices.AddRange(customMesh.GetIndices(0));
        }

        #region Generate From Sprite 2D Mesh
        private static Rect GetInnerRectBySizeAndBorder(Vector2 size, Vector4 border)
        {
            var rect = Rect.zero;
            rect.x = -size.x * 0.5f + border.x;
            rect.y = -size.y * 0.5f + border.y;
            rect.width = size.x - border.x - border.z;
            rect.height = size.y - border.y - border.w;
            return rect;
        }
        
        /// <summary>
        /// get slice index from 0 ~ 8
        /// left bottom = 0, bottom, right bottom, right, right top
        /// top, left top, left, center = 8
        /// </summary>
        private static int GetSliceIndex(Vector3 pos, Rect innerRect)
        {
            // left bottom
            if (pos.x < innerRect.x && pos.y < innerRect.y)
                return 0;
            
            // bottom
            if (pos.x >= innerRect.x && pos.x <= innerRect.x + innerRect.width && pos.y <= innerRect.y)
                return 1;
            
            // right bottom
            if (pos.x > innerRect.x + innerRect.width && pos.y < innerRect.y)
                return 2;
            
            // right 
            if (pos.x >= innerRect.x + innerRect.width && pos.y >= innerRect.y &&
                pos.y <= innerRect.y + innerRect.height)
                return 3;

            // right top
            if (pos.x > innerRect.x + innerRect.width && pos.y > innerRect.y + innerRect.height)
                return 4;
            
            // top
            if (pos.x >= innerRect.x && pos.x <= innerRect.x + innerRect.width && pos.y >= innerRect.y + innerRect.height) 
                return 5;
            
            // left top
            if (pos.x < innerRect.x && pos.y > innerRect.y + innerRect.height)
                return 6;
            
            // left
            if (pos.x <= innerRect.x && pos.y <= innerRect.y + innerRect.height && pos.y >= innerRect.y)
                return 7;
            
            // center
            return 8;
        }

        private readonly Vector3[] m_SliceSpriteScales = new Vector3[9];
        
        /// <summary>
        /// Calculate Slice Sprite Scales
        /// </summary>
        private void CalculateSliceSpriteScales(Vector4 border, Vector4 originBorder, Vector2 size, Vector2 originSize)
        {
            m_SliceSpriteScales[0] = new Vector3(border.x / originBorder.x, border.y / originBorder.y);
            m_SliceSpriteScales[1] = new Vector3(size.x / originSize.x, border.y / originBorder.y);
            m_SliceSpriteScales[2] = new Vector3(border.z / originBorder.z, border.y / originBorder.y);
            m_SliceSpriteScales[3] = new Vector3(border.z / originBorder.z, size.y / originSize.y);
            m_SliceSpriteScales[4] = new Vector3(border.z / originBorder.z, border.w / originBorder.w);
            m_SliceSpriteScales[5] = new Vector3(size.x / originSize.x, border.w / originBorder.w);
            m_SliceSpriteScales[6] = new Vector3(border.x / originBorder.x, border.w / originBorder.w);
            m_SliceSpriteScales[7] = new Vector3(border.x / originBorder.x, size.y / originSize.y);
        }

        private readonly Vector2[] m_SliceSpriteOffset = new Vector2[9];
        
        /// <summary>
        /// Calculate Slice Sprite Offsets
        /// </summary>
        private void CalculateSliceSpriteOffsets(Rect r0, Rect r1)
        {
            var scale = m_SliceSpriteScales;
            var pos0 = new Vector2(r0.x, r0.y);
            var pos1 = new Vector2(r0.x + r0.width, r0.y);
            var pos2 = new Vector2(r0.x + r0.width, r0.y + r0.height);
            var pos3 = new Vector2(r0.x, r0.y + r0.height);
            
            var pos4 = new Vector2(r1.x, r1.y);
            var pos5 = new Vector2(r1.x + r1.width, r1.y);
            var pos6 = new Vector2(r1.x + r1.width, r1.y + r1.height);
            var pos7 = new Vector2(r1.x, r1.y + r1.height);
            
            m_SliceSpriteOffset[0] = pos4 - new Vector2(pos0.x * scale[0].x, pos0.y * scale[0].y);
            m_SliceSpriteOffset[1] = new Vector2(0f,pos5.y - pos1.y * scale[1].y);
            m_SliceSpriteOffset[2] = pos5 - new Vector2(pos1.x * scale[2].x, pos1.y * scale[2].y);
            m_SliceSpriteOffset[3] = new Vector2(pos5.x - pos1.x * scale[3].x, 0f);
            
            m_SliceSpriteOffset[4] = pos6 - new Vector2(pos2.x * scale[4].x, pos2.y * scale[4].y);
            m_SliceSpriteOffset[5] = new Vector2(0f, pos6.y - pos2.y * scale[5].y);
            m_SliceSpriteOffset[6] = pos7 - new Vector2(pos3.x * scale[6].x, pos3.y * scale[6].y);
            m_SliceSpriteOffset[7] = new Vector2(pos4.x - pos0.x * scale[7].x, 0f);
        }
        
        /// <summary>
        /// Generate verts and indices from sprite
        /// </summary>
        public void GenerateFromSprite(Rect rect, float thickness, Color32 color, Color32 sideColor, Vector4 border, bool sliced)
        {
            m_Verts.Clear();
            m_Indices.Clear();
            
            if (sprite2D == null)
            {
                Debug.LogWarningFormat("Missing Sprite REF on sprite 3D {0}", name);
                GenerateRectangle3D(rect, thickness, color);
                return;
            }
            
            var size = new Vector2(sprite2D.texture.width, sprite2D.texture.height);
            var scale = new Vector3(rect.width / size.x, rect.height / size.y, thickness / 30f);
            var originalBorder = sprite2D.border;

            var r0 = GetInnerRectBySizeAndBorder(size, originalBorder);
            var r1 = GetInnerRectBySizeAndBorder(rect.size, border);
            
      
            CalculateSliceSpriteScales(border, originalBorder, r1.size, r0.size);
            CalculateSliceSpriteOffsets(r0, r1);
            
            if (sliced)
            {
                // slice mode
                // ignore last eight verts and two quads: front and back faces
                // only side faces

                for (var i = 0; i < m_Vertices.Count - 9; ++i)
                {
                    var pos = m_Vertices[i];

                    var sliceIndex = GetSliceIndex(pos, r0);
                    var s = m_SliceSpriteScales[sliceIndex];
                    var offset = (Vector3)m_SliceSpriteOffset[sliceIndex];
                    
                    pos = new Vector3(pos.x * s.x, pos.y * s.y, pos.z * scale.z) + offset;
                    pos += new Vector3(0f, 0f, thickness * 0.5f);
                    
                    var vertexColor = generateUv ? color : sideColor;
                
                    m_Verts.Add(new UIVertex
                    {
                        position = pos,
                        normal = m_Normals[i],
                        uv0 = m_Uvs[i],
                        color = vertexColor,
                    });
                }
                m_Indices.AddRange(m_Triangles);
            }
            else
            {
                // simple mode
                for (var i = 0; i < m_Vertices.Count; ++i)
                {
                    var pos = m_Vertices[i];
                    pos = new Vector3(pos.x * scale.x, pos.y * scale.y, pos.z * scale.z);

                    pos += new Vector3(0f, 0f, thickness * 0.5f);
                    
                    var vertexColor = i >= m_Vertices.Count - 9 ? color : sideColor;
                    
                    if (generateUv)
                    {
                        vertexColor = color;
                    }
                
                    m_Verts.Add(new UIVertex
                    {
                        position = pos,
                        normal = m_Normals[i],
                        uv0 = m_Uvs[i],
                        color = vertexColor,
                    });
                }
                m_Indices.AddRange(m_Triangles);
            }
        }
        #endregion
    }
}
