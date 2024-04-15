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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class describes the component - StiElectronicSignature.
    /// </summary>
    [StiServiceBitmap(typeof(StiElectronicSignature), "Stimulsoft.Report.Images.Components.StiElectronicSignature.png")]
    [StiGdiPainter(typeof(StiElectronicSignatureGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiElectronicSignatureWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    [StiToolbox(true)]
    [StiDesigner("Stimulsoft.Report.Components.Design.StiElectronicSignatureDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfElectronicSignatureDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiContextTool(typeof(IStiComponentDesigner))]
    public class StiElectronicSignature : StiSignature
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyEnum(nameof(Mode), Mode, StiSignatureMode.Type);

            if (Draw != null)
                jObject.AddPropertyJObject(nameof(Draw), Draw.SaveToJsonObject(mode));

            if (Image != null)
                jObject.AddPropertyJObject(nameof(Image), Image.SaveToJsonObject(mode));

            if (Text != null)
                jObject.AddPropertyJObject(nameof(Text), Text.SaveToJsonObject(mode));

            if (Type != null)
                jObject.AddPropertyJObject(nameof(Type), Type.SaveToJsonObject(mode));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Brush):
                        this.Brush = property.DeserializeBrush();
                        break;

                    case nameof(Border):
                        this.Border = property.DeserializeBorder();
                        break;

                    case nameof(Mode):
                        this.Mode = property.DeserializeEnum<StiSignatureMode>();
                        break;

                    case nameof(Draw):
                        this.Draw = StiSignatureDraw.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Image):
                        Image = StiSignatureImage.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Text):
                        Text = StiSignatureText.CreateFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Type):
                        Type = StiSignatureType.CreateFromJsonObject((JObject)property.Value);
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiElectronicSignature;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection
            {
                {
                    StiPropertyCategories.ComponentEditor,
                    new[]
                    {
                        propHelper.SignatureEditor()
                    }
                },
                {
                    StiPropertyCategories.Signature,
                    new StiPropertyObject[]
                    {
                        propHelper.SignatureMode(),
                        propHelper.SignatureDraw(),
                        propHelper.SignatureImage(),
                        propHelper.SignatureText(),
                    }
                }
            };

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
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
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
                    propHelper.CanGrow(),
                    propHelper.CanShrink(),
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
                        StiPropertyEventId.GetZipCodeEvent,
                    }
                }
            };
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public override object Clone(bool cloneProperties)
        {
            var signature = base.Clone(cloneProperties) as StiElectronicSignature;

            signature.Text = this.Text?.Clone() as IStiSignatureText;
            signature.Image = this.Image?.Clone() as StiSignatureImage;
            signature.Type = this.Type?.Clone() as StiSignatureType;
            signature.Draw = this.Draw?.Clone() as StiSignatureDraw;

            return signature;
        }
        #endregion

        #region StiComponent override
        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiComponentToolboxPosition.ElectronicSignature;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Components", "StiElectronicSignature");
        #endregion

        #region Properties
        [StiNonSerialized]
        [StiBrowsable(false)]
        [Browsable(false)]
        internal bool AlreadySigned { get; set; }

        [StiSerializable]
        [DefaultValue(StiSignatureMode.Type)]
        [StiCategory("Signature")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiOrder(StiPropertyOrder.SignatureSignatureType)]
        public StiSignatureMode Mode { get; set; } = StiSignatureMode.Type;


        /// <summary>
        /// Gets or sets settings of the signature image.
        /// </summary>
        [Description("Gets or sets settings of the signature image.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Signature")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiOrder(StiPropertyOrder.SignatureDraw)]
        public IStiSignatureDraw Draw { get; set; } = new StiSignatureDraw();

        private bool ShouldSerializeDraw()
        {
            return Draw == null || !Draw.IsDefault;
        }

        /// <summary>
        /// Gets or sets settings of the signature image.
        /// </summary>
        [Description("Gets or sets settings of the signature image.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Signature")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiOrder(StiPropertyOrder.SignatureImage)]
        public IStiSignatureImage Image { get; set; } = new StiSignatureImage();

        private bool ShouldSerializeImage()
        {
            return Image == null || !Image.IsDefault;
        }

        /// <summary>
        /// Gets or sets settings of the signature text.
        /// </summary>
        [Description("Gets or sets settings of the signature text.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Signature")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiOrder(StiPropertyOrder.SignatureText)]
        public IStiSignatureText Text { get; set; } = new StiSignatureText();

        private bool ShouldSerializeText()
        {
            return Text == null || !Text.IsDefault;
        }


        /// <summary>
        /// Gets or sets settings of the signature text.
        /// </summary>
        [Description("Gets or sets settings of the signature text.")]
        [StiSerializable(StiSerializationVisibility.Class)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("Signature")]
        [TypeConverter(typeof(StiUniversalConverter))]
        [StiOrder(StiPropertyOrder.SignatureType)]
        public IStiSignatureType Type { get; set; } = new StiSignatureType();

        private bool ShouldSerializeType()
        {
            return Type == null || !Type.IsDefault;
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew() => new StiElectronicSignature();
        #endregion

        /// <summary>
		/// Creates a new component of the type StiElectronicSignature.
		/// </summary>
		public StiElectronicSignature() : this(RectangleD.Empty)
        {
        }

        /// <summary>
        /// Creates a new component of the type StiElectronicSignature.
        /// </summary>
        /// <param name="rect">The rectangle describes size and position of the component.</param>
        public StiElectronicSignature(RectangleD rect) : base(rect)
        {
            PlaceOnToolbox = false;
        }
    }
}