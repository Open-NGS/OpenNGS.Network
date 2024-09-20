using System;
using UnityEngine;
using System.Collections.Generic;


public class NavLayer
{
    public static INavMap NavMap;

    public static void LoadNavGrid( byte[] data, INavMap navMap = null)
    {
        if (navMap == null)
        {
            NavMap = new NavGrid();
        }
        else
        {
            NavMap = navMap;
        }
        NavMap.LoadData(data);
    }
}