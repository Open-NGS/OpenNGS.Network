using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenNGS.SDK.Auth
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
}
