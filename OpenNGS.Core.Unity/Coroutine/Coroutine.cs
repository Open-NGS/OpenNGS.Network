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
    /// RoutineItem
    /// </summary>
    internal class Coroutine
    {
        private IEnumerator routine;
        public string name;
        public bool isDone;

        public Coroutine(string name, IEnumerator routine)
        {
            this.name = name;
            this.isDone = false;
            this.routine = routine;
        }

        public IEnumerator Run()
        {
#if PROFILER
            Profiling.ProfilerLog.Start("NgCoroutine", name);
#endif
            yield return routine;
#if PROFILER
            Profiling.ProfilerLog.End("NgCoroutine", name);
#endif
            this.isDone = true;

        }

        ~Coroutine()
        {
            if (!this.isDone)
            {
#if PROFILER
                Profiling.ProfilerLog.End("RoutineItem", name);
#endif
            }
        }
    }
}