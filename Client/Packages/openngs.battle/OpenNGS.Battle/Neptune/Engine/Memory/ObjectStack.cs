using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ObjectStack<T> where T : new()
{
    static int i = 0;
    T[] elements;
    public void Init(int size)
    {
        elements = new T[size];
        for(int i=0;i< size;i++)
        {
            elements[i] = new T();
        }
    }

    public T New()
    {
        return elements[i++];
    }


    public void Delete()
    {
        i--;
    }
}