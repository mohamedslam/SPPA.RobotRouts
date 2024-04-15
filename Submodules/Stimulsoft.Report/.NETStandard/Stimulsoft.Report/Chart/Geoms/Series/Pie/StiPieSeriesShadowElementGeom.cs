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
using System.Drawing;
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Report.Chart
{
    public class StiPieSeriesShadowElementGeom : StiCellGeom
    {
        #region Properties
        /// <summary>
        /// Gets value which indicates that this geom object is inivisible 
        /// </summary>
        public override bool Invisible
        {
            get
            {
                return true;
            }
        }

        private IStiPieSeries series;
        public IStiPieSeries Series
        {
            get
            {
                return series;
            }
        }


        private StiContext shadowContext;
        public StiContext ShadowContext
        {
            get
            {
                return shadowContext;
            }
        }

        private float radius;
        public float Radius
        {
            get
            {
                return radius;
            }
        }

        private TimeSpan duration;
        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
        }

        private TimeSpan beginTime;
        public TimeSpan BeginTime
        {
            get
            {
                return beginTime;
            }
        }

        private bool isAnimation;
        public bool IsAnimation
        {
            get
            {
                return isAnimation;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rectPie = this.ClientRectangle;

            if (isAnimation)
            {
                var animationOpacity = new StiOpacityAnimation(duration, beginTime);
                context.DrawShadowCircle(rectPie, rectPie.Height / 2, rectPie.Width / 2, (int)radius, animationOpacity);
            }
            else
            {
                context.DrawShadow(shadowContext, rectPie, radius);
            }
        }
        #endregion

        public StiPieSeriesShadowElementGeom(IStiPieSeries series, RectangleF clientRectangle, float radius, StiContext shadowContext, TimeSpan duration, TimeSpan beginTime)
            : base(clientRectangle)
        {
            this.series = series;
            this.shadowContext = shadowContext;
            this.radius = radius;

            this.duration = duration;
            this.beginTime = beginTime;
            this.isAnimation = ((StiChart)series.Chart).IsAnimation;
        }
    }
}
