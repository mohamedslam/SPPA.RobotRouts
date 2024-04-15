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
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Gauge;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class describes a collection of styles.
    /// </summary>
    public class StiStylesCollection :
        CollectionBase,
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();

            int index = 0;
            foreach (StiBaseStyle style in List)
            {
                jObject.AddPropertyJObject(index.ToString(), style.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public static StiBaseStyle LoadStyleFromJObject(JObject styleJObject)
        {
            var ident = styleJObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            StiBaseStyle style = null;
            switch (ident)
            {
                case "StiChartStyle":
                    style = new StiChartStyle();
                    break;

                case "StiGaugeStyle":
                    style = new StiGaugeStyle();
                    break;

                case "StiCardsStyle":
                    style = new StiCardsStyle();
                    break;

                case "StiIndicatorStyle":
                    style = new StiIndicatorStyle();
                    break;

                case "StiProgressStyle":
                    style = new StiProgressStyle();
                    break;

                case "StiCrossTabStyle":
                    style = new StiCrossTabStyle();
                    break;

                case "StiDialogStyle":
                    style = new StiDialogStyle();
                    break;

                case "StiMapStyle":
                    style = new StiMapStyle();
                    break;

                case "StiTableStyle":
                    style = new StiTableStyle();
                    break;

                case "StiStyle":
                    style = new StiStyle();
                    break;
            }

            if (style == null)
            {
                style = StiOptions.Designer.StyleDesigner.InvokeLoadCustomStyle(ident);

                if (style == null)
                    throw new Exception(string.Format("Type '{0}' is not supported!", ident));
            }

            style.LoadFromJsonObject(styleJObject);
            return style;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var styleJObject = (JObject)property.Value;
                var style = LoadStyleFromJObject(styleJObject);
                List.Add(style);
            }
        }
        #endregion

        #region Collection
        public void Add(StiBaseStyle style)
        {
            List.Add(style);
            hash[style.Name.ToLowerInvariant()] = style;
            style.Report = report;
        }

        protected override void OnClear()
        {
            base.OnClear();

            hash.Clear();
            needUpdateHash = true;
        }

        public void AddRange(StiStylesCollection styles)
        {
            foreach (StiBaseStyle style in styles)
            {
                Add(style);
            }
        }

        public void AddRange(StiBaseStyle[] styles)
        {
            foreach (StiBaseStyle style in styles)
            {
                Add(style);
            }
        }

        public void AddRange(List<StiBaseStyle> styles)
        {
            foreach (StiBaseStyle style in styles)
            {
                Add(style);
            }
        }

        public bool Contains(StiBaseStyle style)
        {
            UpdateHash();

            return hash[style.Name.ToLowerInvariant()] != null;
        }

        public bool Contains(string styleName)
        {
            UpdateHash();

            return hash[styleName.ToLowerInvariant()] != null;
        }

        public int IndexOf(StiBaseStyle style)
        {
            return List.IndexOf(style);
        }

        public void Insert(int index, StiBaseStyle style)
        {
            List.Insert(index, style);

            hash[style.Name.ToLowerInvariant()] = style;
        }

        public void Remove(StiBaseStyle style)
        {
            List.Remove(style);
            hash.Remove(style.Name.ToLowerInvariant());
        }

        public StiBaseStyle this[int index]
        {
            get
            {
                return (StiBaseStyle)List[index];
            }
            set
            {
                List[index] = value;
                needUpdateHash = true;
            }
        }

        public StiBaseStyle this[string name]
        {
            get
            {
                UpdateHash();

                if (hash[name.ToLowerInvariant()] != null)
                    return hash[name.ToLowerInvariant()] as StiBaseStyle;

                foreach (StiBaseStyle style in List)
                {
                    if (style.Name.ToLowerInvariant() == name.ToLowerInvariant()) 
                        return style;
                }

                return null;
            }
            set
            {
                for (int index = 0; index < List.Count; index++)
                {
                    if (((StiBaseStyle)List[index]).Name.ToLowerInvariant() == name.ToLowerInvariant())
                    {
                        List[index] = value;
                        needUpdateHash = true;
                        return;
                    }
                }

                List.Add(value);
            }
        }
        #endregion

        #region Methods
        public void Load(Stream stream)
        {
            var styleSheet = new StiStylesSheet(this);
            styleSheet.Load(stream);
            needUpdateHash = true;
            
            report?.ApplyStyles();
        }

        public void Load(string file)
        {
            var styleSheet = new StiStylesSheet(this);
            styleSheet.Load(file);
            needUpdateHash = true;
            
            report?.ApplyStyles();
        }

        public void Save(Stream stream)
        {
            var styleSheet = new StiStylesSheet(this);
            styleSheet.Save(stream);
        }

        public void Save(string file)
        {
            var styleSheet = new StiStylesSheet(this);
            styleSheet.Save(file);
        }

        private void UpdateHash()
        {
            if (lastCount != this.Count)
            {
                lastCount = this.Count;
                needUpdateHash = true;
            }

            if (needUpdateHash)
            {
                hash.Clear();
                foreach (StiBaseStyle style in List)
                {
                    hash[style.Name.ToLower(CultureInfo.InvariantCulture)] = style;
                }
                needUpdateHash = false;
            }
        }

        public List<StiBaseStyle> ToList()
        {
            var result = new List<StiBaseStyle>();

            foreach (StiBaseStyle style in List)
            {
                result.Add(style);
            }

            return result;
        }

        public StiCustomStyle GetCustomChartStyle(string customStyleName)
        {
            var style = !string.IsNullOrWhiteSpace(customStyleName) ? this[customStyleName] : null;
            return style != null ? new StiCustomStyle(customStyleName) : null;
        }

        public StiCustomGaugeStyle GetCustomGaugeStyle(string customStyleName)
        {
            var style = !string.IsNullOrWhiteSpace(customStyleName)
                ? ToList().FirstOrDefault(x => x.Name == customStyleName) as StiGaugeStyle
                : null;

            return style != null ? new StiCustomGaugeStyle(style) : null;
        }
        #endregion

        #region Fields
        private StiReport report = null;
        private Hashtable hash = new Hashtable();
        private bool needUpdateHash = true;
        private int lastCount = -1;
        #endregion

        public StiStylesCollection() : this(null)
        {
            needUpdateHash = true;
        }

        public StiStylesCollection(StiReport report)
        {
            this.report = report;
            needUpdateHash = true;
        }
    }
}
