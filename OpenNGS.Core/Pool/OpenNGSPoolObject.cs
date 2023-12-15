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

    public class OpenNGSPoolObject<T> : IPoolObject  where T : IPoolObject, new()
    {
        public static T New()
        {
            return OpenNGSPoolManager.New<T>();
        }

        public static void InitPool(int size)
        {
            OpenNGSPoolManager.InitPool<T>(size);
        }

        public void Dispose()
        {
            OpenNGSPoolManager.Delete<T>(this);
        }

        public void Delete()
        {
            OpenNGSPoolManager.Delete<T>(this);
        }

        public static void Delete(T obj)
        {
            OpenNGSPoolManager.Delete<T>(obj);
        }

        public static void DeleteAll()
        {
            OpenNGSPoolManager.DeleteAll<T>();
        }

        public virtual void Create() { }

        public virtual void Initialize(){ }

        public virtual void Clear() { }
    }
}
