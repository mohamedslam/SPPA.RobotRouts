#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Report.Dashboard.Export;
using Stimulsoft.Report.Export;
using System;
using System.IO;
using System.Linq;

namespace Stimulsoft.Report.Dashboard
{
    internal static class StiDashboardExport
    {
        #region Methods
        public static void ExportDocument(StiReport report, Stream stream)
        {
            if (!StiDashboardExportAssembly.IsAssemblyLoaded)
                throw new Exception("Assembly 'Stimulsoft.Dashboard.Export' is not found!");

            var exportSettings = StiInvokeMethodsHelper.InvokeStaticMethod(
                "Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { null },
                new[] { typeof(StiExportSettings) });

            Export(report, stream, (IStiDashboardExportSettings)exportSettings);
        }

        public static void Export(StiReport report, Stream stream, StiExportSettings settings)
        {
            if (!StiDashboardExportAssembly.IsAssemblyLoaded)
                throw new Exception("Assembly 'Stimulsoft.Dashboard.Export' is not found!");

            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var exportSettings = StiInvokeMethodsHelper.InvokeStaticMethod(
                "Stimulsoft.Dashboard.Export", "Helpers.StiExportSettingsHelper", "GetDashboardExportSettings",
                new object[] { settings },
                new[] { typeof(StiExportSettings) });

            if (settings.GetExportFormat() == StiExportFormat.Pdf && ((StiPdfExportSettings)settings).AutoPrintMode == StiPdfAutoPrintMode.Dialog)
                StiInvokeMethodsHelper.SetPropertyValue(exportSettings, "AutoPrint", true);

            Export(report, stream, (IStiDashboardExportSettings)exportSettings);
        }

        public static void Export(StiReport report, Stream stream, IStiDashboardExportSettings settings)
        {
            if (!StiDashboardExportAssembly.IsAssemblyLoaded)
                throw new Exception("Assembly 'Stimulsoft.Dashboard.Export' is not found!");

            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var dashboardStream = StiInvokeMethodsHelper.InvokeStaticMethod(
               "Stimulsoft.Dashboard.Export", "StiDashboardExportTools", "ExportToStream",
               new[] { report, (object)settings },
               new[] { typeof(IStiReport), StiInvokeMethodsHelper.GetType("Stimulsoft.Dashboard.Export", "Settings.StiDashboardExportSettings") });

            var memoryStream = dashboardStream as MemoryStream;
            if (memoryStream != null)
            {
                memoryStream.Position = 0;
                memoryStream.CopyTo(stream);
            }
        }
        #endregion
    }
}
