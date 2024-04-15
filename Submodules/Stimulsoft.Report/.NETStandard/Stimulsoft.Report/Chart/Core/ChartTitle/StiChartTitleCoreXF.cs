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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiChartTitleCoreXF : 
        ICloneable,
        IStiApplyStyle
	{
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.ChartTitle.AllowApplyStyle)
            {
                this.ChartTitle.Brush = new StiSolidBrush(style.Core.AxisTitleColor);
            }
        }
        #endregion

        #region Methods
        public StiCellGeom Render(StiContext context, IStiChartTitle chartTitle, RectangleF rect)
        {
            if (!chartTitle.Visible)
                return null;

            StiFontGeom font = new StiFontGeom(chartTitle.Font);
            StiStringFormatGeom sf = context.GetDefaultStringFormat();
            sf.Alignment = chartTitle.Alignment;
            RectangleF fullRectangle = context.MeasureRotatedString(chartTitle.Text, font, rect, sf, StiRotationMode.CenterCenter, (float)chartTitle.Dock);

            switch (chartTitle.Dock)
            {
                #region StiChartTitleDockXF.Top
                case StiChartTitleDock.Top:
                    fullRectangle.Height += chartTitle.Spacing;
                    fullRectangle.Height *= context.Options.Zoom;
                    fullRectangle.Y = rect.Y;
                    if (chartTitle.Alignment != StringAlignment.Center) fullRectangle.Width *= context.Options.Zoom;
                    if (chartTitle.Alignment == StringAlignment.Near) fullRectangle.X = rect.X;
                    if (chartTitle.Alignment == StringAlignment.Far) fullRectangle.X = rect.X + rect.Width - fullRectangle.Width;
                    break;
                #endregion

                #region StiChartTitleDockXF.Right
                case StiChartTitleDock.Right:
                    fullRectangle.Width += chartTitle.Spacing;
                    fullRectangle.Width *= context.Options.Zoom;
                    fullRectangle.X = rect.X + rect.Width - fullRectangle.Width;
                    if (chartTitle.Alignment != StringAlignment.Center) fullRectangle.Height *= context.Options.Zoom;
                    if (chartTitle.Alignment == StringAlignment.Near) fullRectangle.Y = rect.Y;
                    if (chartTitle.Alignment == StringAlignment.Far) fullRectangle.Y = rect.Y + rect.Height - fullRectangle.Height;
                    break;
                #endregion

                #region StiChartTitleDockXF.Bottom
                case StiChartTitleDock.Bottom:
                    fullRectangle.Height += chartTitle.Spacing;
                    fullRectangle.Height *= context.Options.Zoom;
                    fullRectangle.Y = rect.Y + rect.Height - fullRectangle.Height;
                    if (chartTitle.Alignment != StringAlignment.Center) fullRectangle.Width *= context.Options.Zoom;
                    if (chartTitle.Alignment == StringAlignment.Near) fullRectangle.X = rect.X + rect.Width - fullRectangle.Width;
                    if (chartTitle.Alignment == StringAlignment.Far) fullRectangle.X = rect.X;
                    break;
                #endregion

                #region StiChartTitleDockXF.Left
                case StiChartTitleDock.Left:
                    fullRectangle.Width += chartTitle.Spacing;
                    fullRectangle.Width *= context.Options.Zoom;
                    fullRectangle.X = rect.X;
                    if (chartTitle.Alignment != StringAlignment.Center) fullRectangle.Height *= context.Options.Zoom;
                    if (chartTitle.Alignment == StringAlignment.Near) fullRectangle.Y = rect.Y + rect.Height - fullRectangle.Height;
                    if (chartTitle.Alignment == StringAlignment.Far) fullRectangle.Y = rect.Y;
                    break;
                #endregion
            }

            return new StiChartTitleGeom(chartTitle, fullRectangle);
        }
        #endregion

        #region Properties
        private IStiChartTitle chartTitle;
        public IStiChartTitle ChartTitle
        {
            get
            {
                return chartTitle;
            }
            set
            {
                chartTitle = value;
            }
        }
        #endregion

        public StiChartTitleCoreXF(IStiChartTitle chartTitle)
        {
            this.chartTitle = chartTitle;
        }
	}
}
