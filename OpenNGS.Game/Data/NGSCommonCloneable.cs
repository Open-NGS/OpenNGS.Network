using System.Collections;
using System.Collections.Generic;


namespace OpenNGSCommon
{
    public partial class Queue
    {
        public Queue Clone()
        {
            Queue clone = new Queue()
            {
                Current = this.Current,
                Interval = this.Interval,
                Param = this.Param,
                QueueID = this.QueueID,
                QueueType = this.QueueType,
                RemainTime = this.RemainTime,
                StartTime = this.StartTime,
                Status = this.Status,
                Target = this.Target,
                UpdateType = this.UpdateType
            };
            if (this.Owner != null)
            {
                clone.Owner = new OwnerInfo()
                {
                    Id = this.Owner.Id,
                    Type = this.Owner.Type
                };
            }
            return clone;
        }
    }
    public partial class PlayerBaseInfo
    {
        public PlayerBaseInfo Clone()
        {
            PlayerBaseInfo clone = new PlayerBaseInfo()
            {
                uin = this.uin,
                nickname = this.nickname,
                guildid = this.guildid,
                guild_name = this.guild_name
            };
            return clone;
        }

        public void CopyFrom(PlayerBaseInfo other)
        {
            this.uin = other.uin;
            this.nickname = other.nickname;
            this.guildid = other.guildid;
            this.guild_name = other.guild_name;
        }
    }

}
