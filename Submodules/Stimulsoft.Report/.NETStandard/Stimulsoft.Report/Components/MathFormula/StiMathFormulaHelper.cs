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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Helpers;
using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Stimulsoft.Report.Components.MathFormula
{
    public static class StiMathFormulaHelper
    {
        #region consts
        public static readonly string[] BasicArray = new string[]
            {
                "\\:", "\\quad", "\\plus", "\\minus", "\\div", "\\times", "=",  "\\neq", "x_a^b", "\\sqrt[n]{ab}", "\\overline{ab}", "(" , ")", "\\lbrace" , "\\rbrace",
                "\\sum_a^b", "\\lim_{a \\rightarrow b}", "\\sti{}"
            };
        public static readonly string[] ArrowArray = new string[]
            {
                "\\to", "\\longrightarrow", "\\Rightarrow", "\\Longrightarrow", "\\hookrightarrow", "\\mapsto", "\\longmapsto",
                "\\gets", "\\longleftarrow", "\\Leftarrow","\\Longleftarrow","\\hookleftarrow",
                "\\leftrightarrow","\\longleftrightarrow","\\Leftrightarrow","\\Longleftrightarrow",
                "\\uparrow", "\\Uparrow", "\\downarrow","\\Downarrow","\\updownarrow","\\Updownarrow",
                "\\nearrow","\\searrow","\\swarrow","\\nwarrow","\\leftharpoondown",
                "\\leftharpoonup","\\rightharpoonup","\\rightharpoondown","\\rightleftharpoons"
            };
        public static readonly string[] FunctionArray = new string[]
            {
                "\\log" ,"\\lg" , "\\ln" ,"\\arg","\\ker" , "\\dim" ,"\\hom","\\deg" , "\\exp",
                "\\sin","\\cos" , "\\tan" ,"\\cot","\\sec" , "\\csc" ,"\\sinh" ,"\\cosh" , "\\tanh" ,"\\coth"
            };
        public static readonly string[] MathArray = new string[]
            {
                "\\int", "\\int_{a}^{b}","\\oint","\\oint_a^b","\\sum","\\sum_a^b","\\coprod","\\coprod_a^b",
                "\\prod","\\prod_a^b","\\bigcap","\\bigcap_a^b","\\bigcup","\\bigcup_a^b",
                "\\sqrt{ab}","\\sqrt[n]{ab}","\\log_{a}{b}","\\lg{ab}","a^{b}","a_{b}","x_a^b",
                "\\overline{ab}","\\underline{ab}","\\frac{ab}{cd}","\\lim_{a \\rightarrow b}"
            };
        public static readonly string[] FormulaArray = new string[]
            {
                "\\displaystyle\\sum\\limits_{i=0}^n i^3",
                "\\left(\\begin{array}{c}a\\\\ b\\end{array}\\right)",
                "\\left(\\frac{a^2}{b^3}\\right)",
                "\\left.\\frac{a^3}{3}\\right|_0^1",
                "\\sqrt{\\frac{n}{n-1} S}",
                "\\frac{1}{k}\\log_2 c(f)\\;",
                "x^n + y^n = z^n",
                "E=mc^2",
                "\\frac{1+\\frac{a}{b}}{1+\\frac{1}{1+\\frac{1}{a}}}"
            };
        public static readonly string[] AlphabetsArray = new string[]
            {
                "\\alpha","\\beta","\\gamma","\\delta","\\varepsilon",
                "\\zeta","\\eta","\\theta","\\vartheta","\\iota","\\kappa","\\lambda",
                "\\mu","\\nu","\\xi","\\pi", "\\varpi","\\rho","\\varrho","\\sigma",
                "\\varsigma","\\tau","\\upsilon","\\phi","\\varphi","\\chi","\\psi","\\omega",
            };
        public static readonly string[] OperatorsArray = new string[]
            {
                "\\mp", "\\doteqdot", "\\gtrless", "\\doublebarwedge", "\\gtrdot", "\\eqsim", "\\gvertneqq",
                "\\ngeqq", "\\leqq", "\\ngtr", "\\sim", "\\circ", "\\intercal", "\\amalg", "\\circledast",
                "\\leftthreetimes", "\\star", "\\ast", "\\circledcirc", "\\lessdot", "\\barwedge", "\\circleddash",
                "\\ltimes", "\\bigcirc", "\\odot", "\\uplus", "\\curlyvee", "\\ominus", "\\vee", "\\boxdot", "\\curlywedge",
                "\\oplus", "\\veebar", "\\boxminus", "\\dagger", "\\oslash", "\\wedge", "\\boxplus", "\\ddagger", "\\otimes",
                "\\wr", "\\boxtimes", "\\diamond", "\\pm", "\\bullet", "\\rightthreetimes", "\\divideontimes",
                "\\rtimes", "\\dotplus", "\\setminus", "\\eqslantless", "\\equiv", "\\leqslant", "\\nleq", "\\simeq",
                "\\fallingdotseq", "\\lessapprox", "\\lesseqgtr", "\\nleqslant", "\\nleqq", "\\succ", "\\approx",
                "\\geq", "\\succapprox", "\\approxeq", "\\geqq", "\\lesseqqgtr", "\\nless", "\\succcurlyeq", "\\asymp",
                "\\geqslant", "\\lessgtr", "\\nprec", "\\succeq", "\\backsim", "\\gg", "\\lesssim", "\\npreceq",
                "\\succnapprox", "\\backsimeq", "\\ggg", "\\ll", "\\nsim", "\\succneqq", "\\bumpeq", "\\gnapprox",
                "\\lll", "\\nsucc", "\\succnsim", "\\Bumpeq", "\\gneq", "\\lnapprox", "\\nsucceq", "\\succsim", "\\circeq",
                "\\gneqq", "\\lneq", "\\prec", "\\thickapprox", "\\cong", "\\gnsim", "\\lneqq", "\\precapprox", "\\thicksim",
                "\\curlyeqprec", "\\gtrapprox", "\\lnsim", "\\preccurlyeq", "\\curlyeqsucc", "\\gtreqless", "\\lvertneqq",
                "\\preceq", "\\doteq", "\\gtreqqless", "\\ncong", "\\precnapprox", "\\precneqq", "\\eqcirc", "\\gtrsim",
                "\\ngeq", "\\precnsim", "\\precsim", "\\eqslantgtr", "\\leq", "\\ngeqslant", "\\risingdotseq",
                "\\backepsilon", "\\smallsmile", "\\therefore", "\\because", "\\smile", "\\between", "\\varpropto",
                "\\bowtie", "\\dashv", "\\frown", "\\smallfrown", "\\nvdash", "\\in", "\\nVdash", "\\mid", "\\nvDash",
                "\\models", "\\nVDash", "\\ni", "\\parallel", "\\vdash", "\\nmid", "\\perp", "\\Vdash", "\\notin", "\\pitchfork",
                "\\vDash", "\\propto", "\\Vvdash", "\\nshortmid", "\\shortmid", "\\nparallel", "\\nshortparallel", "\\shortparallel",
                "\\triangleleft", "\\vartriangle", "\\triangleright", "\\ntrianglerighteq", "\\vartriangleright", "\\vartriangleleft",
                "\\ntriangleright", "\\blacktriangleright", "\\ntrianglelefteq", "\\trianglerighteq", "\\blacktriangleleft",
                "\\ntriangleleft", "\\trianglelefteq", "\\bigtriangledown", "\\bigtriangleup", "\\triangleq", "\\sqcap",
                "\\sqcup", "\\sqsupset", "\\sqsubset", "\\sqsupseteq", "\\sqsubseteq", "\\Cap", "\\Cup", "\\Subset", "\\Supset",
                "\\cup", "\\cap", "\\subset", "\\supset", "\\subseteq", "\\subseteqq", "\\varsubsetneq", "\\subsetneqq",
                "\\varsubsetneqq", "\\nsubseteqq", "\\subsetneq", "\\nsubseteq", "\\supseteq", "\\supseteqq", "\\supsetneq",
                "\\varsupsetneq", "\\nsupseteq", "\\supsetneqq", "\\varsupsetneqq", "\\nsupseteqq", "\\bigodot", "\\bigotimes", "\\bigoplus", "\\biguplus"
            };
        #endregion

        #region Methods.SVG
        public static string GetSvgText(string latextContent, float fontSize, string colorHex)
        {
            if (!latextContent.StartsWith(@"\begin{document} $\["))
            {
                latextContent = $"\\begin{{document}} $\\[ {latextContent} \\]$ \\end{{document}}";
            }

            var mathMLContent = StiMathFormulaCache.GetLatextMatml(latextContent);
            if (string.IsNullOrEmpty(mathMLContent))
            {
                mathMLContent = StiMathHelper.GetMatmML(latextContent);
                StiMathFormulaCache.SetLatextMatml(latextContent, mathMLContent);
            }

            var svgMathText = StiMathFormulaCache.GetSvg(mathMLContent, fontSize, colorHex);
            if (svgMathText == null)
            {
                svgMathText = StiMathHelper.GetSvg(mathMLContent, fontSize, colorHex);
                StiMathFormulaCache.SetSvg(svgMathText, latextContent, fontSize, colorHex);
            }

            return svgMathText;
        }

        public static RectangleF GetSvgRect(StiMathFormula mathFormula, string svgMathFormula, RectangleF rect)
        {
            var sizeSvg = GetSizeSVG(svgMathFormula, 1);

            var rectSvgX = 0f;
            var rectSvgY = 0f;

            switch (mathFormula.HorAlignment)
            {
                case StiHorAlignment.Center:
                    rectSvgX = (rect.Width - sizeSvg.Width) / 2;
                    break;

                case StiHorAlignment.Right:
                    rectSvgX = rect.Width - sizeSvg.Width;
                    break;
            }

            switch (mathFormula.VertAlignment)
            {
                case StiVertAlignment.Center:
                    rectSvgY = (rect.Height - sizeSvg.Height) / 2;
                    break;

                case StiVertAlignment.Bottom:
                    rectSvgY = rect.Height - sizeSvg.Height;
                    break;
            }

            return new RectangleF(rectSvgX, rectSvgY, sizeSvg.Width, sizeSvg.Height);
        }

        private static SizeF GetSizeSVG(string svgText, float zoom)
        {
            var widthSVG = 0f;
            var heightSVG = 0f;

            var part = svgText.Split(new string[] { "viewBox" }, StringSplitOptions.None);
            if (part.Length > 1)
            {
                widthSVG = GetWidth(part[0], zoom);
                heightSVG = GetHeight(part[0], zoom);
            }

            return new SizeF(widthSVG, heightSVG);
        }

        private static float GetWidth(string pathSVG, float zoom)
        {
            try
            {
                var regex = new Regex(@"(?<widthAtr>\bwidth="")(?<valueWidth>([0-9]*[.,][0-9]*|[0-9]*))");

                var widthText = "";

                regex.Replace(pathSVG, m => { widthText = m.Groups["valueWidth"].Value; return string.Empty; });

                if (float.TryParse(widthText, out float widthValue))
                {
                    var factor = 100 / 74f;
                    return widthValue * zoom * factor;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static float GetHeight(string pathSVG, float zoom)
        {
            try
            {
                var regex = new Regex(@"(?<heightAtr>\bheight="")(?<valueHeight>([0-9]*[.,][0-9]*|[0-9]*))");

                var heightText = "";

                regex.Replace(pathSVG, m => { heightText = m.Groups["valueHeight"].Value; return string.Empty; });

                if (float.TryParse(heightText, out float heightValue))
                {
                    var factor = 100 / 74f;
                    return heightValue * zoom * factor;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
    }
}
