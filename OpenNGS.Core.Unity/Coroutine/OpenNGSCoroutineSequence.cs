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
    /// RoutineSequence
    /// </summary>
    public class OpenNGSCoroutineSequence
    {
        private List<OpenNGSCoroutine> Routines = new List<OpenNGSCoroutine>();
        public string name;
        public bool isDone;

        public OpenNGSCoroutineSequence(string name)
        {
            this.name = name;
            this.isDone = false;
        }

        private void AddRoutine(OpenNGSCoroutine routine)
        {
            this.Routines.Add(routine);
        }

        public OpenNGSCoroutineSequence AddCoroutine(IEnumerator co)
        {
            AddRoutine(new OpenNGSCoroutine(this.name + "." + co.GetType().FullName, co));
            return this;
        }

        public IEnumerator Run()
        {
#if PROFILER
            Profiling.ProfilerLog.Start("OpenNGSCoroutineSequence(" + name + ")");
#endif
            foreach (OpenNGSCoroutine routine in Routines)
            {
                yield return routine.Run();
            }
#if PROFILER
            Profiling.ProfilerLog.End("OpenNGSCoroutineSequence(" + name + ")");
#endif
            this.isDone = true;
        }

        ~OpenNGSCoroutineSequence()
        {
            if (!this.isDone)
            {
#if PROFILER
                Profiling.ProfilerLog.End("OpenNGSCoroutineSequence(" + name + ")");
#endif
            }
        }
    }
}