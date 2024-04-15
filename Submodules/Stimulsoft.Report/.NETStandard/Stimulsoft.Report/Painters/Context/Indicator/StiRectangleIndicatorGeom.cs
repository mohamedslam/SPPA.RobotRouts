#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.Net											}
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
using Stimulsoft.Base.Indicator;
using System.Drawing;

namespace Stimulsoft.Base.Gauge.GaugeGeoms
{
    public class StiRectangleIndicatorGeom : StiIndicatorGeom
    {
        #region Properties
        public RectangleF Rect { get; }

        public StiBrush Background { get; }

        public StiBrush BorderBrush { get; }

        public float BorderWidth { get; }
        #endregion

        #region Properties.Override
        public override StiIndicatorGeomType Type => StiIndicatorGeomType.Rectangle;
        #endregion

        public StiRectangleIndicatorGeom(RectangleF rect, StiBrush background, StiBrush borderBrush, float borderWidth)
        {
            this.Rect = rect;
            this.Background = background;
            this.BorderBrush = borderBrush;
            this.BorderWidth = borderWidth;
        }
    }
}