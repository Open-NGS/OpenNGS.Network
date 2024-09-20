using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neptune;

public abstract class OrcaAgent
{
    public BattleActor Owner;
    public int Id;
    protected UVector2 Position;
    protected UVector2 LastPosition;
    protected UVector2 Velocity;
    protected UVector2 LastVelocity;
    protected List<OrcaAgent> AgentNeighbors;
    public List<OrcaObstacle> ObstacleNeighbors;

    public OrcaAgent()
    {
        AgentNeighbors = new List<OrcaAgent>();
        ObstacleNeighbors = new List<OrcaObstacle>();
    }

    public virtual void Init()
    {

    }
    protected abstract void ComputeNeighbors();
    protected abstract void ComputeNewVelocity(float dt);
    public abstract void Step(float dt);
    public abstract void Update(float dt);
    public abstract UVector2 GetVelocity();
    public abstract UVector2 GetPosition();
    public abstract UVector2 GetLastPosition();
    public abstract void SetPosition(UVector2 pos);
    public abstract int GetAgentNeighborDist();
    public abstract int GetObstacleNeighborDist();
    public abstract List<OrcaLine> GetOrcaLines();
    public abstract void AgentHasDead();
    public abstract void SetPreferredVelocity(UVector2 moveSpeed);
    public abstract void SetPreferredMaxSpeed(int speed);
    public abstract int GetRadius();

    public List<OrcaAgent> GetNeighbors()
    {
        return AgentNeighbors;
    }

    public BattleEntity GetOwner()
    {
        return Owner;
    }

    public void Sync()
    {
        LastPosition = Position;
        LastVelocity = Velocity;
    }
}