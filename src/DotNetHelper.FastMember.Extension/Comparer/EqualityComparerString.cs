using System;
using System.Collections.Generic;

namespace DotNetHelper.FastMember.Extension.Comparer
{
    internal class EqualityComparerString : IEqualityComparer<string>
    {

        private StringComparison Comparer { get; }
        public EqualityComparerString(StringComparison comparer)
        {
            Comparer = comparer;
        }

        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, Comparer);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
