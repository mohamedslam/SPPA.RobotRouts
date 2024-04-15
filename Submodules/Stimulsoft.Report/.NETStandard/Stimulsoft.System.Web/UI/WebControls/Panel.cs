using System.Drawing;

namespace Stimulsoft.System.Web.UI.WebControls
{
    public class Panel : WebControl
    {
        private static string GetHtmlColor(Color color)
        {
            return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var id = string.Empty;
            if (!string.IsNullOrEmpty(this.ID)) id = string.Format(" id=\"{0}\"", this.ID);

            var cssClass = string.Empty;
            if (!string.IsNullOrEmpty(this.CssClass)) cssClass = string.Format(" class=\"{0}\"", this.CssClass);

            var style = string.Empty;
            if (!this.Width.IsEmpty) style = string.Format("{0}width:{1};", style, this.Width.ToString());
            if (!this.Height.IsEmpty) style = string.Format("{0}height:{1};", style, this.Height.ToString());
            if (!this.BackColor.IsEmpty && this.BackColor != Color.Transparent) style = string.Format("{0}background:{1};", style, GetHtmlColor(this.BackColor));
            if (this.Style.Count > 0)
            {
                foreach (string key in this.Style.Keys)
                {
                    style = string.Format("{0}{1}:{2};", style, key, this.Style[key]);
                }
            }
            if (!string.IsNullOrEmpty(style)) style = string.Format(" style=\"{0}\"", style);

            writer.Write("<div{0}{1}{2}>", id, cssClass, style);

            base.Render(writer);

            writer.Write("</div>", style);
        }
    }
}
