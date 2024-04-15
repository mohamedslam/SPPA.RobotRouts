using Stimulsoft.System.Windows.Forms.Layout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Stimulsoft.System.Windows.Forms
{
    public class ToolStripItemCollection : ArrangedElementCollection, IList, ICollection, IEnumerable
    {
        public new virtual ToolStripItem this[int index] => null;

        public virtual ToolStripItem this[string key] => null;
    }
}
