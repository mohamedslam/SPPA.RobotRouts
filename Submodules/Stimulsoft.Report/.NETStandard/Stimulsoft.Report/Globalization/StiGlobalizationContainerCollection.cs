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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report
{
    public class StiGlobalizationContainerCollection :
        CollectionBase, 
        IStiJsonReportObject
    {
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            if (List.Count == 0)
                return null;

            var jObject = new JObject();
            int index = 0;
            foreach (StiGlobalizationContainer container in List)
            {
                jObject.AddPropertyJObject(index.ToString(), container.SaveToJsonObject(mode));
                index++;
            }

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var container = new StiGlobalizationContainer();
                container.LoadFromJsonObject((JObject)property.Value);

                List.Add(container);
            }
        }

        public void SaveToFile(string file)
        {
            var rep = new StiReport();
            rep.ReportUnit = StiReportUnitType.HundredthsOfInch;
            rep.IsRendered = true;
            var page = new StiPage();
            page.UnlimitedHeight = true;
            rep.RenderedPages.Add(page);

            var cellWidth1 = 150;
            var cellWidthCulture = 300;
            var cellHeight = 20;

            #region Get all actual properties
            var keys = new List<string>();
            foreach (StiGlobalizationContainer obj in report.GlobalizationStrings)
            {
                foreach (StiGlobalizationItem item in obj.Items)
                {
                    if (!keys.Contains(item.PropertyName))
                        keys.Add(item.PropertyName);
                }
            }
            #endregion

            #region Add Geaders
            var brush = new StiSolidBrush(StiColor.Get("ddd"));
            page.Components.Add(new StiText(new RectangleD(0, 0, cellWidth1, cellHeight), "Property") { Brush = brush, VertAlignment = StiVertAlignment.Center });
            int posX = cellWidth1;
            foreach (StiGlobalizationContainer obj in report.GlobalizationStrings)
            {
                page.Components.Add(new StiText(new RectangleD(posX, 0, cellWidthCulture, cellHeight), obj.CultureName) { Brush = brush, VertAlignment = StiVertAlignment.Center });
                posX += cellWidthCulture;
            }
            #endregion

            #region Add all properties
            var posY = cellHeight;
            foreach (var key in keys)
            {
                page.Components.Add(new StiText(new RectangleD(0, posY, cellWidth1, cellHeight), key) { Brush = brush });
                posY += cellHeight;
            }
            #endregion

            #region Add all values
            posX = cellWidth1;
            brush = new StiSolidBrush(Color.White);
            foreach (StiGlobalizationContainer obj in report.GlobalizationStrings)
            {
                foreach (StiGlobalizationItem item in obj.Items)
                {
                    int index = keys.IndexOf(item.PropertyName);
                    page.Components.Add(new StiText(new RectangleD(posX, cellHeight + index * cellHeight, cellWidthCulture, cellHeight), item.Text) { Brush = brush });
                }

                posX += cellWidthCulture;
            }
            #endregion

            page.Width = cellWidth1 + report.GlobalizationStrings.Count * cellWidthCulture;
            page.Height = (keys.Count + 1) * cellHeight;

            rep.ExportDocument(StiExportFormat.Excel2007, file);
        }

        public void LoadFromFile(string file)
        {
            var dataSet = StiExcelConnector.Get().GetDataSet(new StiExcelOptions(File.ReadAllBytes(file), false));
            var dataTable = dataSet.Tables.Cast<DataTable>().FirstOrDefault();
            if (dataTable == null)
                throw new DataException("No required information was presented or an incorrect format of the structure of globalization data!");

            if (dataTable.Columns.Count < 2)
                throw new DataException("Incorrect number of columns!");

            if (dataTable.Rows.Count == 0)
                throw new DataException("Incorrect number of rows!");

            var rowColumns = dataTable.Rows[0];
            if (rowColumns.ItemArray[0] as string != "Property")
                throw new DataException("A 'Property' column wasn't found in the file!");

            //int column = 1;
            //foreach (StiGlobalizationContainer obj in report.GlobalizationStrings)
            //{
            //    if (rowColumns.ItemArray[column] as string != obj.CultureName)
            //        throw new DataException($"A '{obj.CultureName}' column wasn't found in the file!");

            //    column++;
            //}

            report.GlobalizationStrings.Clear();

            int rowIndex = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                if (rowIndex == 0)
                {
                    for (int index = 1; index < row.ItemArray.Length; index++)
                    {
                        var container = new StiGlobalizationContainer(row.ItemArray[index] as string);
                        report.GlobalizationStrings.Add(container);
                    }
                }
                else
                {
                    var propertyName = row.ItemArray[0] as string;
                    for (int index = 1; index < row.ItemArray.Length; index++)
                    {
                        var value = row.ItemArray[index] as string;
                        if (string.IsNullOrEmpty(value))
                            value = string.Empty;

                        var container = report.GlobalizationStrings[index - 1];
                        container.Items.Add(new StiGlobalizationItem(propertyName, value));
                    }
                }

                rowIndex++;
            }
        }
        #endregion

        #region Collection
        public List<StiGlobalizationContainer> ToList()
        {
            return this.Cast<object>().Cast<StiGlobalizationContainer>().ToList();
        }

        public void Add(StiGlobalizationContainer data)
		{
			List.Add(data);
		}

		public void AddRange(StiGlobalizationContainer[] data)
		{
			base.InnerList.AddRange(data);
		}

		public bool Contains(StiGlobalizationContainer data)
		{
			return List.Contains(data);
		}
		
		public int IndexOf(StiGlobalizationContainer data)
		{
			return List.IndexOf(data);
		}

		public void Insert(int index, StiGlobalizationContainer data)
		{
			lock (this)List.Insert(index, data);
		}

		public void Remove(StiGlobalizationContainer data)
		{
			lock (this)List.Remove(data);
		}
		
		
		public StiGlobalizationContainer this[int index]
		{
			get
			{
				return (StiGlobalizationContainer)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		public StiGlobalizationContainer this[string name]
		{
			get
			{
				name = name.ToLowerInvariant();
                foreach (StiGlobalizationContainer container in List)
                {
                    if (container.CultureName.ToLowerInvariant() == name)
                        return container;
                }
				return null;
			}
			set
			{
				name = name.ToLowerInvariant();
				for (int index = 0; index < List.Count; index++)				
				{
					var container = List[index] as StiGlobalizationContainer;
					
					if (container.CultureName.ToLowerInvariant() == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
			}
		}
		#endregion

        #region Fields
        private StiReport report = null;
        #endregion

        #region Properties
        internal bool SkipException { get; set; }
        #endregion

        #region Methods
        private string GetShortName(string cultureName)
        {
            var index = cultureName.IndexOf("-", StringComparison.InvariantCulture);
            if (index > -1)
                return cultureName.Substring(0, index);
            else
                return cultureName;
        }

        public void LocalizeReport(string cultureName)
		{
            var container = this[cultureName];
            if (container == null)
                container = this[GetShortName(cultureName)];

            if (container == null && !SkipException)
				throw new Exception($"Can't find globalized strings for culture {cultureName}");

            if (container != null)
            {
                FillItemsFromReport();

                container.LocalizeReport(report);

                RemoveUnlocalizedItemsFromReport();
            }
		}

		public void LocalizeReport(CultureInfo info)
		{
		    LocalizeReport(info.Name);
		}

		public void FillItemsFromReport()
		{
			foreach (StiGlobalizationContainer container in this)
			{
				container.FillItemsFromReport(report);
			}
		}


		public void RemoveUnlocalizedItemsFromReport()
		{
			foreach (StiGlobalizationContainer container in this)
			{
				container.RemoveUnlocalizedItemsFromReport(report);
			}
		}

        public void RemoveComponent(StiComponent comp)
        {
            var provider = comp as IStiGlobalizationProvider;
            if (provider != null)
            {
                var strs = provider.GetAllStrings();

                foreach (var str in strs)
                {
                    var data = $"{comp.Name}.{str}";

                    foreach (StiGlobalizationContainer container in this)
                    {
                        var index = 0;
                        while (index < container.Items.Count)
                        {
                            var item = container.Items[index];
                            if (item.PropertyName == data)
                            {
                                container.Items.RemoveAt(index);
                            }
                            else index++;
                        }
                    }
                }                                    
            }
        }

        public void RenameComponent(StiComponent comp, string oldName, string newName)
        {
            var provider = comp as IStiGlobalizationProvider;
            if (provider != null)
            {
                var strs = provider.GetAllStrings();

                foreach (var str in strs)
                {
                    var oldData = $"{oldName}.{str}";
                    var newData = $"{newName}.{str}";

                    foreach (StiGlobalizationContainer container in this)
                    {
                        foreach (StiGlobalizationItem item in container.Items)
                        {
                            if (item.PropertyName == oldData)
                            {
                                item.PropertyName = newData;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public StiGlobalizationContainerCollection(StiReport report)
		{
			this.report = report;
		}
    }
}
