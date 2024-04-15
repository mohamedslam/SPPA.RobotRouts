using System.Drawing;

namespace Stimulsoft.System.Web.UI.WebControls
{
    public class WebControl : Control
    {
        public CssStyleCollection Style { get; } = new CssStyleCollection();

        public virtual string CssClass { get; set; }

        public virtual Unit Width { get; set; } = Unit.Empty;

        public virtual Unit Height { get; set; } = Unit.Empty;

        public virtual Color BackColor { get; set; } = Color.Transparent;
    }
}
