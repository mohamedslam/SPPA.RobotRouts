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

namespace Stimulsoft.Blockly.Blocks.Text
{
    public class TextAppend : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var variables = context.Variables;

            var variableName = this.Fields.Get("VAR");
            var textToAppend = (this.Values.Evaluate("TEXT", context) ?? "").ToString();

            if (!variables.ContainsKey(variableName))
            {
                variables.Add(variableName, "");
            }
            var value = variables[variableName].ToString();

            variables[variableName] = value + textToAppend;

            return base.Evaluate(context);
        }
    }
}