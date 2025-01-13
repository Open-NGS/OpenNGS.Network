using System.Collections.Generic;
using UnityEngine;

namespace Neptune
{

    public interface IOrcaSimulator
    {
        OrcaAgent CreateAgent(Actor ele, UVector2 pos, int radius, int maxSpeed, RvoContainType type);
        void ResetDetectionParams(int neighborDist, int neighborObstDist, float neighborTimeHorizon, float neighborObstTimeHorizon, float minSpeedFactor, int maxSpeedThreshold, bool forceSpeed, bool replCollisionWithBuilding);
        void RemoveAgent(Actor ele);
        int GetNumAgents();
        List<OrcaAgent> GetAllAgents();
        List<OrcaObstacle> GetAllObstacles();
        int AddObstacle(IList<UVector2> vertices, OrcaObstacleStatus status = OrcaObstacleStatus.STATIC);
        void ClearNeighbors();
        void AgentDead(OrcaAgent agent);
        void RemoveObstacleAll();
        void RemoveObstacle(int obstacleId);
        void ProcessObstacles(OrcaObstacleStatus status = OrcaObstacleStatus.STATIC);
        List<int> GetObstacleIdsOfEachBuilding();
        void ComputeSuperposeAgent();
    }
}