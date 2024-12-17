using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Auth.Models.Requests
{
    public class UsernamePasswordRequest
    {
        [JsonProperty("username")]
        public string Username;

        [JsonProperty("password")]
        public string Password;
    }
}
