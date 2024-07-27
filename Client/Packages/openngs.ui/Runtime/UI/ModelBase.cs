using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase<T1, T2> : NotificationObject<ViewModelBase, T2> where T1 : new() where T2 : Enum
{
    private static T1 _instance;
    public static T1 Instance => _instance ??= new T1();
}
