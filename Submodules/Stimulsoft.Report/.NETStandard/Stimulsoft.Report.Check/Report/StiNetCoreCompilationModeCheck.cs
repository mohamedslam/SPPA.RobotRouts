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

using Stimulsoft.Report.Helper;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;

namespace Stimulsoft.Report.Check
{
    public class StiNetCoreCompilationModeCheck : StiReportCheck
    {
        #region Properties
        public override bool PreviewVisible => false;

        public override string ShortMessage => StiLocalizationExt.Get("CheckReport", "StiNetCoreCompilationModeCheckShort");

        public override string LongMessage => StiLocalizationExt.Get("CheckReport", "StiNetCoreCompilationModeCheckLong");

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods

        private bool CheckFramework()
        {
            var frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            
            if (!string.IsNullOrEmpty(frameworkName) && (frameworkName.IndexOf("=v") > 0) && frameworkName.StartsWith(".NETCoreApp"))
            {
                var version = frameworkName.Substring(frameworkName.IndexOf("=v") + 2);
                
                if (StiDpiHelper.IsWindows && version.StartsWith("2") || !StiDpiHelper.IsWindows && version.StartsWith("3")) 
                    return true;
            }
            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            List<StiCheck> checks = null;

            if ((report.CalculationMode == StiCalculationMode.Compilation) && (CheckFramework() || StiReport.CheckNeedForceInterpretationMode()))
            {
                var check = new StiNetCoreCompilationModeCheck();
                check.Actions.Add(new StiChangeToInterpretationModeAction());

                checks = new List<StiCheck> { check };
            }

            return checks;
        }
        #endregion
    }
}
