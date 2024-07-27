/*
 * Copyright (c) 2016 Ray Liu
 * 多协程管理组件
 * 用于维护多个协程的并行及顺序管理
 * 通过 CoroutineSequence 和 Coroutine 的自由组合实现复杂的异步流程管理
 * 多个 CoroutineSequence 可并行执行
 * 单个 CoroutineSequence 内的多个 Coroutine 顺序执行
 * 每个 Coroutine 包含一个协程
 * by Ray : ray@raymix.net
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OpenNGS
{
    /// <summary>
    /// RoutineSequence
    /// </summary>
    public class CoroutineSequence
    {
        private List<Coroutine> Routines = new List<Coroutine>();
        public string name;
        public bool isDone;

        public CoroutineSequence(string name)
        {
            this.name = name;
            this.isDone = false;
        }

        private void AddRoutine(Coroutine routine)
        {
            this.Routines.Add(routine);
        }

        public CoroutineSequence AddCoroutine(IEnumerator co)
        {
            AddRoutine(new Coroutine(this.name + "." + co.GetType().FullName, co));
            return this;
        }

        public IEnumerator Run()
        {
#if PROFILER
            Profiling.ProfilerLog.Start("NiCoroutineSequence(" + name + ")");
#endif
            foreach (Coroutine routine in Routines)
            {
                yield return routine.Run();
            }
#if PROFILER
            Profiling.ProfilerLog.End("NiCoroutineSequence(" + name + ")");
#endif
            this.isDone = true;
        }

        ~CoroutineSequence()
        {
            if (!this.isDone)
            {
#if PROFILER
                Profiling.ProfilerLog.End("NiCoroutineSequence(" + name + ")");
#endif
            }
        }
    }
}