using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace OpenNGS.Network
{
   public class ProtoSerializer
    {
        public static void Serialize<T>(Stream destination, T instance)
        {
            try
            {
                global::ProtoBuf.Serializer.Serialize(destination, instance);
            }
            catch (InvalidOperationException e)
            {
                NgDebug.LogError("Serialize protocol fail " + e.ToString());
            }
        }

        public static object Deserialize(Type type, Stream source)
        {
            return global::ProtoBuf.Serializer.Deserialize(type, source);
        }

        public static T Deserialize<T>(Stream source)
        {
            try
            {
                T instance = global::ProtoBuf.Serializer.Deserialize<T>(source);
                return instance;
            }
            catch (InvalidOperationException e)
            {
                NgDebug.LogError("Deserialize protocol fail " + e.ToString());
                return default(T);
            }
        }

        public static T Merge<T>(Stream source, T instance)
        {
            try
            {
                return global::ProtoBuf.Serializer.Merge<T>(source, instance);
            }
            catch (InvalidOperationException e)
            {
                NgDebug.LogError("Merge protocol fail " + e.ToString());
                return default(T);
            }
        }
    }
}
