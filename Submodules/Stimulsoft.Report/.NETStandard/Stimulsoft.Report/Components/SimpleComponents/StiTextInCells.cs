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
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.QuickButtons;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using System.Collections.Generic;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes the class that realizes the component - StiTextInCells.
	/// </summary>
    [StiServiceBitmap(typeof(StiTextInCells), "Stimulsoft.Report.Images.Components.StiTextInCells.png")]
	[StiToolbox(true)]	
    [StiGdiPainter(typeof(StiTextInCellsGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiTextInCellsWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	[StiV1Builder(typeof(StiTextInCellsV1Builder))]
	[StiV2Builder(typeof(StiTextInCellsV2Builder))]
    [StiQuickButton("Stimulsoft.Report.QuickButtons.Design.StiTextQuickButton, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfQuickButton("Stimulsoft.Report.WpfDesign.StiWpfTextQuickButton, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
	public class StiTextInCells : StiText
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("VertAlignment");
            jObject.RemoveProperty("AutoWidth");
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("AllowHtmlTags");
            jObject.RemoveProperty("ShrinkFontToFit");
            jObject.RemoveProperty("ShrinkFontToFitMinimumSize");
            jObject.RemoveProperty("Angle");
            jObject.RemoveProperty("LinesOfUnderline");
            jObject.RemoveProperty("MaxNumberOfLines");
            jObject.RemoveProperty("ProcessingDuplicates");
            jObject.RemoveProperty("RenderTo");
            jObject.RemoveProperty("TextQuality");
            jObject.RemoveProperty("ExcelValue");

            // StiTextInCells
            jObject.AddPropertyFloat("CellWidth", CellWidth);
            jObject.AddPropertyFloat("CellHeight", CellHeight);
            jObject.AddPropertyFloat("HorSpacing", HorSpacing);
            jObject.AddPropertyFloat("VertSpacing", VertSpacing);
            jObject.AddPropertyBool("WordWrap", WordWrap);
            jObject.AddPropertyBool("RightToLeft", RightToLeft);
            jObject.AddPropertyBool("ContinuousText", ContinuousText, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CellWidth":
                        this.CellWidth = property.DeserializeFloat();
                        break;

                    case "CellHeight":
                        this.CellHeight = property.DeserializeFloat();
                        break;

                    case "HorSpacing":
                        this.HorSpacing = property.DeserializeFloat();
                        break;

                    case "VertSpacing":
                        this.VertSpacing = property.DeserializeFloat();
                        break;

                    case "WordWrap":
                        this.WordWrap = property.DeserializeBool();
                        break;

                    case "RightToLeft":
                        this.RightToLeft = property.DeserializeBool();
                        break;

                    case "ContinuousText":
                        this.ContinuousText = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiTextInCells;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var collection = new StiPropertyCollection();

            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Text, new[]
                {
                    propHelper.Text(),
                    propHelper.HorAlignment(),
                    propHelper.TextFormat(),
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Text, new[]
                {
                    propHelper.Text(),
                    propHelper.CellWidth(),
                    propHelper.CellHeight(),
                    propHelper.HorAlignment(),
                    propHelper.HorSpacing(),
                    propHelper.VertSpacing(),
                    propHelper.TextFormat(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.ContinuousText(),
                    propHelper.HideZeros(),
                    propHelper.LineSpacing(),
                    propHelper.WordWrap(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.ContinuousText(),
                    propHelper.Editable(),
                    propHelper.HideZeros(),
                    propHelper.LineSpacing(),
                    propHelper.Margins(),
                    propHelper.OnlyText(),
                    propHelper.RightToLeft(),
                    propHelper.WordWrap()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.TextAdditional, new[]
                {
                    propHelper.ContinuousText(),
                    propHelper.Editable(),
                    propHelper.HideZeros(),
                    propHelper.LineSpacing(),
                    propHelper.Margins(),
                    propHelper.OnlyText(),
                    propHelper.ProcessAt(),
                    propHelper.RightToLeft(),
                    propHelper.WordWrap(),
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Position, new[]
                {
                    propHelper.Left(),
                    propHelper.Top(),
                    propHelper.Width(),
                    propHelper.Height()
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
                    propHelper.MaxSize()
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
                    propHelper.Font(),
                    propHelper.TextBrush(),
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
                    propHelper.Font(),
                    propHelper.TextBrush(),
                    propHelper.UseParentStyles()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Behavior, new[]
                {
                    propHelper.AnchorMode(),
                    propHelper.CanBreak(),
                    propHelper.CanGrow(),
                    propHelper.DockStyle(),
                    propHelper.Enabled(),
                    propHelper.GrowToHeight(),
                    propHelper.InteractionEditor(),
                    propHelper.Printable(),
                    propHelper.PrintOn(),
                    propHelper.ShiftMode()
                });
            }
            
            if (level == StiLevel.Basic)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name()
                });
            }
            else if (level == StiLevel.Standard)
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias()
                });
            }
            else
            {
                collection.Add(StiPropertyCategories.Design, new[]
                {
                    propHelper.Name(),
                    propHelper.Alias(),
                    propHelper.GlobalizedName(),
                    propHelper.Restrictions(),
                    propHelper.Locked(),
                    propHelper.Linked()
                });
            }
            
            if (level == StiLevel.Professional)
            {
                collection.Add(StiPropertyCategories.Export, new[]
                {
                    propHelper.ExportAsImage()
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
                        StiPropertyEventId.GetExcelValueEvent,
                        StiPropertyEventId.GetTagEvent,
                        StiPropertyEventId.GetToolTipEvent,
                        StiPropertyEventId.GetValueEvent,
                    }
                }
            };
        }
        #endregion

        #region StiComponent.Properties

        public override string HelpUrl
        {
            get
            {
                return "user-manual/right_to_left_text_in_cells_component.htm?zoom_highlightsub=Text";
            }
        }

        #endregion

		#region IStiVertAlignment Off
		[StiNonSerialized]
		[Browsable(false)]
		public override StiVertAlignment VertAlignment
		{
			get
			{
				return base.VertAlignment;
			}
			set
			{
				base.VertAlignment = value;
			}
		}
		#endregion

		#region IStiUnitConvert
		/// <summary>
		/// Converts a component out of one unit into another.
		/// </summary>
		/// <param name="oldUnit">Old units.</param>
		/// <param name="newUnit">New units.</param>
        public override void Convert(StiUnit oldUnit, StiUnit newUnit, bool isReportSnapshot = false)
		{
			base.Convert(oldUnit, newUnit, isReportSnapshot);

			this.CellWidth =	(float)newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.CellWidth));
			this.CellHeight =	(float)newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.CellHeight));
			this.HorSpacing =	(float)newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.HorSpacing));
			this.VertSpacing =	(float)newUnit.ConvertFromHInches(oldUnit.ConvertToHInches(this.VertSpacing));
		}
		#endregion

		#region IStiTextFormat Off
        //[StiNonSerialized]
        //[Browsable(false)]
        //public override StiFormatService TextFormat
        //{
        //    get
        //    {
        //        return base.TextFormat;
        //    }
        //    set
        //    {
        //        base.TextFormat = value;
        //    }
        //}
		#endregion

		#region IStiTextOptions Off
		[Browsable(false)]
		public override StiTextOptions TextOptions
		{
			get 
			{
				return base.TextOptions;
			}
			set 
			{
				base.TextOptions = value;
			}
		}
		#endregion

		#region IStiAutoWidth Off
		[StiNonSerialized]
		[Browsable(false)]
		public override bool AutoWidth
		{
			get
			{
				return base.AutoWidth;
			}
			set
			{
				base.AutoWidth = value; 
			}
		}
		#endregion

		#region IStiCanShrink Off
		[StiNonSerialized]
		[Browsable(false)]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
				base.CanShrink = value; 
			}
		}
		#endregion

		#region Properties Off
        [StiNonSerialized]
        [Browsable(false)]
        public override bool AllowHtmlTags
        {
            get
            {
                return base.AllowHtmlTags;
            }
            set
            {
                this.AllowHtmlTags = value;
            }
        }

		[StiNonSerialized]
		[Browsable(false)]
		public override bool ShrinkFontToFit
		{
			get
			{
				return base.ShrinkFontToFit;
			}
			set
			{
				base.ShrinkFontToFit = value; 
			}
		}


		[StiNonSerialized]
		[Browsable(false)]
		public override float ShrinkFontToFitMinimumSize
		{
			get
			{
				return base.ShrinkFontToFitMinimumSize;
			}
			set
			{						
				base.ShrinkFontToFitMinimumSize = value;
			}
		}


		[StiNonSerialized]
		[Browsable(false)]
		public override float Angle
		{
			get
			{
				return base.Angle;
			}
			set
			{
				base.Angle = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiPenStyle LinesOfUnderline
		{
			get
			{
				return base.LinesOfUnderline;
			}
			set
			{
				base.LinesOfUnderline = value;
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override int MaxNumberOfLines
		{
			get
			{
				return base.MaxNumberOfLines;
			}
			set
			{
				base.MaxNumberOfLines = value; 
			}
		}

		[StiNonSerialized]
		[Browsable(false)]
		public override StiProcessingDuplicatesType ProcessingDuplicates
		{
			get
			{
				return base.ProcessingDuplicates;
			}
			set
			{
				base.ProcessingDuplicates = value;
			}
		}


		#endregion

		#region IStiRenderTo Off
		[StiNonSerialized]
		[Browsable(false)]
		public override string RenderTo
		{
			get
			{
				return base.RenderTo;
			}
			set
			{
				base.RenderTo = value; 
			}
		}
		#endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiComponentToolboxPosition.TextInCells;
			}
		}

        public override StiToolboxCategory ToolboxCategory
        {
            get
            {
                return StiToolboxCategory.Components;
            }
        }

		/// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory
		{
			get 
			{
				return StiLocalization.Get("Report", "Components");
			}
		}

		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return StiLocalization.Get("Components", "StiTextInCells");
			}
		}
        #endregion

        #region IStiBorder
        /// <summary>
        /// The appearance and behavior of the component border.
        /// </summary>
        [StiCategory("Appearance")]
        [StiOrder(StiPropertyOrder.AppearanceBorder)]
        [StiSerializable]
        [Description("The appearance and behavior of the component border.")]
        [RefreshProperties(RefreshProperties.All)]
        [StiPropertyLevel(StiLevel.Basic)]
        public override StiBorder Border
        {
            get
            {
                return base.Border;
            }
            set
            {
                base.Border = value;
                if (value.IsDefault)
                {
                    //change size to 2, to force always serialize
                    base.Border = new StiBorder(StiBorderSides.None, value.Color, 2, value.Style, value.DropShadow, value.ShadowSize, value.ShadowBrush, value.Topmost);
                }
            }
        }

        private bool ShouldSerializeBorder()
        {
            return Border == null || !Border.IsDefault;
        }
        #endregion

        #region Properties
        protected static object PropertyCellWidth = new object();
        /// <summary>
        /// Gets or sets width of the cell.
        /// </summary>
		[DefaultValue(0f)]
		[StiSerializable]
		[StiCategory("Text")]
		[StiOrder(StiPropertyOrder.TextCellWidth)]
        [Description("Gets or sets width of the cell.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual float CellWidth
		{
			get
			{
                return Properties.GetFloat(PropertyCellWidth, 0f);
			}
			set
			{
                if (value < 0)
                    Properties.SetFloat(PropertyCellWidth, 0, 0f);
                else
                    Properties.SetFloat(PropertyCellWidth, value, 0f);
			}
		}


        protected static object PropertyCellHeight = new object();
        /// <summary>
        /// Gets or sets height of the cell.
        /// </summary>
		[DefaultValue(0f)]
		[StiSerializable]
		[StiCategory("Text")]
		[StiOrder(StiPropertyOrder.TextCellHeight)]
        [Description("Gets or sets height of the cell.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual float CellHeight
		{
			get
			{
                return Properties.GetFloat(PropertyCellHeight, 0f);
			}
			set
			{
                if (value < 0)
                    Properties.SetFloat(PropertyCellHeight, 0, 0f);
                else
                    Properties.SetFloat(PropertyCellHeight, value, 0f);
            }
		}


        protected static object PropertyHorSpacing = new object();
        /// <summary>
        /// Gets or sets horizontal spacing between cells.
        /// </summary>
		[DefaultValue(0f)]
		[StiSerializable]
		[StiCategory("Text")]
		[StiOrder(StiPropertyOrder.TextHorSpacing)]
        [Description("Gets or sets horizontal spacing between cells.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual float HorSpacing
		{
			get
			{
                return Properties.GetFloat(PropertyHorSpacing, 0f);
			}
			set
			{
                if (value < 0)
                    Properties.SetFloat(PropertyHorSpacing, 0, 0f);
                else
                    Properties.SetFloat(PropertyHorSpacing, value, 0f);
            }
		}


        protected static object PropertyVertSpacing = new object();
        /// <summary>
        /// Gets or sets vertical spacing between cells.
        /// </summary>
		[DefaultValue(0f)]
		[StiSerializable]
		[StiCategory("Text")]
		[StiOrder(StiPropertyOrder.TextVertSpacing)]
        [Description("Gets or sets vertical spacing between cells.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual float VertSpacing
		{
			get
			{
                return Properties.GetFloat(PropertyVertSpacing, 0f);
			}
			set
			{
                if (value < 0)
                    Properties.SetFloat(PropertyVertSpacing, 0, 0f);
                else
                    Properties.SetFloat(PropertyVertSpacing, value, 0f);
            }
		}


		[StiNonSerialized]
		[Browsable(false)]
		public override StiTextQuality TextQuality
		{
			get
			{
				return base.TextQuality;
			}
			set
			{
				base.TextQuality = value;
			}
		}


		/// <summary>
		/// Gets or sets word wrap.
		/// </summary>
		[DefaultValue(false)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextWordWrap)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets word wrap.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
		public override bool WordWrap
		{
			get
			{
				return TextOptions.WordWrap;
			}
			set
			{
				TextOptions.WordWrap = value;
			}
		}


        /// <summary>
        /// Gets or sets horizontal output direction.
        /// </summary>
        [Browsable(true)]
        [StiBrowsable(true)]
        [DefaultValue(false)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextRightToLeft)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets horizontal output direction.")]
        [StiPropertyLevel(StiLevel.Standard)]
		public override bool RightToLeft
		{
			get 
			{
				return TextOptions.RightToLeft;
			}
			set 
			{
				TextOptions.RightToLeft = value;
			}
		}


        protected static object PropertyContinuousText = new object();
		/// <summary>
		/// Gets or sets continuous text flag.
		/// </summary>
		[DefaultValue(true)]
		[StiSerializable]
		[StiCategory("TextAdditional")]
		[StiOrder(StiPropertyOrder.TextContinuousText)]
		[TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [Description("Gets or sets continuous text flag.")]
        [StiShowInContextMenu]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual bool ContinuousText
		{
			get
			{
                return Properties.GetBool(PropertyContinuousText, true);
			}
			set
			{
                Properties.SetBool(PropertyContinuousText, value, true);
			}
		}
		#endregion

		#region Expression
		[StiNonSerialized]
		[Browsable(false)]
		public override StiExcelValueExpression ExcelValue
		{
			get
			{
				return base.ExcelValue;
			}
			set
			{
				base.ExcelValue = value;
			}
		}
		#endregion

        #region IStiGetActualSize
        public override SizeD GetActualSize()
        {
            return new SizeD(this.Width, this.Height);
        }
        #endregion

        #region SplitByCells
        internal static StiContainer SplitByCells(StiTextInCells masterTextInCells, StiComponent renderedComponent, string textString)
        {
            return SplitByCells(masterTextInCells, renderedComponent, textString, false);
        }

        internal static StiContainer SplitByCells(StiTextInCells masterTextInCells, StiComponent renderedComponent, string textString, bool measure)
        {
            var cont = new StiContainer();
            cont.ClientRectangle = renderedComponent.ClientRectangle;
            cont.Name = renderedComponent.Name;
            cont.CanGrow = renderedComponent.CanGrow;

            var rect = cont.ClientRectangle;
            rect.X = 0;
            rect.Y = 0;

            var unit = masterTextInCells.Page.Unit;
            double borderSize = unit.ConvertFromHInches(masterTextInCells.Border.Size / 2 * masterTextInCells.Page.Zoom);

            double horSpacing = masterTextInCells.HorSpacing;
            double vertSpacing = masterTextInCells.VertSpacing;
            double cellWidth = masterTextInCells.CellWidth;
            double cellHeight = masterTextInCells.CellHeight;

            if (masterTextInCells.CellWidth == 0) cellWidth = unit.ConvertFromHInches(masterTextInCells.Font.GetHeight() * 1.5f * StiDpiHelper.DeviceCapsScale);
            if (masterTextInCells.CellHeight == 0) cellHeight = unit.ConvertFromHInches(masterTextInCells.Font.GetHeight() * 1.5f * StiDpiHelper.DeviceCapsScale);

            if (!masterTextInCells.ContinuousText)
            {
                #region New mode - !ContinuousText

                #region Calculate text size
                Size textSize = new Size(1, 1);
                double posX = (float)(rect.X + borderSize + cellWidth);
                while (posX + horSpacing + cellWidth < rect.Right)
                {
                    posX += horSpacing + cellWidth;
                    textSize.Width++;
                }
                double posY = (float)(rect.Y + borderSize + cellHeight);
                while (posY + vertSpacing + cellHeight < rect.Bottom)
                {
                    posY += vertSpacing + cellHeight;
                    textSize.Height++;
                }
                if (!masterTextInCells.WordWrap) textSize.Height = 1;
                #endregion

                #region Make string list
                var stringList = new List<string>();
                string st = string.Empty;
                if (textString != null)
                {
                    foreach (char ch in textString)
                    {
                        if (char.IsControl(ch))
                        {
                            if (ch == '\n')
                            {
                                stringList.Add(StiTextInCellsHelper.TrimEndWhiteSpace(st));
                                st = string.Empty;
                            }
                        }
                        else
                        {
                            st += ch;
                        }
                    }
                }
                if (st != string.Empty) stringList.Add(StiTextInCellsHelper.TrimEndWhiteSpace(st));
                if (stringList.Count == 0) stringList.Add(st);
                #endregion

                #region Wordwrap
                if (masterTextInCells.WordWrap)
                {
                    for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
                    {
                        string stt = stringList[indexLine];
                        if (stt.Length > textSize.Width)
                        {
                            int[] wordarr = new int[stt.Length];
                            int wordCounter = 0;
                            int tempIndexSpace = 0;
                            while ((tempIndexSpace < stt.Length) && char.IsWhiteSpace(stt[tempIndexSpace]))
                            {
                                wordarr[tempIndexSpace] = wordCounter;
                                tempIndexSpace++;
                            }
                            for (int tempIndex = tempIndexSpace; tempIndex < stt.Length; tempIndex++)
                            {
                                if (char.IsWhiteSpace(stt[tempIndex])) wordCounter++;
                                wordarr[tempIndex] = wordCounter;
                            }
                            int index = textSize.Width;
                            int index2 = index - 1;
                            //check words number; if no first - go to begin, else to end of word
                            if (wordarr[index] > 0)	//word is no first
                            {
                                if (wordarr[index] != wordarr[index2])	//end of word
                                {
                                    while (char.IsWhiteSpace(stt[index])) index++;
                                }
                                else
                                {
                                    while (!char.IsWhiteSpace(stt[index])) index--;
                                    index2 = index++;
                                    while (char.IsWhiteSpace(stt[index2])) index2--;
                                }
                            }
                            stringList[indexLine] = stt.Substring(0, index2 + 1);
                            stringList.Insert(indexLine + 1, stt.Substring(index, stt.Length - index));
                        }
                    }
                }
                #endregion

                if (measure && masterTextInCells.CanGrow) textSize.Height = stringList.Count;

                #region Paint
                posY = (float)(rect.Y + borderSize);
                for (int indexY = 0; indexY < textSize.Height; indexY++)
                {
                    string currentLineText = (indexY < stringList.Count ? stringList[indexY] : string.Empty);

                    #region HorAlignment
                    int textOffset = 0;
                    if (masterTextInCells.HorAlignment == StiTextHorAlignment.Center)
                        textOffset = (textSize.Width - currentLineText.Length) / 2;

                    if (masterTextInCells.HorAlignment == StiTextHorAlignment.Right)
                        textOffset = textSize.Width - currentLineText.Length;

                    if (textOffset > 0)
                        currentLineText = new string(' ', textOffset) + currentLineText;
                    #endregion

                    posX = (float)(rect.X + borderSize);
                    for (int indexX = 0; indexX < textSize.Width; indexX++)
                    {
                        double cx = Math.Round(posX, 2);
                        double cy = Math.Round(posY, 2);
                        double cw = Math.Round(posX + cellWidth, 2) - cx;
                        double ch = Math.Round(posY + cellHeight, 2) - cy;
                        var sectorRect = new RectangleD(cx, cy, cw, ch);

                        var text = (StiText)renderedComponent.Clone();
                        text.ClientRectangle = sectorRect;
                        text.HorAlignment = StiTextHorAlignment.Center;
                        text.VertAlignment = StiVertAlignment.Center;
                        text.WordWrap = false;
                        cont.Components.Add(text);

                        string cellText = string.Empty;
                        int indexText = (masterTextInCells.RightToLeft ? textSize.Width - indexX - 1 : indexX);
                        if (indexText < currentLineText.Length)
                        {
                            cellText = new string(currentLineText[indexText], 1);
                        }
                        text.SetTextInternal(cellText);

                        posX += cellWidth + horSpacing;
                    }
                    posY += cellHeight + vertSpacing;
                }
                #endregion

                #endregion
            }
            else
            {
                #region Old mode - ContinuousText
                double posX = rect.X + borderSize;
                double posY = rect.Y + borderSize;

                int widthX = (int)((rect.Width - borderSize) / (cellWidth + horSpacing));
                if (widthX * (cellWidth + horSpacing) > (rect.Width - borderSize)) widthX++;

                bool first = true;
                int index = 0;
                while (1 == 1)
                {
                    double cx = Math.Round(posX, 2);
                    double cy = Math.Round(posY, 2);
                    double cw = Math.Round(posX + cellWidth, 2) - cx;
                    double ch = Math.Round(posY + cellHeight, 2) - cy;
                    RectangleD sectorRect = new RectangleD(cx, cy, cw, ch);

                    if (sectorRect.Right + horSpacing < rect.Right || first)
                    {
                        StiText text = renderedComponent.Clone() as StiText;
                        text.ClientRectangle = sectorRect;
                        text.HorAlignment = StiTextHorAlignment.Center;
                        text.VertAlignment = StiVertAlignment.Center;
                        text.WordWrap = false;
                        cont.Components.Add(text);

                        int indexX = index;
                        if (masterTextInCells.RightToLeft)
                        {
                            int a1 = (index / widthX);
                            int a2 = (index % widthX);
                            indexX = (a1 + 1) * widthX - a2 - 1;
                        }

                        string cellText = string.Empty;
                        if (textString != null && indexX < textString.Length)
                        {
                            cellText = new string(textString[indexX], 1);
                        }
                        text.SetTextInternal(cellText);

                        posX += cellWidth + horSpacing;
                        index++;
                        first = false;
                    }
                    else
                    {
                        posY += cellHeight + vertSpacing;

                        posX = (float)rect.X + borderSize;
                        first = true;

                        if (!masterTextInCells.WordWrap) break;

                        if (measure && masterTextInCells.CanGrow)
                        {
                            if (index >= textString.Length) break;
                        }
                        else
                        {
                            if (rect.Bottom < (posY + cellHeight + vertSpacing)) break;
                        }
                    }
                }
                #endregion
            }
            return cont;
        }

        public static void ReplaceContainerWithContentCells(StiComponent comp, StiContainer cont)
        {
            int compIndex = comp.Parent.Components.IndexOf(comp);
            comp.Parent.Components.RemoveAt(compIndex);
            foreach (StiComponent tempComp in cont.Components)
            {
                tempComp.Left += cont.Left;
                tempComp.Top += cont.Top;
                comp.Parent.Components.Insert(compIndex++, tempComp);
            }
        }
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiTextInCells();
        }

        public override bool IsExportAsImage(Stimulsoft.Report.StiExportFormat format)
        {
            if (format == StiExportFormat.ImageSvg || format == StiExportFormat.ImagePng) return true;

            return base.IsExportAsImage(format);
        }
        #endregion

        /// <summary>
		/// Creates a new StiTextInCells.
		/// </summary>
		public StiTextInCells() : this(RectangleD.Empty, string.Empty)
		{
		}


		/// <summary>
		/// Creates a new StiTextInCells.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiTextInCells(RectangleD rect) : this(rect, string.Empty)
		{
		}
		

		/// <summary>
		/// Creates a new StiTextInCells.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the component.</param>
		/// <param name="text">Text expression.</param>
		public StiTextInCells(RectangleD rect, string text) : base(rect)
		{
			TextBrush = new StiSolidBrush(Color.Black);
			Brush = new StiSolidBrush(Color.White);
			Border.Side = StiBorderSides.All;
			Border.Color = Color.Black;
			Border.Size = 2;

			SetTextInternal(text);
			PlaceOnToolbox = false;
			Font = new Font("Arial", 14, FontStyle.Bold);
		}
	}
}