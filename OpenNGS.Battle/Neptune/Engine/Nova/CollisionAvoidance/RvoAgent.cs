using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Neptune.GameData;
using Neptune;

public class RvoAgent : OrcaAgent
{
    public bool Alive
    {
        get { return alive; }
    }

    public RvoContainType RvoContainType = 0;
    private UVector2 prefVelocity;
    private UVector2 newVelocity;
    private int selfRadius;
    private int maxSpeed;
    private bool alive;
    private float invTimeHorizon;
    private float invTimeHorizonObst;
    private int neighborDist;
    private int rangeSq;
    private int neighborDistObst;
    private int rangeSqObst;
    private int maxNeighborCount;

    private List<OrcaLine> orcaLines = new List<OrcaLine>();

    public RvoAgent(BattleActor ele, UVector2 pos, int radius, int maxSpeed, float timeHorizon, float timeHorizonObst, int neighborDist, int neighborDistObst, int id, RvoContainType rvoContainType) : base()
    {
        this.Owner = ele;
        this.Position = pos;
        this.LastPosition = this.Position;
        this.selfRadius = radius;
        this.maxSpeed = maxSpeed;
        this.Id = id;
        this.invTimeHorizon = UFloat.Round(1.0f / timeHorizon);
        this.invTimeHorizonObst = UFloat.Round(1.0f / timeHorizonObst);
        this.neighborDist = neighborDist;
        this.rangeSq = neighborDist * neighborDist;
        this.neighborDistObst = neighborDistObst;
        this.rangeSqObst = neighborDistObst * neighborDistObst;
        this.maxNeighborCount = 5;
        alive = true;
        this.RvoContainType = rvoContainType;
    }

    protected override void ComputeNeighbors()
    {
        RvoSimulator simulator = NeptuneBattle.Instance.Simulator as RvoSimulator;
        if (simulator != null)
        {
            if (RvoContainType == RvoContainType.Agent)
            {
                AgentNeighbors.Clear();
                ComputeAgentNeighbors(simulator.Agents);
            }
            //else
            //{
            //    AgentNeighbors.Clear();
            //    ComputeAgentNeighbors(simulator.AgentsHero);
            //}
            ObstacleNeighbors.Clear();
            ComputeObstacleNeighbors();

            //AgentNeighbors.Clear();
            //ComputeAgentNeighbors();
        }
    }

    private void ComputeAgentNeighbors(List<RvoAgent> aliveAgents)
    {
        neighborCount = 0;
        agentCount = aliveAgents.Count;

        for (int i = 0; i < agentCount; i++)
        {
            agent = aliveAgents[i];
            if (agent.alive && Owner.IsAirForce == agent.Owner.IsAirForce && Owner != agent.Owner && !agent.Owner.IsRoleType(RoleType.Building))
            {
                distVec = LastPosition - agent.LastPosition;
                dist = distVec.magnitude/* - agent.selfRadius*/;
                if (neighborDist > dist)
                {
                    if (neighborCount < maxNeighborCount)
                    {
                        AgentNeighbors.Add(agent);
                        neighborCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
                
                
        }
    }

    private void ComputeObstacleNeighbors()
    {
        if (this.Owner.IsAirForce)
            return;

        RvoSimulator.Instance._2DTreeStatic.ComputeObstacleNeighbors(this, rangeSqObst);
        RvoSimulator.Instance._2DTreeDynamic.ComputeObstacleNeighbors(this, rangeSqObst);
    }

    int neighborCount;
    int agentCount;
    float dist;
    RvoAgent agent;
    private UVector2 distVec;
    private void ComputeAgentNeighbors()
    {
        if (this.Owner.IsAirForce)
            return;

        if (RvoContainType == RvoContainType.AgentHero)
            return;

        RvoSimulator.Instance._2DTreeDynamic.ComputeAgentNeighbors(this, ref rangeSq, (int)RvoContainType);
    }


    protected override void ComputeNewVelocity(float dt)
    {

        orcaLines.Clear();
        /* Create obstacle ORCA lines */
        int obstNeighborCount = ObstacleNeighbors.Count;
#if BATTLE_LOG

        if (CanOutputBattlelog())
            NeptuneBattle.log("ComputeNewVelocity:{0}, Count:{1} ", newVelocity, obstNeighborCount);
#endif
        for (int i = 0; i < obstNeighborCount; i++)
        {
            OrcaObstacle obstacle1 = ObstacleNeighbors[i];
            OrcaObstacle obstacle2 = obstacle1.Next;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity:OrcaObstacle[{0}] ID:({1}-{2}) Point:{3} NextPoint:{4}", i, obstacle1.Id, obstacle2.Id, obstacle1.Point, obstacle1.Next.Point);
#endif

            UVector2 relativePosition1 = obstacle1.Point - LastPosition;
            UVector2 relativePosition2 = obstacle2.Point - LastPosition;


            bool alreadyCovered = false;
            int orcaLineCount = orcaLines.Count;
            for (int j = 0; j < orcaLineCount; j++)
            {
                OrcaLine orcaLine = orcaLines[j];
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("ComputeNewVelocity CheckCovered:{0},{1}", MathUtil.Det((Vector2)(relativePosition1 * invTimeHorizonObst - orcaLine.Point), orcaLine.Direction) - invTimeHorizonObst * selfRadius,
                       MathUtil.Det((Vector2)(relativePosition2 * invTimeHorizonObst - orcaLine.Point), orcaLine.Direction) - invTimeHorizonObst * selfRadius);
#endif

                if (MathUtil.Det((Vector2)(relativePosition1 * invTimeHorizonObst - orcaLine.Point), orcaLine.Direction) - invTimeHorizonObst * selfRadius >= EngineConst.RVO_EPSILON
                    && MathUtil.Det((Vector2)(relativePosition2 * invTimeHorizonObst - orcaLine.Point), orcaLine.Direction) - invTimeHorizonObst * selfRadius >= EngineConst.RVO_EPSILON)
                {
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("ComputeNewVelocity alreadyCovered:{0},{1:f6},{2},{3},{4}", relativePosition1, invTimeHorizonObst, orcaLine.Point, orcaLine.Direction.ToString(6), selfRadius);
#endif
                    alreadyCovered = true;
                    break;
                }
            }

            if (alreadyCovered)
            {
                continue;
            }

            #region Check for collisions
            /* Not yet covered. Check for collisions */
            int distSq1 = relativePosition1.sqrMagnitude;
            int distSq2 = relativePosition2.sqrMagnitude;

            int radiusSq = selfRadius * selfRadius;

            UVector2 obstacleVector = obstacle2.Point - obstacle1.Point;
            float s = UFloat.Round(MathUtil.Dot(-relativePosition1, obstacleVector) / obstacleVector.sqrMagnitude);
            int distSqLine = (-relativePosition1 - obstacleVector * s).sqrMagnitude;

            OrcaLine line;
            line.Type = OrcaLineType.OBSTACLE;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity orcaLines1:{0:f6},{1},{2},{3},{4},{5}", s, distSq1, distSq2, distSqLine, radiusSq, obstacle1.Convex);
#endif
            if (s < .0f && distSq1 <= radiusSq)
            {
                /* Collision with left vertex. ignore if non-convex. */
                if (obstacle1.Convex)
                {
                    line.Point = EngineConst.Vector2Zero;
                    line.Direction = new Vector2(-relativePosition1.y, relativePosition1.x).normalized;
                    UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("orcaLines.Add1 A:{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                }
                else
                {
                    line.Point = EngineConst.Vector2Zero;
                    line.Direction = -obstacle1.Direction;
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("orcaLines.Add1 B:{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                }
                orcaLines.Add(line);
                continue;
            }
            else if (s > 1.0f && distSq2 <= radiusSq)
            {
                /* Collision with right vertex. ignore if non-convex or if it will be taken care of by neighbor obstacle */
                if (obstacle2.Convex)
                {
                    if (MathUtil.Det((Vector2)relativePosition2, obstacle2.Direction) >= .0f)
                    {
                        line.Point = EngineConst.Vector2Zero;
                        line.Direction = new Vector2(-relativePosition2.y, relativePosition2.x).normalized;
                        UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                        if (CanOutputBattlelog())
                            NeptuneBattle.log("orcaLines.Add2:{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                        orcaLines.Add(line);
                    }
                }
                else
                {
                    line.Point = EngineConst.Vector2Zero;
                    line.Direction = -obstacle1.Direction;
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("orcaLines.Add2 B:{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                    orcaLines.Add(line);
                }

                continue;
            }
            else if (s >= .0f && s <= 1.0f && distSqLine <= radiusSq)
            {
                /* Collision with obstacle segment */
                line.Point = EngineConst.Vector2Zero;
                line.Direction = -obstacle1.Direction;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("orcaLines.Add3:{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                orcaLines.Add(line);

                continue;
            }
            #endregion

            #region No Collision
            /* No Collision. 
             * Compute legs. Both legs can come from a single vertex.
             * Legs extend cut-off line when non-convex vertex.
             */

            Vector2 leftLegDirection, rightLegDirection;
            if (s < .0f && distSqLine <= radiusSq)
            {
                /*
                 *  Obstacle viewd obliquely so that left vertex defines velocity obstacle.
                 */

                if (!obstacle1.Convex)
                {
                    /* Ignore obstacle. */
                    continue;
                }

                obstacle2 = obstacle1;

                float leg1 = UFloat.Round((float)Math.Sqrt(distSq1 - radiusSq));
                float legx = UFloat.Round(relativePosition1.x * leg1);
                float legy = UFloat.Round(relativePosition1.y * leg1);
                leftLegDirection = new Vector2(UFloat.Round(legx - relativePosition1.y * selfRadius), UFloat.Round(relativePosition1.x * selfRadius + legy)) / distSq1;
                rightLegDirection = new Vector2(UFloat.Round(legx + relativePosition1.y * selfRadius), UFloat.Round(-relativePosition1.x * selfRadius + legy)) / distSq1;
                UFloat.Round(ref leftLegDirection);
                UFloat.Round(ref rightLegDirection);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("ComputeNewVelocity LegDirection 0 :{0},{1},{2:f6},{3:f6},{4:f6},{5},{6},{7},{8}", distSq1, radiusSq, leg1, legx, legy, relativePosition1, selfRadius, leftLegDirection.ToString(6), rightLegDirection.ToString(6));
#endif
            }
            else if (s > 1.0f && distSqLine <= radiusSq)
            {
                /*
                 * Obstacle viewed obliquely so that right vertex defines velocity obstacle.
                 */

                if (!obstacle2.Convex)
                {
                    /* Ignore obstacle. */
                    continue;
                }

                obstacle1 = obstacle2;

                float leg2 = UFloat.Round((float)Math.Sqrt(distSq2 - radiusSq));
                float legx = UFloat.Round(relativePosition2.x * leg2);
                float legy = UFloat.Round(relativePosition2.y * leg2);
                leftLegDirection = new Vector2(UFloat.Round(legx - relativePosition2.y * selfRadius), UFloat.Round(relativePosition2.x * selfRadius + legy)) / distSq2;
                rightLegDirection = new Vector2(UFloat.Round(legx + relativePosition2.y * selfRadius), UFloat.Round(-relativePosition2.x * selfRadius + legy)) / distSq2;

                UFloat.Round(ref leftLegDirection);
                UFloat.Round(ref rightLegDirection);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("ComputeNewVelocity LegDirection 1 :{0},{1},{2:f6},{3:f6},{4:f6},{5},{6},{7},{8}", distSq2, radiusSq, leg2, legx, legy, relativePosition2, selfRadius, leftLegDirection.ToString(6), rightLegDirection.ToString(6));
#endif
            }
            else
            {
                /* Unsal situation */
                if (obstacle1.Convex && distSq1 > radiusSq)
                {
                    float leg1 = UFloat.Round((float)Math.Sqrt(distSq1 - radiusSq));
                    float legx = UFloat.Round(relativePosition1.x * leg1);
                    float legy = UFloat.Round(relativePosition2.y * leg1);
                    leftLegDirection = new Vector2(UFloat.Round(legx - relativePosition1.y * selfRadius), UFloat.Round(relativePosition1.x * selfRadius + legy)) / distSq1;
                    UFloat.Round(ref leftLegDirection);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("ComputeNewVelocity leftLegDirection 0 :{0},{1},{2:f6},{3:f6},{4:f6},{5},{6},{7}", distSq2, radiusSq, leg1, legx, legy, relativePosition1, selfRadius, leftLegDirection.ToString(6));
#endif
                }
                else
                {
                    /* Left vertex non-convex; left leg extends cut-off line. */
                    leftLegDirection = -obstacle1.Direction;
                }

                if (obstacle2.Convex && distSq2 > radiusSq)
                {
                    float leg2 = UFloat.Round((float)Math.Sqrt(distSq2 - radiusSq));
                    float legx = UFloat.Round(relativePosition2.x * leg2);
                    float legy = UFloat.Round(relativePosition2.y * leg2);
                    rightLegDirection = new Vector2(UFloat.Round(legx + relativePosition2.y * selfRadius), UFloat.Round(-relativePosition2.x * selfRadius + legy)) / distSq2;
                    UFloat.Round(ref rightLegDirection);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("ComputeNewVelocity rightLegDirection 0 :{0},{1},{2:f6},{3:f6},{4:f6},{5},{6},{7},{8}", distSq2, radiusSq, leg2, legx, legy, relativePosition2, selfRadius, leftLegDirection.ToString(6), rightLegDirection.ToString(6));
#endif
                }
                else
                {
                    /* Right vertex non-convex; right leg extends cut-off line */
                    rightLegDirection = obstacle1.Direction;
                }
            }
            #endregion

            /* 
             * Legs can never point into neighbor edge when convex vertex,take cut-off of neighboring edge instead.
             * If velocity projected on "foreign" leg, no constraint is added
             */

            OrcaObstacle leftNeighbor = obstacle1.Previous;

            bool isLeftLegForeign = false;
            bool isRightLegForeign = false;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity obstacle 1:{0},{1},{2},{3},{4}", obstacle1.Convex, obstacle2.Direction.ToString(6), leftNeighbor.Direction.ToString(6), leftLegDirection.ToString(6), rightLegDirection.ToString(6));
#endif
            if (obstacle1.Convex && MathUtil.Det(leftLegDirection, -leftNeighbor.Direction) >= .0f)
            {
                /* Left leg points into obstacle */
                leftLegDirection = -leftNeighbor.Direction;
                isLeftLegForeign = true;
            }
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity obstacle 2:{0},{1},{2}", obstacle2.Convex, obstacle2.Direction.ToString(6), rightLegDirection.ToString(6));
#endif
            if (obstacle2.Convex && MathUtil.Det(rightLegDirection, obstacle2.Direction) <= .0f)
            {
                /* Right leg points into obstacle */
                rightLegDirection = obstacle2.Direction;
                isRightLegForeign = true;
            }

            /* Compute cut-off centers */
            Vector2 leftCutoff = (obstacle1.Point - LastPosition) * invTimeHorizonObst;
            Vector2 rightCutoff = (obstacle2.Point - LastPosition) * invTimeHorizonObst;
            Vector2 cutOffVector = rightCutoff - leftCutoff;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity leftCutoff:{0},rightCutoff:{1}, cutOffVector:{2} LastPosition:{3} LeftLegDir:{4} RightLegDir:{5}", leftCutoff.ToString(6), rightCutoff.ToString(6), cutOffVector.ToString(6), LastPosition, leftLegDirection.ToString(6), rightLegDirection.ToString(6));
#endif
            /* Project current velocity on velocity obstacle */

            /* Check if current velocity is projected on cutoff circles */
            float t = UFloat.Round(obstacle1 == obstacle2 ? .5f : MathUtil.Dot((LastVelocity - leftCutoff), cutOffVector) / cutOffVector.sqrMagnitude);
            float tLeft = MathUtil.Dot(LastVelocity - leftCutoff, leftLegDirection);
            float tRight = MathUtil.Dot(LastVelocity - rightCutoff, rightLegDirection);

            if ((t < .0f - EngineConst.RVO_Fault_Tolerance && tLeft < .0f) || (obstacle1 == obstacle2 && tLeft < .0f && tRight < .0f))
            {
                /* Project on left cut-off circle */
                Vector2 unitW = (LastVelocity - leftCutoff).normalized;
                UFloat.Round(ref unitW);
                line.Direction = new Vector2(unitW.y, -unitW.x);
                line.Point = leftCutoff + selfRadius * invTimeHorizonObst * unitW;
                orcaLines.Add(line);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("orcaLines.Add4:{0},{1}, {2:f6},{3:f6},{4:f6}", line.Direction.ToString(6), line.Point, t, tLeft, tRight);
#endif
                continue;
            }

            else if (t > 1.0f + EngineConst.RVO_Fault_Tolerance && tRight < .0f)
            {
                /* Project on right cut-off circle */
                Vector2 unitW = (LastVelocity - rightCutoff).normalized;
                UFloat.Round(ref unitW);
                line.Direction = new Vector2(unitW.y, -unitW.x);
                line.Point = rightCutoff + selfRadius * invTimeHorizonObst * unitW;
                orcaLines.Add(line);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("orcaLines.Add5:{0},{1}, {2:f6},{3:f6},{4:f6}", line.Direction.ToString(6), line.Point, t, tLeft, tRight);
#endif
                continue;
            }

            /*
             * Project on left leg, right leg, or cut-off line, whichever is closest to velocity
             */

            float distSqCutoff = (t < .0f || t > 1.0f || obstacle1 == obstacle2)
                ? float.PositiveInfinity
                : UFloat.Round((LastVelocity - (leftCutoff + t * cutOffVector)).sqrMagnitude);
            float distSqLeft = tLeft < .0f
                ? float.PositiveInfinity
                : UFloat.Round((LastVelocity - (leftCutoff + tLeft * leftLegDirection)).sqrMagnitude);
            float distSqRight = tRight < .0f
                ? float.PositiveInfinity
                : UFloat.Round((LastVelocity - (rightCutoff + tRight * rightLegDirection)).sqrMagnitude);

#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("ComputeNewVelocity dist :{0:f6},{1:f6},{2:f6},{3:f6},{4:f6},{5:f6}", distSqCutoff, distSqLeft, distSqRight, t, tLeft, tRight);
#endif

            if (distSqCutoff <= distSqLeft && distSqCutoff <= distSqRight)
            {
                /* Project on cut-off line */
                //line.Direction = -obstacle1.Direction;
                //line.Point = leftCutoff + selfRadius * invTimeHorizonObst * new Vector2(-line.Direction.y, line.Direction.x);
                //orcaLines.Add(line);

                if (tLeft > tRight)
                {
                    line.Direction = leftLegDirection;
                    line.Point = leftCutoff + selfRadius * invTimeHorizonObst * new Vector2(-line.Direction.y, line.Direction.x);
                    orcaLines.Add(line);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("orcaLines.Add6:{0},{1},{2:f6},{3},{4:f6},{5:f6}", leftCutoff.ToString(6), selfRadius, invTimeHorizonObst, line.Direction.ToString(6), tLeft, tRight);
#endif
                }
                else
                {
                    line.Direction = -rightLegDirection;
                    line.Point = rightCutoff + selfRadius * invTimeHorizonObst * new Vector2(-line.Direction.y, line.Direction.x);
                    orcaLines.Add(line);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("orcaLines.Add7:{0},{1},{2:f6},{3},{4:f6},{5:f6}", rightCutoff.ToString(6), selfRadius, invTimeHorizonObst, line.Direction.ToString(6), tLeft, tRight);
#endif
                }

                continue;
            }

            if (distSqLeft <= distSqRight)
            {
                /* Project on left leg */
                if (isLeftLegForeign)
                {
                    continue;
                }

                line.Direction = leftLegDirection;
                line.Point = leftCutoff + selfRadius * invTimeHorizonObst * new Vector2(-line.Direction.y, line.Direction.x);
                orcaLines.Add(line);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("orcaLines.Add8 {0},{1},{2:f6},{3},{4:f6},{5:f6}", leftCutoff.ToString(6), selfRadius, invTimeHorizonObst, line.Direction.ToString(6), distSqLeft, distSqRight);
#endif
            }
            else
            {
                /* Project on right leg */
                if (isRightLegForeign)
                {
                    continue;
                }

                line.Direction = -rightLegDirection;
                line.Point = rightCutoff +
                             selfRadius * invTimeHorizonObst * new Vector2(-line.Direction.y, line.Direction.x);
                orcaLines.Add(line);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("orcaLines.Add9:{0},{1},{2:f6},{3},{4:f6},{5:f6}", rightCutoff.ToString(6), selfRadius, invTimeHorizonObst, line.Direction.ToString(6), distSqLeft, distSqRight);
#endif
            }
        }

        int numObstLines = orcaLines.Count;

        /* Create agent ORCA lines */
        int neighborCount = AgentNeighbors.Count;
        for (int i = 0; i < neighborCount; i++)
        {
            RvoAgent agent = AgentNeighbors[i] as RvoAgent;
            if (agent != null)
            {
                float ratio = UFloat.Round(Owner.Data.Weight / (Owner.Data.Weight + agent.Owner.Data.Weight) * 2);

                UVector2 relativePosition = agent.LastPosition - LastPosition;
                Vector2 relativeVelocity = LastVelocity - agent.LastVelocity;

                int distSq = relativePosition.sqrMagnitude;
                int combineRadius = selfRadius + agent.selfRadius;
                int combineRadiusSq = combineRadius * combineRadius;

                OrcaLine line;
                line.Type = OrcaLineType.AGENT;
                Vector2 u;

                if (distSq > combineRadiusSq)
                {
                    /* No Collision */
                    /* Vector from cutoff center to relative velocity */
                    Vector2 w = relativeVelocity - (Vector2)(relativePosition * invTimeHorizon);
                    float wLengthSqr = UFloat.Round(w.sqrMagnitude);
                    float dotProduct1 = MathUtil.Dot(w, relativePosition);

                    if (dotProduct1 < .0f && MathUtil.Sqr(dotProduct1) > combineRadiusSq * wLengthSqr)
                    {
                        /* project on cut off circle */
                        Vector2 unitW = w.normalized;
                        UFloat.Round(ref unitW);
                        line.Direction = new Vector2(unitW.y, -unitW.x);
                        UFloat.Round(ref line.Direction);
                        u = UFloat.Round(combineRadius * invTimeHorizon - w.magnitude) * unitW;
#if BATTLE_LOG
                        if (CanOutputBattlelog())
                            NeptuneBattle.log("ComputeNewVelocity A:{0},{1},{2},{3:f6},{4},{5}", line.Direction.ToString(6), relativePosition, u.ToString(6), invTimeHorizon, combineRadius, unitW.ToString(6));
#endif
                    }
                    else
                    {
                        /* Project on legs */
                        int leg = UFloat.RoundToInt(MathUtil.Sqrt(distSq - combineRadiusSq));
                        if (MathUtil.Det((Vector2)relativePosition, w) > .0f)
                        {
                            /*Project on left leg*/
                            line.Direction =
                                new Vector2(UFloat.RoundToInt(relativePosition.x * leg * ratio) - relativePosition.y * combineRadius,
                                    relativePosition.x * combineRadius + UFloat.RoundToInt(relativePosition.y * leg * ratio)) / distSq;
                            UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                            if (CanOutputBattlelog())
                                NeptuneBattle.log("ComputeNewVelocity left:{0},{1},{2},{3:f6},{4},{5}", line.Direction.ToString(6), relativePosition, leg, ratio, combineRadius, distSq);
#endif
                        }
                        else
                        {
                            /*Project on right leg*/
                            line.Direction =
                                -new Vector2(UFloat.RoundToInt(relativePosition.x * leg * ratio) + relativePosition.y * combineRadius,
                                    -relativePosition.x * combineRadius + UFloat.RoundToInt(relativePosition.y * leg * ratio)) / distSq;
                            UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                            if (CanOutputBattlelog())
                                NeptuneBattle.log("ComputeNewVelocity rigt:{0},{1},{2},{3:f6},{4},{5}", line.Direction.ToString(6), relativePosition, leg, ratio, combineRadius, distSq);
#endif
                        }

                        float dotProduct2 = MathUtil.Dot(relativeVelocity, line.Direction);
                        u = dotProduct2 * line.Direction - relativeVelocity;
                        UFloat.Round(ref u);
#if BATTLE_LOG
                        if (CanOutputBattlelog())
                            NeptuneBattle.log("ComputeNewVelocity else:{0},{1:f6},{2}", line.Direction.ToString(6), dotProduct2, u.ToString(6));
#endif
                    }
                }
                else
                {
                    /* Collision. Project on cut-off circle of timeStep */
                    //float invDT = UFloat.Round(1.0f / dt);
                    float invDT = 4;
                    Vector2 w = relativeVelocity - (Vector2)(relativePosition * invDT);
                    Vector2 unitW = w.normalized;
                    UFloat.Round(ref unitW);
                    line.Direction = new Vector2(unitW.y, -unitW.x) * ratio;
                    u = UFloat.Round(UFloat.Round(combineRadius * invDT) - w.magnitude) * unitW;
                    UFloat.Round(ref u);
                    UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("ComputeNewVelocity else:{0},{1},{2:f6},{3},{4}", line.Direction.ToString(6), unitW.ToString(6), ratio, combineRadius, u.ToString(6));
#endif
                }

                line.Point = LastVelocity + u * .5f;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("projLines :{0},{1},{2}", line.Point, line.Direction.ToString(6), u.ToString(6));
#endif
                orcaLines.Add(line);
            }
        }

        //
        int lineFail = LinearProgram2(orcaLines, maxSpeed, prefVelocity, false, ref newVelocity);

        if (lineFail < orcaLines.Count)
        {
            /* !!! numObstLines */
            LinearProgram3(orcaLines, numObstLines, lineFail, maxSpeed, ref newVelocity);
        }

        //newVelocity = newVelocity.normalized * maxSpeed;

        if (obstNeighborCount > 0 && EngineConst.RVO_ForceSpeed && this.RvoContainType == RvoContainType.AgentHero)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
            //LinearProgram4(ObstacleNeighbors, maxSpeed, selfRadius, LastPosition, dt, prefVelocity, ref newVelocity);
        }

#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log("ComputeNewVelocity End:{0} ", newVelocity);
#endif
    }

    public override void Step(float dt)
    {
        ComputeNeighbors();
        ComputeNewVelocity(dt);
    }

    public override void Update(float dt)
    {
        Velocity = newVelocity;
        Position = LastPosition + Velocity * dt;
#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log("Update:Position:{0} Velocity:{1} Delta:{2}", this.Position, Velocity, Velocity * dt);
#endif
    }

    public override UVector2 GetVelocity()
    {
        newVelocity.Round();
        return newVelocity;
    }

    public override UVector2 GetPosition()
    {
        Position.Round();
        return Position;
    }

    public override UVector2 GetLastPosition()
    {
        return LastPosition;
    }

    public override int GetRadius()
    {
        return selfRadius;
    }

    public override void SetPosition(UVector2 pos)
    {
        this.Position = pos;
        //this.LastPosition = pos;
#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log("SetPosition:{0}", this.Position);
#endif
    }

    public override int GetAgentNeighborDist()
    {
        return neighborDist;
    }

    public override int GetObstacleNeighborDist()
    {
        return neighborDistObst;
    }

    public override List<OrcaLine> GetOrcaLines()
    {
        return orcaLines;
    }

    public override void AgentHasDead()
    {
        alive = false;
    }

    public override void SetPreferredVelocity(UVector2 velocity)
    {
        prefVelocity = velocity;
    }

    public override void SetPreferredMaxSpeed(int speed)
    {
        /*maxSpeed = UFloat.RoundToInt(forceSpeed ? speed : Math.Min(speed * RvoSimulator.Instance.MinSpeedFactor, RvoSimulator.Instance.MaxSpeedThreshold));*/
        maxSpeed = UFloat.RoundToInt(speed);
    }
    private bool LinearProgram1(List<OrcaLine> lines, int lineNo, int radius, Vector2 optVelocity, Vector2 tempVelocity, bool directionOpt, ref UVector2 result)
    {
        OrcaLine line = lines[lineNo];
        int dotProduct = UFloat.RoundToInt(MathUtil.Dot(line.Point, line.Direction));
        int discriminant = UFloat.RoundToInt(MathUtil.Sqr(dotProduct) + radius * radius - line.Point.sqrMagnitude);

        if (discriminant < 0)
        {
            /*Max speed circle fully invalidates OrcaLine lineNo*/
            return false;
        }

        int sqrtDiscriminant = UFloat.RoundToInt(MathUtil.Sqrt(discriminant));
        int tLeft = -dotProduct - sqrtDiscriminant;
        int tRight = -dotProduct + sqrtDiscriminant;

        for (int i = 0; i < lineNo; i++)
        {
            OrcaLine tempLine = lines[i];
            float denominator = UFloat.Round(MathUtil.Det(line.Direction, tempLine.Direction));
            Vector2 vec = (line.Point - tempLine.Point);
            UFloat.Round(ref vec);
            float numerator = UFloat.Round(MathUtil.Det(tempLine.Direction, vec));
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("LinearProgram1 directionOpt1:{0},{1},{2},{3},{4},denominator:{5:f6} numerator:{6:f6} Vec:{7} tempLine:{8} Discr:{9},{10}", line.Point, tLeft, tRight, line.Direction.ToString(6), result, denominator, numerator, vec.ToString(6), tempLine.Direction.ToString(6), discriminant, sqrtDiscriminant);
#endif
            if (denominator == 0)
            {
                /* Lines lineNo and i are (almost parallel) */
                if (numerator <= 0)
                {
                    return false;
                }
                continue;
            }

            int t = UFloat.RoundToInt(numerator / denominator);

            if (denominator > 0)
            {
                /* OrcaLine i bounds OrcaLine lineNo on the right */
                tRight = Math.Min(tRight, t);
            }
            else
            {
                /* OrcaLine i bounds OrcaLine lineNo on the left */
                tLeft = Math.Max(tLeft, t);
            }

            if (tLeft > tRight)
            {
                return false;
            }
        }

#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log("LinearProgram1 Point:{0} tLeft:{1} tRight:{2},{3},{4},{5},{6}", line.Point, tLeft, tRight, line.Direction.ToString(6), dotProduct, sqrtDiscriminant, optVelocity.ToString(6));
#endif

        Vector2 offset;
        if (directionOpt)
        {
            /* Optimize direction */
            if (UFloat.Round(MathUtil.Dot(optVelocity, line.Direction)) > 0)
            {
                /* Take right extreme */
                offset = tRight * line.Direction;
                UFloat.Round(ref offset);
                result = line.Point + offset;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("LinearProgram1 directionOpt1:{0},{1},{2},{3},{4}", line.Point, tRight, line.Direction.ToString(6), offset.ToString(6), result);
#endif
            }
            else
            {
                /* Take left extreme */
                offset = tLeft * line.Direction;
                UFloat.Round(ref offset);
                result = line.Point + offset;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("LinearProgram1 directionOpt2:{0},{1},{2},{3},{4}", line.Point, tRight, line.Direction.ToString(6), offset.ToString(6), result);
#endif
            }
        }
        else
        {
            /* Optimize closest point */
            int t = UFloat.RoundToInt(MathUtil.Dot(line.Direction, (optVelocity - (Vector2)line.Point)));

            if (t <= tLeft)
            {
                offset = tLeft * line.Direction;
                UFloat.Round(ref offset);
                result = line.Point + offset;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("LinearProgram1 NoOpt1:{0},{1},{2},{3},{4},{5}", line.Point, tRight, line.Direction.ToString(6), offset.ToString(6), t, result);
#endif
            }
            else if (t >= tRight)
            {
                offset = tRight * line.Direction;
                UFloat.Round(ref offset);
                result = line.Point + offset;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("LinearProgram1 NoOpt2:{0},{1},{2},{3},{4},{5}", line.Point, tRight, line.Direction.ToString(6), offset.ToString(6), t, result);
#endif
            }
            else
            {
                offset = t * line.Direction;
                UFloat.Round(ref offset);
                result = line.Point + offset;
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log("LinearProgram1 NoOpt3:{0},{1},{2},{3},{4},{5}", line.Point, tRight, line.Direction.ToString(6), offset.ToString(6), t, result);
#endif

                #region Next version
                //if (EngineConst.RVO_ForceSpeed && this.RvoContainType == RvoContainType.AgentHero)
                //{
                //    if (line.Type == OrcaLineType.OBSTACLE)
                //    {
                //        bool flag = UFloat.Round(MathUtil.Dot(tempVelocity, line.Direction)) >= 0;

                //        if (flag)
                //        {
                //            result = line.Direction * radius;
                //        }
                //        else
                //        {
                //            result = -line.Direction * radius;
                //        }
                //    }
                //} 
                #endregion
            }
        }

        return true;
    }

    private int LinearProgram2(List<OrcaLine> lines, int radius, Vector2 optVelocity, bool directionOpt, ref UVector2 result, bool velocityProt = true)
    {
        if (directionOpt)
        {
            result = optVelocity * radius;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("LinearProgram2 directionOpt:{0} {1} {2}", optVelocity.ToString(6), radius, result);
#endif
        }
        else if (optVelocity.sqrMagnitude > MathUtil.Sqr(maxSpeed))
        {

            result = optVelocity.normalized * radius;
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("LinearProgram2 :{0},{1},{2}", optVelocity.sqrMagnitude, MathUtil.Sqr(maxSpeed), result);
#endif
        }
        else
        {
            result = optVelocity;
        }

        int lineCount = lines.Count;
        for (int i = 0; i < lineCount; i++)
        {
            OrcaLine line = lines[i];
            if (MathUtil.Det(line.Direction, (Vector2)(line.Point - result)) > .0f)
            {
                UVector2 tempResult = result;
                if (!LinearProgram1(lines, i, radius, optVelocity, tempResult, directionOpt, ref result))
                {
                    result = tempResult;
                    return i;
                }
            }
        }
        return lines.Count;
    }

    private float distance = 0;
    private void LinearProgram3(List<OrcaLine> lines, int numObstLines, int beginLine, int radius, ref UVector2 result)
    {
        distance = .0f;

        for (int i = beginLine; i < lines.Count; i++)
        {
            if (MathUtil.Det(lines[i].Direction, (Vector2)(lines[i].Point - result)) > distance)
            {
                /* Result dose not satisfy constraint of line i */
                List<OrcaLine> projLines = new List<OrcaLine>();
                for (int ii = 0; ii < numObstLines; ii++)
                {
                    projLines.Add(lines[ii]);
                }

                for (int j = numObstLines; j < i; j++)
                {
                    OrcaLine line;
                    line.Type = OrcaLineType.AGENT;

                    float determinant = UFloat.Round(MathUtil.Det(lines[i].Direction, lines[j].Direction));
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("LinearProgram3 Abs:{0:f6},{1}", determinant, lines[i].Direction.ToString(6));
#endif
                    if (Math.Abs(determinant) <= EngineConst.RVO_EPSILON)
                    {
                        /* Line i and line j are parallel */
                        if (MathUtil.Dot(lines[i].Direction, lines[j].Direction) > .0f)
                        {
                            /* Line i and line j point in the same direction */
                            continue;
                        }
                        else
                        {
                            /* Line i and line j point in opposite direction */
                            line.Point = (lines[i].Point + lines[j].Point) * 0.5f;
                        }
                    }
                    else
                    {
                        line.Point = lines[i].Point + MathUtil.Det(lines[j].Direction, (Vector2)(lines[i].Point - lines[j].Point)) / determinant * lines[i].Direction;
#if BATTLE_LOG
                        if (CanOutputBattlelog())
                            NeptuneBattle.log("LinearProgram3 :{0},{1},{2},{3},{4:f6},{5}", lines[i].Point, lines[j].Point, lines[j].Direction.ToString(6), lines[i].Direction.ToString(6), determinant, line.Point);
#endif
                    }
                    Vector2 dist = lines[j].Direction - lines[i].Direction;
                    UFloat.Round(ref dist);
                    line.Direction = dist.normalized;
                    UFloat.Round(ref line.Direction);
#if BATTLE_LOG
                    if (CanOutputBattlelog())
                        NeptuneBattle.log("projLines :{0},{1}", line.Point, line.Direction.ToString(6));
#endif
                    projLines.Add(line);
                }

                Vector2 tempResult = result;
                if (LinearProgram2(projLines, selfRadius, new Vector2(-lines[i].Direction.y, lines[i].Direction.x), true, ref result) < projLines.Count)
                {
                    result = tempResult;
                }

                distance = MathUtil.Det(lines[i].Direction, (Vector2)(lines[i].Point - result));
            }
        }
    }

    private void LinearProgram4(List<OrcaObstacle> obstacleNeighbors, int maxSpeed, int radius, UVector2 position, float deltaTime, Vector2 optVeloicty, ref UVector2 newVelocity)
    {
        UVector2 position1 = position;
        UVector2 position2 = position + optVeloicty * deltaTime;

        newVelocity = optVeloicty;

        int obstNeighborCount = obstacleNeighbors.Count;
        for (int i = 0; i < obstNeighborCount; i++)
        {
            OrcaObstacle obstacleI1 = obstacleNeighbors[i];
            OrcaObstacle obstacleI2 = obstacleI1.Next;

            if (LinearRecursive(obstacleI1, obstacleI2, position1, position2, maxSpeed, radius, position, deltaTime, optVeloicty, ref newVelocity, 0)) break;
        }
    }

    private bool LinearRecursive(OrcaObstacle obstacleI1, OrcaObstacle obstacleI2, UVector2 position1, UVector2 position2, int maxSpeed, int radius, UVector2 position, float deltaTime, Vector2 optVeloicty, ref UVector2 newVelocity, int index)
    {
        if (index > 1)
        {
            return true;
        }

        int p1LeftOfI12 = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, position1);
        int p2LeftOfI12 = MathUtil.LeftOf(obstacleI1.Point, obstacleI2.Point, position2);

        bool poi = p1LeftOfI12 >= 0 && p2LeftOfI12 <= 0 || p1LeftOfI12 <= 0 && p2LeftOfI12 >= 0;

#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log(string.Format(
                "LinearRecursive pre: p1 {0},   p2 {1},  oi1 {2},   oi2 {3}, p1LeftOfI12 {4},  p2LeftOfI12 {5}, twoSide: {6}, optVelocity {7}, newVelocity {8}, maxSpeed {9}",
                position1, position2, obstacleI1.Id, obstacleI2.Id, p1LeftOfI12, p2LeftOfI12, poi, optVeloicty.ToString(6), newVelocity,
                maxSpeed));
#endif

        long distSqLineP1 = DistSqLine(obstacleI1.Point, obstacleI2.Point, position1);
        long distSqLineP2 = DistSqLine(obstacleI1.Point, obstacleI2.Point, position2);


        if (poi || distSqLineP2 < radius * radius)
        {
            bool flag = MathUtil.Dot(newVelocity.normalized, obstacleI1.Direction.normalized) >= .0f;

            if (flag)
            {
                OrcaObstacle obstacleI3 = obstacleI2.Next;

                newVelocity = obstacleI1.Direction.normalized * maxSpeed;

                UVector2 position3 = position + newVelocity * deltaTime;
                int p1LeftOfI23 = MathUtil.LeftOf(obstacleI2.Point, obstacleI3.Point, position1);
                int p3LeftOfI23 = MathUtil.LeftOf(obstacleI2.Point, obstacleI3.Point, position3);

                bool twoSide = p1LeftOfI23 > 0 && p3LeftOfI23 < 0 || p1LeftOfI23 < 0 && p3LeftOfI23 > 0;
                if (twoSide)
                {
                    newVelocity = obstacleI2.Direction.normalized * maxSpeed;
                }
                UVector2 position4 = position + newVelocity * deltaTime;

                index++;

                LinearRecursive(obstacleI2, obstacleI3, position1, position4, maxSpeed, radius, position, deltaTime, optVeloicty,
                    ref newVelocity, index);


#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log(string.Format(
                        "LinearRecursive A: p1 {0},   p3 {1},  oi1 {2},   oi2 {3},   oi3 {4}, p1LeftOfI23 {5},  p3LeftOfI23 {6}, twoSide: {7} newVelocity {8}, radius {9}",
                        position1, position3, obstacleI1.Id, obstacleI2.Id, obstacleI3.Id, p1LeftOfI23, p3LeftOfI23,
                        twoSide, newVelocity, radius));
#endif
            }
            else
            {
                OrcaObstacle obstacleI3 = obstacleI1.Previous;

                newVelocity = -obstacleI1.Direction.normalized * maxSpeed;

                UVector2 position3 = position + newVelocity * deltaTime;
                int p1LeftOfI13 = MathUtil.LeftOf(obstacleI3.Point, obstacleI1.Point, position1);
                int p3LeftOfP13 = MathUtil.LeftOf(obstacleI3.Point, obstacleI1.Point, position3);

                bool twoSide = p1LeftOfI13 > 0 && p3LeftOfP13 < 0 || p1LeftOfI13 < 0 && p3LeftOfP13 > 0;
                if (twoSide)
                {
                    newVelocity = -obstacleI3.Direction.normalized * maxSpeed;
                }
                UVector2 position4 = position + newVelocity * deltaTime;

                index++;

                LinearRecursive(obstacleI3, obstacleI1, position1, position4, maxSpeed, radius, position, deltaTime, optVeloicty,
                    ref newVelocity, index);
#if BATTLE_LOG
                if (CanOutputBattlelog())
                    NeptuneBattle.log(string.Format(
                        "LinearRecursive B: p1 {0},   p3 {1},  oi1 {2},   oi2 {3},   oi3 {4}, p1LeftOfI13 {5},  p3LeftOfP13 {6},  , twoSide: {7} newVelocity {8}, radius {9}",
                        position1, position3, obstacleI1.Id, obstacleI2.Id, obstacleI3.Id, p1LeftOfI13, p3LeftOfP13,
                        twoSide, newVelocity, radius));
#endif
            }

            return true;
        }
        else
        {
            LinearProgram5(obstacleI1, obstacleI2, position1, position2, maxSpeed, radius, optVeloicty, ref newVelocity);
        }
        return false;
    }

    private void LinearProgram5(OrcaObstacle obstacleI1, OrcaObstacle obstacleI2, UVector2 position1, UVector2 position2, int maxSpeed, int radius, Vector2 optVeloicty, ref UVector2 newVelocity)
    {
        long distSqLineP1 = DistSqLine(obstacleI1.Point, obstacleI2.Point, position1);
        long distSqLineP2 = DistSqLine(obstacleI1.Point, obstacleI2.Point, position2);

#if BATTLE_LOG
        if (CanOutputBattlelog())
            NeptuneBattle.log(string.Format("LinearProgram5  pre: oi1 {0} {1},   oi2 {2} {3},  p1 {4}  distSqLineP1  {5},  p2 {6}   distSqLineP2  {7}  radius  {8}  optVeloicty  {9}", obstacleI1.Id, obstacleI1.Point, obstacleI2.Id, obstacleI2.Point, position1, distSqLineP1, position2, distSqLineP2, radius, optVeloicty));
#endif

        if (distSqLineP2 < radius * radius && optVeloicty.magnitude > 0)
        {
            float t1 = (float)MathUtil.Det(obstacleI2.Point - obstacleI1.Point, position1 - obstacleI1.Point) /
                       MathUtil.Det(obstacleI2.Point - obstacleI1.Point, position1 - position2);

            UVector2 deltaVector2 = position2 - position1;
            UVector2 splitPoint = position1 + deltaVector2 * t1;

            float t2 = radius / MathUtil.Sqrt(distSqLineP1);

            UVector2 anotherPoint = splitPoint + (position1 - splitPoint) * t2;

            UVector2 deltaVector1 = anotherPoint - position1;
            float t3 = deltaVector1.magnitude * 1.0f / deltaVector2.magnitude;
            newVelocity = optVeloicty * t3;


            long temp = DistSqLine(obstacleI1.Point, obstacleI2.Point, anotherPoint);
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log(string.Format(
                    "LinearProgram5  : p1 {0},   p2 {1},    splitPoint {6},   anotherPoint {8},  distSqAnotherPoint {15},  deltaVector1 {13},   deltaVector2 {14},   oi1 {2}  point {16},   oi2 {3}  point {17}, distSqLine {4}, sqSelfRadius {5},  t1 {7}, t2 {9}, t3 {10}, newVelocity {11}, maxSpeed {12}",
                    position1, position2, obstacleI1.Id, obstacleI2.Id, distSqLineP2, radius * radius, splitPoint, t1,
                    anotherPoint, t2, t3, newVelocity, maxSpeed, deltaVector1, deltaVector2, temp, obstacleI1.Point,
                    obstacleI2.Point));
#endif
        }
    }

    private long DistSqLine(UVector2 position1, UVector2 position2, UVector2 position)
    {
        int agentLeftOfLine = MathUtil.LeftOf(position1, position2, position);
        int x = position2.x - position1.x;
        int y = position2.y - position1.y;

        int division = (x * x + y * y);
        long distSqLine = 0;
        if (division != 0)
        {
            distSqLine = (long)agentLeftOfLine * agentLeftOfLine / division;
        }
        return distSqLine;
    }

    public void InsertAgentNeighbor(RvoAgent agent, ref int rangeSq)
    {
        if (this != agent)
        {
            int distSq = (LastPosition - agent.LastPosition).sqrMagnitude;

            if (distSq < rangeSq)
            {
                if (AgentNeighbors.Count < maxNeighborCount)
                {
                    AgentNeighbors.Add(agent);
                }
            }
        }
    }

    public void InsertObstacleNeighbor(OrcaObstacle obstacle, int rangeSq)
    {
        OrcaObstacle nextObstacle = obstacle.Next;

        int distSq = MathUtil.DistSqPointLineSegment(obstacle.Point, nextObstacle.Point, LastPosition);

        if (distSq < rangeSq)
        {
#if BATTLE_LOG
            if (CanOutputBattlelog())
                NeptuneBattle.log("InsertObstacleNeighbor:{0},{1},{2},{3},{4},{5}", obstacle.Id, obstacle.Point, nextObstacle.Point, Position, distSq, rangeSq);
#endif
            ObstacleNeighbors.Add(obstacle);
        }
    }

#if BATTLE_LOG
    public bool CanOutputBattlelog()
    {
        return EngineGlobal.BattleLog && (this.Owner.ID == EngineGlobal.DebugRoleId || EngineGlobal.DebugRoleId == 0) && (NeptuneBattle.Instance.doneFrameCount > EngineGlobal.MinLogicframe && NeptuneBattle.Instance.doneFrameCount < EngineGlobal.MaxLogicframe);
    }
#endif
}
