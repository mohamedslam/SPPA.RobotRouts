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
using System;

namespace Stimulsoft.Blockly.Blocks.Logic
{
    public class LogicOperation : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var a = (bool)(this.Values.Evaluate("A", context) ?? false);
            var b = (bool)(this.Values.Evaluate("B", context) ?? false);

            var op = this.Fields.Get("OP");

            switch (op)
            {
                case "AND": return a && b;
                case "OR": return a || b;
                default: throw new ApplicationException($"Unknown OP {op}");
            }

        }
    }
}