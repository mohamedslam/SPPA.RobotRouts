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

using Stimulsoft.Base.Exceptions;
using Stimulsoft.Base.Server;
using System.Reflection;

namespace Stimulsoft.Base
{
    internal static class StiLibExcellHelper
    {
        #region Const
        public const string AssemblyName = "Stimulsoft.LibExcel";
        public const string AssemblyVersion = "2022.4.1";
        #endregion

        #region Fields.Static
        private static object lockObject = new object();
        public static string LibExcellAssemblyPrefix = "Stimulsoft.";
        #endregion

        #region Properties.Status
        private static Assembly libExcellAssembly;
        public static Assembly LibExcellAssembly
        {
            get
            {
                lock (lockObject)
                {
                    if (libExcellAssembly == null && !TryToLoadAssembly)
                    {
                        TryToLoadAssembly = true;

                        libExcellAssembly = StiAssemblyFinder.GetAssembly($"{AssemblyName}.dll");

                        if (libExcellAssembly == null)
                        {
                            LibExcellAssemblyPrefix = string.Empty;
                            libExcellAssembly = StiAssemblyFinder.GetAssembly("LibExcel.dll");
                        }

                        if (libExcellAssembly == null)
                        {
                            if (StiAccountSettings.CurrentAccount != null)
                            {
                                if (!StiBaseOptions.IsDashboardViewerWPF)
                                {
                                    try
                                    {
                                        var downloader = StiAccountSettings.CurrentAccount.AccountCreater.GetNuGetDownloader();
                                        downloader.RunDownloadLibExcell();

                                        libExcellAssembly = StiAssemblyFinder.GetAssembly($"{AssemblyName}.dll");
                                        LibExcellAssemblyPrefix = "Stimulsoft.";
                                    }
                                    catch { }
                                }
                            }

                            else
                            {
                                var provider = StiNotFoundProviderCreater.GetNotFoundProvider();
                                provider?.Show($"{AssemblyName}");
                            }
                        }
                    }

                    return libExcellAssembly;
                }
            }
        }

        public static bool TryToLoadAssembly { get; set; }
        #endregion
    }
}
