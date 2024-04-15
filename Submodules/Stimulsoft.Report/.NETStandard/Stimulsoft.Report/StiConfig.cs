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
using System.Linq;
using System.Xml;
using System.Text;
using System.IO;
using System.Globalization;
using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.BarCodes;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using Stimulsoft.Report.Dialogs;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.SaveLoad;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Chart;
using System.Threading;
using System.ComponentModel;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class which describes the configuration of the report engine.
    /// </summary>
    public sealed class StiConfig
    {
        #region Methods.Localization.Obsolete
        /// <summary>
        /// Loads localization from the xml file.
        /// </summary>
        /// <param name="file">A file from which localization will be loaded.</param>
        [Obsolete("Method StiConfig.LoadLocalization is obsolete. Please use method StiOptions.Localization.Load istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void LoadLocalization(string file)
        {
            StiOptions.Localization.Load(file);
        }

        /// <summary>
        /// Loads localization from the stream.
        /// </summary>
        /// <param name="stream">A stream from which localization will be loaded.</param>
        [Obsolete("Method StiConfig.LoadLocalization is obsolete. Please use method StiOptions.Localization.Load istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void LoadLocalization(Stream stream)
        {
            StiOptions.Localization.Load(stream);
        }

        [Obsolete("Method StiConfig.GetDirectoryLocalization is obsolete. Please use method StiOptions.Localization.GetDirectory istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetDirectoryLocalization()
        {
            return StiOptions.Localization.GetDirectory();
        }

        [Obsolete("Method StiConfig.LoadDefaultLocalization is obsolete. Please use method StiOptions.Localization.LoadDefault istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void LoadDefaultLocalization()
        {
            StiOptions.Localization.LoadDefault();
        }
        #endregion

        #region Methods.HideProperties.Obsolete
#if !NETSTANDARD
        [Obsolete("Method StiConfig.HideProperty is obsolete. Please use method StiOptions.Designer.Properties.Hide istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void HideProperty(Type type, string propertyName)
        {
            StiOptions.Designer.Properties.Hide(type, propertyName);
        }

        [Obsolete("Method StiConfig.HideProperty is obsolete. Please use method StiOptions.Designer.Properties.Hide istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void HideProperty(string propertyName)
        {
            StiOptions.Designer.Properties.Hide(propertyName);
        }

        [Obsolete("Method StiConfig.ShowProperty is obsolete. Please use method StiOptions.Designer.Properties.Show istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ShowProperty(Type type, string propertyName)
        {
            StiOptions.Designer.Properties.Show(type, propertyName);
        }

        [Obsolete("Method StiConfig.ShowProperty is obsolete. Please use method StiOptions.Designer.Properties.Show istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ShowProperty(string propertyName)
        {
            StiOptions.Designer.Properties.Show(propertyName);
        }

        [Obsolete("Method StiConfig.IsAllowedProperty is obsolete. Please use method StiOptions.Designer.Properties.IsAllowed istead it.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsAllowedProperty(Type type, string propertyName)
        {
            return StiOptions.Designer.Properties.IsAllowed(type, propertyName);
        }
#endif
        #endregion

        #region Methods
        private static void OnBeforeGetService(object sender, StiServiceActionEventArgs e)
        {
            #region StiServiceActionType.Get
            if (e.ActionType == StiServiceActionType.Get)
            {
                var type = sender as Type;
                if (type == null) return;

                e.Processed = true;

                if (type == typeof(StiAggregateFunctionService)) e.Services = StiOptions.Services.AggregateFunctions.ToList<StiService>();
                else if (type == typeof(StiBarCodeTypeService)) e.Services = StiOptions.Services.BarCodes.ToList<StiService>();

                else if (type == typeof(StiArea)) e.Services = StiOptions.Services.ChartAreas.ToList<StiService>();
                else if (type == typeof(StiSeriesLabels)) e.Services = StiOptions.Services.ChartSerieLabels.ToList<StiService>();
                else if (type == typeof(StiSeries)) e.Services = StiOptions.Services.ChartSeries.ToList<StiService>();
                else if (type == typeof(Chart.StiChartStyle)) e.Services = StiOptions.Services.ChartStyles.ToList<StiService>();
                else if (type == typeof(StiTrendLine)) e.Services = StiOptions.Services.ChartTrendLines.ToList<StiService>();

                else if (type == typeof(StiDataAdapterService)) e.Services = StiOptions.Services.DataAdapters.ToList<StiService>();
                else if (type == typeof(StiDatabase)) e.Services = StiOptions.Services.Databases.ToList<StiService>();
                else if (type == typeof(StiDataTableSetNameService)) e.Services = StiOptions.Services.DataTableSetName.ToList<StiService>();

                else if (type == typeof(StiDictionarySLService)) e.Services = StiOptions.Services.DictionarySLs.ToList<StiService>();
                else if (type == typeof(StiDocumentSLService)) e.Services = StiOptions.Services.DocumentSLs.ToList<StiService>();
                else if (type == typeof(StiPageSLService)) e.Services = StiOptions.Services.PageSLs.ToList<StiService>();
                else if (type == typeof(StiReportSLService)) e.Services = StiOptions.Services.ReportSLs.ToList<StiService>();

                else if (type == typeof(StiComponent)) e.Services = StiOptions.Services.Components.ToList<StiService>();

                else if (type == typeof(StiDialogsProvider)) e.Services = StiOptions.Services.DialogProviders.ToList<StiService>();

                else if (type == typeof(StiExportService)) e.Services = StiOptions.Services.Exports.ToList<StiService>();

                else if (type == typeof(StiFormatService)) e.Services = StiOptions.Services.Formats.ToList<StiService>();
                else if (type == typeof(StiLogService)) e.Services = StiOptions.Services.Logs.ToList<StiService>();
                else if (type == typeof(StiShapeTypeService)) e.Services = StiOptions.Services.Shapes.ToList<StiService>();

                else if (type == typeof(StiViewerConfigService)) e.Services = StiOptions.Services.ViewerConfig.ToList<StiService>();
                else if (type == typeof(StiDotMatrixViewerConfigService)) e.Services = StiOptions.Services.DotMatrixViewerConfig.ToList<StiService>();

                else e.Processed = false;
            }
            #endregion

            #region StiServiceActionType.Add
            if (e.ActionType == StiServiceActionType.Add)
            {
                foreach (var service in e.Services)
                {
                    e.Processed = true;

                    if (service is StiAggregateFunctionService) StiOptions.Services.AggregateFunctions.Add(service as StiAggregateFunctionService);
                    else if (service is StiBarCodeTypeService) StiOptions.Services.BarCodes.Add(service as StiBarCodeTypeService);

                    else if (service is StiArea) StiOptions.Services.ChartAreas.Add(service as StiArea);
                    else if (service is StiSeriesLabels) StiOptions.Services.ChartSerieLabels.Add(service as StiSeriesLabels);
                    else if (service is StiSeries) StiOptions.Services.ChartSeries.Add(service as StiSeries);
                    else if (service is Chart.StiChartStyle) StiOptions.Services.ChartStyles.Add(service as Chart.StiChartStyle);
                    else if (service is StiTrendLine) StiOptions.Services.ChartTrendLines.Add(service as StiTrendLine);

                    else if (service is StiDataAdapterService) StiOptions.Services.DataAdapters.Add(service as StiDataAdapterService);
                    else if (service is StiDatabase) StiOptions.Services.Databases.Add(service as StiDatabase);
                    else if (service is StiDataTableSetNameService) StiOptions.Services.DataTableSetName.Add(service as StiDataTableSetNameService);

                    else if (service is StiDictionarySLService) StiOptions.Services.DictionarySLs.Add(service as StiDictionarySLService);
                    else if (service is StiDocumentSLService) StiOptions.Services.DocumentSLs.Add(service as StiDocumentSLService);
                    else if (service is StiPageSLService) StiOptions.Services.PageSLs.Add(service as StiPageSLService);
                    else if (service is StiReportSLService) StiOptions.Services.ReportSLs.Add(service as StiReportSLService);

                    else if (service is StiComponent) StiOptions.Services.Components.Add(service as StiComponent);

                    else if (service is StiDialogsProvider) StiOptions.Services.DialogProviders.Add(service as StiDialogsProvider);

                    else if (service is StiExportService) StiOptions.Services.Exports.Add(service as StiExportService);
                    else if (service is StiFormatService) StiOptions.Services.Formats.Add(service as StiFormatService);
                    else if (service is StiLogService) StiOptions.Services.Logs.Add(service as StiLogService);
                    else if (service is StiShapeTypeService) StiOptions.Services.Shapes.Add(service as StiShapeTypeService);

                    else if (service is StiViewerConfigService) StiOptions.Services.ViewerConfig.Add(service as StiViewerConfigService);
                    else if (service is StiDotMatrixViewerConfigService) StiOptions.Services.DotMatrixViewerConfig.Add(service as StiDotMatrixViewerConfigService);

                    else e.Processed = false;
                }
            }
            #endregion
        }

        /// <summary>
        /// Disables logging and hides messages of the report engine.
        /// </summary>
        [Obsolete("The method StiConfig.InitWeb is obsolete! Please use method StiOptions.Configuration.InitWeb.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitWeb()
        {
            StiOptions.Configuration.InitWeb();
        }

        /// <summary>
        /// Saves a configuration of the report engine.
        /// </summary>
        public static void Save()
        {
            if (StiOptions.Engine.DontSaveConfig) return;

            if (StiOptions.Engine.AllowSetCurrentDirectory)
                Environment.CurrentDirectory = StiOptions.Configuration.ApplicationDirectory;
            Save(StiPath.GetPath(GetDefaultReportConfigPath()));
        }

        /// <summary>
        /// Saves a configuration of the report engine.
        /// </summary>
        /// <param name="file">A file for saving a configuration of the report.</param>
        public static void Save(string file)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(file);
                stream = new FileStream(file, FileMode.Create);
                Save(stream);

            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }

        /// <summary>
        /// Saves a configuration of the report engine.
        /// </summary>
        /// <param name="stream">A stream for saving configuration of the report engine.</param>
        public static void Save(Stream stream)
        {
            Save(stream, Services);
        }

        /// <summary>
        /// Saves a configuration of the report engine.
        /// </summary>
        /// <param name="stream">A stream for saving configuration of the report engine.</param>
        /// <param name="services">A container which contains services of the report.</param>
        public static void Save(Stream stream, StiServiceContainer services)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                StiLogService.Write(typeof(StiReport), "Saving report configuration");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                using (var tw = new XmlTextWriter(stream, Encoding.UTF8))
                {

                    tw.Formatting = Formatting.Indented;
                    tw.WriteStartDocument(true);

                    tw.WriteStartElement("StiSerializer");
                    tw.WriteAttributeString("version", "1.0");
                    tw.WriteAttributeString("application", "StiReportConfig");

                    #region Services
                    services.Save(tw);
                    #endregion

                    tw.WriteEndElement();
                    tw.Flush();
                    tw.Close();
                }
            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Saving report configuration...ERROR");
                StiLogService.Write(typeof(StiReport), e);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        /// <summary>
        /// Not used more!
        /// </summary>
        [Obsolete("Method StiConfig.Restore is obsolete!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Restore()
        {
        }

        /// <summary>
        /// Not used more!
        /// </summary>
        [Obsolete("Method StiConfig.Synchronize is obsolete!")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Synchronize()
        {
        }

        /// <summary>
        /// Loads a configuration of the report engine if it exist.
        /// </summary>
        public static void Load()
        {
            if (StiOptions.Engine.AllowSetCurrentDirectory)
                Environment.CurrentDirectory = StiOptions.Configuration.ApplicationDirectory;

            var path = StiPath.GetPath(GetDefaultReportConfigPath());
            if (!File.Exists(path)) return;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                Load(stream);
            }
        }

        /// <summary>
        /// Loads a configuration of the report engine.
        /// </summary>
        /// <param name="file">A file for loading a configuration of the report.</param>
        public static void Load(string file)
        {
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        /// <summary>
        /// Loads a configuration of the report engine.
        /// </summary>
        /// <param name="stream">A stream for loading configuration of the report engine.</param>
        public static void Load(Stream stream)
        {
            Load(stream, Services);
        }

        /// <summary>
        /// Loads a configuration of the report engine.
        /// </summary>
        /// <param name="stream">A stream for loading configuration of the report engine.</param>
        /// <param name="services">A container which contains services of the report.</param>
        public static void Load(Stream stream, StiServiceContainer services)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                StiLogService.Write(typeof(StiReport), "Loading report configuration");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                using (var tr = new XmlTextReader(stream))
                {
                    tr.DtdProcessing = DtdProcessing.Ignore;
                    tr.Read();
                    tr.Read();

                    if (tr.IsStartElement())
                    {
                        if (tr.Name == "StiSerializer")
                        {
                            string app = tr.GetAttribute("application");
                            if (app == "StiReportConfig")
                            {
                                #region Skip Localization if exist
                                while (tr.Read() && tr.Name != "Services") ;
                                #endregion

                                #region Services
                                services.Load(tr);
                                #endregion

                                Thread.CurrentThread.CurrentCulture = currentCulture;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                StiLogService.Write(typeof(StiReport), "Loading report configuration...ERROR");
                StiLogService.Write(typeof(StiReport), e);
                throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        public static string GetDefaultReportConfigPath()
        {
            var path = StiOptions.Configuration.DefaultReportConfigPath;
            if (!string.IsNullOrEmpty(path)) return path;

            path = Path.Combine(StiOptions.Configuration.ApplicationDirectory, "Stimulsoft.Report.config");
            if (File.Exists(path)) return path;

            if (!StiOptions.Engine.FullTrust) return path;

            path = "Stimulsoft\\Stimulsoft.Report.config";
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (folder.Length != 0) path = Path.Combine(folder, path);
            folder = Path.GetDirectoryName(path);

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return path;
        }

        public static void Reset()
        {
            if (Services != null)
                Services.BeforeGetService -= OnBeforeGetService;

            Services = new StiServiceContainer();
            Services.BeforeGetService += OnBeforeGetService;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value which indicates whether configuration is used for the web report.
        /// </summary>
        [Obsolete("The property StiConfig.IsWeb is obsolete! Please use property StiOptions.Configuration.IsWeb.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsWeb
        {
            get
            {
                return StiOptions.Configuration.IsWeb;
            }
            set
            {
                StiOptions.Configuration.IsWeb = value;
            }
        }

        /// <summary>
        /// Gets a value which indicates whether configuration is used for the WinForms report.
        /// </summary>
        [Obsolete("The property StiConfig.IsWinForms is obsolete! Please use property StiOptions.Configuration.IsWinForms.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsWinForms
        {
            get
            {
                return StiOptions.Configuration.IsWinForms;
            }
        }

        /// <summary>
        /// Gets a value which indicates whether configuration is used for the WPF report.
        /// </summary>
        [Obsolete("The property StiConfig.IsWPF is obsolete! Please use property StiOptions.Configuration.IsWPF.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsWPF
        {
            get
            {
                return StiOptions.Configuration.IsWPF;
            }
            set
            {
                StiOptions.Configuration.IsWPF = value;
            }
        }

        /// <summary>
        /// Gets a value which indicates whether configuration is used for the Server.
        /// </summary>
        [Obsolete("The property StiConfig.IsServer is obsolete! Please use property StiOptions.Configuration.IsServer.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsServer
        {
            get
            {
                return StiOptions.Configuration.IsServer;
            }
            set
            {
                StiOptions.Configuration.IsServer = value;
            }
        }

        /// <summary>
        /// Gets or sets a default name of the file which is used for configuration of a report.
        /// </summary>
        [Obsolete("Property StiConfig.DefaultReportConfigPath is obsolete! Please use StiOptions.Configuration.DefaultReportConfigPath.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string DefaultReportConfigPath
        {
            get
            {
                return StiOptions.Configuration.DefaultReportConfigPath;
            }
            set
            {
                StiOptions.Configuration.DefaultReportConfigPath = value;
            }
        }

        /// <summary>
        /// Gets or sets an application directory.
        /// </summary>
        [Obsolete("Property StiConfig.ApplicationDirectory is obsolete! Please use StiOptions.Configuration.ApplicationDirectory.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string ApplicationDirectory
        {
            get
            {
                return StiOptions.Configuration.ApplicationDirectory;
            }
            set
            {
                StiOptions.Configuration.ApplicationDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets a collection of services.
        /// </summary>
        public static StiServiceContainer Services { get; set; } = new StiServiceContainer();
        #endregion

        static StiConfig()
        {
            Services.BeforeGetService += OnBeforeGetService;
        }
    }
}