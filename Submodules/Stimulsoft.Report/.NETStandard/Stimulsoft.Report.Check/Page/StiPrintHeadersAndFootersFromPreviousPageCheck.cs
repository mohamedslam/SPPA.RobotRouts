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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiPrintHeadersAndFootersFromPreviousPageCheck : StiPageCheck
    {
        #region Properties
        public override string ShortMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiPrintHeadersAndFootersFromPreviousPageShort");

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckComponent", "StiPrintHeadersAndFootersFromPreviousPageLong"), Element);

        public override StiCheckStatus Status => StiCheckStatus.Information;
        #endregion

        #region Methods
        private int GetPageCount(StiReport report)
        {
            int count = 0;
            foreach (StiPage page in report.Pages)
            {
                if (!(page is IStiForm))
                    count++;
            }
            return count;
        }

        private bool Check()
        {
            StiPage page = this.Element as StiPage;
            StiReport report = page.Report;

            if (page != null && page.PrintHeadersFootersFromPreviousPage && (GetPageCount(report) == 1 || report.Pages.IndexOf(page) == 0))
            {
                return true;
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            bool failed = Check();
            if (failed)
            {
                StiPrintHeadersAndFootersFromPreviousPageCheck check = new StiPrintHeadersAndFootersFromPreviousPageCheck();
                check.Element = obj;
                check.Actions.Add(new StiPrintHeadersFootersFromPreviousPageAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}