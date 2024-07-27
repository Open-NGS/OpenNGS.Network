using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.Pool
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Stack<object>> Pools = new Dictionary<Type, Stack<object>>();

        public static T Get<T>()
        {
            Stack<object> stack = null;
            if (Pools.TryGetValue(typeof(T), out stack))
            {
                if (stack.Count > 0)
                {
                    T t = (T)stack.Pop();
                    return t;
                }
            }

            return (T)Activator.CreateInstance(typeof(T), true);
        }

        public static void Recycle<T>(T obj)
        {
            if (obj == null)
            {
                return;
            }

            Stack<object> stack = null;
            if (!Pools.TryGetValue(obj.GetType(), out stack))
            {
                stack = new Stack<object>();
                Pools.Add(obj.GetType(), stack);
            }

            //设置上限
            if (stack.Count > 100)
            {
                return;
            }

            stack.Push(obj);
        }

        public static void Clear()
        {
            Pools.Clear();
        }
    }
}
