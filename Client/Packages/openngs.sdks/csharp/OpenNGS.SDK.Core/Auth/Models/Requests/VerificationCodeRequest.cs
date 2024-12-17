using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Auth.Models.Requests
{
    public class VerificationCodeRequest
    {
        public VerificationType Type;
        public string Dest { get; set; } = "";
        public string Code { get; set; } = "";

    }
}
