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

using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Painters;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Stimulsoft.Base.Dashboard.StiElementConsts;

namespace Stimulsoft.Base.Context
{
    public class StiContext
    {
        #region Properties
        public StiContextPainter ContextPainter { get; }

        public StiContextOptions Options { get; }

        public List<StiGeom> Geoms { get; } = new List<StiGeom>();

        public List<StiAnimation> Animations { get; }
        #endregion

        #region Methods.Render
        public void Render(RectangleF rect)
        {
            ContextPainter.Render(rect, Geoms);
        }
        #endregion

        #region Methods.StringFormat
        public StiStringFormatGeom GetDefaultStringFormat()
        {
            return ContextPainter.GetDefaultStringFormat();
        }

        public StiStringFormatGeom GetGenericStringFormat()
        {
            return ContextPainter.GetGenericStringFormat();
        }
        #endregion

        #region Methods.Image
        public void DrawImage(byte[] image, RectangleF rect)
        {
            Geoms.Add(new StiImageGeom(rect, image));
        }
        #endregion

        #region Methods.Text
        public StiTextGeom DrawString(string text, StiFontGeom font, object brush, Rectangle rect, StiStringFormatGeom sf)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, false);
            Geoms.Add(textGeom);
            return textGeom;
        }

        internal void DrawRotatedString(string v1, StiFontGeom fontGeom, StiBrush seriesBrush, RectangleF clientRectangle, object p, int v2, bool v3)
        {
            throw new NotImplementedException();
        }

        public StiTextGeom DrawString(string text, StiFontGeom font, object brush, RectangleF rect, StiStringFormatGeom sf, int elementIndex = -1)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, false, elementIndex);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawString(string text, StiFontGeom font, object brush, RectangleF rect, StiStringFormatGeom sf, bool antialiasing, string toolTip)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, 0, antialiasing, null, null, false, toolTip);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, Rectangle rect, StiStringFormatGeom sf, float angle, bool antialiasing)
        {
            var textGeom = new StiTextGeom(text, font, brush, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), sf, angle, antialiasing, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, RectangleF rect, StiStringFormatGeom sf, float angle, bool antialiasing)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, PointF pos, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing)
        {
            var textGeom = new StiTextGeom(text, font, brush, pos, sf, angle, antialiasing, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, Rectangle rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, Rectangle rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing, int maximalWidth)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, maximalWidth, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, RectangleF rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing, int maximalWidth)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, maximalWidth, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, RectangleF rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing)
        {
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public StiTextGeom DrawRotatedString(string text, StiFontGeom font, object brush, PointF pos, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing, int maximalWidth)
        {
            var textGeom = new StiTextGeom(text, font, brush, pos, sf, angle, antialiasing, maximalWidth, mode, true);
            Geoms.Add(textGeom);
            return textGeom;
        }
        #endregion

        #region Methods.Text.Measure
        public SizeF MeasureString(string text, StiFontGeom font)
        {
            return ContextPainter.MeasureString(text, font);
        }

        public SizeF MeasureString(string text, StiFontGeom font, int width, StiStringFormatGeom sf)
        {
            return ContextPainter.MeasureString(text, font, width, sf);
        }

        public RectangleF MeasureRotatedString(string text, StiFontGeom font, RectangleF rect, StiStringFormatGeom sf, float angle)
        {
            return ContextPainter.MeasureRotatedString(text, font, rect, sf, angle);
        }

        public RectangleF MeasureRotatedString(string text, StiFontGeom font, RectangleF rect, StiStringFormatGeom sf, StiRotationMode mode, float angle)
        {
            return ContextPainter.MeasureRotatedString(text, font, rect, sf, mode, angle);
        }

        public RectangleF MeasureRotatedString(string text, StiFontGeom font, PointF point, StiStringFormatGeom sf, StiRotationMode mode, float angle, int maximalWidth)
        {
            return ContextPainter.MeasureRotatedString(text, font, point, sf, mode, angle, maximalWidth);
        }

        public RectangleF MeasureRotatedString(string text, StiFontGeom font, PointF point, StiStringFormatGeom sf, StiRotationMode mode, float angle)
        {
            return ContextPainter.MeasureRotatedString(text, font, point, sf, mode, angle);
        }
        #endregion

        #region Methods.Shadow
        public void DrawShadow(StiContext sg, RectangleF rect, float radius)
        {
            Geoms.Add(new StiShadowGeom(sg, rect, radius));
        }

        public void DrawCachedShadow(RectangleF rect, StiShadowSides sides, bool isPrinting)
        {
            Geoms.Add(new StiCachedShadowGeom(rect, sides, isPrinting));
        }

        public void DrawCachedShadow(RectangleF rect, StiShadowSides sides, bool isPrinting, RectangleF clipRect)
        {
            Geoms.Add(new StiCachedShadowGeom(rect, sides, isPrinting, clipRect));
        }

        public void DrawCachedShadow(RectangleF rect, StiShadowSides sides, bool isPrinting, RectangleF clipRect, StiCornerRadius cornerRadius)
        {
            Geoms.Add(new StiCachedShadowGeom(rect, sides, isPrinting, clipRect, cornerRadius));
        }

        public StiContext CreateShadowGraphics()
        {
            return ContextPainter.CreateShadowGraphics(this.Options.IsPrinting, this.Options.Zoom);
        }
        #endregion  

        #region Methods.Transform
        public void PushTranslateTransform(float x, float y)
        {
            Geoms.Add(new StiPushTranslateTransformGeom(x, y));
        }

        public void PushRotateTransform(float angle)
        {
            Geoms.Add(new StiPushRotateTransformGeom(angle));
        }

        public void PopTransform()
        {
            Geoms.Add(new StiPopTransformGeom());
        }
        #endregion

        #region Methods.Clip
        public void PushClipPath(List<StiSegmentGeom> listGeoms)
        {
            Geoms.Add(new StiPushClipPathGeom(listGeoms));
        }

        public void PushClip(RectangleF clipRect)
        {
            Geoms.Add(new StiPushClipGeom(clipRect));
        }

        public void PopClip()
        {
            Geoms.Add(new StiPopClipGeom());
        }
        #endregion

        #region Methods.Animation
        public StiTextGeom DrawAnimationText(string text, StiFontGeom font, object brush, Rectangle rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool antialiasing, int maximalWidth, StiAnimation animation)
        {
            Animations.Add(animation);
            var textGeom = new StiTextGeom(text, font, brush, rect, sf, angle, antialiasing, maximalWidth, mode, true, null, animation);
            Geoms.Add(textGeom);
            return textGeom;
        }

        public void DrawAnimationColumn(StiBrush wpfBackColor, object brush, StiPenGeom borderPen, object rect, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredColumnSeriesAnimationGeom(wpfBackColor, brush, borderPen, rect, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationColumn(StiBrush wpfBackColor, object brush, StiPenGeom borderPen, object rect, StiCornerRadius cornerRadius, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredColumnSeriesAnimationGeom(wpfBackColor, brush, borderPen, rect, cornerRadius, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationColumn(object brush, StiPenGeom borderPen, object rect, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredColumnSeriesAnimationGeom(brush, borderPen, rect, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationBar(StiBrush wpfBackColor, object brush, StiPenGeom borderPen, object columnRect, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredBarSeriesAnimationGeom(wpfBackColor, brush, borderPen, columnRect, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationBar(StiBrush wpfBackColor, object brush, StiPenGeom borderPen, object columnRect, StiCornerRadius cornerRadius, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredBarSeriesAnimationGeom(wpfBackColor, brush, borderPen, columnRect, cornerRadius, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationBar(object brush, StiPenGeom borderPen, object columnRect, double? value, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiClusteredBarSeriesAnimationGeom(brush, borderPen, columnRect, value, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationRectangle(StiBrush wpfBackColor, object brush, StiPenGeom pen, RectangleF rect, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null, string toolTip = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiBorderAnimationGeom(wpfBackColor, brush, pen, rect, tag, animation, interaction, toolTip));
        }

        public void DrawAnimationCicledRectangle(StiBrush wpfBackColor, object brush, StiPenGeom pen, RectangleF rect, StiCornerRadius cornerRadius, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null, string toolTip = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiBorderAnimationGeom(wpfBackColor, brush, pen, rect, cornerRadius, tag, animation, interaction, toolTip));
        }

        public void DrawAnimationRectangle(object brush, StiPenGeom pen, RectangleF rect, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null, string toolTip = null)
        {
            Geoms.Add(new StiBorderAnimationGeom(brush, pen, rect, tag, animation, interaction, toolTip));
        }

        public void DrawAnimationPathElement(StiBrush wpfBackColor, object brush, StiPenGeom borderPen, List<StiSegmentGeom> path, object rect, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiPathElementAnimationGeom(wpfBackColor, brush, borderPen, path, rect, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationPathElement(object brush, StiPenGeom borderPen, List<StiSegmentGeom> path, object rect, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiPathElementAnimationGeom(brush, borderPen, path, rect, toolTip, tag, animation, interaction));
        }

        public void DrawAnimationLabel(string text, StiFontGeom font, object textBrush, object labelBrush, StiPenGeom penBorder, Rectangle rect, StiStringFormatGeom sf, StiRotationMode mode,
            float angle, bool drawBorder, StiAnimation animation)
        {
            Geoms.Add(new StiLabelAnimationGeom(text, font, textBrush, labelBrush, penBorder, rect, sf, mode, angle, drawBorder, animation));
        }

        public void DrawAnimationLines(StiPenGeom pen, PointF[] points, StiAnimation animation)
        {
            Animations.Add(animation);
            Geoms.Add(new StiLinesAnimationGeom(pen, points, animation));
        }

        public void DrawAnimationCurve(StiPenGeom pen, PointF[] points, float tension, StiAnimation animation)
        {
            Animations.Add(animation);
            Geoms.Add(new StiCurveAnimationGeom(pen, points, tension, animation));
        }

        public void FillDrawAnimationPath(StiBrush wpfBackColor, object brush, StiPenGeom pen, List<StiSegmentGeom> path, object rect, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiPathAnimationGeom(wpfBackColor, brush, pen, path, rect, tag, animation, interaction));
        }

        public void FillDrawAnimationPath(object brush, StiPenGeom pen, List<StiSegmentGeom> path, object rect, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            Animations.Add(animation);
            Geoms.Add(new StiPathAnimationGeom(brush, pen, path, rect, tag, animation, interaction));
        }

        public void FillDrawAnimationEllipse(StiBrush wpfBackColor, object brush, StiPenGeom pen, float x, float y, float width, float height, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            //Animations.Add(animation);
            Geoms.Add(new StiEllipseAnimationGeom(wpfBackColor, brush, pen, new RectangleF(x, y, width, height), toolTip, tag, animation, interaction));
        }

        public void FillDrawAnimationEllipse(object brush, StiPenGeom pen, float x, float y, float width, float height, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction = null)
        {
            //Animations.Add(animation);
            Geoms.Add(new StiEllipseAnimationGeom(brush, pen, new RectangleF(x, y, width, height), toolTip, tag, animation, interaction));
        }
        #endregion
        
        #region Methods.Primitives
        public void DrawLine(StiPenGeom pen, float x1, float y1, float x2, float y2)
        {
            Geoms.Add(new StiLineGeom(pen, x1, y1, x2, y2));
        }

        public void DrawLines(StiPenGeom pen, PointF[] points)
        {
            #region Propection from NaN values
            for (int index = 0; index < points.Length; index++)
            {
                PointF pos = points[index];

                if (double.IsNaN(pos.X))
                    pos.X = 0;
                if (double.IsNaN(pos.Y))
                    pos.Y = 0;

                points[index] = pos;
            }
            #endregion

            Geoms.Add(new StiLinesGeom(pen, points));
        }

        public void DrawRectangle(StiPenGeom pen, Rectangle rect)
        {
            Geoms.Add(new StiBorderGeom(null, null, pen, rect, null, -1));
        }

        public void DrawRectangle(StiPenGeom pen, RectangleF rect)
        {
            Geoms.Add(new StiBorderGeom(null, null, pen, rect, null, -1));
        }

        public void DrawCicledRectangle(StiPenGeom pen, RectangleF rect, StiCornerRadius cornerRadius)
        {
            Geoms.Add(new StiBorderGeom(null, null, pen, rect, cornerRadius, null, -1));
        }

        public void DrawRectangle(StiPenGeom pen, float x, float y, float width, float height)
        {
            Geoms.Add(new StiBorderGeom(null, null, pen, new RectangleF(x, y, width, height), null, -1));
        }

        public void DrawEllipse(StiPenGeom pen, float x, float y, float width, float height)
        {
            Geoms.Add(new StiEllipseGeom(null, pen, new RectangleF(x, y, width, height), null, -1));
        }

        public void DrawEllipse(StiPenGeom pen, RectangleF rect)
        {
            Geoms.Add(new StiEllipseGeom(null, pen, rect, null, -1));
        }

        public void FillEllipse(object brush, float x, float y, float width, float height, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiEllipseGeom(brush, null, new RectangleF(x, y, width, height), interaction, elementIndex));
        }

        public void FillEllipse(object brush, float x, float y, float width, float height, string toolTip, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiEllipseGeom(brush, null, new RectangleF(x, y, width, height), interaction, elementIndex, toolTip));
        }

        public void FillEllipse(object brush, RectangleF rect, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiEllipseGeom(brush, null, rect, interaction, elementIndex));
        }

        public void DrawPath(StiPenGeom pen, List<StiSegmentGeom> path, object rect)
        {
            Geoms.Add(new StiPathGeom(null, pen, path, rect, null, -1));
        }

        public void FillPath(object brush, List<StiSegmentGeom> path, object rect, StiInteractionDataGeom interaction = null, int elementIndex = -1, string toolTip = null)
        {
            Geoms.Add(new StiPathGeom(brush, null, path, rect, interaction, elementIndex, toolTip));
        }

        public void DrawCurve(StiPenGeom pen, PointF[] points, float tension)
        {
            Geoms.Add(new StiCurveGeom(pen, points, tension));
        }

        public void FillRectangle(object brush, object brushMouseOver, Rectangle rect, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiBorderGeom(brush, brushMouseOver, null, rect, interaction, elementIndex));
        }

        public void FillRectangle(object brush, Rectangle rect, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiBorderGeom(brush, null, null, rect, interaction, elementIndex));
        }

        public void FillRectangle(object brush, float x, float y, float width, float height, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiBorderGeom(brush, null, null, new RectangleF(x, y, width, height), interaction, elementIndex));
        }

        public void FillCicledRectangle(object brush, RectangleF rect, StiCornerRadius cornerRadius, StiInteractionDataGeom interaction = null, int elementIndex = -1)
        {
            Geoms.Add(new StiBorderGeom(brush, null, null, rect, cornerRadius, interaction, elementIndex));
        }
        #endregion

        #region Methods.Smothing
        public void PushSmoothingModeToAntiAlias()
        {
            Geoms.Add(new StiPushSmothingModeToAntiAliasGeom());
        }

        public void PopSmoothingMode()
        {
            Geoms.Add(new StiPopSmothingModeGeom());
        }
        #endregion

        #region Methods.Text Hint
        public void PushTextRenderingHintToAntiAlias()
        {
            Geoms.Add(new StiPushTextRenderingHintToAntiAliasGeom());
        }

        public void PopTextRenderingHint()
        {
            Geoms.Add(new StiPopTextRenderingHintGeom());
        }
        #endregion

        #region Methods.GetPathBounds
        public RectangleF GetPathBounds(List<StiSegmentGeom> geoms)
        {
            return ContextPainter.GetPathBounds(geoms);
        }
        #endregion

        #region Methods.Shadow
        public void DrawShadowRect(RectangleF rect, int shadowWidth, StiCornerRadius cornerRadius, StiAnimation animation)
        {
            Geoms.Add(new StiShadowAnimationGeom(rect, shadowWidth, cornerRadius, animation));
        }

        public void DrawShadowCircle(RectangleF rect, double radiusX, double radiusY, int shadowWidth, StiAnimation animation)
        {
            Geoms.Add(new StiShadowAnimationGeom(rect, radiusX, radiusY, shadowWidth, animation));
        }
        #endregion 

        public StiContext(StiContextPainter contextPainter, bool isGdi, bool isWpf, bool isPrinting, float zoom)
        {
            this.ContextPainter = contextPainter;

            this.Options = new StiContextOptions(isGdi, isWpf, isPrinting, zoom);

            Animations = new List<StiAnimation>();
        }
    }
}