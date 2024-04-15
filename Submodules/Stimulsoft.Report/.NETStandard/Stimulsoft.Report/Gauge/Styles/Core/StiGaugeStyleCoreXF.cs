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
using Stimulsoft.Base.Serializing;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Gauge
{
    public abstract class StiGaugeStyleCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a localized name of style.
        /// </summary>
        public abstract string LocalizedName { get; }
        #endregion

        #region Properties

        public abstract StiBrush Brush { get;}

        public abstract Color ForeColor { get; }

        public abstract Color BorderColor { get; }

        public abstract float BorderWidth { get; }

        public abstract Color TargetColor { get; }

        #region Scale

        #region TickMarkMajor        
        public abstract StiBrush TickMarkMajorBrush { get; }

        public abstract StiBrush TickMarkMajorBorder { get; }

        public virtual float TickMarkMajorBorderWidth => 0;
        #endregion

        #region TickMarkMinor
        public abstract StiBrush TickMarkMinorBrush { get; }

        public abstract StiBrush TickMarkMinorBorder { get; }

        public virtual float TickMarkMinorBorderWidth => 0;
        #endregion

        #region TickLabelMajor
        public abstract StiBrush TickLabelMajorTextBrush { get; }

        public abstract Font TickLabelMajorFont { get; }
        #endregion

        #region TickLabelMinor
        public abstract StiBrush TickLabelMinorTextBrush { get; }

        public abstract Font TickLabelMinorFont { get; }
        #endregion

        #region Linear Scale
        public abstract StiBrush LinearScaleBrush { get; }

        #region Bar
        public abstract StiBrush LinearBarBrush { get; }

        public abstract StiBrush LinearBarBorderBrush { get; }

        public abstract StiBrush LinearBarEmptyBrush { get; }

        public abstract StiBrush LinearBarEmptyBorderBrush { get; }

        public abstract float LinearBarStartWidth { get; }

        public abstract float LinearBarEndWidth { get; }
        #endregion

        #endregion

        #region Radial Scale

        #region Bar
        public abstract StiBrush RadialBarBrush { get; }

        public abstract StiBrush RadialBarBorderBrush { get; }

        public abstract StiBrush RadialBarEmptyBrush { get; }

        public abstract StiBrush RadialBarEmptyBorderBrush { get; }

        public abstract float RadialBarStartWidth { get; }

        public abstract float RadialBarEndWidth { get; }
        #endregion

        #region Needle
        public abstract StiBrush NeedleBrush { get; }

        public abstract StiBrush NeedleBorderBrush { get; }

        public abstract StiBrush NeedleCapBrush { get; }

        public abstract StiBrush NeedleCapBorderBrush { get; }

        public abstract float NeedleBorderWidth { get; }

        public abstract float NeedleCapBorderWidth { get; }

        public abstract float NeedleStartWidth { get; }

        public abstract float NeedleEndWidth { get; }

        public abstract float NeedleRelativeHeight { get; }

        public abstract float NeedleRelativeWith { get; }
        #endregion

        #endregion

        #region Marker
        public virtual StiMarkerSkin MarkerSkin => StiMarkerSkin.TriangleRight;

        public abstract StiBrush MarkerBrush { get; }

        public abstract StiBrush LinearMarkerBorder { get; }

        public virtual StiBrush MarkerBorderBrush => new StiEmptyBrush();

        public virtual float MarkerBorderWidth => 0f;
        #endregion

        #endregion

        [StiNonSerialized]
        public virtual StiGaugeStyleId StyleId => StiGaugeStyleId.StiStyle26;

        [Browsable(false)]
        public StiGauge Gauge { get; set; }
        #endregion
    }
}