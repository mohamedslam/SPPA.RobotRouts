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

using System;
using Stimulsoft.Blockly.Model;

namespace Stimulsoft.Blockly.Blocks.Maths
{
    public class MathArithmetic : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var a = StiObjConverter.ToDouble(this.Values.Evaluate("A", context));
            var b = StiObjConverter.ToDouble(this.Values.Evaluate("B", context));

            var opValue = this.Fields.Get("OP");

            switch (opValue)
            {
                case "MULTIPLY": return a * b;
                case "DIVIDE": return a / b;
                case "ADD": return a + b;
                case "MINUS": return a - b;
                case "POWER": return Math.Pow(a, b);

                default: throw new ApplicationException($"Unknown OP {opValue}");
            }
        }
    }
}