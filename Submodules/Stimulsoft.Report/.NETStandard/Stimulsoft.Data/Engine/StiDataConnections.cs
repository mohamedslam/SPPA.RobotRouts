#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public static class StiDataConnections
    {
        #region Fields
        private static object lockObject = new object();
        private static Dictionary<IStiAppConnection, List<object>> connections = new Dictionary<IStiAppConnection, List<object>>();
        #endregion

        #region Methods
        public static bool IsConnectionActive(IStiAppConnection connection)
        {
            if (connection == null)
                return false;

            lock (lockObject)
            {
                return connections.ContainsKey(connection);
            }
        }

        public static void RegisterConnection(IStiAppConnection connection, List<object> items)
        {
            if (connection == null)return;
            items = items ?? new List<object>();

            lock (lockObject)
            {
                if (connections.ContainsKey(connection))
                {
                    var list = connections[connection];
                    if (list != null)
                        list.AddRange(items);
                    else
                        connections[connection] = items;
                }
                else
                    connections.Add(connection, items);
            }
        }

        public static List<object> UnRegisterConnections(List<IStiAppConnection> connections)
        {
            return connections?
                .Where(c => c != null)
                .SelectMany(UnRegisterConnection)
                .ToList();
        }

        public static List<object> UnRegisterConnection(IStiAppConnection connection)
        {
            lock (lockObject)
            {
                if (connection == null || !connections.ContainsKey(connection))
                    return new List<object>();

                var items = connections[connection];
                connections.Remove(connection);

                return items ?? new List<object>();
            }
        }
        #endregion
    }
}