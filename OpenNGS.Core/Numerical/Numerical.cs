using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.Numerical
{
    public interface INumerable
    {
        void Add(INumerable b);
        void Multiply(INumerable b);
        void Reset();
    }

    class NodeBase<T> where T : INumerable
    {
        public T Value;

        public NodeBase()
        {
        }

        public NodeBase(T v)
        {
            Value = v;
        }

        public List<NodeBase<T>> Inputs = new List<NodeBase<T>>();

        protected NodeBase<T> Outter;


        public virtual NodeBase<T> Output()
        {
            return this;
        }


        internal void AddInput(NodeBase<T> node)
        {
            node.Outter = this;
            this.Inputs.Add(node);
        }

        internal void AddInput(T val)
        {
            this.AddInput(new NodeBase<T>(val));
        }

        public void Final()
        {
            if (Outter != null)
                Outter.Final();

            this.Output();
        }
    }

    class NodeAdd<T> : NodeBase<T> where T : INumerable
    {
        public override NodeBase<T> Output()
        {
            this.Value.Reset();
            for (int i = 0; i < Inputs.Count; i++)
            {
                this.Value.Add((INumerable)Inputs[i].Output().Value);
            }
            return this;
        }

    }

    class NodeMultiply<T> : NodeBase<T> where T : INumerable
    {
        public override NodeBase<T> Output()
        {
            this.Value = Inputs[0].Value;
            for(int i=1;i<Inputs.Count;i++)
            {
                this.Value.Multiply(Inputs[i].Output().Value);
            }
            return this;
        }

    }
        
}
