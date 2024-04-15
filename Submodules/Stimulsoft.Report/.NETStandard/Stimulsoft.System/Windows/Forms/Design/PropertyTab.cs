using System;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.System.Windows.Forms.Design
{
    public abstract class PropertyTab
    {
        public virtual Bitmap Bitmap { get; }

        public abstract string TabName { get; }

        public virtual bool CanExtend(object extendee)
        {
            return false;
        }

        public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

        public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            return null;
        }
    }
}
