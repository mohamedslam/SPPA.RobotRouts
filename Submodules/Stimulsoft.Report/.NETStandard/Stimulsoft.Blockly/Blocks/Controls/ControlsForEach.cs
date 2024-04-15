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
using System.Collections;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Controls
{
    public class ControlsForEach : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var variableName = this.Fields.Get("VAR");
            var list = this.Values.Evaluate("LIST", context) as IEnumerable;

            var statement = this.Statements.Where(x => x.Name == "DO").FirstOrDefault();

            if (null == statement) return base.Evaluate(context);

            foreach (var item in list)
            {
                if (context.Variables.ContainsKey(variableName))
                {
                    context.Variables[variableName] = item;
                }
                else
                {
                    context.Variables.Add(variableName, item);
                }
                statement.Evaluate(context);
            }

            return base.Evaluate(context);
        }
    }
}