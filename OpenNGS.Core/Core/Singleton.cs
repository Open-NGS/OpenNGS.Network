using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNGS
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        public void Dispose()
        {
            if (instance != null)
            {
                instance = null;
            }
        }
    }


    public class AppSingleton<T> : IDisposable where T : AppSingleton<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.OnCreate();
                }
                return instance;
            }
        }

        public void Dispose()
        {
            if (instance != null)
            {
                instance.OnDispose();
                instance = null;
            }
        }

        protected virtual void OnCreate() { }

        protected virtual void OnDispose() { }
    }

}
