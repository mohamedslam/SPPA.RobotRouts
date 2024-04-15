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

using System.Collections.Generic;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiCloudCompilationModeCheck : StiReportCheck
    {
        #region Properties
        public override bool PreviewVisible => false;

        public override string ShortMessage => StiLocalizationExt.Get("CheckReport", "StiCloudCompilationModeCheckShort");

        public override string LongMessage => StiLocalizationExt.Get("CheckReport", "StiCloudCompilationModeCheckLong");

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            List<StiCheck> checks = null;

            if (report.CalculationMode == StiCalculationMode.Compilation)
            {
                var check = new StiCloudCompilationModeCheck();
                check.Actions.Add(new StiChangeToInterpretationModeAction());

                checks = new List<StiCheck> { check };
            }

            return checks;
        }
        #endregion
    }
}