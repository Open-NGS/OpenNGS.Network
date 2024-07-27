using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OpenNGS.UI
{
    public class TabView : MonoBehaviour
    {

        public TabButton[] tabButtons;
        public GameObject[] tabPages;

        public UnityAction<int> OnTabSelect;

        public int defaultSelect = 0;
        private int index = -1;
        // Use this for initialization
        IEnumerator Start()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].tabView = this;
                tabButtons[i].tabIndex = i;
            }
            yield return new WaitForEndOfFrame();
            SelectTab(defaultSelect);
        }
        
        [ContextMenu("Init TabViews")]
        private void InitTabButtons()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].tabView = this;
                tabButtons[i].tabIndex = i;
            }
            
            SelectTab(defaultSelect);
        }


        public void SelectTab(int index)
        {
            if (this.index != index)
            {
                for (int i = 0; i < tabButtons.Length; i++)
                {
                    tabButtons[i].Select(i == index);
                    if (i < tabPages.Length)
                        tabPages[i].SetActive(i == index);
                }
                this.index = index;
                if (OnTabSelect != null)
                    OnTabSelect(index);
            }
        }
    }
}
