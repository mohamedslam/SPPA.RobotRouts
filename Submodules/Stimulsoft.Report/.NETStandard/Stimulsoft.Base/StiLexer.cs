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
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Stimulsoft.Base
{
    /// <summary>
	/// Performs the lexical analysis.
	/// </summary>
	public sealed class StiLexer
    {
        #region Properties
        private StringBuilder text;
        /// <summary>
        /// Gets or sets text for analys.
        /// </summary>
        public StringBuilder Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                BaseText = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets text for analys.
        /// </summary>
        internal string BaseText { get; set; }

        /// <summary>
        /// Start positions of token.
        /// </summary>
        private List<int> positions = new List<int>();

        /// <summary>
        /// Gets or sets current position in text.
        /// </summary>
        public int PositionInText { get; set; }

        public bool IsVB { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Saves position of token in text.
        /// </summary>
        public void SavePosToken()
        {
            positions.Add(PositionInText);
        }

        /// <summary>
        /// Gets position of token in text.
        /// </summary>
        /// <param name="positionInText">Position in text.</param>
        /// <returns>Position of token in text.</returns>
        public StiPosition GetPosition(int positionInText)
        {
            var pos = new StiPosition(1, 1);

            for (int ps = 0; ps < positionInText; ps++)
            {
                pos.Column++;
                if (Text[ps] == '\n')
                {
                    pos.Line++;
                    pos.Column = 1;
                }
            }
            return pos;
        }

        /// <summary>
        /// Skips all not control symbols.
        /// </summary>
        public void Skip()
        {
            while (
                PositionInText < Text.Length &&
                (Char.IsWhiteSpace(Text[PositionInText]) ||
                Char.IsControl(Text[PositionInText])))
            {
                PositionInText++;
            }
        }

        /// <summary>
        /// Wait the left paren.
        /// </summary>
        public bool WaitLparen2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.LPar;
        }

        /// <summary>
        /// Wait the right bracket.
        /// </summary>
        public bool WaitComma2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.Comma;
        }

        /// <summary>
        /// Wait the assign.
        /// </summary>
        public bool WaitAssign2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.Assign;
        }

        /// <summary>
        /// Wait the right paren.
        /// </summary>
        public bool WaitRparen2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.RPar;
        }

        /// <summary>
        /// Wait the left brace.
        /// </summary>
        public bool WaitLbrace2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.LBrace;
        }

        /// <summary>
        /// Wait the semicolon.
        /// </summary>
        public bool WaitSemicolon2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.SemiColon;
        }

        /// <summary>
        /// Wait the right brace.
        /// </summary>
        public bool WaitRbrace2()
        {
            StiToken token = GetToken();
            return token.Type == StiTokenType.RBrace;
        }

        /// <summary>
        /// Scans the number.
        /// </summary>
        /// <returns>Token containing number.</returns>
        public StiToken ScanNumber()
        {
            int posStart = PositionInText;

            bool isFloat = false;

            while (PositionInText != Text.Length && Char.IsDigit(Text[PositionInText]))
            {
                PositionInText++;
            }

            if (PositionInText != Text.Length && Text[PositionInText] == '.' &&
                PositionInText + 1 != Text.Length && Char.IsDigit(Text[PositionInText + 1]))
            {
                PositionInText++;
                while (PositionInText != Text.Length && Char.IsDigit(Text[PositionInText]))
                {
                    PositionInText++;
                }
                isFloat = true;
            }

            string nm = BaseText.Substring(posStart, PositionInText - posStart);
            if (isFloat)
            {
                var nfi = new NumberFormatInfo
                {
                    CurrencyDecimalSeparator = "."
                };

                return new StiToken(StiTokenType.Value, posStart, PositionInText - posStart,
                    double.Parse(nm, nfi));
            }
            else
            {
                string valueStr = nm;
                try
                {
                    if (valueStr.Length > 19)   //ulong max is 9,223,372,036,854,775,807
                        return new StiToken(StiTokenType.Value, posStart, PositionInText - posStart, valueStr);

                    ulong value = ulong.Parse(valueStr);
                    return new StiToken(StiTokenType.Value, posStart, PositionInText - posStart,
                        value);
                }
                catch (Exception e)
                {
                    if (e is OverflowException || e is FormatException)
                    {
                        return new StiToken(StiTokenType.Value, posStart, PositionInText - posStart, valueStr);
                    }
                    else throw;
                }
            }
        }

        /// <summary>
        /// Scans the identifier.
        /// </summary>
        /// <returns>Token containing identifier.</returns>
        public StiToken ScanIdent()
        {
            int startIndex = PositionInText;
            string ident = string.Empty;
            while (PositionInText != Text.Length &&
                (Char.IsLetterOrDigit(Text[PositionInText]) ||
                Text[PositionInText] == '_' ||
                Text[PositionInText] == '№'))
            {
                ident += Text[PositionInText++];
            }

            return new StiToken(StiTokenType.Ident, startIndex, PositionInText - startIndex, ident);
        }

        /// <summary>
        /// Scans the string.
        /// </summary>
        /// <returns>Token containing string.</returns>
        public StiToken ScanString()
        {
            int startIndex = PositionInText;
            PositionInText++;
            StringBuilder str = new StringBuilder();
            while (PositionInText != Text.Length && Text[PositionInText] != '"')
            {
                char ch = Text[PositionInText++];
                str.Append(ch);
                if (!IsVB && (ch == '\\') && (PositionInText != Text.Length)) str.Append(Text[PositionInText++]);
            }

            if (PositionInText == Text.Length)
                new StiToken(StiTokenType.Value, startIndex, PositionInText - startIndex, str.ToString());

            PositionInText++;

            return new StiToken(StiTokenType.Value, startIndex, PositionInText - startIndex, str.ToString());
        }


        /// <summary>
        /// Scans the symbol.
        /// </summary>
        /// <returns>Token containing symbol.</returns>
        public StiToken ScanChar()
        {
            if (++PositionInText == Text.Length)
                return new StiToken(StiTokenType.Value, PositionInText - 3, 3, ' ');

            char c = Text[PositionInText++];

            if (PositionInText == Text.Length || Text[PositionInText] != '\'')
                return new StiToken(StiTokenType.Value, PositionInText - 3, 3, c);

            PositionInText++;

            return new StiToken(StiTokenType.Value, PositionInText - 3, 3, c);
        }

        /// <summary>
        /// Returns to previous token.
        /// </summary>
        public void UngetToken()
        {
            PositionInText = positions[positions.Count - 1];
            positions.RemoveAt(positions.Count - 1);
        }

        /// <summary>
        /// Gets next token.
        /// </summary>
        /// <returns>Next token.</returns>
        public StiToken GetToken()
        {
            Skip();

            #region End of the text
            if (Text.Length <= PositionInText)
            {
                return new StiToken(StiTokenType.EOF, PositionInText, 0);
            }
            #endregion

            #region Ident
            else if (Char.IsLetter(Text[PositionInText]) || Text[PositionInText] == '_' || Text[PositionInText] == '№')
            {
                int startIndex = PositionInText;
                SavePosToken();
                StiToken token = ScanIdent();
                switch ((string)token.Data)
                {
                    case "true": return new StiToken(StiTokenType.Value, startIndex, 4, true);
                    case "false": return new StiToken(StiTokenType.Value, startIndex, 5, false);
                }
                return token;
            }
            #endregion

            #region Number
            else if (Char.IsDigit(Text[PositionInText]))
            {
                SavePosToken();
                return ScanNumber();
            }
            #endregion

            #region String
            else if (Text[PositionInText] == '"')
            {
                SavePosToken();
                return ScanString();
            }
            #endregion

            #region Char
            else if (Text[PositionInText] == '\'')
            {
                SavePosToken();
                return ScanChar();
            }
            #endregion

            else
            {
                #region Operators
                switch (Text[PositionInText])
                {
                    case '€': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Euro, PositionInText - 1, 1);
                    case '®': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Copyright, PositionInText - 1, 1);
                    case '(': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.LPar, PositionInText - 1, 1);
                    case ')': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.RPar, PositionInText - 1, 1);
                    case '{': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.LBrace, PositionInText - 1, 1);
                    case '}': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.RBrace, PositionInText - 1, 1);
                    case ',': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Comma, PositionInText - 1, 1);
                    case '.': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Dot, PositionInText - 1, 1);
                    case ';': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.SemiColon, PositionInText - 1, 1);
                    case ':': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Colon, PositionInText - 1, 1);
                    case '!': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Minus, PositionInText - 1, 1);
                    case '*': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Mult, PositionInText - 1, 1);
                    case '^': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Not, PositionInText - 1, 1);
                    case '/': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Div, PositionInText - 1, 1);
                    case '\\': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Splash, PositionInText - 1, 1);
                    case '%': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Percent, PositionInText - 1, 1);
                    case '#': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Sharp, PositionInText - 1, 1);
                    case '$': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Dollar, PositionInText - 1, 1);
                    case '@': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Ampersand, PositionInText - 1, 1);
                    case '[': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.LBracket, PositionInText - 1, 1);
                    case ']': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.RBracket, PositionInText - 1, 1);
                    case '?': SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Question, PositionInText - 1, 1);

                    #region Symbol "|"
                    case '|':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '|')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.DoubleOr, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Or, PositionInText - 1, 1);
                    #endregion

                    #region Symbol "&"
                    case '&':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '&')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.DoubleAnd, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.And, PositionInText - 1, 1);
                    #endregion

                    #region Symbol "+"
                    case '+':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '+')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.DoublePlus, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Plus, PositionInText - 1, 1);
                    #endregion

                    #region Symbol "-"
                    case '-':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '-')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.DoubleMinus, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Minus, PositionInText - 1, 1);
                    #endregion

                    #region Symbol "="
                    case '=':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '=')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.Equal, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Assign, PositionInText - 1, 1);
                    #endregion

                    #region Symbol "<"
                    case '<':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '=')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.LeftEqual, PositionInText - 2, 2);
                        }
                        if (PositionInText != Text.Length && Text[PositionInText] == '<')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.Shl, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Left, PositionInText - 1, 1);
                    #endregion

                    #region Symbol ">"
                    case '>':
                        SavePosToken();
                        PositionInText++;
                        if (PositionInText != Text.Length && Text[PositionInText] == '=')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.RightEqual, PositionInText - 2, 2);
                        }
                        if (PositionInText != Text.Length && Text[PositionInText] == '>')
                        {
                            PositionInText++;
                            return new StiToken(StiTokenType.Shr, PositionInText - 2, 2);
                        }
                        return new StiToken(StiTokenType.Right, PositionInText - 1, 1);
                    #endregion

                    default:
                        {
                            SavePosToken(); PositionInText++; return new StiToken(StiTokenType.Unknown, PositionInText - 1, 1);
                        }
                }
                #endregion
            }
        }

        /// <summary>
        /// Reset state.
        /// </summary>
        public void Reset()
        {
            positions.Clear();
            PositionInText = 0;
        }

        /// <summary>
        /// Replaces all occurrences of a specified String, with another specified String.
        /// Before oldValue must follow specified prefix - token.
        /// Replacing is produced with provision for tokens.
        /// </summary>
        /// <param name="textValue">Text for replace.</param>
        /// <param name="prefix">Prefix - token.</param>
        /// <param name="oldValue">A String to be replaced.</param>
        /// <param name="newValue">A String to replace all occurrences of oldValue.</param>
        /// <returns>Replaced string.</returns>
        public static string ReplaceWithPrefix(string textValue, string prefix, string oldValue, string newValue)
        {
            StringBuilder sb = new StringBuilder(textValue);
            StiLexer lexer = new StiLexer(textValue);

            StiToken prefixToken = lexer.GetToken();
            if (prefixToken.Type == StiTokenType.EOF) return textValue;

            StiToken token = null;
            do
            {
                token = lexer.GetToken();
                if (token.Type == StiTokenType.Ident &&
                    prefixToken.Type == StiTokenType.Ident &&
                    ((string)prefixToken.Data) == prefix &&
                    ((string)token.Data) == oldValue)
                {
                    sb = sb.Replace(oldValue, newValue, token.Index, token.Length);
                    lexer.PositionInText += newValue.Length;
                }
                prefixToken = token;

            }
            while (token.Type != StiTokenType.EOF);

            return sb.ToString();
        }

        /// <summary>
        /// Replaces all occurrences of a specified String, with another specified string.
        /// Before oldValue must follow specified prefix - string.
        /// Replacing is produced with provision for tokens.
        /// </summary>
        /// <param name="prefix">Prefix - string.</param>
        /// <param name="oldValue">A String to be replaced.</param>
        /// <param name="newValue">A String to replace all occurrences of oldValue.</param>
        /// <returns>Replaced string.</returns>
        public void ReplaceWithPrefix(string prefix, string oldValue, string newValue)
        {
            this.Reset();

            StiToken prefixToken = this.GetToken();
            if (prefixToken.Type == StiTokenType.EOF) return;
            StiToken token = null;
            do
            {
                token = this.GetToken();
                if (token.Type == StiTokenType.Ident &&
                    prefixToken.Type == StiTokenType.Ident &&
                    ((string)prefixToken.Data) == prefix &&
                    ((string)token.Data) == oldValue)
                {
                    text = text.Replace(oldValue, newValue, token.Index, token.Length);
                    this.PositionInText += newValue.Length;
                }
                prefixToken = token;

            }
            while (token.Type != StiTokenType.EOF);

            BaseText = text.ToString();
        }

        /// <summary>
        /// Replaces all occurrences of a specified String, with another specified string.
        /// Before oldValue must not follow specified prefix - string.
        /// </summary>
        /// <param name="prefix">Prefix - string.</param>
        /// <param name="oldValue">A String to be replaced.</param>
        /// <param name="newValue">A String to replace all occurrences of oldValue.</param>
        /// <returns>Replaced string.</returns>
        public void ReplaceWithNotEqualPrefix(StiTokenType prefix, string oldValue, string newValue)
        {
            this.Reset();

            StiToken prefixToken = this.GetToken();
            if (prefixToken.Type == StiTokenType.EOF) return;
            StiToken token = null;
            do
            {
                token = this.GetToken();
                if (token.Type == StiTokenType.Ident &&
                    prefixToken.Type != prefix &&
                    ((string)token.Data) == oldValue)
                {
                    text = text.Replace(oldValue, newValue, token.Index, token.Length);
                    this.PositionInText += newValue.Length;
                }
                prefixToken = token;
            }
            while (token.Type != StiTokenType.EOF);

            BaseText = text.ToString();
        }

        public static bool IdentExists(string str, string name, bool caseSensitive)
        {
            var lexer = new StiLexer(str);
            while (true)
            {
                var token = lexer.GetToken();
                if (token == null || token.Type == StiTokenType.EOF) return false;
                if (token.Type == StiTokenType.Ident && token.Data != null)
                {
                    if (caseSensitive && token.Data.ToString() == name) return true;
                    if (!caseSensitive && token.Data.ToString().ToLowerInvariant() == name.ToLowerInvariant()) return true;
                }
            }
        }

        public static List<StiToken> GetAllTokens(string str)
        {
            var tokens = new List<StiToken>();
            var lexer = new StiLexer(str);

            while (true)
            {
                var token = lexer.GetToken();
                if (token == null || token.Type == StiTokenType.EOF)
                    return tokens;

                tokens.Add(token);
            }
        }

        /// <summary>
        /// Creates a new instance of the StiLexer class.
        /// </summary>
        /// <param name="textValue">The Text for lexical analysis.</param>
        public StiLexer(string textValue)
        {
            this.BaseText = textValue;
            this.text = new StringBuilder(textValue);
            PositionInText = 0;
        }
        #endregion
    }
}
