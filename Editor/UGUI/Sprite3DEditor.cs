using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OpenNGS.UI
{
	/// <summary>
	/// Custom Editor for Sprite3D Component
	/// </summary>
    [CustomEditor(typeof(Sprite3D), true)]
    public class Sprite3DEditor : Editor
    {
        private Sprite3D m_Sprite;
        private MeshPreview m_MeshPreview;

        private SerializedProperty m_RoundCornerRadius;
        private SerializedProperty m_Type;
        private SerializedProperty m_CustomMesh;
        private SerializedProperty m_Sprite2D;
        private SerializedProperty m_PixelThreshold;
        private SerializedProperty m_GenerateUv;
        private SerializedProperty m_MergeClosePoints;
        private SerializedProperty m_MergeDistance;
        
        private GUIContent m_TypeContent;
        private GUIContent m_MeshContent;
        private GUIContent m_GenerateUvContent;
        private GUIContent m_PixelThresholdContent;
        private GUIContent m_MergeDistanceContent;

        private void OnEnable()
        {
            m_Sprite = target as Sprite3D;
            
            m_RoundCornerRadius = serializedObject.FindProperty("roundCornerRadius");
            m_Type = serializedObject.FindProperty("type");
            m_CustomMesh = serializedObject.FindProperty("customMesh");
            m_Sprite2D = serializedObject.FindProperty("sprite2D");
            m_PixelThreshold = serializedObject.FindProperty("pixelThreshold");
            m_GenerateUv = serializedObject.FindProperty("generateUv");
            m_MergeClosePoints = serializedObject.FindProperty("mergeClosePoints");
            m_MergeDistance = serializedObject.FindProperty("mergeDistance");

            m_TypeContent = EditorGUIUtility.TrTextContent("Source Type");
            m_MeshContent = EditorGUIUtility.TrTextContent("Custom Mesh");
            m_GenerateUvContent = EditorGUIUtility.TrTextContent("Generate Side Mesh Uv");
            m_PixelThresholdContent = EditorGUIUtility.TrTextContent("Transparency Threshold");
            m_MergeDistanceContent = EditorGUIUtility.TrTextContent("Merge Distance Base on Sprite Size");
        }

        private void OnDisable()
        {
	        if (m_MeshPreview != null)
	        {
		        m_MeshPreview.Dispose();
		        m_MeshPreview = null;
	        }
        }

        public override void OnInspectorGUI()
        {
	        var oldType = m_Sprite.type;
	        
	        EditorGUILayout.PropertyField(m_Type, m_TypeContent);

	        switch (m_Sprite.type)
            {
                case Sprite3D.Type.Rectangle:
	                break;

                case Sprite3D.Type.RoundedRectangle:
                    EditorGUILayout.PropertyField(m_RoundCornerRadius);
                    break;
                
                case Sprite3D.Type.Sprite2D:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(m_Sprite2D);
                    
                    if (GUILayout.Button("Editor", EditorStyles.miniButtonRight, GUILayout.Width(55)))
                    {
                        if (m_Sprite.sprite2D != null)
                        {
                            // SpriteEditorWindow is a internal class, we can not call it's method
                            // Use EditorApplication.ExecuteMenuItem instead
                            // Set Selection.activeObject back after window show
                            // I am a genius :)
                            
                            Selection.activeObject = m_Sprite.sprite2D;
                            EditorApplication.ExecuteMenuItem("Window/2D/Sprite Editor");
                            Selection.activeObject = m_Sprite;
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(m_PixelThreshold, m_PixelThresholdContent);
                    EditorGUILayout.PropertyField(m_GenerateUv, m_GenerateUvContent);
                    EditorGUILayout.PropertyField(m_MergeClosePoints);
                    if (m_MergeClosePoints.boolValue)
                    {
	                    EditorGUILayout.PropertyField(m_MergeDistance, m_MergeDistanceContent);
                    }
                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("Generate Side Edge Mesh", GUILayout.Height(30)))
                    {
	                    GenerateMeshFromSprite();
	                    serializedObject.Update();
	                    serializedObject.ApplyModifiedProperties();
	                    EditorUtility.SetDirty(m_Sprite);
                    }

                    break;

                case Sprite3D.Type.CustomMesh:
                    EditorGUILayout.PropertyField(m_CustomMesh, m_MeshContent);
                    break;
            }
	        
	        serializedObject.ApplyModifiedProperties();
	        
            if (GUI.changed)
            {
	            if (oldType != m_Sprite.type)
	            {
		            if (m_Sprite.type != Sprite3D.Type.CustomMesh)
			            ClearCustomRefsAndData();

		            if (m_Sprite.type != Sprite3D.Type.Sprite2D)
			            ClearSprite2DRefsAndData();

		            serializedObject.Update();
		            serializedObject.ApplyModifiedProperties();
	            }
	            EditorUtility.SetDirty(m_Sprite);
            }
        }

        private void ClearSprite2DRefsAndData()
        {
	        m_Sprite.sprite2D = null;
			        
	        m_Sprite.Triangles.Clear();
	        m_Sprite.Vertices.Clear();
	        m_Sprite.Normals.Clear();
	        m_Sprite.Uvs.Clear();
	        m_Sprite.generateUv = false;
        }

        private void ClearCustomRefsAndData()
        {
	        m_Sprite.customMesh = null;
        }
        
        public override bool HasPreviewGUI()
        {
	        return true;
        }
        
        public override GUIContent GetPreviewTitle()
        {
	        switch (m_Sprite.type)
	        {
		        case Sprite3D.Type.CustomMesh:
			        return new GUIContent("Custom Mesh Preview");
		        case Sprite3D.Type.Sprite2D:
			        return new GUIContent("Sprite Generated Mesh Preview");
	        }

	        return new GUIContent("Mesh Preview");
        }

        private GUIStyle m_LabelStyle;
        private GUIStyle m_SpriteSizeLabelStyle;
        private Mesh m_RectanglePreviewMesh;

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
	        if (m_Sprite.type == Sprite3D.Type.Rectangle ||
	            m_Sprite.type == Sprite3D.Type.RoundedRectangle)
	        {
		        if (m_RectanglePreviewMesh == null)
			        m_RectanglePreviewMesh = new Mesh();
		        
		        m_RectanglePreviewMesh.Clear();

		        m_Sprite.GenerateSprite3D(new Rect(0f, 0f, 100f, 200f), 30f, Color.white);
		        var verts = new List<Vector3>();
		        var normals = new List<Vector3>();
		        
		        m_Sprite.Verts.ForEach(s =>
		        {
			        verts.Add(s.position);
			        normals.Add(s.normal);
		        });
		        
		        m_RectanglePreviewMesh.vertices = verts.ToArray();
		        m_RectanglePreviewMesh.triangles = m_Sprite.Indices.ToArray();
		        m_RectanglePreviewMesh.normals = normals.ToArray();
		        
		        PreviewMesh(m_RectanglePreviewMesh, r, background);
	        }
	        else if (m_Sprite.type == Sprite3D.Type.CustomMesh)
	        {
		        if (m_Sprite.customMesh)
		        {
			        PreviewMesh(m_Sprite.customMesh, r, background);
		        }
	        }
	        else if (m_Sprite.type == Sprite3D.Type.Sprite2D)
	        {
		        m_Sprite.spriteGeneratedMesh = m_Sprite.spriteGeneratedMesh ? m_Sprite.spriteGeneratedMesh : new Mesh();
		        
		        if (m_Sprite.Vertices.Count == 0 ||
		            m_Sprite.Normals.Count == 0 ||
		            m_Sprite.Triangles.Count == 0)
			        return;
		        try
		        {
			        m_Sprite.spriteGeneratedMesh.Clear();
			        m_Sprite.spriteGeneratedMesh.vertices = m_Sprite.Vertices.ToArray();
			        m_Sprite.spriteGeneratedMesh.normals = m_Sprite.Normals.ToArray();
			        m_Sprite.spriteGeneratedMesh.triangles = m_Sprite.Triangles.ToArray();

			        
			        PreviewMesh(m_Sprite.spriteGeneratedMesh, r, background);

			        m_LabelStyle ??= GUI.skin.GetStyle("Label");
			        m_LabelStyle.alignment = TextAnchor.LowerCenter;

			        GUI.Label(r, $"{m_Sprite.Vertices.Count} Vertices, {m_Sprite.Triangles.Count} Triangles", m_LabelStyle);
			        
			        if (m_Sprite.sprite2D != null)
			        {
				        m_SpriteSizeLabelStyle ??= GUI.skin.GetStyle("Label");
				        m_SpriteSizeLabelStyle.alignment = TextAnchor.UpperCenter;
				        var size = new Vector2(m_Sprite.sprite2D.texture.width, m_Sprite.sprite2D.texture.height);
				        GUI.Label(r, $"Sprite Width: {size.x}, Height: {size.y}", m_SpriteSizeLabelStyle);
			        }
		        }
		        catch (Exception e)
		        {
			        Debug.LogError(e.ToString());
		        }
	        }
        }

        private void PreviewMesh(Mesh mesh, Rect r, GUIStyle background)
        {
	        if (m_MeshPreview == null)
		        m_MeshPreview = new MeshPreview(mesh);
	        else
		        m_MeshPreview.mesh = mesh;
	        
	        m_MeshPreview.OnPreviewGUI(r, background);
        }

        private void GenerateMeshFromSprite()
        {
	        if (m_Sprite.sprite2D == null)
	        {
		        Debug.Log("Assign a sprite 2d texture first!");
		        return;
	        }
	        
	        // get the pixels to build the mesh from sprite texture
	        var path = AssetDatabase.GetAssetPath(m_Sprite.sprite2D.texture);
	        var textureImporter = (TextureImporter) AssetImporter.GetAtPath(path);
	        textureImporter.isReadable = true;
	        AssetDatabase.ImportAsset(path);

	        Color[] pixels = m_Sprite.sprite2D.texture.GetPixels();
	        textureImporter.isReadable = false;
	        AssetDatabase.ImportAsset(path);

	        int imageHeight = m_Sprite.sprite2D.texture.height;
	        int imageWidth = m_Sprite.sprite2D.texture.width;

	        // make a surface object to create and store data from image
	        var depth = 30f;
			
	        if (m_Sprite.spriteGeneratedMesh == null)
				m_Sprite.spriteGeneratedMesh = new Mesh();
	        
	        m_Sprite.spriteGeneratedMesh.Clear();

	        var halfDepth = depth / 2f;
	        var halfVerticalPixel = 0.5f / imageHeight;
	        var halfHorizontalPixel = 0.5f / imageWidth;

	        var sse = new SimpleSurfaceEdge(pixels, imageWidth, imageHeight, m_Sprite.pixelThreshold / 255.0f);
	        if (m_Sprite.mergeClosePoints)
	        {
		        sse.MergeClosePoints(m_Sprite.mergeDistance);
	        }

	        // if sse.ContainsIslands()
	        ArrayList allVertexLoops = sse.GetAllEdgeVertices();

	        ArrayList completeVertices = new ArrayList();
	        ArrayList completeIndices = new ArrayList();
	        ArrayList completeUVs = new ArrayList();

	        int verticesOffset = 0;
	        int loopCount = 0;

	        foreach (Vector2[] vertices2D in allVertexLoops)
	        {
		        Vector2[] uvs = new Vector2[vertices2D.Length * 4];
		        // Create the Vector3 vertices
		        Vector3[] vertices = new Vector3[vertices2D.Length * 4];

		        for (int i = 0; i < vertices2D.Length; ++i)
		        {
			        vertices2D[i] *= new Vector2(-1f, 1f);
			        
			        // get X point and normalize
			        float vertX = 1f - vertices2D[i].x / imageWidth - halfHorizontalPixel;

			        // get Y point and normalize
			        float vertY = vertices2D[i].y / imageHeight + halfVerticalPixel;

			        // scale X and position centered
			        vertX = vertX * imageWidth - imageWidth * 1.5f;
			        vertY = vertY * imageHeight - imageHeight / 2f;

			        vertices[i] = new Vector3(vertX, vertY, -halfDepth);
			        vertices[i + vertices2D.Length] = new Vector3(vertX, vertY, halfDepth);

			        uvs[i] = m_Sprite.generateUv ? sse.GetUVForIndex(loopCount, i) : new Vector2(-2, -2);
			        uvs[i + vertices2D.Length] = uvs[i];
			        uvs[i + vertices2D.Length * 2] = uvs[i];
			        uvs[i + vertices2D.Length * 3] = uvs[i];
		        }

		        // make the back side triangle indices
		        // double the indices for front and back, 6 times the number of edges on front
		        int[] allIndices = new int[vertices2D.Length * 6];

		        // create the side triangle indices
		        // for each edge, create a new set of two triangles
		        // edges are just two points from the original set
		        for (int i = 0; i < vertices2D.Length - 1; ++i)
		        {
			        allIndices[6 * i + 0] = i + 1 + verticesOffset;
			        allIndices[6 * i + 1] = i + verticesOffset;
			        allIndices[6 * i + 2] = i + 1 + vertices2D.Length + verticesOffset;
			        allIndices[6 * i + 3] = i + 1 + vertices2D.Length + verticesOffset;
			        allIndices[6 * i + 4] = i + verticesOffset;
			        allIndices[6 * i + 5] = i + vertices2D.Length + verticesOffset;
		        }

		        // wrap around for the last face
		        allIndices[^6] = 0 + verticesOffset;
		        allIndices[^5] = vertices2D.Length - 1 + verticesOffset;
		        allIndices[^4] = vertices2D.Length + verticesOffset;
		        allIndices[^3] = vertices2D.Length + verticesOffset;
		        allIndices[^2] = vertices2D.Length - 1 + verticesOffset;
		        allIndices[^1] = vertices2D.Length * 2 - 1 + verticesOffset;

		        foreach (Vector3 v in vertices)
		        {
			        completeVertices.Add(v);
		        }

		        foreach (Vector2 v in uvs)
		        {
			        completeUVs.Add(v);
		        }

		        foreach (int i in allIndices)
		        {
			        completeIndices.Add(i);
		        }

		        verticesOffset += vertices.Length;
		        loopCount++;
	        }
	        
	        for (var i = 0; i < 2; ++i)
	        {
		        var index = completeVertices.Count;
		        var z = i == 0 ? halfDepth : -halfDepth;
		        var offset = new Vector3(imageWidth * 0.5f, imageHeight * 0.5f, 0f);
	    
		        var pos0 = new Vector3(0f, 0f, z) - offset;
		        var pos1 = new Vector3(0f, imageHeight, z) - offset;
		        var pos2 = new Vector3(imageWidth, imageHeight, z) - offset;
		        var pos3 = new Vector3(imageWidth, 0f, z) - offset;
		        completeVertices.Add(pos0);
		        completeVertices.Add(pos1);
		        completeVertices.Add(pos2);
		        completeVertices.Add(pos3);
				 
		        if (i == 1)
					completeIndices.AddRange(new[] {index, index + 1, index + 2, index, index + 2, index + 3});
		        else
			        completeIndices.AddRange(new[] {index + 3, index + 2, index, index + 2, index + 1, index});
		        
		        completeUVs.Add(new Vector2(0, 0));
		        completeUVs.Add(new Vector2(0, 1));
		        completeUVs.Add(new Vector2(1, 1));
		        completeUVs.Add(new Vector2(1, 0));
	        }

			m_Sprite.spriteGeneratedMesh.vertices = (Vector3[]) completeVertices.ToArray(typeof(Vector3));
			m_Sprite.spriteGeneratedMesh.triangles = (int[]) completeIndices.ToArray(typeof(int));
	        m_Sprite.spriteGeneratedMesh.uv = (Vector2[]) completeUVs.ToArray(typeof(Vector2));
	        m_Sprite.spriteGeneratedMesh.RecalculateNormals();
	        m_Sprite.spriteGeneratedMesh.RecalculateBounds();
	        
	        m_Sprite.Triangles.Clear();
	        m_Sprite.Vertices.Clear();
	        m_Sprite.Normals.Clear();
	        m_Sprite.Uvs.Clear();
	        
	        m_Sprite.Triangles.AddRange(m_Sprite.spriteGeneratedMesh.triangles);
	        m_Sprite.Vertices.AddRange(m_Sprite.spriteGeneratedMesh.vertices);
	        m_Sprite.Normals.AddRange(m_Sprite.spriteGeneratedMesh.normals);
	        m_Sprite.Uvs.AddRange(m_Sprite.spriteGeneratedMesh.uv);
	    }
    }
}