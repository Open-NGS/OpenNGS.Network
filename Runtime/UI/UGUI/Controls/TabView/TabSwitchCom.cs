using OpenNGS.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace OpenNGS.UI
{
    public class TabSwitchCom : MonoBehaviour
    {
        public GameObject[] tabPages;

        public void SelectTab<T>(int index, T[] uls) where T : ITabButton
        {
            for (int i = 0; i < uls.Length; i++)
            {
                if (uls != null)
                {
                    uls[i].Select(i == index);
                    if (i < tabPages.Length)
                        tabPages[i].SetActive(i == index);
                }
            }
        }
    }
}
