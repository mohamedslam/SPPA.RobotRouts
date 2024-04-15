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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;


namespace Stimulsoft.Report.CodeDom
{
    internal class StiCodeDomTotalsFunctionsParser
	{
		private static bool ScanString(ref string argument, string text, ref int position)
		{
			argument += text[position++];
			while (position != text.Length && text[position] != '"')
			{
				argument += text[position++];
			}			
			if (position == text.Length)return false;
			
			argument += text[position++];
			return true;
		}
	
		private static string GetArgument(ref int position, string text)
		{
			string argument = string.Empty;
			int level = 0;
			while (position < text.Length && (text[position] != ',' || level > 0))
			{
				char sym = text[position];
				if (sym == '(')level++;
				else if (sym == ')')
				{
					if (level == 0)break;
					level--;
				}
				else if (sym == '\"')
				{
					bool result = ScanString(ref argument, text, ref position);
					if (!result)return null;
					continue;
				}
				argument += sym;
				position++;
			}
			if (position == text.Length)return null;
			return argument;
		}

        public static string ProcessTotals(StiCodeDomSerializator serializator, string script, string parent)
		{
			try
			{
                int startIndex = script.IndexOf("Totals.", StringComparison.InvariantCulture);
				if (startIndex >= 0)
				{
                    if (startIndex > 0 && (Char.IsLetterOrDigit(script[startIndex - 1]) || script[startIndex - 1] == '_')) return script;

					bool needMore = true;

					while (needMore)
					{
                        if ((startIndex > 1) && (script[startIndex - 1] == '.') && !(script.Substring(0, startIndex).EndsWith("Stimulsoft.Report.")))
                        {
                            startIndex = script.IndexOf("Totals.", startIndex + 1, StringComparison.InvariantCulture);
                            if (startIndex == -1) break;
                            continue;
                        }

						int index = startIndex;

                        startIndex = script.IndexOf("(", startIndex, StringComparison.InvariantCulture);
                        
                        if (startIndex == -1) return script;
                        int endIndexArgument = index + 7;

                        string functionName = script.Substring(endIndexArgument, startIndex - index - 7);
                        if (!functionName.Contains("Count") || functionName.Contains("CountDistinct"))
                        {
                            #region Ищем отсутствие аргумента
                            int endIndex = startIndex + 1;
                            while (endIndex < script.Length && char.IsWhiteSpace(script, endIndex))
                            {
                                endIndex++;
                            }
                            if (endIndex >= script.Length) return script;
                            if (script[endIndex] == ')')//Arguments is absent
                            {
                                return script;
                            }
                            #endregion

                            startIndex++;

                            int storedStartIndex = startIndex;

                            string argument = GetArgument(ref startIndex, script);
                            if (argument == null) return script;

                            //fix, if only one argument specified, to prevent compilation error
                            if ((startIndex < script.Length) && (script[startIndex] == ')') && (functionName != "CountAllLevels"))
                            {
                                string stNull = null;
                                StiComponent parentComponent = serializator.report.GetComponentByName(parent);
                                if (parentComponent != null)
                                {
                                    StiComponent aggregateComponent = parentComponent.GetGroupHeaderBand();
                                    if (aggregateComponent != null)
                                        stNull = aggregateComponent.Name;
                                }

                                if (stNull == null)
                                {
                                    stNull = 
                                        serializator.report.ScriptLanguage == StiReportLanguageType.CSharp ||
                                        serializator.report.ScriptLanguage == StiReportLanguageType.JS 
                                        ? "null" : "nothing";
                                }

                                script = script.Insert(storedStartIndex, stNull + ",");
                                startIndex = startIndex - argument.Length + stNull.Length;
                            }

                            int startIndexArgument = ++startIndex;

                            argument = GetArgument(ref startIndex, script);

                            if (argument == null || argument.Trim().Length == 0) 
                                return script;

                            endIndexArgument = startIndex;

                            string methodName = $"GetTotal{serializator.TotalIndex}";
                            serializator.TotalIndex++;

                            serializator.GenEventMethod(methodName, $"e.Value = {argument}", new StiGetArgumentEvent(), parent);

                            switch (serializator.report.ScriptLanguage)
                            {
                                case StiReportLanguageType.CSharp:
                                case StiReportLanguageType.JS:
                                    argument = " this, ";
                                    break;

                                default:
                                    argument = " Me, ";
                                    break;
                            }

                            argument += string.Format("\"{0}\"", methodName);

                            script = script.Remove(startIndexArgument, endIndexArgument - startIndexArgument);
                            script = script.Insert(startIndexArgument, argument);
                            int length = (endIndexArgument - startIndexArgument) - argument.Length;
                            endIndexArgument -= length;

                            if (functionName.EndsWith("SumDistinct"))
                            {
                                startIndex = endIndexArgument + 1;
                                startIndexArgument = startIndex;
                                argument = GetArgument(ref startIndex, script);
                                if (!string.IsNullOrWhiteSpace(argument))
                                {
                                    #region SumDistinct
                                    endIndexArgument = startIndex;

                                    methodName = "GetTotal" + serializator.TotalIndex.ToString();
                                    serializator.TotalIndex++;

                                    serializator.GenEventMethod(methodName, "e.Value = " + argument, new StiGetArgumentEvent(), parent);

                                    argument = string.Format("\"{0}\"", methodName);

                                    script = script.Remove(startIndexArgument, endIndexArgument - startIndexArgument);
                                    script = script.Insert(startIndexArgument, argument);
                                    length = (endIndexArgument - startIndexArgument) - argument.Length;
                                    endIndexArgument -= length;
                                    #endregion
                                }
                            }
                        }
                        ////

                        startIndex = script.IndexOf("Totals.", endIndexArgument, StringComparison.InvariantCulture);
                        
						if (startIndex == -1)break;
                        needMore = script.IndexOf("Totals.", startIndex, StringComparison.InvariantCulture) > 0;
					}
					return script;
				}

				return script;
			}
			catch
			{
			}
			return script;
		}
	}
}
