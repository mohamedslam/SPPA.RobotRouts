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

using System.Reflection;

namespace Stimulsoft.Base.Exceptions
{
    public partial class StiNotFoundProviderCreater
    {
        #region Fields
        private static object lockObject = new object();
        private static bool isInited;
        #endregion

        #region Properties
        internal static bool IsAssemblyLoaded => Assembly != null;

        private static Assembly assembly;
        internal static Assembly Assembly
        {
            get
            {
                LoadAssembly();

                return assembly;
            }
            set
            {
                assembly = value;
            }
        }
        #endregion

        #region Methods
        internal static void LoadAssembly()
        {

            if (isInited) return;

            lock (lockObject)
            {
                try
                {
                    Assembly = StiAssemblyFinder.GetAssembly($"Stimulsoft.Report.Design, {StiVersion.VersionInfo}");
                }
                catch
                {
                }
                isInited = true;
            }
        }

        internal static IStiNotFoundProvider GetNotFoundProvider()
        {
            try
            {
                LoadAssembly();

                if (!IsAssemblyLoaded)
                    return null;

                var type = Assembly.GetType("Stimulsoft.Report.Design.Dictionary.StiNotFoundPackageProvider");
                if (type != null)
                    return StiActivator.CreateObject(type, new object[] { }) as IStiNotFoundProvider;
            }
            catch
            {
                return null;
            }

            return null;
        }
        #endregion
    }
}
