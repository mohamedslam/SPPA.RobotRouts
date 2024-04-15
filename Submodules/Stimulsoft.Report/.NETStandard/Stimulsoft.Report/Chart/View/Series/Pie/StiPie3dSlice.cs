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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Chart.Geoms.Series.Pie;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiPie3dSlice
    {
        #region Fields
        private RectangleF rectPieSlilce;
        private float pieHeight;
        private StiBrush seriesBrush;
        private Color borderColor;
        private IStiPie3dSeries series;
        private IStiArea area;        
        #endregion

        #region Properties
        public int ColorCount { get; private set; }

        public float StartAngle { get; private set; }
        
        public float SweepAngle { get; private set; }

        public float EndAngle
        {
            get { return (StartAngle + SweepAngle) % 360; }
        }

        public double Value { get; private set; }

        public string ArgumentText { get; private set; }

        public string Tag { get; private set; }

        public int Index { get; private set; }

        public int ColorIndex { get; private set; }

        public PointF TextPosition {  get; private set; }

        public List<StiSeriesElementGeom> ChildGeoms { get; private set; }

        public StiSeriesInteractionData Interaction { get; private set; }

        public bool StartSideExists { get; private set; } = true;

        public bool EndSideExists { get; private set; } = true;
        #endregion

        #region Methods
        internal void DrawLabels(StiAreaGeom areaGeom, StiContext context)
        {
            var seriesLabels = ((StiPie3dSeriesCoreXF)series.Core).GetPieSeriesLabels();

            if (seriesLabels != null && !(seriesLabels is StiNoneLabels) && seriesLabels.Visible)
            {
                if (seriesLabels.Core is StiCenterPie3dLabelsCoreXF core)
                {
                    var geom = core.RenderLabel(series, context, this);

                    areaGeom.CreateChildGeoms();
                    areaGeom.ChildGeoms.Add(geom);
                }
            }
        }

        internal void DrawTopPieSliceGeom(StiAreaGeom areaGeom)
        {
            var geom = new StiPie3dMainSideSeriesElementGeom(this, areaGeom, this.Value, Index, ColorCount, series, this.rectPieSlilce, borderColor, this.seriesBrush, this.StartAngle, this.SweepAngle);

            this.ChildGeoms.Add(geom);

            areaGeom.CreateChildGeoms();
            areaGeom.ChildGeoms.Add(geom);
        }

        internal void DrawBottomPieSliceGeom(StiAreaGeom areaGeom)
        {
            var bottomRectPieSlice = new RectangleF(this.rectPieSlilce.X, this.rectPieSlilce.Y + this.pieHeight, this.rectPieSlilce.Width, this.rectPieSlilce.Height);

            var geom = new StiPie3dMainSideSeriesElementGeom(this, areaGeom, this.Value, Index, ColorCount, series, bottomRectPieSlice, borderColor, this.seriesBrush, this.StartAngle, this.SweepAngle);

            areaGeom.CreateChildGeoms();
            areaGeom.ChildGeoms.Add(geom);
        }

        internal void DrawSides(StiAreaGeom areaGeom)
        {
            var geom = new StiPie3dSidesSeriesElementGeom(this, areaGeom, this.Value, ColorIndex, ColorCount, series, this.rectPieSlilce, borderColor, this.seriesBrush,
                this.StartAngle, this.SweepAngle, this.pieHeight, this.StartSideExists, this.EndSideExists);

            this.ChildGeoms.Add(geom);
            TextPosition = geom.GetTextPosition();

            areaGeom.CreateChildGeoms();
            areaGeom.ChildGeoms.Add(geom);
        }

        internal void InitTextPosition(StiAreaGeom areaGeom)
        {
            var geom = new StiPie3dSidesSeriesElementGeom(this, areaGeom, this.Value, ColorIndex, ColorCount, series, this.rectPieSlilce, borderColor, this.seriesBrush,
                this.StartAngle, this.SweepAngle, this.pieHeight, this.StartSideExists, this.EndSideExists);

            TextPosition = geom.GetTextPosition();
        }

        internal StiPie3dSlice[] Split(float splitAngle)
        {
            // if split angle equals one of bounding angles, then nothing to split
            if (StartAngle == splitAngle || this.EndAngle == splitAngle)
                return new StiPie3dSlice[] { this.MemberwiseClone() as StiPie3dSlice };

            float actualStartAngle = StartAngle;
            float newSweepAngle = (splitAngle - actualStartAngle + 360) % 360;
            var pieSlice1 = GetNewModified(actualStartAngle, newSweepAngle);
            pieSlice1.StartSideExists = true;
            pieSlice1.EndSideExists = false;

            newSweepAngle = EndAngle - splitAngle;
            var pieSlice2 = GetNewModified(splitAngle, newSweepAngle);
            pieSlice2.StartSideExists = false;
            pieSlice2.EndSideExists = true;

            return new StiPie3dSlice[] { pieSlice1, pieSlice2 };
        }

        private StiPie3dSlice GetNewModified(float startAngle, float sweepAngle)
        {
            var slice = new StiPie3dSlice(this.area, this.Value, this.ArgumentText, this.Tag, this.Index, this.series, this.rectPieSlilce,
                this.pieHeight, startAngle, sweepAngle, this.seriesBrush, this.borderColor, this.ColorIndex, this.ColorCount);

            slice.TextPosition = this.TextPosition;

            return slice;
        }
        #endregion

        public StiPie3dSlice(IStiArea area, double? value, string argumentText, string tag, int index, IStiPie3dSeries series, RectangleF rectPieSlilce,
            float pieHeight, float startAngle, float sweepAngle, StiBrush seriesBrush, Color borderColor, int colorIndex, int colorCount)
        {
            this.Value = value.GetValueOrDefault();
            this.ArgumentText = argumentText;
            this.Tag = tag;
            this.Index = index;
            this.ColorIndex = colorIndex;
            this.ColorCount = colorCount;

            this.area = area;
            this.series = series; 
            this.rectPieSlilce = rectPieSlilce;
            this.pieHeight = pieHeight;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
            this.seriesBrush = seriesBrush;
            this.borderColor = borderColor;

            this.ChildGeoms = new List<StiSeriesElementGeom>();

            if (series.Core.Interaction != null)
                this.Interaction = new StiSeriesInteractionData(area, series, ColorIndex);
        }
    }
}
