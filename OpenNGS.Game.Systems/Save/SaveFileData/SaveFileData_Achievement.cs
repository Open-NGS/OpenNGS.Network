using OpenNGS.Achievement.Common;
using OpenNGS.Character.Common;
using System.Collections.Generic;

[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Achievement : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public Dictionary<uint, AchievementInfo> DicAchievement;

    public SaveFileData_Achievement()
    {
        DicAchievement = new Dictionary<uint, AchievementInfo>();
    }
}