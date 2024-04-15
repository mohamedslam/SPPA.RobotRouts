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
using System.Text;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{
	public sealed class StiDataHelper
	{
		public static void SetData(StiComponent component, bool reinit)
		{
			SetData(component, reinit, null);
		}
		
		public static void SetData(StiComponent component, bool reinit, StiComponent masterComponent)
        {
            #region IStiDataSource
            var dataSource = component as IStiDataSource;
			if (dataSource != null && dataSource.DataSource != null)
			{
				#region Prepare relation
				var dataRelation = component as IStiDataRelation;
				string relationName = null;				
				if (dataRelation != null && dataRelation.DataRelation != null)
				    relationName = dataRelation.DataRelationName;

				if (masterComponent == null)
				{
					var msComponent = component as IStiMasterComponent;
					if (!(msComponent != null && msComponent.MasterComponent != null))
					    relationName = null;
				}
				#endregion

				#region Filter
                var filterEventHandler = GetFilterEventHandler(component, dataSource);
				#endregion

				#region Sorts
				var dataSort = component as IStiSort;
				string[] sorts = null;				
				if (dataSort != null && dataSort.Sort != null && dataSort.Sort.Length > 0)
				    sorts = dataSort.Sort;
				#endregion
								
				dataSource.DataSource.SetData(component as StiDataBand, 
					relationName, filterEventHandler, sorts, reinit,
					component);
			}

			var enumerator = component as IStiEnumerator;
			if (enumerator != null)
			    enumerator.First();
            #endregion

            #region IStiBusinessObject
            var businessObject = component as IStiBusinessObject;
            var businessObjectData = businessObject?.BusinessObject;

            if (businessObjectData != null)
            {
                var masterDataBand = component as StiDataBand;
                var needOwnerBand = (masterDataBand != null &&
                    ((masterDataBand.Sort != null && masterDataBand.Sort.Length > 0) ||
                    (masterDataBand.FilterOn && masterDataBand.FilterMethodHandler != null) ||
                    (component.Report != null && component.Report.CalculationMode == StiCalculationMode.Interpretation && masterDataBand.FilterOn && masterDataBand.Filters.Count > 0) ||
                    masterDataBand is StiHierarchicalBand ||
                    NeedGroupSort(masterDataBand)));

                if (needOwnerBand)
                {
                    if (masterDataBand.BusinessObject.OwnerBand != masterDataBand)
                        masterDataBand.BusinessObject.OwnerBand = masterDataBand;
                }
                else
                {
                    if (masterDataBand != null)
                        masterDataBand.BusinessObject.OwnerBand = null;
                }
                    
                businessObjectData.SetDetails();
            }
            #endregion
        }

        internal static bool NeedGroupSort(StiDataBand band)
        {
            if (band == null || band.Report == null) return false;

            var groupHeaderComponents = band.Report.EngineVersion == StiEngineVersion.EngineV1 
                ? band.DataBandInfoV1.GroupHeaderComponents 
                : band.DataBandInfoV2.GroupHeaders;

            if (groupHeaderComponents == null || groupHeaderComponents.Count == 0) return false;

            var groupCount = 0;
            foreach (StiGroupHeaderBand groupHeader in groupHeaderComponents)
            {
                if (groupHeader.SortDirection != StiGroupSortDirection.None)
                    groupCount++;
            }

            return groupCount > 0;
        }

        public static object GetFilterEventHandler(StiComponent component, object dataSource)
        {
            var dataFilter = component as IStiFilter;
            object filterEventHandler = null;

            if (dataFilter != null && dataFilter.FilterMethodHandler != null)
                filterEventHandler = dataFilter.FilterMethodHandler;

            StiReport report = null;
            if (component != null && component.Report != null)
                report = component.Report;

            if (report == null && dataSource != null && dataSource is StiBusinessObject)
            {
                if ((dataSource as StiBusinessObject).Dictionary != null)
                    report = (dataSource as StiBusinessObject).Dictionary.Report;
            }

            if (report != null && (report.CalculationMode == StiCalculationMode.Interpretation || report.IsDesigning))
            {
                #region Interpretation mode
                if (dataFilter != null && dataFilter.FilterOn && dataFilter.Filters.Count > 0)
                {
                    var filterExpression = new StringBuilder("{");
                    for (var index = 0; index < dataFilter.Filters.Count; index++)
                    {
                        var filter = dataFilter.Filters[index];
                        filterExpression.Append("(");
                        
                        if (dataSource is IStiDataSource)
                            filterExpression.Append(GetFilterExpression(filter, StiNameValidator.CorrectName(((IStiDataSource)dataSource).DataSourceName, report) + "." + filter.Column, report));
                        else
                            filterExpression.Append(GetFilterExpression(filter, filter.Column, report));

                        filterExpression.Append(")");
                        if (index < dataFilter.Filters.Count - 1)
                        {
                            if (report != null && report.ScriptLanguage == StiReportLanguageType.VB)
                                filterExpression.Append(dataFilter.FilterMode == StiFilterMode.And ? " And " : " Or ");
                            else
                                filterExpression.Append(dataFilter.FilterMode == StiFilterMode.And ? " && " : " || ");
                        }
                    }
                    filterExpression.Append("}");
                    filterEventHandler = new StiParser.StiFilterParserData(component, filterExpression.ToString());

                    #region Check syntax of expression
                    var ex = StiParser.CheckExpression(filterExpression.ToString(), component);
                    if (ex != null)
                    {
                        StiLogService.Write("StiDataSource Filters ...ERROR");
                        StiLogService.Write(ex as Exception);

                        if (report != null)
                            report.WriteToReportRenderingMessages(component.Name + ".Filters " + (ex as Exception).Message);
                    }
                    #endregion
                }
                #endregion
            }
            return filterEventHandler;
        }

        public static string GetFilterExpression(StiFilter filter, string fullColumnName, StiReport report)
        {
            var filterExpression = new StringBuilder();
            
            #region StiFilterItem.Expression
            if (filter.Item == StiFilterItem.Expression)
            {
                var st = filter.Expression.Value;
                if (st != null)
                {
                    if (st.StartsWith("{") && st.EndsWith("}"))
                        filterExpression.Append(st.Substring(1, st.Length - 2));
                    else
                        filterExpression.Append(st);
                }
            }
            #endregion

            #region StiFilterItem.Value
            else if (filter.Item == StiFilterItem.Value)
            {
                if (report != null && report.ScriptLanguage == StiReportLanguageType.VB)
                {
                    //it is necessary to rewrite the code when the parser will better support VB

                    #region StiFilterCondition.IsNull && StiFilterCondition.IsNotNull
                    if (filter.Condition == StiFilterCondition.IsNull || filter.Condition == StiFilterCondition.IsNotNull)
                    {
                        string expr;
                        var posDot = fullColumnName.LastIndexOf('.');
                        if (posDot < 0)
                        {
                            expr = fullColumnName;
                        }
                        else
                        {
                            var dsName = fullColumnName.Substring(0, posDot);
                            var colName = fullColumnName.Substring(posDot + 1);
                            expr = $"{dsName}[\"{colName}\"]";
                        }

                        switch (filter.Condition)
                        {
                            case StiFilterCondition.IsNull:
                                filterExpression.Append($"{expr} = null Or {expr} = DBNull.Value");
                                break;

                            default:
                                filterExpression.Append($"{expr} <> null And {expr} <> DBNull.Value");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.String
                    else if (filter.DataType == StiFilterDataType.String)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"{fullColumnName}.ToString().ToLower() = \"{report.ToString(filter.Value1).ToLower()}\"");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"{fullColumnName}.ToString().ToLower() <> \"{report.ToString(filter.Value1).ToLower()}\"");
                                break;

                            case StiFilterCondition.Containing:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().IndexOf(\"{report.ToString(filter.Value1).ToLower()}\") <> -1");
                                break;

                            case StiFilterCondition.NotContaining:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().IndexOf(\"{report.ToString(filter.Value1).ToLower()}\") = -1");
                                break;

                            case StiFilterCondition.BeginningWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().StartsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;

                            case StiFilterCondition.EndingWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().EndsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Numeric
                    else if (filter.DataType == StiFilterDataType.Numeric)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} = (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} <> (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"(decimal){fullColumnName} >= (decimal){filter.Value1} And (decimal){fullColumnName} <= (decimal){filter.Value2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"(decimal){fullColumnName} < (decimal){filter.Value1} Or (decimal){fullColumnName} > (decimal){filter.Value2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"(decimal){fullColumnName} > (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} >= (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"(decimal){fullColumnName} < (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} <= (decimal){filter.Value1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.DateTime
                    else if (filter.DataType == StiFilterDataType.DateTime)
                    {
                        var dt1 = string.Empty;
                        if (!string.IsNullOrEmpty(filter.Value1))
                        {
                            var parts = filter.Value1.Split('/');
                            dt1 = $"DateSerial({parts[2]},{parts[0]},{parts[1]})";
                        }

                        var dt2 = string.Empty;
                        if (!string.IsNullOrEmpty(filter.Value2))
                        {
                            var parts = filter.Value2.Split('/');
                            dt2 = $"DateSerial({parts[2]},{parts[0]},{parts[1]})";
                        }

                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} = {dt1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} <> {dt1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"(DateTime){fullColumnName} >= {dt1} And (DateTime){fullColumnName} <= {dt2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"(DateTime){fullColumnName} < {dt1} Or (DateTime){fullColumnName} > {dt2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"(DateTime){fullColumnName} > {dt1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} >= {dt1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"(DateTime){fullColumnName} < {dt1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} <= {dt1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Boolean
                    else if (filter.DataType == StiFilterDataType.Boolean)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(bool){fullColumnName} = {filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(bool){fullColumnName} <> {filter.Value1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Expression
                    else if (filter.DataType == StiFilterDataType.Expression)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"{fullColumnName} = {filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"{fullColumnName} <> ){filter.Value1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"{fullColumnName} >= {filter.Value1} And {fullColumnName} <= {filter.Value2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"{fullColumnName} < {filter.Value1} Or {fullColumnName} > {filter.Value2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"{fullColumnName} > {filter.Value1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"{fullColumnName} >= {filter.Value1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"{fullColumnName} < {filter.Value1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"{fullColumnName} <= {filter.Value1}");
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region StiFilterCondition.IsNull && StiFilterCondition.IsNotNull
                    if (filter.Condition == StiFilterCondition.IsNull || filter.Condition == StiFilterCondition.IsNotNull)
                    {
                        string expr;
                        var posDot = fullColumnName.LastIndexOf('.');
                        if (posDot < 0)
                        {
                            expr = fullColumnName;
                        }
                        else
                        {
                            var dsName = fullColumnName.Substring(0, posDot);
                            var colName = fullColumnName.Substring(posDot + 1);
                            expr = $"{dsName}[\"{colName}\"]";
                        }

                        switch (filter.Condition)
                        {
                            case StiFilterCondition.IsNull:
                                filterExpression.Append($"{expr} == null || {expr} == DBNull.Value");
                                break;

                            default:
                                filterExpression.Append($"{expr} != null && {expr} != DBNull.Value");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.String
                    else if (filter.DataType == StiFilterDataType.String)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"{fullColumnName}.ToString().ToLower() == \"{report.ToString(filter.Value1).ToLower()}\"");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"{fullColumnName}.ToString().ToLower() != \"{report.ToString(filter.Value1).ToLower()}\"");
                                break;

                            case StiFilterCondition.Containing:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().IndexOf(\"{report.ToString(filter.Value1).ToLower()}\") != -1");
                                break;

                            case StiFilterCondition.NotContaining:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().IndexOf(\"{report.ToString(filter.Value1).ToLower()}\") == -1");
                                break;

                            case StiFilterCondition.BeginningWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().StartsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;

                            case StiFilterCondition.EndingWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().EndsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Numeric
                    else if (filter.DataType == StiFilterDataType.Numeric)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} == (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} != (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"(decimal){fullColumnName} >= (decimal){filter.Value1} && (decimal){fullColumnName} <= (decimal){filter.Value2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"(decimal){fullColumnName} < (decimal){filter.Value1} || (decimal){fullColumnName} > (decimal){filter.Value2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"(decimal){fullColumnName} > (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} >= (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"(decimal){fullColumnName} < (decimal){filter.Value1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"(decimal){fullColumnName} <= (decimal){filter.Value1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.DateTime
                    else if (filter.DataType == StiFilterDataType.DateTime)
                    {
                        var dt1 = string.Empty;
                        if (!string.IsNullOrEmpty(filter.Value1))
                        {
                            var parts = filter.Value1.Split('/');
                            dt1 = $"DateSerial({parts[2]},{parts[0]},{parts[1]})";
                        }

                        var dt2 = string.Empty;
                        if (!string.IsNullOrEmpty(filter.Value2))
                        {
                            var parts = filter.Value2.Split('/');
                            dt2 = $"DateSerial({parts[2]},{parts[0]},{parts[1]})";
                        }

                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} == {dt1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} != {dt1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"(DateTime){fullColumnName} >= {dt1} && (DateTime){fullColumnName} <= {dt2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"(DateTime){fullColumnName} < {dt1} || (DateTime){fullColumnName} > {dt2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"(DateTime){fullColumnName} > {dt1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} >= {dt1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"(DateTime){fullColumnName} < {dt1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"(DateTime){fullColumnName} <= {dt1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Boolean
                    else if (filter.DataType == StiFilterDataType.Boolean)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"(bool){fullColumnName} == {filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"(bool){fullColumnName} != {filter.Value1}");
                                break;
                        }
                    }
                    #endregion

                    #region StiFilterDataType.Expression
                    else if (filter.DataType == StiFilterDataType.Expression)
                    {
                        switch (filter.Condition)
                        {
                            case StiFilterCondition.EqualTo:
                                filterExpression.Append($"{fullColumnName} == {filter.Value1}");
                                break;

                            case StiFilterCondition.NotEqualTo:
                                filterExpression.Append($"{fullColumnName} != {filter.Value1}");
                                break;

                            case StiFilterCondition.Between:
                                filterExpression.Append($"{fullColumnName} >= {filter.Value1} && {fullColumnName} <= {filter.Value2}");
                                break;

                            case StiFilterCondition.NotBetween:
                                filterExpression.Append($"{fullColumnName} < {filter.Value1} || {fullColumnName} > {filter.Value2}");
                                break;

                            case StiFilterCondition.GreaterThan:
                                filterExpression.Append($"{fullColumnName} > {filter.Value1}");
                                break;

                            case StiFilterCondition.GreaterThanOrEqualTo:
                                filterExpression.Append($"{fullColumnName} >= {filter.Value1}");
                                break;

                            case StiFilterCondition.LessThan:
                                filterExpression.Append($"{fullColumnName} < {filter.Value1}");
                                break;

                            case StiFilterCondition.LessThanOrEqualTo:
                                filterExpression.Append($"{fullColumnName} <= {filter.Value1}");
                                break;

                            case StiFilterCondition.Containing:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().Contains(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;

                            case StiFilterCondition.NotContaining:
                                filterExpression.Append($"!ToString({fullColumnName}).ToLower().Contains(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;

                            case StiFilterCondition.BeginningWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().StartsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;

                            case StiFilterCondition.EndingWith:
                                filterExpression.Append($"ToString({fullColumnName}).ToLower().EndsWith(\"{report.ToString(filter.Value1).ToLower()}\")");
                                break;
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return filterExpression.ToString();
        }
	}
}
