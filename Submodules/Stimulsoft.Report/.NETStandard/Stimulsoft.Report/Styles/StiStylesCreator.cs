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

using System.Linq;
using System.Collections.Generic;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    public class StiStylesCreator
    {
        #region Fields
        private StiReport report;
        private List<StiBaseStyle> hashAllStyles;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for ReportTitleBands.
        /// </summary>
        public bool ShowReportTitles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for ReportSummaryBands.
        /// </summary>
        public bool ShowReportSummaries { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for PageHeaderBands.
        /// </summary>
        public bool ShowPageHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for PageFooterBands.
        /// </summary>
        public bool ShowPageFooters { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for GroupHeaderBands.
        /// </summary>
        public bool ShowGroupHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for GroupFooterBands.
        /// </summary>
        public bool ShowGroupFooters { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for HeaderBands.
        /// </summary>
        public bool ShowHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for DataBands.
        /// </summary>
        public bool ShowDatas { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style for FooterBands.
        /// </summary>
        public bool ShowFooters { get; set; } = true;

        /// <summary>
        /// Gets or sets a value which indicates that styles creator will create a style showing borders.
        /// </summary>
        public bool ShowBorders { get; set; }

        /// <summary>
        /// Gets or sets color factor translation.
        /// </summary>
        private float ColorFactor
        {
            get
            {
                if (this.NestedFactor == StiNestedFactor.High) return 1.5f;
                if (this.NestedFactor == StiNestedFactor.Normal) return 1f;
                return 0.5f;
            }
        }

        /// <summary>
        /// Gets or sets maximal nested level for created styles.
        /// </summary>
        public int MaxNestedLevel { get; set; } = 3;

        /// <summary>
        /// Gets or sets nested level factor for created styles.
        /// </summary>
        public StiNestedFactor NestedFactor { get; set; } = StiNestedFactor.Normal;
        #endregion

        #region Methods
        public List<StiBaseStyle> CreateStyles(string collectionName, Color baseColor)
        {
            if (baseColor.IsEmpty || baseColor == Color.FromArgb(0, 0, 0, 0))
                baseColor = Color.White;

            var factor = ((float)(baseColor.R + baseColor.G + baseColor.B)) / 3;
            
            var borderColor = StiColorUtils.Dark(baseColor,(byte)( 100f * this.ColorFactor));            
            var border = new StiBorder(StiBorderSides.None, borderColor, 1, StiPenStyle.Solid);

            var simpleCompTypes = StiStyleComponentType.Text | StiStyleComponentType.Image | StiStyleComponentType.Primitive | StiStyleComponentType.CheckBox;

            var styles = new List<StiBaseStyle>();

            #region Create Styles for Report Titles
            if (this.ShowReportTitles)
            {                
                var reportTitleFont = new Font("Arial", 14, FontStyle.Bold);
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 200) : baseColor;
                var reportTitleBrush = new StiSolidBrush(Color.Transparent);
                var reportTitleForeBrush = new StiSolidBrush(foreColor);

                var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiReportTitleBand")).Replace(" ", "_"); 
               
                #region Title 1
                var nameTitle1 = $"{collectionName}_{name}1";
                CreateStyles(nameTitle1, ShowBorders, reportTitleFont, reportTitleBrush, reportTitleForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.ReportTitle, 1, StiStyleConditionOperation.EqualTo, styles);
                #endregion                

                #region Title 2
                reportTitleFont = new Font("Arial", 10, FontStyle.Bold);
                var nameTitle2 = $"{collectionName}_{name}2";
                CreateStyles(nameTitle2, ShowBorders, reportTitleFont, reportTitleBrush, reportTitleForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.ReportTitle, 2, StiStyleConditionOperation.GreaterThanOrEqualTo, styles);
                #endregion
            }
            #endregion

            #region Create Styles for Report Summaries
            if (this.ShowReportSummaries)
            {
                var reportSummaryFont = new Font("Arial", 12, FontStyle.Bold);
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 200) : baseColor;
                var reportSummaryBrush = new StiSolidBrush(Color.Transparent);
                var reportSummaryForeBrush = new StiSolidBrush(foreColor);

                var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiReportSummaryBand")).Replace(" ", "_");

                #region Summary 1                
                var nameSummary1 = $"{collectionName}_{name}1";
                CreateStyles(nameSummary1, ShowBorders, reportSummaryFont, reportSummaryBrush, reportSummaryForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.ReportSummary, 1, StiStyleConditionOperation.EqualTo, styles);
                #endregion

                #region Summary 2
                reportSummaryFont = new Font("Arial", 10, FontStyle.Bold);
                var nameSummary2 = $"{collectionName}_{name}2";
                CreateStyles(nameSummary2, ShowBorders, reportSummaryFont, reportSummaryBrush, reportSummaryForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.ReportSummary, 2, StiStyleConditionOperation.GreaterThanOrEqualTo, styles);
                #endregion
            }
            #endregion

            #region Create Styles for Page Headers
            if (this.ShowPageHeaders)
            {
                var pageHeadersFont = new Font("Arial", 10);
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 200) : baseColor;
                var pageHeadersBrush = new StiSolidBrush(Color.Transparent);
                var pageHeadersForeBrush = new StiSolidBrush(foreColor);

                var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiPageHeaderBand")).Replace(" ", "_");
                name = $"{collectionName}_{name}";
                CreateStyles(name, ShowBorders, pageHeadersFont, pageHeadersBrush, pageHeadersForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.PageHeader, null, null, styles);
            }
            #endregion

            #region Create Styles for Page Footers
            if (this.ShowPageFooters)
            {
                var pageFootersFont = new Font("Arial", 10);
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 200) : baseColor;
                var pageFootersBrush = new StiSolidBrush(Color.Transparent);
                var pageFootersForeBrush = new StiSolidBrush(foreColor);

                var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiPageFooterBand")).Replace(" ", "_");
                name = $"{collectionName}_{name}";
                CreateStyles(name, ShowBorders, pageFootersFont, pageFootersBrush, pageFootersForeBrush, border, simpleCompTypes,
                    StiStyleComponentPlacement.PageFooter, null, null, styles);
            }
            #endregion

            #region Create Styles for Group Headers
            if (this.ShowGroupHeaders)
            {
                var fontPerLevel = 4f / MaxNestedLevel;
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 150) : baseColor;
                var groupHeaderForeBrush = new StiSolidBrush(foreColor);

                for (int index = 1; index <= MaxNestedLevel; index++)
                {
                    var fontFactor = (int)(fontPerLevel * (float)(index - 1));
                    var groupHeaderFont = new Font("Arial", 13 - fontFactor, FontStyle.Bold);

                    var groupHeaderBrush = new StiSolidBrush(Color.Transparent);

                    var operation = index == MaxNestedLevel ?
                        StiStyleConditionOperation.GreaterThanOrEqualTo : StiStyleConditionOperation.EqualTo;

                    var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiGroupHeaderBand")).Replace(" ", "_");
                    name = $"{collectionName}_{name}{index}";
                    CreateStyles(name, ShowBorders, groupHeaderFont, groupHeaderBrush, groupHeaderForeBrush, border, simpleCompTypes,
                        StiStyleComponentPlacement.GroupHeader, index, operation, styles);
                }
            }
            #endregion

            #region Create Styles for Group Footers
            if (this.ShowGroupFooters)
            {
                var fontPerLevel = 4f / MaxNestedLevel;
                var foreColor = factor > 150 ? StiColorUtils.Dark(baseColor, 150) : baseColor;
                var groupFooterForeBrush = new StiSolidBrush(foreColor);

                for (int index = 1; index <= MaxNestedLevel; index++)
                {
                    var fontFactor = (int)(fontPerLevel * (float)(index - 1));
                    var groupFooterFont = new Font("Arial", 13 - fontFactor, FontStyle.Bold);

                    var groupFooterBrush = new StiSolidBrush(Color.Transparent);

                    var operation = index == 0 ? StiStyleConditionOperation.GreaterThanOrEqualTo : StiStyleConditionOperation.EqualTo;

                    var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiGroupFooterBand")).Replace(" ", "_");
                    name = $"{collectionName}_{name}{index}";
                    CreateStyles(name, ShowBorders, groupFooterFont, groupFooterBrush, groupFooterForeBrush, border, simpleCompTypes,
                        StiStyleComponentPlacement.GroupFooter, index, operation, styles);
                }
            }
            #endregion

            #region Create Styles for Header Bands
            if (this.ShowHeaders)
            {
                var colorPerLevel = 100 / MaxNestedLevel;
                var headerFont = new Font("Arial", 8, FontStyle.Bold);
                var foreColor = StiColorUtils.Dark(baseColor, (byte)(200f));
                var headerForeBrush = new StiSolidBrush(foreColor);

                border.Topmost = true;

                for (int index = 1; index <= MaxNestedLevel; index++)
                {                    
                    var colorFactor = (int)(colorPerLevel * (float)(index - 1) * this.ColorFactor);
                    if (colorFactor > 255) colorFactor = 255;

                    var headerBrush = new StiSolidBrush(StiColorUtils.Light(baseColor, (byte)colorFactor));                                    

                    var operation = index == MaxNestedLevel ? StiStyleConditionOperation.GreaterThanOrEqualTo : StiStyleConditionOperation.EqualTo;

                    var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiHeaderBand")).Replace(" ", "_");
                    name = $"{collectionName}_{name}{index}";
                    CreateStyles(name, ShowBorders, headerFont, headerBrush, headerForeBrush, border, simpleCompTypes,
                        StiStyleComponentPlacement.Header, index, operation, styles);
                }
            }
            #endregion

            #region Create Styles for Data Bands
            if (this.ShowDatas)
            {
                var colorPerLevel = 60 / MaxNestedLevel;
                var dataFont = new Font("Arial", 8);
                var foreColor = StiColorUtils.Dark(baseColor, (byte)(200f));
                var dataForeBrush = new StiSolidBrush(foreColor);                

                for (int index = 1; index <= MaxNestedLevel; index++)
                {
                    var colorFactor = (int)((150 + colorPerLevel * (float)(index - 1)) * this.ColorFactor);
                    if (colorFactor > 255) colorFactor = 255;

                    var dataBrush = new StiSolidBrush(StiColorUtils.Light(baseColor, (byte)colorFactor));
                    var dataBrushEven = new StiSolidBrush(StiColorUtils.Light(baseColor, (byte)((float)colorFactor * 0.5)));

                    var operation = index == MaxNestedLevel ? StiStyleConditionOperation.GreaterThanOrEqualTo : StiStyleConditionOperation.EqualTo;

                    #region Data Styles
                    var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiDataBand")).Replace(" ", "_");
                    name = $"{collectionName}_{name}{index}";
                    CreateStyles(name, ShowBorders, dataFont, new StiSolidBrush(Color.Transparent), dataForeBrush, border, simpleCompTypes,
                        StiStyleComponentPlacement.Data, index, operation, styles);
                    #endregion

                    #region Odd Styles
                    name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiDataBand")).Replace(" ", "_");
                    var odd = Loc.Get("PropertyMain", "OddStyle");
                    name = $"{collectionName}_{name}{index}_{odd}";
                    CreateStyles(name, ShowBorders, dataFont, dataBrush, dataForeBrush, border, null,
                        StiStyleComponentPlacement.DataOddStyle, index, operation, styles);
                    #endregion

                    #region Even Styles
                    name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiDataBand")).Replace(" ", "_");
                    var even = Loc.Get("PropertyMain", "EvenStyle");
                    name = $"{collectionName}_{name}{index}_{even}";
                    CreateStyles(name, ShowBorders, dataFont, dataBrushEven, dataForeBrush, border, null,
                        StiStyleComponentPlacement.DataEvenStyle, index, operation, styles);
                    #endregion
                }
            }
            #endregion

            #region Create Styles for Footer Bands
            if (this.ShowFooters)
            {
                var colorPerLevel = 100 / MaxNestedLevel;
                var footerFont = new Font("Arial", 8);
                var foreColor = StiColorUtils.Dark(baseColor, (byte)(200f));
                var footerForeBrush = new StiSolidBrush(foreColor);

                for (int index = 1; index <= MaxNestedLevel; index++)
                {
                    var colorFactor = (int)((colorPerLevel * (float)(index - 1) + 20) * this.ColorFactor);
                    if (colorFactor > 255) colorFactor = 255;

                    var footerBrush = new StiSolidBrush(StiColorUtils.Light(baseColor, (byte)colorFactor));

                    var operation = index == 0 ? StiStyleConditionOperation.GreaterThanOrEqualTo : StiStyleConditionOperation.EqualTo;

                    var name = StiFunctionsStrings.ToProperCase(Loc.Get("Components", "StiFooterBand")).Replace(" ", "_");
                    name = $"{collectionName}_{name}{index}";
                    CreateStyles(name, ShowBorders, footerFont, footerBrush, footerForeBrush, border, simpleCompTypes,
                        StiStyleComponentPlacement.Footer, index, operation, styles);
                }
            }
            #endregion

            foreach (var style in styles)
            {
                style.CollectionName = collectionName;
            }

            return styles;
        }

        private void CreateStyles(string name, bool showBorders,
            Font font, StiBrush headerBrush1, StiBrush headerForeBrush, StiBorder border,
            StiStyleComponentType? simpleCompTypes, StiStyleComponentPlacement placement,
            int? placementNestedLevel, StiStyleConditionOperation? operation,
            List<StiBaseStyle> styles)
        {
            var headerStyles = CreateStyles(name, font, headerBrush1, headerForeBrush, border,
                placement, placementNestedLevel, operation, simpleCompTypes, showBorders);

            foreach (var style in headerStyles)
                styles.Add(style);
        }
        
        private List<StiBaseStyle> CreateStyles(string name, Font font, StiBrush brush, StiBrush textBrush, StiBorder border,
            StiStyleComponentPlacement placement, 
            int? placementNestedLevel, StiStyleConditionOperation? placementNestedLevelOperation, 
            StiStyleComponentType? componentType, bool showBorders)
        {
            var styles = new List<StiBaseStyle>();

            border = new StiBorder(StiBorderSides.None, border.Color, border.Size, border.Style, border.DropShadow, border.ShadowSize, border.ShadowBrush, border.Topmost);            

            var style = CreateStyle(name, font, brush, textBrush, border, placement, placementNestedLevel, placementNestedLevelOperation, componentType);
            if (placement == StiStyleComponentPlacement.ReportTitle ||
                placement == StiStyleComponentPlacement.ReportSummary ||
                placement == StiStyleComponentPlacement.PageFooter ||
                placement == StiStyleComponentPlacement.PageHeader)
                style.AllowUseBorderSidesFromLocation = false;
            else
                style.AllowUseBorderSidesFromLocation = ShowBorders;

            styles.Add(style);
            
            return styles;
        }

        private StiStyle CreateStyle(string name, Font font, StiBrush brush, StiBrush textBrush, StiBorder border,
            StiStyleComponentPlacement placement, 
            int? placementNestedLevel, StiStyleConditionOperation? placementNesterLevelOperation, 
            StiStyleComponentType? componentType)
        {
            var style = new StiStyle(GetStyleName(name))
            {
                Font = font,
                Brush = brush,
                TextBrush = textBrush,
                Border = border
            };

            var elements = new List<StiStyleConditionElement>();

            if (placementNesterLevelOperation != null && placementNestedLevel != null)
                elements.Add(new StiStyleConditionPlacementNestedLevelElement(placementNestedLevel.Value, placementNesterLevelOperation.Value));

            if (componentType != null)
                elements.Add(new StiStyleConditionComponentTypeElement(componentType.Value));

            elements.Add(new StiStyleConditionPlacementElement(placement));
            style.Conditions.Add(elements.ToArray());            
            
            return style;
        }

        private string GetStyleName(string baseName)
        {
            var name = baseName;
            var index = 2;

            while (true)
            {
                var finded = false;

                if (report != null)
                {
                    foreach (StiBaseStyle style in report.Styles)
                    {
                        if (style.Name == name)
                        {
                            finded = true;
                            break;
                        }
                    }
                }
                else
                {
                    finded = (this.hashAllStyles.FirstOrDefault(x => x.Name == name) != null);
                }

                if (!finded) return name;

                name = $"{baseName}_{index}";
                index++;
            }
        }
        #endregion

        public StiStylesCreator(StiReport report)
        {
            this.report = report;
        }

        public StiStylesCreator(List<StiBaseStyle> hashAllStyles)
        {
            this.hashAllStyles = hashAllStyles;
        }
    }
}
