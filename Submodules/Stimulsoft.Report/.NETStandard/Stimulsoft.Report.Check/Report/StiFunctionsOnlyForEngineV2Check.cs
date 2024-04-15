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
    public class StiFunctionsOnlyForEngineV2Check : StiReportCheck
    {
        #region Properties
        public override string ShortMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiFunctionsOnlyForEngineV2CheckShort");

        public override string LongMessage => 
            StiLocalizationExt.Get("CheckComponent", "StiFunctionsOnlyForEngineV2CheckLong");

        public override StiCheckStatus Status => StiCheckStatus.Error;
        #endregion

        #region Methods
        private bool Check()
        {
            StiReport report = this.Element as StiReport;

            if (report.EngineVersion == Stimulsoft.Report.Engine.StiEngineVersion.EngineV1)
            {
                #region Проверка на GroupLine, PageNumberThrough, PageNofMThrough, TotalPageCountThrough, IsFirstPageThrough, IsLastPageThrough
                string[] variables = {
                                "GroupLine",
                                "PageNumberThrough",
                                "PageNofMThrough",
                                "TotalPageCountThrough",
                                "IsFirstPageThrough",
                                "IsLastPageThrough"};

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
                StiFunctionsOnlyForEngineV2Check check = new StiFunctionsOnlyForEngineV2Check();
                check.Element = obj;
                check.Actions.Add(new StiApplyEngineV2Action());
                return check;
            }
            else return null;
        }
        #endregion
    }
}