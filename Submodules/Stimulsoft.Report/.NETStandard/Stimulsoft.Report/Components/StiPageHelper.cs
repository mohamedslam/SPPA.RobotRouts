#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using System.Drawing.Printing;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Summary description for StiPageHelper.
	/// </summary>
	public class StiPageHelper
	{
        #region Methods
        public static PaperSize GetPaperSizeFromPaperKind(PaperKind paperKind)
		{
            if (paperKind == PaperKind.Custom || !StiOptions.Print.AllowUsePaperSizesFromPrinterSettings) 
				return null;

			try
			{
				var paperSizes = StiOptions.Print.CustomPaperSizes;
				if (paperSizes == null)
				{
					var printerSetting = new PrinterSettings();
					paperSizes = printerSetting.PaperSizes;
				}

				foreach (PaperSize paper in paperSizes)
				{
					if (paper.Kind == paperKind) 
						return paper;
				}
			}
			catch 
			{
			}

			return null;
		}

		
		public static SizeD GetPaperSize(StiPage page, PaperSize paperSize)
		{
			double pageWidth = paperSize.Width;
			double pageHeight = paperSize.Height;

			if (page.Unit is StiCentimetersUnit || page.Unit is StiMillimetersUnit)
			{
				pageWidth = PrinterUnitConvert.Convert(paperSize.Width, 
					PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter);

				pageHeight = PrinterUnitConvert.Convert(paperSize.Height, 
					PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter);

				if (page.Unit is StiCentimetersUnit)
				{
					pageWidth = pageWidth / 10;
					pageHeight = pageHeight / 10;
				}
			}
			else 
			{
				pageWidth = Math.Round(page.ConvertFromHInches(page.Unit, pageWidth), 2);
				pageHeight = Math.Round(page.ConvertFromHInches(page.Unit, pageHeight), 2);
			}

			if (page.Orientation == StiPageOrientation.Landscape)
				return new SizeD(pageHeight, pageWidth);
			else 
				return new SizeD(pageWidth, pageHeight);
		}
        #endregion
    }
}