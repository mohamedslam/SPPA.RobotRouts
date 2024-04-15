#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Drawing;

namespace Stimulsoft.Base.Drawing
{
    public partial class StiTextRenderer
    {
        #region Allows Html-tags
        //Allows tag:
        //+ <b> </b>
        //+ <i> </i>
        //+ <u> </u>
        //+ <s> </s>
        //+ <sub> </sub>
        //+ <sup> </sup>
        //+ <font color="#rrggbb" face="FontName" size="1..n"> </font>
        //+ <font-face="FontName"> </font-face>
        //+ <font-name="FontName"> </font-name>
        //+ <font-family="FontName"> </font-family>
        //+ <font-size="1..n"> </font-size>
        //+ <font-color="#rrggbb"> </font-color>
        //+ <color="#rrggbb"> </color>
        //+ <background-color="#rrggbb"> </background-color>
        //+ <letter-spacing="0"> </letter-spacing>
        //+ <word-spacing="0"> </word-spacing>
        //+ <line-height="1"> </line-height>
        //+ <text-align="left"> </text-align> //center, right, justify
        //+ <br> <br/>
        //+ &amp; &lt; &gt; &quot; &nbsp; &#xxxx;
        //+ <p> </p>
        //+ <strong> </strong>  //аналогично <b>
        //+ <em> </em>          //аналогично <i>
        //+ <strike> </strike>  //аналогично <s>
        //+ <ul> </ul> <li> </li>
        //+ <ol> </ol> <li> </li>
        //+ <a href="...">
        //+ &apos

        //+ attribute "style"
        //+ attribute "type" for 'ul', 'ol', 'li'

        //+ color formats: #rrggbb #rgb rgb(r,g,b)
        //+ font formats: name name1,name2

        //+ <p>: style attributes "margin-top", "margin-bottom"; and "margin" but only applied for top and bottom

        //! trimming not work properly for html

        private static Hashtable htmlEscapeSequence = null;
        private static Hashtable HtmlEscapeSequence
        {
            get
            {
                if (htmlEscapeSequence == null)
                {
                    htmlEscapeSequence = new Hashtable();

                    htmlEscapeSequence["&quot;"] = (char)34;
                    htmlEscapeSequence["&amp;"] = (char)38;
                    htmlEscapeSequence["&apos;"] = (char)39;
                    htmlEscapeSequence["&lt;"] = (char)60;
                    htmlEscapeSequence["&gt;"] = (char)62;

                    htmlEscapeSequence["&nbsp;"] = (char)160;
                    htmlEscapeSequence["&iexcl;"] = (char)161;
                    htmlEscapeSequence["&cent;"] = (char)162;
                    htmlEscapeSequence["&pound;"] = (char)163;
                    htmlEscapeSequence["&curren;"] = (char)164;
                    htmlEscapeSequence["&yen;"] = (char)165;
                    htmlEscapeSequence["&brvbar;"] = (char)166;
                    htmlEscapeSequence["&sect;"] = (char)167;
                    htmlEscapeSequence["&uml;"] = (char)168;
                    htmlEscapeSequence["&copy;"] = (char)169;
                    htmlEscapeSequence["&ordf;"] = (char)170;
                    htmlEscapeSequence["&laquo;"] = (char)171;
                    htmlEscapeSequence["&not;"] = (char)172;
                    htmlEscapeSequence["&shy;"] = (char)173;
                    htmlEscapeSequence["&reg;"] = (char)174;
                    htmlEscapeSequence["&macr;"] = (char)175;
                    htmlEscapeSequence["&deg;"] = (char)176;
                    htmlEscapeSequence["&plusmn;"] = (char)177;
                    htmlEscapeSequence["&sup2;"] = (char)178;
                    htmlEscapeSequence["&sup3;"] = (char)179;
                    htmlEscapeSequence["&acute;"] = (char)180;
                    htmlEscapeSequence["&micro;"] = (char)181;
                    htmlEscapeSequence["&para;"] = (char)182;
                    htmlEscapeSequence["&middot;"] = (char)183;
                    htmlEscapeSequence["&cedil;"] = (char)184;
                    htmlEscapeSequence["&sup1;"] = (char)185;
                    htmlEscapeSequence["&ordm;"] = (char)186;
                    htmlEscapeSequence["&raquo;"] = (char)187;
                    htmlEscapeSequence["&frac14;"] = (char)188;
                    htmlEscapeSequence["&frac12;"] = (char)189;
                    htmlEscapeSequence["&frac34;"] = (char)190;
                    htmlEscapeSequence["&iquest;"] = (char)191;
                    htmlEscapeSequence["&Agrave;"] = (char)192;
                    htmlEscapeSequence["&Aacute;"] = (char)193;
                    htmlEscapeSequence["&Acirc;"] = (char)194;
                    htmlEscapeSequence["&Atilde;"] = (char)195;
                    htmlEscapeSequence["&Auml;"] = (char)196;
                    htmlEscapeSequence["&Aring;"] = (char)197;
                    htmlEscapeSequence["&AElig;"] = (char)198;
                    htmlEscapeSequence["&Ccedil;"] = (char)199;
                    htmlEscapeSequence["&Egrave;"] = (char)200;
                    htmlEscapeSequence["&Eacute;"] = (char)201;
                    htmlEscapeSequence["&Ecirc;"] = (char)202;
                    htmlEscapeSequence["&Euml;"] = (char)203;
                    htmlEscapeSequence["&Igrave;"] = (char)204;
                    htmlEscapeSequence["&Iacute;"] = (char)205;
                    htmlEscapeSequence["&Icirc;"] = (char)206;
                    htmlEscapeSequence["&Iuml;"] = (char)207;
                    htmlEscapeSequence["&ETH;"] = (char)208;
                    htmlEscapeSequence["&Ntilde;"] = (char)209;
                    htmlEscapeSequence["&Ograve;"] = (char)210;
                    htmlEscapeSequence["&Oacute;"] = (char)211;
                    htmlEscapeSequence["&Ocirc;"] = (char)212;
                    htmlEscapeSequence["&Otilde;"] = (char)213;
                    htmlEscapeSequence["&Ouml;"] = (char)214;
                    htmlEscapeSequence["&times;"] = (char)215;
                    htmlEscapeSequence["&Oslash;"] = (char)216;
                    htmlEscapeSequence["&Ugrave;"] = (char)217;
                    htmlEscapeSequence["&Uacute;"] = (char)218;
                    htmlEscapeSequence["&Ucirc;"] = (char)219;
                    htmlEscapeSequence["&Uuml;"] = (char)220;
                    htmlEscapeSequence["&Yacute;"] = (char)221;
                    htmlEscapeSequence["&THORN;"] = (char)222;
                    htmlEscapeSequence["&szlig;"] = (char)223;
                    htmlEscapeSequence["&agrave;"] = (char)224;
                    htmlEscapeSequence["&aacute;"] = (char)225;
                    htmlEscapeSequence["&acirc;"] = (char)226;
                    htmlEscapeSequence["&atilde;"] = (char)227;
                    htmlEscapeSequence["&auml;"] = (char)228;
                    htmlEscapeSequence["&aring;"] = (char)229;
                    htmlEscapeSequence["&aelig;"] = (char)230;
                    htmlEscapeSequence["&ccedil;"] = (char)231;
                    htmlEscapeSequence["&egrave;"] = (char)232;
                    htmlEscapeSequence["&eacute;"] = (char)233;
                    htmlEscapeSequence["&ecirc;"] = (char)234;
                    htmlEscapeSequence["&euml;"] = (char)235;
                    htmlEscapeSequence["&igrave;"] = (char)236;
                    htmlEscapeSequence["&iacute;"] = (char)237;
                    htmlEscapeSequence["&icirc;"] = (char)238;
                    htmlEscapeSequence["&iuml;"] = (char)239;
                    htmlEscapeSequence["&eth;"] = (char)240;
                    htmlEscapeSequence["&ntilde;"] = (char)241;
                    htmlEscapeSequence["&ograve;"] = (char)242;
                    htmlEscapeSequence["&oacute;"] = (char)243;
                    htmlEscapeSequence["&ocirc;"] = (char)244;
                    htmlEscapeSequence["&otilde;"] = (char)245;
                    htmlEscapeSequence["&ouml;"] = (char)246;
                    htmlEscapeSequence["&divide;"] = (char)247;
                    htmlEscapeSequence["&oslash;"] = (char)248;
                    htmlEscapeSequence["&ugrave;"] = (char)249;
                    htmlEscapeSequence["&uacute;"] = (char)250;
                    htmlEscapeSequence["&ucirc;"] = (char)251;
                    htmlEscapeSequence["&uuml;"] = (char)252;
                    htmlEscapeSequence["&yacute;"] = (char)253;
                    htmlEscapeSequence["&thorn;"] = (char)254;
                    htmlEscapeSequence["&yuml;"] = (char)255;

                    htmlEscapeSequence["&OElig;"] = (char)338;
                    htmlEscapeSequence["&oelig;"] = (char)339;
                    htmlEscapeSequence["&Scaron;"] = (char)352;
                    htmlEscapeSequence["&scaron;"] = (char)353;
                    htmlEscapeSequence["&Yuml;"] = (char)376;
                    htmlEscapeSequence["&fnof;"] = (char)402;
                    htmlEscapeSequence["&circ;"] = (char)710;
                    htmlEscapeSequence["&tilde;"] = (char)732;

                    htmlEscapeSequence["&Alpha;"] = (char)913;
                    htmlEscapeSequence["&Beta;"] = (char)914;
                    htmlEscapeSequence["&Gamma;"] = (char)915;
                    htmlEscapeSequence["&Delta;"] = (char)916;
                    htmlEscapeSequence["&Epsilon;"] = (char)917;
                    htmlEscapeSequence["&Zeta;"] = (char)918;
                    htmlEscapeSequence["&Eta;"] = (char)919;
                    htmlEscapeSequence["&Theta;"] = (char)920;
                    htmlEscapeSequence["&Iota;"] = (char)921;
                    htmlEscapeSequence["&Kappa;"] = (char)922;
                    htmlEscapeSequence["&Lambda;"] = (char)923;
                    htmlEscapeSequence["&Mu;"] = (char)924;
                    htmlEscapeSequence["&Nu;"] = (char)925;
                    htmlEscapeSequence["&Xi;"] = (char)926;
                    htmlEscapeSequence["&Omicron;"] = (char)927;
                    htmlEscapeSequence["&Pi;"] = (char)928;
                    htmlEscapeSequence["&Rho;"] = (char)929;
                    htmlEscapeSequence["&Sigma;"] = (char)931;
                    htmlEscapeSequence["&Tau;"] = (char)932;
                    htmlEscapeSequence["&Upsilon;"] = (char)933;
                    htmlEscapeSequence["&Phi;"] = (char)934;
                    htmlEscapeSequence["&Chi;"] = (char)935;
                    htmlEscapeSequence["&Psi;"] = (char)936;
                    htmlEscapeSequence["&Omega;"] = (char)937;
                    htmlEscapeSequence["&alpha;"] = (char)945;
                    htmlEscapeSequence["&beta;"] = (char)946;
                    htmlEscapeSequence["&gamma;"] = (char)947;
                    htmlEscapeSequence["&delta;"] = (char)948;
                    htmlEscapeSequence["&epsilon;"] = (char)949;
                    htmlEscapeSequence["&zeta;"] = (char)950;
                    htmlEscapeSequence["&eta;"] = (char)951;
                    htmlEscapeSequence["&theta;"] = (char)952;
                    htmlEscapeSequence["&iota;"] = (char)953;
                    htmlEscapeSequence["&kappa;"] = (char)954;
                    htmlEscapeSequence["&lambda;"] = (char)955;
                    htmlEscapeSequence["&mu;"] = (char)956;
                    htmlEscapeSequence["&nu;"] = (char)957;
                    htmlEscapeSequence["&xi;"] = (char)958;
                    htmlEscapeSequence["&omicron;"] = (char)959;
                    htmlEscapeSequence["&pi;"] = (char)960;
                    htmlEscapeSequence["&rho;"] = (char)961;
                    htmlEscapeSequence["&sigmaf;"] = (char)962;
                    htmlEscapeSequence["&sigma;"] = (char)963;
                    htmlEscapeSequence["&tau;"] = (char)964;
                    htmlEscapeSequence["&upsilon;"] = (char)965;
                    htmlEscapeSequence["&phi;"] = (char)966;
                    htmlEscapeSequence["&chi;"] = (char)967;
                    htmlEscapeSequence["&psi;"] = (char)968;
                    htmlEscapeSequence["&omega;"] = (char)969;
                    htmlEscapeSequence["&thetasym;"] = (char)977;
                    htmlEscapeSequence["&upsih;"] = (char)978;
                    htmlEscapeSequence["&piv;"] = (char)982;

                    htmlEscapeSequence["&ensp;"] = (char)8194;
                    htmlEscapeSequence["&emsp;"] = (char)8195;
                    htmlEscapeSequence["&thinsp;"] = (char)8201;
                    htmlEscapeSequence["&zwnj;"] = (char)8204;
                    htmlEscapeSequence["&zwj;"] = (char)8205;
                    htmlEscapeSequence["&lrm;"] = (char)8206;
                    htmlEscapeSequence["&rlm;"] = (char)8207;
                    htmlEscapeSequence["&ndash;"] = (char)8211;
                    htmlEscapeSequence["&mdash;"] = (char)8212;
                    htmlEscapeSequence["&lsquo;"] = (char)8216;
                    htmlEscapeSequence["&rsquo;"] = (char)8217;
                    htmlEscapeSequence["&sbquo;"] = (char)8218;
                    htmlEscapeSequence["&ldquo;"] = (char)8220;
                    htmlEscapeSequence["&rdquo;"] = (char)8221;
                    htmlEscapeSequence["&bdquo;"] = (char)8222;
                    htmlEscapeSequence["&dagger;"] = (char)8224;
                    htmlEscapeSequence["&Dagger;"] = (char)8225;
                    htmlEscapeSequence["&bull;"] = (char)8226;
                    htmlEscapeSequence["&hellip;"] = (char)8230;
                    htmlEscapeSequence["&permil;"] = (char)8240;
                    htmlEscapeSequence["&prime;"] = (char)8242;
                    htmlEscapeSequence["&Prime;"] = (char)8243;
                    htmlEscapeSequence["&lsaquo;"] = (char)8249;
                    htmlEscapeSequence["&rsaquo;"] = (char)8250;
                    htmlEscapeSequence["&oline;"] = (char)8254;
                    htmlEscapeSequence["&frasl;"] = (char)8260;
                    htmlEscapeSequence["&euro;"] = (char)8364;
                    htmlEscapeSequence["&image;"] = (char)8365;

                    htmlEscapeSequence["&weierp;"] = (char)8472;
                    htmlEscapeSequence["&real;"] = (char)8476;
                    htmlEscapeSequence["&trade;"] = (char)8482;

                    htmlEscapeSequence["&alefsym;"] = (char)8501;
                    htmlEscapeSequence["&larr;"] = (char)8592;
                    htmlEscapeSequence["&uarr;"] = (char)8593;
                    htmlEscapeSequence["&rarr;"] = (char)8594;
                    htmlEscapeSequence["&darr;"] = (char)8595;
                    htmlEscapeSequence["&harr;"] = (char)8596;
                    htmlEscapeSequence["&crarr;"] = (char)8629;
                    htmlEscapeSequence["&lArr;"] = (char)8656;
                    htmlEscapeSequence["&uArr;"] = (char)8657;
                    htmlEscapeSequence["&rArr;"] = (char)8658;
                    htmlEscapeSequence["&dArr;"] = (char)8659;
                    htmlEscapeSequence["&hArr;"] = (char)8660;
                    htmlEscapeSequence["&forall;"] = (char)8704;
                    htmlEscapeSequence["&part;"] = (char)8706;
                    htmlEscapeSequence["&exist;"] = (char)8707;
                    htmlEscapeSequence["&empty;"] = (char)8709;
                    htmlEscapeSequence["&nabla;"] = (char)8711;
                    htmlEscapeSequence["&isin;"] = (char)8712;
                    htmlEscapeSequence["&notin;"] = (char)8713;
                    htmlEscapeSequence["&ni;"] = (char)8715;
                    htmlEscapeSequence["&prod;"] = (char)8719;
                    htmlEscapeSequence["&sum;"] = (char)8721;
                    htmlEscapeSequence["&minus;"] = (char)8722;
                    htmlEscapeSequence["&lowast;"] = (char)8727;
                    htmlEscapeSequence["&radic;"] = (char)8730;
                    htmlEscapeSequence["&prop;"] = (char)8733;
                    htmlEscapeSequence["&infin;"] = (char)8734;
                    htmlEscapeSequence["&ang;"] = (char)8736;
                    htmlEscapeSequence["&and;"] = (char)8743;
                    htmlEscapeSequence["&or;"] = (char)8744;
                    htmlEscapeSequence["&cap;"] = (char)8745;
                    htmlEscapeSequence["&cup;"] = (char)8746;
                    htmlEscapeSequence["&int;"] = (char)8747;
                    htmlEscapeSequence["&there4;"] = (char)8756;
                    htmlEscapeSequence["&sim;"] = (char)8764;
                    htmlEscapeSequence["&cong;"] = (char)8773;
                    htmlEscapeSequence["&asymp;"] = (char)8776;
                    htmlEscapeSequence["&ne;"] = (char)8800;
                    htmlEscapeSequence["&equiv;"] = (char)8801;
                    htmlEscapeSequence["&le;"] = (char)8804;
                    htmlEscapeSequence["&ge;"] = (char)8805;
                    htmlEscapeSequence["&sub;"] = (char)8834;
                    htmlEscapeSequence["&sup;"] = (char)8835;
                    htmlEscapeSequence["&nsub;"] = (char)8836;
                    htmlEscapeSequence["&sube;"] = (char)8838;
                    htmlEscapeSequence["&supe;"] = (char)8839;
                    htmlEscapeSequence["&oplus;"] = (char)8853;
                    htmlEscapeSequence["&otimes;"] = (char)8855;
                    htmlEscapeSequence["&perp;"] = (char)8869;
                    htmlEscapeSequence["&sdot;"] = (char)8901;
                    htmlEscapeSequence["&lceil;"] = (char)8968;
                    htmlEscapeSequence["&rceil;"] = (char)8969;
                    htmlEscapeSequence["&lfloor;"] = (char)8970;
                    htmlEscapeSequence["&rfloor;"] = (char)8971;
                    htmlEscapeSequence["&lang;"] = (char)9001;
                    htmlEscapeSequence["&rang;"] = (char)9002;
                    htmlEscapeSequence["&loz;"] = (char)9674;
                    htmlEscapeSequence["&spades;"] = (char)9824;
                    htmlEscapeSequence["&clubs;"] = (char)9827;
                    htmlEscapeSequence["&hearts;"] = (char)9829;
                    htmlEscapeSequence["&diams;"] = (char)9830;
                }
                return htmlEscapeSequence;
            }
        }
        #endregion

        #region Enum StiHtmlTag
        public enum StiHtmlTag
        {
            None = 0,
            B,
            I,
            U,
            S,
            Sup,
            Sub,
            Font,
            FontName,
            FontSize,
            FontColor,
            Backcolor,
            LetterSpacing,
            WordSpacing,
            LineHeight,
            TextAlign,
            P,
            Br,
            OrderedList,
            UnorderedList,
            ListItem,
            A,
            Unknown
        }
        #endregion

        #region Class StiHtmlTag2
        public enum StiHtmlTag2State
        {
            Start = 0,
            End,
            Empty,
        }

        public class StiHtmlTag2
        {
            public StiHtmlTag Tag;
            public string TagName;
            public List<TagPair> Attributes;

            internal StiHtmlTag2State State;

            public bool IsStart => State == StiHtmlTag2State.Start;
            public bool IsEnd => State == StiHtmlTag2State.End;
            public bool IsEmpty => State == StiHtmlTag2State.Empty;

            public bool IsStartTag(StiHtmlTag tag) => Tag == tag && State == StiHtmlTag2State.Start;
            public bool IsEndTag(StiHtmlTag tag) => Tag == tag && State == StiHtmlTag2State.End;

            public bool Equals(StiHtmlTag2 tag2)
            {
                if (Tag != tag2.Tag) return false;
                if (Tag == StiHtmlTag.Unknown) return TagName == tag2.TagName;
                return true;
            }

            public override string ToString()
            {
                return Tag == StiHtmlTag.Unknown ? $"'{TagName}'" : Tag.ToString() + (IsStart ? "_Open" : "") + (IsEnd ? "_Close" : "");
            }

            public StiHtmlTag2(StiHtmlTag tag = StiHtmlTag.None, StiHtmlTag2State state = StiHtmlTag2State.Start)
            {
                this.Tag = tag;
                this.State = state;
            }
        }
        #endregion

        #region Method.ConvertStringToTag
        private static StiHtmlTag2 ConvertStringToTag(string st)
        {
            StiHtmlTag2 tag2 = new StiHtmlTag2();

            st = st.Trim();
            if (st.EndsWith("/"))
            {
                tag2.State = StiHtmlTag2State.Empty;
                st = st.Substring(0, st.Length - 1).Trim();
            }

            tag2.Attributes = ParseTagIntoPairs(st);
            if (tag2.Attributes.Count == 0) return tag2;

            string tag = tag2.Attributes[0].Key;
            if (tag.StartsWith("/"))
            {
                tag2.State = StiHtmlTag2State.End;
                tag = tag.Substring(1);
            }

            switch (tag)
            {
                case "p":   tag2.Tag = StiHtmlTag.P; break;
                case "br":  tag2.Tag = StiHtmlTag.Br; break;
                case "ol":  tag2.Tag = StiHtmlTag.OrderedList; break;
                case "ul":  tag2.Tag = StiHtmlTag.UnorderedList; break;
                case "li":  tag2.Tag = StiHtmlTag.ListItem; break;
                case "a":   tag2.Tag = StiHtmlTag.A; break;

                case "strong":
                case "b":
                    tag2.Tag = StiHtmlTag.B; break;
                case "em":
                case "i":
                    tag2.Tag = StiHtmlTag.I; break;
                case "u":
                    tag2.Tag = StiHtmlTag.U; break;
                case "strike":
                case "s":
                    tag2.Tag = StiHtmlTag.S; break;
                case "sup":
                    tag2.Tag = StiHtmlTag.Sup; break;
                case "sub":
                    tag2.Tag = StiHtmlTag.Sub; break;

                case "letter-spacing":  tag2.Tag = StiHtmlTag.LetterSpacing; break;
                case "word-spacing":    tag2.Tag = StiHtmlTag.WordSpacing; break;
                case "line-height":     tag2.Tag = StiHtmlTag.LineHeight; break;
                case "text-align":      tag2.Tag = StiHtmlTag.TextAlign; break;

                case "font":
                    tag2.Tag = StiHtmlTag.Font; break;
                case "font-face":
                case "font-family":
                case "font-name":
                    tag2.Tag = StiHtmlTag.FontName; break;
                case "font-size":
                    tag2.Tag = StiHtmlTag.FontSize; break;
                case "font-color":
                case "color":
                    tag2.Tag = StiHtmlTag.FontColor; break;
                case "background-color":
                    tag2.Tag = StiHtmlTag.Backcolor; break;
            }

            if (tag2.Tag == StiHtmlTag.None && !string.IsNullOrWhiteSpace(tag))
            {
                tag2.Tag = StiHtmlTag.Unknown;
                tag2.TagName = tag;
            }

            return tag2;
        }
        #endregion

        #region Struct StiHtmlTagsState
        public struct StiHtmlTagsState
        {
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public bool Strikeout;
            public float FontSize;
            public string FontName;
            public Color FontColor;
            public Color BackColor;
            public bool Subscript;
            public bool Superscript;
            public double LetterSpacing;
            public double WordSpacing;
            public double LineHeight;
            public StiTextHorAlignment TextAlign;
            public bool IsColorChanged;
            public bool IsBackcolorChanged;
            public StiHtmlTag2 Tag;
            public int Indent;
            public string HtmlStyle;
            public string Href;
            public Hashtable StyleAttributes;

            public string GetStyleAttribute(string name)
            {
                if (StyleAttributes != null) return (string)StyleAttributes[name];
                return null;
            }

            public StiHtmlTagsState(bool bold, bool italic, bool underline, bool strikeout, float fontSize, string fontName, Color fontColor, Color backColor,
                bool superscript, bool subscript, double letterSpacing, double wordSpacing, double lineHeight, StiTextHorAlignment textAlign)
            {
                Bold = bold;
                Italic = italic;
                Underline = underline;
                Strikeout = strikeout;
                FontSize = fontSize;
                FontName = fontName;
                FontColor = fontColor;
                BackColor = backColor;
                Subscript = subscript;
                Superscript = superscript;
                LetterSpacing = letterSpacing;
                WordSpacing = wordSpacing;
                LineHeight = lineHeight;
                TextAlign = textAlign;
                IsColorChanged = false;
                IsBackcolorChanged = false;
                Tag = new StiHtmlTag2();
                Indent = 0;
                HtmlStyle = string.Empty;
                Href = null;
                StyleAttributes = null;
            }

            public StiHtmlTagsState(StiHtmlTagsState state)
            {
                Bold = state.Bold;
                Italic = state.Italic;
                Underline = state.Underline;
                Strikeout = state.Strikeout;
                FontSize = state.FontSize;
                FontName = state.FontName;
                FontColor = state.FontColor;
                BackColor = state.BackColor;
                Subscript = state.Subscript;
                Superscript = state.Superscript;
                LetterSpacing = state.LetterSpacing;
                WordSpacing = state.WordSpacing;
                LineHeight = state.LineHeight;
                TextAlign = state.TextAlign;
                IsColorChanged = state.IsColorChanged;
                IsBackcolorChanged = state.IsBackcolorChanged;
                Tag = state.Tag;
                Indent = state.Indent;
                HtmlStyle = string.Empty;
                Href = state.Href;
                StyleAttributes = state.StyleAttributes;
            }
        }
        #endregion

        #region Struct StiHtmlState
        public struct StiHtmlState
        {
            public StiHtmlTagsState TS;
            public StringBuilder Text;
            public int FontIndex;
            public int PosBegin;
            internal List<StiHtmlTagsState> TagsStack;
            internal List<int> ListLevels;

            public override string ToString()
            {
                return "\"" + Text.ToString().Replace("\n", "\\n") + "\" " + (TS.Tag.Tag == StiHtmlTag.None ? "" : TS.Tag.ToString()) + $" {TS.LineHeight}";
            }

            public StiHtmlState(string text)
            {
                TS = new StiHtmlTagsState();
                TS.Tag = new StiHtmlTag2();
                Text = new StringBuilder(text);
                FontIndex = 0;
                PosBegin = 0;
                TagsStack = null;
                ListLevels = null;
            }

            public StiHtmlState(StiHtmlTagsState ts, int posBegin)
            {
                TS = ts;
                Text = new StringBuilder();
                FontIndex = 0;
                PosBegin = posBegin;
                TagsStack = null;
                ListLevels = null;
            }

            public StiHtmlState(StiHtmlState state)
            {
                TS = new StiHtmlTagsState(state.TS);
                Text = new StringBuilder();
                FontIndex = 0;
                PosBegin = state.PosBegin;
                TagsStack = (state.TagsStack != null && state.TagsStack.Count > 0) ? state.TagsStack : null;
                ListLevels = state.ListLevels;

                if (TS.Indent < 0)
                {
                    if (ListLevels != null)
                    {
                        TS.Indent = ListLevels.Count;
                    }
                    else
                    {
                        TS.Indent = 0;
                    }
                }
            }
        }
        #endregion

        #region Methods.ParseHtmlToStates
        public static List<StiHtmlState> ParseHtmlToStates(string inputHtml, StiHtmlState baseState, bool storeStack = false)
        {
            List<StiHtmlState> states = ParseHtmlToStates2(inputHtml, baseState, storeStack);

            #region CheckMissingGlyphs
            if (AllowCheckMissingGlyphs)
            {
                try
                {
                    var states2 = new List<StiHtmlState>();
                    foreach (var state in states)
                    {
                        var newStates = CheckMissingGlyphs(state);
                        if (newStates != null)
                        {
                            states2.AddRange(newStates);
                        }
                        else
                        {
                            states2.Add(state);
                        }
                    }
                    states = states2;
                }
                catch
                {
                    AllowCheckMissingGlyphs = false;
                }
            }
            #endregion

            return states;
        }

        private static List<StiHtmlState> ParseHtmlToStates2(string inputHtml, StiHtmlState baseState, bool storeStack)
        {
            var resultList = new List<StiHtmlState>();
            StiHtmlState state = baseState;
            var stack = new List<StiHtmlTagsState>();
            
            int pos = 0;
            bool lastSymIsSpace = false;
            bool isListItem = false;
            if (inputHtml == null) inputHtml = string.Empty;
            while (pos < inputHtml.Length)
            {
                if (inputHtml[pos] != '<')
                {
                    char ch = inputHtml[pos];
                    if (char.IsWhiteSpace(ch) && (ch != 0xA0))
                    {
                        if (!lastSymIsSpace)
                        {
                            state.Text.Append(' ');
                            lastSymIsSpace = true;
                        }
                    }
                    else
                    {
                        if (char.GetUnicodeCategory(ch) != UnicodeCategory.OtherNotAssigned)
                        {
                            state.Text.Append(ch);
                            lastSymIsSpace = false;
                        }
                    }
                    pos++;
                }
                else
                {
                    if (state.Text.Length > 0)
                    {
                        resultList.Add(state);
                        state = new StiHtmlState(state);
                        state.PosBegin = pos;
                        if (state.TS.Tag.Tag == StiHtmlTag.ListItem) state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                    }
                    while ((pos < inputHtml.Length) && (inputHtml[pos] == '<'))
                    {
                        pos++;
                        int posEnd = pos;
                        while ((posEnd < inputHtml.Length) && (inputHtml[posEnd] != '>'))
                        {
                            posEnd++;
                        }
                        string tag = inputHtml.Substring(pos, posEnd - pos);
                        pos = posEnd;
                        pos++;

                        var tag2 = ConvertStringToTag(tag);

                        if (tag2.Tag == StiHtmlTag.Unknown)
                        {
                            if (tag2.TagName == "div")
                            {
                                tag2.Tag = StiHtmlTag.P;
                                tag2.Attributes.Add(new TagPair() { Key = "style", KeyBase = "style", Value = "margin:0;" });
                            }
                        }

                        if (tag2.IsEndTag(StiHtmlTag.P))
                        {
                            ParseStyleAttributes(tag2, ref state, baseState.TS);

                            state.Text.Append('\n');
                            resultList.Add(state);
                            state = new StiHtmlState(state);
                            state.PosBegin = pos;
                            lastSymIsSpace = true;
                            state.TS.Tag = new StiHtmlTag2(StiHtmlTag.P, StiHtmlTag2State.End);

                            if ((pos < inputHtml.Length) && !string.IsNullOrWhiteSpace(inputHtml.Substring(pos)))
                            {
                                double paragraphLineHeight = GetMarginSize(state, "margin-bottom");

                                state.Text.Append('\n');
                                double storedLineHeight = state.TS.LineHeight;
                                state.TS.LineHeight = paragraphLineHeight;

                                resultList.Add(state);

                                state = new StiHtmlState(state);
                                state.PosBegin = pos;
                                state.TS.LineHeight = storedLineHeight;
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                            }
                            if (state.TS.Indent > 0)
                            {
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                            }
                        }
                        else if (tag2.IsStartTag(StiHtmlTag.P))
                        {
                            ParseStyleAttributes(tag2, ref state, baseState.TS);

                            double paragraphLineHeight = GetMarginSize(state, "margin-top");

                            bool isPreviousTagPClose = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.IsEndTag(StiHtmlTag.P));
                            if (!isPreviousTagPClose)
                            {
                                if ((resultList.Count > 1) || ((resultList.Count == 1) && ((resultList[0]).Text.ToString().Trim().Length != 0)))
                                {
                                    state.Text.Append('\n');
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                                state.PosBegin = pos;
                                lastSymIsSpace = true;

                                state.Text.Append('\n');
                                double storedLineHeight = state.TS.LineHeight;
                                state.TS.LineHeight = paragraphLineHeight;
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.P);

                                resultList.Add(state);

                                state = new StiHtmlState(state);
                                state.TS.LineHeight = storedLineHeight;
                                state.PosBegin = pos;
                                if (state.TS.Indent > 0)
                                {
                                    state.Text.Append(GetIndentString(state.TS.Indent));
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                            }
                            else
                            {
                                var lastp = resultList[resultList.Count - 1];
                                if (lastp.TS.LineHeight < paragraphLineHeight)
                                {
                                    lastp.TS.LineHeight = paragraphLineHeight;
                                    resultList[resultList.Count - 1] = lastp;
                                }
                            }
                        }
                        else if (tag2.Tag == StiHtmlTag.Br)
                        {
                            lastSymIsSpace = true;
                            state.Text.Append('\n');
                            var oldTag = state.TS.Tag;
                            if (isListItem)
                            {
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                            }
                            resultList.Add(state);
                            state = new StiHtmlState(state);
                            state.PosBegin = pos;
                            state.TS.Tag = oldTag;
                            if (state.TS.Indent > 0)
                            {
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                            }
                        }
                        else if (tag2.IsStartTag(StiHtmlTag.ListItem))
                        {
                            isListItem = true;
                            int prevIndex = resultList.Count - 1;
                            while ((prevIndex >= 0) && (resultList[prevIndex].Text.ToString() == "\n")) prevIndex--;
                            bool isPreviousTagListItem = (prevIndex >= 0) && ((resultList[prevIndex]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            //if (state.TS.Tag == StiHtmlTag.ListItem)
                            //{
                            //    //after text break, if break point is inside the list
                            //    lastSymIsSpace = true;
                            //    state.PosBegin = pos;
                            //    state.Text.Append(GetIndentString(state.TS.Indent));
                            //    resultList.Add(state);
                            //    state = new StiHtmlState(state);
                            //    state.TS.Tag = StiHtmlTag.None;
                            //    state.PosBegin = pos;
                            //}
                            //else 
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                if (!((resultList.Count == 0) || (resultList.Count > 0) && (resultList[resultList.Count - 1].Text.ToString() == "\n")))
                                {
                                    state.Text.Append('\n');
                                    state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                if (state.TS.Indent == 0 && state.ListLevels == null)
                                {
                                    state.ListLevels = new List<int>();
                                    state.ListLevels.Add(0);
                                    state.TS.Indent++;
                                }
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                            if (state.TS.Indent == 0) state.TS.Indent++;
                            if (state.ListLevels == null) state.ListLevels = new List<int>();
                            while (state.TS.Indent > state.ListLevels.Count) state.ListLevels.Add(0);
                            var state1 = resultList[resultList.Count - 1];
                            ParseMarkerTypeAttribute(tag2.Attributes, ref state);
                            InsertMarker(state1.Text, state.ListLevels[state.TS.Indent - 1], state.TS.Indent);
                            if (state.ListLevels[state.TS.Indent - 1] > 0)
                            {
                                state.ListLevels = new List<int>(state.ListLevels);
                                state.ListLevels[state.TS.Indent - 1]++;
                            }
                            resultList[resultList.Count - 1] = state1;
                        }
                        else if (tag2.IsEndTag(StiHtmlTag.ListItem))
                        {
                            isListItem = false;
                            bool isPreviousTagListItem = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                if (state.Text.Length > 0)
                                {
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                                state.Text.Append('\n');
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                if (state.TS.Indent == 0) state.TS.Indent++;
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                        }
                        else if (tag2.IsStartTag(StiHtmlTag.UnorderedList))
                        {
                            ParseStyleAttributes(tag2, ref state, baseState.TS);
                            bool isPreviousTagListItem = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                if (!((resultList.Count == 0) || (resultList.Count > 0) && (resultList[resultList.Count - 1].Text.ToString() == "\n")))
                                {
                                    state.Text.Append('\n');
                                    state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                state.Text.Append(GetIndentString(state.TS.Indent + 1));
                                state.TS.Indent++;
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                            else
                            {
                                state.TS.Indent++;
                                var state1 = resultList[resultList.Count - 1];
                                state1.Text.Append(GetIndentString(1));
                                state1.TS.Indent++;
                                resultList[resultList.Count - 1] = state1;
                            }
                            if (state.ListLevels == null) state.ListLevels = new List<int>();
                            while (state.ListLevels.Count < state.TS.Indent) state.ListLevels.Add(0);
                            state.ListLevels[state.TS.Indent - 1] = 1 - state.TS.Indent;
                            ParseMarkerTypeAttribute(tag2.Attributes, ref state);
                        }
                        else if (tag2.IsEndTag(StiHtmlTag.UnorderedList))
                        {
                            isListItem = false;
                            bool isPreviousTagListItem = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                state.Text.Append('\n');
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                if (state.TS.Indent > 0) state.TS.Indent--;
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                if (state.TS.Indent == 0) state.ListLevels = null;
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                            else
                            {
                                if (state.TS.Indent > 0) state.TS.Indent--; 
                                var state1 = resultList[resultList.Count - 1];
                                if (state1.TS.Indent > 0) state1.TS.Indent--;
                                state1.Text = new StringBuilder(GetIndentString(state1.TS.Indent));
                                if (state.TS.Indent == 0)
                                {
                                    state.ListLevels = null;
                                    state1.ListLevels = null;
                                }
                                resultList[resultList.Count - 1] = state1;
                                state.PosBegin = pos;
                            }                            
                        }
                        else if (tag2.IsStartTag(StiHtmlTag.OrderedList))
                        {
                            ParseStyleAttributes(tag2, ref state, baseState.TS);
                            bool isPreviousTagListItem = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                if (!((resultList.Count == 0) || (resultList.Count > 0) && (resultList[resultList.Count - 1].Text.ToString() == "\n")))
                                {
                                    state.Text.Append('\n');
                                    state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                    resultList.Add(state);
                                    state = new StiHtmlState(state);
                                }
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                state.Text.Append(GetIndentString(state.TS.Indent + 1));
                                state.TS.Indent++;
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                            else
                            {
                                state.TS.Indent++;
                                var state1 = resultList[resultList.Count - 1];
                                state1.Text.Append(GetIndentString(1));
                                state1.TS.Indent++;
                                resultList[resultList.Count - 1] = state1;
                            }
                            if (state.ListLevels == null) state.ListLevels = new List<int>();
                            while (state.ListLevels.Count < state.TS.Indent) state.ListLevels.Add(1);
                            state.ListLevels[state.TS.Indent - 1] = 1;
                            ParseMarkerTypeAttribute(tag2.Attributes, ref state);

                            var state2 = resultList[resultList.Count - 1];
                            state2.ListLevels = state.ListLevels;
                            resultList[resultList.Count - 1] = state2;
                        }
                        else if (tag2.IsEndTag(StiHtmlTag.OrderedList))
                        {
                            isListItem = false;
                            bool isPreviousTagListItem = (resultList.Count > 0) && ((resultList[resultList.Count - 1]).TS.Tag.Tag == StiHtmlTag.ListItem);
                            if (!isPreviousTagListItem)
                            {
                                lastSymIsSpace = true;
                                state.Text.Append('\n');
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.ListItem);
                                state.PosBegin = pos;
                                if (state.TS.Indent > 0) state.TS.Indent--;
                                state.Text.Append(GetIndentString(state.TS.Indent));
                                if (state.TS.Indent == 0) state.ListLevels = null;
                                resultList.Add(state);
                                state = new StiHtmlState(state);
                                state.TS.Tag = new StiHtmlTag2(StiHtmlTag.None);
                                state.PosBegin = pos;
                            }
                            else
                            {
                                if (state.TS.Indent > 0) state.TS.Indent--;
                                var state1 = resultList[resultList.Count - 1];
                                if (state1.TS.Indent > 0) state1.TS.Indent--;
                                state1.Text = new StringBuilder(GetIndentString(state1.TS.Indent));
                                if (state.TS.Indent == 0)
                                {
                                    state.ListLevels = null;
                                    state1.ListLevels = null;
                                }
                                resultList[resultList.Count - 1] = state1;
                                state.PosBegin = pos;
                            }
                        }
                        else
                        {
                            if (tag2.Tag != StiHtmlTag.None)
                            {
                                var tsOld = new StiHtmlTagsState(state.TS);
                                ParseHtmlTag(tag2, ref state, stack, baseState);
                                if (tsOld.FontSize != state.TS.FontSize)
                                {
                                    var newState = new StiHtmlState(state);
                                    newState.Text = state.Text;

                                    state.TS = tsOld;
                                    state.Text = new StringBuilder();
                                    resultList.Add(state);

                                    state = newState;
                                    state.TS.Tag = new StiHtmlTag2(StiHtmlTag.Font);
                                }
                            }
                            if (storeStack && stack.Count > 0)
                            {
                                state.TagsStack = new List<StiHtmlTagsState>();
                                foreach (StiHtmlTagsState tagState in stack)
                                {
                                    state.TagsStack.Add(new StiHtmlTagsState(tagState));
                                }
                            }
                        }
                    }
                }
            }
            if (state.Text.Length > 0) resultList.Add(state);
            if (resultList.Count == 0) resultList.Add(state);
            return resultList;
        }

        private static double GetMarginSize(StiHtmlState state, string marginName)
        {
            double paragraphLineHeight = defaultParagraphLineHeight;

            try
            {
                string attrLH = state.TS.GetStyleAttribute(marginName);
                if (string.IsNullOrWhiteSpace(attrLH)) attrLH = state.TS.GetStyleAttribute("margin");
                if (!string.IsNullOrWhiteSpace(attrLH))
                {
                    paragraphLineHeight = ParseSizeToEm(attrLH, state.TS.FontSize, null, defaultParagraphLineHeight);
                }
            }
            catch { }

            return paragraphLineHeight;
        }
        #endregion

        #region Methods.ParseHtmlTag
        private static void ParseHtmlTag(StiHtmlTag2 tag2, ref StiHtmlState state, List<StiHtmlTagsState> stack, StiHtmlState baseState)
        {
            string delimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //var htmlTag = StiHtmlTag.None;
            //bool closeTag = false;

            #region parse pairs
            var tsLast = new StiHtmlTagsState(state.TS);
            TagPair de = tag2.Attributes[0];
            switch (tag2.Tag)
            {
                case StiHtmlTag.B:
                    state.TS.Bold = tag2.IsStart;
                    break;

                case StiHtmlTag.I:
                    state.TS.Italic = tag2.IsStart;
                    break;

                case StiHtmlTag.U:
                    state.TS.Underline = tag2.IsStart;
                    break;

                case StiHtmlTag.S:
                    state.TS.Strikeout = tag2.IsStart;
                    break;

                case StiHtmlTag.Sup:
                    state.TS.Superscript = tag2.IsStart;
                    state.TS.Subscript = false;
                    break;

                case StiHtmlTag.Sub:
                    state.TS.Subscript = tag2.IsStart;
                    state.TS.Superscript = false;
                    break;

                case StiHtmlTag.LetterSpacing:
                    if (tag2.IsStart)
                    {
                        double letterSpacing = 0;
                        if (de.Value != "normal")
                        {
                            letterSpacing = ParseSizeToEm(de.Value, state.TS.FontSize, delimiter);
                        }
                        state.TS.LetterSpacing = letterSpacing;
                    }
                    break;

                case StiHtmlTag.WordSpacing:
                    if (tag2.IsStart)
                    {
                        double wordSpacing = 0;
                        if (de.Value != "normal")
                        {
                            wordSpacing = ParseSizeToEm(de.Value, state.TS.FontSize, delimiter);
                        }
                        state.TS.WordSpacing = wordSpacing;
                    }
                    break;

                case StiHtmlTag.LineHeight:
                    if (tag2.IsStart)
                    {
                        double lineHeight = 1;
                        if (de.Value != "normal")
                        {
                            lineHeight = ParseSizeToEm(de.Value, state.TS.FontSize, delimiter, 1);
                        }
                        if (lineHeight < 0) lineHeight = 1;
                        state.TS.LineHeight = lineHeight;
                    }
                    break;

                case StiHtmlTag.TextAlign:
                    if (tag2.IsStart)
                    {
                        try
                        {
                            string align = ((string)de.Value).ToLowerInvariant();
                            if (align == "left") state.TS.TextAlign = StiTextHorAlignment.Left;
                            if (align == "right") state.TS.TextAlign = StiTextHorAlignment.Right;
                            if (align == "center") state.TS.TextAlign = StiTextHorAlignment.Center;
                            if (align == "justify") state.TS.TextAlign = StiTextHorAlignment.Width;
                        }
                        catch { }
                    }
                    break;

                case StiHtmlTag.Font:
                    if (tag2.IsStart && tag2.Attributes.Count > 1)
                    {
                        #region parse font attributes
                        for (int indexDE = 1; indexDE < tag2.Attributes.Count; indexDE++)
                        {
                            TagPair def = tag2.Attributes[indexDE];
                            switch ((string)def.Key)
                            {
                                case "color":
                                    try
                                    {
                                        state.TS.FontColor = ParseColor((string)def.Value, baseState.TS.FontColor);
                                        state.TS.IsColorChanged = true;
                                    }
                                    catch
                                    {
                                    }
                                    break;

                                case "face":
                                case "family":
                                case "name":
                                    try
                                    {
                                        state.TS.FontName = (string)def.Value;
                                    }
                                    catch
                                    {
                                    }
                                    break;

                                case "size":
                                    float ffontSize = ParseFontSize((string)def.Value, delimiter);
                                    state.TS.FontSize = ffontSize;
                                    break;

                                case "style":
                                    state.TS.HtmlStyle = (string)def.Value;
                                    break;
                            }
                        }
                        #endregion
                    }
                    break;

                case StiHtmlTag.FontName:
                    if (tag2.IsStart)
                    {
                        try
                        {
                            state.TS.FontName = (string)de.Value;
                        }
                        catch { }
                    }
                    break;

                case StiHtmlTag.FontSize:
                    if (tag2.IsStart)
                    {
                        float fontSize = ParseFontSize((string)de.Value, delimiter);
                        state.TS.FontSize = fontSize;
                    }
                    break;

                case StiHtmlTag.FontColor:
                    if (tag2.IsStart)
                    {
                        try
                        {
                            state.TS.FontColor = ParseColor((string)de.Value, baseState.TS.FontColor);
                            state.TS.IsColorChanged = true;
                        }
                        catch { }
                    }
                    break;

                case StiHtmlTag.Backcolor:
                    if (tag2.IsStart)
                    {
                        try
                        {
                            state.TS.BackColor = ParseColor((string)de.Value, baseState.TS.BackColor);
                            state.TS.IsBackcolorChanged = true;
                        }
                        catch { }
                    }
                    break;

                case StiHtmlTag.A:
                    if (tag2.IsStart)
                    {
                        try
                        {
                            if ((tag2.Attributes.Count > 1) && (tag2.Attributes[1].Key == "href"))
                            {
                                string href = (string)tag2.Attributes[1].Value;
                                if (!string.IsNullOrWhiteSpace(href))
                                {
                                    state.TS.Href = href.Trim();

                                    state.TS.FontColor = Color.Blue;    //todo change to default URI color
                                    state.TS.IsColorChanged = true;
                                }
                            }
                        }
                        catch { }
                    }
                    break;

                case StiHtmlTag.Unknown:
                    {
                        if (tag2.TagName == "stihtml")
                        {
                            stack.Clear();
                            try
                            {
                                if (tag2.Attributes.Count > 1 && tag2.Attributes[1].KeyBase != null)
                                {
                                    stack.AddRange(StringToStack((string)tag2.Attributes[1].KeyBase, baseState.TS));
                                }
                            }
                            catch { }
                        }

                        if (tag2.TagName == "stihtml2")
                        {
                            try
                            {
                                if (tag2.Attributes.Count > 2 && tag2.Attributes[2].Key != null)
                                {
                                    state.ListLevels = StringToListLevels((string)tag2.Attributes[2].Key);
                                    if (state.ListLevels != null) state.TS.Indent = state.ListLevels.Count;
                                    int lineInfoIndent = int.Parse(tag2.Attributes[1].Key.ToString());
                                    if (lineInfoIndent > 0) state.TS.Indent = -lineInfoIndent;
                                }
                            }
                            catch { }
                        }
                        break;
                    }
            }

            if (tag2.IsEnd)
            {
                if (stack.Count > 0)
                {
                    for (int index = stack.Count - 1; index >= 0; index--)
                    {
                        StiHtmlTagsState ts = stack[index];
                        if (ts.Tag.Equals(tag2))
                        {
                            state.TS = ts;
                            stack.RemoveRange(index, stack.Count - index);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (tag2.Tag != StiHtmlTag.None)
                {
                    tsLast.Tag = tag2;
                    stack.Add(tsLast);
                }
            }

            if (tag2.IsStart && tag2.Attributes.Count > 1)
            {
                ParseStyleAttributes(tag2, ref state, baseState.TS);
            }
            #endregion
        }

        private static List<TagPair> ParseTagIntoPairs(string tag)
        {
            var attr = new List<TagPair>();
            int pos = 0;
            while ((pos < tag.Length) && (tag[pos] == ' ')) pos++;
            while (pos < tag.Length)
            {
                int pos2 = pos;
                var pair = new TagPair();
                while ((pos2 < tag.Length) && (tag[pos2] != ' ') && (tag[pos2] != '=')) pos2++;
                pair.KeyBase = tag.Substring(pos, pos2 - pos);
                pair.Key = pair.KeyBase.ToLowerInvariant();
                pos = pos2;
                while ((pos < tag.Length) && (tag[pos] == ' ')) pos++;
                if ((pos < tag.Length) && (tag[pos] == '='))
                {
                    pos++;
                    while ((pos < tag.Length) && (tag[pos] == ' ')) pos++;
                    if (pos < tag.Length)
                    {
                        if (tag[pos] == '"')
                        {
                            pos++;
                            pos2 = pos;
                            while ((pos2 < tag.Length) && (tag[pos2] != '"')) pos2++;
                            pair.Value = tag.Substring(pos, pos2 - pos);
                            pos = pos2;
                            pos++;
                        }
                        else
                        {
                            pos2 = pos;
                            while ((pos2 < tag.Length) && (tag[pos2] != ' ')) pos2++;
                            pair.Value = tag.Substring(pos, pos2 - pos);
                            pos = pos2;
                        }
                    }
                }
                while ((pos < tag.Length) && (tag[pos] == ' ')) pos++;
                attr.Add(pair);
            }
            return attr;
        }

        public class TagPair
        {
            public string Key;
            public string KeyBase;
            public string Value;
        }

        private static void ParseMarkerTypeAttribute(List<TagPair> attr, ref StiHtmlState state)
        {
            foreach (TagPair pair in attr)
            {
                if ((pair.KeyBase == "type") && !string.IsNullOrWhiteSpace(pair.Value))
                {
                    if (state.ListLevels != null)
                    {
                        char marker = pair.Value[0];
                        if (pair.Value == "disc") marker = '\u2022';
                        if (pair.Value == "circle") marker = '\u25E6';
                        if (pair.Value == "square") marker = '\u25AA';
                        if (pair.Value == "none") marker = ' ';
                        if (pair.Value == "I")
                        {
                            state.ListLevels[state.ListLevels.Count - 1] = 0x8000;
                        }
                        else
                        {
                            state.ListLevels[state.ListLevels.Count - 1] = -(int)marker;
                        }
                    }
                }
            }
        }

        private static void ParseStyleAttributes(StiHtmlTag2 tag2, ref StiHtmlState state, StiHtmlTagsState baseTS)
        {
            for (int indexDE = 1; indexDE < tag2.Attributes.Count; indexDE++)
            {
                var def = tag2.Attributes[indexDE];
                if ((string)def.Key == "style")
                {
                    ParseStyleAttribute((string)def.Value, ref state, baseTS);
                }
            }
        }

        private static void ParseStyleAttribute(string style, ref StiHtmlState state, StiHtmlTagsState baseTS)
        {
            if (string.IsNullOrEmpty(style)) return;
            string delimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string[] pairs = style.Trim().Split(new char[] { ';' });
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(new char[] { ':' });
                bool hasValue = parts.Length > 1;
                string key = parts[0].Trim();
                string value = hasValue ? parts[1].Trim() : null;
                switch (key)
                {
                    case "color":
                        try
                        {
                            state.TS.FontColor = ParseColor(value, baseTS.FontColor);
                            state.TS.IsColorChanged = true;
                        }
                        catch
                        {
                        }
                        break;

                    case "background-color":
                        try
                        {
                            state.TS.BackColor = ParseColor(value, baseTS.BackColor);
                            state.TS.IsBackcolorChanged = true;
                        }
                        catch
                        {
                        }
                        break;

                    case "text-decoration":
                        if (value == "underline") state.TS.Underline = true;
                        if (value == "line-through") state.TS.Strikeout = true;
                        if (value == "none")
                        {
                            state.TS.Underline = false;
                            state.TS.Strikeout = false;
                        }
                        break;

                    case "font-weight":
                        state.TS.Bold = (value == "bold" || value == "bolder" || value == "600" || value == "700" || value == "800" || value == "900");
                        break;

                    case "font-style":
                        if (value == "normal") state.TS.Italic = false;
                        if (value == "italic" || value == "oblique") state.TS.Italic = true;
                        break;

                    case "font-size":
                        float fontSize = ParseFontSize(value, delimiter);
                        state.TS.FontSize = fontSize;
                        break;

                    case "font-face":
                    case "font-family":
                    case "font-name":
                        try
                        {
                            state.TS.FontName = value.Replace("\'", "").Replace("\"", "");
                        }
                        catch
                        {
                        }
                        break;

                    case "vertical-align":
                        if (value == "baseline")
                        {
                            state.TS.Subscript = false;
                            state.TS.Superscript = false;
                        }
                        if (value == "sub")
                        {
                            state.TS.Subscript = true;
                            state.TS.Superscript = false;
                        }
                        if (value == "super")
                        {
                            state.TS.Subscript = false;
                            state.TS.Superscript = true;
                        }
                        break;

                    case "letter-spacing":
                        double letterSpacing = 0;
                        if (value != "normal")
                        {
                            letterSpacing = ParseSizeToEm(value, state.TS.FontSize, delimiter);
                        }
                        state.TS.LetterSpacing = letterSpacing;
                        break;

                    case "word-spacing":
                        double wordSpacing = 0;
                        if (value != "normal")
                        {
                            wordSpacing = ParseSizeToEm(value, state.TS.FontSize, delimiter);
                        }
                        state.TS.WordSpacing = wordSpacing;
                        break;

                    case "line-height":
                        double lineHeight = 1;
                        if (value != "normal")
                        {
                            lineHeight = ParseSizeToEm(value, state.TS.FontSize, delimiter, 1);
                        }
                        if (lineHeight < 0) lineHeight = 1;
                        state.TS.LineHeight = lineHeight;
                        break;

                    case "text-align":
                        string align = value.ToLowerInvariant();
                        if (align == "left") state.TS.TextAlign = StiTextHorAlignment.Left;
                        if (align == "right") state.TS.TextAlign = StiTextHorAlignment.Right;
                        if (align == "center") state.TS.TextAlign = StiTextHorAlignment.Center;
                        if (align == "justify") state.TS.TextAlign = StiTextHorAlignment.Width;
                        break;

                    default:
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            if (state.TS.StyleAttributes == null) state.TS.StyleAttributes = new Hashtable();
                            state.TS.StyleAttributes[key] = value;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Utils
        public static StringBuilder PrepareStateText(StringBuilder stateText)
        {
            //convert &#xxxx; to unicode symbols
            StringBuilder sbTemp = new StringBuilder();
            int indexChar = 0;
            while (indexChar < stateText.Length)
            {
                bool flag = false;
                if ((stateText[indexChar] == '&') && (indexChar + 3 < stateText.Length))
                {
                    int indexChar2 = indexChar + 1;
                    StringBuilder sbTemp2 = new StringBuilder();
                    if (stateText[indexChar2] == '#')
                    {
                        indexChar2++;
                        if (stateText[indexChar2] == 'x')
                        {
                            // &#xHHHH;
                            indexChar2++;
                            while ((indexChar2 < stateText.Length) && char.IsLetterOrDigit(stateText[indexChar2]))
                            {
                                sbTemp2.Append(stateText[indexChar2]);
                                indexChar2++;
                            }
                            if ((sbTemp2.Length > 0) && (indexChar2 < stateText.Length) && (stateText[indexChar2] == ';'))
                            {
                                indexChar2++;
                                uint num = 0;
                                if (uint.TryParse(sbTemp2.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
                                {
                                    sbTemp.Append((char)num);
                                    indexChar = indexChar2;
                                    flag = true;
                                }
                            }
                        }
                        else
                        {
                            // &#DDDD;
                            while ((indexChar2 < stateText.Length) && char.IsDigit(stateText[indexChar2]))
                            {
                                sbTemp2.Append(stateText[indexChar2]);
                                indexChar2++;
                            }
                            if ((sbTemp2.Length > 0) && (indexChar2 < stateText.Length) && (stateText[indexChar2] == ';'))
                            {
                                indexChar2++;
                                sbTemp.Append((char)(uint.Parse(sbTemp2.ToString())));
                                indexChar = indexChar2;
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        while ((indexChar2 < stateText.Length) && char.IsLetterOrDigit(stateText[indexChar2]))
                        {
                            sbTemp2.Append(stateText[indexChar2]);
                            indexChar2++;
                        }
                        if ((sbTemp2.Length > 0) && (indexChar2 < stateText.Length) && (stateText[indexChar2] == ';'))
                        {
                            object es = HtmlEscapeSequence["&" + sbTemp2 + ";"];
                            if (es != null)
                            {
                                indexChar2++;
                                sbTemp.Append((char)es);
                                indexChar = indexChar2;
                                flag = true;
                            }
                        }
                    }
                }
                if (!flag)
                {
                    sbTemp.Append(stateText[indexChar]);
                    indexChar++;
                }
            }

            //sbTemp.Replace("&nbsp;", "\xA0")
            //      .Replace("&lt;", "<")
            //      .Replace("&gt;", ">")
            //      .Replace("&quot;", "\"")
            //      .Replace("&amp;", "&");
            return sbTemp;
        }

        private static Color ParseColor(string colorAttribute, Color inheritColor)
        {
            if (colorAttribute.ToLowerInvariant() == "inherit") return inheritColor;
            Color color = Color.Transparent;
            if (colorAttribute.Length > 1)
            {
                if (colorAttribute[0] == '#')
                {
                    #region Parse RGB value in hexadecimal notation
                    string colorSt = colorAttribute.Substring(1).ToLowerInvariant();
                    StringBuilder sbc = new StringBuilder();
                    foreach (char ch in colorSt)
                    {
                        if (ch == '0' || ch == '1' || ch == '2' || ch == '3' || ch == '4' || ch == '5' || ch == '6' || ch == '7' ||
                            ch == '8' || ch == '9' || ch == 'a' || ch == 'b' || ch == 'c' || ch == 'd' || ch == 'e' || ch == 'f') sbc.Append(ch);
                    }
                    if (sbc.Length == 3)
                    {
                        colorSt = string.Format("{0}{0}{1}{1}{2}{2}", sbc[0], sbc[1], sbc[2]);
                    }
                    else
                    {
                        colorSt = sbc.ToString();
                    }
                    if (colorSt.Length == 6)
                    {
                        int colorInt = Convert.ToInt32(colorSt, 16);
                        color = Color.FromArgb(0xFF, (colorInt >> 16) & 0xFF, (colorInt >> 8) & 0xFF, colorInt & 0xFF);
                    }
                    #endregion
                }
                else if (colorAttribute.StartsWith("rgb", StringComparison.InvariantCulture))
                {
                    #region Parse RGB function
                    string[] colors = colorAttribute.Trim().Substring(4, colorAttribute.Length - 5).Split(new char[] { ',' });
                    if (colors.Length == 3)
                    {
                        int[] colorsInt = new int[3];
                        if (colors[0].EndsWith("%", StringComparison.InvariantCulture)) colorsInt[0] = (int)Math.Round(Convert.ToInt32(colors[0].Substring(0, colors[0].Length - 1)) * 2.55);
                        else colorsInt[0] = Convert.ToInt32(colors[0]);
                        if (colors[1].EndsWith("%", StringComparison.InvariantCulture)) colorsInt[1] = (int)Math.Round(Convert.ToInt32(colors[1].Substring(0, colors[1].Length - 1)) * 2.55);
                        else colorsInt[1] = Convert.ToInt32(colors[1]);
                        if (colors[2].EndsWith("%", StringComparison.InvariantCulture)) colorsInt[2] = (int)Math.Round(Convert.ToInt32(colors[2].Substring(0, colors[2].Length - 1)) * 2.55);
                        else colorsInt[2] = Convert.ToInt32(colors[2]);
                        color = Color.FromArgb(0xFF, colorsInt[0], colorsInt[1], colorsInt[2]);
                    }
                    #endregion
                }
                else
                {
                    #region Parse color keywords
                    lock (lockHtmlNameToColor)
                    {
                        if (htmlNameToColor == null)
                        {
                            #region Init hashtable
                            string[,] initData = {
                                {"AliceBlue",       "#F0F8FF"},
                                {"AntiqueWhite",    "#FAEBD7"},
                                {"Aqua",        "#00FFFF"},
                                {"Aquamarine",  "#7FFFD4"},
                                {"Azure",       "#F0FFFF"},
                                {"Beige",       "#F5F5DC"},
                                {"Bisque",      "#FFE4C4"},
                                {"Black",       "#000000"},
                                {"BlanchedAlmond",  "#FFEBCD"},
                                {"Blue",        "#0000FF"},
                                {"BlueViolet",  "#8A2BE2"},
                                {"Brown",       "#A52A2A"},
                                {"BurlyWood",   "#DEB887"},
                                {"CadetBlue",   "#5F9EA0"},
                                {"Chartreuse",  "#7FFF00"},
                                {"Chocolate",   "#D2691E"},
                                {"Coral",       "#FF7F50"},
                                {"CornflowerBlue",  "#6495ED"},
                                {"Cornsilk",    "#FFF8DC"},
                                {"Crimson",     "#DC143C"},
                                {"Cyan",        "#00FFFF"},
                                {"DarkBlue",    "#00008B"},
                                {"DarkCyan",    "#008B8B"},
                                {"DarkGoldenRod",   "#B8860B"},
                                {"DarkGray",    "#A9A9A9"},
                                {"DarkGrey",    "#A9A9A9"},
                                {"DarkGreen",   "#006400"},
                                {"DarkKhaki",   "#BDB76B"},
                                {"DarkMagenta", "#8B008B"},
                                {"DarkOliveGreen",  "#556B2F"},
                                {"Darkorange",  "#FF8C00"},
                                {"DarkOrchid",  "#9932CC"},
                                {"DarkRed",     "#8B0000"},
                                {"DarkSalmon",  "#E9967A"},
                                {"DarkSeaGreen",    "#8FBC8F"},
                                {"DarkSlateBlue",   "#483D8B"},
                                {"DarkSlateGray",   "#2F4F4F"},
                                {"DarkSlateGrey",   "#2F4F4F"},
                                {"DarkTurquoise",   "#00CED1"},
                                {"DarkViolet",  "#9400D3"},
                                {"DeepPink",    "#FF1493"},
                                {"DeepSkyBlue", "#00BFFF"},
                                {"DimGray",     "#696969"},
                                {"DimGrey",     "#696969"},
                                {"DodgerBlue",  "#1E90FF"},
                                {"FireBrick",   "#B22222"},
                                {"FloralWhite", "#FFFAF0"},
                                {"ForestGreen", "#228B22"},
                                {"Fuchsia",     "#FF00FF"},
                                {"Gainsboro",   "#DCDCDC"},
                                {"GhostWhite",  "#F8F8FF"},
                                {"Gold",        "#FFD700"},
                                {"GoldenRod",   "#DAA520"},
                                {"Gray",        "#808080"},
                                {"Grey",        "#808080"},
                                {"Green",       "#008000"},
                                {"GreenYellow", "#ADFF2F"},
                                {"HoneyDew",    "#F0FFF0"},
                                {"HotPink",     "#FF69B4"},
                                {"IndianRed",   "#CD5C5C"},
                                {"Indigo",      "#4B0082"},
                                {"Ivory",       "#FFFFF0"},
                                {"Khaki",       "#F0E68C"},
                                {"Lavender",    "#E6E6FA"},
                                {"LavenderBlush",   "#FFF0F5"},
                                {"LawnGreen",   "#7CFC00"},
                                {"LemonChiffon",    "#FFFACD"},
                                {"LightBlue",   "#ADD8E6"},
                                {"LightCoral",  "#F08080"},
                                {"LightCyan",   "#E0FFFF"},
                                {"LightGoldenRodYellow",    "#FAFAD2"},
                                {"LightGray",   "#D3D3D3"},
                                {"LightGrey",   "#D3D3D3"},
                                {"LightGreen",  "#90EE90"},
                                {"LightPink",   "#FFB6C1"},
                                {"LightSalmon", "#FFA07A"},
                                {"LightSeaGreen",   "#20B2AA"},
                                {"LightSkyBlue",    "#87CEFA"},
                                {"LightSlateGray",  "#778899"},
                                {"LightSlateGrey",  "#778899"},
                                {"LightSteelBlue",  "#B0C4DE"},
                                {"LightYellow", "#FFFFE0"},
                                {"Lime",        "#00FF00"},
                                {"LimeGreen",   "#32CD32"},
                                {"Linen",       "#FAF0E6"},
                                {"Magenta",     "#FF00FF"},
                                {"Maroon",      "#800000"},
                                {"MediumAquaMarine",    "#66CDAA"},
                                {"MediumBlue",  "#0000CD"},
                                {"MediumOrchid",    "#BA55D3"},
                                {"MediumPurple",    "#9370D8"},
                                {"MediumSeaGreen",  "#3CB371"},
                                {"MediumSlateBlue", "#7B68EE"},
                                {"MediumSpringGreen",   "#00FA9A"},
                                {"MediumTurquoise", "#48D1CC"},
                                {"MediumVioletRed", "#C71585"},
                                {"MidnightBlue",    "#191970"},
                                {"MintCream",   "#F5FFFA"},
                                {"MistyRose",   "#FFE4E1"},
                                {"Moccasin",    "#FFE4B5"},
                                {"NavajoWhite", "#FFDEAD"},
                                {"Navy",        "#000080"},
                                {"OldLace",     "#FDF5E6"},
                                {"Olive",       "#808000"},
                                {"OliveDrab",   "#6B8E23"},
                                {"Orange",      "#FFA500"},
                                {"OrangeRed",   "#FF4500"},
                                {"Orchid",      "#DA70D6"},
                                {"PaleGoldenRod",   "#EEE8AA"},
                                {"PaleGreen",   "#98FB98"},
                                {"PaleTurquoise",   "#AFEEEE"},
                                {"PaleVioletRed",   "#D87093"},
                                {"PapayaWhip",  "#FFEFD5"},
                                {"PeachPuff",   "#FFDAB9"},
                                {"Peru",        "#CD853F"},
                                {"Pink",        "#FFC0CB"},
                                {"Plum",        "#DDA0DD"},
                                {"PowderBlue",  "#B0E0E6"},
                                {"Purple",      "#800080"},
                                {"Red",         "#FF0000"},
                                {"RosyBrown",   "#BC8F8F"},
                                {"RoyalBlue",   "#4169E1"},
                                {"SaddleBrown", "#8B4513"},
                                {"Salmon",      "#FA8072"},
                                {"SandyBrown",  "#F4A460"},
                                {"SeaGreen",    "#2E8B57"},
                                {"SeaShell",    "#FFF5EE"},
                                {"Sienna",      "#A0522D"},
                                {"Silver",      "#C0C0C0"},
                                {"SkyBlue",     "#87CEEB"},
                                {"SlateBlue",   "#6A5ACD"},
                                {"SlateGray",   "#708090"},
                                {"SlateGrey",   "#708090"},
                                {"Snow",        "#FFFAFA"},
                                {"SpringGreen", "#00FF7F"},
                                {"SteelBlue",   "#4682B4"},
                                {"Tan",         "#D2B48C"},
                                {"Teal",        "#008080"},
                                {"Thistle",     "#D8BFD8"},
                                {"Tomato",      "#FF6347"},
                                {"Turquoise",   "#40E0D0"},
                                {"Violet",      "#EE82EE"},
                                {"Wheat",       "#F5DEB3"},
                                {"White",       "#FFFFFF"},
                                {"WhiteSmoke",  "#F5F5F5"},
                                {"Yellow",      "#FFFF00"},
                                {"YellowGreen", "#9ACD32"},

                                {"ActiveBorder",   "#B4B4B4" },
                                {"ActiveCaption",    "#99B4D1" },
                                {"AppWorkspace",    "#ABABAB" },
                                {"Background",  "#000000" },
                                {"ButtonAlternateFace", "#000000" },
                                {"ButtonDkShadow",  "#696969" },
                                {"ButtonFace",  "#F0F0F0" },
                                {"ButtonHighlight",   "#FFFFFF" },
                                {"ButtonLight", "#E3E3E3" },
                                {"ButtonShadow",    "#A0A0A0" },
                                {"ButtonText",  "#000000" },
                                {"GradientActiveTitle", "#B9D1EA" },
                                {"GradientInactiveTitle",   "#D7E4F2" },
                                {"GrayText",    "#6D6D6D" },
                                {"Highlight",   "#0078D7" },
                                {"HighlightText", "#FFFFFF" },
                                {"HotTrackingColor",    "#0066CC" },
                                {"InactiveBorder",  "#F4F7FC" },
                                {"InactiveCaption",   "#BFCDDB" },
                                {"InactiveCaptionText",   "#000000" },
                                {"InfoText",    "#000000" },
                                {"InfoBackground",  "#FFFFFF" },
                                {"Menu",    "#F0F0F0" },
                                {"MenuBar", "#F0F0F0" },
                                {"MenuHighlight",   "#0078D7" },
                                {"MenuText",    "#000000" },
                                {"Scrollbar",   "#C8C8C8" },
                                {"CaptionText", "#000000" },
                                {"Window",  "#FFFFFF" },
                                {"WindowFrame", "#646464" },
                                {"WindowText",  "#000000" }
                            };

                            htmlNameToColor = new Hashtable();
                            for (int index = 0; index < initData.GetLength(0); index++)
                            {
                                string key = initData[index, 0].ToLowerInvariant();
                                int colorInt = Convert.ToInt32(initData[index, 1].Substring(1), 16);
                                Color value = Color.FromArgb(0xFF, (colorInt >> 16) & 0xFF, (colorInt >> 8) & 0xFF, colorInt & 0xFF);
                                htmlNameToColor[key] = value;
                            }
                            #endregion
                        }
                    }
                    string colorSt = colorAttribute.ToLowerInvariant();
                    if (htmlNameToColor.ContainsKey(colorSt))
                    {
                        color = (Color)htmlNameToColor[colorSt];
                    }
                    #endregion
                }
            }
            return color;
        }

        private static string ColorToHtml(Color color)
        {
            string colorSt = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            if (color.A != 255) colorSt = "#ttt";
            return colorSt;
        }

        private static float ParseFontSize(string fontSizeAttribute, string delimiter)
        {
            GraphicsUnit unit = GraphicsUnit.Point;
            if (fontSizeAttribute.EndsWith("px"))
            {
                unit = GraphicsUnit.Pixel;
                fontSizeAttribute = fontSizeAttribute.Substring(0, fontSizeAttribute.Length - 2);
            }
            if (fontSizeAttribute.EndsWith("pt"))
            {
                fontSizeAttribute = fontSizeAttribute.Substring(0, fontSizeAttribute.Length - 2);
            }

            float ffontSize = 8;
            if (!float.TryParse(fontSizeAttribute.Replace(',', '.').Replace(".", delimiter), out ffontSize))
            {
                ffontSize = 8;
            }

            if (ffontSize < 0.5) ffontSize = 0.5f;

            if (InterpreteFontSizeInHtmlTagsAsInHtml)
            {
                switch ((int)Math.Round(ffontSize))
                {
                    case 1:
                        ffontSize = 7;
                        break;

                    case 2:
                        ffontSize = 10;
                        break;

                    case 3:
                        ffontSize = 12;
                        break;

                    case 4:
                        ffontSize = 14;
                        break;

                    case 5:
                        ffontSize = 16;
                        break;

                    case 6:
                        ffontSize = 22;
                        break;

                    case 7:
                        ffontSize = 36;
                        break;
                }
            }

            if (unit == GraphicsUnit.Pixel) ffontSize *= 0.75f;

            return ffontSize;
        }

        private static double ParseSizeToEm(string value, float emSize, string delimiter = null, double defaultValue = 0)
        {
            double scaling = 1;
            if (value.EndsWith("em"))
            {
                value = value.Substring(0, value.Length - 2);
            }
            else if (value.EndsWith("pt"))
            {
                value = value.Substring(0, value.Length - 2);
                scaling = 1 / emSize;
            }
            else if (value.EndsWith("px"))
            {
                value = value.Substring(0, value.Length - 2);
                scaling = 72/96d / emSize;
            }
            else if (value.EndsWith("in"))
            {
                value = value.Substring(0, value.Length - 2);
                scaling = 72 / emSize;
            }
            else if (value.EndsWith("cm"))
            {
                value = value.Substring(0, value.Length - 2);
                scaling = 72 / 2.54 / emSize;
            }
            else if (value.EndsWith("mm"))
            {
                value = value.Substring(0, value.Length - 2);
                scaling = 72 / 25.4 / emSize;
            }
            else if (value.EndsWith("%"))
            {
                value = value.Substring(0, value.Length - 1);
                scaling = 0.01;
            }

            if (delimiter == null)
                delimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (double.TryParse(value.Trim().Replace(',', '.').Replace(".", delimiter), out double result))
            {
                return result * scaling;
            }
            return defaultValue;
        }

        public static string StateToHtml(StiHtmlState state, StiHtmlState state2, string text, int lineInfoIndent)
        {
            StringBuilder sbb = new StringBuilder();
            sbb.Append(string.Format("<font name=\"{0}\" size=\"{1}\">",
                state.TS.FontName,
                state.TS.FontSize));
            if (state2.TS.IsColorChanged)
            {
                sbb.Append(string.Format("<font-color=\"{0}\">", ColorToHtml(state2.TS.FontColor)));
            }
            if (state2.TS.IsBackcolorChanged)
            {
                sbb.Append(string.Format("<background-color=\"{0}\">", ColorToHtml(state2.TS.BackColor)));
            }
            sbb.Append(string.Format("<{0}b>", state.TS.Bold ? "" : "/"));
            sbb.Append(string.Format("<{0}i>", state.TS.Italic ? "" : "/"));
            sbb.Append(string.Format("<{0}u>", state.TS.Underline ? "" : "/"));
            sbb.Append(string.Format("<{0}s>", state.TS.Strikeout ? "" : "/"));
            sbb.Append(string.Format("<{0}sup>", state.TS.Superscript ? "" : "/"));
            sbb.Append(string.Format("<{0}sub>", state.TS.Subscript ? "" : "/"));
            sbb.Append(string.Format("<letter-spacing=\"{0}\">", state.TS.LetterSpacing));
            sbb.Append(string.Format("<word-spacing=\"{0}\">", state.TS.WordSpacing));
            sbb.Append(string.Format("<line-height=\"{0}\">", state2.TS.LineHeight));
            string align = "left";
            if (state.TS.TextAlign == StiTextHorAlignment.Center) align = "center";
            if (state.TS.TextAlign == StiTextHorAlignment.Right) align = "right";
            if (state.TS.TextAlign == StiTextHorAlignment.Width) align = "justify";
            sbb.Append(string.Format("<text-align=\"{0}\">", align));
            sbb.Append("<StiHtml " + StackToString(state.TagsStack) + ">");

            if (state2.TS.Indent > 0) sbb.Append(string.Format("<StiHtml2 {0} {1}>", lineInfoIndent, ListLevelsToString(state2.ListLevels, state2.TS.Indent)));

            if (text != null)
            {
                //first edition
                //sbb.Append(text.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\xA0", "&nbsp;"));

                //second edition
                //sbb.Append(text);

                //third edition
                sbb.Append(text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"));
            }

            return sbb.ToString();
        }

        private static string GetIndentString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < indent; index++)
            {
                sb.Append("\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0\xA0");
            }
            return sb.ToString();
        }

        private static string bulletBlack = new string('\x2022', 1);
        private static string bulletWhite = new string('\x25E6', 1);

        private static void InsertMarker(StringBuilder sb, int markerInt, int indent)
        {
            string marker = bulletBlack;
            if (markerInt > 0)
            {
                if (markerInt >= 0x8000)
                {
                    marker = ToRoman(markerInt - 0x8000 + 1) + '.';
                }
                else
                {
                    marker = markerInt.ToString() + '.';
                }
            }
            else
            {
                if (markerInt < -32)
                {
                    marker = ((char)(-markerInt)).ToString();
                }
                else
                {
                    int markerInt2 = (0 - markerInt) % 2;
                    if (markerInt2 == 1) marker = bulletWhite;
                }
            }

            int offsetMarker = markerInt > 0 ? 2 : 3;
            if (sb.Length > 3)
            {
                if (marker.Length >= sb.Length - offsetMarker)
                {
                    sb.Remove(0, sb.Length - offsetMarker);
                    sb.Insert(0, marker);
                }
                else
                {
                    int offset = sb.Length - offsetMarker - marker.Length;
                    for (int index = 0; index < marker.Length; index++)
                    {
                        sb[offset + index] = marker[index];
                    }
                }
            }
        }

        public static bool CheckTextForHtmlTags(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || (text.Length < 4)) return false;
            int startIndex = text.IndexOf("<");
            if (startIndex != -1)
            {
                int endIndex = text.IndexOf(">", startIndex + 2);
                if (endIndex != -1)
                {
                    var list = StiTextRenderer.ParseHtmlToStates(text, new StiTextRenderer.StiHtmlState() { Text = new StringBuilder("a") });
                    if (list.Count > 1) return true;
                }
            }
            return false;
        }

        public static string GetPlainTextFromHtmlTags(string input)
        {
            var baseState = new StiHtmlState(new StiHtmlTagsState(false, false, false, false, 8, "Arial", Color.Black, Color.White, false, false, 0, 0, 1, StiTextHorAlignment.Left), 0);
            var states = ParseHtmlToStates(input, baseState);
            StringBuilder sb = new StringBuilder();
            foreach (var state in states)
            {
                if (state.Text.Length == 1 && state.Text[0] == '\n')
                {
                    sb.Append("\r\n");
                }
                else
                {
                    sb.Append(PrepareStateText(state.Text));
                }
            }
            return sb.ToString();
        }

        private static string StackToString(List<StiHtmlTagsState> stack)
        {
            if (stack == null || stack.Count == 0) return string.Empty;
            var sb = new StringBuilder();
            try
            {
                for (int index = 0; index < stack.Count; index++)
                {
                    var state = stack[index];
                    var prevState = new StiHtmlTagsState();
                    bool first = index == 0;
                    if (!first) prevState = stack[index - 1];

                    var lastPos = sb.Length;

                    if (state.IsBackcolorChanged) sb.AppendFormat("bc{0:X2}{1:X2}{2:X2}{3:X2}:", state.BackColor.A, state.BackColor.R, state.BackColor.G, state.BackColor.B);
                    if (state.Bold && (first || state.Bold != prevState.Bold)) sb.Append("bd:");
                    if (state.IsColorChanged) sb.AppendFormat("fc{0:X2}{1:X2}{2:X2}{3:X2}:", state.FontColor.A, state.FontColor.R, state.FontColor.G, state.FontColor.B);
                    if (!string.IsNullOrEmpty(state.FontName) && (first || state.FontName != prevState.FontName)) sb.AppendFormat("fn{0}:", state.FontName.Replace(' ', '_'));
                    if (first || state.FontSize != prevState.FontSize) sb.AppendFormat("fs{0}:", state.FontSize);
                    if (state.Italic && (first || state.Italic != prevState.Italic)) sb.Append("it:");
                    if (first || state.LetterSpacing != prevState.LetterSpacing) sb.AppendFormat("ls{0}:", state.LetterSpacing);
                    if (first || state.LineHeight != prevState.LineHeight) sb.AppendFormat("lh{0}:", state.LineHeight);
                    if (state.Strikeout && (first || state.Strikeout != prevState.Strikeout)) sb.Append("st:");
                    if (state.Subscript && (first || state.Subscript != prevState.Subscript)) sb.Append("sb:");
                    if (state.Superscript && (first || state.Superscript != prevState.Superscript)) sb.Append("sp:");
                    if (first || !state.Tag.Equals(prevState.Tag)) sb.AppendFormat("tg{0}:", state.Tag.Tag != StiHtmlTag.Unknown ? ((int)state.Tag.Tag).ToString() : $"'{state.Tag.TagName}'");
                    if (first || state.TextAlign != prevState.TextAlign) sb.AppendFormat("ta{0}:", (int)state.TextAlign);
                    if (state.Underline && (first || state.Underline != prevState.Underline)) sb.Append("un:");
                    if (first || state.WordSpacing != prevState.WordSpacing) sb.AppendFormat("ws{0}:", state.WordSpacing);
                    if (sb[sb.Length - 1] == ':') sb.Length--;

                    if ((index < stack.Count - 1) && (sb.Length > lastPos)) sb.Append(";");
                }
                while (sb.Length > 1 && sb[sb.Length - 1] == ';') sb.Length--;
            }
            catch
            {
            }
            return sb.ToString();
        }
        private static List<StiHtmlTagsState> StringToStack(string inputString, StiHtmlTagsState baseState)
        {
            var lastState = new StiHtmlTagsState(baseState);
            var output = new List<StiHtmlTagsState>();
            try
            {
                string[] arr = inputString.Split(new char[] { ';' });
                foreach (string stState in arr)
                {
                    var state = new StiHtmlTagsState(lastState);
                    string[] arr2 = stState.Split(new char[] { ':' });
                    foreach (string stPart in arr2)
                    {
                        string stParam = stPart.Substring(2);
                        switch (stPart.Substring(0, 2))
                        {
                            case "bc":
                                state.BackColor = Color.FromArgb(
                                    int.Parse(stParam.Substring(0, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(2, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(4, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(6, 2), NumberStyles.HexNumber));
                                state.IsBackcolorChanged = true;
                                break;
                            case "bd":
                                state.Bold = true;
                                break;
                            case "fc":
                                state.FontColor = Color.FromArgb(
                                    int.Parse(stParam.Substring(0, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(2, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(4, 2), NumberStyles.HexNumber),
                                    int.Parse(stParam.Substring(6, 2), NumberStyles.HexNumber));
                                state.IsColorChanged = true;
                                break;
                            case "fn":
                                state.FontName = stParam.Replace('_', ' ');
                                break;
                            case "fs":
                                state.FontSize = float.Parse(stParam);
                                break;
                            case "it":
                                state.Italic = true;
                                break;
                            case "ls":
                                state.LetterSpacing = double.Parse(stParam);
                                break;
                            case "lh":
                                if (!double.TryParse(stParam.Replace(',', '.').Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), out state.LineHeight))
                                {
                                    state.LineHeight = 1;
                                }
                                break;
                            case "st":
                                state.Strikeout = true;
                                break;
                            case "sb":
                                state.Subscript = true;
                                break;
                            case "sp":
                                state.Superscript = true;
                                break;
                            case "tg":
                                if (stParam.StartsWith("'"))
                                {
                                    state.Tag = ConvertStringToTag(stParam.Substring(1, stParam.Length - 2));
                                }
                                else
                                {
                                    state.Tag = new StiHtmlTag2((StiHtmlTag)int.Parse(stParam));
                                }
                                break;
                            case "ta":
                                state.TextAlign = (StiTextHorAlignment)int.Parse(stParam);
                                break;
                            case "un":
                                state.Underline = true;
                                break;
                            case "ws":
                                state.WordSpacing = double.Parse(stParam);
                                break;
                        }
                    }
                    output.Add(state);
                    lastState = state;
                }
            }
            catch
            {
            }
            return output;
        }

        private static string ListLevelsToString(List<int> list, int indent)
        {
            if (list == null || list.Count == 0) list = new List<int>();
            var sb = new StringBuilder();
            try
            {
                for (int index = 0; index < indent; index++)
                {
                    if (index < list.Count)
                    {
                        sb.Append(list[index].ToString());
                    }
                    else
                    {
                        sb.Append("0");
                    }
                    if (index < indent - 1) sb.Append(";");
                }
            }
            catch
            {
            }
            return sb.ToString();
        }
        private static List<int> StringToListLevels(string inputString)
        {
            var output = new List<int>();
            try
            {
                string[] arr = inputString.Split(new char[] { ';' });
                foreach (string marker in arr)
                {
                    output.Add(int.Parse(marker));
                }
            }
            catch
            {
            }
            return output;
        }

        #region ToRoman
        private static int[] Arabics = 
            { 1, 5, 10, 50, 100, 1000 };

        private static char[] Romans = 
            { 'I', 'V', 'X', 'L', 'C', 'M' };

        private static int[] Subs = 
            { 0, 0, 0, 2, 2, 4 };

        private static string ToRoman(int value)
        {
            var str = new StringBuilder();

            while (value > 0)
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (value >= Arabics[i])
                    {
                        str.Append(Romans[i]);
                        value -= Arabics[i];
                        break;
                    }

                    var flag = false;
                    for (var j = Subs[i]; j < i; j++)
                    {
                        if (Arabics[j] == Arabics[i] - Arabics[j]) continue;
                        
                        if (value >= Arabics[i] - Arabics[j])
                        {
                            str.Append(Romans[j]);
                            str.Append(Romans[i]);
                            value -= Arabics[i] - Arabics[j];
                            flag = true;
                            break;
                        }
                    }
                    if (flag) break;
                }
            }
            return str.ToString();
        }
        #endregion

        #endregion
    }
}
