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

using Stimulsoft.Base.Helpers;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Helper;

namespace Stimulsoft.Report.Check
{
    public class StiVariableInitializationCheck : StiVariableCheck
    {
        #region Properties
        public override string ShortMessage => string.Format(StiLocalizationExt.Get("CheckVariable", "StiVariableInitializationCheckShort"), ElementName);

        public override string LongMessage => string.Format(StiLocalizationExt.Get("CheckVariable", "StiVariableInitializationCheckLong"), ElementName);

        public override StiCheckStatus Status => StiCheckStatus.Warning;
        #endregion

        #region Methods
        private bool CheckInitialization(StiVariable variable)
        {
            decimal result;
            return !decimal.TryParse(variable.Value, out result);
        }

        public override object ProcessCheck(StiReport report, object obj)
        {
            this.Element = obj;

            var variable = obj as StiVariable;
            if (variable?.Type != null && variable.InitBy == StiVariableInitBy.Value && 
                !string.IsNullOrWhiteSpace(variable.Value) && variable.Type.IsNumericType())
            {
                if (CheckInitialization(variable))
                {
                    var check = new StiVariableInitializationCheck();

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