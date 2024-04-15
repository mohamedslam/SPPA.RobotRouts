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

namespace Stimulsoft.Report.Check
{
    public class StiIsFirstPassIsSecondPassCheck : StiReportCheck
    {
        #region Properties
        public override string ShortMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiIsFirstPassIsSecondPassCheckShort");

        public override string LongMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiIsFirstPassIsSecondPassCheckLong");

        public override StiCheckStatus Status => StiCheckStatus.Information;
        #endregion

        #region Methods
        private bool Check()
        {
            var report = this.Element as StiReport;
            if (report.NumberOfPass == StiNumberOfPass.SinglePass)
            {
                #region Проверка на IsFirstPass, IsSecondPass
                string[] variables = { "IsFirstPass", "IsSecondPass" };

                for (int index = 0; index < variables.Length; index++)
                {
                    if (report.Script.IndexOf(variables[index]) != -1)
                        return true;
                }
                #endregion
            }

            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            bool failed = Check();
            if (failed)
            {
                var check = new StiIsFirstPassIsSecondPassCheck();
                check.Element = obj;
                check.Actions.Add(new StiAllowDoublePassAction());
                return check;
            }
            else return null;
        }
        #endregion
    }
}