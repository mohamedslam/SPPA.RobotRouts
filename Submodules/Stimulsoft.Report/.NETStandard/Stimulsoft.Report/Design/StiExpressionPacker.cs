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
using System.Text;
using System.Drawing;
using System.Collections;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base;
using System.Globalization;

namespace Stimulsoft.Report.Design
{
	public class StiExpressionPacker
	{
        #region class Word
		private class Word
		{
			public string First;
			public string Second;

			public Word(string first, string second)
			{
				this.First = first;
				this.Second = second;
			}
		}
        #endregion

        public static string PackExpression(string expressionStr, IStiDesignerBase designer, bool useBraces)
		{
			if (designer != null && designer.UseAliases)
                return PackExpression(expressionStr, designer.Report, useBraces);

			else
                return expressionStr;
		}

        public static string UnPackExpression(string expressionStr, IStiDesignerBase designer, bool useBraces)
		{
            if (designer != null && designer.UseAliases)
                return UnPackExpression(expressionStr, designer.Report, useBraces);

			else
                return expressionStr;
		}

		public static string PackExpression(string expressionStr, StiReport report, bool useBraces)
		{
			var hashFirst = new Hashtable();
			var hashSecond = new Hashtable();
			BuildDictionary(report, ref hashFirst, ref hashSecond, true);

			int index = 0;
			var lexer = new StiLexer(expressionStr);
			StiToken prevToken = null;
			StiToken prevPrevToken = null;
			var str = new StringBuilder(expressionStr);
			int bracesCount = 0;
			do
			{
				StiToken token = lexer.GetToken();
				if (token.Type == StiTokenType.EOF) break;
				else if (useBraces && token.Type == StiTokenType.LBrace) bracesCount++;
				else if (useBraces && token.Type == StiTokenType.RBrace) bracesCount--;
				else if (token.Type == StiTokenType.Ident && ((!useBraces) || (useBraces && bracesCount > 0)))
				{
					string ident = token.Data.ToString().Replace(" ", "").ToLowerInvariant();
					string lexem = null;
					if (prevToken != null && prevToken.Type == StiTokenType.Dot)
					{
						var objs = hashSecond[ident] as ArrayList;
						if (objs != null)
						{
                            foreach (object obj in objs)
                            {
                                var word = obj as Word;
                                if (word != null)
                                {
                                    if (prevPrevToken != null)
                                    {
                                        if (
                                            prevPrevToken.Type == StiTokenType.Ident &&
                                            prevPrevToken.Data.ToString().ToLowerInvariant() == word.First.ToLowerInvariant())
                                        {
                                            lexem = word.Second;
                                        }
                                    }
                                    else
                                    {
                                        lexem = word.Second;
                                    }
                                }
                            }
						}
					}
					else lexem = hashFirst[ident] as string;

					if (lexem != null)
					{
						int dist = lexem.Length - token.Length;

						#region Replace old lexem by new
						str = str.Remove(token.Index + index, token.Length);
						str = str.Insert(token.Index + index, lexem);
						#endregion

						index += dist;
					}
				}
				prevPrevToken = prevToken;
				prevToken = token;                
			}
			while (1 == 1);

			return str.ToString();
		}

		public static string UnPackExpression(string expressionStr, StiReport report, bool useBraces)
		{
			Hashtable hashFirst = new Hashtable();
			Hashtable hashSecond = new Hashtable();
			BuildDictionary(report, ref hashFirst, ref hashSecond, false);

			int index = 0;
			StiLexer lexer = new StiLexer(expressionStr);
			StiToken prevToken = null;
			StiToken prevPrevToken = null;
			StringBuilder str = new StringBuilder(expressionStr);
			int bracesCount = 0;
			string prevLexem = null;
			do
			{
				StiToken token = lexer.GetToken();
				if (token.Type == StiTokenType.EOF) break;
				else if (useBraces && token.Type == StiTokenType.LBrace) bracesCount++;
				else if (useBraces && token.Type == StiTokenType.RBrace) bracesCount--;
				else if (((!useBraces) || (useBraces && bracesCount > 0)) && 
					(token.Type == StiTokenType.LBracket || token.Type == StiTokenType.Ident))
				{
					int startIndex = -1;
					int endIndex = -1;
					string ident = null;

					if (token.Type == StiTokenType.LBracket)
					{						
						startIndex = token.Index + index;
						endIndex = startIndex;
						do
						{
							StiToken token2 = lexer.GetToken();
							if (token == null)token = token2;
							if (token2.Type == StiTokenType.EOF)
							{
								startIndex = -1;
								break;
							}
							else if (token2.Type == StiTokenType.RBracket)
								break;

							endIndex = token2.Index + token2.Length + index;

						}
						while (1 == 1);

						if (startIndex == -1) break;

						startIndex -= index;
						endIndex -= index;

						ident = expressionStr.Substring(startIndex, endIndex - startIndex + 1);
					}
					else
					{
						startIndex = token.Index;
						endIndex = token.Index + token.Length;
						ident = token.Data.ToString();
					}
                    
					int lengthOfLexem = ident.Length;

					ident = ident.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
					string lexem = null;
                    if (prevToken != null && prevToken.Type == StiTokenType.Dot)
                    {
                        ArrayList objs = hashSecond[ident] as ArrayList;
                        if (objs != null)
                        {
                            foreach (object obj in objs)
                            {
                                Word word = obj as Word;
                                if (word != null)
                                {
                                    if (prevPrevToken != null)
                                    {
                                        if (prevLexem != null &&
                                            prevLexem.ToLowerInvariant() == word.First.ToLowerInvariant())
                                        {
                                            lexem = word.Second;
                                        }
                                    }
                                    else
                                    {
                                        lexem = word.Second;
                                    }
                                }
                            }
                        }
                    }
                    else lexem = hashFirst[ident] as string;

					if (lexem != null)
					{
						int dist = lexem.Length - lengthOfLexem;

						#region Replace old lexem by new
						str = str.Remove(token.Index + index, lengthOfLexem);
						str = str.Insert(token.Index + index, lexem);
						#endregion

						index += dist;
					}
					prevLexem = lexem;
				}
				prevPrevToken = prevToken;
				prevToken = token;
			}
			while (true);

			return str.ToString();
		}

		private static bool IsValidName(string name)
		{
			if (string.IsNullOrEmpty(name) || !(char.IsLetter(name[0]) || name[0] == '_'))
				return false;

            foreach (var symbol in name)
            {
                if (!(char.IsLetterOrDigit(symbol) || symbol == '_'))
                    return false;
            }

            return true;
		}

		public static string GetCorrectedAlias(StiReport report, string alias)
        {
            return IsValidName(alias) ? alias : $"[{alias}]";
        }

        private static void AddWord(Hashtable hash, string item, object obj)
        {
            if (hash.ContainsKey(item))
            {
                var list = hash[item] as ArrayList;
                list.Add(obj);
            }
            else
            {
                var list = new ArrayList();
                list.Add(obj);
                hash[item] = list;
            }
        }

		private static void BuildDictionary(StiReport report, ref Hashtable hashFirst, ref Hashtable hashSecond, bool isPack)
		{
			hashFirst = new Hashtable();
			hashSecond = new Hashtable();

            #region Add DataSources and DataColumns
            if (report != null)
            {
                foreach (StiDataSource dataSource in report.Dictionary.DataSources)
                {
                    string dataSourceName = dataSource.Name;
                    string dataSourceAlias = GetCorrectedAlias(report, dataSource.Alias);

                    if (isPack)
                    {
                        dataSourceName = dataSourceName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);

                        if (dataSource.Name != dataSource.Alias)
                            hashFirst[dataSourceName] = dataSourceAlias;

                        if (StiNameValidator.CorrectName(dataSource.Name, report) != dataSource.Alias)
                        {
                            hashFirst[StiNameValidator.CorrectName(dataSourceName, report)] = dataSourceAlias;
                            hashFirst[StiNameValidator.CorrectName(dataSource.Name, report).ToLower(CultureInfo.InvariantCulture)] = dataSourceAlias;
                        }
                    }
                    else
                    {
                        dataSourceAlias = dataSourceAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                        hashFirst[dataSourceAlias] = StiNameValidator.CorrectName(dataSourceName, report);
                    }

                    foreach (StiDataColumn dataColumn in dataSource.Columns)
                    {
                        string dataColumnName = dataColumn.Name;
                        string dataColumnAlias = GetCorrectedAlias(report, dataColumn.Alias);

                        if (isPack)
                        {
                            dataColumnName = dataColumnName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                            string correctedDataColumnName = StiNameValidator.CorrectName(dataColumnName, report);

                            if (dataColumn.Name != dataColumn.Alias)
                            {
                                AddWord(hashSecond, dataColumnName, new Word(dataSourceName, dataColumnAlias));
                            }
                            if (StiNameValidator.CorrectName(dataColumn.Name, report) != dataColumn.Alias)
                            {
                                AddWord(hashSecond, StiNameValidator.CorrectName(dataColumnName, report), new Word(dataSourceName, dataColumnAlias));
                                AddWord(hashSecond, StiNameValidator.CorrectName(dataColumn.Name, report).ToLower(CultureInfo.InvariantCulture), new Word(dataSourceName, dataColumnAlias));
                            }
                        }
                        else
                        {
                            dataColumnAlias = dataColumnAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                            AddWord(hashSecond, dataColumnAlias, new Word(dataSourceName, StiNameValidator.CorrectName(dataColumnName, report)));
                        }
                    }
                }

                foreach (StiBusinessObject businesObject in report.Dictionary.BusinessObjects)
                {
                    BuildBusinessObject(report, businesObject, ref hashFirst, ref hashSecond, isPack);
                }
            }
            #endregion

            #region Add Data Relations
            if (report != null)
            {
                foreach (StiDataRelation dataRelation in report.Dictionary.Relations)
                {
                    string dataRelationName = dataRelation.Name;
                    string dataRelationAlias = GetCorrectedAlias(report, dataRelation.Alias);

                    if (isPack)
                    {
                        dataRelationName = dataRelationName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);

                        if (dataRelation.Name != dataRelation.Alias)
                            AddWord(hashSecond, dataRelationName, dataRelationAlias);

                        if (StiNameValidator.CorrectName(dataRelation.Name, report) != dataRelation.Alias)
                        {
                            AddWord(hashSecond, StiNameValidator.CorrectName(dataRelationName, report), dataRelationAlias);
                            AddWord(hashSecond, StiNameValidator.CorrectName(dataRelation.Name, report).ToLower(CultureInfo.InvariantCulture), dataRelationAlias);
                        }
                    }
                    else
                    {
                        dataRelationAlias = dataRelationAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                        AddWord(hashSecond, dataRelationAlias, StiNameValidator.CorrectName(dataRelationName, report));
                    }
                }
            }
            #endregion

            #region Add Variables
            if (report != null)
            {
                foreach (StiVariable variable in report.Dictionary.Variables)
                {
                    string variableName = variable.Name;
                    string variableAlias = GetCorrectedAlias(report, variable.Alias);

                    if (isPack)
                    {
                        variableName = variableName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);

                        if (variable.Name != variable.Alias)
                            hashFirst[variableName] = variableAlias;

                        if (StiNameValidator.CorrectName(variable.Name, report) != variable.Alias)
                        {
                            hashFirst[StiNameValidator.CorrectName(variableName, report)] = variableAlias;
                            hashFirst[StiNameValidator.CorrectName(variable.Name, report).ToLower(CultureInfo.InvariantCulture)] = variableAlias;
                        }
                    }
                    else
                    {
                        variableAlias = variableAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                        hashFirst[variableAlias] = StiNameValidator.CorrectName(variableName, report);
                    }
                }
            }
            #endregion
		}

        private static void BuildBusinessObject(StiReport report, StiBusinessObject businesObject, ref Hashtable hashFirst, ref Hashtable hashSecond, bool isPack)
        {
            string businesObjectName = businesObject.Name;
            string businesObjectAlias = GetCorrectedAlias(report, businesObject.Alias);

            if (isPack)
            {
                businesObjectName = businesObjectName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);

                if (businesObject.Name != businesObject.Alias)
                {
                    hashFirst[businesObjectName] = businesObjectAlias;
                }

                if (StiNameValidator.CorrectName(businesObject.Name, report) != businesObject.Alias)
                {
                    hashFirst[StiNameValidator.CorrectName(businesObjectName, report)] = businesObjectAlias;
                    hashFirst[StiNameValidator.CorrectName(businesObject.Name, report).ToLower(CultureInfo.InvariantCulture)] = businesObjectAlias;
                }
            }
            else
            {
                businesObjectAlias = businesObjectAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                hashFirst[businesObjectAlias] = StiNameValidator.CorrectName(businesObjectName, report);
            }

            foreach (StiDataColumn dataColumn in businesObject.Columns)
            {
                string dataColumnName = dataColumn.Name;
                string dataColumnAlias = GetCorrectedAlias(report, dataColumn.Alias);

                if (isPack)
                {
                    dataColumnName = dataColumnName.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);

                    if (dataColumn.Name != dataColumn.Alias)
                    {
                        AddWord(hashSecond, dataColumnName, new Word(businesObjectName, dataColumnAlias));
                    }
                    if (StiNameValidator.CorrectName(dataColumn.Name, report) != dataColumn.Alias)
                    {
                        AddWord(hashSecond, StiNameValidator.CorrectName(dataColumnName, report), new Word(businesObjectName, dataColumnAlias));
                        AddWord(hashSecond, StiNameValidator.CorrectName(dataColumn.Name, report).ToLower(CultureInfo.InvariantCulture), new Word(businesObjectName, dataColumnAlias));
                    }
                }
                else
                {
                    dataColumnAlias = dataColumnAlias.Replace(" ", "").ToLower(CultureInfo.InvariantCulture);
                    AddWord(hashSecond, dataColumnAlias, new Word(businesObjectName, StiNameValidator.CorrectName(dataColumnName, report)));
                }
            }

            foreach (StiBusinessObject bo in businesObject.BusinessObjects)
            {
                BuildBusinessObject(report, bo, ref hashFirst, ref hashSecond, isPack);
            }
        }
	}
}