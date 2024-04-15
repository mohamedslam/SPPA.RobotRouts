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
    public class StiValueAxisLabelsCoreXF : StiCenterAxisLabelsCoreXF
    {
        #region Properties.Localization
        /// <summary>
		/// Gets a service name.
		/// </summary>
        public override string LocalizedName
		{
			get
			{
				return StiLocalization.Get("PropertyMain", "Value");
			}
		}
		#endregion

        #region Properties
        public override int Position
        {
            get
            {
                return (int)StiSeriesLabelsPosition.Value;
            }
        }
        #endregion

        #region Methods
        protected override RectangleF GetLabelRect(StiContext context, PointF endPoint, PointF startPoint,
            double? value, string labelText, bool checkHeight, StiFontGeom font, StiStringFormatGeom sf)
        {
            Rectangle labelRect = Rectangle.Round(context.MeasureRotatedString(labelText, font, new PointF(0, 0), sf, StiRotationMode.CenterCenter, 0, (int)(this.SeriesLabels.Width * context.Options.Zoom)));

            return new RectangleF(endPoint.X - labelRect.Width / 2, endPoint.Y - labelRect.Height / 2, labelRect.Width, labelRect.Height);
        }
        #endregion

        public StiValueAxisLabelsCoreXF(IStiSeriesLabels seriesLabels)
            : base(seriesLabels)
        {            
        }
	}
}
