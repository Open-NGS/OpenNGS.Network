using OpenNGS.Character.Common;

[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Character : ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public CharacterInfo characterInfoArray;

    public SaveFileData_Character()
    {
        characterInfoArray = new CharacterInfo();
    }
}