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
using Stimulsoft.Report.Chart.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Stimulsoft.Report.Chart
{   /// <summary>
    /// Describes the series trend lines collection.
    /// </summary>
    [TypeConverter(typeof(StiTrendLinesCollectionConverter))]
    public class StiTrendLinesCollection :
        CollectionBase,
        ICloneable,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiTrendLine trendLine in List)
            {
                jObject.AddPropertyJObject(index.ToString(), trendLine.SaveToJsonObject(mode));
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

                var trendLine = StiOptions.Services.ChartTrendLines.FirstOrDefault(x => x.GetType().Name == ident);
                if (trendLine == null)
                    throw new Exception(string.Format("Type {0} is not found!", ident));

                var trendLineClone = trendLine.CreateNew();

                trendLineClone.LoadFromJsonObject((JObject)property.Value);
                Add(trendLineClone);
            }
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            var lines = new StiTrendLinesCollection();
            foreach (StiTrendLine trendLine in this)
            {
                lines.Add(trendLine.Clone() as StiTrendLine);
            }
            return lines;
        }
        #endregion

        #region Collection
        public void Add(StiTrendLine filter)
        {
            List.Add(filter);
        }

        public void AddRange(StiTrendLinesCollection lines)
        {
            foreach (StiTrendLine trendLine in lines)
            {
                Add(trendLine);
            }
        }

        public void AddRange(StiTrendLine[] lines)
        {
            foreach (StiTrendLine trendLine in lines)
            {
                Add(trendLine);
            }
        }

        public void AddRange(List<StiTrendLine> lines)
        {
            foreach (StiTrendLine trendLine in lines)
            {
                Add(trendLine);
            }
        }

        public bool Contains(StiTrendLine line)
        {
            return List.Contains(line);
        }

        public int IndexOf(StiTrendLine line)
        {
            return List.IndexOf(line);
        }

        public void Insert(int index, StiTrendLine line)
        {
            List.Insert(index, line);
        }

        public void Remove(StiTrendLine line)
        {
            List.Remove(line);
        }

        public StiTrendLine this[int index]
        {
            get
            {
                return (StiTrendLine)List[index];
            }
            set
            {
                List[index] = value;
            }
        }
        #endregion

        #region Methods
        public List<StiTrendLine> ToList()
        {
            var list = new List<StiTrendLine>();
            foreach (StiTrendLine trendLine in List)
            {
                list.Add(trendLine);
            }
            return list;
        }
        #endregion
    }
}
