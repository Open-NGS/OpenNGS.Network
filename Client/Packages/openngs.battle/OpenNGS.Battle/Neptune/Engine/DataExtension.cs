using Neptune.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Vector2Extend
{
    public static string ToString(this Vector2 val,int precision)
    {
        return string.Format("({0:f" + precision + "}, {1:f" + precision + "}}", val.x);
    }
}
