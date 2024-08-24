using System;
using System.Collections;
using UnityEngine;

namespace OpenNGS
{
    public static class Wait
    {
        public enum WaitMode
        {
            No = 0,
            Fps30 = 1,
            Fps60 = 2,
            Fps72 = 3,
            Fps90 = 4,
            Fps120 = 5,
        }
        public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// 0 : No wait
        /// 1 : Wait for 30fps
        /// 2 : Wait for 60fps
        /// </summary>
        public static WaitMode mode = WaitMode.No;

        private static float s_last = 0f;
        private static float[] peroid = new float[] { 1f, 0.02f, 0.01f, 0.0083f, 0.0067f, 0.005f };

        public static bool shouldWaitNextFrame
        {
            get
            {
                float t = UnityEngine.Time.realtimeSinceStartup;
                if (t - s_last < peroid[(int)mode])
                    return false;
                else
                {
                    s_last = t;
                    return true;
                }
            }
        }

        public static WaitForSeconds WaitForHalfSeconds = new WaitForSeconds(0.5f); 
        public static WaitForSeconds WaitForSeconds1 = new WaitForSeconds(1f);
        public static WaitForSeconds WaitForSeconds2 = new WaitForSeconds(2f);
        public static WaitForSeconds WaitForSeconds3 = new WaitForSeconds(3f);
        public static WaitForSeconds WaitForSeconds5 = new WaitForSeconds(5f);
    }
}
