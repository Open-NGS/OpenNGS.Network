using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 1. 网格的中心 对齐逻辑坐标系的原点
/// 2. 格子索引从右下角到左上角递增
/// 3. 客场 逻辑坐标系翻转 网格不翻转 即第2条不变
/// </summary>
public class NavGrid : INavMap
{
    public int GridRows
    {
        get
        {
            return StaticGrids.rows;
        }
    }

    public int GridCols
    {
        get
        {
            return StaticGrids.cols;
        }
    }

    public float GridSize
    {
        get
        {
            return StaticGrids.size;
        }
    }


    public float Width
    {
        get
        {
            return GridSize * GridCols;
        }
    }

    public float Height
    {
        get
        {
            return GridSize * GridRows;
        }
    }

    //public static NavGrid Instance;

    protected float[] CustomAreaCosts = new float[NavArea.Max];
    protected static float[] AreaCosts = new float[NavArea.Max];

    protected static List<AStarNode> NavNodes;
    protected static AStar _aStar;

    NavGridData StaticGrids;

    public NavGrid()
    {
        _aStar = new AStar();

        InitDefaultCost();
    }

    //public static void Build()
    //{
    //    Instance = new NavGrid();
    //}

    public static List<AStarNode> GetGrids()
    {
        return NavNodes;
    }

    public AStarNode GetGridByRC(int row, int col)
    {
        if (row > this.GridRows - 1 || row < 0)
        {
            //行索引超出范围
            return null;
        }

        if (col > this.GridCols - 1 || col < 0)
        {
            //列索引超出范围
            return null;
        }

        return NavNodes[this.GridCols * row + col];
    }

    public AStarNode GetGridByLogicPos(UVector2 logicPos)
    {
        Vector2 rc = BattleField.LogicPos2RC(logicPos);
        return GetGridByRC((int)rc.x, (int)rc.y);
    }

    public Stack<AStarNode> FindPath(UVector2 startPos, UVector2 endPos, int areaMask)
    {
        AStarNode startGrid = GetGridByLogicPos(startPos);
        AStarNode endGrid = GetGridByLogicPos(endPos);

        Stack<AStarNode> result = null;
        if (startGrid != null)
        {
            result = _aStar.FindPath(startGrid, endGrid, areaMask);
        }
        Array.Clear(this.CustomAreaCosts, 0, this.CustomAreaCosts.Length);
        return result;
    }


    public virtual void SetAdditionalArea(UVector2 position, float radius, int area, bool add)
    {
        for (int i = (int)(position.x - radius); i <= (int)(position.x + radius); i += (int)this.GridSize)
        {
            for (int j = (int)(position.y - radius); j <= (int)(position.y + radius); j += (int)this.GridSize)
            {
                Vector2 grid = BattleField.LogicPos2RC(new Vector2(i, j));
                int row = (int)grid.x;
                int col = (int)grid.y;

                row = Mathf.Clamp(row, 0, this.GridRows - 1);
                col = Mathf.Clamp(col, 0, this.GridCols - 1);

                if (add)
                    this.GetGridByRC(row, col).AddArea(area);
                else
                    this.GetGridByRC(row, col).RemoveArea(area);
            }
        }
    }

    public List<AStarNode> GetKeyNodes()
    {
        return _aStar.GetKeyNodes();
    }

    public virtual List<UVector2> GetAddtionalVertices(UVector2 position, float radius)
    {
        List<UVector2> vertices = new List<UVector2>();

        UVector2 vertice1 = new UVector2(position.x + radius, position.y + radius);
        UVector2 vertice2 = new UVector2(position.x - radius, position.y + radius);
        UVector2 vertice3 = new UVector2(position.x - radius, position.y - radius);
        UVector2 vertice4 = new Vector2(position.x + radius, position.y - radius);

        vertices.Add(vertice1);
        vertices.Add(vertice2);
        vertices.Add(vertice3);
        vertices.Add(vertice4);

        return vertices;
    }

    public virtual List<AStarNode> GetNodesOfSpecificArea(UVector2 position, float radius, int cost)
    {
        List<AStarNode> temp = new List<AStarNode>();

        for (int i = (int)(position.x - radius); i <= (int)(position.x + radius); i += (int)this.GridSize)
        {
            for (int j = (int)(position.y - radius); j <= (int)(position.y + radius); j += (int)this.GridSize)
            {
                Vector2 grid = BattleField.LogicPos2RC(new Vector2(i, j));
                int row = (int)grid.x;
                int col = (int)grid.y;

                row = Mathf.Clamp(row, 0, this.GridRows - 1);
                col = Mathf.Clamp(col, 0, this.GridCols - 1);

                AStarNode aStarNode = this.GetGridByRC(row, col);
                if ((aStarNode.Area & cost) == cost)
                {
                    temp.Add(aStarNode);
                }
            }
        }

        return temp;
    }
    public void SetAdditionalObstacleArea(Vector2 position, float minRadius, float maxRadius, int area, bool add)
    {

        for (int i = (int)(position.x - maxRadius); i <= (int)(position.x + maxRadius); i += (int)this.GridSize)
        {
            for (int j = (int)(position.y - maxRadius); j <= (int)(position.y + maxRadius); j += (int)this.GridSize)
            {
                var pos = new Vector2(i, j);
                var pLength = (position - pos).sqrMagnitude;
                if (pLength > maxRadius * maxRadius || pLength< minRadius* minRadius)
                {
                    continue;
                }
                Vector2 grid = BattleField.LogicPos2RC(new Vector2(i, j));
                int row = (int)grid.x;
                int col = (int)grid.y;

                row = Mathf.Clamp(row, 0, this.GridRows - 1);
                col = Mathf.Clamp(col, 0, this.GridCols - 1);

                if (add)
                    this.GetGridByRC(row, col).AddArea(area);
                else
                    this.GetGridByRC(row, col).RemoveArea(area);
            }
        }
    }

    protected virtual void InitDefaultCost()
    {
        for (int i = 0; i < NavArea.Max; i++)
        {
            AreaCosts[i] = i == NavArea.Obstacle ? 10 : 1;
        }
    }

    public float GetAreaCost(int area)
    {

        float cost = 0;
        if (CustomAreaCosts[area] > 0)
        {
            cost = CustomAreaCosts[area];
        }
        else
        {
            cost = AreaCosts[area];
        }
        return cost;
    }

    public void SetAreaCost(int area, float cost)
    {
        this.CustomAreaCosts[area] = cost;
    }

    //public static bool CheckNavLayerMask(Vector2 xy, NavLayer layer)
    //{
    //int x = (int)xy.x;
    //int y = (int)xy.y;

    //Grid temp = GetGridByRC(x, y);
    //if (temp != null)
    //{
    //    return (temp.layerVal & 1 << (int) (Math.Log10((int) layer)/Math.Log10(2))) == (int) layer;
    //}
    //else
    //{
    //    return false;
    //}
    //}

    //public static bool CheckNavLayerMask(AStarNode grid, NavLayer layer)
    //{
    //    return CheckNavLayerMask(new Vector2(grid.Row,grid.Col), layer);
    //}

    //public static int GetNavLayerVal(int row, int col)
    //{
    //    return InitialMap[NumRows - row - 1, col] + AdditionalMap[NumRows - row - 1, col];
    //}

    //public static NavResult GetNavResult(Vector2 xy)
    //{
    //    if (CheckNavLayerMask(xy,NavLayer.OBSTACLE))
    //    {
    //        return NavResult.OBSTACLE;
    //    }
    //    if (CheckNavLayerMask(xy,NavLayer.RIVER))
    //    {
    //        return NavResult.JUMP;
    //    }
    //    return NavResult.SUCCESS;
    //}

    public static bool Belong2Obstacle(AStarNode obstacleGrid, AStarNode grid)
    {
        return false;
    }

    public bool LoadData(byte[] data)
    {
        StaticGrids = new NavGridData(data);

        NavNodes = new List<AStarNode>();

        for (int row = 0; row < GridRows; row++)
        {
            for (int col = 0; col < GridCols; col++)
            {
                AStarNode grid = new AStarNode(row, col, this.StaticGrids.GetArea(row, col));
                NavNodes.Add(grid);
            }
        }


        PreProcessNodes();
        return true;
    }

    void PreProcessNodes()
    {
        for (int i = 0; i < NavNodes.Count; i++)
        {
            FindNeighborNodes(NavNodes[i]);
        }
    }

    static List<Vector2> neighorOffset = new List<Vector2>()
    {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,0),
            new Vector2(-1,1),
    };

    void FindNeighborNodes(AStarNode node)
    {

        AStarNode[] neighbors = new AStarNode[8];
        for (int i = 0; i < 8; i++)
        {
            Vector2 item = node.RowCol + neighorOffset[i];
            AStarNode grid = NavLayer.NavMap.GetGridByRC((int)item.x, (int)item.y);
            if (grid != null && ((grid.Area & NavArea.NotWalkable) != NavArea.NotWalkable))
            {
                neighbors[i] = grid;
            }
        }
        node.SetNeighbors(neighbors);
    }

    public static Vector2 GetRCByLogicPos(Vector2 logicPos)
    {
        Vector2 rc = BattleField.LogicPos2RC(logicPos);
        return new Vector2((int)rc.x, (int)rc.y);
    }
}