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

using System.Runtime.CompilerServices;

namespace Stimulsoft.Base
{
    public class StiPublicKey
    {
        private const string KeyReports = "00240000048000009400000006020000002400005253413100040000010001003bad30e3275cab0470144f7f98457555375744d5e3c5cd451fe72f4b2459d629b0c409f0d6d4af1ec4a01065a73ff0debd63d7392c703d42a8d2c5dcaed1236f1d384ab74d722c159258b58a07060ff74c3c02329f4e8244ff39ee7c113814c1b1965b8c2ccfeb505e31ed16f314937be078c0b81b7f11d815f45316e02e9cd7";
        private const string KeyServer = "0024000004800000940000000602000000240000525341310004000001000100f1fb6d24a930093776409d1222ebc02c34a7b28c2d12e7dd8fdbbaefc55fcb38ad27ac65bc356b2182ce44d7807ed33b262b33195ca215de36c167a2f5fa96b36241f6e6e459d138b79c575e7f0c771ccb1bb2f732550d15bb55a2653778da49adf886bf7bad9baf86397dabd5c52c4b31366da9741fe01571a9658f2c6e08c5";

        public const string Designer = StiAssemblies.Designer + ", PublicKey=" + KeyReports;
        public const string Monitor = StiAssemblies.Monitor + ", PublicKey=" + KeyReports;
        public const string Navigator = StiAssemblies.Navigator + ", PublicKey=" + KeyReports;
        public const string Navigator_Web = StiAssemblies.Navigator_Web + ", PublicKey=" + KeyReports;

        public const string RunMe_Test = StiAssemblies.RunMe_Test + ", PublicKey=" + KeyReports;

        public const string Server_Console = StiAssemblies.Server_Console + ", PublicKey=" + KeyReports;
        public const string Admin_Console = StiAssemblies.Admin_Console + ", PublicKey=" + KeyReports;

        public const string CloudViewer_Web = StiAssemblies.CloudViewer_Web + ", PublicKey=" + KeyReports;
        public const string CloudDesigner_Web = StiAssemblies.CloudDesigner_Web + ", PublicKey=" + KeyReports;
        public const string CloudShare_Web = StiAssemblies.CloudShare_Web + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_BlocklyEditor = StiAssemblies.Stimulsoft_BlocklyEditor_Wpf + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Accounts_Wpf = StiAssemblies.Stimulsoft_Accounts_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Wizard_Wpf = StiAssemblies.Stimulsoft_Wizard_Wpf + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Blockly= StiAssemblies.Stimulsoft_Blockly + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Client = StiAssemblies.Stimulsoft_Client + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Client_Designer = StiAssemblies.Stimulsoft_Client_Designer + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Client_Web = StiAssemblies.Stimulsoft_Client_Web + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Cloud_Test = StiAssemblies.Stimulsoft_Cloud_Test + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Dashboard = StiAssemblies.Stimulsoft_Dashboard + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Design = StiAssemblies.Stimulsoft_Dashboard_Design + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Drawing = StiAssemblies.Stimulsoft_Dashboard_Drawing + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Drawing_Wpf = StiAssemblies.Stimulsoft_Dashboard_Drawing_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Export = StiAssemblies.Stimulsoft_Dashboard_Export + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Viewer = StiAssemblies.Stimulsoft_Dashboard_Viewer + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Viewer_Wpf = StiAssemblies.Stimulsoft_Dashboard_Viewer_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Controls_Wpf = StiAssemblies.Stimulsoft_Controls_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Test = StiAssemblies.Stimulsoft_Dashboard_Test + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Dashboard_Wpf = StiAssemblies.Stimulsoft_Dashboard_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Data = StiAssemblies.Stimulsoft_Data + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Designer_Wpf = StiAssemblies.Stimulsoft_Designer_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_DesignerV2_Wpf = StiAssemblies.Stimulsoft_DesignerV2_Wpf + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_App = StiAssemblies.Stimulsoft_App + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_App_Drawing = StiAssemblies.Stimulsoft_App_Drawing + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_App_Design = StiAssemblies.Stimulsoft_App_Design + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_App_Viewer = StiAssemblies.Stimulsoft_App_Viewer + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_LicenseActivator = StiAssemblies.Stimulsoft_LicenseActivator + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Report = StiAssemblies.Stimulsoft_Report + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Map = StiAssemblies.Stimulsoft_Map + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Blazor = StiAssemblies.Stimulsoft_Report_Blazor + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Angular = StiAssemblies.Stimulsoft_Report_Angular + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Check = StiAssemblies.Stimulsoft_Report_Check + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_TaskScheduler = StiAssemblies.Stimulsoft_TaskScheduler + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Design = StiAssemblies.Stimulsoft_Report_Design + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Design_WebViewer = StiAssemblies.Stimulsoft_Report_Design_WebViewer + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Import = StiAssemblies.Stimulsoft_Report_Import + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Mvc = StiAssemblies.Stimulsoft_Report_Mvc + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Mvc_NetCore = StiAssemblies.Stimulsoft_Report_Mvc_NetCore + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_MvcMobile = StiAssemblies.Stimulsoft_Report_MvcMobile + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Mobile = StiAssemblies.Stimulsoft_Report_Mobile + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_MobileDesign = StiAssemblies.Stimulsoft_Report_MobileDesign + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Publish = StiAssemblies.Stimulsoft_Report_Publish + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Test = StiAssemblies.Stimulsoft_Report_Test + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Win = StiAssemblies.Stimulsoft_Report_Win + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Web = StiAssemblies.Stimulsoft_Report_Web + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_WebViewer = StiAssemblies.Stimulsoft_Report_WebViewer + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_WebDesign = StiAssemblies.Stimulsoft_Report_WebDesign + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Wpf = StiAssemblies.Stimulsoft_Report_Wpf + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_WpfDesign = StiAssemblies.Stimulsoft_Report_WpfDesign + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Report_Xbap = StiAssemblies.Stimulsoft_Report_Xbap + ", PublicKey=" + KeyReports;
        
        public const string Stimulsoft_Server = StiAssemblies.Stimulsoft_Server + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Agent = StiAssemblies.Stimulsoft_Server_Agent + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Activator = StiAssemblies.Stimulsoft_Server_Activator + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Check = StiAssemblies.Stimulsoft_Server_Check + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Cloud_Worker = StiAssemblies.Stimulsoft_Server_Cloud_Worker + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Connect = StiAssemblies.Stimulsoft_Server_Connect + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Controller = StiAssemblies.Stimulsoft_Server_Controller + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Dropbox = StiAssemblies.Stimulsoft_Server_Dropbox + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_OneDrive = StiAssemblies.Stimulsoft_Server_OneDrive + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Server_Test = StiAssemblies.Stimulsoft_Server_Test + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Desktop = StiAssemblies.Stimulsoft_Desktop + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Demo = StiAssemblies.Stimulsoft_Demo + ", PublicKey=" + KeyReports;
        public const string Stimulsoft_Demo_Wpf = StiAssemblies.Stimulsoft_Demo_Wpf + ", PublicKey=" + KeyReports;

        public const string Stimulsoft_Report_Wpf_Test = StiAssemblies.Stimulsoft_Report_Wpf_Test + ", PublicKey=" + KeyReports;
        public const string TestBrowser = StiAssemblies.TestBrowser + ", PublicKey=" + KeyReports;
        public const string SendEmail = StiAssemblies.SendEmail + ", PublicKey=" + KeyReports;
        
        public const string Stimulsoft_Form_Designer = StiAssemblies.Forms_Designer + ", PublicKey=" + KeyReports;
    }
}