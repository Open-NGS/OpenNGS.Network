using Neptune;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDTree
{
    private const int MAX_LEAF_SIZE = 10;

    private RvoAgent[] agents;
    private AgentTreeNode[] agentTree;

    private ObstacleTreeNode obstacleTree;

    private class AgentTreeNode
    {
        internal int Begin;
        internal int End;
        internal int Left;
        internal int Right;
        internal int MaxX;
        internal int MaxY;
        internal int MinX;
        internal int MinY;
    }

    private class ObstacleTreeNode
    {
        internal OrcaObstacle Obstacle;
        internal ObstacleTreeNode Left;
        internal ObstacleTreeNode Right;
    };

    /**
     * <summary>Defines a pair of scalar values.</summary>
     */

    private struct ValuePair
    {
        private int a_;
        private int b_;

        /**
         * <summary>Constructs and initializes a pair of scalar
         * values.</summary>
         *
         * <param name="a">The first scalar value.</returns>
         * <param name="b">The second scalar value.</returns>
         */

        internal ValuePair(int a, int b)
        {
            a_ = a;
            b_ = b;
        }

        /**
         * <summary>Returns true if the first pair of scalar values is less
         * than the second pair of scalar values.</summary>
         *
         * <returns>True if the first pair of scalar values is less than the
         * second pair of scalar values.</returns>
         *
         * <param name="pair1">The first pair of scalar values.</param>
         * <param name="pair2">The second pair of scalar values.</param>
         */

        public static bool operator <(ValuePair pair1, ValuePair pair2)
        {
            return pair1.a_ < pair2.a_ || !(pair2.a_ < pair1.a_) && pair1.b_ < pair2.b_;
        }

        /**
         * <summary>Returns true if the first pair of scalar values is less
         * than or equal to the second pair of scalar values.</summary>
         *
         * <returns>True if the first pair of scalar values is less than or
         * equal to the second pair of scalar values.</returns>
         *
         * <param name="pair1">The first pair of scalar values.</param>
         * <param name="pair2">The second pair of scalar values.</param>
         */

        public static bool operator <=(ValuePair pair1, ValuePair pair2)
        {
            return (pair1.a_ == pair2.a_ && pair1.b_ == pair2.b_) || pair1 < pair2;
        }

        /**
         * <summary>Returns true if the first pair of scalar values is
         * greater than the second pair of scalar values.</summary>
         *
         * <returns>True if the first pair of scalar values is greater than
         * the second pair of scalar values.</returns>
         *
         * <param name="pair1">The first pair of scalar values.</param>
         * <param name="pair2">The second pair of scalar values.</param>
         */

        public static bool operator >(ValuePair pair1, ValuePair pair2)
        {
            return !(pair1 <= pair2);
        }

        /**
         * <summary>Returns true if the first pair of scalar values is
         * greater than or equal to the second pair of scalar values.
         * </summary>
         *
         * <returns>True if the first pair of scalar values is greater than
         * or equal to the second pair of scalar values.</returns>
         *
         * <param name="pair1">The first pair of scalar values.</param>
         * <param name="pair2">The second pair of scalar values.</param>
         */

        public static bool operator >=(ValuePair pair1, ValuePair pair2)
        {
            return !(pair1 < pair2);
        }
    }

    public void BuildAgentTree()
    {
        int agentCount = NeptuneBattle.Instance.Simulator.GetNumAgents();
        if (agents == null || agents.Length != agentCount)
        {
            agents = new RvoAgent[agentCount];

            List<OrcaAgent> allAgents = NeptuneBattle.Instance.Simulator.GetAllAgents();
            for (int i = 0; i < agents.Length; i++)
            {
                agents[i] = allAgents[i] as RvoAgent;
            }

            agentTree = new AgentTreeNode[agentCount * 2];

            for (int i = 0; i < agentTree.Length; i++)
            {
                agentTree[i] = new AgentTreeNode();
            }
        }

        if (agents.Length != 0)
        {
            BuildAgentTreeRecursive(0, agents.Length, 0);
        }
    }

    public void BuildObstacleTree(OrcaObstacleStatus status = OrcaObstacleStatus.STATIC)
    {
        obstacleTree = new ObstacleTreeNode();
        IList<OrcaObstacle> obstacles = new List<OrcaObstacle>();

        List<OrcaObstacle> orcaObstacles = RvoSimulator.Instance.Obstacles.FindAll(new ObstaclePredicater(status).PredicateStatus);
        int count = orcaObstacles.Count;
        for (int i = 0; i < count; i++)
        {
            OrcaObstacle orcaObstacle = orcaObstacles[i];

            if (orcaObstacle.Active)
            {
                obstacles.Add(orcaObstacle);
            }
        }

        obstacleTree = BuildObstacleTreeRecursive(obstacles, status);
    }

    public void ComputeAgentNeighbors(RvoAgent agent, ref int rangeSq, int mask)
    {
        QueryAgentTreeRecursive(agent, ref rangeSq, 0, mask);
    }

    public void ComputeObstacleNeighbors(RvoAgent rvoAgent, int rangeSq)
    {
        QueryObstacleTreeRecursive(rvoAgent, rangeSq, obstacleTree);
    }

    private void QueryAgentTreeRecursive(RvoAgent agent, ref int rangeSq, int node, int mask)
    {
        if (agentTree[node].End - agentTree[node].Begin <= MAX_LEAF_SIZE)
        {
            for (int i = agentTree[node].Begin; i < agentTree[node].End; i++)
            {
                if ((int)agents[i].RvoContainType == mask)
                {
                    agent.InsertAgentNeighbor(agents[i], ref rangeSq);
                }
            }
        }
        else
        {
            long distSqLeft =
                MathUtil.Sqr((long)Math.Max(0, agentTree[agentTree[node].Left].MinX - agent.GetPosition().x)) +
                MathUtil.Sqr((long)Math.Max(0, agent.GetPosition().x - agentTree[agentTree[node].Left].MaxX)) +
                MathUtil.Sqr((long)Math.Max(0, agentTree[agentTree[node].Left].MinY - agent.GetPosition().y)) +
                MathUtil.Sqr((long)Math.Max(0, agent.GetPosition().y - agentTree[agentTree[node].Left].MaxY));

            long distSqRight =
                MathUtil.Sqr((long)Math.Max(0, agentTree[agentTree[node].Right].MinX - agent.GetPosition().x)) +
                MathUtil.Sqr((long)Math.Max(0, agent.GetPosition().x - agentTree[agentTree[node].Right].MaxX)) +
                MathUtil.Sqr((long)Math.Max(0, agentTree[agentTree[node].Right].MinY - agent.GetPosition().y)) +
                MathUtil.Sqr((long)Math.Max(0, agent.GetPosition().y - agentTree[agentTree[node].Right].MaxY));

            if (distSqLeft < distSqRight)
            {
                if (distSqLeft < rangeSq)
                {
                    QueryAgentTreeRecursive(agent, ref rangeSq, agentTree[node].Left, mask);

                    if (distSqRight < rangeSq)
                    {
                        QueryAgentTreeRecursive(agent, ref rangeSq, agentTree[node].Right, mask);
                    }
                }
            }
            else
            {
                if (distSqRight < rangeSq)
                {
                    QueryAgentTreeRecursive(agent, ref rangeSq, agentTree[node].Right, mask);

                    if (distSqLeft < rangeSq)
                    {
                        QueryAgentTreeRecursive(agent, ref rangeSq, agentTree[node].Left, mask);
                    }
                }
            }
        }
    }

    private void QueryObstacleTreeRecursive(RvoAgent rvoAgent, int rangeSq, ObstacleTreeNode obstacleTreeNode)
    {
        if (obstacleTreeNode != null)
        {
            int agentLeftOfLine = MathUtil.LeftOf(obstacleTreeNode.Obstacle.Point, obstacleTreeNode.Obstacle.Next.Point, rvoAgent.GetLastPosition());
#if BATTLE_LOG
            if (rvoAgent.CanOutputBattlelog())
                NeptuneBattle.log("QueryObstacleTreeRecursive 1:{0},{1},{2},{3}", agentLeftOfLine, obstacleTreeNode.Obstacle.Point, obstacleTreeNode.Obstacle.Next.Point, rvoAgent.GetPosition());
#endif
            QueryObstacleTreeRecursive(rvoAgent, rangeSq, agentLeftOfLine >= 0 ? obstacleTreeNode.Left : obstacleTreeNode.Right);

            int x = obstacleTreeNode.Obstacle.Next.Point.x - obstacleTreeNode.Obstacle.Point.x;
            int y = obstacleTreeNode.Obstacle.Next.Point.y - obstacleTreeNode.Obstacle.Point.y;

            int division = (x * x + y * y);
            if (division != 0)
            {
                long distSqLine = (long)agentLeftOfLine  * agentLeftOfLine / division;
                //int distSqLine =MathUtil.Sqr(agentLeftOfLine) / (obstacleTreeNode.Obstacle.Next.Point - obstacleTreeNode.Obstacle.Point).sqrMagnitude;
#if BATTLE_LOG
                if (rvoAgent.CanOutputBattlelog())
                    NeptuneBattle.log("QueryObstacleTreeRecursive 2:{0},{1},{2},{3}", distSqLine, agentLeftOfLine, division, rangeSq);
#endif
                if (distSqLine < rangeSq)
                {
                    if (agentLeftOfLine < 0)
                    {
                        rvoAgent.InsertObstacleNeighbor(obstacleTreeNode.Obstacle, rangeSq);
                    }

                    QueryObstacleTreeRecursive(rvoAgent, rangeSq,
                        agentLeftOfLine > 0 ? obstacleTreeNode.Right : obstacleTreeNode.Left);
                }
            }
        }
    }

    private void BuildAgentTreeRecursive(int begin, int end, int node)
    {
        agentTree[node].Begin = begin;
        agentTree[node].End = end;

        UVector2 agentPosition = agents[begin].GetPosition();
        agentTree[node].MinX = agentTree[node].MaxX = agentPosition.x;
        agentTree[node].MinY = agentTree[node].MaxY = agentPosition.y;

        for (int i = begin + 1; i < end; i++)
        {
            UVector2 tempPosition = agents[i].GetPosition();
            agentTree[node].MaxX = Math.Max(agentTree[node].MaxX, tempPosition.x);
            agentTree[node].MinX = Math.Min(agentTree[node].MinX, tempPosition.x);
            agentTree[node].MaxY = Math.Max(agentTree[node].MaxY, tempPosition.y);
            agentTree[node].MinY = Math.Min(agentTree[node].MinY, tempPosition.y);
        }

        if (end - begin > MAX_LEAF_SIZE)
        {
            bool isVertical = agentTree[node].MaxX - agentTree[node].MinX > agentTree[node].MaxY - agentTree[node].MinY;
            int splitValue = (isVertical ? agentTree[node].MaxX + agentTree[node].MinX : agentTree[node].MaxY + agentTree[node].MinY) / 2;

            int left = begin;
            int right = end;

            while (left < right)
            {
                while (left < right && (isVertical ? agents[left].GetPosition().x : agents[left].GetPosition().y) < splitValue)
                {
                    left++;
                }

                while (left < right && (isVertical ? agents[right - 1].GetPosition().x : agents[right - 1].GetPosition().y) >= splitValue)
                {
                    right--;
                }

                if (left < right)
                {
                    RvoAgent tempAgent = agents[left];
                    agents[left] = agents[right - 1];
                    agents[right - 1] = tempAgent;

                    left++;
                    right--;
                }
            }

            int leftSize = left - begin;

            if (leftSize == 0)
            {
                leftSize++;
                left++;
                right++;
            }

            agentTree[node].Left = node + 1;
            agentTree[node].Right = node + 2 * leftSize;

            BuildAgentTreeRecursive(begin, left, agentTree[node].Left);
            BuildAgentTreeRecursive(left, end, agentTree[node].Right);
        }
    }

    private ObstacleTreeNode BuildObstacleTreeRecursive(IList<OrcaObstacle> obstacles, OrcaObstacleStatus status = OrcaObstacleStatus.STATIC)
    {
        if (obstacles.Count == 0)
        {
            return null;
        }

        ObstacleTreeNode node = new ObstacleTreeNode();

        int optimalSplit = 0;
        int minLeft = obstacles.Count;
        int minRight = obstacles.Count;

        for (int i = 0; i < obstacles.Count; i++)
        {
            int leftSize = 0;
            int rightSize = 0;

            OrcaObstacle obstacleI1 = obstacles[i];
            OrcaObstacle obstacleI2 = obstacleI1.Next;

            for (int j = 0; j < obstacles.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                OrcaObstacle obstacleJ1 = obstacles[j];
                OrcaObstacle obstacleJ2 = obstacleJ1.Next;

                int j1LeftOfI = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, obstacleJ1.Point);
                int j2LeftOfI = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, obstacleJ2.Point);

                if (j1LeftOfI >= 0 && j2LeftOfI >= 0)
                {
                    leftSize++;
                }
                else if (j1LeftOfI <= 0 && j2LeftOfI <= 0)
                {
                    rightSize++;
                }
                else
                {
                    leftSize++;
                    rightSize++;
                }

                if (new ValuePair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) >= new ValuePair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
                {
                    break;
                }
            }

            if (new ValuePair(Math.Max(leftSize, rightSize), Math.Min(leftSize, rightSize)) < new ValuePair(Math.Max(minLeft, minRight), Math.Min(minLeft, minRight)))
            {
                minLeft = leftSize;
                minRight = rightSize;
                optimalSplit = i;
            }
        }
        {
            /* Build split node */
            IList<OrcaObstacle> leftObstacles = new List<OrcaObstacle>();

            for (int n = 0; n < minLeft; n++)
            {
                leftObstacles.Add(null);
            }

            IList<OrcaObstacle> rightObstacles = new List<OrcaObstacle>();

            for (int n = 0; n < minRight; n++)
            {
                rightObstacles.Add(null);
            }

            int leftCounter = 0;
            int rightCounter = 0;
            int i = optimalSplit;

            OrcaObstacle obstacleI1 = obstacles[i];
            OrcaObstacle obstacleI2 = obstacleI1.Next;

            for (int j = 0; j < obstacles.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                OrcaObstacle obstacleJ1 = obstacles[j];
                OrcaObstacle obstacleJ2 = obstacleJ1.Next;

                int j1LeftOfI = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, obstacleJ1.Point);
                int j2LeftOfI = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, obstacleJ2.Point);

                if (j1LeftOfI >= 0 && j2LeftOfI >= 0)
                {
                    leftObstacles[leftCounter++] = obstacles[j];
                    //leftObstacles.Add(obstacles[j]);
                    //leftCounter++;
                }
                else if (j1LeftOfI <= 0 && j2LeftOfI <= 0)
                {
                    rightObstacles[rightCounter++] = obstacles[j];
                    //rightObstacles.Add(obstacles[j]);
                    //rightCounter++;
                }
                else
                {
                    /* Split obstalce j */
                    float t = (float)MathUtil.Det(obstacleI2.Point - obstacleI1.Point, obstacleJ1.Point - obstacleI1.Point) / MathUtil.Det(obstacleI2.Point - obstacleI1.Point, obstacleJ1.Point - obstacleJ2.Point);

                    UVector2 splitPoint = obstacleJ1.Point + (obstacleJ2.Point - obstacleJ1.Point) * t;

                    OrcaObstacle newObstacle = new OrcaObstacle();
                    newObstacle.Point = new UVector2(splitPoint.x, splitPoint.y);
                    newObstacle.Previous = obstacleJ1;
                    newObstacle.Next = obstacleJ2;
                    newObstacle.Convex = UVector2.Angle(obstacleJ1.Point - splitPoint, obstacleJ2.Point - splitPoint) < 175;
                    newObstacle.Direction = obstacleJ1.Direction;
                    newObstacle.Status = status;
                    newObstacle.Id = RvoSimulator.Instance.Obstacles.Count;

                    RvoSimulator.Instance.Obstacles.Add(newObstacle);

                    obstacleJ1.Next = newObstacle;
                    obstacleJ2.Previous = newObstacle;

                    if (j1LeftOfI > 0.0f)
                    {
                        leftObstacles[leftCounter++] = obstacleJ1;
                        rightObstacles[rightCounter++] = newObstacle;

                        //leftObstacles.Add(obstacleJ1);
                        //leftCounter++;
                    }
                    else
                    {
                        rightObstacles[rightCounter++] = obstacleJ1;
                        leftObstacles[leftCounter++] = newObstacle;

                        //rightObstacles.Add(obstacleJ1);
                        //rightCounter++;
                    }
                }
            }

            node.Obstacle = obstacleI1;
            node.Left = BuildObstacleTreeRecursive(leftObstacles);
            node.Right = BuildObstacleTreeRecursive(rightObstacles);

            return node;
        }
    }
}