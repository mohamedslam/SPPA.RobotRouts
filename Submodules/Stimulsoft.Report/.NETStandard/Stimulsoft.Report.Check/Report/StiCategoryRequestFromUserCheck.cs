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
using System.Linq;

namespace Stimulsoft.Report.Check
{
    public class StiCategoryRequestFromUserCheck : StiReportCheck
    {
        #region Properties
        public override string ShortMessage => StiLocalizationExt.Get("CheckReport", "StiCategoryRequestFromUserCheckShort");

        public override string LongMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckReport", "StiCategoryRequestFromUserCheckLong"),
                    ListOfVariables);
            }
        }

        public override StiCheckStatus Status => StiCheckStatus.Warning;

        public override bool PreviewVisible => false;

        public string ListOfVariables { get; set; } = "";
        #endregion

        #region Methods
        public override object ProcessCheck(StiReport report, object obj)
        {
            Element = obj;

            var anyCategories = report.Dictionary.Variables.ToList().Any(v => v.IsCategory && v.RequestFromUser);
            
            if (anyCategories && report.ParametersOrientation == StiOrientation.Horizontal)
            {
                var check = new StiCategoryRequestFromUserCheck();

                check.Element = obj;
                check.ListOfVariables = string.Join(", ",
                        report.Dictionary.Variables.ToList().Where(v => v.IsCategory && v.RequestFromUser)
                        .Select(v => v.Category)
                        .ToArray());
                check.Actions.Add(new StiSwitchToParametersVerticalOrientationAction());

                return check;
            }

            return null;
        }
        #endregion
    }
}
