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

        public GameObject SelectTab(int index)
        {
            for (int i = 0; i < tabPages.Length; i++)
            {
                return tabPages[index];
            }
            return null;
        }

        public void CloseSelectTab()
        {
            for (int i = 0; i < tabPages.Length; i++)
            {
                tabPages[i].SetActive(false);
            }
        }
    }
}
