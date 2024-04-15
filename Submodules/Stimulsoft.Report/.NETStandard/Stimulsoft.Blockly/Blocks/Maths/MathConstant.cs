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
#endregion Copyright (C) 2003-2022 Stimulsoftusing System;

using Stimulsoft.Blockly.Model;
using System;

namespace Stimulsoft.Blockly.Blocks.Maths
{
    public class MathConstant : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var constant = this.Fields.Get("CONSTANT");
            return GetValue(constant);
        }

        static double GetValue(string constant)
        {
            switch (constant)
            {
                case "PI": return Math.PI;
                case "E": return Math.E;
                case "GOLDEN_RATIO": return (1 + Math.Sqrt(5)) / 2;
                case "SQRT2": return Math.Sqrt(2);
                case "SQRT1_2": return Math.Sqrt(0.5);
                case "INFINITY": return double.PositiveInfinity;
                default: throw new ApplicationException($"Unknown CONSTANT {constant}");
            }
        }
    }
}