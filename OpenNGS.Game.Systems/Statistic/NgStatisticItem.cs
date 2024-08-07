using OpenNGS.Statistic.Common;
using OpenNGS.Statistic.Data;
class NgStatisticItem
{
    public StatData Config;

    public delegate void ValueChanged(uint statId, ulong value);

    public event ValueChanged OnValueChanged;

    public ulong Value = 0;

    public NgStatisticItem(StatData config)
    {
        this.Config = config;
    }

    internal bool Execute(STAT_EVENT @event, uint category, uint type, uint subType, uint objId, ulong value)
    {
        if (this.Config.StatEvent != @event) return false;
        if (this.Config.ObjCategory != 0 && this.Config.ObjCategory != category) return false;
        if (this.Config.ObjType != 0 && this.Config.ObjType != type) return false;
        if (this.Config.ObjSubType != 0 && this.Config.ObjSubType != subType) return false;
        if (this.Config.ObjID != 0 && this.Config.ObjID != objId) return false;

        switch (this.Config.StatType)
        {
            case STAT_TYPE.STAT_TYPE_CALCULATE_COUNT:
                this.Set(this.Value + 1);
                break;
            case STAT_TYPE.STAT_TYPE_CALCULATE_MAX:
                {
                    if (this.Value >= value) return false;
                    this.Set(value);
                }
                break;
            case STAT_TYPE.STAT_TYPE_CALCULATE_MIN:
                {
                    if (this.Value <= value) return false;
                    this.Set(value);
                }
                break;
            case STAT_TYPE.STAT_TYPE_CALCULATE_SUM:
                this.Set(this.Value += value);
                break;
            case STAT_TYPE.STAT_TYPE_CALCULATE_UPDATE:
                {
                    if (this.Value == value) return false;
                    this.Set(value);
                }
                break;
            default:
                return false;
        }
        return true;
    }

    public void Set(ulong value)
    {
        this.Value = value;
        if (OnValueChanged != null)
            this.OnValueChanged(this.Config.Id, value);
    }

    internal void Query()
    {
        //switch((QueryStatusType)this.Config.ObjCategory)
        //{
        //    case QueryStatusType.StunRatioInRoom:
        //        {
        //            this.Value = (int)(BattleManager.GetInstance().GetStunRatioInRoom() * 100);
        //        }
        //        break;
        //    case QueryStatusType.ObjectDestroyRatioInRoom:
        //        {
        //            this.Value = (int)(BattleManager.GetInstance().GetObjectDestroyRatioInRoom() * 100);
        //        }
        //        break;
        //    case QueryStatusType.MainJobId:
        //        {
        //            var player = EntityDataManager.GetInstance().GetPlayerData();
        //            if (player != null)
        //                this.Value = (int)player.GetMainJobId();
        //        }
        //        break;
        //}
    }
}
