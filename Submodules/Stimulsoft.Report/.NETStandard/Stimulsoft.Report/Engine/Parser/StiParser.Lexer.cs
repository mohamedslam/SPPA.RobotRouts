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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Globalization;

namespace Stimulsoft.Report.Engine
{
    public partial class StiParser
    {
        #region Lexer

        #region GetNextLexem
        // Получение очередной лексемы.
        private StiToken GetNextLexem()
        {
            //пропустить пробелы, символы табуляции и другие незначащие символы
            while (position < inputExpression.Length && isWhiteSpace(inputExpression[position])) position++;
            if (position >= inputExpression.Length) return null;

            StiToken token = null;
            char ch = inputExpression[position];
            if (char.IsLetter(ch) || ch == '_')
            {
                int pos2 = position + 1;
                while ((pos2 < inputExpression.Length) && (char.IsLetterOrDigit(inputExpression[pos2]) || inputExpression[pos2] == '_')) pos2++;
                token = new StiToken();
                token.Value = inputExpression.Substring(position, pos2 - position);
                token.Type = StiTokenType.Identifier;
                token.Position = position;
                token.Length = pos2 - position;
                position = pos2;

                if (useAliases)
                {
                    string alias = token.Value;
                    if ((token.Position > 0) && (inputExpression[token.Position - 1] == '.')) alias = "." + alias;
                    if (hashAliases.ContainsKey(alias))
                    {
                        token.Value = (string)hashAliases[alias];
                    }
                }

                return token;
            }
            else if (char.IsDigit(ch))
            {
                token = new StiToken();
                token.Type = StiTokenType.Number;
                token.Position = position;
                token.ValueObject = ScanNumber();
                if (token.ValueObject == null)
                {
                    token.Type = StiTokenType.Identifier;
                    token.Value = inputExpression.Substring(token.Position, position - token.Position);
                }
                token.Length = position - token.Position;
                return token;
            }
            else if ((ch == '"') || ((ch == '@') && (position < inputExpression.Length - 1) && (inputExpression[position + 1] == '"')))
            {
                #region "String"
                bool needReplaceBackslash = true;
                if (ch == '@')
                {
                    needReplaceBackslash = false;
                    position++;
                }

                position++;
                int pos2 = position;
                while (pos2 < inputExpression.Length)
                {
                    if (needReplaceBackslash)
                    {
                        if (inputExpression[pos2] == '"') break;
                        if (inputExpression[pos2] == '\\') pos2++;
                    }
                    else
                    {
                        if (inputExpression[pos2] == '"')
                        {
                            if ((pos2 + 1 < inputExpression.Length) && (inputExpression[pos2 + 1] == '"'))
                                pos2++;
                            else
                                break;
                        }
                    }
                    pos2++;
                }
                token = new StiToken();
                token.Type = StiTokenType.String;
                //token.ValueObject = inputExpression.Substring(position, pos2 - position).Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\'", "'").Replace("\\\\", "\\");
                string st = inputExpression.Substring(position, pos2 - position);
                if (needReplaceBackslash)
                {
                    token.ValueObject = ReplaceBackslash(st);
                }
                else
                {
                    token.ValueObject = ReplaceQuotationMark(st);
                }

                token.Position = position - 1;
                position = pos2 + 1;
                token.Length = position - token.Position;
                return token;
                #endregion
            }
            else if ((ch == '\'') && (position < inputExpression.Length - 2) && (inputExpression[position + 2] == '\''))
            {
                #region "Char"
                position++;
                token = new StiToken();
                token.Type = StiTokenType.Char;
                token.ValueObject = inputExpression[position];

                token.Position = position;
                position += 2;
                token.Length = 1;
                return token;
                #endregion
            }
            else
            {
                #region check for alias bracket
                if (ch == '[')
                {
                    int pos2 = inputExpression.IndexOf(']', position);
                    if (pos2 != -1)
                    {
                        pos2++;
                        string alias = inputExpression.Substring(position, pos2 - position);
                        if ((position > 0) && (inputExpression[position - 1] == '.'))
                        {
                            alias = "." + alias; 
                        }
                        if (hashAliases.ContainsKey(alias))
                        {
                            token = new StiToken();
                            token.Value = (string)hashAliases[alias];
                            token.Type = StiTokenType.Identifier;
                            token.Position = position;
                            token.Length = pos2 - position;
                            position = pos2;
                            return token;
                        }
                    }
                }
                #endregion

                #region Delimiters
                int tPos = position;
                position++;
                char ch2 = ' ';
                if (position < inputExpression.Length) ch2 = inputExpression[position];
                switch (ch)
                {
                    case '.': return new StiToken(StiTokenType.Dot, tPos, 1);
                    case '(': return new StiToken(StiTokenType.LParenthesis, tPos, 1);
                    case ')': return new StiToken(StiTokenType.RParenthesis, tPos, 1);
                    case '[': return new StiToken(StiTokenType.LBracket, tPos, 1);
                    case ']': return new StiToken(StiTokenType.RBracket, tPos, 1);
                    case '+': return new StiToken(StiTokenType.Plus, tPos, 1);
                    case '-': return new StiToken(StiTokenType.Minus, tPos, 1);
                    case '*': return new StiToken(StiTokenType.Mult, tPos, 1);
                    case '/': return new StiToken(StiTokenType.Div, tPos, 1);
                    case '%': return new StiToken(StiTokenType.Percent, tPos, 1);
                    case '^': return new StiToken(StiTokenType.Xor, tPos, 1);
                    case ',': return new StiToken(StiTokenType.Comma, tPos, 1);
                    case ':': return new StiToken(StiTokenType.Colon, tPos, 1);
                    case ';': return new StiToken(StiTokenType.SemiColon, tPos, 1);
                    case '?': return new StiToken(StiTokenType.Question, tPos, 1);
                    case '|':
                        if (IsVB) break;
                        if (ch2 == '|')
                        {
                            position++;
                            return new StiToken(StiTokenType.DoubleOr, tPos, 2);
                        }
                        else return new StiToken(StiTokenType.Or, tPos, 1);
                    case '&':
                        if (IsVB)
                        {
                            return new StiToken(StiTokenType.Plus, tPos, 1);
                        }
                        if (ch2 == '&')
                        {
                            position++;
                            return new StiToken(StiTokenType.DoubleAnd, tPos, 2);
                        }
                        else return new StiToken(StiTokenType.And, tPos, 1);
                    case '!':
                        if (ch2 == '=' && !IsVB)
                        {
                            position++;
                            return new StiToken(StiTokenType.NotEqual, tPos, 2);
                        }
                        else return new StiToken(StiTokenType.Not, tPos, 1);
                    case '=':
                        if (IsVB)
                        {
                            return new StiToken(StiTokenType.Equal, tPos, 1);
                        }
                        else
                        {
                            if (ch2 == '=')
                            {
                                position++;
                                return new StiToken(StiTokenType.Equal, tPos, 2);
                            }
                            else return new StiToken(StiTokenType.Assign, tPos, 1);
                        }
                    case '<':
                        if (ch2 == '<')
                        {
                            position++;
                            return new StiToken(StiTokenType.Shl, tPos, 2);
                        }
                        else if (ch2 == '=')
                        {
                            position++;
                            return new StiToken(StiTokenType.LeftEqual, tPos, 2);
                        }
                        else if (ch2 == '>' && IsVB)
                        {
                            position++;
                            return new StiToken(StiTokenType.NotEqual, tPos, 2);
                        }
                        else return new StiToken(StiTokenType.Left, tPos, 1);
                    case '>':
                        if (ch2 == '>')
                        {
                            position++;
                            return new StiToken(StiTokenType.Shr, tPos, 2);
                        }
                        else if (ch2 == '=')
                        {
                            position++;
                            return new StiToken(StiTokenType.RightEqual, tPos, 2);
                        }
                        else return new StiToken(StiTokenType.Right, tPos, 1);
                }

                token = new StiToken(StiTokenType.Unknown);
                token.ValueObject = ch;
                token.Position = tPos;
                token.Length = 1;
                return token;
                #endregion
            }
        }

        private static bool isWhiteSpace(char ch)
        {
            return char.IsWhiteSpace(ch) || ch < 0x20;
        }
        #endregion

        #region BuildAliases
        private void BuildAliases()
        {
            if (hashAliases != null) return;

            hashAliases = new Hashtable();

            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                string dataSourceName = dataSource.Name;
                string dataSourceAlias = GetCorrectedAlias(dataSource.Alias);
                if (dataSourceAlias != dataSourceName)
                {
                    hashAliases[dataSourceAlias] = dataSourceName;
                }

                foreach (StiDataColumn dataColumn in dataSource.Columns)
                {
                    string dataColumnName = dataColumn.Name;
                    string dataColumnAlias = GetCorrectedAlias(dataColumn.Alias);
                    if (dataColumnAlias != dataColumnName)
                    {
                        hashAliases["." + dataColumnAlias] = dataColumnName;
                    }
                }
            }

            foreach (StiDataSource dataSource in report.Dictionary.DataSources)
            {
                string dataSourceName = dataSource.Name;
                if (hashAliases.ContainsKey(dataSourceName))
                {
                    hashAliases.Remove(dataSourceName);
                }
                foreach (StiDataColumn dataColumn in dataSource.Columns)
                {
                    if (dataColumn == null) continue;

                    var dataColumnName = dataColumn.Name;
                    if (hashAliases.ContainsKey("." + dataColumnName))
                    {
                        hashAliases.Remove("." + dataColumnName);
                    }
                }
            }

            foreach (StiBusinessObject businesObject in report.Dictionary.BusinessObjects)
            {
                BuildBusinessObject(report, businesObject);
            }

            foreach (StiDataRelation dataRelation in report.Dictionary.Relations)
            {
                string dataRelationName = dataRelation.Name;
                string dataRelationAlias = GetCorrectedAlias(dataRelation.Alias);
                if (dataRelationAlias != dataRelationName)
                {
                    hashAliases["." + dataRelationAlias] = dataRelationName;
                }
            }

            foreach (StiVariable variable in report.Dictionary.Variables)
            {
                string variableName = variable.Name;
                string variableAlias = GetCorrectedAlias(variable.Alias);
                if (variableAlias != variableName)
                {
                    hashAliases[variableAlias] = variableName;
                }
            }
        }

        private void BuildBusinessObject(StiReport report, StiBusinessObject businesObject)
        {
            string businesObjectName = businesObject.Name;
            string businesObjectAlias = GetCorrectedAlias(businesObject.Alias);
            if (businesObjectAlias != businesObjectName)
            {
                hashAliases[businesObjectAlias] = businesObjectName;
                hashAliases["." + businesObjectAlias] = businesObjectName;
            }

            foreach (StiDataColumn dataColumn in businesObject.Columns)
            {
                string dataColumnName = dataColumn.Name;
                string dataColumnAlias = GetCorrectedAlias(dataColumn.Alias);
                if (dataColumnAlias != dataColumnName)
                {
                    hashAliases["." + dataColumnAlias] = dataColumnName;
                }
            }

            foreach (StiBusinessObject bo in businesObject.BusinessObjects)
            {
                BuildBusinessObject(report, bo);
            }
        }

        private static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name) || !(Char.IsLetter(name[0]) || name[0] == '_'))
                return false;

            for (int pos = 0; pos < name.Length; pos++)
                if (!(Char.IsLetterOrDigit(name[pos]) || (name[pos] == '_'))) return false;

            return true;
        }

        private static string GetCorrectedAlias(string alias)
        {
            if (IsValidName(alias)) return alias;
            return string.Format("[{0}]", alias);
        }
        #endregion

        #region ReplaceBackslash
        private static string ReplaceBackslash(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int index = 0; index < input.Length; index++)
            {
                if ((input[index] == '\\') && (index < input.Length - 1))
                {
                    index++;
                    char ch = input[index];
                    switch (ch)
                    {
                        case '\\':
                            output.Append("\\");
                            break;

                        case '\'':
                            output.Append('\'');
                            break;

                        case '"':
                            output.Append('"');
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

            return output.ToString();
        }
        #endregion

        #region ReplaceQuotationMark
        private static string ReplaceQuotationMark(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int index = 0; index < input.Length; index++)
            {
                if ((input[index] == '"') && (index < input.Length - 1))
                {
                    index++;
                    output.Append('"');
                }
                else
                {
                    output.Append(input[index]);
                }
            }

            return output.ToString();
        }
        #endregion

        #region ScanNumber
        private object ScanNumber()
        {
            TypeCode typecode = TypeCode.Int32;
            int posBegin = position;
            int posBeginAll = position;
            //integer part
            while (position != inputExpression.Length && Char.IsDigit(inputExpression[position]))
            {
                position++;
            }
            if (position != inputExpression.Length && inputExpression[position] == '.' &&
                position + 1 != inputExpression.Length && Char.IsDigit(inputExpression[position + 1]))
            {
                //fractional part
                position++;
                while (position != inputExpression.Length && Char.IsDigit(inputExpression[position]))
                {
                    position++;
                }
                typecode = TypeCode.Double;
            }
            string nm = inputExpression.Substring(posBegin, position - posBegin);
            nm = nm.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (position != inputExpression.Length)
            {
                if (nm == "0" && inputExpression[position] == 'x')
                {
                    //format 0xFFFF
                    position++;
                    posBegin = position;
                    while (position != inputExpression.Length && Char.IsLetterOrDigit(inputExpression[position]))
                    {
                        position++;
                    }
                    string hex = inputExpression.Substring(posBegin, position - posBegin).ToLower();
                    int resInt = 0;
                    if (int.TryParse(hex, NumberStyles.HexNumber, null, out resInt))
                    {
                        return resInt;
                    }
                    return null;
                }
                else if (Char.IsLetter(inputExpression[position]))
                {
                    //postfix
                    posBegin = position;
                    while (position != inputExpression.Length && Char.IsLetter(inputExpression[position]))
                    {
                        position++;
                    }
                    string postfix = inputExpression.Substring(posBegin, position - posBegin).ToLower();
                    switch (postfix)
                    {
                        case "f":
                            typecode = TypeCode.Single;
                            break;
                        case "d":
                            typecode = TypeCode.Double;
                            break;
                        case "m":
                            typecode = TypeCode.Decimal;
                            break;
                        case "l":
                            typecode = TypeCode.Int64;
                            break;
                        case "u":
                        case "ul":
                        case "lu":
                            typecode = TypeCode.UInt64;
                            break;

                        default: return null;
                    }
                }
            }

            if ((typecode == TypeCode.Int32) && (nm.Length > 9)) typecode = TypeCode.Int64;

            object result = null;
            try
            {
                result = Convert.ChangeType(nm, typecode);
            }
            catch
            {
                if (typecode == TypeCode.Int32 || typecode == TypeCode.Int64 || typecode == TypeCode.UInt32 || typecode == TypeCode.UInt64)
                {
                    ThrowError(ParserErrorCode.IntegralConstantIsTooLarge, new StiToken(StiTokenType.Number, posBeginAll, position - posBeginAll));
                }
            }
            return result;
        }
        #endregion

        #region PostProcessTokensList
        private List<StiToken> PostProcessTokensList(List<StiToken> tokensList)
        {
            List<StiToken> newList = new List<StiToken>();
            tokenPos = 0;
            while (tokenPos < tokensList.Count)
            {
                StiToken token = tokensList[tokenPos];
                tokenPos++;
                if (token.Type == StiTokenType.Identifier)
                {
                    StiDataSource ds = report.Dictionary.DataSources[token.Value];
                    StiBusinessObject bos = report.Dictionary.BusinessObjects[token.Value];

                    string stNameSpace = null;
                    if (NamespacesList.Contains(token.Value))
                    {
                        if (tokenPos + 1 >= tokensList.Count) ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                        if (tokensList[tokenPos].Type != StiTokenType.Dot) ThrowError(ParserErrorCode.SyntaxError, token, token.Value);
                        stNameSpace = token.Value + ".";
                        token = tokensList[tokenPos + 1];
                        tokenPos += 2;
                        while (NamespacesList.Contains(stNameSpace + tokensList[tokenPos - 1].Value))
                        {
                            if (tokenPos + 1 >= tokensList.Count) ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                            if (tokensList[tokenPos].Type != StiTokenType.Dot) ThrowError(ParserErrorCode.SyntaxError, token, token.Value);
                            stNameSpace += token.Value + ".";
                            token = tokensList[tokenPos + 1];
                            tokenPos += 2;
                        }
                    }

                    #region check for DataSource field
                    if (ds != null)
                    {
                        StringBuilder fieldPath = new StringBuilder(StiNameValidator.CorrectName(token.Value, report));
                        var fieldInfo = new StiParserDataSourceFieldInfo();
                        fieldInfo.Path.Add(StiNameValidator.CorrectName(token.Value, report));
                        fieldInfo.Objects.Add(ds);
                        while ((tokenPos + 1 < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.Dot) && (ds != null))
                        {
                            token = tokensList[tokenPos + 1];
                            string nextName = StiNameValidator.CorrectName(token.Value, report);

                            StiDataRelation dr = GetDataRelationByName(nextName, ds);
                            if (dr != null)
                            {
                                ds = dr.ParentSource;
                                tokenPos += 2;
                                fieldPath.Append(".");
                                fieldPath.Append(dr.NameInSource);
                                fieldInfo.Path.Add(dr.NameInSource);
                                fieldInfo.Objects.Add(dr);
                                continue;
                            }
                            StiDataColumn dc = GetDataColumnByName(nextName, ds);
                            if ((dc == null) && (tokenPos + 3 < tokensList.Count) && (tokensList[tokenPos + 2].Type == StiTokenType.Dot) && (tokensList[tokenPos + 3].Type == StiTokenType.Identifier))
                            {
                                //check for combined names
                                nextName = StiNameValidator.CorrectName(token.Value + "." + tokensList[tokenPos + 3].Value, report);
                                dc = GetDataColumnByName(nextName, ds);
                                if (dc != null)
                                {
                                    tokenPos += 2;
                                }
                                else
                                {
                                    if ((tokenPos + 5 < tokensList.Count) && (tokensList[tokenPos + 4].Type == StiTokenType.Dot) && (tokensList[tokenPos + 5].Type == StiTokenType.Identifier))
                                    {
                                        nextName = StiNameValidator.CorrectName(token.Value + "." + tokensList[tokenPos + 3].Value + "." + tokensList[tokenPos + 5].Value, report);
                                        dc = GetDataColumnByName(nextName, ds);
                                        if (dc != null)
                                        {
                                            tokenPos += 4;
                                        }
                                    }
                                }
                            }
                            if (dc != null)
                            {
                                tokenPos += 2;
                                fieldPath.Append(".");
                                fieldPath.Append(nextName);
                                fieldInfo.Path.Add(dc.Name);
                                fieldInfo.Objects.Add(dc);
                                break;
                            }
                            token = tokensList[tokenPos - 1];
                            break;
                        }
                        token.Type = StiTokenType.DataSourceField;
                        token.Value = fieldPath.ToString();
                        token.ValueObject = fieldInfo;
                    }
                    #endregion

                    #region check for BusinessObject field
                    else if (bos != null)
                    {
                        StringBuilder fieldPath = new StringBuilder(token.Value);
                        while (tokenPos + 1 < tokensList.Count && tokensList[tokenPos].Type == StiTokenType.Dot)
                        //while (inputExpression[pos2] == '.')
                        {
                            token = tokensList[tokenPos + 1];
                            string nextName = token.Value;

                            if (bos.Columns.Contains(nextName))
                            {
                                tokenPos += 2;
                                fieldPath.Append(".");
                                fieldPath.Append(nextName);
                                break;
                            }
                            bos = bos.BusinessObjects[nextName];
                            if (bos != null)
                            {
                                tokenPos += 2;
                                fieldPath.Append(".");
                                fieldPath.Append(bos.Name);
                                continue;
                            }
                            token = tokensList[tokenPos - 1];
                            break;
                        }
                        token.Type = StiTokenType.BusinessObjectField;
                        //надо оптимизировать и сохранять сразу массив строк !!!!!
                        token.Value = fieldPath.ToString();
                    }
                    #endregion

                    else if ((newList.Count > 0) && (newList[newList.Count - 1].Type == StiTokenType.Dot) && (stNameSpace == null))
                    {
                        if (MethodsList.Contains(token.Value))
                        {
                            token.Type = StiTokenType.Method;
                        }
                        else if (PropertiesList.Contains(token.Value))
                        {
                            token.Type = StiTokenType.Property;
                        }
                        else
                        {
                            bool nextIsParenthesis = (tokenPos < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.LParenthesis);
                            if (!nextIsParenthesis)
                            {
                                if ((newList.Count > 1) && (newList[newList.Count - 2].Type == StiTokenType.DataSourceField))
                                {
                                    #region Check if DataColumn type contain field with name of current token
                                    string fieldPath = newList[newList.Count - 2].Value;
                                    List<string> parts = new List<string>(fieldPath.Split(new char[] { '.' }));
                                    StiDataSource dsTemp = report.Dictionary.DataSources[parts[0]];
                                    StiDataColumn column = null;
                                    if (parts.Count > 1)
                                    {
                                        if (parts.Count == 2)
                                        {
                                            column = dsTemp.Columns[parts[1]];
                                        }
                                        else
                                        {
                                            string nameInSource = parts[1];
                                            dsTemp = dsTemp.GetParentDataSource(nameInSource);
                                            int indexPart = 2;
                                            while (indexPart < parts.Count - 1)
                                            {
                                                nameInSource = parts[indexPart];
                                                dsTemp = dsTemp.GetParentDataSource(nameInSource);
                                                indexPart++;
                                            }
                                            column = dsTemp.Columns[parts[indexPart]];
                                        }
                                    }
                                    if (column != null)
                                    {
                                        try
                                        {
                                            PropertyInfo property = column.Type.GetProperty(token.Value);
                                            if (property != null)
                                            {
                                                token.Type = StiTokenType.Property;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    #endregion
                                }
                            }
                            if (token.Type == StiTokenType.Identifier)
                                ThrowError(ParserErrorCode.FieldMethodOrPropertyNotFound, token, token.Value);
                        }
                    }

                    else if (TypesList.Contains(token.Value) && (tokenPos + 1 < tokensList.Count) && 
                        ((tokensList[tokenPos].Type == StiTokenType.Dot) || (tokensList[tokenPos].Type == StiTokenType.RParenthesis) || (tokensList[tokenPos].Type == StiTokenType.Left)))
                    {
                        object type = TypesList[token.Value];

                        token.Type = StiTokenType.Cast;
                        token.ValueObject = type;

                        if (tokensList[tokenPos].Type == StiTokenType.Dot)
                        {
                            string tempName = token.Value + "." + tokensList[tokenPos + 1].Value;
                            if (FunctionsList.Contains(tempName))
                            {
                                token.Type = StiTokenType.Function;
                                token.Value = tempName;
                                tokenPos += 2;
                            }
                            if (SystemVariablesList.Contains(tempName))
                            {
                                token.Type = StiTokenType.SystemVariable;
                                token.Value = tempName;
                                tokenPos += 2;
                            }
                            if (ConstantsList.Contains(tempName))
                            {
                                token.Type = StiTokenType.Number;
                                token.ValueObject = ConstantsList[tempName]; 
                                token.Value = tempName;
                                tokenPos += 2;
                            }
                        }

                        var type1 = type as Type;
                        if ((type1 != null) && (type1 == typeof(List<>)) && (tokensList[tokenPos].Type == StiTokenType.Left))
                        {
                            object type2 = TypesList[tokensList[tokenPos + 1].Value];
                            if ((tokenPos + 3 >= tokensList.Count) || (type2 == null) || (tokensList[tokenPos + 2].Type != StiTokenType.Right))
                            {
                                ThrowError(ParserErrorCode.SyntaxError, token, token.Value);
                            }
                            Type tempType1 = Type.GetType("System." + ((TypeCode)type2).ToString());
                            string tempName = "System.Collections.Generic.List`1[[" + tempType1.FullName + "]]";
                            token.ValueObject = Type.GetType(tempName);
                            tokenPos += 3;
                        }
                    }

                    else if (ComponentsList.Contains(token.Value) && !((tokenPos < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.LParenthesis)))
                    {
                        token.Type = StiTokenType.Component;
                        if ((tokenPos + 1 < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.Colon) && ComponentsList.Contains(tokensList[tokenPos + 1].Value))
                        {
                            StiComponent comp = (StiComponent)ComponentsList[tokensList[tokenPos + 1].Value];
                            if (comp != null && comp is StiDataBand)
                            {
                                var fieldInfo = new StiParserDataSourceFieldInfo();
                                fieldInfo.Path.Add(StiNameValidator.CorrectName(tokensList[tokenPos + 1].Value, report));
                                fieldInfo.Objects.Add((comp as StiDataBand).DataSource);

                                token.Value = (comp as StiDataBand).DataSourceName;
                                token.ValueObject = fieldInfo;
                                token.Type = StiTokenType.DataSourceField;
                                tokenPos += 2;
                            }
                        }
                    }

                    else if (FunctionsList.Contains(stNameSpace + token.Value) && (tokenPos < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.LParenthesis))
                    {
                        //while ((StiFunctionType)FunctionsList[token.Value] == StiFunctionType.NameSpace)
                        //{
                        //    if (tokenPos + 1 >= tokensList.Count) ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                        //    token.Value += "." + tokensList[tokenPos + 1].Value;
                        //    tokenPos += 2;
                        //    if (!FunctionsList.Contains(token.Value)) ThrowError(ParserErrorCode.FunctionNotFound, token, token.Value);
                        //}
                        token.Value = stNameSpace + token.Value;
                        token.Type = StiTokenType.Function;
                    }

                    else if (UserFunctionsList.Contains(token.Value) && (tokenPos < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.LParenthesis))
                    {
                        token.Type = StiTokenType.Function;
                    }

                    else if (runtimeConstants != null && runtimeConstants.Count > 0 && runtimeConstantsHash.Contains(token.Value))
                    {
                        while (runtimeConstantsHash[token.Value] == namespaceObj)
                        {
                            if (tokenPos + 1 >= tokensList.Count) ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                            string oldTokenValue = token.Value;
                            token.Value += "." + tokensList[tokenPos + 1].Value;
                            if (!runtimeConstantsHash.Contains(token.Value)) ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, token, oldTokenValue, tokensList[tokenPos + 1].Value);
                            tokenPos += 2;
                        }
                        token.Type = StiTokenType.Variable;
                    }

                    else if (OperatorsList.Contains(token.Value) && (stNameSpace == null))
                    {
                        token.Type = (StiTokenType)OperatorsList[token.Value];
                    }

                    else if (ConstantsList.Contains(stNameSpace + token.Value))
                    {
                        while (ConstantsList[stNameSpace + token.Value] == namespaceObj)
                        {
                            if (tokenPos + 1 >= tokensList.Count) ThrowError(ParserErrorCode.UnexpectedEndOfExpression);
                            string oldTokenValue = token.Value;
                            token.Value += "." + tokensList[tokenPos + 1].Value;
                            tokenPos += 2;
                            if (!ConstantsList.Contains(stNameSpace + token.Value)) ThrowError(ParserErrorCode.ItemDoesNotContainDefinition, token, oldTokenValue, tokensList[tokenPos + 1].Value);
                        }
                        token.Value = stNameSpace + token.Value;
                        token.ValueObject = ConstantsList[token.Value];
                        token.Type = StiTokenType.Number;
                    }

                    else if (report.Dictionary.Variables.Contains(token.Value) || (report.Variables != null && report.Variables.ContainsKey(token.Value)))
                    {
                        if ((tokenPos + 1 < tokensList.Count) && (tokensList[tokenPos].Type == StiTokenType.Dot) && (tokensList[tokenPos + 1].Value == "Label"))
                        {
                            token.Type = StiTokenType.String;
                            token.ValueObject = token.Value;
                            newList.Add(new StiToken(StiTokenType.Function, token.Position, token.Length + 6) { Value = "GetLabel" });
                            newList.Add(new StiToken(StiTokenType.LParenthesis, token.Position, token.Length));
                            newList.Add(token);
                            newList.Add(new StiToken(StiTokenType.RParenthesis, token.Position, token.Length));
                            tokenPos += 2;
                            continue;
                        }
                        token.Type = StiTokenType.Variable;
                    }

                    else if (SystemVariablesList.Contains(token.Value) && (token.Value != "value" || component is Stimulsoft.Report.CrossTab.StiCrossCell))
                    {
                        token.Type = StiTokenType.SystemVariable;
                    }

                    else if (KeywordsList.ContainsKey(token.Value))
                    {
                        if (((StiTokenType)KeywordsList[token.Value] == StiTokenType.RefVariable) && (tokenPos < tokensList.Count) && report.Dictionary.Variables.Contains(tokensList[tokenPos].Value))
                        {
                            token.Type = StiTokenType.RefVariable;
                            token.Value = tokensList[tokenPos].Value;
                            tokenPos++;
                        }
                        else if (((StiTokenType)KeywordsList[token.Value] == StiTokenType.New) && (tokenPos + 1 < tokensList.Count) &&
                            TypesList.Contains(tokensList[tokenPos].Value) && (tokensList[tokenPos + 1].Type == StiTokenType.LParenthesis))
                        {
                            Type newType = typeof(object);
                            TypeCode tc = (TypeCode)TypesList[tokensList[tokenPos].Value];
                            if (tc == TypeCode.DateTime) newType = typeof(DateTime);

                            token.Type = StiTokenType.New;
                            token.Value = tokensList[tokenPos].Value;
                            token.ValueObject = newType;
                            tokenPos++;
                        }
                        else
                        {
                            ThrowError(ParserErrorCode.SyntaxError, token, token.Value);
                        }
                    }

                    else
                    {
                        if (!string.IsNullOrEmpty(stNameSpace))
                        {
                            ThrowError(ParserErrorCode.TheTypeOrNamespaceNotExistInTheNamespace, token, token.Value, stNameSpace.Substring(0, stNameSpace.Length - 1));
                        }
                        else
                        {
                            ThrowError(ParserErrorCode.NameDoesNotExistInCurrentContext, token, token.Value);
                        }
                    }
                }
                newList.Add(token);
            }
            return newList;
        }

        private void CreateRuntimeConstantsHash()
        {
            if (runtimeConstantsHash != null) runtimeConstantsHash.Clear();
            else runtimeConstantsHash = new Hashtable(syntaxCaseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
            foreach (DictionaryEntry de in runtimeConstants)
            {
                var value = de.Key.ToString();
                if (value.Contains("."))
                {
                    string[] parts = value.Split(new char[] { '.' });
                    int index = 0;
                    string part = parts[0];
                    while (index < parts.Length - 1)
                    {
                        runtimeConstantsHash[part] = namespaceObj;
                        index++;
                        part += "." + parts[index];
                    }
                }
                runtimeConstantsHash[value] = de.Value;
            }
        }

        //private StiDataSource GetDataSourceByName(string name)
        //{
        //    foreach (StiDataSource ds in report.Dictionary.DataSources)
        //    {
        //        if (ds.Alias == name)
        //        {
        //            return ds;
        //        }
        //    }
        //    return null;
        //}

        private StiDataRelation GetDataRelationByName(string name, StiDataSource ds)
        {
            foreach (StiDataRelation drTemp in report.Dictionary.Relations)
            {
                if ((drTemp.ChildSource == ds) && (drTemp.Name == name || drTemp.NameInSource == name))
                {
                    return drTemp;
                }
            }
            foreach (StiDataRelation drTemp in report.Dictionary.Relations)
            {
                if ((drTemp.ChildSource == ds) && (StiNameValidator.CorrectName(drTemp.Name, report) == name || StiNameValidator.CorrectName(drTemp.NameInSource, report) == name))
                {
                    return drTemp;
                }
            }
            return null;
        }

        private StiDataColumn GetDataColumnByName(string name, StiDataSource ds)
        {
            StiDataColumn dct = ds.Columns[name];
            if (dct == null)
            {
                string name2 = name.ToLower(CultureInfo.InvariantCulture);
                foreach (StiDataColumn column in ds.Columns)
                {
                    if (StiNameValidator.CorrectName(column.Name.ToLower(CultureInfo.InvariantCulture)) == name2)
                    {
                        ds.Columns.CachedDataColumns[name] = column;
                        dct = column;
                        name = column.Name;
                        break;
                    }
                }
            }

            if (ds.DataTable != null)
            {
                int index = ds.GetColumnIndex(name);
                if ((index >= 0) && (index < ds.DataTable.Columns.Count))
                {
                    string nameInSource = ds.DataTable.Columns[index].ColumnName;
                    foreach (StiDataColumn dc in ds.Columns)
                    {
                        if (dc.NameInSource == nameInSource) return dc;
                    }
                }
            }
            return dct;
        }
        #endregion

        private void MakeTokensList()
        {
            BuildAliases();
            tokensList = new List<StiToken>();
            position = 0;
            while (true)
            {
                StiToken token = GetNextLexem();
                if (token == null) break;
                tokensList.Add(token);
            }
            tokensList = PostProcessTokensList(tokensList);
        }

        #endregion
    }
}
