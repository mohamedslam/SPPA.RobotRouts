#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft BI												    }
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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Stimulsoft.Base
{
    public static class StiDataMonitor
    {
        #region class Record
        public class Record
        {
            public int Index { get; set; }

            public DateTime Created { get; set; } = DateTime.Now;

            public string Query { get; set; }
        }
        #endregion

        #region Events
        public static event EventHandler NewMessage;
        #endregion

        #region Properties
        public static int Index { get; set; } = 1;

        public static bool Enable { get; set; }

        private static List<Record> Records { get; } = new List<Record>();
        #endregion

        #region Methods
        public static List<Record> FetchRecords(int fromIndex = -1)
        {
            lock (Records)
            {
                if (fromIndex == -1)
                    return Records.ToList();
                else
                    return Records.Where(r => r.Index >= fromIndex).ToList();
            }
        }

        public static void ClearRecords()
        {
            lock (Records)
            {
                Records.Clear();
            }
        }

        private static string GetUnique(IDbConnection connection)
        {
            var name = connection?.Database;
            if (name == null)
                name = connection.ToString();

            if (name.Length > 10)
                name = name.Substring(0, 10);

            return name;
        }

        public static void Open(IDbConnection connection)
        {
            Write($"{GetUnique(connection)}: open connection");
        }

        public static void Close(IDbConnection connection)
        {
            Write($"{GetUnique(connection)}: close connection");
        }

        public static void GetSchema(IDbConnection connection, string type)
        {
            Write($"{GetUnique(connection)}: get schema '{type}'");
        }

        public static void FillAdapter(DbDataAdapter adapter)
        {
            Write($"{GetUnique(adapter.SelectCommand.Connection)}: select '{adapter.SelectCommand.CommandText}'");
        }

        public static void DeriveParameters(DbCommand command)
        {
            Write($"{GetUnique(command.Connection)}: derive parameters ('{command.CommandText}')");
        }

        public static void Create(DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            var behaviorStr = behavior == CommandBehavior.Default ? "" : behavior.ToString();
            Write($"{GetUnique(command.Connection)}: run ('{command.CommandText}') {behaviorStr}");
        }

        public static void Create(DbDataAdapter adapter)
        {
            Write($"{GetUnique(adapter.SelectCommand.Connection)}: run ('{adapter.SelectCommand.CommandText}')");
        }

        public static void ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            var behaviorStr = behavior == CommandBehavior.Default ? "" : behavior.ToString();
            Write($"{GetUnique(command.Connection)}: run ('{command.CommandText}') {behaviorStr}");
        }

        public static void FillSchema(DbDataAdapter adapter)
        {
            Write($"{GetUnique(adapter.SelectCommand.Connection)}: fill schema ('{adapter.SelectCommand.CommandText}')");
        }

        public static void Write(string message)
        {
            if (!Enable) return;

            lock (Records)
            {
                Records.Add(new Record
                {
                    Index = Index++,
                    Query = message
                });

                NewMessage?.Invoke(null, EventArgs.Empty);
            }
        }
        #endregion
    }
}
