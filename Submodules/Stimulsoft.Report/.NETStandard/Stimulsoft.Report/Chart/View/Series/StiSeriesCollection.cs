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
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
	[TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiSerialConverter))]
    public class StiSeriesCollection : 
        CollectionBase,
        IStiApplyStyle,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (IStiSeries component in List)
            {
                jObject.AddPropertyJObject(index.ToString(), component.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            Clear();

            foreach (var property in jObject.Properties())
            {
                var propJObject = (JObject)property.Value;
                var ident = propJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

                var series = StiOptions.Services.ChartSeries.FirstOrDefault(x => x.GetType().Name == ident);
                if (series == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var seriesClone = series.CreateNew();
                seriesClone.Chart = this.Chart;

                seriesClone.LoadFromJsonObject((JObject)property.Value);
                Add(seriesClone);
            }
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (style == null) return;

            #region Apply style to all series
            int index = 0;
            foreach (IStiSeries series in this)
            {
                var colors = style.Core.GetColors(Count, series?.Core?.SeriesColors);
                series.Core?.ApplyStyle(style, colors[index++]);
            }
            #endregion
        }
        #endregion

        #region Methods
        public StiSeries FirstOrDefault()
        {
            return ToList().FirstOrDefault();
        }

        public List<StiSeries> ToList()
        {
            return this.Cast<StiSeries>().ToList();
        }

        private string GetSeriesTitle()
        {
            string baseTitle = StiLocalization.Get("Chart", "Series");
            string title = baseTitle;

            int index = 1;
            bool finded = true;

            while (finded)
            {
                title = $"{baseTitle} {index.ToString()}";
                finded = false;
                foreach (IStiSeries series in this)
                {
                    if (series.CoreTitle == title)
                    {
                        finded = true;
                        break;
                    }
                }
                index++;
            }
            return title;
        }

        private void AddCore(IStiSeries value)
        {
			if (Chart != null)
			{
				if (string.IsNullOrEmpty(value.CoreTitle))
                    value.CoreTitle = GetSeriesTitle();

				value.Chart = Chart;
			}

            base.List.Add(value);			
        }

        public void Add(IStiSeries value)
        {
            AddCore(value);
            InvokeSeriesAdded(value);
        }

        public void AddRange(IStiSeries[] values)
        {
            foreach (IStiSeries value in values)
            {
                AddCore(value);
                InvokeSeriesAdded(value);
            }
        }

        public void AddRange(StiSeriesCollection values)
        {
            foreach (IStiSeries value in values)
            {
                AddCore(value);
                InvokeSeriesAdded(value);
            }
        }

        public bool Contains(IStiSeries value)
        {
            return List.Contains(value);
        }

        public int IndexOf(IStiSeries value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, IStiSeries value)
        {
            if (Chart != null)
            {
                if (string.IsNullOrEmpty(value.CoreTitle))
                    value.CoreTitle = GetSeriesTitle();

                value.Chart = Chart;
            }

            List.Insert(index, value);

            InvokeSeriesAdded(value);
        }

        protected override void OnClear()
        {
            while (List.Count > 0)
            {
                List.RemoveAt(0);
            }
        }

        public void Remove(IStiSeries value)
        {            
            List.Remove(value);
        }
        
        public IStiSeries this[int index]
        {
            get
            {
                return (IStiSeries)InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }

        public IStiSeries this[string name]
        {
            get
            {
                foreach (IStiSeries series in List)
                {
                    if (series.CoreTitle == name)
                    {
                        return series;
                    }
                }
                return null;
            }
            set
            {
                for (int index = 0; index < List.Count; index++)
                {
                    IStiSeries series = List[index] as IStiSeries;

                    if (series.CoreTitle == name)
                    {
                        List[index] = value;
                        return;
                    }
                }
                Add(value);
            }
        }
		#endregion

        #region Events
        #region SeriesAdded
        public event EventHandler SeriesAdded;

        protected virtual void OnSeriesAdded(EventArgs e)
        {
        }

        private void InvokeSeriesAdded(object sender)
        {
            SeriesAdded?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #region SeriesRemoved
        protected override void OnRemove(int index, object value)
        {
            base.OnRemove(index, value);

            InvokeSeriesRemoved(value);
        }

        public event EventHandler SeriesRemoved;

        protected virtual void OnSeriesRemoved(EventArgs e)
        {
        }

        private void InvokeSeriesRemoved(object sender)
        {
            SeriesRemoved?.Invoke(sender, EventArgs.Empty);
        }
        #endregion
        #endregion

        #region Properties
        [Browsable(false)]
        public StiChart Chart { get; set; }
        #endregion

        [StiUniversalConstructor("Series")]
		public StiSeriesCollection()
		{            
		}
    }
}
