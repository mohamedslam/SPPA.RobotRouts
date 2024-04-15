using System;
using System.ComponentModel;

namespace Stimulsoft.System.Drawing.Design
{
    public class UITypeEditor
    {
        public virtual object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return value;
        }

        public virtual UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
}
