/*
 * Copyright (c) 2016 Ray Liu
 * 多协程管理组件
 * 用于维护多个协程的并行及顺序管理
 * 通过 OpenNGSCoroutineSequence 和 OpenNGSCoroutine 的自由组合实现复杂的异步流程管理
 * 多个 OpenNGSCoroutineSequence 可并行执行
 * 单个 OpenNGSCoroutineSequence 内的多个 OpenNGSCoroutine 顺序执行
 * 每个 OpenNGSCoroutine 包含一个协程
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
    public class OpenNGSCoroutine
    {
        private IEnumerator routine;
        public string name;
        public bool isDone;

        public OpenNGSCoroutine(string name, IEnumerator routine)
        {
            this.name = name;
            this.isDone = false;
            this.routine = routine;
        }

        public IEnumerator Run()
        {
#if PROFILER
            Profiling.ProfilerLog.Start("OpenNGSCoroutine", name);
#endif
            yield return routine;
#if PROFILER
            Profiling.ProfilerLog.End("OpenNGSCoroutine", name);
#endif
            this.isDone = true;

        }

        ~OpenNGSCoroutine()
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