using System;
using System.Collections.Generic;
using OpenNGS.Serialization;
using OpenNGSCommon;
using ProtoBuf;

namespace OpenNGSCommon
{
    public partial class StatusData
    {
        public List<T> Messages<T>()
        {
            List<T> messages = new List<T>();
            foreach (var data in this.Datas)
            {
                T msg = FileSerializer.Deserialize<T>(data);
                if (msg != null)
                    messages.Add(msg);
                else
                {
                    throw new Exception(string.Format("StatusData Message Deserialize Error: [{0}][{1}]{2}", this.SystemName,this.OpCode, typeof(T).Name));
                }
            } 
            return messages;
        }

        public T Message<T>()
        {
            if(this.Datas.Count>0)
            {
                T msg = FileSerializer.Deserialize<T>(this.Datas[0]);
                return msg;
            }
            return default(T);
        }

        public UInt64 Value(string key)
        {
            UInt64 val = 0;
            this.Values.TryGetValue(key, out val);
            return val;
        }
    }
}

public class StatusDispatcher
{
    private readonly Dictionary<string, Action<StatusData>> m_Handlers;

    class DispatcherObject
    {
        public int SystemId;
        public int StatusCode;
        public IExtensible Message;
    }

    private Queue<StatusData> Dispatchers = new Queue<StatusData>();


    public StatusDispatcher()
    {
        m_Handlers = new Dictionary<string, Action<StatusData>>();
    }

    public bool Register(string systemName, Action<StatusData> callBack)
    {
        if (!m_Handlers.TryGetValue(systemName, out var actions))
        {
            m_Handlers.Add(systemName, callBack);
        }
        return true;
    }

    public void UnRegisterSystem(string systemName)
    {
        if (m_Handlers.TryGetValue(systemName, out _))
        {
            m_Handlers.Remove(systemName);
        }
    }

    public void Dispatch(StatusData status)
    {
        this.Dispatchers.Enqueue(status);
    }

    public void Update()
    {

        if (this.Dispatchers.Count == 0) return;
        while (this.Dispatchers.Count > 0)
        {
            var obj = this.Dispatchers.Dequeue();
            if (obj == null) continue;
            this.CallDispatch(obj);
        }
    }

    private void CallDispatch(StatusData obj)
    {
        if (!m_Handlers.TryGetValue(obj.SystemName, out var action))
        {
            NgDebug.LogWarningFormat("Status Process not found[{0}]", obj.SystemName, obj.OpCode);
            return;
        }
        action?.Invoke(obj);
    }
}
