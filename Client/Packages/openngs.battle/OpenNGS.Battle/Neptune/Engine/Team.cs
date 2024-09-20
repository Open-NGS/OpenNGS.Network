using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;


namespace Neptune
{

    public class Team
    {
        public int ID;
        public TeamSide Side;

        // 队伍内玩家列表
        public List<Player> Players = new List<Player>();

    }

}