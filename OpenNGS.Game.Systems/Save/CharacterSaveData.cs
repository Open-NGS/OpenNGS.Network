using OpenNGS.Rank.Data;

[global::ProtoBuf.ProtoContract()]
public class CharacterSaveData : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public CharacterInfoArray characterInfoArray;

    public CharacterSaveData()
    {
        characterInfoArray = new CharacterInfoArray();
    }
}