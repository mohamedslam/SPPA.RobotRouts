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
    public class ControlsWhileUntil : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var mode = this.Fields.Get("MODE");
            var value = this.Values.FirstOrDefault(x => x.Name == "BOOL");

            if (!this.Statements.Any(x => x.Name == "DO") || null == value) return base.Evaluate(context);

            var statement = this.Statements.Get("DO");

            if (mode == "WHILE")
            {
                while ((bool)value.Evaluate(context))
                {
                    statement.Evaluate(context);
                }
            }
            else
            {
                while (!(bool)value.Evaluate(context))
                {
                    statement.Evaluate(context);
                }
            }

            return base.Evaluate(context);
        }
    }
}