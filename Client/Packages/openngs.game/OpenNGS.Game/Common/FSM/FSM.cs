using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS
{
    public class FSM
    {
        public IFSMTimer timer;

        public void Init(IFSMTimer timer)
        {
            this.timer = timer;
        }

        // 当前状态
        FSMState mCurState;

        // 状态集
        Dictionary<int, FSMState> StateDict = new Dictionary<int, FSMState>();

        // 定义委托类型
        public delegate int FSMCallFunc();

        // 检查状态是否存在
        bool HasState(int state)
        {
            return StateDict.ContainsKey(state);
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state">State.</param>
        public bool AddState(int state, FSMCallFunc enterFunc = null, FSMCallFunc leaveFunc = null, FSMCallFunc updateFunc = null)
        {
            if (HasState(state) == true)
            {
                Debug.LogErrorFormat("FSM AddState state exit {0}", state);
                return false;
            }

            FSMState s = new FSMState(this, state, enterFunc, leaveFunc, updateFunc);
            StateDict[state] = s;
            return true;
        }

        /// <summary>
        /// 添加跳转，在多个状态下，对同一事件的处理
        /// </summary>
        /// <param name="translation">Translation.</param>
        public bool addEvent(int[] fromState, int toState, int EvendID, FSMCallFunc eventFunc, int retCode)
        {
            foreach (int state in fromState)
            {
                if (addEvent(state, toState, EvendID, eventFunc, retCode) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 添加跳转，根据事件处理函数返回值跳转到不同状态
        /// </summary>
        /// <param name="translation">Translation.</param>
        public bool addEvent(int fromState, int toState, int EvendID, FSMCallFunc eventFunc, int retCode)
        {
            // 检查状态
            if (HasState(fromState) == false)                
            {
                Debug.LogErrorFormat("FSM addEvent state not exit {0}", fromState);
                return false;
            }

            if (HasState(toState) == false)
            {
                Debug.LogErrorFormat("FSM addEvent state not exit {0}", toState);
                return false;
            }

            // 检查事件
            StateDict[fromState].AddEvent(EvendID, eventFunc, retCode, toState);
            return true;
        }

        /// <summary>
        /// 添加定时器，类似添加事件，根据定时器函数处理返回值跳转到不同状态
        /// 目前仅支持一个定时器，若有多个需求，1. 在状态的update函数中判断，2. 自己添加定时器
        /// </summary>
        /// <param name="state">State.</param>
        public bool AddTimer(int fromState, int toState, FP interval, FSMCallFunc timerFunc, int retCode)
        {
            // 检查状态
            if (HasState(fromState) == false)
            {
                Debug.LogErrorFormat("FSM AddTimer state not exit {0}", fromState);
                return false;
            }

            if (HasState(toState) == false)
            {
                Debug.LogErrorFormat("FSM AddTimer state not exit {0}", toState);
                return false;
            }

            // 设置定时器
            return StateDict[fromState].AddTimer(interval, timerFunc, retCode, toState);            
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="state">State.</param>
        public bool Start(int state)
        {
            // 检查状态
            if (HasState(state) == false)
            {
                Debug.LogErrorFormat("FSM Start state no exit {0}", state);
                return false;
            }

            mCurState = StateDict[state];
            mCurState.OnEnter();
            return true;
        }
        /// <summary>
        /// 关闭状态机
        /// </summary>
        public bool Destroy()
        {
			if (mCurState != null)
			{
				mCurState.OnLeave();
			}
			mCurState = null;
            
            foreach (var State in StateDict)
            {
                if (State.Value != null)
                {
                    State.Value.Destroy();
                }
            }
            StateDict.Clear();
            timer = null;
            return true;
        }
        /// <summary>
        /// 外部模块通知FSM事件，不要在FSM内部调用此接口，FSM内部应该通过addEvent中的retcode
        /// 比如外部模块抛出A事件，FSM某状态处理后，产生一个事件，这个事件再抛给FSM的目的是什么呢？
        /// 这个内部事件，应该接着写处理逻辑，如果需要跳转到S1状态，返回code1，需要跳转到S2状态，返回code2
        /// </summary>
        public void OnEvent(int eventID)
        {
            mCurState?.OnEvent(eventID);
        }

        public void Update()
        {
            mCurState?.OnUpdate();
        }

        /// <summary>
        /// 状态跳转
        /// </summary>
        /// <param name="name">Name.</param>
        public void Translate(int newState)
        {
            if (HasState(newState) == false)
            {
                Debug.LogErrorFormat("FSM Translate state not exit {0}", newState);
                return;
            }
			
			//老状态的离开事件
			mCurState?.OnLeave();
            
            // 设置新状态
            mCurState = StateDict[newState];

            Debug.LogWarning($"FSM Translate newState {newState}");

            //新状态的进入事件
            mCurState?.OnEnter();
        }   

        /// <summary>
        /// 输出当前状态
        /// </summary>
        public void LogCurState()
        {
            mCurState?.LogState();
        }
    }

    class FSMTest
    {
        static public void Test()
        {
            FSM fsm = new FSM();
            // 3个状态，A,B,C
            fsm.AddState((int)state.A, A_enter);
            fsm.AddState((int)state.B, leaveFunc: B_leave);
            fsm.AddState((int)state.C, updateFunc: C_update);
            // A收到X事件，可以跳到B，C状态
            fsm.addEvent((int)state.A, (int)state.B, (int)Event.X, A_EventX, 0);
            fsm.addEvent((int)state.A, (int)state.C, (int)Event.X, A_EventX, 1);
            // b收到Y事件，可以跳到C状态
            fsm.addEvent((int)state.B, (int)state.C, (int)Event.Y, B_EventY, 0);

            // 状态机启动
            fsm.Start((int)state.A);

            // A状态下只会处理X事件
            fsm.OnEvent((int)Event.Y);
            fsm.OnEvent((int)Event.X);

            // 进入B状态后处理Y事件
            fsm.OnEvent((int)Event.Y);
        }

        enum state
        {
            A,
            B,
            C,
        };

        enum Event
        {
            X,
            Y,
        };

        static int A_enter()
        {
            Console.WriteLine("A_enter");
            return 0;
        }
        static int B_leave()
        {
            Console.WriteLine("B_leave");
            return 0;
        }
        static int C_update()
        {
            Console.WriteLine("C_update");
            return 0;
        }
        static int A_EventX()
        {
            Console.WriteLine("A_EventX");
            return 0;
        }
        static int B_EventY()
        {
            Console.WriteLine("B_EventY");
            return 0;
        }
    }
}



