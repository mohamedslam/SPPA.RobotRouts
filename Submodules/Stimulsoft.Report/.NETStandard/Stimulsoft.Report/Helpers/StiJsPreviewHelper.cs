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

using System.IO;
using System.Reflection;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;

namespace Stimulsoft.Report.Helpers
{
    internal static class StiJsPreviewHelper
    {
        #region Consts
        internal const string CssFileName = "stimulsoft.viewer.office2013.whiteblue.css";
        internal const string ReportsFileName = "stimulsoft.reports.js";
        internal const string ReportsMapsFileName = "stimulsoft.reports.maps.js";
        internal const string ViewerFileName = "stimulsoft.viewer.js";
        #endregion

        #region Properties
        internal static bool IsAvailable
        {
            get
            {
                return GetReportsFile() != null
                    && GetReportsMapsFile() != null
                    && GetViewerFile() != null 
                    && GetCssFile() != null;
            }
        }

        private static string GetAssemblyFolder()
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly != null ? Path.GetDirectoryName(assembly.Location) : null;
        }
        #endregion

        #region Methods
        internal static string GetViewerFile()
        {
            return GetFile(ViewerFileName);
        }
        
        internal static string GetReportsFile()
        {
            return GetFile(ReportsFileName);
        }

        internal static string GetReportsMapsFile()
        {
            return GetFile(ReportsMapsFileName);
        }

        internal static string GetCssFile()
        {
            return GetFile(CssFileName);
        }

        internal static string GetFile(string name)
        {
            var file = GetJsFileFromEntryAssembly(name);
            if (file != null)
                return file;

            return GetJsFileFromLibs(name);
        }

        private static string GetJsFileFromEntryAssembly(string name)
        {
            try
            {
                var assemblyFolder = GetAssemblyFolder();
                if (assemblyFolder == null || !Directory.Exists(assemblyFolder))
                    return null;
                
                var path = Path.Combine(assemblyFolder, name);
                return File.Exists(path) ? path : null;
            }
            catch
            {
            }

            return null;
        }

        private static string GetJsFileFromLibs(string name)
        {
            try
            {
                var libFolder = StiLibStatus.GetJsLibPathIfExists();
                if (libFolder == null || !Directory.Exists(libFolder))
                    return null;

                var subFolder = name.EndsWithInvariantIgnoreCase("css") ? "Css" : "Scripts";

                var path = Path.Combine(libFolder, subFolder, name);
                return File.Exists(path) ? path : null;
            }
            catch
            {
            }

            return null;
        }
        #endregion
    }
}