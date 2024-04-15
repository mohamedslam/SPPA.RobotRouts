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
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Units;
using Stimulsoft.Base.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

#if NETSTANDARD
using Stimulsoft.System.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Brushes = Stimulsoft.Drawing.Brushes;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Pen = Stimulsoft.Drawing.Pen;
using Pens = Stimulsoft.Drawing.Pens;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Painters
{
    public class StiPageGdiPainter : 
        StiContainerGdiPainter, 
        IStiPagePainter
    {
        #region Methods.Watermark
        public virtual void PaintText(StiWatermark watermark, Graphics g, RectangleD rect, double zoom, bool isPrinting)
        {
            if (!watermark.Enabled || string.IsNullOrEmpty(watermark.Text)) return;

            var fontSize = watermark.Font.Size * (float)zoom;

            if (isPrinting && watermark.Font.Unit != GraphicsUnit.Pixel && watermark.Font.Unit != GraphicsUnit.World)
                fontSize *= 96f / 100f;

            using (var brush = StiBrush.GetBrush(watermark.TextBrush, rect.ToRectangleF()))
            using (var font = new Font(watermark.Font.FontFamily, fontSize,
                watermark.Font.Style, watermark.Font.Unit,
                watermark.Font.GdiCharSet, watermark.Font.GdiVerticalFont))
            {
                var defaultHint = g.TextRenderingHint;
                if (!StiOptions.Engine.DisableAntialiasingInPainters)
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                var textOptions = new StiTextOptions
                {
                    Angle = watermark.Angle,
                    RightToLeft = watermark.RightToLeft
                };

                StiTextDrawing.DrawString(g, watermark.Text, font, brush, rect,
                    textOptions, StiTextHorAlignment.Center, StiVertAlignment.Center, true, watermark.Angle);

                g.TextRenderingHint = defaultHint;
            }
        }

        public virtual void PaintImage(StiReport report, StiWatermark watermark, Graphics g, RectangleD rect, double zoom)
        {
            if (!watermark.Enabled) return;

            var imageBuffer = watermark.GetImage(report);
            if (imageBuffer == null) return;

            var state = g.Save();
            g.SetClip(rect.ToRectangleF());

            if (watermark.ImageStretch)
            {
                var destRect = rect.ToRectangleF();

                using (var gdiImage = StiImageConverter.BytesToImage(imageBuffer, (int) destRect.Width, (int) destRect.Height))
                {
                    #region AspectRatio
                    if (watermark.AspectRatio)
                    {
                        var xRatio = destRect.Width / gdiImage.Width;
                        var yRatio = destRect.Height / gdiImage.Height;

                        if (xRatio > yRatio)
                        {
                            destRect.X = (destRect.Width - gdiImage.Width * yRatio) / 2;
                            destRect.Width = gdiImage.Width * yRatio;
                        }
                        else
                        {
                            destRect.Y = (destRect.Height - gdiImage.Height * xRatio) / 2;
                            destRect.Height = gdiImage.Height * xRatio;
                        }

                        var minRatio = Math.Min(xRatio, yRatio);
                        var imageSize = new SizeD(gdiImage.Size.Width * minRatio, gdiImage.Height * minRatio);
                        destRect = StiRectangleUtils.AlignSizeInRect(rect, imageSize, watermark.ImageAlignment).ToRectangleF();
                    }
                    #endregion

                    g.DrawImage(gdiImage, destRect);
                }
            }
            else
            {
                using (var gdiImage = StiImageConverter.BytesToImage(imageBuffer))
                {
                    zoom *= watermark.ImageMultipleFactor;
                    var imageSize = new SizeD(gdiImage.Size.Width * zoom, gdiImage.Height * zoom);
                    if (watermark.ImageTiling)
                    {
                        PaintTileImage(gdiImage, g, rect, imageSize);
                    }
                    else
                    {
                        var imageRect = StiRectangleUtils.AlignSizeInRect(rect, imageSize, watermark.ImageAlignment);
                        g.DrawImage(gdiImage, imageRect.ToRectangleF());
                    }
                }
            }

            g.Restore(state);
        }

        public virtual void PaintTileImage(Image watermarkImage, Graphics g, RectangleD rect, SizeD imageSize)
        {
            var y = (float)rect.Y;

            while (y < rect.Bottom)
            {
                var x = (float)rect.X;
                while (x < rect.Right)
                {
                    g.DrawImage(watermarkImage, new RectangleF(x, y, (float)imageSize.Width, (float)imageSize.Height));
                    x += (float)imageSize.Width;
                }
                y += (float)imageSize.Height;
            }
        }

        public Bitmap GetWatermarkImage(StiPage page, double zoom, bool useMargins)
        {
            var imgWidth = (int)Math.Round(page.Unit.ConvertToHInches(page.Width + (useMargins ? page.Margins.Left + page.Margins.Right : 0)) * zoom);
            var imgHeight = (int)Math.Round(page.Unit.ConvertToHInches(page.Height + (useMargins ? page.Margins.Top + page.Margins.Bottom : 0)) * zoom);

            var image = new Bitmap(imgWidth, imgHeight);
            using (var g = Graphics.FromImage(image))
            using (var brush = StiBrush.GetBrush(page.Brush, new RectangleD(0, 0, 0, 0)))
            {
                var selectedBrush = brush;

                if (StiBrush.ToColor(page.Brush) == Color.Transparent)
                    selectedBrush = Brushes.White;

                g.FillRectangle(selectedBrush, 0, 0, (float)imgWidth, (float)imgHeight);

                PaintImage(page.Report, page.Watermark, g, new RectangleD(0, 0, imgWidth, imgHeight), zoom);
                PaintText(page.Watermark, g, new RectangleD(0, 0, imgWidth, imgHeight), zoom, false);
            }

            return image;
        }

        private void PaintWatermark(StiPage page, Graphics g, bool isBehind)
        {
            var scale = page.Zoom * StiScale.Factor;
            var pgWidth = page.Unit.ConvertToHInches(page.DisplayRectangle.Width);
            var pgHeight = page.Unit.ConvertToHInches(page.DisplayRectangle.Height);

            var rect = new RectangleD(0, 0, pgWidth * scale, pgHeight * scale);

            var advWatermark = page.TagValue as StiAdvancedWatermark;
            if (isBehind && advWatermark != null && advWatermark.WeaveEnabled)
            {
                StiAdvancedWatermarkGdiPainter.PaintWatermark(g, advWatermark, null, rect, scale);
                return;
            }

            if (page.Watermark.ShowImageBehind == isBehind)
                PaintImage(page.Report, page.Watermark, g, rect, scale);

            if (page.Watermark.ShowBehind == isBehind)
                PaintText(page.Watermark, g, rect, scale, page.IsPrinting);
        }
        #endregion

        #region Methods.Painter
        public override void Paint(StiComponent comp, StiPaintEventArgs e)
        {
            var isPaintWinFormsViewer = e.IsPaintWinFormsViewer;

            if (!(e.Context is Graphics))
                throw new Exception("StiGdiPainter can work only with System.Drawing.Graphics context!");

            var page = (StiPage) comp;
            if (page.Report == null) return;
            StiScale.IsPrinting = page.IsPrinting;

            if (page.IsDesigning)
                page.SelectedComponents.Clear();

            var g = e.Graphics;

            if (StiOptions.Engine.DisableAntialiasingInPainters)
            {
                g.SmoothingMode = SmoothingMode.None;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
            }

            page.InvokePainting(page, e);

            var unit = page.Unit;
            var scale = page.Zoom * StiScale.Factor;

            var mgLeft = unit.ConvertToHInches(page.Margins.Left);
            var mgTop = unit.ConvertToHInches(page.Margins.Top);
            var mgRight = unit.ConvertToHInches(page.Margins.Right);

            var pgWidth = unit.ConvertToHInches(page.DisplayRectangle.Width);
            var pgHeight = unit.ConvertToHInches(page.DisplayRectangle.Height);

            var widthInHi = unit.ConvertToHInches(page.Width);
            var heightInHi = unit.ConvertToHInches(page.Height);

            var printerMargins = RectangleF.Empty;

            if (page.IsPrinting)
            {
                printerMargins = page.GetPrinterMargins(g);

                #region Page alignment
                var pageWidthInHi = unit.ConvertToHInches(page.PageWidth);
                var pageHeightInHi = unit.ConvertToHInches(page.PageHeight);

                if (printerMargins.Width > pageWidthInHi * 1.005)
                {
                    if (StiOptions.Print.PageHorAlignment == StiHorAlignment.Center)
                        printerMargins.X -= (float) (printerMargins.Width - pageWidthInHi) / 2f;

                    if (StiOptions.Print.PageHorAlignment == StiHorAlignment.Right)
                        printerMargins.X -= (float) (printerMargins.Width - pageWidthInHi);
                }

                if (printerMargins.Height > pageHeightInHi * 1.005)
                {
                    if (StiOptions.Print.PageVertAlignment == StiVertAlignment.Center)
                        printerMargins.Y -= (float) (printerMargins.Height - pageHeightInHi) / 2f;

                    if (StiOptions.Print.PageVertAlignment == StiVertAlignment.Bottom)
                        printerMargins.Y -= (float) (printerMargins.Height - pageHeightInHi);
                }
                #endregion
            }

            if (!e.Cancel)
            {
                if (!e.ClipRectangle.IsEmpty)
                {
                    g.ResetClip();
                    g.SetClip(e.ClipRectangle.ToRectangleF(), CombineMode.Replace);
                }

                #region !IsPrinting
                if (!page.IsPrinting)
                {
                    PaintBackground(e, g, page, pgWidth, pgHeight);
                    PaintWatermark(page, g, true); //Watermark Behind

                    g.TranslateTransform((int) (mgLeft * scale), (int) (mgTop * scale));

                    PaintGrid(g, page, widthInHi, heightInHi);

                    #region IsDesigning, Border of content, Columns
                    if (page.IsDesigning)
                    {
                        PaintContentBorder(g, page, widthInHi, heightInHi);
                        PaintColumns(g, page, heightInHi);
                    }
                    #endregion
                }
                #endregion

                #region IsPrinting
                else
                {
                    g.ResetTransform();
                    g.TranslateTransform(-printerMargins.Left, -printerMargins.Top);
                    PaintBackground(e, g, page, pgWidth, pgHeight);
                    PaintWatermark(page, g, true);

                    if (StiDpiHelper.IsLinux)  //for fix, because on our tests PageScale do not work
                        g.PageUnit = GraphicsUnit.Point;
                    else
                        g.PageUnit = GraphicsUnit.Inch;
                    g.ResetTransform();

                    if (page.SegmentPerHeight > 1 || page.SegmentPerWidth > 1)
                        PaintSegment(g, page, printerMargins);

                    else
                    {
                        if (page.StretchToPrintArea)
                        {
                            g.PageScale = .01f * (float) (printerMargins.Width / (pgWidth - mgLeft - mgRight));
                        }
                        else
                        {
                            g.PageScale = .01f;
                            g.TranslateTransform(
                                -printerMargins.Left + (float) mgLeft,
                                -printerMargins.Top + (float) mgTop);
                        }
                    }
                }
                #endregion
            }

            var ppe = PaintCopyrightBefore(page, g, e);

            e.Cancel = false;
            page.InvokePainted(page, e);

            #region Set ClipRectangle comparatively begin pages
            if (!e.ClipRectangle.IsEmpty)
            {
                e = new StiPaintEventArgs(g,
                    new RectangleD(e.ClipRectangle.X - mgLeft * scale, e.ClipRectangle.Y - mgTop * scale,
                        e.ClipRectangle.Width, e.ClipRectangle.Height));
            }
            #endregion

            var state = g.Save();

            var compClipRect = new RectangleF(
                (float) (-mgLeft * scale), (float) (-mgTop * scale),
                (float) (pgWidth * scale), (float) (pgHeight * scale));

            g.SetClip(compClipRect, CombineMode.Intersect);

            PaintTableLines(page, g);
            PaintComponents(page, e);
            PaintSelectedComponents(page, g, e);

            g.Restore(state);

            PaintBorder(page, g, new RectangleD(0, 0, widthInHi * scale, heightInHi * scale), scale, true, true);
            PaintTopWatermark(page, g, printerMargins);
            PaintPageBreaks(page, g, widthInHi, heightInHi);
            PaintLargeHeightFactorLine(page, g);
            PaintHighlight(page, e);
            PaintOrderAndQuickInfo(page, g, string.Empty);
            PaintInheritedImage(page, g);

            #region Trial
            #if CLOUD
            var isTrial = false; //StiCloudPlan.IsTrialPlan(page.Report != null ? page.Report.ReportGuid : null); //- server used only, for small thumbnails 
#elif SERVER
            var isTrial = StiVersionX.IsSvr;
#else
            var key = StiLicenseKeyValidator.GetLicenseKey();
            var isValidInDesigner = page.IsDashboard 
                ? StiLicenseKeyValidator.IsValidInDashboardsDesignerOrOnPlatform(StiProductIdent.DbsWin, key)
                : StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key);

            var isTrial = !(isPaintWinFormsViewer || StiDesignerAppStatus.IsRunning
                ? isValidInDesigner
                : StiLicenseKeyValidator.IsValidOnNetFramework(key));

            if (!typeof(StiLicense).AssemblyQualifiedName.Contains(StiPublicKeyToken.Key))
                isTrial = true;

            #region IsValidLicenseKey
            if (!isTrial)
            {
                try
                {
                    using (var rsa = new RSACryptoServiceProvider(512))
                    using (var sha = new SHA1CryptoServiceProvider())
                    {
                        rsa.FromXmlString("<RSAKeyValue><Modulus>iyWINuM1TmfC9bdSA3uVpBG6cAoOakVOt+juHTCw/gxz/wQ9YZ+Dd9vzlMTFde6HAWD9DC1IvshHeyJSp8p4H3qXUKSC8n4oIn4KbrcxyLTy17l8Qpi0E3M+CI9zQEPXA6Y1Tg+8GVtJNVziSmitzZddpMFVr+6q8CRi5sQTiTs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                        isTrial = !rsa.VerifyData(key.GetCheckBytes(), sha, key.GetSignatureBytes());
                    }
                }
                catch (Exception)
                {
                    isTrial = true;
                }
            }
            #endregion
            #endif
            if (isTrial)
            {
                using (var sf = new StringFormat())
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    var color = Color.FromArgb(30, 100, 100, 100);
                    if (page.IsPrinting) color = Color.LightGray;
                    using (var brush = new SolidBrush(color))
                    {
                        var rect = new RectangleD(-mgLeft * scale, -mgTop * scale, pgWidth * scale, pgHeight * scale);
                        using (var font = new Font("Arial", (float) (150 * scale / StiScale.Factor), FontStyle.Bold))
                        {
                            StiTextDrawing.DrawString(g, "Trial", font, brush, rect, sf, 45);
                        }
                    }
                }
            }
            #endregion

            PaintSelection(page, g);
            PaintDimensionLines(page, g);
            PaintCopyrightAfter(page, g, ppe);
            StiScale.IsPrinting = false;
        }

        public virtual void PaintSelectedComponents(StiPage page, Graphics g, StiPaintEventArgs e)
        {
            if (!page.IsDesigning) return;

            var index = 0;
            while (index < page.SelectedComponents.Count)
            {
                var component = page.SelectedComponents[index] as StiComponent;
                if (component is StiBand)
                    component.Paint(e);

                index++;
            }

            index = 0;
            while (index < page.SelectedComponents.Count)
            {
                var component = page.SelectedComponents[index] as StiComponent;
                if (!(component is StiBand))
                    component.Paint(e);

                index++;
            }

            var comps = page.GetSelectedComponents();
            if (comps.Count > 1)
            {
                index = 0;
                while (index < comps.Count)
                {
                    var component = comps[index];
                    if (component.IsSelected)
                    {
                        var rect = component.GetPaintRectangle();

                        var centerX = rect.X + rect.Width / 2;
                        var centerY = rect.Y + rect.Height / 2;
                        var color = GetSelectionCornerColor(page);

                        StiDrawing.FillRectangle(g, color, rect.X - StiScale.I1, rect.Y - StiScale.I1, StiScale.I3, StiScale.I3);// LeftTop
                        StiDrawing.FillRectangle(g, color, rect.X - StiScale.I1, centerY - StiScale.I1, StiScale.I3, StiScale.I3);// LeftCenter
                        StiDrawing.FillRectangle(g, color, centerX - StiScale.I1, rect.Y - StiScale.I1, StiScale.I3, StiScale.I3);// TopCenter
                        StiDrawing.FillRectangle(g, color, rect.Right - StiScale.I1, rect.Y - StiScale.I1, StiScale.I3, StiScale.I3);// TopRight
                        StiDrawing.FillRectangle(g, color, rect.Right - StiScale.I1, centerY - StiScale.I1, StiScale.I3, StiScale.I3);// RightCenter
                        StiDrawing.FillRectangle(g, color, rect.Right - StiScale.I1, rect.Bottom - StiScale.I1, StiScale.I3, StiScale.I3);// BottomRight
                        StiDrawing.FillRectangle(g, color, centerX - StiScale.I1, rect.Bottom - StiScale.I1, StiScale.I3, StiScale.I3);// BottomCenter
                        StiDrawing.FillRectangle(g, color, rect.X - StiScale.I1, rect.Bottom - StiScale.I1, StiScale.I3, StiScale.I3);// BottomLeft
                    }

                    index++;
                }
            }
        }

        public virtual StiPagePaintEventArgs PaintCopyrightBefore(StiPage page, Graphics g, StiPaintEventArgs eventArgs)
        {
            var scale = page.Zoom * StiScale.Factor;

            var mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
            var mgTop = page.Unit.ConvertToHInches(page.Margins.Top);
            var pgWidth = page.Unit.ConvertToHInches(page.DisplayRectangle.Width);
            var pgHeight = page.Unit.ConvertToHInches(page.DisplayRectangle.Height);

            var widthInHi = page.Unit.ConvertToHInches(page.Width);
            var heightInHi = page.Unit.ConvertToHInches(page.Height);

            var paintEventArgs = new StiPagePaintEventArgs(
                eventArgs.Graphics,
                eventArgs.ClipRectangle.ToRectangle(),
                new Rectangle((int) (mgLeft * scale), (int) (mgTop * scale), (int) (widthInHi * scale), (int) (heightInHi * scale)),
                new Rectangle(0, 0, (int) (pgWidth * scale), (int) (pgHeight * scale)),
                page.Report.IsPrinting, page.Report.IsDesigning);

            g.TranslateTransform(-(int) (mgLeft * scale), -(int) (mgTop * scale));
            StiPage.InvokePagePainting(page, paintEventArgs);
            g.TranslateTransform((int) (mgLeft * scale), (int) (mgTop * scale));

            return paintEventArgs;
        }

        public virtual void PaintCopyrightAfter(StiPage page, Graphics g, StiPagePaintEventArgs eventArgs)
        {
            var scale = page.Zoom * StiScale.Factor;

            var mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
            var mgTop = page.Unit.ConvertToHInches(page.Margins.Top);

            g.TranslateTransform(-(int) (mgLeft * scale), -(int) (mgTop * scale));
            StiPage.InvokePagePainted(page, eventArgs);
            g.TranslateTransform((int) (mgLeft * scale), (int) (mgTop * scale));
        }

        private static void PaintDimensionLines(StiPage page, Graphics g)
        {
            if (page.Report.Info.ShowDimensionLines && page.IsDesigning && page.Report.Info.CurrentAction != StiAction.None && (!page.IsDashboard))
                StiDimensionLinesHelper.DrawDimensionLines(g, page);
        }

        private void PaintTopWatermark(StiPage page, Graphics g, RectangleF printerMargins)
        {
            var scale = page.Zoom * StiScale.Factor;

            var mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
            var mgRight = page.Unit.ConvertToHInches(page.Margins.Right);
            var mgTop = page.Unit.ConvertToHInches(page.Margins.Top);
            var pgWidth = page.Unit.ConvertToHInches(page.DisplayRectangle.Width);

            if (page.IsPrinting)
            {
                if (page.StretchToPrintArea)
                    g.PageScale = .01f;

                g.ResetTransform();
                g.TranslateTransform(-printerMargins.Left, -printerMargins.Top);
                PaintWatermark(page, g, false); //At top
                g.ResetTransform();

                if (page.StretchToPrintArea)
                    g.PageScale = .01f * (float) (printerMargins.Width / (pgWidth - mgLeft - mgRight));
            }
            else
            {
                g.TranslateTransform(-(int) (mgLeft * scale), -(int) (mgTop * scale));
                PaintWatermark(page, g, false);
                g.TranslateTransform((int) (mgLeft * scale), (int) (mgTop * scale));
            }
        }

        /// <summary>
        /// Paints the highlight of the specified component.
        /// </summary>
        public override void PaintHighlight(StiComponent component, StiPaintEventArgs e)
        {
            var page = component as StiPage;
            if (page.LockHighlight || page.IsPrinting) return;

            base.PaintHighlight(page, e);
        }

        /// <summary>
        /// Paints order numbers and quick info of a component.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="g">The Graphics to paint on.</param>
        /// <param name="number">A number of the component order for painting.</param>
        public override void PaintOrderAndQuickInfo(StiComponent component, Graphics g, string number)
        {
            var page = component.Page;

            if (page == null) return;

            if ((!page.Report.Info.ShowOrder || !page.IsDesigning) &&
                (!page.Report.Info.QuickInfoOverlay || page.Report.Info.QuickInfoType == StiQuickInfoType.None)) return;

            base.PaintOrderAndQuickInfo(component, g, string.Empty);
        }

        public virtual void PaintLargeHeightFactorLine(StiPage page, Graphics g)
        {
            if (page.IsPrinting) return;
            if (!(page.LargeHeightAutoFactor > 1) || !page.IsDesigning) return;

            var scale = page.Zoom * StiScale.Factor;

            var widthInHi = page.Unit.ConvertToHInches(page.Width);
            var mgLeft = page.Unit.ConvertToHInches(page.Margins.Left);
            var mgBottom = page.Unit.ConvertToHInches(page.Margins.Bottom);
            var pgWidth = page.Unit.ConvertToHInches(page.DisplayRectangle.Width);

            var largeHeightValue = (float) (page.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom) *
                         page.SegmentPerHeight * scale);

            using (var pen = new Pen(Color.Red, (float) (2 * scale)))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawLine(pen, 0, largeHeightValue, (float) (widthInHi * scale), largeHeightValue);
                g.DrawLine(pen, 
                    -(float) (mgLeft * scale), (float) (largeHeightValue + mgBottom * scale),
                    (float) (pgWidth * scale), (float) (largeHeightValue + mgBottom * scale));
            }
        }

        public virtual void PaintPageBreaks(StiPage page, Graphics g, double width, double height)
        {
            if (page.DenyDrawSegmentMode || !StiOptions.Viewer.AllowDrawSegmentMode) return;
            if ((page.IsPrinting || page.SegmentPerWidth <= 1 && page.SegmentPerHeight <= 1) &&
                (page.Report.Info.ViewMode != StiViewMode.PageBreakPreview || !page.IsDesigning)) return;

            var scale = page.Zoom * StiScale.Factor;

            #region Segment page
            if (page.SegmentPerWidth > 1 || page.SegmentPerHeight > 1)
            {
                var widthStep = page.Unit.ConvertToHInches(page.Width / page.SegmentPerWidth);
                var heightStep = page.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom);

                using (var pen = new Pen(Color.Blue))
                {
                    pen.Width = 2;
                    pen.DashStyle = DashStyle.Dash;

                    double py = 0;
                    var startPageNumber = 1;

                    if (!page.IsDesigning && page.Report.RenderedPages.Count > 0)
                        startPageNumber = page.Report.GetPageNumber(page.Report.RenderedPages.IndexOf(page));

                    for (var dy = 0; dy < page.SegmentPerHeight; dy++)
                    {
                        double px = 0;
                        py += heightStep;

                        for (var dx = 0; dx < page.SegmentPerWidth; dx++)
                        {
                            px += widthStep;
                            if (dx < page.SegmentPerWidth - 1)
                            {
                                g.DrawLine(pen, (float) (px * scale), 0,
                                    (float) (px * scale), (float) (page.Unit.ConvertToHInches(page.Height) * scale));
                            }

                            var pageRect = new RectangleD(
                                (px - widthStep) * scale, (py - heightStep) * scale,
                                widthStep * scale, heightStep * scale);

                            if (StiOptions.Viewer.IsRightToLeft)
                                pageRect.X = (page.SegmentPerWidth * widthStep - px) * scale;

                            DrawPageNumber(page, g, startPageNumber, pageRect);

                            startPageNumber++;
                        }

                        if (dy < page.SegmentPerHeight - 1)
                        {
                            g.DrawLine(pen, 0, (float) (py * scale),
                                (float) (page.Unit.ConvertToHInches(page.Width) * scale), (float) (py * scale));
                        }
                    }
                }
            }
            #endregion

            else
                DrawPageNumber(page, g, 1, new RectangleD(0, 0, width * scale, height * scale));

            using (var pen = new Pen(Color.Blue))
            {
                pen.Width = 2;
                g.DrawRectangle(pen, 0, 0, (float) (width * scale), (float) (height * scale));
            }
        }

        /// <summary>
        /// Draws page numbers.
        /// </summary>
        public virtual void DrawPageNumber(StiPage page, Graphics g, int pageNumber, RectangleD pageRect)
        {
            if (!(page.Zoom > .1d)) return;

            using (var sf = new StringFormat())
            using (var font = new Font("Arial", 14))
            using (var brush = new SolidBrush(Color.FromArgb(100, Color.Blue)))
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisPath;

                g.DrawString(Loc.Get("Components", "StiPage") + pageNumber, font,
                    brush, pageRect.ToRectangleF(), sf);
            }
        }

        public virtual void PaintTableLines(StiPage page, Graphics g)
        {
            if (page.Report.Info.IsComponentsMoving && page.Report.Info.CurrentAction == StiAction.Move) return;
            if (!StiTableHelper.IsTableMode(page.Report.Designer)) return;

            var xx = new Hashtable();
            var yy = new Hashtable();

            StiTableHelper.GetSelectedLines(page.Report.Designer, ref xx, ref yy, true);
            var rect = StiTableHelper.GetSelectedRectangle(page.Report.Designer, true);

            var penColor = GetTableLinesColor(page);

            using (var pen = new Pen(penColor))
            {
                pen.DashStyle = DashStyle.Dash;
                foreach (double value in xx.Keys)
                {
                    if (!double.IsNaN(value))
                        g.DrawLine(pen, (float)value, (float)rect.Y, (float)value, (float)rect.Bottom);
                }

                foreach (double value in yy.Keys)
                {
                    if (!double.IsNaN(value))
                        g.DrawLine(pen, (float)rect.X, (float)value, (float)rect.Right, (float)value);
                }
            }
        }

        public virtual Color GetTableLinesColor(StiPage page)
        {
            return page is StiForm ? Color.DimGray : Color.Blue;
        }

        public virtual void PaintSelection(StiPage page, Graphics g)
        {
            if (page.SelectedRectangle.IsEmpty) return;

            var scale = page.Zoom * StiScale.Factor;
            using (var brush = new SolidBrush(GetSelectionBackgroundColor(page)))
            using (var pen = new Pen(GetSelectionBorderColor(page)))
            {
                pen.DashStyle = DashStyle.Dash;

                var rect = page.SelectedRectangle;

                rect.X *= scale;
                rect.Y *= scale;
                rect.Width *= scale;
                rect.Height *= scale;

                rect = page.Page.Unit.ConvertToHInches(rect);

                var rectSelect = rect.ToRectangleF();
                g.FillRectangle(brush, rectSelect);
                g.DrawRectangle(pen, rectSelect.X, rectSelect.Y, rectSelect.Width, rectSelect.Height);
            }
        }

        public virtual void PaintSegment(Graphics g, StiPage page, RectangleF printerMargins)
        {
            var mgLeft = (float)page.Unit.ConvertToHInches(page.Margins.Left);
            var mgTop = (float)page.Unit.ConvertToHInches(page.Margins.Top);

            g.PageScale = .01f;
            g.TranslateTransform(-printerMargins.Left + mgLeft, -printerMargins.Top + mgTop);

            PaintWatermark(page, g, true);

            var contentWidth = page.PageWidth - page.Margins.Left - page.Margins.Right;
            var contentHeight = page.PageHeight - page.Margins.Top - page.Margins.Bottom;

            var scLeft = (float)page.Unit.ConvertToHInches(page.CurrentWidthSegment * contentWidth);
            var scTop = (float)page.Unit.ConvertToHInches(page.CurrentHeightSegment * contentHeight);

            g.SetClip(new RectangleF(0, 0,
                (float)page.Unit.ConvertToHInches(contentWidth),
                (float)page.Unit.ConvertToHInches(contentHeight)), CombineMode.Replace);

            g.TranslateTransform(-scLeft, -scTop);
        }

        public virtual void PaintContentBorder(Graphics g, StiPage page, double width, double height)
        {
            var scale = page.Zoom * StiScale.Factor;
            using (var pen = new Pen(StiColorUtils.Dark(Color.Gainsboro, 100)))
            {
                StiDrawing.DrawRectangle(g, pen, 0, 0, width * scale, height * scale);
            }
        }

        public virtual void PaintColumns(Graphics g, StiPage page, double height)
        {
            if (page.Columns <= 1) return;

            var scale = page.Zoom * StiScale.Factor;
            var columnWidth = page.Unit.ConvertToHInches(page.GetColumnWidth()) * scale;
            var columnGaps = page.Unit.ConvertToHInches(page.ColumnGaps) * scale;

            var pos = columnWidth;
            using (var pen = new Pen(Color.Red))
            {
                pen.DashStyle = DashStyle.Dash;

                for (var index = 1; index < page.Columns; index++)
                {
                    g.DrawLine(pen, (float) pos, 0, (float) pos, (float) (height * scale));
                    g.DrawLine(pen, (float) (pos + columnGaps), 0, (float) (pos + columnGaps), (float) (height * scale));

                    pos += columnWidth + columnGaps;
                }

                g.DrawLine(pen, (float) pos, 0, (float) pos, (float) (height * scale));
            }
        }

        public virtual void PaintBackground(StiPaintEventArgs e, Graphics g, StiPage page, double width, double height)
        {
            var scale = page.Zoom * StiScale.Factor;
            var rect = page.IsDesigning ? e.ClipRectangle : new RectangleD(0, 0, width * scale, height * scale);

            if (page.IsPrinting)
            {
                if (!(StiBrush.IsTransparent(page.Brush) || (page.Brush is StiSolidBrush && (page.Brush as StiSolidBrush).Color.Equals(Color.White))))
                    StiDrawing.FillRectangle(g, page.Brush, 0, 0, width * scale, height * scale);
            }
            else
            {
                if (StiBrush.IsTransparent(page.Brush))
                    StiDrawing.FillRectangle(g, Brushes.White, rect);
                else
                    StiDrawing.FillRectangle(g, page.Brush, 0, 0, width * scale, height * scale);
            }
        }

        public virtual void PaintGrid(Graphics g, StiPage page, double width, double height)
        {
            if (!page.IsDesigning || !page.Report.Info.ShowGrid) return;

            switch (page.Report.Info.GridMode)
            {
                case StiGridMode.Lines:
                    PaintGridLines(g, page, width, height);
                    break;

                case StiGridMode.Dots:
                    PaintGridDots(g, page, width, height);
                    break;
            }
        }

        public virtual void PaintGridLines(Graphics g, StiPage page, double width, double height)
        {
            var gridSize = page.Unit.ConvertToHInches(page.GridSize);
            var scale = page.Zoom * StiScale.Factor;

            using (var pen = new Pen(StiColorUtils.Dark(Color.Gainsboro, 30)))
            {
                var posx = 0d;
                var wdMax = width * scale;
                var st = gridSize * scale;
                var a = 0;
                for (posx = 0; posx < wdMax; posx += st)
                {
                    if ((a & 1) == 0)
                        StiDrawing.DrawLine(g, pen, posx, 0, posx, height * scale);
                    else
                        StiDrawing.DrawLine(g, Pens.Gainsboro, posx, 0, posx, height * scale);

                    a++;
                }

                var posy = 0d;
                var htMax = height * scale;
                st = gridSize * scale;
                a = 0;
                for (posy = 0; posy < htMax; posy += st)
                {
                    if ((a & 1) == 0)
                        StiDrawing.DrawLine(g, pen, 0, posy, width * scale, posy);
                    else
                        StiDrawing.DrawLine(g, Pens.Gainsboro, 0, posy, width * scale, posy);

                    a++;
                }
            }
        }

        public virtual void PaintGridDots(Graphics g, StiPage page, double width, double height)
        {
            var gridSize = page.Unit.ConvertToHInches(page.GridSize);
            var scale = page.Zoom * StiScale.Factor;

            using (var bmp = new Bitmap((int) (width * scale) + 1, 1))
            {
                var wdMax = width * scale;
                var st = gridSize * scale;

                for (double posx = 0; posx < wdMax; posx += st)
                {
                    var pixelPos = (int) Math.Round(posx);
                    if (pixelPos < bmp.Width)
                        bmp.SetPixel(pixelPos, 0, StiColorUtils.Dark(Color.Gainsboro, 30));
                }

                var htMax = height * scale;
                for (double posy = 0; posy < htMax; posy += st)
                {
                    g.DrawImage(bmp, 0, (int) posy);
                }
            }
        }

        public virtual Color GetSelectionBackgroundColor(StiPage page)
        {
            return Color.FromArgb(75, Color.Blue);
        }

        public virtual Color GetSelectionBorderColor(StiPage page)
        {
            return Color.DimGray;
        }

        public virtual Color GetSelectionCornerColor(StiPage page)
        {
            return Color.Gray;
        }
        #endregion

    }
}
