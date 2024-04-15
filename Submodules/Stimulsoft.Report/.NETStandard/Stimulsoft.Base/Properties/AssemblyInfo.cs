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

using Stimulsoft.Base;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: AssemblyTitle("Stimulsoft.Base.dll")]
[assembly: AssemblyDescription("Base Functionality")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(StiPublicName.Company)]
[assembly: AssemblyProduct(StiPublicName.Product)]
[assembly: AssemblyCopyright(StiVersion.Copyright)]
[assembly: AssemblyTrademark(StiPublicName.Trademark)]
[assembly: AssemblyCulture("")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyVersion(StiVersion.Version)]
[assembly: AssemblyDelaySign(false)]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#if NETSTANDARD || NETCOREAPP
[assembly: CLSCompliant(false)]
#else
[assembly: CLSCompliant(true)]
#endif
[assembly: InternalsVisibleTo(StiPublicKey.Monitor)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Accounts_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_App_Drawing)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_App_Design)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_BlocklyEditor)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Client_Designer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Client_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Design)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Test)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Drawing)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Drawing_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Viewer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Viewer_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Controls_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Data)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Demo)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Desktop)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_LicenseActivator)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Map)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Check)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Design)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Design_WebViewer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc_NetCore)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_MvcMobile)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mobile)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_MobileDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WebViewer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WebDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Win)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Publish)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WpfDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Xbap)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Activator)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Check)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Connect)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Test)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_TaskScheduler)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Wizard_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Form_Designer)]

public class StiVersion
{
    public const string Version = "2023.1.2.0";
    public const string CreationDate = "15 December 2022";
    public static DateTime Created = new DateTime(2022, 12, 15);
    public const string VersionInfo = "Version=" + Version + ", Culture=neutral, " + StiPublicKeyToken.Key;
    public const string Copyright = "Copyright (C) 2003-2022 Stimulsoft";
}
