using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Auth.Models.Requests
{
    public class LoginRequest : BaseRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public LoginRequest(string username, string password, string appId, string sign, string nonce) : base(appId, sign, nonce)
        {
            this.username = username;
            this.password = password;
        }
    }
}
