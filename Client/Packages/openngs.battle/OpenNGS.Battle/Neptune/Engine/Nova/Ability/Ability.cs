using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neptune.GameData;

namespace Neptune
{
    public class Ability : AbilityBase
    {
        public BattleActor Owner;
        public BattleSkill Talent;
        public AbilityData AbilityData;
        public int ModelID;
        public int TransformIndex;
        public int ShieldRecharge;
        public float Protection;
        public float ProtectionRatio;
        private float TotalDamage;
        public int Recharge;
        public bool DontRemoveOnDeath;

        private bool isDestroy = false;

        private int EffectID;
        private int RoleFXID;

        public Dictionary<int, bool> ControlEffects = new Dictionary<int, bool>();

        private float fAbiltiyRecoveryFrqc;
        private float fAbiltiyDamageFrqc;
        private float fAbiltiyRecoveredHP;
        private float fAbiltiyDamagedHP;

        public float intervalTime = 0.0f;

        // Property
        public bool IsDestroy
        {
            get
            {
                return isDestroy;
            }
        }

        public bool HasControlEffect
        {
            get
            {
                return ControlEffects != null && ControlEffects.Count > 0;
            }
        }

        public int FromTalentID;
        public int ToTalentID;
        public int ChangeIndex;

        public int ZoomIndex;
        private float shieldAdd;
        public Ability()
        {

        }

        public void Create(AbilityData data, BattleActor owner, BattleActor caster, BattleSkill talent, int tid = -100)
        {
            ModelID = 0;
            TransformIndex = 0;
            TotalDamage = 0;
            Recharge = 0;
            ZoomIndex = 0;
            EffectID = 0;
            RoleFXID = 0;
            ChangeIndex = 0;
            FromTalentID = 0;
            ToTalentID = 0;
            ControlEffects.Clear();
            shieldAdd = 0;
            isDestroy = false;
            AbilityData = data.Clone();

            Owner = owner;
            Talent = talent;
            Caster = caster;
            if (Caster != null)
                Caster.RoleSkin.RoleSkinAbilityReplace(AbilityData);

            //this.startTime = this.AbilityData.StartTime;
            intervalTime = AbilityData.StartTime;

            fAbiltiyRecoveryFrqc = AbilityData.PopupFrequent;
            fAbiltiyDamageFrqc = AbilityData.PopupFrequent;
            fAbiltiyRecoveredHP = 0;
            fAbiltiyDamagedHP = 0;
            if (AbilityData.ActiveTalentIDs != null && AbilityData.ActiveTalentIDs.Count > 1)
            {
                FromTalentID = AbilityData.ActiveTalentIDs[0];
                ToTalentID = AbilityData.ActiveTalentIDs[1];
            }

            DontRemoveOnDeath = data.DontRemoveOnDeath;
            InitDuration();
            addControlEffects();
            initShieldValue();
            talentCDReductions();
            talentDamegeFactor();
        }

        private void addControlEffects()
        {
            if (AbilityData.ControlEffects != null)
            {
                for (int i = 0; i < AbilityData.ControlEffects.Count; i++)
                {
                    ControlEffects.Add(AbilityData.ControlEffects[i], true);
                }
            }
            if (ControlEffects.ContainsKey((int)ControlEffect.Charm))
            {
                //增加魅惑Ability时,重置目标普通攻击CD
                Owner.CommonCooldown = 0;
                Owner.BasicTalent.Duration = 0;
            }
        }

        private void initShieldValue()
        {
            Protection = AbilityData.ShieldValue;
            ProtectionRatio = AbilityData.ShieldAbsorbRatio;
            ShieldRecharge = AbilityData.ShieldRecharge;

            if (AbilityData.BaseRatio > 0)
            {
                shieldAdd = UFloat.Round(Caster != null ? AbilityData.BaseRatio * UFloat.Round(Caster.Attributes[(int)AbilityData.BaseAttribute] * EngineConst.Hundredth) : AbilityData.BaseRatio * UFloat.Round(Owner.Attributes[(int)AbilityData.BaseAttribute] * EngineConst.Hundredth));
            }
            if (Owner.Config.HPFactor > 0 && Protection > 0)
            {
                Protection = UFloat.Round(Protection * Owner.Config.HPFactor);
            }
            Protection = UFloat.Round(Protection + shieldAdd);
        }

        private void talentCDReductions()
        {
            //CD缩减
            if (AbilityData.CDReductions != null)
            {
                if (AbilityData.CDReductions.Count % 3 != 0)
                {
                    Logger.LogError("AbilityData.CDReductions error!");
                    return;
                }
                for (int i = 0; i < AbilityData.CDReductions.Count; i += 3)
                {
                    BattleSkill target_talent = Owner.GetTalentById(AbilityData.CDReductions[i]);
                    if (target_talent != null)
                    {
                        float beforeDuration = target_talent.Duration;
                        target_talent.Duration = UFloat.Round(target_talent.Duration - target_talent.Duration * (AbilityData.CDReductions[i + 2] * EngineConst.Hundredth) - AbilityData.CDReductions[i + 1] * EngineConst.Milli);
                        target_talent.Duration = target_talent.Duration < 0 ? 0 : target_talent.Duration;
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy CDReductions Skill:{1} from {2:F6}", Owner.FullName, target_talent.ToString(), beforeDuration);
#endif
                    }
                }
            }
        }

        private void talentDamegeFactor(int addOrReduce = 1)
        {
            if (AbilityData.DamageFactors != null)
            {
                if (AbilityData.DamageFactors.Count % 2 != 0)
                {
                    Logger.LogError("AbilityData.DamageFactors error!");
                    return;
                }
                for (int i = 0; i < AbilityData.DamageFactors.Count; i += 2)
                {
                    BattleSkill target_talent = Owner.GetTalentById(AbilityData.DamageFactors[i]);
                    if (target_talent != null)
                    {
                        float beforeDamageFactor = target_talent.DamageFactor;
                        target_talent.SetDamageFactor(AbilityData.DamageFactors[i + 1] * EngineConst.Hundredth * addOrReduce);
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy DamageFactors Skill:{1} from {2:F6}", Owner.FullName, target_talent.ToString(), beforeDamageFactor);
#endif
                    }
                }
            }
        }

        public void SetTransformModel()
        {
            //在同一个ability 中如果同时存在 ModelId 和TransformID 优先处理TransformID忽略ModelId （两者不能同时生效）
            if (AbilityData.TransformID != 0)
            {
                TransformIndex = Owner.EnterTransformation(AbilityData.TransformID);
            }
            else if (!string.IsNullOrEmpty(AbilityData.Model))
            {
                ModelID = Owner.SetModel(AbilityData.Model);
            }

        }

        public void ModelScale()
        {
            if (AbilityData.ModelScaleDuration > 0)
            {
                int index = 1;
                if (Owner.zoomIndex.Count > 0)
                {
                    index = Owner.zoomIndex.Keys.Max() + 1;
                }
                ZoomIndex = index;
                Owner.zoomIndex.Add(index, AbilityData.ModelScale);
                Owner.StartZoom(AbilityData.ModelScale, AbilityData.ModelScaleDuration);
            }
        }

        public void ChangeTalent()
        {
            if (FromTalentID > 0 && ToTalentID > 0)
            {
                int fromTalentID = FromTalentID;
                int toTalentID = ToTalentID;
                if (Owner.ChangeTalentStack.ContainsKey(fromTalentID))
                {
                    ChangeIndex = ++Owner.changeIndex;
                    Owner.ChangeTalentStack[fromTalentID].Add(ChangeIndex);
                }
            }
            if (AbilityData.TransformID != 0 || FromTalentID > 0 && ToTalentID > 0)
                Owner.InitChangeTalents(null, AbilityData.TransformID == 0 ? FromTalentID : 0);
        }

        public void InitDuration()
        {
            SetDuration(AbilityData.Time);
            if (Duration > 0 && !float.IsInfinity(Duration))
            {
                float factor = 0;
                if (AbilityData != null && Owner != null && Caster != null && Owner.Side != Caster.Side)
                {
                    if (AbilityData.Tough != RoleAttribute.None && Owner.Attributes[(int)AbilityData.Tough] > 0)
                        factor = Owner.Attributes[(int)AbilityData.Tough];
                }
                Duration = UFloat.Round(Duration * Math.Max(0, 1 - UFloat.Round(factor * EngineConst.Hundredth)));
            }
        }

        public override void Delete()
        {
            NObjectPool<Ability>.Delete(this);
        }

        public void UpdateAction(float dt)
        {
            fAbiltiyRecoveryFrqc -= dt;
            fAbiltiyDamageFrqc -= dt;

            if (intervalTime > 0)
            {
                intervalTime -= dt;
            }
            if (intervalTime <= 0)
            {
                intervalTime += AbilityData.IntervalTime;
                if (!Owner.AbilityEffects.Norecover && Owner.HP < Owner.Attributes.MaxHP && (AbilityData.HpIntervalIncrease > 0 || AbilityData.HpIntervalIncrease_a > 0))
                {
                    if (!Owner.AbilityEffects.Incurable)
                    {//持续回复生命
                        float temp = UFloat.Round(AbilityData.HpIntervalIncrease + Owner.Attributes.MaxHP * AbilityData.HpIntervalIncrease_a * EngineConst.Hundredth);
                        if (Owner.Attributes.SHealHp != 0.0f)
                            temp = UFloat.Round(temp * Math.Max(EngineConst.MinSHealHp, 1 + Owner.Attributes.SHealHp * EngineConst.Hundredth));
                        fAbiltiyRecoveredHP += temp;
                        Owner.SetHP(Owner.HP + temp);
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy HpIntervalIncrease {1} {2:F6} +{3:F6}", Owner.FullName, AbilityData.Name, Owner.HP, temp);
#endif
                    }
                }
                int hpd = UFloat.RoundToInt(AbilityData.HpIntervalLost + AbilityData.HpIntervalLost_a * (AbilityData.HpLostRatioBaseOnCurHP ? Owner.HP : Owner.MaxHP) * EngineConst.Hundredth);
                if (hpd > 0 && (!AbilityData.UnDeath || Owner.HP > 1))
                {//持续性伤害处理efd   
                    hpd = NeptuneBattle.Instance.Numeric.CalcFinalInjury(Owner, AbilityData.DamageType, hpd, Caster, this).FinalInjury;
                    if (AbilityData.UnDeath && hpd > Owner.HP)
                    {
                        hpd = UFloat.RoundToInt(Owner.HP - 1);
                    }
                    int damage = Math.Min(hpd, UFloat.RoundToInt(Owner.HP));
                    //damage = damage * this.Owner.dsFactor;
                    if (Owner != Caster)
                    {
                        NeptuneBattle.Instance.Statistic.OnDamage(Caster, damage);
                        Owner.RecordDamage(damage, Caster);
                        NeptuneBattle.Instance.Statistic.RecordDamage(Owner, Caster, Talent, damage);
                        Owner.CheckAttack(Caster);
                    }

                    //持续减少生命
                    fAbiltiyDamagedHP += damage;

                    Owner.SetHP(Owner.HP - damage);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("    {0} Abiltiy HpIntervalLost {1} {2:F6} -{3:F6} / {4:f6}", Owner.FullName, AbilityData.Name, Owner.HP, damage, hpd);
#endif
                    if (Owner.HP == 0)
                        Owner.End(Caster);
                    //this.Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP.ToString(), false, this.Caster);
                }
            }
            if (fAbiltiyRecoveryFrqc <= 0 && fAbiltiyRecoveredHP > 0)
            {
                if (NeptuneBattle.Instance.Scene != null && Owner.Joint != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Heal, EngineConst.SymbolPlus, (int)Math.Floor(fAbiltiyRecoveredHP + 1f / 2f), Owner.Joint, Caster != null ? Caster.Joint : null, false, RoleSide.None, RoleAttribute.MaxHP);
                }
                fAbiltiyRecoveryFrqc = AbilityData.PopupFrequent;
                fAbiltiyRecoveredHP = 0;
            }

            if (fAbiltiyDamageFrqc <= 0 && fAbiltiyDamagedHP > 0)
            {
                PopupType type = PopupType.HolyDamage;
                switch (AbilityData.DamageType)
                {
                    case InjuryType.AbilityPower: type = PopupType.AbilityDamage; break;
                    case InjuryType.AttackDamage: type = PopupType.AttackDamage; break;
                }
                if (NeptuneBattle.Instance.Scene != null && Owner.Joint != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(type, string.Empty, (int)Math.Floor(fAbiltiyDamagedHP), Owner.Joint, Caster.Joint, false, Owner.Side, RoleAttribute.ExtraHoly);
                }
                fAbiltiyDamageFrqc = AbilityData.PopupFrequent;
                fAbiltiyDamagedHP = 0;
            }


        }

        public virtual void OnEnterFrame(float dt)
        {

            UpdateAction(dt);
            if (!Owner.AbilityEffects.Norecover && Owner.HP < Owner.Attributes.MaxHP && (AbilityData.HPRegen > 0 || AbilityData.HPRegen_a > 0))
            {
                if (!Owner.AbilityEffects.Incurable)
                {//持续回复生命

                    float temp = AbilityData.HPRegen + Owner.Attributes.MaxHP * (AbilityData.HPRegen_a * EngineConst.Hundredth);
                    //if (this.Owner.Attributes.SHealHp > 0)
                    temp *= Math.Max(EngineConst.MinSHealHp, 1 + Owner.Attributes.SHealHp * EngineConst.Hundredth);
                    Owner.SetHP(Owner.HP + UFloat.Round(temp * dt));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("    {0} Abiltiy HPRegen {1} {2:F6} +{3:F6}", Owner.FullName, AbilityData.Name, Owner.HP, temp, dt);
#endif
                }
            }
            float hpd = UFloat.Round(AbilityData.HPDecrease + AbilityData.HPDecrease_a * (Owner.MaxHP * EngineConst.Hundredth));
            if (hpd > 0 && (!AbilityData.UnDeath || Owner.HP > 1))
            {//持续性伤害处理efd
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} {2:F6} +{3:F6} {4:f6} {5:f6}", Owner.FullName, AbilityData.Name, Owner.MaxHP, hpd, AbilityData.HPDecrease, AbilityData.HPDecrease_a);
#endif
                int finalHPD = NeptuneBattle.Instance.Numeric.CalcFinalInjury(Owner, AbilityData.DamageType, hpd, Caster, this).FinalInjury;
                hpd = finalHPD * dt;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} hpd = {2:F6}  = {3:F6} * {4:f6} HP:{5:f6} ", Owner.FullName, AbilityData.Name, hpd, finalHPD, dt, Owner.HP);
#endif
                if (AbilityData.UnDeath && hpd > Owner.HP)
                {
                    hpd = Owner.HP - 1;
                }
                float damage = Math.Min(hpd, Owner.HP);
                //damage = damage * this.Owner.dsFactor;
                if (Owner != Caster)
                {
                    NeptuneBattle.Instance.Statistic.OnDamage(Caster, damage);
                    Owner.RecordDamage(UFloat.RoundToInt(damage), Caster);
                    NeptuneBattle.Instance.Statistic.RecordDamage(Owner, Caster, Talent, damage);
                    Owner.CheckAttack(Caster);
                }

                //持续减少生命
                Owner.SetHP(Owner.HP - damage);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} {2:F6} +{3:F6} dt:{4:f6}", Owner.FullName, AbilityData.Name, Owner.HP, hpd, dt);
#endif
                if (Owner.HP == 0)
                    Owner.End(Caster);
                //this.Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP.ToString(), false, this.Caster);
            }
            float oldtimer = Duration;
            if (!float.IsInfinity(Duration))
                Duration = UFloat.Round(Duration - dt);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} ability Tick:{1} {2:f6} = {3:f6} - {4:f6}", Owner.FullName, AbilityData.Name, Duration, oldtimer, dt);
#endif

            if (Duration < 0)
            {
                Owner.RemoveAbility(this);//到达持续时间移除
                fAbiltiyRecoveryFrqc = AbilityData.PopupFrequent;
                fAbiltiyRecoveryFrqc = AbilityData.PopupFrequent;
                fAbiltiyRecoveredHP = 0;
                fAbiltiyDamagedHP = 0;
            }
        }
        public void SetEffectCount(int index)
        {
            if (index >= 0 && AbilityData.MaxOverlayLayer > 1 && index < AbilityData.MaxOverlayLayer && Owner.Joint != null && Owner.Joint.Controller != null)
            {
                Owner.Joint.SetEffectLayer(EffectID, index);
            }
        }

        /// <summary>
        /// 创建弹出Ability效果文字
        /// </summary>
        public void CreateAbilityEffect()
        {
            if (Owner != null && Owner.Joint != null && NeptuneBattle.Instance.Scene != null && !Owner.Data.NoBuffEffect)
            {
                if (!string.IsNullOrEmpty(AbilityData.Effect))
                {
                    if ((Owner.Data.RoleType & AbilityData.EffectRoleType) <= 0)
                    {
                        IActorController rolecontroller = null;
                        if (Talent != null && Owner != Talent.Caster && Talent.Caster.Joint != null)
                        {
                            rolecontroller = Talent.Caster.Joint.Controller;
                        }

                        EffectID = Owner.Joint.AddEffect(AbilityData.Effect, EffectType.Ability, new UnityEngine.Vector3(0, 0, AbilityData.EffectOrder), rolecontroller);
                        int count = Owner.GetAbilityCountById(AbilityData.ID);
                        if (AbilityData.MaxOverlayLayer > 1 && count <= AbilityData.MaxOverlayLayer && Owner.Joint.Controller != null)
                        {
                            Owner.Joint.Controller.SetEffectChild(EffectID, count - 1);
                        }
                    }
                }

                if (AbilityData.RoleFX != 0)
                {
                    RoleFXID = Owner.Joint.SetRoleFX(AbilityData.RoleFX);
                }

                RoleSide side = Owner.Side;
                if (Caster != null)
                {
                    if (!string.IsNullOrEmpty(AbilityData.PopupText))
                    {
                        side = Caster.Side;
                    }

                    NeptuneBattle.Instance.Scene.PopupText(AbilityData.PopupType, string.Empty, 0, Owner.Joint, Caster.Joint, false, side, RoleAttribute.None, AbilityData);
                }
            }
        }

        /// <summary>
        /// 清除Ability效果
        /// </summary>
        public void ClearAbilityEffect()
        {
            if (Owner.Joint != null)
            {
                if (EffectID > 0)
                    Owner.Joint.RemoveEffect(EffectID);
                if (RoleFXID > 0)
                {
                    Owner.Joint.ResetRoleFX(RoleFXID);
                    RoleFXID = 0;
                }
            }
        }

        /// <summary>
        /// 应用Ability效果到所有者
        /// </summary>
        public void InitAbility()
        {
            for (int i = 0; i < (int)RoleAttribute.MAX; i++)
            {
                if (RoleAttribute.HPDecrease == (RoleAttribute)i || RoleAttribute.HPRegen == (RoleAttribute)i)
                {
                    continue;
                }
                float value = AbilityData[i];
                float ratioValue = AbilityData[RoleAttributes.GetRatioValueField(i)];

                if (ratioValue != 0)
                {
                    if (AbilityData.RatioByBase)
                        Owner.AllAttributes[(int)AttributeType.Ability][i] = Owner.AllAttributes[(int)AttributeType.Base][i] * ratioValue;
                    else
                        Owner.AllAttributes[(int)AttributeType.Ability][RoleAttributes.GetRatioValueField(i)] = Owner.AllAttributes[(int)AttributeType.Ability][RoleAttributes.GetRatioValueField(i)] + ratioValue;
                }

                if (value != 0)
                {
                    Owner.AllAttributes[(int)AttributeType.Ability][i] = Owner.AllAttributes[(int)AttributeType.Ability][i] + value;
                }

#if BATTLE_LOG
                if (EngineGlobal.BattleLog && (value != 0 || ratioValue != 0))
                    NeptuneBattle.log("{0} Ability: {1}  {2}:{3:f6} {4:f6}  {5:f6}", Owner.FullName, AbilityData.Name, (RoleAttribute)i, Owner.AllAttributes[(int)AttributeType.Ability][i], value, ratioValue);
#endif
            }
            //foreach (var controlEffect in ControlEffects)
            //{
            //  this.InitEffects(controlEffect.Key);
            //}
        }

        public void TransformAttributes()
        {
            //属性转换（释放者属性转换为Buff 拥有者属性加成）
            if (AbilityData.TransformAttributeFrom != RoleAttribute.None)
            {
                int TransformFrom = AbilityData.FromIsRatio ? RoleAttributes.GetRatioValueField(AbilityData.TransformAttributeFrom) : (int)AbilityData.TransformAttributeFrom;
                float transfromValue = UFloat.Round(Caster.Attributes[TransformFrom] * (AbilityData.TransformAttributeRatio * EngineConst.Hundredth));
                transfromValue = AbilityData.TransformAttributeMaxValue > 0 ? Math.Min(transfromValue, AbilityData.TransformAttributeMaxValue) : transfromValue;
                int TransformTo = AbilityData.ToIsRatio ? RoleAttributes.GetRatioValueField(AbilityData.TransformAttributeTo) : (int)AbilityData.TransformAttributeTo;
                Owner.AllAttributes[(int)AttributeType.Ability][TransformTo] += transfromValue;
            }
        }

        /// <summary>
        /// Set duration (Config Time).(seconds)
        /// </summary>
        /// <param name="time"></param>
        public void SetDuration(float time)
        {
            if (time < 0)
                Duration = float.PositiveInfinity;
            else
                Duration = UFloat.Round(time / 1000f);
        }

        public void ReBuild()
        {
            foreach (var controlEffect in ControlEffects)
            {
                InitEffect((ControlEffect)controlEffect.Key);
            }
        }

        public void InitEffect(ControlEffect effect)
        {
            if (Owner.AbilityEffects[(int)effect])
                return;
            Owner.AbilityEffects[(int)effect] = true;
            if (EngineConst.NestedAbilities.ContainsKey((int)effect))
            {
                foreach (ControlEffect eff in EngineConst.NestedAbilities[(int)effect])
                {
                    InitEffect(eff);
                }
            }
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        public override void OnDelete()
        {
            Owner.RemoveExtraTalentByAbility(this);

            ClearAbilityEffect();

            if (TransformIndex > 0)
            {
                Owner.RemoveTransformation(TransformIndex);
            }
            else if (ModelID > 0)
            {
                Owner.RemoveModel(ModelID);
            }
            if (AbilityData.ModelScaleDuration > 0)
            {

                Owner.zoomIndex.Remove(ZoomIndex);
                Owner.StartZoom(AbilityData.ModelScale, AbilityData.ModelScaleDuration);
            }
            Ability talentcd = null;

            //删除时还原技能
            if (FromTalentID > 0 && ToTalentID > 0)
            {

                if (Owner.ChangeTalentStack.ContainsKey(FromTalentID) && Owner.ChangeTalentStack[FromTalentID].Contains(ChangeIndex))
                {
                    if (ChangeIndex == Owner.ChangeTalentStack[FromTalentID].Max())
                    {
                        talentcd = this;
                    }
                    Owner.ChangeTalentStack[FromTalentID].Remove(ChangeIndex);
                    //                 this.Owner.EnableTalents(this.FromTalentID, this.ToTalentID, false);
                    //                 if (this.Owner.ChangeTalentStack[this.FromTalentID].Count <= 0)
                    //                     this.Owner.EnableTalents(this.FromTalentID, this.FromTalentID, true);
                }
            }
            if (TransformIndex > 0 || FromTalentID > 0 && ToTalentID > 0)
                Owner.InitChangeTalents(talentcd, TransformIndex > 0 ? 0 : FromTalentID);
            talentDamegeFactor(-1);
        }


        /// <summary>
        /// Ability移除事件处理
        /// </summary>
        public virtual void OnDestroyed()
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} lost ability:{1} {2:f6}", Owner.FullName, AbilityData.Name, Duration);
#endif
            isDestroy = true;

            //处理Ability移除时可触发的逻辑
            if (NeptuneBattle.Instance.Scene != null && (AbilityData.ShieldValue > 0 || AbilityData.ShieldRecharge > 0))
            {
                NeptuneBattle.Instance.Scene.PopupText(PopupType.ShieldFade, string.Empty, 0, Owner.Joint, Caster.Joint, false, Owner.Side, RoleAttribute.None, AbilityData);
            }
            for (int i = 0; i < Caster.tActiveTalents.Length; i++)
            {
                BattleSkill talent = Caster.tActiveTalents[i];
                if (talent != null)
                {
                    if (!talent.IsEnabled) continue;
                    if ((talent.Data.TriggerType & TriggerType.Ability) > 0 && talent.Data.TriggerParam1 == (int)TriggerAbilityType.End && talent.Data.TriggerParam == AbilityData.ID)
                    {
                        if (talent.Data.TargetType == TargetType.Self)
                        {
                            talent.Target = Caster;
                        }
                        else if (talent.Data.TargetType == TargetType.Target)
                        {
                            talent.Target = Owner;
                        }
                        else
                        {
                            talent.Target = null;
                        }
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} lost ability:{1} Trigger {2}  {3}", Owner.FullName, AbilityData.Name, talent.ToString(), talent.Target == null ? "null" : talent.Target.FullName);
#endif
                        if (talent.CanUse(talent.Target, true, true) == GameData.ResultType.Success)
                        {
                            if (!talent.Data.Instant)
                            {
                                if (Caster.CurrentTalent != null && Caster.CurrentTalent.Casting)
                                {
                                    Caster.CurrentTalent.End();
                                }
                            }

                            talent.Start(talent.Target);
                        }
                    }
                }
            }


            //吸收护盾返还伤害
            if (AbilityData.ShieldClass == ShieldClass.Absorption && TotalDamage > 0)
            {
                float damage = TotalDamage * AbilityData.ShieldRestorationRatio * 0.01f;
                if (AbilityData.UnDeath && damage > Owner.HP)
                {
                    damage = Owner.HP - 1;
                }
                Owner.SetHP(Owner.HP - damage);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy ShieldRestoration {1} {2:F6} -{3:F6} TDMG:{4:F6} SRR:{5:F6}", Owner.FullName, AbilityData.Name, Owner.HP, damage, TotalDamage, AbilityData.ShieldRestorationRatio);
#endif
                if (Owner.HP == 0)
                    Owner.End(Caster);
                Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP, false, Caster);
            }
            if (!string.IsNullOrEmpty(AbilityData.EndEffect))
            {
                if (Owner.Joint != null)
                    Owner.Joint.AddEffect(AbilityData.EndEffect, EffectType.Normal, EngineConst.Vector3Zero);
            }
            //删除Ability
            Delete();
        }

        /// <summary>
        /// 伤害事件处理
        /// </summary>
        /// <param name="injury"></param>
        /// <param name="injuryType"></param>
        /// <returns></returns>
        public virtual float OnInjury(float injury, InjuryType injuryType, BattleSkill fromTalent)
        {
            if (AbilityData.MPDmgRecovery > 0 && injury > 0)
            {//受伤回复MP
                Owner.Remedy(AbilityData.MPDmgRecovery, RoleAttribute.MaxMP, Owner);
            }

            float finalDmg = injury;
            if (fromTalent == null || AbilityData.ShieldAbsorbType == ShieldAbsorbType.All //任何攻击
                    || AbilityData.ShieldAbsorbType == ShieldAbsorbType.Talent && (fromTalent.Data.SkillType == SkillType.Skill || fromTalent.Data.SkillType == SkillType.ChildSkill)  //技能
                    || AbilityData.ShieldAbsorbType == ShieldAbsorbType.Normal && fromTalent.Data.SkillType == SkillType.Normal)//普攻
            {
                ShieldType shieldtype = AbilityData.ShieldType;
                if (AbilityData.ShieldType == ShieldType.All || (int)shieldtype == (int)injuryType)
                {
                    bool trigger = false;
                    if (AbilityData.ShieldClass == ShieldClass.Measure)
                    {//计量护盾处理
                        if (Protection > 0)
                        {
                            Protection = Protection - finalDmg;

                            if (Protection < 0)
                            {
                                trigger = true;
                                Owner.RemoveAbility(this);
                                finalDmg = -Protection;

                            }
                            else
                            {
                                PopDamageText(shieldtype);
                                finalDmg = 0;
                            }
                        }
                    }
                    else if (AbilityData.ShieldClass == ShieldClass.Rechargeable)
                    {//计数护盾处理
                        if (finalDmg <= 0)
                        {
                            if (Recharge >= ShieldRecharge)
                            {
                                trigger = true;
                                Owner.RemoveAbility(this);
                            }

                            finalDmg = 0;
                        }
                        else if (Protection > 0)
                        {
                            Protection = Protection - finalDmg;
                            float dmg = Protection;
                            Recharge = Recharge + 1;

                            Protection = AbilityData.ShieldValue;
                            if (AbilityData.BaseRatio > 0)
                            {
                                Protection = Protection + shieldAdd;
                            }
                            if (Recharge >= ShieldRecharge)
                            {
                                trigger = true;
                                Owner.RemoveAbility(this);
                            }

                            if (dmg < 0)
                                finalDmg = -dmg;
                            else
                                finalDmg = 0;
                            PopDamageText(shieldtype);
                            if (Owner.Joint != null && Owner.Joint.Controller != null)
                                Owner.Joint.SetEffectLayer(EffectID, Recharge - 1);
                        }
                    }
                    else if (AbilityData.ShieldClass == ShieldClass.Deathless)
                    {//不死护盾处理
                        if (Protection > 0)
                        {
                            float remain = Owner.HP - 1;
                            if (finalDmg > remain)
                            {
                                float quits = finalDmg - remain;
                                if (ShieldRecharge > 0)
                                {
                                    if (Recharge >= ShieldRecharge)
                                    {
                                        trigger = true;
                                        Owner.RemoveAbility(this);
                                    }
                                    else
                                    {
                                        Recharge = Recharge + 1;
                                        if (Owner.Joint != null && Owner.Joint.Controller != null)
                                            Owner.Joint.SetEffectLayer(EffectID, Recharge - 1);
                                    }
                                }
                                else
                                {
                                    if (quits < Protection)
                                    {
                                        Protection = Protection - quits;
                                        if (NeptuneBattle.Instance.Scene != null && Owner.Joint != null)
                                        {
                                            NeptuneBattle.Instance.Scene.PopupText(PopupType.Funeral, string.Empty, 0, Owner.Joint, Caster.Joint, false, Owner.Side);
                                        }
                                    }
                                    else
                                    {
                                        quits = Protection;
                                        Protection = 0;
                                        trigger = true;
                                        Owner.RemoveAbility(this);
                                    }
                                }

                                finalDmg = finalDmg - quits;
                            }
                        }
                    }
                    else if (AbilityData.ShieldClass == ShieldClass.Absorption)
                    {//吸收返回护盾
                     //吸收百分比
                        if (ProtectionRatio > 0)
                        {
                            TotalDamage += finalDmg * ProtectionRatio * 0.01f;
                            finalDmg *= 1 - ProtectionRatio * 0.01f;
                        }
                        //吸收固定值
                        if (Protection > 0)
                        {
                            if (Protection - finalDmg < 0)
                            {
                                finalDmg -= Protection;
                                TotalDamage += Protection;
                            }
                            else
                            {
                                TotalDamage += finalDmg;
                                finalDmg = 0;
                            }
                        }
                        PopDamageText(shieldtype);
                    }
                    else if (AbilityData.ShieldClass == ShieldClass.DamageReduce)
                    {
                        finalDmg = injury * Math.Max(0, 1 - Protection);
                    }
                    if (trigger)
                        Owner.OnBrokeShield(AbilityData.ID);
                }
            }


            float quitsDmg = injury - finalDmg;
            if (quitsDmg > 0 && AbilityData.ShieldToHP > 0)
            {
                //护盾转化生命
                Caster.Remedy(quitsDmg * AbilityData.ShieldToHP);
                if (Caster.Joint != null)
                    Caster.Joint.AddEffect(AbilityData.HitEffect, EffectType.Normal, EngineConst.Vector3Zero);
            }
            return finalDmg;
        }

        private void PopDamageText(ShieldType shieldtype)
        {
            if (NeptuneBattle.Instance.Scene != null && Owner.Joint != null)
            {
                PopupType type = PopupType.None;
                switch (shieldtype)
                {
                    case ShieldType.AttackDamage:
                        type = PopupType.PhyVoid;
                        break;
                    case ShieldType.AbilityPower:
                        type = PopupType.MagVoid;
                        break;
                    case ShieldType.All:
                        type = PopupType.Void;
                        break;
                }
                //Logic.Instance.Scene.PopupText(type, string.Empty, this.Owner.Joint, this.Caster.Joint, false, this.Owner.Side);//护盾免疫不需要冒字
            }
        }

        /// <summary>
        /// 获取是否为控制Ability
        /// </summary>
        public bool IsControlAbility
        {
            get
            {
                bool result = false;
                foreach (var effect in ControlEffects)
                {
                    if (EngineConst.ConflictingAbilities.ContainsKey(effect.Key) && EngineConst.ConflictingAbilities[effect.Key])
                    {
                        if (EngineConst.ConflictingAbilities[effect.Key])
                        {
                            result = true;
                            break;
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取是否不可控制
        /// </summary>
        public bool IsUnControllable
        {
            get
            {
                bool result = false;
                if (ControlEffects.ContainsKey((int)ControlEffect.Unaffected))
                {
                    result = true;
                }
                return result;
            }
        }

        public void CheckTransferAbility(BattleActor attacker)
        {
            if (attacker == null || attacker.IsDead)
            {
                return;
            }
            if (AbilityData.TransferTargetType == TransferTargetType.Attacker)
            {
                if (attacker != Owner && CheckSide(attacker))
                {
                    if ((AbilityData.TargetRoleType & attacker.Data.RoleType) > 0 || AbilityData.TargetRoleType == RoleType.Any)
                    {
                        TransferAbility(attacker);
                    }
                }
            }
            else if (AbilityData.TransferTargetType == TransferTargetType.FinallyHitPlayer)
            {
                if ((AbilityData.TargetRoleType & attacker.Data.RoleType & RoleType.Hero) == RoleType.Hero)
                {
                    BattlePlayer deathPlayer = Owner.Player;
                    BattlePlayer attackerPlayer = attacker.Player;
                    //死亡为非英雄 击杀者为英雄
                    if (deathPlayer == null && attackerPlayer != null && CheckSide(attacker))
                    {
                        TransferAbility(attacker);
                    }
                    else
                    {
                        if (attackerPlayer == null && deathPlayer != null)
                        {
                            if (CheckSide(Owner.FinalKiller))
                                TransferAbility(Owner.FinalKiller);
                        }
                        else if (attackerPlayer != null && deathPlayer != null)
                        {//玩家被玩家直接杀死
                            if (CheckSide(attacker))
                                TransferAbility(attacker);
                        }
                    }
                }
            }
            else if (AbilityData.TransferTargetType == TransferTargetType.AttackerSide)
            {
                if (attacker != null && attacker != Owner && CheckSide(attacker))
                {
                    if ((AbilityData.TargetRoleType & attacker.Data.RoleType) > 0 || AbilityData.TargetRoleType == RoleType.Any)
                    {
                        SafeList<BattleActor> roles = NeptuneBattle.Instance.AliveRoles[(int)attacker.Side];
                        for (int i = 0; i < roles.Count; i++)
                        {
                            if ((AbilityData.TargetRoleType & roles[i].Data.RoleType) > 0)
                                TransferAbility(roles[i]);
                        }
                    }

                }
            }
        }

        public void AbilityAttrPlus(int count)
        {
            for (int i = 0; i < (int)RoleAttribute.MAX; i++)
            {
                AbilityData[i] *= count;

                AbilityData[RoleAttributes.GetRatioValueField(i)] *= count;
            }
        }
        public void TransferAbility(BattleActor role)
        {
            if (role == null || role.IsDead)
                return;
            AbilityData ability_info = NeptuneBattle.Instance.DataProvider.GetAbilityData(AbilityData.TransferAbilityID);
            if (ability_info != null)
            {
                role.AddAbility(ability_info.Clone(), Caster, null);
            }
            else
            {
                role.AddAbility(AbilityData, Caster, null);
            }
        }

        /// <summary>
        /// 检查是否满足阵营条件
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool CheckSide(BattleActor role)
        {
            if (role == null)
                return false;
            if (AbilityData.TargetSide == RelativeSide.Both || AbilityData.TargetSide == Caster.GetRelation(role))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// reset shield value
        /// </summary>
        /// <param name="shieldvalue"></param>
        /// <param name="shieldrecharge"></param>
        public void ResetShield(float shieldvalue, int shieldrecharge)
        {
            if (AbilityData.ShieldClass == ShieldClass.Rechargeable)
                ShieldRecharge = shieldrecharge;
            if (AbilityData.ShieldClass == ShieldClass.Measure)
                Protection = shieldvalue;
        }
    }
}