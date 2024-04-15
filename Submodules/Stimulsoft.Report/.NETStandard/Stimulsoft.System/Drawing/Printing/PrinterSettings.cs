using System;
using System.Drawing.Printing;
using static System.Drawing.Printing.PrinterSettings;

namespace Stimulsoft.System.Drawing.Printing
{
	public class PrinterSettings
	{
        public PaperSizeCollection PaperSizes { get; } = new PaperSizeCollection(new PaperSize[] { });

        public PaperSourceCollection PaperSources { get; } = new PaperSourceCollection(new PaperSource[] { });

		public short Copies { get; set; } = 1;

		public int FromPage { get; set; }

        public int ToPage { get; set; }

        public int MaximumCopies { get; set; }

        public PrinterSettings()
		{
		}
	}
}

