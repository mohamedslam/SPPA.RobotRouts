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

using System.Text;

namespace LatexMath2MathML
{
    internal class StiMatrixConverter : BlockConverter
    {
        #region Properties
        public virtual string MatrixStartSymbol => "";

        public virtual string MatrixEndSymbol => "";
        #endregion

        #region Methods
        /// <summary>
        /// Performs the conversion procedure (comments the expression).
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var bld = new StringBuilder();
            var rows = expr.Expressions[0];

            bld.Append("<mrow>\n");
            
            if (!string.IsNullOrEmpty(this.MatrixStartSymbol))
                bld.Append($"<mo>{this.MatrixStartSymbol}</mo>\n");

            bld.Append("<mtable>\n");
            for (int i = 0; i < rows.Count; i++)
            {
                bld.Append("<mtr>\n");
                for (int j = 0; j < rows[i].Expressions[0].Count; j++)
                {
                    bld.Append("<mtd columnalign=\"left\">\n<mrow>\n");
                    bld.Append(SequenceConverter.ConvertOutline(rows[i].Expressions[0][j].Expressions[0], expr.Customization));
                    bld.Append("</mrow>\n</mtd>\n");
                }
                bld.Append("</mtr>\n");
            }
            bld.Append("</mtable>\n");

            if (!string.IsNullOrEmpty(this.MatrixEndSymbol))
                bld.Append($"<mo>{this.MatrixEndSymbol}</mo>\n");

            bld.Append("</mrow>\n");
            return bld.ToString();
        } 
        #endregion
    }
}
