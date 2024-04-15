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

using System;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiInsideEndAxisLabelsCoreXF : StiCenterAxisLabelsCoreXF
    {
        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.InsideEndAxis;
            }
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "LabelsInsideEnd");
            }
        }
        #endregion

        #region Methods
        protected override RectangleF GetLabelRect(StiContext context, PointF endPoint, PointF startPoint,
            double? value, string labelText, bool checkHeight, StiFontGeom font, StiStringFormatGeom sf)
        {
            SizeF size = context.MeasureString(labelText, font);
            const int deltaWidth = 2;
            if (this.SeriesLabels.Chart.Area.Core.SeriesOrientation == StiChartSeriesOrientation.Vertical)
            {
                
                if (value > 0)
                {
                    var textPointY = endPoint.Y + (float)(Math.Abs(size.Height * Math.Cos(SeriesLabels.Angle * Math.PI / 180)) + Math.Abs((size.Width / 2 + deltaWidth) * Math.Sin(SeriesLabels.Angle * Math.PI / 180)));
                    if (textPointY > startPoint.Y)
                        textPointY = endPoint.Y + (startPoint.Y - endPoint.Y) / 2;

                    return context.MeasureRotatedString(labelText, font,
                        new PointF(endPoint.X, textPointY),
                        sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
                }
                else
                {
                    var textPointY = endPoint.Y - (float)(Math.Abs(size.Height * Math.Cos(SeriesLabels.Angle * Math.PI / 180)) + Math.Abs((size.Width / 2 + deltaWidth) * Math.Sin(SeriesLabels.Angle * Math.PI / 180)));
                    if (textPointY < startPoint.Y)
                        textPointY = endPoint.Y - (endPoint.Y - startPoint.Y) / 2;

                    return context.MeasureRotatedString(labelText, font,
                        new PointF(endPoint.X, textPointY),
                        sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
                }
            }
            else
            {
                if (value > 0)
                {
                    return context.MeasureRotatedString(labelText, font,
                        new PointF(endPoint.X + (float)(Math.Abs(size.Height * Math.Sin(SeriesLabels.Angle * Math.PI / 180)) + Math.Abs((size.Width / 2 + deltaWidth) * Math.Cos(SeriesLabels.Angle * Math.PI / 180))), endPoint.Y),
                        sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
                }
                else
                {
                    return context.MeasureRotatedString(labelText, font,
                        new PointF(endPoint.X - (float)(Math.Abs(size.Height * Math.Sin(SeriesLabels.Angle * Math.PI / 180)) + Math.Abs((size.Width / 2 + deltaWidth) * Math.Cos(SeriesLabels.Angle * Math.PI / 180))), endPoint.Y),
                        sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
                }
            }
        }
        #endregion

        public StiInsideEndAxisLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
    }
}
