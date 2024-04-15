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
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using System.Drawing;

namespace Stimulsoft.Report.Engine
{
	public class StiTableOfContentsV2Builder : StiDataBandV2Builder
    {
        #region Methods
        public override StiComponent InternalRender(StiComponent masterComp)
        {            
            var masterTable = masterComp as StiTableOfContents;

            if (masterTable.NewPageBefore)
                masterTable.Report.Engine.NewPage();

            var renderedContainer = masterTable.IsFirstInReport 
                ? RenderPanelWithBookmarks(masterTable) 
                : RenderTextWithNoInformation(masterTable);

            if (masterTable.NewPageAfter)
                (renderedContainer as StiContainer).Components.Add(new StiNewPageContainer());

            return renderedContainer;
        }

        /// <summary>
        /// Renders text with a message that this table of contents can't be rendered because it is not a first table of contents in that report.
        /// Only one table of contents component allowed.
        /// </summary>
        private static StiComponent RenderTextWithNoInformation(StiComponent comp)
        {
            var masterTable = comp as StiTableOfContents;
            var panel = new StiPanel
            {
                CanGrow = false,
                CanShrink = false,
                CanBreak = false,
                GrowToHeight = false,
                ClientRectangle = masterTable.ClientRectangle,
                Name = masterTable.Name,
                Page = masterTable.Page,
                Border = masterTable.Border?.Clone() as StiBorder,
                Brush = masterTable.Brush?.Clone() as StiBrush,
                RightToLeft = masterTable.RightToLeft
            };
            var text = new StiText
            {
                ClientRectangle = new RectangleD(0, 0, panel.Width, panel.Height),
                Name = masterTable.Name,
                Page = masterTable.Page,
                Printable = masterTable.Printable,
                HorAlignment = StiTextHorAlignment.Center,
                VertAlignment = StiVertAlignment.Center,
                RightToLeft = masterTable.RightToLeft
            };
            text.Text.Value = Loc.Get("Errors", "OneTableOfContentsAllowed");

            panel.Components.Add(text);
            return panel;
        }

        /// <summary>
        /// Renders a new panel with TOC.
        /// </summary>
        private static StiComponent RenderPanelWithBookmarks(StiComponent comp)
        {
            var masterComp = comp as StiTableOfContents;            
            var resultComp = new StiPanel
            {
                CanGrow = true,
                CanShrink = true,
                CanBreak = true,
                GrowToHeight = masterComp.GrowToHeight,
                ClientRectangle = masterComp.ClientRectangle,
                TagValue = masterComp,
                Name = masterComp.Name,
                Page = masterComp.Page,
                Border = masterComp.Border?.Clone() as StiBorder,
                Brush = masterComp.Brush?.Clone() as StiBrush
            };

            if (comp.Report.IsSecondPass)
            {
                var posX = 0d;
                var posY = masterComp.Page.Unit.ConvertFromHInches(masterComp.Margins.Top);
                RenderPointers(1, "TOC", masterComp, resultComp, GetPointers(masterComp, comp), ref posX, ref posY);
            }

            if (resultComp.Components.Count > 0)
            {
                var height = resultComp.Components.Cast<StiComponent>().Max(c => c.Bottom);
                resultComp.Height = Math.Max(resultComp.Height, height) + masterComp.Page.Unit.ConvertFromHInches(masterComp.Margins.Bottom);
            }

            return resultComp;
        }        

        private static void RenderPointers(int level, string baseName, StiTableOfContents masterComp, StiPanel resultComp, 
            StiBookmarksCollection bookmarks, ref double posX, ref double posY)
        {
            if (bookmarks == null) return;

            var style = GetStyle(level, masterComp);
            var index = 1;
            foreach (StiBookmark bookmark in bookmarks)
            {                
                StiText textIndex = null;
                var hyperlink = GetHyperlink(bookmark);

                //Don't show index for the report
                if (!(bookmark.ParentComponent is StiReport))
                {
                    textIndex = new StiText
                    {
                        Brush = null,
                        WordWrap = false,
                        Top = posY,
                        Width = FullSize(resultComp),
                        Height = FullSize(resultComp),
                        Page = resultComp.Page,
                        Printable = masterComp.Printable,
                        Name = $"{resultComp.Name}_Index_{baseName}_{index}",
                        VertAlignment = StiVertAlignment.Center,
                        TextQuality = StiTextQuality.Wysiwyg,
                        TagValue = bookmark,
                        HyperlinkValue = hyperlink,
                        RightToLeft = masterComp.RightToLeft,
                        Margins = new StiMargins(0, masterComp.Margins.Right, 0, 0)
                    };
                    style?.SetStyleToComponent(textIndex);

                    textIndex.HorAlignment = StiTextHorAlignment.Right;
                    textIndex.Left = masterComp.RightToLeft ? 0 : resultComp.Width - FullSize(resultComp);
                    textIndex.NewGuid();
                    SetOffSides(masterComp.RightToLeft, true, false, textIndex);

                    resultComp.Components.Add(textIndex);
                }

                var textName = new StiText
                {
                    Brush = null,
                    WordWrap = false,
                    Top = posY,
                    Height = FullSize(resultComp),
                    Name = $"{resultComp.Name}_Name_{baseName}_{index}",
                    VertAlignment = StiVertAlignment.Center,
                    Page = resultComp.Page,
                    Printable = masterComp.Printable,
                    TextQuality = StiTextQuality.Wysiwyg,
                    Guid = textIndex?.Guid,
                    RightToLeft = masterComp.RightToLeft,
                    Margins = new StiMargins(masterComp.Margins.Left, 0, 0, 0)
                };

                if (textIndex != null)
                    textName.HyperlinkValue = hyperlink;

                textName.Text.Value = bookmark.Text;

                if (textIndex != null)
                {
                    textName.Left = masterComp.RightToLeft ? textIndex.Width : posX;
                    textName.Width = masterComp.RightToLeft ? resultComp.Width - textIndex.Right - posX : textIndex.Left - textName.Left;

                    textName.Text.Value += DotsString;
                }
                else
                {
                    textName.Width = resultComp.Width;
                }

                textName.TextOptions.Trimming = StringTrimming.None;
                style?.SetStyleToComponent(textName);

                float lineSpacing = (style is StiStyle) ? (style as StiStyle).LineSpacing : 1;

                textName.HorAlignment = StiTextHorAlignment.Left;
                textName.Height = masterComp.Report.Unit.ConvertFromHInches(textName.Font.GetHeight() * lineSpacing);

                if (textIndex != null)
                    SetOffSides(masterComp.RightToLeft, false, true, textName);

                resultComp.Components.Insert(0, textName);

                if (textIndex != null)
                    textIndex.Height = textName.Height;

                posY += textName.Height;

                if (bookmark.Bookmarks.Count > 0)
                {
                    var resPosX = posX;
                    posX += Indent(masterComp);

                    RenderPointers(level + 1, $"{baseName}_{index}", masterComp, resultComp, bookmark.Bookmarks, ref posX, ref posY);
                    posX = resPosX;
                }

                index++;
            }
        }        

        internal static void PostProcessTableOfContents(StiReport report)
        {
            if (!report.GetComponents().Cast<StiComponent>().Any(c => c is StiTableOfContents && c.Enabled)) return;

            //var comps = report.RenderedPages.ToList().SelectMany(p => p.GetComponents().ToList()).ToList();
            var comps = report.GetRenderedComponents().ToList();

            var masterComp = report.Pages.ToList().SelectMany(p => p.GetComponents().ToList())
                .FirstOrDefault(c => c is StiTableOfContents) as StiTableOfContents;

            var resultComp = comps.FirstOrDefault(c => c.Name == masterComp?.Name);
            if (resultComp != null)
                PostProcessBookmarks(comps, masterComp, resultComp);

            foreach (StiComponent comp in comps.Where(c => c.TagValue is StiBookmark))
            {
                var bookmark = comp.TagValue as StiBookmark;
                var targetComp = string.IsNullOrWhiteSpace(bookmark.ComponentGuid)
                    ? null
                    : comps.FirstOrDefault(c => c.Guid == bookmark.ComponentGuid);

                var textIndex = comp as StiText;

                if (targetComp != null)
                    textIndex.Text.Value = report.Engine.PageNumbers.GetPageNumber(targetComp.Page).ToString();

                textIndex.AutoWidth = true;
                var prevRight = textIndex.Right;
                textIndex.Width = textIndex.GetActualSize().Width;

                if (!masterComp.RightToLeft)
                    textIndex.Left = prevRight - textIndex.Width;

                textIndex.AutoWidth = false;

                var textName = comps.FirstOrDefault(c => c.Guid == textIndex.Guid && c != textIndex);
                if (textName != null)
                {
                    if (masterComp.RightToLeft)
                    {
                        textName.Width = textName.Right - textIndex.Right;
                        textName.Left = textIndex.Right;
                    }
                    else
                    {
                        textName.Width = textIndex.Left - textName.Left;
                    }
                }
            }
        }

        public static void PostProcessBookmarks(List<StiComponent> comps, StiTableOfContents masterComp, StiComponent resultComp)
        {
            PostProcessBookmarks(comps, "TOC", resultComp, GetPointersForPostProcessing(masterComp, resultComp));
        }

        private static StiBookmarksCollection GetPointers(StiTableOfContents masterComp, StiComponent comp)
        {
            var pointer = comp.Report.Engine.FirstPassPointer;

            if (string.IsNullOrWhiteSpace(masterComp.ReportPointer))
            {
                return pointer?.Bookmarks;
            }
            else
            {
                pointer.Text = StiReportParser.Parse(masterComp.ReportPointer, masterComp);
                pointer.ParentComponent = masterComp.Report;

                return new StiBookmarksCollection { pointer };
            }
        }

        private static StiBookmarksCollection GetPointersForPostProcessing(StiTableOfContents masterComp, StiComponent resultComp)
        {
            if (string.IsNullOrWhiteSpace(masterComp.ReportPointer))
                return resultComp.Report.Pointer.Bookmarks;
            
            else
                return new StiBookmarksCollection { resultComp.Report.Pointer };
        }

        public static void PostProcessBookmarks(List<StiComponent> comps, string baseName, StiComponent panel, StiBookmarksCollection bookmarks)
        {
            var index = 1;
            foreach (StiBookmark bookmark in bookmarks)
            {
                var strIndex = $"{panel.Name}_Index_{baseName}_{index}";
                var strName = $"{panel.Name}_Name_{baseName}_{index}";

                var textIndex = comps.FirstOrDefault(c => c.Name == strIndex);
                if (textIndex != null)
                {
                    textIndex.TagValue = bookmark;
                    textIndex.HyperlinkValue = GetHyperlink(bookmark);
                }

                var textName = comps.FirstOrDefault(c => c.Name == strName);
                if (textName != null)
                    textName.HyperlinkValue = GetHyperlink(bookmark);

                if (bookmark.Bookmarks.Count > 0)
                    PostProcessBookmarks(comps, $"{baseName}_{index}", panel, bookmark.Bookmarks);

                index++;
            }
        }

        private static string GetHyperlink(StiBookmark bookmark)
        {
            return $"###{bookmark.Text}#GUID#{bookmark.ComponentGuid}";
        }

        private static void SetOffSides(bool isRightToLeft, bool setLeftOff, bool setRightOff, StiText text)
        {
            //Left is right and right is left for RightToLeft
            if (isRightToLeft)
            {
                if (setLeftOff)
                    SetOffSide(text, StiBorderSides.Right);

                if (setRightOff)
                    SetOffSide(text, StiBorderSides.Left);
            }
            else
            {
                if (setLeftOff)
                    SetOffSide(text, StiBorderSides.Left);

                if (setRightOff)
                    SetOffSide(text, StiBorderSides.Right);
            }
        }

        private static void SetOffSide(StiText text, StiBorderSides side)
        {
            if ((text.Border.Side & side) > 0)
                text.Border.Side -= side;
        }

        private static StiBaseStyle GetStyle(int level, StiTableOfContents masterComp)
        {
            var styles = masterComp.GetStylesList();

            if (level >= 1 && level <= styles.Count)
                return styles[level - 1];

            if (styles.Count > 0)
                return styles.Last();

            return null;
        }

        private static double FullSize(StiComponent comp)
        {
            return comp.Report.Unit.ConvertFromHInches(20d);
        }

        private static double Indent(StiTableOfContents table)
        {
            return table.Report.Unit.ConvertFromHInches((double)table.Indent);
        }
        #endregion

        #region Properties
        private static string dotsString;
        private static string DotsString
        {
            get
            {
                if (dotsString == null)
                    dotsString = new string('.', 300);

                return dotsString;
            }
        }
        #endregion
    }
}
