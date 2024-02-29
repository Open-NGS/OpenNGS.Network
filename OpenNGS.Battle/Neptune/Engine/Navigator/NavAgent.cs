using UnityEngine;
using System.Collections.Generic;
using Neptune.GameData;
using UnityEngine.AI;
using Neptune;

public abstract class NavAgent
{
    public ArenaFlag BirthArena;
    public NavResult NavResult;

    public int AreaMask;
    protected float[] AreaCosts = new float[NavArea.Max];
    protected BattleEntity Owner;

    public UVector2 NextPosition;

    public NavAgent(BattleEntity element)
    {
        this.Owner = element;
        this.AreaMask = -1;
    }
    public NavAgent(BattleEntity element, int AreaMask)
    {
        this.Owner = element;
        this.AreaMask = AreaMask;
    }
    public virtual void Init()
    {
        NextPosition = EngineConst.UVector2Zero;
    }
    public abstract void SetDestination(UVector2 destination, bool forceRPF = false, NavMeshPath path = null);
    public abstract int Move(float time);
    public int MonitorLength;
    public abstract bool MonitorPosition(Vector2 position);
    //public abstract void ResetDestination(Vector2 destination);

    public virtual float GetAreaCost(int area)
    {
        return AreaCosts[area];
    }

    public virtual void SetAreaCost(int area, float cost)
    {
        this.AreaCosts[area] = cost;
    }

    public abstract bool Walkable(UVector2 destination);
    public abstract UVector2 GetNearWalkablePos(UVector3 sourcePos, UVector3 targetPos);
    public abstract UVector2 GetObstacleHit(UVector3 sourcePos, UVector3 targetPos, bool ignore);
    public abstract UVector2 GetTeleportPos(UVector3 sourcePos, UVector3 targetPos);
    public abstract UVector2 GetDashObstacleHit(UVector3 sourcePos, UVector3 targetPos, bool ignore);
    public abstract int GetArea(UVector2 position);
}