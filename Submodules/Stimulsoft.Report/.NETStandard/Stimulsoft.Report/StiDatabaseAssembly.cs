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

using System;
using System.Reflection;
using Stimulsoft.Base;
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Internal use only.
    /// </summary>
    public static class StiDatabaseAssembly
    {
        #region Fields
        private static Assembly databaseAssembly;
        #endregion

        #region Methods
        public static Assembly GetAssembly()
        {
            if (databaseAssembly != null) return databaseAssembly;

            try
            {
                databaseAssembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Database, {StiVersion.VersionInfo}");
            }
            catch
            {
            }

            try
            {
                if (databaseAssembly == null)
                    databaseAssembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Database.Wpf, {StiVersion.VersionInfo}");
            }
            catch
            {
            }

            return databaseAssembly;
        }

        public static Type GetConnectionStringHelper(string typeName)
        {
            if (typeName == null) return null;

            var assembly = GetAssembly();
            if (assembly == null) return null;

            return StiTypeFinder.GetType($"Stimulsoft.Database.Connection.{typeName}");
        }

        public static Type GetWpfConnectionStringHelper(string typeName)
        {
            if (typeName == null) return null;

            var assembly = GetAssembly();
            if (assembly == null) return null;

            return StiTypeFinder.GetType($"Stimulsoft.Database.Wpf.Connection.{typeName}");
        }

        public static string EditConnectionString(StiDatabase database, string connectionString, string typeName)
        {
            var type = GetConnectionStringHelper(typeName);
            if (type == null) return connectionString;

            var method = type.GetMethod("EditConnectionString", BindingFlags.Static | BindingFlags.Public);
            return method.Invoke(null, new object[] { connectionString }) as string;
        }

        public static bool CanEditConnectionString(string typeName)
        {
            return GetConnectionStringHelper(typeName) != null ||
                GetWpfConnectionStringHelper(typeName) != null;
        }
        #endregion
    }
}
