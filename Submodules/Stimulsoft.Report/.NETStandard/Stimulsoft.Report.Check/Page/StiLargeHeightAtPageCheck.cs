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
    public class StiLargeHeightAtPageCheck : StiPageCheck
    {
        #region Properties
        public override string ShortMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiLargeHeightAtPageCheckShort");

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckComponent", "StiLargeHeightAtPageCheckLong"), Element);

        public override StiCheckStatus Status => StiCheckStatus.Information;
        #endregion

        #region Methods
        private bool Check()
        {
            StiPage page = this.Element as StiPage;

            if (!(page is IStiForm))
            {
                double sumHeight = 0;
                int count = 0;
                foreach (StiComponent comp in page.Components)
                {
                    if (comp is StiBand && !((StiBand)comp).IsCross)
                    {
                        sumHeight += comp.Height;
                        count++;
                    }
                }

                if (page.Report.Info.ShowHeaders && count > 0) sumHeight += page.GridSize * count * 2;
                sumHeight += count > 1 ? page.GridSize * count - 1 : page.GridSize;
                
                double pageHeight = page.Height;
                pageHeight *= page.LargeHeight ? page.LargeHeightFactor : 1;
                if (pageHeight - sumHeight <= pageHeight * 0.1)
                {
                    return true;
                }
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            bool failed = Check();
            if (failed)
            {
                StiLargeHeightAtPageCheck check = new StiLargeHeightAtPageCheck();
                check.Element = obj;
                check.Actions.Add(new StiLargeHeightAtPageAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}