using System;
using System.Collections;

namespace Stimulsoft.System.Web.UI
{
    public class ControlCollection : ICollection, IEnumerable
    {
        private Control[] _controls;
        private int _size;
        private int _version;

        private int _defaultCapacity = 5;
        private int _growthFactor = 4;

        public virtual int Count
        {
            get
            {
                return _size;
            }
        }

        public virtual int IndexOf(Control value)
        {
            if (_controls == null)
                return -1;

            return Array.IndexOf(_controls, value, 0, _size);
        }

        public Object SyncRoot
        {
            get
            {
                return this;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        virtual public Control this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException("index");

                return _controls[index];
            }
        }

        public void Add(Control child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            // Make sure we have room 
            if (_controls == null)
            {
                _controls = new Control[_defaultCapacity];
            }
            else if (_size >= _controls.Length)
            {
                Control[] newArray = new Control[_controls.Length * _growthFactor];
                Array.Copy(_controls, newArray, _controls.Length);
                _controls = newArray;
            }

            // Add the control
            int index = _size;
            _controls[index] = child;
            _size++;
            _version++;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator GetEnumerator()
        {
            return new ControlCollectionEnumerator(this);
        }

        private class ControlCollectionEnumerator : IEnumerator
        {
            private ControlCollection list;
            private int index;
            private int version;
            private Control currentElement;

            internal ControlCollectionEnumerator(ControlCollection list)
            {
                this.list = list;
                this.index = -1;
                version = list._version;
            }

            public bool MoveNext()
            {
                if (index < (list.Count - 1))
                {
                    if (version != list._version)
                        throw new InvalidOperationException("ListEnumVersionMismatch");

                    index++;
                    currentElement = list[index];
                    return true;
                }
                else
                    index = list.Count;
                return false;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Control Current
            {
                get
                {
                    if (index == -1)
                        throw new InvalidOperationException("ListEnumCurrentOutOfRange");

                    if (index >= list.Count)
                        throw new InvalidOperationException("ListEnumCurrentOutOfRange");

                    return currentElement;
                }
            }

            public void Reset()
            {
                if (version != list._version)
                    throw new InvalidOperationException("ListEnumVersionMismatch");

                currentElement = null;
                index = -1;
            }
        }
    }
}
