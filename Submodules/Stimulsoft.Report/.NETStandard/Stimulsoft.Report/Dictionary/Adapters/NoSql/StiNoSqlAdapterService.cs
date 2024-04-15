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
using Stimulsoft.Base.Localization;
using System;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to SqlConnection.
    /// </summary>
    public abstract class StiNoSqlAdapterService : StiDataStoreAdapterService
    {
        #region Methods
        public override string GetDataCategoryName(StiData data)
        {
            return data.Name;
        }
       
        public override Type[] GetDataTypes()
        {
            try
            {
                return new[] { CreateConnector().GetType() };
            }
            catch
            {
                return null;
            }
        }

	    /// <summary>
	    /// Returns new data connector for this type of the database.
	    /// </summary>
	    /// <returns>Created connector.</returns>
        public abstract StiNoSqlDataConnector CreateConnector(string connectionString = null);


        public virtual string TestConnection(string connectionString)
        {
            try
            {
                var connector = CreateConnector(connectionString);
                var result = connector.TestConnection();

                if (result.Success)
                    return StiLocalization.Get("DesignerFx", "ConnectionSuccessfull");
                else
                    return result.Notice;

            }
            catch (Exception e)
            {
                return StiLocalization.Get("DesignerFx", "ConnectionError") + ": " + e.Message;
            }
        }

        public virtual void CreateConnectionInDataStore(StiDictionary dictionary, StiNoSqlDatabase database)
        {
            try
            {
                if (database.Name == null) return;

                //Remove all old data from datastore
                var data = dictionary.DataStore.ToList().FirstOrDefault(d => d.Name != null && d.Name.ToLowerInvariant() == database.Name.ToLowerInvariant());
                if (data != null) dictionary.DataStore.Remove(data);

                var connector = CreateConnector(database.ConnectionString);
                dictionary.DataStore.Add(new StiData(database.Name, connector)
                {
                    IsReportData = true
                });
            }
            catch (Exception e)
            {
                StiLogService.Write(this.GetType(), e);
                if (!StiOptions.Engine.HideExceptions) throw;
            }
        }
        #endregion
    }
}
