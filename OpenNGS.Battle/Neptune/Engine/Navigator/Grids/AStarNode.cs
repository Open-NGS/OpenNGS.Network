using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 格子的行列索引和坐标 创建后不会再变
/// </summary>
public class AStarNode
{
    public int Index;
    public int Row { get { return (int)rowCol.x; } }
    public int Col { get { return (int)rowCol.y; } }
    public UVector2 RowCol { get { return rowCol; } }
    public UVector2 Position { get { return _position; } }
    private UVector2 rowCol;

    //public bool IsKeyNode;路径点为公用数据  不能存特有数据
    public int Area
    {
        get
        {
            if (overlayArea > NavArea.AllArea)
                return overlayArea;
            return area;
        }
    }
    int area;
    int overlayArea = NavArea.AllArea;

    public bool Walkable
    {
        get
        {
            return (this.Area & NavArea.NotWalkable) != NavArea.NotWalkable;
        }
    }

    public bool IsObstacle
    {
        get
        {
            return (this.Area & NavArea.Obstacle) == NavArea.Obstacle;
        }
    }

    public bool IsWater
    {
        get
        {
            return (this.Area & NavArea.Water) == NavArea.Water;
        }
    }

    public AStarNode parent;
    private AStarNode[] neighbors;

    public float f; //一个节点的全部代价 等于g+h
    public float g; //起点到当前点的代价 它是确定的 因为肯定知道从起点到这一点的实际路径
    public float h; //当前点到终点的估计代价 是用估价函数算出的 它只能是一个估计值 因为还不知道接下来要怎么走

    public float costMultiplier = 1;

    private Vector2 _position; //方格位置 中心点在网格中的坐标

    public AStarNode(int row, int col, int area)
    {
        rowCol = new Vector2(row, col);
        this.Index = row * NavLayer.NavMap.GridCols + col;
        this.area = area;
        _position.x = NavLayer.NavMap.GridSize * row + NavLayer.NavMap.GridSize / 2 - NavLayer.NavMap.Height / 2;
        _position.y = NavLayer.NavMap.GridSize * col + NavLayer.NavMap.GridSize / 2 - NavLayer.NavMap.Width / 2;
    }


    public void SetNeighbors(AStarNode[] neighbors)
    {
        this.neighbors = neighbors;
    }

    public List<AStarNode> GetNeighbors(Vector2 dir, int areaMask)
    {
        int idir = 0;
        if (dir.x == 0 && dir.y > 0)
            idir = 0;
        if (dir.x > 0 && dir.y > 0)
            idir = 1;
        if (dir.x > 0 && dir.y == 0)
            idir = 2;
        if (dir.x > 0 && dir.y < 0)
            idir = 3;
        if (dir.x == 0 && dir.y < 0)
            idir = 4;
        if (dir.x < 0 && dir.y < 0)
            idir = 5;
        if (dir.x < 0 && dir.y == 0)
            idir = 6;
        if (dir.x < 0 && dir.y > 0)
            idir = 7;

        StringBuilder sb = new StringBuilder();

        List<AStarNode> result = new List<AStarNode>();
        for (int i = 0; i < 8; i++)
        {
            int index = (i + idir) % 8;
            if (index < this.neighbors.Length)
            {
                AStarNode node = this.neighbors[index];

#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                {
                    if (node != null)
                    {
                        string log = string.Format("currentNode:{0}, neighborIndex:{1}, neighbor:{2}, areaMask:{3}", this, index, node,node.Area);
                        sb.AppendLine(log);
                    }
                }
#endif
                //if (node != null && (((node.Position - this.Position).normalized + dir.normalized).magnitude > 0) && (node.Area == NavArea.Walkable || (node.Area & areaMask) == node.Area))
				if (node != null && (node.Area == NavArea.Walkable || (node.Area & areaMask) == node.Area))
                    result.Add(node);
            }
        }
#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
        {
            NeptuneBattle.log(sb.ToString());
        }
#endif
        return result;
    }

    public void AddArea(int area)
    {
        this.overlayArea = area;
    }

    public void RemoveArea(int area)
    {
        this.overlayArea = NavArea.AllArea;
    }

    public override string ToString()
    {
        return string.Format("AStarNode[{0},{1}][{2}]({3})", rowCol.x, rowCol.y, this.Index, this.Position);
    }
}