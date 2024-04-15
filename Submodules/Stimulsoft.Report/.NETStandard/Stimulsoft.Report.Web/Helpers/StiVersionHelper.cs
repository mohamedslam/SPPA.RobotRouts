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
using System.Security.Cryptography;
using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;

namespace Stimulsoft.Report.Web
{
    internal class StiVersionHelper
    {
        public static string AssemblyVersion
        {
            get
            {
                string[] values = StiVersion.Version.Split('.');
                string assemblyVersion = values[0] + "." + values[1] + "." + values[2];
                if (values.Length > 3 && values[3] != "0") assemblyVersion += "." + values[3];

                return assemblyVersion;
            }
        }
        
        public static string ProductVersion
        {
            get
            {
                return AssemblyVersion + " from " + StiVersion.CreationDate;
            }
        }

        public static string ReportVersion
        {
            get
            {
                return "Stimulsoft Reports " + ProductVersion;
            }
        }
    }
}
