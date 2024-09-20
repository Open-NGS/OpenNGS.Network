using UnityEngine;

public class OrcaObstacle
{
    public OrcaObstacle Next;
    public OrcaObstacle Previous;
    public Vector2 Direction;
    public UVector2 Point;
    public int Id;
    public bool Convex;
    public bool Active;

    public OrcaObstacleStatus Status;

    public OrcaObstacle()
    {
        Active = true;
    }
}

public enum OrcaObstacleStatus
{
    STATIC = 0,
    DYNAMIC = 1
}