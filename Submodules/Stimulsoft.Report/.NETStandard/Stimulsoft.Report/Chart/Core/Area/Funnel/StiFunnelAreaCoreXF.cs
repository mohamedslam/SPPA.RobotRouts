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
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiFunnelAreaCoreXF : StiAreaCoreXF
    {
        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string LocalizedName
        {
            get
            {
                return StiLocalization.Get("Chart", "Funnel");
            }
        }
        #endregion

        #region Properties.Settings
        public override int Position
        {
            get
            {
                return (int)StiChartAreaPosition.Funnel;
            }
        }
        #endregion  

        #region Methods
        public override StiCellGeom Render(StiContext context, RectangleF rect)
        {
            var areaGeom = new StiPieAreaGeom(this.Area, rect);
            
            #region Render Series
            List<IStiSeries> seriesCollection = GetSeries();
            RenderSeries(context, rect, areaGeom, seriesCollection);
            #endregion

            return areaGeom;
        } 

        public void RenderSeries(StiContext context, RectangleF rect, StiAreaGeom geom, List<IStiSeries> seriesCollection)
        {
            var seriesTypes = new List<List<IStiSeries>>();
            var seriesTypesHash = new Hashtable();
            foreach (var ser in seriesCollection)
            {
                var list = seriesTypesHash[ser.GetType()] as List<IStiSeries>;
                if (list == null)
                {
                    list = new List<IStiSeries>();
                    seriesTypes.Add(list);
                    seriesTypesHash.Add(ser.GetType(), list);
                }

                list.Add(ser);
            }

            foreach (var seriesType in seriesTypes)
            {
                var seriesArray = seriesType.ToArray();
                seriesArray[0].Core.RenderSeries(context, rect, geom, seriesArray);
            }
        }        

        protected override void PrepareInfo(RectangleF rect)
        {   
        }
        #endregion        

        public StiFunnelAreaCoreXF(IStiArea area)
            : base(area)
        {            
        }
    }
}
