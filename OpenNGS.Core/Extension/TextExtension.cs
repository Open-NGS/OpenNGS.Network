using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextExtension
{
    public static void CopyToClipboard(this string s)
    {
        TextEditor te = new TextEditor {text = s};
        te.SelectAll();
        te.Copy();
        
        Debug.Log($"{nameof(CopyToClipboard)} {s}");
    }
}