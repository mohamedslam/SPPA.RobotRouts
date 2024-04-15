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
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Design;
using Stimulsoft.Base.Design;
using System.Drawing;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Pen = Stimulsoft.Drawing.Pen;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report
{
    /// <summary>
    /// Describes the class that contains a base style.
    /// </summary>
    public abstract class StiBaseStyle :
        StiService,
        IStiPropertyGridObject, 
        IStiJsonReportObject
    {
        #region StiService override
        /// <summary>
        /// Gets a service type.
        /// </summary>
        [Browsable(false)]
        public override Type ServiceType => typeof(StiBaseStyle);
        #endregion

        #region IStiJsonReportObject.override
        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", GetType().Name);

            // StiBaseStyle
            jObject.AddPropertyStringNullOrEmpty(nameof(CollectionName), CollectionName);
            jObject.AddPropertyJObject(nameof(Conditions), Conditions.SaveToJsonObject(mode));
            jObject.AddPropertyStringNullOrEmpty(nameof(Description), Description);
            jObject.AddPropertyStringNullOrEmpty(nameof(Name), Name);

            return jObject;
        }

        public virtual void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch(property.Name)
                {
                    case nameof(CollectionName):
                        this.CollectionName = property.DeserializeString();
                        break;

                    case nameof(Conditions):
                        this.Conditions.LoadFromJsonObject((JObject)property.Value);
                        break;

                    case nameof(Description):
                        this.Description = property.DeserializeString();
                        break;

                    case nameof(Name):
                        this.PropName = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region Properties.Static
        public static double? WindowsScaleInternal { get; set; }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public abstract StiComponentId ComponentId { get; }

        [Browsable(false)]
        public string PropName { get; internal set; }

        public abstract StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level);

        public StiEventCollection GetEvents(IStiPropertyGrid propertyGrid)
        {
            return null;
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var style = MemberwiseClone() as StiBaseStyle;

            #region Conditions
            style.conditions = (this.conditions != null)
                ? (StiStyleConditionsCollection)this.conditions.Clone()
                : null;
            #endregion

            return style;
        }
        #endregion

        #region Methods.Equals
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, true, true);
        }

        public bool Equals(object obj, bool allowEqualName, bool allowEqualDescription)
        {
            #region StiStyle
            if (this is StiStyle)
            {
                var style1 = obj as StiStyle;
                
                if (style1 == null)
                    return false;

                var style2 = this as StiStyle;                

                if (!style1.Border.Equals(style2.Border))return false;
                if (!style1.Brush.Equals(style2.Brush))return false;
                if (!style1.Font.Equals(style2.Font))return false;
                if (style1.Image == null && style1.Image != null)return false;
                if (style1.Image != null && style1.Image == null) return false;
                if (style1.Image != null && style2.Image != null && (!style1.Image.Equals(style2.Image)))return false;
                if (!style1.TextBrush.Equals(style2.TextBrush))return false;

                if (!style1.CollectionName.Equals(style2.CollectionName)) return false;
                if (!style1.Conditions.Equals(style2.Conditions)) return false;

                if (style1.AllowUseBorderFormatting != style2.AllowUseBorderFormatting) return false;
                if (style1.AllowUseBorderSides != style2.AllowUseBorderSides) return false;
                if (style1.AllowUseBorderSidesFromLocation != style2.AllowUseBorderSidesFromLocation) return false;
                if (style1.AllowUseBrush != style2.AllowUseBrush) return false;
                if (style1.AllowUseFont != style2.AllowUseFont) return false;
                if (style1.AllowUseHorAlignment != style2.AllowUseHorAlignment) return false;
                if (style1.AllowUseImage != style2.AllowUseImage) return false;
                if (style1.AllowUseTextBrush != style2.AllowUseTextBrush) return false;
                if (style1.AllowUseVertAlignment != style2.AllowUseVertAlignment) return false;
                                
                if (style1.HorAlignment != style2.HorAlignment) return false;
                if (style1.VertAlignment != style2.VertAlignment) return false;
                
                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region Report.StiChartStyle
            if (this is StiChartStyle)
            {
                var style1 = obj as StiChartStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiChartStyle;

                if (!style1.Border.Equals(style2.Border)) return false;
                if (!style1.Brush.Equals(style2.Brush)) return false;
                if (!style1.BasicStyleColor.Equals(style2.BasicStyleColor)) return false;
                if (!style1.BrushType.Equals(style2.BrushType)) return false;
                if (!style1.StyleColors.Equals(style2.StyleColors)) return false;

                if (style1.AllowUseBorderFormatting != style2.AllowUseBorderFormatting) return false;
                if (style1.AllowUseBorderSides != style2.AllowUseBorderSides) return false;
                if (style1.AllowUseBrush != style2.AllowUseBrush) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region Report.StiCardsStyle
            if (this is StiCardsStyle)
            {
                var style1 = obj as StiCardsStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiCardsStyle;

                if (!style1.BackColor.Equals(style2.BackColor)) return false;
                if (!style1.ForeColor.Equals(style2.ForeColor)) return false;
                if (!style1.LineColor.Equals(style2.LineColor)) return false;
                if (!style1.SeriesColors.Equals(style2.SeriesColors)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region Report.StiIndicatorStyle
            if (this is StiIndicatorStyle)
            {
                var style1 = obj as StiIndicatorStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiIndicatorStyle;

                if (!style1.BackColor.Equals(style2.BackColor)) return false;
                if (!style1.GlyphColor.Equals(style2.GlyphColor)) return false;
                if (!style1.ForeColor.Equals(style2.ForeColor)) return false;
                if (!style1.HotBackColor.Equals(style2.HotBackColor)) return false;
                if (!style1.PositiveColor.Equals(style2.PositiveColor)) return false;
                if (!style1.NegativeColor.Equals(style2.NegativeColor)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region Report.StiProgressStyle
            if (this is StiProgressStyle)
            {
                var style1 = obj as StiProgressStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiProgressStyle;

                if (!style1.TrackColor.Equals(style2.TrackColor)) return false;
                if (!style1.BandColor.Equals(style2.BandColor)) return false;
                if (!style1.SeriesColors.Equals(style2.SeriesColors)) return false;
                if (!style1.ForeColor.Equals(style2.ForeColor)) return false;
                if (!style1.BackColor.Equals(style2.BackColor)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region Chart.StiChartStyle
            if (this is Chart.StiChartStyle)
            {
                var style1 = obj as Chart.StiChartStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Chart.StiChartStyle;

                if (style1 is StiCustomStyle && style2 is StiCustomStyle)
                {
                    return ((StiCustomStyleCoreXF)style1.Core).ReportStyleName == ((StiCustomStyleCoreXF)style2.Core).ReportStyleName;
                }

                if (style1 is StiCustomStyle || style2 is StiCustomStyle) return false;

                return style1.GetType() == style2.GetType();
            }
            #endregion

            #region StiCrossTabStyle
            if (this is StiCrossTabStyle)
            {
                var style1 = obj as StiCrossTabStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiCrossTabStyle;

                if (!style1.Color.Equals(style2.Color)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region StiTableStyle
            if (this is StiTableStyle)
            {
                var style1 = obj as StiTableStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiTableStyle;

                if (!style1.HeaderColor.Equals(style2.HeaderColor)) return false;
                if (!style1.HeaderForeground.Equals(style2.HeaderForeground)) return false;
                if (!style1.FooterForeground.Equals(style2.FooterForeground)) return false;
                if (!style1.DataColor.Equals(style2.DataColor)) return false;
                if (!style1.DataForeground.Equals(style2.DataForeground)) return false;
                if (!style1.GridColor.Equals(style2.GridColor)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region StiGaugeStyleXF
            if (this is Stimulsoft.Report.Gauge.StiGaugeStyleXF)
            {
                var style1 = obj as Stimulsoft.Report.Gauge.StiGaugeStyleXF;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Gauge.StiGaugeStyleXF;

                return style1.StyleIdent == style2.StyleIdent;
            }
            #endregion

            #region StiTableElementStyle
            if (this is Stimulsoft.Report.Dashboard.Styles.StiTableElementStyle)
            {
                var style1 = obj as Stimulsoft.Report.Dashboard.Styles.StiTableElementStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Dashboard.Styles.StiTableElementStyle;

                return style1.Ident == style2.Ident;
            }
            #endregion

            #region StiPivotElementStyle
            if (this is Stimulsoft.Report.Dashboard.Styles.StiPivotElementStyle)
            {
                var style1 = obj as Stimulsoft.Report.Dashboard.Styles.StiPivotElementStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Dashboard.Styles.StiPivotElementStyle;

                return style1.Ident == style2.Ident;
            }
            #endregion

            #region StiIndicatorElementStyle
            if (this is Stimulsoft.Report.Dashboard.Styles.StiIndicatorElementStyle)
            {
                var style1 = obj as Stimulsoft.Report.Dashboard.Styles.StiIndicatorElementStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Dashboard.Styles.StiIndicatorElementStyle;

                return style1.Ident == style2.Ident;
            }
            #endregion

            #region StiProgressElementStyle
            if (this is Stimulsoft.Report.Dashboard.Styles.StiProgressElementStyle)
            {
                var style1 = obj as Stimulsoft.Report.Dashboard.Styles.StiProgressElementStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Dashboard.Styles.StiProgressElementStyle;

                return style1.Ident == style2.Ident;
            }
            #endregion

            #region StiMapStyleFX
            if (this is Stimulsoft.Report.Maps.StiMapStyleFX)
            {
                var style1 = obj as Stimulsoft.Report.Maps.StiMapStyleFX;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Maps.StiMapStyleFX;

                if (!style1.AllowDashboard || style2.AllowDashboard)
                    return false;

                return style1.StyleIdent == style2.StyleIdent;
            }
            #endregion

            #region StiControlElementStyle
            if (this is Stimulsoft.Report.Dashboard.Styles.StiControlElementStyle)
            {
                var style1 = obj as Stimulsoft.Report.Dashboard.Styles.StiControlElementStyle;

                if (style1 == null)
                    return false;

                var style2 = this as Stimulsoft.Report.Dashboard.Styles.StiControlElementStyle;

                return style1.Ident == style2.Ident;
            }
            #endregion

            #region StiGaugeStyle
            if (this is StiGaugeStyle)
            {
                var style1 = obj as StiGaugeStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiGaugeStyle;

                if (!style1.Brush.Equals(style2.Brush)) return false;
                if (!style1.BorderColor.Equals(style2.BorderColor)) return false;
                if (!style1.ForeColor.Equals(style2.ForeColor)) return false;
                if (!style1.BorderWidth.Equals(style2.BorderWidth)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region StiMapStyle
            if (this is StiMapStyle)
            {
                var style1 = obj as StiMapStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiMapStyle;

                if (!style1.Colors.Equals(style2.Colors)) return false;
                if (!style1.Heatmap.Equals(style2.Heatmap)) return false;
                if (!style1.HeatmapWithGroup.Equals(style2.HeatmapWithGroup)) return false;
                if (!style1.DefaultColor.Equals(style2.DefaultColor)) return false;
                if (!style1.BackColor.Equals(style2.BackColor)) return false;
                if (!style1.LabelForeground.Equals(style2.LabelForeground)) return false;
                if (!style1.LabelShadowForeground.Equals(style2.LabelShadowForeground)) return false;
                if (!style1.BorderSize.Equals(style2.BorderSize)) return false;
                if (!style1.BorderColor.Equals(style2.BorderColor)) return false;
                if (!style1.BubbleBackColor.Equals(style2.BubbleBackColor)) return false;
                if (!style1.BubbleBorderColor.Equals(style2.BubbleBorderColor)) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            #region StiDialogStyle
            if (this is StiDialogStyle)
            {
                var style1 = obj as StiDialogStyle;

                if (style1 == null)
                    return false;

                var style2 = this as StiDialogStyle;

                if (!style1.ForeColor.Equals(style2.ForeColor)) return false;
                if (!style1.BackColor.Equals(style2.BackColor)) return false;
                if (style1.AllowUseFont != style2.AllowUseFont) return false;
                if (style1.AllowUseBackColor != style2.AllowUseBackColor) return false;
                if (style1.AllowUseForeColor != style2.AllowUseForeColor) return false;

                if (allowEqualName && style1.Name != style2.Name) return false;
                if (allowEqualDescription && style1.Description != style2.Description) return false;

                return true;
            }
            #endregion

            return false;
        }
        #endregion

        #region Methods.Static
        public static void DrawBox(Graphics g, Rectangle rect, StiBaseStyle style, bool paintValue, bool paintImage)
        {
			style.DrawBox(g, rect, paintValue, paintImage);
        }

		public static void DrawStyle(Graphics g, Rectangle rect, StiBaseStyle style, DrawItemState state, bool fillBackground, bool paintImage, Color? background = null)
		{
		    if (style == null) return;

            style.DrawStyle(g, rect, state, fillBackground, paintImage, background);
		}

        public static StiBaseStyle GetStyle(StiComponent component, StiStyleElements styleElements)
        {
            return GetStyle(component, styleElements, null);
        }

        internal static StiBaseStyle GetStyle(StiComponent component, StiStyleElements styleElements, StiBaseStyle componentStyle)
        {
            if (component is StiChart)
            {
                var chart = (StiChart)component;
                return chart.Style as StiBaseStyle;
            }
            else if (component is StiCrossTab)
            {
            }
            else
            {
                StiStyle style = new StiStyle();
                style.GetStyleFromComponent(component, styleElements, componentStyle);
                return style;
            }
            return null;
        }

        public static StiBaseStyle GetStyle(StiComponent component)
        {
            return GetStyle(component, StiStyleElements.All, null);
        }

        internal static StiBaseStyle GetStyle(StiComponent component, StiBaseStyle componentStyle)
        {
            return GetStyle(component, StiStyleElements.All, componentStyle);
        }
        #endregion

        #region Methods.Style
        public abstract void DrawStyle(Graphics g, Rectangle rect, bool paintValue, bool paintImage);

        public virtual void DrawBox(Graphics g, Rectangle rect, bool paintValue, bool paintImage)
        {
            rect.X += StiScaleUI.I1;
            rect.Y += StiScaleUI.I1;
            rect.Width -= StiScaleUI.I2 + 1;
            rect.Height -= StiScaleUI.I2 + 1;

            this.DrawStyle(g, rect, paintValue, paintImage);

            using (var pen = new Pen(Color.FromArgb(150, Color.Gray)))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, rect);
            }
        }

        public virtual Rectangle DrawStyleImage(Graphics g, Rectangle rect, Image image)
        {
            var imageRect = GetStyleImageRect(rect);

            imageRect = new Rectangle(
                            imageRect.X + (imageRect.Width - image.Width) / 2,
                            imageRect.Y + (imageRect.Height - image.Width) / 2, image.Width, image.Height);

            g.DrawImage(image, imageRect);
            return imageRect;
        }

        public virtual void DrawStyleName(Graphics g, Rectangle rect)
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            var textRect = GetStyleNameRect(rect);

            using (var br = new SolidBrush(StiUX.ItemForeground))
            using (var font = new Font("Arial", 8))
            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(Name, font, br, textRect, sf);
            }
        }

        public virtual void DrawStyleImage(Graphics g, Rectangle rect)
        {
            Rectangle imageRect = new Rectangle(rect.X + 2, rect.Y + (rect.Height - 16) / 2, 16, 16);
            Image image = null;

            if (this is StiStyle)
                image = Stimulsoft.Report.Images.StylesResource.Style2013;

            else if (this is StiChartStyle || this is Stimulsoft.Report.Chart.StiChartStyle)
                image = Stimulsoft.Report.Images.StylesResource.StyleChart2013;

            else if (this is StiGaugeStyle)
                image = Stimulsoft.Report.Images.StylesResource.StyleGauge2013;

            else if (this is StiCrossTabStyle)
                image = Stimulsoft.Report.Images.StylesResource.StyleCrossTab2013;

            else if (this is StiDialogStyle)
                image = Stimulsoft.Report.Images.StylesResource.StyleDialog2013;

            g.DrawImageUnscaled(image, imageRect);
        }

        protected virtual Rectangle GetStyleImageRect(Rectangle rect)
        {
            return new Rectangle(rect.X + StiScaleUI.I4, rect.Y + StiScaleUI.I4, rect.Height + StiScaleUI.I4, rect.Height - StiScaleUI.I8);
        }

        protected virtual Rectangle GetStyleNameRect(Rectangle rect, bool paintImage = true)
        {
            if (!paintImage)
            {
                rect.Inflate(-StiScaleUI.I2, -StiScaleUI.I2);
                return rect;
            }
            else
            {
                var imageRect = GetStyleImageRect(rect);

                return new Rectangle(
                    imageRect.Right + StiScaleUI.I4, rect.Y,
                    rect.Width - imageRect.Width - StiScaleUI.I8, rect.Height);
            }
        }

        /// <summary>
        /// Gets the style from the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public abstract void GetStyleFromComponent(StiComponent component, StiStyleElements styleElements);

        /// <summary>
        /// Sets style to the component.
        /// </summary>
        /// <param name="component">Component.</param>
        public abstract void SetStyleToComponent(StiComponent component);
        #endregion

        #region Methods
        /// <summary>
        /// Drawes the style on the specified graphics context.
        /// </summary>
        public void DrawStyle(Graphics g, Rectangle rect, DrawItemState state, bool fillBackground, bool paintImage, Color? background = null)
        {
            if (fillBackground)
                StiControlPaint.DrawItemBackground(g, rect, state, background);

            rect.Inflate(-StiScale.I1, -StiScale.I1);
            DrawBox(g, rect, this, false, paintImage);
        }

        /// <summary>
        /// Saves the style in the stream.
        /// </summary>
        /// <param name="stream">Stream for saving the style.</param>
        public virtual void Save(Stream stream)
        {
            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Serialize(this, stream, GetType().Name);
        }

        /// <summary>
        /// Loads the style from the stream.
        /// </summary>
        /// <param name="stream">Stream for loading the style.</param>
        public virtual void Load(Stream stream)
        {
            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Deserialize(this, stream, GetType().Name);
        }

        /// <summary>
        /// Saves the style in the file.
        /// </summary>
        /// <param name="file">File for style saving.</param>
        public virtual void Save(string file)
        {
            StiFileUtils.ProcessReadOnly(file);
            var stream = new FileStream(file, FileMode.Create, FileAccess.Write);

            var ser = new StiSerializing(new StiReportObjectStringConverter());
            ser.Serialize(this, stream, "StiStyle");

            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Saves the style to the string.
        /// </summary>
        /// <returns>The string which contains the style.</returns>
        public virtual string SaveToString()
        {
            MemoryStream ms = null;
            StreamReader sr = null;
            string styleStr = null;
            try
            {
                ms = new MemoryStream();
                sr = new StreamReader(ms);

                this.Save(ms);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                styleStr = sr.ReadToEnd();

            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Saving style to string");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (ms != null) ms.Close();
            }
            return styleStr;
        }

        /// <summary>
        /// Loads the style from the string.
        /// </summary>
        /// <param name="styleStr">The string which contains the style.</param>
        public virtual void LoadFromString(string styleStr)
        {
            MemoryStream ms = null;
            StreamWriter sw = null;
            try
            {
                ms = new MemoryStream();
                sw = new StreamWriter(ms);
                sw.Write(styleStr);
                sw.Flush();
                ms.Flush();

                ms.Seek(0, SeekOrigin.Begin);

                Load(ms);
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), "Loading style from string");
                StiLogService.Write(this.GetType(), e);

                if (!StiOptions.Engine.HideExceptions) throw;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// Loads the style from the string.
        /// </summary>
        /// <param name="styleStr">The string in the Base 64 format which contains the style.</param>
        public static StiBaseStyle LoadStyleFromString(string styleStr)
        {
            StiBaseStyle baseStyle = null;

            if (styleStr.IndexOf("StiChartStyle", StringComparison.InvariantCulture) != -1)
                baseStyle = new StiChartStyle();

            else if (styleStr.IndexOf("StiGaugeStyle", StringComparison.InvariantCulture) != -1)
                baseStyle = new StiGaugeStyle();

            else if (styleStr.IndexOf("StiCrossTabStyle", StringComparison.InvariantCulture) != -1)
                baseStyle = new StiCrossTabStyle();

            else if (styleStr.IndexOf("StiStyle", StringComparison.InvariantCulture) != -1)
                baseStyle = new StiStyle();

            if (baseStyle != null)
                baseStyle.LoadFromString(styleStr);

            return baseStyle;
        }

        /// <summary>
        /// Loads the style from a file.
        /// </summary>
        /// <param name="file">File for style loading.</param>
        public virtual void Load(string file)
        {
            if (File.Exists(file))
            {
                var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var ser = new StiSerializing(new StiReportObjectStringConverter());
                ser.Deserialize(this, stream, "StiStyle");

                stream.Flush();
                stream.Close();
            }
        }
               
        /// <summary>
        /// Gets a style from the components.
        /// </summary>
        /// <param name="comps">Components collection.</param>
        public void GetStyleFromComponents(StiComponentsCollection comps, StiStyleElements styleElements)
        {
            for (int index = comps.Count - 1; index >= 0; index--)
            {
                GetStyleFromComponent(comps[index], styleElements);
            }
        }
        
        /// <summary>
        /// Returns string representation of the style.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Description) ? Name : Description;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a name of the styles collection.
        /// </summary>
        [StiSerializable]
        [StiCategory("Main")]
        [StiOrder(StiPropertyOrder.StyleCollectionName)]
        [DefaultValue("")]
        [Description("Gets or sets a name of the styles collection.")]
        public string CollectionName { get; set; } = string.Empty;

        private StiStyleConditionsCollection conditions;
        /// <summary>
        /// Gets or sets a collection of the style conditions.
        /// </summary>
        [StiSerializable(StiSerializationVisibility.List)]
        [StiCategory("Main")]
        [Description("Gets or sets a collection of the style conditions.")]
        [StiOrder(StiPropertyOrder.StyleConditions)]
        [TypeConverter(typeof(StiStyleConditionsCollectionConverter))]
        [Editor("Stimulsoft.Report.Design.StiStyleConditionsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]//
        public virtual StiStyleConditionsCollection Conditions
        {
            get
            {
                return conditions ?? (conditions = new StiStyleConditionsCollection());
            }
            set
            {
                conditions = value;
            }
        }

        private bool ShouldSerializeConditions()
        {
            return Conditions == null || Conditions.Count > 0;
        }

        /// <summary>
        /// Gets or sets a style description.
        /// </summary>
        [StiSerializable]
        [DefaultValue("")]
        [Description("Gets or sets a style description.")]
        [StiCategory("Main")]
        [StiOrder(StiPropertyOrder.StyleDescription)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a style name.
        /// </summary>
        [StiSerializable]
        [Description("Gets or sets a style name.")]
        [StiCategory("Main")]
        [StiOrder(StiPropertyOrder.StyleName)]
        public virtual string Name
        {
            get
            {
                return PropName;
            }
            set
            {
                if (Report != null && 
                    Report.IsDesigning && 
                    !string.IsNullOrWhiteSpace(value) && 
                    !string.IsNullOrWhiteSpace(PropName))
                {
                    Report.RenameStyle(PropName, value);
                }

                PropName = value;
            }
        }

        /// <summary>
        /// Gets or sets a style name.
        /// </summary>
        [StiNonSerialized]
        [Browsable(false)]
        public virtual string DashboardName => null;

        [Browsable(false)]
        internal StiReport Report { get; set; }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiBaseStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        internal StiBaseStyle(string name, string description, StiReport report)
        {
            this.Report = report;
            this.PropName = name;
            this.Description = description;
        }

        /// <summary>
        /// Creates a new object of the type StiBaseStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        /// <param name="description">Style description.</param>
        public StiBaseStyle(string name, string description)
            : this(name, description, null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiBaseStyle.
        /// </summary>
        /// <param name="name">Style name.</param>
        public StiBaseStyle(string name)
            : this(name, "")
        {
        }

        /// <summary>
        /// Creates a new object of the type StiBaseStyle.
        /// </summary>
        public StiBaseStyle()
            : this("")
        {
        }
    }
}