using OpenNGS.Character.Common;

[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Character : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public CharacterInfoArray characterInfoArray;

    public SaveFileData_Character()
    {
        characterInfoArray = new CharacterInfoArray();
    }
}