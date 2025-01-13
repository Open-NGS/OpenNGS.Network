using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;

namespace Neptune
{
    public interface ITargetSelector
    {
        TargetType Type { get; }
        float Select(Actor role);
    }

    class RandomSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Random; }
        }

        public float Select(Actor role)
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

        public float Select(Actor role)
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

        public float Select(Actor role)
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

        private Actor Caster;

        public NearestSelector(Actor caster)
        {
            this.Caster = caster;
        }

        public float Select(Actor role)
        {
            return -this.Caster.Distance(role, NeptuneConst.EnableRadiusInDistance);
        }
    }
    class FarthestSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.Farthest; }
        }
        private Actor Caster;
        public FarthestSelector(Actor caster)
        {
            this.Caster = caster;
        }

        public float Select(Actor role)
        {
            return this.Caster.Distance(role, NeptuneConst.EnableRadiusInDistance);
        }
    }

    class MaxMPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MaxMP; }
        }

        public float Select(Actor role)
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

        public float Select(Actor role)
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

        public float Select(Actor role)
        {
            return role.AttributeFinalValue.Intelligence;
        }
    }

    class MinHPSelector : ITargetSelector
    {
        public TargetType Type
        {
            get { return TargetType.MinHP; }
        }

        public float Select(Actor role)
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

        public float Select(Actor role)
        {
            return role.AttributeFinalValue.AttackDamage;
        }
    }

}