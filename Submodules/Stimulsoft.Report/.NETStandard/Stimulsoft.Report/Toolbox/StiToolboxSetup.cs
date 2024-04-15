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

using Stimulsoft.Base;
using Stimulsoft.Base.Json;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stimulsoft.Report.Toolbox
{
    public static class StiToolboxSetup
    {
        static StiToolboxSetup()
        {
            Load();
        }

        public class StiBaseInfo
        {
            #region Properties
            public List<string> VisibleComponents { get; set; } = new List<string>();

            public List<string> HideComponents { get; set; } = new List<string>();

            public List<string> NewComponents { get; set; } = new List<string>()
            {
                //Artem: add in the future new component after 2022.4.1 build.
                //exp. add: typeof(StiPdfDigitalSignature).FullName
            };
            #endregion

            #region Methods
            public void ClearStateComponents()
            {
                VisibleComponents.Clear();
                HideComponents.Clear();
            }

            public void WriteStateComponent(string name, bool state)
            {
                if (state)
                    VisibleComponents.Add(name);

                else
                    HideComponents.Add(name);
            }

            public bool IsComponentVisible(StiComponent comp)
            {
                var fullName = comp.GetType().FullName;

                if (this.VisibleComponents.Contains(fullName)) return true;

                if (this.HideComponents.Contains(fullName)) return false;

                return this.NewComponents.Contains(fullName); ;
            }
            #endregion
        }

        #region class StiReportsInfo
        public sealed class StiReportsInfo : StiBaseInfo
        {
            internal StiReportsInfo() { }

            #region Properties
            public bool ShowBandsCategory { get; set; }

            public bool ShowCrossBandsCategory { get; set; }

            public bool ShowComponentsCategory { get; set; }

            public bool ShowBarCodesCategory { get; set; }

            public bool ShowShapesCategory { get; set; }

            public bool ShowChartCategory { get; set; }

            public bool ShowGaugeCategory { get; set; }

            public bool ShowMapCategory { get; set; }

            public bool ShowSignaturesCategory { get; set; } = true;
            #endregion            
        }
        #endregion

        #region class StiDashbordsInfo
        public sealed class StiDashbordsInfo : StiBaseInfo
        {
            internal StiDashbordsInfo() { }
        }
        #endregion

        #region class StiComponentUIsInfo
        public sealed class StiComponentUIsInfo : StiBaseInfo
        {
            internal StiComponentUIsInfo() 
            { 
            }
        }
        #endregion

        #region class StiDialogsInfo
        public sealed class StiDialogsInfo : StiBaseInfo
        {
            internal StiDialogsInfo() { }
        }
        #endregion

        #region class StiContainerInfo
        private class StiContainerInfo
        {
            public StiReportsInfo Reports = new StiReportsInfo();

            public StiDashbordsInfo Dashboards = new StiDashbordsInfo();
            
            public StiComponentUIsInfo ComponentUIs = new StiComponentUIsInfo();

            public StiDialogsInfo Dialogs = new StiDialogsInfo();

            public List<string> CollapsedCategories = new List<string>();
        }
        #endregion

        #region Properties
        public static StiReportsInfo Reports { get; private set; } = new StiReportsInfo();

        public static StiDashbordsInfo Dashboards { get; private set; } = new StiDashbordsInfo();

        public static StiDialogsInfo Dialogs { get; private set; } = new StiDialogsInfo();
        
        public static StiComponentUIsInfo ComponentUIs { get; private set; } = new StiComponentUIsInfo();

        public static List<string> CollapsedCategories { get; private set; } = new List<string>();
        #endregion

        #region Methods
        private static string GetSettingsPath()
        {
            string path = "Stimulsoft" + Path.DirectorySeparatorChar + "ToolboxSetup.json";
            string folder;

            try
            {
                if (StiOptions.Engine.FullTrust)
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (folder.Length != 0) 
                        path = Path.Combine(folder, path);
                }

                folder = Path.GetDirectoryName(path);

                if (StiOptions.Engine.FullTrust && !string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder)) 
                    Directory.CreateDirectory(folder);
            }
            catch
            {
                try
                {
                    path = "Stimulsoft" + Path.DirectorySeparatorChar + "ToolboxSetup.json";
                    folder = "Stimulsoft";

                    if (StiOptions.Engine.FullTrust && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    path = "ToolboxSetup.json";
                }
            }

            return path;
        }

        public static void LoadFromString(string text)
        {
            var container = JsonConvert.DeserializeObject<StiContainerInfo>(text);

            Reports = container.Reports;
            Dashboards = container.Dashboards;
            Dialogs = container.Dialogs;
            ComponentUIs = container.ComponentUIs;
            CollapsedCategories = container.CollapsedCategories;
        }

        public static void Load()
        {
            bool isLoaded = false;

            try
            {
                var path = GetSettingsPath();
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    LoadFromString(text);

                    isLoaded = true;
                }
            }
            catch
            {
            }

            if (!isLoaded)
            {
                Reports = new StiReportsInfo();
                Dashboards = new StiDashbordsInfo();
                Dialogs = new StiDialogsInfo();
                ComponentUIs = new StiComponentUIsInfo();
                CollapsedCategories = new List<string>();

                InitDefaultSettings();
            }
        }

        public static string SaveToString()
        {
            var container = new StiContainerInfo
            {
                Reports = Reports,
                Dashboards = Dashboards,
                ComponentUIs = ComponentUIs,
                Dialogs = Dialogs,
                CollapsedCategories = CollapsedCategories
            };

            return JsonConvert.SerializeObject(container, Formatting.Indented);
        }

        public static void Save()
        {
            try
            {
                var json = SaveToString();
                var path = GetSettingsPath();
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {                
            }
        }

        public static void InitDefaultSettings()
        {
            #region Reports
            Reports.ShowBandsCategory = true;
            Reports.ShowCrossBandsCategory = true;
            Reports.ShowComponentsCategory = true;
            Reports.ShowBarCodesCategory = true;
            Reports.ShowShapesCategory = true;
            Reports.ShowChartCategory = true;
            Reports.ShowGaugeCategory = true;
            Reports.ShowMapCategory = true;
            Reports.ShowSignaturesCategory = true;

            Reports.ClearStateComponents();

            foreach (var service in StiOptions.Services.Components.Where(s => !(s is IStiElement) && !(s is IStiReportControl)))
            {
                Reports.WriteStateComponent(service.GetType().FullName, service.PlaceOnToolbox);
            }
            #endregion

            #region Dashboards
            Dashboards.ClearStateComponents();

            foreach (var service in StiOptions.Services.Components.Where(s => s is IStiElement))
            {
                Dashboards.WriteStateComponent(service.GetType().FullName, service.PlaceOnToolbox);
            }
            #endregion

            #region Dialogs
            Dialogs.ClearStateComponents();

            foreach (var service in StiOptions.Services.Components.Where(s => s is IStiReportControl))
            {
                Dialogs.WriteStateComponent(service.GetType().FullName, service.PlaceOnToolbox);
            }
            #endregion

            #region ComponentUIs
            ComponentUIs.ClearStateComponents();

            foreach (var service in StiOptions.Services.Components.Where(s => s is IStiComponentUI))
            {
                ComponentUIs.WriteStateComponent(service.GetType().FullName, service.PlaceOnToolbox);
            }
            #endregion
        }
        #endregion
    }
}