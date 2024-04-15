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

using LatexMath2MathML;
using SvgMath;

namespace Stimulsoft.MathFX
{
    public class StiLatexService
    {
        #region Fields
        private string latex;
        private string mathML;
        #endregion

        #region Methods
        public string GetMathML()
        {
            if (!string.IsNullOrEmpty(mathML))
                return mathML;

            var lmm = new LatexMathToMathMLConverter();
            this.mathML = lmm.ConvertToMathML(latex);

            return mathML;
        }

        public string GetSVG(float fontSize, string colorHex)
        {
            var mathMLText = this.GetMathML();

            var m = new Mml(mathMLText);
            var xElement = m.MakeSvg(fontSize, colorHex);
            return xElement.ToString();
        }
        #endregion

        public StiLatexService(string latex)
        {
            this.latex = latex;
        }
    }
}
