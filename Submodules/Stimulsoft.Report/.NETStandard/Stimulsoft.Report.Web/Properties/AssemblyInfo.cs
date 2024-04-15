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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using Stimulsoft.Base;

[assembly: AssemblyTitle("Stimulsoft.Report.Web.dll")]
[assembly: AssemblyDescription("Stimulsoft Reports for ASP.NET")]
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
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Blazor)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Angular)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_Mvc_NetCore)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Report_WebDesign)]
[assembly: InternalsVisibleTo(StiPublicKey.CloudShare_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.CloudViewer_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Navigator_Web)]
[assembly: InternalsVisibleTo(StiPublicKey.Stimulsoft_Client_Web)]