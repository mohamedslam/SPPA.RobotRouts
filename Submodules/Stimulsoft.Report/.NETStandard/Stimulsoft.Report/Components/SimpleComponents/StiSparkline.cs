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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
    [StiServiceBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiServiceCategoryBitmap(typeof(StiComponent), "Stimulsoft.Report.Images.Components.StiChart.png")]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiSparklineDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfSparklineDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiSparklineWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(StiSparklineGdiPainter))]
    [StiV2Builder(typeof(StiSparklineV2Builder))]
    [StiContextTool(typeof(IStiComponentDesigner))]
    [StiEngine(StiEngineVersion.EngineV2)]
    public class StiSparkline :
        StiComponent,
        IStiExportImageExtended,
        IStiDataRelation,
        IStiBorder,
        IStiBrush
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiSparkline
            jObject.AddPropertyStringNullOrEmpty("DataRelationName", DataRelationName);
            jObject.AddPropertyBrush("Brush", Brush);
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyStringNullOrEmpty("ValueDataColumn", ValueDataColumn);
            jObject.AddPropertyEnum("Type", Type, StiSparklineType.Area);
            jObject.AddPropertyBool("ShowHighLowPoints", ShowHighLowPoints);
            jObject.AddPropertyBool("ShowFirstLastPoints", ShowFirstLastPoints, true);

            jObject.AddPropertyColor("PositiveColor", PositiveColor, "#537eb6");

            jObject.AddPropertyColor("NegativeColor", NegativeColor, "#ff0000");

            if (mode == StiJsonSaveMode.Document)
                jObject.AddPropertyStringNullOrEmpty("ValuesContainer", ValuesContainer);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "DataRelationName":
                        this.DataRelationName = property.DeserializeString();
                        break;

                    case "Brush":
                        this.Brush = property.DeserializeBrush();
                        break;

                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "ValueDataColumn":
                        this.ValueDataColumn = property.DeserializeString();
                        break;

                    case "Type":
                        this.Type = property.DeserializeEnum<StiSparklineType>();
                        break;

                    case "ShowHighLowPoints":
                        this.ShowHighLowPoints = property.DeserializeBool();
                        break;

                    case "ShowFirstLastPoints":
                        this.ShowFirstLastPoints = property.DeserializeBool();
                        break;

                    case "PositiveColor":
                        this.PositiveColor = property.DeserializeColor();
                        break;

                    case "NegativeColor":
                        this.NegativeColor = property.DeserializeColor();
                        break;

                    case "ValuesContainer":
                        this.ValuesContainer = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiSparkline;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            collection.Add(StiPropertyCategories.ComponentEditor, new[]
            {
                propHelper.ShapeEditor()
            });

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Data, new[]
                {
                    propHelper.ValueDataColumn(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Data, new[]
                {
                    propHelper.ValueDataColumn(),
                    propHelper.DataRelation(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height(),
                    propHelper.MinSize(),
                    propHelper.MaxSize(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Appearance, new[]
                {
                    propHelper.Brush(),
                    propHelper.Border(),
                    propHelper.Conditions(),
                    propHelper.ComponentStyle(),
                    propHelper.UseParentStyles(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked(),
                });
            }
            
            return collection;
        }

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return new StiEventCollection
            {
                {
                    StiPropertyCategories.MouseEvents,
                    new[]
                    {
                        StiPropertyEventId.ClickEvent,
                        StiPropertyEventId.DoubleClickEvent,
                        StiPropertyEventId.MouseEnterEvent,
                        StiPropertyEventId.MouseLeaveEvent
                    }
                },
                {
                    StiPropertyCategories.NavigationEvents,
                    new[]
                    {
                        StiPropertyEventId.GetBookmarkEvent,
                        StiPropertyEventId.GetDrillDownReportEvent,
                        StiPropertyEventId.GetHyperlinkEvent,
                        StiPropertyEventId.GetPointerEvent,
                    }
                },
                {
                    StiPropertyCategories.PrintEvents,
                    new[]
                    {
                        StiPropertyEventId.AfterPrintEvent,
                        StiPropertyEventId.BeforePrintEvent,
                    }
                },
                {
                    StiPropertyCategories.ValueEvents,
                    new[]
                    {
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                    }
                }
            };
        }
        #endregion

        #region IStiDataRelation
        /// <summary>
        /// Get link that is used for master-detail reports rendering.
        /// </summary>
        [TypeConverter(typeof(StiDataRelationConverter))]
        [Editor("Stimulsoft.Report.Components.Design.StiDataRelationEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiCategory("Data")]
        [StiOrder(2)]
        [Description("Get link that is used for master-detail reports rendering.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public virtual StiDataRelation DataRelation
        {
            get
            {
                if (Page == null ||
                    Report?.Dictionary?.Relations == null ||
                    DataRelationName == null ||
                    DataRelationName.Length == 0) 
                    return null;

                return Report.Dictionary.Relations[DataRelationName];
            }
        }

        /// <summary>
        /// Gets or sets relation name.
        /// </summary>
        [Browsable(false)]
        [StiSerializable]
        [DefaultValue("")]
        public string DataRelationName { get; set; } = "";
        #endregion
        
        #region IStiExportImageExtended
        public virtual Image GetImage(ref float zoom)
        {
            return GetImage(ref zoom, StiExportFormat.None);
        }

        public virtual Image GetImage(ref float zoom, StiExportFormat format)
        {
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
        }

        [Browsable(false)]
        public override bool IsExportAsImage(StiExportFormat format)
        {
            return format != StiExportFormat.Pdf;
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var sparkline = (StiSparkline)base.Clone(cloneProperties);

            sparkline.Border = this.Border?.Clone() as StiBorder;
            sparkline.Brush = this.Brush?.Clone() as StiBrush;

            return sparkline;
        }
        #endregion

        #region IStiBrush
        /// <summary>
        /// The brush, which is used to draw background.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBrush)]
        [StiSerializable]
        [Description("The brush, which is used to draw background.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        [StiExpressionAllowed]
        public StiBrush Brush { get; set; } = new StiSolidBrush();

        private bool ShouldSerializeBrush()
        {
            return !(Brush is StiSolidBrush && ((StiSolidBrush)Brush).Color == Color.Transparent);
        }
        #endregion

        #region IStiBorder
        /// <summary>
        /// Gets or sets border of the component.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("Gets or sets border of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBorder Border { get; set; } = new StiBorder();

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region IStiCanShrink override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool CanShrink
        {
            get
            {
                return base.CanShrink;
            }
            set
            {
            }
        }
        #endregion

        #region IStiCanGrow override
        [Browsable(false)]
        [StiNonSerialized]
        public override bool CanGrow
        {
            get
            {
                return base.CanGrow;
            }
            set
            {
            }
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets a component priority.
        /// </summary>
        public override int Priority => (int)StiComponentPriority.Component;

        public override string LocalizedCategory => StiLocalization.Get("Report", "Components");

        /// <summary>
        /// Gets or sets the default client area of a component.
        /// </summary>
        [Browsable(false)]
        public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 100, 30);

        /// <summary>
        /// Gets the type of processing when printing.
        /// </summary>
        public override StiComponentType ComponentType => StiComponentType.Simple;

        public override int ToolboxPosition => (int)StiComponentToolboxPosition.Sparkline;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiSparkline");
        #endregion

        #region StiComponent.Properties
        public override string HelpUrl => "user-manual/index.html?report_internals_sparkline.htm";
        #endregion

        #region Methods
        public override StiComponent CreateNew()
        {
            return new StiSparkline();
        }

        internal object[] FetchValues()
        {
            if (Values == null && IsDesigning)
            {
                var tempValues = StiSparklineV2Builder.GetValuesFromValueDataColumn(this, 10);
                if (tempValues != null && tempValues.Length > 0)
                    Values = tempValues;
                else
                    Values = new decimal[] { 1, 2, 3, 6, 3, 2, 7, 4, 2, -3, -5 };
            }

            return Values?.Cast<object>().ToArray();
        }

        public StiDataSource GetDataSource()
        {
            return StiDataColumn.GetDataSourceFromDataColumn(Report.Dictionary, ValueDataColumn);
        }
        #endregion

        #region Properties
        private string valueDataColumn = string.Empty;
        /// <summary>
        /// Gets or sets a name of the column that contains the values.
        /// </summary>
        [DefaultValue("")]
        [StiSerializable]
        [Description("Gets or sets a name of the column that contains the values.")]
        [StiCategory("Data")]
        [Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder(1)]
        public virtual string ValueDataColumn
        {
            get 
            { 
                return valueDataColumn; 
            }
            set
            {
                if (valueDataColumn != value)
                {
                    valueDataColumn = value;
                    Values = null;
                }
            }
        }

        [Browsable(false)]
        public decimal[] Values { get; set; }

        [Browsable(false)]
        [StiSerializable(StiSerializeTypes.SerializeToDocument)]
        public string ValuesContainer
        {
            get
            {
                return Values != null ? string.Join(";", Values) : null;
            }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    Values = null;
                }
                else
                {
                    var strs = value.Split(new char[] { ';' });

                    Values = strs.Select(s => StiValueHelper.TryToDecimal(s)).ToArray();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the sparklines.
        /// </summary>
        [DefaultValue(StiSparklineType.Area)]
        [TypeConverter(typeof(StiEnumConverter))]
        [StiSerializable]
        [Browsable(false)]
        public StiSparklineType Type { get; set; } = StiSparklineType.Area;

        /// <summary>
        /// Gets or sets the value which indicates that high and low points should be show.
        /// </summary>
        [DefaultValue(false)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiSerializable]
        [Browsable(false)]
        public bool ShowHighLowPoints { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates that first and last points should be show.
        /// </summary>
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiSerializable]
        [Browsable(false)]
        public bool ShowFirstLastPoints { get; set; } = true;

        /// <summary>
        /// Gets or sets a back color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Browsable(false)]
        [DefaultValue(null)]
        public Color PositiveColor { get; set; } = StiColor.Get("537eb6");

        private bool ShouldSerializePositiveColor()
        {
            return PositiveColor != StiColor.Get("537eb6");
        }

        /// <summary>
        /// Gets or sets a back color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Browsable(false)]
        [DefaultValue(null)]
        public Color NegativeColor { get; set; } = StiColor.Get("ff0000");

        private bool ShouldSerializeNegativeColor()
        {
            return NegativeColor != StiColor.Get("ff0000");
        }
        #endregion

        /// <summary>
        /// Creates a new StiSparkline.
        /// </summary>
        public StiSparkline() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new StiSparkline.
        /// </summary>
        /// <param name="rect">The rectangle describes sizes and position of the component.</param>
        public StiSparkline(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}