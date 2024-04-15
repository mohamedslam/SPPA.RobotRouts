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
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiGrowToHeightOverlappingCheck : StiComponentCheck
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
                return StiLocalizationExt.Get("CheckComponent", "StiGrowToHeightOverlappingShort");
            }
        }

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckComponent", "StiGrowToHeightOverlappingLong"), this.ElementName);
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
            StiComponent component = Element as StiComponent;
            if (component.GrowToHeight && component.Parent != null && !(component.Parent is StiPage) && !(component.Parent is StiTable) && component.Parent.Components.Count > 1)
            {
                RectangleD baseRect = component.ClientRectangle;
                RectangleD baseRectGrow = new RectangleD(baseRect.X, baseRect.Y, baseRect.Width, component.Parent.Height - baseRect.Y);
                foreach (StiComponent comp in component.Parent.Components)
                {
                    if (comp == component) continue;
                    if (comp.Top < baseRect.Bottom) continue;
                    if (baseRectGrow.IntersectsWith(comp.ClientRectangle)) return true;
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
                    StiGrowToHeightOverlappingCheck check = new StiGrowToHeightOverlappingCheck();
                    check.Element = obj;
                    check.Actions.Add(new StiGrowToHeightOverlappingAction());
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