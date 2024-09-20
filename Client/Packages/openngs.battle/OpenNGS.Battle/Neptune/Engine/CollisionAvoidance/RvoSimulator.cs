using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neptune.Datas;
#if UNITY_EDITOR
    using UnityEditor;
#endif


namespace Neptune
{
    public enum RvoContainType
    {
        All = 0,
        Agent,
        AgentHero,
    }

    public class RvoSimulator : IOrcaSimulator
    {
        public TwoDTree _2DTreeStatic;
        public TwoDTree _2DTreeDynamic;

        public List<RvoAgent> AgentsHero
        {
            get { return agentsHero; }
        }

        public List<RvoAgent> Agents
        {
            get { return agents; }
        }

        public List<OrcaObstacle> Obstacles
        {
            get { return obstacles; }
        }

        public Dictionary<UVector2, List<RvoAgent>> SuperposeAgents
        {
            get { return superposeAgents; }
        }

        public int MaxSpeedThreshold
        {
            get { return normalMaxSpeedThreshold; }
        }

        public float MinSpeedFactor
        {
            get { return normalMinSpeedFactor; }
        }

        public bool ForceSpeed
        {
            get { return normalForceSpeed; }
        }

        public bool RepelCollisionWithBuilding
        {
            get { return normalRepelCollisionWithBuilding; }
        }

        private static RvoSimulator instance = null;
        private List<RvoAgent> agents;
        private List<RvoAgent> agentsHero;
        private List<OrcaObstacle> obstacles;
        private Dictionary<UVector2, List<RvoAgent>> superposeAgents = new Dictionary<UVector2, List<RvoAgent>>();
        private int normalNeighborDist = NeptuneConst.RVO_NeighborDist;
        private int normalNeighborObstDist = NeptuneConst.RVO_NeighborObstDist;
        private float normalNeighborTimeHorizon = NeptuneConst.RVO_NeighborTimeHorizon;
        private float normalNeighborObstTimeHorizon = NeptuneConst.RVO_NeighborObstTimeHorizon;
        private int normalMaxSpeedThreshold = NeptuneConst.RVO_MaxSpeedThreshold;
        private float normalMinSpeedFactor = NeptuneConst.RVO_MinSpeedFactor;
        private bool normalForceSpeed = NeptuneConst.RVO_ForceSpeed;
        private bool normalRepelCollisionWithBuilding = NeptuneConst.RVO_RepelCollision_Building;

        private RvoSimulator()
        {
            globalObstNo = 0;
            agents = new List<RvoAgent>();
            agentsHero = new List<RvoAgent>();
            obstacles = new List<OrcaObstacle>();
            _2DTreeStatic = new TwoDTree();
            _2DTreeDynamic = new TwoDTree();
        }

        public static RvoSimulator Instance
        {
            get { return instance ?? (instance = new RvoSimulator()); }
        }

        public OrcaAgent CreateAgent(Actor ele, UVector2 pos, int radius, int maxSpeed, RvoContainType type)
        {
            float neighborHorizon = normalNeighborTimeHorizon;
            float neighborObstHorizon = normalNeighborObstTimeHorizon;
            int neighborDist = ele.Data.RoleType == RoleType.Building ? radius : normalNeighborDist;
            int neighborObstDist = normalNeighborObstDist;

            RvoAgent agent = new RvoAgent(ele, pos, radius, maxSpeed, neighborHorizon, neighborObstHorizon, neighborDist, neighborObstDist, agents.Count + 1, type);
            if (type == RvoContainType.AgentHero)
            {
                agentsHero.Add(agent);
            }
            else
            {
                agents.Add(agent);
            }


#if BATTLE_LOG
            NeptuneBattle.log("Add RVO Agent Name: {0} Position: {1}", ele.FullName, ele.Position);
#endif

            return agent;
        }

        public void ResetDetectionParams(int neighborDist, int neighborObstDist, float neighborTimeHorizon, float neighborObstTimeHorizon, float minSpeedFactor, int maxSpeedThreshold, bool forceSpeed, bool replCollisionWithBuilding)
        {
            normalNeighborDist = neighborDist;
            normalNeighborObstDist = neighborObstDist;
            normalNeighborTimeHorizon = neighborTimeHorizon;
            normalNeighborObstTimeHorizon = neighborObstTimeHorizon;
            normalMinSpeedFactor = minSpeedFactor;
            normalMaxSpeedThreshold = maxSpeedThreshold;
            normalForceSpeed = forceSpeed;
            normalRepelCollisionWithBuilding = replCollisionWithBuilding;
        }

        public void RemoveAgent(Actor ele)
        {
            if (ele != null)
            {
                agents.Remove((RvoAgent)ele.OrcaAgent);

                if (((RvoAgent)ele.OrcaAgent).RvoContainType == RvoContainType.AgentHero)
                {
                    agentsHero.Remove((RvoAgent)ele.OrcaAgent);
                }
                else
                {
                    agents.Remove((RvoAgent)ele.OrcaAgent);
                }
            }
        }

        public int GetNumAgents()
        {
            return agents.Count + agentsHero.Count;
        }

        public List<OrcaAgent> GetAllAgents()
        {
            List<OrcaAgent> orcaAgents = new List<OrcaAgent>();

            foreach (var agent in agentsHero)
            {
                orcaAgents.Add(agent);
            }

            foreach (var agent in agents)
            {
                orcaAgents.Add(agent);
            }

            return orcaAgents;
        }

        public List<OrcaObstacle> GetAllObstacles()
        {
            return Obstacles;
        }

        private int globalObstNo;
        private List<int> obstIds = new List<int>();

        public int AddObstacle(IList<UVector2> vertices, OrcaObstacleStatus status = OrcaObstacleStatus.STATIC)
        {
            if (vertices.Count < 2)
            {
                return -1;
            }
            obstIds.Clear();

            globalObstNo = obstacles.Count;

            int obstacleNo = globalObstNo;

            for (int i = 0; i < vertices.Count; i++)
            {
                OrcaObstacle orcaObstacle = new OrcaObstacle();
                orcaObstacle.Point = vertices[i];

                if (i != 0)
                {
                    orcaObstacle.Previous = obstacles[globalObstNo - 1];
                    orcaObstacle.Previous.Next = orcaObstacle;
                }

                if (i == vertices.Count - 1)
                {
                    orcaObstacle.Next = obstacles[obstacleNo];
                    orcaObstacle.Next.Previous = orcaObstacle;
                }

                orcaObstacle.Direction = (vertices[i == vertices.Count - 1 ? 0 : i + 1] - vertices[i]).normalized;
                UFloat.Round(ref orcaObstacle.Direction);

                if (vertices.Count == 2)
                {
                    orcaObstacle.Convex = true;
                }
                else
                {
                    UVector2 a = vertices[i == 0 ? vertices.Count - 1 : i - 1];
                    UVector2 b = vertices[i];
                    UVector2 c = vertices[i == vertices.Count - 1 ? 0 : i + 1];
                    orcaObstacle.Convex = MathUtil.LeftOf(a, b, c) > 0 && UVector2.Angle(a - b, c - b) < 175;
                }

                orcaObstacle.Id = globalObstNo;
                orcaObstacle.Status = status;
                obstacles.Add(orcaObstacle);
                globalObstNo++;
                obstIds.Add(orcaObstacle.Id);
            }

            return obstacleNo;
        }

        public void ClearNeighbors()
        {
            agents.Clear();
            agentsHero.Clear();
            obstacles.Clear();
            globalObstNo = 0;
        }

        public void AgentDead(OrcaAgent agent)
        {
            RvoAgent temp = agent as RvoAgent;
            if (temp != null)
            {
                temp.AgentHasDead();
                if (temp.RvoContainType == RvoContainType.AgentHero)
                {
                    AgentsHero.Remove(temp);
                }
                else
                {

                    Agents.Remove(temp);
                }
            }
        }


        public void RemoveObstacleAll()
        {
            obstacles.Clear();
        }


        public void RemoveObstacle(int obstacleId)
        {
            List<OrcaObstacle> temp2Rm = new List<OrcaObstacle>();
            OrcaObstacle obstacle = obstacles.Find(new ObstaclePredicater(obstacleId).PredicateID);
            if (obstacle != null)
            {
                temp2Rm.Add(obstacle);
                OrcaObstacle temp = obstacle;
                while (temp.Next != obstacle)
                {
                    temp2Rm.Add(temp = temp.Next);
                }
            }

            int tempCount = temp2Rm.Count;
            for (int i = 0; i < tempCount; i++)
            {
                OrcaObstacle temp = obstacles.Find(new ObstaclePredicater(temp2Rm[i].Id).PredicateID);
                if (temp != null)
                {
                    if (temp.Next != null)
                    {
                        temp.Next.Previous = null;
                        temp.Next = null;
                    }

                    temp.Active = false;
                }
            }
        }

        public List<int> GetObstacleIdsOfEachBuilding()
        {
            List<int> temp = new List<int>();
            int count = obstIds.Count;
            for (int i = 0; i < count; i++)
            {
                temp.Add(obstIds[i]);
            }
            return temp;
        }

        public void ComputeSuperposeAgent()
        {
            superposeAgents = new Dictionary<UVector2, List<RvoAgent>>();

            int agentCount = agents.Count;

            for (int i = 0; i < agentCount; i++)
            {
                RvoAgent agent = agents[i];
                bool flag = superposeAgents.Keys.Contains(agent.GetLastPosition());
                if (flag)
                {
                    superposeAgents[agent.GetLastPosition()].Add(agent);
                }
                else
                {
                    List<RvoAgent> temp = new List<RvoAgent>();
                    temp.Add(agent);
                    superposeAgents.Add(agent.GetLastPosition(), temp);
                }
            }
        }


        public void ProcessObstacles(OrcaObstacleStatus status = OrcaObstacleStatus.STATIC)
        {
            if (status == OrcaObstacleStatus.STATIC)
            {
                _2DTreeStatic.BuildObstacleTree();
            }
            else
            {
                _2DTreeDynamic.BuildObstacleTree(OrcaObstacleStatus.DYNAMIC);
            }
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                instance.agents.Clear();
                instance.agentsHero.Clear();
                instance.obstacles.Clear();
                instance._2DTreeStatic = null;
                instance._2DTreeDynamic = null;
                instance = null;
            }

        }

        public void OnDrawGizmos(IEditorHelper helper)
        {
            if (obstacles == null)
            {
                return;
            }
            Color[] colors = new Color[4];
            colors[0] = new Color(1, 0, 0, 1F);
            colors[1] = new Color(0, 1, 1, 1F);
            colors[2] = new Color(1, 0, 1, 1F);
            colors[3] = new Color(1, 1, 0, 1F);
            Gizmos.color = new Color(1, 0, 0, 1F);
            int clolorIndex = 0;
            //OrcaObstacle current = obstacles[0];
            //for (int i = 0; i < obstacles.Count; i++)
            //{
            //    if (i%4 == 0)
            //    {
            //        clolorIndex = 0;
            //    }
            //    Gizmos.color = colors[clolorIndex];
            //    clolorIndex++;
            //    Gizmos.DrawLine(new Vector3(current.Point.x  0.01f, 0.5f, current.Point.y  0.01f), new Vector3(current.Next.Point.x  0.01f, 0.5f, current.Next.Point.y  0.01f));
            //    Gizmos.DrawCube(new Vector3(current.Point.x  0.01f, 0.5f, current.Point.y  0.01f),Vector3.one);
            //    current = current.Next;
            //}
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (i % 4 == 0)
                {
                    clolorIndex = 0;
                }
                Gizmos.color = colors[clolorIndex];
                clolorIndex++;

                if (obstacles[i].Next == null)
                {
                    continue;
                }
                Gizmos.DrawLine(new Vector3(obstacles[i].Point.x * 0.01f, 0.5f, obstacles[i].Point.y * 0.01f), new Vector3(obstacles[i].Next.Point.x * 0.01f, 0.5f, obstacles[i].Next.Point.y * 0.01f));

                //#if UNITY_EDITOR
                //            GUIStyle style = new GUIStyle();
                //            style.normal.textColor = obstacles[i].Status == OrcaObstacleStatus.STATIC ? Color.white : Color.red;
                //            Handles.Label(new Vector3(obstacles[i].Point.x * 0.01f, 0.5f, obstacles[i].Point.y * 0.01f), obstacles[i].Id.ToString(), style);
                //#endif

                //Gizmos.DrawCube(new Vector3(obstacles[i].Point.x  0.01f, 0.5f, obstacles[i].Point.y  0.01f), Vector3.one);
#if UNITY_EDITOR
                if (helper != null)
                    helper.Label(new Vector3(obstacles[i].Point.x * 0.01f, 0.5f, obstacles[i].Point.y * 0.01f), obstacles[i].Id.ToString(), obstacles[i].Convex ? Color.white : Color.green);
#endif

            }

            //if (obstacles.Count != 4)
            //{
            //    return;
            //}
            //Gizmos.color = new Color(1, 0, 0, 1F);
            //foreach (OrcaObstacle obstacle in obstacles)
            //{
            //    Gizmos.DrawLine(new Vector3(obstacle.Point.x  0.01f, 0.5f, obstacle.Point.y  0.01f), new Vector3(obstacle.Next.Point.x  0.01f, 0.5f, obstacle.Next.Point.y  0.01f));
            //}
            //for (int i = 0; i < vertices1.Count; i += 4)
            //{
            //    Gizmos.DrawLine(new Vector3(vertices1[i].x  0.01f, 0.5f, vertices1[i].y  0.01f), new Vector3(vertices1[i + 1].x  0.01f, 0.5f, vertices1[i + 1].y  0.01f));
            //    Gizmos.DrawLine(new Vector3(vertices1[i + 1].x  0.01f, 0.5f, vertices1[i + 1].y  0.01f), new Vector3(vertices1[i + 2].x  0.01f, 0.5f, vertices1[i + 2].y  0.01f));
            //    Gizmos.DrawLine(new Vector3(vertices1[i + 2].x  0.01f, 0.5f, vertices1[i + 2].y  0.01f), new Vector3(vertices1[i + 3].x  0.01f, 0.5f, vertices1[i + 3].y  0.01f));
            //    Gizmos.DrawLine(new Vector3(vertices1[i + 3].x  0.01f, 0.5f, vertices1[i + 3].y  0.01f), new Vector3(vertices1[i].x  0.01f, 0.5f, vertices1[i].y  0.01f));
            //}
        }
    }
}