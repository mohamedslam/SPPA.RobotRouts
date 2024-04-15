#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using Stimulsoft.Report.Export;

namespace Stimulsoft.Report.Web
{
    public class StiDefaultExportSettings
    {
        /// <summary>
        /// Gets or sets default export settings to the PDF format.
        /// </summary>
        public StiPdfExportSettings ExportToPdf { get; } = new StiPdfExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the XPS format.
        /// </summary>
        public StiXpsExportSettings ExportToXps { get; } = new StiXpsExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the Power Point 2007-2010 format.
        /// </summary>
        public StiPpt2007ExportSettings ExportToPowerPoint { get; } = new StiPpt2007ExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the HTML format.
        /// </summary>
        public StiHtmlExportSettings ExportToHtml { get; } = new StiHtmlExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the TEXT format.
        /// </summary>
        public StiTxtExportSettings ExportToText { get; } = new StiTxtExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the RTF format.
        /// </summary>
        public StiRtfExportSettings ExportToRtf { get; } = new StiRtfExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the Word 2007-2010 format.
        /// </summary>
        public StiWord2007ExportSettings ExportToWord2007 { get; } = new StiWord2007ExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the Open Document Text format.
        /// </summary>
        public StiOdtExportSettings ExportToOpenDocumentWriter { get; } = new StiOdtExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the Excel BIFF format.
        /// </summary>
        public StiExcelExportSettings ExportToExcel { get; } = new StiExcelExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the Open Document Calc format.
        /// </summary>
        public StiOdsExportSettings ExportToOpenDocumentCalc { get; } = new StiOdsExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the data format.
        /// </summary>
        public StiDataExportSettings ExportToData { get; } = new StiDataExportSettings();

        /// <summary>
        /// Gets or sets default export settings to the image format.
        /// </summary>
        public StiImageExportSettings ExportToImage { get; } = new StiImageExportSettings();
    }
}
