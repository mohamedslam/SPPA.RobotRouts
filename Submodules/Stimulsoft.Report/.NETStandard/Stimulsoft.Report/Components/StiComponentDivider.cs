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
using System.Collections.Generic;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Helpers;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Components
{
    /// <summary>
    /// Class helps devide component by part.
    /// </summary>
    public class StiComponentDivider
    {
        #region Methods
        /// <summary>
        /// Returns text visible in rectangle.
        /// </summary>
        /// <param name="g">Graphics for measure size.</param>
        /// <param name="rect">Rectangle for measure.</param>
        /// <param name="text">Text for check. After check this argument contain visible text.</param>
        /// <param name="font">Font of text.</param>
        /// <returns>Not visible text.</returns>
        internal static string BreakText(Graphics g, RectangleD rect, ref string text,
            Font font, StiTextOptions textOptions, StiTextQuality textQuality, bool allowHtmlTags, StiText textComp)
        {
            if (textComp?.Report != null)
            {
                if (textComp.Report.IsWpf && (!allowHtmlTags))
                {
                    return StiWpfTextRender.BreakText(rect, ref text, textComp);
                }

                else if (StiOptions.Engine.UseNewHtmlEngine && textComp.AllowHtmlTags)
                {
                    return StiHtmlTextRender.BreakText(rect, ref text, textComp);
                }

                else if (StiDpiHelper.IsWindows && (textQuality == StiTextQuality.Wysiwyg || allowHtmlTags))
                {
                    return StiTextRenderer.BreakText(g, ref text, font, rect, StiBrush.ToColor(textComp.TextBrush),
                        StiBrush.ToColor(textComp.Brush), textComp.LineSpacing, textComp.HorAlignment, textOptions.WordWrap,
                        textOptions.RightToLeft, 1, textOptions.Angle, textOptions.Trimming, allowHtmlTags, textOptions, 1);
                }
            }
            string renderedText = null;

            using (var sf = textOptions.GetStringFormat(textQuality == StiTextQuality.Typographic, 1))
            {
                if (!textOptions.WordWrap) 
                    rect.Width = 10000000;

                if (textComp.HorAlignment == StiTextHorAlignment.Width)
                {
                    var rectScale = new RectangleD(rect.X, rect.Y, rect.Width / StiDpiHelper.DeviceCapsScale, rect.Height / StiDpiHelper.DeviceCapsScale);
                    return StiTextDrawing.BreakTextWidth(g, ref text, font, rectScale, sf, textComp.LineSpacing);
                }

                sf.FormatFlags = StringFormatFlags.LineLimit;

                int charactersFitted = 0;
                int linesFilled = 0;

                if (!StiDpiHelper.IsWindows)
                {
                    var tempText = text;
                    renderedText = StiTextDrawing.CutLineLimit(
                        ref tempText,
                        g,
                        font,
                        new RectangleD(0, 0, rect.Width / StiDpiHelper.DeviceCapsScale, rect.Height / (StiDpiHelper.DeviceCapsScale * textComp.LineSpacing)),
                        textOptions,
                        textQuality == StiTextQuality.Typographic,
                        false);

                    charactersFitted = renderedText.Length;
                    text = tempText;
                }
                else
                {
                    try
                    {
                        g.MeasureString(text,
                            font,
                            new SizeF((float)(rect.Width / StiDpiHelper.DeviceCapsScale),
                            (float)(rect.Height / (StiDpiHelper.DeviceCapsScale * textComp.LineSpacing))),
                            sf,
                            out charactersFitted,
                            out linesFilled);
                    }
                    catch
                    {
                        if (text.Length > 16384)
                        {
                            try
                            {
                                g.MeasureString(text.Substring(0, 16384),
                                    font,
                                    new SizeF((float)(rect.Width / StiDpiHelper.DeviceCapsScale),
                                    (float)(rect.Height / StiDpiHelper.DeviceCapsScale)),
                                    sf,
                                    out charactersFitted,
                                    out linesFilled);
                            }
                            catch
                            {
                            }
                        }
                    }

                    renderedText = text.Substring(0, charactersFitted);

                    #region Correction
                    if (linesFilled == 1)
                    {
                        var size = g.MeasureString(renderedText, font,
                            (int)(rect.Width / StiDpiHelper.DeviceCapsScale), sf);

                        if (size.Height * StiDpiHelper.DeviceCapsScale > rect.Height)
                        {
                            charactersFitted = 0;
                            linesFilled = 0;
                            renderedText = null;
                        }
                    }
                    #endregion

                    text = charactersFitted < text.Length 
                        ? text.Substring(charactersFitted) 
                        : string.Empty;
                }
            }
            return renderedText;
        }


        /// <summary>
        /// Returns image visible in rectangle.
        /// </summary>
        /// <param name="imageBytes">Image for checking.</param>
        /// <returns>Not visible part of image.</returns>
        internal static byte[] BreakImage(StiImage imageComp, ref byte[] imageBytes, double devideFactor)
        {
            Image gdiImage;

            if (StiImageHelper.IsMetafile(imageBytes))
            {
                var imageRect = imageComp.GetPaintRectangle(true, false);
                gdiImage = StiMetafileConverter.MetafileToBitmap(imageBytes, (int)imageRect.Width * 2, (int)imageRect.Height * 2);
            }
            else
            {
                gdiImage = StiImageConverter.BytesToImage(imageBytes);
            }

            int firstPartHeight = (int)Math.Round(gdiImage.Height * devideFactor);
            if (firstPartHeight == 0) 
                firstPartHeight = 1;

            int secondPartHeight = gdiImage.Height - firstPartHeight;
            if (secondPartHeight == 0)
            {
                secondPartHeight++;
                
                if (firstPartHeight > 1) 
                    firstPartHeight--;
            }

            var bmp = new Bitmap(gdiImage.Width, firstPartHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                var destRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var sourceRect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                g.DrawImage(gdiImage, destRect, sourceRect, GraphicsUnit.Pixel);
            }

            var bmp2 = new Bitmap(gdiImage.Width, secondPartHeight);
            using (var gg = Graphics.FromImage(bmp2))
            {
                var destRect = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
                var sourceRect = new Rectangle(0, firstPartHeight, bmp2.Width, bmp2.Height);

                gg.DrawImage(gdiImage, destRect, sourceRect, GraphicsUnit.Pixel);
            }

            imageBytes = StiImageConverter.ImageToBytes(bmp2);

            return StiImageConverter.ImageToBytes(bmp);
        }

        [StiEngine(StiEngineVersion.EngineV1)]
        private static bool AllowBreak(StiComponent comp)
        {
            if (comp is StiContainer)
            {
                if (!(comp is StiHeaderBand ||
                    comp is StiGroupHeaderBand ||
                    comp is StiDataBand ||
                    comp is StiFooterBand))
                {
                    return false;
                }
            }

            var breakableComp = comp as IStiBreakable;
            if (breakableComp != null)
                return breakableComp.CanBreak;

            return false;
        }

        /// <summary>
        /// Break container with all components. EngineV1.
        /// </summary>
        /// <param name="renderedContainer">Rendered container with components.</param>
        [StiEngine(StiEngineVersion.EngineV1)]
        internal static StiComponentsCollection BreakContainer(StiContainer renderedContainer)
        {
            var newComps = new StiComponentsCollection();
            var comps = renderedContainer.Components;

            #region Search divide line
            var divideLine = renderedContainer.Height;
            var divLine = 0d;
            var failed = true;

            while (failed)
            {
                failed = false;

                for (int compIndex = 0; compIndex < comps.Count; compIndex++)
                {
                    var comp = comps[compIndex];

                    if (comp.Top < divideLine && comp.Bottom > divideLine)
                    {
                        if (AllowBreak(comp))
                        {
                            var height = comp.Height;
                            comp.Height = divideLine - comp.Top;

                            var comp2 = comp.Clone() as StiComponent;
                            var newComp = comp.Clone() as StiComponent;
                            var breakableComp = comp2 as IStiBreakable;

                            //If brekable component break wrong, do not break component
                            if (breakableComp.Break(newComp, comp.Height / height, ref divLine))
                            {
                                comp.Height = height;
                                continue;
                            }
                            comp.Height = height;
                        }

                        divideLine = Math.Min(divideLine, comp.Top);
                        failed = true;
                    }
                }
            }
            #endregion

            #region Sort components
            int index = 0;
            while (index < comps.Count)
            {
                var comp = comps[index];
                if (comp.Bottom > divideLine)
                {
                    var breakableComp = comp as IStiBreakable;
                    if (comp.Top < divideLine && breakableComp != null && breakableComp.CanBreak)
                    {
                        var newHeight = divideLine - comp.Top;

                        #region Create divided component
                        var newComp = comp.Clone() as StiComponent;

                        var oldHeight = comp.Height;
                        newComp.Height = comp.Height - newHeight;
                        newComp.Top = 0;

                        if (newComp is StiImage)
                        {
                            newComp.CanGrow = false;
                            newComp.CanShrink = false;
                        }
                        else
                        {
                            newComp.CanGrow = true;
                            newComp.CanShrink = false;
                        }
                        newComps.Add(newComp);

                        comp.Height = newHeight;
                        #endregion

                        if (!breakableComp.Break(newComp, newHeight / oldHeight, ref divLine))
                        {
                            comps.Remove(comp);
                            newComp.CanGrow = false;
                            continue;
                        }
                    }
                    else
                    {
                        if (comp.Height > 0)
                        {
                            comp.Top -= divideLine;
                            comps.Remove(comp);
                            newComps.Add(comp);
                            continue;
                        }
                        else
                        {
                            comp.Height = 0;
                        }
                    }
                }
                index++;
            }
            #endregion

            if (newComps.Count > 0)
                renderedContainer.Border.Side &= StiBorderSides.Left | StiBorderSides.Right | StiBorderSides.Top;

            return newComps;
        }

        /// <summary>
        /// Break container with all components. EngineV2.
        /// </summary>
        /// <param name="renderedContainer">Rendered container with components.</param>
        [StiEngine(StiEngineVersion.EngineV2)]
        internal static StiContainer BreakContainer(double maxAllowedHeight, StiContainer renderedContainer)
        {
            if (StiOptions.Engine.AllowBreakContainerOptimization) 
                return BreakContainerV2(maxAllowedHeight, renderedContainer);

            var breakedContainer = renderedContainer.Clone(false, false) as StiContainer;
            var newComps = breakedContainer.Components;
            var comps = renderedContainer.Components;

            #region Search divide line
            var divideLine = maxAllowedHeight;
            var divLine = 0d;
            var divideFlag1 = false;
            var divideFlag2 = false;
            var failed = true;

            while (failed)
            {
                failed = false;

                foreach (StiComponent comp in comps)
                {
                    #region Check if the break line splits the component in two vertically
                    if ((decimal)comp.Top < (decimal)divideLine && (decimal)(comp.Top + comp.Height) > (decimal)divideLine)
                    {
                        #region If the component can break, then break the component
                        var breakableComp = comp as IStiBreakable;
                        if (breakableComp != null && breakableComp.CanBreak)
                        {
                            var oldHeight = comp.Height;
                            var newHeight = divideLine - comp.Top;
                            comp.Height = newHeight;

                            var comp2 = comp.Clone() as StiComponent;

                            var newComp = comp is StiContainer 
                                ? (comp as StiContainer).Clone(true, false) as StiComponent 
                                : comp.Clone() as StiComponent;

                            var breakableComp2 = comp2 as IStiBreakable;

                            //If breakable component break wrong, do not break component
                            if (breakableComp2.Break(newComp, newHeight / oldHeight, ref divLine))
                            {
                                if (divLine > divideLine) 
                                    divideLine = divLine;//fix, NewPage in SubReports

                                comp.Height = oldHeight;
                                if (!divideFlag2 && comp2.Bottom < divideLine)
                                {
                                    //if the bottom of the old component is above the split line, then one of the nested components cannot be split;
                                    //in this case, it is necessary to raise the dividing line;
                                    //if another component cannot be separated, then we return the line to its place,
                                    //since in this case there cannot be a line that fits both components
                                    if (!divideFlag1)
                                    {
                                        divideLine = comp2.Bottom;
                                        divideFlag1 = true;
                                    }
                                    else
                                    {
                                        divideLine = maxAllowedHeight;
                                        divideFlag2 = true;
                                    }
                                    failed = true;
                                    break;
                                }
                                continue;
                            }
                            comp.Height = oldHeight;
                        }
                        #endregion

                        if (comp is StiNewPageContainer)
                        {
                            divideLine = comp.Bottom;
                            divideFlag1 = true;
                            failed = true;
                            break;
                        }

                        divideLine = Math.Min(divideLine, comp.Top);
                        failed = true;
                    }
                    #endregion
                }
            }
            #endregion

            #region Sort components
            int index = 0;
            var oldComps = new List<StiComponent>();
            while (index < comps.Count)
            {
                var comp = comps[index];
                if ((decimal)(comp.Top + comp.Height) > (decimal)divideLine)
                {
                    var breakableComp = comp as IStiBreakable;
                    if ((decimal)comp.Top < (decimal)divideLine && breakableComp != null && breakableComp.CanBreak)
                    {
                        var newHeight = divideLine - comp.Top;

                        #region Create divided component
                        var newComp = comp is StiContainer 
                            ? (comp as StiContainer).Clone(true, false) as StiComponent 
                            : comp.Clone() as StiComponent;

                        var oldHeight = comp.Height;
                        newComp.Height = comp.Height - newHeight;
                        newComp.Top = 0;

                        if (newComp is StiImage)
                        {
                            newComp.CanGrow = false;
                            newComp.CanShrink = false;
                        }
                        else
                        {
                            newComp.CanGrow = true;
                            newComp.CanShrink = false;
                        }
                        newComps.Add(newComp);

                        comp.Height = newHeight;
                        #endregion

                        if (!breakableComp.Break(newComp, newHeight / oldHeight, ref divLine))
                        {
                            index++;

                            newComp.CanGrow = false;
                            continue;
                        }
                    }
                    else
                    {
                        comp.Top -= divideLine;
                        index++;

                        newComps.Add(comp);
                        continue;
                    }
                }
                oldComps.Add(comp);
                index++;
            }

            if (oldComps.Count != comps.Count)
            {
                for (int index2 = 0; index2 < oldComps.Count; index2++)
                {
                    comps[index2] = oldComps[index2];
                }

                while (comps.Count > oldComps.Count)
                {
                    comps.RemoveAt(comps.Count - 1);
                }
            }
            #endregion

            if (StiOptions.Engine.RemoveBottomBorderOfSplitContainer && (newComps.Count > 0))
            {
                renderedContainer.Border = renderedContainer.Border.Clone() as StiBorder;
                renderedContainer.Border.Side &= StiBorderSides.Left | StiBorderSides.Right | StiBorderSides.Top;
            }
            renderedContainer.Height = divideLine;

            return breakedContainer;
        }

        internal static StiContainer BreakContainerV2(double maxAllowedHeight, StiContainer renderedContainer)
        {
            var breakedContainer = renderedContainer.Clone(false, false) as StiContainer;
            var newComps = breakedContainer.Components;
            var comps = renderedContainer.Components;

            double divideLine = maxAllowedHeight;
            divideLine = GetDivideLine(renderedContainer, divideLine);

            #region Sort components
            int index = 0;
            var oldComps = new List<StiComponent>();
            while (index < comps.Count)
            {
                var comp = comps[index];
                if ((decimal)(comp.Top + comp.Height) > (decimal)divideLine)
                {
                    var breakableComp = comp as IStiBreakable;
                    if ((decimal)comp.Top < (decimal)divideLine && breakableComp != null && breakableComp.CanBreak)
                    {
                        var newHeight = divideLine - comp.Top;

                        #region Create divided component
                        var newComp = comp is StiContainer 
                            ? (comp as StiContainer).Clone(true, false) as StiComponent 
                            : comp.Clone() as StiComponent;

                        var oldHeight = comp.Height;
                        newComp.Height = comp.Height - newHeight;
                        newComp.Top = 0;

                        if (newComp is StiImage)
                        {
                            newComp.CanGrow = false;
                            newComp.CanShrink = false;
                        }
                        else
                        {
                            newComp.CanGrow = true;
                            newComp.CanShrink = false;
                        }
                        newComps.Add(newComp);

                        comp.Height = newHeight;
                        #endregion

                        var divLine = divideLine;
                        if (!breakableComp.Break(newComp, newHeight / oldHeight, ref divLine))
                        {
                            index++;

                            newComp.CanGrow = false;
                            continue;
                        }
                    }
                    else
                    {
                        comp.Top -= divideLine;
                        index++;

                        newComps.Add(comp);
                        continue;
                    }
                }

                oldComps.Add(comp);
                index++;
            }

            if (oldComps.Count != comps.Count)
            {
                for (int index2 = 0; index2 < oldComps.Count; index2++)
                {
                    comps[index2] = oldComps[index2];
                }

                while (comps.Count > oldComps.Count)
                {
                    comps.RemoveAt(comps.Count - 1);
                }
            }
            #endregion

            if (StiOptions.Engine.RemoveBottomBorderOfSplitContainer && (newComps.Count > 0))
            {
                renderedContainer.Border = renderedContainer.Border.Clone() as StiBorder;
                renderedContainer.Border.Side &= StiBorderSides.Left | StiBorderSides.Right | StiBorderSides.Top;
            }
            renderedContainer.Height = divideLine;

            return breakedContainer;
        }

        private static double GetDivideLine(StiContainer container, double divideLine)
        {
            var breakCounter = 2;
            var divLine = SearchDivideLine(container, divideLine, divideLine, ref breakCounter);
            if (divLine == divideLine) 
                return divideLine;

            while (divLine > 0)
            {
                var divLine2 = SearchDivideLine(container, divLine, divideLine, ref breakCounter);
                if (divLine2 == divLine) 
                    return divLine;

                divLine = divLine2;
            }
            return 0;
        }

        private static double SearchDivideLine(StiContainer container, double divideLine, double maxAllowedHeight, ref int breakCounter)
        {
            var divLine = 0d;
            foreach (StiComponent comp in container.Components)
            {
                //Check if the break line splits the component in two vertically
                if ((decimal)comp.Top < (decimal)divideLine && (decimal)(comp.Top + comp.Height) > (decimal)divideLine)
                {
                    #region If the component can break, then break the component
                    var breakableComp = comp as IStiBreakable;
                    if ((breakableComp != null) && breakableComp.CanBreak)
                    {
                        if (breakCounter == 0) continue;

                        var oldHeight = comp.Height;
                        var newHeight = divideLine - comp.Top;

                        if (comp is StiContainer)
                        {
                            divLine = GetDivideLine(comp as StiContainer, newHeight);
                            if (divLine < newHeight)
                            {
                                breakCounter--;

                                if (breakCounter == 0) 
                                    return maxAllowedHeight;
                            }

                            if (divLine != newHeight) 
                                return comp.Top + divLine;

                            continue;
                        }
                        else
                        {
                            var comp2 = comp.Clone() as StiComponent;
                            var newComp = comp.Clone() as StiComponent;
                            comp2.Height = newHeight;
                            newComp.Height = newHeight;

                            var breakableComp2 = comp2 as IStiBreakable;

                            //If breakable component break wrong, do not break component
                            if (breakableComp2.Break(newComp, newHeight / oldHeight, ref divLine))
                            {
                                if (comp2.Bottom < divideLine)
                                {
                                    //if the bottom of the old component is above the split line, then one of the nested components cannot be split;
                                    //in this case, it is necessary to raise the dividing line;
                                    return comp2.Bottom;
                                }
                                continue;
                            }
                        }
                    }
                    #endregion

                    if (comp is StiNewPageContainer)
                        return comp.Bottom;

                    divideLine = comp.Top;
                }
            }
            return divideLine;
        }

        /// <summary>
        /// Process previously geerated collections of components
        /// </summary>
        /// <param name="renderedContainer">Container with rendered components</param>
        /// <param name="breakedComponents">Collections with component for update container</param>
        /// <param name="freeSpace">Reference to free space</param>
        internal static void ProcessPreviousBreakedContainer(StiContainer renderedContainer,
            StiComponentsCollection breakedComponents, ref double freeSpace)
        {
            renderedContainer.Border.Side &= StiBorderSides.Left | StiBorderSides.Right | StiBorderSides.Bottom;

            var comps = renderedContainer.Components;
            comps.Clear();

            foreach (StiComponent comp in breakedComponents)
            {
                comps.Add(comp);
            }

            #region Recheck size of container
            var heightCont = renderedContainer.Height;

            renderedContainer.CanGrow = true;
            renderedContainer.CanShrink = true;
            StiContainerHelper.CheckSize(renderedContainer);

            freeSpace += heightCont - renderedContainer.Height;
            #endregion
        }
        #endregion
    }
}
