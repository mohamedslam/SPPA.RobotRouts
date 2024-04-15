using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Web.UI
{
    public sealed class CssStyleCollection
    {
        private Hashtable styles;

        public ICollection Keys
        {
            get
            {
                if (styles == null) return new string[] { };
                return styles.Keys;
            }
        }

        public int Count
        {
            get
            {
                if (styles == null) return 0;
                return styles.Count;
            }
        }

        public string this[string key]
        {
            get
            {
                if (styles != null)
                {
                    var value = styles[key];
                    if (value is string) return (string)value;
                }
                return null;
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(string key, string value)
        {
            if (styles == null) styles = new Hashtable();
            if (styles.Contains(key)) styles.Remove(key);
            styles.Add(key, value);
        }

        public void Remove(string key)
        {
            if (styles != null && styles.Contains(key)) styles.Remove(key);
        }

        public void Clear()
        {
            styles.Clear();
        }
    }
}
