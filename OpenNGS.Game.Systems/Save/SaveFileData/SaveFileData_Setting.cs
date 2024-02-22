using Dynamic.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[global::ProtoBuf.ProtoContract()]
public class SaveFileData_Setting :ISaveInfo
{
    [global::ProtoBuf.ProtoMember(1)]
    public VerticalSynchronizationData _vertical;
    public Dictionary<string, AudioSettinData> _audio;
    public Dictionary<string, KeyControlSettingData> _keyControl;
    public Dictionary<string, LanguageData> _language;
    public ResolutionRatiosData resolution;

    public SaveFileData_Setting() 
    {
        _vertical = new VerticalSynchronizationData();
        _audio = new Dictionary<string, AudioSettinData>();
        _keyControl = new Dictionary<string, KeyControlSettingData>();
        _language = new Dictionary<string, LanguageData>();
        resolution = new ResolutionRatiosData();
    }
}
