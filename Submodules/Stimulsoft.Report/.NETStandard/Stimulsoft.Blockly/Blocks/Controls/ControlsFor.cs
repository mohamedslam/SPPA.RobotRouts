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

using Stimulsoft.Blockly.Model;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Controls
{
    public class ControlsFor : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var variableName = this.Fields.Get("VAR");

            var fromValue = StiObjConverter.ToDouble(this.Values.Evaluate("FROM", context));
            var toValue = StiObjConverter.ToDouble(this.Values.Evaluate("TO", context));
            var byValue = StiObjConverter.ToDouble(this.Values.Evaluate("BY", context));

            var statement = this.Statements.FirstOrDefault();


            if (context.Variables.ContainsKey(variableName))
            {
                context.Variables[variableName] = fromValue;
            }
            else
            {
                context.Variables.Add(variableName, fromValue);
            }


            while ((double)context.Variables[variableName] <= toValue)
            {
                statement.Evaluate(context);
                context.Variables[variableName] = (double)context.Variables[variableName] + byValue;
            }

            return base.Evaluate(context);
        }
    }
}