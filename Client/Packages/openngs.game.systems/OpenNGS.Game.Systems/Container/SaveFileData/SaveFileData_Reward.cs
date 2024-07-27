using OpenNGS.Reward.Data;
using System.Collections.Generic;


[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Reward : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<uint, RewardSaveData> DicReward;

    public SaveFileData_Reward()
    {
        DicReward = new Dictionary<uint, RewardSaveData>();
    }
}
