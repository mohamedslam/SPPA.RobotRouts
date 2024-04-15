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
using System.Collections;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Engine
{
    public class StiComponentV2Builder : StiV2Builder
    {
        #region Methods.Render
        /// <summary>
        /// Sets system variables which are specific for the specified component.
        /// </summary>
        public override void SetReportVariables(StiComponent masterComp)
        {
        }

        /// <summary>
        /// Prepares a component for rendering.
        /// </summary>
        public override void Prepare(StiComponent masterComp)
        {
            StiComponentHelper.FillComponentPlacement(masterComp);

            if (!StiOptions.Engine.AllowResetValuesAtComponent) return;

            masterComp.TagValue = null;
            masterComp.ToolTipValue = null;
            masterComp.PointerValue = null;
            masterComp.BookmarkValue = null;
            masterComp.HyperlinkValue = null;
        }

        /// <summary>
        /// Clears a component after rendering.
        /// </summary>
        public override void UnPrepare(StiComponent masterComp)
        {
        }

        public override StiComponent InternalRender(StiComponent masterComp)
        {
            return masterComp.Clone(false) as StiComponent;
        }

        public override StiComponent Render(StiComponent masterComp)
        {
            StiComponent renderedComponent = null;

            #region Store Conditions
            var textBrush = masterComp as IStiTextBrush;
            var brush = masterComp as IStiBrush;
            var font = masterComp as IStiFont;
            var border = masterComp as IStiBorder;
            var textHorAlign = masterComp as IStiTextHorAlignment;

            StiBrush savedTextBrush = null;
            StiBrush savedBrush = null;
            Font savedFont = null;
            
            var savedBorderSides = StiBorderSides.None;
            var savedTextHorAlign = StiTextHorAlignment.Left;

            if (textBrush != null)
                savedTextBrush = textBrush.TextBrush;

            if (brush != null)
                savedBrush = brush.Brush;

            if (font != null)
                savedFont = font.Font;

            if (border != null && border.Border != null)
                savedBorderSides = border.Border.Side;

            var savedEnabled = masterComp.Enabled;

            if (textHorAlign != null)
                savedTextHorAlign = textHorAlign.HorAlignment;
            #endregion

            #region Check UseParentStyles part1
            var report = masterComp.Report;
            var needUseParentStyles = false;
            StiBaseStyle tempStyle = null;

            if (masterComp.UseParentStyles && masterComp.Parent != null)
            {
                if (report?.Engine?.HashParentStyles != null && report.Engine.HashParentStyles.Count > 0)
                    tempStyle = report.Engine.HashParentStyles[masterComp.Parent] as StiBaseStyle;

                if (tempStyle == null)
                    tempStyle = StiBaseStyle.GetStyle(masterComp.Parent);

                if (tempStyle != null)
                {
                    tempStyle.SetStyleToComponent(masterComp);
                    needUseParentStyles = true;

                    var compStyle = masterComp.GetComponentStyle();
                    compStyle?.SetStyleToComponent(masterComp);
                }
            }
            #endregion

            masterComp.InvokeBeforePrint(masterComp, EventArgs.Empty);

            #region Check UseParentStyles part2
            if (report?.Engine != null && report.Engine.HashUseParentStyles.ContainsKey(masterComp))
            {
                var tempStyle2 = StiBaseStyle.GetStyle(masterComp, tempStyle);

                if (report.Engine.HashParentStyles == null)
                    report.Engine.HashParentStyles = new Hashtable();

                report.Engine.HashParentStyles[masterComp] = tempStyle2;
                needUseParentStyles = true;
            }
            #endregion

            if (masterComp.IsEnabled)
            {
                //Do not generate Bookmarks for the EmptyBand because they are generated in the method if EmptyBands rendering
                if (!(masterComp is StiEmptyBand))
                {
                    bool isNewGuidCreated = masterComp.DoBookmark();
                    masterComp.DoPointer(!isNewGuidCreated);
                }

                renderedComponent = masterComp.InternalRender();

                if (renderedComponent != null)
                {
                    if (renderedComponent.Page == null)
                    {
                        renderedComponent.Page = masterComp.Page;
                        renderedComponent.InvokeEvents();
                        renderedComponent.Page = null;
                    }
                    else
                    {
                        renderedComponent.InvokeEvents();
                    }
                }
            }
            else if (masterComp is StiCrossField)
            {
                (masterComp as StiCrossField).DisabledByCondition = true;
            }

            masterComp.InvokeAfterPrint(masterComp, EventArgs.Empty);

            #region Check UseParentStyles part3
            if (needUseParentStyles && report.Engine.HashParentStyles != null)
                report.Engine.HashParentStyles.Remove(masterComp);
            #endregion

            #region Restore Conditions
            if (textBrush != null)
                textBrush.TextBrush = savedTextBrush;

            if (brush != null)
                brush.Brush = savedBrush;

            if (font != null)
                font.Font = savedFont;

            if (border != null && border.Border != null)
                border.Border.Side = savedBorderSides;

            if (!(masterComp is StiSubReport)) //Do not restore enabled property for SubReports, because SubReports renders in other method
                masterComp.Enabled = savedEnabled;

            if (textHorAlign != null)
                textHorAlign.HorAlignment = savedTextHorAlign;
            #endregion

            #region Optimize border objects
            var masterBorderObject = masterComp as IStiBorder;
            if (masterBorderObject != null)
            {
                var renderedBorderObject = renderedComponent as IStiBorder;
                if (renderedBorderObject != null)
                {
                    if (!ReferenceEquals(masterBorderObject.Border, renderedBorderObject.Border) &&
                        masterBorderObject.Border.Equals(renderedBorderObject.Border))
                    {
                        renderedBorderObject.Border = masterBorderObject.Border;
                    }
                }
            }
            #endregion

            #region Optimize brush objects
            var masterBrushObject = masterComp as IStiBrush;
            if (masterBrushObject != null)
            {
                var renderedBrushObject = renderedComponent as IStiBrush;
                if (renderedBrushObject != null)
                {
                    if (!ReferenceEquals(masterBrushObject.Brush, renderedBrushObject.Brush) &&
                        masterBrushObject.Brush.Equals(renderedBrushObject.Brush))
                    {
                        renderedBrushObject.Brush = masterBrushObject.Brush;
                    }
                }
            }
            #endregion

            #region Optimize text brush objects
            var masterTextBrushObject = masterComp as IStiTextBrush;
            if (masterTextBrushObject != null)
            {
                var renderedTextBrushObject = renderedComponent as IStiTextBrush;
                if (renderedTextBrushObject != null)
                {
                    if (!ReferenceEquals(masterTextBrushObject.TextBrush, renderedTextBrushObject.TextBrush) &&
                        masterTextBrushObject.TextBrush.Equals(renderedTextBrushObject.TextBrush))
                    {
                        renderedTextBrushObject.TextBrush = masterTextBrushObject.TextBrush;
                    }
                }
            }
            #endregion

            masterComp.IsRendered = true;

            return renderedComponent;
        }
        #endregion
    }
}