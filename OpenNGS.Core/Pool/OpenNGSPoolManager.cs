using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Pool
{
    public class OpenNGSPoolManager
    {
        public class NObjectPool<T> where T : IPoolObject
        {
            Queue<T> availableObjects;
            HashSet<T> usedObjects;

            public NObjectPool(int size)
            {
                availableObjects = new Queue<T>(size);
                usedObjects = new HashSet<T>();
            }

            public T GetObject()
            {
                if(availableObjects.Count >0)
                {
                    T obj = availableObjects.Dequeue();
                    usedObjects.Add(obj);
                    return obj;
                }
                return default(T);
            }

            public void Recycle(T obj)
            {
                availableObjects.Enqueue(obj);
                usedObjects.Remove(obj);
            }
            public void RecycleAll()
            {
                foreach (T obj in usedObjects)
                {
                    obj.Clear();
                    availableObjects.Enqueue(obj);
                }
                usedObjects.Clear();
            }
        }

        static Dictionary<Type, NObjectPool<IPoolObject>> Pools = new Dictionary<Type, NObjectPool<IPoolObject>>();
        static int initSize = 10;
        public static T New<T>() where T : IPoolObject, new()
        {
            //OpenNGS.Profiling.Profiler.BeginSample("NObjectPool.New");
            NObjectPool<IPoolObject> pool;
            T obj = default(T);

            if (Pools.TryGetValue(typeof(T), out pool))
            {
                obj = (T)pool.GetObject();
            }
            else
            {
                pool = new NObjectPool<IPoolObject>(initSize);
                Pools.Add(typeof(T), pool);
            }
            if (obj == null)
            {
                obj = new T();
                obj.Create();
            }
            obj.Initialize();

            OpenNGS.Profiling.Profiler.EndSample();
            return obj;
        }

        public static void InitPool<T>(int size) where T : IPoolObject, new()
        {
            initSize = size;
            NObjectPool<IPoolObject> pool;
            if (!Pools.TryGetValue(typeof(T), out pool))
            {
                pool = new NObjectPool<IPoolObject>(size);
                Pools.Add(typeof(T), pool);
            }
            for (int i = 0; i < size; i++)
            {
                T obj = new T();
                obj.Create();
                pool.Recycle(obj);
            }
        }

        public static void Delete<T>(IPoolObject obj) where T : IPoolObject, new()
        {
            NObjectPool<IPoolObject> pool;
            if (Pools.TryGetValue(typeof(T), out pool))
            {
                obj.Clear();
                pool.Recycle(obj);
            }
        }


        public static void DeleteAll<T>() where T : IPoolObject, new()
        {
            NObjectPool<IPoolObject> pool;
            if (Pools.TryGetValue(typeof(T), out pool))
            {
                pool.RecycleAll();
            }
        }
    }
}
