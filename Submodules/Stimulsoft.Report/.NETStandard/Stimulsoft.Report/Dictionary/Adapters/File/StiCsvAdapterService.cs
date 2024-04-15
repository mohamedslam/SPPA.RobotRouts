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
using Stimulsoft.Base.Plans;
using System;
using System.Data;
using System.IO;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to Csv files.
    /// </summary>
    public class StiCsvAdapterService : StiFileAdapterService
	{
		#region StiDataAdapterService override
		/// <summary>
		/// Returns a collection of columns of data.
		/// </summary>
		/// <param name="data">Data to find column.</param>
		/// <returns>Collection of columns found.</returns>
		public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource)
		{
            return GetColumnsFromData(data, dataSource, CommandBehavior.SchemaOnly);
		}

        /// <summary>
        /// Returns a collection of columns of data.
        /// </summary>
        /// <param name="data">Data to find column.</param>
        /// <returns>Collection of columns found.</returns>
        public override StiDataColumnsCollection GetColumnsFromData(StiData data, StiDataSource dataSource, CommandBehavior retrieveMode)
        {
            var dataColumns = new StiDataColumnsCollection();
            var csvSource = dataSource as StiCsvSource;
            if (csvSource == null)
                return dataColumns;

            try
            {
                if (!(File.Exists(csvSource.Path)))
                    throw new FileNotFoundException($"Can't find file '{csvSource.Path}'");

                using (var dataTable = StiCsvHelper.GetTable(csvSource.Path, csvSource.CodePage, csvSource.Separator, 1))
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        var columnName = StiNameValidator.CorrectName(column.ColumnName, dataSource.Dictionary?.Report);
                        dataColumns.Add(new StiDataColumn(columnName, column.Caption, column.DataType));
                    }
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }

            return dataColumns;
        }

        /// <summary>
        /// Returns a collection of parameters of data.
        /// </summary>
        /// <param name="data">Data to find parameters.</param>
        /// <returns>Collection of parameters found.</returns>
        public override StiDataParametersCollection GetParametersFromData(StiData data, StiDataSource dataSource)
        {
            return new StiDataParametersCollection();
        }

		/// <summary>
		/// Fills a name and alias of the Data Source relying on data.
		/// </summary>
		/// <param name="data">Data relying on which names will be filled.</param>
		/// <param name="dataSource">Data Source in which names will be filled.</param>
		public override void SetDataSourceNames(StiData data, StiDataSource dataSource)
		{
			base.SetDataSourceNames(data, dataSource);

			var path = ((StiCsvSource)dataSource).Path;
			if (path == null || path.Trim().Length == 0)
				path = "Csv";
			else
			    path = Path.GetFileNameWithoutExtension(path);

			dataSource.Name = path;
			dataSource.Alias = path;
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiCsvSource);
		}

	    /// <summary>
	    /// Returns the array of data types to which the Data Source may refer.
	    /// </summary>
	    /// <returns>Array of data types.</returns>
	    public override Type[] GetDataTypes()
	    {
	        return new[] { typeof (StiCsvSource) };
	    }

	    public override void ConnectDataSourceToData(StiDictionary dictionary, StiDataSource dataSource, bool loadData)
		{
			try
			{
                StiDataLeader.Disconnect(dataSource);

                var csvSource = dataSource as StiCsvSource;

				if (!File.Exists(csvSource.Path))return;

#if CLOUD
                var maxRows = loadData ? StiCloudReport.GetMaxDataRows(dictionary?.Report?.ReportGuid) : 1;
#else
                var maxRows = loadData ? 0 : 1;
#endif

                var dataTable = StiCsvHelper.GetTable(csvSource.Path, csvSource.CodePage, csvSource.Separator, maxRows);
			    if (!loadData) 
                    dataTable.Rows.Clear();

				dataTable.TableName = csvSource.Name;
				dataSource.DataTable = dataTable;

                CheckConvertNulls(csvSource);
			}
			catch (Exception e)
			{
				StiLogService.Write(this.GetType(), e);
				if (!StiOptions.Engine.HideExceptions)throw;
			}
		
		}

        private void CheckConvertNulls(StiCsvSource dataSource)
        {
            if (dataSource.Dictionary == null || dataSource.Dictionary.Report == null || dataSource.Dictionary.Report.ConvertNulls) return;
            if (!dataSource.ConvertEmptyStringToNull) return;

            var dataTable = dataSource.DataTable;

            //prepare conversion info
            var columns = new bool[dataTable.Columns.Count];
            for (var index = 0; index < dataTable.Columns.Count; index++)
            {
                var dataColumn = dataTable.Columns[index];
                var dc = dataSource.Columns[dataColumn.ColumnName];
                if (dc != null && dc.Type != typeof(string))
                    columns[index] = true;
            }

            //convert data
            foreach (DataRow dr in dataTable.Rows)
            {
                for (var index = 0; index < dataTable.Columns.Count; index++)
                {
                    if (columns[index])
                    {
                        var obj = dr[index];
                        if (obj is string && (string) obj == string.Empty)
                            dr[index] = null;
                    }
                }
            }
        }
		#endregion

        #region Methods
        /// <summary>
        /// Returns new data connector for this type of the database.
        /// </summary>
        /// <returns>Created connector.</returns>
        public override StiFileDataConnector CreateConnector()
        {
            return StiCsvConnector.Get();
        }
        #endregion
	}
}
