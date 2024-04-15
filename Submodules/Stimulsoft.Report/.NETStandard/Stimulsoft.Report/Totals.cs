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
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Engine;
using Stimulsoft.Base;
using System.Data;

namespace Stimulsoft.Report
{
	public sealed class Totals
	{
		public static MethodInfo GetMethod(StiReport report, string name)
		{
			return report.GetType().GetMethod(name);
		}

        private class StiFunctionCalcData
        {
            public bool IsCompilationMode = true;
            public List<StiParser.StiAsmCommand> AsmList = null;
            public List<StiParser.StiAsmCommand> AsmList2 = null;
            public List<StiParser.StiAsmCommand> ConditionAsmList = null;
            public StiParser Parser = null;
            public MethodInfo Method = null;
            public MethodInfo Method2 = null;
            public StiAggregateFunctionService Function = null;
            public StiSumDistinctDecimalFunctionService SumDistinctFunction = null;
            public StiReport Report = null;
        }

        #region Calculate main methods
        private static object Calculate(StiAggregateFunctionService function, object data, StiReport report, string name, bool allLevels, bool onlyChilds, bool isPageTotal)
        {
            StiReport tempReport = report;
            if (tempReport == null)
            {
                object tempData = data;
                StiParser.StiParserData parserData = data as StiParser.StiParserData;
                if (parserData != null)
                {
                    tempData = parserData.Data;
                }
                if (tempData is StiDataSource)
                {
                    StiDictionary dict = (tempData as StiDataSource).Dictionary;
                    if (dict != null) tempReport = dict.Report;
                }
                if (tempData is StiBand)
                {
                    tempReport = (tempData as StiBand).Report;
                }
                if (tempData is StiBusinessObject)
                {
                    var dict = (tempData as StiBusinessObject).Dictionary;
                    if (dict != null) tempReport = dict.Report;
                }
            }

            bool needCacheTotals = (tempReport != null) && tempReport.CacheTotals && (data != null);
            object result = null;
            string storeId = null;

            if (needCacheTotals)
            {
                bool alreadyCalculated = GetCachedValue(function, data, tempReport, name, allLevels, onlyChilds, isPageTotal, ref result, out storeId);
                if (alreadyCalculated) return result;
            }

            if (tempReport != null) tempReport.CachedTotalsLocked = true;

            result = Calculate1(function, data, report, name, allLevels, onlyChilds, isPageTotal);

            if (tempReport != null) tempReport.CachedTotalsLocked = false;

            if (needCacheTotals)
            {
                StoreCachedValue(data, tempReport, storeId, result);
            }

            return result;
        }

        private static object Calculate1(StiAggregateFunctionService function, object data, StiReport report, string name, bool allLevels, bool onlyChilds, bool isPageTotal)
        {
            #region Check for Interpretation mode
            bool isCompilationMode = true;
            if (report != null) isCompilationMode = report.CalculationMode == StiCalculationMode.Compilation;

            var calcData = new StiFunctionCalcData();

            if (!isCompilationMode)
            {
                StiParser.StiParserData parserData = data as StiParser.StiParserData;
                if (parserData != null)
                {
                    calcData.AsmList = parserData.AsmList;
                    calcData.AsmList2 = parserData.AsmList2;
                    calcData.ConditionAsmList = parserData.ConditionAsmList;
                    calcData.Parser = parserData.Parser;
                    data = parserData.Data;
                }
                else
                {
                    isCompilationMode = true;
                }
            }
            calcData.IsCompilationMode = isCompilationMode;
            calcData.Report = report;
            calcData.Function = function;
            #endregion

            #region Prepare methods
            string name2 = null;
            var tempSumDistinctFunction = function as StiSumDistinctDecimalFunctionService;
            if (tempSumDistinctFunction != null)
            {
                if (isCompilationMode)
                {
                    string[] arr = name.Split(new char[] { '~' });
                    if (arr.Length > 1)
                    {
                        name = arr[0];
                        name2 = arr[1];
                        calcData.SumDistinctFunction = tempSumDistinctFunction;
                    }
                }
                else
                {
                    if (calcData.AsmList2 != null)
                    {
                        calcData.SumDistinctFunction = tempSumDistinctFunction;
                    }
                }
            }

            if (isCompilationMode)
            {
                if (name != null)
                {
                    calcData.Method = GetMethod(report, name);
                    if (calcData.Method == null) return 0;
                }
                if (name2 != null)
                {
                    calcData.Method2 = GetMethod(report, name2);
                    if (calcData.Method2 == null) return 0;
                }
            }
            #endregion

            #region Calculate page totals
            StiDataBand dataBand5 = data as StiDataBand;
            if (dataBand5 != null && isPageTotal)
            {
                if (report == null) report = dataBand5.Report;
                function.Init();

                //dataBand5.SaveState("Totals");
                SaveState(dataBand5);

                if (!dataBand5.IsBusinessObjectEmpty)
                {
                    dataBand5.BusinessObject.CreateEnumerator();
                    dataBand5.BusinessObject.specTotalsCalculation = true;
                }

                #region Proccess rendered page
                if (report.Engine != null && report.Engine.IndexPageForPageTotal != -1)
                {
                    int startPageIndex = report.Engine.IndexPageForPageTotal;
                    if (report.Engine.StartIndexPageForPageTotal != -1)
                        startPageIndex = report.Engine.StartIndexPageForPageTotal;

                    string breakedName = "Breaked_" + dataBand5.Name;
                    string continuedName = "Continued_" + dataBand5.Name;

                    for (int pageIndex = startPageIndex; pageIndex <= report.Engine.IndexPageForPageTotal; pageIndex++)
                    {
                        StiPage page = report.RenderedPages[pageIndex];

                        StiComponentsCollection comps = page.GetComponents();
                        foreach (StiComponent comp in comps)
                        {
                            StiContainer cont = comp as StiContainer;
                            if (cont != null && (dataBand5.Name == cont.Name || breakedName == cont.Name || continuedName == cont.Name) && cont.ContainerInfoV2.DataBandPosition != -1)
                            {
                                DataRow[] storedRows = null;
                                object storedBusinessObject = null;
                                if (!dataBand5.IsDataSourceEmpty)
                                {
                                    dataBand5.Position = cont.ContainerInfoV2.DataBandPosition;
                                    if (cont.ContainerInfoV2.DataSourceRow != null)
                                    {
                                        dataBand5.Position = 0;
                                        storedRows = dataBand5.DataSource.DetailRows;
                                        dataBand5.DataSource.DetailRows = new DataRow[1];
                                        dataBand5.DataSource.DetailRows[0] = cont.ContainerInfoV2.DataSourceRow;
                                    }
                                }
                                if (!dataBand5.IsBusinessObjectEmpty)
                                {
                                    if (cont.ContainerInfoV2.BusinessObjectCurrent != null)
                                    {
                                        storedBusinessObject = dataBand5.BusinessObject.currentObject;
                                        dataBand5.BusinessObject.currentObject = cont.ContainerInfoV2.BusinessObjectCurrent;
                                    }
                                }

                                CalcItem(calcData);

                                if (!dataBand5.IsDataSourceEmpty)
                                {
                                    if (cont.ContainerInfoV2.DataSourceRow != null)
                                    {
                                        dataBand5.DataSource.DetailRows = storedRows;
                                    }
                                }
                                if (!dataBand5.IsBusinessObjectEmpty)
                                {
                                    if (cont.ContainerInfoV2.BusinessObjectCurrent != null)
                                    {
                                        dataBand5.BusinessObject.currentObject = storedBusinessObject;
                                    }
                                }

                            }
                        }
                    }
                }
                #endregion

                if (!dataBand5.IsBusinessObjectEmpty)
                {
                    dataBand5.BusinessObject.specTotalsCalculation = false;
                }

                //dataBand5.RestoreState("Totals");
                RestoreState(dataBand5);

                return function.GetValue();
            }
            #endregion

			#region Calculate based on data source
			StiDataSource dataSource = data as StiDataSource;			
			if (dataSource != null)
			{
				if (allLevels)
					throw new Exception("You can't use AllLevels together with DataSource! Only HierarchicalBand can be used.");

				function.Init();
				dataSource.SaveState("Totals");
				dataSource.First();
				
				dataSource.ResetData();
				dataSource.ResetDetailsRows();

				while (!dataSource.IsEof)
				{
                    CalcItem(calcData);
					dataSource.Next();
				}
				dataSource.RestoreState("Totals");

				return function.GetValue();
			}
			#endregion

            #region Calculate based on business object
            StiBusinessObject businessObject = data as StiBusinessObject;

            if (businessObject != null)
            {
                if (allLevels)
                    throw new Exception("You can't use AllLevels together with BusinessObject! Only HierarchicalBand can be used.");

                function.Init();
                businessObject.SaveState("Totals");
                businessObject.CreateEnumerator();
                businessObject.specTotalsCalculation = true;   //fix

                while (!businessObject.IsEof)
                {
                    CalcItem(calcData);
                    businessObject.Next();
                }
                businessObject.specTotalsCalculation = false;   //fix
                businessObject.RestoreState("Totals");

                return function.GetValue();
            }
            #endregion

            #region Calculate based on hierarchical data band
            StiHierarchicalBand treeBand = data as StiHierarchicalBand;
            if (treeBand != null)
            {
                bool childsOneLevel = false;
                if ((!allLevels) && onlyChilds)
                {
                    childsOneLevel = true;
                    allLevels = true;
                }

                if (report == null) report = treeBand.Report;

                function.Init();
                                
                #region Data Source
                if (!treeBand.IsDataSourceEmpty)
                {
                    treeBand.SaveState("Totals");

                    if (treeBand.HierarchicalBandInfoV2.FinalFooterCalculation)
                        treeBand.Position = 0;

                    int currentLevel = treeBand.DataSource.GetLevel();
                    if (treeBand.HierarchicalBandInfoV2.SpecifiedLevel != -1) currentLevel = treeBand.HierarchicalBandInfoV2.SpecifiedLevel;

                    #region Calc Current Item
                    if ((treeBand.DataSource.Count > 0 && (treeBand.HierarchicalBandInfoV2.SpecifiedLevel == -1 || allLevels)) && (!onlyChilds))
                    {
                        CalcItem(calcData);
                    }
                    #endregion

                    int resPosition = treeBand.Position;

                    #region Calculate Previous Values
                    while (treeBand.Position > 0 && (!onlyChilds))
                    {
                        treeBand.Position--;
                        int level = treeBand.DataSource.GetLevel();
                        if (level < currentLevel) break;
                        if ((!allLevels) && level != currentLevel) continue;

                        CalcItem(calcData);
                    }
                    treeBand.Position = resPosition;
                    #endregion

                    #region Calculate Next Values
                    while (treeBand.Position < treeBand.Count - 1)
                    {
                        treeBand.Position++;
                        int level = treeBand.DataSource.GetLevel();
                        if (level < currentLevel) break;
                        if (onlyChilds)
                        {
                            if (childsOneLevel && ((level - currentLevel) > 1)) continue;
                            if (level == currentLevel) break;
                        }
                        if ((!allLevels) && level != currentLevel) continue;

                        CalcItem(calcData);
                    }
                    treeBand.Position = resPosition;
                    #endregion

                    treeBand.RestoreState("Totals");
                }
                #endregion

                #region Business Object
                if (!treeBand.IsBusinessObjectEmpty)
                {
                    int resPosition2 = treeBand.Position;
                    treeBand.SaveState("Totals");
                                        
                    List<object> rows = new List<object>();

                    treeBand.SaveState("Totals2");
                    treeBand.BusinessObject.CreateEnumerator();
                    while(!treeBand.BusinessObject.IsEof)
                    {
                        rows.Add(treeBand.BusinessObject.enumerator.Current);
                        treeBand.BusinessObject.Next();
                    }
                    treeBand.RestoreState("Totals2");

                    if (treeBand.HierarchicalBandInfoV2.FinalFooterCalculation)
                        treeBand.Position = 0;

                    treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                    int currentLevel = treeBand.BusinessObject.GetLevel();
                    if (treeBand.HierarchicalBandInfoV2.SpecifiedLevel != -1) currentLevel = treeBand.HierarchicalBandInfoV2.SpecifiedLevel;

                    #region Calc Current Item
                    if ((treeBand.BusinessObject.Count > 0 && (treeBand.HierarchicalBandInfoV2.SpecifiedLevel == -1 || allLevels)) && (!onlyChilds))
                    {
                        CalcItem(calcData);
                    }
                    #endregion

                    int resPosition = treeBand.Position;

                    #region Calculate Previous Values
                    while (treeBand.Position > 0 && (!onlyChilds))
                    {
                        treeBand.Position--;
                        treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                        int level = treeBand.BusinessObject.GetLevel();
                        if (level < currentLevel) break;
                        if ((!allLevels) && level != currentLevel) continue;

                        CalcItem(calcData);
                    }
                    treeBand.Position = resPosition;
                    #endregion

                    #region Calculate Next Values
                    while (treeBand.Position < treeBand.Count - 1)
                    {
                        treeBand.Position++;
                        treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                        int level = treeBand.BusinessObject.GetLevel();
                        if (level < currentLevel) break;
                        if (onlyChilds)
                        {
                            if (childsOneLevel && ((level - currentLevel) > 1)) continue;
                            if (level == currentLevel) break;
                        }
                        if ((!allLevels) && level != currentLevel) continue;

                        CalcItem(calcData);
                    }
                    treeBand.Position = resPosition;
                    #endregion

                    treeBand.RestoreState("Totals");
                }
                #endregion

                return function.GetValue();
            }
            #endregion

			#region Calculate based on data band
			StiDataBand dataBand = data as StiDataBand;
			if (dataBand != null)
			{
				if (allLevels)
					throw new Exception("You can't use AllLevels together with DataBand! Only HierarchicalBand can be used.");

                if (report == null) report = dataBand.Report;
				function.Init();

                #region DataBand based on Business Object
                if (dataBand.BusinessObject != null)
                {
                    StiBusinessObject businessObject2 = dataBand.BusinessObject;
                    businessObject2.SaveState("Totals");
                    businessObject2.CreateEnumerator();
                    businessObject2.specTotalsCalculation = true;   //fix

                    while (!businessObject2.IsEof)
                    {
                        CalcItem(calcData);
                        businessObject2.Next();
                    }
                    businessObject2.specTotalsCalculation = false;   //fix
                    businessObject2.RestoreState("Totals");
                }
                #endregion

                #region DataBand based on Data Source
                else
                {
                    //dataBand.SaveState("Totals");
                    SaveState(dataBand);
                    StiDataHelper.SetData(dataBand, false);
                    dataBand.First();
                    while (!dataBand.IsEof)
                    {
                        CalcItem(calcData);
                        dataBand.Next();
                    }
                    //dataBand.RestoreState("Totals");
                    RestoreState(dataBand);
                }
                #endregion

                return function.GetValue();
			}
			#endregion

			#region Calculate based on group header band
			StiGroupHeaderBand groupHeaderBand = data as StiGroupHeaderBand;
			if (groupHeaderBand != null)
			{
				if (allLevels)
                    throw new Exception("You can't use AllLevels together with StiGroupHeaderBand! Only HierarchicalBand can be used.");

                if (report == null) report = groupHeaderBand.Report;

				function.Init();

				dataBand = groupHeaderBand.GetDataBand();

				if (dataBand == null)return 0;
				
                SaveState(dataBand);
                bool[] groupHeaderResults = null;
                bool[] groupFooterResults = null;
                if (report.EngineVersion == StiEngineVersion.EngineV2 && dataBand.DataBandInfoV2 != null)
                {
                    if (dataBand.DataBandInfoV2.GroupHeaderResults != null) groupHeaderResults = dataBand.DataBandInfoV2.GroupHeaderResults.Clone() as bool[];
                    if (dataBand.DataBandInfoV2.GroupFooterResults != null) groupFooterResults = dataBand.DataBandInfoV2.GroupFooterResults.Clone() as bool[];
                }

                StiDataBandV1Builder builderV1 = null;
                StiDataBandV2Builder builderV2 = null;

                if (report.EngineVersion == StiEngineVersion.EngineV1)
				    builderV1 = StiDataBandV1Builder.GetBuilder(typeof(StiDataBand)) as StiDataBandV1Builder;
                else
                    builderV2 = StiDataBandV2Builder.GetBuilder(typeof(StiDataBand)) as StiDataBandV2Builder;

                StiBusinessObject businessObject3 = dataBand.BusinessObject;

				#region Only one value
                bool groupHeaderResult = false;
                bool groupFooterResult = false;

                if (report.EngineVersion == StiEngineVersion.EngineV1)
                {
                    groupHeaderResult = builderV1.GetGroupHeaderResult(dataBand, groupHeaderBand);
                    groupFooterResult = builderV1.GetGroupFooterResult(dataBand, groupHeaderBand);
                }
                else
                {
                    StiDataBandV2Builder.PrepareGroupResults(dataBand);
                    groupHeaderResult = builderV2.GetGroupHeaderResult(dataBand, groupHeaderBand);
                    groupFooterResult = builderV2.GetGroupFooterResult(dataBand, groupHeaderBand);
                }

                if (groupHeaderResult && groupFooterResult)
                {
                    try
                    {
                        //fix - set details to initial state
                        if (report != null && report.EngineVersion == StiEngineVersion.EngineV1)
                            StiDataBandV1Builder.SetDetails(dataBand);
                        else
                            StiDataBandV2Builder.SetDetails(dataBand);

                        CalcItem(calcData);
                    }
                    catch
                    {
                    }
                }
                #endregion

                #region Multiple values
                else
                {
                    if (businessObject3 != null)
                    {
                        int originalPosition = businessObject3.Position;

                        businessObject3.SaveState("Totals");
                        businessObject3.CreateEnumerator();
                        businessObject3.specTotalsCalculation = true;   //fix

                        List<bool> values = new List<bool>();

                        #region считаем результат группы для каждой строки до текущей позиции в бизнес-объекте
                        int position = 0;
                        while (position != originalPosition)
                        {
                            if (position == 0) StiDataBandV2Builder.PrepareGroupResults(dataBand);  //fix
                            //Calculation only for builderV2
                            values.Add(builderV2.GetGroupHeaderResult(dataBand, groupHeaderBand));
                            dataBand.Next();
                            position++;
                        }
                        position--;
                        #endregion

                        if (!groupHeaderResult) //for current position result is already calculated, so if true - then it's first row of group
                        {
                            #region определяем строки которые нужно считать в бизнес-объекте до текущей позиции
                            Hashtable positions = new Hashtable();
                            if (originalPosition > 0)
                            {
                                try
                                {
                                    while (true)
                                    {
                                        if (values[position])
                                        {
                                            positions[position] = position;
                                            break;
                                        }
                                        positions[position] = position;

                                        position--;
                                        if (position < 0) break;
                                    }
                                }
                                catch
                                {
                                    originalPosition = 0;
                                }
                            }
                            #endregion

                            #region снова бежим по бизнес-объекту, считаем только те строки, которые нужно считать
                            businessObject3.First();
                            position = 0;
                            while (position != originalPosition)
                            {
                                if (positions[position] != null)
                                {
                                    CalcItem(calcData);
                                }
                                position++;
                                businessObject3.Next();
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        while (1 == 1)
                        {
                            if (report.EngineVersion == StiEngineVersion.EngineV1)
                            {
                                if (builderV1.GetGroupHeaderResult(dataBand, groupHeaderBand)) break;
                            }
                            else
                            {
                                if (builderV2.GetGroupHeaderResult(dataBand, groupHeaderBand)) break;
                            }

                            dataBand.Prior();
                        }
                    }

                    while (1 == 1)
                    {
                        if (report.EngineVersion == StiEngineVersion.EngineV1)
                        {
                            if (builderV1.GetGroupFooterResult(dataBand, groupHeaderBand)) break;
                        }
                        else
                        {
                            if (builderV2.GetGroupFooterResult(dataBand, groupHeaderBand)) break;
                        }

                        CalcItem(calcData);

                        if (businessObject3 != null)
                        {
                            if (businessObject3.IsEof)
                            {
                                break;
                            }
                            businessObject3.Next();
                            StiDataBandV2Builder.PrepareGroupResults(dataBand);
                        }
                        else
                        {
                            dataBand.Next();
                        }
                    }

                    if (report.EngineVersion == StiEngineVersion.EngineV1)
                    {
                        groupHeaderResult = builderV1.GetGroupHeaderResult(dataBand, groupHeaderBand);
                        groupFooterResult = builderV1.GetGroupFooterResult(dataBand, groupHeaderBand);
                    }
                    else
                    {
                        groupHeaderResult = builderV2.GetGroupHeaderResult(dataBand, groupHeaderBand);
                        groupFooterResult = builderV2.GetGroupFooterResult(dataBand, groupHeaderBand);
                    }

                    if (!groupHeaderResult && groupFooterResult)
                    {
                        CalcItem(calcData);
                    }
                }
                #endregion

                if (businessObject3 != null)
                {
                    businessObject3.specTotalsCalculation = false;   //fix
                    businessObject3.RestoreState("Totals");
                }

                RestoreState(dataBand);
                if (groupHeaderResults != null) dataBand.DataBandInfoV2.GroupHeaderResults = groupHeaderResults;
                if (groupFooterResults != null) dataBand.DataBandInfoV2.GroupFooterResults = groupFooterResults;

                return function.GetValue();
			}
			#endregion

			return 0;
		}

        private static void CalcItem(StiFunctionCalcData calcData)
        {
            try
            {
                object value = null;

                if (calcData.IsCompilationMode)
                {
                    if (calcData.Method != null)
                    {
                        StiValueEventArgs e = new StiValueEventArgs();
                        calcData.Method.Invoke(calcData.Report, new Object[] { calcData.Report, e });
                        value = e.Value;
                    }
                    if (calcData.SumDistinctFunction != null)
                    {
                        StiValueEventArgs e = new StiValueEventArgs();
                        calcData.Method2.Invoke(calcData.Report, new Object[] { calcData.Report, e });
                        object value2 = e.Value;
                        calcData.SumDistinctFunction.CalcItem(value, value2);
                    }
                    else
                    {
                        calcData.Function.CalcItem(value);
                    }
                }
                else
                {
                    if (calcData.ConditionAsmList == null || Convert.ToBoolean(calcData.Parser.ExecuteAsm(calcData.ConditionAsmList)))
                    {
                        value = calcData.Parser.ExecuteAsm(calcData.AsmList);
                        if (calcData.SumDistinctFunction != null)
                        {
                            object value2 = calcData.Parser.ExecuteAsm(calcData.AsmList2);
                            calcData.SumDistinctFunction.CalcItem(value, value2);
                        }
                        else
                        {
                            calcData.Function.CalcItem(value);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static object CalculateByCondition(StiAggregateFunctionService function,
            object data, StiReport report, string name, StiFilterCondition filterCondition,
            decimal value1, decimal value2, bool allLevels, bool onlyChilds)
        {
            #region Check for Interpretation mode
            bool isCompilationMode = true;
            if (report != null) isCompilationMode = report.CalculationMode == StiCalculationMode.Compilation;

            List<StiParser.StiAsmCommand> asmList = null;
            StiParser parser = null;
            if (!isCompilationMode)
            {
                StiParser.StiParserData parserData = data as StiParser.StiParserData;
                if (parserData != null)
                {
                    asmList = parserData.AsmList;
                    parser = parserData.Parser;
                    data = parserData.Data;
                }
                else
                {
                    isCompilationMode = true;
                }
            }
            #endregion

            MethodInfo method = null;
            if (isCompilationMode)
            {
                if (name != null)
                {
                    method = GetMethod(report, name);
                    if (method == null) return 0;
                }
            }

            #region Calculate based on hierarchical data band
            StiHierarchicalBand treeBand = data as StiHierarchicalBand;
            if (treeBand != null)
            {
                bool childsOneLevel = false;
                if ((!allLevels) && onlyChilds)
                {
                    childsOneLevel = true;
                    allLevels = true;
                }

                if (report == null) report = treeBand.Report;

                function.Init();

                treeBand.SaveState("Totals");

                #region Businessobject
                List<object> rows = new List<object>();


                if (treeBand.BusinessObject != null)
                {
                    treeBand.SaveState("Totals2");
                    treeBand.BusinessObject.CreateEnumerator();                    

                    while (!treeBand.BusinessObject.IsEof)
                    {
                        rows.Add(treeBand.BusinessObject.enumerator.Current);
                        treeBand.BusinessObject.Next();
                    }
                    treeBand.RestoreState("Totals2");
                }
                #endregion

                if (treeBand.HierarchicalBandInfoV2.FinalFooterCalculation)
                    treeBand.Position = 0;

                if (treeBand.BusinessObject != null)
                    treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                int currentLevel = treeBand.DataSource != null ? treeBand.DataSource.GetLevel() : treeBand.BusinessObject.GetLevel();
                if (treeBand.HierarchicalBandInfoV2.SpecifiedLevel != -1) currentLevel = treeBand.HierarchicalBandInfoV2.SpecifiedLevel;


                #region Calc Current Item
                if (((rows.Count > 0 || treeBand.DataSource.Count > 0) && (treeBand.HierarchicalBandInfoV2.SpecifiedLevel == -1 || allLevels)) && (!onlyChilds))
                {
                    try
                    {
                        object value = null;
                        if (isCompilationMode)
                        {
                            if (method != null)
                            {
                                StiValueEventArgs e = new StiValueEventArgs();
                                method.Invoke(report, new Object[] { report, e });
                                //value = e.Value;
                                
                                value = CompareValue(e.Value, filterCondition, value1, value2);
                                
                            }
                        }
                        else
                        {
                            value = CompareValue(parser.ExecuteAsm(asmList), filterCondition, value1, value2);
                        }
                        function.CalcItem(value);
                    }
                    catch
                    {
                    }
                }
                #endregion

                int resPosition = treeBand.Position;
                #region Calculate Previous Values
                while (treeBand.Position > 0 && (!onlyChilds))
                {
                    treeBand.Position--;
                    if (treeBand.BusinessObject != null)
                        treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                    int level = treeBand.DataSource != null ? treeBand.DataSource.GetLevel() : treeBand.BusinessObject.GetLevel();
                    if (level < currentLevel) break;
                    if ((!allLevels) && level != currentLevel) continue;

                    #region Calc Item
                    try
                    {
                        object value = null;
                        if (isCompilationMode)
                        {
                            if (method != null)
                            {
                                StiValueEventArgs e = new StiValueEventArgs();
                                method.Invoke(report, new Object[] { report, e });
                                //value = e.Value;
                                value = CompareValue(e.Value, filterCondition, value1, value2);
                            }
                        }
                        else
                        {
                            value = CompareValue(parser.ExecuteAsm(asmList), filterCondition, value1, value2);
                        }
                        function.CalcItem(value);
                    }
                    catch
                    {
                    }
                    #endregion
                }
                treeBand.Position = resPosition;
                #endregion

                #region Calculate Next Values
                while (treeBand.Position < treeBand.Count - 1)
                {
                    treeBand.Position++;
                    if (treeBand.BusinessObject != null)
                        treeBand.BusinessObject.currentObject = rows[treeBand.Position];

                    int level = treeBand.DataSource != null ? treeBand.DataSource.GetLevel() : treeBand.BusinessObject.GetLevel();
                    if (level < currentLevel) break;
                    if (onlyChilds)
                    {
                        if (childsOneLevel && ((level - currentLevel) > 1)) continue;
                        if (level == currentLevel) break;
                    }
                    if ((!allLevels) && level != currentLevel) continue;

                    #region Calc Item
                    try
                    {
                        object value = null;
                        if (isCompilationMode)
                        {
                            if (method != null)
                            {
                                StiValueEventArgs e = new StiValueEventArgs();
                                method.Invoke(report, new Object[] { report, e });
                                //value = e.Value;
                                value = CompareValue(e.Value, filterCondition, value1, value2);
                            }
                        }
                        else
                        {
                            value = CompareValue(parser.ExecuteAsm(asmList), filterCondition, value1, value2);
                        }
                        function.CalcItem(value);
                    }
                    catch
                    {
                    }
                    #endregion
                }
                treeBand.Position = resPosition;
                #endregion

                treeBand.RestoreState("Totals");

                return function.GetValue();
            }
            #endregion

            return 0;
        }

        private static decimal CalculateDecimalByCondition(StiAggregateFunctionService function,
            object data, StiReport report, string name, StiFilterCondition filterCondition,
            decimal value1, decimal value2, bool allLevels, bool onlyChilds)
        {
            object value = CalculateByCondition(function, data, report, name, filterCondition, value1, value2, allLevels, onlyChilds);
            return StiObjectConverter.ConvertToDecimal(value);
        }

        private static object CompareValue(object value, StiFilterCondition filterCondition, decimal value1, decimal value2)
        {
            decimal result = 0;
            decimal.TryParse(value.ToString(), out result);
            switch (filterCondition)
            {
                case StiFilterCondition.Between:
                    return (result >= value1 && result <= value2) ? result : 0;
                case StiFilterCondition.EqualTo:
                    return (result == value1) ? result : 0;
                case StiFilterCondition.GreaterThan:
                    return (result > value1) ? result : 0;
                case StiFilterCondition.GreaterThanOrEqualTo:
                    return (result >= value1) ? result : 0;
                case StiFilterCondition.LessThan:
                    return (result < value1) ? result : 0;
                case StiFilterCondition.LessThanOrEqualTo:
                    return (result <= value1) ? result : 0;
                case StiFilterCondition.NotBetween:
                    return (result < value1 || result > value2) ? result : 0;
                case StiFilterCondition.NotEqualTo:
                    return (result != value1) ? result : 0;
                case StiFilterCondition.NotContaining:
                case StiFilterCondition.EndingWith:
                case StiFilterCondition.Containing:
                case StiFilterCondition.BeginningWith:
                    return 0;
            }

            return 0;
        }

		private static decimal CalculateDecimal(StiAggregateFunctionService function,
            object data, StiReport report, string name, bool allLevels, bool onlyChilds)
		{
            object value = Calculate(function, data, report, name, allLevels, onlyChilds, false);
			return StiObjectConverter.ConvertToDecimal(value);
		}

        private static decimal? CalculateDecimalNullable(StiAggregateFunctionService function,
            object data, StiReport report, string name, bool allLevels, bool onlyChilds)
        {
            object value = Calculate(function, data, report, name, allLevels, onlyChilds, false);
            if (value == null) return null;
            return StiObjectConverter.ConvertToDecimal(value);
        }

        private static double CalculateDouble(StiAggregateFunctionService function,
            object data, StiReport report, string name, bool allLevels, bool onlyChilds)
		{
            object value = Calculate(function, data, report, name, allLevels, onlyChilds, false);
			return StiObjectConverter.ConvertToDouble(value);
		}

	    private static Int64 CalculateInt64(StiAggregateFunctionService function,
            object data, StiReport report, string name, bool allLevels, bool onlyChilds)
		{
            object value = Calculate(function, data, report, name, allLevels, onlyChilds, false);
			return StiObjectConverter.ConvertToInt64(value);
		}

        private static object Calculate(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            return Calculate(function, data, report, name, false, false, true);
        }

        private static decimal CalculateDecimal(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            return StiObjectConverter.ConvertToDecimal(Calculate(function, data, report, name, false, false, true));
        }

        private static double CalculateDouble(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            return StiObjectConverter.ConvertToDouble(Calculate(function, data, report, name, false, false, true));
        }

        private static Int64 CalculateInt64(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            return StiObjectConverter.ConvertToInt64(Calculate(function, data, report, name, false, false, true));
        }

        private static object CalculateRunning(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            try
            {
                if (report == null)
                {
                    if (data is StiDataBand) report = (data as StiDataBand).Report;
                    else if (data is StiParser.StiParserData)
                    {
                        StiDataBand band = (data as StiParser.StiParserData).Data as StiDataBand;
                        if (band != null) report = band.Report;
                    }
                }
                report.Engine.StartIndexPageForPageTotal = 0;
                return Calculate(function, data, report, name, false, false, true);
            }
            finally
            {
                report.Engine.StartIndexPageForPageTotal = -1;
            }
        }

        private static decimal CalculateDecimalRunning(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            try
            {
                if (report == null)
                {
                    if (data is StiDataBand) report = (data as StiDataBand).Report;
                    else if (data is StiParser.StiParserData)
                    {
                        StiDataBand band = (data as StiParser.StiParserData).Data as StiDataBand;
                        if (band != null) report = band.Report;
                    }
                }
                report.Engine.StartIndexPageForPageTotal = 0;
                return StiObjectConverter.ConvertToDecimal(Calculate(function, data, report, name, false, false, true));
            }
            finally
            {
                report.Engine.StartIndexPageForPageTotal = -1;
            }
        }

        private static double CalculateDoubleRunning(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            try
            {
                if (report == null)
                {
                    if (data is StiDataBand) report = (data as StiDataBand).Report;
                    else if (data is StiParser.StiParserData)
                    {
                        StiDataBand band = (data as StiParser.StiParserData).Data as StiDataBand;
                        if (band != null) report = band.Report;
                    }
                }
                report.Engine.StartIndexPageForPageTotal = 0;
                return StiObjectConverter.ConvertToDouble(Calculate(function, data, report, name, false, false, true));
            }
            finally
            {
                report.Engine.StartIndexPageForPageTotal = -1;
            }
        }

        private static Int64 CalculateInt64Running(StiAggregateFunctionService function,
            object data, StiReport report, string name)
        {
            try
            {
                if (report == null)
                {
                    if (data is StiDataBand) report = (data as StiDataBand).Report;
                    else if (data is StiParser.StiParserData)
                    {
                        StiDataBand band = (data as StiParser.StiParserData).Data as StiDataBand;
                        if (band != null) report = band.Report;
                    }
                }
                report.Engine.StartIndexPageForPageTotal = 0;
                return StiObjectConverter.ConvertToInt64(Calculate(function, data, report, name, false, false, true));
            }
            finally
            {
                report.Engine.StartIndexPageForPageTotal = -1;
            }
        }
        #endregion

        #region Sum
        public static decimal Sum(object data, StiReport report, string name)
		{
			return CalculateDecimal(new StiSumDecimalFunctionService(), data, report, name, false, false);
		}

        public static decimal? SumNullable(object data, StiReport report, string name)
        {
            return CalculateDecimalNullable(new StiSumDecimalNullableFunctionService(), data, report, name, false, false);
        }

        public static decimal SumDistinct(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiSumDistinctDecimalFunctionService(), data, report, name, false, false);
        }
        public static decimal SumDistinct(object data, StiReport report, string name, string name2)
        {
            return CalculateDecimal(new StiSumDistinctDecimalFunctionService(), data, report, name + "~" + name2, false, false);
        }

        public static decimal cSum(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiSumDecimalFunctionService(), data, report, name);
        }

        public static decimal cSumRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiSumDecimalFunctionService(), data, report, name);
        }

		public static decimal SumAllLevels(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiSumDecimalFunctionService(), data, report, name, true, false);
		}

        public static decimal SumAllLevelsByCondition(object data, StiReport report, string name, StiFilterCondition filterCondition, decimal value1, decimal value2)
        {
            return CalculateDecimalByCondition(new StiSumDecimalFunctionService(), data, report, name, filterCondition, value1, value2, true, false);
        }

        public static decimal SumAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiSumDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal SumOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiSumDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region SumD
        public static double SumD(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiSumDoubleFunctionService(), data, report, name, false, false);
		}

        public static double cSumD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiSumDoubleFunctionService(), data, report, name);
        }

        public static double cSumDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiSumDoubleFunctionService(), data, report, name);
        }

		public static double SumDAllLevels(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiSumDoubleFunctionService(), data, report, name, true, false);
		}

        public static double SumDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiSumDoubleFunctionService(), data, report, name, true, true);
        }

        public static double SumDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiSumDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region SumI
        public static Int64 SumI(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiSumIntFunctionService(), data, report, name, false, false);
		}

        public static Int64 cSumI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiSumIntFunctionService(), data, report, name);
        }

        public static Int64 cSumIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiSumIntFunctionService(), data, report, name);
        }

		public static Int64 SumIAllLevels(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiSumIntFunctionService(), data, report, name, true, false);
		}

        public static Int64 SumIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiSumIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 SumIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiSumIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region SumTime
        public static TimeSpan SumTime(object data, StiReport report, string name)
        {
            object value = Calculate(new StiSumTimeFunctionService(), data, report, name, false, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan cSumTime(object data, StiReport report, string name)
        {
            return (TimeSpan)Calculate(new StiSumTimeFunctionService(), data, report, name);
        }

        public static TimeSpan SumTimeAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiSumTimeFunctionService(), data, report, name, true, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan SumTimeAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiSumTimeFunctionService(), data, report, name, true, true, false);
            return (TimeSpan)value;
        }

        public static TimeSpan SumTimeOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiSumTimeFunctionService(), data, report, name, false, true, false);
            return (TimeSpan)value;
        }
        #endregion

        #region Avg
        public static decimal Avg(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiAvgDecimalFunctionService(), data, report, name, false, false);
		}

        public static decimal cAvg(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiAvgDecimalFunctionService(), data, report, name);
        }

        public static decimal cAvgRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiAvgDecimalFunctionService(), data, report, name);
        }

		public static decimal AvgAllLevels(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiAvgDecimalFunctionService(), data, report, name, true, false);
		}

        public static decimal AvgAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiAvgDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal AvgOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiAvgDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region AvgDate
        public static DateTime AvgDate(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgDateFunctionService(), data, report, name, false, false, false);
            return (DateTime)value;
        }

        public static DateTime cAvgDate(object data, StiReport report, string name)
        {
            return (DateTime)Calculate(new StiAvgDateFunctionService(), data, report, name);
        }

        public static DateTime AvgDateAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgDateFunctionService(), data, report, name, true, false, false);
            return (DateTime)value;
        }

        public static DateTime AvgDateAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgDateFunctionService(), data, report, name, true, true, false);
            return (DateTime)value;
        }

        public static DateTime AvgDateOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgDateFunctionService(), data, report, name, false, true, false);
            return (DateTime)value;
        }
        #endregion

        #region AvgTime
        public static TimeSpan AvgTime(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgTimeFunctionService(), data, report, name, false, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan cAvgTime(object data, StiReport report, string name)
        {
            return (TimeSpan)Calculate(new StiAvgTimeFunctionService(), data, report, name);
        }

        public static TimeSpan AvgTimeAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgTimeFunctionService(), data, report, name, true, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan AvgTimeAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgTimeFunctionService(), data, report, name, true, true, false);
            return (TimeSpan)value;
        }

        public static TimeSpan AvgTimeOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiAvgTimeFunctionService(), data, report, name, false, true, false);
            return (TimeSpan)value;
        }
        #endregion

        #region AvgD
        public static double AvgD(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiAvgDoubleFunctionService(), data, report, name, false, false);
		}

        public static double cAvgD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiAvgDoubleFunctionService(), data, report, name);
        }

        public static double cAvgDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiAvgDoubleFunctionService(), data, report, name);
        }

		public static double AvgDAllLevels(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiAvgDoubleFunctionService(), data, report, name, true, false);
		}

        public static double AvgDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiAvgDoubleFunctionService(), data, report, name, true, true);
        }

        public static double AvgDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiAvgDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region AvgI
        public static Int64 AvgI(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiAvgIntFunctionService(), data, report, name, false, false);
		}

        public static Int64 cAvgI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiAvgIntFunctionService(), data, report, name);
        }

        public static Int64 cAvgIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiAvgIntFunctionService(), data, report, name);
        }

		public static Int64 AvgIAllLevels(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiAvgIntFunctionService(), data, report, name, true, false);
		}

        public static Int64 AvgIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiAvgIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 AvgIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiAvgIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region Max
        public static decimal Max(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiMaxDecimalFunctionService(), data, report, name, false, false);
		}

        public static decimal cMax(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMaxDecimalFunctionService(), data, report, name);
        }

        public static decimal cMaxRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiMaxDecimalFunctionService(), data, report, name);
        }

		public static decimal MaxAllLevels(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiMaxDecimalFunctionService(), data, report, name, true, false);
		}

        public static decimal MaxAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMaxDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal MaxOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMaxDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MaxD
        public static double MaxD(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiMaxDoubleFunctionService(), data, report, name, false, false);
		}

        public static double cMaxD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMaxDoubleFunctionService(), data, report, name);
        }

        public static double cMaxDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiMaxDoubleFunctionService(), data, report, name);
        }

		public static double MaxDAllLevels(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiMaxDoubleFunctionService(), data, report, name, true, false);
		}

        public static double MaxDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMaxDoubleFunctionService(), data, report, name, true, true);
        }

        public static double MaxDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMaxDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MaxI
        public static Int64 MaxI(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiMaxIntFunctionService(), data, report, name, false, false);
		}

        public static Int64 cMaxI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMaxIntFunctionService(), data, report, name);
        }

        public static Int64 cMaxIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiMaxIntFunctionService(), data, report, name);
        }

		public static Int64 MaxIAllLevels(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiMaxIntFunctionService(), data, report, name, true, false);
		}

        public static Int64 MaxIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMaxIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 MaxIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMaxIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region Min
        public static decimal Min(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiMinDecimalFunctionService(), data, report, name, false, false);
		}

        public static decimal cMin(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMinDecimalFunctionService(), data, report, name);
        }

        public static decimal cMinRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiMinDecimalFunctionService(), data, report, name);
        }

		public static decimal MinAllLevels(object data, StiReport report, string name)
		{
            return CalculateDecimal(new StiMinDecimalFunctionService(), data, report, name, true, false);
		}

        public static decimal MinAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMinDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal MinOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMinDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MinD
        public static double MinD(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiMinDoubleFunctionService(), data, report, name, false, false);
		}

        public static double cMinD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMinDoubleFunctionService(), data, report, name);
        }

        public static double cMinDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiMinDoubleFunctionService(), data, report, name);
        }

		public static double MinDAllLevels(object data, StiReport report, string name)
		{
            return CalculateDouble(new StiMinDoubleFunctionService(), data, report, name, true, false);
		}

        public static double MinDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMinDoubleFunctionService(), data, report, name, true, true);
        }

        public static double MinDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMinDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MinI
        public static Int64 MinI(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiMinIntFunctionService(), data, report, name, false, false);
		}

        public static Int64 cMinI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMinIntFunctionService(), data, report, name);
        }

        public static Int64 cMinIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiMinIntFunctionService(), data, report, name);
        }

		public static Int64 MinIAllLevels(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiMinIntFunctionService(), data, report, name, true, false);
		}

        public static Int64 MinIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMinIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 MinIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMinIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region Median
        public static decimal Median(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMedianDecimalFunctionService(), data, report, name, false, false);
        }

        public static decimal cMedian(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMedianDecimalFunctionService(), data, report, name);
        }

        public static decimal cMedianRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiMedianDecimalFunctionService(), data, report, name);
        }

        public static decimal MedianAllLevels(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMedianDecimalFunctionService(), data, report, name, true, false);
        }

        public static decimal MedianAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMedianDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal MedianOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiMedianDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MedianD
        public static double MedianD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMedianDoubleFunctionService(), data, report, name, false, false);
        }

        public static double cMedianD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMedianDoubleFunctionService(), data, report, name);
        }

        public static double cMedianDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiMedianDoubleFunctionService(), data, report, name);
        }

        public static double MedianDAllLevels(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMedianDoubleFunctionService(), data, report, name, true, false);
        }

        public static double MedianDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMedianDoubleFunctionService(), data, report, name, true, true);
        }

        public static double MedianDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiMedianDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MedianI
        public static Int64 MedianI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMedianIntFunctionService(), data, report, name, false, false);
        }

        public static Int64 cMedianI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMedianIntFunctionService(), data, report, name);
        }

        public static Int64 cMedianIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiMedianIntFunctionService(), data, report, name);
        }

        public static Int64 MedianIAllLevels(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMedianIntFunctionService(), data, report, name, true, false);
        }

        public static Int64 MedianIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMedianIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 MedianIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiMedianIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region Mode
        public static decimal Mode(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiModeDecimalFunctionService(), data, report, name, false, false);
        }

        public static decimal cMode(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiModeDecimalFunctionService(), data, report, name);
        }

        public static decimal cModeRunning(object data, StiReport report, string name)
        {
            return CalculateDecimalRunning(new StiModeDecimalFunctionService(), data, report, name);
        }

        public static decimal ModeAllLevels(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiModeDecimalFunctionService(), data, report, name, true, false);
        }

        public static decimal ModeAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiModeDecimalFunctionService(), data, report, name, true, true);
        }

        public static decimal ModeOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDecimal(new StiModeDecimalFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region ModeD
        public static double ModeD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiModeDoubleFunctionService(), data, report, name, false, false);
        }

        public static double cModeD(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiModeDoubleFunctionService(), data, report, name);
        }

        public static double cModeDRunning(object data, StiReport report, string name)
        {
            return CalculateDoubleRunning(new StiModeDoubleFunctionService(), data, report, name);
        }

        public static double ModeDAllLevels(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiModeDoubleFunctionService(), data, report, name, true, false);
        }

        public static double ModeDAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiModeDoubleFunctionService(), data, report, name, true, true);
        }

        public static double ModeDOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateDouble(new StiModeDoubleFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region ModeI
        public static Int64 ModeI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiModeIntFunctionService(), data, report, name, false, false);
        }

        public static Int64 cModeI(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiModeIntFunctionService(), data, report, name);
        }

        public static Int64 cModeIRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiModeIntFunctionService(), data, report, name);
        }

        public static Int64 ModeIAllLevels(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiModeIntFunctionService(), data, report, name, true, false);
        }

        public static Int64 ModeIAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiModeIntFunctionService(), data, report, name, true, true);
        }

        public static Int64 ModeIOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiModeIntFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region First
        public static object First(object data, StiReport report, string name)
		{
            return Calculate(new StiFirstFunctionService(), data, report, name, false, false, false);
		}

        public static object cFirst(object data, StiReport report, string name)
        {
            return Calculate(new StiFirstFunctionService(), data, report, name);
        }

        public static object cFirstRunning(object data, StiReport report, string name)
        {
            return CalculateRunning(new StiFirstFunctionService(), data, report, name);
        }

		public static object FirstAllLevels(object data, StiReport report, string name)
		{
            return Calculate(new StiFirstFunctionService(), data, report, name, true, false, false);
		}

        public static object FirstAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return Calculate(new StiFirstFunctionService(), data, report, name, true, true, false);
        }

        public static object FirstOnlyChilds(object data, StiReport report, string name)
        {
            return Calculate(new StiFirstFunctionService(), data, report, name, false, true, false);
        }
        #endregion

        #region Last
        public static object Last(object data, StiReport report, string name)
		{
            return Calculate(new StiLastFunctionService(), data, report, name, false, false, false);
		}

        public static object cLast(object data, StiReport report, string name)
        {
            return Calculate(new StiLastFunctionService(), data, report, name);
        }

        public static object cLastRunning(object data, StiReport report, string name)
        {
            return CalculateRunning(new StiLastFunctionService(), data, report, name);
        }

		public static object LastAllLevels(object data, StiReport report, string name)
		{
            return Calculate(new StiLastFunctionService(), data, report, name, true, false, false);
		}

        public static object LastAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return Calculate(new StiLastFunctionService(), data, report, name, true, true, false);
        }

        public static object LastOnlyChilds(object data, StiReport report, string name)
        {
            return Calculate(new StiLastFunctionService(), data, report, name, false, true, false);
        }
        #endregion

        #region Count
        public static Int64 Count(object data)
		{
            return CalculateInt64(new StiCountFunctionService(), data, null, null, false, false);
		}
        public static Int64 Count(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiCountFunctionService(), data, report, name, false, false);
        }

        public static Int64 cCount(object data)
        {
            return CalculateInt64(new StiCountFunctionService(), data, null, null);
        }
        public static Int64 cCount(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiCountFunctionService(), data, report, name);
        }

        public static Int64 cCountRunning(object data)
        {
            return CalculateInt64Running(new StiCountFunctionService(), data, null, null);
        }
        public static Int64 cCountRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiCountFunctionService(), data, report, name);
        }

		public static Int64 CountAllLevels(object data)
		{
            return CalculateInt64(new StiCountFunctionService(), data, null, null, true, false);
		}

        public static Int64 CountAllLevelsOnlyChilds(object data)
        {
            return CalculateInt64(new StiCountFunctionService(), data, null, null, true, true);
        }

        public static Int64 CountOnlyChilds(object data)
        {
            return CalculateInt64(new StiCountFunctionService(), data, null, null, false, true);
        }
        #endregion

        #region CountDistinct
        public static Int64 CountDistinct(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiCountDistinctFunctionService(), data, report, name, false, false);
		}

        public static Int64 cCountDistinct(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiCountDistinctFunctionService(), data, report, name);
        }

        public static Int64 cCountDistinctRunning(object data, StiReport report, string name)
        {
            return CalculateInt64Running(new StiCountDistinctFunctionService(), data, report, name);
        }

		public static Int64 CountDistinctAllLevels(object data, StiReport report, string name)
		{
            return CalculateInt64(new StiCountDistinctFunctionService(), data, report, name, true, false);
		}

        public static Int64 CountDistinctAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiCountDistinctFunctionService(), data, report, name, true, true);
        }

        public static Int64 CountDistinctOnlyChilds(object data, StiReport report, string name)
        {
            return CalculateInt64(new StiCountDistinctFunctionService(), data, report, name, false, true);
        }
        #endregion

        #region MinDate
        public static DateTime MinDate(object data, StiReport report, string name)
		{
            object value = Calculate(new StiMinDateFunctionService(), data, report, name, false, false, false);
			return (DateTime)value;
		}

        public static DateTime cMinDate(object data, StiReport report, string name)
        {
            return (DateTime)Calculate(new StiMinDateFunctionService(), data, report, name);
        }

        public static DateTime MinDateAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinDateFunctionService(), data, report, name, true, false, false);
            return (DateTime)value;
        }

        public static DateTime MinDateAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinDateFunctionService(), data, report, name, true, true, false);
            return (DateTime)value;
        }

        public static DateTime MinDateOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinDateFunctionService(), data, report, name, false, true, false);
            return (DateTime)value;
        }
        #endregion

        #region MinTime
        public static TimeSpan MinTime(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinTimeFunctionService(), data, report, name, false, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan cMinTime(object data, StiReport report, string name)
        {
            return (TimeSpan)Calculate(new StiMinTimeFunctionService(), data, report, name);
        }

        public static TimeSpan MinTimeAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinTimeFunctionService(), data, report, name, true, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan MinTimeAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinTimeFunctionService(), data, report, name, true, true, false);
            return (TimeSpan)value;
        }

        public static TimeSpan MinTimeOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinTimeFunctionService(), data, report, name, false, true, false);
            return (TimeSpan)value;
        }
        #endregion

        #region MinStr
        public static string MinStr(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinStrFunctionService(), data, report, name, false, false, false);
            return (string)value;
        }

        public static string cMinStr(object data, StiReport report, string name)
        {
            return (string)Calculate(new StiMinStrFunctionService(), data, report, name);
        }

        public static string MinStrAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinStrFunctionService(), data, report, name, true, false, false);
            return (string)value;
        }

        public static string MinStrAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinStrFunctionService(), data, report, name, true, true, false);
            return (string)value;
        }

        public static string MinStrOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMinStrFunctionService(), data, report, name, false, true, false);
            return (string)value;
        }
        #endregion

        #region MaxDate
        public static DateTime MaxDate(object data, StiReport report, string name)
		{
            object value = Calculate(new StiMaxDateFunctionService(), data, report, name, false, false, false);
			return (DateTime)value;
		}

        public static DateTime cMaxDate(object data, StiReport report, string name)
        {
            return (DateTime)Calculate(new StiMaxDateFunctionService(), data, report, name);
        }

		public static DateTime MaxDateAllLevels(object data, StiReport report, string name)
		{
            object value = Calculate(new StiMaxDateFunctionService(), data, report, name, true, false, false);
			return (DateTime)value;
		}

        public static DateTime MaxDateAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxDateFunctionService(), data, report, name, true, true, false);
            return (DateTime)value;
        }

        public static DateTime MaxDateOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxDateFunctionService(), data, report, name, false, true, false);
            return (DateTime)value;
        }
        #endregion

        #region MaxTime
        public static TimeSpan MaxTime(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxTimeFunctionService(), data, report, name, false, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan cMaxTime(object data, StiReport report, string name)
        {
            return (TimeSpan)Calculate(new StiMaxTimeFunctionService(), data, report, name);
        }

        public static TimeSpan MaxTimeAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxTimeFunctionService(), data, report, name, true, false, false);
            return (TimeSpan)value;
        }

        public static TimeSpan MaxTimeAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxTimeFunctionService(), data, report, name, true, true, false);
            return (TimeSpan)value;
        }

        public static TimeSpan MaxTimeOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxTimeFunctionService(), data, report, name, false, true, false);
            return (TimeSpan)value;
        }
        #endregion

        #region MaxStr
        public static string MaxStr(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxStrFunctionService(), data, report, name, false, false, false);
            return (string)value;
        }

        public static string cMaxStr(object data, StiReport report, string name)
        {
            return (string)Calculate(new StiMaxStrFunctionService(), data, report, name);
        }

        public static string MaxStrAllLevels(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxStrFunctionService(), data, report, name, true, false, false);
            return (string)value;
        }

        public static string MaxStrAllLevelsOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxStrFunctionService(), data, report, name, true, true, false);
            return (string)value;
        }

        public static string MaxStrOnlyChilds(object data, StiReport report, string name)
        {
            object value = Calculate(new StiMaxStrFunctionService(), data, report, name, false, true, false);
            return (string)value;
        }
        #endregion

        #region Rank
        public static int Rank(object data, StiReport report, string name)
        {
            return Rank(data, report, name, true, StiRankOrder.Asc);
        }

        public static int Rank(object data, StiReport report, string name, bool dense, StiRankOrder sortOrder)
        {
            object obj = Calculate(new StiRankFunctionService(false, dense, sortOrder), data, report, name, false, false, false);
            Hashtable hash = obj as Hashtable;
            if (hash != null && data != null)
            {
                object value = null;
                StiParser.StiParserData parserData = data as StiParser.StiParserData;
                if (parserData != null)
                {
                    List<StiParser.StiAsmCommand> asmList = parserData.AsmList;
                    StiParser parser = parserData.Parser;
                    value = parser.ExecuteAsm(asmList);
                }
                else
                {
                    if (name != null)
                    {
                        MethodInfo method = GetMethod(report, name);
                        if (method == null) return 0;
                        StiValueEventArgs e = new StiValueEventArgs();
                        method.Invoke(report, new Object[] { report, e });
                        value = e.Value;
                    }
                }
                if (value != null && value != DBNull.Value)
                {
                    return (int)hash[value];
                }
                return 0;
            }
            return 0;
        }
        #endregion

        private static void SaveState(StiDataBand dataBand)
        {
            dataBand.SaveState("Totals");
            if ((dataBand.DataSource != null) && (dataBand.DataSource is StiSqlSource))
            {
                (dataBand.DataSource as StiSqlSource).ReconnectOnEachRow = false;
            }
            if (dataBand.DataBandInfoV2 != null)
            {
                if (dataBand.DataBandInfoV2.DetailDataBands != null)
                {
                    foreach (StiDataBand detailBand in dataBand.DataBandInfoV2.DetailDataBands)
                    {
                        SaveState(detailBand);
                    }
                }
                if (dataBand.DataBandInfoV2.DetailDataBandsFromSubReports != null)
                {
                    foreach (DictionaryEntry de in dataBand.DataBandInfoV2.DetailDataBandsFromSubReports)
                    {
                        if (de.Value == null)
                        {
                            SaveState(de.Key as StiDataBand);
                        }
                    }
                }
            }
        }

        private static void RestoreState(StiDataBand dataBand)
        {
            dataBand.RestoreState("Totals");
            if (dataBand.DataBandInfoV2 != null)
            {
                if (dataBand.DataBandInfoV2.DetailDataBands != null)
                {
                    foreach (StiDataBand detailBand in dataBand.DataBandInfoV2.DetailDataBands)
                    {
                        RestoreState(detailBand);
                    }
                }
                if (dataBand.DataBandInfoV2.DetailDataBandsFromSubReports != null)
                {
                    foreach (DictionaryEntry de in dataBand.DataBandInfoV2.DetailDataBandsFromSubReports)
                    {
                        if (de.Value == null)
                        {
                            RestoreState(de.Key as StiDataBand);
                        }
                    }
                }
            }
        }

        private static bool GetCachedValue(StiAggregateFunctionService function, object data, StiReport report, string name, bool allLevels, bool onlyChilds, bool isPageTotal, ref object result, out string storeId)
        {
            StringBuilder asmListString = new StringBuilder();
            StiParser.StiParserData parserData = data as StiParser.StiParserData;
            if (parserData != null)
            {
                data = parserData.Data;
                if (parserData.AsmList != null)
                {
                    foreach (StiParser.StiAsmCommand asmCommand in parserData.AsmList)
                    {
                        asmListString.Append(asmCommand);
                        asmListString.Append("*");
                    }
                }
            }

            storeId = string.Format("{0}_*_{1}_*_{2}_*_{3}_*_{4}_*_{5}",
                function.ServiceName,
                name,
                asmListString,
                allLevels,
                onlyChilds,
                isPageTotal ? "PageTotal" + report.CurrentPrintPage : "False");

            if (report.CachedTotals == null) report.CachedTotals = new Hashtable();
            Hashtable list = report.CachedTotals[data] as Hashtable;
            if ((list != null) && list.ContainsKey(storeId))
            {
                result = list[storeId];
                return true;
            }

            return false;
        }

        private static void StoreCachedValue(object data, StiReport report, string storeId, object result)
        {
            StiParser.StiParserData parserData = data as StiParser.StiParserData;
            if (parserData != null)
            {
                data = parserData.Data;
            }

            Hashtable list = report.CachedTotals[data] as Hashtable;
            if (list == null)
            {
                list = new Hashtable();
                report.CachedTotals[data] = list;
            }
            list[storeId] = result;
        }


        private Totals()
		{
		}
	}
}
