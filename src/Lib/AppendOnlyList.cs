using System;
using System.Collections.Generic;

namespace AppendOnly
{
    public class AppendOnlyList<T>
    {

        private LinkedList<T> _items;
        public long Length { get; }

        public AppendOnlyList()
        {
            _items = new LinkedList<T>();
            Length = 0;
        }

        private AppendOnlyList(LinkedList<T> items, T newItem)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _items.AddLast(newItem);
            Length = items.Count;
        }

        public AppendOnlyList<T> AddItem(T item)
        {
            return new AppendOnlyList<T>(_items, item);
        }

        public IEnumerable<T> Items
        {
            get
            {
                // new classes share the same original LinkedList
                // and "mutate" it, so we can't simply enumerate the whole list.  
                return _items.EnumerateTheFirst(Length);
            }
        }
    }
}
