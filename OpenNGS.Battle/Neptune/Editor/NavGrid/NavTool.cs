using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using Neptune;

public class NavTool
{

    static NavGridRoot navroot;

    static void BuildPathData()
    {

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        navroot = GameObject.FindAnyObjectByType<NavGridRoot>();

        mapPathsBytes = new byte[navroot.Rows * navroot.Cols];
        BuildGrids();
        Byte[] verticesCountBytes ;
        Byte[] verticesDataBytes ;

        buildObstacle(out verticesCountBytes, out verticesDataBytes);

        string fileName = "Assets/Game/BuildAssets/NavData/" + scene.name + "nav.bytes";
        FileStream fs = File.Create(fileName);

        // mapPathsBytes length  4
        // Obstacle  Length 4
        // navroot.Rows 2
        // navroot.Cols 2
        // navroot.Size 4
        // mapPathsBytes
        // Obstacle
        fs.Write(BitConverter.GetBytes(mapPathsBytes.Length+8), 0, 4);
        fs.Write(BitConverter.GetBytes(verticesCountBytes.Length), 0, 4);
        fs.Write(BitConverter.GetBytes(verticesDataBytes.Length), 0, 4);
        fs.Write(BitConverter.GetBytes(navroot.Rows), 0, 2);
        fs.Write(BitConverter.GetBytes(navroot.Cols), 0, 2);
        fs.Write(BitConverter.GetBytes(navroot.Size), 0, 4);
        fs.Write(mapPathsBytes,0, mapPathsBytes.Length);
        fs.Write(verticesCountBytes, 0, verticesCountBytes.Length);
        fs.Write(verticesDataBytes, 0, verticesDataBytes.Length);
        fs.Close();
        EditorUtility.DisplayDialog("Nova Navigation System", "NavGrid build sucessed", "OK");
        AssetDatabase.Refresh();
    }

    private static byte[] mapPathsBytes ;

    private static void BuildGrids()
    {
        List<string> Layers = new List<string>();
        UnityEngine.AI.NavMeshHit hit;
        Vector3 worldPos = new Vector3(0, 1, 0);
        Ray ray = new Ray(Vector3.zero, Vector3.down);
        for (int row = 0; row < navroot.Rows; row++)
        {
            for (int col = 0; col < navroot.Cols; col++)
            {
                worldPos = GridToWorldPostion(new Vector2(row , col ));

                worldPos.y = 10;
                RaycastHit hit1;
                ray.origin = worldPos;
                if (Physics.Raycast(ray, out hit1))
                {
                    //if (hit1.collider.gameObject.tag == "Obstacle")
                    {
                        mapPathsBytes[row * navroot.Rows + col] = 1;
                    }

                    //if (!Layers.Contains(hit1.collider.gameObject.tag))
                    //{
                    //    Layers.Add(hit1.collider.gameObject.tag);
                    //}
                }

                //NavMesh.FindClosestEdge(worldPos, out hit, 1);
                //if (hit.distance < 0.01f)
                //{
                //    mapPathsBytes[row * navroot.Rows + col] = (byte)(hit.distance < 1 ? 1 : 0);
                //}
                
            }
        }
        //DebugShowSelf.mapPathsBytes = mapPathsBytes;
    }
    public static Vector3 GridToWorldPostion(Vector2 gridpos)
    {
        gridpos.x -= navroot.Cols*0.5f;
        gridpos.y -= navroot.Rows * 0.5f;
        var logicpos = new Vector2(navroot.Size / 2f + gridpos.x* navroot.Size, navroot.Size / 2f + gridpos.y* navroot.Size);
        Vector3 world = new Vector3(logicpos.x / BattleField.LogicWorldFactor, 0, logicpos.y / BattleField.LogicWorldFactor);
        return world;
    }





    static Dictionary<byte, Color32> ColorMap = new Dictionary<byte, Color32>()
    {

    };

    static Color32 GetColor(byte data)
    {
        Color32 color = new Color32();
        if (data == 0)
            return new Color32(255, 255, 255, 255);
        if((data & 1) == 1)
        {
            color.r = 255;
            color.g = data;
            color.b = data;
        }else 
        if ((data & 2) == 2)
        {
            color.r = data;
            color.g = data;
            color.b = 255;
        }else
        {
            color.r = data;
            color.g = 255;
            color.b = data;
        }
        color.a = 255;
        return color;
    }
    [MenuItem("NavGrid/Build NavGrid")]
    static void SaveData()
    {
        BuildPathData();
    }

    [MenuItem("NavGrid/ImportNavGrid")]
    static void ImportNavGridData()
    {
        NavGridTool.ImportData();
    }

    //[MenuItem("NavGrid/SaveNavGrid2")]
    static void SaveData2()
    {
        int a = UnityEngine.AI.NavMesh.GetAreaFromName("Walkable");//G
        int b = UnityEngine.AI.NavMesh.GetAreaFromName("Not Walkable");//R
        int c = UnityEngine.AI.NavMesh.GetAreaFromName("Jump");//B

        Debug.Log("Walkable:" + a);
        Debug.Log("Not Walkable:" + b);
        Debug.Log("Jump:" + c);
        return;

        short NumRows = 64;
        short NumCols = 36;
        float GridSize = 50f;

        byte[] file = File.ReadAllBytes("Assets/StreamingAssets/NavGrid.bytes");

        FileStream fs = File.Create("Assets/StreamingAssets/NavGrid2.bytes");
        fs.Write(BitConverter.GetBytes(NumRows),0,2);
        fs.Write(BitConverter.GetBytes(NumCols), 0, 2);
        fs.Write(BitConverter.GetBytes(GridSize), 0, 4);
        fs.Write(file, 0, file.Length);
        fs.Close();
    }
    private static void buildObstacle(out Byte[] ObstacleCount, out Byte[] ObstacleData)
    {
        int typefSize = sizeof (float);
        int typeiSize = sizeof(float);
        List<float> vertices  = new List<float>();
        List<int> verticesCount = new List<int>();
        for (int i = 0; i < NavGridTool.NavData.Count; i++)
        {
            GetUnitVertices(NavGridTool.NavData[i], ref vertices,ref verticesCount);
        }

        ObstacleCount = new byte[verticesCount.Count * typeiSize ];
        ObstacleData = new byte[vertices.Count * typefSize];
        //加入每组数量数据
        for (int i = 0; i < verticesCount.Count; i++)
        {
            Array.Copy(BitConverter.GetBytes(verticesCount[i]),0, ObstacleCount, i * typeiSize, typeiSize);
        }
        
        for (int i = 0; i < vertices.Count; i++)
        {
            Array.Copy(BitConverter.GetBytes(vertices[i]), 0, ObstacleData, i * typefSize, typefSize);
        }
    }

    private static void GetUnitVertices(Transform transform, ref List<float> vertices)
    {
        var point = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(-0.5f, 0,0.5f));
        vertices.Add(point.x * BattleField.LogicWorldFactor);
        vertices.Add(point.z * BattleField.LogicWorldFactor);
        point = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(-0.5f , 0, -0.5f));
        vertices.Add(point.x * BattleField.LogicWorldFactor);
        vertices.Add(point.z * BattleField.LogicWorldFactor);
        point = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0.5f , 0, -0.5f ));
        vertices.Add(point.x * BattleField.LogicWorldFactor);
        vertices.Add(point.z * BattleField.LogicWorldFactor);
        point = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0.5f, 0, 0.5f ));
        vertices.Add(point.x * BattleField.LogicWorldFactor);
        vertices.Add(point.z * BattleField.LogicWorldFactor);
    }


    private static void GetUnitVertices(Vector3[] vectorArray, ref List<float> vertices,ref List<int> verticesCount)
    {
        verticesCount.Add(vectorArray.Length);
        for (int i = 0; i < vectorArray.Length; i++)
        {
            vertices.Add(vectorArray[i].x * BattleField.LogicWorldFactor);
            vertices.Add(vectorArray[i].z * BattleField.LogicWorldFactor);
        }
    }
}
