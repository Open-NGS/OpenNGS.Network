using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenNGS.SDK.Auth.Models
{
    public class UserInfo
    {
        [JsonProperty("sub")]
        public string OpenID { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }


        [JsonProperty("name")]
        public string NickName { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }

    public class EegamesUserInfo
    {
        public string AuthId { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public string Eeid { get; set; }
    }
}
