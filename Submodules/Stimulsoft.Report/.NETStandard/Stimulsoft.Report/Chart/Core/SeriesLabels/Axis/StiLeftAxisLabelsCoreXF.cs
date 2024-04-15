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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiLeftAxisLabelsCoreXF : StiCenterAxisLabelsCoreXF
    {
        #region Properties.Localization
        /// <summary>
		/// Gets a service name.
		/// </summary>
        public override string LocalizedName
		{
			get
			{
				return StiLocalization.Get("PropertyMain", "Left");
			}
		}
		#endregion

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.Left;
            }
        }
        #endregion

        #region Methods
        protected override RectangleF GetLabelRect(StiContext context, PointF endPoint, PointF startPoint,
            double? value, string labelText, bool checkHeight, StiFontGeom font, StiStringFormatGeom sf)
        {
            SizeF size = context.MeasureString(labelText, font);
            if (this.SeriesLabels.Width > 0)
            {
                size.Width = this.SeriesLabels.Width * context.Options.Zoom;
            }
            else
            {
                size.Width += context.Options.Zoom * 2;
            }

            if (!((IStiAxisArea)SeriesLabels.Chart.Area).ReverseHor)
            {
                return context.MeasureRotatedString(labelText, font,
                    new PointF(endPoint.X - size.Width, endPoint.Y),
                    sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
            }
            else
            {
                return context.MeasureRotatedString(labelText, font,
                    new PointF(endPoint.X + size.Width, endPoint.Y),
                    sf, StiRotationMode.CenterCenter, 0f, (int)(this.SeriesLabels.Width * context.Options.Zoom));
            }
        }
        #endregion

        public StiLeftAxisLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
