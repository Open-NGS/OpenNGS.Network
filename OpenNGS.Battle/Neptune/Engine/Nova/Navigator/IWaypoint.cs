using UnityEngine;
using System.Collections.Generic;
using Neptune.GameData;

public interface IWaypoint
{
    List<NodeData> ChoosePath(ArenaFlag flag, RoleSide roleSide);
    AStarNode GetNearestWaypoint(ArenaFlag flag, RoleSide roleSide, Vector2 position, Vector2 targetPosition, float radius);
    //void NotifyWaypointIgnorable(Vector2 position);
    ArenaFlag MarkArena(Vector2 position);
}