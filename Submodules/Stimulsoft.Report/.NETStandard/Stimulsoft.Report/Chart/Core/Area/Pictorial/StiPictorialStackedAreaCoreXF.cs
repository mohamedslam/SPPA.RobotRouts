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
using Stimulsoft.Base.Localization;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiPictorialStackedAreaCoreXF : StiAreaCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "PictorialStacked");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.PictorialStacked;
            }
        }
        #endregion

        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            var pictorialAreaGeom = new StiPictorialAreaGeom(this.Area, rect);
            var seriesCollection = GetSeries();

            context.PushTranslateTransform(rect.X, rect.Y);

            if (seriesCollection.Count > 0)
                seriesCollection[0].Core.RenderSeries(context, new RectangleF(0, 0, rect.Width, rect.Height), pictorialAreaGeom, seriesCollection.ToArray());

            context.PopTransform();
            return pictorialAreaGeom;
        }

        protected override void PrepareInfo(RectangleF rect)
        {
        }
        #endregion

        public StiPictorialStackedAreaCoreXF(IStiArea area)
            : base(area)
        {
        }
    }
}
