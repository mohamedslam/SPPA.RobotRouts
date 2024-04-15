#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Table;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using System.Reflection;
using Stimulsoft.Report.Engine;
using System.Globalization;
using Stimulsoft.Report.Labels;
using System.IO;
using Stimulsoft.Report.Units;
using System.Threading;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Helpers;
using System.Drawing;
using System.Drawing.Printing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiWizardHelper
    {
        #region classes
        public class VariableInfo
        {
            public VariableInfo(string name, string locName)
            {
                this.Name = name;
                this.LocName = locName;
            }

            public string Name;
            public string LocName;
        }
        #endregion

        private static Hashtable WizardData = new Hashtable();

        #region constants
        private static readonly string[] InvoiceReports = new string[]{"Stimulsoft.Report.Web.Designer.WizardReports.Invoice.Invoice.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Invoice.SalesInvoice.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Invoice.ServiceInvoice.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Invoice.BillingStatement.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Invoice.BusinessInvoice.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Invoice.RecurringInvoice.mrt"};

        private static readonly string[] OrderReports = new string[]{"Stimulsoft.Report.Web.Designer.WizardReports.Order.Order.mrt",
                                                 "Stimulsoft.Report.Web.Designer.WizardReports.Order.PurchaseOrder.mrt"};

        private static readonly string[] QuotationReports = new string[]{"Stimulsoft.Report.Web.Designer.WizardReports.Quotation.Quotation.mrt"};

        private static readonly string[] AllReports = InvoiceReports.Concat(OrderReports).Concat(QuotationReports).ToArray();
        #endregion

        private static ArrayList GetGroupsFromDataSource(string dataSourceName, Hashtable dataSources)
        {            
            Hashtable dataSource = (Hashtable)dataSources[dataSourceName];
            ArrayList groups = (ArrayList)dataSource["groups"];
            
            return groups;
        }

        private static ArrayList GetColumnsFromDataSource(string dataSourceName, Hashtable dataSources)
        {
            Hashtable dataSource = (Hashtable)dataSources[dataSourceName];
            ArrayList columns = (ArrayList)dataSource["columns"];

            return columns;
        }

        private static ArrayList GetSortFromDataSource(string dataSourceName, Hashtable dataSources)
        {
            Hashtable dataSource = (Hashtable)dataSources[dataSourceName];
            ArrayList sort = (ArrayList)dataSource["sort"];

            return sort;
        }

        private static ArrayList GetFiltersFromDataSource(string dataSourceName, Hashtable dataSources)
        {
            Hashtable dataSource = (Hashtable)dataSources[dataSourceName];
            ArrayList filters = (ArrayList)dataSource["filters"];

            return filters;
        }

        private static Hashtable GetTotalsFromDataSource(string dataSourceName, Hashtable dataSources)
        {
            Hashtable dataSource = (Hashtable)dataSources[dataSourceName];
            Hashtable totals = (Hashtable)dataSource["totals"];

            return totals;
        }
                
        private static double AlignToMaxGrid(StiPage page, double value, bool converted)
		{
			if (converted)value = page.Unit.ConvertFromHInches(value);
			return StiAlignValue.AlignToMaxGrid(value, 
				page.GridSize, page.Report.Info.AlignToGrid);
		}

        private static double AlignToGrid(StiPage page, double value, bool converted)
        {
            if (converted) value = page.Unit.ConvertFromHInches(value);
            return StiAlignValue.AlignToGrid(value,
                page.GridSize, page.Report.Info.AlignToGrid);
        }

        public static Hashtable GetWizardData(Hashtable param, StiRequestParams requestParams)
        {
            var reportType = param["reportType"] as string;

            lock (WizardData)
            {
                if (WizardData[reportType] != null)
                    return WizardData[reportType] as Hashtable;
                var result = new Hashtable();

                if (reportType == "Invoice" || reportType == "Order" || reportType == "Quotation")
                {
                    Assembly assembly = typeof(StiWizardHelper).Assembly;
                    var resources = reportType == "Invoice" ? InvoiceReports : reportType == "Order" ? OrderReports : QuotationReports;
                    var templates = new ArrayList();

                    foreach (var res in resources)
                    {
                        var template = new Hashtable();
                        var report = new StiReport();
                        using (var stream = assembly.GetManifestResourceStream(res))
                        {
                            report.Load(stream);
                        }

                        if (requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryNew) 
                        {
                            RemoveDemoDataFormDictionary(report);
                        }

                        report.Render(false);
                        using (var preview = report.RenderedPages[0].GetThumbnail(300, 390))
                        {
                            template["thumbnail"] = StiImageConverter.ImageToString(preview);
                        }

                        template["name"] = report.ReportName;
                        template["templateFileName"] = res;

                        var datasources = new ArrayList();
                        foreach (StiDataSource datasource in report.Dictionary.DataSources)
                        {
                            var dsHash = new Hashtable
                            {
                                ["name"] = datasource.ToString(),
                                ["category"] = datasource.GetCategoryName(),
                                ["typeItem"] = "DataSource"
                            };

                            var columns = new ArrayList();
                            foreach (StiDataColumn column in datasource.Columns)
                            {
                                var columnHash = new Hashtable
                                {
                                    ["name"] = column.Name
                                };
                                columns.Add(columnHash);
                            }
                            dsHash["columns"] = columns;

                            datasources.Add(dsHash);
                        }
                        template["datasources"] = datasources;

                        var bandComps = report.Pages[0].GetComponentsList().Where(x => x is StiText && x.Interaction.Tag.Value.Contains("DATA")).ToList();
                        var dataBand = bandComps[0].Parent as StiDataBand;

                        var builder = new StiDataBandV1Builder();
                        var headers = builder.GetHeaders(dataBand);

                        var headerBand = (StiHeaderBand)headers[headers.Count - 1];
                        var headerComps = new ArrayList();
                        foreach (StiComponent comp in headerBand.Components)
                        {
                            if (comp is StiText textComp)
                            {
                                var headerComp = new Hashtable
                                {
                                    ["name"] = textComp.Name,
                                    ["text"] = textComp.Text.Value
                                };
                                headerComps.Add(headerComp);
                            }
                        }
                        template["headerComps"] = headerComps;

                        var variables = new ArrayList();
                        var reportVariables = report.Dictionary.Variables.ToList();
                        foreach (var info in GetCompanyInfoVariables())
                        {
                            var variable = reportVariables.FirstOrDefault(x => x.Name == info.Name);
                            if (variable != null)
                            {
                                var variableHash = new Hashtable();
                                variableHash["name"] = info.Name;
                                variableHash["locName"] = info.LocName;
                                variableHash["value"] = variable.Value;
                                variables.Add(variableHash);
                            }
                        }
                        template["variables"] = variables;

                        foreach (StiResource reportResource in report.Dictionary.Resources)
                        {
                            if (reportResource.Type == StiResourceType.Image && reportResource.Name == "Logo")
                            {
                                using (var image = StiImageConverter.TryBytesToImage(reportResource.Content))
                                {
                                    if (image != null)
                                        template["logo"] = StiImageConverter.ImageToString(image);
                                }
                                break;
                            }
                        }

                        //languages
                        var strings = report.GlobalizationStrings.ToList();
                        var languages = new ArrayList();
                        languages.Add(StiLocalization.Get("Export", "Auto"));

                        var en = strings.FirstOrDefault(x => x.CultureName.ToLowerInvariant().StartsWithInvariant("en"));
                        var de = strings.FirstOrDefault(x => x.CultureName.ToLowerInvariant().StartsWithInvariant("de"));
                        var ru = strings.FirstOrDefault(x => x.CultureName.ToLowerInvariant().StartsWithInvariant("ru"));

                        if (en != null)
                        {
                            strings.Remove(en);
                            languages.Add(en.CultureName);
                        }
                        if (de != null)
                        {
                            strings.Remove(de);
                            languages.Add(de.CultureName);
                        }
                        if (ru != null)
                        {
                            strings.Remove(ru);
                            languages.Add(ru.CultureName);
                        }

                        foreach (StiGlobalizationContainer container in strings)
                            languages.Add(container.CultureName);

                        template["languages"] = languages;

                        //themes
                        var styles = new List<StiStyle>();
                        var stylesCollection = new List<string>();
                        var themes = new ArrayList();

                        foreach (var value in report.Styles)
                        {
                            var style = value as StiStyle;
                            if (style == null) continue;

                            if (string.IsNullOrEmpty(style.CollectionName))
                            {
                                styles.Add(style);
                            }
                            else
                            {
                                if (!stylesCollection.Contains(style.CollectionName))
                                    stylesCollection.Add(style.CollectionName);
                            }
                        }

                        if (stylesCollection.Count > 0 && report.Pages.Count > 0)
                        {
                            var cloneReport = (StiReport)report.Clone();

                            foreach (var name in stylesCollection)
                            {
                                var style = report.Styles[name + "Sample"] as StiStyle;
                                if (style != null)
                                {
                                    cloneReport.ApplyStyleCollection(name);

                                    string desc = (string.IsNullOrEmpty(style.Description))
                                        ? name
                                        : style.Description;
                                    
                                    var page = cloneReport.Pages[0];

                                    var scalingFactor = param["scalingFactor"] != null ? StiReportEdit.StrToDouble(param["scalingFactor"] as string) : 1;

                                    using (var preview = page.GetThumbnail((int)(150 * scalingFactor), (int)(200 * scalingFactor)))
                                    {
                                        var theme = new Hashtable();
                                        theme["name"] = name;
                                        theme["desc"] = desc;
                                        theme["img"] = StiImageConverter.ImageToString(preview);
                                        themes.Add(theme);
                                    }
                                }
                            }
                        }
                        template["themes"] = themes;

                        templates.Add(template);
                    }

                    result["templates"] = templates;
                }else if (reportType == "Label")
                {
                    result["isMetrics"] = RegionInfo.CurrentRegion.IsMetric;

                    var templates = new ArrayList();
                    templates.Add(new Hashtable
                    {
                        ["name"] = "Label",
                        ["templateFileName"] = "Label"
                    });
                    result["templates"] = templates;

                    var paperItems = new ArrayList();
                    foreach (object obj in StiOptions.Designer.Wizards.LabelPageSizes)
                        paperItems.Add(((StiPaperLabelInfo)obj).Clone());
                    paperItems.Add(new StiPaperLabelInfo(StiLocalization.Get("Wizards", "Custom"), StiPageOrientation.Portrait, 0, 0));
                    paperItems.Add(new StiPaperLabelInfo("A4", StiPageOrientation.Portrait, 21, 29.7));
                    paperItems.Add(new StiPaperLabelInfo("A4", StiPageOrientation.Landscape, 29.7, 21));
                    paperItems.Add(new StiPaperLabelInfo("A5", StiPageOrientation.Portrait, 14.8, 21));
                    paperItems.Add(new StiPaperLabelInfo("A5", StiPageOrientation.Landscape, 21, 14.8));
                    paperItems.Add(new StiPaperLabelInfo("Letter", StiPageOrientation.Portrait, 21.59, 27.94));
                    paperItems.Add(new StiPaperLabelInfo("Letter", StiPageOrientation.Landscape, 27.94, 21.59));
                    paperItems.Add(new StiPaperLabelInfo("B5", StiPageOrientation.Portrait, 18.2, 25.70));
                    paperItems.Add(new StiPaperLabelInfo("B5", StiPageOrientation.Landscape, 25.70, 18.2));
                    paperItems.Add(new StiPaperLabelInfo("Mini", StiPageOrientation.Portrait, 10.795, 12.7));
                    paperItems.Add(new StiPaperLabelInfo("Vertical Half Sheet", StiPageOrientation.Portrait, 10.795, 25.4));
                    paperItems.Add(new StiPaperLabelInfo("Vertical Half Sheet", StiPageOrientation.Landscape, 25.4, 10.795));
                    paperItems.Add(new StiPaperLabelInfo("Hagaki", StiPageOrientation.Portrait, 10, 14.8));
                    paperItems.Add(new StiPaperLabelInfo("Hagaki", StiPageOrientation.Landscape, 14.8, 10));
                    paperItems.Add(new StiPaperLabelInfo("B4 JIS", StiPageOrientation.Portrait, 25.7, 36.4));


                    var paper = new Hashtable();
                    var label = new Hashtable();
                    var units = new StiUnit[] { new StiCentimetersUnit(), new StiMillimetersUnit(), new StiInchesUnit(), new StiHundredthsOfInchUnit() };
                    var oLabels = GetLabels();
                    foreach (StiUnit unit in units)
                    {
                        var papers = new ArrayList();
                        foreach (StiPaperLabelInfo item in paperItems)
                        {
                            item.Unit = unit;
                            papers.Add(new Hashtable
                            {
                                ["name"] = item.Name,
                                ["orientation"] = item.Orientation,
                                ["width"] = item.Width,
                                ["height"] = item.Height,
                                ["caption"] = item.ToString()
                            });
                        }
                        paper[unit.ShortName] = papers;                        

                        var labels = new ArrayList();
                        foreach (var lb in oLabels)
                        {
                            lb.Unit = unit;
                            labels.Add(new Hashtable
                            {
                                ["caption"] = lb.LabelName != "Custom" ? $"{lb.Manufacturer}-{lb.ToString()}" : "Custom",
                                ["labelName"] = lb.LabelName,
                                ["labelWidth"] = lb.LabelWidth,
                                ["labelHeight"] = lb.LabelHeight,
                                ["leftMargin"] = lb.LeftMargin,
                                ["topMargin"] = lb.TopMargin,
                                ["horizontalGap"] = lb.HorizontalGap,
                                ["verticalGap"] = lb.VerticalGap,
                                ["paperWidth"] = lb.PaperWidth,
                                ["paperHeight"] = lb.PaperHeight,
                                ["numberOfColumns"] = lb.NumberOfColumns,
                                ["numberOfRows"] = lb.NumberOfRows
                            });
                        }
                        label[unit.ShortName] = labels;
                    }
                    result["paper"] = paper;
                    result["label"] = label;

                }
                WizardData[reportType] = result;
                return result;
            }
            
        }

        public static StiReport GetReportFromWizardOptions2(StiReport createdReport, Hashtable reportOptions, StiRequestParams requestParams)
        {
            var typeReport = reportOptions["typeReport"] as string;
            if (typeReport == "Invoice" || typeReport == "Order" || typeReport == "Quotation")
            {
                Assembly assembly = typeof(StiWizardHelper).Assembly;
                var report = new StiReport();
                using (var stream = assembly.GetManifestResourceStream(AllReports.FirstOrDefault(x => x.EndsWith(reportOptions["templateFileName"] as string))))
                {
                    report.Load(stream);
                }

                if (requestParams.Designer.NewReportDictionary == StiNewReportDictionary.DictionaryNew)
                {
                    RemoveDemoDataFormDictionary(report);
                }

                InitDictionary(report, createdReport);

                #region Mapping
                var mapping = reportOptions["mapping"] as Hashtable;
                var bandComps = report.Pages[0].GetComponentsList().Where(x => x is StiText && x.Interaction.Tag.Value.Contains("DATA")).ToList();
                var dataBand = bandComps[0].Parent as StiDataBand;

                var datasourceName = reportOptions["datasourceName"] as string;
                if (datasourceName != null)
                    dataBand.DataSourceName = datasourceName;

                var builder = new StiDataBandV1Builder();
                var headers = builder.GetHeaders(dataBand);

                var headerComps = ((StiHeaderBand)headers[headers.Count - 1]).Components.ToList().Where(x => x is StiText).OrderBy(x => x.Name).ToList();
                for (var i = 0; i < bandComps.Count; i++)
                    if (mapping[headerComps[i].Name] != null)
                        (bandComps[i] as StiText).Text = mapping[headerComps[i].Name] as string;
                    else
                    {
                        (bandComps[i] as StiText).Text = string.Empty;
                        (headerComps[i] as StiText).Text = string.Empty;
                    }

                #endregion
                #region Theme
                var themeName = reportOptions["theme"] as string;
                for (int index = 0; index < report.Styles.Count;)
                {
                    var style = report.Styles[index] as StiStyle;
                    if (style == null || style.CollectionName == themeName)
                    {
                        index++;
                        continue;
                    }
                    report.Styles.RemoveAt(index);
                }

                report.ApplyStyleCollection(themeName);
                #endregion
                #region GlobalizationStrings
                var language = reportOptions["language"] as string;
                if (StiLocalization.Get("Export", "Auto") != language)
                {
                    report.LocalizeReport(language);
                    report.GlobalizationStrings.Clear();
                    report.AutoLocalizeReportOnRun = false;
                }
                #endregion
                #region Variables
                var company = reportOptions["company"] as ArrayList;
                if (company.Count > 0)
                    foreach (Hashtable pair in company)
                        report.Dictionary.Variables[pair["name"] as string].Value = pair["value"] as string;
                #endregion
                #region Logo
                var logo = reportOptions["logo"] as string;
                if (logo != null && logo.StartsWith("data:image/png;base64,"))
                    logo = logo.Substring("data:image/png;base64,".Length);
                foreach (StiResource res in report.Dictionary.Resources)
                {
                    if (res.Type == StiResourceType.Image && res.Name == "Logo")
                    {
                        res.Content = StiImageConverter.StringToByteArray(logo);
                        break;
                    }
                }
                #endregion
                return report;
            }
            else if (typeReport == "Label")
            {
                var report = new StiReport();
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    InitDictionary(report, createdReport);
                    var unit = reportOptions["unit"] as string;
                    if (unit == "mm") report.ReportUnit = StiReportUnitType.Millimeters;
                    else if (unit == "cm") report.ReportUnit = StiReportUnitType.Centimeters;
                    else if (unit == "in") report.ReportUnit = StiReportUnitType.Inches;
                    else if (unit == "hi") report.ReportUnit = StiReportUnitType.HundredthsOfInch;

                    double dataBandHeight = double.Parse(reportOptions["labelHeight"] as string) + double.Parse(reportOptions["labelVertical"] as string);

                    var page = report.GetCurrentPage();

                    double pageWidth = double.Parse(reportOptions["paperWidth"] as string);
                    double pageHeight = double.Parse(reportOptions["paperHeight"] as string);
                    if (pageWidth > pageHeight) page.Orientation = StiPageOrientation.Landscape;

                    page.PageWidth = pageWidth;
                    page.PageHeight = pageHeight;
                    double marginsLeft = double.Parse(reportOptions["labelLeft"] as string);
                    double marginsRight = double.Parse(reportOptions["labelLeft"] as string);
                    double marginsTop = double.Parse(reportOptions["labelTop"] as string);
                    double marginsBottom = page.PageHeight - marginsTop - dataBandHeight * Convert.ToInt16(reportOptions["numericNumberOfRows"]) - page.PageHeight * 0.01;

                    page.Margins = new StiMargins(marginsLeft, marginsRight, marginsTop, marginsBottom);

                    #region Create Data Band

                    var dataBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.DataBand) as StiDataBand;
                    dataBand.Name = "LabelsDataBand";

                    var datasourceName = reportOptions["datasourceName"] as string;
                    if (datasourceName != null)
                        dataBand.DataSourceName = datasourceName;
                    else
                        dataBand.CountData = Convert.ToInt16(reportOptions["numericNumberOfColumns"]) * Convert.ToInt16(reportOptions["numericNumberOfRows"]);

                    dataBand.Height = double.Parse(reportOptions["labelHeight"] as string);
                    dataBand.ColumnWidth = double.Parse(reportOptions["labelWidth"] as string);
                    dataBand.ColumnGaps = double.Parse(reportOptions["labelHorizontal"] as string);
                    dataBand.Columns = Convert.ToInt16(reportOptions["numericNumberOfColumns"]);
                    dataBand.ColumnDirection = (reportOptions["direction"] as string) == "0" ?
                        StiColumnDirection.AcrossThenDown : StiColumnDirection.DownThenAcross;

                    page.Components.Add(dataBand);

                    var childBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.ChildBand) as StiChildBand;
                    childBand.Name = "LabelsChildBand";
                    childBand.Height = double.Parse(reportOptions["labelVertical"] as string);
                    if (childBand.Height != 0) page.Components.Add(childBand);

                    var container = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.ComplexComponents.Panel) as StiPanel;
                    container.Name = "Label_Area";
                    container.Border.Side = StiBorderSides.All;
                    container.Width = double.Parse(reportOptions["labelWidth"] as string);
                    container.Height = double.Parse(reportOptions["labelHeight"] as string);
                    dataBand.Components.Add(container);

                    #endregion

                    report.IsModified = true;
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                }

                return report;
            }
            return null;
        }

        public static StiReport GetReportFromWizardOptions(StiReport createdReport, Hashtable reportOptions, Hashtable wizardDataSources, StiRequestParams requestParams)
        {
            if (reportOptions["typeReport"] != null)
                return GetReportFromWizardOptions2(createdReport, reportOptions, requestParams);

		    #region Fields

            ArrayList sorts = new ArrayList();
		    ArrayList filters = new ArrayList();
		    ArrayList totals = new ArrayList();
            ArrayList orderDataNames = (ArrayList)reportOptions["dataSourcesOrder"];
            Hashtable relations = (Hashtable)reportOptions["relations"];

            #endregion

            #region Set Report Language

            createdReport.ScriptLanguage = (string)reportOptions["language"] == "C" ? StiReportLanguageType.CSharp : StiReportLanguageType.VB;

            #endregion

            #region Set report unit

            StiReportEdit.ChangeUnit(createdReport, (string)reportOptions["unit"]);

            #endregion
            
            #region Set report orientation

            StiPage page = createdReport.GetCurrentPage();
            page.Orientation = (string)reportOptions["orientation"] == "Portrait" ? StiPageOrientation.Portrait : StiPageOrientation.Landscape;

            #endregion
            
            Hashtable businessObjects = new Hashtable();
            StiDataSourcesCollection dataSources = new StiDataSourcesCollection(null);

            foreach (DictionaryEntry wizardDataSource in wizardDataSources)
            {
                if (((Hashtable)wizardDataSource.Value)["typeItem"] as string == "BusinessObject")
                {
                    var fullName = wizardDataSource.Key as string;
                    ArrayList nameArray = new ArrayList();
                    nameArray.AddRange(fullName.Split('.'));
                    nameArray.Reverse();
                    StiBusinessObject businessObject = StiDictionaryHelper.GetBusinessObjectByFullName(createdReport, nameArray);
                    if (businessObject != null) businessObjects[fullName] = businessObject;
                }
                else
                {
                    StiDataSource dataSource = createdReport.DataSources[(string)wizardDataSource.Key];
                    if (dataSource != null) dataSources.Add(dataSource);
                }
            }
            if (dataSources.Count == 0 && businessObjects.Count == 0) return createdReport;

            bool first = true;
            string titleText = string.Empty;
            foreach (string orderDataName in orderDataNames)
            {
                StiDataSource dataSource = dataSources[orderDataName];
                StiBusinessObject businessObject = businessObjects[orderDataName] as StiBusinessObject;

                if (dataSource != null || businessObject != null)
                {
                    if (!first) titleText += ", ";
                    titleText += dataSource != null ? dataSource.Alias : businessObject.Alias;
                    first = false;
                }
            }

            #region Create Report Title
            StiReportTitleBand reportTitleBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.ReportTitleBand) as StiReportTitleBand;
            reportTitleBand.Name = "ReportTitle";
            reportTitleBand.Height = AlignToMaxGrid(page, 50, true);
            page.Components.Add(reportTitleBand);

            StiText reportTitleText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
            reportTitleText.HorAlignment = StiTextHorAlignment.Center;
            reportTitleText.VertAlignment = StiVertAlignment.Center;
            reportTitleText.Font = new Font("Arial", 20, FontStyle.Bold);
            reportTitleText.Name = "ReportTitleText";
            reportTitleText.Text = titleText;
            reportTitleText.Top = 0;
            reportTitleText.Left = 0;
            reportTitleText.Width = page.Width;
            reportTitleText.Height = reportTitleBand.Height;
            reportTitleBand.Components.Add(reportTitleText);
            #endregion
			
            StiDataBand masterDataband = null;

            #region Get all aggregates functions
            var aggrs = new Hashtable();
            foreach (var service in StiOptions.Services.AggregateFunctions.Where(s => s.ServiceEnabled))
            {
                aggrs[service.ServiceName] = service;
            }
            #endregion

            #region DataSources
            int dataSourceIndex = 0;
            foreach (string orderDataName in orderDataNames)
            {
                StiDataSource dataSource = dataSources[orderDataName];
                StiBusinessObject businessObject = businessObjects[orderDataName] as StiBusinessObject;
                if (dataSource == null && businessObject == null) return createdReport;

                StiHeaderBand headerBand = null;
                StiDataBand dataBand = null;
                StiTable table = null;
                StiFooterBand footerBand = null;

                string dataValue = dataSource != null
                    ? StiNameValidator.CorrectName(dataSource.Name) 
                    : businessObject != null 
                        ? StiNameValidator.CorrectBusinessObjectName(businessObject.GetFullName())
                        : null;

                string dataName = (orderDataNames.Count == 1) ? string.Empty : StiNameValidator.CorrectName(dataValue);

                ArrayList columnItems = GetColumnsFromDataSource(orderDataName, wizardDataSources);
                ArrayList groupItems = GetGroupsFromDataSource(orderDataName, wizardDataSources);
                Hashtable totalItems = GetTotalsFromDataSource(orderDataName, wizardDataSources);

                #region Create Header Band
                if ((string)reportOptions["componentType"] == "Data")
                {
                    headerBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.HeaderBand) as StiHeaderBand;
                    headerBand.Name = "Header" + dataName;
                    headerBand.Height = AlignToMaxGrid(page, 20, true);                    
                    page.Components.Add(headerBand);
                }
                #endregion				

                #region Create GroupHeaderBand
                for (int index = 0; index < groupItems.Count; index++)
                {
                    StiGroupHeaderBand groupHeaderBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.GroupHeaderBand) as StiGroupHeaderBand;
                    groupHeaderBand.Name =
                        string.Format("GroupHeader{0}{1}", dataName, index);
                    groupHeaderBand.Height = AlignToMaxGrid(page, 30, true);
                    groupHeaderBand.Condition.Value = "{" + dataValue + "." + groupItems[index] as string + "}";
                    page.Components.Add(groupHeaderBand);

                    StiText groupHeaderText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                    groupHeaderText.Text = "{" + dataValue + "." + groupItems[index] as string + "}";
                    groupHeaderText.VertAlignment = StiVertAlignment.Center;
                    groupHeaderText.Font = new Font("Arial", 16, FontStyle.Bold);
                    groupHeaderText.Name =
                        string.Format("GroupHeaderText{0}{1}", dataName, index);
                    groupHeaderText.Top = 0;
                    groupHeaderText.Left = 0;
                    groupHeaderText.Width = page.Width;
                    groupHeaderText.Height = groupHeaderBand.Height;
                    groupHeaderText.CanGrow = true;
                    groupHeaderText.WordWrap = true;
                    groupHeaderBand.Components.Add(groupHeaderText);
                }
                #endregion
                
                #region Create Data Band
                if ((string)reportOptions["componentType"] == "Data")
                {
                    dataBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.DataBand) as StiDataBand;                                        
                    dataBand.Name = "Data" + dataName;
                    if (dataSource != null)
                        dataBand.DataSourceName = dataSource.Name;
                    else if (businessObject != null)
                        dataBand.BusinessObjectGuid = businessObject.Guid;
                    dataBand.Height = (AlignToMaxGrid(page, 20, true));
                    dataBand.MasterComponent = masterDataband;                    
                    page.Components.Add(dataBand);
                    StiReportEdit.SetSortDataProperty(dataBand, GetSortFromDataSource(orderDataName, wizardDataSources));
                    
                    #region Set relation
                    if (relations[orderDataName] != null)
                        if ((bool)((Hashtable)relations[orderDataName])["checked"])
                            dataBand.DataRelationName = (string)((Hashtable)relations[orderDataName])["nameInSource"];
                    #endregion

                    if (dataSourceIndex == 0)
                        masterDataband = dataBand;
                }
                else
                {
                    table = new StiTable();
                    table.Name = "Data" + dataName;
                    table.RowCount = 3;
                    table.HeaderRowsCount = 1;
                    table.FooterRowsCount = 1;
                    table.ColumnCount = columnItems.Count;
                    if (dataSource != null)
                        table.DataSourceName = dataSource.Name;
                    else if (businessObject != null)
                        table.BusinessObjectGuid = businessObject.Guid;
                    page.Components.Add(table);
                    table.Height = (page.GridSize * 9);
                    table.Width = page.Width;
                    table.MasterComponent = masterDataband;
                    table.AutoSizeCells();
                    StiReportEdit.SetSortDataProperty(table, GetSortFromDataSource(orderDataName, wizardDataSources));
                    
                    #region Set relation
                    if (relations[orderDataName] != null)
                        if ((bool)((Hashtable)relations[orderDataName])["checked"])
                            table.DataRelationName = (string)((Hashtable)relations[orderDataName])["nameInSource"];
                    #endregion

                    if (dataSourceIndex == 0)
                        masterDataband = table;
                }
                #endregion

                #region Create Filters
                bool filterOn = (bool)(((Hashtable)wizardDataSources[orderDataName])["filterOn"]);
                StiFilterMode filterMode = ((string)(((Hashtable)wizardDataSources[orderDataName])["filterMode"]) == "And") ? StiFilterMode.And : StiFilterMode.Or;
                StiFilterEngine filterEngine = (StiFilterEngine)Enum.Parse(typeof(StiFilterEngine), ((Hashtable)wizardDataSources[orderDataName])["filterEngine"] as string);

                if ((string)reportOptions["componentType"] == "Data")
                {
                    dataBand.FilterMode = filterMode;
                    StiReportEdit.SetFilterDataProperty(dataBand, GetFiltersFromDataSource(orderDataName, wizardDataSources));
                    dataBand.FilterOn = filterOn;
                    dataBand.FilterEngine = filterEngine;
                }
                else
                {
                    table.FilterMode = filterMode;
                    StiReportEdit.SetFilterDataProperty(table, GetFiltersFromDataSource(orderDataName, wizardDataSources));
                    table.FilterOn = filterOn;
                    table.FilterEngine = filterEngine;
                }
                #endregion
                
                #region Create GroupFooterBand
                for (int index = 0; index < groupItems.Count; index++)
                {
                    StiGroupFooterBand groupFooterBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.GroupFooterBand) as StiGroupFooterBand;
                    groupFooterBand.Name = 
                        string.Format("GroupFooter{0}{1}", dataName, index);
                    groupFooterBand.Height = AlignToMaxGrid(page, 10, true);
                    page.Components.Add(groupFooterBand);
                }
                #endregion
			
                #region Create Footer Band
                if ((string)reportOptions["componentType"] == "Data")
                {
                    footerBand = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.Bands.FooterBand) as StiFooterBand;
                    footerBand.Name = "Footer" + dataName;
                    footerBand.Height = AlignToMaxGrid(page, 20, true);
                    page.Components.Add(footerBand);
                }
                #endregion

                #region Create columns
                if (columnItems.Count > 0)
                {
                    double columnWidth = AlignToGrid(page, page.Width / columnItems.Count, false);
                    if (columnWidth * columnItems.Count > page.Width && columnWidth > createdReport.Info.GridSize * 2)
                    {
                        columnWidth -= createdReport.Info.GridSize;
                    }
                    double pos = 0;

                    int indexHeaderText = 1;
                    int indexDataText = 1;
                    int indexFooterText = 1;

                    if ((string)reportOptions["componentType"] == "Table")
                    {
                        indexHeaderText = 0;
                        indexDataText = table.ColumnCount;
                        indexFooterText = table.ColumnCount * 2;
                    }

                    for (int indexColum = 0; indexColum < columnItems.Count; indexColum++)
                    {
                        string columnFullName = dataValue + "." + columnItems[indexColum];
                        double width = columnWidth;
                        if (indexColum + 1 == columnItems.Count)
                        {
                            width = AlignToGrid(page, page.Width - pos, false);
                            if (width <= 0) width = page.GridSize;
                        }

                        #region DataBand
                        if ((string)reportOptions["componentType"] == "Data")
                        {
                            #region HeaderText
                            StiText headerText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                            headerText.VertAlignment = StiVertAlignment.Center;
                            headerText.Font = new Font("Arial", 10, FontStyle.Bold);
                            headerText.Name = string.Format("HeaderText{0}{1}", dataName, indexHeaderText);
                            headerText.Top = 0;
                            headerText.Left = pos;
                            headerText.Width = width;
                            headerText.Height = headerBand.Height;
                            headerText.CanGrow = true;
                            headerText.WordWrap = true;
                            if (columnItems.Count > 1) headerText.GrowToHeight = true;
                            
                            int index = columnFullName.LastIndexOf('.');

                            StiDataColumn column = StiDataPathFinder.GetColumnFromPath(
                                columnFullName, createdReport.Dictionary);
                            if (column != null)
                            {
                                headerText.Text = column.Alias;
                            }
                            else
                            {
                                if (index == -1) headerText.Text = columnFullName;
                                else headerText.Text = columnFullName.Substring(index + 1);
                            }

                            indexHeaderText++;
                            headerBand.Components.Add(headerText);
                            #endregion
                            
                            #region Data
                            StiDataColumn dataColumn = StiDataPathFinder.GetColumnFromPath(columnFullName, dataBand.Report.Dictionary);
                            #region Image
                            if (dataColumn != null && (dataColumn.Type == typeof(Image) || dataColumn.Type == typeof(byte[])))
                            {
                                StiImage dataImage = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Image) as StiImage;
                                dataImage.Name = string.Format("DataImage{0}{1}", dataName, indexDataText);
                                dataImage.Top = 0;
                                dataImage.Left = pos;
                                dataImage.Width = width;
                                dataImage.Height = dataBand.Height;
                                dataImage.DataColumn = columnFullName;
                                dataImage.CanGrow = true;
                                if (columnItems.Count > 1) dataImage.GrowToHeight = true;
                                dataBand.Components.Add(dataImage);
                            }
                            #endregion

                            #region CheckBox
                            else if (dataColumn != null && dataColumn.Type == typeof(bool))
                            {
                                Stimulsoft.Report.Components.StiCheckBox dataCheck = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.CheckBox) as Stimulsoft.Report.Components.StiCheckBox;
                                dataCheck.Name = string.Format("DataCheck{0}{1}", dataName, indexDataText);
                                dataCheck.Top = 0;
                                dataCheck.Left = pos;
                                dataCheck.Width = width;
                                dataCheck.Height = dataBand.Height;
                                dataCheck.Checked.Value = "{" + columnFullName + "}";
                                if (columnItems.Count > 1) dataCheck.GrowToHeight = true;
                                dataBand.Components.Add(dataCheck);
                            }
                            #endregion

                            #region Text
                            else
                            {
                                StiText dataText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                                dataText.Name = string.Format("DataText{0}{1}", dataName, indexDataText);
                                dataText.TextQuality = StiTextQuality.Wysiwyg;
                                dataText.VertAlignment = StiVertAlignment.Center;
                                dataText.Top = 0;
                                dataText.Left = pos;
                                dataText.Width = width;
                                dataText.Height = dataBand.Height;
                                dataText.Text = "{" + columnFullName as string + "}";
                                dataText.CanGrow = true;
                                if (columnItems.Count > 1) dataText.GrowToHeight = true;
                                dataText.WordWrap = true;
                                dataBand.Components.Add(dataText);
                            }
                            #endregion

                            indexDataText++;
                            #endregion

                            #region FooterText
                            string totalStr = string.Empty;
                            if (totalItems.Count > 0 && totalItems[columnItems[indexColum]] != null)
                            {
                                totalStr = (string)totalItems[columnItems[indexColum]];
                                StiAggregateFunctionService service = aggrs[totalStr] as
                                    StiAggregateFunctionService;

                                if (service != null && service.RecureParam) totalStr += "(" + columnFullName + ")";
                                else totalStr += "()";

                                StiText footerText = StiActivator.CreateObject(StiOptions.Designer.ComponentsTypes.SimpleComponents.Text) as StiText;
                                footerText.VertAlignment = StiVertAlignment.Center;
                                footerText.HorAlignment = StiTextHorAlignment.Right;
                                footerText.Font = new Font("Arial", 10, FontStyle.Bold);
                                footerText.Name = string.Format("FooterText{0}{1}", dataName, indexFooterText);
                                footerText.Top = 0;
                                footerText.Left = pos;
                                footerText.Width = width;
                                footerText.Height = footerBand.Height;
                                footerText.CanGrow = true;
                                if (columnItems.Count > 1) footerText.GrowToHeight = true;
                                footerText.WordWrap = true;

                                footerText.Text = "{" + totalStr + "}";
                                indexFooterText++;
                                footerBand.Components.Add(footerText);
                            }
                            #endregion
                        }
                        #endregion

                        #region Table
                        else
                        {
                            #region HeaderCell
                            StiTableCell headerCell = (StiTableCell)table.Components[indexHeaderText];
                            headerCell.VertAlignment = StiVertAlignment.Center;
                            headerCell.Font = new Font("Arial", 10, FontStyle.Bold);
                            headerCell.WordWrap = true;

                            int index = columnFullName.LastIndexOf('.');

                            StiDataColumn column = StiDataPathFinder.GetColumnFromPath(
                                columnFullName, createdReport.Dictionary);
                            if (column != null)
                            {
                                headerCell.Text = column.Alias;
                            }
                            else
                            {
                                if (index == -1) headerCell.Text = columnFullName;
                                else headerCell.Text = columnFullName.Substring(index + 1);
                            }

                            indexHeaderText++;
                            #endregion

                            #region DataCell
                            StiDataColumn dataColumn = StiDataPathFinder.GetColumnFromPath(columnFullName, table.Report.Dictionary);

                            #region Image
                            if (dataColumn != null && (dataColumn.Type == typeof(Image) || dataColumn.Type == typeof(byte[])))
                            {
                                ((IStiTableCell)table.Components[indexDataText]).CellType = StiTablceCellType.Image;
                                StiTableCellImage dataCell = (StiTableCellImage)table.Components[indexDataText];
                                dataCell.VertAlignment = StiVertAlignment.Center;
                                dataCell.CanGrow = true;
                                dataCell.DataColumn = columnFullName;
                            }
                            #endregion

                            #region CheckBox
                            else if (dataColumn != null && dataColumn.Type == typeof(bool))
                            {
                                ((IStiTableCell)table.Components[indexDataText]).CellType = StiTablceCellType.CheckBox;
                                StiTableCellCheckBox dataCell = (StiTableCellCheckBox)table.Components[indexDataText];
                                dataCell.CanGrow = true;
                                dataCell.Checked.Value = "{" + columnFullName + "}";
                            }
                            #endregion

                            #region Text
                            else
                            {
                                StiTableCell dataCell = (StiTableCell)table.Components[indexDataText];
                                dataCell.CanGrow = true;
                                dataCell.VertAlignment = StiVertAlignment.Center;
                                dataCell.Text = "{" + columnFullName as string + "}";
                                dataCell.WordWrap = true;
                            }
                            #endregion

                            indexDataText++;
                            #endregion

                            #region FooterCell
                            string totalStr = string.Empty;
                            if (totalItems.Count > 0 && totalItems[columnItems[indexColum]] != null)
                            {
                                totalStr = (string)totalItems[columnItems[indexColum]];
                                StiAggregateFunctionService service = aggrs[totalStr] as
                                    StiAggregateFunctionService;

                                if (service != null && service.RecureParam) totalStr += "(" + columnFullName + ")";
                                else totalStr += "()";

                                StiTableCell footerCell = (StiTableCell)table.Components[indexFooterText];
                                footerCell.VertAlignment = StiVertAlignment.Center;
                                footerCell.HorAlignment = StiTextHorAlignment.Right;
                                footerCell.Font = new Font("Arial", 10, FontStyle.Bold);
                                footerCell.WordWrap = true;

                                footerCell.Text = "{" + totalStr + "}";
                                indexFooterText++;
                            }
                            #endregion
                        }
                        #endregion
                        pos += columnWidth;
                    }
                }
                #endregion

                dataSourceIndex++;
            }
            #endregion

            #region Aplly styles collection
            string[] theme = ((string)reportOptions["theme"]).Split('_');
            Color colorBase = Color.Empty;
            string themeName = string.Empty;
            string themeColor = theme[0];
            
            if (themeColor != "None")
            {
                int themePercent = int.Parse(theme[1]);
                switch (theme[0])
                {
                    case "Red": { colorBase = Color.FromArgb(144, 60, 57); themeName = StiLocalization.Get("PropertyColor", "Red"); break; }
                    case "Green": { colorBase = Color.FromArgb(117, 140, 72); themeName = StiLocalization.Get("PropertyColor", "Green"); break; }
                    case "Blue": { colorBase = Color.FromArgb(69, 98, 135); themeName = StiLocalization.Get("PropertyColor", "Blue"); break; }
                    case "Gray": { colorBase = Color.FromArgb(75, 75, 75); themeName = StiLocalization.Get("PropertyColor", "Gray"); break; }
                }

                StiStylesCreator creator = new StiStylesCreator(createdReport);
                string text = themeName + " " + theme[1] + "%";
                Color color = StiColorUtils.Light(colorBase, (byte)(Math.Abs((themePercent - 100) / 25) * 45));
                List<StiBaseStyle> styles = creator.CreateStyles(text, color);
                StiStylesCollection stylesCollection = new StiStylesCollection();

                foreach (StiBaseStyle style in styles)
                {
                    stylesCollection.Add(style);
                }
                
                if (stylesCollection != null)
                {
                    foreach (StiBaseStyle style in stylesCollection)
                    {
                        createdReport.Styles.Add(style);
                    }

                    string nameStylesCollection = null;
                    foreach (StiBaseStyle style in stylesCollection)
                    {
                        nameStylesCollection = style.CollectionName;
                    }

                    createdReport.ApplyStyleCollection(nameStylesCollection);
                }
            }
            #endregion

            return createdReport;
        }

        private static void RemoveDemoDataFormDictionary(StiReport report)
        {
            report.Dictionary.Databases.Clear();
            report.Dictionary.DataSources.Clear();
            report.Dictionary.Relations.Clear();
            if (report.Dictionary.Resources.Contains("Invoice_Data"))
                report.Dictionary.Resources.Remove("Invoice_Data");
        }

        private static void InitDictionary(StiReport report, StiReport baseReport)
        {
            if (report == null || baseReport == null)
                return;

            if (baseReport != null)
            {
                if (StiOptions.Designer.NewReport.AllowRegisterDataStoreFromOldReportInNewReport)
                {
                    foreach (StiData data in baseReport.Dictionary.DataStore)
                    {
                        if (!report.Dictionary.DataStore.Contains(data.Name))
                            report.Dictionary.DataStore.Add(data);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterDatabasesFromOldReportInNewReport)
                {
                    foreach (StiDatabase data in baseReport.Dictionary.Databases)
                    {
                        if (!report.Dictionary.Databases.Contains(data.Name))
                            report.Dictionary.Databases.Add(data);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterDataSourcesFromOldReportInNewReport)
                {
                    foreach (StiDataSource ds in baseReport.Dictionary.DataSources)
                    {
                        if (!report.Dictionary.DataSources.Contains(ds.Name))
                            report.Dictionary.DataSources.Add(ds);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterRelationsFromOldReportInNewReport)
                {
                    foreach (StiDataRelation relation in baseReport.Dictionary.Relations)
                    {
                        if (!report.Dictionary.Relations.Contains(relation.Name))
                            report.Dictionary.Relations.Add(relation);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterVariablesFromOldReportInNewReport)
                {
                    foreach (StiVariable variable in baseReport.Dictionary.Variables)
                    {
                        if (!report.Dictionary.Variables.Contains(variable.Name))
                            report.Dictionary.Variables.Add(variable);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterResourcesFromOldReportInNewReport)
                {
                    foreach (StiResource resource in baseReport.Dictionary.Resources)
                    {
                        if (!report.Dictionary.Resources.Contains(resource.Name))
                            report.Dictionary.Resources.Add(resource);
                    }
                }

                if (StiOptions.Designer.NewReport.AllowRegisterRestrictionsFromOldReportInNewReport)
                {
                    report.Dictionary.Restrictions = baseReport.Dictionary.Restrictions;
                }
            }

            report.Dictionary.SynchronizeRelations();
        }

        public static VariableInfo[] GetCompanyInfoVariables()
        {
            return new VariableInfo[]
            {
                new VariableInfo("CompanyName", StiLocalization.Get("PropertyMain", "Name")),
                new VariableInfo("CompanyContactName", "Contact Name"),
                new VariableInfo("CompanyPhone", "Phone"),
                new VariableInfo("CompanyEmail", StiLocalization.Get("A_WebViewer", "Email").Replace(":", string.Empty)),
                new VariableInfo("CompanySite", "Site"),
                new VariableInfo("CompanyCountry", "Country"),
                new VariableInfo("CompanyCity", "City"),
                new VariableInfo("CompanyNotes", "Notes"),
                new VariableInfo("CompanyThanks", "Thanks"),
                new VariableInfo("CompanyAddress", StiLocalization.Get("Report", "Address")),
            };
        }

        private static List<StiLabelInfo> GetLabels()
        {
            Stream stream = typeof(StiReport).Assembly.GetManifestResourceStream("Stimulsoft.Report.Labels.Labels.data");
            StreamReader reader = new StreamReader(stream);
            reader.ReadLine();

            var labels = new List<StiLabelInfo>
            {
                new StiLabelInfo("", "Custom", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            };

            foreach (object obj in StiOptions.Designer.Wizards.Labels) labels.Add(((StiLabelInfo)obj).Clone() as StiLabelInfo);

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                do
                {
                    try
                    {
                        string line = reader.ReadLine();
                        if (line == null) break;
                        string[] lines = line.Split(new char[] { ';' });

                        string strManufacturer = lines[0];
                        string strLabelName = lines[1];
                        string strLabelWidth = lines[2];
                        string strLabelHeight = lines[3];
                        string strPaperWidth = lines[4];
                        string strPaperHeight = lines[5];

                        string strNumberOfRows = lines[6];
                        string strNumberOfColumns = lines[7];

                        string strHorizontalGap = lines[8];
                        string strVerticalGap = lines[9];
                        string strLeftMargin = lines[10];
                        string strTopMargin = lines[11];

                        labels.Add(
                            new StiLabelInfo(
                            strManufacturer, strLabelName,
                            double.Parse(strLabelWidth), double.Parse(strLabelHeight),
                            double.Parse(strHorizontalGap), double.Parse(strVerticalGap),
                            double.Parse(strLeftMargin), double.Parse(strTopMargin),
                            double.Parse(strPaperWidth), double.Parse(strPaperHeight),
                            int.Parse(strNumberOfColumns), int.Parse(strNumberOfRows)));
                    }
                    catch
                    {
                    }
                }
                while (1 == 1);
                stream.Close();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            return labels;
        }

        private static byte[] GetJsonDemoDataResource()
        {
            using (var stream = typeof(StiWebDesigner).Assembly.GetManifestResourceStream("Stimulsoft.Report.Web.Designer.WizardDashboards.DemoDbs.json"))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                return data;
            }
        }

        private static byte[] GetDemoXMLResource()
        {
            var name = "Stimulsoft.Report.Web.Designer.WizardReports.Demo.xml";
            using (var stream = typeof(StiWebDesigner).Assembly.GetManifestResourceStream(name))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                return data;
            }
        }

        private static byte[] GetDemoXSDResource()
        {
            var name = "Stimulsoft.Report.Web.Designer.WizardReports.Demo.xsd";
            using (var stream = typeof(StiWebDesigner).Assembly.GetManifestResourceStream(name))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                return data;
            }
        }

        internal static StiReport PrepareReportBeforeGetData(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            if (report != null)
                callbackResult["oldReportContent"] = report.SavePackedReportToString();

            report = StiWebDesigner.GetNewReport(requestParams);
            report.Info.ForceDesigningMode = true;
            StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(StiDesignerOptionsHelper.GetDesignerOptions(requestParams), report);

            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report, zoom);
            callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;

            return report;
        }

        internal static void ChangeReportType(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            report.Pages.Clear();

            if (param["reportType"] as string == "Dashboard")
            {
                report.ReportUnit = StiReportUnitType.HundredthsOfInch;
                report.Pages.Add(StiDashboardCreator.CreateDashboard(report) as StiPage);
            }
            else
            {
                report.ReportUnit = (StiReportUnitType)requestParams.GetEnum("defaultUnit", typeof(StiReportUnitType));
                report.Pages.Add(new StiPage(report));
            }

            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report, zoom);
            callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
        }

        internal static void RestoreOldReport(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            report.LoadPackedReportFromString(param["oldReportContent"] as string);
            report.Info.Zoom = 1;
            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report, zoom);
            callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
        }

        internal static void AddJsonDemoDataToReport(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            var resource = new StiResource()
            {
                Name = "StimulsoftDemo",
                Type = StiResourceType.Json,
                Content = GetJsonDemoDataResource()
            };
            report.Dictionary.Resources.Add(resource);

            var database = resource.CreateFileDatabase();
            if (database == null) return;

            var name = StiNameCreation.CreateConnectionName(report, resource.Name.Replace(" ", string.Empty));
            database.Name = name;
            database.Alias = name;

            report.Dictionary.Databases.Add(database);
            database.Synchronize(report);

            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report, zoom);
            callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
        }

        internal static void AddXmlDemoDataToReport(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            var resourceDemoXML = new StiResource
            {
                Name = "StimulsoftDemoXML",
                Type = StiResourceType.Xml,
                Content = GetDemoXMLResource()
            };
            report.Dictionary.Resources.Add(resourceDemoXML);

            var resourceDemoXSD = new StiResource
            {
                Name = "StimulsoftDemoXSD",
                Type = StiResourceType.Xsd,
                Content = GetDemoXSDResource()
            };
            report.Dictionary.Resources.Add(resourceDemoXSD);

            var database = new StiXmlDatabase("Demo", "resource://StimulsoftDemoXSD", "resource://StimulsoftDemoXML");
            report.Dictionary.Databases.Add(database);
            database.Synchronize(report);

            var zoom = param["zoom"] != null ? Convert.ToDouble(param["zoom"]) : 1d;
            callbackResult["reportObject"] = StiReportEdit.WriteReportToJsObject(report, zoom);
            callbackResult["reportGuid"] = requestParams.Cache.ClientGuid;
        }

        internal static void AddDemoDataToReport(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            var isDashboard = Convert.ToBoolean(param["isDashboard"]);
            if (isDashboard)
                AddJsonDemoDataToReport(report, param, callbackResult, requestParams);
            else
                AddXmlDemoDataToReport(report, param, callbackResult, requestParams);
        }
    }
}