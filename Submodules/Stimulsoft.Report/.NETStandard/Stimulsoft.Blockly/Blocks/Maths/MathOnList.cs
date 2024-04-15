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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Blockly.Blocks.Maths
{
    public class MathOnList : IronBlock
    {
        static Random rnd = new Random();
        public override object Evaluate(Context context)
        {
            var op = this.Fields.Get("OP");
            var list = this.Values.Evaluate("LIST", context) as IEnumerable<object>;

            var doubleList = list.Select(x => (double)x).ToArray();

            switch (op)
            {
                case "SUM": return doubleList.Sum();
                case "MIN": return doubleList.Min();
                case "MAX": return doubleList.Max();
                case "AVERAGE": return doubleList.Average();
                case "MEDIAN": return Median(doubleList);
                case "RANDOM": return doubleList.Any() ? doubleList[rnd.Next(doubleList.Count())] as object : null;
                case "MODE": return doubleList.Any() ? doubleList.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key as object : null;

                case "STD_DEV":
                    throw new NotImplementedException($"OP {op} not implemented");

                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }

        object Median(IEnumerable<double> values)
        {
            if (!values.Any()) return null;
            var sortedValues = values.OrderBy(x => x).ToArray();
            double mid = (sortedValues.Length - 1) / 2.0;
            return (sortedValues[(int)(mid)] + sortedValues[(int)(mid + 0.5)]) / 2;
        }
    }
}