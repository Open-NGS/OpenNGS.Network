using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune
{

    /// <summary>
    /// Battle data statistic interface
    /// </summary>
    public interface IBattleStatistic
    {
        void OnDamage(BattleActor role, float damage);
        void RecordDamage(BattleActor selfRole, BattleActor attackRole, BattleSkill attackTalent, float damage);
        void Clear();

    }
}