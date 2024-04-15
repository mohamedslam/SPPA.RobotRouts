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
    public class StiVerySmallSizesOfComponentsCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiVerySmallSizesOfComponentsCheckShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiVerySmallSizesOfComponentsCheckLong"), this.ElementName);
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
            #region Check
            bool failed = false;
            if (report.Designer != null && report.Designer.Info != null)
            {
                if (obj is StiPointPrimitive)
                {
                    failed = false;
                } 
                else if (obj is StiHorizontalLinePrimitive)
                {
                    if (((StiHorizontalLinePrimitive)obj).Width >= 0 && ((StiHorizontalLinePrimitive)obj).Width < report.Designer.Info.GridSize)
                        failed = true;
                }
                else if (obj is StiVerticalLinePrimitive)
                {
                    if (((StiVerticalLinePrimitive)obj).Height >= 0 && ((StiVerticalLinePrimitive)obj).Height < report.Designer.Info.GridSize)
                        failed = true;
                }
                else if (obj is StiBand)
                {
                    StiBand band = obj as StiBand;
                    if ((band.Components.Count > 0) && (band.Height < report.Designer.Info.GridSize))
                    failed = true;
                }
                else if (!(obj is IStiElement))
                {
                    StiComponent comp = obj as StiComponent;
                    if ((comp.Width >= 0 && comp.Width < report.Designer.Info.GridSize) || (comp.Height >= 0 && comp.Height < report.Designer.Info.GridSize))
                        failed = true;
                }
            }
            #endregion

            try
            {
                if (failed)
                {
                    StiVerySmallSizesOfComponentsCheck check = new StiVerySmallSizesOfComponentsCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiDeleteComponentAction());
                    check.Actions.Add(new StiVerySmallSizesOfComponentsAction());
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