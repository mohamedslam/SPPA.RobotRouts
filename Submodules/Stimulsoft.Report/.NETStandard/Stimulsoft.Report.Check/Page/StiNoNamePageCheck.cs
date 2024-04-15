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
    public class StiNoNamePageCheck : StiPageCheck
    {
        #region Properties
        private StiPage Page => this.Element as StiPage;

        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckComponent", "StiNoNamePageCheckShort"), this.Page.Report.Pages.IndexOf(this.Page));

        public override string LongMessage
        {
            get
            {
                if (Page != null)
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiNoNamePageCheckLong"), this.Page.Report.Pages.IndexOf(this.Page));
                else
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiNoNamePageCheckLong"), "");
            }
        }

        public override StiCheckStatus Status => StiCheckStatus.Error;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            StiComponent comp = obj as StiComponent;

            bool failed = false;

            if (string.IsNullOrEmpty(comp.Name))
            {
                failed = true;
            }

            if (failed)
            {
                StiNoNamePageCheck check = new StiNoNamePageCheck();
                check.Element = obj;
                check.Actions.Add(new StiGenerateNewNamePageAction());
                check.Actions.Add(new StiDeletePageAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}
