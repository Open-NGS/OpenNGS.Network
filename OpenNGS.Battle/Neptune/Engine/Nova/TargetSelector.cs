using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.GameData;

namespace Neptune
{
    public interface ITargetSelector
    {
        TargetType Type { get; }
        float Select(BattleActor role);
    }

    class RandomSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Random; }
        }

        public float Select(BattleActor role)
        {
            return Util.Random.Rand();
        }
    }

    class WeakestSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Weakest; }
        }

        public float Select(BattleActor role)
        {
            return -role.CurrentHPRatio;
        }
    }
    class MaxHPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MaxHP; }
        }

        public float Select(BattleActor role)
        {
            return role.HP;
        }
    }

    class NearestSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Nearest; }
        }

        private BattleActor Caster;

        public NearestSelector(BattleActor caster)
        {
            this.Caster = caster;
        }

        public float Select(BattleActor role)
        {
            return -this.Caster.Distance(role, EngineConst.EnableRadiusInDistance);
        }
    }
    class FarthestSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Farthest; }
        }
        private BattleActor Caster;
        public FarthestSelector(BattleActor caster)
        {
            this.Caster = caster;
        }

        public float Select(BattleActor role)
        {
            return this.Caster.Distance(role, EngineConst.EnableRadiusInDistance);
        }
    }

    class MaxMPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MaxMP; }
        }

        public float Select(BattleActor role)
        {
            return role.MP % 1000;
        }
    }

    class MinMPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MinMP; }
        }

        public float Select(BattleActor role)
        {
            return -role.MP % 1000;
        }
    }

    class MaxIntelligenceSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MaxIntelligence; }
        }

        public float Select(BattleActor role)
        {
            return role.Attributes.Intelligence;
        }
    }

    class MinHPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MinHP; }
        }

        public float Select(BattleActor role)
        {
            return -role.HP;
        }
    }

    class MaxADSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MaxAttackDamage; }
        }

        public float Select(BattleActor role)
        {
            return role.Attributes.AttackDamage;
        }
    }

}