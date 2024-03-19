using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenNGS.Network
{
    public class ProtoPool : OpenNGS.Singleton<ProtoPool>
    {
        public Dictionary<int, Tuple<int, Type, Stack<IProtoExtension>>> ObjectPoolDict = new Dictionary<int, Tuple<int, Type, Stack<IProtoExtension>>>();
        public void RegisterTypes(Assembly assembly)
        {
            Type InterfaceType = typeof(IProtoExtension);
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && InterfaceType.IsAssignableFrom(type))
                {
                    FieldInfo info = type.GetField("ClassID", BindingFlags.Static | BindingFlags.Public);
                    //int classid = (int)info.GetValue(null);
                    int classid = type.GetHashCode();
                    #if DEBUG_LOG
                    NgDebug.LogFormat("ProtoPool.RegisterTypes:{0}:{1}", classid, type.Name);
                    #endif
                    Tuple<int, Type, Stack<IProtoExtension>> poolinfo = new Tuple<int, Type, Stack<IProtoExtension>>(classid, type, new Stack<IProtoExtension>());
                    ObjectPoolDict.Add(classid, poolinfo);
                }

            }
        }

        public void Clear()
        {
            foreach (var pool in ObjectPoolDict)
            {
                pool.Value.Item3.Clear();
            }

            ObjectPoolDict.Clear();

        }

        public bool Release(IProtoExtension obj)
        {
            if (obj == null)
                return false;

            int classid = obj.GetType().GetHashCode();
            #if DEBUG_LOG
            NgDebug.LogFormat("ProtoPool.Release:{0}:{1}", classid, obj.GetType().Name);
            #endif
            if (ObjectPoolDict.TryGetValue(classid, out Tuple<int, Type, Stack<IProtoExtension>> objectinfo))
            {
                obj.OnRelease();
                objectinfo.Item3.Push(obj);
                return true;
            }
            return false;

        }

        public IProtoExtension Get(Type type)
        {
            int classid = type.GetHashCode();
            #if DEBUG_LOG
            NgDebug.LogFormat("ProtoPool.Get:{0}:{1}", classid, type.Name);
            #endif
            if (ObjectPoolDict.TryGetValue(classid, out Tuple<int, Type, Stack<IProtoExtension>> objectinfo))
            {
                IProtoExtension obj;
                if (objectinfo.Item3.Count > 0)
                {
                    obj = objectinfo.Item3.Pop();
                    obj.OnSpawn();

                    return obj;
                }

                obj = (IProtoExtension)Activator.CreateInstance(objectinfo.Item2);
                obj.OnSpawn();

                return obj;
            }
            return null;


        }
    }
}
