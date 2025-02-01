using OpenNGS.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Platform.PS5
{

    public class PS5User : IUserProvider
    {
        public PLATFORM_MODULE Module => PLATFORM_MODULE.USER;

        public long GetUserID()
        {
            if(PSUser.activeUser==null)
            {
                return -1;
            }
            return PSUser.activeUser.loggedInUser.userId;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void Update()
        {
            
        }
    }
}