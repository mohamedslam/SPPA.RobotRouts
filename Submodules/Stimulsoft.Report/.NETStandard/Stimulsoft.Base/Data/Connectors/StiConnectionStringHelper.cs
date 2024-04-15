#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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

using System.Linq;

namespace Stimulsoft.Base.Data.Connectors
{
    internal static class StiConnectionStringHelper
    {
        #region Methods
        internal static string GetConnectionStringKey(string connectionString, string key, char[] separator = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            separator = separator ?? new char[] { ';', ',' };
            var strs = connectionString.Split(separator);

            var address = strs.FirstOrDefault(s => s.ToLowerInvariant().StartsWith(key.ToLowerInvariant()));
            if (address == null)
                return null;

            var startIndex = address.IndexOf('=');

            var pairs = address.Split('=');
            if (pairs.Length < 2)
                return null;

            var value = address.Substring(startIndex + 1, address.Length - startIndex - 1);

            if (value.StartsWith("\"") && value.EndsWith("\""))
                value = value.Substring(0, value.Length - 2);

            return value;
        }

        internal static string GetConnectionStringKey(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            var strs = connectionString.Split(';', ',');

            return strs.FirstOrDefault(s => !s.Contains("="));
        }

        internal static string SetConnectionStringKey(string connectionString, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return $"{key}={value}";

            var strs = connectionString.Split(';', ',').Where(s => !s.ToLowerInvariant().StartsWith(key.ToLowerInvariant()));
            return string.Join(";", strs) + $";{key}={value}";
        }

        internal static string RemoveConnectionStringKey(string connectionString, string key)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            var strs = connectionString.Split(';', ',').Where(s => !s.ToLowerInvariant().StartsWith(key.ToLowerInvariant()));
            return string.Join(";", strs);
        }
        #endregion
    }
}
