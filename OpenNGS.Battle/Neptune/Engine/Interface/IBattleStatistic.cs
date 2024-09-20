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
        void OnDamage(Actor role, float damage);
        void RecordDamage(Actor selfRole, Actor attackRole, Skill attackSkill, float damage);
        void Clear();

    }
}