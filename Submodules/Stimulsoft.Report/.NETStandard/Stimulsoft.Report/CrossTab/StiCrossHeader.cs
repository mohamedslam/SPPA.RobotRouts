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
using Stimulsoft.Report.CrossTab.Core;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base;
using Stimulsoft.Data.Engine;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.CrossTab
{
    [StiGdiPainter(typeof(StiCrossHeaderGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiCrossHeaderWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public abstract class StiCrossHeader : StiCrossCell
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("HideZeros");

            // StiCrossHeader
            jObject.AddPropertyJObject("GetDisplayCrossValueEvent", GetDisplayCrossValueEvent.SaveToJsonObject(mode));
            jObject.AddPropertyJObject("DisplayValue", DisplayValue.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty("TotalGuid", TotalGuid);
            jObject.AddPropertyBool("ShowTotal", ShowTotal, true);
            jObject.AddPropertyEnum("SortDirection", SortDirection, StiSortDirection.Asc);
            jObject.AddPropertyEnum("SortType", SortType, StiSortType.ByDisplayValue);
            jObject.AddPropertyBool("PrintOnAllPages", PrintOnAllPages, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "GetDisplayCrossValueEvent":
                        {
                            var valueEvent = new StiGetDisplayCrossValueEvent();
                            valueEvent.LoadFromJsonObject((JObject)property.Value);
                            this.GetDisplayCrossValueEvent = valueEvent;
                        }
                        break;

                    case "DisplayValue":
                        {
                            var expression = new StiDisplayCrossValueExpression();
                            expression.LoadFromJsonObject((JObject)property.Value);
                            this.DisplayValue = expression;
                        }
                        break;

                    case "TotalGuid":
                        this.TotalGuid = property.DeserializeString();
                        break;

                    case "ShowTotal":
                        this.ShowTotal = property.DeserializeBool();
                        break;

                    case "SortDirection":
                        this.SortDirection = property.DeserializeEnum<StiSortDirection>();
                        break;

                    case "SortType":
                        this.SortType = property.DeserializeEnum<StiSortType>();
                        break;

                    case "PrintOnAllPages":
                        this.PrintOnAllPages = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region Events
        #region OnGetDisplayCrossValue
        public event StiGetCrossValueEventHandler GetDisplayCrossValue;

        /// <summary>
        /// Raises the GetDisplayCrossValue event for this component.
        /// </summary>
        protected virtual void OnGetDisplayCrossValue(StiGetCrossValueEventArgs e)
        {
        }

        public void InvokeGetDisplayCrossValue(StiGetCrossValueEventArgs e)
        {
            try
            {
                if (Report.CalculationMode == StiCalculationMode.Compilation)
                {
                    OnGetDisplayCrossValue(e);
                    this.GetDisplayCrossValue?.Invoke(this, e);
                }
                else
                {
                    OnGetDisplayCrossValue(e);

                    var parserResult = StiParser.ParseTextValue(this.DisplayValue, this);
                    if (parserResult != null)
                        e.Value = parserResult;

                    this.GetDisplayCrossValue?.Invoke(this, e);
                }
            }
            catch (Exception ee)
            {
                StiLogService.Write(this.GetType(), "Do GetDisplayCrossValue ... ERROR");
                StiLogService.Write(this.GetType(), ee);

                Report?.WriteToReportRenderingMessages($"{Name} {ee.Message}");
            }
        }

        /// <summary>
        /// Gets or sets a script of the event GetDisplayCrossValueEvent.
        /// </summary>
        [StiSerializable]
        [Browsable(false)]
        [StiCategory("Data")]
        [Description("Gets or sets a script of the event GetDisplayCrossValueEvent.")]
        public StiGetDisplayCrossValueEvent GetDisplayCrossValueEvent { get; set; } = new StiGetDisplayCrossValueEvent();
        #endregion
        #endregion

        #region Text Expression override
        [Browsable(false)]
        public override StiExpression Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        #endregion

        #region Expressions
        #region CrossValue override
        public override StiCrossValueExpression Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (base.Value.Value == this.DisplayValue.Value)
                    this.DisplayValue.Value = value.Value;

                base.Value = value;
            }
        }
        #endregion

        #region DisplayCrossValue
        /// <summary>
        /// Gets or sets the expression that is used for calculation of a cell value which will be output in the table.
        /// </summary>
        [Description("Gets or sets the expression that is used for calculation of a cell value which will be output in the table.")]
        [StiSerializable(
             StiSerializeTypes.SerializeToCode |
             StiSerializeTypes.SerializeToDesigner |
             StiSerializeTypes.SerializeToSaveLoad)]
        [StiCategory("Data")]
        public virtual StiDisplayCrossValueExpression DisplayValue { get; set; } = new StiDisplayCrossValueExpression();
        #endregion
        #endregion

        #region StiText override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool HideZeros
        {
            get
            {
                return base.HideZeros;
            }
            set
            {
                base.HideZeros = value;
            }
        }
        #endregion

        #region Properties
        [StiNonSerialized]
        [Browsable(false)]
        public StiCrossTotal Total
        {
            get
            {
                if (TotalGuid == null || Parent == null) 
                    return null;

                foreach (StiComponent component in Parent.Components)
                {
                    var total = component as StiCrossTotal;
                    if (total?.Guid == TotalGuid) 
                        return total;
                }
                return null;
            }
            set
            {
                TotalGuid = value?.Guid;
            }
        }

        [StiNonSerialized]
        [Browsable(false)]
        public bool IsTotalVisible => ShowTotal && Total != null && Total.Enabled;

        [Browsable(false)]
        public int HeaderLevel { get; set; } = -1;

        [Browsable(false)]
        public string HeaderValue { get; set; }
        
        [StiSerializable]
        [Browsable(false)]
        public string TotalGuid { get; set; }

        /// <summary>
        /// Gets or sets value that indicates whether it is necessary to output totals or not.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Description("Gets or sets value that indicates whether it is necessary to output totals or not.")]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorShowTotal)]
        public virtual bool ShowTotal { get; set; } = true;

        /// <summary>
        /// Gets or sets the sorting direction.
        /// </summary>
        [DefaultValue(StiSortDirection.Asc)]
        [StiSerializable]
        [StiCategory("Data")]
        [Description("Gets or sets the sorting direction.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiSortDirection SortDirection { get; set; } = StiSortDirection.Asc;

        /// <summary>
        /// Gets or sets the type of sorting.
        /// </summary>
        [Description("Gets or sets the type of sorting.")]
        [DefaultValue(StiSortType.ByDisplayValue)]
        [StiSerializable]
        [StiCategory("Data")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiSortType SortType { get; set; } = StiSortType.ByDisplayValue;

        /// <summary>
        /// Gets or sets value indicates that the component is printed on all pages.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [StiOrder(StiPropertyOrder.BehaviorPrintOnAllPages)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets value indicates that the component is printed on all pages.")]
        public virtual bool PrintOnAllPages { get; set; } = true;

        [StiSerializable(StiSerializationVisibility.Class)]
        [Browsable(false)]
        public StiDataTopN TopN { get; set; }

        private bool ShouldSerializeTopN()
        {
            return TopN == null || !TopN.IsDefault;
        }

        /// <summary>
		/// Gets or sets a key of the original StiMeter object which generated that component.
		/// </summary>
		[StiNonSerialized]
        [Browsable(false)]
        public string MeterKey { get; set; }

        [Browsable(false)]
        public string ExpandExpression { get; set; }

        [Browsable(false)]
        public bool IsExpanded { get; set; }
        #endregion

        public StiCrossHeader()
        {
            Brush = new StiSolidBrush(Color.LightGray);
        }        
    }
}