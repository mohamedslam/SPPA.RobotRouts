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
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.Check
{
    public class StiCalculatedColumnRecursionCheck : StiDataColumnCheck
    {
        #region Properties
        public override string ShortMessage
        {
            get
            {
                return string.Format(StiLocalizationExt.Get("CheckDataSource", "StiCalculatedColumnRecursionCheckShort"), this.ElementName);
            }
        }

        public override string LongMessage
        {
            get
            {
                var column = this.Element as StiCalcDataColumn;
                var dataSource = column != null ? column.DataSource : null;
                var name = dataSource != null ? dataSource.Name : string.Empty;

                return string.Format(StiLocalizationExt.Get("CheckDataSource", "StiCalculatedColumnRecursionCheckLong"), this.ElementName, name);
            }
        }

        public override StiCheckStatus Status
        {
            get
            {
                return StiCheckStatus.Error;
            }
        }
        #endregion

        #region Methods
        private bool CheckForRecursion(StiCalcDataColumn column, StiReport report)
        {
            try
            {
                string fullColumnName = column.DataSource.Name + "." + column.Name;
                bool storeToPrint = false;
                var comp = new StiText() { Name = column.Name, Page = report.Pages[0] };
                object result = StiParser.ParseTextValue("{" + column.Expression + "}", comp, ref storeToPrint, false, true);
                var list = result as List<Stimulsoft.Report.Engine.StiParser.StiAsmCommand>;
                if (list != null)
                {
                    foreach (Stimulsoft.Report.Engine.StiParser.StiAsmCommand command in list)
                    {
                        if ((command.Type == StiParser.StiAsmCommandType.PushDataSourceField) && (fullColumnName.Equals(command.Parameter1)))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            var dataColumn = obj as StiCalcDataColumn;
            if (dataColumn != null)
            {
                if (CheckForRecursion(dataColumn, report))
                {
                    StiCalculatedColumnRecursionCheck check = new StiCalculatedColumnRecursionCheck();
                    check.Element = obj;

                    check.Actions.Add(new StiEditPropertyAction());

                    return check;
                }
            }
            return null;
        }
        #endregion
    }
}