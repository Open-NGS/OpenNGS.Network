/********************************************************************
	created:	2013/11/14
	created:	14:11:2013   17:58
	filename: 	E:\Alice\UnityProj\Assets\Scripts\Common\MissQScheduler.cs
	file path:	E:\Alice\UnityProj\Assets\Scripts\Common
	file base:	MissQScheduler
	file ext:	cs
	author:		benzhou
	
	purpose:	调度器
*********************************************************************/

using OpenNGS;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenNGS.Common;

/// <summary>
/// 调度器
/// </summary>
// ReSharper disable once CheckNamespace
public class Scheduler : MonoSingleton<Scheduler>, IFSMTimer
{
    public interface ITimerCallbackHandler
    {
        void OnTimer(uint timerId);
    }

    /// <summary>
    /// 更新委托
    /// </summary>
    public delegate void UpdateDelegate();
    public delegate void LateUpdateDelegate();
    public delegate void FixedUpdateDelegate();

    /// <summary>
    /// 协同委托
    /// </summary>
    public delegate IEnumerator CoroutineDelegate();

    /// <summary>
    /// 定时委托
    /// </summary>
    public delegate void TimerFrameDelegate();

    /// <summary>
    /// 协同数据
    /// </summary>
    public class CoroutineData
    {
        // 是否销毁
        public bool Destroyed;

        // 接收器
        public CoroutineDelegate Handler;

        // 枚举
        public IEnumerator Enumerator;
    }

    /// <summary>
    /// 定时数据
    /// </summary>
    public class TimerData
    {
        /// <summary>
        /// 毫秒 间隔
        /// </summary>
        public int Interval;
        /// <summary>
        /// 毫秒 剩余时间
        /// </summary>
        public int RemainTime;

        // 是否重复
        public bool Repeat;

        // 是否销毁
        public bool Destroyed;

        // 定时接收器
        public TimerFrameDelegate TimerHandler;

        // 定时接口接收器
        public uint TimerId;
        public ITimerCallbackHandler TimerCallbackHandler;
        public string name;
    }

    /// <summary>
    /// 定帧数据
    /// </summary>
    public class FrameData
    {
        // 间隔
        public uint Interval;

        // 剩余帧数
        public uint RemainFrame;

        // 是否重复
        public bool Repeat;

        // 是否销毁
        public bool Destroyed;

        // 定帧接收器
        public TimerFrameDelegate FrameHandler;
    }

    private const int CoroutineInitializeCount = 10;
    private const int TimerInitializeCount = 1000;
    private const int RealTimerInitializeCount = 10;
    private const int FrameinitializeCount = 500;

    // 更新接收器
    private event UpdateDelegate UpdateHandler;
    private event LateUpdateDelegate LateUpdateHandler;
    private event FixedUpdateDelegate FixedUpdateHandler;
    // 协同数据
    private readonly PoolLinkList<CoroutineData> _coroutineData = new PoolLinkList<CoroutineData>(CoroutineInitializeCount);

    // 定时数据
    private readonly PoolLinkList<TimerData> _timerData = new PoolLinkList<TimerData>(TimerInitializeCount);
    private readonly PoolLinkList<TimerData> _realTimerData = new PoolLinkList<TimerData>(RealTimerInitializeCount);

    private uint _globalNextTimerId = 0;

    // 定帧数据
    private readonly PoolLinkList<FrameData> _frameData = new PoolLinkList<FrameData>(FrameinitializeCount);

	public void UnInit()
	{
		UpdateHandler = null;
		LateUpdateHandler = null;
		FixedUpdateHandler = null;
		_coroutineData.Clear();
		_timerData.Clear();
		_frameData.Clear();
	}

    /// <summary>
    /// 添加更新委托
    /// </summary>
    /// <param name="d">更新委托</param>
    public void AddUpdate(UpdateDelegate d)
    {
        UpdateHandler += d;
    }

    /// <summary>
    /// 移除更新委托
    /// </summary>
    /// <param name="d">更新委托</param>
    public void RemoveUpdate(UpdateDelegate d)
    {
        UpdateHandler -= d;
    }
    /// <summary>
    /// 添加更新委托
    /// </summary>
    /// <param name="d">更新委托</param>
    public void AddFixedUpdate(FixedUpdateDelegate d)
    {
        FixedUpdateHandler += d;
    }

    /// <summary>
    /// 移除更新委托
    /// </summary>
    /// <param name="d">更新委托</param>
    public void RemoveFixedUpdate(FixedUpdateDelegate d)
    {
        FixedUpdateHandler -= d;
    }
    public void AddLateUpdate(LateUpdateDelegate d)
    {
        LateUpdateHandler += d;
    }

    public void RemoveLateUpdate(LateUpdateDelegate d)
    {
        LateUpdateHandler -= d;
    }

	void SafeInvoke(TimerFrameDelegate d)
	{
		if (d != null)
		{
			try
			{
				d.Invoke();
			}
			catch (Exception e)
			{
                NgDebug.LogError(e);
			}
		}
	}

	void SafeInvoke(ITimerCallbackHandler timerCallback, uint timerId)
	{
		if (timerCallback != null)
		{
			try
			{
				timerCallback.OnTimer(timerId);
			}
			catch (Exception e)
			{
				NgDebug.LogError(e);
			}
		}
	}

	/// <summary>
	/// 添加协同委托
	/// </summary>
	/// <param name="d">协同委托</param>
	public LinkedListNode<CoroutineData> AddCoroutine(CoroutineDelegate d)
    {
        if (d == null)
        {
            return null;
        }

        var data = _coroutineData.Alloc();
        if (data == null)
        {
            return null;
        }
        data.Value.Destroyed = false;
        data.Value.Handler = d;
        data.Value.Enumerator = null;
        _coroutineData.AddLast(data);

        return data;
    }

    /// <summary>
    /// 移除协同委托
    /// </summary>
    /// <param name="d">协同委托</param>
    public void RemoveCoroutine(CoroutineDelegate d)
    {
        var item = _coroutineData.First;
        while (item != null)
        {
            if (item.Value.Handler == d)
            {
                item.Value.Destroyed = true;
            }

            item = item.Next;
        }
    }

    /// <summary>
    /// 添加定时
    /// </summary>
    /// <param name="interval">间隔时间</param>
    /// <param name="repeat">是否重复</param>
    /// <param name="d">定时委托</param>
    public LinkedListNode<TimerData> AddTimer(float interval, bool repeat, TimerFrameDelegate d)
    {
        if (interval < 0.0f || d == null)
        {
            return null;
        }

        var data = _timerData.Alloc();
        if (data == null)
        {
            return null;
        }
        data.Value.Interval = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.RemainTime = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.Repeat = repeat;
        data.Value.Destroyed = false;
        data.Value.TimerHandler = d;
        data.Value.TimerId = GetNextTimerId();
        data.Value.TimerCallbackHandler = null;
        data.Value.name = name;
        _timerData.AddLast(data);
        return data;
    }

    /// <summary>
    /// 添加定时
    /// </summary>
    /// <param name="interval">间隔时间</param>
    /// <param name="repeat">是否重复</param>
    /// <param name="d">定时委托</param>
    public LinkedListNode<TimerData> AddTimerAndInvokeOnce(float interval, bool repeat, TimerFrameDelegate d)
    {
        if (interval < 0.0f || d == null)
        {
            return null;
        }

        var data = _timerData.Alloc();
        if (data == null)
        {
            return null;
        }
        data.Value.Interval = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.RemainTime = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.Repeat = repeat;
        data.Value.Destroyed = false;
        data.Value.TimerHandler = d;
        data.Value.TimerId = GetNextTimerId();
        data.Value.TimerCallbackHandler = null;
        data.Value.name = name;
        _timerData.AddLast(data);

		SafeInvoke(d);
		
        return data;
    }

    public LinkedListNode<TimerData> AddTimer(float interval, bool repeat, ITimerCallbackHandler handler)
    {
        if (interval < 0.0f || handler == null)
        {
            return null;
        }

        var data = _timerData.Alloc();
        if (data == null)
        {
            return null;
        }
        data.Value.Interval = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.RemainTime = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.Repeat = repeat;
        data.Value.Destroyed = false;
        data.Value.TimerHandler = null;
        data.Value.TimerCallbackHandler = handler;
        data.Value.TimerId = GetNextTimerId();
        _timerData.AddLast(data);

        return data;
    }

    public uint AddRealTimer(float interval, bool repeat, TimerFrameDelegate d)
    {
        if (interval < 0.0f || d == null)
        {
            return 0;
        }

        var data = _realTimerData.Alloc();
        if (data == null)
        {
            return 0;
        }
        data.Value.Interval = (int)(interval * BaseConst.CONSTUnitThousand);
        data.Value.RemainTime = (int)((Time.RealtimeSinceStartup + interval) * BaseConst.CONSTUnitThousand);
        data.Value.Repeat = repeat;
        data.Value.Destroyed = false;
        data.Value.TimerHandler = d;
        data.Value.TimerId = GetNextTimerId();
        data.Value.TimerCallbackHandler = null;

        _realTimerData.AddLast(data);
        return data.Value.TimerId;
    }

    /// <summary>
    /// 移除定时 (可以同时移除多个定时器，慢！）
    /// </summary>
    /// <param name="d">定时委托</param>
    public void RemoveTimer(TimerFrameDelegate d)
    {
        var item = _timerData.First;
        while (item != null)
        {
            if (item.Value.TimerHandler != null && item.Value.TimerHandler.Equals(d))
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }

    public void RemoveTimer(ITimerCallbackHandler handler, uint timerId)
    {
        var item = _timerData.First;
        while (item != null)
        {
            if (item.Value.TimerCallbackHandler != null && item.Value.TimerCallbackHandler.Equals(handler) && item.Value.TimerId == timerId)
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }

    public void RemoveTimer(uint timerId)
    {
        var item = _timerData.First;
        while (item != null)
        {
            if (item.Value.TimerId == timerId)
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }

    public void RemoveRealTimer(uint timerId)
    {
        var item = _realTimerData.First;
        while (item != null)
        {
            if (item.Value.TimerId == timerId)
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }

    public void RemoveRealTimer(TimerFrameDelegate d)
    {
        var item = _realTimerData.First;
        while (item != null)
        {
            if (item.Value.TimerHandler != null && item.Value.TimerHandler.Equals(d))
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }

    /// <summary>
    ///     返回当前Timer
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public TimerData GetTimer(TimerFrameDelegate d)
    {
        var item = _timerData.First;
        while (item != null)
        {
            if (item.Value.TimerHandler != null && item.Value.TimerHandler.Equals(d))
            {
                return item.Value;
            }
            item = item.Next;
        }

        return null;
    }

    /// <summary>
    ///     返回当前定帧
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public FrameData GetFrame(TimerFrameDelegate d)
    {
        var item = _frameData.First;
        while (item != null)
        {
            if (item.Value.FrameHandler != null && item.Value.FrameHandler.Equals(d))
            {
                return item.Value;
            }
            item = item.Next;
        }

        return null;
    }

    /// <summary>
    /// 添加定帧
    /// </summary>
    /// <param name="interval">间隔帧数</param>
    /// <param name="repeat">是否重复</param>
    /// <param name="d">定帧委托</param>
    public LinkedListNode<FrameData> AddFrame(uint interval, bool repeat, TimerFrameDelegate d)
    {
        if (interval == 0 || d == null)
        {
            return null;
        }

        var data = _frameData.Alloc();
        if (data == null)
        {
            return null;
        }
        data.Value.Interval = interval;
        data.Value.RemainFrame = interval;
        data.Value.Repeat = repeat;
        data.Value.Destroyed = false;
        data.Value.FrameHandler = d;
        _frameData.AddLast(data);
        return data;
    }
	
    /// <summary>
    /// 移除定帧
    /// </summary>
    /// <param name="d">定帧委托</param>
    public void RemoveFrame(TimerFrameDelegate d)
    {
        var item = _frameData.First;
        while (item != null)
        {
            if (item.Value.FrameHandler.Equals(d))
            {
                item.Value.Destroyed = true;
            }
            item = item.Next;
        }
    }
    public void UpdateInfo()
    {
        // 协同
        var corutineData = _coroutineData.First;
        while (corutineData != null)
        {
            if (corutineData.Value == null)
            {
                NgDebug.LogError("corutineData.Value is null");
                break;
            }
            
            if (corutineData.Value.Destroyed)
            {
                corutineData.Value.Handler = null;
                corutineData.Value.Enumerator = null;

                var next = corutineData.Next;
                _coroutineData.Remove(corutineData);

                corutineData = next;
                continue;
            }

            if (corutineData.Value.Enumerator == null)
            {
                corutineData.Value.Enumerator = corutineData.Value.Handler();
            }

            if (!corutineData.Value.Enumerator.MoveNext())
            {
                corutineData.Value.Destroyed = true;
            }

            corutineData = corutineData.Next;
        }

        // 定时
        var deltaTime = (int)(UnityEngine.Time.fixedDeltaTime * BaseConst.CONSTUnitThousand);

        var timerData = _timerData.First;
        while (timerData != null)
        {
            if (timerData.Value == null)
            {
                NgDebug.LogError("timerData.Value is null");
                break;
            }
            
            if (timerData.Value.Destroyed)
            {
                timerData.Value.TimerHandler = null;
                timerData.Value.TimerCallbackHandler = null;
                timerData.Value.TimerId = 0;
                var next = timerData.Next;
                _timerData.Remove(timerData);

                timerData = next;
                continue;
            }

            timerData.Value.RemainTime -= deltaTime;
            if (timerData.Value.RemainTime > 0)
            {
                timerData = timerData.Next;
                continue;
            }

			SafeInvoke(timerData.Value.TimerHandler);
			SafeInvoke(timerData.Value.TimerCallbackHandler, timerData.Value.TimerId);
			
            if (timerData.Value.Repeat)
            {
                timerData.Value.RemainTime += timerData.Value.Interval;
            }
            else
            {
                timerData.Value.Destroyed = true;
            }

            timerData = timerData.Next;
        }

        //
        float realtimeSinceStartup = Time.RealtimeSinceStartup;

        timerData = _realTimerData.First;
        while (timerData != null)
        {
            if (timerData.Value.Destroyed)
            {
                timerData.Value.TimerHandler = null;
                timerData.Value.TimerCallbackHandler = null;
                timerData.Value.TimerId = 0;
                var next = timerData.Next;
                _realTimerData.Remove(timerData);

                timerData = next;
                continue;
            }

            if ((realtimeSinceStartup * BaseConst.CONSTUnitThousand) < timerData.Value.RemainTime)
            {
                timerData = timerData.Next;
                continue;
            }

			SafeInvoke(timerData.Value.TimerHandler);
			SafeInvoke(timerData.Value.TimerCallbackHandler, timerData.Value.TimerId);
			
            if (timerData.Value.Repeat)
            {
                timerData.Value.RemainTime += timerData.Value.Interval;
            }
            else
            {
                timerData.Value.Destroyed = true;
            }

            timerData = timerData.Next;
        }

        // 定帧
        var frameData = _frameData.First;
        while (frameData != null)
        {
            if (frameData.Value.Destroyed)
            {
                frameData.Value.FrameHandler = null;

                var next = frameData.Next;
                _frameData.Remove(frameData);

                frameData = next;

                continue;
            }

            --frameData.Value.RemainFrame;
            if (frameData.Value.RemainFrame > 0)
            {
                frameData = frameData.Next;
                continue;
            }

			SafeInvoke(frameData.Value.FrameHandler);
			
            if (frameData.Value.Repeat)
            {
                frameData.Value.RemainFrame = frameData.Value.Interval;
            }
            else
            {
                frameData.Value.Destroyed = true;
            }

            frameData = frameData.Next;
        }
    }
    void FixedUpdate()
    {
        if (null != FixedUpdateHandler)
        {
            FixedUpdateHandler();
        }

        UpdateInfo();
    }
    // MonoBehaviour
    protected void Update()
    {
        // 更新
        if (null != UpdateHandler)
        {
            UpdateHandler();
        }


    }

    protected void LateUpdate()
    {
        // 更新
        if (null != LateUpdateHandler)
        {
            LateUpdateHandler();
        }
    }

    private uint GetNextTimerId()
    {
        _globalNextTimerId++;

        if (_globalNextTimerId == 0)
        {
            _globalNextTimerId = 1;
        }

        return _globalNextTimerId;
    }

    public int FsmAddTimer(FP interval, bool repeat, Action callback)
    {
        uint timerId = AddRealTimer(interval.AsFloat(), repeat, ()=> callback());
        return (int)timerId;
    }

    public void FsmRemoveTimer(int timerId)
    {
        RemoveRealTimer((uint)timerId);
    }
}
