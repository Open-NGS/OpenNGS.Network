using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenNGS.Events
{

    public class EventSystem : OpenNGS.Singleton<EventSystem>
    {

        Dictionary<int, Delegate> m_EventsGroup = new Dictionary<int, Delegate>();

        protected void OnDispose()
        {
            m_EventsGroup.Clear();

        }



        public void Subscribe(int EventID, Action handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp != null && temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to add invalid type of eventhandler");
                    return;
                }


                m_EventsGroup[EventID] = Delegate.Combine(temp, handler);

            }
            else
            {
                m_EventsGroup.Add(EventID, handler);
            }

        }

        public void Subscribe<T>(int EventID, Action<T> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp != null && temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to add invalid type of eventhandler");
                    return;
                }


                m_EventsGroup[EventID] = Delegate.Combine(temp, handler);

            }
            else
            {
                m_EventsGroup.Add(EventID, handler);
            }
        }

        public void Subscribe<T, U>(int EventID, Action<T, U> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp != null && temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to add invalid type of eventhandler");
                    return;
                }


                m_EventsGroup[EventID] = Delegate.Combine(temp, handler);

            }
            else
            {
                m_EventsGroup.Add(EventID, handler);
            }
        }

        public void Subscribe<T, U, V>(int EventID, Action<T, U, V> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp != null && temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to add invalid type of eventhandler");
                    return;
                }


                m_EventsGroup[EventID] = Delegate.Combine(temp, handler);

            }
            else
            {
                m_EventsGroup.Add(EventID, handler);
            }
        }

        public void Subscribe<T, U, V, W>(int EventID, Action<T, U, V, W> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp != null && temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to add invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Combine(temp, handler);


            }
            else
            {
                m_EventsGroup.Add(EventID, handler);
            }
        }

        public void Unsubscribe(int EventID, Action handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp == null)
                {
                    m_EventsGroup.Remove(EventID);
                    return;
                }

                if (temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to remove invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Remove(temp, handler);
            }
        }

        public void Unsubscribe<T>(int EventID, Action<T> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp == null)
                {
                    m_EventsGroup.Remove(EventID);
                    return;
                }

                if (temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to remove invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Remove(temp, handler);
            }
        }

        public void Unsubscribe<T, U>(int EventID, Action<T, U> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp == null)
                {
                    m_EventsGroup.Remove(EventID);
                    return;
                }

                if (temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to remove invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Remove(temp, handler);
            }
        }

        public void Unsubscribe<T, U, V>(int EventID, Action<T, U, V> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp == null)
                {
                    m_EventsGroup.Remove(EventID);
                    return;
                }

                if (temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to remove invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Remove(temp, handler);
            }
        }

        public void Unsubscribe<T, U, V, W>(int EventID, Action<T, U, V, W> handler)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                if (temp == null)
                {
                    m_EventsGroup.Remove(EventID);
                    return;
                }

                if (temp.GetType() != handler.GetType())
                {
                    Debug.LogError("Try to remove invalid type of eventhandler");
                    return;
                }

                m_EventsGroup[EventID] = Delegate.Remove(temp, handler);
            }
        }

        public void PostEvent(int EventID)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                (temp as Action)?.Invoke();
            }
        }

        public void PostEvent<T>(int EventID, T t)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                (temp as Action<T>)?.Invoke(t);
            }
        }

        public void PostEvent<T, U>(int EventID, T t, U u)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                (temp as Action<T, U>)?.Invoke(t, u);
            }
        }

        public void PostEvent<T, U, V>(int EventID, T t, U u, V v)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                (temp as Action<T, U, V>)?.Invoke(t, u, v);
            }
        }

        public void PostEvent<T, U, V, W>(int EventID, T t, U u, V v, W w)
        {
            Delegate temp;
            if (m_EventsGroup.TryGetValue(EventID, out temp))
            {
                (temp as Action<T, U, V, W>)?.Invoke(t, u, v, w);
            }
        }





    }
}
