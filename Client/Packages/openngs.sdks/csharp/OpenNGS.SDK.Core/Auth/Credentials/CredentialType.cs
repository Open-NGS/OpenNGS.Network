using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.SDK.Auth.Credentials
{
    public enum CredentialType : uint
    {
        None,
        Generic,
        DomainPassword,
        DomainCertificate,
        DomainVisiblePassword
    }
}
