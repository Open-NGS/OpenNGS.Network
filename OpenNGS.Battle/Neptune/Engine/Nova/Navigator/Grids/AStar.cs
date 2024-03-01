using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AStar
{
    private AStarNode _start;
    private AStarNode _end;

    private Dictionary<int, int> _open = new Dictionary<int, int>();   //待考察表 已经估价的节点 其中代价最小的那个 是下次计算的当前节点
    private List<AStarNode> _openList = new List<AStarNode>();
    private byte[] _closed; //已考察表 当前节点周围8个格子的代价估算完后 把当前节点放入此表

    private List<AStarNode> keyNodes = new List<AStarNode>();       // 用于存放 keynode 

    delegate float Heuristic(AStarNode grid);
    private Heuristic _heuristic;

    public AStar()
    {
        _heuristic = Diagonal;
    }

    public Stack<AStarNode> FindPath(AStarNode start, AStarNode end, int areaMask)
    {
        if (start == end || !start.Walkable || !end.Walkable)
        {
            return null;
        }

        _start = start;
        _end = end;

        _open.Clear();
        _openList.Clear();
        //每次查找路径时 新建一个 每次递归中 当前节点周围的8个节点被估价后放进此表
        //周围节点全估价后 对此表排序 移出代价最小的节点作为下次计算的当前节点

        if (_closed == null)
            _closed = new byte[NavLayer.NavMap.GridRows * NavLayer.NavMap.GridCols]; //每次查找路径时 新建一个 在整个递归查找过程中 只添加不移除
        else
            Array.Clear(_closed, 0, _closed.Length);

        _start.g = 0;
        _start.h = _heuristic(_start);
        _start.f = _start.g + _start.h;

        return Search(areaMask);
    }

    private Stack<AStarNode> Search(int areaMask)
    {
        AStarNode grid = _start; //当前格子 要给它周围的8个格子估价

        while (grid != _end)
        {
//#if BATTLE_LOG
//            if (EngineGlobal.BattleLog)
//            {
//                Logic.log("Current Node:{0}, start:{1}, end；{2}, delta:{3}", grid, _start.Position, _end.Position, _end.Position - _start.Position);
//            }
//#endif
            List<AStarNode> result = grid.GetNeighbors(_end.Position - _start.Position, areaMask);

            foreach (AStarNode neighbor in result)
            {
                if (!CheckNeighbor(neighbor, grid, areaMask))
                {
                    continue;
                }
            }

            _closed[grid.Index] = 1; //周围格子都估价过了 把当前格子添加进已考察表

            if (_open.Count == 0)
            {
                //待考察表为空 路径查找失败
                return null;
            }

            grid = RemoveMFG();
        }

        return BuildPath();
    }

    private bool CheckNeighbor(AStarNode neighbor, AStarNode grid, int areaMask)
    {
        if (neighbor == null)
        {
            return false;
        }
        AStarNode diagonalGrid1 = NavLayer.NavMap.GetGridByRC(grid.Row, neighbor.Col);
        AStarNode diagonalGrid2 = NavLayer.NavMap.GetGridByRC(neighbor.Row, grid.Col);

        if (neighbor == grid || !neighbor.Walkable || !diagonalGrid1.Walkable || !diagonalGrid2.Walkable)
        {
            //如果待估价格子是当前格子 或待估价格子不可走 并防止对角线穿越
            return false;
        }
        if ((neighbor.Area & areaMask) != neighbor.Area)
        {
            return false;
        }

        float mycost = NavLayer.NavMap.GetAreaCost(grid.Area);
        float neighborcost = NavLayer.NavMap.GetAreaCost(neighbor.Area);
        float cost = neighborcost;

        if (neighbor.Col != grid.Col && neighbor.Row != grid.Row)
        {
            //角上的4个格子 使用对角代价
            //cost = _dgCost;
            cost = Mathf.Sqrt(mycost * mycost + neighborcost * neighborcost);
        }

        float g = grid.g + cost * neighbor.costMultiplier;
        float h = _heuristic(neighbor);
        float f = g + h;

#if BATTLE_LOG
        if (EngineGlobal.BattleLog)
        {
            NeptuneBattle.log(string.Format("node:{0}, neighbor:{1}, greedCost:{2}, movementCost:{3}, finalCost:{4}", grid, neighbor, g, h, f));
        }
#endif

        bool inOpen = IsInOpen(neighbor);
        if (inOpen || IsInClosed(neighbor))
        {
            if (neighbor.f > f)
            {
                //如果上次估价大于这次估价 使用本次估价
                neighbor.f = f;
                neighbor.g = g;
                neighbor.h = h;
                neighbor.parent = grid;
            }
        }
        else
        {
            neighbor.f = f;
            neighbor.g = g;
            neighbor.h = h;
            neighbor.parent = grid;
            _openList.Add(neighbor);
            _open[neighbor.Index] = 1; //把估价过的周围格子添加进待考察表
        }
        return true;
    }

    private bool IsInOpen(AStarNode grid)
    {
        return _open.ContainsKey(grid.Index);
    }

    private bool IsInClosed(AStarNode grid)
    {
        return _closed[grid.Index] == 1;
    }

    /// <summary>
    /// 从待考察表中 移出代价最小的那个节点
    /// </summary>
    private AStarNode RemoveMFG()
    {
        float f = float.MaxValue;
        AStarNode grid = null;
        int index = 0;
        for (int i = 0; i < _openList.Count; i++)
        {
            AStarNode node = _openList[i];
            if (node.f < f)
            {
                f = node.f;
                grid = node;
                index = i;
            }
        }
        _open.Remove(grid.Index);
        _openList.RemoveAt(index);
        return grid;
    }

    private Stack<AStarNode> BuildPath()
    {
        keyNodes.Clear();

        Stack<AStarNode> path = new Stack<AStarNode>();
        AStarNode grid = _end;
        //_end.IsKeyNode = true;
        //_start.IsKeyNode = true;

        keyNodes.Add(_end);

        while (grid != _start)
        {
            path.Push(grid);
            AStarNode last = grid;
            grid = grid.parent;

            if (grid.parent != null && (grid.RowCol - last.RowCol) != (grid.parent.RowCol - grid.RowCol))
            {
                //grid.IsKeyNode = true;
                keyNodes.Add(grid);
            }
            //else
            //{
            //    grid.IsKeyNode = false;
            //}
        }

        keyNodes.Add(_start);

        return path;
    }

    public List<AStarNode> GetKeyNodes()
    {
        List<AStarNode> nodes = new List<AStarNode>();
        for (int i = keyNodes.Count - 1; i >= 0; i--)
        {
            nodes.Add(keyNodes[i]);
        }
        return nodes;
    }

    #region Heuristics

    private float Manhattan(AStarNode grid)
    {
        float cost = NavLayer.NavMap.GetAreaCost(grid.Area);// NavLayer.NavMap.GetAreaCost(NavArea.Walkable);
        return Math.Abs(grid.Col - _end.Col) * cost + Math.Abs(grid.Row - _end.Row) * cost;
    }

    private float Euclidian(AStarNode grid)
    {
        float cost = NavLayer.NavMap.GetAreaCost(grid.Area);// NavLayer.NavMap.GetAreaCost(NavArea.Walkable);
        float dc = grid.Col - _end.Col;
        float dr = grid.Row - _end.Row;

        float dist = (float)Math.Sqrt(dc * dc + dr * dr);

        return dist * cost;
    }

    private float Diagonal(AStarNode grid)
    {
        float cost = NavLayer.NavMap.GetAreaCost(grid.Area);
        float dc = Math.Abs(grid.Col - _end.Col);
        float dr = Math.Abs(grid.Row - _end.Row);

        float diag = Math.Min(dc, dr);
        float strt = dc + dr;

        return strt * cost + (1.41f - 2) * diag * cost;
        //return diag * cost + (strt - diag * 2) * cost;
    }

    #endregion
}