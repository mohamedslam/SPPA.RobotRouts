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
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report
{
	internal static class StiInheritedReportComparer
	{        		
		internal static void Compare(StiReport report, StiReport masterReport)
		{
			CompareComponents(report, masterReport);
			CompareVariables(report, masterReport);
			CompareDataSources(report, masterReport);
			CompareRelations(report, masterReport);
            CompareBusinessObjects(report, masterReport);
            CompareDatabases(report, masterReport);
			CompareEvents(report, masterReport);			
		}

	
		private static void CompareComponents(StiReport report, StiReport masterReport)
		{
			StiComponentsCollection comps = report.GetComponents();
			StiComponentsCollection compsMaster = masterReport.GetComponents();
				
			Hashtable compsHash = new Hashtable();
			Hashtable compsMasterHash = new Hashtable();

			foreach (StiComponent compMaster in compsMaster)
			{
				compsMasterHash[compMaster.Name] = compMaster;
			}

			#region Check existing components
			foreach (StiComponent comp in comps)
			{
				#region Skip pages
				//if (comp is StiPage)
				//{
				//	compsHash[comp.Name] = comp;
				//	continue;
				//}
				#endregion

				if (comp.Inherited || comp is StiPage)
				{
					StiComponent compMaster = compsMasterHash[comp.Name] as StiComponent;

					#region If new comp does not exist in master report
					if (compMaster == null)
					{
						if (!(comp is StiPage))comp.Parent.Components.Remove(comp);
						else if (((StiPage)comp).Components.Count == 0)report.Pages.Remove(comp as StiPage);
					}
					#endregion

					#region Replace comp in report from master report
					else
					{
						StiComponent newComp = null;
						StiContainer contMaster = compMaster as StiContainer;

						if (contMaster != null)newComp = contMaster.Clone(true, false) as StiComponent;
						else newComp = compMaster.Clone(true) as StiComponent;

						compsHash[comp.Name] = newComp;

						if (comp is StiPage)
						{
							int index = report.Pages.IndexOf(comp as StiPage);
							report.Pages.RemoveAt(index);
							report.Pages.Insert(index, newComp as StiPage);
						}
						else
						{
							int index = comp.Parent.Components.IndexOf(comp);
							comp.Parent.Components.RemoveAt(index);
							comp.Parent.Components.Insert(index, newComp);
						}

						StiContainer cont = comp as StiContainer;

						if (cont != null)
						{
							foreach (StiComponent component in cont.Components)
							{
								((StiContainer)newComp).Components.Add(component);
							}
						}
					}
					#endregion
				}
			}
			#endregion
				
			#region Add components
			foreach (StiComponent comp in compsMaster)
			{
				if (compsHash[comp.Name] == null)
				{
					StiComponent newComp = null;
						
					StiContainer cont = comp as StiContainer;
							 
					if (cont != null)newComp = cont.Clone(true, false) as StiComponent;
					else newComp = comp.Clone(true) as StiComponent;

					if (comp is StiPage)
					{
						report.Pages.Add(newComp as StiPage);
					}
					else
					{
						StiContainer contParent = compsHash[comp.Parent.Name] as StiContainer;
                        int index = GetPositionForComponent(comp, contParent);
						if (index >= 0 && index < contParent.Components.Count)
						{
                            contParent.Components.Insert(index, newComp);
						}
						else contParent.Components.Add(newComp);
					}

					compsHash[newComp.Name] = newComp;
				}
			}
			#endregion

            #region If we have master components then fill it with new component reference
            comps = report.GetComponents();

            foreach (StiComponent comp in comps)
            {
                compsHash[comp.Name] = comp;
            }

            #region Check Components for IStiMasterComponent
            foreach (StiComponent comp in comps)
            {
                #region Create collection of MasterComponent
                IStiMasterComponent masterComponent = comp as IStiMasterComponent;
                if (masterComponent != null && masterComponent.MasterComponent != null)
                {
                    string nameOfMasterComponent = masterComponent.MasterComponent.Name;
                    StiComponent compParent = compsHash[nameOfMasterComponent] as StiComponent;
                    if (compParent != null)
                    {
                        masterComponent.MasterComponent = compParent;
                    }
                }
                #endregion
            }
            #endregion
            #endregion

            foreach (StiPage page in report.Pages)
			{
				page.Report = report;
				SetParent(page, page, page);
				page.SortByPriority();
			}
		}

        private static int GetPositionForComponent(StiComponent comp, StiContainer contParent)
        {
            StiContainer compParent = comp.Parent;

            int index = compParent.Components.IndexOf(comp);
            while (index > 0)
            {
                StiComponent compEx = compParent.Components[index - 1];
                int newIndex = contParent.Components.IndexOf(compEx.Name);
                if (newIndex != -1)
                {
                    return newIndex + 1;
                }
                index--;
            }

            index = compParent.Components.IndexOf(comp);
            while (index < (compParent.Components.Count - 1))
            {
                StiComponent compEx = compParent.Components[index + 1];
                int newIndex = contParent.Components.IndexOf(compEx.Name);
                if (newIndex != -1)
                {
                    return newIndex;
                }
                index++;
            }

            return 0;
            
        }

		private static void SetParent(StiComponent component, StiPage page, StiContainer parent)
		{
			component.Page = page;

			parent = component as StiContainer;
			if (parent != null)
			{
				foreach (StiComponent comp in parent.Components)
				{
                    SetParent(comp, page, parent);			
				}
			}
		}

		
		private static void CompareVariables(StiReport report, StiReport masterReport)
		{
			StiVariablesCollection variables = new StiVariablesCollection();
			foreach (StiVariable variable in report.Dictionary.Variables)
			{
				variables.Add(variable);
			}

			Hashtable variablesHash = new Hashtable();
			Hashtable variablesMasterHash = new Hashtable();

			foreach (StiVariable variableMaster in masterReport.Dictionary.Variables)
			{
				variablesMasterHash[variableMaster.Name] = variableMaster;
			}

			#region Check existing variables
			foreach (StiVariable variable in variables)
			{
				if (variable.Inherited)
				{
					StiVariable variableMaster = variablesMasterHash[variable.Name] as StiVariable;

					#region If new variable does not exist in master report
					if (variableMaster == null)
					{
						report.Dictionary.Variables.Remove(variable);
					}
					#endregion

					#region Replace variable in report from master report
					else
					{
						variablesHash[variable.Name] = variable;

						StiVariable newVariable = variableMaster.Clone() as StiVariable;

						int index = report.Dictionary.Variables.IndexOf(variable);
						report.Dictionary.Variables.RemoveAt(index);
						report.Dictionary.Variables.Insert(index, newVariable);
					}
					#endregion
				}
			}
			#endregion

			#region Add variables
			foreach (StiVariable variable in masterReport.Dictionary.Variables)
			{
				if (variablesHash[variable.Name] == null)
				{
					StiVariable newVariable = variable.Clone() as StiVariable;

					int index = masterReport.Dictionary.Variables.IndexOf(variable);
					if (index >= 0 && index < report.Dictionary.Variables.Count)
					{
						report.Dictionary.Variables.Insert(0, newVariable);
					}
					else report.Dictionary.Variables.Add(newVariable);

					variablesHash[newVariable.Name] = newVariable;
				}
			}
			#endregion
		}


		private static void CompareDataSources(StiReport report, StiReport masterReport)
		{
			StiDataSourcesCollection dataSources = new StiDataSourcesCollection(null);
			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				dataSources.Add(dataSource);
			}

			Hashtable dataSourcesHash = new Hashtable();
			Hashtable dataSourcesMasterHash = new Hashtable();

			foreach (StiDataSource dataSourceMaster in masterReport.Dictionary.DataSources)
			{
				dataSourcesMasterHash[dataSourceMaster.Name] = dataSourceMaster;
			}

			#region Check existing datasources
			foreach (StiDataSource dataSource in dataSources)
			{
				if (dataSource.Inherited)
				{
					StiDataSource dataSourceMaster = dataSourcesMasterHash[dataSource.Name] as StiDataSource;

					#region If new datasource does not exist in master report
					if (dataSourceMaster == null)
					{
						report.Dictionary.DataSources.Remove(dataSource);
					}
					#endregion

					#region Replace datasource in report from master report
					else
					{
						dataSourcesHash[dataSource.Name] = dataSource;

						StiDataSource newDataSource = dataSourceMaster.Clone() as StiDataSource;
						newDataSource.Dictionary = report.Dictionary;

						int index = report.Dictionary.DataSources.IndexOf(dataSource);
						report.Dictionary.DataSources.RemoveAt(index);
						report.Dictionary.DataSources.Insert(index, newDataSource);
					}
					#endregion
				}
			}
			#endregion

			#region Add datasources
			foreach (StiDataSource dataSource in masterReport.Dictionary.DataSources)
			{
				if (dataSourcesHash[dataSource.Name] == null)
				{
					StiDataSource newDataSource = dataSource.Clone() as StiDataSource;
					newDataSource.Dictionary = report.Dictionary;

					int index = masterReport.Dictionary.DataSources.IndexOf(dataSource);
					if (index >= 0 && index < report.Dictionary.DataSources.Count)
					{
						report.Dictionary.DataSources.Insert(0, newDataSource);
					}
					else report.Dictionary.DataSources.Add(newDataSource);

					dataSourcesHash[newDataSource.Name] = newDataSource;
				}
			}
			#endregion
		}


		private static void CompareRelations(StiReport report, StiReport masterReport)
		{
			Hashtable dataSourcesHash = new Hashtable();

			foreach (StiDataSource dataSource in report.Dictionary.DataSources)
			{
				dataSourcesHash[dataSource.Name] = dataSource;
			}

			StiDataRelationsCollection rels = new StiDataRelationsCollection(null);
			foreach (StiDataRelation rel in report.Dictionary.Relations)
			{
				rels.Add(rel);
			}

			Hashtable relsHash = new Hashtable();
			Hashtable relsMasterHash = new Hashtable();

			foreach (StiDataRelation relMaster in masterReport.Dictionary.Relations)
			{
				relsMasterHash[relMaster.Name] = relMaster;
			}

			#region Check existing relations
			foreach (StiDataRelation rel in rels)
			{
				if (rel.Inherited)
				{
					StiDataRelation relMaster = relsMasterHash[rel.Name] as StiDataRelation;

					#region If new relation does not exist in master report
					if (relMaster == null)
					{
						report.Dictionary.Relations.Remove(rel);
					}
					#endregion

					#region Replace relation in report from master report
					else
					{
						relsHash[rel.Name] = rel;

						StiDataRelation newRel = relMaster.Clone() as StiDataRelation;
						newRel.Dictionary = report.Dictionary;

                        if (newRel.ParentSource != null)
						    newRel.ParentSource = dataSourcesHash[newRel.ParentSource.Name] as StiDataSource;
                        if (newRel.ChildSource != null)
						    newRel.ChildSource = dataSourcesHash[newRel.ChildSource.Name] as StiDataSource;

						int index = report.Dictionary.Relations.IndexOf(rel);
						report.Dictionary.Relations.RemoveAt(index);
						report.Dictionary.Relations.Insert(index, newRel);
					}
					#endregion
				}
			}
			#endregion

			#region Add relations
			foreach (StiDataRelation rel in masterReport.Dictionary.Relations)
			{
				if (relsHash[rel.Name] == null)
				{
					StiDataRelation newRel = rel.Clone() as StiDataRelation;
					newRel.Dictionary = report.Dictionary;
					newRel.ParentSource = dataSourcesHash[newRel.ParentSource.Name] as StiDataSource;
					newRel.ChildSource = dataSourcesHash[newRel.ChildSource.Name] as StiDataSource;

					int index = masterReport.Dictionary.Relations.IndexOf(rel);
					if (index >= 0 && index < report.Dictionary.Relations.Count)
					{
						report.Dictionary.Relations.Insert(0, newRel);
					}
					else report.Dictionary.Relations.Add(newRel);

					relsHash[newRel.Name] = newRel;
				}
			}
			#endregion
		}

		
		private static void CompareDatabases(StiReport report, StiReport masterReport)
		{
			StiDatabaseCollection databases = new StiDatabaseCollection();
			foreach (StiDatabase database in report.Dictionary.Databases)
			{
				databases.Add(database);
			}

			Hashtable databasesHash = new Hashtable();
			Hashtable databasesMasterHash = new Hashtable();

			foreach (StiDatabase databaseMaster in masterReport.Dictionary.Databases)
			{
				databasesMasterHash[databaseMaster.Name] = databaseMaster;
			}

			#region Check existing databases
			foreach (StiDatabase database in databases)
			{
				if (database.Inherited)
				{
					StiDatabase databaseMaster = databasesMasterHash[database.Name] as StiDatabase;

					#region If new database does not exist in master report
					if (databaseMaster == null)
					{
						report.Dictionary.Databases.Remove(database);
					}
					#endregion

					#region Replace database in report from master report
					else
					{					

						StiDatabase newDatabase = databaseMaster.Clone() as StiDatabase;
						databasesHash[newDatabase.Name] = database;

						int index = report.Dictionary.Databases.IndexOf(database);
						report.Dictionary.Databases.RemoveAt(index);
						report.Dictionary.Databases.Insert(index, newDatabase);
					}
					#endregion
				}
			}
			#endregion

			#region Add database
			foreach (StiDatabase database in masterReport.Dictionary.Databases)
			{
				if (databasesHash[database.Name] == null)
				{
					StiDatabase newDatabase = database.Clone() as StiDatabase;

					int index = masterReport.Dictionary.Databases.IndexOf(database);
					if (index >= 0 && index < report.Dictionary.Databases.Count)
					{
						report.Dictionary.Databases.Insert(0, newDatabase);
					}
					else report.Dictionary.Databases.Add(newDatabase);

					databasesHash[newDatabase.Name] = newDatabase;
				}
			}
			#endregion
		}


        private static void CompareBusinessObjects(StiReport report, StiReport masterReport)
        {
            StiBusinessObjectsCollection businessObjects = new StiBusinessObjectsCollection(null, null);
            foreach (StiBusinessObject businessObject in report.Dictionary.BusinessObjects)
            {
                businessObjects.Add(businessObject);
            }

            Hashtable businessObjectsHash = new Hashtable();
            Hashtable businessObjectsMasterHash = new Hashtable();

            foreach (StiBusinessObject businessObjectsMaster in masterReport.Dictionary.BusinessObjects)
            {
                businessObjectsMasterHash[businessObjectsMaster.Name] = businessObjectsMaster;
            }

            #region Check existing businessObjects
            foreach (StiBusinessObject businessObject in businessObjects)
            {
                if (businessObject.Inherited)
                {
                    StiBusinessObject businessObjectMaster = businessObjectsMasterHash[businessObject.Name] as StiBusinessObject;

                    #region If new datasource does not exist in master report
                    if (businessObjectMaster == null)
                    {
                        report.Dictionary.BusinessObjects.Remove(businessObject);
                    }
                    #endregion

                    #region Replace datasource in report from master report
                    else
                    {
                        businessObjectsHash[businessObject.Name] = businessObject;

                        StiBusinessObject newBusinessObject = businessObjectMaster.Clone() as StiBusinessObject;
                        newBusinessObject.Dictionary = report.Dictionary;

                        int index = report.Dictionary.BusinessObjects.IndexOf(businessObject);
                        report.Dictionary.BusinessObjects.RemoveAt(index);
                        report.Dictionary.BusinessObjects.Insert(index, newBusinessObject);
                    }
                    #endregion
                }
            }
            #endregion

            #region Add datasources
            foreach (StiBusinessObject businessObject in masterReport.Dictionary.BusinessObjects)
            {
                if (businessObjectsHash[businessObject.Name] == null)
                {
                    StiBusinessObject newBusinessObject = businessObject.Clone() as StiBusinessObject;
                    newBusinessObject.Dictionary = report.Dictionary;

                    int index = masterReport.Dictionary.BusinessObjects.IndexOf(businessObject);
                    if (index >= 0 && index < report.Dictionary.BusinessObjects.Count)
                    {
                        report.Dictionary.BusinessObjects.Insert(0, newBusinessObject);
                    }
                    else report.Dictionary.BusinessObjects.Add(newBusinessObject);

                    businessObjectsHash[newBusinessObject.Name] = newBusinessObject;
                }
            }
            #endregion
        }


		private static void CompareEvents(StiReport report, StiReport masterReport)
		{
			if (masterReport.BeginRenderEvent.Script.Length > 0)
				report.BeginRenderEvent.Script = masterReport.BeginRenderEvent.Script;

			if (masterReport.EndRenderEvent.Script.Length > 0)
				report.EndRenderEvent.Script = masterReport.EndRenderEvent.Script;

			if (masterReport.RenderingEvent.Script.Length > 0)
				report.RenderingEvent.Script = masterReport.RenderingEvent.Script;

			if (masterReport.ExportingEvent.Script.Length > 0)
				report.ExportingEvent.Script = masterReport.ExportingEvent.Script;

			if (masterReport.ExportedEvent.Script.Length > 0)
				report.ExportedEvent.Script = masterReport.ExportedEvent.Script;

			if (masterReport.PrintingEvent.Script.Length > 0)
				report.PrintingEvent.Script = masterReport.PrintingEvent.Script;

			if (masterReport.PrintedEvent.Script.Length > 0)
				report.PrintedEvent.Script = masterReport.PrintedEvent.Script;
		}
	}
}
