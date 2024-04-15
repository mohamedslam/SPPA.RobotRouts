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

namespace Stimulsoft.Blockly.Blocks.Controls
{
    public class ControlsIf : IronBlock
    {
        public override object Evaluate(Context context)
        {

            var ifCount = 1;
            if (null != this.Mutations.GetValue("elseif"))
            {
                var elseIf = this.Mutations.GetValue("elseif");
                ifCount = int.Parse(elseIf) + 1;
            }

            var done = false;
            for (var i = 0; i < ifCount; i++)
            {
                if ((bool)Values.Evaluate($"IF{i}", context))
                {
                    var statement = this.Statements.Get($"DO{i}");
                    statement.Evaluate(context);
                    done = true;
                    break;
                }
            }

            if (!done)
            {
                if (null != this.Mutations.GetValue("else"))
                {
                    var elseExists = this.Mutations.GetValue("else");
                    if (elseExists == "1")
                    {
                        var statement = this.Statements.Get("ELSE");
                        statement.Evaluate(context);
                    }
                }
            }

            return base.Evaluate(context);
        }
    }
}