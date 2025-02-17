using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICreditsElementText : MonoBehaviour
{
    public TextMeshProUGUI elementText;
    public void SetText(string content)
    {
        if (elementText != null)
        {
            elementText.text = content;
        }
    }
}
