using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptune.GameData
{
    /// <summary>
    /// Ability效果属性
    /// </summary>
    public class AbilityEffects : Neptune.GameData.GameDataArray<bool>
    {
        public bool Root { get { return base.Values[(int)ControlEffect.Root]; } set { base.Values[(int)ControlEffect.Root] = value; } }
        public bool Inhibition { get { return base.Values[(int)ControlEffect.Inhibition]; } set { base.Values[(int)ControlEffect.Inhibition] = value; } }
        public bool Disable { get { return base.Values[(int)ControlEffect.Disable]; } set { base.Values[(int)ControlEffect.Disable] = value; } }
        public bool Static { get { return base.Values[(int)ControlEffect.Static]; } set { base.Values[(int)ControlEffect.Static] = value; } }
        public bool Void { get { return base.Values[(int)ControlEffect.Void]; } set { base.Values[(int)ControlEffect.Void] = value; } }
        public bool Invincible { get { return base.Values[(int)ControlEffect.Invincible]; } set { base.Values[(int)ControlEffect.Invincible] = value; } }
        public bool Unaffected { get { return base.Values[(int)ControlEffect.Unaffected]; } set { base.Values[(int)ControlEffect.Unaffected] = value; } }
        public bool Charm { get { return base.Values[(int)ControlEffect.Charm]; } set { base.Values[(int)ControlEffect.Charm] = value; } }
        public bool Immoblilize { get { return base.Values[(int)ControlEffect.Immoblilize]; } set { base.Values[(int)ControlEffect.Immoblilize] = value; } }
        public bool Directed { get { return base.Values[(int)ControlEffect.Directed]; } set { base.Values[(int)ControlEffect.Directed] = value; } }
        public bool Incurable { get { return base.Values[(int)ControlEffect.Incurable]; } set { base.Values[(int)ControlEffect.Incurable] = value; } }
        public bool Norecover { get { return base.Values[(int)ControlEffect.Norecover]; } set { base.Values[(int)ControlEffect.Norecover] = value; } }
        public bool MindChain { get { return base.Values[(int)ControlEffect.MindChain]; } set { base.Values[(int)ControlEffect.MindChain] = value; } }
        public bool MindGain { get { return base.Values[(int)ControlEffect.MindGain]; } set { base.Values[(int)ControlEffect.MindGain] = value; } }
        public bool Sleep { get { return base.Values[(int)ControlEffect.Sleep]; } set { base.Values[(int)ControlEffect.Sleep] = value; } }
        public bool Imprisonment { get { return base.Values[(int)ControlEffect.Imprisonment]; } set { base.Values[(int)ControlEffect.Imprisonment] = value; } }
        public bool Solidifying { get { return base.Values[(int)ControlEffect.Solidifying]; } set { base.Values[(int)ControlEffect.Solidifying] = value; } }
        public bool Inhuman { get { return base.Values[(int)ControlEffect.Inhuman]; } set { base.Values[(int)ControlEffect.Inhuman] = value; } }
        public bool OnlyNormalAttack { get { return base.Values[(int)ControlEffect.OnlyNormalAttack]; } set { base.Values[(int)ControlEffect.OnlyNormalAttack] = value; } }
        public bool Taunt { get { return base.Values[(int)ControlEffect.Taunt]; } set { base.Values[(int)ControlEffect.Taunt] = value; } }
        public bool Fear { get { return base.Values[(int)ControlEffect.Fear]; } set { base.Values[(int)ControlEffect.Fear] = value; } }
        public bool Rebirth { get { return base.Values[(int)ControlEffect.Rebirth]; } set { base.Values[(int)ControlEffect.Rebirth] = value; } }
        public bool Invisible { get { return base.Values[(int)ControlEffect.Invisible]; } set { base.Values[(int)ControlEffect.Invisible] = value; } }
        public bool Bare { get { return base.Values[(int)ControlEffect.Bare]; } set { base.Values[(int)ControlEffect.Bare] = value; } }
        public bool Grass { get { return base.Values[(int)ControlEffect.Grass]; } set { base.Values[(int)ControlEffect.Grass] = value; } }
        public bool Disarmed { get { return base.Values[(int)ControlEffect.Disarmed]; } set { base.Values[(int)ControlEffect.Disarmed] = value; } }
        public bool Vision { get { return base.Values[(int)ControlEffect.Vision]; } set { base.Values[(int)ControlEffect.Vision] = value; } }
        public bool OnlyAttackBuilding { get { return base.Values[(int)ControlEffect.OnlyAttackBuilding]; } set { base.Values[(int)ControlEffect.OnlyAttackBuilding] = value; } }
        public AbilityEffects()
            : base((int)ControlEffect.MAX)
        {
        }
    }
}
