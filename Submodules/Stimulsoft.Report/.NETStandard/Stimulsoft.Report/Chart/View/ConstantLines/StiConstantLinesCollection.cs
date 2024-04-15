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
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(Stimulsoft.Report.Chart.Design.StiSerialConverter))]
    public class StiConstantLinesCollection :
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
            foreach (IStiConstantLines constantLines in List)
            {
                jObject.AddPropertyJObject(index.ToString(), constantLines.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var constantLines = new StiConstantLines();
                constantLines.Chart = this.Chart;

                Add(constantLines);
                constantLines.LoadFromJsonObject((JObject)property.Value);
            }
        }
        #endregion

        #region IStiApplyStyle
        /// <summary>
        /// Applying specified style to this area.
        /// </summary>
        /// <param name="style"></param>
        public void ApplyStyle(IStiChartStyle style)
        {
            foreach (IStiConstantLines lines in this)
            {
                lines.Core.ApplyStyle(style);
            }
        }
        #endregion

        #region Methods
        public List<StiConstantLines> ToList()
        {
            return this.Cast<StiConstantLines>().ToList();
        }

        private string GetConstantLineTitle()
        {
            var baseTitle = StiLocalization.Get("Chart", "ConstantLine");
            var title = baseTitle;

            var index = 1;
            var finded = true;

            while (finded)
            {
                title = $"{baseTitle} {index}";
                finded = false;
                foreach (IStiConstantLines line in this)
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

        private void AddCore(IStiConstantLines value)
        {
            if (Chart != null)
            {
                if (string.IsNullOrEmpty(value.Text))
                    value.Text = GetConstantLineTitle();

                value.Chart = Chart;
            }

            base.List.Add(value);
        }

        public void Add(IStiConstantLines value)
        {
            AddCore(value);
            InvokeConstantLinesAdded(value, List.Count - 1);
        }

        public void AddRange(IStiConstantLines[] values)
        {
            foreach (IStiConstantLines value in values)
            {
                AddCore(value);
                InvokeConstantLinesAdded(value, List.Count - 1);
            }
        }

        public void AddRange(StiConstantLinesCollection values)
        {
            foreach (IStiConstantLines value in values)
            {
                AddCore(value);
                InvokeConstantLinesAdded(value, List.Count - 1);
            }
        }

        public bool Contains(IStiConstantLines value)
        {
            return List.Contains(value);
        }

        public int IndexOf(IStiConstantLines value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, IStiConstantLines value)
        {
            List.Insert(index, value);

            InvokeConstantLinesAdded(value, index);
        }

        protected override void OnClear()
        {
            while (List.Count > 0)
            {
                InvokeConstantLinesRemoved(List[0], 0);
                List.RemoveAt(0);
            }
        }

        public void Remove(IStiConstantLines value)
        {
            InvokeConstantLinesRemoved(value, List.IndexOf(value));
            List.Remove(value);
        }

        public IStiConstantLines this[int index]
        {
            get
            {
                return (IStiConstantLines)InnerList[index];
            }
            set
            {
                InnerList[index] = value;
            }
        }
        #endregion

        #region Events.ConstantLinesAdded
        public event EventHandler ConstantLinesAdded;

        protected virtual void OnConstantLinesAdded(EventArgs e)
        {
        }

        private void InvokeConstantLinesAdded(object sender, int index)
        {
            ConstantLinesAdded?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #region Events.ConstantLinesRemoved
        public event EventHandler ConstantLinesRemoved;

        protected virtual void OnConstantLinesRemoved(EventArgs e)
        {
        }

        private void InvokeConstantLinesRemoved(object sender, int index)
        {
            ConstantLinesRemoved?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public StiChart Chart { get; set; }
        #endregion

        [StiUniversalConstructor("ConstantLines")]
        public StiConstantLinesCollection()
        {
        }

        public StiConstantLinesCollection(List<StiConstantLines> lines)
        {
            AddRange(lines.ToArray());
        }
    }
}