
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenNGS
{
    /// <summary>
    /// 状态类
    /// </summary>
    class FSMState
    {
        FSM fsm;
        int state;
        FSM.FSMCallFunc enterFunc;
        FSM.FSMCallFunc leaveFunc;
        FSM.FSMCallFunc updateFunc;
        Dictionary<int, FSMEvent> EventDict = new Dictionary<int, FSMEvent>();
        FSMTimer timer = new FSMTimer { interval = 0,};

        public FSMState(FSM fsm, int state, FSM.FSMCallFunc enterFunc, FSM.FSMCallFunc leaveFunc, FSM.FSMCallFunc updateFunc)
        {
            this.fsm = fsm;
            this.state = state;
            this.enterFunc = enterFunc;
            this.leaveFunc = leaveFunc;
            this.updateFunc = updateFunc;
        }

        /// <summary>
        /// 事件处理类
        /// </summary>
        class FSMEvent
        {
            public int eventID;
            public FSM.FSMCallFunc eventFunc;
            // 根据事件处理的不同返回值，跳转到不同状态
            public Dictionary<int, int> StateDict = new Dictionary<int, int>();
        }

        /// <summary>
        /// 定时器处理类
        /// </summary>
        class FSMTimer
        {
            public FP interval = 0;
            public FSM.FSMCallFunc timerFunc = null;
            // 根据定时器处理的不同返回值，跳转到不同状态
            public Dictionary<int, int> StateDict = new Dictionary<int, int>();
            public int timerId;
        }

        public void AddEvent(int EvendID, FSM.FSMCallFunc eventFunc, int retCode, int toState)
        {
            if (EventDict.ContainsKey(EvendID) == false)
            {
                FSMEvent evt = new FSMEvent()
                {
                    eventID = EvendID,
                    eventFunc = eventFunc,
                };
                EventDict[EvendID] = evt;
            }
            FSMEvent e = EventDict[EvendID];
            e.StateDict[retCode] = toState;
        }

        public bool AddTimer(FP interval, FSM.FSMCallFunc timerFunc, int retCode, int toState)
        {
            // interval相同时可以添加不同retCode的toState
            if (timer.interval != 0 && timer.interval != interval)
            {
                NgDebug.LogErrorFormat("FSMState AddTimer interval exit {0}", state);
                return false;
            }

            timer.interval = interval;
            timer.timerFunc = timerFunc;
            timer.StateDict[retCode] = toState;
            return true;
        }

        public void OnEnter()
        {
            if (enterFunc != null)
            {
                enterFunc.Invoke();
            }

            if (timer.interval != 0)
            {
                if (fsm.timer == null)
                {
                    NgDebug.LogErrorFormat("FSM timer is null");
                    return;
                }

                timer.timerId = fsm.timer.FsmAddTimer(timer.interval, false, OnTimer);
            }
        }

        public void OnLeave()
        {
            if (leaveFunc != null)
            {
                leaveFunc.Invoke();
            }

            if (timer.interval != 0)
            {
                if (fsm.timer == null)
                {
                    NgDebug.LogErrorFormat("FSM timer is null");
                    return;
                }

                fsm.timer.FsmRemoveTimer(timer.timerId);
            }
        }
        public void Destroy()
        {
            this.enterFunc = null;
            this.leaveFunc = null;
            this.updateFunc = null;

            EventDict.Clear();
        }
        public void OnUpdate()
        {
            if (updateFunc != null)
            {
                updateFunc.Invoke();
            }
        }

        public void OnEvent(int eventID)
        {
            if (EventDict.ContainsKey(eventID) == false)
            {
                return;
            }
        
            FSMEvent e = EventDict[eventID];
            int retCode = 0;
            if (e.eventFunc != null)
            {
                retCode = e.eventFunc();
            }

            if (e.StateDict.ContainsKey(retCode) == true)
            {
                fsm.Translate(e.StateDict[retCode]);
            }
        }

        void OnTimer()
        {
            int retCode = 0;
            if (timer.timerFunc != null)
            {
                retCode = timer.timerFunc();
            }

            if (timer.StateDict.ContainsKey(retCode) == true)
            {
                fsm.Translate(timer.StateDict[retCode]);
            }
        }

        public void LogState()
        {
            NgDebug.LogFormat("FSM state {0}", state);
        }
    }
}

