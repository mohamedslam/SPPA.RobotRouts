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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Stimulsoft.Report.Components;
using System.Globalization;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Describes class which contains static methods which helps parse expressions.
	/// </summary>
	public class StiCodeDomExpressionHelper
	{	
		#region Methods
        private static StringBuilder ParseRtf(StringBuilder str, StiRichText richText)
		{
			if (richText != null && (!richText.FullConvertExpression))
			{
                string strText = str.ToString();
				bool whiteSpace = false;

                int startIndex = strText.LastIndexOf("__LP__", StringComparison.InvariantCulture);
				if (startIndex == -1)startIndex = 0;

				for (int index = str.Length - 1; index > startIndex; index--)
				{
					if (str[index] == ' ')
					{
						whiteSpace = true;
						break;
					}
					if (str[index] == '\\')
					{
						break;
					}
				}

                if (!whiteSpace)
                {
                    var sb = new StringBuilder();
                    sb.Append('"');
                    sb.Append(str);
                    sb.Append(' ');
                    sb.Append('"');
                    return sb;
                }
			}
            if (str.Length > 0)
            {
                var sb = new StringBuilder();
                sb.Append('"');
                sb.Append(str);
                sb.Append('"');
                return sb;
            }
            return new StringBuilder();
		}		

		public static void ReadString(StiCodeGenerator codeGenerator, ref int pos, ref StringBuilder lexem, string script, ref List<string> al, bool isRichText, bool fullRtf)
		{
			lexem = lexem.Append(script[pos++]);
			var sb = new StringBuilder();

			while (pos != script.Length && script[pos] != '"')
			{
                if (script[pos] == '\\' && (pos + 1) != script.Length)
                {
                    if (script[pos + 1] == '\\')
                    {
                        sb.Append("\\\\");
                        pos += 2;
                        continue;
                    }
                    if (script[pos + 1] == '"')
                    {
                        pos++;
                    }
                }
				sb.Append(script[pos++]);
			}
			string str;
			if (codeGenerator != null)
			{
                sb = ReplaceBackslash(sb, isRichText, fullRtf);

				str = codeGenerator.QuoteSnippetString(sb.ToString());
			}
			else str = sb.ToString();

            if (str.StartsWith("\"", StringComparison.InvariantCulture) && str.EndsWith("\"", StringComparison.InvariantCulture) && str.Length > 1)
                lexem = lexem.Append(str.Substring(1, str.Length - 2));
            else
                lexem = lexem.Append(str);

			if (pos != script.Length)
			{
				lexem = lexem.Append(script[pos]);
				pos++;
			}
						
			if (pos == script.Length)
			{				
				if (lexem.Length > 0) al.Add(lexem.ToString());
                lexem = new StringBuilder();
			}
			pos --;
		}


		private static void ReadChar(ref int pos, ref StringBuilder lexem, string script)
		{
			lexem = lexem.Append(script[pos++]);
			if (pos != script.Length)lexem = lexem.Append(script[pos++]);
            if (pos != script.Length) lexem = lexem.Append(script[pos]);
		}


        private static void AddLexem(int pos, int startPos, StiRichText richText, RichTextBox rich, string script, List<string> al, StiCodeGenerator codeGenerator)
		{
			if (pos > startPos)
			{
				string str;
				if (richText != null)
				{
					rich.SelectionStart = startPos;
					rich.SelectionLength = pos - startPos;
					str = rich.SelectedRtf;
				}
				else str = script.Substring(startPos, pos - startPos);
				if (codeGenerator != null)str = codeGenerator.QuoteSnippetString(str);
				al.Add(str);
			}
		}


        public static List<string> GetLexemSimple(StiCodeGenerator codeGenerator, string script, StiRichText richText)
		{
            var al = new List<string>();

            var lexem = new StringBuilder();

			var code = false;

			int pos = 0;
			while (pos < script.Length)
			{
				if (code == false)
				{
					if (script[pos] == '{')
					{
						code = true;
						lexem = ParseRtf(lexem, richText);
						if (lexem.Length > 0)
						{
							string str = lexem.ToString().Substring(1, lexem.Length - 2);
							if (codeGenerator != null)str = codeGenerator.QuoteSnippetString(str);
							al.Add(str);
						}
                        lexem = new StringBuilder();
					}
					else lexem = lexem.Append(script[pos]);
				}
				else
				{
					if (script[pos] =='"')
					{
                        //if ((codeGenerator is StiCSharpCodeGenerator) && (richText != null)) lexem.Append("@");
                        ReadString(codeGenerator, ref pos, ref lexem, script, ref al, richText != null, false);
					}
					else if (script[pos] =='\'')
					{
						ReadChar(ref pos, ref lexem, script);
					}
					else if (script[pos] =='}')
					{
						code = false;
                        if (codeGenerator == null)
                        {
                            string str = lexem.ToString();
                            lexem = new StringBuilder();
                            lexem = lexem.Append("{");
                            lexem = lexem.Append(str);
                            lexem = lexem.Append("}");
                        }
						if (lexem.Length > 0)al.Add(lexem.ToString());
                        lexem = new StringBuilder();
					}
					else lexem = lexem.Append(script[pos]);
				}
				pos++;
			}

			if (code == false)
			{
				lexem = ParseRtf(lexem, richText);
				if (lexem.Length > 0)
				{
					string str = lexem.ToString().Substring(1, lexem.Length - 2);
					if (codeGenerator != null)str = codeGenerator.QuoteSnippetString(str);
					al.Add(str);
				}
			}
			else 
			{
				al.Clear();
			}

			return al;
		}

		
		/// <summary>
		/// Breaks the text value to the script and text.
		/// </summary>
		/// <returns>Array of lexems.</returns>
        public static List<string> GetLexemFullRtf(StiCodeGenerator codeGenerator, string script, StiRichText richText)
		{
            var al = new List<string>();

			RichTextBox rich = null;
			if (richText != null)
			{
				rich = new Controls.StiRichTextBox(false);
				rich.Rtf = StiRichText.UnpackRtf(script);
				script = rich.Text;
			}

			var lexem = new StringBuilder();

			var code = false;

			int pos = 0;
			int startPos = 0;
			while (pos != script.Length)
			{
				#region Text
				if (!code)
				{
					if (script[pos] == '{')
					{
						code = true;
						AddLexem(pos, startPos, richText, rich, script, al, codeGenerator);
						lexem = new StringBuilder();
						startPos = pos;

						if (rich != null)
						{							
							rich.SelectionStart = startPos;
							rich.SelectionLength = 1;

							string text = rich.SelectedText;

                            if (codeGenerator != null)
							    al.Add(codeGenerator.QuoteSnippetString(rich.SelectedRtf));
                            else
                                al.Add(rich.SelectedRtf);
						}
					}
				}
				#endregion
				
				#region Expressions
				else
				{
					if (script[pos] =='"')
					{
                        ReadString(codeGenerator, ref pos, ref lexem, script, ref al, richText != null, true);
					}
					else if (script[pos] =='\'')
					{
						ReadChar(ref pos, ref lexem, script);
					}
					else if (script[pos] =='}')
					{
						code = false;
						startPos = pos + 1;
                        if (codeGenerator == null)
                        {
                            string str = lexem.ToString();
                            lexem = new StringBuilder();
                            lexem = lexem.Append("{");
                            lexem = lexem.Append(str);
                            lexem = lexem.Append("}");
                        }
						if (lexem.Length > 0)al.Add(lexem.ToString());
						lexem = new StringBuilder();
					}
					else lexem = lexem.Append(script[pos]);
				}
				#endregion

				pos++;
			}

			if (code == false)
			{
				AddLexem(pos, startPos, richText, rich, script, al, codeGenerator);
			}
			else 
			{
				al.Clear();
			}

            //fix - add "\par" command for correct alignment of the last line
            if (al.Count > 0)
            {
                string st = al[al.Count - 1];
                string lastPart = st.Length > 30 ? st.Substring(st.Length - 30) : st;
                int lastPos = st.LastIndexOf('}');
                if ((lastPos != -1) && st.StartsWith("\"{\\") && (lastPart.IndexOf("\\\\par") == -1))
                {
                    st = st.Insert(lastPos, "\\\\par");
                    al[al.Count - 1] = st;
                }
            }

			if (rich != null)rich.Dispose();

			return al;
		}


        public static List<string> GetLexem(string script)
		{
			return GetLexemSimple(null, script, null);
		}


	    /// <summary>
	    /// Returns the text value as the script string.
	    /// </summary>
	    /// <param name="codeGenerator">Code generator.</param>
	    /// <param name="script">Script.</param>
	    /// <param name="fullConvert">If true then the value to be returned is the always string.</param>
	    /// <param name="onlyText">Process as simple text, without expressions.</param>
	    /// <param name="component">Component.</param>
	    /// <param name="isTextExpression">Expression is text expression.</param>
	    /// <returns>Script string.</returns>
	    public static string ConvertToString(StiCodeGenerator codeGenerator, string script, 
			bool fullConvert, bool onlyText, StiComponent component, bool isTextExpression)
		{
            var richText = component as StiRichText;
			if (richText != null) script = script.Replace((char)0, ' ');
			if (onlyText)return codeGenerator.QuoteSnippetString(script);

			try
			{
				bool first = true;
                List<string> al;
				if (richText != null && richText.FullConvertExpression && isTextExpression) al = GetLexemFullRtf(codeGenerator, script, richText);
				else al = GetLexemSimple(codeGenerator, script, richText);
				var res = new StringBuilder(script.Length * 2);
			
				if (al.Count == 0)return codeGenerator.QuoteSnippetString(script);
				foreach (string str in al)
				{
					if (!first)
					{
						if (richText != null && richText.FullConvertExpression)res = res.Append(" , ");
						else res = res.Append(" + ");
					}
                    if (str[0] == '"') res = res.Append(str);
					else 
					{
                        if (fullConvert)
                        {
                            #region Fill ExcelValue of text component if ExcelValue expression is empty
                            var text = component as StiText;
                            if (text == null || !string.IsNullOrEmpty(text.ExcelValue.Value))
                                res = res.Append("ToString(" + str + ")");
                            #endregion
                            else
                            {
                                res = res.Append(string.Format("ToString(sender, {0}, {1})", str, isTextExpression ? "true" : "false"));
                            }
                        }
                        else res = res.Append(str);
					}
					first = false;
				}
		
				if (richText != null)
				{
					string str2 = res.Replace('\0', ' ').ToString();

					if (!richText.FullConvertExpression)return str2;
					return string.Format("ConvertRtf({0})", str2);
				}
				return res.ToString();
			}
			catch (Exception ex)
			{
				StiLogService.Write(typeof(StiExpression), "ConvertToString...ERROR");
				StiLogService.Write(typeof(StiExpression), ex.Message);
				return null;
			}
		}

        private static StringBuilder ReplaceBackslash(StringBuilder input, bool isRichText, bool fullRtf)
        {
            var output = new StringBuilder();
            for (int index = 0; index < input.Length; index++)
            {
                if ((input[index] == '\\') && (index < input.Length - 1))
                {
                    index++;
                    char ch = input[index];
                    switch (ch)
                    {
                        case '\\':
                            if (isRichText && !fullRtf && (index + 1 < input.Length) && (input[index + 1] == 'n'))
                            {
                                output.Append("\n");
                                index++;
                            }
                            else
                            {
                                output.Append("\\");
                            }
                            break;

                        case '\'':
                            if (isRichText)
                            {
                                output.Append("\\\'");
                            }
                            else
                            {
                                output.Append('\'');
                            }
                            break;

                        case '0':
                            output.Append('\0');
                            break;
                        
                        case 'n':
                            output.Append('\n');
                            break;
                        
                        case 'r':
                            output.Append('\r');
                            break;
                        
                        case 't':
                            output.Append('\t');
                            break;

                        case 'x':
                            StringBuilder sbHex = new StringBuilder();
                            int hexLen = 0;
                            while ((index < input.Length - 1) && (hexLen < 4) && ("0123456789abcdefABCDEF".IndexOf(input[index + 1]) != -1))
                            {
                                sbHex.Append(input[index + 1]);
                                index++;
                                hexLen++;
                            }
                            int resInt = 0;
                            bool resBool = int.TryParse(sbHex.ToString(), NumberStyles.HexNumber, null, out resInt);
                            output.Append((char)resInt);
                            break;

                        default:
                            output.Append("\\" + ch);
                            break;
                    }
                }
                else
                {
                    output.Append(input[index]);
                }
            }

            return output;
        }
		#endregion
	}
}
