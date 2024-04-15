using System;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class TextBox : Control
    {
        public int MaxLength { get; set; }
        public char PasswordChar { get; set; }
        public bool Multiline { get; set; }
        public bool WordWrap { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }

        public bool ReadOnly { get; set; }

        public bool AutoSize { get; set; }
    }
}
