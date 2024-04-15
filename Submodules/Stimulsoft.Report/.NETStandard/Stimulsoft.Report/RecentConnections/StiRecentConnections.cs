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

using Stimulsoft.Report.Dictionary;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report
{
    public class StiRecentConnections
    {
        #region class Item
        public class Item
        {
            public Connection Connection { get; }

            public string Name { get; }

            public Item(Connection connection, string name)
            {
                this.Connection = connection;
                this.Name = name;
            }
        }
        #endregion

        #region class Connection
        public class Connection
        {
            public int Id { get; internal set; }

            public string DataAdapterType { get; set; }

            public string Name { get; set; }

            public string Alias { get; set; }

            public string ConnectionString { get; set; }

            public string XmlType { get; set; }

            public string PathSchema { get; set; }

            public string PathData { get; set; }

            public int CodePage { get; set; }

            public string Separator { get; set; }

            public bool PromptUserNameAndPassword { get; set; }

            public bool Pinned { get; set; }
        }
        #endregion

        #region Properties
        public static StiRecentConnections RecentConnections { get; set; }
        #endregion

        #region Consts
        private const string id = "StiRecentConnections";
        private const byte maxCountRecentConnections = 15;
        #endregion

        #region Fields
        private int countRecentConnections;
        private List<Item> connections;
        #endregion

        #region Methods
        public List<Item> GetRecentConnections()
        {
            return connections;
        }

        public StiDatabase GetDatabase(Connection recentConnection)
        {
            if (recentConnection != null)
            {
                StiDatabase recentDatabase = null;

                if (recentConnection.DataAdapterType == "StiXmlDatabase")
                {
                    recentDatabase = new StiXmlDatabase(recentConnection.Name, recentConnection.PathSchema, recentConnection.PathData);
                    recentDatabase.Alias = recentConnection.Alias;

                    if (!string.IsNullOrEmpty(recentConnection.XmlType))
                    {
                        ((StiXmlDatabase)recentDatabase).XmlType = recentConnection.XmlType == StiXmlType.AdoNetXml.ToString()
                            ? StiXmlType.AdoNetXml 
                            : StiXmlType.Xml;
                    }
                }
                else if (recentConnection.DataAdapterType == "StiCsvDatabase")
                {
                    recentDatabase = new StiCsvDatabase(recentConnection.Name, recentConnection.PathData, recentConnection.CodePage, recentConnection.Separator);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else if (recentConnection.DataAdapterType == "StiDBaseDatabase")
                {
                    recentDatabase = new StiDBaseDatabase(recentConnection.Name, recentConnection.PathData, recentConnection.CodePage);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else if (recentConnection.DataAdapterType == "StiODataDatabase")
                {
                    recentDatabase = new StiODataDatabase(recentConnection.Name, recentConnection.ConnectionString);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else if (recentConnection.DataAdapterType == "StiExcelDatabase")
                {
                    recentDatabase = new StiExcelDatabase(recentConnection.Name, recentConnection.PathData);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else if (recentConnection.DataAdapterType == "StiJsonDatabase")
                {
                    recentDatabase = new StiJsonDatabase(recentConnection.Name, recentConnection.PathData);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else if (recentConnection.DataAdapterType == "StiGisDatabase")
                {
                    recentDatabase = new StiGisDatabase(recentConnection.Name, recentConnection.PathData);
                    recentDatabase.Alias = recentConnection.Alias;
                }
                else
                {
                    var database = StiOptions.Services.Databases.FirstOrDefault(d => d.GetType().Name == recentConnection.DataAdapterType);
                    if (database != null)
                    {
                        recentDatabase = database.Clone() as StiDatabase;

                        recentDatabase.Name = recentConnection.Name;
                        recentDatabase.Alias = recentConnection.Alias;

                        if (recentDatabase is StiSqlDatabase)
                        {
                            ((StiSqlDatabase) recentDatabase).ConnectionString = recentConnection.ConnectionString;
                            ((StiSqlDatabase) recentDatabase).PromptUserNameAndPassword = recentConnection.PromptUserNameAndPassword;
                        }
                    }
                }

                return recentDatabase;
            }

            return null;
        }

        private void Load()
        {
            StiSettings.Load();
            countRecentConnections = StiSettings.GetInt(id, "Count", 0);
            if (countRecentConnections == 0) return;

            connections = new List<Item>();

            var services = StiOptions.Services.Databases.Where(s => s.ServiceEnabled);
            for (int index = 0; index < countRecentConnections; index++)
            {
                var connection = new Connection
                {
                    Id = index,
                    DataAdapterType = StiSettings.GetStr(id, $"dataAdapterType{index}", string.Empty)
                };

                if (string.IsNullOrEmpty(connection.DataAdapterType))
                    continue;

                var service = services.FirstOrDefault(x => x.GetType().Name == connection.DataAdapterType);
                if (service == null)
                    continue;

                connection.Name = StiSettings.GetStr(id, $"name{index}", string.Empty);
                connection.Alias = StiSettings.GetStr(id, $"alias{index}", string.Empty);
                connection.Pinned = StiSettings.GetBool(id, $"pinned{index}", false);

                if (connection.DataAdapterType == "StiXmlDatabase")
                {
                    connection.PathData = StiSettings.GetStr(id, "pathData" + index, string.Empty);
                    connection.PathSchema = StiSettings.GetStr(id, "pathSchema" + index, string.Empty);
                    connection.XmlType = StiSettings.GetStr(id, "xmlType" + index, string.Empty);
                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }
                else if (connection.DataAdapterType == "StiJsonDatabase")
                {
                    connection.PathData = StiSettings.GetStr(id, "pathData" + index, string.Empty);
                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }
                else if (connection.DataAdapterType == "StiGisDatabase")
                {
                    connection.PathData = StiSettings.GetStr(id, "pathData" + index, string.Empty);
                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }
                else if (connection.DataAdapterType == "StiCsvDatabase")
                {
                    connection.PathData = StiSettings.GetStr(id, $"pathData{index}", string.Empty);
                    connection.CodePage = StiSettings.GetInt(id, $"codePage{index}", 0);
                    connection.Separator = StiSettings.GetStr(id, $"separator{index}", string.Empty);

                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }
                else if (connection.DataAdapterType == "StiExcelDatabase")
                {
                    connection.PathData = StiSettings.GetStr(id, $"pathData{index}", string.Empty);

                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }
                else
                {
                    connection.ConnectionString = StiSettings.GetStr(id, $"connectionString{index}", string.Empty);
                    connection.PromptUserNameAndPassword = StiSettings.GetBool(id, $"promptUserNameAndPassword{index}", false);

                    AddObjects(connection, $"[{service.ServiceName}].{connection.Name}");
                }

                if (connections.Count >= maxCountRecentConnections) break;
            }
        }

        private void AddObjects(Connection connection, string name)
        {
            var item = new Item(connection, name);
            connections.Add(item);
        }

        public void Save()
        {
            if (connections == null) 
                connections = new List<Item>();

            StiSettings.Load();
            StiSettings.ClearKey(id);

            StiSettings.Set(id, "Count", connections.Count);

            int index = 0;
            foreach (Item objs in connections)
            {
                var recentConn = objs.Connection;

                StiSettings.Set(id, "dataAdapterType" + index, recentConn.DataAdapterType);
                StiSettings.Set(id, "name" + index, recentConn.Name);
                StiSettings.Set(id, "alias" + index, recentConn.Alias);
                StiSettings.Set(id, "pinned" + index, recentConn.Pinned);
                StiSettings.Set(id, "pathData" + index, recentConn.PathData);
                StiSettings.Set(id, "pathSchema" + index, recentConn.PathSchema);
                StiSettings.Set(id, "xmlType" + index, recentConn.XmlType);
                StiSettings.Set(id, "codePage" + index, recentConn.CodePage);
                StiSettings.Set(id, "separator" + index, recentConn.Separator);
                StiSettings.Set(id, "connectionString" + index, recentConn.ConnectionString);
                StiSettings.Set(id, "promptUserNameAndPassword" + index, recentConn.PromptUserNameAndPassword);

                index++;
                if (index > maxCountRecentConnections) break;
            }

            StiSettings.Save();
        }

        public void AddAndSave(StiDatabase dataBase)
        {
            if (dataBase == null) return;

            if (connections == null)
                connections = new List<Item>();

            #region Удаляем лишние соединения (если Count > maxCountRecentConnections)
            if (connections.Count >= maxCountRecentConnections)
            {
                for (int index = 0; index < connections.Count; index++ )
                {
                    var recent = connections[index].Connection;
                    if (!recent.Pinned)
                    {
                        connections.RemoveAt(index);
                        break;
                    }
                }
            }
            #endregion

            #region Add
            var addRecentConn = new Connection();
            if (dataBase is StiXmlDatabase)
            {
                var xmlDatabase = (StiXmlDatabase)dataBase;
                if (string.IsNullOrEmpty(xmlDatabase.PathData) && string.IsNullOrEmpty(xmlDatabase.PathSchema))
                    return;

                addRecentConn.DataAdapterType = "StiXmlDatabase";
                addRecentConn.PathData = xmlDatabase.PathData;
                addRecentConn.PathSchema = xmlDatabase.PathSchema;
                addRecentConn.XmlType = xmlDatabase.XmlType.ToString();
            }
            else if (dataBase is StiExcelDatabase)
            {
                var excelDatabase = (StiExcelDatabase)dataBase;
                if (string.IsNullOrEmpty(excelDatabase.PathData))
                    return;

                addRecentConn.DataAdapterType = "StiExcelDatabase";
                addRecentConn.PathData = excelDatabase.PathData;
            }
            else if (dataBase is StiCsvDatabase)
            {
                var csvDatabase = (StiCsvDatabase)dataBase;
                if (string.IsNullOrEmpty(csvDatabase.PathData))
                    return;

                addRecentConn.DataAdapterType = "StiCsvDatabase";
                addRecentConn.PathData = csvDatabase.PathData;
                addRecentConn.CodePage = csvDatabase.CodePage;
                addRecentConn.Separator = csvDatabase.Separator;
            }
            else if (dataBase is StiJsonDatabase)
            {
                var jsonDatabase = (StiJsonDatabase)dataBase;
                if (string.IsNullOrEmpty(jsonDatabase.PathData))
                    return;

                addRecentConn.DataAdapterType = "StiJsonDatabase";
                addRecentConn.PathData = jsonDatabase.PathData;
            }
            else if (dataBase is StiGisDatabase)
            {
                var gisDatabase = (StiGisDatabase)dataBase;
                if (string.IsNullOrEmpty(gisDatabase.PathData))
                    return;

                addRecentConn.DataAdapterType = "StiGisDatabase";
                addRecentConn.PathData = gisDatabase.PathData;
            }
            else if (dataBase is StiSqlDatabase)
            {
                var sqlDB = (StiSqlDatabase)dataBase;
                if (string.IsNullOrEmpty(sqlDB.ConnectionString))
                    return;

                if (dataBase is StiUndefinedDatabase)
                    addRecentConn.DataAdapterType = "Undefined SqlDatabase";

                else
                    addRecentConn.DataAdapterType = dataBase.GetType().Name;

                addRecentConn.PromptUserNameAndPassword = sqlDB.PromptUserNameAndPassword;
                addRecentConn.ConnectionString = sqlDB.ConnectionString;
            }
            else
            {
                Save();
                return;
            }

            addRecentConn.Name = dataBase.Name;
            addRecentConn.Alias = dataBase.Alias;

            if (CheckingForAMatch(addRecentConn))
                AddObjects(addRecentConn, string.Empty);
            #endregion

            Save();
        }

        public void Remove(Connection recentConn)
        {
            if (connections == null) return;

            foreach (var item in connections)
            {
                if (item.Connection.Id == recentConn.Id)
                {
                    connections.Remove(item);
                    return;
                }
            }
        }

        public void SetPin(int ID, bool pinValue)
        {
            if (connections == null) return;

            foreach (var connection in connections)
            {
                var recent = connection.Connection;
                if (recent.Id == ID)
                {
                    recent.Pinned = pinValue;
                    break;
                }
            }
        }

        private bool CheckingForAMatch(Connection addRecentConn)
        {
            int index = 0;
            while (index < connections.Count)
            {
                var recConn = connections[index].Connection;

                if (recConn.DataAdapterType == addRecentConn.DataAdapterType)
                {
                    if (recConn.DataAdapterType == "StiXmlDatabase")
                    {
                        if (addRecentConn.PathData == recConn.PathData && addRecentConn.PathSchema == recConn.PathSchema)
                            return false;
                    }
                    else if (recConn.DataAdapterType == "StiJsonDatabase")
                    {
                        if (addRecentConn.PathData == recConn.PathData)
                            return false;
                    }
                    else if (recConn.DataAdapterType == "StiGisDatabase")
                    {
                        if (addRecentConn.PathData == recConn.PathData)
                            return false;
                    }
                    else
                    {
                        if (addRecentConn.ConnectionString == recConn.ConnectionString)
                            return false;
                    }
                }

                index++;
            }

            return true;
        }
        #endregion

        public StiRecentConnections()
        {
            Load();
        }
    }
}