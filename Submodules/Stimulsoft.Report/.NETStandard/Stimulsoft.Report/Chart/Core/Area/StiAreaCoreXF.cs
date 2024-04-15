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
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{	
	/// <summary>
	/// Describes base class for all chart areas.
	/// </summary>
    public abstract class StiAreaCoreXF : 
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
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (this.Area.AllowApplyStyle)
            {
                this.Area.Brush = style.Core.ChartAreaBrush as StiBrush;
                this.Area.BorderColor = style.Core.ChartAreaBorderColor;
                this.Area.BorderThickness = style.Core.ChartAreaBorderThickness;
                this.Area.ShowShadow = style.Core.ChartAreaShowShadow;
            }
        }
        #endregion
                
        #region Methods
        public abstract StiCellGeom Render(StiContext context, RectangleF rect);
        
        protected abstract void PrepareInfo(RectangleF rect);

        /// <summary>
        /// Internal use only.
        /// </summary>
        public bool CheckInLabelsTypes(Type typeForCheck)
        {
            Type[] types = area.GetSeriesLabelsTypes();
            foreach (Type type in types)
            {
                if (type.ToString() == typeForCheck.ToString()) return true;
            }
            return false;
        }


        /// <summary>
        /// Returns collections of corect series for this area.
        /// </summary>
        /// <returns></returns>
        public List<IStiSeries> GetSeries()
        {
            var cachedSeriesTypes = new Hashtable();
            var types = Area.GetSeriesTypes();
            foreach (Type type in types)
            {
                cachedSeriesTypes[type] = type;
            }

            var newSeries = new List<IStiSeries>();
            foreach (IStiSeries series in this.Area.Chart.Series)
            {
                if (cachedSeriesTypes[series.GetType()] == null) continue;
                newSeries.Add(series);
            }
            return newSeries;
        }

        public bool IsAcceptableSeries(Type seriesType)
        {
            Type[] types = Area.GetSeriesTypes();
            foreach (Type type in types)
            {
                if (type == seriesType)
                    return true;
            }
            return false;
        }

        public bool IsAcceptableSeriesLabels(Type seriesLabelsType)
        {
            Type[] types = Area.GetSeriesLabelsTypes();
            foreach (Type type in types)
            {
                if (type == seriesLabelsType)
                    return true;
            }
            return false;
        }
        #endregion

        #region Properties
        private IStiArea area;
        public IStiArea Area
        {
            get
            {
                return area;
            }
            set
            {
                area = value;
            }
        }
        #endregion

        #region Properties.Localization
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public abstract string LocalizedName
        {
            get;
        }
        #endregion

        #region Properties.Settings
        /// <summary>
        /// Gets series orientation of this area.
        /// </summary>
        [Browsable(false)]
        public virtual StiChartSeriesOrientation SeriesOrientation
        {
            get
            {
                return StiChartSeriesOrientation.Vertical;
            }
        }

        /// <summary>
        /// Gets position of this area.
        /// </summary>
        [Browsable(false)]
        public abstract int Position
        {
            get;
        }
        #endregion

        public StiAreaCoreXF(IStiArea area)
        {
            this.area = area;
        }
	}
}
