using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class CheckedListBox : Control
    {
        public SelectionMode SelectionMode { get; set; }
        public bool Sorted { get; set; }
        public int SelectedIndex { get; set; }
        public object SelectedItem { get; set; }
        public object SelectedValue { get; set; }
        public int ItemHeight { get; set; }
        public bool CheckOnClick { get; set; }
        public bool HorizontalScrollbar { get; set; }
        public EventHandler SelectedIndexChanged { get; set; }
        public List<object> CheckedItems { get; set; }
        public List<object> Items { get; set; }
    }
}
