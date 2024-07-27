using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.SaveData
{
    public enum SaveDataResult
    {
        Success = 0,
        Recovered = 1,
        NotFound = 2,
        IOError = 3,
        InvalidData = 4,
        VerifyFailed = 5,
    }
}
