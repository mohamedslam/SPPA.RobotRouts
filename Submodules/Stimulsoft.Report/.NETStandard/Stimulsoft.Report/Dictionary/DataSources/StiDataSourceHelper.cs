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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Extensions;
using System;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public class StiDataSourceHelper
	{
        #region Methods
	    public static StiDatabase GetDatabaseFromDataSource(StiDataSource dataSource)
	    {
	        if (dataSource == null || dataSource.Dictionary == null) 
                return null;

	        var category = dataSource.GetCategoryName();

	        if (string.IsNullOrWhiteSpace(category)) 
                return null;

	        category = category.ToLowerInvariant();

	        return dataSource.Dictionary.Databases.ToList()
	            .FirstOrDefault(d => d.Name != null && d.Name.ToLowerInvariant() == category);
	    }

        public static List<StiDataSource> GetDataSourcesFromDatabase(StiReport report, StiDatabase database)
        {
            if (database == null || database.Name == null) 
                return null;

            return report.Dictionary.DataSources.ToList()
                .Where(d => d.GetCategoryName() != null)
                .Where(d => database.Name.ToLowerInvariant() == d.GetCategoryName().ToLowerInvariant())
                .ToList();
        }

        public static List<string> GetUsedDataSourcesNamesList(StiReport report)
        {
            return GetUsedDataSourcesNames(report)
                .Cast<DictionaryEntry>()
                .Select(d => d.Key)
                .Cast<string>()
                .ToList();
        }

        public static Hashtable GetUsedDataSourcesNames(StiReport report)
        {
            var datasourcesNames = new Hashtable();
            var tempPage = report.RenderedPages.Count > 0 ? report.RenderedPages[0] : new StiPage() { Report = report };
            var tempComp = new StiText { Page = tempPage, Name = "*GetUsedDataSourcesNames*" };

            #region Check components
            var comps = report.GetComponents();
            for (var indexComp = 0; indexComp < comps.Count; indexComp++)
            {
                var component = comps[indexComp];

                StiImage stiImage = component as StiImage;
                try
                {
                    var dataBand = component as StiDataBand;
                    if (dataBand != null && dataBand.DataSourceName != null && dataBand.DataSourceName.Length > 0)
                    {
                        AddDataSourceName(datasourcesNames, dataBand.DataSourceName);
                        AddRelation(datasourcesNames, dataBand.DataRelation);
                        AddSort(datasourcesNames, tempComp, dataBand.Sort, dataBand.DataSourceName);
                    }

                    var crosstab = component as StiCrossTab;
                    if (crosstab != null && crosstab.DataSourceName != null && crosstab.DataSourceName.Length > 0)
                    {
                        AddDataSourceName(datasourcesNames, crosstab.DataSourceName);
                        AddSort(datasourcesNames, tempComp, crosstab.Sort, crosstab.DataSourceName);
                    }

                    var groupHeaderBand = component as StiGroupHeaderBand;
                    if (groupHeaderBand != null)
                        CheckExpression(groupHeaderBand.Condition.Value, component, datasourcesNames);

                    var crossCell = component as StiCrossCell;
                    if (crossCell != null)
                        CheckExpression(crossCell.Value.Value, component, datasourcesNames);

                    var crossHeader = component as StiCrossHeader;
                    if (crossHeader != null)
                        CheckExpression(crossHeader.DisplayValue.Value, component, datasourcesNames);

                    var stiSimpleText = component as StiSimpleText;
                    if (stiSimpleText != null)
                        CheckExpression(stiSimpleText.Text.Value, component, datasourcesNames);

                    var stiText = component as StiText;
                    if (stiText != null)
                        CheckExpression(stiText.ExcelValue.Value, component, datasourcesNames);

                    var richText = component as StiRichText;
                    if (richText != null)
                    {
                        using (RichTextBox richTextBox = new Controls.StiRichTextBox(false))
                        {
                            richTextBox.Rtf = richText.RtfText;
                            CheckExpression(richTextBox.Text, component, datasourcesNames);
                        }
                    }

                    if (stiImage != null)
                    {
                        CheckExpression(stiImage.ImageData.Value, component, datasourcesNames);
                        CheckExpression("{" + stiImage.DataColumn + "}", component, datasourcesNames);
                    }

                    var barcode = component as StiBarCode;
                    if (barcode != null)
                        CheckExpression(barcode.Code.Value, component, datasourcesNames);

                    var zipcode = component as StiZipCode;
                    if (zipcode != null)
                        CheckExpression(zipcode.Code.Value, component, datasourcesNames);

                    var checkbox = component as StiCheckBox;
                    if (checkbox != null)
                        CheckExpression(checkbox.Checked.Value, component, datasourcesNames);

                    var chart = component as StiChart;
                    if (chart != null)
                    {
                        if (!string.IsNullOrEmpty(chart.DataSourceName))
                            AddDataSourceName(datasourcesNames, chart.DataSourceName);

                        foreach (StiSeries series in chart.Series)
                        {
                            CheckExpression(series.Argument.Value, component, datasourcesNames);
                            CheckExpression(series.Value.Value, component, datasourcesNames);
                            CheckExpression("{" + series.ArgumentDataColumn + "}", component, datasourcesNames);
                            CheckExpression("{" + series.ValueDataColumn + "}", component, datasourcesNames);
                        }
                    }

                    var page = component as StiPage;
                    if (page != null)
                        CheckExpression(page.ExcelSheet.Value, component, datasourcesNames);

                    var icondition = component as IStiConditions;
                    if (icondition != null && icondition.Conditions.Count > 0)
                    {
                        var conditionList = new List<DictionaryEntry>();

                        #region Prepare conditions
                        foreach (StiBaseCondition cond in icondition.Conditions)
                        {
                            var condition = cond as StiCondition;
                            if (cond is StiMultiCondition)
                            {
                                var multiCondition = cond as StiMultiCondition;
                                if (multiCondition.FilterOn && multiCondition.Filters.Count > 0)
                                {
                                    var conditionExpression = new StringBuilder("{");
                                    for (int index = 0; index < multiCondition.Filters.Count; index++)
                                    {
                                        var filter = multiCondition.Filters[index];

                                        conditionExpression.Append("(");
                                        conditionExpression.Append(StiDataHelper.GetFilterExpression(filter, filter.Column, report));
                                        conditionExpression.Append(")");

                                        if (index < multiCondition.Filters.Count - 1)
                                            conditionExpression.Append(multiCondition.FilterMode == StiFilterMode.And ? " && " : " || ");
                                    }
                                    conditionExpression.Append("}");

                                    var de = new DictionaryEntry(multiCondition, conditionExpression.ToString());
                                    conditionList.Add(de);
                                }
                            }
                            else if (condition != null)
                            {
                                var expression = "{" + StiDataHelper.GetFilterExpression(condition, condition.Column, report) + "}";
                                var de = new DictionaryEntry(condition, expression);
                                conditionList.Add(de);
                            }
                        }
                        #endregion

                        if (conditionList.Count > 0)
                        {
                            foreach (var de in conditionList)
                            {
                                CheckExpression((string)de.Value, component, datasourcesNames);
                            }
                        }
                    }

                    var ifilter = component as IStiFilter;
                    if (ifilter != null && ifilter.Filters.Count > 0)
                    {
                        #region Get dataSource name
                        string dsName = "ds";
                        var ds = component as IStiDataSource;
                        if ((ds != null) && !string.IsNullOrWhiteSpace(ds.DataSourceName))
                        {
                            dsName = ds.DataSourceName;
                        }
                        #endregion

                        #region Prepare filters
                        var filterExpression = new StringBuilder("{");
                        for (int index = 0; index < ifilter.Filters.Count; index++)
                        {
                            var filter = ifilter.Filters[index];

                            filterExpression.Append("(");
                            filterExpression.Append(StiDataHelper.GetFilterExpression(filter, dsName + "." + filter.Column, report));
                            filterExpression.Append(")");

                            if (index < ifilter.Filters.Count - 1)
                                filterExpression.Append(ifilter.FilterMode == StiFilterMode.And ? " && " : " || ");
                        }
                        filterExpression.Append("}");
                        #endregion

                        CheckExpression(filterExpression.ToString(), component, datasourcesNames);
                    }
                }
                catch
                {
                }
            }
            #endregion

            #region Check variables
            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.InitBy == StiVariableInitBy.Expression)
                {
                    CheckExpression("{" + variable.Value + "}", tempComp, datasourcesNames);

                    if (variable.Type == typeof(Range))
                    {
                        CheckExpression("{" + variable.InitByExpressionFrom + "}", tempComp, datasourcesNames);
                        CheckExpression("{" + variable.InitByExpressionTo + "}", tempComp, datasourcesNames);
                    }
                }

                if (variable.RequestFromUser && variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
                {
                    CheckExpression("{" + variable.DialogInfo.KeysColumn + "}", tempComp, datasourcesNames);
                    CheckExpression("{" + variable.DialogInfo.ValuesColumn + "}", tempComp, datasourcesNames);
                }
            }
            #endregion

            #region Check VirtualDataSources
            var newDS = new List<string>();
            foreach (DictionaryEntry de in datasourcesNames)
            {
                var dsName = (string)de.Key;
                var ds = report.Dictionary.DataSources[dsName];
                if (ds == null) continue;

                var vds = ds as StiVirtualSource;
                if (vds != null && !string.IsNullOrEmpty(vds.NameInSource))
                    newDS.Add(dsName);
            }

            foreach (string st in newDS)
            {
                var vs = report.Dictionary.DataSources[st] as StiVirtualSource;
                var ds = report.Dictionary.DataSources[vs.NameInSource];
                AddDataSourceName(datasourcesNames, ds.Name);

                foreach (var column in vs.GroupColumns)
                {
                    AddDataSourceColumn(datasourcesNames, ds.Name, column);
                }

                var index = 0;
                while (index < vs.Results.Length)
                {
                    AddDataSourceColumn(datasourcesNames, ds.Name, vs.Results[index]);
                    index += 3;
                }

                AddSort(datasourcesNames, tempComp, vs.Sort, ds.Name);
            }
            #endregion

            #region Check CalculatedColumns and StiDataTransformationColumns
            var dsNamesClone = datasourcesNames.Clone() as Hashtable;
            foreach (DictionaryEntry de in dsNamesClone)
            {
                var dsName = (string)de.Key;
                var ds = report.Dictionary.DataSources[dsName];
                if (ds == null) continue;

                Hashtable columns = (de.Value as Hashtable).Clone() as Hashtable;
                foreach (object colName in columns.Keys)
                {
                    var dc = ds.Columns[(string)colName];

                    StiCalcDataColumn cdc = dc as StiCalcDataColumn;
                    if (cdc != null)
                    {
                        CheckExpression("{" + cdc.Expression + "}", tempComp, datasourcesNames);
                    }

                    StiDataTransformationColumn dtc = dc as StiDataTransformationColumn;
                    if (dtc != null)
                    {
                        try
                        {
                            CheckExpression("{" + dtc.Expression + "}", tempComp, datasourcesNames);
                        }
                        catch
                        { }
                    }
                }
            }
            #endregion

            if (report.ListOfUsedData != null)
            {
                foreach (string st in report.ListOfUsedData)
                {
                    AddDataSourceName(datasourcesNames, st);
                }
            }

            return datasourcesNames;
        }

        internal static void CheckExpression(string expression, StiComponent component, Hashtable datasourcesNames, bool throwException = false)
        {
            try
            {
                var storeToPrint = false;
                var result = StiParser.ParseTextValue(expression, component, ref storeToPrint, false, true);
                if (result == null || !(result is List<StiParser.StiAsmCommand>)) return;

                CheckAsmCommandList(result as List<StiParser.StiAsmCommand>, component, datasourcesNames);
            }
            catch (Exception e)
            {
                if (throwException)
                    throw e;
            }
        }

        private static void CheckAsmCommandList(List<StiParser.StiAsmCommand> list, StiComponent component, Hashtable datasourcesNames)
        {
            try
            {
                foreach (var asmCommand in list)
                {
                    if ((asmCommand.Type == StiParser.StiAsmCommandType.PushValue) && (asmCommand.Parameter1 is List<StiParser.StiAsmCommand>))
                    {
                        CheckAsmCommandList(asmCommand.Parameter1 as List<StiParser.StiAsmCommand>, component, datasourcesNames);
                        continue;
                    }
                    if (asmCommand.Type != StiParser.StiAsmCommandType.PushDataSourceField) continue;

                    var parts = new List<string>(((string)asmCommand.Parameter1).Split('.'));

                    var dataSource = component.Report.Dictionary.DataSources[parts[0]];
                    if (dataSource is StiVirtualSource && parts.Count > 2)
                    {
                        //Recombine the fields names with comma, especially for StiVirtualSource
                        var columnName = parts[1] + "." + parts[2];
                        if (dataSource.Columns.Contains(columnName))
                        {
                            parts[1] = columnName;
                            parts.RemoveAt(2);
                        }
                        else
                        {
                            if (parts.Count > 3)
                            {
                                columnName += "." + parts[3];
                                if (dataSource.Columns.Contains(columnName))
                                {
                                    parts[1] = columnName;
                                    parts.RemoveAt(2);
                                    parts.RemoveAt(2);
                                }
                            }
                        }
                    }

                    AddDataSourceName(datasourcesNames, dataSource.Name);

                    if (parts.Count > 2)
                    {
                        var nameInSource = parts[1];
                        AddRelation(datasourcesNames, dataSource.ParentRelationList().First(rel => rel.NameInSource == nameInSource));

                        dataSource = dataSource.GetParentDataSource(nameInSource);
                        AddDataSourceName(datasourcesNames, dataSource.Name);

                        var indexPart = 2;
                        while (indexPart < parts.Count - 1)
                        {
                            nameInSource = parts[indexPart];
                            AddRelation(datasourcesNames, dataSource.ParentRelationList().First(rel => rel.NameInSource == nameInSource));

                            dataSource = dataSource.GetParentDataSource(nameInSource);
                            AddDataSourceName(datasourcesNames, dataSource.Name);
                            indexPart++;
                        }
                    }

                    if (parts.Count > 1)
                        AddDataSourceColumn(datasourcesNames, dataSource.Name, parts[parts.Count - 1]);
                }
            }
            catch
            {
            }
        }

        private static void AddDataSourceName(Hashtable hashtable, string dataSourceName)
        {
            if (!hashtable.ContainsKey(dataSourceName))
                hashtable[dataSourceName] = new Hashtable();
        }

        private static void AddDataSourceColumn(Hashtable hashtable, string dataSourceName, string columnName)
        {
            var hash = hashtable[dataSourceName] as Hashtable;
            if (hash == null)
            {
                hash = new Hashtable();
                hashtable[dataSourceName] = hash;
            }

            hash[columnName] = columnName;
        }

        private static void AddRelation(Hashtable hashtable, StiDataRelation relation)
        {
            if (relation == null) return;

            if (relation.ChildSource != null)
            {
                AddDataSourceName(hashtable, relation.ChildSource.Name);
                foreach (var column in relation.ChildColumns)
                {
                    AddDataSourceColumn(hashtable, relation.ChildSource.Name, column);
                }
            }

            if (relation.ParentSource != null)
            {
                AddDataSourceName(hashtable, relation.ParentSource.Name);

                foreach (var column in relation.ParentColumns)
                {
                    AddDataSourceColumn(hashtable, relation.ParentSource.Name, column);
                }
            }
        }

        private static void AddSort(Hashtable hashtable, StiComponent tempComp, string[] sortArray, string dataSourceName)
        {
            var index = 1;
            while (index < sortArray.Length)
            {
                var st2 = sortArray[index];
                if (st2.StartsWith("{"))
                    CheckExpression(st2, tempComp, hashtable);

                else
                    AddDataSourceColumn(hashtable, dataSourceName, st2);

                index += 2;
            }
        }

        public static Hashtable GetDataSourcesUsedInRequestFromUsersVariables(StiReport report)
        {
            var datasourcesNames = new Hashtable();

            var comp = new StiText
            {
                Name = "*RequestFromUserVariable*",
                Page = new StiPage(report)
            };

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (!variable.RequestFromUser) continue;

                try
                {
                    if (variable.DialogInfo.ItemsInitializationType == StiItemsInitializationType.Columns)
                        CheckExpression("{" + variable.DialogInfo.KeysColumn + "}", comp, datasourcesNames, true);

                    if (variable.InitBy == StiVariableInitBy.Expression)
                    {
                        CheckExpression("{" + variable.Value + "}", comp, datasourcesNames, true);

                        if (variable.Type == typeof(Range))
                        {
                            CheckExpression("{" + variable.InitByExpressionFrom + "}", comp, datasourcesNames, true);
                            CheckExpression("{" + variable.InitByExpressionTo + "}", comp, datasourcesNames, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    report.WriteToReportRenderingMessages($"A problem with the '{variable.Name}' variable parsing! {e.Message}");
                }
            }

            return datasourcesNames;
        }
		#endregion
	}
}
