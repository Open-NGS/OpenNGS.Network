using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICreditsElement : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject content;
    public void SetTitle(string content, TMP_FontAsset _font)
    {
        if (title != null)
        {
            title.text = content;
            if (_font != null)
            {
                title.font = _font;
            }
        }
    }
}
