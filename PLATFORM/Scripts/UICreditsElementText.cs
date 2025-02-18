using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICreditsElementText : MonoBehaviour
{
    public TextMeshProUGUI elementText;
    public void SetText(string content, TMP_FontAsset _font)
    {
        if (elementText != null)
        {
            elementText.text = content;
            if (_font != null)
            {
                elementText.font = _font;
            }
        }
    }
}
