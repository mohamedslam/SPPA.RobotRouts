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

namespace Stimulsoft.Blockly.Blocks.Maths
{
    public class MathSingle : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var op = this.Fields.Get("OP");
            var number = StiObjConverter.ToDouble(this.Values.Evaluate("NUM", context));

            switch (op)
            {
                case "ROOT": return Math.Sqrt(number);
                case "ABS": return Math.Abs(number);
                case "NEG": return -1 * number;
                case "LN": return Math.Log(number);
                case "LOG10": return Math.Log10(number);
                case "EXP": return Math.Exp(number);
                case "POW10": return Math.Pow(10, number);

                case "SIN": return Math.Sin(number / 180 * Math.PI);
                case "COS": return Math.Cos(number / 180 * Math.PI);
                case "TAN": return Math.Tan(number / 180 * Math.PI);
                case "ASIN": return Math.Asin(number);
                case "ACOS": return Math.Acos(number);
                case "ATAN": return Math.Atan(number);

                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }
    }
}