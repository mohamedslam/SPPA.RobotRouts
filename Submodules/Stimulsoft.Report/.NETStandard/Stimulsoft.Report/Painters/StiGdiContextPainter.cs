#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Painters.Context.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

#if STIDRAWING
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using GraphicsState = Stimulsoft.Drawing.Drawing2D.GraphicsState;
using Pen = Stimulsoft.Drawing.Pen;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiGdiContextPainter : StiContextPainter, IStiAnimationContextPainter<StiGeom>
    {
        #region Fields
        private bool isClone = false;
        private object lockObject = new object();
        private Graphics graphics;
        private List<GraphicsState> states = new List<GraphicsState>();
        private List<SmoothingMode> smothingModes = new List<SmoothingMode>();
        private List<TextRenderingHint> textRenderingHints = new List<TextRenderingHint>();
        #endregion

        #region Properties
        internal StiContext Context { get; set; }

        public List<StiGeom> Geoms
        {
            get
            {
                return Context.Geoms;
            }
        }

        public StiAnimationEngine AnimationEngine { get; set; }
        #endregion

        #region Methods.Override
        public override StiContextPainter Clone()
        {
            isClone = true;

            var globalMeasureGraphics = Graphics.FromImage(new Bitmap(1, 1));
            globalMeasureGraphics.PageUnit = GraphicsUnit.Pixel;
            globalMeasureGraphics.PageScale = 1f;
            var painter = new StiGdiContextPainter(globalMeasureGraphics);

            return painter;
        }

        public override void Dispose()
        {
            if (isClone)
            {
                this.graphics.Dispose();
            }
        }
        #endregion

        #region Methods.String Format
        public override StiStringFormatGeom GetDefaultStringFormat()
        {
            using (StringFormat sf = new StringFormat())
            {
                return new StiStringFormatGeom(sf);
            }
        }

        public override StiStringFormatGeom GetGenericStringFormat()
        {
            using (StringFormat sf = StringFormat.GenericDefault.Clone() as StringFormat)
            {
                StiStringFormatGeom sfGeom = new StiStringFormatGeom(sf);
                sfGeom.IsGeneric = true;
                return sfGeom;
            }
        }
        #endregion

        #region Methods.Shadow
        public override StiContext CreateShadowGraphics(bool isPrinting, float zoom)
        {
            return new StiContext(new StiGdiContextPainter(this.graphics), true, false, isPrinting, zoom);
        }
        #endregion

        #region Methods.Path
        public override RectangleF GetPathBounds(List<StiSegmentGeom> geoms)
        {
            using (GraphicsPath path = GetPath(geoms))
            {
                return path.GetBounds();
            }
        }
        #endregion

        #region Methods.Measure String
        public override SizeF MeasureString(string text, StiFontGeom font)
        {
            text = CheckSurrogatesAndSupplementaryCharacters(text);
            lock (lockObject)
            {
                using (Font fontGdi = GetFont(font))
                {
                    if (NeedHtmlTags(text))
                    {
                        return MeasureHtmlTags(graphics, text, new RectangleD(0, 0, int.MaxValue, int.MaxValue), fontGdi, null, 0);
                    }
                    else
                    {
                        return graphics.MeasureString(text, fontGdi);
                    }
                }
            }
        }

        public override SizeF MeasureString(string text, StiFontGeom font, int width, StiStringFormatGeom sf)
        {
            text = CheckSurrogatesAndSupplementaryCharacters(text);
            using (Font fontGdi = GetFont(font))
            using (StringFormat sfGdi = GetStringFormat(sf))
            {
                if (NeedHtmlTags(text))
                {
                    return MeasureHtmlTags(graphics, text, new RectangleD(0, 0, width, int.MaxValue), fontGdi, sf, 0);
                }
                else
                {
                    return graphics.MeasureString(text, fontGdi, width, sfGdi);
                }
            }
        }

        public override RectangleF MeasureRotatedString(string text, StiFontGeom font, RectangleF rect, StiStringFormatGeom sf, float angle)
        {
            using (Font fontGdi = GetFont(font))
            using (StringFormat sfGdi = GetStringFormat(sf))
            {
                return StiRotatedTextDrawing.MeasureString(graphics, CheckSurrogatesAndSupplementaryCharacters(text), fontGdi, rect, sfGdi, StiRotationMode.CenterCenter, angle);
            }
        }

        public override RectangleF MeasureRotatedString(string text, StiFontGeom font, RectangleF rect, StiStringFormatGeom sf, StiRotationMode mode, float angle)
        {
            using (Font fontGdi = GetFont(font))
            using (StringFormat sfGdi = GetStringFormat(sf))
            {
                return StiRotatedTextDrawing.MeasureString(graphics, CheckSurrogatesAndSupplementaryCharacters(text), fontGdi, rect, sfGdi, mode, angle);
            }
        }
    
        public override RectangleF MeasureRotatedString(string text, StiFontGeom font, PointF point, StiStringFormatGeom sf, StiRotationMode mode, float angle, int maximalWidth)
        {
            using (Font fontGdi = GetFont(font))
            using (StringFormat sfGdi = GetStringFormat(sf))
            {
                return StiRotatedTextDrawing.MeasureString(graphics, CheckSurrogatesAndSupplementaryCharacters(text), fontGdi, point, sfGdi, mode, angle, maximalWidth);
            }
        }

        public override RectangleF MeasureRotatedString(string text, StiFontGeom font, PointF point, StiStringFormatGeom sf, StiRotationMode mode, float angle)
        {
            using (Font fontGdi = GetFont(font))
            using (StringFormat sfGdi = GetStringFormat(sf))
            {
                return StiRotatedTextDrawing.MeasureString(graphics, CheckSurrogatesAndSupplementaryCharacters(text), fontGdi, point, sfGdi, mode, angle);
            }
        }
        #endregion

        #region Methods.Render
        public override void Render(RectangleF rect, List<StiGeom> geoms)
        {
            StiAnimationEngine.CorrectAnimationTimes(this, geoms);

            foreach (StiGeom geom in geoms)
            {
                try
                {
                    switch (geom.Type)
                    {
                        #region Image
                        case StiGeomType.Image:
                            DrawImageGeom(geom as StiImageGeom);
                            break;
                        #endregion

                        #region Border
                        case StiGeomType.Border:
                            DrawBorderGeom(geom as StiBorderGeom);
                            break;
                        #endregion

                        #region Line
                        case StiGeomType.Line:
                            DrawLineGeom(geom as StiLineGeom);
                            break;
                        #endregion

                        #region Lines
                        case StiGeomType.Lines:
                            DrawLinesGeom(geom as StiLinesGeom);
                            break;
                        #endregion

                        #region Curve
                        case StiGeomType.Curve:
                            DrawCurveGeom(geom as StiCurveGeom);
                            break;
                        #endregion

                        #region Ellipse
                        case StiGeomType.Ellipse:
                            DrawEllipseGeom(geom as StiEllipseGeom);
                            break;
                        #endregion

                        #region CachedShadow
                        case StiGeomType.CachedShadow:
                            DrawCachedShadowGeom(geom as StiCachedShadowGeom);
                            break;
                        #endregion

                        #region Shadow
                        case StiGeomType.Shadow:
                            DrawShadowGeom(geom as StiShadowGeom);
                            break;
                        #endregion

                        #region Path
                        case StiGeomType.Path:
                            DrawPathGeom(geom as StiPathGeom);
                            break;
                        #endregion

                        #region Text
                        case StiGeomType.Text:
                            DrawTextGeom(geom as StiTextGeom);
                            break;
                        #endregion

                        #region PushClipPath
                        case StiGeomType.PushClipPath:
                            PushClipPathGeom(geom as StiPushClipPathGeom);
                            break;
                        #endregion

                        #region PushClip
                        case StiGeomType.PushClip:
                            PushClipGeom(geom as StiPushClipGeom);
                            break;
                        #endregion

                        #region PushTranslateTransform
                        case StiGeomType.PushTranslateTransform:
                            PushTranslateTransformGeom(geom as StiPushTranslateTransformGeom);
                            break;
                        #endregion

                        #region PushRotateTransform
                        case StiGeomType.PushRotateTransform:
                            PushRotateTransformGeom(geom as StiPushRotateTransformGeom);
                            break;
                        #endregion

                        #region PushSmothingModeToAntiAlias
                        case StiGeomType.PushSmothingModeToAntiAlias:
                            PushSmothingModeToAntiAliasGeom(geom as StiPushSmothingModeToAntiAliasGeom);
                            break;
                        #endregion

                        #region PushTextRenderingHintToAntiAlias
                        case StiGeomType.PushTextRenderingHintToAntiAlias:
                            PushTextRenderingHintToAntiAliasGeom(geom as StiPushTextRenderingHintToAntiAliasGeom);
                            break;
                        #endregion

                        #region PopSmothingMode
                        case StiGeomType.PopSmothingMode:
                            PopSmothingModeGeom(geom as StiPopSmothingModeGeom);
                            break;
                        #endregion

                        #region PopTextRenderingHint
                        case StiGeomType.PopTextRenderingHint:
                            PopTextRenderingHintGeom(geom as StiPopTextRenderingHintGeom);
                            break;
                        #endregion

                        #region PopTransform
                        case StiGeomType.PopTransform:
                            PopTransformGeom(geom as StiPopTransformGeom);
                            break;
                        #endregion

                        #region PopClip
                        case StiGeomType.PopClip:
                            PopClipGeom(geom as StiPopClipGeom);
                            break;
                        #endregion

                        case StiGeomType.AnimationBar:
                            DrawAnimationBar(geom as StiClusteredBarSeriesAnimationGeom);
                            break;

                        case StiGeomType.AnimationBorder:
                            DrawAnimationBorder(geom as StiBorderAnimationGeom);
                            break;

                        case StiGeomType.AnimationColumn:
                            DrawAnimationColumn(geom as StiClusteredColumnSeriesAnimationGeom);
                            break;

                        case StiGeomType.AnimationEllipse:
                            DrawAnimationEllipse(geom as StiEllipseAnimationGeom);
                            break;

                        case StiGeomType.AnimationPath:
                            DrawAnimationPath(geom as StiPathAnimationGeom);
                            break;

                        case StiGeomType.AnimationPathElement:
                            DrawAnimationPathElement(geom as StiPathElementAnimationGeom);
                            break;

                        case StiGeomType.AnimationLines:
                            DrawAnimationLines(geom as StiLinesAnimationGeom);
                            break;

                        case StiGeomType.AnimationCurve:
                            DrawAnimationCurve(geom as StiCurveAnimationGeom);
                            break;

                        case StiGeomType.AnimationLabel:
                            DrawAnimationLabel(geom as StiLabelAnimationGeom);
                            break;

                        case StiGeomType.AnimationShadow:
                            DrawAnimationShadow(geom as StiShadowAnimationGeom);
                            break;
                    }
                }
                catch
                {
                }
            }
        }

        private void DrawImageGeom(StiImageGeom imageGeom)
        {
            using (var image = StiImageHelper.GetImageFromObject(imageGeom.Image))
            {
                var rect = imageGeom.Rect;
                var scaleX = rect.Width / image.Width;
                var scaleY = rect.Height / image.Height;

                scaleX = scaleY = scaleX < scaleY ? scaleX : scaleY;

                var imageWidth = (float)(image.Width * scaleX);
                var imageHeight = (float)(image.Height * scaleY);

                var x = rect.X + (rect.Width - imageWidth) / 2;
                var y = rect.Y + (rect.Height - imageHeight) / 2;

                rect = new RectangleF(x, y, imageWidth, imageHeight);

                graphics.DrawImage(image, rect);
            }
        }

        private void DrawBorderGeom(StiBorderGeom borderGeom)
        {
            var rectF = GetRectangleF(borderGeom.Rect);

            using (var backgroundBrush = GetBrush(borderGeom.Background, rectF))
            {
                using (var borderPen = GetPen(borderGeom.BorderPen))
                {
                    var point = new PointF(rectF.X + rectF.Width / 2, rectF.Y + rectF.Height / 2);
                    if (borderGeom.Angle != 0)
                    {
                        graphics.TranslateTransform(point.X, point.Y);
                        graphics.RotateTransform(borderGeom.Angle);
                        var startPoint = StiRotatedTextDrawing.GetStartPoint(borderGeom.RotationMode, new RectangleF(0, 0, rectF.Width, rectF.Height));
                        rectF.X = -startPoint.X;
                        rectF.Y = -startPoint.Y;
                    }


                    if (borderGeom.CornerRadius != null && !borderGeom.CornerRadius.IsEmpty)
                    {
                        using (var path = StiRoundedRectangleCreator.Create(rectF, borderGeom.CornerRadius, 1))
                        {
                            if (backgroundBrush != null)
                                graphics.FillPath(backgroundBrush, path);

                            if (borderPen != null)
                                graphics.DrawPath(borderPen, path);
                        }
                    }
                    else
                    {
                        if (backgroundBrush != null)
                            graphics.FillRectangle(backgroundBrush, rectF);

                        if (borderPen != null)
                            graphics.DrawRectangle(borderPen, rectF.X, rectF.Y, rectF.Width, rectF.Height);
                    }

                    if (borderGeom.Angle != 0)
                    {
                        graphics.RotateTransform(-borderGeom.Angle);
                        graphics.TranslateTransform(-point.X, -point.Y);
                    }
                }
            }
        }

        private void DrawLineGeom(StiLineGeom lineGeom)
        {
            Pen linePen = GetPen(lineGeom.Pen);
            if (linePen == null)
                return;

            graphics.DrawLine(linePen, lineGeom.X1, lineGeom.Y1, lineGeom.X2, lineGeom.Y2);

            if (linePen != null)
                linePen.Dispose();
        }

        private void DrawLinesGeom(StiLinesGeom linesGeom)
        {
            Pen linesPen = GetPen(linesGeom.Pen);
            if (linesPen == null)
                return;

            graphics.DrawLines(linesPen, linesGeom.Points);

            if (linesPen != null)
                linesPen.Dispose();
        }

        private void DrawCurveGeom(StiCurveGeom curveGeom)
        {
            Pen curvePen = GetPen(curveGeom.Pen);
            if (curvePen == null)
                return;

            graphics.DrawCurve(curvePen, curveGeom.Points, curveGeom.Tension);

            if (curvePen != null)
                curvePen.Dispose();
        }

        private void DrawEllipseGeom(StiEllipseGeom ellipseGeom)
        {
            object rect = GetRect(ellipseGeom.Rect);

            Brush backgroundBrush = GetBrush(ellipseGeom.Background, rect);
            Pen borderPen = GetPen(ellipseGeom.BorderPen);

            if (rect is Rectangle)
            {
                if (backgroundBrush != null)
                    graphics.FillEllipse(backgroundBrush, (Rectangle)rect);
                if (borderPen != null)
                    graphics.DrawEllipse(borderPen, (Rectangle)rect);
            }
            else if (rect is RectangleF)
            {
                RectangleF rectF = (RectangleF)rect;
                if (backgroundBrush != null)
                    graphics.FillEllipse(backgroundBrush, rectF);
                if (borderPen != null)
                    graphics.DrawEllipse(borderPen, rectF.X, rectF.Y, rectF.Width, rectF.Height);
            }
            if (backgroundBrush != null)
                backgroundBrush.Dispose();
            if (borderPen != null)
                borderPen.Dispose();
        }

        private void DrawCachedShadowGeom(StiCachedShadowGeom cachedShadowGeom)
        {
            var rect = GetRectangleF(cachedShadowGeom.Rect);
            StiShadow.DrawCachedShadow(graphics, rect, cachedShadowGeom.Sides, cachedShadowGeom.CornerRadius, cachedShadowGeom.IsPrinting);            
        }

        private void DrawShadowGeom(StiShadowGeom shadowGeom)
        {
            using (StiShadowGraphics shadowGraphics = new StiShadowGraphics(shadowGeom.Rect))
            {
                ((StiGdiContextPainter)shadowGeom.ShadowContext.ContextPainter).graphics = shadowGraphics.Graphics;
                shadowGeom.ShadowContext.Render(shadowGeom.Rect);
                shadowGraphics.DrawShadow(graphics, shadowGeom.Rect, shadowGeom.Radius);
            }
        }

        private void DrawPathGeom(StiPathGeom pathGeom)
        {
            using (GraphicsPath path = GetPath(pathGeom.Geoms))
            {
                object rect = (pathGeom.Rect == StiPathGeom.GetBoundsState) ? path.GetBounds() : GetRect(pathGeom.Rect);

                Brush backgroundBrush = GetBrush(pathGeom.Background, rect);
                Pen borderPen = GetPen(pathGeom.Pen);

                if (backgroundBrush != null)
                    graphics.FillPath(backgroundBrush, path);
                if (borderPen != null)
                    graphics.DrawPath(borderPen, path);

                if (backgroundBrush != null)
                    backgroundBrush.Dispose();
                if (borderPen != null)
                    borderPen.Dispose();
            }

        }

        private void DrawTextGeom(StiTextGeom textGeom)
        {
            var textBrush = textGeom.Brush;

            if (textGeom.Animation is StiOpacityAnimation animation)
                textBrush = StiAnimationEngine.GetAnimationOpacity(this, textBrush, animation);

            var textHint = graphics.TextRenderingHint;

            if (textGeom.Antialiasing)
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            var text = CheckSurrogatesAndSupplementaryCharacters(textGeom.Text);

            if (!textGeom.IsRotatedText)
            {
                object rectObj = GetRect(textGeom.Location);
                if (rectObj is Rectangle rect)
                {
                    using (var br = GetBrush(textBrush, rect))
                    using (var fontGdi = GetFont(textGeom.Font))
                    using (var sfGdi = GetStringFormat(textGeom.StringFormat))
                    {
                        if (NeedHtmlTags(text))
                        {
                            DrawHtmlTags(graphics, textGeom, RectangleD.CreateFromRectangle(rect), fontGdi);
                        }
                        else
                        {
                            graphics.DrawString(text, fontGdi, br, rect, sfGdi);
                        }
                    }
                }
                else if (rectObj is RectangleF)
                {
                    RectangleF rectF = (RectangleF)rectObj;

                    using (Brush br = GetBrush(textBrush, rectF))
                    using (Font fontGdi = GetFont(textGeom.Font))
                    using (StringFormat sfGdi = GetStringFormat(textGeom.StringFormat))
                    {
                        if (NeedHtmlTags(text))
                        {
                            DrawHtmlTags(graphics, textGeom, RectangleD.CreateFromRectangle(rectF), fontGdi);
                        }
                        else
                        {
                            graphics.DrawString(text, fontGdi, br, rectF, sfGdi);
                        }
                    }
                }
            }
            else
            {
                object rectObj = GetRect(textGeom.Location);
                if ((rectObj is RectangleF || rectObj is Rectangle) && textGeom.IsRounded)
                {
                    var rect = Rectangle.Empty;

                    if (rectObj is Rectangle)
                        rect = (Rectangle)rectObj;
                    else
                        rect = Rectangle.Round((RectangleF)rectObj);

                    using (var br = GetBrush(textBrush, rect))
                    using (var fontGdi = GetFont(textGeom.Font))
                    using (var sfGdi = GetStringFormat(textGeom.StringFormat))
                    {
                        if (NeedHtmlTags(text))
                        {
                            DrawHtmlTags(graphics, textGeom, RectangleD.CreateFromRectangle(rect), fontGdi);
                        }
                        else
                        {
                            StiRotatedTextDrawing.DrawString(graphics, text, fontGdi, br, rect, sfGdi, textGeom.RotationMode.GetValueOrDefault(), textGeom.Angle, textGeom.Antialiasing);
                        }
                    }
                }
                else if (rectObj is RectangleF || rectObj is Rectangle)
                {
                    RectangleF rectF;

                    if (rectObj is RectangleF)
                        rectF = (RectangleF)rectObj;
                    else
                        rectF = (Rectangle)rectObj;

                    using (var br = GetBrush(textBrush, rectF))
                    using (var fontGdi = GetFont(textGeom.Font))
                    using (var sfGdi = GetStringFormat(textGeom.StringFormat))
                    {
                        if (NeedHtmlTags(text))
                        {
                            DrawHtmlTags(graphics, textGeom, RectangleD.CreateFromRectangle(rectF), fontGdi);
                        }
                        else
                        {
                            if (textGeom.MaximalWidth != null)
                                StiRotatedTextDrawing.DrawString(graphics, text, fontGdi, br, rectF, sfGdi, textGeom.RotationMode.GetValueOrDefault(), textGeom.Angle, textGeom.Antialiasing, textGeom.MaximalWidth.GetValueOrDefault());
                            else
                                StiRotatedTextDrawing.DrawString(graphics, text, fontGdi, br, rectF, sfGdi, textGeom.RotationMode.GetValueOrDefault(), textGeom.Angle, textGeom.Antialiasing);
                        }
                    }
                }
                else if (textGeom.Location is PointF)
                {
                    var pointF = (PointF)textGeom.Location;

                    using (var br = GetBrush(textBrush, RectangleF.Empty))
                    using (var fontGdi = GetFont(textGeom.Font))
                    using (var sfGdi = GetStringFormat(textGeom.StringFormat))
                    {
                        if (textGeom.MaximalWidth != null)
                            StiRotatedTextDrawing.DrawString(graphics, text, fontGdi, br, pointF, sfGdi, textGeom.RotationMode.GetValueOrDefault(), textGeom.Angle, textGeom.Antialiasing, textGeom.MaximalWidth.GetValueOrDefault());
                        else
                            StiRotatedTextDrawing.DrawString(graphics, text, fontGdi, br, pointF, sfGdi, textGeom.RotationMode.GetValueOrDefault(), textGeom.Angle, textGeom.Antialiasing);
                    }
                }
            }

            if (textGeom.Antialiasing)
                graphics.TextRenderingHint = textHint;
        }

        private void PushTranslateTransformGeom(StiPushTranslateTransformGeom translate)
        {
            PushState();
            graphics.TranslateTransform(translate.X, translate.Y);
        }

        private void PushClipPathGeom(StiPushClipPathGeom clip)
        {
            PushState();
            graphics.SetClip(GetPath(clip.Geoms), CombineMode.Intersect);
        }

        private void PushClipGeom(StiPushClipGeom clip)
        {
            PushState();
            graphics.SetClip(clip.ClipRectangle, CombineMode.Intersect);
        }

        private void PushRotateTransformGeom(StiPushRotateTransformGeom rotate)
        {
            PushState();
            graphics.RotateTransform(rotate.Angle);
        }

        private void PopClipGeom(StiPopClipGeom clip)
        {
            PopState();
        }

        private void PopTransformGeom(StiPopTransformGeom translate)
        {
            PopState();
        }

        private void PushSmothingModeToAntiAliasGeom(StiPushSmothingModeToAntiAliasGeom smothing)
        {
            PushState();
            smothingModes.Add(graphics.SmoothingMode);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void PopSmothingModeGeom(StiPopSmothingModeGeom smothing)
        {
            if (smothingModes.Count > 0)
            {
                graphics.SmoothingMode = smothingModes[smothingModes.Count - 1];
                smothingModes.RemoveAt(smothingModes.Count - 1);
            }

            PopState();
        }

        private void PushTextRenderingHintToAntiAliasGeom(StiPushTextRenderingHintToAntiAliasGeom text)
        {
            PushState();
            textRenderingHints.Add(graphics.TextRenderingHint);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        }

        private void PopTextRenderingHintGeom(StiPopTextRenderingHintGeom text)
        {
            graphics.TextRenderingHint = textRenderingHints[textRenderingHints.Count - 1];
            textRenderingHints.RemoveAt(textRenderingHints.Count - 1);
            PopState();
        }

        private void PushState()
        {
            states.Add(graphics.Save());
        }

        private void PopState()
        {
            if (states.Count > 0)
            {
                graphics.Restore(states[states.Count - 1]);
                states.RemoveAt(states.Count - 1);
            }
        }

        private void DrawAnimationBar(StiClusteredBarSeriesAnimationGeom animationGeom)
        {
            var rect = (RectangleF)animationGeom.ColumnRect;
            var background = animationGeom.Background;
            var borderPen = animationGeom.BorderPen;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    borderPen = StiAnimationEngine.GetAnimationOpacity(this, borderPen, animation);
                    break;

                case StiColumnAnimation animation:
                    var from = animation.RectFrom;
                    rect = StiAnimationEngine.GetAnimationRectangle(this, rect, from, animation);
                    break;
            }

            DrawBorderGeom(
                new StiBorderGeom(background, null, borderPen, rect, animationGeom.CornerRadius, animationGeom.Interaction, -1));
        }

        private void DrawAnimationColumn(StiClusteredColumnSeriesAnimationGeom animationGeom)
        {
            var rect = (RectangleF)animationGeom.ColumnRect;
            var background = animationGeom.Background;
            var borderPen = animationGeom.BorderPen;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    borderPen = StiAnimationEngine.GetAnimationOpacity(this, borderPen, animation);
                    break;

                case StiColumnAnimation animation:
                    var from = animation.RectFrom;
                    rect = StiAnimationEngine.GetAnimationRectangle(this, rect, from, animation);
                    break;
            }

            DrawBorderGeom(
                new StiBorderGeom(background, null, borderPen, rect, animationGeom.CornerRadius, animationGeom.Interaction, -1));
        }

        private void DrawAnimationBorder(StiBorderAnimationGeom animationGeom)
        {
            var rect = (RectangleF)animationGeom.Rect;
            var background = animationGeom.Background;
            var borderPen = animationGeom.BorderPen;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    borderPen = StiAnimationEngine.GetAnimationOpacity(this, borderPen, animation);
                    break;
                case StiColumnAnimation animation:
                    rect = StiAnimationEngine.GetAnimationRectangle(this, rect, animation.RectFrom, animation);
                    break;
            }

            DrawBorderGeom(
                new StiBorderGeom(background, null, borderPen, rect, animationGeom.CornerRadius, animationGeom.Interaction, -1));
        }

        private void DrawAnimationEllipse(StiEllipseAnimationGeom animationGeom)
        {
            var rect = (RectangleF)animationGeom.Rect;
            var background = animationGeom.Background;
            var borderPen = animationGeom.BorderPen;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    borderPen = StiAnimationEngine.GetAnimationOpacity(this, borderPen, animation);
                    break;
                case StiScaleAnimation animation:
                    rect = StiAnimationEngine.GetAnimationScale(this, rect, animation);
                    break;
            }

            DrawEllipseGeom(
                new StiEllipseGeom(background, borderPen, rect, animationGeom.Interaction, -1));
        }

        private void DrawAnimationPath(StiPathAnimationGeom animationGeom)
        {
            var background = animationGeom.Background;
            var pen = animationGeom.Pen;
            var geoms = animationGeom.Geoms;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    pen = StiAnimationEngine.GetAnimationOpacity(this, pen, animation);
                    break;

                case StiScaleAnimation animation:
                    geoms = StiAnimationEngine.GetAnimationScale(this, geoms, animation);
                    break;

                case null:
                    geoms = geoms.Select(segmentGeom =>
                    {
                        switch (segmentGeom)
                        {
                            case StiCurveSegmentGeom geom:
                                ((StiPointsAnimation)geom.Animation).BeginTime = TimeSpan.Zero;
                                return new StiCurveSegmentGeom(StiAnimationEngine.GetAnimationPoints(this, geom.Points, (StiPointsAnimation)geom.Animation), geom.Tension);

                            case StiLineSegmentGeom geom:
                                ((StiPointsAnimation)geom.Animation).BeginTime = TimeSpan.Zero;
                                var points = StiAnimationEngine.GetAnimationPoints(this, new PointF[] { new PointF(geom.X1, geom.Y1), new PointF(geom.X2, geom.Y2) }, (StiPointsAnimation)geom.Animation);
                                return new StiLineSegmentGeom(points[0], points[1]);

                            case StiLinesSegmentGeom geom:
                                ((StiPointsAnimation)geom.Animation).BeginTime = TimeSpan.Zero;
                                return new StiLinesSegmentGeom(StiAnimationEngine.GetAnimationPoints(this, geom.Points, (StiPointsAnimation)geom.Animation));

                            default: return segmentGeom;
                        }
                    }).ToList();
                    break;
            }

            DrawPathGeom(
                new StiPathGeom(background, pen, geoms, animationGeom.Rect, animationGeom.Interaction, -1));
        }

        private void DrawAnimationPathElement(StiPathElementAnimationGeom animationGeom)
        {
            var background = animationGeom.Background;
            var borderPen = animationGeom.BorderPen;
            var geoms = animationGeom.PathGeoms.ToList();

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    background = StiAnimationEngine.GetAnimationOpacity(this, background, animation);
                    borderPen = StiAnimationEngine.GetAnimationOpacity(this, borderPen, animation);
                    break;
                case StiPieSegmentAnimation animation:
                    if (animation.RectDtFrom.IsEmpty)
                    {
                        geoms = geoms.Select(segmentGeom =>
                        {
                            switch (segmentGeom)
                            {
                                case StiPieSegmentGeom geom:
                                    return StiAnimationEngine.GetAnimationAngle(this, geom, animation);
                                default:
                                    return segmentGeom;
                            }
                        }).ToList();
                    }
                    else if (geoms.Count == 4 && geoms[0] is StiArcSegmentGeom && geoms[2] is StiArcSegmentGeom)
                    {
                        geoms = StiAnimationEngine.GetAnimationAngle(this, geoms, animation);
                    }
                    break;
            }

            DrawPathGeom(
                new StiPathGeom(background, borderPen, geoms, animationGeom.Rect, animationGeom.Interaction, -1));
        }

        private void DrawAnimationLines(StiLinesAnimationGeom animationGeom)
        {
            var pen = animationGeom.Pen;
            var points = animationGeom.Points;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    pen = StiAnimationEngine.GetAnimationOpacity(this, pen, animation);
                    break;
                case StiPointsAnimation animation:
                    points = StiAnimationEngine.GetAnimationPoints(this, points, animation);
                    break;
                case StiTranslationAnimation animation:
                    pen = StiAnimationEngine.GetAnimationTranslation(this, points, pen, animation);
                    break;
            }

            DrawLinesGeom(
                new StiLinesGeom(pen, points));
        }

        private void DrawAnimationCurve(StiCurveAnimationGeom animationGeom)
        {
            var pen = animationGeom.Pen;
            var points = animationGeom.Points;

            switch (animationGeom.Animation)
            {
                case StiOpacityAnimation animation:
                    pen = StiAnimationEngine.GetAnimationOpacity(this, pen, animation);
                    break;
                case StiPointsAnimation animation:
                    points = StiAnimationEngine.GetAnimationPoints(this, points, animation);
                    break;
                case StiTranslationAnimation animation:
                    pen = StiAnimationEngine.GetAnimationTranslation(this, points, pen, animation);
                    break;
            }

            DrawCurveGeom(
                new StiCurveGeom(pen, points, animationGeom.Tension));
        }

        private void DrawAnimationLabel(StiLabelAnimationGeom animationGeom)
        {
            var textBrush = animationGeom.TextBrush;
            var labelBrush = animationGeom.LabelBrush;
            var penBorder = animationGeom.PenBorder;

            if (animationGeom.Animation is StiOpacityAnimation animation)
            {
                textBrush = StiAnimationEngine.GetAnimationOpacity(this, textBrush, animation);
                labelBrush = StiAnimationEngine.GetAnimationOpacity(this, labelBrush, animation);
                penBorder = StiAnimationEngine.GetAnimationOpacity(this, penBorder, animation);
            }

            if (animationGeom.DrawBorder)
                DrawBorderGeom(
                    new StiBorderGeom(labelBrush, null, penBorder, animationGeom.Rectangle, animationGeom.Interaction, -1, animationGeom.Angle, (StiRotationMode)animationGeom.RotationMode));

            DrawTextGeom(
                new StiTextGeom(animationGeom.Text, animationGeom.Font, textBrush, animationGeom.Rectangle, animationGeom.StringFormat, animationGeom.Angle, true, null, animationGeom.RotationMode, animationGeom.Angle != 0));
        }

        private void DrawAnimationShadow(StiShadowAnimationGeom animationGeom)
        {
            var brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));

            var rect = animationGeom.Rect;
            for (int indexPix = 1; indexPix <= animationGeom.ShadowWidth; indexPix++)
            {
                rect.X++;
                rect.Y++;
                brush = new SolidBrush(Color.FromArgb((byte)(150 / animationGeom.ShadowWidth * (animationGeom.ShadowWidth - indexPix)), 0, 0, 0));

                switch (animationGeom.Animation)
                {
                    case StiOpacityAnimation animation:
                        brush = (SolidBrush)StiAnimationEngine.GetAnimationOpacity(this, brush, animation);
                        break;
                }

                var path = new GraphicsPath();
                if (animationGeom.RadiusX != 0)
                {
                    path.AddEllipse(rect);
                }

                else if (animationGeom.CornerRadius != null && !animationGeom.CornerRadius.IsEmpty)
                {
                    path = StiRoundedRectangleCreator.Create(rect, animationGeom.CornerRadius, 1);
                }

                else
                {
                    path.AddRectangle(rect);
                }

                graphics.FillPath(brush, path);
            }

            brush.Dispose();
        }
        #endregion

        #region Methods.Helper
        private PenAlignment GetPenAlignment(StiPenAlignment alignment)
        {
            switch (alignment)
            {
                case StiPenAlignment.Inset:
                    return PenAlignment.Inset;

                case StiPenAlignment.Left:
                    return PenAlignment.Left;

                case StiPenAlignment.Outset:
                    return PenAlignment.Outset;

                case StiPenAlignment.Right:
                    return PenAlignment.Right;

                case StiPenAlignment.Center:
                    return PenAlignment.Center;

                default:
                    return PenAlignment.Center;
            }
        }

        private object GetRect(object rect)
        {
            if (rect is Rectangle)
                return (Rectangle)rect;
            if (rect is RectangleF)
                return (RectangleF)rect;
            if (rect is RectangleD)
                return ((RectangleD)rect).ToRectangleF();
            return null;
        }

        private RectangleF GetRectangleF(object rect)
        {
            if (rect is Rectangle rectangle)
                return rectangle;

            if (rect is RectangleF rectangleF)
                return rectangleF;

            if (rect is RectangleD rectangleD)
                return rectangleD.ToRectangleF();

            return RectangleF.Empty;
        }

        private GraphicsPath GetPath(List<StiSegmentGeom> geoms)
        {
            GraphicsPath path = new GraphicsPath();
            foreach (StiSegmentGeom geom in geoms)
            {
                if (geom is StiArcSegmentGeom arcSegment)
                {
                    path.AddArc(arcSegment.Rect, arcSegment.StartAngle, arcSegment.SweepAngle);
                }

                else if (geom is StiCloseFigureSegmentGeom)
                {
                    path.CloseAllFigures();
                }

                else if (geom is StiCurveSegmentGeom curveSegment)
                {
                    path.AddCurve(curveSegment.Points, curveSegment.Tension);
                }

                else if (geom is StiBezierSegmentGeom bezierSegmentGeom)
                {
                    path.AddBezier(bezierSegmentGeom.Pt1, bezierSegmentGeom.Pt2, bezierSegmentGeom.Pt3, bezierSegmentGeom.Pt4);
                }

                else if (geom is StiLineSegmentGeom lineSegment)
                {
                    path.AddLine(lineSegment.X1, lineSegment.Y1, lineSegment.X2, lineSegment.Y2);
                }

                else if (geom is StiLinesSegmentGeom linesSegment)
                {
                    path.AddLines(linesSegment.Points);
                }

                else if (geom is StiPieSegmentGeom pieSegment)
                {
                    path.AddPie(pieSegment.Rect.X, pieSegment.Rect.Y, pieSegment.Rect.Width, pieSegment.Rect.Height, pieSegment.StartAngle, pieSegment.SweepAngle);
                }
            }
            return path;
        }

        private Brush GetBrush(object brush, object rect)
        {
            if (brush == null)
                return null;
            if (brush is Color)
                return new SolidBrush((Color)brush);

            if (brush is StiBrush)
            {
                if (rect is Rectangle)
                    return StiBrush.GetBrush((StiBrush)brush, (Rectangle)rect);
                if (rect is RectangleF)
                    return StiBrush.GetBrush((StiBrush)brush, (RectangleF)rect);
            }

            return brush as Brush;
        }

        private Pen GetPen(StiPenGeom penGeom)
        {
            if (penGeom == null)
                return null;

            if (penGeom.Brush == null)
                return null;

            if (penGeom.PenStyle == StiPenStyle.None)
                return null;

            if (penGeom.Brush is Color)
            {
                Pen pen = new Pen((Color)penGeom.Brush, penGeom.Thickness);
                pen.Alignment = GetPenAlignment(penGeom.Alignment);
                pen.DashStyle = StiPenUtils.GetPenStyle(penGeom.PenStyle);
                pen.StartCap = StiPenUtils.GetLineCap(penGeom.StartCap);
                pen.EndCap = StiPenUtils.GetLineCap(penGeom.EndCap);
                return pen;
            }

            if (penGeom.Brush is Brush)
            {
                return new Pen((Brush)penGeom.Brush, penGeom.Thickness)
                {
                    Alignment = GetPenAlignment(penGeom.Alignment),
                    DashStyle = StiPenUtils.GetPenStyle(penGeom.PenStyle),
                    StartCap = StiPenUtils.GetLineCap(penGeom.StartCap),
                    EndCap = StiPenUtils.GetLineCap(penGeom.EndCap)
                };
            }

            return null;
        }

        private Font GetFont(StiFontGeom fontGeom)
        {
            if (fontGeom.FontFamily != null)
                return new Font(fontGeom.FontFamily, (float)(fontGeom.FontSize * StiDpiHelper.DeviceCapsScale));

            return new Font(StiFontCollection.GetFontFamily(fontGeom.FontName), (float)(fontGeom.FontSize * StiDpiHelper.DeviceCapsScale), fontGeom.FontStyle, fontGeom.Unit, fontGeom.GdiCharSet, fontGeom.GdiVerticalFont);
        }

        private StringFormat GetStringFormat(StiStringFormatGeom sfGeom)
        {
            StringFormat sf = sfGeom.IsGeneric ? StringFormat.GenericDefault.Clone() as StringFormat : new StringFormat();
            sf.Alignment = sfGeom.Alignment;
            sf.FormatFlags = sfGeom.FormatFlags;
            sf.HotkeyPrefix = sfGeom.HotkeyPrefix;
            sf.LineAlignment = sfGeom.LineAlignment;
            sf.Trimming = sfGeom.Trimming;

            return sf;
        }

        private bool NeedHtmlTags(string text)
        {
            return StiOptions.Engine.AllowHtmlTagsInChart && StiTextRenderer.CheckTextForHtmlTags(text);
        }

        private void DrawHtmlTags(Graphics graphics, StiTextGeom textGeom, RectangleD rect, Font fontGdi)
        {
            StiTextRenderer.DrawText(
                graphics,
                textGeom.Text,
                fontGdi,
                rect,
                StiBrush.ToColor(textGeom.Brush as StiBrush),
                Color.Transparent,
                1f,
                (StiTextHorAlignment)textGeom.StringFormat.Alignment,
                (StiVertAlignment)textGeom.StringFormat.LineAlignment,
                !((textGeom.StringFormat.FormatFlags & StringFormatFlags.NoWrap) > 0),
                (textGeom.StringFormat.FormatFlags & StringFormatFlags.DirectionRightToLeft) > 0,
                1f,
                textGeom.Angle,
                textGeom.StringFormat.Trimming,
                false,
                true);
        }

        private SizeF MeasureHtmlTags(Graphics graphics, string text, RectangleD rect, Font fontGdi, StiStringFormatGeom sf, float angle)
        {
            return StiTextRenderer.MeasureText(
                graphics,
                text,
                fontGdi,
                rect,
                1f,
                sf != null ? !((sf.FormatFlags & StringFormatFlags.NoWrap) > 0) : false,
                sf != null ? (sf.FormatFlags & StringFormatFlags.DirectionRightToLeft) > 0 : false,
                1f,
                angle,
                sf != null ? sf.Trimming : StringTrimming.None,
                false,
                true, new StiTextOptions(), 1).ToSizeF();
        }

        private static string CheckSurrogatesAndSupplementaryCharacters(string txt)
        {
#if NETSTANDARD
            if (txt == null || txt.Length < 2) return txt;


            //check for Surrogates and Supplementary Characters; it's workaround for Linux - strange random bugs in GDI+
            var sb = new global::System.Text.StringBuilder();
            bool isChanged = false;
            for (int indexChar = 0; indexChar < txt.Length; indexChar++)
            {
                int ch = txt[indexChar];

                //check for Surrogates and Supplementary Characters; D800-DBFF, DC00-DFFF. 
                if (((ch & 0xD800) == 0xD800) && (indexChar + 1 < txt.Length) && ((txt[indexChar + 1] & 0xDC00) == 0xDC00))
                {
                    indexChar++;
                    sb.Append("¿");
                    isChanged = true;
                }
                else
                {
                    sb.Append((char)ch);
                }
            }
            return isChanged ? sb.ToString() : txt;
#else
            return txt;
#endif
        }
        #endregion

        public StiGdiContextPainter(Graphics graphics)
        {
            this.graphics = graphics;
        }
    }
}
