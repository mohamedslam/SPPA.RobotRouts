/*  
    This file is part of LatexMath2MathML.

    LatexMath2MathML is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LatexMath2MathML is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with LatexMath2MathML.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace LatexMath2MathML
{
    /// <summary>
    /// The proxy class between a command and the corresponding converter.
    /// </summary>
    internal class CommandConverter : NamedConverter
    {
        /// <summary>
        /// The hash table of command converters.
        /// </summary>
        public static readonly IDictionary<int, CommandConverter> CommandConverters = new Dictionary<int, CommandConverter>
        {
            #region Initialization
            {(new FracCommandConverter()).GetHashCode(), new FracCommandConverter()},
            {(new DfracCommandConverter()).GetHashCode(), new DfracCommandConverter()},
            {(new TfracCommandConverter()).GetHashCode(), new TfracCommandConverter()},
            {(new SqrtCommandConverter()).GetHashCode(), new SqrtCommandConverter()},            
            //{(new ItemCommandConverter()).GetHashCode(), new ItemCommandConverter()},

			//TODO WIll we need these in an equation editor? Probably not
            {(new StiUnderlineAccentConverter()).GetHashCode(), new StiUnderlineAccentConverter()},
            {(new OverlineAccentConverter()).GetHashCode(), new OverlineAccentConverter()},
            {(new HatAccentConverter()).GetHashCode(), new HatAccentConverter()},
            {(new WidehatAccentConverter()).GetHashCode(), new WidehatAccentConverter()},
            {(new CheckAccentConverter()).GetHashCode(), new CheckAccentConverter()},
            {(new TildeAccentConverter()).GetHashCode(), new TildeAccentConverter()},
            {(new WidetildeAccentConverter()).GetHashCode(), new WidetildeAccentConverter()},
            {(new AcuteAccentConverter()).GetHashCode(), new AcuteAccentConverter()},
            {(new GraveAccentConverter()).GetHashCode(), new GraveAccentConverter()},
            {(new DotAccentConverter()).GetHashCode(), new DotAccentConverter()},
            {(new DdotAccentConverter()).GetHashCode(), new DdotAccentConverter()},
            {(new BreveAccentConverter()).GetHashCode(), new BreveAccentConverter()},
            {(new BarAccentConverter()).GetHashCode(), new BarAccentConverter()},
            {(new VecAccentConverter()).GetHashCode(), new VecAccentConverter()},
            {(new HatTextAccentConverter()).GetHashCode(), new HatTextAccentConverter()},
            {(new CheckTextAccentConverter()).GetHashCode(), new CheckTextAccentConverter()},
            {(new TildeTextAccentConverter()).GetHashCode(), new TildeTextAccentConverter()},
            {(new AcuteTextAccentConverter()).GetHashCode(), new AcuteTextAccentConverter()},
            {(new GraveTextAccentConverter()).GetHashCode(), new GraveTextAccentConverter()},
            {(new DotTextAccentConverter()).GetHashCode(), new DotTextAccentConverter()},
            {(new DdotTextAccentConverter()).GetHashCode(), new DdotTextAccentConverter()},
            {(new BreveTextAccentConverter()).GetHashCode(), new BreveTextAccentConverter()},
            {(new BarTextAccentConverter()).GetHashCode(), new BarTextAccentConverter()},

            {(new StateCommandConverter()).GetHashCode(), new StateCommandConverter()},
            {(new StatexCommandConverter()).GetHashCode(), new StatexCommandConverter()},
            {(new ProcedureCommandConverter()).GetHashCode(), new ProcedureCommandConverter()},
            {(new EndProcedureCommandConverter()).GetHashCode(), new EndProcedureCommandConverter()},
            {(new MathcalCommandConverter()).GetHashCode(), new MathcalCommandConverter()},
            {(new StiMathbbCommandConverter()).GetHashCode(), new StiMathbbCommandConverter()},
            
			#endregion
        };


        public static readonly Dictionary<string, string> CommandConstants = new Dictionary<string, string>
        {
            #region Initialization
			{"\\", "\n</mrow>\n<mrow>\n"},
            {"footnoterule", "\n</mrow>\n<mrow>\n"},
            {"Alpha", "&#x391;<!-- &Alpha; -->"},
            {"alpha", "&#x3B1;<!-- &alpha; -->"},
            {"Beta", "&#x392;<!-- &Beta; -->"},
            {"beta", "&#x3B2;<!-- &beta; -->"},
            {"Gamma", "&#x393;<!-- &Gamma; -->"},
            {"gamma", "&#x3B3;<!-- &gamma; -->"},
            {"Delta", "&#x394;<!-- &Delta; -->"},
            {"delta", "&#x3B4;<!-- &delta; -->"},
            {"Epsilon", "&#x395;<!--&Epsilon; -->"},
            {"epsilon", "&#x3B5;<!-- &epsilon; -->"},
            {"varepsilon", "&#x3B5;<!-- &epsilon; -->"},
            {"Zeta", "&#x396;<!-- &Zeta; -->"},
            {"zeta", "&#x3B6;<!-- &zeta; -->"},
            {"Eta", "&#x397;<!-- &Eta; -->"},
            {"eta", "&#x3B7;<!-- &eta; -->"},
            {"Theta", "&#x398;<!-- &Theta; -->"},
            {"theta", "&#x3B8;<!-- &theta; -->"},
            {"vartheta", "&#x03D1;<!-- &theta -->"},
            {"Iota", "&#x399;<!-- &Iota; -->"},
            {"iota", "&#x3B9;<!-- &iota; -->"},
            {"Kappa", "&#x39a;<!-- &Kappa; -->"},
            {"kappa", "&#x3BA;<!-- &kappa; -->"},
            {"varkappa", "&#x03F0;<!-- &kappav; -->"},
            {"Lambda", "&#x39b;<!-- &Lambda; -->"},
            {"lambda", "&#x3BB;<!-- &lambda; -->"},
            {"Mu", "&#x39c;<!-- &Mu; -->"},
            {"mu", "&#x3BC;<!-- &mu; -->"},
            {"nu", "&#x3BD;<!-- &nu; -->"},
            {"Xi", "&#x39e;<!-- &Xi; -->"},
            {"xi", "&#x3BE;<!-- &xi; -->"},
            {"omicron", "&#x3BF;<!-- &omicron; -->"},
            {"Pi", "&#x3a0;<!-- &Pi; -->"},
            {"pi", "&#x3C0;<!-- &pi; -->"},
            {"varpi", "&#x03D6;<!-- &piv; -->"},
            {"rho", "&#x3C1;<!-- &rho; -->"},
            {"varrho", "&#x03F1;<!--&rhov -->"},
            {"Sigma", "&#x3a3;<!--&Sigma -->"},
            {"sigma", "&#x3C3;<!-- &sigma; -->"},
            {"varsigma", "&#x03C2;<!--&sigmav; -->"},
            {"tau", "&#x3C4;<!-- &tau; -->"},
            {"Upsilon", "&#x3a5;<!-- &Upsilon; -->"},
            {"upsilon", "&#x3C5;<!-- &upsilon; -->"},
            {"Phi", "&#x3a6;<!-- &Phi; -->"},
            {"phi", "&#x3c6;<!-- &phi; -->"},
            {"varphi", "&#x03D5;<!-- &phiv; -->"},
            {"chi", "&#x3C7;<!-- &chi; -->"},
            {"Psi", "&#x3a8;<!-- &Psi; -->"},
            {"psi", "&#x3C8;<!-- &psi; -->"},
            {"Omega", "&#x3a9;<!-- &Omega; -->"},
            {"omega", "&#x3C9;<!-- &omega; -->"},
            {"dag", "&#x2020;<!-- &dagger; -->"},
            {"ddag", "&#x2021;<!-- &Dagger; -->"},
            {"pounds", "&#x00A3;<!-- &pound; -->"},
            {"textsterling", "&#x00A3;<!-- &pound; -->"},
            {"euro", "&#x20AC;<!-- &euro; -->"},
            {"EUR", "&#x20AC;<!-- &euro; -->"},
            {"S", "&#x00A7;<!-- &sect; -->"},
            {"hline", "<hr />"},
            {"vert", "|"},
            {"~", "~"},
            #endregion
        };

        public static readonly Dictionary<string, string> MathFunctionsCommandConstants = new Dictionary<string, string>
        {          
            #region Initialization
            {"log", "<mi>log</mi>\n"},
            {"arg", "<mi>arg</mi>\n"},
            {"hom", "<mi>hom</mi>\n"},
            {"lg", "<mi>lg</mi>\n"},
            {"deg", "<mi>deg</mi>\n"},
            {"ln", "<mi>ln</mi>\n"},
            {"dim", "<mi>dim</mi>\n"},
            {"exp", "<mi>exp</mi>\n"},
            {"sin", "<mi>sin</mi>\n"},
            {"cos", "<mi>cos</mi>\n"},
            {"tan", "<mi>tan</mi>\n"},
            {"sec", "<mi>sec</mi>\n"},
            {"csc", "<mi>csc</mi>\n"},
            {"cot", "<mi>cot</mi>\n"},
            {"sinh", "<mi>sinh</mi>\n"},
            {"cosh", "<mi>cosh</mi>\n"},
            {"tanh", "<mi>tanh</mi>\n"},
            {"sech", "<mi>sech</mi>\n"},
            {"csch", "<mi>csch</mi>\n"},
            {"coth", "<mi>coth</mi>\n"},
            {"arcsin", "<mi>arcsin</mi>\n"},
            {"arccos", "<mi>arccos</mi>\n"},
            {"arctan", "<mi>arctan</mi>\n"},
            {"arccosh", "<mi>arccosh</mi>\n"},
            {"arccot", "<mi>arccot</mi>\n"},
            {"arccoth", "<mi>arccoth</mi>\n"},
            {"arccsc", "<mi>arccsc</mi>\n"},
            {"arccsch", "<mi>arccsch</mi>\n"},
            {"arcsec", "<mi>arcsec</mi>\n"},
            {"arcsech", "<mi>arcsech</mi>\n"},
            {"arcsinh", "<mi>arcsinh</mi>\n"},
            {"arctanh", "<mi>arctanh</mi>\n"},             
            #endregion
        };

        public static readonly Dictionary<string, string> MathFunctionsScriptCommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"int", "<mo>&#x222B;<!-- &int; --></mo>\n"},
            {"iint", "<mo>&#x222C;<!-- iint --></mo>\n"},
            {"iiint", "<mo>&#x222D;<!-- iiint --></mo>\n"},
            {"oint", "<mo>&#x222E;<!-- oint --></mo>\n"},
            {"oiint", "<mo>&#x222F;<!-- oiint --></mo>\n"},
            {"oiiint", "<mo>&#x2230;<!-- oiiint --></mo>\n"},
            {"ointclockwise", "<mo>&#x2232;<!-- ointclockwise --></mo>\n"},
            {"ointctrclockwise", "<mo>&#x2233;<!-- ointctrclockwise --></mo>\n"},

            {"sum", "<mo>&#x2211;<!-- &sum; --></mo>\n"},
            {"bigcap", "<mo>&#x22C2;<!-- &bigcap; --></mo>\n"},
            {"bigotimes", "<mo>&#x2A02;<!-- &bigotimes; --></mo>\n" },
            {"bigwedge", "<mo>&#x22C0;<!-- &bigwedge; --></mo>\n"},
            {"lim", "<mo>lim</mo>\n"},
            {"max", "<mo>max</mo>\n"},
            {"inf", "<mo>inf</mo>\n"},
            {"gcd", "<mo>gcd</mo>\n"},

            {"prod", "<mo>&#x220F;<!-- &prod; --></mo>\n"},
            {"coprod", "<mo>&#x2210;<!-- &coprod; --></mo>\n"},
            {"bigodot", "<mo>&#x2A00;<!-- &bigodot; --></mo>\n"},
            {"biguplus", "<mo>&#x2A04;<!-- &biguplus; --></mo>\n"},
            {"limsup", "<mo>lim sup</mo>\n"},
            {"min", "<mo>min</mo>\n"},
            {"det", "<mo>det</mo>\n"},

            {"bigcup", "<mo>&#x22C3;<!-- &bigcup; --></mo>\n"},
            {"bigoplus", "<mo>&#x2A01;<!-- &bigoplus; --></mo>\n"},
            {"bigvee", "<mo>&#x22C1;<!-- &bigvee; --></mo>\n"},
            {"bigsqcup", "<mo>&#x2A06;<!-- &bigsqcup; --></mo>\n"},
            {"liminf", "<mo>lim inf</mo>\n"},
            {"sup", "<mo>sup</mo>\n"},
            {"Pr", "<mo>Pr</mo>\n"},

            {"ker", "<mo>ker</mo>\n"},
            #endregion
        };

        public static readonly Dictionary<string, string> MathCommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"displaystyle", ""},

            {"ne", "<mo>&#x2260;<!-- &ne; --></mo>\n"},
            {"neq", "<mo>&#x2260;<!-- &ne; --></mo>\n"},
            {"approx", "<mo>&#x2248;<!-- &asymp; --></mo>\n"},
            {"ll", "<mo>&#x226A;<!-- &Lt; --></mo>\n"},
            {"succ", "<mo>&#x227B;<!-- &succ; --></mo>\n"},
            {"dashv", "<mo>&#x22A3;<!-- &dashv; --></mo>\n"},
            {"mid", "<mo>&#x2223;<!-- &mid; --></mo>\n"},
            {"smile", "<mo>&#x2323;</mo>\n"},
            {"bowtie", "<mo>&#x22C8;<!-- &bowtie; --></mo>\n"},
            {"frown", "<mo>&#x2322;<!-- &frown; --></mo>\n"},

            {"equiv", "<mo>&#x2261;<!-- equiv --></mo>\n"},
            {"leq", "<mo>&#8804; <!-- leq --> </mo> \n"},
            {"le", "<mo>&#8804; <!-- le --> </mo> \n"},
            {"geq", "<mo>&#8805; <!-- geq --> </mo> \n"},
            {"ge", "<mo>&#8805; <!-- ge --> </mo> \n"},
            {"sim", "<mo>&#x223C;<!-- &sim; --></mo>\n"},
            {"cup", "<mo>&#x2230;<!-- &cup; --></mo>\n"},
            {"in", "<mo>&#x2208;<!-- &isin; --></mo>\n"},
            {"notin", "<mo>&#x2209;<!-- &notin; --></mo>\n"},
            {"ni", "<mo>&#x220B;<!-- &ni; --></mo>\n"},
            {"forall", "<mo>&#x2200;<!-- &forall; --></mo>\n"},
            {"infty", "<mo>&#x221E;<!-- &infty; --></mo>\n"},
            {"exists", "<mo>&#x2203;<!-- &exist; --></mo>\n"},
            {"nexists", "<mo>&#x2204;<!-- &nexists --></mo>\n"},
            {"to", "<mo>&#x2192;<!-- &rarr; --></mo>\n"},
            {"gg", "<mo>&#x226B;<!-- &Gt; --></mo>\n"},
            {"subset", "<mo>&#x2282;<!-- &sub; --></mo>\n"},
            {"subseteq", "<mo>&#x2286;<!-- &sube; --></mo>\n"},
            {"nsubseteq", "<mo>&#x2288;<!-- &nsube; --></mo>\n"},
            {"supset", "<mo>&#x2283;<!-- &sup; --></mo>\n"},
            {"supseteq", "<mo>&#x2287;<!-- &supe; --></mo>\n"},
            {"nsupseteq", "<mo>&#x2289;<!-- &nsupe; --></mo>\n"},
            {"sqsubset", "<mo>&#x228F;<!-- &sqsub; --></mo>\n"},
            {"sqsubseteq", "<mo>&#x2291;<!-- &sqsube; --></mo>\n"},
            {"sqsupset", "<mo>&#x2290;<!-- &sqsup; --></mo>\n"},
            {"sqsupseteq", "<mo>&#x2292;<!-- &sqsupe; --></mo>\n"},
            {"preceq", "<mo>&#x227C;<!-- &cupre; --></mo>\n"},
            {"succeq", "<mo>&#x227D;<!-- &sccue; --></mo>\n"},
            {"doteq", "<mo>&#x2250;<!-- &esdot; --></mo>\n"},
            {"cong", "<mo>&#x2245;<!-- &cong; --></mo>\n"},
            {"simeq", "<mo>&#x2243;<!-- &sime; --></mo>\n"},
            {"propto", "<mo>&#x221D;<!-- &vprop; --></mo>\n"},
            {"parallel", "<mo>&#x20E6;</mo>\n"},
            {"asymp", "<mo>&#x224d;<!-- &asymp; --></mo>\n"},
            {"vdash", "<mo>&#x22A2;<!-- &vdash; --></mo>\n"},
            {"models", "<mo>&#x22A7;<!-- &models; --></mo>\n"},
            {"perp", "<mo>&#x22A5;<!-- &bottom; --></mo>\n"},
            {"prec", "<mo>&#x227A;<!-- &pr; --></mo>\n"},
            {"sphericalangle", "<mo>&#x2222;<!-- &angsph; --></mo>\n"},
            {"ast", "<mo></mo>\n"},

            {"pm", "<mo>&#xB1;<!-- pm --></mo>\n"},
            {"div", "<mo>&#xF7;<!-- div --></mo>\n"},
            {"circ", "<mo>&#x2C6;<!-- circ --></mo>\n"},
            {"sqcup", "<mo>&#x2294;<!-- sqcup --></mo>\n"},
            {"oplus", "<mo>&#x2295;<!-- &oplus --></mo>\n"},
            {"odot", "<mo>&#x2299;<!-- &odot --></mo>\n"},
            {"triangleright", "<mo>&#x25B9;<!-- &triangleright --></mo>\n"},
            {"wr", "<mo>&#x2240;<!-- &wr --></mo>\n"},
            {"ddagger", "<mo>&#x2021;<!-- &ddagger --></mo>\n"},
            {"bigtriangledown", "<mo>&#x25BD;<!-- &bigtriangledown --></mo>\n"},
            {"mp", "<mo>&#x2213;<!-- mp --></mo>\n"},
            {"setminus", "<mo>&#x2216;<!-- setminus --></mo>\n"},
            {"bullet", "<mo>&#x2022;<!-- bullet --></mo>\n"},
            {"uplus", "<mo>&#x228E;<!-- uplus --></mo>\n"},
            {"vee", "<mo>&#x2228;<!-- vee --></mo>\n"},
            {"ominus", "<mo>&#x2296;<!-- &ominus --></mo>\n"},
            {"oslash", "<mo>&#x2298;<!-- &oslash --></mo>\n"},
            {"amalg", "<mo>&#x2A3F;<!-- &amalg --></mo>\n"},
            {"star", "<mo>&#x2606;<!-- &star --></mo>\n"},
            {"bigtriangleup", "<mo>&#x25B3;<!-- &bigtriangleup --></mo>\n"},
            {"times","<mo>&#xD7;<!-- &times --></mo>\n"},
            {"cdot", "<mo>&#x10B;<!-- &cdot --></mo>\n"},
            {"cap", "<mo>&#x2229;<!-- &cap; --></mo>\n"},
            {"sqcap", "<mo>&#x2293;<!-- &sqcap; --></mo>\n"},
            {"wedge", "<mo>&#x2227;<!-- &wedge; --></mo>\n"},
            {"otimes", "<mo>&#x2297;<!-- &otimes --></mo>\n"},
            {"triangleleft", "<mo>&#x25C3;<!-- &triangleleft --></mo>\n"},
            {"diamond", "<mo>&#x22CF;<!-- &diamond; --></mo>\n"},
            {"dagger", "<mo>&#x2020;<!-- &dagger; --></mo>\n"},
            {"bigcirc", "<mo>&#x25EF;<!-- &bigcirc; --></mo>\n"},

            {"doteqdot", "<mo>&#x2251;<!-- doteqdot --></mo>\n"},
            {"gtrless", "<mo>&#x2277;<!-- gtrless --></mo>\n"},
            
            {"quad", "<mspace width=\"2em\"/>"},
            {"qquad", "<mspace width=\"4em\"/>"},
            {";", "<mspace width=\"1.1em\"/>"},
            {":", "<mspace width=\"0.9em\"/>"},
            {",", "<mspace width=\"0.7em\"/>"},
            {"!", "<mspace width=\"0.3em\"/>"}, //Should be negative space... but is not
            {"Leftrightarrow", "<mo>&#x21D4;<!-- &hArr; --></mo>\n"},
            {"Updownarrow", "<mo>&#x21D5;<!-- \\Updownarrow --></mo>\n"},
            {"Downarrow", "<mo>&#x21D3;<!-- &dArr; --></mo>\n"},
            {"Rightarrow", "<mo>&#x21D2;<!-- &rArr; --></mo>\n"},
            {"Uparrow", "<mo>&#x21D1;<!-- &uArr; --></mo>\n"},
            {"Leftarrow", "<mo>&#x21D0;<!-- &lArr; --></mo>\n"},
            {"nRightarrow", "<mo>&#x21CF;<!-- \\nRightarrow --></mo>\n"},
            {"nLeftrightarrow", "<mo>&#x21CE;<!-- \\nLeftrightarrow --></mo>\n"},
            {"nLeftarrow", "<mo>&#x21CD;<!-- \\nLeftarrow --></mo>\n"},
            {"rightleftharpoons", "<mo>&#x21CC;<!-- \\rightleftharpoons --></mo>\n"},
            {"leftrightharpoons", "<mo>&#x21CB;<!-- \\leftrightharpoons --></mo>\n"},
            {"downdownarrows", "<mo>&#x21CA;<!-- \\downdownarrows --></mo>\n"},
            {"rightrightarrows", "<mo>&#x21C9;<!-- \\rightrightarrows --></mo>\n"},
            {"upuparrows", "<mo>&#x21C8;<!-- \\upuparrows --></mo>\n"},
            {"leftleftarrows", "<mo>&#x21C7;<!-- \\leftleftarrows --></mo>\n"},
            {"leftrightarrows", "<mo>&#x21C6;<!-- \\leftrightarrows --></mo>\n"},
            {"updownarrows", "<mo>&#x21C5;<!-- \\updownarrows --></mo>\n"},
            {"rightleftarrows", "<mo>&#x21C4;<!-- \\rightleftarrows --></mo>\n"},
            {"downharpoonleft", "<mo>&#x21C3;<!-- \\downharpoonleft --></mo>\n"},
            {"downharpoonright", "<mo>&#x21C2;<!-- \\downharpoonright --></mo>\n"},
            {"rightharpoondown", "<mo>&#x21C1;<!-- \\rightharpoondown --></mo>\n"},
            {"rightharpoonup", "<mo>&#x21C0;<!-- \\rightharpoonup --></mo>\n"},
            {"upharpoonleft", "<mo>&#x21BF;<!-- \\upharpoonleft --></mo>\n"},
            {"upharpoonright", "<mo>&#x21BE;<!-- \\upharpoonright --></mo>\n"},
            {"leftharpoondown", "<mo>&#x21BD;<!-- \\leftharpoondown --></mo>\n"},
            {"leftharpoonup", "<mo>&#x21BC;<!-- \\leftharpoonup --></mo>\n"},
            {"circlearrowright", "<mo>&#x21BB;<!-- \\circlearrowright --></mo>\n"},
            {"circlearrowleft", "<mo>&#x21BA;<!-- \\circlearrowleft --></mo>\n"},
            {"curvearrowright", "<mo>&#x21B7;<!-- \\curvearrowright --></mo>\n"},
            {"curvearrowleft", "<mo>&#x21B6;<!-- \\curvearrowleft --></mo>\n"},
            {"Rsh", "<mo>&#x21B1;<!-- \\Rsh --></mo>\n"},
            {"Lsh", "<mo>&#x21B0;<!-- \\Lsh --></mo>\n"},
            {"looparrowright", "<mo>&#x21AC;<!-- \\looparrowright --></mo>\n"},
            {"looparrowleft", "<mo>&#x21AB;<!-- \\looparrowleft --></mo>\n"},
            {"rightarrowtail", "<mo>&#x21A3;<!-- \\rightarrowtail --></mo>\n"},
            {"leftarrowtail", "<mo>&#x21A2;<!-- \\leftarrowtail --></mo>\n"},
            {"twoheaddownarrow", "<mo>&#x21A1;<!-- \\twoheaddownarrow --></mo>\n"},
            {"twoheadrightarrow", "<mo>&#x21A0;<!-- \\twoheadrightarrow --></mo>\n"},
            {"twoheaduparrow", "<mo>&#x219F;<!-- \\twoheaduparrow --></mo>\n"},
            {"twoheadleftarrow", "<mo>&#x219E;<!-- \\twoheadleftarrow --></mo>\n"},
            {"swarrow", "<mo>&#x2199;<!-- \\swarrow --></mo>\n"},
            {"searrow", "<mo>&#x2198;<!-- \\searrow --></mo>\n"},
            {"nearrow", "<mo>&#x2197;<!-- \\nearrow --></mo>\n"},
            {"nwarrow", "<mo>&#x2196;<!-- \\nwarrow --></mo>\n"},
            {"updownarrow", "<mo>&#x2195;<!-- \\updownarrow --></mo>\n"},
            {"leftrightarrow", "<mo>&#x2194;<!-- \\leftrightarrow --></mo>\n"},
            {"downarrow", "<mo>&#x2193;<!-- &darr; --></mo>\n"},
            {"rightarrow", "<mo>&#x2192;<!-- &rarr; --></mo>\n"},
            {"uparrow", "<mo>&#x2191;<!-- &uarr; --></mo>\n"},
            {"leftarrow", "<mo>&#x2190;<!-- &larr; --></mo>\n"},

            {"LongRightArrow", "<mo>&#x27F6;<!-- &LongRightArrow; --></mo>\n"},
            {"mapsto", "<mo>&#x21A6;<!-- &mapsto; --></mo>\n"},
            {"gets", "<mo></mo>\n"},
            {"LongLeftArrow", "<mo>&#x27F5;<!-- &LongLeftArrow; --></mo>\n"},
            {"longrightarrow", "<mo>&#x27F6;<!-- &longrightarrow; --></mo>\n"},
            {"hookrightarrow", "<mo>&#x21AA;<!-- &hookrightarrow; --></mo>\n"},
            {"longmapsto", "<mo>&#x27FC;<!-- &longmapsto; --></mo>\n"},
            {"longleftarrow", "<mo>&#x27F5;<!-- &longleftarrow; --></mo>\n"},
            {"hookleftarrow", "<mo>&#x21A9;<!-- &hookleftarrow; --></mo>\n"},
            {"longleftrightarrow", "<mo>&#x27F7;<!-- &longleftrightarrow; --></mo>\n"},
            {"Longleftrightarrow", "<mo>&#x27F7;<!-- &Longleftrightarrow; --></mo>\n"},

            {"varnothing", "<mo>&#x2205;<!-- &empty; --></mo>\n"},
            {"triangle", "<mo>&#x2206;<!-- \\triangle --></mo>\n"},
            {"nabla", "<mo>&#x2207;<!-- &nabla; --></mo>\n"},
            {"triangledown", "<mo>&#x2207;<!-- &nabla; --></mo>\n"},
            {"blacksquare", "<mo>&#x220E;<!-- \\blacksquare --></mo>\n"},
            {"partial", "<mo>&#x2202;<!-- &part; --></mo>\n"},
            {"dashrightarrow", "<mo>&#x21E2;<!-- \\dashrightarrow --></mo>\n"},
            {"dashleftarrow", "<mo>&#x21E0;<!-- \\dashleftarrow --></mo>\n"},
            {"copyright", "<mo>&#x00a9;<!-- \\copyright --></mo>\n"},
            {"P", "<mo>&#x00B6;<!-- \\P --></mo>\n"},

            {"left", ""},
            {"right", ""},
            {"[", "<mo>[</mo>\n"},
            {"]", "<mo>]</mo>\n"},
            {"{", "<mo>{</mo>\n"},
            {"}", "<mo>}</mo>\n"},
            {"|", "<mo>|</mo>\n"},

			//{"dots", "<mi>&#x2026;<!-- &hellip; --></mi>\n"},
			{"dots", "<mo>&hellip;</mo>\n"},
            {"dotsm", "<mo>&ctdot;</mo>\n"},
            {"vdots", "<mo>&dtdot;</mo>\n"},
            {"ddots", "<mo>&vellip;</mo>\n"},

            #endregion
        };
        public static readonly Dictionary<string, string> StiLatexCommandConstants = new Dictionary<string, string>
        {
            #region Initialization
            {"Aacute","<mo>&#x00C1;</mo>\n"},{"aacute","<mo>&#x00E1;</mo>\n"},{"Abreve","<mo>&#x0102;</mo>\n"},{"abreve","<mo>&#x0103;</mo>\n"},{"ac","<mo>&#x223E;</mo>\n"},{"acd","<mo>&#x223F;</mo>\n"},{"acE","<mo>&#x223E;</mo>\n"},{"Acirc","<mo>&#x00C2;</mo>\n"},{"acirc","<mo>&#x00E2;</mo>\n"},{"acute","<mo>&#x00B4;</mo>\n"},{"Acy","<mo>&#x0410;</mo>\n"},{"acy","<mo>&#x0430;</mo>\n"},{"AElig","<mo>&#x00C6;</mo>\n"},{"aelig","<mo>&#x00E6;</mo>\n"},{"af","<mo>&#x2061;</mo>\n"},{"Afr","<mo>&#xD504;</mo>\n"},{"afr","<mo>&#xD51E;</mo>\n"},{"Agrave","<mo>&#x00C0;</mo>\n"},{"agrave","<mo>&#x00E0;</mo>\n"},{"aleph","<mo>&#x2135;</mo>\n"},{"alpha","<mo>&#x03B1;</mo>\n"},{"Amacr","<mo>&#x0100;</mo>\n"},{"amacr","<mo>&#x0101;</mo>\n"},{"amalg","<mo>&#x2A3F;</mo>\n"},{"amp","<mo>&#x0026;</mo>\n"},{"And","<mo>&#x2A53;</mo>\n"},{"and","<mo>&#x2227;</mo>\n"},{"andand","<mo>&#x2A55;</mo>\n"},{"andd","<mo>&#x2A5C;</mo>\n"},{"andslope","<mo>&#x2A58;</mo>\n"},{"andv","<mo>&#x2A5A;</mo>\n"},{"ang","<mo>&#x2220;</mo>\n"},{"ange","<mo>&#x29A4;</mo>\n"},{"angle","<mo>&#x2220;</mo>\n"},{"angmsd","<mo>&#x2221;</mo>\n"},{"angmsdaa","<mo>&#x29A8;</mo>\n"},{"angmsdab","<mo>&#x29A9;</mo>\n"},{"angmsdac","<mo>&#x29AA;</mo>\n"},{"angmsdad","<mo>&#x29AB;</mo>\n"},{"angmsdae","<mo>&#x29AC;</mo>\n"},{"angmsdaf","<mo>&#x29AD;</mo>\n"},{"angmsdag","<mo>&#x29AE;</mo>\n"},{"angmsdah","<mo>&#x29AF;</mo>\n"},{"angrt","<mo>&#x221F;</mo>\n"},{"angrtvb","<mo>&#x22BE;</mo>\n"},{"angrtvbd","<mo>&#x299D;</mo>\n"},{"angsph","<mo>&#x2222;</mo>\n"},{"angst","<mo>&#x212B;</mo>\n"},{"angzarr","<mo>&#x237C;</mo>\n"},{"Aogon","<mo>&#x0104;</mo>\n"},{"aogon","<mo>&#x0105;</mo>\n"},{"Aopf","<mo>&#xD538;</mo>\n"},{"aopf","<mo>&#xD552;</mo>\n"},{"ap","<mo>&#x2248;</mo>\n"},{"apacir","<mo>&#x2A6F;</mo>\n"},{"apE","<mo>&#x2A70;</mo>\n"},{"ape","<mo>&#x224A;</mo>\n"},{"apid","<mo>&#x224B;</mo>\n"},{"apos","<mo>&#x0027;</mo>\n"},{"ApplyFunction","<mo>&#x2061;</mo>\n"},{"approx","<mo>&#x2248;</mo>\n"},{"approxeq","<mo>&#x224A;</mo>\n"},{"Aring","<mo>&#x00C5;</mo>\n"},{"aring","<mo>&#x00E5;</mo>\n"},{"Ascr","<mo>&#xD49C;</mo>\n"},{"ascr","<mo>&#xD4B6;</mo>\n"},{"Assign","<mo>&#x2254;</mo>\n"},{"ast","<mo>&#x002A;</mo>\n"},{"asymp","<mo>&#x2248;</mo>\n"},{"asympeq","<mo>&#x224D;</mo>\n"},{"Atilde","<mo>&#x00C3;</mo>\n"},{"atilde","<mo>&#x00E3;</mo>\n"},{"Auml","<mo>&#x00C4;</mo>\n"},{"auml","<mo>&#x00E4;</mo>\n"},{"awconint","<mo>&#x2233;</mo>\n"},{"awint","<mo>&#x2A11;</mo>\n"},{"backcong","<mo>&#x224C;</mo>\n"},{"backepsilon","<mo>&#x03F6;</mo>\n"},{"backprime","<mo>&#x2035;</mo>\n"},{"backsim","<mo>&#x223D;</mo>\n"},{"backsimeq","<mo>&#x22CD;</mo>\n"},{"Backslash","<mo>&#x2216;</mo>\n"},{"Barv","<mo>&#x2AE7;</mo>\n"},{"barvee","<mo>&#x22BD;</mo>\n"},{"Barwed","<mo>&#x2306;</mo>\n"},{"barwed","<mo>&#x2305;</mo>\n"},{"barwedge","<mo>&#x2305;</mo>\n"},{"bbrk","<mo>&#x23B5;</mo>\n"},{"bbrktbrk","<mo>&#x23B6;</mo>\n"},{"bcong","<mo>&#x224C;</mo>\n"},{"Bcy","<mo>&#x0411;</mo>\n"},{"bcy","<mo>&#x0431;</mo>\n"},{"becaus","<mo>&#x2235;</mo>\n"},{"Because","<mo>&#x2235;</mo>\n"},{"because","<mo>&#x2235;</mo>\n"},{"bemptyv","<mo>&#x29B0;</mo>\n"},{"bepsi","<mo>&#x03F6;</mo>\n"},{"bernou","<mo>&#x212C;</mo>\n"},{"Bernoullis","<mo>&#x212C;</mo>\n"},{"beta","<mo>&#x03B2;</mo>\n"},{"beth","<mo>&#x2136;</mo>\n"},{"between","<mo>&#x226C;</mo>\n"},{"Bfr","<mo>&#xD505;</mo>\n"},{"bfr","<mo>&#xD51F;</mo>\n"},{"bigcap","<mo>&#x22C2;</mo>\n"},{"bigcirc","<mo>&#x25EF;</mo>\n"},{"bigcup","<mo>&#x22C3;</mo>\n"},{"bigodot","<mo>&#x2A00;</mo>\n"},{"bigoplus","<mo>&#x2A01;</mo>\n"},{"bigotimes","<mo>&#x2A02;</mo>\n"},{"bigsqcup","<mo>&#x2A06;</mo>\n"},{"bigstar","<mo>&#x2605;</mo>\n"},{"bigtriangledown","<mo>&#x25BD;</mo>\n"},{"bigtriangleup","<mo>&#x25B3;</mo>\n"},{"biguplus","<mo>&#x2A04;</mo>\n"},{"bigvee","<mo>&#x22C1;</mo>\n"},{"bigwedge","<mo>&#x22C0;</mo>\n"},{"bkarow","<mo>&#x290D;</mo>\n"},{"blacklozenge","<mo>&#x29EB;</mo>\n"},{"blacksquare","<mo>&#x25AA;</mo>\n"},{"blacktriangle","<mo>&#x25B4;</mo>\n"},{"blacktriangledown","<mo>&#x25BE;</mo>\n"},{"blacktriangleleft","<mo>&#x25C2;</mo>\n"},{"blacktriangleright","<mo>&#x25B8;</mo>\n"},{"blank","<mo>&#x2423;</mo>\n"},{"blk12","<mo>&#x2592;</mo>\n"},{"blk14","<mo>&#x2591;</mo>\n"},{"blk34","<mo>&#x2593;</mo>\n"},{"block","<mo>&#x2588;</mo>\n"},{"bne","<mo>&#x003D;</mo>\n"},{"bnequiv","<mo>&#x2261;</mo>\n"},{"bNot","<mo>&#x2AED;</mo>\n"},{"bnot","<mo>&#x2310;</mo>\n"},{"Bopf","<mo>&#xD539;</mo>\n"},{"bopf","<mo>&#xD553;</mo>\n"},{"bot","<mo>&#x22A5;</mo>\n"},{"bottom","<mo>&#x22A5;</mo>\n"},{"bowtie","<mo>&#x22C8;</mo>\n"},{"boxbox","<mo>&#x29C9;</mo>\n"},{"boxDL","<mo>&#x2557;</mo>\n"},{"boxDl","<mo>&#x2556;</mo>\n"},{"boxdL","<mo>&#x2555;</mo>\n"},{"boxdl","<mo>&#x2510;</mo>\n"},{"boxDR","<mo>&#x2554;</mo>\n"},{"boxDr","<mo>&#x2553;</mo>\n"},{"boxdR","<mo>&#x2552;</mo>\n"},{"boxdr","<mo>&#x250C;</mo>\n"},{"boxH","<mo>&#x2550;</mo>\n"},{"boxh","<mo>&#x2500;</mo>\n"},{"boxHD","<mo>&#x2566;</mo>\n"},{"boxHd","<mo>&#x2564;</mo>\n"},{"boxhD","<mo>&#x2565;</mo>\n"},{"boxhd","<mo>&#x252C;</mo>\n"},{"boxHU","<mo>&#x2569;</mo>\n"},{"boxHu","<mo>&#x2567;</mo>\n"},{"boxhU","<mo>&#x2568;</mo>\n"},{"boxhu","<mo>&#x2534;</mo>\n"},{"boxminus","<mo>&#x229F;</mo>\n"},{"boxplus","<mo>&#x229E;</mo>\n"},{"boxtimes","<mo>&#x22A0;</mo>\n"},{"boxUL","<mo>&#x255D;</mo>\n"},{"boxUl","<mo>&#x255C;</mo>\n"},{"boxuL","<mo>&#x255B;</mo>\n"},{"boxul","<mo>&#x2518;</mo>\n"},{"boxUR","<mo>&#x255A;</mo>\n"},{"boxUr","<mo>&#x2559;</mo>\n"},{"boxuR","<mo>&#x2558;</mo>\n"},{"boxur","<mo>&#x2514;</mo>\n"},{"boxV","<mo>&#x2551;</mo>\n"},{"boxv","<mo>&#x2502;</mo>\n"},{"boxVH","<mo>&#x256C;</mo>\n"},{"boxVh","<mo>&#x256B;</mo>\n"},{"boxvH","<mo>&#x256A;</mo>\n"},{"boxvh","<mo>&#x253C;</mo>\n"},{"boxVL","<mo>&#x2563;</mo>\n"},{"boxVl","<mo>&#x2562;</mo>\n"},{"boxvL","<mo>&#x2561;</mo>\n"},{"boxvl","<mo>&#x2524;</mo>\n"},{"boxVR","<mo>&#x2560;</mo>\n"},{"boxVr","<mo>&#x255F;</mo>\n"},{"boxvR","<mo>&#x255E;</mo>\n"},{"boxvr","<mo>&#x251C;</mo>\n"},{"bprime","<mo>&#x2035;</mo>\n"},{"Breve","<mo>&#x02D8;</mo>\n"},{"breve","<mo>&#x02D8;</mo>\n"},{"brvbar","<mo>&#x00A6;</mo>\n"},{"Bscr","<mo>&#x212C;</mo>\n"},{"bscr","<mo>&#xD4B7;</mo>\n"},{"bsemi","<mo>&#x204F;</mo>\n"},{"bsim","<mo>&#x223D;</mo>\n"},{"bsime","<mo>&#x22CD;</mo>\n"},{"bsol","<mo>&#x005C;</mo>\n"},{"bsolb","<mo>&#x29C5;</mo>\n"},{"bsolhsub","<mo>&#x005C;</mo>\n"},{"bull","<mo>&#x2022;</mo>\n"},{"bullet","<mo>&#x2022;</mo>\n"},{"bump","<mo>&#x224E;</mo>\n"},{"bumpE","<mo>&#x2AAE;</mo>\n"},{"bumpe","<mo>&#x224F;</mo>\n"},{"Bumpeq","<mo>&#x224E;</mo>\n"},{"bumpeq","<mo>&#x224F;</mo>\n"},{"Cacute","<mo>&#x0106;</mo>\n"},{"cacute","<mo>&#x0107;</mo>\n"},{"Cap","<mo>&#x22D2;</mo>\n"},{"cap","<mo>&#x2229;</mo>\n"},{"capand","<mo>&#x2A44;</mo>\n"},{"capbrcup","<mo>&#x2A49;</mo>\n"},{"capcap","<mo>&#x2A4B;</mo>\n"},{"capcup","<mo>&#x2A47;</mo>\n"},{"capdot","<mo>&#x2A40;</mo>\n"},{"CapitalDifferentialD","<mo>&#x2145;</mo>\n"},{"caps","<mo>&#x2229;</mo>\n"},{"caret","<mo>&#x2041;</mo>\n"},{"caron","<mo>&#x02C7;</mo>\n"},{"Cayleys","<mo>&#x212D;</mo>\n"},{"ccaps","<mo>&#x2A4D;</mo>\n"},{"Ccaron","<mo>&#x010C;</mo>\n"},{"ccaron","<mo>&#x010D;</mo>\n"},{"Ccedil","<mo>&#x00C7;</mo>\n"},{"ccedil","<mo>&#x00E7;</mo>\n"},{"Ccirc","<mo>&#x0108;</mo>\n"},{"ccirc","<mo>&#x0109;</mo>\n"},{"Cconint","<mo>&#x2230;</mo>\n"},{"ccups","<mo>&#x2A4C;</mo>\n"},{"ccupssm","<mo>&#x2A50;</mo>\n"},{"Cdot","<mo>&#x010A;</mo>\n"},{"cdot","<mo>&#x010B;</mo>\n"},{"cedil","<mo>&#x00B8;</mo>\n"},{"Cedilla","<mo>&#x00B8;</mo>\n"},{"cemptyv","<mo>&#x29B2;</mo>\n"},{"cent","<mo>&#x00A2;</mo>\n"},{"CenterDot","<mo>&#x00B7;</mo>\n"},{"centerdot","<mo>&#x00B7;</mo>\n"},{"Cfr","<mo>&#x212D;</mo>\n"},{"cfr","<mo>&#xD520;</mo>\n"},{"CHcy","<mo>&#x0427;</mo>\n"},{"chcy","<mo>&#x0447;</mo>\n"},{"check","<mo>&#x2713;</mo>\n"},{"checkmark","<mo>&#x2713;</mo>\n"},{"chi","<mo>&#x03C7;</mo>\n"},{"cir","<mo>&#x25CB;</mo>\n"},{"circ","<mo>&#x02C6;</mo>\n"},{"circeq","<mo>&#x2257;</mo>\n"},{"circlearrowleft","<mo>&#x21BA;</mo>\n"},{"circlearrowright","<mo>&#x21BB;</mo>\n"},{"circledast","<mo>&#x229B;</mo>\n"},{"circledcirc","<mo>&#x229A;</mo>\n"},{"circleddash","<mo>&#x229D;</mo>\n"},{"CircleDot","<mo>&#x2299;</mo>\n"},{"circledR","<mo>&#x00AE;</mo>\n"},{"circledS","<mo>&#x24C8;</mo>\n"},{"CircleMinus","<mo>&#x2296;</mo>\n"},{"CirclePlus","<mo>&#x2295;</mo>\n"},{"CircleTimes","<mo>&#x2297;</mo>\n"},{"cirE","<mo>&#x29C3;</mo>\n"},{"cire","<mo>&#x2257;</mo>\n"},{"cirfnint","<mo>&#x2A10;</mo>\n"},{"cirmid","<mo>&#x2AEF;</mo>\n"},{"cirscir","<mo>&#x29C2;</mo>\n"},{"ClockwiseContourIntegral","<mo>&#x2232;</mo>\n"},{"CloseCurlyDoubleQuote","<mo>&#x201D;</mo>\n"},{"CloseCurlyQuote","<mo>&#x2019;</mo>\n"},{"clubs","<mo>&#x2663;</mo>\n"},{"clubsuit","<mo>&#x2663;</mo>\n"},{"Colon","<mo>&#x2237;</mo>\n"},{"colon","<mo>&#x003A;</mo>\n"},{"Colone","<mo>&#x2A74;</mo>\n"},{"colone","<mo>&#x2254;</mo>\n"},{"coloneq","<mo>&#x2254;</mo>\n"},{"comma","<mo>&#x002C;</mo>\n"},{"commat","<mo>&#x0040;</mo>\n"},{"comp","<mo>&#x2201;</mo>\n"},{"compfn","<mo>&#x2218;</mo>\n"},{"complement","<mo>&#x2201;</mo>\n"},{"complexes","<mo>&#x2102;</mo>\n"},{"cong","<mo>&#x2245;</mo>\n"},{"congdot","<mo>&#x2A6D;</mo>\n"},{"Congruent","<mo>&#x2261;</mo>\n"},{"Conint","<mo>&#x222F;</mo>\n"},{"conint","<mo>&#x222E;</mo>\n"},{"ContourIntegral","<mo>&#x222E;</mo>\n"},{"Copf","<mo>&#x2102;</mo>\n"},{"copf","<mo>&#xD554;</mo>\n"},{"coprod","<mo>&#x2210;</mo>\n"},{"Coproduct","<mo>&#x2210;</mo>\n"},{"copy","<mo>&#x00A9;</mo>\n"},{"copysr","<mo>&#x2117;</mo>\n"},{"Cross","<mo>&#x2A2F;</mo>\n"},{"cross","<mo>&#x2717;</mo>\n"},{"Cscr","<mo>&#xD49E;</mo>\n"},{"cscr","<mo>&#xD4B8;</mo>\n"},{"csub","<mo>&#x2ACF;</mo>\n"},{"csube","<mo>&#x2AD1;</mo>\n"},{"csup","<mo>&#x2AD0;</mo>\n"},{"csupe","<mo>&#x2AD2;</mo>\n"},{"ctdot","<mo>&#x22EF;</mo>\n"},{"cudarrl","<mo>&#x2938;</mo>\n"},{"cudarrr","<mo>&#x2935;</mo>\n"},{"cuepr","<mo>&#x22DE;</mo>\n"},{"cuesc","<mo>&#x22DF;</mo>\n"},{"cularr","<mo>&#x21B6;</mo>\n"},{"cularrp","<mo>&#x293D;</mo>\n"},{"Cup","<mo>&#x22D3;</mo>\n"},{"cup","<mo>&#x222A;</mo>\n"},{"cupbrcap","<mo>&#x2A48;</mo>\n"},{"CupCap","<mo>&#x224D;</mo>\n"},{"cupcap","<mo>&#x2A46;</mo>\n"},{"cupcup","<mo>&#x2A4A;</mo>\n"},{"cupdot","<mo>&#x228D;</mo>\n"},{"cupor","<mo>&#x2A45;</mo>\n"},{"cups","<mo>&#x222A;</mo>\n"},{"curarr","<mo>&#x21B7;</mo>\n"},{"curarrm","<mo>&#x293C;</mo>\n"},{"curlyeqprec","<mo>&#x22DE;</mo>\n"},{"curlyeqsucc","<mo>&#x22DF;</mo>\n"},{"curlyvee","<mo>&#x22CE;</mo>\n"},{"curlywedge","<mo>&#x22CF;</mo>\n"},{"curren","<mo>&#x00A4;</mo>\n"},{"curvearrowleft","<mo>&#x21B6;</mo>\n"},{"curvearrowright","<mo>&#x21B7;</mo>\n"},{"cuvee","<mo>&#x22CE;</mo>\n"},{"cuwed","<mo>&#x22CF;</mo>\n"},{"cwconint","<mo>&#x2232;</mo>\n"},{"cwint","<mo>&#x2231;</mo>\n"},{"cylcty","<mo>&#x232D;</mo>\n"},{"Dagger","<mo>&#x2021;</mo>\n"},{"dagger","<mo>&#x2020;</mo>\n"},{"daleth","<mo>&#x2138;</mo>\n"},{"Darr","<mo>&#x21A1;</mo>\n"},{"dArr","<mo>&#x21D3;</mo>\n"},{"darr","<mo>&#x2193;</mo>\n"},{"dash","<mo>&#x2010;</mo>\n"},{"Dashv","<mo>&#x2AE4;</mo>\n"},{"dashv","<mo>&#x22A3;</mo>\n"},{"dbkarow","<mo>&#x290F;</mo>\n"},{"dblac","<mo>&#x02DD;</mo>\n"},{"Dcaron","<mo>&#x010E;</mo>\n"},{"dcaron","<mo>&#x010F;</mo>\n"},{"Dcy","<mo>&#x0414;</mo>\n"},{"dcy","<mo>&#x0434;</mo>\n"},{"DD","<mo>&#x2145;</mo>\n"},{"dd","<mo>&#x2146;</mo>\n"},{"ddagger","<mo>&#x2021;</mo>\n"},{"ddarr","<mo>&#x21CA;</mo>\n"},{"DDotrahd","<mo>&#x2911;</mo>\n"},{"ddotseq","<mo>&#x2A77;</mo>\n"},{"deg","<mo>&#x00B0;</mo>\n"},{"Del","<mo>&#x2207;</mo>\n"},{"Delta","<mo>&#x0394;</mo>\n"},{"delta","<mo>&#x03B4;</mo>\n"},{"demptyv","<mo>&#x29B1;</mo>\n"},{"dfisht","<mo>&#x297F;</mo>\n"},{"Dfr","<mo>&#xD507;</mo>\n"},{"dfr","<mo>&#xD521;</mo>\n"},{"dHar","<mo>&#x2965;</mo>\n"},{"dharl","<mo>&#x21C3;</mo>\n"},{"dharr","<mo>&#x21C2;</mo>\n"},{"DiacriticalAcute","<mo>&#x00B4;</mo>\n"},{"DiacriticalDot","<mo>&#x02D9;</mo>\n"},{"DiacriticalDoubleAcute","<mo>&#x02DD;</mo>\n"},{"DiacriticalGrave","<mo>&#x0060;</mo>\n"},{"DiacriticalTilde","<mo>&#x02DC;</mo>\n"},{"diam","<mo>&#x22C4;</mo>\n"},{"Diamond","<mo>&#x22C4;</mo>\n"},{"diamond","<mo>&#x22C4;</mo>\n"},{"diamondsuit","<mo>&#x2666;</mo>\n"},{"diams","<mo>&#x2666;</mo>\n"},{"die","<mo>&#x00A8;</mo>\n"},{"DifferentialD","<mo>&#x2146;</mo>\n"},{"digamma","<mo>&#x03DD;</mo>\n"},{"disin","<mo>&#x22F2;</mo>\n"},{"div","<mo>&#x00F7;</mo>\n"},{"divide","<mo>&#x00F7;</mo>\n"},{"divideontimes","<mo>&#x22C7;</mo>\n"},{"divonx","<mo>&#x22C7;</mo>\n"},{"DJcy","<mo>&#x0402;</mo>\n"},{"djcy","<mo>&#x0452;</mo>\n"},{"dlcorn","<mo>&#x231E;</mo>\n"},{"dlcrop","<mo>&#x230D;</mo>\n"},{"dollar","<mo>&#x0024;</mo>\n"},{"Dopf","<mo>&#xD53B;</mo>\n"},{"dopf","<mo>&#xD555;</mo>\n"},{"Dot","<mo>&#x00A8;</mo>\n"},{"dot","<mo>&#x02D9;</mo>\n"},{"DotDot","<mo>&#x20DC;</mo>\n"},{"doteq","<mo>&#x2250;</mo>\n"},{"doteqdot","<mo>&#x2251;</mo>\n"},{"DotEqual","<mo>&#x2250;</mo>\n"},{"dotminus","<mo>&#x2238;</mo>\n"},{"dotplus","<mo>&#x2214;</mo>\n"},{"dotsquare","<mo>&#x22A1;</mo>\n"},{"doublebarwedge","<mo>&#x2306;</mo>\n"},{"DoubleContourIntegral","<mo>&#x222F;</mo>\n"},{"DoubleDot","<mo>&#x00A8;</mo>\n"},{"DoubleDownArrow","<mo>&#x21D3;</mo>\n"},{"DoubleLeftArrow","<mo>&#x21D0;</mo>\n"},{"DoubleLeftRightArrow","<mo>&#x21D4;</mo>\n"},{"DoubleLeftTee","<mo>&#x2AE4;</mo>\n"},{"DoubleLongLeftArrow","<mo>&#x27F8;</mo>\n"},{"DoubleLongLeftRightArrow","<mo>&#x27FA;</mo>\n"},{"DoubleLongRightArrow","<mo>&#x27F9;</mo>\n"},{"DoubleRightArrow","<mo>&#x21D2;</mo>\n"},{"DoubleRightTee","<mo>&#x22A8;</mo>\n"},{"DoubleUpArrow","<mo>&#x21D1;</mo>\n"},{"DoubleUpDownArrow","<mo>&#x21D5;</mo>\n"},{"DoubleVerticalBar","<mo>&#x2225;</mo>\n"},{"DownArrow","<mo>&#x2193;</mo>\n"},{"Downarrow","<mo>&#x21D3;</mo>\n"},{"downarrow","<mo>&#x2193;</mo>\n"},{"DownArrowBar","<mo>&#x2913;</mo>\n"},{"DownArrowUpArrow","<mo>&#x21F5;</mo>\n"},{"DownBreve","<mo>&#x0311;</mo>\n"},{"downdownarrows","<mo>&#x21CA;</mo>\n"},{"downharpoonleft","<mo>&#x21C3;</mo>\n"},{"downharpoonright","<mo>&#x21C2;</mo>\n"},{"DownLeftRightVector","<mo>&#x2950;</mo>\n"},{"DownLeftTeeVector","<mo>&#x295E;</mo>\n"},{"DownLeftVector","<mo>&#x21BD;</mo>\n"},{"DownLeftVectorBar","<mo>&#x2956;</mo>\n"},{"DownRightTeeVector","<mo>&#x295F;</mo>\n"},{"DownRightVector","<mo>&#x21C1;</mo>\n"},{"DownRightVectorBar","<mo>&#x2957;</mo>\n"},{"DownTee","<mo>&#x22A4;</mo>\n"},{"DownTeeArrow","<mo>&#x21A7;</mo>\n"},{"drbkarow","<mo>&#x2910;</mo>\n"},{"drcorn","<mo>&#x231F;</mo>\n"},{"drcrop","<mo>&#x230C;</mo>\n"},{"Dscr","<mo>&#xD49F;</mo>\n"},{"dscr","<mo>&#xD4B9;</mo>\n"},{"DScy","<mo>&#x0405;</mo>\n"},{"dscy","<mo>&#x0455;</mo>\n"},{"dsol","<mo>&#x29F6;</mo>\n"},{"Dstrok","<mo>&#x0110;</mo>\n"},{"dstrok","<mo>&#x0111;</mo>\n"},{"dtdot","<mo>&#x22F1;</mo>\n"},{"dtri","<mo>&#x25BF;</mo>\n"},{"dtrif","<mo>&#x25BE;</mo>\n"},{"duarr","<mo>&#x21F5;</mo>\n"},{"duhar","<mo>&#x296F;</mo>\n"},{"dwangle","<mo>&#x29A6;</mo>\n"},{"DZcy","<mo>&#x040F;</mo>\n"},{"dzcy","<mo>&#x045F;</mo>\n"},{"dzigrarr","<mo>&#x27FF;</mo>\n"},{"Eacute","<mo>&#x00C9;</mo>\n"},{"eacute","<mo>&#x00E9;</mo>\n"},{"easter","<mo>&#x2A6E;</mo>\n"},{"Ecaron","<mo>&#x011A;</mo>\n"},{"ecaron","<mo>&#x011B;</mo>\n"},{"ecir","<mo>&#x2256;</mo>\n"},{"Ecirc","<mo>&#x00CA;</mo>\n"},{"ecirc","<mo>&#x00EA;</mo>\n"},{"ecolon","<mo>&#x2255;</mo>\n"},{"Ecy","<mo>&#x042D;</mo>\n"},{"ecy","<mo>&#x044D;</mo>\n"},{"eDDot","<mo>&#x2A77;</mo>\n"},{"Edot","<mo>&#x0116;</mo>\n"},{"eDot","<mo>&#x2251;</mo>\n"},{"edot","<mo>&#x0117;</mo>\n"},{"ee","<mo>&#x2147;</mo>\n"},{"efDot","<mo>&#x2252;</mo>\n"},{"Efr","<mo>&#xD508;</mo>\n"},{"efr","<mo>&#xD522;</mo>\n"},{"eg","<mo>&#x2A9A;</mo>\n"},{"Egrave","<mo>&#x00C8;</mo>\n"},{"egrave","<mo>&#x00E8;</mo>\n"},{"egs","<mo>&#x2A96;</mo>\n"},{"egsdot","<mo>&#x2A98;</mo>\n"},{"el","<mo>&#x2A99;</mo>\n"},{"Element","<mo>&#x2208;</mo>\n"},{"elinters","<mo>&#xFFFD;</mo>\n"},{"ell","<mo>&#x2113;</mo>\n"},{"els","<mo>&#x2A95;</mo>\n"},{"elsdot","<mo>&#x2A97;</mo>\n"},{"Emacr","<mo>&#x0112;</mo>\n"},{"emacr","<mo>&#x0113;</mo>\n"},{"empty","<mo>&#x2205;</mo>\n"},{"emptyset","<mo>&#x2205;</mo>\n"},{"EmptySmallSquare","<mo>&#x25FB;</mo>\n"},{"emptyv","<mo>&#x2205;</mo>\n"},{"EmptyVerySmallSquare","<mo>&#x25AB;</mo>\n"},{"emsp","<mo>&#x2003;</mo>\n"},{"emsp13","<mo>&#x2004;</mo>\n"},{"emsp14","<mo>&#x2005;</mo>\n"},{"ENG","<mo>&#x014A;</mo>\n"},{"eng","<mo>&#x014B;</mo>\n"},{"ensp","<mo>&#x2002;</mo>\n"},{"Eogon","<mo>&#x0118;</mo>\n"},{"eogon","<mo>&#x0119;</mo>\n"},{"Eopf","<mo>&#xD53C;</mo>\n"},{"eopf","<mo>&#xD556;</mo>\n"},{"epar","<mo>&#x22D5;</mo>\n"},{"eparsl","<mo>&#x29E3;</mo>\n"},{"eplus","<mo>&#x2A71;</mo>\n"},{"epsi","<mo>&#x03F5;</mo>\n"},{"epsiv","<mo>&#x03B5;</mo>\n"},{"eqcirc","<mo>&#x2256;</mo>\n"},{"eqcolon","<mo>&#x2255;</mo>\n"},{"eqsim","<mo>&#x2242;</mo>\n"},{"eqslantgtr","<mo>&#x2A96;</mo>\n"},{"eqslantless","<mo>&#x2A95;</mo>\n"},{"Equal","<mo>&#x2A75;</mo>\n"},{"equals","<mo>&#x003D;</mo>\n"},{"EqualTilde","<mo>&#x2242;</mo>\n"},{"equest","<mo>&#x225F;</mo>\n"},{"Equilibrium","<mo>&#x21CC;</mo>\n"},{"equiv","<mo>&#x2261;</mo>\n"},{"equivDD","<mo>&#x2A78;</mo>\n"},{"eqvparsl","<mo>&#x29E5;</mo>\n"},{"erarr","<mo>&#x2971;</mo>\n"},{"erDot","<mo>&#x2253;</mo>\n"},{"Escr","<mo>&#x2130;</mo>\n"},{"escr","<mo>&#x212F;</mo>\n"},{"esdot","<mo>&#x2250;</mo>\n"},{"Esim","<mo>&#x2A73;</mo>\n"},{"esim","<mo>&#x2242;</mo>\n"},{"eta","<mo>&#x03B7;</mo>\n"},{"ETH","<mo>&#x00D0;</mo>\n"},{"eth","<mo>&#x00F0;</mo>\n"},{"Euml","<mo>&#x00CB;</mo>\n"},{"euml","<mo>&#x00EB;</mo>\n"},{"excl","<mo>&#x0021;</mo>\n"},{"exist","<mo>&#x2203;</mo>\n"},{"Exists","<mo>&#x2203;</mo>\n"},{"expectation","<mo>&#x2130;</mo>\n"},{"ExponentialE","<mo>&#x2147;</mo>\n"},{"exponentiale","<mo>&#x2147;</mo>\n"},{"fallingdotseq","<mo>&#x2252;</mo>\n"},{"Fcy","<mo>&#x0424;</mo>\n"},{"fcy","<mo>&#x0444;</mo>\n"},{"female","<mo>&#x2640;</mo>\n"},{"ffilig","<mo>&#xFB03;</mo>\n"},{"fflig","<mo>&#xFB00;</mo>\n"},{"ffllig","<mo>&#xFB04;</mo>\n"},{"Ffr","<mo>&#xD509;</mo>\n"},{"ffr","<mo>&#xD523;</mo>\n"},{"filig","<mo>&#xFB01;</mo>\n"},{"FilledSmallSquare","<mo>&#x25FC;</mo>\n"},{"FilledVerySmallSquare","<mo>&#x25AA;</mo>\n"},{"flat","<mo>&#x266D;</mo>\n"},{"fllig","<mo>&#xFB02;</mo>\n"},{"fltns","<mo>&#x25B1;</mo>\n"},{"fnof","<mo>&#x0192;</mo>\n"},{"Fopf","<mo>&#xD53D;</mo>\n"},{"fopf","<mo>&#xD557;</mo>\n"},{"ForAll","<mo>&#x2200;</mo>\n"},{"forall","<mo>&#x2200;</mo>\n"},{"fork","<mo>&#x22D4;</mo>\n"},{"forkv","<mo>&#x2AD9;</mo>\n"},{"Fouriertrf","<mo>&#x2131;</mo>\n"},{"fpartint","<mo>&#x2A0D;</mo>\n"},{"frac12","<mo>&#x00BD;</mo>\n"},{"frac13","<mo>&#x2153;</mo>\n"},{"frac14","<mo>&#x00BC;</mo>\n"},{"frac15","<mo>&#x2155;</mo>\n"},{"frac16","<mo>&#x2159;</mo>\n"},{"frac18","<mo>&#x215B;</mo>\n"},{"frac23","<mo>&#x2154;</mo>\n"},{"frac25","<mo>&#x2156;</mo>\n"},{"frac34","<mo>&#x00BE;</mo>\n"},{"frac35","<mo>&#x2157;</mo>\n"},{"frac38","<mo>&#x215C;</mo>\n"},{"frac45","<mo>&#x2158;</mo>\n"},{"frac56","<mo>&#x215A;</mo>\n"},{"frac58","<mo>&#x215D;</mo>\n"},{"frac78","<mo>&#x215E;</mo>\n"},{"frown","<mo>&#x2322;</mo>\n"},{"Fscr","<mo>&#x2131;</mo>\n"},{"fscr","<mo>&#xD4BB;</mo>\n"},{"gacute","<mo>&#x01F5;</mo>\n"},{"Gamma","<mo>&#x0393;</mo>\n"},{"gamma","<mo>&#x03B3;</mo>\n"},{"Gammad","<mo>&#x03DC;</mo>\n"},{"gammad","<mo>&#x03DD;</mo>\n"},{"gap","<mo>&#x2A86;</mo>\n"},{"Gbreve","<mo>&#x011E;</mo>\n"},{"gbreve","<mo>&#x011F;</mo>\n"},{"Gcedil","<mo>&#x0122;</mo>\n"},{"Gcirc","<mo>&#x011C;</mo>\n"},{"gcirc","<mo>&#x011D;</mo>\n"},{"Gcy","<mo>&#x0413;</mo>\n"},{"gcy","<mo>&#x0433;</mo>\n"},{"Gdot","<mo>&#x0120;</mo>\n"},{"gdot","<mo>&#x0121;</mo>\n"},{"gE","<mo>&#x2267;</mo>\n"},{"ge","<mo>&#x2265;</mo>\n"},{"gEl","<mo>&#x2A8C;</mo>\n"},{"gel","<mo>&#x22DB;</mo>\n"},{"geq","<mo>&#x2265;</mo>\n"},{"geqq","<mo>&#x2267;</mo>\n"},{"geqslant","<mo>&#x2A7E;</mo>\n"},{"ges","<mo>&#x2A7E;</mo>\n"},{"gescc","<mo>&#x2AA9;</mo>\n"},{"gesdot","<mo>&#x2A80;</mo>\n"},{"gesdoto","<mo>&#x2A82;</mo>\n"},{"gesdotol","<mo>&#x2A84;</mo>\n"},{"gesl","<mo>&#x22DB;</mo>\n"},{"gesles","<mo>&#x2A94;</mo>\n"},{"Gfr","<mo>&#xD50A;</mo>\n"},{"gfr","<mo>&#xD524;</mo>\n"},{"Gg","<mo>&#x22D9;</mo>\n"},{"gg","<mo>&#x226B;</mo>\n"},{"ggg","<mo>&#x22D9;</mo>\n"},{"gimel","<mo>&#x2137;</mo>\n"},{"GJcy","<mo>&#x0403;</mo>\n"},{"gjcy","<mo>&#x0453;</mo>\n"},{"gl","<mo>&#x2277;</mo>\n"},{"gla","<mo>&#x2AA5;</mo>\n"},{"glE","<mo>&#x2A92;</mo>\n"},{"glj","<mo>&#x2AA4;</mo>\n"},{"gnap","<mo>&#x2A8A;</mo>\n"},{"gnapprox","<mo>&#x2A8A;</mo>\n"},{"gnE","<mo>&#x2269;</mo>\n"},{"gne","<mo>&#x2A88;</mo>\n"},{"gneq","<mo>&#x2A88;</mo>\n"},{"gneqq","<mo>&#x2269;</mo>\n"},{"gnsim","<mo>&#x22E7;</mo>\n"},{"Gopf","<mo>&#xD53E;</mo>\n"},{"gopf","<mo>&#xD558;</mo>\n"},{"grave","<mo>&#x0060;</mo>\n"},{"GreaterEqual","<mo>&#x2265;</mo>\n"},{"GreaterEqualLess","<mo>&#x22DB;</mo>\n"},{"GreaterFullEqual","<mo>&#x2267;</mo>\n"},{"GreaterGreater","<mo>&#x2AA2;</mo>\n"},{"GreaterLess","<mo>&#x2277;</mo>\n"},{"GreaterSlantEqual","<mo>&#x2A7E;</mo>\n"},{"GreaterTilde","<mo>&#x2273;</mo>\n"},{"Gscr","<mo>&#xD4A2;</mo>\n"},{"gscr","<mo>&#x210A;</mo>\n"},{"gsim","<mo>&#x2273;</mo>\n"},{"gsime","<mo>&#x2A8E;</mo>\n"},{"gsiml","<mo>&#x2A90;</mo>\n"},{"Gt","<mo>&#x226B;</mo>\n"},{"gt","<mo>&#x003E;</mo>\n"},{"gtcc","<mo>&#x2AA7;</mo>\n"},{"gtcir","<mo>&#x2A7A;</mo>\n"},{"gtdot","<mo>&#x22D7;</mo>\n"},{"gtlPar","<mo>&#x2995;</mo>\n"},{"gtquest","<mo>&#x2A7C;</mo>\n"},{"gtrapprox","<mo>&#x2A86;</mo>\n"},{"gtrarr","<mo>&#x2978;</mo>\n"},{"gtrdot","<mo>&#x22D7;</mo>\n"},{"gtreqless","<mo>&#x22DB;</mo>\n"},{"gtreqqless","<mo>&#x2A8C;</mo>\n"},{"gtrless","<mo>&#x2277;</mo>\n"},{"gtrsim","<mo>&#x2273;</mo>\n"},{"gvertneqq","<mo>&#x2269;</mo>\n"},{"gvnE","<mo>&#x2269;</mo>\n"},{"Hacek","<mo>&#x02C7;</mo>\n"},{"hairsp","<mo>&#x200A;</mo>\n"},{"half","<mo>&#x00BD;</mo>\n"},{"hamilt","<mo>&#x210B;</mo>\n"},{"HARDcy","<mo>&#x042A;</mo>\n"},{"hardcy","<mo>&#x044A;</mo>\n"},{"hArr","<mo>&#x21D4;</mo>\n"},{"harr","<mo>&#x2194;</mo>\n"},{"harrcir","<mo>&#x2948;</mo>\n"},{"harrw","<mo>&#x21AD;</mo>\n"},{"Hat","<mo>&#x005E;</mo>\n"},{"hbar","<mo>&#x210F;</mo>\n"},{"Hcirc","<mo>&#x0124;</mo>\n"},{"hcirc","<mo>&#x0125;</mo>\n"},{"hearts","<mo>&#x2665;</mo>\n"},{"heartsuit","<mo>&#x2665;</mo>\n"},{"hellip","<mo>&#x2026;</mo>\n"},{"hercon","<mo>&#x22B9;</mo>\n"},{"Hfr","<mo>&#x210C;</mo>\n"},{"hfr","<mo>&#xD525;</mo>\n"},{"HilbertSpace","<mo>&#x210B;</mo>\n"},{"hksearow","<mo>&#x2925;</mo>\n"},{"hkswarow","<mo>&#x2926;</mo>\n"},{"hoarr","<mo>&#x21FF;</mo>\n"},{"homtht","<mo>&#x223B;</mo>\n"},{"hookleftarrow","<mo>&#x21A9;</mo>\n"},{"hookrightarrow","<mo>&#x21AA;</mo>\n"},{"Hopf","<mo>&#x210D;</mo>\n"},{"hopf","<mo>&#xD559;</mo>\n"},{"horbar","<mo>&#x2015;</mo>\n"},{"HorizontalLine","<mo>&#x2500;</mo>\n"},{"Hscr","<mo>&#x210B;</mo>\n"},{"hscr","<mo>&#xD4BD;</mo>\n"},{"hslash","<mo>&#x210F;</mo>\n"},{"Hstrok","<mo>&#x0126;</mo>\n"},{"hstrok","<mo>&#x0127;</mo>\n"},{"HumpDownHump","<mo>&#x224E;</mo>\n"},{"HumpEqual","<mo>&#x224F;</mo>\n"},{"hybull","<mo>&#x2043;</mo>\n"},{"hyphen","<mo>&#x2010;</mo>\n"},{"Iacute","<mo>&#x00CD;</mo>\n"},{"iacute","<mo>&#x00ED;</mo>\n"},{"ic","<mo>&#x2063;</mo>\n"},{"Icirc","<mo>&#x00CE;</mo>\n"},{"icirc","<mo>&#x00EE;</mo>\n"},{"Icy","<mo>&#x0418;</mo>\n"},{"icy","<mo>&#x0438;</mo>\n"},{"Idot","<mo>&#x0130;</mo>\n"},{"IEcy","<mo>&#x0415;</mo>\n"},{"iecy","<mo>&#x0435;</mo>\n"},{"iexcl","<mo>&#x00A1;</mo>\n"},{"iff","<mo>&#x21D4;</mo>\n"},{"Ifr","<mo>&#x2111;</mo>\n"},{"ifr","<mo>&#xD526;</mo>\n"},{"Igrave","<mo>&#x00CC;</mo>\n"},{"igrave","<mo>&#x00EC;</mo>\n"},{"ii","<mo>&#x2148;</mo>\n"},{"iiiint","<mo>&#x2A0C;</mo>\n"},{"iiint","<mo>&#x222D;</mo>\n"},{"iinfin","<mo>&#x29DC;</mo>\n"},{"iiota","<mo>&#x2129;</mo>\n"},{"IJlig","<mo>&#x0132;</mo>\n"},{"ijlig","<mo>&#x0133;</mo>\n"},{"Im","<mo>&#x2111;</mo>\n"},{"Imacr","<mo>&#x012A;</mo>\n"},{"imacr","<mo>&#x012B;</mo>\n"},{"image","<mo>&#x2111;</mo>\n"},{"ImaginaryI","<mo>&#x2148;</mo>\n"},{"imagline","<mo>&#x2110;</mo>\n"},{"imagpart","<mo>&#x2111;</mo>\n"},{"imath","<mo>&#x0131;</mo>\n"},{"imof","<mo>&#x22B7;</mo>\n"},{"imped","<mo>&#x01B5;</mo>\n"},{"Implies","<mo>&#x21D2;</mo>\n"},{"in","<mo>&#x2208;</mo>\n"},{"incare","<mo>&#x2105;</mo>\n"},{"infin","<mo>&#x221E;</mo>\n"},{"infintie","<mo>&#x29DD;</mo>\n"},{"inodot","<mo>&#x0131;</mo>\n"},{"Int","<mo>&#x222C;</mo>\n"},{"int","<mo>&#x222B;</mo>\n"},{"intcal","<mo>&#x22BA;</mo>\n"},{"integers","<mo>&#x2124;</mo>\n"},{"Integral","<mo>&#x222B;</mo>\n"},{"intercal","<mo>&#x22BA;</mo>\n"},{"Intersection","<mo>&#x22C2;</mo>\n"},{"intlarhk","<mo>&#x2A17;</mo>\n"},{"intprod","<mo>&#x2A3C;</mo>\n"},{"InvisibleComma","<mo>&#x2063;</mo>\n"},{"InvisibleTimes","<mo>&#x2062;</mo>\n"},{"IOcy","<mo>&#x0401;</mo>\n"},{"iocy","<mo>&#x0451;</mo>\n"},{"Iogon","<mo>&#x012E;</mo>\n"},{"iogon","<mo>&#x012F;</mo>\n"},{"Iopf","<mo>&#xD540;</mo>\n"},{"iopf","<mo>&#xD55A;</mo>\n"},{"iota","<mo>&#x03B9;</mo>\n"},{"iprod","<mo>&#x2A3C;</mo>\n"},{"iquest","<mo>&#x00BF;</mo>\n"},{"Iscr","<mo>&#x2110;</mo>\n"},{"iscr","<mo>&#xD4BE;</mo>\n"},{"isin","<mo>&#x2208;</mo>\n"},{"isindot","<mo>&#x22F5;</mo>\n"},{"isinE","<mo>&#x22F9;</mo>\n"},{"isins","<mo>&#x22F4;</mo>\n"},{"isinsv","<mo>&#x22F3;</mo>\n"},{"isinv","<mo>&#x2208;</mo>\n"},{"it","<mo>&#x2062;</mo>\n"},{"Itilde","<mo>&#x0128;</mo>\n"},{"itilde","<mo>&#x0129;</mo>\n"},{"Iukcy","<mo>&#x0406;</mo>\n"},{"iukcy","<mo>&#x0456;</mo>\n"},{"Iuml","<mo>&#x00CF;</mo>\n"},{"iuml","<mo>&#x00EF;</mo>\n"},{"Jcirc","<mo>&#x0134;</mo>\n"},{"jcirc","<mo>&#x0135;</mo>\n"},{"Jcy","<mo>&#x0419;</mo>\n"},{"jcy","<mo>&#x0439;</mo>\n"},{"Jfr","<mo>&#xD50D;</mo>\n"},{"jfr","<mo>&#xD527;</mo>\n"},{"jmath","<mo>&#x006A;</mo>\n"},{"Jopf","<mo>&#xD541;</mo>\n"},{"jopf","<mo>&#xD55B;</mo>\n"},{"Jscr","<mo>&#xD4A5;</mo>\n"},{"jscr","<mo>&#xD4BF;</mo>\n"},{"Jsercy","<mo>&#x0408;</mo>\n"},{"jsercy","<mo>&#x0458;</mo>\n"},{"Jukcy","<mo>&#x0404;</mo>\n"},{"jukcy","<mo>&#x0454;</mo>\n"},{"kappa","<mo>&#x03BA;</mo>\n"},{"kappav","<mo>&#x03F0;</mo>\n"},{"Kcedil","<mo>&#x0136;</mo>\n"},{"kcedil","<mo>&#x0137;</mo>\n"},{"Kcy","<mo>&#x041A;</mo>\n"},{"kcy","<mo>&#x043A;</mo>\n"},{"Kfr","<mo>&#xD50E;</mo>\n"},{"kfr","<mo>&#xD528;</mo>\n"},{"kgreen","<mo>&#x0138;</mo>\n"},{"KHcy","<mo>&#x0425;</mo>\n"},{"khcy","<mo>&#x0445;</mo>\n"},{"KJcy","<mo>&#x040C;</mo>\n"},{"kjcy","<mo>&#x045C;</mo>\n"},{"Kopf","<mo>&#xD542;</mo>\n"},{"kopf","<mo>&#xD55C;</mo>\n"},{"Kscr","<mo>&#xD4A6;</mo>\n"},{"kscr","<mo>&#xD4C0;</mo>\n"},{"lAarr","<mo>&#x21DA;</mo>\n"},{"Lacute","<mo>&#x0139;</mo>\n"},{"lacute","<mo>&#x013A;</mo>\n"},{"laemptyv","<mo>&#x29B4;</mo>\n"},{"lagran","<mo>&#x2112;</mo>\n"},{"Lambda","<mo>&#x039B;</mo>\n"},{"lambda","<mo>&#x03BB;</mo>\n"},{"Lang","<mo>&#x300A;</mo>\n"},{"lang","<mo>&#x2329;</mo>\n"},{"langd","<mo>&#x2991;</mo>\n"},{"langle","<mo>&#x2329;</mo>\n"},{"lap","<mo>&#x2A85;</mo>\n"},{"Laplacetrf","<mo>&#x2112;</mo>\n"},{"laquo","<mo>&#x00AB;</mo>\n"},{"Larr","<mo>&#x219E;</mo>\n"},{"lArr","<mo>&#x21D0;</mo>\n"},{"larr","<mo>&#x2190;</mo>\n"},{"larrb","<mo>&#x21E4;</mo>\n"},{"larrbfs","<mo>&#x291F;</mo>\n"},{"larrfs","<mo>&#x291D;</mo>\n"},{"larrhk","<mo>&#x21A9;</mo>\n"},{"larrlp","<mo>&#x21AB;</mo>\n"},{"larrpl","<mo>&#x2939;</mo>\n"},{"larrsim","<mo>&#x2973;</mo>\n"},{"larrtl","<mo>&#x21A2;</mo>\n"},{"lat","<mo>&#x2AAB;</mo>\n"},{"lAtail","<mo>&#x291B;</mo>\n"},{"latail","<mo>&#x2919;</mo>\n"},{"late","<mo>&#x2AAD;</mo>\n"},{"lates","<mo>&#x2AAD;</mo>\n"},{"lBarr","<mo>&#x290E;</mo>\n"},{"lbarr","<mo>&#x290C;</mo>\n"},{"lbbrk","<mo>&#x3014;</mo>\n"},{"lbrace","<mo>&#x007B;</mo>\n"},{"lbrack","<mo>&#x005B;</mo>\n"},{"lbrke","<mo>&#x298B;</mo>\n"},{"lbrksld","<mo>&#x298F;</mo>\n"},{"lbrkslu","<mo>&#x298D;</mo>\n"},{"Lcaron","<mo>&#x013D;</mo>\n"},{"lcaron","<mo>&#x013E;</mo>\n"},{"Lcedil","<mo>&#x013B;</mo>\n"},{"lcedil","<mo>&#x013C;</mo>\n"},{"lceil","<mo>&#x2308;</mo>\n"},{"lcub","<mo>&#x007B;</mo>\n"},{"Lcy","<mo>&#x041B;</mo>\n"},{"lcy","<mo>&#x043B;</mo>\n"},{"ldca","<mo>&#x2936;</mo>\n"},{"ldquo","<mo>&#x201C;</mo>\n"},{"ldquor","<mo>&#x201E;</mo>\n"},{"ldrdhar","<mo>&#x2967;</mo>\n"},{"ldrushar","<mo>&#x294B;</mo>\n"},{"ldsh","<mo>&#x21B2;</mo>\n"},{"lE","<mo>&#x2266;</mo>\n"},{"le","<mo>&#x2264;</mo>\n"},{"LeftAngleBracket","<mo>&#x2329;</mo>\n"},{"LeftArrow","<mo>&#x2190;</mo>\n"},{"Leftarrow","<mo>&#x21D0;</mo>\n"},{"leftarrow","<mo>&#x2190;</mo>\n"},{"LeftArrowBar","<mo>&#x21E4;</mo>\n"},{"LeftArrowRightArrow","<mo>&#x21C6;</mo>\n"},{"leftarrowtail","<mo>&#x21A2;</mo>\n"},{"LeftCeiling","<mo>&#x2308;</mo>\n"},{"LeftDoubleBracket","<mo>&#x301A;</mo>\n"},{"LeftDownTeeVector","<mo>&#x2961;</mo>\n"},{"LeftDownVector","<mo>&#x21C3;</mo>\n"},{"LeftDownVectorBar","<mo>&#x2959;</mo>\n"},{"LeftFloor","<mo>&#x230A;</mo>\n"},{"leftharpoondown","<mo>&#x21BD;</mo>\n"},{"leftharpoonup","<mo>&#x21BC;</mo>\n"},{"leftleftarrows","<mo>&#x21C7;</mo>\n"},{"LeftRightArrow","<mo>&#x2194;</mo>\n"},{"Leftrightarrow","<mo>&#x21D4;</mo>\n"},{"leftrightarrow","<mo>&#x2194;</mo>\n"},{"leftrightarrows","<mo>&#x21C6;</mo>\n"},{"leftrightharpoons","<mo>&#x21CB;</mo>\n"},{"leftrightsquigarrow","<mo>&#x21AD;</mo>\n"},{"LeftRightVector","<mo>&#x294E;</mo>\n"},{"LeftTee","<mo>&#x22A3;</mo>\n"},{"LeftTeeArrow","<mo>&#x21A4;</mo>\n"},{"LeftTeeVector","<mo>&#x295A;</mo>\n"},{"leftthreetimes","<mo>&#x22CB;</mo>\n"},{"LeftTriangle","<mo>&#x22B2;</mo>\n"},{"LeftTriangleBar","<mo>&#x29CF;</mo>\n"},{"LeftTriangleEqual","<mo>&#x22B4;</mo>\n"},{"LeftUpDownVector","<mo>&#x2951;</mo>\n"},{"LeftUpTeeVector","<mo>&#x2960;</mo>\n"},{"LeftUpVector","<mo>&#x21BF;</mo>\n"},{"LeftUpVectorBar","<mo>&#x2958;</mo>\n"},{"LeftVector","<mo>&#x21BC;</mo>\n"},{"LeftVectorBar","<mo>&#x2952;</mo>\n"},{"lEg","<mo>&#x2A8B;</mo>\n"},{"leg","<mo>&#x22DA;</mo>\n"},{"leq","<mo>&#x2264;</mo>\n"},{"leqq","<mo>&#x2266;</mo>\n"},{"leqslant","<mo>&#x2A7D;</mo>\n"},{"les","<mo>&#x2A7D;</mo>\n"},{"lescc","<mo>&#x2AA8;</mo>\n"},{"lesdot","<mo>&#x2A7F;</mo>\n"},{"lesdoto","<mo>&#x2A81;</mo>\n"},{"lesdotor","<mo>&#x2A83;</mo>\n"},{"lesg","<mo>&#x22DA;</mo>\n"},{"lesges","<mo>&#x2A93;</mo>\n"},{"lessapprox","<mo>&#x2A85;</mo>\n"},{"lessdot","<mo>&#x22D6;</mo>\n"},{"lesseqgtr","<mo>&#x22DA;</mo>\n"},{"lesseqqgtr","<mo>&#x2A8B;</mo>\n"},{"LessEqualGreater","<mo>&#x22DA;</mo>\n"},{"LessFullEqual","<mo>&#x2266;</mo>\n"},{"LessGreater","<mo>&#x2276;</mo>\n"},{"lessgtr","<mo>&#x2276;</mo>\n"},{"LessLess","<mo>&#x2AA1;</mo>\n"},{"lesssim","<mo>&#x2272;</mo>\n"},{"LessSlantEqual","<mo>&#x2A7D;</mo>\n"},{"LessTilde","<mo>&#x2272;</mo>\n"},{"lfisht","<mo>&#x297C;</mo>\n"},{"lfloor","<mo>&#x230A;</mo>\n"},{"Lfr","<mo>&#xD50F;</mo>\n"},{"lfr","<mo>&#xD529;</mo>\n"},{"lg","<mo>&#x2276;</mo>\n"},{"lgE","<mo>&#x2A91;</mo>\n"},{"lHar","<mo>&#x2962;</mo>\n"},{"lhard","<mo>&#x21BD;</mo>\n"},{"lharu","<mo>&#x21BC;</mo>\n"},{"lharul","<mo>&#x296A;</mo>\n"},{"lhblk","<mo>&#x2584;</mo>\n"},{"LJcy","<mo>&#x0409;</mo>\n"},{"ljcy","<mo>&#x0459;</mo>\n"},{"Ll","<mo>&#x22D8;</mo>\n"},{"ll","<mo>&#x226A;</mo>\n"},{"llarr","<mo>&#x21C7;</mo>\n"},{"llcorner","<mo>&#x231E;</mo>\n"},{"Lleftarrow","<mo>&#x21DA;</mo>\n"},{"llhard","<mo>&#x296B;</mo>\n"},{"lltri","<mo>&#x25FA;</mo>\n"},{"Lmidot","<mo>&#x013F;</mo>\n"},{"lmidot","<mo>&#x0140;</mo>\n"},{"lmoust","<mo>&#x23B0;</mo>\n"},{"lmoustache","<mo>&#x23B0;</mo>\n"},{"lnap","<mo>&#x2A89;</mo>\n"},{"lnapprox","<mo>&#x2A89;</mo>\n"},{"lnE","<mo>&#x2268;</mo>\n"},{"lne","<mo>&#x2A87;</mo>\n"},{"lneq","<mo>&#x2A87;</mo>\n"},{"lneqq","<mo>&#x2268;</mo>\n"},{"lnsim","<mo>&#x22E6;</mo>\n"},{"loang","<mo>&#x3018;</mo>\n"},{"loarr","<mo>&#x21FD;</mo>\n"},{"lobrk","<mo>&#x301A;</mo>\n"},{"LongLeftArrow","<mo>&#x27F5;</mo>\n"},{"Longleftarrow","<mo>&#x27F8;</mo>\n"},{"longleftarrow","<mo>&#x27F5;</mo>\n"},{"LongLeftRightArrow","<mo>&#x27F7;</mo>\n"},{"Longleftrightarrow","<mo>&#x27FA;</mo>\n"},{"longleftrightarrow","<mo>&#x27F7;</mo>\n"},{"longmapsto","<mo>&#x27FC;</mo>\n"},{"LongRightArrow","<mo>&#x27F6;</mo>\n"},{"Longrightarrow","<mo>&#x27F9;</mo>\n"},{"longrightarrow","<mo>&#x27F6;</mo>\n"},{"looparrowleft","<mo>&#x21AB;</mo>\n"},{"looparrowright","<mo>&#x21AC;</mo>\n"},{"lopar","<mo>&#x2985;</mo>\n"},{"Lopf","<mo>&#xD543;</mo>\n"},{"lopf","<mo>&#xD55D;</mo>\n"},{"loplus","<mo>&#x2A2D;</mo>\n"},{"lotimes","<mo>&#x2A34;</mo>\n"},{"lowast","<mo>&#x2217;</mo>\n"},{"lowbar","<mo>&#x005F;</mo>\n"},{"LowerLeftArrow","<mo>&#x2199;</mo>\n"},{"LowerRightArrow","<mo>&#x2198;</mo>\n"},{"loz","<mo>&#x25CA;</mo>\n"},{"lozenge","<mo>&#x25CA;</mo>\n"},{"lozf","<mo>&#x29EB;</mo>\n"},{"lpar","<mo>&#x0028;</mo>\n"},{"lparlt","<mo>&#x2993;</mo>\n"},{"lrarr","<mo>&#x21C6;</mo>\n"},{"lrcorner","<mo>&#x231F;</mo>\n"},{"lrhar","<mo>&#x21CB;</mo>\n"},{"lrhard","<mo>&#x296D;</mo>\n"},{"lrtri","<mo>&#x22BF;</mo>\n"},{"Lscr","<mo>&#x2112;</mo>\n"},{"lscr","<mo>&#xD4C1;</mo>\n"},{"Lsh","<mo>&#x21B0;</mo>\n"},{"lsh","<mo>&#x21B0;</mo>\n"},{"lsim","<mo>&#x2272;</mo>\n"},{"lsime","<mo>&#x2A8D;</mo>\n"},{"lsimg","<mo>&#x2A8F;</mo>\n"},{"lsqb","<mo>&#x005B;</mo>\n"},{"lsquo","<mo>&#x2018;</mo>\n"},{"lsquor","<mo>&#x201A;</mo>\n"},{"Lstrok","<mo>&#x0141;</mo>\n"},{"lstrok","<mo>&#x0142;</mo>\n"},{"Lt","<mo>&#x226A;</mo>\n"},{"lt","<mo>&#x003C;</mo>\n"},{"ltcc","<mo>&#x2AA6;</mo>\n"},{"ltcir","<mo>&#x2A79;</mo>\n"},{"ltdot","<mo>&#x22D6;</mo>\n"},{"lthree","<mo>&#x22CB;</mo>\n"},{"ltimes","<mo>&#x22C9;</mo>\n"},{"ltlarr","<mo>&#x2976;</mo>\n"},{"ltquest","<mo>&#x2A7B;</mo>\n"},{"ltri","<mo>&#x25C3;</mo>\n"},{"ltrie","<mo>&#x22B4;</mo>\n"},{"ltrif","<mo>&#x25C2;</mo>\n"},{"ltrPar","<mo>&#x2996;</mo>\n"},{"lurdshar","<mo>&#x294A;</mo>\n"},{"luruhar","<mo>&#x2966;</mo>\n"},{"lvertneqq","<mo>&#x2268;</mo>\n"},{"lvnE","<mo>&#x2268;</mo>\n"},{"macr","<mo>&#x00AF;</mo>\n"},{"male","<mo>&#x2642;</mo>\n"},{"malt","<mo>&#x2720;</mo>\n"},{"maltese","<mo>&#x2720;</mo>\n"},{"Map","<mo>&#x2905;</mo>\n"},{"map","<mo>&#x21A6;</mo>\n"},{"mapsto","<mo>&#x21A6;</mo>\n"},{"mapstodown","<mo>&#x21A7;</mo>\n"},{"mapstoleft","<mo>&#x21A4;</mo>\n"},{"mapstoup","<mo>&#x21A5;</mo>\n"},{"marker","<mo>&#x25AE;</mo>\n"},{"mcomma","<mo>&#x2A29;</mo>\n"},{"Mcy","<mo>&#x041C;</mo>\n"},{"mcy","<mo>&#x043C;</mo>\n"},{"mdash","<mo>&#x2014;</mo>\n"},{"mDDot","<mo>&#x223A;</mo>\n"},{"measuredangle","<mo>&#x2221;</mo>\n"},{"MediumSpace","<mo>&#x205F;</mo>\n"},{"Mellintrf","<mo>&#x2133;</mo>\n"},{"Mfr","<mo>&#xD510;</mo>\n"},{"mfr","<mo>&#xD52A;</mo>\n"},{"mho","<mo>&#x2127;</mo>\n"},{"micro","<mo>&#x00B5;</mo>\n"},{"mid","<mo>&#x2223;</mo>\n"},{"midast","<mo>&#x002A;</mo>\n"},{"midcir","<mo>&#x2AF0;</mo>\n"},{"middot","<mo>&#x00B7;</mo>\n"},{"minus","<mo>&#x2212;</mo>\n"},{"minusb","<mo>&#x229F;</mo>\n"},{"minusd","<mo>&#x2238;</mo>\n"},{"minusdu","<mo>&#x2A2A;</mo>\n"},{"MinusPlus","<mo>&#x2213;</mo>\n"},{"mlcp","<mo>&#x2ADB;</mo>\n"},{"mldr","<mo>&#x2026;</mo>\n"},{"mnplus","<mo>&#x2213;</mo>\n"},{"models","<mo>&#x22A7;</mo>\n"},{"Mopf","<mo>&#xD544;</mo>\n"},{"mopf","<mo>&#xD55E;</mo>\n"},{"mp","<mo>&#x2213;</mo>\n"},{"Mscr","<mo>&#x2133;</mo>\n"},{"mscr","<mo>&#xD4C2;</mo>\n"},{"mstpos","<mo>&#x223E;</mo>\n"},{"mu","<mo>&#x03BC;</mo>\n"},{"multimap","<mo>&#x22B8;</mo>\n"},{"mumap","<mo>&#x22B8;</mo>\n"},{"nabla","<mo>&#x2207;</mo>\n"},{"Nacute","<mo>&#x0143;</mo>\n"},{"nacute","<mo>&#x0144;</mo>\n"},{"nang","<mo>&#x2220;</mo>\n"},{"nap","<mo>&#x2249;</mo>\n"},{"napE","<mo>&#x2A70;</mo>\n"},{"napid","<mo>&#x224B;</mo>\n"},{"napos","<mo>&#x0149;</mo>\n"},{"napprox","<mo>&#x2249;</mo>\n"},{"natur","<mo>&#x266E;</mo>\n"},{"natural","<mo>&#x266E;</mo>\n"},{"naturals","<mo>&#x2115;</mo>\n"},{"nbsp","<mo>&#x00A0;</mo>\n"},{"nbump","<mo>&#x224E;</mo>\n"},{"nbumpe","<mo>&#x224F;</mo>\n"},{"ncap","<mo>&#x2A43;</mo>\n"},{"Ncaron","<mo>&#x0147;</mo>\n"},{"ncaron","<mo>&#x0148;</mo>\n"},{"Ncedil","<mo>&#x0145;</mo>\n"},{"ncedil","<mo>&#x0146;</mo>\n"},{"ncong","<mo>&#x2247;</mo>\n"},{"ncongdot","<mo>&#x2A6D;</mo>\n"},{"ncup","<mo>&#x2A42;</mo>\n"},{"Ncy","<mo>&#x041D;</mo>\n"},{"ncy","<mo>&#x043D;</mo>\n"},{"ndash","<mo>&#x2013;</mo>\n"},{"ne","<mo>&#x2260;</mo>\n"},{"nearhk","<mo>&#x2924;</mo>\n"},{"neArr","<mo>&#x21D7;</mo>\n"},{"nearr","<mo>&#x2197;</mo>\n"},{"nearrow","<mo>&#x2197;</mo>\n"},{"nedot","<mo>&#x2250;</mo>\n"},{"NegativeMediumSpace","<mo>&#x200B;</mo>\n"},{"NegativeThickSpace","<mo>&#x200B;</mo>\n"},{"NegativeThinSpace","<mo>&#x200B;</mo>\n"},{"NegativeVeryThinSpace","<mo>&#x200B;</mo>\n"},{"nequiv","<mo>&#x2262;</mo>\n"},{"nesear","<mo>&#x2928;</mo>\n"},{"nesim","<mo>&#x2242;</mo>\n"},{"NestedGreaterGreater","<mo>&#x226B;</mo>\n"},{"NestedLessLess","<mo>&#x226A;</mo>\n"},{"NewLine","<mo>&#x000A;</mo>\n"},{"nexist","<mo>&#x2204;</mo>\n"},{"nexists","<mo>&#x2204;</mo>\n"},{"Nfr","<mo>&#xD511;</mo>\n"},{"nfr","<mo>&#xD52B;</mo>\n"},{"ngE","<mo>&#x2267;</mo>\n"},{"nge","<mo>&#x2271;</mo>\n"},{"ngeq","<mo>&#x2271;</mo>\n"},{"ngeqq","<mo>&#x2267;</mo>\n"},{"ngeqslant","<mo>&#x2A7E;</mo>\n"},{"nges","<mo>&#x2A7E;</mo>\n"},{"nGg","<mo>&#x22D9;</mo>\n"},{"ngsim","<mo>&#x2275;</mo>\n"},{"nGt","<mo>&#x226B;</mo>\n"},{"ngt","<mo>&#x226F;</mo>\n"},{"ngtr","<mo>&#x226F;</mo>\n"},{"nGtv","<mo>&#x226B;</mo>\n"},{"nhArr","<mo>&#x21CE;</mo>\n"},{"nharr","<mo>&#x21AE;</mo>\n"},{"nhpar","<mo>&#x2AF2;</mo>\n"},{"ni","<mo>&#x220B;</mo>\n"},{"nis","<mo>&#x22FC;</mo>\n"},{"nisd","<mo>&#x22FA;</mo>\n"},{"niv","<mo>&#x220B;</mo>\n"},{"NJcy","<mo>&#x040A;</mo>\n"},{"njcy","<mo>&#x045A;</mo>\n"},{"nlArr","<mo>&#x21CD;</mo>\n"},{"nlarr","<mo>&#x219A;</mo>\n"},{"nldr","<mo>&#x2025;</mo>\n"},{"nlE","<mo>&#x2266;</mo>\n"},{"nle","<mo>&#x2270;</mo>\n"},{"nLeftarrow","<mo>&#x21CD;</mo>\n"},{"nleftarrow","<mo>&#x219A;</mo>\n"},{"nLeftrightarrow","<mo>&#x21CE;</mo>\n"},{"nleftrightarrow","<mo>&#x21AE;</mo>\n"},{"nleq","<mo>&#x2270;</mo>\n"},{"nleqq","<mo>&#x2266;</mo>\n"},{"nleqslant","<mo>&#x2A7D;</mo>\n"},{"nles","<mo>&#x2A7D;</mo>\n"},{"nless","<mo>&#x226E;</mo>\n"},{"nLl","<mo>&#x22D8;</mo>\n"},{"nlsim","<mo>&#x2274;</mo>\n"},{"nLt","<mo>&#x226A;</mo>\n"},{"nlt","<mo>&#x226E;</mo>\n"},{"nltri","<mo>&#x22EA;</mo>\n"},{"nltrie","<mo>&#x22EC;</mo>\n"},{"nLtv","<mo>&#x226A;</mo>\n"},{"nmid","<mo>&#x2224;</mo>\n"},{"NoBreak","<mo>&#x2060;</mo>\n"},{"NonBreakingSpace","<mo>&#x00A0;</mo>\n"},{"Nopf","<mo>&#x2115;</mo>\n"},{"nopf","<mo>&#xD55F;</mo>\n"},{"Not","<mo>&#x2AEC;</mo>\n"},{"not","<mo>&#x00AC;</mo>\n"},{"NotCongruent","<mo>&#x2262;</mo>\n"},{"NotCupCap","<mo>&#x226D;</mo>\n"},{"NotDoubleVerticalBar","<mo>&#x2226;</mo>\n"},{"NotElement","<mo>&#x2209;</mo>\n"},{"NotEqual","<mo>&#x2260;</mo>\n"},{"NotEqualTilde","<mo>&#x2242;</mo>\n"},{"NotExists","<mo>&#x2204;</mo>\n"},{"NotGreater","<mo>&#x226F;</mo>\n"},{"NotGreaterEqual","<mo>&#x2271;</mo>\n"},{"NotGreaterFullEqual","<mo>&#x2266;</mo>\n"},{"NotGreaterGreater","<mo>&#x226B;</mo>\n"},{"NotGreaterLess","<mo>&#x2279;</mo>\n"},{"NotGreaterSlantEqual","<mo>&#x2A7E;</mo>\n"},{"NotGreaterTilde","<mo>&#x2275;</mo>\n"},{"NotHumpDownHump","<mo>&#x224E;</mo>\n"},{"NotHumpEqual","<mo>&#x224F;</mo>\n"},{"notin","<mo>&#x2209;</mo>\n"},{"notindot","<mo>&#x22F5;</mo>\n"},{"notinE","<mo>&#x22F9;</mo>\n"},{"notinva","<mo>&#x2209;</mo>\n"},{"notinvb","<mo>&#x22F7;</mo>\n"},{"notinvc","<mo>&#x22F6;</mo>\n"},{"NotLeftTriangle","<mo>&#x22EA;</mo>\n"},{"NotLeftTriangleBar","<mo>&#x29CF;</mo>\n"},{"NotLeftTriangleEqual","<mo>&#x22EC;</mo>\n"},{"NotLess","<mo>&#x226E;</mo>\n"},{"NotLessEqual","<mo>&#x2270;</mo>\n"},{"NotLessGreater","<mo>&#x2278;</mo>\n"},{"NotLessLess","<mo>&#x226A;</mo>\n"},{"NotLessSlantEqual","<mo>&#x2A7D;</mo>\n"},{"NotLessTilde","<mo>&#x2274;</mo>\n"},{"NotNestedGreaterGreater","<mo>&#x2AA2;</mo>\n"},{"NotNestedLessLess","<mo>&#x2AA1;</mo>\n"},{"notni","<mo>&#x220C;</mo>\n"},{"notniva","<mo>&#x220C;</mo>\n"},{"notnivb","<mo>&#x22FE;</mo>\n"},{"notnivc","<mo>&#x22FD;</mo>\n"},{"NotPrecedes","<mo>&#x2280;</mo>\n"},{"NotPrecedesEqual","<mo>&#x2AAF;</mo>\n"},{"NotPrecedesSlantEqual","<mo>&#x22E0;</mo>\n"},{"NotReverseElement","<mo>&#x220C;</mo>\n"},{"NotRightTriangle","<mo>&#x22EB;</mo>\n"},{"NotRightTriangleBar","<mo>&#x29D0;</mo>\n"},{"NotRightTriangleEqual","<mo>&#x22ED;</mo>\n"},{"NotSquareSubset","<mo>&#x228F;</mo>\n"},{"NotSquareSubsetEqual","<mo>&#x22E2;</mo>\n"},{"NotSquareSuperset","<mo>&#x2290;</mo>\n"},{"NotSquareSupersetEqual","<mo>&#x22E3;</mo>\n"},{"NotSubset","<mo>&#x2282;</mo>\n"},{"NotSubsetEqual","<mo>&#x2288;</mo>\n"},{"NotSucceeds","<mo>&#x2281;</mo>\n"},{"NotSucceedsEqual","<mo>&#x2AB0;</mo>\n"},{"NotSucceedsSlantEqual","<mo>&#x22E1;</mo>\n"},{"NotSucceedsTilde","<mo>&#x227F;</mo>\n"},{"NotSuperset","<mo>&#x2283;</mo>\n"},{"NotSupersetEqual","<mo>&#x2289;</mo>\n"},{"NotTilde","<mo>&#x2241;</mo>\n"},{"NotTildeEqual","<mo>&#x2244;</mo>\n"},{"NotTildeFullEqual","<mo>&#x2247;</mo>\n"},{"NotTildeTilde","<mo>&#x2249;</mo>\n"},{"NotVerticalBar","<mo>&#x2224;</mo>\n"},{"npar","<mo>&#x2226;</mo>\n"},{"nparallel","<mo>&#x2226;</mo>\n"},{"nparsl","<mo>&#x2AFD;</mo>\n"},{"npart","<mo>&#x2202;</mo>\n"},{"npolint","<mo>&#x2A14;</mo>\n"},{"npr","<mo>&#x2280;</mo>\n"},{"nprcue","<mo>&#x22E0;</mo>\n"},{"npre","<mo>&#x2AAF;</mo>\n"},{"nprec","<mo>&#x2280;</mo>\n"},{"npreceq","<mo>&#x2AAF;</mo>\n"},{"nrArr","<mo>&#x21CF;</mo>\n"},{"nrarr","<mo>&#x219B;</mo>\n"},{"nrarrc","<mo>&#x2933;</mo>\n"},{"nrarrw","<mo>&#x219D;</mo>\n"},{"nRightarrow","<mo>&#x21CF;</mo>\n"},{"nrightarrow","<mo>&#x219B;</mo>\n"},{"nrtri","<mo>&#x22EB;</mo>\n"},{"nrtrie","<mo>&#x22ED;</mo>\n"},{"nsc","<mo>&#x2281;</mo>\n"},{"nsccue","<mo>&#x22E1;</mo>\n"},{"nsce","<mo>&#x2AB0;</mo>\n"},{"Nscr","<mo>&#xD4A9;</mo>\n"},{"nscr","<mo>&#xD4C3;</mo>\n"},{"nshortmid","<mo>&#x2224;</mo>\n"},{"nshortparallel","<mo>&#x2226;</mo>\n"},{"nsim","<mo>&#x2241;</mo>\n"},{"nsime","<mo>&#x2244;</mo>\n"},{"nsimeq","<mo>&#x2244;</mo>\n"},{"nsmid","<mo>&#x2224;</mo>\n"},{"nspar","<mo>&#x2226;</mo>\n"},{"nsqsube","<mo>&#x22E2;</mo>\n"},{"nsqsupe","<mo>&#x22E3;</mo>\n"},{"nsub","<mo>&#x2284;</mo>\n"},{"nsubE","<mo>&#x2AC5;</mo>\n"},{"nsube","<mo>&#x2288;</mo>\n"},{"nsubset","<mo>&#x2282;</mo>\n"},{"nsubseteq","<mo>&#x2288;</mo>\n"},{"nsubseteqq","<mo>&#x2AC5;</mo>\n"},{"nsucc","<mo>&#x2281;</mo>\n"},{"nsucceq","<mo>&#x2AB0;</mo>\n"},{"nsup","<mo>&#x2285;</mo>\n"},{"nsupE","<mo>&#x2AC6;</mo>\n"},{"nsupe","<mo>&#x2289;</mo>\n"},{"nsupset","<mo>&#x2283;</mo>\n"},{"nsupseteq","<mo>&#x2289;</mo>\n"},{"nsupseteqq","<mo>&#x2AC6;</mo>\n"},{"ntgl","<mo>&#x2279;</mo>\n"},{"Ntilde","<mo>&#x00D1;</mo>\n"},{"ntilde","<mo>&#x00F1;</mo>\n"},{"ntlg","<mo>&#x2278;</mo>\n"},{"ntriangleleft","<mo>&#x22EA;</mo>\n"},{"ntrianglelefteq","<mo>&#x22EC;</mo>\n"},{"ntriangleright","<mo>&#x22EB;</mo>\n"},{"ntrianglerighteq","<mo>&#x22ED;</mo>\n"},{"nu","<mo>&#x03BD;</mo>\n"},{"num","<mo>&#x0023;</mo>\n"},{"numero","<mo>&#x2116;</mo>\n"},{"numsp","<mo>&#x2007;</mo>\n"},{"nvap","<mo>&#x224D;</mo>\n"},{"nVDash","<mo>&#x22AF;</mo>\n"},{"nVdash","<mo>&#x22AE;</mo>\n"},{"nvDash","<mo>&#x22AD;</mo>\n"},{"nvdash","<mo>&#x22AC;</mo>\n"},{"nvge","<mo>&#x2265;</mo>\n"},{"nvgt","<mo>&#x003E;</mo>\n"},{"nvHarr","<mo>&#x2904;</mo>\n"},{"nvinfin","<mo>&#x29DE;</mo>\n"},{"nvlArr","<mo>&#x2902;</mo>\n"},{"nvle","<mo>&#x2264;</mo>\n"},{"nvlt","<mo>&#x003C;</mo>\n"},{"nvltrie","<mo>&#x22B4;</mo>\n"},{"nvrArr","<mo>&#x2903;</mo>\n"},{"nvrtrie","<mo>&#x22B5;</mo>\n"},{"nvsim","<mo>&#x223C;</mo>\n"},{"nwarhk","<mo>&#x2923;</mo>\n"},{"nwArr","<mo>&#x21D6;</mo>\n"},{"nwarr","<mo>&#x2196;</mo>\n"},{"nwarrow","<mo>&#x2196;</mo>\n"},{"nwnear","<mo>&#x2927;</mo>\n"},{"Oacute","<mo>&#x00D3;</mo>\n"},{"oacute","<mo>&#x00F3;</mo>\n"},{"oast","<mo>&#x229B;</mo>\n"},{"ocir","<mo>&#x229A;</mo>\n"},{"Ocirc","<mo>&#x00D4;</mo>\n"},{"ocirc","<mo>&#x00F4;</mo>\n"},{"Ocy","<mo>&#x041E;</mo>\n"},{"ocy","<mo>&#x043E;</mo>\n"},{"odash","<mo>&#x229D;</mo>\n"},{"Odblac","<mo>&#x0150;</mo>\n"},{"odblac","<mo>&#x0151;</mo>\n"},{"odiv","<mo>&#x2A38;</mo>\n"},{"odot","<mo>&#x2299;</mo>\n"},{"odsold","<mo>&#x29BC;</mo>\n"},{"OElig","<mo>&#x0152;</mo>\n"},{"oelig","<mo>&#x0153;</mo>\n"},{"ofcir","<mo>&#x29BF;</mo>\n"},{"Ofr","<mo>&#xD512;</mo>\n"},{"ofr","<mo>&#xD52C;</mo>\n"},{"ogon","<mo>&#x02DB;</mo>\n"},{"Ograve","<mo>&#x00D2;</mo>\n"},{"ograve","<mo>&#x00F2;</mo>\n"},{"ogt","<mo>&#x29C1;</mo>\n"},{"ohbar","<mo>&#x29B5;</mo>\n"},{"ohm","<mo>&#x2126;</mo>\n"},{"oint","<mo>&#x222E;</mo>\n"},{"olarr","<mo>&#x21BA;</mo>\n"},{"olcir","<mo>&#x29BE;</mo>\n"},{"olcross","<mo>&#x29BB;</mo>\n"},{"olt","<mo>&#x29C0;</mo>\n"},{"Omacr","<mo>&#x014C;</mo>\n"},{"omacr","<mo>&#x014D;</mo>\n"},{"Omega","<mo>&#x03A9;</mo>\n"},{"omega","<mo>&#x03C9;</mo>\n"},{"omid","<mo>&#x29B6;</mo>\n"},{"ominus","<mo>&#x2296;</mo>\n"},{"Oopf","<mo>&#xD546;</mo>\n"},{"oopf","<mo>&#xD560;</mo>\n"},{"opar","<mo>&#x29B7;</mo>\n"},{"OpenCurlyDoubleQuote","<mo>&#x201C;</mo>\n"},{"OpenCurlyQuote","<mo>&#x2018;</mo>\n"},{"operp","<mo>&#x29B9;</mo>\n"},{"oplus","<mo>&#x2295;</mo>\n"},{"Or","<mo>&#x2A54;</mo>\n"},{"or","<mo>&#x2228;</mo>\n"},{"orarr","<mo>&#x21BB;</mo>\n"},{"ord","<mo>&#x2A5D;</mo>\n"},{"order","<mo>&#x2134;</mo>\n"},{"orderof","<mo>&#x2134;</mo>\n"},{"ordf","<mo>&#x00AA;</mo>\n"},{"ordm","<mo>&#x00BA;</mo>\n"},{"origof","<mo>&#x22B6;</mo>\n"},{"oror","<mo>&#x2A56;</mo>\n"},{"orslope","<mo>&#x2A57;</mo>\n"},{"orv","<mo>&#x2A5B;</mo>\n"},{"oS","<mo>&#x24C8;</mo>\n"},{"Oscr","<mo>&#xD4AA;</mo>\n"},{"oscr","<mo>&#x2134;</mo>\n"},{"Oslash","<mo>&#x00D8;</mo>\n"},{"oslash","<mo>&#x00F8;</mo>\n"},{"osol","<mo>&#x2298;</mo>\n"},{"Otilde","<mo>&#x00D5;</mo>\n"},{"otilde","<mo>&#x00F5;</mo>\n"},{"Otimes","<mo>&#x2A37;</mo>\n"},{"otimes","<mo>&#x2297;</mo>\n"},{"otimesas","<mo>&#x2A36;</mo>\n"},{"Ouml","<mo>&#x00D6;</mo>\n"},{"ouml","<mo>&#x00F6;</mo>\n"},{"ovbar","<mo>&#x233D;</mo>\n"},{"OverBar","<mo>&#x00AF;</mo>\n"},{"OverBrace","<mo>&#xFE37;</mo>\n"},{"OverBracket","<mo>&#x23B4;</mo>\n"},{"OverParenthesis","<mo>&#xFE35;</mo>\n"},{"par","<mo>&#x2225;</mo>\n"},{"para","<mo>&#x00B6;</mo>\n"},{"parallel","<mo>&#x2225;</mo>\n"},{"parsim","<mo>&#x2AF3;</mo>\n"},{"parsl","<mo>&#x2AFD;</mo>\n"},{"part","<mo>&#x2202;</mo>\n"},{"PartialD","<mo>&#x2202;</mo>\n"},{"Pcy","<mo>&#x041F;</mo>\n"},{"pcy","<mo>&#x043F;</mo>\n"},{"percnt","<mo>&#x0025;</mo>\n"},{"period","<mo>&#x002E;</mo>\n"},{"permil","<mo>&#x2030;</mo>\n"},{"perp","<mo>&#x22A5;</mo>\n"},{"pertenk","<mo>&#x2031;</mo>\n"},{"Pfr","<mo>&#xD513;</mo>\n"},{"pfr","<mo>&#xD52D;</mo>\n"},{"Phi","<mo>&#x03A6;</mo>\n"},{"phi","<mo>&#x03D5;</mo>\n"},{"phiv","<mo>&#x03C6;</mo>\n"},{"phmmat","<mo>&#x2133;</mo>\n"},{"phone","<mo>&#x260E;</mo>\n"},{"Pi","<mo>&#x03A0;</mo>\n"},{"pi","<mo>&#x03C0;</mo>\n"},{"pitchfork","<mo>&#x22D4;</mo>\n"},{"piv","<mo>&#x03D6;</mo>\n"},{"planck","<mo>&#x210F;</mo>\n"},{"planckh","<mo>&#x210E;</mo>\n"},{"plankv","<mo>&#x210F;</mo>\n"},{"plus","<mo>&#x002B;</mo>\n"},{"plusacir","<mo>&#x2A23;</mo>\n"},{"plusb","<mo>&#x229E;</mo>\n"},{"pluscir","<mo>&#x2A22;</mo>\n"},{"plusdo","<mo>&#x2214;</mo>\n"},{"plusdu","<mo>&#x2A25;</mo>\n"},{"pluse","<mo>&#x2A72;</mo>\n"},{"PlusMinus","<mo>&#x00B1;</mo>\n"},{"plusmn","<mo>&#x00B1;</mo>\n"},{"plussim","<mo>&#x2A26;</mo>\n"},{"plustwo","<mo>&#x2A27;</mo>\n"},{"pm","<mo>&#x00B1;</mo>\n"},{"Poincareplane","<mo>&#x210C;</mo>\n"},{"pointint","<mo>&#x2A15;</mo>\n"},{"Popf","<mo>&#x2119;</mo>\n"},{"popf","<mo>&#xD561;</mo>\n"},{"pound","<mo>&#x00A3;</mo>\n"},{"Pr","<mo>&#x2ABB;</mo>\n"},{"pr","<mo>&#x227A;</mo>\n"},{"prap","<mo>&#x2AB7;</mo>\n"},{"prcue","<mo>&#x227C;</mo>\n"},{"prE","<mo>&#x2AB3;</mo>\n"},{"pre","<mo>&#x2AAF;</mo>\n"},{"prec","<mo>&#x227A;</mo>\n"},{"precapprox","<mo>&#x2AB7;</mo>\n"},{"preccurlyeq","<mo>&#x227C;</mo>\n"},{"Precedes","<mo>&#x227A;</mo>\n"},{"PrecedesEqual","<mo>&#x2AAF;</mo>\n"},{"PrecedesSlantEqual","<mo>&#x227C;</mo>\n"},{"PrecedesTilde","<mo>&#x227E;</mo>\n"},{"preceq","<mo>&#x2AAF;</mo>\n"},{"precnapprox","<mo>&#x2AB9;</mo>\n"},{"precneqq","<mo>&#x2AB5;</mo>\n"},{"precnsim","<mo>&#x22E8;</mo>\n"},{"precsim","<mo>&#x227E;</mo>\n"},{"Prime","<mo>&#x2033;</mo>\n"},{"prime","<mo>&#x2032;</mo>\n"},{"primes","<mo>&#x2119;</mo>\n"},{"prnap","<mo>&#x2AB9;</mo>\n"},{"prnE","<mo>&#x2AB5;</mo>\n"},{"prnsim","<mo>&#x22E8;</mo>\n"},{"prod","<mo>&#x220F;</mo>\n"},{"Product","<mo>&#x220F;</mo>\n"},{"profalar","<mo>&#x232E;</mo>\n"},{"profline","<mo>&#x2312;</mo>\n"},{"profsurf","<mo>&#x2313;</mo>\n"},{"prop","<mo>&#x221D;</mo>\n"},{"Proportion","<mo>&#x2237;</mo>\n"},{"Proportional","<mo>&#x221D;</mo>\n"},{"propto","<mo>&#x221D;</mo>\n"},{"prsim","<mo>&#x227E;</mo>\n"},{"prurel","<mo>&#x22B0;</mo>\n"},{"Pscr","<mo>&#xD4AB;</mo>\n"},{"pscr","<mo>&#xD4C5;</mo>\n"},{"Psi","<mo>&#x03A8;</mo>\n"},{"psi","<mo>&#x03C8;</mo>\n"},{"puncsp","<mo>&#x2008;</mo>\n"},{"Qfr","<mo>&#xD514;</mo>\n"},{"qfr","<mo>&#xD52E;</mo>\n"},{"qint","<mo>&#x2A0C;</mo>\n"},{"Qopf","<mo>&#x211A;</mo>\n"},{"qopf","<mo>&#xD562;</mo>\n"},{"qprime","<mo>&#x2057;</mo>\n"},{"Qscr","<mo>&#xD4AC;</mo>\n"},{"qscr","<mo>&#xD4C6;</mo>\n"},{"quaternions","<mo>&#x210D;</mo>\n"},{"quatint","<mo>&#x2A16;</mo>\n"},{"quest","<mo>&#x003F;</mo>\n"},{"questeq","<mo>&#x225F;</mo>\n"},{"quot","<mo>&#x0022;</mo>\n"},{"rAarr","<mo>&#x21DB;</mo>\n"},{"race","<mo>&#x29DA;</mo>\n"},{"Racute","<mo>&#x0154;</mo>\n"},{"racute","<mo>&#x0155;</mo>\n"},{"radic","<mo>&#x221A;</mo>\n"},{"raemptyv","<mo>&#x29B3;</mo>\n"},{"Rang","<mo>&#x300B;</mo>\n"},{"rang","<mo>&#x232A;</mo>\n"},{"rangd","<mo>&#x2992;</mo>\n"},{"range","<mo>&#x29A5;</mo>\n"},{"rangle","<mo>&#x232A;</mo>\n"},{"raquo","<mo>&#x00BB;</mo>\n"},{"Rarr","<mo>&#x21A0;</mo>\n"},{"rArr","<mo>&#x21D2;</mo>\n"},{"rarr","<mo>&#x2192;</mo>\n"},{"rarrap","<mo>&#x2975;</mo>\n"},{"rarrb","<mo>&#x21E5;</mo>\n"},{"rarrbfs","<mo>&#x2920;</mo>\n"},{"rarrc","<mo>&#x2933;</mo>\n"},{"rarrfs","<mo>&#x291E;</mo>\n"},{"rarrhk","<mo>&#x21AA;</mo>\n"},{"rarrlp","<mo>&#x21AC;</mo>\n"},{"rarrpl","<mo>&#x2945;</mo>\n"},{"rarrsim","<mo>&#x2974;</mo>\n"},{"Rarrtl","<mo>&#x2916;</mo>\n"},{"rarrtl","<mo>&#x21A3;</mo>\n"},{"rarrw","<mo>&#x219D;</mo>\n"},{"rAtail","<mo>&#x291C;</mo>\n"},{"ratail","<mo>&#x291A;</mo>\n"},{"ratio","<mo>&#x2236;</mo>\n"},{"rationals","<mo>&#x211A;</mo>\n"},{"RBarr","<mo>&#x2910;</mo>\n"},{"rBarr","<mo>&#x290F;</mo>\n"},{"rbarr","<mo>&#x290D;</mo>\n"},{"rbbrk","<mo>&#x3015;</mo>\n"},{"rbrace","<mo>&#x007D;</mo>\n"},{"rbrack","<mo>&#x005D;</mo>\n"},{"rbrke","<mo>&#x298C;</mo>\n"},{"rbrksld","<mo>&#x298E;</mo>\n"},{"rbrkslu","<mo>&#x2990;</mo>\n"},{"Rcaron","<mo>&#x0158;</mo>\n"},{"rcaron","<mo>&#x0159;</mo>\n"},{"Rcedil","<mo>&#x0156;</mo>\n"},{"rcedil","<mo>&#x0157;</mo>\n"},{"rceil","<mo>&#x2309;</mo>\n"},{"rcub","<mo>&#x007D;</mo>\n"},{"Rcy","<mo>&#x0420;</mo>\n"},{"rcy","<mo>&#x0440;</mo>\n"},{"rdca","<mo>&#x2937;</mo>\n"},{"rdldhar","<mo>&#x2969;</mo>\n"},{"rdquo","<mo>&#x201D;</mo>\n"},{"rdquor","<mo>&#x201D;</mo>\n"},{"rdsh","<mo>&#x21B3;</mo>\n"},{"Re","<mo>&#x211C;</mo>\n"},{"real","<mo>&#x211C;</mo>\n"},{"realine","<mo>&#x211B;</mo>\n"},{"realpart","<mo>&#x211C;</mo>\n"},{"reals","<mo>&#x211D;</mo>\n"},{"rect","<mo>&#x25AD;</mo>\n"},{"reg","<mo>&#x00AE;</mo>\n"},{"ReverseElement","<mo>&#x220B;</mo>\n"},{"ReverseEquilibrium","<mo>&#x21CB;</mo>\n"},{"ReverseUpEquilibrium","<mo>&#x296F;</mo>\n"},{"rfisht","<mo>&#x297D;</mo>\n"},{"rfloor","<mo>&#x230B;</mo>\n"},{"Rfr","<mo>&#x211C;</mo>\n"},{"rfr","<mo>&#xD52F;</mo>\n"},{"rHar","<mo>&#x2964;</mo>\n"},{"rhard","<mo>&#x21C1;</mo>\n"},{"rharu","<mo>&#x21C0;</mo>\n"},{"rharul","<mo>&#x296C;</mo>\n"},{"rho","<mo>&#x03C1;</mo>\n"},{"rhov","<mo>&#x03F1;</mo>\n"},{"RightAngleBracket","<mo>&#x232A;</mo>\n"},{"RightArrow","<mo>&#x2192;</mo>\n"},{"Rightarrow","<mo>&#x21D2;</mo>\n"},{"rightarrow","<mo>&#x2192;</mo>\n"},{"RightArrowBar","<mo>&#x21E5;</mo>\n"},{"RightArrowLeftArrow","<mo>&#x21C4;</mo>\n"},{"rightarrowtail","<mo>&#x21A3;</mo>\n"},{"RightCeiling","<mo>&#x2309;</mo>\n"},{"RightDoubleBracket","<mo>&#x301B;</mo>\n"},{"RightDownTeeVector","<mo>&#x295D;</mo>\n"},{"RightDownVector","<mo>&#x21C2;</mo>\n"},{"RightDownVectorBar","<mo>&#x2955;</mo>\n"},{"RightFloor","<mo>&#x230B;</mo>\n"},{"rightharpoondown","<mo>&#x21C1;</mo>\n"},{"rightharpoonup","<mo>&#x21C0;</mo>\n"},{"rightleftarrows","<mo>&#x21C4;</mo>\n"},{"rightleftharpoons","<mo>&#x21CC;</mo>\n"},{"rightrightarrows","<mo>&#x21C9;</mo>\n"},{"rightsquigarrow","<mo>&#x219D;</mo>\n"},{"RightTee","<mo>&#x22A2;</mo>\n"},{"RightTeeArrow","<mo>&#x21A6;</mo>\n"},{"RightTeeVector","<mo>&#x295B;</mo>\n"},{"rightthreetimes","<mo>&#x22CC;</mo>\n"},{"RightTriangle","<mo>&#x22B3;</mo>\n"},{"RightTriangleBar","<mo>&#x29D0;</mo>\n"},{"RightTriangleEqual","<mo>&#x22B5;</mo>\n"},{"RightUpDownVector","<mo>&#x294F;</mo>\n"},{"RightUpTeeVector","<mo>&#x295C;</mo>\n"},{"RightUpVector","<mo>&#x21BE;</mo>\n"},{"RightUpVectorBar","<mo>&#x2954;</mo>\n"},{"RightVector","<mo>&#x21C0;</mo>\n"},{"RightVectorBar","<mo>&#x2953;</mo>\n"},{"ring","<mo>&#x02DA;</mo>\n"},{"risingdotseq","<mo>&#x2253;</mo>\n"},{"rlarr","<mo>&#x21C4;</mo>\n"},{"rlhar","<mo>&#x21CC;</mo>\n"},{"rmoust","<mo>&#x23B1;</mo>\n"},{"rmoustache","<mo>&#x23B1;</mo>\n"},{"rnmid","<mo>&#x2AEE;</mo>\n"},{"roang","<mo>&#x3019;</mo>\n"},{"roarr","<mo>&#x21FE;</mo>\n"},{"robrk","<mo>&#x301B;</mo>\n"},{"ropar","<mo>&#x2986;</mo>\n"},{"Ropf","<mo>&#x211D;</mo>\n"},{"ropf","<mo>&#xD563;</mo>\n"},{"roplus","<mo>&#x2A2E;</mo>\n"},{"rotimes","<mo>&#x2A35;</mo>\n"},{"RoundImplies","<mo>&#x2970;</mo>\n"},{"rpar","<mo>&#x0029;</mo>\n"},{"rpargt","<mo>&#x2994;</mo>\n"},{"rppolint","<mo>&#x2A12;</mo>\n"},{"rrarr","<mo>&#x21C9;</mo>\n"},{"Rrightarrow","<mo>&#x21DB;</mo>\n"},{"Rscr","<mo>&#x211B;</mo>\n"},{"rscr","<mo>&#xD4C7;</mo>\n"},{"Rsh","<mo>&#x21B1;</mo>\n"},{"rsh","<mo>&#x21B1;</mo>\n"},{"rsqb","<mo>&#x005D;</mo>\n"},{"rsquo","<mo>&#x2019;</mo>\n"},{"rsquor","<mo>&#x2019;</mo>\n"},{"rthree","<mo>&#x22CC;</mo>\n"},{"rtimes","<mo>&#x22CA;</mo>\n"},{"rtri","<mo>&#x25B9;</mo>\n"},{"rtrie","<mo>&#x22B5;</mo>\n"},{"rtrif","<mo>&#x25B8;</mo>\n"},{"rtriltri","<mo>&#x29CE;</mo>\n"},{"RuleDelayed","<mo>&#x29F4;</mo>\n"},{"ruluhar","<mo>&#x2968;</mo>\n"},{"rx","<mo>&#x211E;</mo>\n"},{"Sacute","<mo>&#x015A;</mo>\n"},{"sacute","<mo>&#x015B;</mo>\n"},{"Sc","<mo>&#x2ABC;</mo>\n"},{"sc","<mo>&#x227B;</mo>\n"},{"scap","<mo>&#x2AB8;</mo>\n"},{"Scaron","<mo>&#x0160;</mo>\n"},{"scaron","<mo>&#x0161;</mo>\n"},{"sccue","<mo>&#x227D;</mo>\n"},{"scE","<mo>&#x2AB4;</mo>\n"},{"sce","<mo>&#x2AB0;</mo>\n"},{"Scedil","<mo>&#x015E;</mo>\n"},{"scedil","<mo>&#x015F;</mo>\n"},{"Scirc","<mo>&#x015C;</mo>\n"},{"scirc","<mo>&#x015D;</mo>\n"},{"scnap","<mo>&#x2ABA;</mo>\n"},{"scnE","<mo>&#x2AB6;</mo>\n"},{"scnsim","<mo>&#x22E9;</mo>\n"},{"scpolint","<mo>&#x2A13;</mo>\n"},{"scsim","<mo>&#x227F;</mo>\n"},{"Scy","<mo>&#x0421;</mo>\n"},{"scy","<mo>&#x0441;</mo>\n"},{"sdot","<mo>&#x22C5;</mo>\n"},{"sdotb","<mo>&#x22A1;</mo>\n"},{"sdote","<mo>&#x2A66;</mo>\n"},{"searhk","<mo>&#x2925;</mo>\n"},{"seArr","<mo>&#x21D8;</mo>\n"},{"searr","<mo>&#x2198;</mo>\n"},{"searrow","<mo>&#x2198;</mo>\n"},{"sect","<mo>&#x00A7;</mo>\n"},{"semi","<mo>&#x003B;</mo>\n"},{"seswar","<mo>&#x2929;</mo>\n"},{"setminus","<mo>&#x2216;</mo>\n"},{"setmn","<mo>&#x2216;</mo>\n"},{"sext","<mo>&#x2736;</mo>\n"},{"Sfr","<mo>&#xD516;</mo>\n"},{"sfr","<mo>&#xD530;</mo>\n"},{"sfrown","<mo>&#x2322;</mo>\n"},{"sharp","<mo>&#x266F;</mo>\n"},{"SHCHcy","<mo>&#x0429;</mo>\n"},{"shchcy","<mo>&#x0449;</mo>\n"},{"SHcy","<mo>&#x0428;</mo>\n"},{"shcy","<mo>&#x0448;</mo>\n"},{"ShortDownArrow","<mo>&#x2193;</mo>\n"},{"ShortLeftArrow","<mo>&#x2190;</mo>\n"},{"shortmid","<mo>&#x2223;</mo>\n"},{"shortparallel","<mo>&#x2225;</mo>\n"},{"ShortRightArrow","<mo>&#x2192;</mo>\n"},{"ShortUpArrow","<mo>&#x2191;</mo>\n"},{"shy","<mo>&#x00AD;</mo>\n"},{"Sigma","<mo>&#x03A3;</mo>\n"},{"sigma","<mo>&#x03C3;</mo>\n"},{"sigmav","<mo>&#x03C2;</mo>\n"},{"sim","<mo>&#x223C;</mo>\n"},{"simdot","<mo>&#x2A6A;</mo>\n"},{"sime","<mo>&#x2243;</mo>\n"},{"simeq","<mo>&#x2243;</mo>\n"},{"simg","<mo>&#x2A9E;</mo>\n"},{"simgE","<mo>&#x2AA0;</mo>\n"},{"siml","<mo>&#x2A9D;</mo>\n"},{"simlE","<mo>&#x2A9F;</mo>\n"},{"simne","<mo>&#x2246;</mo>\n"},{"simplus","<mo>&#x2A24;</mo>\n"},{"simrarr","<mo>&#x2972;</mo>\n"},{"slarr","<mo>&#x2190;</mo>\n"},{"SmallCircle","<mo>&#x2218;</mo>\n"},{"smallsetminus","<mo>&#x2216;</mo>\n"},{"smashp","<mo>&#x2A33;</mo>\n"},{"smeparsl","<mo>&#x29E4;</mo>\n"},{"smid","<mo>&#x2223;</mo>\n"},{"smile","<mo>&#x2323;</mo>\n"},{"smt","<mo>&#x2AAA;</mo>\n"},{"smte","<mo>&#x2AAC;</mo>\n"},{"smtes","<mo>&#x2AAC;</mo>\n"},{"SOFTcy","<mo>&#x042C;</mo>\n"},{"softcy","<mo>&#x044C;</mo>\n"},{"sol","<mo>&#x002F;</mo>\n"},{"solb","<mo>&#x29C4;</mo>\n"},{"solbar","<mo>&#x233F;</mo>\n"},{"Sopf","<mo>&#xD54A;</mo>\n"},{"sopf","<mo>&#xD564;</mo>\n"},{"spades","<mo>&#x2660;</mo>\n"},{"spadesuit","<mo>&#x2660;</mo>\n"},{"spar","<mo>&#x2225;</mo>\n"},{"sqcap","<mo>&#x2293;</mo>\n"},{"sqcaps","<mo>&#x2293;</mo>\n"},{"sqcup","<mo>&#x2294;</mo>\n"},{"sqcups","<mo>&#x2294;</mo>\n"},{"Sqrt","<mo>&#x221A;</mo>\n"},{"sqsub","<mo>&#x228F;</mo>\n"},{"sqsube","<mo>&#x2291;</mo>\n"},{"sqsubset","<mo>&#x228F;</mo>\n"},{"sqsubseteq","<mo>&#x2291;</mo>\n"},{"sqsup","<mo>&#x2290;</mo>\n"},{"sqsupe","<mo>&#x2292;</mo>\n"},{"sqsupset","<mo>&#x2290;</mo>\n"},{"sqsupseteq","<mo>&#x2292;</mo>\n"},{"squ","<mo>&#x25A1;</mo>\n"},{"Square","<mo>&#x25A1;</mo>\n"},{"square","<mo>&#x25A1;</mo>\n"},{"SquareIntersection","<mo>&#x2293;</mo>\n"},{"SquareSubset","<mo>&#x228F;</mo>\n"},{"SquareSubsetEqual","<mo>&#x2291;</mo>\n"},{"SquareSuperset","<mo>&#x2290;</mo>\n"},{"SquareSupersetEqual","<mo>&#x2292;</mo>\n"},{"SquareUnion","<mo>&#x2294;</mo>\n"},{"squarf","<mo>&#x25AA;</mo>\n"},{"squf","<mo>&#x25AA;</mo>\n"},{"srarr","<mo>&#x2192;</mo>\n"},{"Sscr","<mo>&#xD4AE;</mo>\n"},{"sscr","<mo>&#xD4C8;</mo>\n"},{"ssetmn","<mo>&#x2216;</mo>\n"},{"ssmile","<mo>&#x2323;</mo>\n"},{"sstarf","<mo>&#x22C6;</mo>\n"},{"Star","<mo>&#x22C6;</mo>\n"},{"star","<mo>&#x2606;</mo>\n"},{"starf","<mo>&#x2605;</mo>\n"},{"straightepsilon","<mo>&#x03F5;</mo>\n"},{"straightphi","<mo>&#x03D5;</mo>\n"},{"strns","<mo>&#x00AF;</mo>\n"},{"Sub","<mo>&#x22D0;</mo>\n"},{"sub","<mo>&#x2282;</mo>\n"},{"subdot","<mo>&#x2ABD;</mo>\n"},{"subE","<mo>&#x2AC5;</mo>\n"},{"sube","<mo>&#x2286;</mo>\n"},{"subedot","<mo>&#x2AC3;</mo>\n"},{"submult","<mo>&#x2AC1;</mo>\n"},{"subnE","<mo>&#x2ACB;</mo>\n"},{"subne","<mo>&#x228A;</mo>\n"},{"subplus","<mo>&#x2ABF;</mo>\n"},{"subrarr","<mo>&#x2979;</mo>\n"},{"Subset","<mo>&#x22D0;</mo>\n"},{"subset","<mo>&#x2282;</mo>\n"},{"subseteq","<mo>&#x2286;</mo>\n"},{"subseteqq","<mo>&#x2AC5;</mo>\n"},{"SubsetEqual","<mo>&#x2286;</mo>\n"},{"subsetneq","<mo>&#x228A;</mo>\n"},{"subsetneqq","<mo>&#x2ACB;</mo>\n"},{"subsim","<mo>&#x2AC7;</mo>\n"},{"subsub","<mo>&#x2AD5;</mo>\n"},{"subsup","<mo>&#x2AD3;</mo>\n"},{"succ","<mo>&#x227B;</mo>\n"},{"succapprox","<mo>&#x2AB8;</mo>\n"},{"succcurlyeq","<mo>&#x227D;</mo>\n"},{"Succeeds","<mo>&#x227B;</mo>\n"},{"SucceedsEqual","<mo>&#x2AB0;</mo>\n"},{"SucceedsSlantEqual","<mo>&#x227D;</mo>\n"},{"SucceedsTilde","<mo>&#x227F;</mo>\n"},{"succeq","<mo>&#x2AB0;</mo>\n"},{"succnapprox","<mo>&#x2ABA;</mo>\n"},{"succneqq","<mo>&#x2AB6;</mo>\n"},{"succnsim","<mo>&#x22E9;</mo>\n"},{"succsim","<mo>&#x227F;</mo>\n"},{"SuchThat","<mo>&#x220B;</mo>\n"},{"Sum","<mo>&#x2211;</mo>\n"},{"sum","<mo>&#x2211;</mo>\n"},{"sung","<mo>&#x266A;</mo>\n"},{"Sup","<mo>&#x22D1;</mo>\n"},{"sup","<mo>&#x2283;</mo>\n"},{"sup1","<mo>&#x00B9;</mo>\n"},{"sup2","<mo>&#x00B2;</mo>\n"},{"sup3","<mo>&#x00B3;</mo>\n"},{"supdot","<mo>&#x2ABE;</mo>\n"},{"supdsub","<mo>&#x2AD8;</mo>\n"},{"supE","<mo>&#x2AC6;</mo>\n"},{"supe","<mo>&#x2287;</mo>\n"},{"supedot","<mo>&#x2AC4;</mo>\n"},{"Superset","<mo>&#x2283;</mo>\n"},{"SupersetEqual","<mo>&#x2287;</mo>\n"},{"suphsol","<mo>&#x2283;</mo>\n"},{"suphsub","<mo>&#x2AD7;</mo>\n"},{"suplarr","<mo>&#x297B;</mo>\n"},{"supmult","<mo>&#x2AC2;</mo>\n"},{"supnE","<mo>&#x2ACC;</mo>\n"},{"supne","<mo>&#x228B;</mo>\n"},{"supplus","<mo>&#x2AC0;</mo>\n"},{"Supset","<mo>&#x22D1;</mo>\n"},{"supset","<mo>&#x2283;</mo>\n"},{"supseteq","<mo>&#x2287;</mo>\n"},{"supseteqq","<mo>&#x2AC6;</mo>\n"},{"supsetneq","<mo>&#x228B;</mo>\n"},{"supsetneqq","<mo>&#x2ACC;</mo>\n"},{"supsim","<mo>&#x2AC8;</mo>\n"},{"supsub","<mo>&#x2AD4;</mo>\n"},{"supsup","<mo>&#x2AD6;</mo>\n"},{"swarhk","<mo>&#x2926;</mo>\n"},{"swArr","<mo>&#x21D9;</mo>\n"},{"swarr","<mo>&#x2199;</mo>\n"},{"swarrow","<mo>&#x2199;</mo>\n"},{"swnwar","<mo>&#x292A;</mo>\n"},{"szlig","<mo>&#x00DF;</mo>\n"},{"Tab","<mo>&#x0009;</mo>\n"},{"target","<mo>&#x2316;</mo>\n"},{"tau","<mo>&#x03C4;</mo>\n"},{"tbrk","<mo>&#x23B4;</mo>\n"},{"Tcaron","<mo>&#x0164;</mo>\n"},{"tcaron","<mo>&#x0165;</mo>\n"},{"Tcedil","<mo>&#x0162;</mo>\n"},{"tcedil","<mo>&#x0163;</mo>\n"},{"Tcy","<mo>&#x0422;</mo>\n"},{"tcy","<mo>&#x0442;</mo>\n"},{"tdot","<mo>&#x20DB;</mo>\n"},{"telrec","<mo>&#x2315;</mo>\n"},{"Tfr","<mo>&#xD517;</mo>\n"},{"tfr","<mo>&#xD531;</mo>\n"},{"there4","<mo>&#x2234;</mo>\n"},{"Therefore","<mo>&#x2234;</mo>\n"},{"therefore","<mo>&#x2234;</mo>\n"},{"Theta","<mo>&#x0398;</mo>\n"},{"theta","<mo>&#x03B8;</mo>\n"},{"thetav","<mo>&#x03D1;</mo>\n"},{"thickapprox","<mo>&#x2248;</mo>\n"},{"thicksim","<mo>&#x223C;</mo>\n"},{"ThickSpace","<mo>&#x2009;</mo>\n"},{"thinsp","<mo>&#x2009;</mo>\n"},{"ThinSpace","<mo>&#x2009;</mo>\n"},{"thkap","<mo>&#x2248;</mo>\n"},{"thksim","<mo>&#x223C;</mo>\n"},{"THORN","<mo>&#x00DE;</mo>\n"},{"thorn","<mo>&#x00FE;</mo>\n"},{"Tilde","<mo>&#x223C;</mo>\n"},{"tilde","<mo>&#x02DC;</mo>\n"},{"TildeEqual","<mo>&#x2243;</mo>\n"},{"TildeFullEqual","<mo>&#x2245;</mo>\n"},{"TildeTilde","<mo>&#x2248;</mo>\n"},{"times","<mo>&#x00D7;</mo>\n"},{"timesb","<mo>&#x22A0;</mo>\n"},{"timesbar","<mo>&#x2A31;</mo>\n"},{"timesd","<mo>&#x2A30;</mo>\n"},{"tint","<mo>&#x222D;</mo>\n"},{"toea","<mo>&#x2928;</mo>\n"},{"top","<mo>&#x22A4;</mo>\n"},{"topbot","<mo>&#x2336;</mo>\n"},{"topcir","<mo>&#x2AF1;</mo>\n"},{"Topf","<mo>&#xD54B;</mo>\n"},{"topf","<mo>&#xD565;</mo>\n"},{"topfork","<mo>&#x2ADA;</mo>\n"},{"tosa","<mo>&#x2929;</mo>\n"},{"tprime","<mo>&#x2034;</mo>\n"},{"trade","<mo>&#x2122;</mo>\n"},{"triangle","<mo>&#x25B5;</mo>\n"},{"triangledown","<mo>&#x25BF;</mo>\n"},{"triangleleft","<mo>&#x25C3;</mo>\n"},{"trianglelefteq","<mo>&#x22B4;</mo>\n"},{"triangleq","<mo>&#x225C;</mo>\n"},{"triangleright","<mo>&#x25B9;</mo>\n"},{"trianglerighteq","<mo>&#x22B5;</mo>\n"},{"tridot","<mo>&#x25EC;</mo>\n"},{"trie","<mo>&#x225C;</mo>\n"},{"triminus","<mo>&#x2A3A;</mo>\n"},{"TripleDot","<mo>&#x20DB;</mo>\n"},{"triplus","<mo>&#x2A39;</mo>\n"},{"trisb","<mo>&#x29CD;</mo>\n"},{"tritime","<mo>&#x2A3B;</mo>\n"},{"trpezium","<mo>&#xFFFD;</mo>\n"},{"Tscr","<mo>&#xD4AF;</mo>\n"},{"tscr","<mo>&#xD4C9;</mo>\n"},{"TScy","<mo>&#x0426;</mo>\n"},{"tscy","<mo>&#x0446;</mo>\n"},{"TSHcy","<mo>&#x040B;</mo>\n"},{"tshcy","<mo>&#x045B;</mo>\n"},{"Tstrok","<mo>&#x0166;</mo>\n"},{"tstrok","<mo>&#x0167;</mo>\n"},{"twixt","<mo>&#x226C;</mo>\n"},{"twoheadleftarrow","<mo>&#x219E;</mo>\n"},{"twoheadrightarrow","<mo>&#x21A0;</mo>\n"},{"Uacute","<mo>&#x00DA;</mo>\n"},{"uacute","<mo>&#x00FA;</mo>\n"},{"Uarr","<mo>&#x219F;</mo>\n"},{"uArr","<mo>&#x21D1;</mo>\n"},{"uarr","<mo>&#x2191;</mo>\n"},{"Uarrocir","<mo>&#x2949;</mo>\n"},{"Ubrcy","<mo>&#x040E;</mo>\n"},{"ubrcy","<mo>&#x045E;</mo>\n"},{"Ubreve","<mo>&#x016C;</mo>\n"},{"ubreve","<mo>&#x016D;</mo>\n"},{"Ucirc","<mo>&#x00DB;</mo>\n"},{"ucirc","<mo>&#x00FB;</mo>\n"},{"Ucy","<mo>&#x0423;</mo>\n"},{"ucy","<mo>&#x0443;</mo>\n"},{"udarr","<mo>&#x21C5;</mo>\n"},{"Udblac","<mo>&#x0170;</mo>\n"},{"udblac","<mo>&#x0171;</mo>\n"},{"udhar","<mo>&#x296E;</mo>\n"},{"ufisht","<mo>&#x297E;</mo>\n"},{"Ufr","<mo>&#xD518;</mo>\n"},{"ufr","<mo>&#xD532;</mo>\n"},{"Ugrave","<mo>&#x00D9;</mo>\n"},{"ugrave","<mo>&#x00F9;</mo>\n"},{"uHar","<mo>&#x2963;</mo>\n"},{"uharl","<mo>&#x21BF;</mo>\n"},{"uharr","<mo>&#x21BE;</mo>\n"},{"uhblk","<mo>&#x2580;</mo>\n"},{"ulcorn","<mo>&#x231C;</mo>\n"},{"ulcorner","<mo>&#x231C;</mo>\n"},{"ulcrop","<mo>&#x230F;</mo>\n"},{"ultri","<mo>&#x25F8;</mo>\n"},{"Umacr","<mo>&#x016A;</mo>\n"},{"umacr","<mo>&#x016B;</mo>\n"},{"uml","<mo>&#x00A8;</mo>\n"},{"UnderBar","<mo>&#x0332;</mo>\n"},{"UnderBrace","<mo>&#xFE38;</mo>\n"},{"UnderBracket","<mo>&#x23B5;</mo>\n"},{"UnderParenthesis","<mo>&#xFE36;</mo>\n"},{"Union","<mo>&#x22C3;</mo>\n"},{"UnionPlus","<mo>&#x228E;</mo>\n"},{"Uogon","<mo>&#x0172;</mo>\n"},{"uogon","<mo>&#x0173;</mo>\n"},{"Uopf","<mo>&#xD54C;</mo>\n"},{"uopf","<mo>&#xD566;</mo>\n"},{"UpArrow","<mo>&#x2191;</mo>\n"},{"Uparrow","<mo>&#x21D1;</mo>\n"},{"uparrow","<mo>&#x2191;</mo>\n"},{"UpArrowBar","<mo>&#x2912;</mo>\n"},{"UpArrowDownArrow","<mo>&#x21C5;</mo>\n"},{"UpDownArrow","<mo>&#x2195;</mo>\n"},{"Updownarrow","<mo>&#x21D5;</mo>\n"},{"updownarrow","<mo>&#x2195;</mo>\n"},{"UpEquilibrium","<mo>&#x296E;</mo>\n"},{"upharpoonleft","<mo>&#x21BF;</mo>\n"},{"upharpoonright","<mo>&#x21BE;</mo>\n"},{"uplus","<mo>&#x228E;</mo>\n"},{"UpperLeftArrow","<mo>&#x2196;</mo>\n"},{"UpperRightArrow","<mo>&#x2197;</mo>\n"},{"Upsi","<mo>&#x03D2;</mo>\n"},{"upsi","<mo>&#x03C5;</mo>\n"},{"Upsilon","<mo>&#x03A5;</mo>\n"},{"upsilon","<mo>&#x03C5;</mo>\n"},{"UpTee","<mo>&#x22A5;</mo>\n"},{"UpTeeArrow","<mo>&#x21A5;</mo>\n"},{"upuparrows","<mo>&#x21C8;</mo>\n"},{"urcorn","<mo>&#x231D;</mo>\n"},{"urcorner","<mo>&#x231D;</mo>\n"},{"urcrop","<mo>&#x230E;</mo>\n"},{"Uring","<mo>&#x016E;</mo>\n"},{"uring","<mo>&#x016F;</mo>\n"},{"urtri","<mo>&#x25F9;</mo>\n"},{"Uscr","<mo>&#xD4B0;</mo>\n"},{"uscr","<mo>&#xD4CA;</mo>\n"},{"utdot","<mo>&#x22F0;</mo>\n"},{"Utilde","<mo>&#x0168;</mo>\n"},{"utilde","<mo>&#x0169;</mo>\n"},{"utri","<mo>&#x25B5;</mo>\n"},{"utrif","<mo>&#x25B4;</mo>\n"},{"uuarr","<mo>&#x21C8;</mo>\n"},{"Uuml","<mo>&#x00DC;</mo>\n"},{"uuml","<mo>&#x00FC;</mo>\n"},{"uwangle","<mo>&#x29A7;</mo>\n"},{"vangrt","<mo>&#x299C;</mo>\n"},{"varepsilon","<mo>&#x03B5;</mo>\n"},{"varkappa","<mo>&#x03F0;</mo>\n"},{"varnothing","<mo>&#x2205;</mo>\n"},{"varphi","<mo>&#x03C6;</mo>\n"},{"varpi","<mo>&#x03D6;</mo>\n"},{"varpropto","<mo>&#x221D;</mo>\n"},{"vArr","<mo>&#x21D5;</mo>\n"},{"varr","<mo>&#x2195;</mo>\n"},{"varrho","<mo>&#x03F1;</mo>\n"},{"varsigma","<mo>&#x03C2;</mo>\n"},{"varsubsetneq","<mo>&#x228A;</mo>\n"},{"varsubsetneqq","<mo>&#x2ACB;</mo>\n"},{"varsupsetneq","<mo>&#x228B;</mo>\n"},{"varsupsetneqq","<mo>&#x2ACC;</mo>\n"},{"vartheta","<mo>&#x03D1;</mo>\n"},{"vartriangleleft","<mo>&#x22B2;</mo>\n"},{"vartriangleright","<mo>&#x22B3;</mo>\n"},{"Vbar","<mo>&#x2AEB;</mo>\n"},{"vBar","<mo>&#x2AE8;</mo>\n"},{"vBarv","<mo>&#x2AE9;</mo>\n"},{"Vcy","<mo>&#x0412;</mo>\n"},{"vcy","<mo>&#x0432;</mo>\n"},{"VDash","<mo>&#x22AB;</mo>\n"},{"Vdash","<mo>&#x22A9;</mo>\n"},{"vDash","<mo>&#x22A8;</mo>\n"},{"vdash","<mo>&#x22A2;</mo>\n"},{"Vdashl","<mo>&#x2AE6;</mo>\n"},{"Vee","<mo>&#x22C1;</mo>\n"},{"vee","<mo>&#x2228;</mo>\n"},{"veebar","<mo>&#x22BB;</mo>\n"},{"veeeq","<mo>&#x225A;</mo>\n"},{"vellip","<mo>&#x22EE;</mo>\n"},{"Verbar","<mo>&#x2016;</mo>\n"},{"verbar","<mo>&#x007C;</mo>\n"},{"Vert","<mo>&#x2016;</mo>\n"},{"vert","<mo>&#x007C;</mo>\n"},{"VerticalBar","<mo>&#x2223;</mo>\n"},{"VerticalLine","<mo>&#x007C;</mo>\n"},{"VerticalSeparator","<mo>&#x2758;</mo>\n"},{"VerticalTilde","<mo>&#x2240;</mo>\n"},{"VeryThinSpace","<mo>&#x200A;</mo>\n"},{"Vfr","<mo>&#xD519;</mo>\n"},{"vfr","<mo>&#xD533;</mo>\n"},{"vltri","<mo>&#x22B2;</mo>\n"},{"vnsub","<mo>&#x2282;</mo>\n"},{"vnsup","<mo>&#x2283;</mo>\n"},{"Vopf","<mo>&#xD54D;</mo>\n"},{"vopf","<mo>&#xD567;</mo>\n"},{"vprop","<mo>&#x221D;</mo>\n"},{"vrtri","<mo>&#x22B3;</mo>\n"},{"Vscr","<mo>&#xD4B1;</mo>\n"},{"vscr","<mo>&#xD4CB;</mo>\n"},{"vsubnE","<mo>&#x2ACB;</mo>\n"},{"vsubne","<mo>&#x228A;</mo>\n"},{"vsupnE","<mo>&#x2ACC;</mo>\n"},{"vsupne","<mo>&#x228B;</mo>\n"},{"Vvdash","<mo>&#x22AA;</mo>\n"},{"vzigzag","<mo>&#x299A;</mo>\n"},{"Wcirc","<mo>&#x0174;</mo>\n"},{"wcirc","<mo>&#x0175;</mo>\n"},{"wedbar","<mo>&#x2A5F;</mo>\n"},{"Wedge","<mo>&#x22C0;</mo>\n"},{"wedge","<mo>&#x2227;</mo>\n"},{"wedgeq","<mo>&#x2259;</mo>\n"},{"weierp","<mo>&#x2118;</mo>\n"},{"Wfr","<mo>&#xD51A;</mo>\n"},{"wfr","<mo>&#xD534;</mo>\n"},{"Wopf","<mo>&#xD54E;</mo>\n"},{"wopf","<mo>&#xD568;</mo>\n"},{"wp","<mo>&#x2118;</mo>\n"},{"wr","<mo>&#x2240;</mo>\n"},{"wreath","<mo>&#x2240;</mo>\n"},{"Wscr","<mo>&#xD4B2;</mo>\n"},{"wscr","<mo>&#xD4CC;</mo>\n"},{"xcap","<mo>&#x22C2;</mo>\n"},{"xcirc","<mo>&#x25EF;</mo>\n"},{"xcup","<mo>&#x22C3;</mo>\n"},{"xdtri","<mo>&#x25BD;</mo>\n"},{"Xfr","<mo>&#xD51B;</mo>\n"},{"xfr","<mo>&#xD535;</mo>\n"},{"xhArr","<mo>&#x27FA;</mo>\n"},{"xharr","<mo>&#x27F7;</mo>\n"},{"Xi","<mo>&#x039E;</mo>\n"},{"xi","<mo>&#x03BE;</mo>\n"},{"xlArr","<mo>&#x27F8;</mo>\n"},{"xlarr","<mo>&#x27F5;</mo>\n"},{"xmap","<mo>&#x27FC;</mo>\n"},{"xnis","<mo>&#x22FB;</mo>\n"},{"xodot","<mo>&#x2A00;</mo>\n"},{"Xopf","<mo>&#xD54F;</mo>\n"},{"xopf","<mo>&#xD569;</mo>\n"},{"xoplus","<mo>&#x2A01;</mo>\n"},{"xotime","<mo>&#x2A02;</mo>\n"},{"xrArr","<mo>&#x27F9;</mo>\n"},{"xrarr","<mo>&#x27F6;</mo>\n"},{"Xscr","<mo>&#xD4B3;</mo>\n"},{"xscr","<mo>&#xD4CD;</mo>\n"},{"xsqcup","<mo>&#x2A06;</mo>\n"},{"xuplus","<mo>&#x2A04;</mo>\n"},{"xutri","<mo>&#x25B3;</mo>\n"},{"xvee","<mo>&#x22C1;</mo>\n"},{"xwedge","<mo>&#x22C0;</mo>\n"},{"Yacute","<mo>&#x00DD;</mo>\n"},{"yacute","<mo>&#x00FD;</mo>\n"},{"YAcy","<mo>&#x042F;</mo>\n"},{"yacy","<mo>&#x044F;</mo>\n"},{"Ycirc","<mo>&#x0176;</mo>\n"},{"ycirc","<mo>&#x0177;</mo>\n"},{"Ycy","<mo>&#x042B;</mo>\n"},{"ycy","<mo>&#x044B;</mo>\n"},{"yen","<mo>&#x00A5;</mo>\n"},{"Yfr","<mo>&#xD51C;</mo>\n"},{"yfr","<mo>&#xD536;</mo>\n"},{"YIcy","<mo>&#x0407;</mo>\n"},{"yicy","<mo>&#x0457;</mo>\n"},{"Yopf","<mo>&#xD550;</mo>\n"},{"yopf","<mo>&#xD56A;</mo>\n"},{"Yscr","<mo>&#xD4B4;</mo>\n"},{"yscr","<mo>&#xD4CE;</mo>\n"},{"YUcy","<mo>&#x042E;</mo>\n"},{"yucy","<mo>&#x044E;</mo>\n"},{"Yuml","<mo>&#x0178;</mo>\n"},{"yuml","<mo>&#x00FF;</mo>\n"},{"Zacute","<mo>&#x0179;</mo>\n"},{"zacute","<mo>&#x017A;</mo>\n"},{"Zcaron","<mo>&#x017D;</mo>\n"},{"zcaron","<mo>&#x017E;</mo>\n"},{"Zcy","<mo>&#x0417;</mo>\n"},{"zcy","<mo>&#x0437;</mo>\n"},{"Zdot","<mo>&#x017B;</mo>\n"},{"zdot","<mo>&#x017C;</mo>\n"},{"zeetrf","<mo>&#x2128;</mo>\n"},{"ZeroWidthSpace","<mo>&#x200B;</mo>\n"},{"zeta","<mo>&#x03B6;</mo>\n"},{"Zfr","<mo>&#x2128;</mo>\n"},{"zfr","<mo>&#xD537;</mo>\n"},{"ZHcy","<mo>&#x0416;</mo>\n"},{"zhcy","<mo>&#x0436;</mo>\n"},{"zigrarr","<mo>&#x21DD;</mo>\n"},{"Zopf","<mo>&#x2124;</mo>\n"},{"zopf","<mo>&#xD56B;</mo>\n"},{"Zscr","<mo>&#xD4B5;</mo>\n"},{"zscr","<mo>&#xD4CF;</mo>\n"},
            #endregion
        };

        /// <summary>
        /// Gets an empty string. This property must be overriden by all the inheritors.
        /// </summary>
        public override string Name
        {
            get { return ""; }
        }

        /// <summary>
        /// Returns true if the block cancels a math environment; otherwise, false.
        /// </summary>
        /// <returns></returns>
        public virtual bool CancelsMathMode()
        {
            return false;
        }

        public static bool CancelsMathMode(string name)
        {
            foreach (var commandConverter in CommandConverters)
            {
                if (commandConverter.Value.Name == name)
                {
                    return commandConverter.Value.CancelsMathMode();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public virtual int ExpectedBranchesCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the type of the corresponding expression (that is, ExpressionType.Command).
        /// </summary>
        public override ExpressionType ExprType
        {
            get
            {
                return ExpressionType.Command;
            }
        }

        /// <summary>
        /// Gets the Expressions[0][0] string or an empty string if no child expressions exist.
        /// This property can be overriden by an inheritor.
        /// </summary>
        public virtual string FirstValue
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the hash code of this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ExprType.GetHashCode() ^ FirstValue.GetHashCode();
        }

        /// <summary>
        /// Converts the value of this instance to a System.String.
        /// </summary>
        /// <returns>The System.String instance.</returns>
        public override string ToString()
        {
            return string.Format("[{0}] {1} ({2})", ExprType, Name, FirstValue);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the specified System.Object is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var conv = obj as CommandConverter;
            if (conv == null)
            {
                return false;
            }
            return Name == conv.Name && ExprType == conv.ExprType && FirstValue == conv.FirstValue;
        }

        /// <summary>
        /// Searches in a predefined conversion table and returns the converted result or null.
        /// </summary>
        /// <param name="table">The conversion table to search in.</param>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The converted result or null.</returns>
        private static string SearchInTables(IDictionary<string, string> table, LatexExpression expr)
        {
            string constant;
            if (table.TryGetValue(expr.Name, out constant))
            {
                string children = "";
                if (expr.Expressions != null && expr.Options != null && expr.Options.AsExpressions != null)
                {
                    for (int i = 0; i < expr.Options.AsExpressions.Count; i++)
                    {
                        children += expr.Options.AsExpressions[i].Convert();
                    }
                }
                if (expr.Expressions != null)
                {
                    for (int i = 0; i < expr.Expressions.Count; i++)
                    {
                        for (int j = 0; j < expr.Expressions[i].Count; j++)
                        {
                            children += expr.Expressions[i][j].Convert();
                        }
                    }
                }
                return constant + children;
            }
            return null;
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            CommandConverter converter;
            if (CommandConverters.TryGetValue(expr.GetCommandConverterHashCode(), out converter))
            {
                if (converter.ExpectedBranchesCount > 0 && (expr.Expressions == null || expr.Expressions.Count < converter.ExpectedBranchesCount))
                {

                    throw new FormatException(@"Unexpected format in command \\" + converter.Name);
                    //return "<!-- Unexpected format in command \\" + converter.Name + " -->";
                }
                var result = converter.Convert(expr);
                // Make sure that {} blocks which were attached to the command by mistake will be converted, too.
                // Goddamn ancient Latex
                if (expr.Expressions != null && expr.Expressions.Count > converter.ExpectedBranchesCount)
                {
                    for (int i = converter.ExpectedBranchesCount; i < expr.Expressions.Count; i++)
                    {
                        result += SequenceConverter.ConvertOutline(expr.Expressions[i], expr.Customization);
                    }
                }

                #region Stimulsoft Correction
                if (converter.Name == "mathcal" || converter.Name == "mathbb")
                    return "<mi>" + result + "</mi>"; 
                #endregion

                return result;
            }
            string constant;
            if (CommandConstants.TryGetValue(expr.Name, out constant))
            {
                if (expr.MathMode)
                {
                    return "<mi>" + constant + "</mi>";
                }
                return constant;
            }
            if ((constant = SearchInTables(MathCommandConstants, expr)) != null)
            {
                return constant;
            }
            if ((constant = SearchInTables(MathFunctionsCommandConstants, expr)) != null)
            {
                return constant;
            }
            if ((constant = SearchInTables(MathFunctionsScriptCommandConstants, expr)) != null)
            {
                return constant;
            }
            if ((constant = SearchInTables(StiLatexCommandConstants, expr)) != null)
            {
                return constant;
            }
            return "<!-- \\" + LatexStringToXmlString(expr.Name) + " -->\n";
        }
    }
}
