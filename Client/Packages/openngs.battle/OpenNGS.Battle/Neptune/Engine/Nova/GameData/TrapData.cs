using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Neptune.GameData
{
    public class TrapData
    {
        public int TrapId { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public int DuringTime { get; set; }

        public int StartDelay { get; set; }

        public AreaShape ShapeType { get; set; }

        public int ShapeParam1 { get; set; }

        public int ShapeParam2 { get; set; }

        public AreaShape IgnoreShapeType { get; set; }

        public int IgnoreShapeParam1 { get; set; }

        public int IgnoreShapeParam2 { get; set; }

        public int ShapeChangeRatio { get; set; }

        public int ShapeChangeTime { get; set; }

        public int IgnoreShapeChangeRatio { get; set; }

        public int IgnoreShapeChangeTime { get; set; }

        public int FollowType { get; set; }

        public string EffectName { get; set; }

        public int MaxEffectCount { get; set; }

        public int EffectZOrder { get; set; }

        public int Speed { get; set; }

        public int OffsetAngle { get; set; }

        public int DirectionX { get; set; }

        public int DirectionY { get; set; }

        public int StartAction { get; set; }

        public int EnterAction { get; set; }

        public int TriggerAction { get; set; }

        public int TriggerInterval { get; set; }

        public int LeaveAction { get; set; }

        public int EndAction { get; set; }
        public int ManualAction { get; set; }

        public string EndEffect { get; set; }

        public bool InterruptEnd { get; set; }

        public bool CasterDeadEnd { get; set; }
        public int LimitTriggerType { get; set; }
        public int LimitTimes { get; set; }
        public List<int> RemoveAbilitys { get; set; }
        public string TriggerSound { get; set; }
        public string StartSound { get; set; }
        public string EndSound { get; set; }

        public TrapTriggerType TriggerType { get; set; }
        public bool IsShowIndicator { get; set; }
        public RoleType RoleType { get; set; }
        public RelativeSide AffectSide { get; set; }

        public EffectPointType EffectPointType { get; set; }

        public bool CloseEndAction { get; set; }

        public bool SelfTrigger { get; set; }

        public bool UseTrapRange { get; set; }
        public RelativeSide ShowIndicatorSide { get; set; }

        public int OrcaObstacleRadius { get; set; }

        public TargetType TargetType { get; set; }

        public TriggerType EnterTriggerType { get; set; }
        public int TriggerParam { get; set; }
        public int TriggerParam1 { get; set; }

        public List<int> PosOffset { get; set; }

        public int EffectAction { get; set; }

        public int EffectInterval { get; set; }
        public bool StartDontSetOrientation { get; set; }
        public TrapData Clone()
        {
            TrapData clone = this.MemberwiseClone() as TrapData;
            return clone;
        }
    }
}
