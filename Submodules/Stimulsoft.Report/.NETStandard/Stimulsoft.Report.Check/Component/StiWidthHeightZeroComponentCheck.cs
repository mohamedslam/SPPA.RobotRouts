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
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiWidthHeightZeroComponentCheck : StiComponentCheck
    {
        #region Properties
        public override string ShortMessage
        {
            get
            {
                StiComponent comp = this.Element as StiComponent;

                if (comp.Width == 0 && comp.Height == 0)
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckShortWidthHeight"), this.ElementName);
                else if (comp.Width == 0)
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckShortWidth"), this.ElementName);
                else
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckShortHeight"), this.ElementName);
            }
        }

        public override string LongMessage
        {
            get
            {
                StiComponent comp = this.Element as StiComponent;

                if (comp != null)
                {
                    if (comp.Width == 0 && comp.Height == 0)
                        return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckLongWidthHeight"), this.ElementName);
                    else if (comp.Width == 0)
                        return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckLongWidth"), this.ElementName);
                    else
                        return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckLongHeight"), this.ElementName);
                }
                else
                {
                    return string.Format(StiLocalizationExt.Get("CheckComponent", "StiWidthHeightZeroComponentCheckLongWidthHeight"), this.ElementName);
                }
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Information;
            }
        }
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            StiComponent comp = obj as StiComponent;

            bool failed = false;

            if ((!(obj is IStiElement)) && (comp.Width == 0 || comp.Height == 0) && (!(comp is StiBand)) && (!(comp is StiPointPrimitive)))
            {
                failed = true;
            }

            if (failed)
            {
                StiWidthHeightZeroComponentCheck check = new StiWidthHeightZeroComponentCheck();
                check.Element = obj;
                check.Actions.Add(new StiDeleteComponentAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}
