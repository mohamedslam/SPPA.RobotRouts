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
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge
{
    public class StiCustomGaugeStyleCoreXF : StiGaugeStyleCoreXF25
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName => "CustomStyle";
        #endregion

        #region Properties
        public StiGaugeStyle ReportGaugeStyle { get; private set; }

        public string ReportStyleName => (ReportGaugeStyle == null) ? null : ReportGaugeStyle.Name;
        #endregion

        #region Properties.override
        public override StiBrush LinearScaleBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.LinearScaleBrush
                    : base.LinearScaleBrush;
            }
        }

        public override StiBrush Brush
        {
            get
            {
                return (ReportGaugeStyle != null) 
                    ? ReportGaugeStyle.Brush
                    : base.Brush;
            }
        }

        public override Color BorderColor
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.BorderColor
                    : base.BorderColor;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.ForeColor
                    : base.ForeColor;
            }
        }

        public override Color TargetColor
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TargetColor
                    : base.TargetColor;
            }
        }


        public override float BorderWidth
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.BorderWidth
                    : base.BorderWidth;
            }
        }

        #region Scale

        #region TickMarkMajor        
        public override StiBrush TickMarkMajorBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMajorBrush
                    : base.TickMarkMajorBrush;
            }
        }

        public override StiBrush TickMarkMajorBorder
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMajorBorder
                    : base.TickMarkMajorBorder;
            }
        }

        public override float TickMarkMajorBorderWidth
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMajorBorderWidth
                    : base.TickMarkMajorBorderWidth;
            }
        }
        #endregion

        #region TickMarkMinor
        public override StiBrush TickMarkMinorBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMinorBrush
                    : base.TickMarkMinorBrush;
            }
        }

        public override StiBrush TickMarkMinorBorder
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMinorBorder
                    : base.TickMarkMinorBorder;
            }
        }

        public override float TickMarkMinorBorderWidth
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickMarkMinorBorderWidth
                    : base.TickMarkMinorBorderWidth;
            }
        }
        #endregion

        #region TickLabelMajor
        public override StiBrush TickLabelMajorTextBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickLabelMajorTextBrush
                    : base.TickLabelMajorTextBrush;
            }
        }

        public override Font TickLabelMajorFont
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickLabelMajorFont
                    : base.TickLabelMajorFont;
            }
        }
        #endregion

        #region TickLabelMinor
        public override StiBrush TickLabelMinorTextBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickLabelMinorTextBrush
                    : base.TickLabelMinorTextBrush;
            }
        }

        public override Font TickLabelMinorFont
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.TickLabelMinorFont
                    : base.TickLabelMinorFont;
            }
        }
        #endregion

        #region Marker

        public override StiBrush MarkerBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.MarkerBrush
                    : base.MarkerBrush;
            }
        }

        #endregion

        #region Linear Scale

        #region Bar
        public override StiBrush LinearBarBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.LinearBarBrush
                    : new StiSolidBrush("#4472c4");
            }
        }

        public override StiBrush LinearBarBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.LinearBarBorderBrush
                    : new StiEmptyBrush();
            }
        }

        public override StiBrush LinearBarEmptyBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.LinearBarEmptyBrush
                    : new StiEmptyBrush();
            }
        }

        public override StiBrush LinearBarEmptyBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.LinearBarEmptyBorderBrush
                    : new StiEmptyBrush();
            }
        }

        public override float LinearBarStartWidth => 0.1f;

        public override float LinearBarEndWidth => 0.1f;
        #endregion

        #endregion

        #region Radial Scale

        #region Bar
        public override StiBrush RadialBarBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.RadialBarBrush
                    : new StiSolidBrush("#ffc000");
            }
        }

        public override StiBrush RadialBarBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.RadialBarBorderBrush
                    : new StiEmptyBrush();
            }
        }

        public override StiBrush RadialBarEmptyBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.RadialBarEmptyBrush
                    : new StiSolidBrush("#43682b");
            }
        }

        public override StiBrush RadialBarEmptyBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.RadialBarEmptyBorderBrush
                    : new StiEmptyBrush();
            }
        }

        public override float RadialBarStartWidth => 0.1f;

        public override float RadialBarEndWidth => 0.1f;
        #endregion

        #region Needle
        public override StiBrush NeedleBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleBrush
                    : new StiSolidBrush("#ffc000");
            }
        }

        public override StiBrush NeedleBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleBorderBrush
                    : new StiEmptyBrush();
            }
        }

        public override StiBrush NeedleCapBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleCapBrush
                    : new StiSolidBrush("#ffc000");
            }
        }

        public override StiBrush NeedleCapBorderBrush
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleCapBorderBrush
                    : new StiSolidBrush("#ffc000");
            }
        }

        public override float NeedleBorderWidth
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleBorderWidth
                    : base.NeedleBorderWidth;
            }
        }

        public override float NeedleCapBorderWidth
        {
            get
            {
                return (ReportGaugeStyle != null)
                    ? ReportGaugeStyle.NeedleBorderWidth
                    : base.NeedleBorderWidth;
            }
        }

        public override float NeedleStartWidth => 0.1f;

        public override float NeedleEndWidth => 1f;

        public override float NeedleRelativeHeight => 0.08f;

        public override float NeedleRelativeWith => 0.55f;
        #endregion

        #endregion

        #endregion
        #endregion

        public StiCustomGaugeStyleCoreXF(StiGaugeStyle style)
        {
            this.ReportGaugeStyle = style;
        }
    }
}