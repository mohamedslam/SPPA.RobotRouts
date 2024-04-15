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

using Stimulsoft.Drawing;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: AssemblyTitle("Stimulsoft.System.dll")]
[assembly: AssemblyDescription("Base functions from System library")]
[assembly: CLSCompliant(false)]
[assembly: AssemblyCompany(StiDrawindPublicName.Company)]
[assembly: AssemblyProduct(StiDrawindPublicName.Product)]
[assembly: AssemblyCopyright(StiDrawingSystemVersion.Copyright)]
[assembly: AssemblyTrademark(StiDrawindPublicName.Trademark)]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(StiDrawingSystemVersion.Version)]
[assembly: AssemblyDelaySign(false)]
[assembly: SecurityRules(SecurityRuleSet.Level1)]

public class StiDrawingSystemVersion
{
    public const string Version = "2023.1.2.0";
    public const string Copyright = "Copyright (C) 2003-2022 Stimulsoft";
}
