using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BubbleSort
{
    public static void Sort<T>(List<T> list, IComparer<T> comparer)
    {
        if (list == null) return;
        for (int end = list.Count - 1; end > 0; end--)
        {
            Sort(list, 0, end, comparer);
        }
    }

    public static void Sort<T>(List<T> list, int start, int end, IComparer<T> comparer)
    {
        if (list == null) return;
        for (int i = start; i < end; i++)
        {
            if (comparer.Compare(list[i], list[end]) > 0)
                swap(list, i, end);
        }
    }

    public static void swap<T>(List<T> list, int a, int b)
    {
        T temp = list[a];
        list[a] = list[b];
        list[b] = temp;
    }
}
