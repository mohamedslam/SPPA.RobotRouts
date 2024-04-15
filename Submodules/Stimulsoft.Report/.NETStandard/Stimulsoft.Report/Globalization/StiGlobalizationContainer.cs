#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Report
{
    public class StiGlobalizationContainer : IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyStringNullOrEmpty("CultureName", CultureName);

            #region Items

            if (Items.Count > 0)
            {
                var jObjectArray = new JObject();
                int index = 0;
                foreach (StiGlobalizationItem item in Items)
                {
                    jObjectArray.AddPropertyJObject(index.ToString(), item.SaveToJsonObject(mode));
                    index++;
                }

                jObject.AddPropertyJObject("Items", jObjectArray);
            }

            #endregion

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CultureName":
                        this.CultureName = property.DeserializeString();
                        break;

                    case "Items":
                        {
                            foreach(var prop1 in ((JObject)property.Value).Properties())
                            {
                                var item = new StiGlobalizationItem();
                                item.LoadFromJsonObject((JObject)prop1.Value);

                                this.Items.Add(item);
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #region Properties
        [DefaultValue("")]
		[StiSerializable]
		public string CultureName { get; set; }

        [StiSerializable(StiSerializationVisibility.List)]
		public StiGlobalizationItemCollection Items { get; set; } = new StiGlobalizationItemCollection();
        #endregion

        #region Methods
        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="report">Report</param>
        /// <returns>Array of strings</returns>
		public Hashtable GetAllStringsForReport(StiReport report)
		{
			var compsList = new Hashtable();

			#region Components
			var comps = report.GetComponents();			
			foreach (StiComponent comp in comps)
            {
                var provider = comp as IStiGlobalizationProvider;
                if (provider == null)continue;

                var strs = provider.GetAllStrings();

                foreach (var str in strs)
                {
                    var data = $"{comp.Name}.{str}";
                    var value = (string)provider.GetString(str)?.Clone();
                    if (value != null)
                        compsList[data] = value;
                }
            }
            #endregion

            #region Variables
            foreach (StiVariable variable in report.Dictionary.Variables)
			{
                var variableName = $"Variable.{variable.Name}";

                compsList[variableName] = variable.Value;

                if (variable.RequestFromUser && StiOptions.Engine.Globalization.AllowUseVariableAlias)
                {
                    compsList[$"{variableName}.Alias"] = variable.Alias ?? "";

                    if (variable.DialogInfo?.Keys != null && variable.DialogInfo?.Keys.Length > 0)
                        compsList[$"{variableName}.Items.Keys"] = string.Join(Environment.NewLine, variable.DialogInfo.Keys);

                    if (variable.DialogInfo?.Values != null && variable.DialogInfo?.Values.Length > 0)
                        compsList[$"{variableName}.Items.Values"] = string.Join(Environment.NewLine, variable.DialogInfo.Values);
                }
            }
			#endregion

			#region Report Alias
			compsList["ReportAuthor"] = report.ReportAuthor ?? "";
			compsList["ReportDescription"] = report.ReportDescription ?? "";
			compsList["ReportAlias"] = report.ReportAlias ?? "";
			#endregion

			return compsList;
		}

		public void LocalizeReport(StiReport report)
		{
			var variables = new Hashtable();
			foreach (StiVariable variable in report.Dictionary.Variables)
			{
                var variableName = $"Variable.{variable.Name}";

                variables[variableName] = variable;

                if (variable.RequestFromUser && StiOptions.Engine.Globalization.AllowUseVariableAlias)
                {
                    variables[$"{variableName}.Alias"] = variable;
                    
                    if (variable.DialogInfo?.Keys != null && variable.DialogInfo?.Keys.Length > 0)
                        variables[$"{variableName}.Items.Keys"] = variable;

                    if (variable.DialogInfo?.Values != null && variable.DialogInfo?.Values.Length > 0)
                        variables[$"{variableName}.Items.Values"] = variable;
                }                
            }

            foreach (StiGlobalizationItem item in this.Items)
			{
				string str = item.PropertyName;
				
				#region Report properties
				if (str == "ReportAuthor")
                    report.ReportAuthor = item.Text;

				else if (str == "ReportDescription")
                    report.ReportDescription = item.Text;

				else if (str == "ReportAlias")
                    report.ReportAlias = item.Text;
				#endregion

				#region Variables
				else if (variables[str] is StiVariable)
				{
					var variable = variables[str] as StiVariable;
                    
                    if (str.EndsWith(".Alias"))                    
                        variable.Alias = item.Text;
                    
                    else if (str.EndsWith(".Items.Keys"))                 
                        variable.DialogInfo.Keys = item.Text.Split(new string[] { Environment.NewLine, "\n" /* but IE 11 inserts line break as \n */ }, StringSplitOptions.RemoveEmptyEntries);

                    else if(str.EndsWith(".Items.Values"))                    
                        variable.DialogInfo.Values = item.Text.Split(new string[] { Environment.NewLine, "\n" /* but IE 11 inserts line break as \n */ }, StringSplitOptions.RemoveEmptyEntries);
                    
                    else                    
                        variable.Value = item.Text;                    
				}
				#endregion

				#region Components
				else
				{
                    int index = str.IndexOfInvariant(".");
					if (index != -1)
					{
						var compName = str.Substring(0, index);
						var propertyName = str.Substring(index + 1);

						var provider = report.GetComponents()[compName] as IStiGlobalizationProvider;
					    provider?.SetString(propertyName, item.Text);
					}
				}
				#endregion
			}
		}
        
		public void FillItemsFromReport(StiReport report)
		{
			var list = GetAllStringsForReport(report);
			var hash = new Hashtable();

			foreach (StiGlobalizationItem item in this.Items)
			{
				hash[item.PropertyName] = item;
			}

			foreach (string str in list.Keys)
			{
				if (hash[str] != null)continue;

				this.Items.Add(new StiGlobalizationItem(str, list[str] as string));
			}
		}
        
		public void RemoveUnlocalizedItemsFromReport(StiReport report)
		{
			var list = GetAllStringsForReport(report);
			var hash = new Hashtable();

			foreach (StiGlobalizationItem item in this.Items)
			{
				hash[item.PropertyName] = item;
			}

			foreach (string str in list.Keys)
			{
				var value = list[str] as string;
				var item = hash[str] as StiGlobalizationItem;

				if (item != null && value == item.Text) 
                    this.Items.Remove(item);
			}
		}
        #endregion

        public StiGlobalizationContainer() : this("en")
		{
		}
		

		public StiGlobalizationContainer(string cultureName)
		{			
			this.CultureName = cultureName;
		}
    }
}
