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

namespace Stimulsoft.Report.Print
{
    public interface IStiWpfPrintProvider
    {
        /// <summary>
        /// Prints the rendered report. If the report is not rendered then its rendering starts.
        /// </summary>
        /// <param name="printTicketObject">Specifies information about how a document is printed.</param>
        /// <param name="showPrintDialog">Show the print dialog or no.</param>
        /// <param name="fromPage">Number of the first page to print. Starts from 1.</param>
        /// <param name="toPage">Number of the last page to print. Starts from 1.</param>
        /// <param name="copies">Number of copies to print.</param>
        void Print(object printTicketObject, bool showPrintDialog, int fromPage, int toPage, int copies, string printerName);
    }
}