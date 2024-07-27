using UnityEngine;

namespace OpenNGS.UI
{
    public static class MessageBox
    {

        public static void ShowMessage(string message)
        {
            Debug.LogFormat(message);
        }
    }
}
