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
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiColumnsWidthGreaterPageWidthCheck : StiPageCheck
    {
        #region Properties
        public override bool PreviewVisible => true;

        public override string ShortMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiColumnsWidthGreaterPageWidthCheckShort");

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckComponent", "StiColumnsWidthGreaterPageWidthCheckLong"), this.ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        private bool Check()
        {
            StiPage page = Element as StiPage;

            if (page != null && page.Columns > 0)
            {
                double sumWidth = page.ColumnWidth * page.Columns + page.ColumnGaps * (page.Columns - 1);
                if (sumWidth * 0.99 > page.Width) return true;
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                bool failed = Check();

                if (failed)
                {
                    StiColumnsWidthGreaterPageWidthCheck check = new StiColumnsWidthGreaterPageWidthCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiColumnsWidthGreaterContainerWidthAction());
                    return check;
                }
                else return null;
            }
            finally
            {
                this.Element = null;
            }
        }
        #endregion
    }
}