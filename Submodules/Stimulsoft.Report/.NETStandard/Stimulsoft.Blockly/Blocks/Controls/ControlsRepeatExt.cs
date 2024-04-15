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
    public class ControlsRepeatExt : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var timesValue = StiObjConverter.ToDouble(this.Values.Evaluate("TIMES", context));

            if (!this.Statements.Any(x => x.Name == "DO")) return base.Evaluate(context);

            var statement = this.Statements.Get("DO");

            for (var i = 0; i < timesValue; i++)
            {
                statement.Evaluate(context);

                if (context.EscapeMode == EscapeMode.Break)
                {
                    context.EscapeMode = EscapeMode.None;
                    break;
                }

                context.EscapeMode = EscapeMode.None;
            }

            context.EscapeMode = EscapeMode.None;

            return base.Evaluate(context);
        }
    }

}