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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using Stimulsoft.Base;

[assembly: AssemblyTitle("Stimulsoft.Report.dll")]
[assembly: AssemblyDescription("Main reports core functionality")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(StiPublicName.Company)]
[assembly: AssemblyProduct(StiPublicName.ProductReports)]
[assembly: AssemblyCopyright(StiVersion.Copyright)]
[assembly: AssemblyTrademark(StiPublicName.Trademark)]
[assembly: AssemblyCulture("")]
[assembly: AllowPartiallyTrustedCallers]
#if NETSTANDARD || NETCOREAPP
[assembly: CLSCompliant(false)]
#else
[assembly: CLSCompliant(true)]
#endif
[assembly: AssemblyVersion(StiVersion.Version)]
[assembly: AssemblyDelaySign(false)]
[assembly: SecurityRules(SecurityRuleSet.Level1)]

[assembly: InternalsVisibleTo(StiPublicKey.Designer)]
[assembly: InternalsVisibleTo(StiPublicKey.Navigator)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Accounts_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Blockly)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Wizard_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Client_Designer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Client_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Design)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Viewer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Viewer_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_App)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_App_Drawing)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Blazor)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Angular)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Check)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Design)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Design_WebViewer)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Import)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mobile)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_MobileDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc_NetCore)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Test)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WebDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Win)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WpfDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Xbap)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Controller)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Connect)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Server_Test)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Desktop)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Demo)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Drawing_Wpf)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Export)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Dashboard_Drawing)]