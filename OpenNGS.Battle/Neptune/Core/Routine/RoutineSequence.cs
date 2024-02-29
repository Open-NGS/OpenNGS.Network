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
/// RoutineSequence
/// </summary>
public class RoutineSequence
{
    private List<RoutineItem> Routines = new List<RoutineItem>();
    public string name;
    public bool isDone;

    public RoutineSequence(string name)
    {
        this.name = name;
        this.isDone = false;
    }

    public void AddRoutine(RoutineItem routine)
    {
        this.Routines.Add(routine);
    }

    public IEnumerator Run()
    {
        EasyCounter.Instance.Start("RoutineSequence:" + name);
        Logger.AddIndent();
        foreach (RoutineItem routine in Routines)
        {
            yield return routine.Run();
        }
        Logger.DecIndent();
        EasyCounter.Instance.End("RoutineSequence:" + name);
        this.isDone = true;
    }

    ~RoutineSequence()
    {
        if (!this.isDone)
        {
            Logger.DecIndent();
            EasyCounter.Instance.End("RoutineSequence:" + name);
        }
    }
}