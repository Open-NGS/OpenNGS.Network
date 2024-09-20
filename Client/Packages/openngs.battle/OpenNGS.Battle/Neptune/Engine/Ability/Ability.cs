using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neptune.Datas;

namespace Neptune
{
    public class Ability : AbilityBase
    {
        public Actor Owner;
        public Skill Skill;
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

        public int FromSkillID;
        public int ToSkillID;
        public int ChangeIndex;

        public int ZoomIndex;
        private float shieldAdd;
        public Ability()
        {

        }

        public void Create(AbilityData data, Actor owner, Actor caster, Skill talent, int tid = -100)
        {
            this.ModelID = 0;
            this.TransformIndex = 0;
            this.TotalDamage = 0;
            this.Recharge = 0;
            this.ZoomIndex = 0;
            this.EffectID = 0;
            this.RoleFXID = 0;
            this.ChangeIndex = 0;
            this.FromSkillID = 0;
            this.ToSkillID = 0;
            this.ControlEffects.Clear();
            this.shieldAdd = 0;
            isDestroy = false;
            this.AbilityData = data;// data.Clone();

            this.Owner = owner;
            this.Skill = talent;
            this.Caster = caster;
            if (this.Caster != null)
                this.Caster.RoleSkin.RoleSkinAbilityReplace(this.AbilityData);

            //this.startTime = this.AbilityData.StartTime;
            this.intervalTime = this.AbilityData.StartTime;

            fAbiltiyRecoveryFrqc = this.AbilityData.PopupFrequent;
            fAbiltiyDamageFrqc = this.AbilityData.PopupFrequent;
            fAbiltiyRecoveredHP = 0;
            fAbiltiyDamagedHP = 0;
            if (this.AbilityData.ActiveSkillIDs != null && this.AbilityData.ActiveSkillIDs.Length > 1)
            {
                FromSkillID = this.AbilityData.ActiveSkillIDs[0];
                ToSkillID = this.AbilityData.ActiveSkillIDs[1];
            }

            this.DontRemoveOnDeath = data.DontRemoveOnDeath;
            InitDuration();
            this.addControlEffects();
            this.initShieldValue();
            this.talentCDReductions();
            this.talentDamegeFactor();
        }

        private void addControlEffects()
        {
            if (this.AbilityData.ControlEffects != null)
            {
                for (int i = 0; i < this.AbilityData.ControlEffects.Length; i++)
                {
                    ControlEffects.Add(this.AbilityData.ControlEffects[i], true);
                }
            }
            if (this.ControlEffects.ContainsKey((int)ControlEffect.Charm))
            {
                //增加魅惑Ability时,重置目标普通攻击CD
                this.Owner.CommonCooldown = 0;
                this.Owner.BasicSkill.Duration = 0;
            }
        }

        private void initShieldValue()
        {
            this.Protection = this.AbilityData.ShieldValue;
            this.ProtectionRatio = this.AbilityData.ShieldAbsorbRatio;
            this.ShieldRecharge = this.AbilityData.ShieldRecharge;

            if (this.AbilityData.BaseRatio > 0)
            {
                this.shieldAdd = UFloat.Round(this.Caster != null ? this.AbilityData.BaseRatio * UFloat.Round(this.Caster.AttributeFinalValue[(int)this.AbilityData.BaseAttribute] * NeptuneConst.Hundredth) : this.AbilityData.BaseRatio * UFloat.Round(this.Owner.AttributeFinalValue[(int)this.AbilityData.BaseAttribute] * NeptuneConst.Hundredth));
            }
            if (this.Owner.Config.HPFactor > 0 && this.Protection > 0)
            {
                this.Protection = UFloat.Round(this.Protection * this.Owner.Config.HPFactor);
            }
            this.Protection = UFloat.Round(this.Protection + this.shieldAdd);
        }

        private void talentCDReductions()
        {
            //CD缩减
            if (this.AbilityData.CDReductions != null)
            {
                if (this.AbilityData.CDReductions.Length % 3 != 0)
                {
                    Logger.LogError("AbilityData.CDReductions error!");
                    return;
                }
                for (int i = 0; i < this.AbilityData.CDReductions.Length; i += 3)
                {
                    Skill target_talent = this.Owner.GetSkillById(this.AbilityData.CDReductions[i]);
                    if (target_talent != null)
                    {
                        float beforeDuration = target_talent.Duration;
                        target_talent.Duration = UFloat.Round(target_talent.Duration - target_talent.Duration * (this.AbilityData.CDReductions[i + 2] * NeptuneConst.Hundredth) - this.AbilityData.CDReductions[i + 1] * NeptuneConst.Milli);
                        target_talent.Duration = target_talent.Duration < 0 ? 0 : target_talent.Duration;
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy CDReductions Skill:{1} from {2:F6}", this.Owner.FullName, target_talent.ToString(), beforeDuration);
#endif
                    }
                }
            }
        }

        private void talentDamegeFactor(int addOrReduce = 1)
        {
            if (this.AbilityData.DamageFactors != null)
            {
                if (this.AbilityData.DamageFactors.Length % 2 != 0)
                {
                    Logger.LogError("AbilityData.DamageFactors error!");
                    return;
                }
                for (int i = 0; i < this.AbilityData.DamageFactors.Length; i += 2)
                {
                    Skill target_talent = this.Owner.GetSkillById(this.AbilityData.DamageFactors[i]);
                    if (target_talent != null)
                    {
                        float beforeDamageFactor = target_talent.DamageFactor;
                        target_talent.SetDamageFactor(this.AbilityData.DamageFactors[i + 1] * NeptuneConst.Hundredth * addOrReduce);
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy DamageFactors Skill:{1} from {2:F6}", this.Owner.FullName, target_talent.ToString(), beforeDamageFactor);
#endif
                    }
                }
            }
        }

        public void SetTransformModel()
        {
            //在同一个ability 中如果同时存在 ModelId 和TransformID 优先处理TransformID忽略ModelId （两者不能同时生效）
            if (this.AbilityData.TransformID != 0)
            {
                this.TransformIndex = this.Owner.EnterTransformation(this.AbilityData.TransformID);
            }
            else if (!string.IsNullOrEmpty(this.AbilityData.Model))
            {
                this.ModelID = this.Owner.SetModel(this.AbilityData.Model);
            }

        }

        public void ModelScale()
        {
            if (this.AbilityData.ModelScaleDuration > 0)
            {
                int index = 1;
                if (this.Owner.zoomIndex.Count > 0)
                {
                    index = this.Owner.zoomIndex.Keys.Max() + 1;
                }
                this.ZoomIndex = index;
                this.Owner.zoomIndex.Add(index, this.AbilityData.ModelScale);
                this.Owner.StartZoom(this.AbilityData.ModelScale, this.AbilityData.ModelScaleDuration);
            }
        }

        public void ChangeSkill()
        {
            if (this.FromSkillID > 0 && this.ToSkillID > 0)
            {
                int fromSkillID = this.FromSkillID;
                int toSkillID = this.ToSkillID;
                if (this.Owner.ChangeSkillStack.ContainsKey(fromSkillID))
                {
                    this.ChangeIndex = ++this.Owner.changeIndex;
                    this.Owner.ChangeSkillStack[fromSkillID].Add(this.ChangeIndex);
                }
            }
            if (this.AbilityData.TransformID != 0 || (this.FromSkillID > 0 && this.ToSkillID > 0))
                this.Owner.InitChangeSkills(null, this.AbilityData.TransformID == 0 ? this.FromSkillID : 0);
        }

        public void InitDuration()
        {
            this.SetDuration(this.AbilityData.Time);
            if (this.Duration > 0 && !float.IsInfinity(this.Duration))
            {
                float factor = 0;
                if (this.AbilityData != null && this.Owner != null && this.Caster != null && this.Owner.Side != this.Caster.Side)
                {
                    if (this.AbilityData.Tough != RoleAttribute.None && this.Owner.AttributeFinalValue[(int)this.AbilityData.Tough] > 0)
                        factor = this.Owner.AttributeFinalValue[(int)this.AbilityData.Tough];
                }
                this.Duration = UFloat.Round(this.Duration * Math.Max(0, 1 - UFloat.Round(factor * NeptuneConst.Hundredth)));
            }
        }

        public override void Delete()
        {
            ObjectPool<Ability>.Delete(this);
        }

        public void UpdateAction(float dt)
        {
            fAbiltiyRecoveryFrqc -= dt;
            fAbiltiyDamageFrqc -= dt;

            if (this.intervalTime > 0)
            {
                this.intervalTime -= dt;
            }
            if (this.intervalTime <= 0)
            {
                this.intervalTime += this.AbilityData.IntervalTime;
                if (!this.Owner.AbilityEffects.Value.Norecover && this.Owner.HP < this.Owner.AttributeFinalValue.MaxHP && (this.AbilityData.Value.HpIntervalIncrease > 0 || this.AbilityData.Ratio.HpIntervalIncrease > 0))
                {
                    if (!this.Owner.AbilityEffects.Value.Incurable)
                    {//持续回复生命
                        float temp = UFloat.Round(this.AbilityData.Value.HpIntervalIncrease + this.Owner.AttributeFinalValue.MaxHP * this.AbilityData.Ratio.HpIntervalIncrease * NeptuneConst.Hundredth);
                        if (this.Owner.AttributeFinalValue.SHealHp != 0.0f)
                            temp = UFloat.Round(temp * Math.Max(NeptuneConst.MinSHealHp, (1 + this.Owner.AttributeFinalValue.SHealHp * NeptuneConst.Hundredth)));
                        fAbiltiyRecoveredHP += temp;
                        this.Owner.SetHP(this.Owner.HP + temp);
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("    {0} Abiltiy HpIntervalIncrease {1} {2:F6} +{3:F6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.HP, temp);
#endif
                    }
                }
                int hpd = UFloat.RoundToInt(this.AbilityData.Value.HpIntervalLost + this.AbilityData.Ratio.HpIntervalLost * (this.AbilityData.HpLostRatioBaseOnCurHP ? this.Owner.HP : this.Owner.MaxHP) * NeptuneConst.Hundredth);
                if (hpd > 0 && (!this.AbilityData.UnDeath || this.Owner.HP > 1))
                {//持续性伤害处理efd   
                    hpd = NeptuneBattle.Instance.Numeric.CalcFinalInjury(this.Owner, this.AbilityData.DamageType, hpd, this.Caster, this).FinalDamage;
                    if (this.AbilityData.UnDeath && hpd > this.Owner.HP)
                    {
                        hpd = UFloat.RoundToInt(this.Owner.HP - 1);
                    }
                    int damage = Math.Min(hpd, UFloat.RoundToInt(this.Owner.HP));
                    //damage = damage * this.Owner.dsFactor;
                    if (this.Owner != this.Caster)
                    {
                        NeptuneBattle.Instance.Statistic.OnDamage(this.Caster, damage);
                        this.Owner.RecordDamage(damage, this.Caster);
                        NeptuneBattle.Instance.Statistic.RecordDamage(this.Owner, this.Caster, this.Skill, damage);
                        this.Owner.CheckAttack(this.Caster);
                    }

                    //持续减少生命
                    fAbiltiyDamagedHP += damage;

                    this.Owner.SetHP(this.Owner.HP - damage);
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("    {0} Abiltiy HpIntervalLost {1} {2:F6} -{3:F6} / {4:f6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.HP, damage, hpd);
#endif
                    if (this.Owner.HP == 0)
                        this.Owner.End(this.Caster);
                    //this.Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP.ToString(), false, this.Caster);
                }
            }
            if (fAbiltiyRecoveryFrqc <= 0 && fAbiltiyRecoveredHP > 0)
            {
                if (NeptuneBattle.Instance.Scene != null && this.Owner.Joint != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(PopupType.Heal, NeptuneConst.SymbolPlus, (int)Math.Floor(fAbiltiyRecoveredHP + 1f / 2f), this.Owner.Joint, this.Caster != null ? this.Caster.Joint : null, false, RoleSide.None, RoleAttribute.MaxHP);
                }
                fAbiltiyRecoveryFrqc = this.AbilityData.PopupFrequent;
                fAbiltiyRecoveredHP = 0;
            }

            if (fAbiltiyDamageFrqc <= 0 && fAbiltiyDamagedHP > 0)
            {
                PopupType type = PopupType.HolyDamage;
                switch (this.AbilityData.DamageType)
                {
                    case InjuryType.AbilityPower: type = PopupType.AbilityDamage; break;
                    case InjuryType.AttackDamage: type = PopupType.AttackDamage; break;
                }
                if (NeptuneBattle.Instance.Scene != null && this.Owner.Joint != null)
                {
                    NeptuneBattle.Instance.Scene.PopupText(type, string.Empty, (int)Math.Floor(fAbiltiyDamagedHP), this.Owner.Joint, this.Caster.Joint, false, this.Owner.Side, RoleAttribute.ExtraHoly);
                }
                fAbiltiyDamageFrqc = this.AbilityData.PopupFrequent;
                fAbiltiyDamagedHP = 0;
            }


        }

        public virtual void OnEnterFrame(float dt)
        {

            this.UpdateAction(dt);
            if (!this.Owner.AbilityEffects.Value.Norecover && this.Owner.HP < this.Owner.AttributeFinalValue.MaxHP && (this.AbilityData.Value.HPRegen > 0 || this.AbilityData.Ratio.HPRegen > 0))
            {
                if (!this.Owner.AbilityEffects.Value.Incurable)
                {//持续回复生命

                    float temp = this.AbilityData.Value.HPRegen + this.Owner.AttributeFinalValue.MaxHP * (this.AbilityData.Ratio.HPRegen * NeptuneConst.Hundredth);
                    //if (this.Owner.Attributes.SHealHp > 0)
                    temp *= Math.Max(NeptuneConst.MinSHealHp, (1 + this.Owner.AttributeFinalValue.SHealHp * NeptuneConst.Hundredth));
                    this.Owner.SetHP(this.Owner.HP + UFloat.Round(temp * dt));
#if BATTLE_LOG
                    if (EngineGlobal.BattleLog)
                        NeptuneBattle.log("    {0} Abiltiy HPRegen {1} {2:F6} +{3:F6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.HP, temp, dt);
#endif
                }
            }
            float hpd = UFloat.Round(this.AbilityData.Value.HPDecrease + this.AbilityData.Ratio.HPDecrease * (this.Owner.MaxHP * NeptuneConst.Hundredth));
            if (hpd > 0 && (!this.AbilityData.UnDeath || this.Owner.HP > 1))
            {//持续性伤害处理efd
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} {2:F6} +{3:F6} {4:f6} {5:f6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.MaxHP, hpd, this.AbilityData.Value.HPDecrease, this.AbilityData.Ratio.HPDecrease);
#endif
                int finalHPD = NeptuneBattle.Instance.Numeric.CalcFinalInjury(this.Owner, this.AbilityData.DamageType, hpd, this.Caster, this).FinalDamage;
                hpd = finalHPD * dt;
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} hpd = {2:F6}  = {3:F6} * {4:f6} HP:{5:f6} ", this.Owner.FullName, this.AbilityData.Name, hpd, finalHPD, dt, this.Owner.HP);
#endif
                if (this.AbilityData.UnDeath && hpd > this.Owner.HP)
                {
                    hpd = this.Owner.HP - 1;
                }
                float damage = Math.Min(hpd, this.Owner.HP);
                //damage = damage * this.Owner.dsFactor;
                if (this.Owner != this.Caster)
                {
                    NeptuneBattle.Instance.Statistic.OnDamage(this.Caster, damage);
                    this.Owner.RecordDamage(UFloat.RoundToInt(damage), this.Caster);
                    NeptuneBattle.Instance.Statistic.RecordDamage(this.Owner, this.Caster, this.Skill, damage);
                    this.Owner.CheckAttack(this.Caster);
                }

                //持续减少生命
                this.Owner.SetHP(this.Owner.HP - damage);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy HPDecrease {1} {2:F6} +{3:F6} dt:{4:f6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.HP, hpd, dt);
#endif
                if (this.Owner.HP == 0)
                    this.Owner.End(this.Caster);
                //this.Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP.ToString(), false, this.Caster);
            }
            float oldtimer = this.Duration;
            if (!float.IsInfinity(this.Duration))
                this.Duration = UFloat.Round(this.Duration - dt);
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("    {0} ability Tick:{1} {2:f6} = {3:f6} - {4:f6}", this.Owner.FullName, this.AbilityData.Name, this.Duration, oldtimer, dt);
#endif

            if (this.Duration < 0)
            {
                this.Owner.RemoveAbility(this);//到达持续时间移除
                fAbiltiyRecoveryFrqc = this.AbilityData.PopupFrequent;
                fAbiltiyRecoveryFrqc = this.AbilityData.PopupFrequent;
                fAbiltiyRecoveredHP = 0;
                fAbiltiyDamagedHP = 0;
            }
        }
        public void SetEffectCount(int index)
        {
            if (index >= 0 && this.AbilityData.MaxOverlayLayer > 1 && index < this.AbilityData.MaxOverlayLayer && this.Owner.Joint != null && this.Owner.Joint.Controller != null)
            {
                this.Owner.Joint.SetEffectLayer(this.EffectID, index);
            }
        }

        /// <summary>
        /// 创建弹出Ability效果文字
        /// </summary>
        public void CreateAbilityEffect()
        {
            if (this.Owner != null && this.Owner.Joint != null && NeptuneBattle.Instance.Scene != null && !this.Owner.Data.NoBuffEffect)
            {
                if (!string.IsNullOrEmpty(this.AbilityData.Effect))
                {
                    if ((this.Owner.Data.RoleType & this.AbilityData.EffectRoleType) <= 0)
                    {
                        IActorController rolecontroller = null;
                        if (this.Skill != null && this.Owner != this.Skill.Caster && this.Skill.Caster.Joint != null)
                        {
                            rolecontroller = (IActorController)this.Skill.Caster.Joint.Controller;
                        }

                        this.EffectID = this.Owner.Joint.AddEffect(this.AbilityData.Effect, EffectType.Ability, new UnityEngine.Vector3(0, 0, this.AbilityData.EffectOrder), rolecontroller);
                        int count = this.Owner.GetAbilityCountById(this.AbilityData.ID);
                        if (this.AbilityData.MaxOverlayLayer > 1 && count <= this.AbilityData.MaxOverlayLayer && this.Owner.Joint.Controller != null)
                        {
                            ((IActorController)this.Owner.Joint.Controller).SetEffectChild(this.EffectID, count - 1);
                        }
                    }
                }

                if (this.AbilityData.RoleFX != 0)
                {
                    this.RoleFXID = this.Owner.Joint.SetRoleFX(this.AbilityData.RoleFX);
                }

                RoleSide side = this.Owner.Side;
                if (this.Caster != null)
                {
                    if (!string.IsNullOrEmpty(this.AbilityData.PopupText))
                    {
                        side = this.Caster.Side;
                    }

                    NeptuneBattle.Instance.Scene.PopupText(this.AbilityData.PopupType, string.Empty, 0, this.Owner.Joint, this.Caster.Joint, false, side, RoleAttribute.None, this.AbilityData);
                }
            }
        }

        /// <summary>
        /// 清除Ability效果
        /// </summary>
        public void ClearAbilityEffect()
        {
            if (this.Owner.Joint != null)
            {
                if (this.EffectID > 0)
                    this.Owner.Joint.RemoveEffect(this.EffectID);
                if (this.RoleFXID > 0)
                {
                    this.Owner.Joint.ResetRoleFX(this.RoleFXID);
                    this.RoleFXID = 0;
                }
            }
        }

        /// <summary>
        /// 应用Ability效果到所有者
        /// </summary>
        public void InitAbility()
        {
            for (RoleAttribute i = 0; i < RoleAttribute._End; i++)
            {
                if (RoleAttribute.HPDecrease == i || RoleAttribute.HPRegen == i)
                {
                    continue;
                }
                float value = this.AbilityData[i];
                float ratioValue = this.AbilityData.Ratio[(int)i];

                if (ratioValue != 0)
                {
                    if (this.AbilityData.RatioByBase)
                        this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[(int)i] = this.Owner.AllActorAttributes[(int)AttributeType.Base].Ratio[(int)i] * ratioValue;
                    else
                        this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[(int)i] = this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[(int)i] + ratioValue;
                }

                if (value != 0)
                {
                    this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[(int)i] = this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[(int)i] + value;
                }

#if BATTLE_LOG
                if (EngineGlobal.BattleLog && (value != 0 || ratioValue != 0))
                    NeptuneBattle.log("{0} Ability: {1}  {2}:{3:f6} {4:f6}  {5:f6}", this.Owner.FullName, this.AbilityData.Name, (RoleAttribute)i, this.Owner.AllAttributes[(int)AttributeType.Ability][(int)i], value, ratioValue);
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
            if (this.AbilityData.TransformAttributeFrom != RoleAttribute.None)
            {
                int TransformFrom = (int)this.AbilityData.TransformAttributeFrom;
                float transfromValue = 0.0f;
                if(this.AbilityData.FromIsRatio == true)
                {
                    transfromValue = UFloat.Round(this.Caster.AttributeFinalRatio[TransformFrom] * (this.AbilityData.TransformAttributeRatio * NeptuneConst.Hundredth));
                }
                else
                {
                    transfromValue = UFloat.Round(this.Caster.AttributeFinalValue[TransformFrom] * (this.AbilityData.TransformAttributeRatio * NeptuneConst.Hundredth));
                }
                transfromValue = this.AbilityData.TransformAttributeMaxValue > 0 ? Math.Min(transfromValue, this.AbilityData.TransformAttributeMaxValue) : transfromValue;
                int TransformTo = (int)this.AbilityData.TransformAttributeTo;
                if (this.AbilityData.ToIsRatio == true)
                {
                    this.Owner.AllActorAttributes[(int)AttributeType.Ability].Ratio[TransformTo] += transfromValue;
                }
                else
                {
                    this.Owner.AllActorAttributes[(int)AttributeType.Ability].Value[TransformTo] += transfromValue;
                }
            }
        }

        /// <summary>
        /// Set duration (Config Time).(seconds)
        /// </summary>
        /// <param name="time"></param>
        public void SetDuration(float time)
        {
            if (time < 0)
                this.Duration = float.PositiveInfinity;
            else
                this.Duration = UFloat.Round(time / 1000f);
        }

        public void ReBuild()
        {
            foreach (var controlEffect in ControlEffects)
            {
                this.InitEffect((ControlEffect)controlEffect.Key);
            }
        }

        public void InitEffect(ControlEffect effect)
        {
            if (this.Owner.AbilityEffects[effect])
                return;
            this.Owner.AbilityEffects[effect] = true;
            if (NeptuneConst.NestedAbilities.ContainsKey((int)effect))
            {
                foreach (ControlEffect eff in NeptuneConst.NestedAbilities[(int)effect])
                {
                    this.InitEffect(eff);
                }
            }
        }

        /// <summary>
        /// 删除事件
        /// </summary>
        public override void OnDelete()
        {
            this.Owner.RemoveExtraSkillByAbility(this);

            this.ClearAbilityEffect();

            if (this.TransformIndex > 0)
            {
                this.Owner.RemoveTransformation(this.TransformIndex);
            }
            else if (this.ModelID > 0)
            {
                this.Owner.RemoveModel(this.ModelID);
            }
            if (this.AbilityData.ModelScaleDuration > 0)
            {

                this.Owner.zoomIndex.Remove(this.ZoomIndex);
                this.Owner.StartZoom(this.AbilityData.ModelScale, this.AbilityData.ModelScaleDuration);
            }
            Ability talentcd = null;

            //删除时还原技能
            if (this.FromSkillID > 0 && this.ToSkillID > 0)
            {

                if (this.Owner.ChangeSkillStack.ContainsKey(this.FromSkillID) && this.Owner.ChangeSkillStack[this.FromSkillID].Contains(this.ChangeIndex))
                {
                    if (this.ChangeIndex == this.Owner.ChangeSkillStack[this.FromSkillID].Max())
                    {
                        talentcd = this;
                    }
                    this.Owner.ChangeSkillStack[this.FromSkillID].Remove(this.ChangeIndex);
                    //                 this.Owner.EnableSkills(this.FromSkillID, this.ToSkillID, false);
                    //                 if (this.Owner.ChangeSkillStack[this.FromSkillID].Count <= 0)
                    //                     this.Owner.EnableSkills(this.FromSkillID, this.FromSkillID, true);
                }
            }
            if (this.TransformIndex > 0 || (this.FromSkillID > 0 && this.ToSkillID > 0))
                this.Owner.InitChangeSkills(talentcd, this.TransformIndex > 0 ? 0 : this.FromSkillID);
            this.talentDamegeFactor(-1);
        }


        /// <summary>
        /// Ability移除事件处理
        /// </summary>
        public virtual void OnDestroyed()
        {
#if BATTLE_LOG
            if (EngineGlobal.BattleLog)
                NeptuneBattle.log("{0} lost ability:{1} {2:f6}", this.Owner.FullName, this.AbilityData.Name, this.Duration);
#endif
            isDestroy = true;

            //处理Ability移除时可触发的逻辑
            if (NeptuneBattle.Instance.Scene != null && (this.AbilityData.ShieldValue > 0 || this.AbilityData.ShieldRecharge > 0))
            {
                NeptuneBattle.Instance.Scene.PopupText(PopupType.ShieldFade, string.Empty, 0, this.Owner.Joint, this.Caster.Joint, false, this.Owner.Side, RoleAttribute.None, this.AbilityData);
            }
            if (this.Caster!=null && this.Caster.tActiveSkills != null)
            {
                for (int i = 0; i < this.Caster.tActiveSkills.Length; i++)
                {
                    Skill talent = this.Caster.tActiveSkills[i];
                    if (talent != null)
                    {
                        if (!talent.IsEnabled) continue;
                        if ((talent.Data.TriggerType & TriggerType.Ability) > 0 && talent.Data.TriggerParam1 == (int)TriggerAbilityType.End && talent.Data.TriggerParam == this.AbilityData.ID)
                        {
                            if (talent.Data.TargetType == TargetType.Self)
                            {
                                talent.Target = this.Caster;
                            }
                            else if (talent.Data.TargetType == TargetType.Target)
                            {
                                talent.Target = this.Owner;
                            }
                            else
                            {
                                talent.Target = null;
                            }
#if BATTLE_LOG
                        if (EngineGlobal.BattleLog)
                            NeptuneBattle.log("{0} lost ability:{1} Trigger {2}  {3}", this.Owner.FullName, this.AbilityData.Name, talent.ToString(), talent.Target == null ? "null" : talent.Target.FullName);
#endif
                            if (talent.CanUse(talent.Target, true, true) == ResultType.Success)
                            {
                                if (!talent.Data.Instant)
                                {
                                    if (this.Caster.CurrentSkill != null && this.Caster.CurrentSkill.Casting)
                                    {
                                        this.Caster.CurrentSkill.End();
                                    }
                                }

                                talent.Start(talent.Target);
                            }
                        }
                    }
                }
            }

            //吸收护盾返还伤害
            if (this.AbilityData.ShieldClass == ShieldClass.Absorption && this.TotalDamage > 0)
            {
                float damage = this.TotalDamage * this.AbilityData.ShieldRestorationRatio * 0.01f;
                if (this.AbilityData.UnDeath && damage > this.Owner.HP)
                {
                    damage = this.Owner.HP - 1;
                }
                this.Owner.SetHP(this.Owner.HP - damage);
#if BATTLE_LOG
                if (EngineGlobal.BattleLog)
                    NeptuneBattle.log("    {0} Abiltiy ShieldRestoration {1} {2:F6} -{3:F6} TDMG:{4:F6} SRR:{5:F6}", this.Owner.FullName, this.AbilityData.Name, this.Owner.HP, damage, this.TotalDamage, this.AbilityData.ShieldRestorationRatio);
#endif
                if (this.Owner.HP == 0)
                    this.Owner.End(this.Caster);
                this.Owner.OnDamaged(InjuryType.AttackDamage, damage, RoleAttribute.MaxHP, false, this.Caster);
            }
            if (!string.IsNullOrEmpty(this.AbilityData.EndEffect))
            {
                if (this.Owner.Joint != null)
                    this.Owner.Joint.AddEffect(this.AbilityData.EndEffect, EffectType.Normal, NeptuneConst.Vector3Zero);
            }
            //删除Ability
            this.Delete();
        }

        /// <summary>
        /// 伤害事件处理
        /// </summary>
        /// <param name="injury"></param>
        /// <param name="injuryType"></param>
        /// <returns></returns>
        public virtual float OnInjury(float injury, InjuryType injuryType, Skill fromSkill)
        {
            if (this.AbilityData.Value.MPDmgRecovery > 0 && injury > 0)
            {//受伤回复MP
                this.Owner.Remedy(this.AbilityData.Value.MPDmgRecovery, RoleAttribute.MaxMP, this.Owner);
            }

            float finalDmg = injury;
            if (fromSkill == null || ((this.AbilityData.ShieldAbsorbType == ShieldAbsorbType.All //任何攻击
                    || this.AbilityData.ShieldAbsorbType == ShieldAbsorbType.Skill && (fromSkill.Data.SkillType == SkillType.Skill || fromSkill.Data.SkillType == SkillType.ChildSkill)  //技能
                    || this.AbilityData.ShieldAbsorbType == ShieldAbsorbType.Normal && fromSkill.Data.SkillType == SkillType.Normal)))//普攻
            {
                ShieldType shieldtype = this.AbilityData.ShieldType;
                if ((this.AbilityData.ShieldType == ShieldType.All || (int)shieldtype == (int)injuryType))
                {
                    bool trigger = false;
                    if (this.AbilityData.ShieldClass == ShieldClass.Measure)
                    {//计量护盾处理
                        if (this.Protection > 0)
                        {
                            this.Protection = this.Protection - finalDmg;

                            if (this.Protection < 0)
                            {
                                trigger = true;
                                this.Owner.RemoveAbility(this);
                                finalDmg = -this.Protection;

                            }
                            else
                            {
                                PopDamageText(shieldtype);
                                finalDmg = 0;
                            }
                        }
                    }
                    else if (this.AbilityData.ShieldClass == ShieldClass.Rechargeable)
                    {//计数护盾处理
                        if (finalDmg <= 0)
                        {
                            if (this.Recharge >= this.ShieldRecharge)
                            {
                                trigger = true;
                                this.Owner.RemoveAbility(this);
                            }

                            finalDmg = 0;
                        }
                        else if (this.Protection > 0)
                        {
                            this.Protection = this.Protection - finalDmg;
                            float dmg = this.Protection;
                            this.Recharge = this.Recharge + 1;

                            this.Protection = this.AbilityData.ShieldValue;
                            if (this.AbilityData.BaseRatio > 0)
                            {
                                this.Protection = this.Protection + this.shieldAdd;
                            }
                            if (this.Recharge >= this.ShieldRecharge)
                            {
                                trigger = true;
                                this.Owner.RemoveAbility(this);
                            }

                            if (dmg < 0)
                                finalDmg = -dmg;
                            else
                                finalDmg = 0;
                            PopDamageText(shieldtype);
                            if (this.Owner.Joint != null && this.Owner.Joint.Controller != null)
                                this.Owner.Joint.SetEffectLayer(this.EffectID, this.Recharge - 1);
                        }
                    }
                    else if (this.AbilityData.ShieldClass == ShieldClass.Deathless)
                    {//不死护盾处理
                        if (this.Protection > 0)
                        {
                            float remain = this.Owner.HP - 1;
                            if (finalDmg > remain)
                            {
                                float quits = finalDmg - remain;
                                if (this.ShieldRecharge > 0)
                                {
                                    if (this.Recharge >= this.ShieldRecharge)
                                    {
                                        trigger = true;
                                        this.Owner.RemoveAbility(this);
                                    }
                                    else
                                    {
                                        this.Recharge = this.Recharge + 1;
                                        if (this.Owner.Joint != null && this.Owner.Joint.Controller != null)
                                            this.Owner.Joint.SetEffectLayer(this.EffectID, this.Recharge - 1);
                                    }
                                }
                                else
                                {
                                    if (quits < this.Protection)
                                    {
                                        this.Protection = this.Protection - quits;
                                        if (NeptuneBattle.Instance.Scene != null && this.Owner.Joint != null)
                                        {
                                            NeptuneBattle.Instance.Scene.PopupText(PopupType.Funeral, string.Empty, 0, this.Owner.Joint, this.Caster.Joint, false, this.Owner.Side);
                                        }
                                    }
                                    else
                                    {
                                        quits = this.Protection;
                                        this.Protection = 0;
                                        trigger = true;
                                        this.Owner.RemoveAbility(this);
                                    }
                                }

                                finalDmg = finalDmg - quits;
                            }
                        }
                    }
                    else if (this.AbilityData.ShieldClass == ShieldClass.Absorption)
                    {//吸收返回护盾
                     //吸收百分比
                        if (this.ProtectionRatio > 0)
                        {
                            TotalDamage += finalDmg * this.ProtectionRatio * 0.01f;
                            finalDmg *= (1 - this.ProtectionRatio * 0.01f);
                        }
                        //吸收固定值
                        if (this.Protection > 0)
                        {
                            if (this.Protection - finalDmg < 0)
                            {
                                finalDmg -= this.Protection;
                                TotalDamage += this.Protection;
                            }
                            else
                            {
                                TotalDamage += finalDmg;
                                finalDmg = 0;
                            }
                        }
                        PopDamageText(shieldtype);
                    }
                    else if (this.AbilityData.ShieldClass == ShieldClass.DamageReduce)
                    {
                        finalDmg = injury * Math.Max(0, (1 - this.Protection));
                    }
                    if (trigger)
                        this.Owner.OnBrokeShield(this.AbilityData.ID);
                }
            }


            float quitsDmg = injury - finalDmg;
            if (quitsDmg > 0 && this.AbilityData.ShieldToHP > 0)
            {
                //护盾转化生命
                this.Caster.Remedy(quitsDmg * this.AbilityData.ShieldToHP);
                if (this.Caster.Joint != null)
                    this.Caster.Joint.AddEffect(this.AbilityData.HitEffect, EffectType.Normal, NeptuneConst.Vector3Zero);
            }
            return finalDmg;
        }

        private void PopDamageText(ShieldType shieldtype)
        {
            if (NeptuneBattle.Instance.Scene != null && this.Owner.Joint != null)
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
                //NeptuneBattle.Instance.Scene.PopupText(type, string.Empty, this.Owner.Joint, this.Caster.Joint, false, this.Owner.Side);//护盾免疫不需要冒字
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
                foreach (var effect in this.ControlEffects)
                {
                    if (NeptuneConst.ConflictingAbilities.ContainsKey(effect.Key) && NeptuneConst.ConflictingAbilities[effect.Key])
                    {
                        if (NeptuneConst.ConflictingAbilities[effect.Key])
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
                if (this.ControlEffects.ContainsKey((int)ControlEffect.Unaffected))
                {
                    result = true;
                }
                return result;
            }
        }

        public void CheckTransferAbility(Actor attacker)
        {
            if (attacker == null || attacker.IsDead)
            {
                return;
            }
            if (this.AbilityData.TransferTargetType == TransferTargetType.Attacker)
            {
                if (attacker != Owner && CheckSide(attacker))
                {
                    if ((this.AbilityData.TargetRoleType & attacker.Data.RoleType) > 0 || this.AbilityData.TargetRoleType == RoleType.Any)
                    {
                        TransferAbility(attacker);
                    }
                }
            }
            else if (this.AbilityData.TransferTargetType == TransferTargetType.FinallyHitPlayer)
            {
                if ((this.AbilityData.TargetRoleType & attacker.Data.RoleType & RoleType.Hero) == RoleType.Hero)
                {
                    Player deathPlayer = this.Owner.Player;
                    Player attackerPlayer = attacker.Player;
                    //死亡为非英雄 击杀者为英雄
                    if (deathPlayer == null && attackerPlayer != null && CheckSide(attacker))
                    {
                        TransferAbility(attacker);
                    }
                    else
                    {
                        if (attackerPlayer == null && deathPlayer != null)
                        {
                            if (CheckSide(this.Owner.FinalKiller))
                                TransferAbility(this.Owner.FinalKiller);
                        }
                        else if (attackerPlayer != null && deathPlayer != null)
                        {//玩家被玩家直接杀死
                            if (CheckSide(attacker))
                                TransferAbility(attacker);
                        }
                    }
                }
            }
            else if (this.AbilityData.TransferTargetType == TransferTargetType.AttackerSide)
            {
                if (attacker != null && attacker != Owner && CheckSide(attacker))
                {
                    if ((this.AbilityData.TargetRoleType & attacker.Data.RoleType) > 0 || this.AbilityData.TargetRoleType == RoleType.Any)
                    {
                        SafeList<Actor> roles = NeptuneBattle.Instance.AliveRoles[(int)attacker.Side];
                        for (int i = 0; i < roles.Count; i++)
                        {
                            if ((this.AbilityData.TargetRoleType & roles[i].Data.RoleType) > 0)
                                TransferAbility(roles[i]);
                        }
                    }

                }
            }
        }

        public void AbilityAttrPlus(int count)
        {
            for (RoleAttribute i = 0; i < RoleAttribute.MAX; i++)
            {
                this.AbilityData[i] *= count;
                this.AbilityData.Ratio[(int)i] *= count;
            }
        }
        public void TransferAbility(Actor role)
        {
            if (role == null || role.IsDead)
                return;
            AbilityData ability_info = NeptuneBattle.Instance.DataProvider.GetAbilityData(this.AbilityData.TransferAbilityID);
            if (ability_info != null)
            {
                role.AddAbility(ability_info.Clone(), this.Caster, null);
            }
            else
            {
                role.AddAbility(this.AbilityData, this.Caster, null);
            }
        }

        /// <summary>
        /// 检查是否满足阵营条件
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool CheckSide(Actor role)
        {
            if (role == null)
                return false;
            if (this.AbilityData.TargetSide == RelativeSide.Both || this.AbilityData.TargetSide == this.Caster.GetRelation(role))
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
            if (this.AbilityData.ShieldClass == ShieldClass.Rechargeable)
                this.ShieldRecharge = shieldrecharge;
            if (this.AbilityData.ShieldClass == ShieldClass.Measure)
                this.Protection = shieldvalue;
        }
    }
}