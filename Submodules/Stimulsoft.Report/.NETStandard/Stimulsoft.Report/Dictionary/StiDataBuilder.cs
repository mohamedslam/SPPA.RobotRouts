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
using System.Collections;
using System.ComponentModel;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Controls;
using Stimulsoft.Data.Functions;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Used to build trees by the data source.
    /// </summary>
    public class StiDataBuilder
    {
        #region Fields
        /// <summary>
        /// Relations being used in the dictionary.
        /// </summary>
        private Hashtable usedRelations;

        /// <summary>
        /// Data sources being used.
        /// </summary>
        private Hashtable usedDataSources;

        /// <summary>
        /// Columns being used.
        /// </summary>
        private Hashtable usedColumns;

        /// <summary>
        /// Data source level.
        /// </summary>
        private int level;

        private StiTreeView treeView;
        #endregion

        #region Properties
        public bool ShowParameters { get; set; }        
        #endregion

        #region Methods.Get
        private static StiReport GetReport(object nodeTag)
        {
            if (nodeTag is StiDataColumn)
                return ((StiDataColumn)nodeTag).DataSource?.Dictionary?.Report;

            if (nodeTag is StiDataSource)
                return ((StiDataSource)nodeTag).Dictionary?.Report;

            if (nodeTag is StiBusinessObject)
                return ((StiBusinessObject)nodeTag).Dictionary?.Report;

            if (nodeTag is StiDataRelation)
                return ((StiDataRelation)nodeTag).Dictionary?.Report;

            return null;
        }

        public static string GetParameterPathFromNode(TreeNode node)
        {
            var parameter = node.Tag as StiDataParameter;
            var dataSource = parameter.DataSource;
            var report = dataSource.Dictionary.Report;

            var dataSourceName = StiNameValidator.CorrectName(dataSource.Name, report);
            return $"{dataSourceName}.Parameters[\"{parameter.Name}\"].ParameterValue";
        }

        /// <summary>
        /// Returns the ColumnPath from TreeNode.
        /// </summary>
        /// <param name="node">TreeNode to build the string view of a column.</param>
        /// <returns>ColumnPath.</returns>
        public static string GetColumnPathFromNode(TreeNode node)
        {
            return GetColumnPathFromNode(node, false);
        }

        /// <summary>
        /// Returns the ColumnPath from TreeNode.
        /// </summary>
        /// <param name="node">TreeNode to build the string view of a column.</param>
        /// <returns>ColumnPath.</returns>
        public static string GetColumnPathFromNode(TreeNode node, bool useRelationName, bool isDataExpression = false)
        {
            var path = string.Empty;
            if (node == null)
                return path;

            if (node.Tag is StiDataColumn)
                path = ((StiDataColumn)node.Tag).Name;

            if (node.Tag is StiDataSource)
                path = ((StiDataSource)node.Tag).Name;

            if (node.Tag is StiBusinessObject)
                path = ((StiBusinessObject)node.Tag).Name;

            if (!isDataExpression)
                path = StiNameValidator.CorrectName(path, GetReport(node.Tag));

            while (true)
            {
                node = node.Parent;
                if (node == null) break;

                var dataRelation = node.Tag as StiDataRelation;
                if (dataRelation != null)
                {
                    var relationName = useRelationName ? dataRelation.NameInSource : dataRelation.Name;
                    if (!isDataExpression)
                        relationName = StiNameValidator.CorrectName(relationName, GetReport(node.Tag));

                    path = $"{relationName}.{path}";
                }

                var dataSource = node.Tag as StiDataSource;
                if (dataSource != null)
                {
                    var dataSourceName = !isDataExpression
                        ? StiNameValidator.CorrectName(dataSource.Name, GetReport(dataSource))
                        : dataSource.Name;

                    path = $"{dataSourceName}.{path}";
                    break;
                }

                var businessObject = node.Tag as StiBusinessObject;
                if (businessObject != null)
                {
                    var businessObjectName = !isDataExpression
                        ? StiNameValidator.CorrectBusinessObjectName(businessObject.GetCorrectFullName(), GetReport(businessObject))
                        : businessObject.GetCorrectFullName();

                    path = $"{businessObjectName}.{path}";
                    break;
                }
            }

            return isDataExpression
                ? Funcs.ToExpression(path)
                : path;
        }

        /// <summary>
        /// Returns the ColumnPath from TreeNode for Interaction.
        /// </summary>
        /// <param name="node">TreeNode to build the string view of a column.</param>
        /// <returns>ColumnPath.</returns>
        public static string GetDataBandColumnPathFromNode(TreeNode node, bool useRelationName)
        {
            var path = string.Empty;

            if (node.Tag is StiDataColumn)
                path = ((StiDataColumn)node.Tag).Name;

            if (node.Tag is StiDataBand)
                path = ((StiDataBand)node.Tag).Name;

            path = StiNameValidator.CorrectName(path, GetReport(node.Tag));

            while (true)
            {
                node = node.Parent;

                var dataRelation = node.Tag as StiDataRelation;
                if (dataRelation != null)
                {
                    var relationName = useRelationName ? dataRelation.NameInSource : dataRelation.Name;
                    relationName = StiNameValidator.CorrectName(relationName, GetReport(dataRelation));
                    path = $"{relationName}.{path}";
                }

                var dataBand = node.Tag as StiDataBand;
                if (dataBand != null)
                {
                    var dataBandName = StiNameValidator.CorrectName(dataBand.Name, GetReport(dataBand));
                    path = $"{dataBandName}.{path}";
                    break;
                }
            }

            return path;
        }

        /// <summary>
        /// Returns the PropertyInfoPath from TreeNode.
        /// </summary>
        /// <param name="node">TreeNode to build the string view of a PropertyInfo.</param>
        /// <returns>PropertyInfoPath.</returns>
        public static string GetPropertyInfoPathFromNode(TreeNode node)
        {
            var path = StiNameValidator.CorrectName(((PropertyInfo)node.Tag).Name, GetReport(node.Tag));

            while (true)
            {
                node = node.Parent;

                var dataColumn = node.Tag as StiDataColumn;
                if (dataColumn != null)
                {
                    var dataColumnName = StiNameValidator.CorrectName(dataColumn.Name, GetReport(dataColumn));
                    path = $"{dataColumnName}.{path}";
                }

                var propertyInfo = node.Tag as PropertyInfo;
                if (propertyInfo != null)
                {
                    var propertyName = StiNameValidator.CorrectName(propertyInfo.Name, GetReport(propertyInfo));
                    path = $"{propertyName}.{path}";
                }

                var dataSource = node.Tag as StiDataSource;
                if (dataSource != null)
                {
                    var dataSourceName = StiNameValidator.CorrectName(dataSource.Name, GetReport(dataSource));
                    path = $"{dataSourceName}.{path}";
                    break;
                }

                var dataRelation = node.Tag as StiDataRelation;
                if (dataRelation != null)
                {
                    var dataRelationName = StiNameValidator.CorrectName(dataRelation.Name, GetReport(dataRelation));
                    path = $"{dataRelationName}.{path}";
                }
            }
            return path;
        }

        /// <summary>
        /// Returns the string view of a column from TreeNode taking into consideration aliases.
        /// </summary>
        /// <param name="node">TreeNode to build the string view of a column.</param>
        /// <returns></returns>
        public static string GetColumnPathFromNodeWithAlias(TreeNode node)
        {
            var path = StiNameValidator.CorrectName(((StiDataColumn)node.Tag).Name, GetReport(node.Tag));

            while (true)
            {
                node = node.Parent;
                if (node == null) break;

                var dataRelation = node.Tag as StiDataRelation;
                if (dataRelation != null)
                {
                    var dataRelationName = StiNameValidator.CorrectName(dataRelation.Name, GetReport(dataRelation));
                    path = $"{dataRelationName}.{path}";
                }

                var dataSource = node.Tag as StiDataSource;
                if (dataSource != null)
                {
                    var dataSourceName = StiNameValidator.CorrectName(dataSource.Name, GetReport(dataSource));
                    path = $"{dataSourceName}.{path}";
                    break;
                }

                var businessObject = node.Tag as StiBusinessObject;
                if (businessObject != null)
                {
                    var businessObjectName = StiNameValidator.CorrectBusinessObjectName(businessObject.GetCorrectFullName(), GetReport(businessObject));
                    path = $"{businessObjectName}.{path}";
                    break;
                }
            }
            return path;
        }

        public static string GetColumnPathFromNodeWithAliasWithoutReplace(TreeNode node)
        {
            var path = ((StiDataColumn)node.Tag).Name;

            while (true)
            {
                node = node.Parent;
                if (node == null) break;

                var dataRelation = node.Tag as StiDataRelation;
                if (dataRelation != null)
                {
                    path = $"{dataRelation.Name}.{path}";
                }

                var dataSource = node.Tag as StiDataSource;
                if (dataSource != null)
                {
                    path = $"{dataSource.Name}.{path}";
                    break;
                }
            }
            return path;
        }
        #endregion

        #region Methods.Helpers
        private bool IsSkipColumn(StiDataColumn column)
        {
            if (column.DataSource is StiBusinessObjectSource)
            {
                if (column.Name.StartsWithInvariant("_ID") ||
                    column.Name.StartsWithInvariant("_parentID") ||
                    column.Name.StartsWithInvariant("_Current")) return true;
            }
            return false;
        }
        #endregion
        
        #region Methods.Build
        /// <summary>
        /// Builds TreeNodeCollection from dictionary.
        /// </summary>
        /// <param name="nodes">Collection TreeNode.</param>
        /// <param name="dictionary">Dictionary data.</param>
        /// <param name="dictionaryDesign">If true then, when rendering, aliases are used.</param>
        /// <param name="includeCalcColumn">If true then, when rendering, CalcDataColumn are included.</param>
        public void Build(TreeNodeCollection nodes, StiDictionary dictionary, bool dictionaryDesign,
            bool includeCalcColumn)
        {
            level = 0;

            #region Build DataSources
            var dataSources = dictionary.DataSources;
            var businessObjects = dictionary.BusinessObjects;

            foreach (StiDataSource data in dataSources)
            {
                if (dictionary.Restrictions.IsAllowShow(data.Name, StiDataType.Variable))
                {
                    var dataNode = new TreeNodeEx();
                    BuildDataSource(data, ref dataNode, includeCalcColumn);
                    nodes.Add(dataNode);
                }
            }

            foreach (StiBusinessObject data in businessObjects)
            {
                if (dictionary.Restrictions.IsAllowShow(data.Name, StiDataType.Variable))
                {
                    var dataNode = new TreeNode();

                    var useAliases = data.Dictionary?.Report?.Designer?.UseAliases;
                    dataNode.Text = data.ToString(StiOptions.Dictionary.ShowOnlyAliasForBusinessObject && useAliases.GetValueOrDefault(false));
                    dataNode.ImageKey = dataNode.SelectedImageKey = "BusinessObject";
                    dataNode.Tag = data;

                    BuildBusinessObject(data, dataNode, true, includeCalcColumn);
                    nodes.Add(dataNode);
                }
            }
            #endregion
        }

        #region Methods.BuildDataSource
        /// <summary>
        /// Builds TreeNode from the Data Source.
        /// </summary>
        /// <param name="dataSource">Data Source for building.</param>
        /// <param name="dataNode">Builded TreeNode.</param>
        public void BuildDataSource(StiDataSource dataSource, ref TreeNodeEx dataNode, bool includeCalcColumn)
        {
            if (dataSource != null)
            {
                var useAliases = dataSource.Dictionary?.Report?.Designer?.UseAliases;
                dataNode.Text = dataSource.ToString(StiOptions.Dictionary.ShowOnlyAliasForDataSource && useAliases.GetValueOrDefault(false));
                dataNode.ImageKey = dataNode.SelectedImageKey = "DataSource";
                dataNode.Tag = dataSource;

                if (usedDataSources != null && usedDataSources[dataSource] != null && treeView != null)
                    treeView.SetBold(dataNode, true);

                var isExpanded = CheckDataSource(dataSource, includeCalcColumn);
                if (isExpanded)
                    dataNode.Nodes.Add(new TreeNode("Loading"));
            }
            else dataNode = null;
        }

        private void ExpandedDataSource(StiDataSource dataSource, TreeNodeEx parentNode, bool includeCalcColumn)
        {
            var datas = new StiDataSourcesCollection(null);
            parentNode.FillFromNodeEx(ref datas, ref this.level);
            ExpandedDataSource(dataSource, parentNode, datas, includeCalcColumn);
        }

        private void ExpandedDataSource(StiDataSource dataSource, TreeNode parentNode,
            StiDataSourcesCollection datas, bool includeCalcColumn)
        {
            level++;
            try
            {
                if (StiOptions.Designer.MaxLevelOfDictionaryObjects > 0 && StiOptions.Designer.MaxLevelOfDictionaryObjects < level) return;

                if (!datas.Contains(dataSource))
                {
                    datas.Add(dataSource);
                    BuildRelations(dataSource.GetParentRelations(), parentNode, datas, includeCalcColumn);
                    datas.Remove(dataSource);
                }

                BuildColumns(dataSource, dataSource.Columns, parentNode, includeCalcColumn);

                var sqlDataSource = dataSource as StiSqlSource;
                if (sqlDataSource != null && sqlDataSource.Parameters.Count > 0)
                {
                    BuildParameters(sqlDataSource, parentNode);
                }
            }
            finally
            {
                level--;
            }
        }
        #endregion

        #region BuildRelations
        /// <summary>
        /// Builds TreeNode fom the collection of relations.
        /// </summary>
        /// <param name="relations">Collection of relations.</param>
        /// <param name="parentNode">TreeNode in which TreeNode obtained will be added.</param>
        /// <param name="datas">Data Source collection.</param>
        private void BuildRelations(StiDataRelationsCollection relations, TreeNode parentNode,
            StiDataSourcesCollection datas, bool includeCalcColumn)
        {
            if (relations.Count <= 0)return;
            
            foreach (StiDataRelation relation in relations)
            {
                if (relation.Dictionary == null || relation.Dictionary.Restrictions.IsAllowShow(relation.Name, StiDataType.DataRelation))
                {
                    var useAliases = relation.Dictionary?.Report?.Designer?.UseAliases;
                    var text = relation.ToString(StiOptions.Dictionary.ShowOnlyAliasForDataRelation && useAliases.GetValueOrDefault(false));

                    var relationNode = new TreeNodeEx(text);
                    relationNode.FillNodeEx(datas, this.level);
                    relationNode.Tag = relation;
                    relationNode.ImageKey = relationNode.SelectedImageKey = relation.Inherited ? "LockedDataRelation" : "DataRelation";

                    if (relation.Active)
                        relationNode.ImageKey = relationNode.SelectedImageKey += "Active";

                    if (usedRelations != null && usedRelations[relation] != null && treeView != null)
                        treeView.SetBold(relationNode, true);

                    if (relation.ParentSource != null)
                    {
                        bool isExpanded = CheckDataSource(relation.ParentSource, includeCalcColumn);

                        if (isExpanded)
                        {
                            relationNode.Nodes.Add(new TreeNode("Loading"));
                        }
                    }

                    parentNode.Nodes.Add(relationNode);
                }
            }
        }

        private void ExpandedRelations(TreeNodeEx parentNode, bool includeCalcColumn)
        {
            var relation = parentNode.Tag as StiDataRelation;
            var dataSource = relation.ParentSource;

            if (dataSource != null)
            {
                var datas = new StiDataSourcesCollection(null);
                parentNode.FillFromNodeEx(ref datas, ref this.level);

                ExpandedDataSource(relation.ParentSource, parentNode, datas, includeCalcColumn);
            }
        }
        #endregion

        #region BusinessObjects
        public void BuildBusinessObject(StiBusinessObject businessObject, ref TreeNodeEx dataNode, bool includeCalcColumn)
        {
            if (businessObject != null)
            {
                var useAliases = businessObject.Dictionary?.Report?.Designer?.UseAliases;
                dataNode.Text = businessObject.ToString(StiOptions.Dictionary.ShowOnlyAliasForBusinessObject && useAliases.GetValueOrDefault(false));
                dataNode.ImageKey = dataNode.SelectedImageKey = "BusinessObject";
                dataNode.Tag = businessObject;

                if (usedDataSources != null && usedDataSources[businessObject] != null)
                {
                    if (treeView != null) 
                        treeView.SetBold(dataNode, true);
                }

                bool isExpanded = CheckBusinessObject(businessObject, includeCalcColumn, true);
                if (isExpanded)
                {
                    dataNode.Nodes.Add(new TreeNode("Loading"));
                }
            }
            else 
                dataNode = null;
        }

        public void BuildBusinessObjects(StiBusinessObjectsCollection objects, TreeNode parentNode, bool includeColumns, bool includeCalcColumns)
        {
            BuildBusinessObjects(objects, parentNode, includeColumns, includeCalcColumns, true);
        }

        private void BuildBusinessObjects(StiBusinessObjectsCollection objects, TreeNode parentNode, bool includeColumns, bool includeCalcColumns, bool showCategories)
        {
            foreach (StiBusinessObject obj in objects)
            {
                if (obj.Dictionary == null || obj.Dictionary.Restrictions.IsAllowShow(obj.Name, StiDataType.BusinessObject))
                {
                    var useAliases = obj.Dictionary?.Report?.Designer?.UseAliases;
                    var text = obj.ToString(StiOptions.Dictionary.ShowOnlyAliasForBusinessObject && useAliases.GetValueOrDefault(false));

                    var objNode = new TreeNodeEx(text);
                    objNode.ImageKey = objNode.SelectedImageKey =
                        obj.Inherited ? "LockedBusinessObject" : "BusinessObject";

                    objNode.Tag = obj;

                    if (showCategories)
                    {
                        TreeNode categoryNode = null;
                        if (!string.IsNullOrEmpty(obj.Category))
                        {
                            #region Search Category Node
                            foreach (TreeNode node in parentNode.Nodes)
                            {
                                if (node.Text == obj.Category)
                                {
                                    categoryNode = node;
                                    break;
                                }
                            }
                            #endregion

                            #region Create category node
                            if (categoryNode == null)
                            {
                                categoryNode = new TreeNode(obj.Category)
                                {
                                    Tag = new StiBusinessObjectCategory(obj.Category),
                                    ImageKey = "Category",
                                    SelectedImageKey = "Category"
                                };
                                parentNode.Nodes.Add(categoryNode);
                            }
                            #endregion

                            categoryNode.Nodes.Add(objNode);

                        }
                        else parentNode.Nodes.Add(objNode);
                    }
                    else
                    {
                        parentNode.Nodes.Add(objNode);
                    }

                    if (CheckBusinessObject(obj, includeColumns, includeCalcColumns))
                    {
                        objNode.Nodes.Add(new TreeNode("Loading"));
                    }
                }
            }
        }

        private void ExpandedBusinessObject(StiBusinessObject businessObject, TreeNode parentNode, bool includeColumns, bool includeCalcColumns)
        {
            BuildBusinessObject(businessObject, parentNode, includeColumns, includeCalcColumns);
        }

        /// <summary>
        /// Builds TreeNode from the Business Object.
        /// </summary>
        public void BuildBusinessObject(StiBusinessObject businessObject, TreeNode parentNode, bool includeColumns, bool includeCalcColumns)
        {
            BuildBusinessObjects(businessObject.BusinessObjects, parentNode, includeColumns, includeCalcColumns, false);
            if (includeColumns)
                BuildColumns(businessObject, businessObject.Columns, parentNode, includeCalcColumns);
        }
        #endregion

        /// <summary>
        /// Build TreeNode from the column collection.
        /// </summary>
        /// <param name="dataSource">Data source.</param>
        /// <param name="columns">Column collection.</param>
        /// <param name="parentNode">TreeNode in which TreeNode obtained will be added.</param>
        private void BuildColumns(StiDataSource dataSource, StiDataColumnsCollection columns,
            TreeNode parentNode, bool includeCalcColumn)
        {
            if (dataSource == null || dataSource.Dictionary == null) return;

            StiDictionary dictionary = dataSource.Dictionary;

            foreach (StiDataColumn column in columns)
            {
                if (IsSkipColumn(column)) continue;

                if (column.DataSource == null ||
                    dictionary.Restrictions.IsAllowShow(column.DataSource.Name + "." + column.Name, StiDataType.DataColumn))
                {
                    if (!(column is StiCalcDataColumn) || (includeCalcColumn))
                    {
                        var useAliases = dictionary?.Report?.Designer?.UseAliases;
                        var text = column.ToString(StiOptions.Dictionary.ShowOnlyAliasForDataColumn && useAliases.GetValueOrDefault(false));
                        var imageKey = StiDataImages.GetImageKeyFromColumn(column);
                        var columnNode = new TreeNode
                        {
                            Text = text,
                            ImageKey = imageKey,
                            SelectedImageKey = imageKey,
                            Tag = column
                        };

                        BuildType(column.Type, imageKey, columnNode);

                        parentNode.Nodes.Add(columnNode);
                        if (usedColumns != null && usedColumns[column] != null)
                        {
                            if (treeView != null) 
                                treeView.SetBold(columnNode, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Build TreeNode from the column collection.
        /// </summary>
        private void BuildColumns(StiBusinessObject businessObject, StiDataColumnsCollection columns,
            TreeNode parentNode, bool includeCalcColumn)
        {
            if (businessObject == null || businessObject.Dictionary == null) return;

            foreach (StiDataColumn column in columns)
            {
                if (IsSkipColumn(column)) continue;

                if (column.BusinessObject == null ||
                    businessObject.Dictionary.Restrictions.IsAllowShow(column.BusinessObject.Name + "." + column.Name, StiDataType.DataColumn))
                {
                    if (!(column is StiCalcDataColumn) || (includeCalcColumn))
                    {
                        var useAliases = businessObject?.Dictionary?.Report?.Designer?.UseAliases;
                        var text = column.ToString(StiOptions.Dictionary.ShowOnlyAliasForDataColumn && useAliases.GetValueOrDefault(false));
                        var nameColumn = text;
                        var imageKey = StiDataImages.GetImageKeyFromColumn(column);

                        TreeNode columnNode = new TreeNode(nameColumn);
                        columnNode.ImageKey = columnNode.SelectedImageKey = imageKey;
                        columnNode.Tag = column;

                        Type type = column.Type;
                        BuildType(type, imageKey, columnNode);

                        parentNode.Nodes.Add(columnNode);

                        if (usedColumns != null && usedColumns[column] != null)
                        {
                            if (treeView != null) 
                                treeView.SetBold(columnNode, true);
                        }
                    }
                }
            }
        }

        private void BuildParameters(StiSqlSource dataSource, TreeNode parentNode)
        {
            if (!ShowParameters) return;

            var parametersNode = new TreeNode(StiLocalization.Get("PropertyMain", "Parameters"));
            parametersNode.ImageKey = parametersNode.SelectedImageKey = "Parameter";

            if (dataSource != null && dataSource.Dictionary != null)
            {
                var parameters = dataSource.Parameters;
                foreach (StiDataParameter parameter in parameters)
                {
                    if (parameter.DataSource != null)
                    {
                        var nameParameter = parameter.ToString();

                        var parameterNode = new TreeNode(nameParameter);
                        parameterNode.ImageKey =
                            parameterNode.SelectedImageKey =
                            dataSource.Inherited ? "LockedParameter" : "Parameter";

                        parameterNode.Tag = parameter;
                        parametersNode.Nodes.Add(parameterNode);
                    }
                }
            }

            parentNode.Nodes.Add(parametersNode);
        }

        private void BuildType(Type propertyType, string imageKey, TreeNode parentNode)
        {
            int maxLevel = 5;
            if (StiTypeFinder.FindInterface(propertyType, typeof(IComponent))) 
                maxLevel = 2;

            level++;

            if (level < maxLevel && propertyType != typeof(Image) &&
                propertyType != typeof(string) &&
                (propertyType.IsClass || propertyType.IsInterface) &&
                (!StiTypeFinder.FindInterface(propertyType, typeof(IEnumerable))))
            {
                #region Process Properties
                if (StiOptions.Dictionary.BusinessObjects.AllowUseProperties)
                {
                    var props = propertyType.GetProperties();

                    foreach (PropertyInfo prop in props)
                    {
                        var attrs = prop.GetCustomAttributes(typeof(BrowsableAttribute), true);
                        if (attrs != null && attrs.Length > 0 && (!((BrowsableAttribute)attrs[0]).Browsable) &&
                            StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType == StiPropertiesProcessingType.Browsable) continue;

                        var columnNode = new TreeNode(prop.Name);
                        columnNode.ImageKey = columnNode.SelectedImageKey = imageKey;
                        columnNode.Tag = prop;

                        BuildType(prop.PropertyType, imageKey, columnNode);

                        parentNode.Nodes.Add(columnNode);
                    }
                }
                #endregion

                #region Process Fields
                if (StiOptions.Dictionary.BusinessObjects.AllowUseFields)
                {
                    var fields = propertyType.GetFields();

                    foreach (FieldInfo field in fields)
                    {
                        var attrs = field.GetCustomAttributes(typeof(BrowsableAttribute), true);
                        if (attrs != null && attrs.Length > 0 && (!((BrowsableAttribute)attrs[0]).Browsable) &&
                            StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType == StiPropertiesProcessingType.Browsable) continue;

                        var columnNode = new TreeNode(field.Name);
                        columnNode.ImageKey = columnNode.SelectedImageKey = imageKey;
                        columnNode.Tag = field;

                        BuildType(field.FieldType, imageKey, columnNode);

                        parentNode.Nodes.Add(columnNode);
                    }
                }
                #endregion
            }

            level--;
        }
        #endregion

        #region Methods.Check
        private bool CheckDataSource(StiDataSource dataSource, bool includeCalcColumn)
        {
            var isExpanded = CheckRelations(dataSource);
            if (!isExpanded)
                isExpanded = CheckColumns(dataSource, includeCalcColumn);

            if (!isExpanded)
            {
                var sqlDataSource = dataSource as StiSqlSource;
                if (sqlDataSource != null && sqlDataSource.Parameters.Count > 0)
                    isExpanded = CheckParameters(sqlDataSource);
            }

            return isExpanded;
        }

        public bool CheckBusinessObject(StiBusinessObject businessObject, bool includeColumns, bool includeCalcColumns)
        {
            var isExpanded = CheckBusinessObjects(businessObject.BusinessObjects);
            if (!isExpanded && includeColumns)
                isExpanded = CheckColumns(businessObject, includeCalcColumns);

            return isExpanded;
        }

        private bool CheckBusinessObjects(StiBusinessObjectsCollection objects)
        {
            foreach (StiBusinessObject obj in objects)
            {
                if (obj.Dictionary == null || obj.Dictionary.Restrictions.IsAllowShow(obj.Name, StiDataType.BusinessObject))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckColumns(StiBusinessObject businessObject, bool includeCalcColumn)
        {
            if (businessObject == null || businessObject.Dictionary == null) 
                return false;

            var dictionary = businessObject.Dictionary;
            var columns = businessObject.Columns;
            foreach (StiDataColumn column in columns)
            {
                if (IsSkipColumn(column)) continue;

                if (column.DataSource == null || dictionary.Restrictions.IsAllowShow(column.BusinessObject.Name + "." + column.Name, StiDataType.DataColumn))
                {
                    if (!(column is StiCalcDataColumn) || includeCalcColumn)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckRelations(StiDataSource dataSource)
        {
            var dictionary = dataSource.Dictionary;
            if (dictionary == null)            
                return false;
            
            foreach (StiDataRelation relation in dictionary.Relations)
            {
                if (relation.ChildSource == dataSource && dictionary.Restrictions.IsAllowShow(relation.Name, StiDataType.DataRelation))
                    return true;
            }

            return false;
        }

        private bool CheckColumns(StiDataSource dataSource, bool includeCalcColumn)
        {
            if (dataSource == null || dataSource.Dictionary == null) 
                return false;

            var dictionary = dataSource.Dictionary;
            var columns = dataSource.Columns;

            foreach (StiDataColumn column in columns)
            {
                if (IsSkipColumn(column)) continue;

                if (column.DataSource == null || dictionary.Restrictions.IsAllowShow(column.DataSource.Name + "." + column.Name, StiDataType.DataColumn))
                {
                    if (!(column is StiCalcDataColumn) || (includeCalcColumn))
                        return true;
                }
            }

            return false;
        }

        private bool CheckParameters(StiSqlSource dataSource)
        {
            if (!ShowParameters || dataSource == null || dataSource.Dictionary == null) 
                return false;

            var parameters = dataSource.Parameters;
            foreach (StiDataParameter parameter in parameters)
            {
                if (parameter.DataSource != null)
                    return true;
            }

            return false;
        }
        #endregion

        #region Methods.Expanded&Collapsed
        public void ExpandedNode(TreeNodeEx node, bool includeCalcColumn)
        {
            object tag = node.Tag;
            ExpandedNode(node, includeCalcColumn, tag);
        }

        public void ExpandedNode(TreeNodeEx node, bool includeColumn, bool includeCalcColumn)
        {
            object tag = node.Tag;
            ExpandedNode(node, includeColumn, includeCalcColumn, tag);
        }

        public void ExpandedNode(TreeNodeEx node, bool includeCalcColumn, object tag)
        {
            ExpandedNode(node, true, includeCalcColumn, tag);
        }

        public void ExpandedNode(TreeNodeEx node, bool includeColumn, bool includeCalcColumn, object tag)
        {
            if (tag != null && node.Nodes.Count == 1 && node.Nodes[0].Text == "Loading")
            {
                node.Nodes.Clear();

                if (tag is StiDataSource)
                    ExpandedDataSource(tag as StiDataSource, node, includeCalcColumn);

                else if (tag is StiDataRelation)
                    ExpandedRelations(node, includeCalcColumn);

                else if (tag is StiBusinessObject)
                    ExpandedBusinessObject(tag as StiBusinessObject, node, includeColumn, includeCalcColumn);
            }
        }

        public void CollapsedNode(TreeNode node)
        {
            if (node is TreeNodeEx)
            {
                var tag = node.Tag;
                if (tag != null && node.Nodes.Count == 1 && node.Nodes[0].Text == "Loading")
                {
                    node.Nodes.Clear();

                    if (tag is StiDataSource || tag is StiDataRelation || tag is StiBusinessObject)
                        node.Nodes.Add(new TreeNode("Loading"));
                }
            }
        }
        #endregion

        /// <summary>
        /// Creates a new object of the type StiDataBuilder.
        /// </summary>
        public StiDataBuilder()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataBuilder.
        /// </summary>
        public StiDataBuilder(StiTreeView treeView)
            : this(null, null, null, treeView)
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataBuilder.
        /// </summary>
        /// <param name="usedRelations">Relations being used in the dictionary.</param>
        /// <param name="usedDataSources">Data sources being used.</param>
        /// <param name="usedColumns">Columns being used.</param>
        public StiDataBuilder(Hashtable usedRelations, Hashtable usedDataSources,
            Hashtable usedColumns, StiTreeView treeView)
        {
            this.usedRelations = usedRelations;
            this.usedDataSources = usedDataSources;
            this.usedColumns = usedColumns;
            this.treeView = treeView;
        }
    }
}