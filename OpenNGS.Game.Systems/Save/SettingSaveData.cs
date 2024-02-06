using OpenNGS.Setting.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[global::ProtoBuf.ProtoContract()]
public class SettingSaveData: ISaveInfo
{
        [global::ProtoBuf.ProtoMember(1)]
        public List<AudioSettingInfo> AudioInfo;
        [global::ProtoBuf.ProtoMember(2)]
        public List<KeyControlSettingInfo> KeyControlInfo;
        [global::ProtoBuf.ProtoMember(3)]
        public FramesInfo FramesInfo;
        [global::ProtoBuf.ProtoMember(4)]
        public Language Language;
        [global::ProtoBuf.ProtoMember(5)]
        public OpenNGS.Setting.Common.RESOLUTIONRATION_TYPE Resoulution;
}
