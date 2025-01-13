using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;
using UnityEngine;

namespace Neptune
{

    /// <summary>
    /// RuleManager interface, provide basic game rule for BattleEngine
    /// </summary>
    public interface IRuleManager
    {
        void Update(float dt);
        void Reset();
        void Clear();
        void OnBattleEnd(bool immediately = false);

        double LimitTime { get; set; }

        void OnRoleDeath(Actor role, Actor attacker);
        void OnRoleHit(Actor role);

        Team GetPlayerTeam(TeamSide side);
        Player GetCurrentPlayer();
        UVector2 ObstacleWallHit(UVector2 curPos, UVector2 nextPos);
        void SendMsgFrameOp();

        bool IsArenaMode { get; }
    }
}