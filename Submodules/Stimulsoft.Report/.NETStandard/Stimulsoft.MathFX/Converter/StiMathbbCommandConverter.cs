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

using System.Collections.Generic;

namespace LatexMath2MathML
{
    /// <summary>
    /// The converter class for roots.
    /// </summary>
    internal sealed class StiMathbbCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "mathbb";
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 1; }
        }

        private static readonly Dictionary<char, string> ConversionTable = new Dictionary<char, string>
        {
            #region Initialization
            {'A', "&#x1D538;"},
            {'B', "&#x1D539;"},
            {'C', "&#x2102;"},
            {'D', "&#x1D53B;"},
            {'E', "&#x1D53C;"},
            {'F', "&#x1D53D;"},
            {'G', "&#x1D53E;"},
            {'H', "&#x210D;"},
            {'I', "&#x1D540;"},
            {'J', "&#x1D541;"},
            {'K', "&#x1D542;"},
            {'L', "&#x1D543;"},
            {'M', "&#x1D544;"},
            {'N', "&#x2115;"},
            {'O', "&#x1D546;"},
            {'P', "&#x2119;"},
            {'Q', "&#x211A;"},
            {'R', "&#x211D;"},
            {'S', "&#x1D54A;"},
            {'T', "&#x1D54B;"},
            {'U', "&#x1D54C;"},
            {'V', "&#x1D54D;"},
            {'W', "&#x1D54E;"},
            {'X', "&#x1D54F;"},
            {'Y', "&#x1D550;"},
            {'Z', "&#x2124;"},

            {'a', "&#x1D552;"},
            {'b', "&#x1D553;"},
            {'c', "&#x1D554;"},
            {'d', "&#x1D555;"},
            {'e', "&#x1D556;"},
            {'f', "&#x1D557;"},
            {'g', "&#x1D558;"},
            {'h', "&#x1D559;"},
            {'i', "&#x1D55A;"},
            {'j', "&#x1D55B;"},
            {'k', "&#x1D55C;"},
            {'l', "&#x1D55D;"},
            {'m', "&#x1D55E;"},
            {'n', "&#x1D55F;"},
            {'o', "&#x1D560;"},
            {'p', "&#x1D561;"},
            {'q', "&#x1D562;"},
            {'r', "&#x1D563;"},
            {'s', "&#x1D564;"},
            {'t', "&#x1D565;"},
            {'u', "&#x1D566;"},
            {'v', "&#x1D567;"},
            {'w', "&#x1D568;"},
            {'x', "&#x1D569;"},
            {'y', "&#x1D56A;"},
            {'z', "&#x1D56B;"},
            #endregion
        };

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            var letter = expr.Expressions[0][0].Name[0];
            string converted;
            if (!ConversionTable.TryGetValue(letter, out converted))
            {
                converted = "" + letter;
            }
            return converted;
        }
    }
}
