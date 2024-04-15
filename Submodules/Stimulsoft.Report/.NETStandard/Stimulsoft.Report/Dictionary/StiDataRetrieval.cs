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
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// The service allows to find all data sources, columns, relations, which are used in the report.
	/// </summary>
	public class StiDataRetrieval
	{
        #region class StiDictionaryBuilder
        /// <summary>
        /// The class contains methods which allow to create a list dictionary of data names.
        /// </summary>
        private class StiDictionaryBuilder
		{
		    /// <summary>
			/// Processes a collection of columns.
			/// </summary>
			/// <param name="elements">Hashtable for filling.</param>
			/// <param name="columns">Collection of columns for building.</param>
			/// <param name="name">Parent name.</param>
			private void BuildColumns(Hashtable elements, StiDataColumnsCollection columns, string name)
			{
				foreach (StiDataColumn column in columns)
				{
					var elStr = $"{name}.{StiNameValidator.CorrectName(column.Name, columns.DataSource?.Dictionary?.Report)}";
					elements[elStr] = column;
				}
			}

			/// <summary>
			/// Processes a collection of relations.
			/// </summary>
			/// <param name="elements">Hashtable for filling.</param>
			/// <param name="relations">Collection of relations.</param>
			/// <param name="datas">Table with have already been built Data Sources.</param>
			/// <param name="name">Parent name.</param>
			private void BuildRelations(Hashtable elements, StiDataRelationsCollection relations, 
				StiDataSourcesCollection datas, string name)
			{
				if (relations.Count > 0)
				{
					foreach (StiDataRelation relation in relations)
					{
						var elStr = $"{name}.{StiNameValidator.CorrectName(relation.Name, relation.Dictionary?.Report)}";
						elements[elStr] = relation;
						BuildData(elements, relation.ParentSource, datas, elStr, true);

						if (relation.Name != relation.NameInSource)
						{
							elStr = $"{name}.{StiNameValidator.CorrectName(relation.NameInSource, relation.Dictionary?.Report)}";
					
							elements[elStr] = relation;
							elements[StiNameValidator.CorrectName(relation.NameInSource, relation.Dictionary?.Report)] = relation;
						
							BuildData(elements, relation.ParentSource, datas, elStr, true);
						}
					}
				}
			}
            
			/// <summary>
			/// Processes a collection of Data Sources.
			/// </summary>
			/// <param name="elements">Hashtable for filling.</param>
			/// <param name="dataSource">Data source.</param>
			/// <param name="datas">Table with have already been built Data Sources.</param>
			/// <param name="name">Parent name.</param>
			/// <param name="skipName">Do not add Parent name.</param>
			private void BuildData(Hashtable elements, StiDataSource dataSource, 
				StiDataSourcesCollection datas, string name, bool skipName)
			{
				var nm = string.Empty;
			    if (name != string.Empty)
			        nm = $"{name}.{StiNameValidator.CorrectName(dataSource.Name, dataSource.Dictionary?.Report)}";

			    else if (name == string.Empty)
			        nm = StiNameValidator.CorrectName(dataSource.Name, dataSource.Dictionary?.Report);

			    if (!datas.Contains(dataSource))
				{
					datas.Add(dataSource);

				    if (skipName)
				        BuildRelations(elements, dataSource.GetParentRelations(), datas, name);
				    else
				        BuildRelations(elements, dataSource.GetParentRelations(), datas, nm);

				    datas.Remove(dataSource);
				}

			    if (skipName)
			        BuildColumns(elements, dataSource.Columns, name);
			    else
			        BuildColumns(elements, dataSource.Columns, nm);
			}

			/// <summary>
			/// Processes a dictionary.
			/// </summary>
			/// <param name="elements">Hashtable for filling.</param>
			/// <param name="dictionary">Dictionary.</param>
			public void Build(Hashtable elements, StiDictionary dictionary)
			{
				var dataSources = dictionary.DataSources;
				var datas = new StiDataSourcesCollection(null);

				foreach (StiDataSource data in dataSources)
				{
					var dataStr = StiNameValidator.CorrectName(data.Name, dictionary?.Report);
					elements[dataStr] = data;
					BuildData(elements, data, datas, "", false);
				}
			}
		}
        #endregion

        #region enum StiState
        /// <summary>
        /// The state - used in BuildTokens.
        /// </summary>
        private enum StiState 
		{
			/// <summary>
			/// Read Lexem.
			/// </summary>
			Lexem, 
			/// <summary>
			/// Find Lexem.
			/// </summary>
			Find,
		}
        #endregion

        #region Methods
        public void Dispose()
	    {
	        UsedColumns.Clear();
	        UsedColumns = null;

	        UsedRelations.Clear();
	        UsedRelations = null;

	        UsedDataSources.Clear();
	        UsedDataSources = null;
	    }

        /// <summary>
        /// Parses script on tokens.
        /// </summary>
        /// <param name="script">Script to parse.</param>
        /// <returns>Hashtable that contains tokens.</returns>
        private Hashtable BuildTokens(string script)
		{
			var scriptTokens = new Hashtable();
			var pos = 0;
			var st = StiState.Find;
			var lex = "";
			while (pos < script.Length)
			{
				var c = script[pos];

				#region Find
				if (st == StiState.Find)
				{
					if (Char.IsLetter(c) || c == '_')
					{
						st = StiState.Lexem;
						lex += c;
					}

					if (c == '"')
					{
                        var ps = script.IndexOf('"', pos + 1);
						if (ps != -1)
						{
							var s = script.Substring(pos + 1, ps - pos - 1);
							scriptTokens[s] = s;
							pos = ps + 1;
						}						
					}
				}
				#endregion

				#region Lexem
				else if (st == StiState.Lexem)
				{
					if (Char.IsLetterOrDigit(c) || c == '_' || c == '.')
					{
						st = StiState.Lexem;
						lex += c;
					}
					else 
					{
						if (lex != "")
						{
							while (pos < script.Length)
							{
								if (script[pos] == '.' || script[pos] != ' ')break;
								pos++;
							}
							if (pos < script.Length && script[pos] == '.')
							{
								lex += '.';
								while (pos < script.Length)
								{
									if (script[pos] != ' ')break;
									pos++;
								}
							}
							else
							{
								pos--;
								scriptTokens[lex] = lex;
								lex = "";
								st = StiState.Find;
							}
						}
					}
				}
				#endregion

				pos++;
			}

		    if (st == StiState.Lexem)
		        scriptTokens[lex] = lex;

		    return scriptTokens;
		}

        /// <summary>
        /// Builds hashtables with elements being used.
        /// </summary>
        public virtual void Retrieval(StiReport report)
        {
            if (report == null) return;

            var serializator = new StiCodeDomSerializator();

            var reservScript = report.Script;
            var text = serializator.Serialize(report, report.GetReportName(), report.Language, false, false);
            report.Script = StiLanguage.ReplaceGeneratedCode(report.Script, text, report.Language);

            var scriptTokens = BuildTokens(report.Script);
            report.Script = reservScript;

            var dictBuilder = new StiDictionaryBuilder();

            #region Get all elements of the columniated dictionary and relation in the manner of lines
            var usedElements = new Hashtable();
            dictBuilder.Build(usedElements, report.Dictionary);
            #endregion

            #region Search for elements which meet in script
            foreach (string str in usedElements.Keys)
            {
                if (scriptTokens[str] == null) continue;

                var value = usedElements[str];
                if (value is StiDataSource && UsedDataSources[value] == null)
                    UsedDataSources[value] = value;

                else if (value is StiDataColumn && UsedColumns[value] == null)
                    UsedColumns[value] = value;

                else if (value is StiDataRelation && UsedRelations[value] == null)
                    UsedRelations[value] = value;

                #region If there are points in line, that executes additional check
                var strs = str.Split('.');
                if (strs.Length <= 0) continue;

                foreach (var dataStr in strs)
                {
                    var data = usedElements[dataStr];
                    if (data == null) continue;

                    if (data is StiDataSource && UsedDataSources[data] == null)
                        UsedDataSources[data] = data;

                    else if (data is StiDataColumn && UsedColumns[data] == null)
                        UsedColumns[data] = data;

                    else if (data is StiDataRelation && UsedRelations[data] == null)
                        UsedRelations[data] = data;
                }
                #endregion
            }
            #endregion

            #region Scan components on presence of the sources data, relations, sorting
            var comps = report.GetComponents();
            foreach (StiComponent comp in comps)
            {
                #region IStiDataSource
                var dataSource = comp as IStiDataSource;
                if (dataSource != null && dataSource.DataSource != null)
                    UsedDataSources[dataSource.DataSource] = dataSource.DataSource;
                #endregion

                #region IStiDataRelation
                var dataRelation = comp as IStiDataRelation;
                if (dataRelation != null && dataRelation.DataRelation != null)
                    UsedRelations[dataRelation.DataRelation] = dataRelation.DataRelation;
                #endregion

                #region IStiSort
                var sort = comp as IStiSort;
                if (sort != null && sort.Sort != null)
                {
                    var data = string.Empty;
                    var ds = comp as IStiDataSource;
                    if (ds != null && ds.DataSource != null)
                        data = StiNameValidator.CorrectName(ds.DataSource.Name, report) + '.';

                    var index = 1;
                    while (index < sort.Sort.Length)
                    {
                        var columnName = string.Empty;

                        while (
                            index < sort.Sort.Length &&
                            sort.Sort[index] != "ASC" &&
                            sort.Sort[index] != "DESC")
                        {
                            if (columnName.Length == 0)
                                columnName = sort.Sort[index];
                            else
                                columnName += '.' + sort.Sort[index];
                            index++;
                        }

                        index++;

                        columnName = StiNameValidator.CorrectName(columnName, report);
                        var column = usedElements[data + columnName] as StiDataColumn;
                        if (column != null)
                            UsedColumns[column] = column;
                    }
                }
                #endregion
            }
            #endregion

            #region Add Data Sources, columns which are used
            var columns = UsedColumns.Values;
            foreach (StiDataColumn column in columns)
            {
                if (column.DataSource != null && !UsedDataSources.Contains(column.DataSource))
                    UsedDataSources[column.DataSource] = column.DataSource;
            }
            #endregion

            #region Add all Data Sources relation which meet in used relations
            var relations = UsedRelations.Values;
            foreach (StiDataRelation dataRelation in relations)
            {
                UsedDataSources[dataRelation.ParentSource] = dataRelation.ParentSource;
                UsedDataSources[dataRelation.ChildSource] = dataRelation.ChildSource;
            }
            #endregion

            #region Add all relations which meet in usedDataSource
            var datasources = UsedDataSources.Values;
            foreach (StiDataSource dataSource in datasources)
            {
                var rels = dataSource.GetParentRelations();
                foreach (StiDataRelation relation in rels)
                {
                    //Use Parent Data Source
                    if (UsedDataSources[relation.ParentSource] != null)
                    {
                        //If relation not found, that adds relation
                        UsedRelations[relation] = relation;
                    }
                }

                rels = dataSource.GetChildRelations();
                foreach (StiDataRelation relation in rels)
                {
                    //Use Child Data Source
                    if (UsedDataSources[relation.ChildSource] != null)
                    {
                        //If relation not found, that adds relation
                        UsedRelations[relation] = relation;
                    }
                }
            }
            #endregion

            #region Add columnID in relations
            relations = UsedRelations.Values;
            foreach (StiDataRelation dataRelation in relations)
            {
                if (dataRelation.ParentSource != null)
                {
                    foreach (var columnName in dataRelation.ParentColumns)
                    {
                        var column = dataRelation.ParentSource.Columns[columnName];
                        if (column != null)
                            UsedColumns[column] = column;
                    }
                }

                if (dataRelation.ChildSource != null)
                {
                    foreach (var columnName in dataRelation.ChildColumns)
                    {
                        var column = dataRelation.ChildSource.Columns[columnName];
                        if (column != null)
                            UsedColumns[column] = column;
                    }
                }
            }
            #endregion
        }

        #endregion

        #region Properties
	    /// <summary>
		/// Gets columns to use.
		/// </summary>
		public virtual Hashtable UsedColumns { get; private set; } = new Hashtable();

	    /// <summary>
		/// Gets relations to use.
		/// </summary>
		public virtual Hashtable UsedRelations { get; private set; } = new Hashtable();

	    /// <summary>
		/// Gets Data Sources used.
		/// </summary>
		public virtual Hashtable UsedDataSources { get; private set; } = new Hashtable();
	    #endregion
	}
}
