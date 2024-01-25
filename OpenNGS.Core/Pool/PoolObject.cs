using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS.Pool
{
    public interface IPoolObject : IDisposable
    {
        void Create();
        void Initialize();
        void Clear();
    }

    public interface IPoolObject<T> : IPoolObject
    {
    }

    public class PoolObject<T> : IPoolObject  where T : IPoolObject, new()
    {
        public static T New()
        {
            return PoolManager.New<T>();
        }

        public static void InitPool(int size)
        {
            PoolManager.InitPool<T>(size);
        }

        public void Dispose()
        {
            PoolManager.Delete<T>(this);
        }

        public void Delete()
        {
            PoolManager.Delete<T>(this);
        }

        public static void Delete(T obj)
        {
            PoolManager.Delete<T>(obj);
        }

        public static void DeleteAll()
        {
            PoolManager.DeleteAll<T>();
        }

        public virtual void Create() { }

        public virtual void Initialize(){ }

        public virtual void Clear() { }
    }
}
