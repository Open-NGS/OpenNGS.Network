using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public interface IConnector
{
    void Init(string address);
    void Connect();
    void Disonnect();
    void SendCaller(string message);
    void SendToAll(string message);
    void SendMessage(string message);
    void SendMessageToAll(string message);

    void Invoke(string methodName, object arg1);
    void Invoke(string methodName, object arg1, object arg2);
    void Invoke(string methodName, object arg1, object arg2, object arg3);


    Task InvokeAsync(string methodName);
    Task<T> InvokeAsync<T>(string methodName);
    Task<T> InvokeAsync<T>(string methodName, object args1);
    Task<T> InvokeAsync<T>(string methodName, object args1, object args2);
    Task<T> InvokeAsync<T>(string methodName, object args1, object args2, object args3);

    void On<T1>(string methodName, Action<T1> handler);

    event Action<string> OnMessageReceived;
    event Action OnConnected;
    event Action OnDisconnected;
    event Action OnError;
}
