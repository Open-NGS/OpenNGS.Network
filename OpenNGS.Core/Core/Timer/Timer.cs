using OpenNGS.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenNGS.Timer
{
    public enum TimerType
    {
        OneShot,
        Period
    }

    public class Timer
    {
        public float delay;
        public Action action;
        public bool oneShot;
        public float triggerTime;
        public static Timer Start(float delay, Action action, bool oneShot, string owner = null)
        {
            Timer timer = new Timer()
            {
                delay = delay,
                action = action,
                oneShot = oneShot,
                triggerTime = UnityEngine.Time.time,
            };
            TimerManager.AddTimer(timer, owner);
            return null;
        }
        public static void ClearTimers(string owner = null)
        {
            TimerManager.ClearTimers(owner);
        }

        public static void ClearAll()
        {
            TimerManager.ClearAllTimers();
        }
    }

    class TimerManager : MonoBehaviour
    {
        static Dictionary<string, NList<Timer>> Timers = new Dictionary<string, NList<Timer>>();

        private static TimerManager instance = null;
        public static void AddTimer(Timer timer, string owner = null)
        {
            string timerOwner = owner == null ? "default" : owner;
            if (!Timers.ContainsKey(timerOwner))
                Timers[timerOwner] = new NList<Timer>();
            Timers[timerOwner].Add(timer);
            if (instance == null)
            {
                GameObject obj = new GameObject("TimerManager");
                obj.hideFlags = HideFlags.HideAndDontSave;
                instance = obj.AddComponent<TimerManager>();
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void FixedUpdate()
        {
            if(Timers.Count >0)
            {
                foreach (var kv in Timers)
                {
                    foreach (var timer in kv.Value)
                    {
                        if (timer.triggerTime + timer.delay < UnityEngine.Time.time)
                        {
                            if (timer.action != null)
                                timer.action();
                            if (timer.oneShot)
                                kv.Value.Remove(timer);
                            else
                                timer.triggerTime = UnityEngine.Time.time;
                        }
                    }
                }
            }
        }

        public static void ClearTimers(string owner = null)
        {
            string timerOwner = owner == null ? "default" : owner;
            if (Timers.ContainsKey(timerOwner))
                Timers[timerOwner].Clear();
        }

        public static void ClearAllTimers()
        {
            Timers.Clear();
        }
    }
}
