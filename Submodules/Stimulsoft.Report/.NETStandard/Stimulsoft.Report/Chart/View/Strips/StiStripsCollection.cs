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

using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.ComponentModel;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiSerialConverter))]
    public class StiStripsCollection :
        CollectionBase,
        ICloneable,
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
            foreach (IStiStrips strips in List)
            {
                jObject.AddPropertyJObject(index.ToString(), strips.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var strips = new StiStrips();
                strips.Chart = this.Chart;

                Add(strips);
                strips.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region ICloneable.override
        public object Clone()
        {
            var strips = new StiStripsCollection();
            foreach (StiStrips strip in this)
            {
                strips.Add(strip.Clone() as StiStrips);
            }
            return strips;
        }
        #endregion

        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public void ApplyStyle(IStiChartStyle style)
        {
            foreach (IStiStrips strips in this)
            {
                strips.Core.ApplyStyle(style);
            }
        }
        #endregion

        #region Methods
        private string GetStripsTitle()
        {
            string baseTitle = StiLocalization.Get("Chart", "Strip");
            string title = baseTitle;

            int index = 1;
            bool finded = true;

            while (finded)
            {
                title = $"{baseTitle} {index}";
                finded = false;
                foreach (IStiStrips line in this)
                {
                    if (line.Text == title)
                    {
                        finded = true;
                        break;
                    }
                }
                index++;
            }
            return title;
        }

        private void AddCore(IStiStrips value)
        {
            if (Chart != null)
            {
                if (string.IsNullOrEmpty(value.Text)) 
                    value.Text = GetStripsTitle();

                value.Chart = Chart;
            }

            base.List.Add(value);
        }

        public void Add(IStiStrips value)
        {
            AddCore(value);
            InvokeStripsAdded(value, List.Count - 1);
        }

        public void AddRange(IStiStrips[] values)
        {
            foreach (IStiStrips value in values)
            {
                AddCore(value);
                InvokeStripsAdded(value, List.Count - 1);
            }
        }

        public void AddRange(StiStripsCollection values)
        {
            foreach (IStiStrips value in values)
            {
                AddCore(value);
                InvokeStripsAdded(value, List.Count - 1);
            }
        }

        public bool Contains(IStiStrips value)
        {
            return List.Contains(value);
        }

        public int IndexOf(IStiStrips value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, IStiStrips value)
        {
            List.Insert(index, value);

            InvokeStripsAdded(value, index);
        }

        protected override void OnClear()
        {
            while (List.Count > 0)
            {
                InvokeStripsRemoved(List[0], 0);
                List.RemoveAt(0);
            }
        }

        public void Remove(IStiStrips value)
        {
            InvokeStripsRemoved(value, List.IndexOf(value));
            List.Remove(value);
        }

        public IStiStrips this[int index]
        {
            get
            {
                return (IStiStrips)InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }
        #endregion

        #region Events

        #region StripsAdded
        public event EventHandler StripsAdded;

        protected virtual void OnStripsAdded(EventArgs e)
        {
        }

        private void InvokeStripsAdded(object sender, int index)
        {
            StripsAdded?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #region StripsRemoved
        public event EventHandler StripsRemoved;

        protected virtual void OnStripsRemoved(EventArgs e)
        {
        }

        private void InvokeStripsRemoved(object sender, int index)
        {
            StripsRemoved?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #endregion

        #region Properties
        [Browsable(false)]
        public StiChart Chart { get; set; }
        #endregion

        [StiUniversalConstructor("Strips")]
        public StiStripsCollection()
        {
        }
    }
}
