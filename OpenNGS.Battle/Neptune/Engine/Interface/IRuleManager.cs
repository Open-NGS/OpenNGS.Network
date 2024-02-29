using Neptune.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRuleManager
{
    void Update(float dt);
    void Reset();
    void Clear();
    void OnBattleEnd(bool immediately = false);

    double LimitTime { get; set; }

    void OnRoleDeath(BattleActor role, BattleActor attacker);
    void OnRoleHit(BattleActor role);

    BattleTeam GetPlayerTeam(TeamSide side);
    BattlePlayer GetCurrentPlayer();
    UVector2 ObstacleWallHit(UVector2 curPos, UVector2 nextPos);
    void SendMsgFrameOp();

    bool IsArenaMode { get; }
}