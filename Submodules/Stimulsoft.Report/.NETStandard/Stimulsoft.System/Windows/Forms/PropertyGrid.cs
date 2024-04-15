using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.System.Windows.Forms
{
    public class PropertyTabCollection : List<object>
    {
        public void AddTabType(Type propertyTabType, PropertyTabScope tabScope)
        {

        }

        public void RemoveTabType(Type propertyTabType)
        {

        }
    }

    public class PropertyGrid : ScrollableControl
    {
        public virtual bool HelpVisible { get; set; }
        public Color LineColor { get; set; }
        public Color HelpBackColor { get; set; }
        public Color CategoryForeColor { get; set; }
        public Color CategorySplitterColor { get; set; }
        public Color ViewBackColor { get; set; }
        public Color ViewForeColor { get; set; }
        public Color ViewBorderColor { get; set; }
        public Color HelpForeColor { get; set; }
        public Color HelpBorderColor { get; set; }
        public Color SelectedItemWithFocusBackColor { get; set; }
        public Color SelectedItemWithFocusForeColor { get; set; }

        public PropertyTabCollection PropertyTabs { get; }
        public object[] SelectedObjects { get; set; }
        public PropertySort PropertySort { get; set; }
    }
}
