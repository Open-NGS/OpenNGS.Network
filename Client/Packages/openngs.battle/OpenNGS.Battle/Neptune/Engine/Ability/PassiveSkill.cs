using Neptune.Datas;

namespace Neptune

{
    public class PassiveSkill : AbilityBase
    {
        public SkillData Data;
        public SkillGroupData GroupData;
        public int Level;
        public Actor Source;

        public PassiveSkill()
        {

        }

        public PassiveSkill(SkillData data, SkillGroupData groupdata, Actor caster)
            : this(data, groupdata, caster, null, null)
        {
        }

        public PassiveSkill(SkillData data, SkillGroupData groupdata, Actor caster, Actor target, Actor source)
        {
            this.Caster = caster;
            this.Target = target;
            this.Source = source;
            this.Data = data;
            this.GroupData = groupdata;
        }
        public virtual void LevelUp(int level)
        {
            if (this.Level < level)
                this.Level = level;
            if (this.Level == level && this.Level >= 1)
            {
                SkillData talentData = this.Caster.GetSkillData(this.GroupData, this.Level, this.Caster.ID);
                if (talentData != null)
                {
                    this.Data = talentData;
                    this.Caster.RoleSkin.RoleSkinSkillReplace(this.Data);
                }
            }
        }

        public override void Delete()
        {
            ObjectPool<PassiveSkill>.Delete(this);
        }

        public override void OnDelete()
        {

        }

        public bool CheckTarget(Actor target)
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
}