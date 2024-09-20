using UnityEngine;

public struct OrcaLine
{
    public Vector2 Direction;
    public UVector2 Point;
    public OrcaLineType Type;
}


public enum OrcaLineType
{
    OBSTACLE,
    AGENT
}