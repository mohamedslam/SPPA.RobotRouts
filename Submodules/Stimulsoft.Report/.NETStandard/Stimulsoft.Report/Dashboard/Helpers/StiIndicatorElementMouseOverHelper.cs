#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

namespace Stimulsoft.Report.Dashboard.Helpers
{
    internal static class StiIndicatorElementMouseOverHelper
    {
        #region Properties
        internal static IStiIndicatorElement IndicatorElement { get; set; }

        internal static PointD? MouseOverPoint { get; set; }

        internal static StiIndicatorElementMouseOverData MouseOverData { get; set; }
        #endregion

        #region Methods
        internal static void SetMouseOverPoint(IStiIndicatorElement indicator, PointD point)
        {
            IndicatorElement = indicator;
            MouseOverPoint = point;
        }

        internal static void SetMouseOverData(IStiIndicatorElement indicator, StiIndicatorElementMouseOverData data)
        {
            IndicatorElement = indicator;
            MouseOverData = data;
        }

        public static PointD? GetMouseOverPoint(IStiIndicatorElement indicator, bool useZoom = false)
        {
            if (IndicatorElement?.GetKey() == indicator?.GetKey() && MouseOverPoint != null)
            {
                return !useZoom ? MouseOverPoint :
                    new PointD(MouseOverPoint.Value.X * indicator.Zoom, MouseOverPoint.Value.Y * indicator.Zoom);             
            }
            return null;
        }

        internal static StiIndicatorElementMouseOverData GetMouseOverData(IStiIndicatorElement indicator)
        {
            if (IndicatorElement?.GetKey() == indicator?.GetKey() && MouseOverData != null)
            {
                return MouseOverData;
            }
            return null;
        }

        public static void ResetMouseOver(IStiIndicatorElement indicator)
        {
            if (IndicatorElement?.GetKey() == indicator?.GetKey())
            {
                IndicatorElement = null;
                MouseOverPoint = null;
                MouseOverData = null;
            }
        }
        #endregion
    }
}
