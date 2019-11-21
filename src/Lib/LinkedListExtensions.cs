using System.Collections.Generic;

namespace AppendOnly
{
    public static class LinkedListExtensions
    {
        /// <summary>
        /// returns an enumerator that enumerates over only the first (n) elements. If the collection is never 
        /// modified other than having elements added to it, then this should(?) tbc, should be thread safe.
        /// cannot use the default enumerator as that will throw an exception if the list is modified during interation.
        /// i.e. added to, but no changes or deletions.
        ///</summary>
        public static IEnumerable<T> EnumerateTheFirst<T>(this LinkedList<T> src, long cnt)
        {
            int i = 0;
            var item = src.First;
            while (item != null && i++ < cnt)
            {
                if (i == 1)
                {
                    yield return item.Value;
                }
                else
                {
                    item = item.Next;
                    if (item == null) break;
                    yield return item.Value;
                }
            }
        }
    }
}