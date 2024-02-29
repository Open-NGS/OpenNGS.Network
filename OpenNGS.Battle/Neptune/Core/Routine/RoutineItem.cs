/*
 * Copyright (c) 2016 Ray Liu
 * 多协程管理组件
 * 用于维护多个协程的并行及顺序管理
 * 通过 RoutineSequence 和 RoutineItem 的自由组合实现复杂的异步流程管理
 * 多个 RoutineSequence 可并行执行
 * 单个 RoutineSequence 内的多个 RoutineItem 顺序执行
 * 每个 RoutineItem 包含一个协程
 * by Ray : ray@raymix.net
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// RoutineItem
/// </summary>
public class RoutineItem
{
    private IEnumerator routine;
    public string name;
    public bool isDone;

    public RoutineItem(string name, IEnumerator routine)
    {
        this.name = name;
        this.isDone = false;
        this.routine = routine;
    }

    public IEnumerator Run()
    {
        EasyCounter.Instance.Start("RoutineItem:" + name);
        Logger.AddIndent();
        yield return routine;
        Logger.DecIndent();
        EasyCounter.Instance.End("RoutineItem:" + name);
        this.isDone = true;

    }

    ~RoutineItem()
    {
        if (!this.isDone)
        {
            Logger.DecIndent();
            EasyCounter.Instance.End("RoutineItem:" + name);
        }
    }
}
