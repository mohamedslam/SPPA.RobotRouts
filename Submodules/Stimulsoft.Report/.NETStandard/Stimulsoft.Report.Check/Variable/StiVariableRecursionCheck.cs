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
using System.Collections;

namespace Stimulsoft.Report.Check
{
    public class StiVariableRecursionCheck : StiVariableCheck
    {
        #region Properties
        public override string ShortMessage => 
            string.Format(StiLocalizationExt.Get("CheckVariable", "StiVariableRecursionCheckShort"), this.ElementName);

        public override string LongMessage => 
            string.Format(StiLocalizationExt.Get("CheckVariable", "StiVariableRecursionCheckLong"), this.ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Error;
        #endregion

        #region Methods
        private bool CheckForRecursion(StiVariable variable, StiReport report)
        {
            var variablesHash = new Hashtable();
            return CheckForRecursion2(variable.Name, report, variablesHash);
        }

        private bool CheckForRecursion2(string name, StiReport report, Hashtable variablesHash)
        {
            try
            {
                if (variablesHash.ContainsKey(name)) 
                    return true;

                var variable = report.Dictionary.Variables[name];
                
                if ((variable == null) || (variable.InitBy != StiVariableInitBy.Expression))
                    return false;

                if (variable.Type == typeof(DateTimeRange) && variable.Value != null && variable.Value.Contains("<<|>>")) 
                    return false;

                bool storeToPrint = false;
                var comp = new StiText() 
                { 
                    Name = variable.Name, 
                    Page = report.Pages[0] 
                };

                var result = StiParser.ParseTextValue("{" + variable.Value + "}", comp, ref storeToPrint, false, true);
                
                var list = result as List<StiParser.StiAsmCommand>;
                if (list != null)
                {
                    foreach (StiParser.StiAsmCommand command in list)
                    {
                        if (command.Type == StiParser.StiAsmCommandType.PushVariable)
                        {
                            var name2 = (string)command.Parameter1;
                            if (variable.Name.Equals(name2)) 
                                return true;

                            variablesHash[name] = null;
                            
                            if (CheckForRecursion2(name2, report, variablesHash)) 
                                return true;

                            variablesHash.Remove(name);
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

            var variable = obj as StiVariable;
            if ((variable != null) && (variable.InitBy == StiVariableInitBy.Expression) && CheckForRecursion(variable, report))
            {
                var check = new StiVariableRecursionCheck();
                
                check.Element = obj;
                check.Actions.Add(new StiEditPropertyAction());

                return check;
            }
            return null;
        }
        #endregion
    }
}