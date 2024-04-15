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
using System.ComponentModel;
using Stimulsoft.Base.Design;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        #region Runtime Variables
        private int pageNumber;
        /// <summary>
        /// Gets or sets an index of the current page printed taking into consideration segmented pages.
        /// Number starts from 1. Property ResetPageNumber can reset value of this variable.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets an index of the current page printed taking into consideration segmented pages. Number starts from 1. Property ResetPageNumber can reset value of this variable.")]
        public int PageNumber
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                {
                    if (EngineV1.ReportPageNumbers != null &&
                        EngineV1.ReportPageNumbers[CurrentPrintPage] is int &&
                        CurrentPrintPage < EngineV1.ReportPageNumbers.Count)
                    {
                        return (int)EngineV1.ReportPageNumbers[CurrentPrintPage];
                    }
                    else return pageNumber;
                }
                else
                {
                    return Engine != null && Engine.PageNumbers != null 
                        ? Engine.PageNumbers.GetPageNumber(CurrentPrintPage - 1) 
                        : pageNumber;
                }
            }
            set
            {
                pageNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets an index of the current page printed taking into consideration segmented pages. Number starts from 1.
        /// Property ResetPageNumber is ignored.
        /// </summary>
        [StiEngine(StiEngineVersion.EngineV2)]
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets an index of the current page printed taking into consideration segmented pages. Number starts from 1. Property ResetPageNumber is ignored.")]
        public int PageNumberThrough
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    throw new Exception("You can use system variable 'PageNumberThrough' only with EngineV2.");

                return Engine != null && Engine.PageNumbers != null 
                    ? Engine.PageNumbers.GetPageNumberThrough(CurrentPrintPage - 1) 
                    : 0;
            }
        }

        internal int totalPageCountValue;
        /// <summary>
        /// Gets or sets the number of pages in a report (RunTime Variable).
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the number of pages in a report. Property ResetPageNumber can reset value of this variable.")]
        public int TotalPageCount
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                {
                    if (EngineV1.ReportTotalPageCounts != null &&
                        EngineV1.ReportTotalPageCounts[CurrentPrintPage] is int &&
                        CurrentPrintPage < EngineV1.ReportTotalPageCounts.Count)
                    {
                        return (int)EngineV1.ReportTotalPageCounts[CurrentPrintPage];
                    }
                    else return totalPageCountValue;
                }
                else
                {
                    if (Engine != null && Engine.PageNumbers != null)
                        return Engine.PageNumbers.GetTotalPageCount(CurrentPrintPage - 1);
                    else
                        return 0;
                }
            }
            set
            {
                totalPageCountValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of pages in a report. Property ResetPageNumber is ignored.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the number of pages in a report. Property ResetPageNumber is ignored.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        public int TotalPageCountThrough
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    throw new Exception("You can use system variable 'TotalPageCountThrough' only with EngineV2.");

                if (Engine != null && Engine.PageNumbers != null)
                    return Engine.PageNumbers.GetTotalPageCountThrough(CurrentPrintPage - 1);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets string value which contains "Page N of M". Property ResetPageNumber can reset value of N.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets string value which contains \"Page N of M\". Property ResetPageNumber can reset value of N.")]
        public string PageNofM => StiSystemVariableLocHelper.GetPageNofM(this);

        /// <summary>
        /// Gets string value which contains "Page N of M". Property ResetPageNumber is ignored.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets string value which contains \"Page N of M\". Property ResetPageNumber is ignored.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        public string PageNofMThrough
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    throw new Exception("You can use system variable 'PageNofMThrough' only with EngineV2.");

                return StiSystemVariableLocHelper.GetPageNofMThrough(this);
            }
        }

        /// <summary>
        /// Gets string value which contains localization template for "Page N of M". For example: Page {0} of {1}.
        /// By default this property equal to null and report engine use global localization string.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets string value which contains localization template for 'Page N of M'. For example: Page {0} of {1}." +
             "By default this property equal to null and report engine use global localization string.")]
        public string PageNofMLocalizationString { get; set; }

        /// <summary>
        /// Gets or sets the current line that starts at the beginning of a group.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets current line that starts at the beginning of a group.")]
        public int Line { get; set; } = 1;

        private int groupLine = 1;
        /// <summary>
        /// Gets or sets the current group number.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the current group number.")]
        [StiEngine(StiEngineVersion.EngineV2)]
        public int GroupLine
        {
            get
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    throw new Exception("You can use system variable 'GroupLine' only with EngineV2.");

                return groupLine;
            }
            set
            {
                if (this.EngineVersion == StiEngineVersion.EngineV1)
                    throw new Exception("You can use system variable 'GroupLine' only with EngineV2.");

                groupLine = value;
            }
        }

        /// <summary>
        /// Gets the current line (Roman numerals) that starts at the beginning of a group.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the current line (Roman numerals) that starts at the beginning of a group.")]
        public string LineRoman => Func.Convert.ToRoman(Line);

        /// <summary>
        /// Gets or sets the current line (Alphabetical) that starts at the beginning of a group.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets current line (Alphabetical) that starts at the beginning of a group.")]
        public string LineABC => Func.Convert.ToABC(Line);

        /// <summary>
        /// Gets or sets the current column.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets current column.")]
        public int Column { get; set; } = 1;

        /// <summary>
        /// Gets or sets the current line which starts at the beginning of a report.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets current line starts at the beginning of a report.")]
        public int LineThrough { get; set; } = 1;

        /// <summary>
        /// Gets the current date.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the current date.")]
        public DateTime Date => DateTime.Today;

        /// <summary>
        /// Gets the current date.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the current date.")]
        public DateTime Today => DateTime.Today;

        /// <summary>
        /// Gets the current time.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the current time.")]
        public DateTime Time => DateTime.Now;

        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFirstPage => PageNumber == 1;
        
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLastPage => PageNumber == TotalPageCount;

        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFirstPageThrough => PageNumberThrough == 1;

        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLastPageThrough => PageNumberThrough == TotalPageCountThrough;

        /// <summary>
        /// Gets value which indicates that first report pass is rendered now.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFirstPass => ReportPass == StiReportPass.First || ReportPass == StiReportPass.None;

        /// <summary>
        /// Gets value which indicates that second report pass is rendered now.
        /// </summary>
        [StiBrowsable(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSecondPass => ReportPass == StiReportPass.Second;

        /// <summary>
        /// Gets or sets the index of the current page. Counting starts from 0.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the index of the current page.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentPrintPage { get; set; }

        /// <summary>
        /// Gets or sets a number of a current copy of a page.
        /// </summary>
        [Browsable(false)]
        [StiBrowsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets a number of a current copy of a page.")]
        public int PageCopyNumber { get; set; } = 1;
        #endregion
    }
}