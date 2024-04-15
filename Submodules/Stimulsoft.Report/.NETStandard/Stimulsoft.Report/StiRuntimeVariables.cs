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
using System.Collections;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report
{
	/// <summary>
	/// Describes the class which allows to save and restore variables of a report.
	/// </summary>
	public class StiRuntimeVariables : ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Fields
        public StiPage Page;
		internal StiSimpleText TextBox = null;
		public int Line;
		public int Column;
		public int LineThrough;
		public Hashtable DataSourcesPosition = new Hashtable();

        private int pageIndex = 0;
        internal int PageIndex
        {
            get
            {
                if (TextBox != null && TextBox.Report != null && (TextBox.Report.EngineVersion == StiEngineVersion.EngineV2))
                {
                    var tempPageIndex = TextBox.Report.RenderedPages.IndexOf(TextBox.Page);
                    if (tempPageIndex != -1)
                        return tempPageIndex + 1;
                }
                return pageIndex;
            }
            set
            {
                pageIndex = value;
            }
        }

        private int currentPrintPage;
        public int CurrentPrintPage
        {
            get
            {
                if (TextBox != null && TextBox.Report != null && TextBox.Report.EngineVersion == StiEngineVersion.EngineV2)
                {
                    var tempPageIndex = TextBox.Report.RenderedPages.IndexOf(TextBox.Page);
                    if (tempPageIndex != -1)
                        return tempPageIndex + 1;
                }
                return currentPrintPage;
            }
            set
            {
                currentPrintPage = value;
            }
        }
        #endregion

        /// <summary>
		/// Sets runtime-variables to the specified report.
		/// </summary>
		public void SetVariables(StiReport report)
		{
			if (report.EngineVersion == StiEngineVersion.EngineV1)
			{
				if (Page.PageInfoV1.TotalPageCount != -1)
				    report.TotalPageCount = Page.PageInfoV1.TotalPageCount;

				report.PageNumber = Page.PageInfoV1.PageNumber;
			}
			report.Line = this.Line;
			report.LineThrough = this.LineThrough;
			report.Column = this.Column;
            report.CurrentPrintPage = this.CurrentPrintPage;

			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				dataSource.Position = (int)DataSourcesPosition[dataSource.Name];
			}
		}


		/// <summary>
		/// Creates a new instance of the StiRuntimeVariables class.
		/// </summary>
		public StiRuntimeVariables(StiReport report)
		{
			this.Page = report.EngineVersion == StiEngineVersion.EngineV1 ? report.EngineV1.PageInProgress : report.Engine.Page;
				
			this.Line = report.Line;
			this.LineThrough = report.LineThrough;
			this.Column = report.Column;
			this.CurrentPrintPage = report.CurrentPrintPage;

			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				DataSourcesPosition[dataSource.Name] = dataSource.Position;
			}
		}

	}
}
