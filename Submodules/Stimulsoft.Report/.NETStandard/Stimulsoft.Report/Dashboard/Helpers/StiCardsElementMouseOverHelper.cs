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
    internal static class StiCardsElementMouseOverHelper
    {
        #region Properties
        internal static IStiCardsElement CardsElement { get; set; }

        internal static PointD? MouseOverPoint { get; set; }
        #endregion

        #region Methods
        internal static void SetMouseOverPoint(IStiCardsElement cards, PointD point)
        {
            CardsElement = cards;
            MouseOverPoint = point;
        }

        public static PointD? GetMouseOverPoint(IStiCardsElement cards, bool useZoom = false)
        {
            if (CardsElement?.GetKey() == cards?.GetKey() && MouseOverPoint != null)
            {
                return !useZoom ? MouseOverPoint :
                    new PointD(MouseOverPoint.Value.X * cards.Zoom, MouseOverPoint.Value.Y * cards.Zoom);
            }
            return null;
        }


        public static void ResetMouseOver(IStiCardsElement cards)
        {
            if (CardsElement?.GetKey() == cards?.GetKey())
            {
                CardsElement = null;
                MouseOverPoint = null;
            }
        }
        #endregion
    }
}
