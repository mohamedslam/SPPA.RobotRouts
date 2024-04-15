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

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.SaveLoad;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Drawing.Printing;

#if SERVER
using Stimulsoft.Server;
using Stimulsoft.Server.Connect;
using Stimulsoft.Server.Objects;
#endif

#if NETSTANDARD
using Stimulsoft.System.Web.UI;
using Stimulsoft.System.Web.UI.WebControls;
#else
using System.Web.UI;
using System.Web.UI.WebControls;
using Stimulsoft.Report.Design.Forms;
#endif

namespace Stimulsoft.Report.Web
{
    public partial class StiWebDesigner :
        WebControl,
        INamingContainer
    {
        #region Check file formats

        /// <summary>
        /// Returns true if specified stream contains packed report.
        /// </summary>
        private static bool IsPackedFile(Stream stream)
        {
            if (stream.Length < 4) return false;

            int first = stream.ReadByte();
            int second = stream.ReadByte();
            int third = stream.ReadByte();
            stream.Seek(-3, SeekOrigin.Current);

            if (first == 0x1F && second == 0x8B && third == 0x08) return true;//Gzip
            if (first == 0x50 && second == 0x4B && third == 0x03) return true;//PKZip

            return false;
        }

        /// <summary>
        /// Returns true if specified stream contains encrypted report.
        /// </summary>
        private static bool IsEncryptedFile(Stream stream)
        {
            if (stream.Length < 4) return false;

            int first = stream.ReadByte();
            int second = stream.ReadByte();
            int third = stream.ReadByte();
            stream.Seek(-3, SeekOrigin.Current);

            if (first == 0x6D && second == 0x64 && third == 0x78) return true;  //mdx
            return false;
        }

        /// <summary>
        /// Returns true if specified stream contains packed report.
        /// </summary>
        private static bool IsJsonFile(Stream stream)
        {
            if (stream.Length < 2) return false;

            int first = stream.ReadByte();
            stream.Seek(-1, SeekOrigin.Current);

            if (first == 0x0000007b) return true;//JSON
            return false;
        }

        #endregion

        #region Helpers for commands

        internal static StiReport GetReportForDesigner(StiRequestParams requestParams, StiReport currentReport)
        {
            var designerOptions = StiDesignerOptionsHelper.GetDesignerOptions(requestParams);

            #region Get Report Server Mode
            if (requestParams.ServerMode)
            {
#if SERVER
            
                if (!string.IsNullOrEmpty(requestParams.GetString("stiweb_parameters")))
                {
                    try
                    {
                        var stiweb_parameters = JsonConvert.DeserializeObject<Hashtable>(Encoding.UTF8.GetString(Convert.FromBase64String(requestParams.GetString("stiweb_parameters"))));
                        var cloudParameters = (Hashtable)((JObject)stiweb_parameters["cloudParameters"]).ToObject(typeof(Hashtable));

                        if (cloudParameters.Contains("reportTemplateItemKey"))
                        {
                            var getReportForDesignCommand = new StiReportCommands.Design()
                            {
                                ReportTemplateItemKey = (string)cloudParameters["reportTemplateItemKey"],
                                SessionKey = (string)cloudParameters["sessionKey"]
                            };

                            if (cloudParameters.ContainsKey("versionKey"))
                            {
                                getReportForDesignCommand.VersionKey = (string)cloudParameters["versionKey"];
                            }

                            var resultReportForDesignCommand = StiCommandToServer.RunCommand(getReportForDesignCommand) as StiReportCommands.Design;

                            if (resultReportForDesignCommand.ResultSuccess && resultReportForDesignCommand.ResultReport != null)
                            {
                                StiReport newReport = new StiReport();

                                newReport.Load(new MemoryStream(resultReportForDesignCommand.ResultReport));

                                currentReport = newReport;
                            }
                        }
                        else
                        {
                            if (Convert.ToBoolean(cloudParameters["isDashboard"]))
                                currentReport = GetNewDashboard(requestParams);
                            else
                                currentReport = GetNewReport(requestParams);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
#endif
            }
            #endregion

            #region Get Report Standart Mode
            else
            {
                if (currentReport == null)
                {
                    var startScreen = designerOptions != null ? designerOptions["startScreen"] as string : "Welcome";

                    if (startScreen == "BlankReport")
                        currentReport = GetNewReport(requestParams);
                    else if (startScreen == "BlankDashboard")
                        currentReport = GetNewDashboard(requestParams);

                    if (currentReport != null)
                    {
                        currentReport.Info.Zoom = 1;

                        if (requestParams.Designer.IsNewReport)
                            currentReport.ReportFile = string.Empty;
                        else if (string.IsNullOrEmpty(currentReport.ReportFile))
                            currentReport.ReportFile = !string.IsNullOrWhiteSpace(currentReport.ReportName) ? currentReport.ReportName + ".mrt" : "Report.mrt";
                    }
                }
            }
            #endregion

            if (currentReport != null)
                StiDesignerOptionsHelper.ApplyDesignerOptionsToReport(designerOptions, currentReport);

            return currentReport;
        }

        internal static StiReport GetNewReport(StiRequestParams requestParams)
        {
            var newReport = new StiReport();

            if (!StiOptions.Engine.FullTrust || requestParams.CloudMode)
                newReport.CalculationMode = StiCalculationMode.Interpretation;

            newReport.ReportUnit = (StiReportUnitType)requestParams.GetEnum("defaultUnit", typeof(StiReportUnitType));
            newReport.Pages[0].PaperSize = PaperKind.Custom;
            newReport.Pages[0].PageWidth = newReport.Unit.ConvertFromHInches(827d);
            newReport.Pages[0].PageHeight = newReport.Unit.ConvertFromHInches(1169d);
            newReport.Info.Zoom = 1;

            return newReport;
        }

        internal static StiReport GetNewDashboard(StiRequestParams requestParams)
        {
            var newDashboard = StiReport.CreateNewDashboard();

            if (!StiOptions.Engine.FullTrust || requestParams.CloudMode)
                newDashboard.CalculationMode = StiCalculationMode.Interpretation;

            newDashboard.Info.Zoom = 1;

            return newDashboard;
        }

        internal static StiReport GetNewForm(StiRequestParams requestParams)
        {
            var newForm = StiReport.CreateNewForm();

            if (!StiOptions.Engine.FullTrust || requestParams.CloudMode)
                newForm.CalculationMode = StiCalculationMode.Interpretation;

            newForm.Info.Zoom = 1;

            return newForm;
        }

        internal static StiReport LoadReportFromContent(StiRequestParams requestParams)
        {
            var report = new StiReport();

            if (requestParams.Data != null && requestParams.Data.Length > 0)
            {
                var stream = new MemoryStream();
                stream.Write(requestParams.Data, 0, requestParams.Data.Length);
                StiReportSLService service = null;
                stream.Position = 0;

                if (requestParams.Designer.Password != null) report.LoadEncryptedReport(stream, requestParams.Designer.Password);
                else if (IsPackedFile(stream)) report.LoadPackedReport(stream);
                else
                {
                    if (IsJsonFile(stream)) service = new StiJsonReportSLService();
                    else service = new StiXmlReportSLService();

                    report.Load(service, stream);
                }
                stream.Close();
            }
            
            report.Info.Zoom = 1;
            if (requestParams.Designer.FileName != null) report.ReportFile = requestParams.Designer.FileName;
            if (!StiOptions.Engine.FullTrust) report.CalculationMode = StiCalculationMode.Interpretation;

            return report;
        }

        #endregion

        #region URLs (.NET Core)
#if NETSTANDARD
        /// <summary>
        /// Get the URL to load the images of the report designer.
        /// </summary>
        internal static string GetImageUrl(StiRequestParams requestParams, string imageName)
        {
            return imageName.Replace("'", "\\'").Replace("\"", "&quot;");
        }
#endif
        #endregion

        #region Error Messages
        internal static string GetErrorMessageText(Exception e)
        {
            if (e == null) return string.Empty;

            return (!string.IsNullOrEmpty(e.Message) ? e.Message : string.Empty) +
                (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? $" {e.InnerException.Message}" : string.Empty);
        }
        #endregion
    }
}
