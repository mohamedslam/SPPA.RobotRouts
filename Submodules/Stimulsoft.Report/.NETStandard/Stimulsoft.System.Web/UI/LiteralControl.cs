namespace Stimulsoft.System.Web.UI
{
    public class LiteralControl : Control
    {
        public virtual string Text { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Text))
                writer.Write(this.Text);

            base.Render(writer);
        }
    }
}
