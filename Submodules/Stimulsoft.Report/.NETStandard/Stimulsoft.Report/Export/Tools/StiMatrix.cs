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
using System.Globalization;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Plans;
using Stimulsoft.Report.BarCodes;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Security.Cryptography;
#else
using System.Windows.Forms;
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiMatrix
    {
        #region enum StiTableLineInfo
        public enum StiTableLineInfo
        {
            Empty = 0,
            Unknown,
            PageHeader,
            PageFooter,
            HeaderAP,
            FooterAP,
            HeaderD,
            FooterD,
            Data,
            Trash
        }
        #endregion

        internal class RoundedRectangleInfo
        {
            public Rectangle Rect;
            public StiRoundedRectanglePrimitive Primitive;
        }

        #region Fields
        internal StiCell[,] cells2;
        public StiMatrixCellsCollection Cells;
        internal StiBorderSide[,] bordersX2;
        internal StiBorderSide[,] bordersY2;
        public StiMatrixBorderSidesXCollection BordersX;
        public StiMatrixBorderSidesYCollection BordersY;
        internal StiCellStyle[,] cellStyles2;
        public StiMatrixCellStylesCollection CellStyles;
        internal string[,] bookmarks2;
        public StiMatrixBookmarksCollection Bookmarks;
        internal StiExportFormat exportFormat = StiExportFormat.Excel;
        private bool isHtmlService;
        private bool isHtmlOrExcelXmlService;
        //private bool isHtmlPngMode;
        internal bool isPaginationMode;
        internal StiReport report;
        private StiPagesCollection pages;
        private bool addComponentWithInteractions;
        private bool replaceCheckboxes;
        private Hashtable hyperlinksToTag;
        internal Hashtable pointerToBookmark;    //convert pointer link to bookmark
        private Hashtable pointerToTag;    //convert pointer link to bookmark
        internal Dictionary<int, List<RoundedRectangleInfo>> RoundedRectanglesList;
        private List<KeyValuePair<StiComponent, double>> tempRoundedRectangleInfoList;

        private double maxCoordY;

        private double defaultLinePrimitiveWidth;

        internal bool useCacheMode;
        private StiMatrixCacheManager cacheManager;
        internal StiText additionalInfo;
        internal StiContainer paginationContainerInfo;

        private bool[] coordXCheck;
        private bool[] coordYCheck;
        private int[] coordXNew;
        private int[] coordYNew;
        private int[] coordXPrim;
        private int[] coordYPrim;
        internal Hashtable imagesBaseRect;

        private Hashtable leftCached = new Hashtable();
        private Hashtable topCached = new Hashtable();

        private Hashtable xcHash = new Hashtable();
        private Hashtable ycHash = new Hashtable();

        private Hashtable tagSplitCache = new Hashtable();

        private Hashtable stylesCache = new Hashtable();
        private Hashtable fontsCache = new Hashtable();

        private ArrayList createdCells = new ArrayList();

        internal bool useFullExceedMargins = false;
        #endregion

        #region Properties
        private StiCell[,] CellsMap { get; set; }

        public double TotalWidth { get; set; }

        private double TotalHeight { get; set; }

        public ArrayList Styles { get; private set; } = new ArrayList();

        public SortedList CoordX { get; private set; } = new SortedList();

        public SortedList CoordY { get; private set; } = new SortedList();

        public StiTableLineInfo[] LinePlacement { get; private set; }

        public string[] ParentBandName { get; private set; }

        public List<int> HorizontalPageBreaks { get; private set; } = new List<int>();
        public Dictionary<int, object> HorizontalPageBreaksHash { get; private set; } = new Dictionary<int, object>();

        public int[,,] Interactions { get; }

        private List<StiBorderSide> borderSides;
        internal List<StiBorderSide> BorderSides
        {
            get
            {
                return borderSides ?? (borderSides = new List<StiBorderSide>());
            }
        }
        #endregion

        #region Consts
        private const float maxRowHeight = 200;
        private const int _defaultLinePrimitiveWidth = 1; //in hinch
        internal const double htmlScaleX = 0.96;
        internal const double htmlScaleY = 0.956;
        private const int paginationModeOffset = 2;
        #endregion

        #region Fields.Static
        private static readonly StiRectanglePrimitive staticRectanglePrimitive = new StiRectanglePrimitive();
        #endregion

        #region Methods
        private double Round(double value)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private void AddCoord(RectangleD rect)
        {
            AddCoord(rect.Left, rect.Top + TotalHeight);
            AddCoord(rect.Right, rect.Bottom + TotalHeight);
        }

        private void AddCoord(double x, double y, bool convert = true)
        {
            if (y > maxCoordY) maxCoordY = y;

            if (convert && isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization)
            {
                x = Round(x * htmlScaleX);
                y = Round(y * htmlScaleY);
            }
            else
            {
                x = Round(x);
                y = Round(y);
            }

            if (!xcHash.ContainsKey(x))
            {
                CoordX[x] = x;
                xcHash[x] = null;
            }
            if (!ycHash.ContainsKey(y))
            {
                CoordY[y] = y;
                ycHash[y] = null;
            }
        }

        private int GetCoordXRounded(double x)
        {
            if (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization)
            {
                return (int)Round(x * htmlScaleX);
            }
            return (int)Round(x);
        }
        private int GetCoordYRounded(double y)
        {
            if (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization)
            {
                return (int)Round(y * htmlScaleY);
            }
            return (int)Round(y);
        }

        public void PrepareTable()
        {
            for (int rowIndex = 1; rowIndex < CoordY.Count; rowIndex++)
            {
                double rowHeight = (double)CoordY.GetByIndex(rowIndex) - (double)CoordY.GetByIndex(rowIndex - 1);
                rowHeight = Round(rowHeight);

                double maxLineHeight = maxRowHeight;
                if (rowHeight > maxLineHeight)
                {
                    if (rowHeight / maxLineHeight > 100)    //very big, possible error
                    {
                        maxLineHeight = (int)(rowHeight / 100);
                        for (int index = 1; index < 100; index ++)
                        {
                            double newY = (double)CoordY.GetByIndex(rowIndex - 1) + maxLineHeight * index;
                            AddCoord(0f, newY, false);
                        }
                        rowIndex += 99;
                        continue;
                    }
                    if (rowHeight < maxLineHeight * 2)
                    {
                        maxLineHeight = Round(rowHeight / 2);
                    }
                    double endHeight = (double)CoordY.GetByIndex(rowIndex - 1) + maxLineHeight;
                    AddCoord(0f, endHeight, false);
                }
            }

            //restriction of excel - max column width is 255pt
            for (int columnIndex = 1; columnIndex < CoordX.Count; columnIndex++)
            {
                double columnWidth = (double)CoordX.GetByIndex(columnIndex) - (double)CoordX.GetByIndex(columnIndex - 1);
                columnWidth = Round(columnWidth);

                double maxColumnWidth = 1900;
                if (columnWidth > maxColumnWidth)
                {
                    if (columnWidth < maxColumnWidth * 2)
                    {
                        maxColumnWidth = Round(columnWidth / 2);
                    }
                    double endWidth = (double)CoordX.GetByIndex(columnIndex - 1) + maxColumnWidth;
                    AddCoord(endWidth, 0f, false);
                }
            }
        }

        public Rectangle GetRange(RectangleD rect)
        {
            double scaleX = (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization) ? htmlScaleX : 1;
            double scaleY = (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization) ? htmlScaleY : 1;

            double ll = Round(rect.Left * scaleX);
            double tt = Round((rect.Top + TotalHeight) * scaleY);
            double rr = Round(rect.Right * scaleX);
            double bb = Round((rect.Bottom + TotalHeight) * scaleY);

            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;

            object obj = leftCached[ll];
            if (obj == null)
            {
                left = CoordX.IndexOfValue(ll);
                leftCached[ll] = left;
            }
            else left = (int)obj;

            obj = topCached[tt];
            if (obj == null)
            {
                top = CoordY.IndexOfValue(tt);
                topCached[tt] = top;
            }
            else top = (int)obj;

            obj = leftCached[rr];
            if (obj == null)
            {
                right = CoordX.IndexOfValue(rr);
                leftCached[rr] = right;
            }
            else right = (int)obj;

            obj = topCached[bb];
            if (obj == null)
            {
                bottom = CoordY.IndexOfValue(bb);
                topCached[bb] = bottom;
            }
            else bottom = (int)obj;

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public StiCellStyle GetStyleFromComponent(StiComponent component, int x, int y)
        {
            Color color = Color.White;
            IStiBrush brushComp = component as IStiBrush;
            if ((brushComp != null) && !(component is StiShape)) color = StiBrush.ToColor(brushComp.Brush);
            if (component is StiShape /* && isHtmlPngMode */) color = Color.Transparent;

            StiRichText richText = component as StiRichText;
            if (richText != null) color = richText.BackColor;

            if (component is StiPrimitive || component is StiBarCode) color = Color.Transparent;

            if ((x != -1) && (y != -1) && (color.A == 0) && (CellsMap[y, x] != null))
            {
                StiCell cell = CellsMap[y, x];
                color = cell.CellStyle.Color;
            }

            Color textColor = Color.Black;
            IStiTextBrush textBrushComp = component as IStiTextBrush;
            if (textBrushComp != null) textColor = StiBrush.ToColor(textBrushComp.TextBrush);

            Font font = null;
            IStiFont textFontComp = component as IStiFont;
            if (textFontComp != null) font = textFontComp.Font;
            else font = new Font("Arial", 8);

            StiTextHorAlignment horAlign = StiTextHorAlignment.Left;
            IStiTextHorAlignment horAlignComp = component as IStiTextHorAlignment;
            if (horAlignComp != null) horAlign = horAlignComp.HorAlignment;

            StiVertAlignment vertAlign = StiVertAlignment.Top;
            IStiVertAlignment vertAlignComp = component as IStiVertAlignment;
            if (vertAlignComp != null) vertAlign = vertAlignComp.VertAlignment;

            StiBorderSide borderL = null;
            StiBorderSide borderR = null;
            StiBorderSide borderT = null;
            StiBorderSide borderB = null;
            IStiBorder borderComp = component as IStiBorder;
            if (borderComp != null)
            {
                StiAdvancedBorder advBorder = borderComp.Border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    borderL = advBorder.LeftSide;
                    borderR = advBorder.RightSide;
                    borderT = advBorder.TopSide;
                    borderB = advBorder.BottomSide;
                }
                else
                {
                    borderL = new StiBorderSide(borderComp.Border.Color, borderComp.Border.Size, borderComp.Border.Style);
                    if (borderComp.Border.IsRightBorderSidePresent) borderR = borderL;
                    if (borderComp.Border.IsTopBorderSidePresent) borderT = borderL;
                    if (borderComp.Border.IsBottomBorderSidePresent) borderB = borderL;
                    if (!borderComp.Border.IsLeftBorderSidePresent) borderL = null;
                }
            }

            StiTextOptions textOptions = null;
            IStiTextOptions textOptionsComp = component as IStiTextOptions;
            if (textOptionsComp != null) textOptions = textOptionsComp.TextOptions;

            bool wordWrap = false;
            IStiText text = component as IStiText;
            if (text != null && text.GetTextInternal() != null) wordWrap = text.GetTextInternal().IndexOf("\n", StringComparison.InvariantCulture) != -1;
            if (isHtmlOrExcelXmlService && textOptions != null)
            {
                wordWrap = textOptions.WordWrap;
            }

            string format = null;
            StiText textComp = component as StiText;
            if ((exportFormat == StiExportFormat.ExcelXml) && (textComp != null))
            {
                #region Scan value format
                string inputFormat = textComp.Format;
                bool isFormatCurrency = false;
                bool isFormatNumeric = false;
                bool isFormatPercent = false;
                string outputFormat = string.Empty;
                int decimalDigits = 2;
                int groupDigits = 0;
                char currencySymbol = '$';
                bool currencyPositionBefore = false;
                bool negativeBraces = false;
                bool hideZeros = (textComp != null) && textComp.HideZeros;

                string positivePatternString = null;
                string negativePatternString = null;
                int posPatternDelimiter = (inputFormat != null ? inputFormat.IndexOf("|") : -1);
                if (posPatternDelimiter != -1)
                {
                    positivePatternString = StiExportUtils.GetPositivePattern((int)inputFormat[posPatternDelimiter + 1] - (int)'A');
                    negativePatternString = StiExportUtils.GetNegativePattern((int)inputFormat[posPatternDelimiter + 2] - (int)'A');
                    inputFormat = inputFormat.Substring(0, posPatternDelimiter);
                }

                #region get value format
                if (!string.IsNullOrEmpty(inputFormat))
                {
                    if (inputFormat[0] == 'C')
                    {
                        isFormatCurrency = true;
                    }
                    if (inputFormat[0] == 'N')
                    {
                        isFormatNumeric = true;
                    }
                    if (inputFormat[0] == 'P')
                    {
                        isFormatPercent = true;
                    }
                    if ((isFormatCurrency || isFormatNumeric || isFormatPercent) && (inputFormat.Length > 1))
                    {
                        #region scan parameters
                        int indexPos = 1;
                        if (char.IsDigit(inputFormat[indexPos]))
                        {
                            var decimalSB = new StringBuilder();
                            while ((indexPos < inputFormat.Length) && (char.IsDigit(inputFormat[indexPos])))
                            {
                                decimalSB.Append(inputFormat[indexPos]);
                                indexPos++;
                            }
                            decimalDigits = int.Parse(decimalSB.ToString());
                        }
                        if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == 'G'))
                        {
                            indexPos++;
                            groupDigits = 3;
                        }
                        if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == '('))
                        {
                            indexPos++;
                            negativeBraces = true;
                        }
                        if ((indexPos < inputFormat.Length) && (inputFormat[indexPos] == '.' || inputFormat[indexPos] == ','))
                        {
                            indexPos++;
                        }
                        if ((indexPos < inputFormat.Length) &&
                            ((inputFormat[indexPos] == '+') || (inputFormat[indexPos] == '-')))
                        {
                            if (inputFormat[indexPos] == '+') currencyPositionBefore = true;
                            indexPos++;
                            if (indexPos < inputFormat.Length)
                            {
                                currencySymbol = inputFormat[indexPos];
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region make format string
                if (isFormatCurrency || isFormatNumeric || isFormatPercent)
                {
                    if (posPatternDelimiter != -1)
                    {
                        StringBuilder outputSB = new StringBuilder();
                        if (groupDigits > 1)
                        {
                            outputSB.Append("#,");
                            outputSB.Append('#', groupDigits - 1);
                        }
                        outputSB.Append('0');
                        if (decimalDigits > 0)
                        {
                            outputSB.Append(".");
                            outputSB.Append('0', decimalDigits);
                        }
                        string nn = outputSB.ToString();

                        string positivePattern = positivePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");
                        string negativePattern = negativePatternString.Replace("n", nn).Replace("$", $"\"{currencySymbol}\"");

                        outputFormat = positivePattern + ";" + negativePattern + (hideZeros ? ";" : "");
                    }
                    else
                    {
                        StringBuilder outputSB = new StringBuilder();
                        if ((isFormatCurrency) && (currencyPositionBefore == true))
                        {
                            outputSB.Append("\"");
                            outputSB.Append(currencySymbol);
                            outputSB.Append("\"");
                        }
                        if (groupDigits > 1)
                        {
                            outputSB.Append("#,");
                            outputSB.Append('#', groupDigits - 1);
                        }
                        outputSB.Append('0');
                        if (decimalDigits > 0)
                        {
                            outputSB.Append(".");
                            outputSB.Append('0', decimalDigits);
                        }
                        if ((isFormatCurrency) && (currencyPositionBefore == false))
                        {
                            outputSB.Append("\"");
                            outputSB.Append(currencySymbol);
                            outputSB.Append("\"");
                        }
                        if (isFormatPercent)
                        {
                            outputSB.Append("%");
                        }
                        outputFormat = outputSB.ToString();
                        string negativePattern = (negativeBraces ? "(" : "-") + outputFormat + (negativeBraces ? ")" : "");
                        if (hideZeros)
                        {
                            outputFormat = outputFormat + ";" + negativePattern + ";";
                        }
                        else
                        {
                            if (negativeBraces) outputFormat = outputFormat + ";" + negativePattern;
                        }
                    }
                }
                #endregion

                #endregion

                format = outputFormat;
            }

            string styleName = null;
            if (!string.IsNullOrEmpty(component.ComponentStyle))
            {
                styleName = component.ComponentStyle;
            }
            if (isHtmlOrExcelXmlService && !StiOptions.Export.Html.UseComponentStyleName) styleName = null;

            double lineSpacing = (textComp != null) ? textComp.LineSpacing : 1;

            bool allowHtmlTags = isHtmlService && (textComp != null) && textComp.CheckAllowHtmlTags();

            //return GetStyle(new StiCellStyle(color, textColor, font, horAlign, vertAlign, borderT, borderL, borderR, borderB,
            //    textOptions, wordWrap, format, styleName));
            return StiCellStyle.GetStyleFromCache(color, textColor, font, horAlign, vertAlign, borderT, borderL, borderR, borderB,
                textOptions, wordWrap, format, styleName, lineSpacing, allowHtmlTags, stylesCache, Styles, fontsCache, null, false);
        }

        private StiCellStyle GetStyle(StiCellStyle style)
        {
            foreach (StiCellStyle stl in Styles)
            {
                if (stl.Equals(style)) return stl;
            }
            Styles.Add(style);
            return style;
        }

        private void RenderComponent(StiComponent component, int indexComponent, bool exportData, bool isPage)
        {
            var baseComponent = component;
            ReplaceComponentForExport(ref component, replaceCheckboxes);

            RectangleD rectD = component.Page.Unit.ConvertToHInches(component.DisplayRectangle);
            if (rectD.Height < 0)
            {
                rectD.Y += rectD.Height;
                rectD.Height = Math.Abs(rectD.Height);
            }

            if ((!(component is StiLinePrimitive)) && ((rectD.Width == 0) || (rectD.Height == 0))) return;

            if (component is StiLinePrimitive)
            {
                if (component is StiRectanglePrimitive)
                {
                    rectD.Y -= defaultLinePrimitiveWidth;
                    rectD.X -= defaultLinePrimitiveWidth;
                }
                else
                {
                    if (rectD.Height < 1.5)
                    {
                        rectD.Height = defaultLinePrimitiveWidth;
                        rectD.Y -= rectD.Height;
                    }
                    if (rectD.Width < 1.5)
                    {
                        rectD.Width = defaultLinePrimitiveWidth;
                        rectD.X -= rectD.Width;
                    }
                }
            }
            else
            {
                if (rectD.Height < 1.5 && rectD.Height > 0)
                {
                    if (GetCoordYRounded(rectD.Top) == GetCoordYRounded(rectD.Bottom))
                    {
                        rectD.Height = 1d / GetCoordYRounded(1) + 0.01;
                    }
                }
                if (rectD.Width < 1.5 && rectD.Width > 0)
                {
                    if (GetCoordXRounded(rectD.Left) == GetCoordXRounded(rectD.Right))
                    {
                        rectD.Width = 1d / GetCoordXRounded(1) + 0.01;
                    }
                }
            }

            Rectangle rect = GetRange(rectD);

            if (rect.Left != -1)
            {
                bool needAdd = true;

                string bookmark = component.BookmarkValue as string;
                string tag = component.TagValue as string;
                if (bookmark == null)
                {
                    if (!string.IsNullOrEmpty(tag) && hyperlinksToTag.ContainsKey(tag))
                    {
                        bookmark = tag;
                    }
                    else if (!string.IsNullOrWhiteSpace((string)component.PointerValue) && !string.IsNullOrEmpty(component.Guid))
                    {
                        bookmark = $"{component.PointerValue}#GUID#{component.Guid}";
                    }
                }

                IStiBorder iborderComp = component as IStiBorder;
                StiBorder cBorder = iborderComp != null ? iborderComp.Border : null;
                bool haveBookmark = !string.IsNullOrEmpty(bookmark);
                bool haveBorder = (cBorder != null) && (cBorder.Side != StiBorderSides.None);
                bool haveText = false;
                bool haveBrush = false;
                bool haveExcel = false;
                bool haveIndicator = false;
                StiText sText = component as StiText;
                if (sText != null)
                {
                    haveText = (sText.Text != null) && (sText.GetTextInternal() != null) && ((sText.GetTextInternal().Length > 0) || (exportData));
                    haveBrush = (sText.Brush != null) && (StiBrush.ToColor(sText.Brush).A != 0);
                    haveExcel = sText.ExcelDataValue != null;
                    haveIndicator = sText.Indicator != null;
                    needAdd = haveText || haveBrush || haveExcel || haveBorder || haveIndicator || sText.Editable;
                }
                StiContainer cont = component as StiContainer;
                if (cont != null)
                {
                    haveBrush = (cont.Brush != null) && (StiBrush.ToColor(cont.Brush).A != 0);
                    needAdd = haveBrush || (haveBorder && !isPage);
                }
                if (haveBookmark) needAdd = true;

                int rectLeft = rect.Left;
                int rectRight = rect.Right;
                int rectTop = rect.Top;
                int rectBottom = rect.Bottom;

                #region Check narrow for StiRectanglePrimitive
                if (component is StiRectanglePrimitive)
                {
                    needAdd = false;
                    if (coordXPrim[rectLeft] == 0) coordXPrim[rectLeft] = 1;
                    if (coordXPrim[rectRight] == 0) coordXPrim[rectRight] = 1;
                    if (coordYPrim[rectTop] == 0) coordYPrim[rectTop] = 1;
                    if (coordYPrim[rectBottom] == 0) coordYPrim[rectBottom] = 1;
                }
                #endregion

                bool haveInteraction = IsComponentHasInteraction(component);
                int interactionPageId = 0;
                int interactionComponentId = 0;
                if (haveInteraction)
                {
                    needAdd = true;
                    interactionPageId = component.Report.RenderedPages.IndexOf(component.Page) + 1;
                    interactionComponentId = component.Page.Components.IndexOf(component) + 1;
                }

                if (isHtmlService && StiOptions.Export.Html.AllowRoundedRectangles && (component is StiRoundedRectanglePrimitive))
                {
                    tempRoundedRectangleInfoList.Add(new KeyValuePair<StiComponent, double>(component, TotalHeight));
                    haveBorder = false;
                }

                if (needAdd)
                {
                    #region Add cell
                    StiCell cell = useCacheMode ? new StiCell2(this) : new StiCell(exportFormat);
                    createdCells.Add(cell);

                    if (!(exportFormat == StiExportFormat.Csv || exportFormat == StiExportFormat.Dbf || exportFormat == StiExportFormat.Xml || exportFormat == StiExportFormat.Json))
                    {
                        cell.CellStyle = GetStyleFromComponent(component, rectLeft, rectTop);
                    }

                    #region Assign component property
                    bool assignComponent = true;
                    if (exportFormat == StiExportFormat.ExcelXml)
                    {
                        assignComponent = false;
                        if (component is StiRichText) assignComponent = true;
                        if (component is StiCheckBox) assignComponent = true;
                        if (sText != null && sText.ExcelDataValue != null) assignComponent = true;
                        if (StiOptions.Export.Excel.AllowFreezePanes && (component.Locked || (component.TagValue != null && component.TagValue.ToString().Contains("excelfreezepanes")))) assignComponent = true;
                    }
                    if (assignComponent)
                    {
                        if (useCacheMode)
                        {
                            SetCellComponent(cell, baseComponent, indexComponent);
                        }
                        else
                        {
                            cell.Component = baseComponent;
                        }
                    }
                    #endregion

                    if (assignComponent && (cell.ExportImage != null))
                    {
                        if (useCacheMode)
                        {
                            string cellID = string.Format("{0}*{1}", (cell as StiCell2).PageId, (cell as StiCell2).ComponentId);
                            imagesBaseRect[cellID] = new RectangleD(rectD.X, rectD.Y + TotalHeight, rectD.Width, rectD.Height);
                        }
                        else
                        {
                            imagesBaseRect[component] = new RectangleD(rectD.X, rectD.Y + TotalHeight, rectD.Width, rectD.Height);
                        }
                    }

                    Cells[rectTop, rectLeft] = cell;
                    cell.Left = rectLeft;
                    cell.Top = rectTop;

                    IStiText text = component as IStiText;
                    if (text != null)
                    {
                        cell.Text = text.Text;
                        if (sText != null &&
                            (sText.TextQuality == StiTextQuality.Wysiwyg || sText.HorAlignment == StiTextHorAlignment.Width) &&
                            !string.IsNullOrEmpty(cell.Text) &&
                            cell.Text.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
                        {
                            cell.Text = cell.Text.Substring(0, cell.Text.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
                        }

                        if ((sText != null) && (sText.TextOptions != null) && (sText.TextOptions.LineLimit))
                        {
                            RectangleD rect2 = sText.ConvertTextMargins(rectD, false);
                            rect2 = sText.ConvertTextBorders(rect2, false);

                            var img = new Bitmap(1, 1);
                            var g = Graphics.FromImage(img);
                            string tempText = cell.Text;
                            string cutted = StiTextDrawing.CutLineLimit(
                                ref tempText,
                                g,
                                StiFontUtils.ChangeFontSize(sText.Font, sText.Font.Size * (float)StiDpiHelper.GraphicsScale), //textComp.Font,
                                rect2,
                                sText.TextOptions,
                                sText.TextQuality == StiTextQuality.Typographic,
                                StiDpiHelper.IsWindows);

                            cell.Text = cutted;
                        }
                    }
                    else cell.Text = string.Empty;

                    cell.Width = rect.Width - 1;
                    cell.Height = rect.Height - 1;

                    for (int indexX = rect.X; indexX < rectRight; indexX++)
                    {
                        for (int indexY = rect.Y; indexY < rectBottom; indexY++)
                        {
                            CellsMap[indexY, indexX] = cell;
                            if (haveBookmark) Bookmarks[indexY, indexX] = bookmark;
                            if (haveInteraction)
                            {
                                Interactions[indexY, indexX, 0] = interactionPageId;
                                Interactions[indexY, indexX, 1] = interactionComponentId;
                            }
                        }
                    }
                    #endregion

                    #region Check narrow
                    if (component is StiVerticalLinePrimitive)
                    {
                        if (coordXPrim[cell.Left] == 0) coordXPrim[cell.Left] = 1;
                    }
                    else
                    {
                        coordXPrim[cell.Left] = -1;
                    }
                    if (component is StiHorizontalLinePrimitive)
                    {
                        if (coordYPrim[cell.Top] == 0) coordYPrim[cell.Top] = 1;
                    }
                    else
                    {
                        coordYPrim[cell.Top] = -1;
                    }
                    #endregion
                }

                if (haveBorder)
                {
                    double borderSize = cBorder.Size;
                    StiBorderSide border = new StiBorderSide(cBorder.Color, borderSize, cBorder.Style);
                    StiAdvancedBorder advBorder = cBorder as StiAdvancedBorder;

                    StiRectanglePrimitive primitive = staticRectanglePrimitive;  //by default all sides are enabled
                    if (component is StiVerticalLinePrimitive) rect.X += 1;
                    if (component is StiHorizontalLinePrimitive) rect.Y += 1;
                    if (component is StiRectanglePrimitive)
                    {
                        rect.X += 1;
                        rect.Y += 1;
                        primitive = component as StiRectanglePrimitive;
                    }

                    rectLeft = rect.Left;
                    rectRight = rect.Right;
                    rectTop = rect.Top;
                    rectBottom = rect.Bottom;

                    #region Set borders
                    if (advBorder != null) border = advBorder.TopSide;
                    if (cBorder.IsTopBorderSidePresent && (border.Style != StiPenStyle.None) && primitive.TopSide)
                    {
                        for (int index = rectLeft; index < rectRight; index++)
                        {
                            if (BordersX[rectTop, index] != null)
                            {
                                if (BordersX[rectTop, index].Size <= borderSize)
                                {
                                    BordersX[rectTop, index] = border;
                                }
                            }
                            else
                            {
                                BordersX[rectTop, index] = border;
                            }
                        }
                    }
                    if (advBorder != null) border = advBorder.BottomSide;
                    if (cBorder.IsBottomBorderSidePresent && (border.Style != StiPenStyle.None) && primitive.BottomSide)
                    {
                        for (int index = rectLeft; index < rectRight; index++)
                        {
                            if (BordersX[rectBottom, index] != null)
                            {
                                if (BordersX[rectBottom, index].Size <= borderSize)
                                {
                                    BordersX[rectBottom, index] = border;
                                }
                            }
                            else
                            {
                                BordersX[rectBottom, index] = border;
                            }
                        }
                    }
                    if (advBorder != null) border = advBorder.LeftSide;
                    if (cBorder.IsLeftBorderSidePresent && (border.Style != StiPenStyle.None) && primitive.LeftSide)
                    {
                        for (int index = rectTop; index < rectBottom; index++)
                        {
                            if (BordersY[index, rectLeft] != null)
                            {
                                if (BordersY[index, rectLeft].Size <= borderSize)
                                {
                                    BordersY[index, rectLeft] = border;
                                }
                            }
                            else
                            {
                                BordersY[index, rectLeft] = border;
                            }
                        }
                    }
                    if (advBorder != null) border = advBorder.RightSide;
                    if (cBorder.IsRightBorderSidePresent && (border.Style != StiPenStyle.None) && primitive.RightSide)
                    {
                        for (int index = rectTop; index < rectBottom; index++)
                        {
                            if (BordersY[index, rectRight] != null)
                            {
                                if (BordersY[index, rectRight].Size <= borderSize)
                                {
                                    BordersY[index, rectRight] = border;
                                }
                            }
                            else
                            {
                                BordersY[index, rectRight] = border;
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private Rectangle GetCellRectangle(int startX, int startY, StiCell cell)
        {
            int rectLeft = startX;
            int rectTop = startY;
            int rectRight = startX;
            int rectBottom = startY;

            #region Detect left corner
            while (rectLeft <= cell.Width && CellsMap[rectTop + cell.Top, rectLeft + cell.Left] != cell)
            {
                rectLeft++;
            }

            if (rectLeft > cell.Width) return Rectangle.Empty;
            #endregion

            rectRight = rectLeft;

            #region Detect right corner
            while (rectRight <= cell.Width && CellsMap[rectTop + cell.Top, rectRight + cell.Left] == cell)
            {
                rectRight++;
            }
            if (rectLeft == rectRight) return Rectangle.Empty;
            #endregion

            #region Detect left bottom and right bottom corners
            bool fail = false;
            rectBottom = rectTop + 1;
            while (rectBottom <= cell.Height && fail == false)
            {
                if (rectLeft > 0 && CellsMap[cell.Top + rectBottom, cell.Left + rectLeft - 1] == cell)
                {
                    fail = true;
                    break;
                }

                if (rectRight <= cell.Width && CellsMap[rectBottom + cell.Top, cell.Left + rectRight] == cell)
                {
                    fail = true;
                    break;
                }

                for (int index = rectLeft; index < rectRight; index++)
                {
                    if (CellsMap[rectBottom + cell.Top, index + cell.Left] != cell)
                    {
                        fail = true;
                        break;
                    }
                }
                if (!fail) rectBottom++;
            }
            #endregion

            return new Rectangle(rectLeft, rectTop, rectRight - rectLeft, rectBottom - rectTop);
        }

        private void CutRectangleFromCellsMap(Rectangle cellRect, StiCell cell)
        {
            for (int mapX = cellRect.Left; mapX < cellRect.Right; mapX++)
            {
                for (int mapY = cellRect.Top; mapY < cellRect.Bottom; mapY++)
                {
                    CellsMap[cell.Top + mapY, cell.Left + mapX] = null;
                }
            }
        }

        internal bool IsComponentHasInteraction(StiComponent comp)
        {
            if (addComponentWithInteractions && comp.Interaction != null)
            {
                if (comp.Interaction.SortingEnabled && !string.IsNullOrWhiteSpace(comp.Interaction.SortingColumn)) return true;
                if (comp.Interaction.DrillDownEnabled && (!string.IsNullOrEmpty(comp.Interaction.DrillDownPageGuid) || !string.IsNullOrEmpty(comp.Interaction.DrillDownReport))) return true;
                if (comp.Interaction is StiBandInteraction && ((StiBandInteraction)comp.Interaction).CollapsingEnabled) return true;
            }
            return false;
        }
        
        #region ScanComponentsPlacement
        internal void ScanComponentsPlacement(bool optimize)
        {
            ScanComponentsPlacement(optimize, false);
        }
        internal void ScanComponentsPlacement(bool optimize, bool exportObjectFormatting)
        {
            //make array of values
            LinePlacement = new StiTableLineInfo[CoordY.Count];
            ParentBandName = new string[CoordY.Count];
            int indexCP = -1;
            for (int rowIndex = 0; rowIndex < CoordY.Count - 1; rowIndex++)
            {
                //linePlacement[rowIndex] = StiTableLineInfo.Unknown;
                //StiTableLineInfo lineInfo = StiTableLineInfo.Empty;
                StiTableLineInfo lineInfo = LinePlacement[rowIndex];
                StringBuilder parentName = new StringBuilder();
                bool flag = false;
                int skipCount = 0;
                for (int columnIndex = 1; columnIndex < CoordX.Count; columnIndex++)
                {
                    StiCell cell = Cells[rowIndex, columnIndex - 1];

                    if ((cell != null) && (!(cell.Component is Stimulsoft.Report.CrossTab.StiCrossColumnTotal)))
                    {
                        string cp = cell.Component.ComponentPlacement;
                        if (cp == null) cp = string.Empty;

                        int stPos = cp.IndexOf("Hd_HPnl");  //fix for Tables
                        if (stPos != -1)
                        {
                            cp = cp.Substring(0, stPos + 7);
                        }

                        if (!flag)
                        {
                            if (cp.StartsWith("d", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.Data;
                            else if (cp.StartsWith("h.ap", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderAP;
                            else if (cp.StartsWith("f.ap", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterAP;
                            else if (cp.StartsWith("h", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderD;
                            else if (cp.StartsWith("f", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterD;
                            else if (cp.StartsWith("gh", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderD;
                            else if (cp.StartsWith("gf", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterD;
                            else if (cp.StartsWith("ph", StringComparison.InvariantCulture))
                            {
                                lineInfo = StiTableLineInfo.PageHeader;
                                flag = true;
                            }
                            else if (cp.StartsWith("pf", StringComparison.InvariantCulture))
                            {
                                lineInfo = StiTableLineInfo.PageFooter;
                                flag = true;
                            }
                        }

                        if ((cp != string.Empty) && (lineInfo == StiTableLineInfo.Empty)) lineInfo = StiTableLineInfo.Unknown;

                        if (parentName.Length == 0) parentName.Append(cp + (char)0x1f);
                        if ((parentName.Length == 1) && (parentName[0] == (char)0x1f) && (cp != string.Empty)) parentName.Insert(0, cp);
                        if (((lineInfo == StiTableLineInfo.HeaderD) || (lineInfo == StiTableLineInfo.HeaderAP)) && !string.IsNullOrWhiteSpace(cell.Text)) parentName.Append(cell.Text);

                        int cellHeight = cell.Height;
                        if (!exportObjectFormatting) cellHeight = 0;
                        for (int indexHeight = 0; indexHeight <= cellHeight; indexHeight++)
                        {
                            if (rowIndex + indexHeight > indexCP)
                            {
                                LinePlacement[rowIndex + indexHeight] = lineInfo;
                                ParentBandName[rowIndex + indexHeight] = parentName.ToString();
                            }
                        }
                        if (cellHeight > skipCount) skipCount = cellHeight;
                    }
                }
                LinePlacement[rowIndex] = lineInfo;
                //rowIndex += skipCount;

                if (rowIndex + skipCount >= indexCP) indexCP = rowIndex + skipCount;
            }

            //optimize array for pageHeaders and pageFooters, header on all pages
            if (optimize)
            {
                //array must hold only one "header on all pages"
                Hashtable headerNames = new Hashtable();
                string lastParentBandName = null;
                for (int tempOffset = 0; tempOffset < CoordY.Count - 1; tempOffset++)
                {
                    if (LinePlacement[tempOffset] == StiTableLineInfo.HeaderAP)
                    {
                        //store header name
                        string headerName = ParentBandName[tempOffset];
                        int symPos = headerName.IndexOf('\x1f');
                        if (symPos != -1)
                        {
                            string currentParentBandName = headerName.Substring(0, symPos);
                            if (currentParentBandName != lastParentBandName)
                            {
                                lastParentBandName = currentParentBandName;
                                headerNames.Clear();
                            }
                        }
                        //check for repeated lines
                        if (headerNames.ContainsKey(headerName))
                        {
                            //cut this line
                            LinePlacement[tempOffset] = StiTableLineInfo.Trash;	//for cut

                            //additional check
                            string currName = GetParentBandName(tempOffset);
                            while ((tempOffset + 1 < CoordY.Count - 1) && (LinePlacement[tempOffset + 1] == StiTableLineInfo.HeaderAP) && (GetParentBandName(tempOffset + 1) == currName))
                            {
                                tempOffset++;
                                LinePlacement[tempOffset] = StiTableLineInfo.Trash;
                            }
                        }
                        else
                        {
                            headerNames.Add(headerName, headerName);

                            //additional check
                            string currName = GetParentBandName(tempOffset);
                            while ((tempOffset + 1 < CoordY.Count - 1) && (LinePlacement[tempOffset + 1] == StiTableLineInfo.HeaderAP) && (GetParentBandName(tempOffset + 1) == currName))
                            {
                                tempOffset++;
                            }
                        }
                    }
                }

                //array must hold only one "footer on all pages"
                Hashtable footerNames = new Hashtable();
                lastParentBandName = null;
                for (int tempOffset = CoordY.Count - 1 - 1; tempOffset >= 0; tempOffset--)
                {
                    if (LinePlacement[tempOffset] == StiTableLineInfo.FooterAP)
                    {
                        //store header name
                        string footerName = ParentBandName[tempOffset];
                        int symPos = footerName.IndexOf('\x1f');
                        if (symPos != -1)
                        {
                            string currentParentBandName = footerName.Substring(0, symPos);
                            if (currentParentBandName != lastParentBandName)
                            {
                                lastParentBandName = currentParentBandName;
                                footerNames.Clear();
                            }
                        }
                        //check for repeated lines
                        if (footerNames.ContainsKey(footerName))
                        {
                            //cut this line
                            LinePlacement[tempOffset] = StiTableLineInfo.Trash;	//for cut

                            //additional check
                            string currName = GetParentBandName(tempOffset);
                            while ((tempOffset - 1 >= 0) && (LinePlacement[tempOffset - 1] == StiTableLineInfo.FooterAP) && (GetParentBandName(tempOffset - 1) == currName))
                            {
                                tempOffset--;
                                LinePlacement[tempOffset] = StiTableLineInfo.Trash;
                            }
                        }
                        else
                        {
                            footerNames.Add(footerName, footerName);

                            //additional check
                            string currName = GetParentBandName(tempOffset);
                            while ((tempOffset - 1 >= 0) && (LinePlacement[tempOffset - 1] == StiTableLineInfo.FooterAP) && (GetParentBandName(tempOffset - 1) == currName))
                            {
                                tempOffset--;
                            }
                        }
                    }
                }


                //optimize array for pageHeaders and pageFooters
                for (int rowIndex = 0; rowIndex < CoordY.Count - 1; rowIndex++)
                {
                    if ((LinePlacement[rowIndex] == StiTableLineInfo.PageHeader) ||
                        (LinePlacement[rowIndex] == StiTableLineInfo.PageFooter) ||
                        (LinePlacement[rowIndex] == StiTableLineInfo.Trash))
                    {
                        int offset = 0;
                        while ((rowIndex + offset > 0) && (LinePlacement[rowIndex + offset - 1] == StiTableLineInfo.Empty))
                        {
                            offset--;
                            LinePlacement[rowIndex + offset] = LinePlacement[rowIndex];
                        }
                        offset = 0;
                        while ((rowIndex + offset < CoordY.Count - 1) && (LinePlacement[rowIndex + offset + 1] == StiTableLineInfo.Empty))
                        {
                            offset++;
                            LinePlacement[rowIndex + offset] = LinePlacement[rowIndex];
                        }
                    }
                }

            }
        }

        private string GetParentBandName(int rowIndex)
        {
            string st = ParentBandName[rowIndex];
            int symPos = st.IndexOf('\x1f');
            if (symPos == -1) return st;
            return st.Substring(0, symPos);
        }
        #endregion

        #region Process intersected cells
        private void ProcessIntersectedCells(ArrayList createdCells2)
        {
            if (createdCells2 == null) return;

            IList listX = CoordX.GetValueList();
            IList listY = CoordY.GetValueList();

            foreach (StiCell cell in createdCells2)
            {
                if (cell.Width != 0 || cell.Height != 0)
                {
                    int startX = 0;
                    int startY = 0;

                    Rectangle cellRect = GetCellRectangle(startX, startY, cell);

                    if (cellRect.Width == (cell.Width + 1) && cellRect.Height == (cell.Height + 1))
                    {
                        CutRectangleFromCellsMap(cellRect, cell);
                        continue;
                    }

                    ArrayList newCells = new ArrayList();

                    for (int y = 0; y <= cell.Height; y++)
                    {
                        if (coordYPrim[cell.Top + y] == 1) continue;

                        for (int x = 0; x <= cell.Width;)
                        {
                            if (coordXPrim[cell.Left + x] == 1)
                            {
                                x++;
                                continue;
                            }

                            cellRect = GetCellRectangle(x, y, cell);

                            if (cellRect.Width == 0)
                            {
                                x = cell.Width + 1;
                                continue;
                            }

                            CutRectangleFromCellsMap(cellRect, cell);

                            #region Create new cell
                            StiCell newCell = cell.Clone() as StiCell;
                            newCell.Left = cell.Left + cellRect.X;
                            newCell.Top = cell.Top + cellRect.Y;
                            newCell.Width = cellRect.Width - 1;
                            newCell.Height = cellRect.Height - 1;

                            Cells[newCell.Top, newCell.Left] = newCell;

                            newCells.Add(newCell);
                            #endregion

                            x += cellRect.Width;
                        }
                    }

                    #region Retransfer text and image
                    if (newCells.Count > 0)
                    {
                        double maxArea = 0;

                        StiCell selectedCell = null;
                        foreach (StiCell cell2 in newCells)
                        {
                            double cellWidth = (double)listX[cell2.Left + cell2.Width + 1] - (double)listX[cell2.Left];
                            double cellHeight = (double)listY[cell2.Top + cell2.Height + 1] - (double)listY[cell2.Top];

                            if (maxArea < cellWidth * cellHeight)
                            {
                                maxArea = cellWidth * cellHeight;
                                selectedCell = cell2;
                            }
                        }

                        foreach (StiCell cell3 in newCells)
                        {
                            if (cell3 != selectedCell)
                            {
                                cell3.Text = string.Empty;
                                cell3.ExportImage = null;
                            }
                        }
                    }
                    #endregion

                    #region Check borders
                    //if (newCells.Count > 0 && cell.CellStyle.Border != null)
                    //{
                    //    StiBorderSides side = cell.CellStyle.Border.Side;
                    //    bool dropShadow = cell.CellStyle.Border.DropShadow;

                    //    if (side != StiBorderSides.None || dropShadow)
                    //    {
                    //        foreach (StiCell cell4 in newCells)
                    //        {
                    //            StiCellStyle cellStyle = cell4.CellStyle.Clone() as StiCellStyle;
                    //            cellStyle.Border.Side = StiBorderSides.None;

                    //            if (cell4.Left == cell.Left && (side & StiBorderSides.Left) > 0)
                    //            {
                    //                cellStyle.Border.Side |= StiBorderSides.Left;
                    //            }

                    //            if (cell4.Top == cell.Top && (side & StiBorderSides.Top) > 0)
                    //            {
                    //                cellStyle.Border.Side |= StiBorderSides.Top;
                    //            }

                    //            if ((cell4.Left + cell4.Width) == (cell.Left + cell.Width) && 
                    //                (side & StiBorderSides.Right) > 0)
                    //            {
                    //                cellStyle.Border.Side |= StiBorderSides.Right;
                    //            }

                    //            if ((cell4.Top + cell4.Height) == (cell.Top + cell.Height) &&
                    //                (side & StiBorderSides.Bottom) > 0)
                    //            {
                    //                cellStyle.Border.Side |= StiBorderSides.Bottom;
                    //            }

                    //            cellStyle.Border.DropShadow = false;

                    //            cell4.CellStyle = GetStyle(cellStyle);
                    //        }
                    //    }
                    //}
                    #endregion

                }
            }
            createdCells2.Clear();
        }
        #endregion

        #region ForHtmlChromeBug
        private void GetMaxRectFromCell(StiCell cell, int y, int x, bool[,] readyCells)
        {
            int maxX = cell.Left + cell.Width + 1;
            if (y == cell.Top)
            {
                bool hasBorderTop = BordersX[y, x] != null;
                for (int index = x + 1; index < cell.Left + cell.Width + 1; index++)
                {
                    if ((BordersX[y, index] != null) != hasBorderTop)
                    {
                        maxX = index;
                        break;
                    }
                }
            }

            int maxY = cell.Top + cell.Height + 1;
            if (x == cell.Left)
            {
                bool hasBorderLeft = BordersY[y, x] != null;
                for (int index = y + 1; index < cell.Top + cell.Height + 1; index++)
                {
                    if ((BordersY[index, x] != null) != hasBorderLeft)
                    {
                        maxY = index;
                        break;
                    }
                }
            }

            if (maxY == cell.Top + cell.Height + 1)
            {
                bool hasBorderBottom = BordersX[maxY, x] != null;
                for (int index = x + 1; index < maxX; index++)
                {
                    if ((BordersX[maxY, index] != null) != hasBorderBottom)
                    {
                        maxX = index;
                        break;
                    }
                }
            }

            if (maxX == cell.Left + cell.Width + 1)
            {
                bool hasBorderRight = BordersY[y, maxX] != null;
                for (int index = y + 1; index < maxY; index++)
                {
                    if ((BordersY[index, maxX] != null) != hasBorderRight)
                    {
                        maxY = index;
                        break;
                    }
                }
            }

            #region range
            for (int yy = y; yy <= maxY - 1; yy++)
            {
                for (int xx = x; xx <= maxX - 1; xx++)
                {
                    readyCells[yy, xx] = true;
                }
            }
            #endregion

            if ((cell.Left == x) && (cell.Top == y) && (cell.Width == maxX - x - 1) && (cell.Height == maxY - y - 1))
            {
                return;
            }

            StiCell cell2 = cell.Clone() as StiCell;
            cell2.Left = x;
            cell2.Top = y;
            cell2.Width = maxX - x - 1;
            cell2.Height = maxY - y - 1;

            Cells[y, x] = cell2;
        }
        #endregion

        #endregion

        #region Methods.Export data

        #region Methods.Split Tag
        public string[] SplitTagWithCache(string inputString)
        {
            object obj = tagSplitCache[inputString];
            if (obj != null)
            {
                return (string[])obj;
            }

            string[] arr = SplitTag(inputString);

            tagSplitCache[inputString] = arr;

            return arr;
        }

        public static string[] SplitTag(string inputString)
        {
            string inString = inputString;
            if (!inString.EndsWith(";", StringComparison.InvariantCulture)) //add ";" for close last part of tag
            {
                inString = inString + ";";
            }
            ArrayList outString = new ArrayList();
            StringBuilder sb = new StringBuilder();
            int stPos = 0;
            bool posInString = false;
            while (stPos < inString.Length)
            {
                char sym = inString[stPos];
                stPos++;
                if (sym == '\"')
                {
                    if (posInString)
                    {
                        if (inString[stPos] == '\"')
                        {
                            sb.Append(sym);
                            stPos++;
                        }
                        else
                        {
                            posInString = !posInString;
                        }
                    }
                    else
                    {
                        posInString = !posInString;
                    }
                }
                if (((sym == ';') && (!posInString)) || (stPos >= inString.Length))
                {
                    string stTemp = sb.ToString().Trim();
                    if (stTemp.Length > 0)
                    {
                        outString.Add(stTemp);
                    }
                    sb = new StringBuilder();
                    continue;
                }
                sb.Append(sym);
            }
            string[] outStArray = new string[outString.Count];
            outString.CopyTo(0, outStArray, 0, outString.Count);
            return outStArray;
        }
        #endregion

        #region Methods.GetStringsFromTag
        public static string[] GetStringsFromTag(string tag, int startPosition)
        {
            ArrayList outString = new ArrayList();
            int pos = startPosition;
            while ((pos < tag.Length) && (tag[pos] != '"')) pos++;

            StringBuilder sb = new StringBuilder();
            bool posInString = false;
            while (pos < tag.Length)
            {
                char sym = tag[pos];
                pos++;
                if (sym == '\"')
                {
                    if (!posInString)	//new string
                    {
                        posInString = true;
                        continue;
                    }
                    else
                    {
                        if ((pos < tag.Length) && (tag[pos] == '\"'))
                        {
                            sb.Append(sym);
                            pos++;
                            continue;
                        }
                        outString.Add(sb.ToString());
                        sb = new StringBuilder();
                        posInString = false;
                        continue;
                    }
                }
                if (posInString)
                {
                    sb.Append(sym);
                }
            }
            if (sb.Length > 0)  //string not closed
            {
                outString.Add(sb.ToString());
            }
            string[] outStArray = new string[outString.Count];
            outString.CopyTo(0, outStArray, 0, outString.Count);
            return outStArray;
        }
        #endregion

        #region Methods.CopyFieldsListToFields()
        private void CopyFieldsListToFields()
        {
            Fields = new DataField[FieldsList.Count];
            for (int index = 0; index < FieldsList.Count; index++)
            {
                Fields[index] = (DataField)FieldsList[index];
            }
        }
        #endregion

        #region Class DataField
        public class DataField
        {
            public string Name;
            public int[] Info;
            public string FormatString;
            public string[] DataArray;
            public bool readyName;
            public bool readyType;

            public DataField(int size)
            {
                Name = string.Empty;
                Info = new int[3];
                FormatString = string.Empty;
                DataArray = new string[size];
                readyName = false;
                readyType = false;
            }
        }
        #endregion

        public DataField[] Fields;
        public int DataArrayLength;
        private ArrayList FieldsList;
        private int sizeX;
        private int sizeY;
        private Hashtable htName;

        public void PrepareDocument(StiExportService service, StiDataExportMode mode)
        {
            RichTextBox richtextForConvert = null;

            #region count lines
            sizeX = CoordX.Count - 1;
            sizeY = CoordY.Count - 1;
            int dataArraySizeY = 0;
            for (int index = 0; index < sizeY; index++)
            {
                if ((((mode & StiDataExportMode.Data) > 0) && (LinePlacement[index] == StiMatrix.StiTableLineInfo.Data)) ||
                    (((mode & StiDataExportMode.Headers) > 0) && (LinePlacement[index] == StiMatrix.StiTableLineInfo.HeaderD || LinePlacement[index] == StiMatrix.StiTableLineInfo.HeaderAP)) ||
                    (((mode & StiDataExportMode.Footers) > 0) && (LinePlacement[index] == StiMatrix.StiTableLineInfo.FooterD || LinePlacement[index] == StiMatrix.StiTableLineInfo.FooterAP)) ||
                     (mode == StiDataExportMode.AllBands))
                {
                    dataArraySizeY++;
                }
            }
            //DataArray = new string[dataArraySizeY, sizeX];
            FieldsList = new ArrayList();
            for (int index = 0; index < sizeX; index++)
            {
                FieldsList.Add(new DataField(dataArraySizeY));
            }
            CopyFieldsListToFields();
            #endregion

            #region prepare default names of fields
            for (int columnIndex = 0; columnIndex < sizeX; columnIndex++)
            {
                Fields[columnIndex].Name = "FIELD" + columnIndex.ToString("D4");
            }
            #endregion

            string exportName = string.Empty;
            bool useAliases = false;
            bool removeHtmlTags = true;
            switch (service.ExportFormat)
            {
                case StiExportFormat.Csv:
                    exportName = "csv";
                    useAliases = StiOptions.Export.Csv.UseAliases;
                    removeHtmlTags = StiOptions.Export.Csv.RemoveHtmlTags;
                    break;
                case StiExportFormat.Dbf:
                    exportName = "dbf";
                    useAliases = StiOptions.Export.Dbf.UseAliases;
                    removeHtmlTags = StiOptions.Export.Dbf.RemoveHtmlTags;
                    break;
                case StiExportFormat.Xml:
                    exportName = "xml";
                    useAliases = StiOptions.Export.Xml.UseAliases;
                    removeHtmlTags = StiOptions.Export.Xml.RemoveHtmlTags;
                    break;
                case StiExportFormat.Json:
                    exportName = "json";
                    useAliases = StiOptions.Export.Json.UseAliases;
                    removeHtmlTags = StiOptions.Export.Json.RemoveHtmlTags;
                    break;
            }

            #region Render document
            int dataArrayCounter = 0;
            htName = new Hashtable();

            for (int rowIndex = 0; rowIndex < sizeY; rowIndex++)
            {
                bool needWriteData = (((mode & StiDataExportMode.Data) > 0) && (LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.Data)) ||
                    (((mode & StiDataExportMode.Headers) > 0) && (LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderD || LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderAP)) ||
                    (((mode & StiDataExportMode.Footers) > 0) && (LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.FooterD || LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.FooterAP)) ||
                     (mode == StiDataExportMode.AllBands);
                if (needWriteData)
                {
                    for (int columnIndex = 0; columnIndex < sizeX; columnIndex++)
                    {
                        StiCell cell = Cells[rowIndex, columnIndex];

                        if (cell != null)
                        {
                            //process the Tag
                            string sTag = cell.Component.TagValue as string;
                            string[] sTagArray = null;
                            if (!string.IsNullOrEmpty(sTag))
                            {
                                sTagArray = SplitTagWithCache(sTag);
                            }

                            #region scan for additional columns
                            if (sTagArray != null)
                            {
                                for (int index = 0; index < sTagArray.Length; index++)
                                {
                                    if (sTagArray[index].ToLower().StartsWith("column", StringComparison.InvariantCulture))
                                    {
                                        string[] stArr = GetStringsFromTag(sTagArray[index], 6);
                                        if (stArr.Length > 1)
                                        {
                                            if (!htName.ContainsKey(stArr[0]))
                                            {
                                                int newColumn = Fields.Length;
                                                FieldsList.Add(new DataField(dataArraySizeY));
                                                CopyFieldsListToFields();
                                                Fields[newColumn].Name = stArr[0];
                                                Fields[newColumn].readyName = true;
                                                Fields[newColumn].readyType = true;
                                                htName[stArr[0]] = newColumn;
                                            }
                                            int column = (int)htName[stArr[0]];
                                            Fields[column].DataArray[dataArrayCounter] = stArr[1];
                                        }
                                    }
                                }
                            }
                            #endregion

                            if ((cell.Component != null) && ((cell.Component is StiSimpleText) || (cell.Component is StiCheckBox)))
                            {
                                #region scan column name
                                if (!Fields[columnIndex].readyName)
                                {
                                    string st = cell.Component.Name;
                                    if (useAliases && !string.IsNullOrEmpty(cell.Component.Alias))
                                    {
                                        st = cell.Component.Alias;
                                    }
                                    if (sTagArray != null)
                                    {
                                        #region check tag for export name and field name
                                        for (int tagIndex = 0; tagIndex < sTagArray.Length; tagIndex++)
                                        {
                                            string text = sTagArray[tagIndex].Trim();
                                            if ((text.ToLower().StartsWith(exportName, StringComparison.InvariantCulture)) ||
                                                (text.ToLower().StartsWith("default", StringComparison.InvariantCulture)))
                                            {
                                                int firstIndex = text.IndexOf("\"", StringComparison.InvariantCulture);
                                                if (firstIndex != -1)
                                                {
                                                    firstIndex++;
                                                    int stringLength = text.IndexOf("\"", firstIndex, StringComparison.InvariantCulture) - (firstIndex);
                                                    if (stringLength > 0)
                                                    {
                                                        st = text.Substring(firstIndex, stringLength);
                                                        if (!(text.ToLower().StartsWith("default", StringComparison.InvariantCulture)))
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw (new ArgumentException("Name of DataColumn not found in property: " + st));
                                                    }
                                                }
                                                else
                                                {
                                                    throw (new ArgumentException("Name of DataColumn not found in property: " + st));
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    if (service.ExportFormat == StiExportFormat.Dbf)
                                    {
                                        if (st.Length > 10) st = st.Substring(0, 10);
                                    }
                                    Fields[columnIndex].Name = st;
                                    Fields[columnIndex].readyName = true;
                                }
                                #endregion

                                #region scan column type
                                if ((!Fields[columnIndex].readyType) && (sTagArray != null))
                                {
                                    if (service.ExportFormat == StiExportFormat.Xml ||
                                        service.ExportFormat == StiExportFormat.Json ||
                                        service.ExportFormat == StiExportFormat.Dbf)
                                    {
                                        #region get data type
                                        string[] args = sTagArray[0].Split(":".ToCharArray(), 3);
                                        args[0] = args[0].Trim().ToLower();
                                        if (args[0].StartsWith("int", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Int;
                                            Fields[columnIndex].Info[1] = 15;
                                        }
                                        if (args[0].StartsWith("long", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Long;
                                            Fields[columnIndex].Info[1] = 25;
                                        }
                                        if (args[0].StartsWith("float", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Float;
                                            Fields[columnIndex].Info[1] = 15;
                                            Fields[columnIndex].Info[2] = 5;
                                        }
                                        if (args[0].StartsWith("double", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Double;
                                            Fields[columnIndex].Info[1] = 20;
                                            Fields[columnIndex].Info[2] = 10;
                                        }
                                        if (args[0].StartsWith("date", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Date;
                                            Fields[columnIndex].Info[1] = 8;
                                            Fields[columnIndex].Info[2] = 0;
                                        }
                                        if (args[0].StartsWith("bool", StringComparison.InvariantCulture))
                                        {
                                            Fields[columnIndex].Info[0] = (int)StiExportDataType.Bool;
                                            Fields[columnIndex].Info[1] = 1;
                                            Fields[columnIndex].Info[2] = 0;
                                        }
                                        //	if (args[0].StartsWith("string")) fieldsData[columnIndex, 0] = StiExportDataType.String;
                                        if (args.Length > 1)
                                        {
                                            args[1] = args[1].Trim();
                                            int param1 = 0;
                                            if (int.TryParse(args[1], out param1))
                                            {
                                                if (param1 > 0)
                                                {
                                                    Fields[columnIndex].Info[1] = param1;
                                                }
                                            }
                                            else
                                            {
                                                param1 = 0;
                                            }

                                            if (args.Length > 2)
                                            {
                                                args[2] = args[2].Trim();
                                                int param2 = 0;
                                                if (int.TryParse(args[2], out param2))
                                                {
                                                    if ((param2 > 0) && (param2 < param1))
                                                    {
                                                        Fields[columnIndex].Info[2] = param2;
                                                    }
                                                }
                                                else
                                                {
                                                    param2 = 0;
                                                }
                                            }
                                        }
                                        #endregion

                                        Fields[columnIndex].readyType = true;
                                    }
                                }
                                #endregion
                            }

                            #region prepare text
                            StiRichText rtf = cell.Component as StiRichText;
                            StiCheckBox mCheckbox = cell.Component as StiCheckBox;
                            StiText txt = cell.Component as StiText;

                            string str = cell.Text;
                            if (removeHtmlTags && (txt != null) && txt.AllowHtmlTags)
                            {
                                var baseState = new StiTextRenderer.StiHtmlState(string.Empty);
                                var listStates = StiTextRenderer.ParseHtmlToStates(str, baseState);
                                var sb = new StringBuilder();
                                foreach (var state in listStates)
                                {
                                    sb.Append(StiTextRenderer.PrepareStateText(state.Text));
                                }
                                str = sb.ToString();
                            }

                            if (!string.IsNullOrWhiteSpace(str))
                            {                                
                                if ((txt != null) && !string.IsNullOrWhiteSpace(txt.Format) && (txt.Format != "g") && (str.IndexOf('\xA0') != -1))
                                {
                                    str = str.Replace('\xA0', ' ');
                                }
                            }

                            if ((rtf != null) && (rtf.RtfText != string.Empty))
                            {
                                if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
                                rtf.GetPreparedText(richtextForConvert);
                                str = richtextForConvert.Text;
                            }
                            if (str == null) str = string.Empty;

                            if ((mCheckbox != null) && (mCheckbox.CheckedValue != null))
                            {
                                bool isTrue = false;
                                bool isFalse = false;
                                if (mCheckbox.CheckedValue is bool)	//for compiled reports
                                {
                                    if ((bool)mCheckbox.CheckedValue) isTrue = true; else isFalse = true;
                                }
                                if (mCheckbox.CheckedValue is string)	//for reports loaded from mdc-files
                                {
                                    if ((string)mCheckbox.CheckedValue == "True") isTrue = true; else isFalse = true;
                                }
                                if (isTrue) str = StiOptions.Export.CheckBoxTextForTrue;
                                if (isFalse) str = StiOptions.Export.CheckBoxTextForFalse;
                            }

                            if (!string.IsNullOrEmpty(str))
                            {
                                if (service.ExportFormat == StiExportFormat.Csv && StiOptions.Export.Csv.UseMultilineText ||
                                    service.ExportFormat == StiExportFormat.Xml ||
                                    service.ExportFormat == StiExportFormat.Json)
                                {
                                    //str = str.Replace("\n", "");
                                    str = str.Replace("\r", "");
                                }
                                else
                                {
                                    str = str.Replace("\n", " ");
                                    str = str.Replace("\r", "");
                                }
                                if (service.ExportFormat == StiExportFormat.Dbf)
                                {
                                    if (str.Length > 254) str = str.Substring(0, 254);
                                }
                            }
                            #endregion

                            Fields[columnIndex].DataArray[dataArrayCounter] = str;
                        }
                        else
                        {
                            Fields[columnIndex].DataArray[dataArrayCounter] = string.Empty;
                        }
                    }
                    dataArrayCounter++;
                }
            }
            #endregion

            if (richtextForConvert != null) richtextForConvert.Dispose();

            #region optimize fields
            for (int columnIndex = FieldsList.Count - 1; columnIndex >= 0; columnIndex--)
            {
                //check column existance
                if (!Fields[columnIndex].readyName)
                {
                    FieldsList.RemoveAt(columnIndex);
                }
            }
            CopyFieldsListToFields();
            #endregion

            DataArrayLength = dataArraySizeY;
        }

        public void CheckStylesNames()
        {
            //check styles names for duplicates
            Hashtable hs = new Hashtable();
            for (int indexStyle = 0; indexStyle < Styles.Count; indexStyle++)
            {
                StiCellStyle style = (StiCellStyle)Styles[indexStyle];
                string st = style.StyleName;
                string stNum = string.Empty;
                int num = 0;
                while (true)
                {
                    if (!hs.Contains(st + stNum)) break;
                    num++;
                    stNum = num.ToString();
                }
                if (stNum != string.Empty)
                {
                    st += stNum;
                    style.StyleName = st;
                }
                hs.Add(st, st);
            }
        }

        public Image GetRealImageData(StiCell cell, Image baseImage)
        {
            if (cell == null || cell.Component == null || baseImage == null) return null;

            object obj = null;
            if (useCacheMode)
            {
                StiCell2 cell2 = cell as StiCell2;
                if (cell2 == null) return null;
                string cellID = string.Format("{0}*{1}", cell2.PageId, cell2.ComponentId);
                obj = imagesBaseRect[cellID];
            }
            else
            {
                obj = imagesBaseRect[cell.Component];
            }
            if (obj == null || !(obj is RectangleD)) return null;
            RectangleD rectD = (RectangleD)obj;

            double scaleX = (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization) ? htmlScaleX : 1;
            double scaleY = (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization) ? htmlScaleY : 1;

            double ll = Round(rectD.Left * scaleX);
            double tt = Round(rectD.Top * scaleY);
            double rr = Round(rectD.Right * scaleX);
            double bb = Round(rectD.Bottom * scaleY);

            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;

            if (leftCached[ll] == null)
            {
                left = CoordX.IndexOfValue(ll);
                leftCached[ll] = left;
            }
            else left = (int)leftCached[ll];

            if (topCached[tt] == null)
            {
                top = CoordY.IndexOfValue(tt);
                topCached[tt] = top;
            }
            else top = (int)topCached[tt];

            //if (rightCached[rr] == null)
            if (leftCached[rr] == null)
            {
                right = CoordX.IndexOfValue(rr);
                //rightCached[rr] = right;
                leftCached[rr] = right;
            }
            //else right = (int)rightCached[rr];
            else right = (int)leftCached[rr];


            //if (bottomCached[bb] == null)
            if (topCached[bb] == null)
            {
                bottom = CoordY.IndexOfValue(bb);
                //bottomCached[bb] = bottom;
                topCached[bb] = bottom;
            }
            //else bottom = (int)bottomCached[bb];
            else bottom = (int)topCached[bb];

            if (left == -1 || right == -1 || top == -1 || bottom == -1) return null;

            if (left == cell.Left && right == cell.Left + cell.Width + 1 && top == cell.Top && bottom == cell.Top + cell.Height + 1) return null; //full image

            //get real coordinates of cell
            int ll2 = (int)(double)CoordX.GetByIndex(cell.Left);
            int tt2 = (int)(double)CoordY.GetByIndex(cell.Top);
            int rr2 = (int)(double)CoordX.GetByIndex(cell.Left + cell.Width + 1);
            int bbc = cell.Top + cell.Height + 1;
            if (bbc > (CoordY.Count - 1)) bbc = CoordY.Count - 1;
            int bb2 = (int)(double)CoordY.GetByIndex(bbc);

            //get percents
            double ppl = (ll2 - ll) / (rr - ll);
            double ppr = (rr2 - ll) / (rr - ll);
            double ppt = (tt2 - tt) / (bb - tt);
            double ppb = (bb2 - tt) / (bb - tt);

            //get coordinates in base image
            int nl = (int)(baseImage.Width * ppl);
            int nr = (int)Math.Round(baseImage.Width * ppr + 0.45);
            int nt = (int)(baseImage.Height * ppt);
            int nb = (int)Math.Round(baseImage.Height * ppb + 0.45);

            Bitmap bmp = new Bitmap(nr - nl, nb - nt);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.PageUnit = GraphicsUnit.Pixel;
                g.Clear(Color.FromArgb(1, 255, 255, 255));
                g.DrawImage(baseImage, 0, 0, new Rectangle(nl, nt, nr - nl, nb - nt), GraphicsUnit.Pixel);
            }

            return bmp;
        }

        private bool CheckComponentPlacement(StiComponent component, StiDataExportMode dataMode)
        {
            if (!StiOptions.Export.OptimizeDataOnlyMode) return true;
            if (dataMode == StiDataExportMode.AllBands) return true;

            string cp = component.ComponentPlacement;
            if (cp == null) cp = string.Empty;
            int stPos = cp.IndexOf("Hd_HPnl", StringComparison.InvariantCulture);  //fix for Tables
            if (stPos != -1)
            {
                cp = cp.Substring(0, stPos + 7);
            }
            var lineInfo = StiTableLineInfo.Unknown;
            if (cp.StartsWith("d", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.Data;
            else if (cp.StartsWith("h.ap", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderD;
            else if (cp.StartsWith("f.ap", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterD;
            else if (cp.StartsWith("h", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderD;
            else if (cp.StartsWith("f", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterD;
            else if (cp.StartsWith("gh", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.HeaderD;
            else if (cp.StartsWith("gf", StringComparison.InvariantCulture)) lineInfo = StiTableLineInfo.FooterD;

            if ((((dataMode & StiDataExportMode.Data) > 0) && (lineInfo == StiMatrix.StiTableLineInfo.Data)) ||
                (((dataMode & StiDataExportMode.Headers) > 0) && (lineInfo == StiMatrix.StiTableLineInfo.HeaderD)) ||
                (((dataMode & StiDataExportMode.Footers) > 0) && (lineInfo == StiMatrix.StiTableLineInfo.FooterD)))
            {
                return true;
            }

            return false;
        }

        #region Methods.Replace component for export
        internal static void ReplaceComponentForExport(ref StiComponent component, bool replaceCheckboxes)
        {
            if (replaceCheckboxes)
            {
                #region Replace checkbox
                var checkbox = component as StiCheckBox;
                bool needReplaceCheckbox = checkbox != null;
                if (needReplaceCheckbox)
                {
                    if (!((checkbox.TextBrush is StiSolidBrush) && (StiBrush.ToColor(checkbox.TextBrush).Equals(checkbox.ContourColor)))) needReplaceCheckbox = false;
                }
                if (needReplaceCheckbox)
                {
                    var tempComp = new StiText();
                    tempComp.Page = checkbox.Page;
                    tempComp.ClientRectangle = checkbox.ClientRectangle;
                    tempComp.Brush = checkbox.Brush;
                    tempComp.TextBrush = checkbox.TextBrush;
                    tempComp.Border = checkbox.Border;
                    tempComp.HorAlignment = StiTextHorAlignment.Center;
                    tempComp.VertAlignment = StiVertAlignment.Center;
                    var textSize = component.Report.Unit.ConvertToHInches(Math.Min(checkbox.Width, checkbox.Height));

                    if (checkbox.CheckedValue != null)
                    {
                        bool checkBoxValue = false;
                        string checkedValueStr = checkbox.CheckedValue.ToString().Trim().ToLower(CultureInfo.InvariantCulture);
                        string[] strs = checkbox.Values.Split(new char[] { '/', ';', ',' });
                        if (strs != null && strs.Length > 0)
                        {
                            string firstValue = strs[0].Trim().ToLower(CultureInfo.InvariantCulture);
                            checkBoxValue = checkedValueStr == firstValue;
                        }

                        char value = ' ';
                        string selectedFontFamily = null;

                        #region Set style
                        switch (checkBoxValue ? checkbox.CheckStyleForTrue : checkbox.CheckStyleForFalse)
                        {
                            case StiCheckStyle.Cross:
                                value = (char)0xf0fb;  //251
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.Check:
                                value = (char)0xf0fc;  //252
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.CrossRectangle:
                                value = (char)0xf0fd;  //253
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.CheckRectangle:
                                value = (char)0xf0fe;  //254
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.CrossCircle:
                                value = (char)0xf056;   //86
                                selectedFontFamily = "Wingdings 2";
                                break;

                            case StiCheckStyle.DotCircle:
                                value = (char)0xf0a4;  //164
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.DotRectangle:
                                value = (char)0xf0a9;  //169
                                selectedFontFamily = "Wingdings 2";
                                break;

                            case StiCheckStyle.NoneCircle:
                                value = (char)0xf0a1;  //161
                                selectedFontFamily = "Wingdings";
                                break;

                            case StiCheckStyle.NoneRectangle:
                                value = (char)0xf0a8;  //168
                                selectedFontFamily = "Wingdings";
                                break;
                        }
                        #endregion

                        if (value != ' ')
                        {
                            tempComp.Font = StiFontCollection.CreateFont(selectedFontFamily, (float)(textSize * 0.72), FontStyle.Regular);
                            tempComp.Text = value.ToString();
                        }
                    }
                    component = tempComp;
                }
                #endregion
            }
        }
        #endregion

        #endregion

        #region Matrix.Cache
        private StiPage lastPage;
        private int lastPageId = -1;
        private StiComponentsCollection lastComps;

        public StiComponent GetCellComponent(StiCell cell2)
        {
            StiCell2 cell = cell2 as StiCell2;
            if ((cell == null) || (cell.ComponentId == -1)) return null;
            if (cell.ComponentId == -2) return additionalInfo;
            if (cell.ComponentId == -3) return paginationContainerInfo;
            if (lastPageId != cell.PageId)
            {
                lastPageId = cell.PageId;
                lastPage = pages[cell.PageId];
                pages.GetPage(lastPage);
                if (lastComps != null) lastComps.Clear();
                lastComps = lastPage.GetComponents();
            }
            var comp = lastComps[cell.ComponentId];
            ReplaceComponentForExport(ref comp, replaceCheckboxes);
            return comp;
        }

        public void SetCellComponent(StiCell cell2, StiComponent component, int indexComponent)
        {
            StiCell2 cell = cell2 as StiCell2;
            if (cell == null) return;
            if ((additionalInfo != null) && (component == additionalInfo))
            {
                cell.ComponentId = -2;
                return;
            }
            if ((paginationContainerInfo != null) && (component == paginationContainerInfo))
            {
                cell.ComponentId = -3;
                return;
            }
            if (lastPage != component.Page)
            {
                lastPage = component.Page;
                lastPageId = pages.IndexOf(lastPage);
                if (lastComps != null) lastComps.Clear();
                lastComps = lastPage.GetComponents();
            }
            cell.PageId = lastPageId;
            if (indexComponent >= 0)
                cell.ComponentId = indexComponent;
            else
                cell.ComponentId = lastComps.IndexOf(component);
        }

        public int GetBorderSideIndex(StiBorderSide side)
        {
            if (side == null) return 0;
            if (BorderSides.Count > 0)
            {
                for (int index = 0; index < borderSides.Count; index++)
                {
                    if (borderSides[index].Equals(side))
                    {
                        return index + 1;
                    }
                }
            }
            borderSides.Add(side);
            return borderSides.Count;
        }

        private void ClearCellsMapArea(double y1, double y2)
        {
            RectangleD rectYY = new RectangleD();
            rectYY.Y = y1;
            rectYY.Height = y2 - y1;
            rectYY.X = (double)CoordX.GetByIndex(0);
            rectYY.Width = 0;

            double storedTotal = TotalHeight;
            TotalHeight = 0;
            Rectangle yy = GetRange(rectYY);
            TotalHeight = storedTotal;

            for (int indexY = yy.Y; indexY < yy.Y + yy.Height; indexY++)
            {
                for (int indexX = 0; indexX < CoordX.Count; indexX++)
                {
                    CellsMap[indexY, indexX] = null;
                }
            }
        }

        internal static void GCCollect()
        {
            if (StiOptions.Engine.ReportCache.AllowGCCollect)
            {
                GC.Collect();

                if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                    GC.WaitForPendingFinalizers();
            }
        }

        internal void AllowModification(bool flag)
        {
            cacheManager.flagForceSaveSegment = flag;
        }
        #endregion

        #region Methods.Clear
        public void Clear()
        {
            cells2 = null;
            bordersX2 = null;
            bordersY2 = null;
            bookmarks2 = null;
            cellStyles2 = null;

            CellsMap = null;
            Styles = null;
            CoordX = null;
            CoordY = null;
            LinePlacement = null;
            ParentBandName = null;
            HorizontalPageBreaks = null;

            xcHash.Clear();
            xcHash = null;
            ycHash.Clear();
            ycHash = null;

            coordXCheck = null;
            coordYCheck = null;
            coordXNew = null;
            coordYNew = null;
            coordXPrim = null;
            coordYPrim = null;

            leftCached.Clear();
            leftCached = null;
            topCached.Clear();
            topCached = null;

            tagSplitCache.Clear();
            tagSplitCache = null;

            stylesCache.Clear();
            stylesCache = null;
            fontsCache.Clear();
            fontsCache = null;

            Fields = null;
            FieldsList = null;
            htName = null;

            report = null;
            pages = null;
            lastPage = null;
            if (lastComps != null) lastComps.Clear();
            if (borderSides != null) borderSides.Clear();
            borderSides = null;
            if (cacheManager != null) cacheManager.Clear();
        }
        #endregion

        #region this
        public StiMatrix(StiPagesCollection pages, StiExportService service) : this(pages, false, service, null)
        {
        }


        public StiMatrix(StiPagesCollection pages, bool checkForExcel, StiExportService service) :
            this(pages, checkForExcel, service, null)
        {
        }


        public StiMatrix(StiPagesCollection pages, StiExportService service, ArrayList styles)
            : this(pages, false, service, styles)
        {
        }


        public StiMatrix(StiPagesCollection pages, bool checkForExcel, StiExportService service, ArrayList styles, StiDataExportMode dataMode = StiDataExportMode.AllBands, bool hasDividedPages = false)
        {
            this.report = pages.Report;
            this.pages = pages;
            this.exportFormat = service.ExportFormat;
            bool exportData =
                exportFormat == StiExportFormat.Dbf || exportFormat == StiExportFormat.Csv ||
                exportFormat == StiExportFormat.Xml || exportFormat == StiExportFormat.Json;
            this.isHtmlService = service is StiHtmlExportService;
            this.isHtmlOrExcelXmlService = isHtmlService || (service is StiExcelXmlExportService);
            //this.isHtmlPngMode = (service is StiHtmlExportService) && ((service as StiHtmlExportService).imageFormat == ImageFormat.Png || (service as StiHtmlExportService).imageFormat == null);
            this.isPaginationMode = (service is StiWord2007ExportService) ||
                (service is StiRtfExportService) ||
                (service is StiOdtExportService) ||
                (service is StiTxtExportService && (service as StiTxtExportService).putFeedPageCode);

            if (service is StiHtmlExportService)
            {
                addComponentWithInteractions = ((StiHtmlExportService)service).RenderWebInteractions;
                useFullExceedMargins = ((StiHtmlExportService)service).useFullExceedMargins;
            }

            if (service is StiExcel2007ExportService || service is StiExcelExportService) replaceCheckboxes = !StiOptions.Export.Excel.RenderCheckBoxAsImage;
            if (service is StiWord2007ExportService) replaceCheckboxes = !StiOptions.Export.Word.RenderCheckBoxAsImage;

            int tempStylesCount = -1;
            if (styles != null)
            {
                this.Styles = styles;
                tempStylesCount = styles.Count;
            }

            double[] totalHeightPage = new double[pages.Count + 1];     //normal
            double[] totalHeightPage2 = new double[pages.Count + 1];    //scaled and rounded 
            int totalHeightPageCounter = 0;
            double totalHeight2 = 0;
            maxCoordY = 0;

            bool isPrinting = (pages.Report != null ? pages.Report.IsPrinting : false);

            defaultLinePrimitiveWidth = _defaultLinePrimitiveWidth / ((isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization) ? htmlScaleY : 1);

            #region Calculating Coordinates
            service.StatusString = StiLocalization.Get("Export", "ExportingCalculatingCoordinates");

            bool removeEmptySpaceAtBottom = true;
            if (service is StiRtfExportService) removeEmptySpaceAtBottom = (service as StiRtfExportService).RemoveEmptySpaceAtBottom;
            if (service is StiWord2007ExportService) removeEmptySpaceAtBottom = (service as StiWord2007ExportService).RemoveEmptySpaceAtBottom;
            if (service is StiOdtExportService) removeEmptySpaceAtBottom = (service as StiOdtExportService).RemoveEmptySpaceAtBottom;
            if (service is StiOdsExportService) removeEmptySpaceAtBottom = StiOptions.Export.OpenDocumentCalc.RemoveEmptySpaceAtBottom;
            if (service is StiExcelExportService) removeEmptySpaceAtBottom = StiOptions.Export.Excel.RemoveEmptySpaceAtBottom;
            if (service is StiExcel2007ExportService) removeEmptySpaceAtBottom = StiOptions.Export.Excel.RemoveEmptySpaceAtBottom;
            if (service is StiHtmlExportService) removeEmptySpaceAtBottom = (service as StiHtmlExportService).RemoveEmptySpaceAtBottom;

            hyperlinksToTag = new Hashtable();
            pointerToBookmark = new Hashtable();
            pointerToTag = new Hashtable();
            int pageIndex = 0;
            int maxLinesCount = 0;
            foreach (StiPage page in pages)
            {
                pages.GetPage(page);

                service.InvokeExporting(page, pages, service.CurrentPassNumber, service.MaximumPassNumber);
                if (service.IsStopped) return;

                int lastLinesCount = CoordY.Count;

                totalHeightPage[totalHeightPageCounter] = TotalHeight;
                totalHeightPage2[totalHeightPageCounter] = totalHeight2;
                totalHeightPageCounter++;

                //AddCoord(page.Unit.ConvertToHInches(page.ClientRectangle));
                RectangleD pageRect = page.Unit.ConvertToHInches(page.ClientRectangle);
                var marginLeft = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Left) : 0;
                var marginRight = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Right) : 0;
                var marginTop = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Top) : 0;
                var marginBottom = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Bottom) * 0.5 : 0;
                pageRect = new RectangleD(pageRect.X - marginLeft, pageRect.Y, pageRect.Width + marginLeft + marginRight, pageRect.Height + marginTop + marginBottom);
                var pageRectBase = pageRect;

                if ((page.Border != null) && (page.Border.Side != StiBorderSides.None))
                {
                    StiContainer borderComponent = new StiContainer();
                    borderComponent.DisplayRectangle = page.ClientRectangle;
                    AddCoord(page.Unit.ConvertToHInches(borderComponent.DisplayRectangle));
                }

                if (removeEmptySpaceAtBottom && !(service is StiHtmlExportService))
                {
                    pageRect.Width = 0;
                }
                if (removeEmptySpaceAtBottom || ((pageIndex == pages.Count - 1) && !(service is StiHtmlExportService)))
                {
                    pageRect.Height = 0;
                }
                AddCoord(pageRect);

                //pageRect = page.Unit.ConvertToHInches(page.ClientRectangle);
                pageRect = pageRectBase;
                TotalHeight += marginTop;

                foreach (StiComponent component in page.Components)
                {
                    if (component.Enabled && !(isPrinting && !component.Printable))
                    {
                        RectangleD rect = page.Unit.ConvertToHInches(component.DisplayRectangle);
                        if (rect.Height < 0)
                        {
                            rect.Y += rect.Height;
                            rect.Height = Math.Abs(rect.Height);
                        }
                        bool needAdd = true;
                        if (component is StiPointPrimitive) needAdd = false;
                        if ((rect.Right < pageRect.Left) ||
                            (rect.Left > pageRect.Right) ||
                            (rect.Bottom < pageRect.Top) ||
                            (rect.Top > pageRect.Bottom)) needAdd = false;

                        if (StiOptions.Configuration.IsWeb && StiOptions.Engine.AllowInteractionInChartWithComponents && component.Name.EndsWith("Interaction#FX%")) needAdd = false;
                        if ((component is StiContainer) && !string.IsNullOrWhiteSpace(component.Name) && component.Name.StartsWith("TAG##")) needAdd = false;

                        if (!CheckComponentPlacement(component, dataMode)) needAdd = false;

                        if (needAdd)
                        {
                            if (component is StiLinePrimitive)
                            {
                                if (component is StiRectanglePrimitive)
                                {
                                    AddCoord(rect);
                                    rect.Y -= defaultLinePrimitiveWidth;
                                    rect.X -= defaultLinePrimitiveWidth;
                                }
                                else
                                {
                                    if (rect.Height < 1.5)
                                    {
                                        rect.Height = defaultLinePrimitiveWidth;
                                        rect.Y -= rect.Height;
                                    }
                                    if (rect.Width < 1.5)
                                    {
                                        rect.Width = defaultLinePrimitiveWidth;
                                        rect.X -= rect.Width;
                                    }
                                }
                            }
                            else
                            {
                                if (rect.Height < 1.5 && rect.Height > 0)
                                {
                                    if (GetCoordYRounded(rect.Top) == GetCoordYRounded(rect.Bottom))
                                    {
                                        rect.Height = 1d / GetCoordYRounded(1) + 0.01;
                                    }
                                }
                                if (rect.Width < 1.5 && rect.Width > 0)
                                {
                                    if (GetCoordXRounded(rect.Left) == GetCoordXRounded(rect.Right))
                                    {
                                        rect.Width = 1d / GetCoordXRounded(1) + 0.01;
                                    }
                                }
                            }
                            AddCoord(rect);

                            #region ExceedMargins
                            StiText stiText2 = component as StiText;
                            if ((stiText2 != null) && (stiText2.ExceedMargins != StiExceedMargins.None) && !(stiText2.Brush is StiEmptyBrush))
                            {
                                rect = page.Unit.ConvertToHInches(component.DisplayRectangle);
                                if ((stiText2.ExceedMargins & StiExceedMargins.Left) > 0)
                                {
                                    rect.Width += rect.Left + marginLeft;
                                    rect.X = pageRectBase.Left;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Top) > 0)
                                {
                                    rect.Height += rect.Top + marginTop;
                                    rect.Y = pageRectBase.Top - marginTop;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Right) > 0)
                                {
                                    rect.Width += pageRectBase.Right - rect.Right;
                                }
                                if ((stiText2.ExceedMargins & StiExceedMargins.Bottom) > 0)
                                {
                                    rect.Height += pageRectBase.Height - marginTop - rect.Bottom;
                                }
                                AddCoord(rect);
                            }
                            #endregion

                            if (component.HyperlinkValue != null)
                            {
                                string hyperlink = component.HyperlinkValue as string;
                                if (!string.IsNullOrEmpty(hyperlink) && (hyperlink.Length > 2) && hyperlink.StartsWith("##"))
                                {
                                    if (hyperlink[2] != '#')
                                        hyperlinksToTag[hyperlink.Substring(2)] = null;
                                }
                            }

                            if ((component.PointerValue != null) && !string.IsNullOrWhiteSpace((string)component.PointerValue) && !string.IsNullOrEmpty(component.Guid))
                            {
                                string pointerValue = $"{component.PointerValue}#GUID#{component.Guid}";

                                string bookmarkValue = component.BookmarkValue as string;
                                if (!string.IsNullOrWhiteSpace(bookmarkValue))
                                {
                                    pointerToBookmark[pointerValue] = bookmarkValue;
                                }

                                string tag = component.TagValue as string;
                                if ((bookmarkValue == null) && !string.IsNullOrEmpty(tag))
                                {
                                    pointerToTag[pointerValue] = tag;
                                }
                            }
                        }
                    }
                }
                TotalHeight -= marginTop;
                if (hasDividedPages && (maxCoordY > TotalHeight + pageRect.Height))
                {
                    maxCoordY = TotalHeight + pageRect.Height;
                    AddCoord(0, maxCoordY);
                }
                if (isPaginationMode && !(pageIndex == pages.Count - 1))
                {
                    AddCoord(0, maxCoordY + paginationModeOffset);
                }
                TotalHeight = Round(maxCoordY);
                totalHeight2 = Round(maxCoordY * (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization ? htmlScaleY : 1));
                TotalWidth = Math.Max(TotalWidth, pageRectBase.Width * (isHtmlService && StiOptions.Export.Html.PrintLayoutOptimization ? htmlScaleX : 1));
                pageIndex++;

                if ((CoordY.Count - lastLinesCount) > maxLinesCount) maxLinesCount = CoordY.Count - lastLinesCount;
            }
            totalHeightPage[totalHeightPageCounter] = TotalHeight;

            //add only existing tags to bookmark list
            foreach (DictionaryEntry de in pointerToTag)
            {
                if (hyperlinksToTag.Contains(de.Value))
                {
                    pointerToBookmark[de.Key] = de.Value;
                }
            }
            pointerToTag.Clear();

            #region Trial
#if CLOUD
            var isTrial = !StiCloudPlan.IsReportsAvailable(report != null ? report.ReportGuid : null);
#elif SERVER
            var isTrial = StiVersionX.IsSvr;
#else
            var key = StiLicenseKeyValidator.GetLicenseKey();

            var isValidInDesigner = StiLicenseKeyValidator.IsValidInReportsDesignerOrOnPlatform(StiProductIdent.Net, key) && Base.Design.StiDesignerAppStatus.IsRunning;
            var isTrial = !(StiLicenseKeyValidator.IsValidOnNetFramework(key) || isValidInDesigner);

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

            bool putAdditionalInformation = false;
            if (isTrial)
            {
                StiPage tempPage = pages.Count > 0 ? pages[0] : new StiPage(report);
                RectangleD tempRect = new RectangleD(0, tempPage.Unit.ConvertFromHInches(-20d), tempPage.Width, tempPage.Unit.ConvertFromHInches(20d));

                additionalInfo = new StiText(tempRect, "Stimulsoft Reports - Trial");
                additionalInfo.HorAlignment = StiTextHorAlignment.Center;
                additionalInfo.VertAlignment = StiVertAlignment.Center;
                additionalInfo.Font = new Font("Arial", 12f, FontStyle.Bold);
                additionalInfo.TextBrush = new StiSolidBrush(Color.Red);
                additionalInfo.Brush = new StiSolidBrush(Color.White);
                additionalInfo.Border = new StiBorder(StiBorderSides.All, Color.Black, 1d, StiPenStyle.Solid);
                additionalInfo.Page = tempPage;

                putAdditionalInformation = (pages.Count > 0) &&
                                                ((exportFormat == StiExportFormat.Excel) || (exportFormat == StiExportFormat.ExcelXml) ||
                                                 (exportFormat == StiExportFormat.Odt) || (exportFormat == StiExportFormat.Ods) ||
                                                 (exportFormat == StiExportFormat.Rtf) || (exportFormat == StiExportFormat.RtfTable));
                if (putAdditionalInformation)
                {
                    RectangleD tempRect2 = tempPage.Unit.ConvertToHInches(additionalInfo.DisplayRectangle);
                    AddCoord(tempRect2.Left, tempRect2.Top);
                    AddCoord(tempRect2.Right, tempRect2.Bottom);
                }
            }
            #endregion

            #endregion

            if (checkForExcel) PrepareTable();

            //additional line for fix problem with width of merged cells in Html
            if (isHtmlOrExcelXmlService) AddCoord(0, totalHeight2 + 1, false);


            //speed optimization 15.01.2008
            for (int indexY = 0; indexY < CoordY.Count; indexY++)
            {
                topCached[CoordY.GetByIndex(indexY)] = indexY;
            }
            for (int indexX = 0; indexX < CoordX.Count; indexX++)
            {
                leftCached[CoordX.GetByIndex(indexX)] = indexX;
            }

            //memory optimize for big reports
            xcHash.Clear();
            ycHash.Clear();


            useCacheMode = StiOptions.Export.UseCacheModeForStiMatrix && pages.CacheMode;

            cacheManager = new StiMatrixCacheManager(this, CoordX.Count, CoordY.Count, maxLinesCount);

            #region Prepare arrays
            Cells = new StiMatrixCellsCollection(cacheManager, this);
            Bookmarks = new StiMatrixBookmarksCollection(cacheManager, this);
            BordersX = new StiMatrixBorderSidesXCollection(cacheManager, this);
            BordersY = new StiMatrixBorderSidesYCollection(cacheManager, this);
            CellStyles = new StiMatrixCellStylesCollection(cacheManager, this);

            if (!useCacheMode)
            {
                cells2 = new StiCell[CoordY.Count, CoordX.Count];
                bordersX2 = new StiBorderSide[CoordY.Count, CoordX.Count];
                bordersY2 = new StiBorderSide[CoordY.Count, CoordX.Count];
                bookmarks2 = new string[CoordY.Count, CoordX.Count];
            }

            CellsMap = new StiCell[CoordY.Count, CoordX.Count];
            coordXCheck = new bool[CoordX.Count];
            coordYCheck = new bool[CoordY.Count];
            coordXNew = new int[CoordX.Count];
            coordYNew = new int[CoordY.Count];
            coordXPrim = new int[CoordX.Count];
            coordYPrim = new int[CoordY.Count];
            imagesBaseRect = new Hashtable();
            totalHeightPageCounter = 0;
            tempRoundedRectangleInfoList = new List<KeyValuePair<StiComponent, double>>();

            if (addComponentWithInteractions)
            {
                Interactions = new int[CoordY.Count, CoordX.Count, 2];
            }
            #endregion

            #region Formating Objects
            service.StatusString = StiLocalization.Get("Export", "ExportingFormatingObjects");

            ArrayList storedCreatedCells = null;

            pageIndex = 0;
            foreach (StiPage page in pages)
            {
                pages.GetPage(page);
                service.InvokeExporting(page, pages, service.CurrentPassNumber + 1, service.MaximumPassNumber);
                if (service.IsStopped) return;

                TotalHeight = totalHeightPage[totalHeightPageCounter++];

                if ((page.Border != null) && (page.Border.Side != StiBorderSides.None))
                {
                    StiContainer borderComponent = new StiContainer();
                    borderComponent.DisplayRectangle = page.ClientRectangle;
                    borderComponent.Border = page.Border;
                    borderComponent.Brush = page.Brush;
                    borderComponent.Page = page;
                    RenderComponent(borderComponent, -1, exportData, true);
                }

                RectangleD pageRect = page.Unit.ConvertToHInches(page.ClientRectangle);
                var marginLeft = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Left) : 0;
                var marginRight = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Right) : 0;
                var marginTop = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Top) : 0;
                var marginBottom = useFullExceedMargins ? page.Unit.ConvertToHInches(page.Margins.Bottom) * 0.5 : 0;
                pageRect = new RectangleD(pageRect.X - marginLeft, pageRect.Y, pageRect.Width + marginLeft + marginRight, pageRect.Height + marginTop + marginBottom);

                TotalHeight += marginTop;

                #region ExceedMargins
                foreach (StiComponent component in page.Components)
                {
                    var stiText = component as StiText;
                    if (component.Enabled && !(isPrinting && !component.Printable) && (stiText != null) && (stiText.ExceedMargins != StiExceedMargins.None) && !(stiText.Brush is StiEmptyBrush))
                    {
                        RectangleD rect = page.Unit.ConvertToHInches(component.DisplayRectangle);
                        if (rect.Height < 0)
                        {
                            rect.Y += rect.Height;
                            rect.Height = Math.Abs(rect.Height);
                        }
                        bool needAdd = true;
                        if ((rect.Right < pageRect.Left) ||
                            (rect.Left > pageRect.Right) ||
                            (rect.Bottom < pageRect.Top) ||
                            (rect.Top > pageRect.Bottom)) needAdd = false;
                        if (!CheckComponentPlacement(component, dataMode)) needAdd = false;

                        if (needAdd)
                        {
                            if ((stiText.ExceedMargins & StiExceedMargins.Left) > 0)
                            {
                                rect.Width += rect.Left + marginLeft;
                                rect.X = pageRect.Left;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Top) > 0)
                            {
                                rect.Height += rect.Top + marginTop;
                                rect.Y = pageRect.Top - marginTop;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Right) > 0)
                            {
                                rect.Width += pageRect.Right - rect.Right;
                            }
                            if ((stiText.ExceedMargins & StiExceedMargins.Bottom) > 0)
                            {
                                rect.Height += pageRect.Height - marginTop - rect.Bottom;
                            }

                            var newText = new StiText(page.Unit.ConvertFromHInches(rect));
                            newText.Page = page;
                            newText.Brush = stiText.Brush;

                            RenderComponent(newText, -1, exportData, false);
                        }
                    }
                }
                #endregion

                int indexComponent = 0;
                foreach (StiComponent component in page.Components)
                {
                    if (component.Enabled && !(isPrinting && !component.Printable))
                    {
                        RectangleD rect = page.Unit.ConvertToHInches(component.DisplayRectangle);
                        if (rect.Height < 0)
                        {
                            rect.Y += rect.Height;
                            rect.Height = Math.Abs(rect.Height);
                        }
                        bool needAdd = true;
                        if (component is StiPointPrimitive) needAdd = false;
                        if ((rect.Right < pageRect.Left) ||
                            (rect.Left > pageRect.Right) ||
                            (rect.Bottom < pageRect.Top) ||
                            (rect.Top > pageRect.Bottom)) needAdd = false;

                        if (StiOptions.Configuration.IsWeb && StiOptions.Engine.AllowInteractionInChartWithComponents && component.Name.EndsWith("Interaction#FX%")) needAdd = false;
                        if ((component is StiContainer) && !string.IsNullOrWhiteSpace(component.Name) && component.Name.StartsWith("TAG##")) needAdd = false;

                        if (!CheckComponentPlacement(component, dataMode)) needAdd = false;

                        if (needAdd)
                        {
                            RenderComponent(component, indexComponent, exportData, false);
                        }
                    }
                    indexComponent++;
                }
                //totalHeight += Math.Round(page.Unit.ConvertToHInches(page.Height), 0);

                //bookmark on page
                if (page.BookmarkValue != null)
                {
                    string bookmark = page.BookmarkValue as string;
                    if (!string.IsNullOrEmpty(bookmark))
                    {
                        #region bookmark on page
                        Rectangle rect = GetRange(page.Unit.ConvertToHInches(page.ClientRectangle));
                        if (rect.Left != -1)
                        {
                            if (rect.Bottom == -1) rect.Height = CoordY.Count - 1 - rect.Top;
                            for (int indexY = rect.Y; indexY < rect.Bottom; indexY++)
                            {
                                bool written = false;
                                for (int indexX = rect.X; indexX < rect.Right; indexX++)
                                {
                                    if ((CellsMap[indexY, indexX] != null) && (Bookmarks[indexY, indexX] == null))
                                    {
                                        Bookmarks[indexY, indexX] = bookmark;
                                        written = true;
                                        break;
                                    }
                                }
                                if (!written)
                                {
                                    for (int indexX = rect.X; indexX < rect.Right; indexX++)
                                    {
                                        if (CellsMap[indexY, indexX] != null)
                                        {
                                            Bookmarks[indexY, indexX] = bookmark;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }

                if (isPaginationMode && !(pageIndex == pages.Count - 1))
                {
                    var tY = totalHeightPage[totalHeightPageCounter] - totalHeightPage[totalHeightPageCounter - 1];
                    var rr = GetRange(new RectangleD(0, tY - paginationModeOffset, TotalWidth, paginationModeOffset));

                    if (paginationContainerInfo == null)
                    {
                        paginationContainerInfo = new StiContainer()
                        {
                            Brush = new StiSolidBrush(Color.Red),
                            Page = page
                        };
                    }

                    StiCell cell = useCacheMode ? new StiCell2(this) : new StiCell(exportFormat);
                    cell.Component = paginationContainerInfo;
                    cell.CellStyle = GetStyleFromComponent(paginationContainerInfo, -1, -1);
                    cell.Left = rr.Left;
                    cell.Top = rr.Top;
                    cell.Width = rr.Width - 1;
                    cell.Height = 0;

                    createdCells.Add(cell);
                    Cells[rr.Top, rr.Left] = cell;
                    for (int indexX = rr.Left; indexX < rr.Right; indexX++)
                    {
                        CellsMap[rr.Top, indexX] = cell;
                    }
                }

                #region Trial
                if (isTrial)
                {
                    if (putAdditionalInformation && pageIndex == 0 && additionalInfo != null)
                    {
                        RenderComponent(additionalInfo, -2, exportData, false);
                    }
                }
                #endregion

                if (useCacheMode)
                {
                    ProcessIntersectedCells(storedCreatedCells);
                    storedCreatedCells = createdCells;
                    createdCells = new ArrayList();

                    if (pageIndex > 0)
                    {
                        ClearCellsMapArea(totalHeightPage[pageIndex - 1], totalHeightPage[pageIndex]);
                    }
                }

                TotalHeight -= marginTop;

                pageIndex++;
            }
            #endregion

            if (useCacheMode)
            {
                ProcessIntersectedCells(storedCreatedCells);
                ProcessIntersectedCells(createdCells);
                ClearCellsMapArea(totalHeightPage[0], totalHeightPage[pageIndex]);

                GCCollect();
            }
            else
            {
                ProcessIntersectedCells(createdCells);
            }

            #region Check narrow
            int maxCurrentCounterX = 0;
            for (int index = 0; index < CoordX.Count; index++)
            {
                coordXNew[index] = maxCurrentCounterX;
                if ((index < CoordX.Count - 1) && ((double)CoordX.GetByIndex(index + 1) - (double)CoordX.GetByIndex(index) < 1.1))
                {
                    if (coordXPrim[index] == 1)
                    {
                        coordXCheck[index] = true;
                        maxCurrentCounterX++;
                    }
                }
            }

            int maxCurrentCounterY = 0;
            for (int index = 0; index < CoordY.Count; index++)
            {
                coordYNew[index] = maxCurrentCounterY;
                if ((index < CoordY.Count - 1) && ((double)CoordY.GetByIndex(index + 1) - (double)CoordY.GetByIndex(index) < 1.1))
                {
                    if (coordYPrim[index] == 1)
                    {
                        coordYCheck[index] = true;
                        maxCurrentCounterY++;
                    }
                }
            }
            #endregion

            cacheManager.flagForceSaveSegment = true;

            #region Post-render primitive
            bool check = true;
            if (check && ((maxCurrentCounterY > 0) || (maxCurrentCounterX > 0)))
            {
                #region Remake array
                bool[,] readyCells = new bool[CoordY.Count, CoordX.Count];

                for (int indexY = 0; indexY < CoordY.Count - 1; indexY++)
                {
                    for (int indexX = 0; indexX < CoordX.Count - 1; indexX++)
                    {
                        int correctValueX = coordXNew[indexX];
                        int correctValueY = coordYNew[indexY];
                        StiCell currentCell = Cells[indexY, indexX];

                        BordersX[indexY - correctValueY, indexX - correctValueX] = BordersX[indexY, indexX];
                        BordersY[indexY - correctValueY, indexX - correctValueX] = BordersY[indexY, indexX];
                        Bookmarks[indexY - correctValueY, indexX - correctValueX] = Bookmarks[indexY, indexX];
                        if (addComponentWithInteractions)
                        {
                            Interactions[indexY - correctValueY, indexX - correctValueX, 0] = Interactions[indexY, indexX, 0];
                            Interactions[indexY - correctValueY, indexX - correctValueX, 1] = Interactions[indexY, indexX, 1];
                        }

                        //here add collect data for GetBookmarkTable method

                        if ((currentCell != null) && (readyCells[indexY, indexX] == false))
                        {
                            #region range
                            for (int yy = 0; yy <= currentCell.Height; yy++)
                            {
                                for (int xx = 0; xx <= currentCell.Width; xx++)
                                {
                                    readyCells[indexY + yy, indexX + xx] = true;
                                    if ((yy != 0) || (xx != 0))
                                    {
                                        Cells[indexY + yy, indexX + xx] = null;
                                    }
                                }
                            }
                            #endregion

                            if ((coordXCheck[indexX] == true) || (coordYCheck[indexY] == true))
                            {
                                Cells[indexY, indexX] = null;
                                currentCell = null;
                            }
                            else
                            {
                                currentCell.Width -= coordXNew[indexX + currentCell.Width + 1] - correctValueX;
                                currentCell.Height -= coordYNew[indexY + currentCell.Height + 1] - correctValueY;
                                currentCell.Left -= correctValueX;
                                currentCell.Top -= correctValueY;
                            }
                            if ((correctValueX > 0) || (correctValueY > 0))
                            {
                                Cells[indexY - correctValueY, indexX - correctValueX] = currentCell;
                                Cells[indexY, indexX] = null;
                                currentCell = null;
                            }

                        }

                    }
                }

                //move last borders lines
                for (int index = 0; index < CoordX.Count - 1; index++)
                {
                    int correctValueX = coordXNew[index];
                    BordersX[CoordY.Count - 1 - maxCurrentCounterY, index - correctValueX] = BordersX[CoordY.Count - 1, index];
                }
                for (int index = 0; index < CoordY.Count - 1; index++)
                {
                    int correctValueY = coordYNew[index];
                    BordersY[index - correctValueY, CoordX.Count - 1 - maxCurrentCounterX] = BordersY[index, CoordX.Count - 1];
                }

                //remove trash lines
                for (int index = CoordX.Count - 1 - 1; index >= 0; index--)
                {
                    if (coordXCheck[index] == true)
                    {
                        CoordX.RemoveAt(index);
                    }
                }

                for (int index = CoordY.Count - 1 - 1; index >= 0; index--)
                {
                    if (coordYCheck[index] == true)
                    {
                        CoordY.RemoveAt(index);
                    }
                }
                #endregion
            }
            #endregion

            //memory optimization
            leftCached.Clear();
            topCached.Clear();
            coordXNew = null;
            coordXCheck = null;
            coordXPrim = null;
            coordYNew = null;
            coordYCheck = null;
            coordYPrim = null;
            CellsMap = null;

            if (useCacheMode)
            {
                GCCollect();
            }

            #region Check Chrome bug
            if (isHtmlService)
            {
                bool[,] readyCells = new bool[CoordY.Count, CoordX.Count];
                for (int rowIndex = 1; rowIndex < CoordY.Count - 1; rowIndex++) //-1 because last line is for compensation
                {
                    for (int columnIndex = 1; columnIndex < CoordX.Count; columnIndex++)
                    {
                        if (readyCells[rowIndex - 1, columnIndex - 1]) continue;

                        StiCell cell = Cells[rowIndex - 1, columnIndex - 1];
                        if ((cell == null) || !(cell.Component is StiContainer)) continue;

                        if (cell.CellStyle.Color.Equals(Color.White) || cell.CellStyle.Color.Equals(Color.Transparent)) continue;

                        #region range
                        for (int yy = 0; yy <= cell.Height; yy++)
                        {
                            for (int xx = 0; xx <= cell.Width; xx++)
                            {
                                if (!readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx])
                                {
                                    GetMaxRectFromCell(cell, rowIndex - 1 + yy, columnIndex - 1 + xx, readyCells);
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region Make RoundedRectanglesList
            RoundedRectanglesList = new Dictionary<int, List<RoundedRectangleInfo>>();
            if (tempRoundedRectangleInfoList.Count > 0)
            {
                foreach (KeyValuePair<StiComponent, double> pair in tempRoundedRectangleInfoList)
                {
                    var component = pair.Key;
                    RectangleD rectD = component.Page.Unit.ConvertToHInches(component.DisplayRectangle);
                    if (rectD.Height < 0)
                    {
                        rectD.Y += rectD.Height;
                        rectD.Height = Math.Abs(rectD.Height);
                    }
                    TotalHeight = pair.Value;
                    Rectangle rect = GetRange(rectD);
                    if (rect.Left != -1)
                    {
                        List<RoundedRectangleInfo> list;
                        if (!RoundedRectanglesList.TryGetValue(rect.Top, out list))
                        {
                            list = new List<RoundedRectangleInfo>();
                            RoundedRectanglesList[rect.Top] = list;
                        }
                        var newInfo = new RoundedRectangleInfo() { Rect = rect, Primitive = component as StiRoundedRectanglePrimitive };
                        list.Add(newInfo);
                    }
                }
                tempRoundedRectangleInfoList.Clear();
            }
            #endregion

            #region Html styles correction
            if (isHtmlOrExcelXmlService)
            {
                cellStyles2 = new StiCellStyle[CoordY.Count, CoordX.Count];
                bool[,] readyCells = new bool[CoordY.Count, CoordX.Count];
                Font defaultFont = new Font("Arial", 8);

                ArrayList currentStyles = Styles;
                ArrayList storedStyles = new ArrayList();
                storedStyles.AddRange(Styles);

                if (tempStylesCount == -1)
                {
                    Styles.Clear();
                }
                else
                {
                    Styles.RemoveRange(tempStylesCount, Styles.Count - tempStylesCount);
                }

                stylesCache.Clear();
                var tempList = new ArrayList();
                foreach (StiCellStyle style in Styles)
                {
                    StiCellStyle.GetStyleFromCache(style.Color, style.TextColor, style.Font, style.HorAlignment, style.VertAlignment,
                        style.Border, style.BorderL, style.BorderR, style.BorderB, style.TextOptions, style.WordWrap, style.Format, style.InternalStyleName, style.LineSpacing, style.AllowHtmlTags, stylesCache, tempList, fontsCache,
                        style, true);
                }

                //Remake styles
                for (int rowIndex = 1; rowIndex < CoordY.Count - 1; rowIndex++) //-1 because last line is for compensation
                {
                    for (int columnIndex = 1; columnIndex < CoordX.Count; columnIndex++)
                    {
                        if (readyCells[rowIndex - 1, columnIndex - 1] == false)
                        {
                            StiCell cell = Cells[rowIndex - 1, columnIndex - 1];
                            if (cell != null)
                            {
                                #region cell exist

                                #region range
                                for (int yy = 0; yy <= cell.Height; yy++)
                                {
                                    for (int xx = 0; xx <= cell.Width; xx++)
                                    {
                                        readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx] = true;
                                    }
                                }
                                #endregion

                                bool needBorderLeft = true;
                                bool needBorderRight = true;
                                for (int index = 0; index < cell.Height + 1; index++)
                                {
                                    if (BordersY[cell.Top + index, cell.Left] == null) needBorderLeft = false;
                                    if (BordersY[cell.Top + index, cell.Left + cell.Width + 1] == null) needBorderRight = false;
                                }
                                bool needBorderTop = true;
                                bool needBorderBottom = true;
                                for (int index = 0; index < cell.Width + 1; index++)
                                {
                                    if (BordersX[cell.Top, cell.Left + index] == null) needBorderTop = false;
                                    if (BordersX[cell.Top + cell.Height + 1, cell.Left + index] == null) needBorderBottom = false;
                                }

                                this.Styles = storedStyles;
                                StiCellStyle style = cell.CellStyle;
                                this.Styles = currentStyles;

                                //StiCellStyle newStyle = GetStyle(new StiCellStyle(
                                StiCellStyle newStyle = StiCellStyle.GetStyleFromCache(
                                    style.Color,
                                    style.TextColor,
                                    style.Font,
                                    style.HorAlignment,
                                    style.VertAlignment,
                                    (needBorderTop ? BordersX[cell.Top, cell.Left] : null),
                                    (needBorderLeft ? BordersY[cell.Top, cell.Left] : null),
                                    (needBorderRight ? BordersY[cell.Top, cell.Left + cell.Width + 1] : null),
                                    (needBorderBottom ? BordersX[cell.Top + cell.Height + 1, cell.Left] : null),
                                    style.TextOptions,
                                    style.WordWrap,
                                    style.Format,
                                    style.InternalStyleName,
                                    style.LineSpacing,
                                    style.AllowHtmlTags,
                                    stylesCache,
                                    Styles,
                                    fontsCache,
                                    style,
                                    false);
                                CellStyles[rowIndex - 1, columnIndex - 1] = newStyle;
                                #endregion
                            }
                            else
                            {
                                #region cell not exist - make new style
                                bool needBorderLeft = true;
                                bool needBorderRight = true;
                                if (BordersY[rowIndex - 1, columnIndex - 1] == null) needBorderLeft = false;
                                if (BordersY[rowIndex - 1, columnIndex - 0] == null) needBorderRight = false;
                                bool needBorderTop = true;
                                bool needBorderBottom = true;
                                if (BordersX[rowIndex - 1, columnIndex - 1] == null) needBorderTop = false;
                                if (BordersX[rowIndex - 0, columnIndex - 1] == null) needBorderBottom = false;

                                if (needBorderLeft || needBorderRight || needBorderTop || needBorderBottom)
                                {
                                    //StiCellStyle newStyle = GetStyle(new StiCellStyle(
                                    StiCellStyle newStyle = StiCellStyle.GetStyleFromCache(
                                        Color.Transparent,
                                        Color.Black,
                                        defaultFont,
                                        StiTextHorAlignment.Center,
                                        StiVertAlignment.Center,
                                        (needBorderTop ? BordersX[rowIndex - 1, columnIndex - 1] : null),
                                        (needBorderLeft ? BordersY[rowIndex - 1, columnIndex - 1] : null),
                                        (needBorderRight ? BordersY[rowIndex - 1, columnIndex - 0] : null),
                                        (needBorderBottom ? BordersX[rowIndex - 0, columnIndex - 1] : null),
                                        null,
                                        false,
                                        null,
                                        null,
                                        1,
                                        false,
                                        stylesCache,
                                        Styles,
                                        fontsCache,
                                        null,
                                        false);
                                    CellStyles[rowIndex - 1, columnIndex - 1] = newStyle;
                                }
                                #endregion
                            }
                        }
                    }
                }
                CheckStylesNames();
            }
            #endregion

            cacheManager.flagForceSaveSegment = false;

            #region Check horizontal page breaks
            if (pages.Count > 1)
            {
                var totalHeightPage2Reverse = new Hashtable();
                for (int indexPage = 1; indexPage < pages.Count; indexPage++)
                {
                    totalHeightPage2Reverse[totalHeightPage2[indexPage]] = null;
                }

                for (int indexY = 0; indexY < CoordY.Count; indexY++)
                {
                    double currentY = (double)CoordY.GetByIndex(indexY);
                    if (totalHeightPage2Reverse.ContainsKey(currentY))
                    {
                        HorizontalPageBreaks.Add(indexY);
                        HorizontalPageBreaksHash.Add(indexY, null);
                    }
                }
            }
            #endregion

        }
        #endregion
    }
}
