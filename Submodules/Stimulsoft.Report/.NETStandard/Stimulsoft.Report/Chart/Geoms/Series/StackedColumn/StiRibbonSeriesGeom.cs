#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Collections.Generic;
using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiRibbonSeriesGeom : StiSeriesGeom
    {
        #region Fields
        private StiRibbonSeriesMetadata metadata;

        private TimeSpan beginTime; 
        #endregion

        #region Methods
        internal static RectangleF GetClientRectangle(List<RectangleF> rectangles)
        {
            var mainRect = RectangleF.Empty;

            foreach (var metadataRect in rectangles)
            {
                mainRect = RectangleF.Union(mainRect, metadataRect);
            }

            return mainRect;
        }

        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var path = new List<StiSegmentGeom>();
            var delta = 3;

            for (int index = 0; index < metadata.Rectangles.Count; index++)
            {
                var firstRect = metadata.Rectangles[index];
                var nextRect = index == metadata.Rectangles.Count - 1
                    ? RectangleF.Empty
                    : metadata.Rectangles[index + 1];

                path.Add(new StiLineSegmentGeom(firstRect.X, firstRect.Y, firstRect.Right, firstRect.Y));

                if (index == metadata.Rectangles.Count - 1)
                {
                    path.Add(new StiLineSegmentGeom(firstRect.Right, firstRect.Y, firstRect.Right, firstRect.Bottom));
                }

                else
                {
                    var pt1 = new PointF(firstRect.Right, firstRect.Y);
                    var pt2 = new PointF(nextRect.X - (nextRect.X - firstRect.Right) / delta, firstRect.Y);
                    var pt3 = new PointF(firstRect.Right + (nextRect.X - firstRect.Right) / delta, nextRect.Y);
                    var pt4 = new PointF(nextRect.X, nextRect.Y);

                    path.Add(new StiBezierSegmentGeom(pt1, pt2, pt3, pt4));
                }
            }

            for (var index = metadata.Rectangles.Count - 1; index >= 0; index--)
            {
                var lastRect = metadata.Rectangles[index];
                var nextRect = index == 0
                    ? RectangleF.Empty
                    : metadata.Rectangles[index - 1];

                path.Add(new StiLineSegmentGeom(lastRect.Right, lastRect.Bottom, lastRect.X, lastRect.Bottom));

                if (index == 0)
                {
                    path.Add(new StiLineSegmentGeom(lastRect.Right, lastRect.Bottom, lastRect.X, lastRect.Bottom));
                }

                else
                {
                    var pt1 = new PointF(lastRect.X, lastRect.Bottom);
                    var pt2 = new PointF(nextRect.Right + (lastRect.X - nextRect.Right) / delta, lastRect.Bottom);
                    var pt3 = new PointF(lastRect.X - (lastRect.X - nextRect.Right) / delta, nextRect.Bottom);
                    var pt4 = new PointF(nextRect.Right, nextRect.Bottom);

                    path.Add(new StiBezierSegmentGeom(pt1, pt2, pt3, pt4));
                }
            }

            var chart = this.Series.Chart as StiChart;
            context.PushSmoothingModeToAntiAlias();
            if (chart.IsAnimation)
            {
                var animationOpacity = new StiOpacityAnimation(TimeSpan.FromSeconds(1), this.beginTime);
                context.FillDrawAnimationPath(metadata.Brush, null, path, this.ClientRectangle, null, animationOpacity);
            }

            else
                context.FillPath(metadata.Brush, path, this.ClientRectangle, null);
            context.PopSmoothingMode();
        }
        #endregion

        public StiRibbonSeriesGeom(StiAreaGeom areaGeom, StiRibbonSeriesMetadata metadata, IStiSeries series, TimeSpan beginTime)
            : base(areaGeom, series, GetClientRectangle(metadata.Rectangles))
        {
            this.metadata = metadata;
            this.beginTime = beginTime;
        }
    }
}
