using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpenNGS.UI
{
    /// <summary>
    /// Class for Create Sprite3D Assets Menu
    /// </summary>
    internal static class Sprite3DContextMenu
    {
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Rectangle")]
        static void AssetsCreateSpriteRectangle(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Rectangle.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Sprite 2D")]
        static void AssetsCreateSpriteSprite2D(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.Sprite2D;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Sprite3D.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Rounded Rectangle")]
        static void AssetsCreateSpriteRoundedRectangleRadius10(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.RoundedRectangle;
            asset.roundCornerRadius = 10;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Rounded Rectangle.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Rounded Rectangle@15x")]
        static void AssetsCreateSpriteRoundedRectangleRadius15(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.RoundedRectangle;
            asset.roundCornerRadius = 15;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Rounded Rectangle.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Rounded Rectangle@20x")]
        static void AssetsCreateSpriteRoundedRectangleRadius20(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.RoundedRectangle;
            asset.roundCornerRadius = 20;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Rounded Rectangle.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Rounded Rectangle@30x")]
        static void AssetsCreateSpriteRoundedRectangleRadius30(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.RoundedRectangle;
            asset.roundCornerRadius = 30;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Rounded Rectangle.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
        
        [MenuItem("Assets/Create/3D/Sprite 3D/Custom Mesh")]
        static void AssetsCreateSpriteCustomMesh(MenuCommand menuCommand)
        {
            var asset = ScriptableObject.CreateInstance<Sprite3D>();
            asset.type = Sprite3D.Type.CustomMesh;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path += "/New Custom Mesh.asset";
            ProjectWindowUtil.CreateAsset(asset, path);
        }
    }
}
