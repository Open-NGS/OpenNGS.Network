using System.Collections;
using System.Collections.Generic;
using System;


public interface IIdentifierAttribute<TIdentifier>
{
    TIdentifier ID { get; }
}

//用于标识拥有协议处理函数类，为了减少反射查询方法数量，先查找相关类，再从对应类的所有方法找出需要注册的方法
public class MsgHandlerClassAttribute : Attribute
{

}

//标识对应协议号的处理函数，必须是静态函数
public class MsgHandlerMethodAttribute : Attribute, IIdentifierAttribute<int>
{
    int _MsgID;
    Type _MsgType;
    public int ID { get { return _MsgID; } }

    public Type MsgType { get { return _MsgType; } }

    public MsgHandlerMethodAttribute(int msgID, Type msgType = null)
    {
        _MsgID = msgID;
        _MsgType = msgType;
    }
}



