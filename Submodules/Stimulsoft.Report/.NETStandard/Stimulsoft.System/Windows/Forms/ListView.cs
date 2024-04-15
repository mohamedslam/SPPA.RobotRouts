using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class ListView : Control
    {
        public bool Scrollable { get; set; }
        public EventHandler SelectedIndexChanged { get; set; }

        public ItemActivation Activation { get; set; }

        public bool FullRowSelect { get; set; }

        public bool HotTracking { get; set; }

        public bool HoverSelection { get; set; }

        public bool UseCompatibleStateImageBehavior { get; set; }

        public View View { get; set; }

        public void Clear()
        {
        }

        public ColumnHeaderCollection Columns { get; }

        public List<object> Items { get; }

        public class ColumnHeaderCollection : IList
        {
            public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public bool IsFixedSize => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsSynchronized => throw new NotImplementedException();

            public object SyncRoot => throw new NotImplementedException();

            public int Add(object value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(object value)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(object value)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public void Remove(object value)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public void Add(string v1, int v2, HorizontalAlignment left)
            {
                throw new NotImplementedException();
            }
        }
    }
}
