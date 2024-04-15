using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.System.Windows.Forms.PropertyGridInternal
{
    public class PropertiesTab
    {
        public virtual Bitmap Bitmap { get; }

        public virtual string TabName { get; }

        public virtual bool CanExtend(object extendee)
        {
            return false;
        }

        public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            return null;
        }

        public virtual PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return null;
        }
    }
}
