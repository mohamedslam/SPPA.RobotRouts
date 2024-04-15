using System;
using System.Drawing.Printing;
using Stimulsoft.System.Windows.Forms;

namespace Stimulsoft.System.Drawing.Printing
{
    public class PrintDialog : IDisposable
    {
        public bool UseEXDialog { get; set; }
        public bool AllowPrintToFile { get; set; }
        public bool AllowSomePages { get; set; }
        public PrintDocument Document { get; set; }
        public PrinterSettings PrinterSettings { get; set; } = new PrinterSettings();
        public bool AllowSelection { get; set; }

        public void Dispose()
        {
        }

        public DialogResult ShowDialog()
        {
            return DialogResult.OK;
        }
    }
}
