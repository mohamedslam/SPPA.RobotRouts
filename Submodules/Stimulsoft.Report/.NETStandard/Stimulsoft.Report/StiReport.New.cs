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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.App;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Design.Forms;

namespace Stimulsoft.Report
{
    public partial class StiReport
    {
        public static StiReport CreateNewReport()
        {
            return StiActivator.CreateObject(StiOptions.Engine.BaseReportType) as StiReport;
        }

        public static StiReport CreateNewDashboard()
        {
            var report = CreateNewReport();
            report.CalculationMode = StiCalculationMode.Interpretation;
            report.Pages.Clear();
            report.IndexName = 1;

            if (!StiDashboardAssembly.IsAssemblyLoaded)
                StiDashboardAssembly.LoadAssembly();

            var dashboard = StiDashboardCreator.CreateDashboard(report) as StiPage;
            if (dashboard == null)
                dashboard = StiActivator.CreateObject("Stimulsoft.Dashboard.Components.StiDashboard") as StiPage;

            if (dashboard == null)
                throw new StiDashboardAssemblyIsNotFoundException();

            dashboard.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(dashboard));
            report.Pages.Add(dashboard);
            
            return report;
        }

        public static StiReport CreateNewScreen()
        {
            var report = CreateNewReport();
            report.CalculationMode = StiCalculationMode.Interpretation;
            report.Pages.Clear();
            report.IndexName = 1;

            if (!StiAppAssembly.IsAssemblyLoaded)
                StiAppAssembly.LoadAssembly();

            var screen = StiAppCreator.CreateScreen(report) as StiPage;
            if (screen == null)
                screen = StiActivator.CreateObject("Stimulsoft.App.Components.StiScreen") as StiPage;

            if (screen == null)
                throw new StiAppAssemblyIsNotFoundException();

            screen.Name = StiNameCreation.CreateName(report, StiNameCreation.GenerateName(screen));
            report.Pages.Add(screen);

            return report;
        }

        public static StiReport CreateNewForm()
        {
            var report = CreateNewReport();
            report.Pages.Clear();
            report.IndexName = 1;

            if (!StiFormAssembly.IsAssemblyLoaded)
                StiFormAssembly.LoadAssembly();

            try
            {
                var formContainer = new StiFormContainer();
                report.Pages.Add(formContainer);
            }
            catch
            {
                throw new StiFormAssemblyIsNotFoundException();
            }

            return report;
        }
    }
}