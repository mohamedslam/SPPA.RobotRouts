namespace Stimulsoft.System.Web.UI
{
    public class Control
    {
        public virtual string ID { get; set; }

        public virtual ControlCollection Controls { get; } = new ControlCollection();

        protected virtual void Render(HtmlTextWriter writer)
        {
            foreach (Control child in Controls)
            {
                child.Render(writer);
            }
        }

        public virtual void RenderControl(HtmlTextWriter writer)
        {
            Render(writer);
        }

        protected internal virtual void CreateChildControls()
        {
        }
    }
}
