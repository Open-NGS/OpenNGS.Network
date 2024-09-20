using Neptune;
using Neptune.GameData;

public class PassiveSkill : AbilityBase
{
    public TalentData Data;
    public TalentGroupData GroupData;
    public int Level;
    public BattleActor Source;

    public PassiveSkill()
    {

    }

    public PassiveSkill(TalentData data, TalentGroupData groupdata, BattleActor caster)
        : this(data, groupdata, caster, null, null)
    {
    }

    public PassiveSkill(TalentData data, TalentGroupData groupdata, BattleActor caster, BattleActor target, BattleActor source)
    {
        this.Caster = caster;
        this.Target = target;
        this.Source = source;
        this.Data = data.Clone();
        this.GroupData = groupdata;
    }
    public virtual void LevelUp(int level)
    {
        if (this.Level < level)
            this.Level = level;
        if (this.Level == level && this.Level >= 1)
        {
            TalentData talentData = this.Caster.GetTalentData(this.GroupData, this.Level, this.Caster.ID);
            if (talentData != null)
            {
                this.Data = talentData;
                this.Caster.RoleSkin.RoleSkinTalentReplace(this.Data);
            }
        }
    }

    public override void Delete()
    {
        NObjectPool<PassiveSkill>.Delete(this);
    }

    public override void OnDelete()
    {

    }

    public bool CheckTarget(BattleActor target)
    {
        if (this.Source == null)
            return true;
        if (this.Data.TargetType == TargetType.Target)
        {
            return this.Source == target;
        }
        else if (this.Data.TargetType == TargetType.Self)
        {
            return this.Source == this.Caster;
        }
        return true;
    }
}