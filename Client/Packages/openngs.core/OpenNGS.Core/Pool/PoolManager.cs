using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Pool
{
    public class PoolManager
    {
        public class ObjectPool<T> where T : IPoolObject
        {
            protected Queue<T> availableObjects;
            protected HashSet<T> usedObjects;

            public ObjectPool(int size)
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

            public void Use(T obj)
            {
                usedObjects.Add(obj);
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

        protected static Dictionary<Type, ObjectPool<IPoolObject>> Pools = new Dictionary<Type, ObjectPool<IPoolObject>>();
        protected static int initSize = 10;
        public static T New<T>() where T : IPoolObject, new()
        {
#if UNITY_EDITOR
            OpenNGS.Profiling.Profiler.BeginSample("ObjectPool.New");
#endif
            ObjectPool<IPoolObject> pool;
            T obj = default(T);

            if (Pools.TryGetValue(typeof(T), out pool))
            {
                obj = (T)pool.GetObject();
            }
            else
            {
                pool = new ObjectPool<IPoolObject>(initSize);
                Pools.Add(typeof(T), pool);
            }
            if (obj == null)
            {
                obj = new T();
                obj.Create();
                pool.Use(obj);
            }
            obj.Initialize();
#if UNITY_EDITOR
            OpenNGS.Profiling.Profiler.EndSample();
#endif
            return obj;
        }

        public static void InitPool<T>(int size) where T : IPoolObject, new()
        {
            initSize = size;
            ObjectPool<IPoolObject> pool;
            if (!Pools.TryGetValue(typeof(T), out pool))
            {
                pool = new ObjectPool<IPoolObject>(size);
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
            ObjectPool<IPoolObject> pool;
            if (Pools.TryGetValue(typeof(T), out pool))
            {
                obj.Clear();
                pool.Recycle(obj);
            }
        }


        public static void DeleteAll<T>() where T : IPoolObject, new()
        {
            ObjectPool<IPoolObject> pool;
            if (Pools.TryGetValue(typeof(T), out pool))
            {
                pool.RecycleAll();
            }
        }

    }
}
