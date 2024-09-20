
using System.Diagnostics;

namespace Neptune
{
    public class NObjectPool<T> where T : PoolObj, new()
    {
        public static int Capacity;
        public static int Count;
        static int maxIndex = 0;
        static T[] elements;

        public static int MaxObject
        {
            get { return maxIndex; }
        }

        public NObjectPool()
        {

        }


        public static void Init(int size)
        {
            Capacity = size;
            elements = new T[size];
            for (int i = 0; i < size; i++)
            {
                elements[i] = new T();
                elements[i].poolIdx = i;
            }
            Count = 0;
        }

        public static T New()
        {
            Count++;
            T obj;
            if (maxIndex < Capacity)
            {
                obj = elements[maxIndex++];
                obj.IsNull = false;
                return obj;
            }

            for (int i = 0; i < Capacity; i++)
            {
                obj = elements[i];
                if (obj.IsNull)
                {
                    obj.IsNull = false;
                    return obj;
                }
            }
            Count--;
            //if (typeof(T) == typeof(Trap)) Debug.LogWarning(typeof(NObjectPool<T>) + " is Full");
            obj = new T();
            obj.IsNull = false;
            return obj;
        }

        /// <summary>
        /// 删除一个元素
        /// 注意：
        /// 只能且必需在 PoolObj 对象的 Delete 重载中调用
        /// 由 PoolObj 派生的一切子类及所有下级子类均需重载 Delete 方法
        /// </summary>
        /// <param name="obj"></param>
        public static void Delete(T obj)
        {
            obj.OnDelete();
            if (obj.poolIdx == -1 || obj.IsNull)
                return;
            obj.IsNull = true;

            //if (typeof(T) == typeof(Trap)) Debug.LogFormat("{0} Delete {1}", typeof(ObjectPool<T>), obj.poolIdx );
            Count--;
            if (obj.poolIdx == maxIndex - 1)
            {
                while (maxIndex > 0)
                {
                    maxIndex--;
                    if (maxIndex > 0 && !elements[maxIndex - 1].IsNull)
                        break;
                }
            }
        }
    }
    public abstract class PoolObj
    {
        public bool IsNull = true;
        public int poolIdx = -1;
        /// <summary>
        /// 元素删除通知
        /// Delete时发生
        /// </summary>
        public abstract void OnDelete();

        /// <summary>
        /// 删除元素接口
        /// 此类派生的一切子类及所有下级子类均需重载 Delete 方法
        /// </summary>
        public abstract void Delete();
    }
}