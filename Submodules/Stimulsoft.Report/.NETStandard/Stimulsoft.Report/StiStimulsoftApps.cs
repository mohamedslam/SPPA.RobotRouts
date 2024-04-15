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

using System.Reflection;

namespace Stimulsoft.Report
{
    public static class StiStimulsoftApps
    {
        public static bool? IsDesignerRunFromDesktop { get; set; }

        public static bool IsDesktopApp
        {
            get
            {
                if (IsDesignerRunFromDesktop != null)
                    return IsDesignerRunFromDesktop.GetValueOrDefault();

                var location = Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
                return location.Contains("desktop");
            }
        }

        public static string StimulsoftCurrentAppName => IsDesktopApp ? "Stimulsoft Desktop" : "Stimulsoft Designer";

        public static string StimulsoftCurrentAppFile => IsDesktopApp ? "Stimulsoft-Desktop" : "Stimulsoft-Designer";

        public const string StimulsoftReportsFile = "Stimulsoft-Reports";
        public const string StimulsoftUltimateFile = "Stimulsoft-Ultimate";
    }
}
