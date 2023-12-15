using System;
using System.Collections.Generic;

namespace MissQ.Tools
{
    public enum CompareOperation
    {
        LessThan,
        LessThanOrEqualTo,
        EqualTo,
        NotEqualTo,
        GreaterThanOrEqualTo,
        GreaterThan,
    }

    public class ValueComparer
    {
        public static bool Result<T>(CompareOperation op, T expA, T expB) where T : IComparable
        {
            switch (op)
            {
                // value1 < value2
                case CompareOperation.LessThan:
                    return Comparer<T>.Default.Compare(expA, expB) < 0;

                // value1 <= value2
                case CompareOperation.LessThanOrEqualTo:
                    return Comparer<T>.Default.Compare(expA, expB) <= 0;

                // value1 == value2
                case CompareOperation.EqualTo:
                    return Comparer<T>.Default.Compare(expA, expB) == 0;

                // value1 != value2
                case CompareOperation.NotEqualTo:
                    return Comparer<T>.Default.Compare(expA, expB) != 0;

                // value1 >= value2
                case CompareOperation.GreaterThanOrEqualTo:
                    return Comparer<T>.Default.Compare(expA, expB) >= 0;

                // value1 > value2
                case CompareOperation.GreaterThan:
                    return Comparer<T>.Default.Compare(expA, expB) > 0;

                default:
                    return false;
            }
        }
    }
}
