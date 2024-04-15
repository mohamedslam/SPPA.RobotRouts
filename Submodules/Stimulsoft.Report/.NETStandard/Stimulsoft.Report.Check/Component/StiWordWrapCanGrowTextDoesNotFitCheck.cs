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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helper;
using System.Drawing;

namespace Stimulsoft.Report.Check
{
    public class StiWordWrapCanGrowTextDoesNotFitCheck : StiComponentCheck
    {
        #region Properties
        public override bool PreviewVisible
        {
            get
            {
                return true;
            }
        }

        public override string ShortMessage
        {
            get
            {
                return StiLocalizationExt.Get("CheckComponent", "StiWordWrapCanGrowTextDoesNotFitShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWordWrapCanGrowTextDoesNotFitLong"), this.ElementName);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Warning;
            }
        }
        #endregion

        #region Methods
        private bool Check()
        {
            StiText comp = Element as StiText;

            if ((comp == null) || (comp.Text.Value == null) || !string.IsNullOrEmpty(comp.RenderTo) || !comp.WordWrap || comp.CanGrow) return false;

            if (comp.OnlyText || !comp.Text.Value.Contains("{"))
            {
                var comp2 = comp.Clone() as StiText;
                comp2.CanGrow = true;
                var size = comp2.GetActualSize();
                if (size.Height * 0.98 < comp.Height) return false; //everything fits

                comp2.Text = "a";
                size = comp2.GetActualSize();
                if (size.Height * 0.98 > comp.Height) return false; //it's StiInsufficientTextHeightForOneLineCheck
            }

            return true;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            try
            {
                bool failed = Check();

                if (failed)
                {
                    StiWordWrapCanGrowTextDoesNotFitCheck check = new StiWordWrapCanGrowTextDoesNotFitCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiWordWrapCanGrowTextDoesNotFitAction());
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