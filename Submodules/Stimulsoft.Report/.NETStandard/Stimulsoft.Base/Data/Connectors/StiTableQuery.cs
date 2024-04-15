#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

namespace Stimulsoft.Base
{
    public class StiTableQuery
    {
        #region Methods
        private string CorrectName(string name)
        {
            var specifiedName = connector != null ? connector.GetDatabaseSpecificName(name) : null;

            var finalName = name.Trim().IndexOfAny(new[] { ' ', '.', '-' }) != -1 ? specifiedName : name;
            return string.IsNullOrWhiteSpace(finalName) ? name : finalName;
        }

        public string GetName(string schema, string table)
        {
            return string.IsNullOrWhiteSpace(schema)
                ? CorrectName(table)
                : $"{CorrectName(schema)}.{CorrectName(table)}";
        }

        public string GetSelectQuery(string table)
        {
            return GetSelectQuery(null, table);
        }

        public string GetSelectQuery(string schema, string table)
        {
            return $"select * from {GetName(schema, table)}";
        }

        public string GetExecuteQuery(string table)
        {
            return GetExecuteQuery(null, table);
        }

        public string GetExecuteQuery(string schema, string table)
        {
            return $"execute {GetName(schema, table)}";
        }

        public string GetCallQuery(string table)
        {
            return GetCallQuery(null, table);
        }

        public string GetCallQuery(string schema, string table)
        {
            return $"call {GetName(schema, table)}";
        }

        public string GetProcQuery(string table)
        {
            return GetProcQuery(null, table);
        }

        public string GetProcQuery(string schema, string table)
        {
            return GetName(schema, table);
        }
        #endregion

        #region Methods.Get
        public static StiTableQuery Get(StiSqlDataConnector connector)
        {
            return new StiTableQuery(connector);
        }
        #endregion

        #region Fields
        private StiSqlDataConnector connector;
        #endregion

        public StiTableQuery(StiSqlDataConnector connector)
        {
            this.connector = connector;
        }
    }
}