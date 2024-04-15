using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class ComboBox : Control
    {
        public ComboBoxStyle DropDownStyle { get; set; }
        public int MaxLength { get; set; }
        public bool Sorted { get; set; }
        public int SelectedIndex { get; set; }
        public object SelectedItem { get; set; }
        public object SelectedValue { get; set; }
        public int ItemHeight { get; set; }
        public int MaxDropDownItems { get; set; }
        public int DropDownWidth { get; set; }
        public EventHandler SelectedIndexChanged { get; set; }
        public List<object> Items { get; set; }

        public DrawMode DrawMode { get; set; }

        public bool DroppedDown { get; set; }

        public bool Focused { get; }

        protected virtual void OnMeasureItem(MeasureItemEventArgs e)
        {

        }

        public string GetItemText(object item)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnDropDown(EventArgs e)
        {

        }


    }
}
