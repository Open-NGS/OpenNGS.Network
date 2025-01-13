using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class NavGridTool
{
    private static GameObject NavGridRoot;

    /// <summary>
    /// 碰撞数据
    /// </summary>
    public static List<Vector3[]> NavData = new List<Vector3[]>();

    /// <summary>
    /// 增加碰撞组
    /// </summary>
    [MenuItem("GameObject/NavgridTool/AddNavGroup", false, 0)]
    static void AddNavGroup()
    {
        ReflushData();
        if (NavGridRoot == null)
        {
            NavGridRoot  = new GameObject("NavGridRoot");
            NavGridRoot.transform.SetParent(NavGridRoot.transform);
        }
        GameObject obj = new GameObject("Group_"+ NavData.Count);
        obj.transform.SetParent(NavGridRoot.transform);
        NavGridGroup gridGroup= obj.AddComponent<NavGridGroup>();
        gridGroup.AddNode(Vector3.down);
        gridGroup.AddNode(Vector3.down);
        gridGroup.AddNode(Vector3.down);
        gridGroup.AddNode(Vector3.down);
    }

    /// <summary>
    /// 增加碰撞点
    /// </summary>
    [MenuItem("GameObject/NavgridTool/AddNavGroupUnit1", false, 0)]
    static void AddNavGroupUnit()
    {
        NavGridGroup gridGroup = UnityEditor.Selection.activeGameObject.GetComponent<NavGridGroup>();
        if (gridGroup != null)
        {
            gridGroup.AddNode(Vector3.down);
        }
        ReflushMesh();
    }
    
    /// <summary>
    /// 增加碰撞点
    /// </summary>
    [MenuItem("GameObject/NavgridTool/AddNavGroupUnit4", false, 0)]
    static void AddNavGroupUnit4()
    {
        NavGridGroup gridGroup = UnityEditor.Selection.activeGameObject.GetComponent<NavGridGroup>();
        if (gridGroup != null)
        {
            gridGroup.AddNode(Vector3.down);
            gridGroup.AddNode(Vector3.down);
            gridGroup.AddNode(Vector3.down);
            gridGroup.AddNode(Vector3.down);
        }
        ReflushMesh();
    }/// <summary>
     /// 增加碰撞点
     /// </summary>
    [MenuItem("GameObject/NavgridTool/DelteNavGroupUnit", false, 0)]
    static void DelteNavGroupUnit()
    {
        NavGridGroup gridGroup = UnityEditor.Selection.activeGameObject.transform.parent.GetComponent<NavGridGroup>();
        if (gridGroup != null)
        {
            gridGroup.DeleteNode(UnityEditor.Selection.activeGameObject);
        }
        ReflushMesh();
    }
    /// <summary>
    /// 刷新碰撞体显示
    /// </summary>
    [MenuItem("GameObject/NavgridTool/ReflushData", false, 0)]
    static void ReflushData()
    {
        NavData.Clear();
        NavGridRoot = GameObject.Find("NavGridRoot");
        if (NavGridRoot != null)
        {
            NavGridGroup[] gridGroup = NavGridRoot.GetComponentsInChildren<NavGridGroup>();

            foreach (NavGridGroup navNode in gridGroup)
            {
                navNode.ReflushMesh();
                NavData.Add(navNode.GetNavGroupData());
            }
        }
    }

    static void ReflushMesh()
    {
        if (NavGridRoot == null)
        {
            NavGridRoot = GameObject.Find("NavGridRoot");
        }
        if (NavGridRoot != null)
        {
            NavGridGroup[] gridGroup = NavGridRoot.GetComponentsInChildren<NavGridGroup>();

            foreach (NavGridGroup navNode in gridGroup)
            {
                navNode.ReflushMesh();
            }
        }
    }

    public static void ImportData()
    {
        AssetDatabase.Refresh();
        string path = "Assets/Game/BuildAssets/NavData/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "nav.bytes";
        TextAsset obj = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (obj != null)
        {
            int NavDataSize = BitConverter.ToInt32(obj.bytes, 0);
            int ObstacleCountSize = BitConverter.ToInt32(obj.bytes, 4);
            int ObstacleDataSize = BitConverter.ToInt32(obj.bytes, 8);

            byte[] ObstacleCountData = new byte[ObstacleCountSize];
            byte[] ObstacleData = new byte[ObstacleDataSize];

            if (NavDataSize != 0)
            {
                Array.Copy(obj.bytes, 12 + NavDataSize, ObstacleCountData, 0, ObstacleCountSize);
                Array.Copy(obj.bytes, 12 + NavDataSize + ObstacleCountSize, ObstacleData, 0, ObstacleDataSize);

                List<int> verticesCount =MathUtil.BitConverterToInt(ObstacleCountData);
                List<UVector2> verticesData = MathUtil.BitConverterToVector2s(ObstacleData);
                NavData.Clear();
                int pointIndex = 0;
                for (int i = 0; i < verticesCount.Count; i++)
                {
                    Vector3[] pointArr = new Vector3[verticesCount[i]];
                    List<UVector2> pointList = verticesData.GetRange(pointIndex, verticesCount[i]);
                    for (int j = 0; j < pointList.Count; j++)
                    {
                        pointArr[j] = new Vector3(pointList[j].x*Const.WorldLogicFactor, 0,
                            pointList[j].y*Const.WorldLogicFactor);
                    }
                    NavData.Add(pointArr);
                    pointIndex += verticesCount[i];
                }
            }

            NavGridRoot = GameObject.Find("NavGridRoot");
            Object.DestroyImmediate(NavGridRoot);
            if (NavGridRoot == null)
            {
                NavGridRoot = new GameObject("NavGridRoot");
                NavGridRoot.transform.SetParent(NavGridRoot.transform);
            }
            for (int i = 0; i < NavData.Count; i++)
            {
                GameObject oGroupobj = new GameObject("Group_" + i);
                oGroupobj.transform.SetParent(NavGridRoot.transform);
                NavGridGroup gridGroup = oGroupobj.AddComponent<NavGridGroup>();
                for (int j = 0; j < NavData[i].Length; j++)
                {
                    gridGroup.AddNode(NavData[i][j]);
                }
            }
        }

    }
}

