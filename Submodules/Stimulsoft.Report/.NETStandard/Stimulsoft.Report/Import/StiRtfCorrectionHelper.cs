#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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
using System.IO;
using System.Text;

namespace Stimulsoft.Report.Import
{
	/// <summary>
	/// Summary description for StiRtfCorrectionHelper.
	/// </summary>
	public class StiRtfCorrectionHelper
    {
        #region enum StiRtfTokenType
	    public enum StiRtfTokenType
	    {
		    None = 0,
		    Keyword = 1,
		    Control = 2,
		    Text = 3,
		    Eof = 4,
		    GroupStart = 5,
		    GroupEnd = 6
	    }
	    #endregion

        #region class StiRtfToken
        /// <summary>
	    /// Summary description for StiRtfToken.
	    /// </summary>
	    public class StiRtfToken
        {
            #region Properties
            public StiRtfTokenType Type { get; set; }

	        public string Key { get; set; }

	        public bool HasParameter { get; set; }

	        public int Parameter { get; set; }

            public int Position { get; set; }
            #endregion

	        public override string ToString()
	        {
	            return $"{Type} {Key} {(HasParameter ? Parameter.ToString() : string.Empty)}";
	        }

	        public StiRtfToken()
		    {
			    Type = StiRtfTokenType.None;
			    Key = string.Empty;
		    }
	    }
        #endregion

        #region Fields
        private TextReader rtf;
		private const int Eof = -1;
	    private int position = 0;
        #endregion

        #region Methods
        public StiRtfToken NextToken(bool addText = true)
		{
		    var token = new StiRtfToken();

			var c = rtf.Read();
            position++;
            while (c == '\r' || c == '\n' || c == '\t' || c == '\0')
            {
                c = rtf.Read();
                position++;
            }

            token.Position = position - 1;

		    if (c != Eof)
		    {
		        switch (c)
		        {
		            case '{':
		                token.Type = StiRtfTokenType.GroupStart;
		                break;

		            case '}':
		                token.Type = StiRtfTokenType.GroupEnd;
		                break;

		            case '\\':
		                ParseKeyword(token);
		                break;

		            default:
		                token.Type = StiRtfTokenType.Text;
		                ParseText((char) c, token, addText);
		                break;
		        }
		    }

		    else
		        token.Type = StiRtfTokenType.Eof;

		    return token;
        }

        private void ParseKeyword(StiRtfToken token)
		{
			var c = rtf.Peek();

            #region Special character or control symbol
            if (!char.IsLetter((char)c))
            {
                rtf.Read();
                position++;
                if (c == '\\' || c == '{' || c == '}')
                {
                    //Special character
                    token.Type = StiRtfTokenType.Text;
                    token.Key = ((char)c).ToString();
                }
                else
                {
                    //Control symbol
                    token.Type = StiRtfTokenType.Control;
                    token.Key = ((char)c).ToString();
                    if (token.Key == "\'")
                    {
                        var code = $"{(char) rtf.Read()}{(char) rtf.Read()}";
                        position += 2;
                        token.HasParameter = true;
                        token.Parameter = Convert.ToInt32(code, 16);
                    }
                }
                return;
            }
            #endregion

            #region Keyword
            var keyWord = new StringBuilder();
            c = rtf.Peek();
			while (char.IsLetter((char)c))
			{
				rtf.Read();
                position++;
                keyWord.Append((char)c);
				c = rtf.Peek();
			}
			token.Type = StiRtfTokenType.Keyword;
            token.Key = keyWord.ToString();
            #endregion

            #region Parameter
            if (char.IsDigit((char)c) || c == '-')
			{
				token.HasParameter = true;

                var negative = false;
                if (c == '-')
				{
					negative = true;
					rtf.Read();
                    position++;
				}

			    var paramStr = new StringBuilder();
                c = rtf.Peek();
				while (char.IsDigit((char)c))
				{
					rtf.Read();
                    position++;
					paramStr.Append((char)c);
					c = rtf.Peek();
				}

			    var paramInt = Convert.ToInt32(paramStr.ToString());
			    if (negative)
			        paramInt = -paramInt;

			    token.Parameter = paramInt;
            }
            #endregion

            if (c == ' ')
			{
				rtf.Read();
                position++;
			}
        }

        private void ParseText(char ch, StiRtfToken token, bool addText)
		{
			var text = new StringBuilder(ch.ToString());

			var c = rtf.Peek();
			while (c == '\r' || c == '\n' || c == '\t' || c == '\0')
			{
				rtf.Read();
                position++;
				c = rtf.Peek();
			}

			while (c != '\\' && c != '}' && c != '{' && c != Eof)
			{
				rtf.Read();
                position++;
				if (addText) text.Append((char)c);
				c = rtf.Peek();
				while (c == '\r' || c == '\n' || c == '\t' || c == '\0')
				{
					rtf.Read();
                    position++;
					c = rtf.Peek();
				}
			}

            if (addText) token.Key = text.ToString();
        }
        #endregion

        public StiRtfCorrectionHelper(TextReader rtfReader)
		{
			rtf = rtfReader;
        }
    }
}
