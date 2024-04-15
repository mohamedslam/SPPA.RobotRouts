#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.IO;
using System.Data;
using Stimulsoft.Base.Localization;
using System.Xml;
using System.Collections;
using Stimulsoft.Base;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Stimulsoft.Report.Design;
using Stimulsoft.Report.Components;
using System.Reflection;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Helpers;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.CodeDom;
using System.Threading;
using Stimulsoft.Report.CrossTab;
using Stimulsoft.Report.Check.Helper;
using Stimulsoft.Report.Web.Helpers.Dashboards;
using Stimulsoft.Report.SaveLoad;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dictionary.Databases.Azure;
using Stimulsoft.Report.Dictionary.Databases.Google;
using System.Net;
using Stimulsoft.Report.Maps;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Exceptions;
using Stimulsoft.Report.Import;
using Stimulsoft.Base.Data.Connectors;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiDictionaryHelper
    {
        private static CultureInfo en_us_culture = CultureInfo.CreateSpecificCulture("en-US");
        private static int maxLevelRelations = 4;

        #region Database Item
        private static Hashtable DatabaseItem(string name, string nameInSource, string alias, bool dataInStore)
        {
            Hashtable dataBaseItem = new Hashtable();
            dataBaseItem["typeItem"] = "DataBase";
            dataBaseItem["typeIcon"] = dataInStore ? "DataStore" : "ConnectionFail";
            dataBaseItem["typeConnection"] = null;
            dataBaseItem["name"] = name;
            dataBaseItem["alias"] = alias;
            dataBaseItem["nameInSource"] = nameInSource;
            dataBaseItem["dataInStore"] = dataInStore;
            dataBaseItem["dataSources"] = new ArrayList();

            return dataBaseItem;
        }

        private static Hashtable DatabaseItem(StiDatabase database, StiReport report)
        {
            Hashtable dataBaseItem = DatabaseItem(database.Name, database.ToString(), database.Alias, false);
            dataBaseItem["typeIcon"] = (database is StiUndefinedDatabase) ? (database.Inherited ? "LockedConnection" : "UndefinedConnection") : "Connection";
            dataBaseItem["typeConnection"] = database.GetType().Name;
            dataBaseItem["serviceName"] = database.ServiceName;
            dataBaseItem["restrictions"] = GetDictionaryRestrictions(database.Name, StiDataType.Database, report?.Dictionary);

            if (database is StiFileDatabase)
            {
                dataBaseItem["pathData"] = ((StiFileDatabase)database).PathData != null ? StiEncodingHelper.Encode(((StiFileDatabase)database).PathData) : string.Empty;
            }
            else
            {
                var connectionStringProp = database.GetType().GetProperty("ConnectionString");
                dataBaseItem["connectionString"] = (connectionStringProp != null) ? StiEncodingHelper.Encode((string)connectionStringProp.GetValue(database, null)) : "";
                var promptUserNameAndPasswordProp = database.GetType().GetProperty("PromptUserNameAndPassword");
                dataBaseItem["promptUserNameAndPassword"] = (promptUserNameAndPasswordProp != null) ? (bool)promptUserNameAndPasswordProp.GetValue(database, null) : false;
            }

            if (database is StiXmlDatabase)
            {
                dataBaseItem["pathSchema"] = ((StiXmlDatabase)database).PathSchema != null ? StiEncodingHelper.Encode(((StiXmlDatabase)database).PathSchema) : string.Empty;
                dataBaseItem["xmlType"] = ((StiXmlDatabase)database).XmlType;
                dataBaseItem["relationDirection"] = ((StiXmlDatabase)database).RelationDirection;
            }            
            else if (database is StiJsonDatabase)
            {
                dataBaseItem["relationDirection"] = ((StiJsonDatabase)database).RelationDirection;
            }
            else if (database is StiExcelDatabase)
            {
                dataBaseItem["firstRowIsHeader"] = ((StiExcelDatabase)database).FirstRowIsHeader;
            }
            else if (database is StiDBaseDatabase)
            {
                dataBaseItem["codePage"] = ((StiDBaseDatabase)database).CodePage.ToString();
            }
            else if (database is StiCsvDatabase)
            {
                dataBaseItem["codePage"] = ((StiCsvDatabase)database).CodePage.ToString();
                dataBaseItem["separator"] = ((StiCsvDatabase)database).Separator;
            }
            else if (database is StiODataDatabase)
            {
                dataBaseItem["version"] = ((StiODataDatabase)database).Version;
            }
            else if (database is StiGisDatabase)
            {
                dataBaseItem["dataType"] = ((StiGisDatabase)database).DataType;
                dataBaseItem["separator"] = ((StiGisDatabase)database).Separator;
            }
            else if (database is StiGoogleSheetsDatabase)
            {
                var googleDatabase = database as StiGoogleSheetsDatabase;
                dataBaseItem["clientId"] = googleDatabase.ClientId != null ? StiEncodingHelper.Encode(googleDatabase.ClientId) : string.Empty;
                dataBaseItem["clientSecret"] = googleDatabase.ClientSecret != null ? StiEncodingHelper.Encode(googleDatabase.ClientSecret) : string.Empty;
                dataBaseItem["spreadsheetId"] = googleDatabase.SpreadsheetId != null ? StiEncodingHelper.Encode(googleDatabase.SpreadsheetId) : string.Empty;
                dataBaseItem["firstRowIsHeader"] = ((StiGoogleSheetsDatabase)database).FirstRowIsHeader;
            }
            else if (database is StiDataWorldDatabase)
            {
                var dataWorldDatabase = database as StiDataWorldDatabase;
                dataBaseItem["owner"] = dataWorldDatabase.Owner != null ? StiEncodingHelper.Encode(dataWorldDatabase.Owner) : string.Empty;
                dataBaseItem["database"] = dataWorldDatabase.Database != null ? StiEncodingHelper.Encode(dataWorldDatabase.Database) : string.Empty;
                dataBaseItem["token"] = dataWorldDatabase.Token != null ? StiEncodingHelper.Encode(dataWorldDatabase.Token) : string.Empty;
            }
            else if (database is StiBigQueryDatabase)
            {
                var bigQueryDatabase = database as StiBigQueryDatabase;
                dataBaseItem["base64EncodedAuthSecret"] = bigQueryDatabase.Base64EncodedAuthSecret;
                dataBaseItem["projectId"] = bigQueryDatabase.ProjectId != null ? StiEncodingHelper.Encode(bigQueryDatabase.ProjectId) : string.Empty;
                dataBaseItem["datasetId"] = bigQueryDatabase.DatasetId != null ? StiEncodingHelper.Encode(bigQueryDatabase.DatasetId) : string.Empty;
            }
            else if (database is StiGraphQLDatabase)
            {
                var graphQLDatabase = database as StiGraphQLDatabase;
                dataBaseItem["connectionString"] = UnPackGraphSqlConnectionString(graphQLDatabase.ConnectionString);
            }

            return dataBaseItem;
        }
        #endregion

        #region DataTransformationCategory Item
        private static Hashtable DataTransformationCategoryItem(string name, string nameInSource, string alias)
        {
            Hashtable item = DatabaseItem(name, nameInSource, alias, false);
            item["typeIcon"] = "DataTransformationCategory";

            return item;
        }
        #endregion

        #region Datasource Item
        private static Hashtable DatasourceItem(StiDataSource datasource)
        {
            string typeIcon = "DataSource";
            if (datasource is StiUndefinedDataSource)
                typeIcon = "UndefinedDataSource";
            else if (datasource.Inherited)
                typeIcon = "LockedDataSource";

            Hashtable datasourceItem = new Hashtable();
            datasourceItem["typeItem"] = "DataSource";
            datasourceItem["typeIcon"] = datasource.IsCloud ? "CloudDataSource" : typeIcon;
            datasourceItem["isCloud"] = datasource.IsCloud;
            datasourceItem["typeDataSource"] = datasource.GetType().Name;
            StiDataAdapterService dataAdapter = datasource.GetDataAdapter();
            if (dataAdapter != null) datasourceItem["typeDataAdapter"] = dataAdapter.GetType().Name;
            datasourceItem["name"] = datasource.Name;
            datasourceItem["correctName"] = StiNameValidator.CorrectName(datasource.Name);
            datasourceItem["nameInSource"] = ((StiDataStoreSource)datasource).NameInSource;
            datasourceItem["alias"] = datasource.Alias;
            datasourceItem["relations"] = GetRelationsTree(null, datasource.GetParentRelations(), datasource.IsCloud, new Hashtable());
            datasourceItem["parameters"] = GetParametersTree(datasource.Parameters, datasource.IsCloud);
            datasourceItem["columns"] = GetColumnsTree(datasource.Columns, datasource.IsCloud);
            datasourceItem["restrictions"] = GetDictionaryRestrictions(datasource.Name, StiDataType.DataSource, datasource.Dictionary);

            if (datasource is StiSqlSource && !(datasource is StiNoSqlSource) && !(datasource is StiODataSource))
            {
                datasourceItem["parameterTypes"] = GetDataParameterTypes(datasource as StiSqlSource);
                datasourceItem["reconnectOnEachRow"] = ((StiSqlSource)datasource).ReconnectOnEachRow;
                datasourceItem["commandTimeout"] = ((StiSqlSource)datasource).CommandTimeout.ToString();
                datasourceItem["allowExpressions"] = ((StiSqlSource)datasource).AllowExpressions;
                datasourceItem["connectOnStart"] = ((StiSqlSource)datasource).ConnectOnStart;
            }

            var sqlCommandProp = datasource.GetType().GetProperty("SqlCommand");
            if (sqlCommandProp != null)
                datasourceItem["sqlCommand"] = StiEncodingHelper.Encode((string)sqlCommandProp.GetValue(datasource, null));

            var typeProp = datasource.GetType().GetProperty("Type");
            if (sqlCommandProp != null)
                datasourceItem["type"] = ((StiSqlSourceType)typeProp.GetValue(datasource, null)).ToString();

            if (datasource is StiVirtualSource)
            {
                datasourceItem["nameInSource"] = ((StiVirtualSource)datasource).NameInSource;
                datasourceItem["sortData"] = StiReportEdit.GetSortDataProperty(datasource);
                datasourceItem["filterData"] = StiEncodingHelper.Encode(JSON.Encode(StiReportEdit.GetFiltersObject(((StiVirtualSource)datasource).Filters)));
                datasourceItem["filterMode"] = ((StiVirtualSource)datasource).FilterMode.ToString();
                datasourceItem["filterOn"] = ((StiVirtualSource)datasource).FilterOn;
                datasourceItem["groupsData"] = GetGroupColumnsProperty(datasource as StiVirtualSource);
                datasourceItem["resultsData"] = GetResultsProperty(datasource as StiVirtualSource);
            }

            return datasourceItem;
        }
        #endregion

        #region DataTransformation Item
        public static Hashtable DataTransformationItem(StiDataTransformation dataTransformation)
        {
            Hashtable datasourceItem = new Hashtable();
            datasourceItem["typeItem"] = "DataSource";
            datasourceItem["typeIcon"] = dataTransformation.Inherited ? "LockedDataTransformation" : "DataTransformation";
            datasourceItem["typeDataSource"] = dataTransformation.GetType().Name;
            datasourceItem["name"] = dataTransformation.Name;
            datasourceItem["correctName"] = StiNameValidator.CorrectName(dataTransformation.Name);
            datasourceItem["nameInSource"] = ((StiDataStoreSource)dataTransformation).NameInSource;
            datasourceItem["alias"] = dataTransformation.Alias;
            datasourceItem["relations"] = GetRelationsTree(null, dataTransformation.GetParentRelations(), false, new Hashtable());
            datasourceItem["columns"] = StiDataTransformationHelper.GetColumns(dataTransformation);
            datasourceItem["sortRules"] = StiDataTransformationHelper.GetSortRules(dataTransformation);
            datasourceItem["filterRules"] = StiDataTransformationHelper.GetFilterRules(dataTransformation);
            datasourceItem["actionRules"] = StiDataTransformationHelper.GetActionRules(dataTransformation);

            return datasourceItem;
        }
        #endregion

        #region Column Item
        private static Hashtable ColumnItem(StiDataColumn column)
        {
            Hashtable columnItem = new Hashtable();
            columnItem["typeItem"] = "Column";
            columnItem["typeIcon"] = GetIconTypeForColumn(column).ToString();
            columnItem["type"] = StiDataFiltersHelper.TypeToString(column.Type);
            columnItem["name"] = column.Name;
            columnItem["correctName"] = StiNameValidator.CorrectName(column.Name);
            columnItem["alias"] = column.Alias;
            columnItem["nameInSource"] = column.NameInSource;
            columnItem["isCalcColumn"] = column is StiCalcDataColumn;
            columnItem["expression"] = column is StiCalcDataColumn ? StiEncodingHelper.Encode(((StiCalcDataColumn)column).Expression) : "";
            columnItem["restrictions"] = GetDictionaryRestrictions(column.Name, StiDataType.DataColumn, column.DataSource?.Dictionary);

            return columnItem;
        }
        #endregion

        #region Parameter Item
        private static Hashtable ParameterItem(StiDataParameter parameter)
        {
            Hashtable parameterItem = new Hashtable();
            parameterItem["typeItem"] = "Parameter";
            parameterItem["typeIcon"] = "Parameter";
            parameterItem["type"] = parameter.Type.ToString();
            parameterItem["size"] = parameter.Size.ToString();
            parameterItem["name"] = parameter.Name;
            parameterItem["correctName"] = StiNameValidator.CorrectName(parameter.Name);
            parameterItem["expression"] = StiEncodingHelper.Encode(parameter.Expression);

            return parameterItem;
        }
        #endregion

        #region Relation Item
        private static Hashtable RelationItem(StiDataRelation relation, Hashtable upLevelRelations, int level = 1)
        {
            Hashtable relationItem = new Hashtable();
            relationItem["typeItem"] = "Relation";
            relationItem["typeIcon"] = relation.IsCloud ? "CloudRelation" : (relation.Inherited ? "LockedRelation" : (relation.Active ? "RelationActive" : "Relation"));
            relationItem["isCloud"] = relation.IsCloud;
            relationItem["name"] = relation.Name;
            relationItem["correctName"] = StiNameValidator.CorrectName(relation.Name);
            relationItem["alias"] = relation.Alias;
            relationItem["nameInSource"] = relation.NameInSource;
            relationItem["active"] = relation.Active;
            relationItem["parentDataSource"] = relation.ParentSource != null ? relation.ParentSource.Name : "";
            relationItem["childDataSource"] = relation.ChildSource != null ? relation.ChildSource.Name : "";
            ArrayList jsParentColumns = new ArrayList();
            foreach (string parentColumns in relation.ParentColumns) jsParentColumns.Add(parentColumns);
            relationItem["parentColumns"] = jsParentColumns;
            ArrayList jsChildColumns = new ArrayList();
            foreach (string childColumns in relation.ChildColumns) jsChildColumns.Add(childColumns);
            relationItem["childColumns"] = jsChildColumns;
            relationItem["restrictions"] = GetDictionaryRestrictions(relation.Name, StiDataType.DataRelation, relation.Dictionary);

            upLevelRelations[relation.NameInSource] = true;
            relationItem["relations"] = relation.ParentSource != null && level < maxLevelRelations
                ? GetRelationsTree(relation, relation.ParentSource.GetParentRelations(), relation.ParentSource.IsCloud, upLevelRelations, level + 1)
                : new ArrayList();
            upLevelRelations[relation.NameInSource] = null;

            

            return relationItem;
        }
        #endregion

        #region BusinessObject Item
        private static Hashtable BusinessObjectItem(StiBusinessObject businessObject)
        {
            Hashtable businessObjectItem = new Hashtable();
            businessObjectItem["typeItem"] = "BusinessObject";
            businessObjectItem["typeIcon"] = "BusinessObject";
            businessObjectItem["category"] = businessObject.Category;
            businessObjectItem["name"] = businessObject.Name;
            businessObjectItem["correctName"] = StiNameValidator.CorrectName(businessObject.Name);
            businessObjectItem["alias"] = businessObject.Alias;
            businessObjectItem["columns"] = GetColumnsTree(businessObject.Columns);
            businessObjectItem["businessObjects"] = GetChildBusinessObjectsTree(businessObject);
            businessObjectItem["fullName"] = businessObject.GetFullName();
            businessObjectItem["restrictions"] = GetDictionaryRestrictions(businessObject.Name, StiDataType.DataColumn, businessObject.Dictionary);

            return businessObjectItem;
        }
        #endregion

        #region Variable Item
        private static Hashtable VariableItem(StiVariable variable, StiReport report)
        {
            var variableType = GetVariableType(variable);
            var variableItem = new Hashtable();

            variableItem["typeItem"] = "Variable";
            variableItem["typeIcon"] = GetDataColumnImageIdFromType(variable.Type, !variable.Inherited).ToString();
            variableItem["basicType"] = GetVariableBasicType(variable);
            variableItem["type"] = variableType;
            variableItem["name"] = variable.Name;
            variableItem["correctName"] = StiNameValidator.CorrectName(variable.Name);
            variableItem["alias"] = variable.Alias;
            variableItem["category"] = variable.Category;
            variableItem["description"] = StiEncodingHelper.Encode(variable.Description);
            variableItem["initBy"] = variable.InitBy;
            variableItem["readOnly"] = variable.ReadOnly;
            variableItem["allowUseAsSqlParameter"] = variable.AllowUseAsSqlParameter;
            variableItem["requestFromUser"] = variable.RequestFromUser;
            variableItem["allowUserValues"] = variable.DialogInfo.AllowUserValues;
            variableItem["dateTimeFormat"] = variable.DialogInfo.DateTimeType;
            variableItem["sortDirection"] = variable.DialogInfo.SortDirection;
            variableItem["sortField"] = variable.DialogInfo.SortField;
            variableItem["dataSource"] = variable.DialogInfo.ItemsInitializationType;
            variableItem["selection"] = variable.Selection.ToString();
            variableItem["formatMask"] = StiEncodingHelper.Encode(variable.DialogInfo.Mask);
            variableItem["items"] = GetItems(variable, (string)variableItem["type"]);
            variableItem["keys"] = variable.DialogInfo.KeysColumn;
            variableItem["values"] = variable.DialogInfo.ValuesColumn;
            variableItem["checkedColumn"] = variable.DialogInfo.CheckedColumn;
            variableItem["checkedStates"] = new ArrayList(variable.DialogInfo.CheckedStates);
            variableItem["dependentValue"] = variable.DialogInfo.BindingValue;
            variableItem["dependentVariable"] = variable.DialogInfo.BindingVariable != null ? variable.DialogInfo.BindingVariable.Name : "";
            variableItem["dependentColumn"] = variable.DialogInfo.BindingValuesColumn;
            variableItem["restrictions"] = GetDictionaryRestrictions(variable.Name, StiDataType.Variable, report.Dictionary);

            #region Get Value & Expression
            switch ((string)variableItem["basicType"])
            {
                case "Value":
                case "NullableValue":
                    {
                        if (variable.InitBy == StiVariableInitBy.Value)
                        {
                            if (variableType == "image")
                            {
                                variableItem["value"] = variable.ValueObject != null ? StiReportEdit.ImageToBase64(variable.ValueObject as Image) : null;
                            }
                            else
                            {
                                var valueString = variable.Value != null ? variable.Value : "";

                                if (variable.ValueObject != null)
                                {
                                    if (variableType == "datetime")
                                    {
                                        valueString = ((DateTime)variable.ValueObject).ToString(en_us_culture);
                                    }
                                    else if (variableType == "datetimeoffset")
                                    {
                                        valueString = ((DateTimeOffset)variable.ValueObject).ToString(en_us_culture);
                                    }
                                }
                                
                                variableItem["value"] = StiEncodingHelper.Encode(valueString);
                            }
                        }
                        else
                            variableItem["expression"] = StiEncodingHelper.Encode(variable.Value);
                        break;
                    }
                case "Range":
                    {
                        if (variable.InitBy == StiVariableInitBy.Value)
                        {
                            object fromObject = ((Range)variable.ValueObject).FromObject;
                            object toObject = ((Range)variable.ValueObject).ToObject;
                            string fromObjectString = string.Empty;
                            string toObjectString = string.Empty;
                            if (fromObject != null) fromObjectString = (variableType == "datetime") ? ((DateTime)fromObject).ToString(en_us_culture) : fromObject.ToString();
                            if (toObject != null) toObjectString = (variableType == "datetime") ? ((DateTime)toObject).ToString(en_us_culture) : toObject.ToString();
                            variableItem["valueFrom"] = StiEncodingHelper.Encode(fromObjectString);
                            variableItem["valueTo"] = StiEncodingHelper.Encode(toObjectString);
                        }
                        else
                        {
                            variableItem["expressionFrom"] = StiEncodingHelper.Encode(variable.InitByExpressionFrom);
                            variableItem["expressionTo"] = StiEncodingHelper.Encode(variable.InitByExpressionTo);
                        }
                        break;
                    }
            }
            #endregion                        

            return variableItem;
        }
        #endregion

        #region Table Item
        private static Hashtable TableItem(DataTable table)
        {
            Hashtable tableItem = new Hashtable();
            tableItem["typeItem"] = "Table";
            tableItem["name"] = table.TableName;
            tableItem["correctName"] = StiNameValidator.CorrectName(table.TableName);
            tableItem["columns"] = GetColumnsTree(table.Columns);
            if (table.ParentRelations != null && table.ParentRelations.Count > 0)
            {
                tableItem["relations"] = GetRelationsTree(table.ParentRelations);
            }

            return tableItem;
        }
        #endregion

        #region Function Item
        private static Hashtable FunctionItem(StiFunction function, StiReport report)
        {
            Hashtable functionItem = new Hashtable();
            functionItem["typeItem"] = "Function";
            functionItem["typeIcon"] = "Function";
            functionItem["name"] = function.FunctionName;
            functionItem["caption"] = function.GetFunctionString(report.ScriptLanguage);
            functionItem["descriptionHeader"] = function.GetLongFunctionString(report.ScriptLanguage);
            functionItem["description"] = function.Description;
            functionItem["returnDescription"] = function.ReturnDescription;

            ArrayList argumentDescriptions = new ArrayList();
            if (function.ArgumentDescriptions != null)
            {
                foreach (string argumentDescription in function.ArgumentDescriptions)
                {
                    argumentDescriptions.Add(argumentDescription);
                }
            }
            functionItem["argumentDescriptions"] = argumentDescriptions;

            ArrayList argumentNames = new ArrayList();
            if (function.ArgumentNames != null)
            {
                foreach (string argumentName in function.ArgumentNames)
                {
                    argumentNames.Add(argumentName);
                }
            }
            functionItem["argumentNames"] = argumentNames;


            functionItem["text"] = string.Format("{0}({1})",
                            function.FunctionName,
                            (function.ArgumentNames == null || function.ArgumentNames.Length == 0) ? string.Empty :
                            new string(',', function.ArgumentNames.Length - 1));

            return functionItem;
        }
        #endregion

        #region Functions Category Item
        private static Hashtable FunctionsCategoryItem(string name, string typeIcon)
        {
            Hashtable categoryItem = new Hashtable();
            categoryItem["typeItem"] = "FunctionsCategory";
            categoryItem["typeIcon"] = typeIcon;
            categoryItem["name"] = name;
            categoryItem["items"] = new ArrayList();

            return categoryItem;
        }
        #endregion

        #region Resource Item
        internal static Hashtable ResourceItem(StiResource resource, StiReport report)
        {
            Hashtable resourceItem = new Hashtable();
            resourceItem["typeItem"] = "Resource";
            resourceItem["type"] = resource.Type;
            resourceItem["typeIcon"] = resource.Content != null ? "Resources.Resource" + resource.Type : "Resources.Resource";
            resourceItem["name"] = resource.Name;
            resourceItem["alias"] = resource.Alias;
            resourceItem["availableInTheViewer"] = resource.AvailableInTheViewer;
            resourceItem["restrictions"] = GetDictionaryRestrictions(resource.Name, StiDataType.Resource, report?.Dictionary);

            if (resource.Type == StiResourceType.Map)
            {
                try
                {
                    string json = StiGZipHelper.ConvertByteArrayToString(resource.Content);

                    var container = new StiMapSvgContainer();
                    JsonConvert.PopulateObject(json, container, StiJsonHelper.DefaultSerializerSettings);
                    resourceItem["mapIcon"] = container.Icon;
                }
                catch { }
            }

            return resourceItem;
        }
        #endregion

        #region Images Gallery Item
        private static Hashtable ImagesGalleryItem(string name, Type type, string source)
        {
            Hashtable galleryItem = new Hashtable();
            galleryItem["name"] = name;
            galleryItem["type"] = type.Name;
            galleryItem["src"] = source;

            return galleryItem;
        }
        #endregion

        #region RichText Gallery Item
        private static Hashtable RichTextGalleryItem(string name, Type type, string imageName)
        {
            Hashtable galleryItem = new Hashtable();
            galleryItem["name"] = name;
            galleryItem["type"] = type.Name;
            galleryItem["imageName"] = imageName;

            return galleryItem;
        }
        #endregion

        #region Helper Methods
        private static Hashtable GetDictionaryRestrictions(string itemName, StiDataType dataType, StiDictionary dictionary)
        {
            if (dictionary != null)
            {
                return new Hashtable()
                {
                    ["isAllowDelete"] = dictionary.Restrictions.IsAllowDelete(itemName, dataType),
                    ["isAllowEdit"] = dictionary.Restrictions.IsAllowEdit(itemName, dataType),
                    ["isAllowMove"] = dictionary.Restrictions.IsAllowMove(itemName, dataType),
                    ["isAllowShow"] = dictionary.Restrictions.IsAllowShow(itemName, dataType)
                };
            }
            return null;
        }

        public static string GetGroupColumnsProperty(StiVirtualSource dataSource)
        {
            ArrayList result = new ArrayList();
            string[] groupColumns = dataSource.GroupColumns;

            if (groupColumns != null)
            {
                for (int i = 0; i < groupColumns.Length; i++)
                {
                    Hashtable oneGroupColumn = new Hashtable();
                    string fullColumnProperty = groupColumns[i];
                    if (fullColumnProperty.StartsWith("DESC"))
                    {
                        oneGroupColumn["direction"] = "DESC";
                        oneGroupColumn["column"] = fullColumnProperty.Replace("DESC", "");
                    }
                    else if (fullColumnProperty.StartsWith("NONE"))
                    {
                        oneGroupColumn["direction"] = "NONE";
                        oneGroupColumn["column"] = fullColumnProperty.Replace("NONE", "");
                    }
                    else
                    {
                        oneGroupColumn["direction"] = "ASC";
                        oneGroupColumn["column"] = fullColumnProperty;
                    }
                    result.Add(oneGroupColumn);
                }
            }

            return result.Count != 0 ? StiEncodingHelper.Encode(JSON.Encode(result)) : string.Empty;
        }

        public static string GetResultsProperty(StiVirtualSource dataSource)
        {
            ArrayList resultArray = new ArrayList();

            string[] results = dataSource.Results;

            if (results != null)
            {
                for (int i = 0; i < results.Length; i += 3)
                {
                    Hashtable oneResult = new Hashtable();
                    oneResult["column"] = results[i];
                    oneResult["aggrFunction"] = results[i + 1];
                    oneResult["name"] = results[i + 2];

                    resultArray.Add(oneResult);
                }
            }

            return resultArray.Count != 0 ? StiEncodingHelper.Encode(JSON.Encode(resultArray)) : string.Empty;
        }

        private static void SetGroupColumnsAndResultsProperty(StiVirtualSource dataSource, object groupsData, object resultsData)
        {
            StiDataSource parentDataSource = dataSource.Dictionary.DataSources[dataSource.NameInSource];

            #region Store calculated columns
            var tempColumns = new StiDataColumnsCollection(dataSource);
            foreach (StiDataColumn tempColumn in dataSource.Columns)
            {
                if (tempColumn is StiCalcDataColumn) tempColumns.Add(tempColumn);
            }
            #endregion

            #region Create collection of columns
            var prevFields = new Hashtable();
            foreach (StiDataColumn column in dataSource.Columns)
            {
                prevFields[column.Name] = column.Type;
            }

            dataSource.Columns.Clear();
            #endregion

            #region Group cloumns
            List<string> groups = new List<string>();

            if (!string.IsNullOrEmpty(groupsData as string))
            {
                ArrayList groupColumnsArray = (ArrayList)JSON.Decode(StiEncodingHelper.DecodeString(groupsData as string));
                for (int i = 0; i < groupColumnsArray.Count; i++)
                {
                    Hashtable groupColumnObject = (Hashtable)groupColumnsArray[i];
                    groups.Add((string)groupColumnObject["direction"] != "ASC"
                        ? (string)groupColumnObject["direction"] + (string)groupColumnObject["column"]
                        : (string)groupColumnObject["column"]);
                }
            }
            dataSource.GroupColumns = groups.ToArray();

            #region Create columns from Groups
            var names = new Hashtable();
            foreach (string group in dataSource.GroupColumns)
            {
                string groupField = group;
                if (groupField.StartsWith("DESC", StringComparison.InvariantCulture))
                {
                    //check for "DESC" word in column name
                    if (parentDataSource != null)
                    {
                        StiDataColumn tempColumn1 = parentDataSource.Columns[groupField];
                        StiDataColumn tempColumn2 = parentDataSource.Columns[groupField.Substring(4)];
                        if (tempColumn1 != null && tempColumn2 == null)
                        {
                            groupField = group;
                        }
                        else
                        {
                            groupField = groupField.Substring(4);
                        }
                    }
                    else
                    {
                        groupField = groupField.Substring(4);
                    }
                }
                else if (groupField.StartsWith("NONE", StringComparison.InvariantCulture)) groupField = groupField.Substring(4);

                var dataColumn = new StiDataColumn(groupField);
                dataColumn.Type = prevFields[dataColumn.Name] as Type;
                if (dataColumn.Type == null)
                {
                    dataColumn.Type = typeof(object);
                    if (parentDataSource != null)
                    {
                        var tempColumn = parentDataSource.Columns[groupField];
                        if (tempColumn != null) dataColumn.Type = tempColumn.Type;
                    }
                }
                dataSource.Columns.Add(dataColumn);
                names[groupField] = groupField;
            }
            #endregion

            #endregion

            #region Results
            List<string> results = new List<string>();
            if (!string.IsNullOrEmpty(resultsData as string))
            {
                ArrayList resultsArray = (ArrayList)JSON.Decode(StiEncodingHelper.DecodeString(resultsData as string));
                foreach (Hashtable result in resultsArray)
                {
                    string column = (string)result["column"];
                    string function = (string)result["aggrFunction"];
                    string name = (string)result["name"];

                    int index2 = 2;
                    if (name == "Count") name += "1";
                    while (names[name] != null)
                    {
                        name = (string)result["name"] + (index2++).ToString();
                    }
                    names[name] = name;

                    results.Add(column);
                    results.Add(function == "No" ? string.Empty : function);
                    results.Add(name);
                }
            }
            dataSource.Results = results.ToArray();

            #region Create columns from results
            int index = 0;
            while (index < dataSource.Results.Length)
            {
                string column = dataSource.Results[index++];
                string function = dataSource.Results[index++];
                string name = dataSource.Results[index++];

                var dataColumn = new StiDataColumn(name);
                if (function == "First" || function == "Last" || function.Length == 0)
                {
                    dataColumn.Type = prevFields[dataColumn.Name] as Type;
                    if (dataColumn.Type == null)
                    {
                        dataColumn.Type = typeof(object);
                        if (parentDataSource != null)
                        {
                            StiDataColumn tempColumn = parentDataSource.Columns[column];
                            if (tempColumn != null) dataColumn.Type = tempColumn.Type;
                        }
                    }
                }
                else if (function == "Count" || function == "CountDistinct")
                {
                    dataColumn.Type = typeof(long);
                }
                else if (function == "MinDate" || function == "MaxDate")
                {
                    dataColumn.Type = typeof(DateTime);
                }
                else if (function == "MinTime" || function == "MaxTime")
                {
                    dataColumn.Type = typeof(TimeSpan);
                }
                else if (function == "MinStr" || function == "MaxStr")
                {
                    dataColumn.Type = typeof(string);
                }
                else
                {
                    dataColumn.Type = prevFields[dataColumn.Name] as Type;
                    if (dataColumn.Type == null) dataColumn.Type = typeof(decimal);
                }

                dataSource.Columns.Add(dataColumn);
            }
            #endregion
            #endregion

            #region Restore calculated columns
            if (tempColumns.Count > 0)
            {
                foreach (StiDataColumn calcColumn in tempColumns)
                {
                    dataSource.Columns.Add(calcColumn);
                }
            }
            #endregion
        }

        private static ArrayList GetDataParameterTypes(StiSqlSource source)
        {
            ArrayList types = new ArrayList();
            var parameterTypes = source.GetParameterTypesEnum();

            if (parameterTypes != null)
            {
                var array = Enum.GetValues(parameterTypes);

                foreach (var type in array)
                {
                    Hashtable typeObject = new Hashtable();
                    typeObject["typeValue"] = (int)type;
                    typeObject["typeName"] = type.ToString();
                    types.Add(typeObject);
                }
            }

            return types;
        }

        private static string GetItemType(StiDialogInfoItem itemVariable)
        {
            if (itemVariable is StiExpressionDialogInfoItem || itemVariable is StiExpressionRangeDialogInfoItem) return "expression";
            return "value";
        }

        private static object GetItemKeyObject(object itemKey, string type, string itemType)
        {
            object keyObject = null;
            if (itemKey != null)
                keyObject = StiEncodingHelper.Encode(type == "datetime" && itemType == "value" ? ((DateTime)itemKey).ToString(en_us_culture) : itemKey.ToString());

            return keyObject;
        }

        private static ArrayList GetItems(StiVariable variable, string type)
        {
            ArrayList items = new ArrayList();
            int index = 0;
            if (variable.DialogInfo.Keys != null && variable.DialogInfo.Keys.Length != 0)
            {
                List<StiDialogInfoItem> itemsVariable = variable.DialogInfo.GetDialogInfoItems(variable.Type);
                foreach (StiDialogInfoItem itemVariable in itemsVariable)
                {
                    Hashtable item = new Hashtable();
                    string itemType = GetItemType(itemVariable);
                    item["type"] = itemType;
                    item["value"] = StiEncodingHelper.Encode(itemVariable.Value);
                    item["key"] = GetItemKeyObject(itemVariable.KeyObject, type, itemType);
                    item["keyTo"] = GetItemKeyObject(itemVariable.KeyObjectTo, type, itemType);

                    items.Add(item);
                    index++;
                }
            }
            return index != 0 ? items : null;
        }

        private static StiDatabase CreateDataBaseByTypeName(string typeDatabase)
        {
            var database = StiOptions.Services.Databases.Where(s => s.ServiceEnabled).FirstOrDefault(d => d.GetType().Name == typeDatabase);
            return database != null ? database.Clone() as StiDatabase : null;
        }

        private static StiDataAdapterService CreateDataAdapterByTypeName(string typeDataAdapter)
        {
            var adapter = StiOptions.Services.DataAdapters.FirstOrDefault(a => typeDataAdapter == a.GetType().Name);
            return adapter != null ? adapter.Clone() as StiDataAdapterService : null;
        }

        private static void CopyProperties(string[] propertyNames, Hashtable fromObject, Hashtable toObject)
        {
            foreach (string propName in propertyNames) toObject[propName] = fromObject[propName];
        }

        public static StiDataColumnsCollection GetColumnsByTypeAndNameOfObject(StiReport report, Hashtable props)
        {
            string currentParentType = (string)props["currentParentType"];
            if (currentParentType == "DataSource")
            {
                var dataSource = report.Dictionary.DataSources[(string)props["currentParentName"]] as StiDataSource;
                if (dataSource != null) return dataSource.Columns;
            }
            else if (currentParentType == "BusinessObject")
            {
                var businessObject = GetBusinessObjectByFullName(report, props["currentParentName"]) as StiBusinessObject;
                return businessObject.Columns;
            }

            return null;
        }

        private static Hashtable GetDatabaseByName(string name, ArrayList databasesTree)
        {
            foreach (Hashtable database in databasesTree)
                if (((string)database["name"]).ToLowerInvariant() == name.ToLowerInvariant()) return database;

            return null;
        }

        private static void UpdateColumns(StiDataColumnsCollection columns, ArrayList columnsSource)
        {
            columns.Clear();
            foreach (Hashtable columnProps in columnsSource)
            {
                StiDataColumn column;
                if ((bool)columnProps["isCalcColumn"])
                    column = new StiCalcDataColumn();
                else
                    column = new StiDataColumn();
                columns.Add(column);
                ApplyColumnProps(column, columnProps);
            }
        }

        private static void UpdateParameters(StiDataParametersCollection parameters, ArrayList parametersSource)
        {
            parameters.Clear();
            foreach (Hashtable parametersProps in parametersSource)
            {
                StiDataParameter parameter = new StiDataParameter();
                parameters.Add(parameter);
                ApplyParameterProps(parameter, parametersProps);
            }
        }

        private static string GetVariableBasicType(StiVariable variable)
        {
            Dictionary.StiTypeMode typeMode;
            StiType.GetTypeModeFromType(variable.Type, out typeMode);

            return typeMode.ToString();
        }

        private static string GetVariableType(StiVariable variable)
        {
            if (variable.Type == typeof(string) || variable.Type == typeof(StringList) || variable.Type == typeof(StringRange)) return "string";
            if (variable.Type == typeof(char) || variable.Type == typeof(char?) || variable.Type == typeof(CharRange) || variable.Type == typeof(CharList)) return "char";
            if (variable.Type == typeof(bool) || variable.Type == typeof(bool?) || variable.Type == typeof(BoolList)) return "bool";
            if (variable.Type == typeof(DateTime) || variable.Type == typeof(DateTime?) || variable.Type == typeof(DateTimeList) || variable.Type == typeof(DateTimeRange)) return "datetime";
            if (variable.Type == typeof(DateTimeOffset) || variable.Type == typeof(DateTimeOffset?)) return "datetimeoffset";
            if (variable.Type == typeof(TimeSpan) || variable.Type == typeof(TimeSpan?) || variable.Type == typeof(TimeSpanList) || variable.Type == typeof(TimeSpanRange)) return "timespan";
            if (variable.Type == typeof(Guid) || variable.Type == typeof(Guid?) || variable.Type == typeof(GuidList) || variable.Type == typeof(GuidRange)) return "guid";
            if (variable.Type == typeof(Image) || variable.Type == typeof(Bitmap)) return "image";
            if (variable.Type == typeof(float) || variable.Type == typeof(float?) || variable.Type == typeof(FloatList) || variable.Type == typeof(FloatRange)) return "float";
            if (variable.Type == typeof(double) || variable.Type == typeof(double?) || variable.Type == typeof(DoubleList) || variable.Type == typeof(DoubleRange)) return "double";
            if (variable.Type == typeof(Decimal) || variable.Type == typeof(Decimal?) || variable.Type == typeof(DecimalList) || variable.Type == typeof(DecimalRange)) return "decimal";
            if (variable.Type == typeof(int) || variable.Type == typeof(int?) || variable.Type == typeof(IntList) || variable.Type == typeof(IntRange)) return "int";
            if (variable.Type == typeof(uint) || variable.Type == typeof(uint?)) return "uint";
            if (variable.Type == typeof(short) || variable.Type == typeof(short?) || variable.Type == typeof(ShortList) || variable.Type == typeof(ShortRange)) return "short";
            if (variable.Type == typeof(ushort) || variable.Type == typeof(ushort?)) return "ushort";
            if (variable.Type == typeof(long) || variable.Type == typeof(long?) || variable.Type == typeof(LongList) || variable.Type == typeof(LongRange)) return "long";
            if (variable.Type == typeof(ulong) || variable.Type == typeof(ulong?)) return "ulong";
            if (variable.Type == typeof(byte) || variable.Type == typeof(byte?) || variable.Type == typeof(ByteList) || variable.Type == typeof(ByteRange)) return "byte";
            if (variable.Type == typeof(byte[])) return "byte[]";
            if (variable.Type == typeof(sbyte) || variable.Type == typeof(sbyte?)) return "sbyte";

            return "object";
        }

        private static object GetValueByType(string stringValue, string typeVariable, string basicType, bool canReturnNull)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (typeVariable == "string") { return stringValue; }
            if (stringValue == "" && basicType == "NullableValue" && canReturnNull) return null;

            if (typeVariable == "float")
            {
                float value = 0;
                float.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                return value;
            }
            if (typeVariable == "double")
            {
                double value = 0;
                double.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                return value;
            }
            if (typeVariable == "decimal")
            {
                decimal value = 0;
                decimal.TryParse(stringValue.Replace(".", ",").Replace(",", decimalSeparator), out value);
                return value;
            }
            if (typeVariable == "int")
            {
                int value = 0;
                int.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "uint")
            {
                uint value = 0;
                uint.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "short")
            {
                short value = 0;
                short.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "ushort")
            {
                ushort value = 0;
                ushort.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "long")
            {
                long value = 0;
                long.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "ulong")
            {
                ulong value = 0;
                ulong.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "byte")
            {
                byte value = 0;
                byte.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "sbyte")
            {
                sbyte value = 0;
                sbyte.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "char")
            {
                char value = ' ';
                char.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "bool")
            {
                bool value = false;
                bool.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "datetime")
            {
                if (stringValue == "" && canReturnNull) return null;
                DateTime value = DateTime.Now;
                DateTime.TryParse(stringValue, en_us_culture, DateTimeStyles.None, out value);
                return value;
            }
            if (typeVariable == "datetimeoffset")
            {
                if (stringValue == "" && canReturnNull) return null;
                DateTimeOffset value = DateTimeOffset.Now;
                DateTimeOffset.TryParse(stringValue, en_us_culture, DateTimeStyles.None, out value);
                return value;
            }
            if (typeVariable == "timespan")
            {
                if (stringValue == "" && canReturnNull) return null;
                TimeSpan value = TimeSpan.Zero;
                TimeSpan.TryParse(stringValue, out value);
                return value;
            }
            if (typeVariable == "guid")
            {
                if (stringValue == "" && basicType == "NullableValue" && canReturnNull) return null;
                Guid variableGuid;
                try
                {
                    variableGuid = new Guid(stringValue);
                }
                catch
                {
                    variableGuid = Guid.Empty;
                }
                return variableGuid;
            }
            if (typeVariable == "object")
            {
                if (stringValue == "" && canReturnNull) return null;
                return (object)stringValue;
            }

            return null;
        }

        private static void SetDialogInfoItems(StiVariable variable, object itemsObject, string type, string basicType)
        {
            if (itemsObject == null)
            {
                variable.DialogInfo.Keys = new string[0];
                variable.DialogInfo.Values = new string[0];
                return;
            }

            ArrayList items = (ArrayList)itemsObject;
            var keys = new string[items.Count];
            var values = new string[items.Count];

            int index = 0;
            foreach (Hashtable item in items)
            {
                object keyObject = null;
                string itemType = (string)item["type"];
                string key = item["key"] != null ? StiEncodingHelper.DecodeString((string)item["key"]) : string.Empty;
                string keyTo = item["keyTo"] != null ? StiEncodingHelper.DecodeString((string)item["keyTo"]) : string.Empty;
                string value = item["value"] != null ? StiEncodingHelper.DecodeString((string)item["value"]) : string.Empty;

                try
                {
                    if (itemType == "expression")
                    {
                        if (basicType != "Range")
                            keyObject = string.Format("{{{0}}}", key);
                        else
                            keyObject = string.Format("{{{0}<<|>>{1}}}", key, keyTo);
                    }
                    else
                    {
                        if (basicType != "Range")
                        {
                            if (type != "datetime")
                            {
                                object obj = GetValueByType(key, type, basicType, false);
                                keyObject = obj != null ? obj.ToString() : "";
                            }
                            else
                                keyObject = key;
                        }
                        else
                        {
                            Range range = (Range)StiActivator.CreateObject(variable.Type);
                            range.FromObject = GetValueByType(key, type, basicType, false);
                            range.ToObject = GetValueByType(keyTo, type, basicType, false);
                            keyObject = RangeConverter.RangeToString(range);
                        }
                    }
                }
                catch
                {
                }

                keys[index] = keyObject == null ? string.Empty : keyObject.ToString();
                values[index] = StiEncodingHelper.DecodeString((string)item["value"]);
                index++;
            }
            variable.DialogInfo.Keys = keys;
            variable.DialogInfo.Values = values;
        }

        public static StiBusinessObject GetBusinessObjectByFullName(StiReport report, object fullName)
        {
            StiBusinessObjectsCollection businessObjectsCollection = report.Dictionary.BusinessObjects;
            StiBusinessObject businessObject = null;
            if (fullName != null && ((ArrayList)fullName).Count != 0)
            {
                ArrayList fullNameArray = (ArrayList)fullName;
                for (int i = fullNameArray.Count - 1; i >= 0; i--)
                {
                    businessObject = businessObjectsCollection[(string)fullNameArray[i]];
                    if (businessObject != null)
                    {
                        businessObjectsCollection = businessObject.BusinessObjects;
                    }
                    else return null;
                }
            }
            return businessObject;
        }

        private static Hashtable GetAjaxDataFromDatabaseInformation(StiDatabaseInformation information)
        {
            Hashtable data = new Hashtable();
            List<DataTable>[] collections = new List<DataTable>[] { information.Tables, information.Views, information.StoredProcedures };
            string[] types = new string[] { "Table", "View", "StoredProcedure" };
            for (var i = 0; i < collections.Length; i++)
            {
                ArrayList tables = new ArrayList();
                foreach (DataTable table in collections[i])
                {
                    Hashtable item = TableItem(table);
                    if (table.ExtendedProperties["Query"] is string) item["query"] = table.ExtendedProperties["Query"];
                    item["typeItem"] = types[i];
                    tables.Add(item);
                }
                if (tables.Count > 0) data[types[i] + "s"] = tables;
            }

            return data;
        }

        public class StiSortDataTableComparer : IComparer<DataTable>
        {
            public int Compare(DataTable x, DataTable y)
            {
                if (x.TableName == null)
                {
                    if (y.TableName == null)
                        return 0;

                    return -1;
                }

                if (y.TableName == null)
                    return 1;

                return string.CompareOrdinal(x.TableName, y.TableName);
            }
        }

        private static StiDatabaseInformation ConvertAjaxDatabaseInfoToDatabaseInfo(ArrayList data, bool allInfo)
        {
            StiDatabaseInformation information = new StiDatabaseInformation();
            StiDatabaseInformation allInformation = new StiDatabaseInformation();
            foreach (Hashtable tableObject in data)
            {
                DataTable dataTable = new DataTable((string)tableObject["name"]);

                var columns = allInfo ? (ArrayList)tableObject["allColumns"] : (ArrayList)tableObject["columns"];
                foreach (Hashtable columnObject in columns)
                {
                    DataColumn dataColumn = new DataColumn((string)columnObject["name"], GetTypeFromString((string)columnObject["type"]));
                    dataTable.Columns.Add(dataColumn);
                    if ((string)tableObject["typeItem"] == "StoredProcedure")
                    {
                        dataColumn.Caption = ((string)columnObject["typeItem"] == "Parameter") ? "Parameters" : "Columns";
                    }
                }

                if (tableObject["query"] != null) dataTable.ExtendedProperties["Query"] = tableObject["query"];

                if ((string)tableObject["typeItem"] == "Table")
                {
                    information.Tables.Add(dataTable);
                    information.Tables.Sort(new StiSortDataTableComparer());
                }
                else if ((string)tableObject["typeItem"] == "View")
                {
                    information.Views.Add(dataTable);
                    information.Views.Sort(new StiSortDataTableComparer());
                }
                if ((string)tableObject["typeItem"] == "StoredProcedure")
                {
                    information.StoredProcedures.Add(dataTable);
                    information.StoredProcedures.Sort(new StiSortDataTableComparer());
                }
            }

            return information;
        }

        private static StiDataStoreSource CreateDataStoreSourceFromParams(StiReport report, Hashtable param)
        {
            StiDataStoreSource dataSource = null;
            StiDataStoreAdapterService adapterDataStore = CreateDataAdapterByTypeName((string)param["typeDataAdapter"]) as StiDataStoreAdapterService;
            if (adapterDataStore != null)
            {
                dataSource = Stimulsoft.Base.StiActivator.CreateObject(adapterDataStore.GetDataSourceType()) as StiDataStoreSource;
                if (dataSource != null)
                {
                    dataSource.Dictionary = report.Dictionary;
                    ApplyDataSourceProperties(dataSource, param, report);
                }
            }

            return dataSource;
        }

        private static void SaveDataSourceParam(ref StiDataStoreSource dataSourceWithParam, StiReport report, StiDataStoreSource dataSource, Hashtable param)
        {
            if (dataSource == null) return;
            ArrayList currColumns = (ArrayList)param["columns"];
            ArrayList currParameters = (ArrayList)param["parameters"];

            if (dataSourceWithParam == null)
            {
                dataSourceWithParam = Stimulsoft.Base.StiActivator.CreateObject(dataSource.GetType()) as StiDataStoreSource;
                dataSourceWithParam.Dictionary = report.Dictionary;
            }

            #region Columns
            var columns = new StiDataColumnsCollection();
            columns.AddRange(dataSourceWithParam.Columns);

            dataSourceWithParam.Columns.Clear();

            for (int i = 0; i < currColumns.Count; i++)
            {
                StiDataColumn column;
                Hashtable columnProps = (Hashtable)currColumns[i];

                if ((bool)columnProps["isCalcColumn"])
                    column = new StiCalcDataColumn();
                else
                    column = new StiDataColumn();
                ApplyColumnProps(column, columnProps);

                var cl = dataSourceWithParam.Columns[(string)columnProps["name"]];
                if (cl == null)
                {
                    dataSourceWithParam.Columns.Add(column);
                }
                else dataSourceWithParam.Columns.Add(cl);
            }
            #endregion

            if (dataSourceWithParam is StiSqlSource)
            {
                #region Parameters
                var parameters = new StiDataParametersCollection();
                parameters.AddRange(dataSourceWithParam.Parameters);
                dataSourceWithParam.Parameters.Clear();

                for (int i = 0; i < currParameters.Count; i++)
                {
                    var parameter = new StiDataParameter();
                    ApplyParameterProps(parameter, (Hashtable)currParameters[i]);

                    var pr = dataSourceWithParam.Parameters[parameter.Name];
                    if (pr == null)
                    {
                        dataSourceWithParam.Parameters.Add(parameter);
                    }
                    else
                    {
                        dataSourceWithParam.Parameters.Add(pr);
                    }
                }
                #endregion

                ((StiSqlSource)dataSourceWithParam).SqlCommand = StiEncodingHelper.DecodeString(param["sqlCommand"] as string);

                if (param["commandTimeout"] != null)
                {
                    ((StiSqlSource)dataSourceWithParam).CommandTimeout = StiReportEdit.StrToInt(param["commandTimeout"] as string);
                }

                if ((string)param["type"] == "Table")
                    ((StiSqlSource)dataSourceWithParam).Type = StiSqlSourceType.Table;
                else
                    ((StiSqlSource)dataSourceWithParam).Type = StiSqlSourceType.StoredProcedure;
            }

            dataSourceWithParam.Name = (string)param["name"];
            dataSourceWithParam.NameInSource = (string)param["nameInSource"];
            dataSourceWithParam.Alias = (string)param["alias"];

            if (dataSourceWithParam.Name == dataSourceWithParam.Alias)
                dataSourceWithParam.Alias = (string)param["name"];
            dataSourceWithParam.Name = (string)param["name"];
        }

        public static Hashtable GetViewDataItemValue(object item, StiDataColumn dictionaryColumn)
        {
            Hashtable resultItem = new Hashtable();
            Type type = dictionaryColumn != null ? dictionaryColumn.Type : item.GetType();

            resultItem["type"] = StiDataFiltersHelper.TypeToString(type);

            if (type == typeof(byte[]))
            {
                resultItem["value"] = StiReportEdit.ImageToBase64((byte[])item);
                resultItem["type"] = "image";
            }
            else if (type == typeof(Image))
            {
                resultItem["value"] = StiReportEdit.ImageToBase64((Image)item);
                resultItem["type"] = "image";
            }
            else
            {
                resultItem["value"] = item?.ToString();
            }

            return resultItem;
        }

        public static StiFileDatabase CreateNewDatabaseFromResource(StiReport report, StiResource resource)
        {
            if (resource == null) return null;

            var databases = StiOptions.Services.Databases;

            if (resource.Type == StiResourceType.Dbf)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiDBaseDatabase))
                    .CreateNew() as StiFileDatabase;

            if (resource.Type == StiResourceType.Csv)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiCsvDatabase))
                    .CreateNew() as StiFileDatabase;

            if (resource.Type == StiResourceType.Xml)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiXmlDatabase))
                    .CreateNew() as StiFileDatabase;

            if (resource.Type == StiResourceType.Json)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiJsonDatabase))
                    .CreateNew() as StiFileDatabase;

            if (resource.Type == StiResourceType.Excel)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiExcelDatabase))
                    .CreateNew() as StiFileDatabase;

            if (resource.Type == StiResourceType.Gis)
                return databases.FirstOrDefault(a => a.GetType() == typeof(StiGisDatabase))
                    .CreateNew() as StiFileDatabase;

            return null;
        }

        public static string GetNewDatabaseName(StiReport report, string fileName)
        {
            string baseName = fileName.Trim();
            var counter = 1;
            while (true)
            {
                var testName = counter == 1 ? baseName : baseName + counter;

                if (!IsExistInDatabases(report, testName))
                    return testName;

                counter++;
            }
        }

        private static bool IsCategoryVariable(StiVariable variable)
        {
            return (string.IsNullOrEmpty(variable.Name) && !string.IsNullOrEmpty(variable.Category));
        }

        private static StiVariable GetVariableCategory(StiReport report, string categoryName)
        {
            foreach (StiVariable variable in report.Dictionary.Variables)
                if (variable.Category == categoryName && variable.Name == string.Empty)
                    return variable;

            return null;
        }

        private static string GetUniqueName(StiReport report, StiDataType type, string baseName, string copyOfText, bool isVariableCategories = false, bool isNameInSource = false)
        {
            var index = 2;
            var name = baseName = $"{baseName}{copyOfText}";

            do
            {
                if (type == StiDataType.Resource && !report.Dictionary.Resources.Contains(name)) break;
                if (type == StiDataType.DataSource && !isNameInSource && !report.Dictionary.DataSources.Contains(name)) break;
                if (type == StiDataType.DataSource && isNameInSource && report.Dictionary.DataSources.ToList().Find(d => d.GetNameInSource() == name) == null) break;
                if (type == StiDataType.DataRelation && !isNameInSource && !report.Dictionary.Relations.Contains(name)) break;
                if (type == StiDataType.DataRelation && isNameInSource && report.Dictionary.Relations.ToList().Find(r => r.NameInSource == name) == null) break;
                if (type == StiDataType.Database && !report.Dictionary.Databases.Contains(name)) break;
                if (type == StiDataType.Variable && !report.Dictionary.Variables.Contains(name) && !isVariableCategories) break;
                if (type == StiDataType.Variable && !report.Dictionary.Variables.ContainsCategory(name) && isVariableCategories) break;

                name = $"{baseName}{index++}";

            }
            while (true || index > 100);

            return name;
        }

        private static string UnPackGraphSqlConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;

            var endPoint = StiConnectionStringHelper.GetConnectionStringKey(connectionString, "EndPoint");
            var query = StiConnectionStringHelper.GetConnectionStringKey(connectionString, "Query");
            var headers = StiConnectionStringHelper.GetConnectionStringKey(connectionString, "Headers");

            if (!string.IsNullOrEmpty(headers))
                headers = StiEncodingHelper.Encode(StiManuallyDataHelper.ConvertPackedStringToJSData(headers));

            return StiEncodingHelper.Encode($"EndPoint={endPoint};Query={query};Headers={headers}");
        }

        private static string PackGraphSqlConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;

            var headersKey = "Headers=";
            var headersIndex = connectionString.IndexOf(headersKey) + headersKey.Length;
            var resultStr = connectionString.Substring(0, headersIndex);
            var headers = connectionString.Substring(headersIndex);

            return $"{resultStr}{StiManuallyDataHelper.ConvertJSDataToPackedString(StiEncodingHelper.DecodeString(headers))}";
        }

        #region Apply Properties
        private static void ApplyParametersToSqlSourse(StiSqlSource sqlSource, Hashtable parameters)
        {
            if (sqlSource == null) return;

            //Report Variables
            foreach (DictionaryEntry parameter in parameters)
            {
                string key = parameter.Key as string;
                if (key.StartsWith("{") && key.EndsWith("}"))
                {
                    sqlSource.SqlCommand = sqlSource.SqlCommand.Replace(key, StiEncodingHelper.DecodeString(parameter.Value as string));
                }
            }

            //Sql Parameters
            foreach (StiDataParameter parameter in sqlSource.Parameters)
            {
                if (parameters[parameter.Name] == null) continue;
                string text = StiEncodingHelper.DecodeString(parameters[parameter.Name] as string);
                string sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                parameter.Value = text;

                Type dbType = sqlSource.ConvertDbTypeToType(parameter.Type);
                if (dbType == typeof(DateTime))
                {
                    DateTime d = new DateTime();
                    DateTime.TryParse(text, out d);
                    parameter.ParameterValue = d;
                    if (text == "")
                        parameter.ParameterValue = DBNull.Value;
                }
                else if (dbType == typeof(bool))
                {
                    Boolean b = false;
                    Boolean.TryParse(text, out b);
                    parameter.ParameterValue = b;
                }
                else if (dbType == typeof(string))
                {
                    parameter.ParameterValue = text;
                }
                else if (dbType == typeof(Int64))
                {
                    Int64 num = 0;
                    Int64.TryParse(text, out num);
                    parameter.ParameterValue = num;
                }
                else if (dbType == typeof(Decimal))
                {
                    Decimal num = 0m;
                    Decimal.TryParse(text.Replace(".", ",").Replace(",", sep), out num);
                    parameter.ParameterValue = num;
                }
                else if (dbType == typeof(Double))
                {
                    Double num = 0d;
                    Double.TryParse(text.Replace(".", ",").Replace(",", sep), out num);
                    parameter.ParameterValue = num;
                }
                else if (dbType == typeof(Guid))
                {
                    Guid g = Guid.Empty;
                    Guid.TryParse(text, out g);
                    parameter.ParameterValue = g;
                }
                else
                {
                    parameter.ParameterValue = StiConvert.ChangeType(text, dbType);
                }
            }
        }

        private static void ApplyDataSourceProperties(StiDataSource dataSource, Hashtable dataSourceProps, StiReport report)
        {
            string[] props = { "name", "nameInSource", "alias", "sqlCommand", "type", "reconnectOnEachRow", "commandTimeout" };
            foreach (string propName in props)
            {
                string propNameUpper = propName[0].ToString().ToUpperInvariant() + propName.Remove(0, 1);
                PropertyInfo property = dataSource.GetType().GetProperty(propNameUpper);
                if (property != null && dataSourceProps[propName] != null)
                {
                    object p;
                    if (propName == "type")
                        p = (StiSqlSourceType)Enum.Parse(typeof(StiSqlSourceType), (string)dataSourceProps[propName]);
                    else if (propName == "sqlCommand")
                        p = StiEncodingHelper.DecodeString((string)dataSourceProps[propName]);
                    else if (propName == "reconnectOnEachRow")
                        p = (bool)dataSourceProps[propName];
                    else if (propName == "commandTimeout")
                        p = StiReportEdit.StrToInt((string)dataSourceProps[propName]);
                    else
                        p = (string)dataSourceProps[propName];
                    property.SetValue(dataSource, p, null);
                }
            }

            if (dataSource is StiVirtualSource)
            {
                ((StiVirtualSource)dataSource).NameInSource = (string)dataSourceProps["nameInSource"];
                StiReportEdit.SetSortDataProperty(dataSource, dataSourceProps["sortData"]);
                StiReportEdit.SetFilterDataProperty(dataSource, dataSourceProps["filterData"]);
                ((StiVirtualSource)dataSource).FilterOn = (bool)dataSourceProps["filterOn"];
                ((StiVirtualSource)dataSource).FilterMode = (string)dataSourceProps["filterMode"] == "And" ? StiFilterMode.And : StiFilterMode.Or;
                SetGroupColumnsAndResultsProperty(dataSource as StiVirtualSource, dataSourceProps["groupsData"], dataSourceProps["resultsData"]);
            }

            if (dataSource is StiCrossTabDataSource)
            {
                var crossTabName = dataSourceProps["nameInSource"] as string;
                if (!String.IsNullOrEmpty(crossTabName))
                {
                    var crossTab = report.Pages.GetComponentByName(crossTabName) as StiCrossTab;
                    if (crossTab != null)
                    {
                        ((StiCrossTabDataSource)dataSource).NameInSource = crossTabName;
                        ((StiCrossTabDataSource)dataSource).BuildColumnsForDictionaryPanel(crossTab);
                    }
                }
            }
        }        

        private static void ApplyConnectionProps(StiDatabase database, Hashtable connectionProps, StiDictionary dictionary)
        {
            string[] props = { "name", "alias", "connectionString", "xmlType", "pathData", "pathSchema", "promptUserNameAndPassword", "codePage",
                "separator", "firstRowIsHeader", "relationDirection", "version", "clientId", "clientSecret", "spreadsheetId", "dataType" };

            foreach (string propName in props)
            {
                string propNameUpper = propName[0].ToString().ToUpperInvariant() + propName.Remove(0, 1);
                PropertyInfo property = database.GetType().GetProperty(propNameUpper);
                if (property != null && connectionProps[propName] != null)
                {
                    object p;
                    if (propName == "connectionString") 
                    {
                        var connectionString = StiEncodingHelper.DecodeString((string)connectionProps[propName]);
                        p = database is StiGraphQLDatabase ? PackGraphSqlConnectionString(connectionString) : connectionString;
                    }
                    else if (propName == "pathData" || propName == "pathSchema" || propName == "clientId" || propName == "clientSecret" || propName == "spreadsheetId")
                        p = StiEncodingHelper.DecodeString((string)connectionProps[propName]);
                    else if (propName == "promptUserNameAndPassword" || propName == "firstRowIsHeader")
                        p = (bool)connectionProps[propName];
                    else if (propName == "xmlType")
                        p = Enum.Parse(typeof(StiXmlType), (string)connectionProps[propName]);
                    else if (propName == "relationDirection")
                        p = Enum.Parse(typeof(StiRelationDirection), (string)connectionProps[propName]);
                    else if (propName == "codePage")
                        p = StiReportEdit.StrToInt((string)connectionProps[propName]);
                    else if (propName == "version")
                        p = Enum.Parse(typeof(StiODataVersion), (string)connectionProps[propName]);
                    else if (propName == "dataType")
                        p = Enum.Parse(typeof(StiGisDataType), (string)connectionProps[propName]);
                    else if (propName == "name")
                    {
                        p = (string)connectionProps[propName];
                        dictionary.RenameDatabase(database, (string)connectionProps[propName]);
                    }
                    else
                        p = (string)connectionProps[propName];

                    property.SetValue(database, p, null);
                }
            }

            if (database is StiFileDatabase fileDatabase) fileDatabase.DataSchema = null;
        }

        private static void ApplyColumnProps(StiDataColumn column, Hashtable columnProps)
        {
            column.Name = (string)columnProps["name"];
            column.NameInSource = (string)columnProps["nameInSource"];
            column.Alias = (string)columnProps["alias"];
            column.Type = GetTypeFromString((string)columnProps["type"]);

            if (column is StiDataTransformationColumn)
                ((StiDataTransformationColumn)column).Expression = StiEncodingHelper.DecodeString((string)columnProps["expression"]);
            else if (column is StiCalcDataColumn)
                ((StiCalcDataColumn)column).Expression = StiEncodingHelper.DecodeString((string)columnProps["expression"]);
        }

        private static void ApplyParameterProps(StiDataParameter parameter, Hashtable parameterProps)
        {
            parameter.Name = (string)parameterProps["name"];
            parameter.Type = StiReportEdit.StrToInt((string)parameterProps["type"]);
            parameter.Size = StiReportEdit.StrToInt((string)parameterProps["size"]);
            parameter.Expression = StiEncodingHelper.DecodeString((string)parameterProps["expression"]);
        }

        private static void ApplyRelationProps(StiReport report, StiDataRelation relation, Hashtable relationProps)
        {
            var isRelationActive = relation.Active;

            relation.NameInSource = (string)relationProps["nameInSource"];
            relation.Active = Convert.ToBoolean(relationProps["active"]);
            relation.ParentSource = report.Dictionary.DataSources[(string)relationProps["parentDataSource"]];
            relation.ChildSource = report.Dictionary.DataSources[(string)relationProps["childDataSource"]];
            relation.ChildColumns = (string[])((ArrayList)relationProps["childColumns"]).ToArray(typeof(string));
            relation.ParentColumns = (string[])((ArrayList)relationProps["parentColumns"]).ToArray(typeof(string));
            relation.Name = StiNameCreation.CreateRelationName(report, relation, (string)relationProps["name"]);
            relation.Alias = relation.Name;

            if (Convert.ToBoolean(relationProps["copyModeActivated"]))
            {
                relation.Active = false;

                #region Create Relation Name
                var relationName = relation.Name;
                if (DataSourceContainDataRelation(relation.ChildSource, relationName))
                {
                    var index2 = 1;
                    while (true)
                    {
                        relationName = index2 == 1
                            ? $"{relation.Name}{StiLocalization.Get("Report", "CopyOf")}"
                            : $"{relation.Name}{StiLocalization.Get("Report", "CopyOf")}{index2}";

                        if (!DataSourceContainDataRelation(relation.ChildSource, relationName))
                            break;

                        index2++;
                    }
                }

                var nameInSource = relation.NameInSource;
                if (DataSourceContainNameInSource(relation.ChildSource, nameInSource))
                {
                    var index2 = 1;
                    while (true)
                    {
                        nameInSource = index2 == 1
                            ? $"{relation.NameInSource}{StiLocalization.Get("Report", "CopyOf")}"
                            : $"{relation.NameInSource}{StiLocalization.Get("Report", "CopyOf")}{index2}";

                        if (!DataSourceContainNameInSource(relation.ChildSource, nameInSource))
                            break;

                        index2++;
                    }
                }

                if (relation.Name == relation.Alias)
                    relation.Alias = relationName;

                relation.Name = relationName;
                relation.NameInSource = nameInSource;
                #endregion
            }

            #region Make Relation Active
            if (!isRelationActive && relation.Active)
            {
                var relations = relation.ChildSource.GetParentRelations().Cast<StiDataRelation>();
                relations.Where(r => r != relation)
                    .ToList()
                    .ForEach(r => r.Active = false);
            }
            #endregion
        }

        private static bool DataSourceContainDataRelation(StiDataSource dataSource, string relationName)
        {
            var relations = dataSource.GetParentRelations();
            foreach (StiDataRelation relation in relations)
            {
                if (relation.Name == relationName)
                    return true;
            }
            return false;
        }

        private static bool DataSourceContainNameInSource(StiDataSource dataSource, string nameInSource)
        {
            var relations = dataSource.GetParentRelations();
            foreach (StiDataRelation relation in relations)
            {
                if (relation.NameInSource == nameInSource)
                    return true;
            }
            return false;
        }

        private static void ApplyBusinessObjectProps(StiBusinessObject businessObject, Hashtable businessObjectProps)
        {
            businessObject.Name = (string)businessObjectProps["name"];
            businessObject.Alias = (string)businessObjectProps["alias"];
            businessObject.Category = (string)businessObjectProps["category"];
        }

        private static void ApplyVariableProps(StiReport report, StiVariable variable, Hashtable variableProps)
        {
            string type = (string)variableProps["type"];
            string basicType = (string)variableProps["basicType"];
            Dictionary.StiTypeMode typeMode = (Dictionary.StiTypeMode)Enum.Parse(typeof(Dictionary.StiTypeMode), (string)variableProps["basicType"]);
            Type variableType = Dictionary.StiType.GetTypeFromTypeMode(GetTypeFromString((string)variableProps["type"]), typeMode);
            variable.Type = variableType;

            variable.Name = (string)variableProps["name"];
            variable.Alias = (string)variableProps["alias"];
            variable.Description = StiEncodingHelper.DecodeString((string)variableProps["description"]);
            variable.Category = (string)variableProps["category"];
            if (variableProps["initBy"] != null) variable.InitBy = (StiVariableInitBy)Enum.Parse(typeof(StiVariableInitBy), (string)variableProps["initBy"]);
            variable.ReadOnly = (bool)variableProps["readOnly"];
            variable.RequestFromUser = (bool)variableProps["requestFromUser"];
            variable.AllowUseAsSqlParameter = (bool)variableProps["allowUseAsSqlParameter"];
            variable.Selection = (StiSelectionMode)Enum.Parse(typeof(StiSelectionMode), (string)variableProps["selection"]);
            variable.DialogInfo.AllowUserValues = (bool)variableProps["allowUserValues"];
            variable.DialogInfo.DateTimeType = (StiDateTimeType)Enum.Parse(typeof(StiDateTimeType), (string)variableProps["dateTimeFormat"]);
            variable.DialogInfo.SortDirection = (StiVariableSortDirection)Enum.Parse(typeof(StiVariableSortDirection), (string)variableProps["sortDirection"]);
            variable.DialogInfo.SortField = (StiVariableSortField)Enum.Parse(typeof(StiVariableSortField), (string)variableProps["sortField"]);
            variable.DialogInfo.ItemsInitializationType = (StiItemsInitializationType)Enum.Parse(typeof(StiItemsInitializationType), (string)variableProps["dataSource"]);
            variable.DialogInfo.Mask = StiEncodingHelper.DecodeString((string)variableProps["formatMask"]);
            variable.DialogInfo.KeysColumn = (string)variableProps["keys"];
            variable.DialogInfo.ValuesColumn = (string)variableProps["values"];
            variable.DialogInfo.CheckedColumn = (string)variableProps["checkedColumn"];
            variable.DialogInfo.CheckedStates = (bool[])((ArrayList)variableProps["checkedStates"]).ToArray(typeof(bool));
            variable.DialogInfo.BindingValue = (bool)variableProps["dependentValue"];
            variable.DialogInfo.BindingVariable = (string)variableProps["dependentVariable"] == "" ? null : report.Dictionary.Variables[(string)variableProps["dependentVariable"]];
            variable.DialogInfo.BindingValuesColumn = (string)variableProps["dependentColumn"];

            #region Set Value
            if (typeMode == StiTypeMode.Value || typeMode == StiTypeMode.NullableValue)
            {
                if (variable.Type == typeof(Image))
                {
                    variable.ValueObject = variableProps["value"] != null ? StiReportEdit.Base64ToImage((string)variableProps["value"]) : null;
                }
                else
                {
                    if (variable.InitBy == StiVariableInitBy.Value && variableProps["value"] != null)
                    {
                        variable.ValueObject = GetValueByType(StiEncodingHelper.DecodeString((string)variableProps["value"]), type, basicType, true);
                        if (variableProps["value"] as string == String.Empty) variable.Value = variableProps["value"] as string;
                    }
                    if (variable.InitBy == StiVariableInitBy.Expression && variableProps["expression"] != null)
                    {
                        string expression = StiEncodingHelper.DecodeString((string)variableProps["expression"]);
                        if (!String.IsNullOrEmpty(expression) && expression.StartsWith("{") && expression.EndsWith("}"))
                            expression = expression.Substring(1, expression.Length - 2);
                        variable.ValueObject = expression;
                    }
                }
            }
            else if (typeMode == StiTypeMode.Range)
            {
                if (variable.InitBy == StiVariableInitBy.Value && StiTypeFinder.FindType(variableType, typeof(Range)))
                {
                    Range range = Activator.CreateInstance(variableType) as Range;
                    if (variableProps["valueFrom"] != null) range.FromObject = GetValueByType(StiEncodingHelper.DecodeString((string)variableProps["valueFrom"]), type, basicType, true);
                    if (variableProps["valueTo"] != null) range.ToObject = GetValueByType(StiEncodingHelper.DecodeString((string)variableProps["valueTo"]), type, basicType, true);
                    variable.ValueObject = range;
                }
                if (variable.InitBy == StiVariableInitBy.Expression)
                {
                    string expressionFrom = StiEncodingHelper.DecodeString((string)variableProps["expressionFrom"]);
                    if (!String.IsNullOrEmpty(expressionFrom) && expressionFrom.StartsWith("{") && expressionFrom.EndsWith("}"))
                        expressionFrom = expressionFrom.Substring(1, expressionFrom.Length - 2);
                    string expressionTo = StiEncodingHelper.DecodeString((string)variableProps["expressionTo"]);
                    if (!String.IsNullOrEmpty(expressionTo) && expressionTo.StartsWith("{") && expressionTo.EndsWith("}"))
                        expressionTo = expressionTo.Substring(1, expressionTo.Length - 2);

                    if (variableProps["expressionFrom"] != null) variable.InitByExpressionFrom = expressionFrom;
                    if (variableProps["expressionTo"] != null) variable.InitByExpressionTo = expressionTo;
                }
            }
            #endregion

            SetDialogInfoItems(variable, variableProps["items"], type, basicType);
        }

        private static void ApplyResourceProps(StiReport report, StiResource resource, Hashtable resourceProps)
        {
            resource.Name = (string)resourceProps["name"];
            resource.Alias = (string)resourceProps["alias"];
            resource.AvailableInTheViewer = Convert.ToBoolean(resourceProps["availableInTheViewer"]);

            if (resourceProps["type"] != null)
            {
                resource.Type = (StiResourceType)Enum.Parse(typeof(StiResourceType), (string)resourceProps["type"]);

                if ((bool)resourceProps["haveContent"])
                {
                    if (resourceProps["loadedContent"] != null)
                    {
                        string contentBase64Str = resourceProps["loadedContent"] as string;
                        resource.Content = Convert.FromBase64String(contentBase64Str.Substring(contentBase64Str.IndexOf("base64,") + 7));
                    }
                }
                else
                {
                    resource.Content = null;
                }
            }
        }
        #endregion

        #region Get Column Icon Type
        public static StiImagesID GetIconTypeForColumn(StiDataColumn column)
        {
            bool inherited = (column.DataSource != null)
                ? column.DataSource.Inherited
                : ((column.BusinessObject != null)
                    ? column.BusinessObject.Inherited
                    : false);

            if (column is StiCalcDataColumn)
                return GetLockedCalcImageIDFromType(column.Type, inherited);
            else
                return GetDataColumnImageIdFromType(column.Type, !inherited);
        }

        public static StiImagesID GetLockedCalcImageIDFromType(Type type, bool inherited)
        {
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnBool :
                    StiImagesID.CalcColumnBool;
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnChar :
                    StiImagesID.CalcColumnChar;
            }

            if (type == typeof(DateTime) ||
                type == typeof(TimeSpan) ||
                type == typeof(DateTime?) ||
                type == typeof(TimeSpan?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnDateTime :
                    StiImagesID.CalcColumnDateTime;
            }

            if (type == typeof(Decimal) ||
                type == typeof(Decimal?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnDecimal :
                    StiImagesID.CalcColumnDecimal;
            }

            if (type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int?) ||
                type == typeof(uint?) ||
                type == typeof(long?) ||
                type == typeof(ulong?) ||
                type == typeof(byte?) ||
                type == typeof(sbyte?) ||
                type == typeof(short?) ||
                type == typeof(ushort?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnInt :
                    StiImagesID.CalcColumnInt;
            }

            if (type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(double?))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnFloat :
                    StiImagesID.CalcColumnFloat;
            }

            if (type == typeof(Image))
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnImage :
                    StiImagesID.CalcColumnImage;
            }

            if (type != null && type.IsArray)
            {
                return inherited ?
                    StiImagesID.LockedCalcColumnBinary :
                    StiImagesID.CalcColumnBinary;
            }

            return inherited ?
                StiImagesID.LockedCalcColumnString :
                StiImagesID.CalcColumnString;
        }

        private static StiImagesID GetDataColumnImageIdFromType(Type type, bool isDataColumn)
        {
            #region Simple Types
            if (type == typeof(bool) || type == typeof(bool?))
                return isDataColumn ? StiImagesID.DataColumnBool : StiImagesID.LockedDataColumnBool;

            if (type == typeof(char) || type == typeof(char?))
                return isDataColumn ? StiImagesID.DataColumnChar : StiImagesID.LockedDataColumnChar;

            if (type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(DateTime?) ||
                type == typeof(DateTimeOffset?) ||
                type == typeof(TimeSpan?))
                return isDataColumn ? StiImagesID.DataColumnDateTime : StiImagesID.LockedDataColumnDateTime;

            if (type == typeof(Decimal) ||
                type == typeof(Decimal?))
                return isDataColumn ? StiImagesID.DataColumnDecimal : StiImagesID.LockedDataColumnDecimal;

            if (type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int?) ||
                type == typeof(uint?) ||
                type == typeof(long?) ||
                type == typeof(ulong?) ||
                type == typeof(byte?) ||
                type == typeof(sbyte?) ||
                type == typeof(short?) ||
                type == typeof(ushort?))
                return isDataColumn ? StiImagesID.DataColumnInt : StiImagesID.LockedDataColumnInt;

            if (type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(float?) ||
                type == typeof(double?))
                return isDataColumn ? StiImagesID.DataColumnFloat : StiImagesID.LockedDataColumnFloat;

            if (type == typeof(Image) || type == typeof(Bitmap))
                return isDataColumn ? StiImagesID.DataColumnImage : StiImagesID.LockedDataColumnImage;
            #endregion

            #region Range Types
            if (type == typeof(CharRange))
                return isDataColumn ? StiImagesID.VariableRangeChar : StiImagesID.LockedVariableRangeChar;

            if (type == typeof(DateTimeRange) ||
                type == typeof(TimeSpanRange))
                return isDataColumn ? StiImagesID.VariableRangeDateTime : StiImagesID.LockedVariableRangeDateTime;

            if (type == typeof(DecimalRange))
                return isDataColumn ? StiImagesID.VariableRangeDecimal : StiImagesID.LockedVariableRangeDecimal;

            if (type == typeof(IntRange) ||
                type == typeof(LongRange) ||
                type == typeof(ByteRange) ||
                type == typeof(ShortRange))
                return isDataColumn ? StiImagesID.VariableRangeInt : StiImagesID.LockedVariableRangeInt;

            if (type == typeof(FloatRange) ||
                type == typeof(DoubleRange))
                return isDataColumn ? StiImagesID.VariableRangeFloat : StiImagesID.LockedVariableRangeFloat;

            if (type == typeof(StringRange) ||
                type == typeof(GuidRange))
                return isDataColumn ? StiImagesID.VariableRangeString : StiImagesID.LockedVariableRangeString;
            #endregion

            #region List Types
            if (type == typeof(BoolList))
                return isDataColumn ? StiImagesID.VariableListBool : StiImagesID.LockedVariableListBool;

            if (type == typeof(StringList) ||
                type == typeof(GuidList))
                return isDataColumn ? StiImagesID.VariableListString : StiImagesID.LockedVariableListString;

            if (type == typeof(CharList))
                return isDataColumn ? StiImagesID.VariableListChar : StiImagesID.LockedVariableListChar;

            if (type == typeof(DateTimeList) ||
                type == typeof(TimeSpanList))
                return isDataColumn ? StiImagesID.VariableListDateTime : StiImagesID.LockedVariableListDateTime;

            if (type == typeof(DecimalList))
                return isDataColumn ? StiImagesID.VariableListDecimal : StiImagesID.LockedVariableListDecimal;

            if (type == typeof(IntList) ||
                type == typeof(LongList) ||
                type == typeof(ByteList) ||
                type == typeof(ShortList))
                return isDataColumn ? StiImagesID.VariableListInt : StiImagesID.LockedVariableListInt;

            if (type == typeof(FloatList) ||
                type == typeof(DoubleList))
                return isDataColumn ? StiImagesID.VariableListFloat : StiImagesID.LockedVariableListFloat;
            #endregion

            if (type != null && type.IsArray)
                return isDataColumn ? StiImagesID.DataColumnBinary : StiImagesID.LockedDataColumnBinary;

            return isDataColumn ? StiImagesID.DataColumnString : StiImagesID.LockedDataColumnString;
        }
        #endregion

        #region String To Type
        public static Type GetTypeFromString(string type)
        {
            if (type == "null") return null;
            if (type == "bool") return typeof(bool);
            if (type == "byte") return typeof(byte);
            if (type == "byte[]") return typeof(byte[]);
            if (type == "char") return typeof(char);
            if (type == "datetime") return typeof(DateTime);
            if (type == "datetimeoffset") return typeof(DateTimeOffset);
            if (type == "decimal") return typeof(decimal);
            if (type == "double") return typeof(double);
            if (type == "guid") return typeof(Guid);
            if (type == "short") return typeof(short);
            if (type == "int") return typeof(int);
            if (type == "long") return typeof(long);
            if (type == "sbyte") return typeof(sbyte);
            if (type == "float") return typeof(float);
            if (type == "string") return typeof(string);
            if (type == "timespan") return typeof(TimeSpan);
            if (type == "ushort") return typeof(ushort);
            if (type == "uint") return typeof(uint);
            if (type == "ulong") return typeof(ulong);
            if (type == "image") return typeof(Image);

            if (type == "bool (Nullable)") return typeof(bool?);
            if (type == "byte (Nullable)") return typeof(byte?);
            if (type == "char (Nullable)") return typeof(char?);
            if (type == "datetime (Nullable)") return typeof(DateTime?);
            if (type == "decimal (Nullable)") return typeof(decimal?);
            if (type == "double (Nullable)") return typeof(double?);
            if (type == "guid (Nullable)") return typeof(Guid?);
            if (type == "short (Nullable)") return typeof(short?);
            if (type == "int (Nullable)") return typeof(int?);
            if (type == "long (Nullable)") return typeof(long?);
            if (type == "sbyte (Nullable)") return typeof(sbyte?);
            if (type == "float (Nullable)") return typeof(float?);
            if (type == "timespan (Nullable)") return typeof(TimeSpan?);
            if (type == "ushort (Nullable)") return typeof(ushort?);
            if (type == "uint (Nullable)") return typeof(uint?);
            if (type == "ulong (Nullable)") return typeof(ulong?);

            if (type == "object") return typeof(object);

            return typeof(object);
        }
        #endregion
        #endregion

        #region Build Tree
        public static Hashtable GetDictionaryTree(StiReport report)
        {
            Hashtable dictionary = new Hashtable();
            dictionary["databases"] = GetDataBasesTree(report);
            dictionary["businessObjects"] = GetBusinessObjectsTree(report);
            dictionary["variables"] = GetVariablesTree(report);
            dictionary["systemVariables"] = GetSystemVariablesTree(report);
            dictionary["functions"] = GetFunctionsTree(report);
            dictionary["resources"] = GetResourcesTree(report);

            return dictionary;
        }

        public static ArrayList GetResourcesTree(StiReport report)
        {
            ArrayList resourcesTree = new ArrayList();

            foreach (StiResource resource in report.Dictionary.Resources)
            {
                var resourceItem = ResourceItem(resource, report);

                if (StiReportResourceHelper.IsFontResourceType(resource.Type))
                {
                    StiFontResourceHelper.AddFontToReport(report, resource, resourceItem);
                }

                resourcesTree.Add(resourceItem);
            }

            return resourcesTree;
        }

        public static ArrayList GetFunctionsTree(StiReport report)
        {
            var dictionary = report.Dictionary;
            ArrayList functionsTree = new ArrayList();

            var hash = StiFunctions.GetFunctionsGrouppedInCategories();
            var categories = new string[hash.Keys.Count];
            hash.Keys.CopyTo(categories, 0);

            foreach (string category in categories)
            {
                var categoryItem = FunctionsCategoryItem(category, "Folder");
                functionsTree.Add(categoryItem);

                #region Sort Functions
                var list = hash[category] as List<StiFunction>;

                var hashFunctions = new Hashtable();
                foreach (StiFunction function in list)
                {
                    hashFunctions[function.GroupFunctionName] = function.GroupFunctionName;
                }

                var functions = new string[hashFunctions.Count];
                hashFunctions.Keys.CopyTo(functions, 0);
                Array.Sort(functions);
                #endregion

                foreach (string function in functions)
                {
                    var funcs = StiFunctions.GetFunctions(dictionary.Report, function, false);
                    Array.Sort(funcs);

                    var parentNode = categoryItem;
                    if (funcs.Length > 1)
                    {
                        parentNode = FunctionsCategoryItem(function, "Function");
                        ArrayList items = (ArrayList)categoryItem["items"];
                        items.Add(parentNode);
                    }

                    foreach (var func in funcs)
                    {
                        var functionItem = FunctionItem(func, report);
                        ArrayList items = (ArrayList)parentNode["items"];
                        items.Add(functionItem);
                    }
                }
            }

            return functionsTree;
        }

        public static ArrayList GetSystemVariablesTree(StiReport report)
        {
            ArrayList systemVariables = new ArrayList();
            systemVariables.AddRange(StiSystemVariablesHelper.GetSystemVariables(report));

            return systemVariables;
        }

        private static ArrayList GetDataBasesTree(StiReport report)
        {
            StiDictionary dictionary = report.Dictionary;
            ArrayList databasesTree = new ArrayList();

            #region Add Databases
            foreach (StiDatabase database in dictionary.Databases)
            {
                if (dictionary.Restrictions.IsAllowShow(database.Name, StiDataType.Database))
                    databasesTree.Add(DatabaseItem(database, report));
            }
            #endregion

            #region Add DataSources
            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (dictionary.Restrictions.IsAllowShow(dataSource.Name, StiDataType.DataSource))
                {
                    string category = dataSource.GetCategoryName();
                    Hashtable databaseItem = GetDatabaseByName(category, databasesTree);
                    if (databaseItem == null)
                    {
                        databaseItem = dataSource is StiDataTransformation
                            ? DataTransformationCategoryItem(category, category, category)
                            : DatabaseItem(category, category, category, dataSource is StiCrossTabDataSource);

                        if (!(dataSource is StiDataTransformation))
                            databaseItem["isDataStore"] = true;

                        databaseItem["isCloud"] = dataSource.IsCloud;
                        databasesTree.Add(databaseItem);
                    }
                    ((ArrayList)databaseItem["dataSources"]).Add(dataSource is StiDataTransformation
                        ? DataTransformationItem(dataSource as StiDataTransformation)
                        : DatasourceItem(dataSource));
                }
            }
            #endregion                        

            return databasesTree;
        }

        private static bool IsExistInDatabases(StiReport report, string databaseName)
        {
            StiDictionary dictionary = report.Dictionary;

            foreach (StiDatabase database in dictionary.Databases)
            {
                if (dictionary.Restrictions.IsAllowShow(database.Name, StiDataType.Database) && database.Name.ToLowerInvariant() == databaseName.ToLowerInvariant())
                {
                    return true;
                }
            }

            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (dictionary.Restrictions.IsAllowShow(dataSource.Name, Stimulsoft.Report.Dictionary.StiDataType.DataSource))
                {
                    string category = dataSource.GetCategoryName();
                    if (dictionary.Databases[category] == null && category.ToLowerInvariant() == databaseName.ToLowerInvariant())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static ArrayList GetObjectsTreeByCategories(StiReport report, CollectionBase collectionObjects)
        {
            StiDictionary dictionary = report.Dictionary;
            ArrayList mainTree = new ArrayList();
            Hashtable categories = new Hashtable();

            bool isVariablesCollection = collectionObjects is StiVariablesCollection;
            ArrayList objects;
            StiVariable variable = null;
            StiBusinessObject businessObject = null;

            foreach (object object_ in collectionObjects)
            {
                if (isVariablesCollection) variable = (StiVariable)object_;
                else businessObject = (StiBusinessObject)object_;
                
                var  name = isVariablesCollection ? variable.Name : businessObject.Name;
                var  category = isVariablesCollection ? variable.Category : businessObject.Category;
                
                if (!dictionary.Restrictions.IsAllowShow(name, StiDataType.Variable)) continue;

                if (category == "")
                    objects = mainTree;
                else
                {
                    if (categories[category] == null)
                    {
                        objects = new ArrayList();
                        categories[category] = objects;

                        Hashtable categoryObject = new Hashtable();
                        categoryObject["typeItem"] = "Category";
                        categoryObject["name"] = category;
                        categoryObject["categoryItems"] = objects;

                        if (isVariablesCollection) 
                        {
                            categoryObject["requestFromUser"] = variable.RequestFromUser;
                            categoryObject["readOnly"] = variable.ReadOnly;
                        }

                        mainTree.Add(categoryObject);
                    }
                    else
                        objects = (ArrayList)categories[category];
                }
                if (!isVariablesCollection)
                    objects.Add(BusinessObjectItem(businessObject));
                else
                    if (variable.Name.Length > 0) objects.Add(VariableItem(variable, report));
            }

            return mainTree;
        }

        private static ArrayList GetBusinessObjectsTree(StiReport report)
        {
            return GetObjectsTreeByCategories(report, report.Dictionary.BusinessObjects);
        }

        private static ArrayList GetChildBusinessObjectsTree(StiBusinessObject businessObject)
        {
            ArrayList businessObjets = new ArrayList();
            if (businessObject.BusinessObjects != null)
            {
                foreach (StiBusinessObject childBusinessObject in businessObject.BusinessObjects)
                    businessObjets.Add(BusinessObjectItem(childBusinessObject));
            }
            return businessObjets;
        }

        private static ArrayList GetVariablesTree(StiReport report)
        {
            return GetObjectsTreeByCategories(report, report.Dictionary.Variables);
        }

        private static ArrayList GetColumnsTree(DataColumnCollection columnsCollection)
        {
            ArrayList columns = new ArrayList();
            foreach (DataColumn column in columnsCollection)
            {
                Hashtable columnItem = new Hashtable();
                columnItem["typeItem"] = column.Caption == "Parameters" ? "Parameter" : "Column";
                columnItem["typeIcon"] = column.Caption == "Parameters" ? "Parameter" : GetDataColumnImageIdFromType(column.DataType, true).ToString();
                columnItem["type"] = StiDataFiltersHelper.TypeToString(column.DataType);
                columnItem["name"] = column.ColumnName;
                if (column.Caption == "Parameters") columnItem["expression"] = column.Expression;

                columns.Add(columnItem);
            }

            return columns;
        }

        private static ArrayList GetColumnsTree(StiDataColumnsCollection columnsCollection, bool isCloud)
        {
            ArrayList columns = new ArrayList();
            foreach (StiDataColumn column in columnsCollection)
            {
                Hashtable columnObject = ColumnItem(column);
                columnObject["isCloud"] = isCloud;
                columns.Add(columnObject);
            }

            return columns;
        }

        private static ArrayList GetColumnsTree(StiDataColumnsCollection columnsCollection)
        {
            ArrayList columns = new ArrayList();
            foreach (StiDataColumn column in columnsCollection)
                columns.Add(ColumnItem(column));

            return columns;
        }

        private static ArrayList GetParametersTree(StiDataParametersCollection parametersCollection, bool isCloud)
        {
            ArrayList parameters = new ArrayList();
            foreach (StiDataParameter parameter in parametersCollection)
            {
                Hashtable parameterObject = ParameterItem(parameter);
                parameterObject["isCloud"] = isCloud;
                parameters.Add(parameterObject);
            }

            return parameters;
        }

        private static ArrayList GetRelationsTree(StiDataRelation parentRelation, StiDataRelationsCollection relations, bool isCloud, Hashtable upLevelRelations, int level = 1)
        {
            ArrayList relationsTree = new ArrayList();

            foreach (StiDataRelation relation in relations)
            {
                if (parentRelation != relation)
                {
                    if (upLevelRelations[relation.NameInSource] != null) return new ArrayList();
                    Hashtable relationObject = RelationItem(relation, upLevelRelations, level);
                    relationObject["isCloud"] = isCloud;
                    relationsTree.Add(relationObject);
                }
            }

            return relationsTree;
        }

        private static ArrayList GetRelationsTree(StiDataRelation parentRelation, StiDataRelationsCollection relations, Hashtable upLevelRelations, int level = 1)
        {
            ArrayList relationsTree = new ArrayList();

            foreach (StiDataRelation relation in relations)
            {
                if (parentRelation != relation)
                    relationsTree.Add(RelationItem(relation, upLevelRelations, level));
            }

            return relationsTree;
        }

        private static ArrayList GetRelationsTree(DataRelationCollection relationsCollection)
        {
            ArrayList relations = new ArrayList();
            foreach (DataRelation relation in relationsCollection)
            {
                Hashtable relationItem = new Hashtable();
                relationItem["typeItem"] = "Relation";
                relationItem["typeIcon"] = "Relation";
                relationItem["name"] = relation.RelationName;
                relationItem["correctName"] = StiNameValidator.CorrectName(relation.ParentTable.TableName);
                relationItem["nameInSource"] = relation.RelationName;
                relationItem["alias"] = relation.ParentTable.TableName;
                relationItem["parentDataSource"] = relation.ParentTable.TableName;
                relationItem["childDataSource"] = relation.ChildTable.TableName;
                ArrayList jsParentColumns = new ArrayList();
                foreach (DataColumn parentColumn in relation.ParentColumns) jsParentColumns.Add(parentColumn.ColumnName);
                relationItem["parentColumns"] = jsParentColumns;
                ArrayList jsChildColumns = new ArrayList();
                foreach (DataColumn childColumn in relation.ChildColumns) jsChildColumns.Add(childColumn.ColumnName);
                relationItem["childColumns"] = jsChildColumns;

                relations.Add(relationItem);
            }

            return relations;
        }

        #endregion

        #region Callback Methods
        public static void GetConnectionTypes(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable connections = new Hashtable();

            #region Add Report Connections

            var listCreatedConnection = new List<StiDatabase>();
            foreach (StiDatabase database in report.Dictionary.Databases)
            {
                if (!report.Dictionary.Restrictions.IsAllowShow(database.Name, StiDataType.Database)) continue;
                listCreatedConnection.Add(database);
            }

            if (listCreatedConnection.Count > 0)
            {
                connections["ReportConnections"] = new ArrayList();
                var sortedList = listCreatedConnection.OrderBy(x => x.Name);

                foreach (var database in sortedList)
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.Name;
                    properties["typeConnection"] = database.GetType().Name;

                    if (database is StiSqlDatabase)
                        properties["dataAdapterType"] = ((StiSqlDatabase)database).GetDataAdapter()?.GetType().Name;
                    else if (database is StiNoSqlDatabase)
                        properties["dataAdapterType"] = ((StiNoSqlDatabase)database).GetDataAdapter()?.GetType().Name;

                    ((ArrayList)connections["ReportConnections"]).Add(properties);
                }
            }

            #endregion

            #region Get Databases Groups

            #region Add Databases To Groups

            var databases = new Dictionary<StiConnectionType, List<StiDatabase>>();
            foreach (StiDatabase data in StiOptions.Services.Databases.Where(x => (x.ServiceEnabled)))
            {
                if (data is StiUndefinedDatabase) continue;

                List<StiDatabase> list = null;
                if (databases.ContainsKey(data.ConnectionType))
                {
                    list = databases[data.ConnectionType];
                }
                else
                {
                    list = new List<StiDatabase>();
                    databases.Add(data.ConnectionType, list);
                }

                list.Add(data);
            }

            #endregion

            #region SQL
            if (databases.ContainsKey(StiConnectionType.Sql))
            {
                var list = databases[StiConnectionType.Sql].OrderBy(x => x.ConnectionOrder);
                connections["SQL"] = new ArrayList();

                foreach (var database in list)
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["SQL"]).Add(properties);
                }
            }
            #endregion

            #region NoSQL
            if (databases.ContainsKey(StiConnectionType.NoSql))
            {
                var list = databases[StiConnectionType.NoSql].OrderBy(x => x.ConnectionOrder);
                connections["NoSQL"] = new ArrayList();

                foreach (var database in list)
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["NoSQL"]).Add(properties);
                }
            }
            #endregion

            #region Azure
            if (databases.ContainsKey(StiConnectionType.Azure))
            {
                var list = databases[StiConnectionType.Azure].OrderBy(x => x.ConnectionOrder);
                connections["Azure"] = new ArrayList();

                foreach (var database in list.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["Azure"]).Add(properties);
                }
            }
            #endregion

            #region Google
            if (databases.ContainsKey(StiConnectionType.Google))
            {
                var list = databases[StiConnectionType.Google].OrderBy(x => x.ConnectionOrder);
                connections["Google"] = new ArrayList();

                foreach (var database in list.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["Google"]).Add(properties);
                }
            }
            #endregion

            #region Online Services
            if (databases.ContainsKey(StiConnectionType.OnlineServices))
            {
                var list = databases[StiConnectionType.OnlineServices].OrderBy(x => x.ConnectionOrder);
                connections["OnlineServices"] = new ArrayList();

                foreach (var database in list.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["OnlineServices"]).Add(properties);
                }
            }
            #endregion

            #region Rest
            if (databases.ContainsKey(StiConnectionType.Rest))
            {
                var list = databases[StiConnectionType.Rest].OrderBy(x => x.ConnectionOrder);
                connections["REST"] = new ArrayList();

                foreach (var database in list.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["REST"]).Add(properties);
                }
            }
            #endregion

            #region Files

            if (databases.ContainsKey(StiConnectionType.Other))
            {
                var list = databases[StiConnectionType.Other].OrderBy(x => x.ConnectionOrder);

                connections["Files"] = new ArrayList();
                foreach (var database in list.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = database.ServiceName;
                    properties["typeConnection"] = database.GetType().Name;
                    ((ArrayList)connections["Files"]).Add(properties);
                }
            }

            #endregion

            #region Objects

            var adapters = StiOptions.Services.DataAdapters.Where(x => (x.ServiceEnabled && x.IsObjectAdapter));
            if (adapters.Count() > 0)
            {
                connections["Objects"] = new ArrayList();
                foreach (var adapter in adapters.OrderBy(x => x.ServiceName))
                {
                    Hashtable properties = new Hashtable();
                    properties["name"] = adapter.ServiceName;
                    properties["typeConnection"] = adapter.GetType().Name;
                    ((ArrayList)connections["Objects"]).Add(properties);
                }
            }

            #endregion

            #endregion

            callbackResult["connections"] = connections;
        }

        public static void CreateOrEditConnection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable connectionProps = (Hashtable)param["connectionFormResult"];
            StiDatabase database = null;

            if ((string)connectionProps["mode"] == "Edit")
                database = report.Dictionary.Databases[(string)connectionProps["oldName"]];
            else
            {
                string typeConnection = (string)connectionProps["typeConnection"];
                database = CreateDataBaseByTypeName(typeConnection);
                if (database != null) report.Dictionary.Databases.Add(database);
            }

            if (database != null)
            {
                ApplyConnectionProps(database, connectionProps, report.Dictionary);
                callbackResult["itemObject"] = DatabaseItem(database, report);
            }
            callbackResult["mode"] = connectionProps["mode"];
            callbackResult["skipSchemaWizard"] = connectionProps["skipSchemaWizard"];
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void DeleteConnection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDatabase database = report.Dictionary.Databases[(string)param["connectionName"]];
            bool canDelete = database != null && report.Dictionary.Databases.Contains(database);
            if (canDelete) report.Dictionary.Databases.Remove(database);

            Hashtable dataSourceNames = (Hashtable)param["dataSourceNames"];
            foreach (DictionaryEntry dataSourceName in dataSourceNames)
            {
                StiDataSource dataSource = report.Dictionary.DataSources[(string)dataSourceName.Value];
                if (dataSource != null)
                {
                    try
                    {
                        report.Dictionary.DataSources.Remove(dataSource);
                    }
                    catch (Exception e)
                    {
                        callbackResult["error"] = e.Message;
                    }
                }
            }

            callbackResult["deleteResult"] = canDelete;
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void CreateOrEditRelation(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable relationProps = (Hashtable)param["relationFormResult"];
            StiDataRelation relation = null;

            if ((string)relationProps["mode"] == "Edit" && !Convert.ToBoolean(relationProps["copyModeActivated"]))
                relation = report.Dictionary.Relations[(string)relationProps["oldNameInSource"]];
            else
            {
                relation = new StiDataRelation();
                report.Dictionary.Relations.Add(relation);
            }

            if (relation != null)
            {
                ApplyRelationProps(report, relation, relationProps);
                callbackResult["itemObject"] = RelationItem(relation, new Hashtable());
            }

            CopyProperties(new string[] { "mode", "oldNameInSource", "changedChildDataSource", "copyModeActivated" }, relationProps, callbackResult);
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void DeleteRelation(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDataRelation relation = report.Dictionary.Relations[(string)param["relationNameInSource"]];
            bool canDelete = relation != null && report.Dictionary.Relations.Contains(relation);
            if (canDelete) report.Dictionary.Relations.Remove(relation);

            callbackResult["deleteResult"] = canDelete;
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void CreateOrEditColumn(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable columnProps = (Hashtable)param["columnFormResult"];
            StiDataColumn column = null;
            StiDataColumnsCollection columns = GetColumnsByTypeAndNameOfObject(report, columnProps);

            if (columns != null)
            {
                if ((string)columnProps["mode"] == "Edit")
                    column = columns[(string)columnProps["oldName"]];
                else
                {
                    if (Convert.ToBoolean(columnProps["isCalcColumn"]))
                        column = new StiCalcDataColumn();
                    else
                        column = new StiDataColumn();

                    if (Convert.ToBoolean(columnProps["isDataTransformationColumn"]))
                    {
                        ApplyColumnProps(column, columnProps);
                        column.DataSource = columns.DataSource;
                        column = StiDataTransformationHelper.CreateTransformationColumnFromDataColumn(column);
                    }

                    columns.Add(column);
                }
            }

            if (column != null)
            {
                ApplyColumnProps(column, columnProps);
                callbackResult["itemObject"] = Convert.ToBoolean(columnProps["isDataTransformationColumn"])
                    ? StiDataTransformationHelper.ColumnItem(column as StiDataTransformationColumn, report.Dictionary, StiDataFiltersHelper.TypeToString(column.Type))
                    : ColumnItem(column);
            }

            CopyProperties(new string[] { "currentParentType", "currentParentName", "mode" }, columnProps, callbackResult);

            if ((string)columnProps["currentParentType"] == "DataSource")
                callbackResult["databases"] = GetDataBasesTree(report);
            else
                callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void DeleteColumn(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            try
            {
                StiDataColumnsCollection columns = GetColumnsByTypeAndNameOfObject(report, param);

                StiDataColumn column = null;
                bool canDelete = false;
                if (columns != null)
                {
                    column = columns[(string)param["columnName"]];
                    canDelete = column != null && !column.Inherited && columns.Contains(column);
                    if (canDelete) columns.Remove(column);
                }
                callbackResult["deleteResult"] = canDelete;
                CopyProperties(new string[] { "currentParentType", "currentParentName", "mode" }, param, callbackResult);
            }
            catch (Exception e)
            {
                callbackResult["error"] = e.Message;
            }
            if ((string)param["currentParentType"] == "DataSource")
                callbackResult["databases"] = GetDataBasesTree(report);
            else
                callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void CreateOrEditParameter(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable parameterProps = (Hashtable)param["parameterFormResult"];
            StiDataSource dataSource = report.Dictionary.DataSources[(string)parameterProps["currentParentName"]] as StiDataSource;
            if (dataSource != null)
            {
                StiDataParameter parameter = null;
                StiDataParametersCollection parameters = dataSource.Parameters;

                if ((string)parameterProps["mode"] == "Edit")
                    parameter = parameters[(string)parameterProps["oldName"]];
                else
                {
                    parameter = new StiDataParameter();
                    parameters.Add(parameter);
                }

                if (parameter != null)
                {
                    ApplyParameterProps(parameter, parameterProps);
                    callbackResult["itemObject"] = ParameterItem(parameter);
                }
            }

            CopyProperties(new string[] { "currentParentName", "mode" }, parameterProps, callbackResult);
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void DeleteParameter(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            try
            {
                StiDataSource dataSource = report.Dictionary.DataSources[(string)param["currentParentName"]] as StiDataSource;
                if (dataSource != null)
                {
                    StiDataParametersCollection parameters = dataSource.Parameters;
                    StiDataParameter parameter = null;
                    bool canDelete = false;
                    if (parameters != null)
                    {
                        parameter = parameters[(string)param["parameterName"]];
                        canDelete = parameter != null && !parameter.Inherited && parameters.Contains(parameter);
                        if (canDelete) parameters.Remove(parameter);
                    }
                    callbackResult["deleteResult"] = canDelete;
                    CopyProperties(new string[] { "currentParentName", "mode" }, param, callbackResult);
                }
            }
            catch (Exception e)
            {
                callbackResult["error"] = e.Message;
            }

            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void CreateOrEditDataSource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable dataSourceProps = (Hashtable)param["dataSourceFormResult"];
            StiDataSource dataSource = null;
            StiDataStoreAdapterService adapterDataStore = null;

            if ((string)dataSourceProps["mode"] == "Edit")
                dataSource = report.Dictionary.DataSources[(string)dataSourceProps["oldName"]];
            else
            {
                if ((string)dataSourceProps["typeDataSource"] == "StiDataTransformation")
                {
                    dataSource = new StiDataTransformation();
                    report.Dictionary.DataSources.Add(dataSource);
                }
                else
                {
                    adapterDataStore = CreateDataAdapterByTypeName((string)dataSourceProps["typeDataAdapter"]) as StiDataStoreAdapterService;
                    if (adapterDataStore != null)
                    {
                        var dict = report.Dictionary;
                        dataSource = adapterDataStore.Create(dict);
                        if (dataSource != null) dataSource.Connect(false);
                    }
                }
            }

            if (dataSource != null)
            {
                if (dataSource is StiDataTransformation)
                {
                    StiDataTransformationHelper.ApplyProperties(dataSource as StiDataTransformation, dataSourceProps, report);
                    callbackResult["itemObject"] = DataTransformationItem(dataSource as StiDataTransformation);
                }
                else
                {
                    ApplyDataSourceProperties(dataSource, dataSourceProps, report);
                    if (!(dataSource is StiVirtualSource) && !(dataSource is StiCrossTabDataSource))
                    {
                        UpdateColumns(dataSource.Columns, (ArrayList)dataSourceProps["columns"]);
                        UpdateParameters(dataSource.Parameters, (ArrayList)dataSourceProps["parameters"]);
                    }
                    callbackResult["itemObject"] = DatasourceItem(dataSource);
                }
            }
            callbackResult["mode"] = dataSourceProps["mode"];
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void DeleteDataSource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDataSource dataSource = report.Dictionary.DataSources[(string)param["dataSourceName"]];
            bool canDelete = dataSource != null && report.Dictionary.DataSources.Contains(dataSource);
            if (canDelete)
            {
                report.Dictionary.DataSources.Remove(dataSource);
                if (param["dataSourceNameInSource"] != null && !(dataSource is StiUserSource))
                {
                    bool canRemoveDataStore = true;
                    foreach (StiDataStoreSource dsSource in report.Dictionary.DataSources)
                    {
                        if (dsSource != null && dsSource.NameInSource == param["dataSourceNameInSource"] as string)
                        {
                            canRemoveDataStore = false;
                            break;
                        }
                    }
                    if (canRemoveDataStore)
                    {
                        var dataStore = report.Dictionary.DataStore[param["dataSourceNameInSource"] as string];
                        if (dataStore != null) report.Dictionary.DataStore.Remove(dataStore);
                    }
                }
            }
            callbackResult["deleteResult"] = canDelete;
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void CreateOrEditBusinessObject(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable businessObjectProps = (Hashtable)param["businessObjectFormResult"];
            StiBusinessObject businessObject = null;

            if ((string)businessObjectProps["mode"] == "Edit")
                businessObject = GetBusinessObjectByFullName(report, businessObjectProps["businessObjectFullName"]);
            else
            {
                businessObject = new StiBusinessObject();
                StiBusinessObject parentBusinessObject = GetBusinessObjectByFullName(report, businessObjectProps["businessObjectFullName"]);
                StiBusinessObjectsCollection businessObjectsCollection = parentBusinessObject != null
                    ? parentBusinessObject.BusinessObjects
                    : report.Dictionary.BusinessObjects;

                businessObjectsCollection.Add(businessObject);
                if (parentBusinessObject != null) callbackResult["parentBusinessObjectFullName"] = businessObjectProps["businessObjectFullName"];
            }

            if (businessObject != null)
            {
                ApplyBusinessObjectProps(businessObject, businessObjectProps);
                UpdateColumns(businessObject.Columns, (ArrayList)businessObjectProps["columns"]);
                callbackResult["itemObject"] = BusinessObjectItem(businessObject);
            }
            callbackResult["mode"] = businessObjectProps["mode"];
            callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void DeleteBusinessObject(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiBusinessObject businessObject = GetBusinessObjectByFullName(report, param["businessObjectFullName"]);
            bool canDelete = businessObject != null;
            if (canDelete)
            {
                StiBusinessObjectsCollection businessObjectsCollection = businessObject.ParentBusinessObject != null
                    ? businessObject.ParentBusinessObject.BusinessObjects
                    : report.Dictionary.BusinessObjects;

                businessObjectsCollection.Remove(businessObject);
            }
            callbackResult["deleteResult"] = canDelete;
            callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void CreateOrEditVariable(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable variableProps = (Hashtable)param["variableFormResult"];
            StiVariable variable = null;

            if ((string)variableProps["mode"] == "Edit")
                variable = report.Dictionary.Variables[(string)variableProps["oldName"]];
            else
            {
                variable = new StiVariable();
                report.Dictionary.Variables.Add(variable);
            }

            if (variable != null)
            {
                ApplyVariableProps(report, variable, variableProps);
                callbackResult["itemObject"] = VariableItem(variable, report);
            }

            CopyProperties(new string[] { "mode", "oldName" }, variableProps, callbackResult);
            callbackResult["variables"] = GetVariablesTree(report);
        }

        public static void DeleteVariable(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiVariable variable = report.Dictionary.Variables[(string)param["variableName"]];
            bool canDelete = variable != null && report.Dictionary.Variables.Contains(variable);
            if (canDelete) report.Dictionary.Variables.Remove(variable);
            callbackResult["deleteResult"] = canDelete;
            callbackResult["variables"] = GetVariablesTree(report);
        }

        public static void DeleteVariablesCategory(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.Dictionary.Variables.RemoveCategory((string)param["categoryName"]);
            callbackResult["variables"] = GetVariablesTree(report);
        }

        public static void EditVariablesCategory(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            var categoryProps = param["categoryFormResult"] as Hashtable;
            var oldCategoryName = categoryProps["oldName"] as string;
            var newCategoryName = categoryProps["name"] as string;
            var requestFromUser = Convert.ToBoolean(categoryProps["requestFromUser"]);
            var readOnly = Convert.ToBoolean(categoryProps["readOnly"]);

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                if (variable.Category == oldCategoryName)
                {
                    variable.Category = newCategoryName;

                    if (variable.IsCategory) {
                        variable.RequestFromUser = requestFromUser;
                        variable.ReadOnly = readOnly;
                    }
                }
            }

            callbackResult["requestFromUser"] = requestFromUser;
            callbackResult["readOnly"] = readOnly;
            callbackResult["newName"] = newCategoryName;
            
            callbackResult["variables"] = GetVariablesTree(report);
        }

        public static void CreateVariablesCategory(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable categoryProps = param["categoryFormResult"] as Hashtable;
            var category = new StiVariable(categoryProps["name"] as string);
            category.RequestFromUser = Convert.ToBoolean(categoryProps["requestFromUser"]);
            category.ReadOnly = Convert.ToBoolean(categoryProps["readOnly"]);
            report.Dictionary.Variables.Add(category);

            callbackResult["name"] = categoryProps["name"] as string;
            callbackResult["requestFromUser"] = category.RequestFromUser;
            callbackResult["readOnly"] = category.ReadOnly;
            callbackResult["variables"] = GetVariablesTree(report);
        }

        public static void CreateOrEditResource(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            Hashtable resourceProps = (Hashtable)param["resourceFormResult"];
            StiResource resource = null;

            #region Load big resources by blocks
            if (param["countBlocks"] != null)
            {
                var cacheGuid = param["cacheGuid"] as string;
                var countBlocks = Convert.ToInt32(param["countBlocks"]);

                var blocks = requestParams.Cache.Helper.GetObjectInternal(requestParams, cacheGuid) as List<string>;
                if (blocks == null) blocks = new List<string>();
                blocks.Add(param["blockContent"] as string);

                callbackResult["progress"] = blocks.Count / (float)countBlocks;

                if (countBlocks > blocks.Count)
                {
                    requestParams.Cache.Helper.SaveObjectInternal(blocks, requestParams, cacheGuid);
                    callbackResult["loadingCompleted"] = false;
                    return;
                }
                else
                {
                    var resultStrings = new string[countBlocks];
                    var resultString = string.Empty;
                    blocks.ForEach(s => { resultStrings[Convert.ToInt32(s.Substring(0, 1))] = s.Substring(1); });
                    resultStrings.ToList().ForEach(s => { resultString += s; });
                    resourceProps["loadedContent"] = resultString;
                    requestParams.Cache.Helper.RemoveObject(cacheGuid);
                }
            }
            #endregion

            if ((string)resourceProps["mode"] == "Edit")
            {
                resource = report.Dictionary.Resources[(string)resourceProps["oldName"]];
            }
            else
            {
                resource = new StiResource();
                report.Dictionary.Resources.Add(resource);
                if (resourceProps["saveCopy"] != null)
                {
                    var sourceResource = report.Dictionary.Resources[(string)resourceProps["oldName"]];
                    if (sourceResource != null && sourceResource.Content != null)
                        resource.Content = sourceResource.Content;
                }

            }

            if (resource != null)
            {
                ApplyResourceProps(report, resource, resourceProps);
                callbackResult["itemObject"] = ResourceItem(resource, report);
            }

            CopyProperties(new string[] { "mode", "oldName" }, resourceProps, callbackResult);
            callbackResult["resources"] = GetResourcesTree(report);
            callbackResult["loadingCompleted"] = true;
            callbackResult["callbackFunctionId"] = param["callbackFunctionId"];
        }

        public static void DeleteResource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiResource resource = report.Dictionary.Resources[(string)param["resourceName"]];
            bool canDelete = resource != null && report.Dictionary.Resources.Contains(resource);

            if (canDelete)
            {
                if (param["ignoreCheckUsed"] == null)
                {
                    var usedFileDatabeses = StiUsedResourceHelper.GetDatabasesUsedResource(report, resource);
                    var usedComponents = StiUsedResourceHelper.GetComponentsUsedResource(report, resource);

                    if (usedFileDatabeses.Count > 0 || usedComponents.Count > 0)
                    {
                        var usedObjects = new ArrayList();
                        foreach (StiDatabase database in usedFileDatabeses)
                        {
                            usedObjects.Add(DatabaseItem(database, report));
                        }
                        foreach (StiComponent component in usedComponents)
                        {
                            var componentItem = new Hashtable();
                            componentItem["typeItem"] = "Component";
                            componentItem["name"] = component.Name;
                            componentItem["alias"] = component.Alias;
                            componentItem["type"] = component.GetType().Name;
                            usedObjects.Add(componentItem);
                        }

                        callbackResult["usedObjects"] = usedObjects;
                        callbackResult["resourceName"] = resource.Name;
                        canDelete = false;
                    }
                    else
                    {
                        canDelete = true;
                    }
                }

            }

            if (canDelete)
            {
                report.Dictionary.Resources.Remove(resource);
                callbackResult["resources"] = GetResourcesTree(report);
            }

            callbackResult["deleteResult"] = canDelete;
        }

        public static void DeleteBusinessObjectCategory(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList collectionForRemove = new ArrayList();
            foreach (StiBusinessObject businessObject in report.Dictionary.BusinessObjects)
                if (businessObject.Category == (string)param["categoryName"])
                    collectionForRemove.Add(businessObject);

            foreach (StiBusinessObject businessObject in collectionForRemove)
                report.Dictionary.BusinessObjects.Remove(businessObject);

            callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void EditBusinessObjectCategory(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable categoryProps = (Hashtable)param["categoryFormResult"];
            string oldCategoryName = (string)categoryProps["oldName"];
            string newCategoryName = (string)categoryProps["name"];

            foreach (StiBusinessObject businessObject in report.Dictionary.BusinessObjects)
                if (businessObject.Category == oldCategoryName)
                    businessObject.Category = newCategoryName;

            CopyProperties(new string[] { "oldName", "name" }, categoryProps, callbackResult);
            callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
        }

        public static void SynchronizeDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.Dictionary.Databases.ToList().ForEach(db => { if (db is StiFileDatabase fileDatabase) fileDatabase.DataSchema = null; });
            report.Dictionary.Synchronize();
            callbackResult["dictionary"] = GetDictionaryTree(report);
        }

        public static void NewDictionary(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.Dictionary.Clear();
            callbackResult["dictionary"] = GetDictionaryTree(report);
        }

        public static void GetAllConnections(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDataAdapterService dataAdapter = CreateDataAdapterByTypeName((string)param["typeDataAdapter"]);
            var result = new Hashtable();

            if (dataAdapter != null)
            {
                report.Dictionary.Connect(false);
                StiDataCollection dataStore = report.DataStore;

                foreach (StiData data in dataStore)
                {
                    if (dataAdapter is StiBusinessObjectAdapterService)
                    {
                        if (data.Data is DataSet ||
                            data.Data is DataTable ||
                            data.Data is DataView ||
                            data.Data is IDbConnection ||
                            data.Data is StiUserData) continue;
                    }

                    var types = dataAdapter.GetDataTypes();
                    if (types != null)
                    {
                        foreach (Type type in types)
                        {
                            if ((StiTypeFinder.FindInterface(data.Data.GetType(), type)) ||
                                StiTypeFinder.FindType(data.Data.GetType(), type))
                            {
                                string category = dataAdapter.GetDataCategoryName(data);
                                if (result[category] == null) result[category] = new ArrayList();
                                var datas = result[category] as ArrayList;
                                if (!datas.Contains(data)) datas.Add(data.Name);
                            }
                        }
                    }
                }

                report.Dictionary.Disconnect();
            }

            callbackResult["connections"] = result;
        }

        public static void RetrieveColumns(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDictionary dictionary = report.Dictionary;
            dictionary.Connect(false);
            ArrayList columns = new ArrayList();
            ArrayList parameters = new ArrayList();
            StiDataStoreSource dataSource = null;
            StiDataStoreSource currentDataSource = null;
            StiDataAdapterService adapter = null;
            string nameInSource = (string)param["nameInSource"];

            var dataSourceName = (string)param["mode"] == "Edit" && param["oldName"] != null ? (string)param["oldName"] : (string)param["name"];
            currentDataSource = dictionary.DataSources[dataSourceName] as StiDataStoreSource;
            if (currentDataSource == null) currentDataSource = CreateDataStoreSourceFromParams(report, param);
            SaveDataSourceParam(ref dataSource, report, currentDataSource, param);

            if (nameInSource.Trim().Length == 0)
            {
                dictionary.Disconnect();
                callbackResult["error"] = string.Format(StiLocalization.Get("Errors", "FieldRequire"), StiLocalization.Get("Report", "LabelNameInSource"));
                return;
            }

            if (dataSource != null)
                foreach (StiData dt in dictionary.DataStore)
                    if (dt.Name.ToLower(CultureInfo.InvariantCulture) == nameInSource.ToLower(CultureInfo.InvariantCulture))
                    {
                        try
                        {
                            if (!(dataSource is StiSqlSource))
                            {
                                adapter = StiDataAdapterService.GetDataAdapter(dataSource);
                                if (adapter != null)
                                {
                                    dictionary.SynchronizeColumns(dt, dataSource);
                                    columns = GetColumnsTree(dataSource.Columns);
                                }
                            }
                            else
                            {
                                #region Get From Sql Data
                                adapter = StiDataAdapterService.GetDataAdapter(dataSource);
                                if (adapter == null) adapter = new StiSqlAdapterService();
                                var sqlSource = dataSource as StiSqlSource;
                                sqlSource.SqlCommand = StiEncodingHelper.DecodeString((string)param["sqlCommand"]);
                                sqlSource.Type = (StiSqlSourceType)Enum.Parse(typeof(StiSqlSourceType), (string)param["type"]);
                                //Apply Parameters
                                if (param["parametersValues"] != null)
                                    ApplyParametersToSqlSourse(sqlSource, (Hashtable)param["parametersValues"]);

                                //Get Columns
                                if (param["onlyParameters"] == null)
                                {
                                    var index = 1;
                                    var dataColumns = adapter.GetColumnsFromData(dt, sqlSource, param["retrieveColumnsAllowRun"] != null ? CommandBehavior.KeyInfo : CommandBehavior.SchemaOnly);
                                    columns = GetColumnsTree(new StiDataColumnsCollection(dataColumns.ToList().Select(c =>
                                    {
                                        if (string.IsNullOrEmpty(c.Name))
                                        {
                                            var baseName = Loc.GetMain("Column");
                                            var columnName = $"{baseName}{ index++}";
                                            while (dataSource.Columns.Contains(columnName) || dataColumns.Contains(columnName))
                                            {
                                                columnName = $"{baseName}{ index++}";
                                            }
                                            return new StiDataColumn(columnName, c.Type);
                                        }
                                        return c;
                                    }).ToList()));
                                }

                                //Get Parameters
                                if (!(adapter is StiODataAdapterService) && !(adapter is StiMongoDbAdapterService))
                                {
                                    var parametersFromData = adapter.GetParametersFromData(dt, sqlSource);
                                    parameters = GetParametersTree(parametersFromData, false);
                                }
                                #endregion
                            }
                        }
                        catch (Exception e)
                        {
                            callbackResult["error"] = e.Message;
                        }
                    }

            dictionary.Disconnect();
            callbackResult["columns"] = columns;
            callbackResult["parameters"] = parameters;
        }

        public static void GetDatabaseData(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDatabase database = report.Dictionary.Databases[(string)param["databaseName"]];
            if (database != null)
            {
                try
                {
                    StiDatabaseInformation information = database.GetDatabaseInformation(report);
                    if (information != null) callbackResult["data"] = GetAjaxDataFromDatabaseInformation(information);
                }
                catch (Exception e)
                {
                    callbackResult["error"] = e.Message;
                }
            }
        }

        public static void ApplySelectedData(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            ArrayList data = (ArrayList)param["data"];
            string databaseName = (string)param["databaseName"];
            StiDatabase database = report.Dictionary.Databases[databaseName];
            if (database != null && data.Count > 0)
            {
                try
                {
                    StiDatabaseInformation info = ConvertAjaxDatabaseInfoToDatabaseInfo(data, false);
                    StiDatabaseInformation allInfo = ConvertAjaxDatabaseInfoToDatabaseInfo(data, true);
                    database.ApplyDatabaseInformation(info, report, allInfo);

                    var i = 0;
                    info.Tables.ToList().ForEach(t => ((Hashtable)data[i++])["correctName"] = t.TableName);

                    //Add Relations                   
                    foreach (Hashtable dataSourceObject in data)
                    {
                        if (dataSourceObject["relations"] != null)
                        {
                            foreach (Hashtable relationObject in ((ArrayList)dataSourceObject["relations"]))
                            {
                                StiDataRelation relation = new StiDataRelation();
                                relationObject["childDataSource"] = dataSourceObject["correctName"];

                                Hashtable parentData = null;
                                foreach (Hashtable d in data)
                                {
                                    if (d["name"].Equals(relationObject["parentDataSource"])) 
                                        parentData = d;
                                }

                                if (parentData != null)
                                {
                                    relationObject["parentDataSource"] = parentData["correctName"];
                                    relationObject["name"] = parentData["correctName"];
                                    relationObject["alias"] = parentData["correctName"];
                                }


                                report.Dictionary.Relations.Add(relation);
                                ApplyRelationProps(report, relation, relationObject);
                            }
                        }
                    }
                    //----------

                    callbackResult["dictionary"] = GetDictionaryTree(report);
                    if (data != null && data.Count > 0) callbackResult["selectedDataSource"] = data[data.Count - 1];
                    callbackResult["databaseName"] = databaseName;
                }
                catch (Exception e)
                {
                    callbackResult["error"] = e.Message;
                }
            }
        }

        public static void TestConnection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            string typeConnection = (string)param["typeConnection"];
            string connectionString = StiEncodingHelper.DecodeString((string)param["connectionString"]);

            var database = CreateDataBaseByTypeName(typeConnection);

            if (database is StiSqlDatabase)
                callbackResult["testResult"] = ((StiSqlDatabase)database).GetDataAdapter().TestConnection(connectionString);
            else if (database is StiNoSqlDatabase)
                callbackResult["testResult"] = ((StiNoSqlDatabase)database).GetDataAdapter().TestConnection(connectionString);
        }

        public static void RunQueryScript(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDictionary dictionary = report.Dictionary;
            StiDataStoreSource dataSource = null;
            StiDataStoreSource currentDataSource = null;

            try
            {
                var dataSourceName = (string)param["mode"] == "Edit" && param["oldName"] != null ? (string)param["oldName"] : (string)param["name"];
                currentDataSource = dictionary.DataSources[dataSourceName] as StiDataStoreSource;
                if (currentDataSource == null) currentDataSource = CreateDataStoreSourceFromParams(report, param);
                SaveDataSourceParam(ref dataSource, report, currentDataSource, param);

                if (param["parametersValues"] != null && dataSource is StiSqlSource)
                    ApplyParametersToSqlSourse(dataSource as StiSqlSource, (Hashtable)param["parametersValues"]);

                if (dataSource != null)
                {
                    if (dataSource.NameInSource == null ||
                        dataSource.NameInSource.Trim().Length == 0)
                    {
                        callbackResult["resultQueryScript"] = string.Format(StiLocalization.Get("Errors", "FieldRequire"), StiLocalization.Get("Report", "LabelNameInSource"));
                        return;
                    }

                    try
                    {
                        dataSource.Dictionary = dictionary;
                        dataSource.Dictionary.ConnectToDatabases(true);
                        dataSource.Connect(true);
                        callbackResult["resultQueryScript"] = "successfully";
                    }
                    finally
                    {
                        dataSource.Disconnect();
                        dictionary.Disconnect();
                    }
                }
            }
            catch (Exception ee)
            {
                callbackResult["resultQueryScript"] = ee.Message;
            }
        }

        public static void ViewData(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDictionary dictionary = report.Dictionary;
            StiDataStoreSource dataSource = null;
            StiBusinessObject businessObject = null;
            dictionary.Connect(false);

            try
            {
                if ((string)param["typeItem"] == "BusinessObject")
                {
                    var fullName = param["fullName"] as string;
                    ArrayList nameArray = new ArrayList(fullName.Split('.'));
                    nameArray.Reverse();
                    businessObject = GetBusinessObjectByFullName(report, nameArray);
                }
                else
                {
                    var dataSourceName = (string)param["mode"] == "Edit" && param["oldName"] != null ? (string)param["oldName"] : (string)param["name"];
                    dataSource = dictionary.DataSources[dataSourceName] as StiDataStoreSource;

                    if (dataSource == null || dataSource is StiSqlSource)
                    {
                        var currentDataSource = CreateDataStoreSourceFromParams(report, param);
                        SaveDataSourceParam(ref dataSource, report, currentDataSource, param);
                    }
                }

                if (dataSource != null || businessObject != null)
                {
                    if (param["parametersValues"] != null && dataSource is StiSqlSource)
                    {
                        ApplyParametersToSqlSourse(dataSource as StiSqlSource, (Hashtable)param["parametersValues"]);
                    }

                    var viewDataHelper = new StiViewDataHelper(dataSource, businessObject);
                    DataTable dataTable = viewDataHelper.ResultDataTable;

                    ArrayList resultData = new ArrayList();
                    List<StiDataColumn> dictionaryColumns = new List<StiDataColumn>();

                    if (dataTable != null)
                    {
                        ArrayList captions = new ArrayList();
                        for (int k = 0; k < dataTable.Columns.Count; k++)
                        {
                            StiDataColumn dictionaryColumn = businessObject != null
                                ? businessObject.Columns[dataTable.Columns[k].Caption]
                                : dataSource.Columns[dataTable.Columns[k].Caption];
                            captions.Add(dataTable.Columns[k].Caption);
                            dictionaryColumns.Add(dictionaryColumn);
                        }
                        resultData.Add(captions);

                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            ArrayList rowArray = new ArrayList();
                            resultData.Add(rowArray);
                            for (int k = 0; k < dataTable.Columns.Count; k++)
                            {
                                rowArray.Add(GetViewDataItemValue(dataTable.Rows[i][k], dictionaryColumns[k]));
                            }
                        }
                    }

                    callbackResult["resultData"] = resultData;
                    callbackResult["dataSourceName"] = businessObject != null ? businessObject.Name : dataSource.Name;
                }
            }
#if CLOUD
            catch (StiMaxDataRowsException)
            {
                callbackResult["cloudNotificationMaxDataRows"] = true;
            }
#endif
            catch (Exception e)
            {
                callbackResult["error"] = e.Message;
            }
            finally
            {
                if (dataSource != null)
                    dataSource.Disconnect();

                if (businessObject != null)
                    businessObject.Disconnect();

                dictionary.Disconnect();
            }
        }

        public static void GetSqlParameterTypes(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            string typeDataSource = (string)((Hashtable)param["dataSource"])["typeDataSource"];
            var dataSource = StiOptions.Services.DataSource.FirstOrDefault(a => typeDataSource == a.GetType().Name);

            if (dataSource != null && dataSource is StiSqlSource && !(dataSource is StiMongoDbSource))
            {
                callbackResult["sqlParameterTypes"] = GetDataParameterTypes(dataSource as StiSqlSource);
            }
        }

        public static void CreateFieldOnDblClick(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDataColumn column = null;
            StiDataParameter parameter = null;
            StiDataSource dataSource = null;
            StiDataBand dataBand = null;
            StiDataBand firstDataBand = null;
            StiDataBand selectedDataBand = null;
            StiPage currentPage = report.Pages[(string)param["pageName"]];
            ArrayList selectedComponentNames = (ArrayList)param["selectedComponents"];
            ArrayList newComponents = new ArrayList();
            double zoom = StiReportEdit.StrToDouble((string)param["zoom"]);

            if (param["columnName"] != null)
            {
                StiDataColumnsCollection columns = GetColumnsByTypeAndNameOfObject(report, param);
                if (columns != null) column = columns[(string)param["columnName"]];
                if (column != null) dataSource = column.DataSource;
            }
            else if (param["parameterName"] != null)
            {
                dataSource = report.Dictionary.DataSources[(string)param["currentParentName"]] as StiDataSource;
                if (dataSource != null)
                {
                    StiDataParametersCollection parameters = dataSource.Parameters;
                    if (parameters != null) parameter = parameters[(string)param["parameterName"]];
                }
            }

            //set not selected all components            
            foreach (StiComponent component in currentPage.GetComponents())
            {
                component.IsSelected = false;
            }

            //set selected current components
            foreach (string componentName in selectedComponentNames)
            {
                StiComponent component = report.Pages.GetComponentByName(componentName);
                if (component != null) component.Select();
            }

            #region Search DataBand
            var selectedDataBands = new List<StiDataBand>();

            var comps = currentPage.GetComponents();
            foreach (StiComponent component in comps)
            {
                var band = component as StiDataBand;
                if (band == null) continue;

                if (firstDataBand == null) firstDataBand = band;
                if (selectedDataBand == null && band.IsSelected) selectedDataBand = band;
                if (band.DataSource == dataSource)
                {
                    selectedDataBands.Add(band);
                }
            }

            if (selectedDataBands.Count > 0)
            {
                foreach (var band in selectedDataBands)
                {
                    if (dataBand == null) dataBand = band;
                    else if (((!dataBand.IsSelected) && band.IsSelected))
                    {
                        dataBand = band;
                        break;
                    }
                }
            }
            #endregion

            if (dataBand == null)
            {
                if (selectedDataBand != null) dataBand = selectedDataBand;
                else dataBand = firstDataBand;
            }

            if (dataBand != null)
            {
                StiBaseStyle style = new StiStyle();
                double posX = 0;
                foreach (StiComponent comp in dataBand.Components)
                {
                    if (posX < comp.Right)
                    {
                        posX = comp.Right;
                        if (!(comp is IStiIgnoryStyle)) style = StiBaseStyle.GetStyle(comp);
                    }
                }

                double width = StiAlignValue.AlignToMaxGrid(dataBand.Width / 8,
                    currentPage.GridSize, true);

                var rect = new RectangleD(posX, 0, width, dataBand.Height);

                #region DataBands
                StiComponent newComp = null;

                if (param["resourceName"] != null)
                {
                    if (param["resourceType"] as string == "Image")
                        newComp = new StiImage(rect)
                        {
                            ImageURL = new StiImageURLExpression(StiHyperlinkProcessor.CreateResourceName(param["resourceName"] as string))
                        };
                    else if (param["resourceType"] as string == "Rtf")
                        newComp = new StiRichText(rect)
                        {
                            DataUrl = new StiDataUrlExpression(StiHyperlinkProcessor.CreateResourceName(param["resourceName"] as string))
                        };
                }
                else
                {
                    if (column != null && column.Type == typeof(Image))
                    {
                        newComp = new StiImage(rect)
                        {
                            DataColumn = column.GetColumnPath()
                        };
                    }
                    else
                    {
                        newComp = new StiText(rect)
                        {
                            Text = (string)param["fullName"],
                            Type = StiSystemTextType.DataColumn
                        };
                    }
                }

                if (!(newComp is IStiIgnoryStyle))
                {
                    if (param["lastStyleProperties"] != null)
                        StiReportEdit.SetAllProperties(newComp, param["lastStyleProperties"] as ArrayList);
                    else
                        style.SetStyleToComponent(newComp);
                }

                dataBand.Components.Add(newComp);
                newComponents.Add(StiReportEdit.GetComponentMainProperties(newComp, zoom));
                #endregion

                var builder = StiDataBandV1Builder.GetBuilder(typeof(StiDataBand)) as StiDataBandV1Builder;
                var headers = builder.GetHeaders(dataBand);

                #region Headers
                foreach (StiHeaderBand header in headers)
                {
                    posX = 0;
                    foreach (StiComponent comp in header.Components)
                    {
                        if (posX < comp.Right)
                        {
                            posX = comp.Right;
                            if (!(comp is IStiIgnoryStyle)) style = StiBaseStyle.GetStyle(comp);
                        }
                    }

                    var text = new StiText(rect)
                    {
                        Height = header.Height,
                        HorAlignment = StiTextHorAlignment.Center
                    };

                    if (!(text is IStiIgnoryStyle)) style.SetStyleToComponent(text);

                    if (param["resourceName"] != null)
                    {
                        text.Text = param["resourceName"] as string;
                    }
                    else if (column != null)
                    {
                        text.Text = ((StiDataColumn)column).Alias;
                    }
                    else if (parameter != null)
                    {
                        string str = ((StiDataParameter)parameter).Name;
                        if (str.StartsWith("@", StringComparison.InvariantCulture)) str = str.Substring(1);
                        text.Text = str;
                    }
                    header.Components.Add(text);
                    newComponents.Add(StiReportEdit.GetComponentMainProperties(text, zoom));
                }
                #endregion
            }

            callbackResult["pageName"] = currentPage.Name;
            callbackResult["newComponents"] = newComponents;
            callbackResult["rebuildProps"] = StiReportEdit.GetPropsRebuildPage(report, currentPage);
        }

        public static void GetParamsFromQueryString(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiSqlSource dataSource = report.Dictionary.DataSources[(string)param["dataSourceName"]] as StiSqlSource;
            if (dataSource != null)
            {
                string queryString = StiEncodingHelper.DecodeString((string)param["queryString"]);
                var exps = dataSource.AllowExpressions ? StiCodeDomExpressionHelper.GetLexem(queryString) : new List<string>();
                var parameters = new ArrayList();
                foreach (string value in exps)
                {
                    if (value.StartsWith("{") && value.EndsWith("}") && !parameters.Contains(value))
                        parameters.Add(value);
                }

                callbackResult["params"] = parameters;
            }
        }

        public static void GetImagesGallery(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            #region Variables
            var variables = StiGalleriesHelper.GetImageVariables(report);
            ArrayList resultVariables = new ArrayList();

            foreach (var variable in variables)
            {
                resultVariables.Add(ImagesGalleryItem(variable.Name, typeof(StiVariable), StiReportEdit.ImageToBase64(variable.ValueObject as Image)));
            }
            #endregion

            #region Resources
            var resources = StiGalleriesHelper.GetImageResources(report);
            ArrayList resultResources = new ArrayList();

            foreach (var resource in resources)
            {
                string base64Content = string.Empty;
                if (resource.Content != null)
                {
                    if (Stimulsoft.Report.Helpers.StiImageHelper.IsMetafile(resource.Content))
                        base64Content = StiReportEdit.GetBase64PngFromMetaFileBytes(resource.Content);
                    else
                        base64Content = StiReportEdit.ImageToBase64(resource.Content);
                }
                resultResources.Add(ImagesGalleryItem(resource.Name, typeof(StiResource), base64Content));
            }
            #endregion

            #region Columns
            ArrayList resultColumns = new ArrayList();
            if (StiOptions.Designer.Editors.AllowConnectToDataInGallery)
            {
                try
                {
                    var columns = StiGalleriesHelper.GetImageColumns(report);
                    var dataSources = columns.Select(c => c.DataSource).Distinct().ToList();
                    report.Dictionary.Connect(true, dataSources);

                    foreach (var column in columns)
                    {
                        var image = StiGalleriesHelper.GetImageFromColumn(column, report);
                        if (image == null) continue;

                        var columnPath = column.GetColumnPath();
                        resultColumns.Add(ImagesGalleryItem(column.GetColumnPath(), typeof(StiDataColumn), StiReportEdit.ImageToBase64(image)));

                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            #endregion

            Hashtable imagesGallery = new Hashtable();
            imagesGallery["variables"] = resultVariables;
            imagesGallery["resources"] = resultResources;
            imagesGallery["columns"] = resultColumns;

            callbackResult["imagesGallery"] = imagesGallery;
        }

        public static void GetRichTextGallery(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            #region Variables
            var variables = StiGalleriesHelper.GetRichTextVariables(report);
            ArrayList resultVariables = new ArrayList();

            foreach (var variable in variables)
            {
                string imageName = "Resources." + (variable.ValueObject is string && StiRtfHelper.IsRtfText(variable.ValueObject as string) ? "BigResourceRtf" : "BigResourceTxt");

                resultVariables.Add(RichTextGalleryItem(variable.Name, variable.GetType(), imageName));
            }
            #endregion

            #region Resources
            var resources = StiGalleriesHelper.GetRichTextResources(report);
            ArrayList resultResources = new ArrayList();

            foreach (var resource in resources)
            {
                resultResources.Add(RichTextGalleryItem(resource.Name, resource.GetType(), "Resources.BigResource" + resource.Type.ToString()));
            }
            #endregion

            #region Columns
            ArrayList resultColumns = new ArrayList();
            if (StiOptions.Designer.Editors.AllowConnectToDataInGallery)
            {
                try
                {
                    var columns = StiGalleriesHelper.GetRichTextColumns(report);
                    var dataSources = columns.Select(c => c.DataSource).Distinct().ToList();
                    report.Dictionary.Connect(true, dataSources);

                    foreach (var column in columns)
                    {
                        if (!StiGalleriesHelper.IsRtfColumn(column, report)) continue;

                        var columnPath = column.GetColumnPath();
                        resultColumns.Add(RichTextGalleryItem(column.GetColumnPath(), column.GetType(), "Resources.BigResourceRtf"));

                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            #endregion

            Hashtable richTextGallery = new Hashtable();
            richTextGallery["variables"] = resultVariables;
            richTextGallery["resources"] = resultResources;
            richTextGallery["columns"] = resultColumns;

            callbackResult["richTextGallery"] = richTextGallery;
        }

        public static void GetSampleConnectionString(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            string typeConnection = (string)param["typeConnection"];
            var database = CreateDataBaseByTypeName(typeConnection);
            if (database is StiSqlDatabase)
            {
                callbackResult["connectionString"] = ((StiSqlDatabase)database).CreateSqlConnector().GetSampleConnectionString();
            }
            else if (database is StiNoSqlDatabase)
            {
                callbackResult["connectionString"] = ((StiNoSqlDatabase)database).CreateNoSqlConnector().GetSampleConnectionString();
            }
        }

        public static void CreateDatabaseFromResource(StiReport report, Hashtable param, Hashtable callbackResult, StiRequestParams requestParams)
        {
            Hashtable resourceProps = param["resourceObject"] as Hashtable;
            StiResource resource = null;

            #region Load big datas by blocks
            if (param["countBlocks"] != null)
            {
                var cacheGuid = param["cacheGuid"] as string;
                var countBlocks = Convert.ToInt32(param["countBlocks"]);
                var blocks = requestParams.Cache.Helper.GetObjectInternal(requestParams, cacheGuid) as List<string>;
                if (blocks == null) blocks = new List<string>();
                blocks.Add(param["blockContent"] as string);

                callbackResult["progress"] = blocks.Count / (float)countBlocks;

                if (countBlocks > blocks.Count)
                {
                    requestParams.Cache.Helper.SaveObjectInternal(blocks, requestParams, cacheGuid);
                    callbackResult["loadingCompleted"] = false;
                    return;
                }
                else
                {
                    var resultStrings = new string[countBlocks];
                    var resultString = string.Empty;
                    blocks.ForEach(s => { resultStrings[Convert.ToInt32(s.Substring(0, 1))] = s.Substring(1); });
                    resultStrings.ToList().ForEach(s => { resultString += s; });
                    resourceProps["loadedContent"] = resultString;
                    requestParams.Cache.Helper.RemoveObject(cacheGuid);
                }
            }
            #endregion

            if (Convert.ToBoolean(param["existingResource"]))
            {
                resource = report.Dictionary.Resources[(string)resourceProps["name"]];
            }
            else
            {
                resource = new StiResource();
                report.Dictionary.Resources.Add(resource);
                ApplyResourceProps(report, resource, resourceProps);
                callbackResult["resourceItemObject"] = ResourceItem(resource, report);
                callbackResult["resources"] = GetResourcesTree(report);
            }

            StiFileDatabase database = CreateNewDatabaseFromResource(report, resource);
            if (database != null)
            {
                database.Name = database.Alias = GetNewDatabaseName(report, resource.Name);
                database.PathData = StiHyperlinkProcessor.ResourceIdent + resource.Name;
                report.Dictionary.Databases.Add(database);
                database.CreateDataSources(report.Dictionary);

                callbackResult["newDataBaseName"] = database.Name;
                callbackResult["databases"] = GetDataBasesTree(report);
            }

            callbackResult["loadingCompleted"] = true;
        }

        public static void DeleteAllDataSources(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            if (param["dataSources"] != null)
            {
                ArrayList dataSources = param["dataSources"] as ArrayList;
                foreach (Hashtable dataSourceObject in dataSources)
                {
                    if (dataSourceObject["typeItem"] as string == "BusinessObject")
                    {
                        var fullName = dataSourceObject["name"] as string;
                        ArrayList nameArray = new ArrayList();
                        nameArray.AddRange(fullName.Split('.'));
                        nameArray.Reverse();
                        StiBusinessObject businessObject = GetBusinessObjectByFullName(report, nameArray);
                        if (businessObject != null)
                        {
                            businessObject.ParentBusinessObject.BusinessObjects.Remove(businessObject);
                        }
                    }
                    else
                    {
                        var dataSource = report.Dictionary.DataSources[dataSourceObject["name"] as string];
                        var connectionName = dataSource.GetCategoryName();
                        if (dataSource != null && (param["connectionName"] == null || connectionName == param["connectionName"] as string))
                        {
                            report.Dictionary.DataSources.Remove(dataSource);
                            if (dataSourceObject["nameInSource"] != null)
                            {
                                var dataStore = report.Dictionary.DataStore[dataSourceObject["nameInSource"] as string];
                                if (dataStore != null) report.Dictionary.DataStore.Remove(dataStore);
                            }
                            if (!String.IsNullOrEmpty(connectionName))
                            {
                                var database = report.Dictionary.Databases[connectionName];
                                if (database != null)
                                {
                                    bool allowDelete = true;
                                    foreach (StiDataSource dataSource_ in report.Dictionary.DataSources)
                                    {
                                        if (dataSource_.GetCategoryName() == connectionName)
                                        {
                                            allowDelete = false;
                                            break;
                                        }
                                    }
                                    if (allowDelete) report.Dictionary.Databases.Remove(database);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                report.Dictionary.BusinessObjects.Clear();
                report.Dictionary.DataSources.Clear();
                report.Dictionary.Databases.Clear();
                report.Dictionary.DataStore.Clear();
                report.Dictionary.Synchronize();
            }

            callbackResult["businessObjects"] = GetBusinessObjectsTree(report);
            callbackResult["databases"] = GetDataBasesTree(report);
        }

        public static void GetVariableItemsFromDataColumn(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            report.Dictionary.Connect(false);
            ArrayList items = new ArrayList();
            var keys = StiDataColumn.GetDatasFromDataColumn(report.Dictionary, param["keysColumn"] as string);
            var values = StiDataColumn.GetDatasFromDataColumn(report.Dictionary, param["valuesColumn"] as string);

            if (keys != null || values != null)
            {
                int maxLength = Math.Max(keys.Length, values.Length);
                for (int index = 0; index < maxLength; index++)
                {
                    Hashtable item = new Hashtable();
                    item["key"] = index < keys.Length ? keys[index] as string : String.Empty;
                    item["value"] = index < values.Length ? values[index] as string : String.Empty;
                    items.Add(item);
                }
            }

            report.Dictionary.Disconnect();
            callbackResult["items"] = items;
        }

        public static void MoveDictionaryItem(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            Hashtable fromObject = param["fromObject"] as Hashtable;
            Hashtable toObject = param["toObject"] as Hashtable;
            string direction = param["direction"] as string;

            if (fromObject != null && toObject != null)
            {
                string typeItem = fromObject["typeItem"] as string;

                if ((typeItem == "Variable" || typeItem == "Category"))
                {
                    #region Move Variables 
                    StiVariable fromVariable = fromObject["typeItem"] as string == "Category"
                        ? GetVariableCategory(report, fromObject["name"] as string) : report.Dictionary.Variables[fromObject["name"] as string];

                    StiVariable toVariable = toObject["typeItem"] as string == "Category"
                        ? GetVariableCategory(report, toObject["name"] as string) : report.Dictionary.Variables[toObject["name"] as string];

                    if ((toVariable == null && toObject["typeItem"] as string != "VariablesMainItem") || fromVariable == null || fromVariable == toVariable)
                        return;

                    #region variable to variables main item
                    if (toObject["typeItem"] as string == "VariablesMainItem")
                    {
                        fromVariable.Category = string.Empty;
                        report.Dictionary.Variables.Add(fromVariable);
                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion

                    #region variable to variable
                    else if (!IsCategoryVariable(fromVariable) && !IsCategoryVariable(toVariable))
                    {
                        int fromIndex = report.Dictionary.Variables.IndexOf(fromVariable);
                        int toIndex = report.Dictionary.Variables.IndexOf(toVariable);

                        report.Dictionary.Variables.Remove(fromVariable);
                        int toIndex2 = report.Dictionary.Variables.IndexOf(toVariable);

                        if (fromIndex < toIndex)
                            report.Dictionary.Variables.Insert(toIndex2 + 1, fromVariable);
                        else
                            report.Dictionary.Variables.Insert(toIndex2, fromVariable);

                        fromVariable.Category = toVariable.Category;
                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion

                    #region variable to category
                    else if (!IsCategoryVariable(fromVariable) && IsCategoryVariable(toVariable))
                    {
                        int index = report.Dictionary.Variables.GetLastCategoryIndex(toVariable.Category);
                        report.Dictionary.Variables.Remove(fromVariable);
                        if (index + 1 < report.Dictionary.Variables.Count)
                        {
                            report.Dictionary.Variables.Insert(index, fromVariable);
                        }
                        else
                        {
                            report.Dictionary.Variables.Add(fromVariable);
                        }

                        fromVariable.Category = toVariable.Category;
                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion

                    #region category to category
                    else if (IsCategoryVariable(fromVariable) && IsCategoryVariable(toVariable))
                    {
                        report.Dictionary.Variables.MoveCategoryTo(fromVariable.Category, toVariable.Category);
                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion
                    else
                    {
                        int index = report.Dictionary.Variables.IndexOf(toVariable);
                        report.Dictionary.Variables.Remove(fromVariable);
                        report.Dictionary.Variables.Insert(index, fromVariable);

                        if (!IsCategoryVariable(fromVariable))
                            fromVariable.Category = toVariable.Category;

                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion
                }
                else if (typeItem == "DataSource")
                {
                    #region Move DataSource
                    StiDataSource fromDataSource = report.Dictionary.DataSources[fromObject["name"] as string];
                    StiDataSource toDataSource = report.Dictionary.DataSources[toObject["name"] as string];

                    if (fromDataSource != null && toDataSource != null)
                    {
                        //store relations for this datasource
                        var relations = new StiDataRelationsCollection(report.Dictionary);

                        foreach (StiDataRelation relation in report.Dictionary.Relations)
                        {
                            if (relation.ParentSource == fromDataSource || relation.ChildSource == fromDataSource)
                            {
                                relations.Insert(0, relation);
                            }
                        }

                        int index = report.Dictionary.DataSources.IndexOf(toDataSource);
                        report.Dictionary.DataSources.Remove(fromDataSource);
                        report.Dictionary.DataSources.Insert(index, fromDataSource);

                        //restore relations
                        if (relations.Count > 0)
                        {
                            foreach (StiDataRelation relation in relations)
                            {
                                report.Dictionary.Relations.Add(relation);
                            }
                            relations.Clear();
                        }

                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion;
                }
                else if (typeItem == "Column")
                {
                    #region Move Column
                    StiDataColumnsCollection columns = GetColumnsByTypeAndNameOfObject(report, param);

                    if (columns != null)
                    {
                        StiDataColumn fromColumn = columns[fromObject["name"] as string];
                        StiDataColumn toColumn = columns[toObject["name"] as string];

                        if (toColumn == null || fromColumn == null || fromColumn == toColumn)
                            return;

                        int index = columns.IndexOf(toColumn);
                        columns.Remove(fromColumn);
                        columns.Insert(index, fromColumn);
                        callbackResult["moveCompleted"] = true;
                    }
                    #endregion
                }

                callbackResult["direction"] = direction;
                callbackResult["fromObject"] = param["fromObject"];
                callbackResult["toObject"] = param["toObject"];
                callbackResult["variablesTree"] = GetVariablesTree(report);
                callbackResult["databases"] = GetDataBasesTree(report);
            }
        }

        public static void MoveConnectionDataToResource(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            try
            {
                var propertyNames = new string[] { "pathSchema", "pathData" };
                var typeConnection = param["typeConnection"] as string;

                foreach (var propertyName in propertyNames)
                {
                    string path = param[propertyName] as string;

                    if (!String.IsNullOrEmpty(path))
                    {
                        path = StiEncodingHelper.DecodeString(path);

                        if (!path.ToLowerInvariant().StartsWithInvariant(StiHyperlinkProcessor.ResourceIdent))
                        {
                            string resName = StiNameCreation.CreateResourceName(report, Path.GetFileNameWithoutExtension(path).Normalize());

                            var type = StiResourceTypeHelper.GetTypeFromExtension(Path.GetExtension(path).ToLowerInvariant());

                            if (type == null)
                            {
                                if (typeConnection == "StiDBaseDatabase" || typeConnection == "StiCsvDatabase")
                                {
                                    #region Read data

                                    var ext = typeConnection == "StiDBaseDatabase" ? "*.dbf" : "*.csv";
                                    var datas = StiUniversalDataLoader.LoadMutiple(report, path, ext);
                                    if (datas != null)
                                    {
                                        DataSet dataSet = null;

                                        foreach (var data in datas)
                                        {
                                            try
                                            {
                                                var dataTable = typeConnection == "StiDBaseDatabase"
                                                    ? StiDBaseHelper.GetTable(data.Array, StiReportEdit.StrToInt(param["codePageDbase"] as string))
                                                    : StiCsvHelper.GetTable(data.Array, StiReportEdit.StrToInt(param["codePageCsv"] as string), param["separator"] as string);

                                                if (dataTable == null) continue;

                                                if (dataSet == null)
                                                    dataSet = new DataSet { EnforceConstraints = true };

                                                dataTable.TableName = data.Name;
                                                dataSet.Tables.Add(dataTable);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        if (dataSet != null)
                                        {
                                            string resNameXml = StiNameCreation.CreateResourceName(report, Path.GetFileNameWithoutExtension(path).Normalize() + "Xml");
                                            string resNameXsd = StiNameCreation.CreateResourceName(report, Path.GetFileNameWithoutExtension(path).Normalize() + "Xsd");

                                            StiResource resourceXml = null;
                                            StiResource resourceXsd = null;

                                            using (var streamXml = new MemoryStream())
                                            {
                                                dataSet.WriteXml(streamXml);
                                                streamXml.Flush();

                                                if (streamXml.Length > StiOptions.Engine.ReportResources.MaximumSize)
                                                {
                                                    callbackResult["error"] = StiLocalization.Get("Notices", "QuotaMaximumFileSizeExceeded");
                                                    return;
                                                }

                                                resourceXml = new StiResource(resNameXml, resNameXml, StiResourceType.Xml, streamXml.ToArray());
                                            }
                                            using (var streamXsd = new MemoryStream())
                                            {
                                                dataSet.WriteXmlSchema(streamXsd);
                                                streamXsd.Flush();

                                                if (streamXsd.Length > StiOptions.Engine.ReportResources.MaximumSize)
                                                {
                                                    callbackResult["error"] = StiLocalization.Get("Notices", "QuotaMaximumFileSizeExceeded");
                                                    return;
                                                }

                                                resourceXsd = new StiResource(resNameXsd, resNameXsd, StiResourceType.Xsd, streamXsd.ToArray());
                                            }

                                            dataSet.Dispose();

                                            report.Dictionary.Resources.Add(resourceXml);
                                            report.Dictionary.Resources.Add(resourceXsd);

                                            #region replace old connection
                                            if (param["oldName"] != null)
                                            {
                                                StiDatabase database = report.Dictionary.Databases[param["oldName"] as string];
                                                if (database != null) report.Dictionary.Databases.Remove(database);
                                            }

                                            var xmlDatabase = new StiXmlDatabase
                                            {
                                                Name = param["name"] as string,
                                                Alias = param["alias"] as string,
                                                PathData = "resource://" + resNameXml,
                                                PathSchema = "resource://" + resNameXsd,
                                            };

                                            report.Dictionary.Databases.Add(xmlDatabase);
                                            #endregion

                                            callbackResult["databases"] = GetDataBasesTree(report);

                                            if (param["oldName"] == null)
                                            {
                                                callbackResult["connectionObject"] = callbackResult["itemObject"] = DatabaseItem(xmlDatabase, report);
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    callbackResult["error"] = String.Format(StiLocalization.Get("Notices", "IsNotCorrect"), path);
                                }
                            }
                            else
                            {
                                byte[] bytes = null;

                                if (File.Exists(path))
                                {
                                    if (new FileInfo(path).Length > StiOptions.Engine.ReportResources.MaximumSize)
                                    {
                                        callbackResult["error"] = StiLocalization.Get("Notices", "QuotaMaximumFileSizeExceeded");
                                        return;
                                    }
                                    else
                                        bytes = File.ReadAllBytes(path);
                                }
                                else
                                {
                                    bytes = StiBytesFromURL.Load(path);

                                    if (bytes != null && bytes.Length > StiOptions.Engine.ReportResources.MaximumSize)
                                    {
                                        callbackResult["error"] = StiLocalization.Get("Notices", "QuotaMaximumFileSizeExceeded");
                                        return;
                                    }
                                }

                                if (bytes != null)
                                {
                                    var resource = new StiResource(resName, resName, type.GetValueOrDefault(), bytes);
                                    report.Dictionary.Resources.Add(resource);
                                    callbackResult[propertyName] = StiHyperlinkProcessor.ResourceIdent + resName;
                                }
                                else
                                {
                                    callbackResult["error"] = String.Format(StiLocalization.Get("Notices", "IsNotCorrect"), path);
                                }
                            }
                        }
                    }
                }

                callbackResult["resourcesTree"] = GetResourcesTree(report);
            }
            catch (Exception e)
            {
                callbackResult["error"] = e.Message;
            }
        }

        public static void OpenDictionary(StiRequestParams requestParams, StiReport report, Hashtable callbackResult)
        {
            using (var stream = new MemoryStream(requestParams.Data))
            {
                var slServices = StiSLService.GetDictionarySLServices(StiSLActions.Load);
                if (slServices.Count > 0)
                {
                    report.Dictionary.Load(slServices[0], stream);
                    StiCacheCleaner.Clean(report);
                    report.Dictionary.Synchronize();
                }
                callbackResult["dictionary"] = GetDictionaryTree(report);
            }
        }

        public static void MergeDictionary(StiRequestParams requestParams, StiReport report, Hashtable callbackResult)
        {
            using (var stream = new MemoryStream(requestParams.Data))
            {
                var slServices = StiSLService.GetDictionarySLServices(StiSLActions.Merge);
                if (slServices.Count > 0)
                {
                    report.Dictionary.Merge(slServices[0], stream);
                    StiCacheCleaner.Clean(report);
                    report.Dictionary.Synchronize();
                }
                callbackResult["dictionary"] = GetDictionaryTree(report);
            }
        }

        public static void EmbedAllDataToResources(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            StiDataResourceHelper.SaveSnapshot(report);
            callbackResult["dictionary"] = GetDictionaryTree(report);
        }

        public static void TestODataConnection(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            try
            {
                var result = StiODataConnector.Get(StiEncodingHelper.DecodeString(param["connectionString"] as string)).TestConnection();
                callbackResult["resultTest"] = result.Success ? "Test OK!" : result.Notice;
            }
            catch
            {
                callbackResult["resultTest"] = "Error Test Connection!";
            }
        }

        public static void GetAzureBlobStorageContainerNamesItems(Hashtable param, Hashtable callbackResult)
        {
            var connectionString = StiEncodingHelper.DecodeString(param["connectionString"] as string);
            var connector = StiAzureBlobStorageConnector.Get(connectionString);
            callbackResult["items"] = connector.GetContainerNamesList();
        }

        public static void GetAzureBlobStorageBlobNameItems(Hashtable param, Hashtable callbackResult)
        {
            var connectionString = StiEncodingHelper.DecodeString(param["connectionString"] as string);
            var connector = StiAzureBlobStorageConnector.Get(connectionString);
            callbackResult["items"] = connector.GetBlobNamesList();
        }

        public static void GetAzureBlobContentTypeOrDefault(Hashtable param, Hashtable callbackResult)
        {
            var connectionString = StiEncodingHelper.DecodeString(param["connectionString"] as string);
            var connector = StiAzureBlobStorageConnector.Get(connectionString);
            var blobContentType = connector.GetBlobContentTypeOrDefault();

            if (blobContentType != null)
                callbackResult["blobContentType"] = blobContentType;
        }

        public static void DuplicateDictionaryElement(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            try
            {
                object element = null;
                StiDataType type = StiDataType.Total;

                var itemObject = param["itemObject"] as Hashtable;
                var typeItem = itemObject["typeItem"] as string;
                var itemName = itemObject["name"] as string;
                var itemNameInSource = itemObject["nameInSource"] as string;

                callbackResult["typeItem"] = typeItem;

                if (typeItem == "Category")
                {
                    var newVariable = new StiVariable(GetUniqueName(report, StiDataType.Variable, itemName, param["locCopyOf"] as string, true));
                    newVariable.Key = StiKeyHelper.GenerateKey();
                    report.Dictionary.Variables.Add(newVariable);
                    callbackResult["variables"] = GetVariablesTree(report);
                    callbackResult["name"] = newVariable.Category;
                    return;
                }

                switch (typeItem)
                {
                    case "DataBase":
                        element = report.Dictionary.Databases[itemName];
                        type = StiDataType.Database;
                        break;

                    case "DataSource":
                        element = report.Dictionary.DataSources[itemName];
                        type = StiDataType.DataSource;
                        break;

                    case "Relation":
                        element = report.Dictionary.Relations[itemNameInSource];
                        type = StiDataType.DataRelation;
                        break;

                    case "Variable":
                        element = report.Dictionary.Variables[itemName];
                        type = StiDataType.Variable;
                        break;

                    case "Resource":
                        element = report.Dictionary.Resources[itemName];
                        type = StiDataType.Resource;
                        break;
                }

                if (element != null)
                {
                    var oldName = element as IStiName;
                    var oldAlias = element as IStiAlias;

                    var newElement = (element as ICloneable).Clone();
                    var newName = newElement as IStiName;
                    var newAlias = newElement as IStiAlias;
                    var newCell = newElement as IStiAppCell;
                    newCell.SetKey(StiKeyHelper.GenerateKey());

                    newName.Name = GetUniqueName(report, type, oldName.Name, param["locCopyOf"] as string);
                    newAlias.Alias = oldName.Name == oldAlias.Alias ? newName.Name : oldAlias.Alias;

                    if (type == StiDataType.Variable)
                    {
                        var newVariable = newElement as StiVariable;
                        report.Dictionary.Variables.Add(newElement as StiVariable);
                        callbackResult["itemObject"] = VariableItem(newVariable, report);
                        callbackResult["variables"] = GetVariablesTree(report);
                    }
                    else if (type == StiDataType.DataSource)
                    {
                        var newDataSource = newElement as StiDataSource;
                        report.Dictionary.DataSources.Add(newDataSource);
                        callbackResult["itemObject"] = newElement is StiDataTransformation ? DataTransformationItem(newDataSource as StiDataTransformation) : DatasourceItem(newDataSource);
                        callbackResult["databases"] = GetDataBasesTree(report);
                    }
                    else if (type == StiDataType.DataRelation)
                    {
                        var newRelation = newElement as StiDataRelation;
                        newRelation.NameInSource = GetUniqueName(report, type, newRelation.NameInSource, param["locCopyOf"] as string, false, true);
                        report.Dictionary.Relations.Add(newRelation);
                        callbackResult["itemObject"] = RelationItem(newRelation, new Hashtable());
                        callbackResult["databases"] = GetDataBasesTree(report);
                    }
                    else if (type == StiDataType.Database)
                    {
                        var newDatabase = newElement as StiDatabase;
                        report.Dictionary.Databases.Add(newDatabase);
                        callbackResult["itemObject"] = DatabaseItem(newDatabase, report);
                        callbackResult["databases"] = GetDataBasesTree(report);
                    }
                    else if (type == StiDataType.Resource)
                    {
                        var newResource = newElement as StiResource;
                        report.Dictionary.Resources.Add(newResource);
                        callbackResult["itemObject"] = ResourceItem(newResource, report);
                        callbackResult["resources"] = GetResourcesTree(report);
                    }
                    else
                        throw new NotSupportedException($"The {type} is not supported in the StiActions.Dictionary.InvokeElementDuplicate method!");
                }
            }
            catch (Exception ee)
            {
                StiExceptionProvider.Show(ee);
            }
        }

        public static void SetDictionaryElementProperty(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            object propertyValue = param["propertyValue"];
            var propertyName = StiReportEdit.UpperFirstChar(param["propertyName"] as string);
            var itemObject = param["itemObject"] as Hashtable;
            var currentParentObject = param["currentParentObject"] as Hashtable;
            var typeItem = itemObject["typeItem"] as string;
            var itemName = itemObject["name"] as string;
            var itemNameInSource = itemObject["nameInSource"] as string;

            object dataObject = null;

            switch (typeItem) 
            {
                case "DataBase":
                    dataObject = report.Dictionary.Databases[itemName];
                    break;

                case "DataSource":
                    dataObject = report.Dictionary.DataSources[itemName];
                    break;

                case "BusinessObject":
                    dataObject = report.Dictionary.BusinessObjects[itemName];
                    break;

                case "Relation":
                    dataObject = report.Dictionary.Relations[itemNameInSource];
                    break;

                case "Parameter":
                    var dataSource = report.Dictionary.DataSources[currentParentObject["currentParentName"] as string];
                    if (dataSource != null)
                        dataObject = dataSource.Parameters[itemName];
                    break;

                case "Column":
                    var columns = GetColumnsByTypeAndNameOfObject(report, currentParentObject);
                    if (columns != null)
                        dataObject = columns[itemName];
                    break;

                case "Variable":
                    dataObject = report.Dictionary.Variables[itemName];
                    break;
            }

            if (Convert.ToBoolean(param["isEncodedProperty"]))
                propertyValue = StiEncodingHelper.DecodeString(propertyValue as string);

            if (dataObject != null)
                StiReportEdit.SetPropertyValue(report, propertyName, dataObject, propertyValue);

            callbackResult["dictionary"] = GetDictionaryTree(report);
        }
        #endregion
    }
}
