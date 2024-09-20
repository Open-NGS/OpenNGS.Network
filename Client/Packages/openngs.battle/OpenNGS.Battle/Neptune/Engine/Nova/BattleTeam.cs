

using Neptune.GameData;
using System.Collections.Generic;

public class BattleTeam
{
    public int ID;
    public TeamSide Side;

    // 队伍内玩家列表
    public List<BattlePlayer> Players = new List<BattlePlayer>();

}
