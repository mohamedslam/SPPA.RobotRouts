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
    public class MathNumberProperty : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var op = this.Fields.Get("PROPERTY");
            var number = Convert.ToDouble(this.Values.Evaluate("NUMBER_TO_CHECK", context));

            switch (op)
            {
                case "EVEN": return 0 == number % 2.0;
                case "ODD": return 1 == number % 2.0;
                case "PRIME": return IsPrime((int)number);
                case "WHOLE": return 0 == number % 1.0;
                case "POSITIVE": return number > 0;
                case "NEGATIVE": return number < 0;
                case "DIVISIBLE_BY": return 0 == number % (double)this.Values.Evaluate("DIVISOR", context);
                default: throw new ApplicationException($"Unknown PROPERTY {op}");
            }
        }

        static bool IsPrime(int number)
        {
            if (number == 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }

    }

}