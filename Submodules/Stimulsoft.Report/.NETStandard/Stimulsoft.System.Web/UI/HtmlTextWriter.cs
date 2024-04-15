using System.Globalization;
using System.IO;
using System.Text;

namespace Stimulsoft.System.Web.UI
{
    public class HtmlTextWriter : TextWriter
    {
        private TextWriter writer;
        private bool tabsPending;
        private int indentLevel;
        private string tabString;

        public const string DefaultTabString = "\t";

        public override Encoding Encoding
        {
            get
            {
                return writer.Encoding;
            }
        }

        protected virtual void OutputTabs()
        {
            if (tabsPending)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    writer.Write(tabString);
                }
                tabsPending = false;
            }
        }

        public override void Write(string s)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(s);
        }

        public HtmlTextWriter(TextWriter writer) : this(writer, DefaultTabString)
        {
        }

        public HtmlTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
        {
            this.writer = writer;
            this.tabString = tabString;
            indentLevel = 0;
            tabsPending = false;
        }
    }
}
