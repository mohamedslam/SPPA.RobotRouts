#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Base.Data.Connectors.Google;
using Stimulsoft.Report.Dictionary.Databases.Google;
using System.Collections;

namespace Stimulsoft.Report.Web
{
    internal class StiGoogleAnalyticsHelper
    {
        internal static void GetGoogleAnalyticsParameters(Hashtable param, Hashtable callbackResult)
        {
            var connectionString = StiEncodingHelper.DecodeString(param["connectionString"] as string);
            var accountId = param["accountId"] as string;
            var propertyId = param["propertyId"] as string;

            var parameters = new Hashtable();
            var accounts = GetAccounts(connectionString);

            if (string.IsNullOrEmpty(accountId) && accounts.Count > 0)
                accountId = (accounts[0] as Hashtable)["key"] as string;

            if (!string.IsNullOrEmpty(accountId))
            {
                var properties = GetProperties(connectionString, accountId);
                
                if (string.IsNullOrEmpty(propertyId) && properties.Count > 0)
                    propertyId = (properties[0] as Hashtable)["key"] as string;

                parameters["properties"] = properties;
            }

            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(propertyId))
                parameters["views"] = GetViews(connectionString, accountId, propertyId);

            parameters["accounts"] = accounts;
            parameters["metrics"] = GetMetrics(connectionString);
            parameters["dimensions"] = GetDimensions(connectionString);

            callbackResult["parameters"] = parameters;
        }

        private static ArrayList GetAccounts(string connectionString)
        {
            var items = new ArrayList();
            var accounts = StiGoogleAnalyticsConnector.Get(connectionString).GetAccounts();
            if (accounts != null)
            {
                foreach (var account in accounts)
                {
                    items.Add(new Hashtable()
                    {
                        ["key"] = account.Key,
                        ["value"] = account.Value
                    });
                }
            }

            return items;
        }

        private static Hashtable GetMetrics(string connectionString)
        {   
            var groups = new Hashtable();
            var metrics = StiGoogleAnalyticsConnector.Get(connectionString).GetMetrics();

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    var groupName = metric.Value["group"];

                    if (groups[groupName] == null)
                        groups[groupName] = new ArrayList();

                    ((ArrayList)groups[groupName]).Add(new Hashtable()
                    {
                        ["key"] = metric.Key,
                        ["description"] = metric.Value["description"]
                    });
                }
            }

            return groups;
        }

        private static Hashtable GetDimensions(string connectionString)
        {
            var groups = new Hashtable();
            var dimensions = StiGoogleAnalyticsConnector.Get(connectionString).GetDimensions();
            if (dimensions != null)
            {
                foreach (var dimension in dimensions)
                {
                    var groupName = dimension.Value["group"];

                    if (groups[groupName] == null)
                        groups[groupName] = new ArrayList();

                    ((ArrayList)groups[groupName]).Add(new Hashtable()
                    {
                        ["key"] = dimension.Key,
                        ["description"] = dimension.Value["description"]
                    });
                }
            }

            return groups;
        }

        internal static ArrayList GetProperties(string connectionString, string accountId)
        {
            var items = new ArrayList();
            var properties = StiGoogleAnalyticsConnector.Get(connectionString).GetProperties(accountId);
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    items.Add(new Hashtable()
                    {
                        ["key"] = property.Key,
                        ["value"] = property.Value
                    });
                }
            }

            return items;
        }

        internal static ArrayList GetViews(string connectionString, string accountId, string propertyId)
        {
            var items = new ArrayList();
            var views = StiGoogleAnalyticsConnector.Get(connectionString).GetViews(accountId, propertyId);
            if (views != null)
            {
                foreach (var view in views)
                {
                    items.Add(new Hashtable()
                    {
                        ["key"] = view.Key,
                        ["value"] = view.Value
                    });
                }
            }

            return items;
        }
    }
}