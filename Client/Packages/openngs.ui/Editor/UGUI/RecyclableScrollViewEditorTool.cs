using UnityEngine;
using UnityEditor;

namespace OpenNGS.UI
{
    [ExecuteInEditMode]
    public static class RecyclableScrollViewEditorTool
    {
        const string PrefabPath = "Prefab/Recyclable Scroll View";

        [MenuItem("GameObject/UI/Recyclable Scroll View")]
        private static void CreateRecyclableScrollView()
        {
            GameObject selected = Selection.activeGameObject;

            // If selected isn't a UI gameObject then find a Canvas
            if (!selected || !(selected.transform is RectTransform))
            {
                selected = GameObject.FindObjectOfType<Canvas>().gameObject;
            }

            if (!selected) return;

            GameObject asset = Resources.Load(PrefabPath, typeof(GameObject)) as GameObject;

            GameObject item = Object.Instantiate(asset, selected.transform);
            item.name = "Recyclable Scroll View";
            item.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = item;
            Undo.RegisterCreatedObjectUndo(item, "Create Recycalable Scroll view");
        }
    }
}
