using System.Collections.Generic;

/// <summary>
/// 本对象池用于预创建对象的链表
/// 用内部对象池保存节点信息
/// 请勿在外部自己new LinkedListNode， 否则会导致内存池的节点变多，内存不可控
/// 只使用Alloc()分配节点
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolLinkList<T> where T : new()
{
    internal class LinkedListNodePool<TNode> where TNode : new()
    {
        private LinkedList<TNode> _mPool = new LinkedList<TNode>();
        private int _mCapacity;
        public LinkedListNodePool(int initCount)
        {
            _mCapacity = initCount;
            for (int i = 0; i < initCount; i++)
            {
                _mPool.AddLast(new TNode());
            }
        }

        public LinkedListNode<TNode> Alloc()
        {
            if (_mPool.Count > 0)
            {
                var res = _mPool.Last;
                _mPool.RemoveLast();
                return res;
            }

            _mCapacity++;
            return new LinkedListNode<TNode>(new TNode());
        }

        public void Release(LinkedListNode<TNode> node)
        {
            if (node.List == null)
            {
                _mPool.AddLast(node);

                if (_mPool.Count > _mCapacity)
                {
                    //CustomDebug.LogError("LinkList:LinkedListNodePool:Release get more node!");
                }
            }
            else
            {
                // CustomDebug.LogError("LinkList:LinkedListNodePool:Release error!");
            }
        }
    }

    //////////////////////////////////////////////
    // 摘要: 
    //     获取 PoolLinkedList<T> 中实际包含的节点数。
    //
    // 返回结果: 
    //     PoolLinkedList<T> 中实际包含的节点数。
    public int Count
    {
        get { return _mList.Count; }
    }

    //
    // 摘要: 
    //     获取 PoolLinkedList<T> 的第一个节点。
    //
    // 返回结果: 
    //     PoolLinkedList<T> 的第一个 System.Collections.Generic.LinkedListNode<T>。  
    public LinkedListNode<T> First
    {
        get
        {
            return _mList.First;
        }
    }

    //
    // 摘要: 
    //     获取 PoolLinkedList<T> 的最后一个节点。
    //
    // 返回结果: 
    //     PoolLinkedList<T> 的最后一个 System.Collections.Generic.LinkedListNode<T>。

    public LinkedListNode<T> Last
    {
        get
        {
            return _mList.Last;
        }
    }

    private LinkedListNodePool<T> _mNodePool;
    private LinkedList<T> _mList = new LinkedList<T>();

    // 摘要: 
    //     初始化capacity数量的节点的 PoolLinkList<T> 类的新实例。
    public PoolLinkList(int capacity)
    {
        _mNodePool = new LinkedListNodePool<T>(capacity);
    }

    // 摘要: 
    //     检查node是否属于PoolLinkList<T> 
    public bool IsVaild(LinkedListNode<T> node)
    {
        return (node != null) && (node.List == _mList);
    }

    public LinkedListNode<T> Alloc()
    {
        return _mNodePool.Alloc();
    }

    public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> value)
    {
        _mList.AddAfter(node, value);
    }


    public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> value)
    {
        _mList.AddBefore(node, value);
    }

    public void AddFirst(LinkedListNode<T> value)
    {
        _mList.AddFirst(value);
    }

    public void AddLast(LinkedListNode<T> value)
    {
        _mList.AddLast(value);
    }

    public void Remove(LinkedListNode<T> node)
    {
        _mList.Remove(node);

        _mNodePool.Release(node);
    }

    public bool Remove(T value)
    {
        var node = _mList.Find(value);
        if (node != null)
        {
            Remove(node);
            return true;
        }

        return false;
    }

    public void RemoveFirst()
    {
        var value = _mList.First;
        _mList.RemoveFirst();
        _mNodePool.Release(value);
    }

    public void RemoveLast()
    {
        var value = _mList.Last;
        _mList.RemoveLast();
        _mNodePool.Release(value);
    }

    public void Clear()
    {
        while (_mList.Count > 0)
        {
            _mList.RemoveLast();
        }
    }
}