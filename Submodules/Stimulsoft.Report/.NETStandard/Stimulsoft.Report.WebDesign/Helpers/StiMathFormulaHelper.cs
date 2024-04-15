#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using Stimulsoft.Report.Components;
using System.Collections;
using System.Text.RegularExpressions;

namespace Stimulsoft.Report.Web
{
    internal class StiMathFormulaHelper
    {
        #region Fields
        private static Hashtable mathGroups = new Hashtable()
        {
            ["Basic"] = new string[]
            {
                "\\:", "\\quad", "\\plus", "\\minus", "\\div", "\\times", "=",  "\\neq", "x_a^b", "\\sqrt[n]{ab}", "\\overline{ab}", "(" , ")", "\\lbrace" , "\\rbrace",
                "\\sum_a^b", "\\lim_{a \\rightarrow b}", "\\sti{}"
            },
            ["Arrows"] = new string[]
            {
                "\\to", "\\longrightarrow", "\\Rightarrow", "\\Longrightarrow", "\\hookrightarrow", "\\mapsto", "\\longmapsto",
                "\\gets", "\\longleftarrow", "\\Leftarrow","\\Longleftarrow","\\hookleftarrow",
                "\\leftrightarrow","\\longleftrightarrow","\\Leftrightarrow","\\Longleftrightarrow",
                "\\uparrow", "\\Uparrow", "\\downarrow","\\Downarrow","\\updownarrow","\\Updownarrow",
                "\\nearrow","\\searrow","\\swarrow","\\nwarrow","\\leftharpoondown",
                "\\leftharpoonup","\\rightharpoonup","\\rightharpoondown","\\rightleftharpoons"
            },
            ["Functions"] = new string[]
            {
                "\\log" ,"\\lg" , "\\ln" ,"\\arg","\\ker" , "\\dim" ,"\\hom","\\deg" , "\\exp",
                "\\sin","\\cos" , "\\tan" ,"\\cot","\\sec" , "\\csc" ,"\\sinh" ,"\\cosh" , "\\tanh" ,"\\coth"
            },
            ["Maths"] = new string[]
            {
                "\\int", "\\int_{a}^{b}","\\oint","\\oint_a^b","\\sum","\\sum_a^b","\\coprod","\\coprod_a^b",
                "\\prod","\\prod_a^b","\\bigcap","\\bigcap_a^b","\\bigcup","\\bigcup_a^b",
                "\\sqrt{ab}","\\sqrt[n]{ab}","\\log_{a}{b}","\\lg{ab}","a^{b}","a_{b}","x_a^b",
                "\\overline{ab}","\\underline{ab}","\\frac{ab}{cd}","\\lim_{a \\rightarrow b}"
            },
            ["Formulas"] = new string[]
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
            },
            ["Alphabets"] = new string[]
            {
                "\\alpha","\\beta","\\gamma","\\delta","\\varepsilon",
                "\\zeta","\\eta","\\theta","\\vartheta","\\iota","\\kappa","\\lambda",
                "\\mu","\\nu","\\xi","\\pi", "\\varpi","\\rho","\\varrho","\\sigma",
                "\\varsigma","\\tau","\\upsilon","\\phi","\\varphi","\\chi","\\psi","\\omega",
            },
            ["Operators"] = new string[]
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
            }
        };
        #endregion

        public static void GetMathFormulaInfo(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            callbackResult["mathGroups"] = BuldMetaInfo(param);
        }

        private static Hashtable BuldMetaInfo(Hashtable param)
        {
            var metaInfoHashtable = new Hashtable();

            foreach (DictionaryEntry mathGroup in mathGroups)
            {
                var groupType = mathGroup.Key as string;
                var groupArray = mathGroup.Value as string[];
                var groupValues = new ArrayList();

                foreach (var formula in groupArray) 
                {
                    var valueObject = new Hashtable();
                    valueObject["value"] = formula;
                    valueObject["icon"] = GetMathIcon(groupType, formula, param);
                    groupValues.Add(valueObject);
                }
                metaInfoHashtable[groupType] = groupValues;
            }

            return metaInfoHashtable;
        }

        public static string GetMathIcon(string groupType, string formula, Hashtable param)
        {
            var scaleFactor = StiReportEdit.StrToDouble(param["imagesScalingFactor"] as string);
            var scaleSuffix = StiScalingImagesHelper.GetScaleSuffix(StiReportEdit.StrToDouble(param["imagesScalingFactor"] as string));
            var assembly = typeof(StiWebDesigner).Assembly;
            var imagePath = $"{typeof(StiWebDesigner).Namespace}.Designer.MathImages.{groupType}.{GetImageFileName(groupType, formula)}{scaleSuffix}.png";

            if (!StiImageUtils.ExistsImage(assembly, imagePath))
            {
                imagePath = imagePath.Replace($"{scaleSuffix}.png", ".png");
                scaleFactor = 1;
            }

            if (StiImageUtils.ExistsImage(assembly, imagePath))
                return StiViewerResourcesHelper.GetImageData(assembly, imagePath, scaleFactor);

            return null;
        }

        private static string GetImageFileName(string groupType, string formula)
        {
            formula = formula.Replace("\\", "").Replace("|", "").Replace(" ", "-");

            if (groupType == "Basic")
            {
                formula = formula.Replace(":", "space").Replace("=", "eq").Replace("(", "lgroup").Replace(")", "rgroup");
                if (formula.Equals("sti{}"))
                    return "sti";
            }

            string[] split = Regex.Split(formula, @"(?<!^)(?=[A-Z])");
            formula = string.Join("_", split);

            if (formula.Length > 0 && char.IsUpper(formula[0]))
                formula = $"_{formula}";

            return formula.ToLower();
        }
    }
}