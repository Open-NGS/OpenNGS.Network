using UnityEngine;
using System.Collections.Generic;
using Neptune.Datas;

namespace Neptune
{
    public interface IWaypoint
    {
        List<PathData> ChoosePath(ArenaFlag flag, RoleSide roleSide);
        AStarNode GetNearestWaypoint(ArenaFlag flag, RoleSide roleSide, Vector2 position, Vector2 targetPosition, float radius);
        //void NotifyWaypointIgnorable(Vector2 position);
        ArenaFlag MarkArena(Vector2 position);
    }
}