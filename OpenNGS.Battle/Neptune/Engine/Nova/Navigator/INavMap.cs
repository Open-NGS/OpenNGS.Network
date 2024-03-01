using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface INavMap
{
    int GridRows { get; }
    int GridCols { get; }
    float GridSize { get; }

    float Width { get; }
    float Height { get; }

    /// <summary>
    /// 加载导航数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    bool LoadData(byte[] data);

    AStarNode GetGridByRC(int row, int col);
    AStarNode GetGridByLogicPos(UVector2 logicPos);

    Stack<AStarNode> FindPath(UVector2 startPos, UVector2 endPos, int areaMask);
    float GetAreaCost(int area);
    void SetAreaCost(int area, float cost);


    void SetAdditionalArea(UVector2 position, float radius, int area,bool add);

    List<AStarNode> GetKeyNodes();
    List<UVector2> GetAddtionalVertices(UVector2 position, float radius);
    List<AStarNode> GetNodesOfSpecificArea(UVector2 position, float radius, int cost);
}
