#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.IO;

namespace Stimulsoft.Base.Exceptions
{
    public sealed class StiUnsaveReportRepository
    {
        #region Methods
        private static string GetRootUnsavedReportFolder()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft", "UnsavedReports");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        public static string GetReportPath(string reportName)
        {
            var path = GetRootUnsavedReportFolder();

            if (string.IsNullOrEmpty(reportName))
                reportName = "Report";

            var date = DateTime.Now.ToString("yyyyMMddThhmmss");

            return $"{path}\\{reportName}-{date}.mrt";
        }

        public static string[] GetReportPaths()
        {
            var dir = GetRootUnsavedReportFolder();

            return Directory.GetFiles(dir, "*.mrt"); ;
        }

        public static bool IsAvailableReports()
        {
            var paths = GetReportPaths();
            return paths != null && paths.Length > 0;
        }

        public static void DeleteRootUnsavedReportFolder()
        {
            try
            {
                var path = GetRootUnsavedReportFolder();
                Directory.Delete(path, true);
            }
            catch { }
        }

        public static void DeleteUnsavedReportFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }
        #endregion
    }
}
