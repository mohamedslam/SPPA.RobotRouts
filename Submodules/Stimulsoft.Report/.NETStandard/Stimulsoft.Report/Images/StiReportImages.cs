#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Collections;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Images
{
    public static class StiReportImages
    {
        #region class Actions
        public static class Actions
        {
            public static Image DropDown(bool allowCache = true) => GetImage("Actions.DropDown", StiImageSize.Normal, allowCache);

            public static Image DropDownArrowMouseOver(bool allowCache = true) => GetImage("Actions.DropDownArrowMouseOver", StiImageSize.Normal, allowCache);

            public static Image DropDownFilter(bool allowCache = true) => GetImage("Actions.DropDownFilter", StiImageSize.Normal, allowCache);

            public static Image DropDownFilterMouseOver(bool allowCache = true) => GetImage("Actions.DropDownFilterMouseOver", StiImageSize.Normal, allowCache);

            public static Image DropDownArrowMouseOverWhite(bool allowCache = true) => GetImage("Actions.DropDownArrowMouseOver-white", StiImageSize.Normal, allowCache);

            public static Image DropDownFilterWhite(bool allowCache = true) => GetImage("Actions.DropDownFilter-white", StiImageSize.Normal, allowCache);

            public static Image DropDownFilterMouseOverWhite(bool allowCache = true) => GetImage("Actions.DropDownFilterMouseOver-white", StiImageSize.Normal, allowCache);

            public static Image SortArrowAsc(bool allowCache = true) => GetImage("Actions.SortArrowAsc", StiImageSize.Normal, allowCache);

            public static Image SortArrowAscMouseOver(bool allowCache = true) => GetImage("Actions.SortArrowAscMouseOver", StiImageSize.Normal, allowCache);

            public static Image SortArrowDesc(bool allowCache = true) => GetImage("Actions.SortArrowDesc", StiImageSize.Normal, allowCache);

            public static Image SortArrowDescMouseOver(bool allowCache = true) => GetImage("Actions.SortArrowDescMouseOver", StiImageSize.Normal, allowCache);

            public static Image SortArrowAscWhite(bool allowCache = true) => GetImage("Actions.SortArrowAsc-white", StiImageSize.Normal, allowCache);

            public static Image SortArrowAscMouseOverWhite(bool allowCache = true) => GetImage("Actions.SortArrowAscMouseOver-white", StiImageSize.Normal, allowCache);

            public static Image SortArrowAscMouseOverDisable(bool allowCache = true) => GetImage("Actions.SortArrowAscMouseOver-disable", StiImageSize.Normal, allowCache);

            public static Image SortArrowDescWhite(bool allowCache = true) => GetImage("Actions.SortArrowDesc-white", StiImageSize.Normal, allowCache);

            public static Image SortArrowDescMouseOverWhite(bool allowCache = true) => GetImage("Actions.SortArrowDescMouseOver-white", StiImageSize.Normal, allowCache);

            public static Image SortAsc(bool allowCache = true) => GetImage("Actions.SortAsc", StiImageSize.Normal, allowCache);

            public static Image SortDesc(bool allowCache = true) => GetImage("Actions.SortDesc", StiImageSize.Normal, allowCache);
        }
        #endregion

        #region class Components
        public static class Components
        {
            public static Image BarCode(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiBarCode", size, allowCache);

            public static Image ButtonControl(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiButtonControl", size, allowCache);
            
            public static Image ButtonElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiButtonElement", size, allowCache);

            public static Image CardsElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCardsElement", size, allowCache);

            public static Image CatCrossBands(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("catCrossBands", size, allowCache);

            public static Image CatComponents(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("catComponents", size, allowCache);

            public static Image CatBands(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("catBands", size, allowCache);

            public static Image CatSignatures(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("catSignatures", size, allowCache);

            public static Image CatUncategorized(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("catUncategorized", size, allowCache);

            public static Image ChartElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiChartElement", size, allowCache);

            public static Image ComboBoxElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiComboBoxElement", size, allowCache);

            public static Image Chart(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiChart", size, allowCache);

            public static Image CheckBox(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCheckBox", size, allowCache);

            public static Image ChildBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiChildBand", size, allowCache);

            public static Image Clone(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiClone", size, allowCache);

            public static Image ColumnFooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiColumnFooterBand", size, allowCache);

            public static Image ColumnHeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiColumnHeaderBand", size, allowCache);

            public static Image Component(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiComponent", size, allowCache);

            public static Image Container(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiContainer", size, allowCache);

            public static Image CrossDataBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossDataBand", size, allowCache);

            public static Image CrossFooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossFooterBand", size, allowCache);

            public static Image CrossGroupFooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossGroupFooterBand", size, allowCache);

            public static Image CrossGroupHeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossGroupHeaderBand", size, allowCache);

            public static Image CrossHeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossHeaderBand", size, allowCache);

            public static Image CrossTab(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiCrossTab", size, allowCache);

            public static Image DataBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiDataBand", size, allowCache);

            public static Image Dashboard(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiDashboard", size, allowCache);

            public static Image DatePickerElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiDatePickerElement", size, allowCache);

            public static Image EmptyBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiEmptyBand", size, allowCache);

            public static Image FilterElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiFilterElement", size, allowCache);

            public static Image FooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiFooterBand", size, allowCache);

            public static Image Form(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiForm", size, allowCache);

            public static Image ImageElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiImageElement", size, allowCache);

            public static Image IndicatorElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiIndicatorElement", size, allowCache);

            public static Image ListBoxElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiListBoxElement", size, allowCache);

            public static Image MapElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiMapElement", size, allowCache);

            public static Image Gauge(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiGauge", size, allowCache);

            public static Image GaugeElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiGaugeElement", size, allowCache);

            public static Image GroupFooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiGroupFooterBand", size, allowCache);

            public static Image GroupHeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiGroupHeaderBand", size, allowCache);

            public static Image HeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiHeaderBand", size, allowCache);

            public static Image HierarchicalBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiHierarchicalBand", size, allowCache);

            public static Image HorizontalLinePrimitive(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiHorizontalLinePrimitive", size, allowCache);

            public static Image Image(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiImage", size, allowCache);

            public static Image Map(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiMap", size, allowCache);

            public static Image OnlineMapElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiOnlineMapElement", size, allowCache);

            public static Image OverlayBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiOverlayBand", size, allowCache);

            public static Image RegionMapElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiRegionMapElement", size, allowCache);

            public static Image Page(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPage", size, allowCache);

            public static Image PanelElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPanelElement", size, allowCache);

            public static Image PageFooterBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPageFooterBand", size, allowCache);

            public static Image PageHeaderBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPageHeaderBand", size, allowCache);

            public static Image Panel(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPanel", size, allowCache);

            public static Image PivotTableElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiPivotTableElement", size, allowCache);

            public static Image ProgressElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiProgressElement", size, allowCache);

            public static Image RectanglePrimitive(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiRectanglePrimitive", size, allowCache);

            public static Image Report(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiReport", size, allowCache);

            public static Image ReportTitleBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiReportTitleBand", size, allowCache);

            public static Image ReportSummaryBand(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiReportSummaryBand", size, allowCache);

            public static Image RichText(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiRichText", size, allowCache);

            public static Image Shape(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiShape", size, allowCache);

            public static Image ShapeElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiShapeElement", size, allowCache);

            public static Image Screen(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiScreen", size, allowCache);

            public static Image SubReport(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiSubReport", size, allowCache);

            public static Image Table(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTable", size, allowCache);

            public static Image TableElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTableElement", size, allowCache);

            public static Image Text(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiText", size, allowCache);

            public static Image TextElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTextElement", size, allowCache);

            public static Image TreeViewElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTreeViewElement", size, allowCache);

            public static Image TreeViewBoxElement(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTreeViewBoxElement", size, allowCache);

            public static Image TextInCells(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTextInCells", size, allowCache);

            public static Image ToolBox(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("ToolBox", size, allowCache);

            public static Image TreeViewControl(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiTreeViewControl", size, allowCache);

            public static Image WinControl(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiWinControl", size, allowCache);

            public static Image VerticalLinePrimitive(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiVerticalLinePrimitive", size, allowCache);

            public static Image ZipCode(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetToolboxImage("StiZipCode", size, allowCache);
        }
        #endregion

        #region class Dictionary
        public static class Dictionary
        {
            public static Image BusinessObject(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("BusinessObject", size, allowCache);

            public static Image CalcColumn(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumn", size, allowCache);

            public static Image CalcColumnBool(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnBool", size, allowCache);

            public static Image CalcColumnChar(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnChar", size, allowCache);

            public static Image CalcColumnDateTime(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnDateTime", size, allowCache);

            public static Image CalcColumnInt(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnInt", size, allowCache);

            public static Image CalcColumnFloat(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnFloat", size, allowCache);

            public static Image CalcColumnImage(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnImage", size, allowCache);

            public static Image CalcColumnBinary(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnBinary", size, allowCache);

            public static Image CalcColumnDecimal(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnDecimal", size, allowCache);

            public static Image CalcColumnString(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("CalcColumnString", size, allowCache);

            public static Image Category(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Category", size, allowCache);

            public static Image Close(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Close", size, allowCache);

            public static Image Database(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Database", size, allowCache);

            public static Image DataColumn(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumn", size, allowCache);

            public static Image DataColumnBool(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnBool", size, allowCache);

            public static Image DataColumnChar(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnChar", size, allowCache);

            public static Image DataColumnDateTime(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnDateTime", size, allowCache);

            public static Image DataColumnDecimal(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnDecimal", size, allowCache);

            public static Image DataColumnInt(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnInt", size, allowCache);

            public static Image DataColumnFloat(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnFloat", size, allowCache);

            public static Image DataColumnImage(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnImage", size, allowCache);

            public static Image DataColumnBinary(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnBinary", size, allowCache);

            public static Image DataColumnString(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataColumnString", size, allowCache);

            public static Image DataParameter(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataParameter", size, allowCache);

            public static Image DataParameters(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataParameters", size, allowCache);

            public static Image DataRelation(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataRelation", size, allowCache);

            public static Image DataRelations(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataRelations", size, allowCache);

            public static Image DataTable(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataTable", size, allowCache);

            public static Image DataTables(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataTables", size, allowCache);

            public static Image DataView(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataView", size, allowCache);

            public static Image DataViews(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataViews", size, allowCache);

            public static Image DataSource(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("DataSource", size, allowCache);

            public static Image Query(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Query", size, allowCache);

            public static Image Queries(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Queries", size, allowCache);
            
            public static Image Resource(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Resource", size, allowCache);
            
            public static Image ResourceCsv(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceCsv", size, allowCache);
            
            public static Image ResourceData(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceData", size, allowCache);
            
            public static Image ResourceDbf(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceDbf", size, allowCache);
            
            public static Image ResourceExcel(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceExcel", size, allowCache);
            
            public static Image ResourceFontEot(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceFontEot", size, allowCache);
            
            public static Image ResourceFontOtf(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceFontOtf", size, allowCache);
            
            public static Image ResourceFontTtc(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceFontTtc", size, allowCache);
            
            public static Image ResourceFontTtf(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceFontTtf", size, allowCache);
            
            public static Image ResourceFontWoff(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceFontWoff", size, allowCache);
            
            public static Image ResourceGis(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceGis", size, allowCache);
            
            public static Image ResourceHtml(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceHtml", size, allowCache);
            
            public static Image ResourceImage(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceImage", size, allowCache);
            
            public static Image ResourceJson(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceJson", size, allowCache);
            
            public static Image ResourceMap(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceMap", size, allowCache);
            
            public static Image ResourceMht(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceMht", size, allowCache);
            
            public static Image ResourcePdf(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourcePdf", size, allowCache);
            
            public static Image ResourceOdc(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceOdc", size, allowCache);
            
            public static Image ResourceOdt(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceOdt", size, allowCache);
            
            public static Image ResourcePowerPoint(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourcePowerPoint", size, allowCache);
            
            public static Image ResourceReport(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceReport", size, allowCache);
            
            public static Image ResourceReportSnapshot(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceReportSnapshot", size, allowCache);
            
            public static Image ResourceRtf(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceRtf", size, allowCache);
            
            public static Image ResourceSylk(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceSylk", size, allowCache);
            
            public static Image ResourceTxt(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceTxt", size, allowCache);
            
            public static Image ResourceWord(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceWord", size, allowCache);
            
            public static Image ResourceXml(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceXml", size, allowCache);
            
            public static Image ResourceXps(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceXps", size, allowCache);
            
            public static Image ResourceXsd(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("ResourceXsd", size, allowCache);
                        
            public static Image StoredProcedure(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("StoredProcedure", size, allowCache);

            public static Image StoredProcedures(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("StoredProcedures", size, allowCache);

            public static Image Variable(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("Variable", size, allowCache);
                                    
            public static Image VariableBinary(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableBinary", size, allowCache);

            public static Image VariableBool(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableBool", size, allowCache);

            public static Image VariableChar(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableChar", size, allowCache);

            public static Image VariableDateTime(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableDateTime", size, allowCache);

            public static Image VariableDecimal(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableDecimal", size, allowCache);

            public static Image VariableFloat(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableFloat", size, allowCache);
                                    
            public static Image VariableImage(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableImage", size, allowCache);

            public static Image VariableInt(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableInt", size, allowCache);

            public static Image VariableString(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetDictionaryImage("VariableString", size, allowCache);

        }
        #endregion

        #region class Engine
        public static class Engine
        {
            public static Image Close(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Close", size, allowCache);
            
            public static Image Collapsed(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Collapsed", size, allowCache);
                                    
            public static Image Condition(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Condition", size, allowCache);

            public static Image DropDown(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.DropDown", size, allowCache);

            public static Image Expanded(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Expanded", size, allowCache);

            public static Image Interactions(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Interactions", size, allowCache);
            
            public static Image Filter(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Filter", size, allowCache);
            
            public static Image Locked(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.Locked", size, allowCache);

            public static Image SortAsc(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.SortAsc", size, allowCache);
            
            public static Image SortDesc(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.SortDesc", size, allowCache);
            
            public static Image SpinDown(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.SpinDown", size, allowCache);
            
            public static Image SpinUp(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Engine.SpinUp", size, allowCache);
        }
        #endregion

        #region class Formats
        public static class Formats
        {
            public static Image Boolean(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Boolean", size, allowCache);

            public static Image Currency(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Currency", size, allowCache);

            public static Image Date(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Date", size, allowCache);

            public static Image DateAndTime(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.DateAndTime", size, allowCache);

            public static Image Format(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Format", size, allowCache);

            public static Image General(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.General", size, allowCache);

            public static Image Number(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Number", size, allowCache);

            public static Image Percentage(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Percentage", size, allowCache);

            public static Image Time(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Formats.Time", size, allowCache);

        }
        #endregion

        #region class PropertyGrid
        public static class PropertyGrid
        {
            public static Image Alphabetical(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("PropertyGrid.Alphabetical", size, allowCache);

            public static Image Categorized(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("PropertyGrid.Categorized", size, allowCache);

            public static Image Events(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("PropertyGrid.Events", size, allowCache);

            public static Image Properties(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("PropertyGrid.Properties", size, allowCache);
        }
        #endregion

        #region class Styles
        public static class Styles
        {
            public static Image Cards(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Cards", size, allowCache);

            public static Image Chart(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Chart", size, allowCache);

            public static Image CheckBox(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.CheckBox", size, allowCache);

            public static Image Image(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Image", size, allowCache);
                        
            public static Image Map(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Map", size, allowCache);

            public static Image Progress(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Progress", size, allowCache);

            public static Image RichText(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.RichText", size, allowCache);

            public static Image Style(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Style", size, allowCache);

            public static Image Text(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Styles.Text", size, allowCache);
        }
        #endregion

        #region Consts
        private const string RootPath = "Stimulsoft.Report.Images";
        #endregion

        #region Fields
        private static Hashtable mainCache = new Hashtable();
        private static Hashtable dictionaryCache = new Hashtable();
        #endregion

        #region Methods
        public static Image GetImage(string path, StiImageSize size = StiImageSize.Normal, 
            bool allowCache = true, StiErrorProcessing errorProcessing = StiErrorProcessing.Exception)
        {
            string imagePath;
            var regularImagePath = $"{RootPath}.{path}";

            if (StiUX.IconSet == StiUXIconSet.Regular)
                imagePath = regularImagePath;
            else
                imagePath = StiUX.IsDark ? $"{RootPath}Dark.{path}" : $"{RootPath}Light.{path}";

            var cachedImage = allowCache ? mainCache[$"{imagePath}{size}"] as Bitmap : null;
            if (cachedImage != null && !IsDisposedImage(cachedImage))
                return cachedImage;

            var assembly = typeof(StiReportImages).Assembly;
            var image = StiScaledImagesHelper.GetImage(assembly, imagePath, size, StiErrorProcessing.Null);
            if (image == null)
                image = StiScaledImagesHelper.GetImage(assembly, regularImagePath, size, errorProcessing);

            if (allowCache)
                mainCache[$"{imagePath}{size}"] = image;

            return image;
        }

        public static Image GetToolboxImage(string path, StiImageSize size = StiImageSize.Normal, bool allowCache = true)
        {
            return GetImage($"Components.{path}", size, allowCache, StiErrorProcessing.Null);
        }

        public static Image GetDictionaryImage(string path, StiImageSize size = StiImageSize.Normal, bool allowCache = true)
        {
            string imagePath;
            var regularImagePath = $"{RootPath}.Dictionary.{path}";

            if (StiUX.IconSet == StiUXIconSet.Regular)
                imagePath = regularImagePath;
            else
                imagePath = StiUX.IsDark ? $"{RootPath}Dark.Dictionary.{path}" : $"{RootPath}Light.Dictionary.{path}";

            var cachedImage = allowCache ? mainCache[$"{imagePath}{size}"] as Bitmap : null;
            if (cachedImage != null && !IsDisposedImage(cachedImage))
                return cachedImage;

            var assembly = typeof(StiReportImages).Assembly;
            var image = StiScaledImagesHelper.GetImage(assembly, imagePath, size, StiErrorProcessing.Null);
            if (image == null)
                image = StiScaledImagesHelper.GetImage(assembly, regularImagePath, size, StiErrorProcessing.Exception);

            if (allowCache)
                mainCache[$"{imagePath}{size}"] = image;

            return image;
        }

        private static bool IsDisposedImage(Image image)
        {
            try
            {
                return image.Width == 0;//Access to the property of the image to check its disposed state.
            }
            catch
            {
                return true;
            }
        }
        #endregion
    }
}