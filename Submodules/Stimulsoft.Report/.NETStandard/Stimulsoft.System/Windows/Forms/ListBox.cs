using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class ListBox : Control
    {
        public SelectionMode SelectionMode { get; set; }
        public bool Sorted { get; set; }
        public int SelectedIndex { get; set; }
        public object SelectedItem { get; set; }
        public object SelectedValue { get; set; }
        public int ItemHeight { get; set; }
        public bool HorizontalScrollbar { get; set; }
        public EventHandler SelectedIndexChanged { get; set; }
        public List<object> Items { get; set; }
        public virtual DrawMode DrawMode { get; set; }

        public event DrawItemEventHandler DrawItem;

        public string GetItemText(object item)
        {
            if (DrawItem != null) return null; // only for fix warning
            return null;
        }

        public class ObjectCollection : IList, ICollection, IEnumerable
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
        }
    }
}
