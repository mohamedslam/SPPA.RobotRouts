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
using System.Drawing;

namespace Stimulsoft.Blockly.Blocks.Logic
{
    public class LogicCompare : IronBlock
    {
        public override object Evaluate(Context context)
        {
            var a = StiObjConverter.GetValue(this.Values.Evaluate("A", context));
            var b = StiObjConverter.GetValue(this.Values.Evaluate("B", context));

            if (a == DBNull.Value) a = null;
            if (b == DBNull.Value) b = null;

            var opValue = this.Fields.Get("OP");

            if (a is string) return Compare(opValue, a as string, b as string);
            if (a is double || a is float || a is decimal) return Compare(opValue, Convert.ToDouble(a), Convert.ToDouble(b));
            if (a is int) return Compare(opValue, Convert.ToInt32(a), Convert.ToInt32(b));
            if (a is bool) return Compare(opValue, (bool)a, (bool)b);

            if (a is Color aValue && b is string)
            {
                var bValue = StiObjConverter.ToColor(b.ToString());

                switch (opValue)
                {
                    case "EQ": return aValue.A == bValue.A && aValue.R == bValue.R && aValue.G == bValue.G && aValue.B == bValue.B;
                    case "NEQ": return aValue.A != bValue.A || aValue.R != bValue.R || aValue.G != bValue.G || aValue.B != bValue.B;
                }
            }

            if (a == null && b == null)
            {
                switch (opValue)
                {
                    case "EQ": return true;
                    case "NEQ": return false;
                }
            }
            else if (a == null && b != null)
            {
                switch (opValue)
                {
                    case "EQ": return false;
                    case "NEQ": return true;
                }
            }
            else if (a != null && b == null)
            {
                switch (opValue)
                {
                    case "EQ": return false;
                    case "NEQ": return true;
                }
            }

            throw new ApplicationException("unexpected value type");
        }

        bool Compare(string op, string a, string b)
        {
            switch (op)
            {
                case "EQ": return a == b;
                case "NEQ": return a != b;
                case "LT": return string.Compare(a, b) < 0;
                case "LTE": return string.Compare(a, b) <= 0;
                case "GT": return string.Compare(a, b) > 0;
                case "GTE": return string.Compare(a, b) >= 0;
                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }

        bool Compare(string op, double a, double b)
        {
            switch (op)
            {
                case "EQ": return a == b;
                case "NEQ": return a != b;
                case "LT": return a < b;
                case "LTE": return a <= b;
                case "GT": return a > b;
                case "GTE": return a >= b;
                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }

        bool Compare(string op, int a, int b)
        {
            switch (op)
            {
                case "EQ": return a == b;
                case "NEQ": return a != b;
                case "LT": return a < b;
                case "LTE": return a <= b;
                case "GT": return a > b;
                case "GTE": return a >= b;
                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }

        bool Compare(string op, bool a, bool b)
        {
            switch (op)
            {
                case "EQ": return a == b;
                case "NEQ": return a != b;
                default: throw new ApplicationException($"Unknown OP {op}");
            }
        }
    }
}