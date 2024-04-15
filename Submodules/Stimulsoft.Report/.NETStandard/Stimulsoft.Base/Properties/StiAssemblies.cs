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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;

namespace Stimulsoft.Base
{
    public class StiAssemblies
    {
        public const string Designer = "Designer";
        public const string Monitor = "Monitor";
        public const string Navigator = "Navigator";
        public const string Navigator_Web = "Navigator.Web";

        public const string RunMe_Test = "RunMe.Test";

        public const string TestBrowser = "TestBrowser";
        public const string Stimulsoft_Report_Wpf_Test = "Stimulsoft.Report.Wpf.Test";
        public const string SendEmail = "SendEmail";

        public const string Server_Console = "Server.Console";

        public const string CloudViewer_Web = "CloudViewer.Web";
        public const string CloudDesigner_Web = "CloudViewer.Web";
        public const string CloudShare_Web = "CloudShare.Web";

        public const string Stimulsoft_BlocklyEditor_Wpf = "Stimulsoft.BlocklyEditor.Wpf";

        public const string Stimulsoft_Accounts_Wpf = "Stimulsoft.Accounts.Wpf";
        public const string Stimulsoft_Wizard_Wpf = "Stimulsoft.Wizard.Wpf";

        public const string Stimulsoft_Blockly = "Stimulsoft.Blockly";

        public const string Stimulsoft_Client = "Stimulsoft.Client";
        public const string Stimulsoft_Client_Designer = "Stimulsoft.Client.Designer";
        public const string Stimulsoft_Client_Web = "Stimulsoft.Client.Web";
        public const string Stimulsoft_Cloud_Test = "Stimulsoft.Cloud.Test";

        public const string Stimulsoft_Dashboard = "Stimulsoft.Dashboard";
        public const string Stimulsoft_Dashboard_Design = "Stimulsoft.Dashboard.Design";
        public const string Stimulsoft_Dashboard_Drawing = "Stimulsoft.Dashboard.Drawing";
        public const string Stimulsoft_Dashboard_Drawing_Wpf = "Stimulsoft.Dashboard.Drawing.Wpf";
        public const string Stimulsoft_Dashboard_Export = "Stimulsoft.Dashboard.Export";
        public const string Stimulsoft_Dashboard_Viewer = "Stimulsoft.Dashboard.Viewer";
        public const string Stimulsoft_Dashboard_Viewer_Wpf = "Stimulsoft.Dashboard.Viewer.Wpf";
        public const string Stimulsoft_Controls_Wpf = "Stimulsoft.Controls.Wpf";
        public const string Stimulsoft_Dashboard_Wpf = "Stimulsoft.Dashboard.Wpf";
        public const string Stimulsoft_Dashboard_Test = "Stimulsoft.Dashboard.Test";

        public const string Stimulsoft_Data = "Stimulsoft.Data";

        public const string Stimulsoft_App = "Stimulsoft.App";
        public const string Stimulsoft_App_Drawing = "Stimulsoft.App.Drawing";
        public const string Stimulsoft_App_Design = "Stimulsoft.App.Design";
        public const string Stimulsoft_App_Viewer = "Stimulsoft.App.Viewer";

        public const string Stimulsoft_Designer_Wpf = "Designer.Wpf";
        public const string Stimulsoft_DesignerV2_Wpf = "DesignerV2.Wpf";
        public const string Stimulsoft_LicenseActivator = "LicenseActivator";
        public const string Stimulsoft_Server_Activator = "Stimulsoft.Server.Activator";

        public const string Stimulsoft_Map = "Stimulsoft.Map";

        public const string Stimulsoft_Report = "Stimulsoft.Report";        
        public const string Stimulsoft_Report_Blazor = "Stimulsoft.Report.Blazor";
        public const string Stimulsoft_Report_Angular = "Stimulsoft.Report.Angular";
        public const string Stimulsoft_Report_Check = "Stimulsoft.Report.Check";        
        public const string Stimulsoft_Report_Design = "Stimulsoft.Report.Design";
        public const string Stimulsoft_Report_Design_WebViewer = "Stimulsoft.Report.Design.WebViewer";
        public const string Stimulsoft_Report_Import = "Stimulsoft.Report.Import";
        public const string Stimulsoft_Report_Mvc = "Stimulsoft.Report.Mvc";
        public const string Stimulsoft_Report_Mvc_NetCore = "Stimulsoft.Report.Mvc.NetCore";
        public const string Stimulsoft_Report_MvcMobile = "Stimulsoft.Report.MvcMobile";
        public const string Stimulsoft_Report_Mobile = "Stimulsoft.Report.Mobile";
        public const string Stimulsoft_Report_MobileDesign = "Stimulsoft.Report.MobileDesign";
        public const string Stimulsoft_Report_Web = "Stimulsoft.Report.Web";
        public const string Stimulsoft_Report_WebDesign = "Stimulsoft.Report.WebDesign";
        public const string Stimulsoft_Report_Publish = "Stimulsoft.Report.Publish";
        public const string Stimulsoft_Report_Test = "Stimulsoft.Report.Test";
        public const string Stimulsoft_Report_Win = "Stimulsoft.Report.Win";
        public const string Stimulsoft_Report_WebViewer = "Stimulsoft.Report.WebViewer";
        public const string Stimulsoft_Report_Wpf = "Stimulsoft.Report.Wpf";
        public const string Stimulsoft_Report_WpfDesign = "Stimulsoft.Report.WpfDesign";
        public const string Stimulsoft_Report_Xbap = "Stimulsoft.Report.Xbap";
        
        public const string Stimulsoft_Server = "Stimulsoft.Server";
        public const string Stimulsoft_Server_Agent = "Stimulsoft.Server.Agent";
        public const string Stimulsoft_Server_Check = "Stimulsoft.Server.Check";
        public const string Stimulsoft_Server_Cloud_Worker = "Stimulsoft.Server.Cloud.Worker";
        public const string Stimulsoft_Server_Connect = "Stimulsoft.Server.Connect";
        public const string Stimulsoft_Server_Connect_Test = "Stimulsoft.Server.Connect.Test";
        public const string Stimulsoft_Server_Controller = "Stimulsoft.Server.Controller";
        public const string Stimulsoft_Server_Dropbox = "Stimulsoft.Server.Dropbox";
        public const string Stimulsoft_Server_OneDrive = "Stimulsoft.Server.OneDrive";
        public const string Stimulsoft_Server_Test = "Stimulsoft.Server.Test";

        public const string Stimulsoft_TaskScheduler = "Stimulsoft.TaskScheduler";

        public const string Stimulsoft_Desktop = "Desktop";
        public const string Stimulsoft_Demo = "Demo";
        public const string Stimulsoft_Demo_Wpf = "Demo.Wpf";

        public const string Admin_Console = "AdminConsole";

        public const string Forms_Designer = "Forms.Designer";
    }
}
