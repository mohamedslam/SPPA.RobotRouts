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
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiTextTextFormatCheck : StiComponentCheck
    {
        #region Properties
        public override string ShortMessage
        {
            get
            {
                return StiLocalizationExt.Get("CheckComponent", "StiTextTextFormatCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiTextTextFormatCheckLong"), this.ElementName);
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
            var comp = Element as StiText;

            if (comp != null && !(comp.TextFormat is StiGeneralFormatService))
            {
                var text = comp.Text.Value;
                
                if (!string.IsNullOrEmpty(text) &&
                    text.Contains("{") && text.Contains("}") &&
                    !(text.StartsWith("{") && text.EndsWith("}")))//example -> Date: {Today}
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(text) && 
                    text.Contains("{") && text.Contains("}") && text.Contains("ToString"))//example -> {Today.ToString("Y")}
                {
                    return true;
                }
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
                    var check = new StiTextTextFormatCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiApplyGeneralTextFormat());
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
