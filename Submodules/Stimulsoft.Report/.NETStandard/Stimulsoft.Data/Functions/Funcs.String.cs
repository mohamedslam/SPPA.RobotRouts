#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base;
using Stimulsoft.Base.Helpers;
using Stimulsoft.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        #region Fields.Static
        private static Dictionary<string, string> toProperCaseCache = new Dictionary<string, string>();
        private static Dictionary<string, string> toLowerCaseCache = new Dictionary<string, string>();
        private static Dictionary<string, string> toUpperCaseCache = new Dictionary<string, string>();
        private static Dictionary<string, string> toDataNameCache = new Dictionary<string, string>();
        #endregion

        #region Methods.Funcs
        public static object Format(string format, object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToList(value).Select(v => Format(format, v));
            else
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    //check if format already has parenthesis, as in reports
                    int pos1 = format.IndexOf("{");
                    int pos2 = format.IndexOf("}");
                    if (pos1 != -1 && pos2 != -1 && pos1 < pos2)
                    {
                        var format2 = format.Substring(0, pos1 + 1) + format.Substring(pos1 + 1, pos2 - pos1 - 1).Replace("[", "").Replace("]", "") + format.Substring(pos2);   //replace brackets
                        return string.Format(format2, value);
                    }
                }
                return string.Format("{0:" + format + "}", value);
            }
        }

        public static string Insert(string str, int startIndex, string subStr)
        {
            if (str == null)
                return null;

            if (startIndex < 0 || startIndex > str.Length || subStr == null)
                return str;

            return str.Insert(startIndex, subStr);
        }

        public static object InsertObject(object value, int startIndex, string subStr)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Insert(v, startIndex, subStr));
            else
                return Insert(StiValueHelper.TryToString(value), startIndex, subStr);
        }

        public static bool IsDataEqual(IStiAppDataSource dataSource, string dataColumnName, string searchColumnName)
        {
            if (ToDataName(dataColumnName) == ToDataName(searchColumnName))
                return true;

            if (!searchColumnName.Contains("."))
                return false;

            return ToDataName($"{dataSource.GetName()}.{dataColumnName}") == searchColumnName;
        }

        public static string Left(string str, int length = -1)
        {
            if (str == null)
                return null;

            if (length <= 0)
                return "";

            if (length >= str.Length)
                return str;

            return str.Substring(0, length);
        }

        public static object LeftObject(object value, int length = -1)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Left(v, length));
            else
                return Left(StiValueHelper.TryToString(value), length);
        }

        public static int Length(string str)
        {
            return str == null ? 0 : str.Length;
        }

        public static object LengthObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(Length).Cast<object>();
            else
                return Length(StiValueHelper.TryToString(value));
        }

        public static string Remove(string str, int startIndex, int count)
        {
            if (str == null)
                return null;

            if (count <= 0 || startIndex < 0 || startIndex >= str.Length)
                return str;

            if (startIndex + count > str.Length)
                count = str.Length - startIndex;

            return str.Remove(startIndex, count);
        }

        public static object RemoveObject(object value, int startIndex, int count)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Remove(v, startIndex, count));
            else
                return Remove(StiValueHelper.TryToString(value), startIndex, count);
        }

        public static string Replace(string str, string oldValue, string newValue)
        {
            if (str == null)
                return null;

            if (oldValue == null)
                return str;

            if (newValue == null)
                newValue = "";

            return str.Replace(oldValue, newValue);
        }

        public static object ReplaceObject(object value, string oldValue, string newValue)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Replace(v, oldValue, newValue));
            else
                return Replace(StiValueHelper.TryToString(value), oldValue, newValue);
        }

        public static string Right(string str, int length = -1)
        {
            if (str == null)
                return null;

            if (length <= 0)
                return "";

            if (length >= str.Length)
                return str;

            return str.Substring(str.Length - length, length);
        }

        public static object RightObject(object value, int length = -1)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Right(v, length));
            else
                return Right(StiValueHelper.TryToString(value), length);
        }

        public static string ToDataName(string name)
        {
            if (name == null)
                return null;

            lock (toDataNameCache)
            {
                if (toDataNameCache.ContainsKey(name))
                    return toDataNameCache[name];

                var lowerName = name.ToLowerInvariant().Replace(" ", "");

                if (lowerName.StartsWith("["))
                    lowerName = lowerName.Substring(1);

                if (lowerName.EndsWith("]"))
                    lowerName = lowerName.Substring(0, lowerName.Length - 1);

                toDataNameCache.Add(name, lowerName);

                return lowerName;
            }
        }

        public static string ToExpression(string name)
        {
            return ToExpression(name, null);
        }

        public static string ToExpression(string sourceName, string columnName)
        {
            if (sourceName == null)
                return null;

            sourceName = sourceName.Replace(" ", "");

            if (sourceName.Length == 0)
                return string.Empty;
            
            if (!string.IsNullOrEmpty(columnName))
            {
                columnName = columnName.Replace(" ", "");

                if (sourceName.Any(c => !(char.IsLetterOrDigit(c) || c == '_')) ||
                    columnName.Any(c => !(char.IsLetterOrDigit(c) || c == '_')) ||
                    char.IsDigit(sourceName[0]) || char.IsDigit(columnName[0]))
                {
                    return $"[{sourceName}.{columnName}]";
                }

                return $"{sourceName}.{columnName}";
            }

            if (sourceName.Any(c => !(char.IsLetterOrDigit(c) || c == '.' || c == '_')) ||
                char.IsDigit(sourceName[0]))
            {
                return $"[{sourceName}]";
            }

            return sourceName;
        }

        public static string ToLowerCase(string str)
        {
            if (str == null)
                return null;

            lock (toLowerCaseCache)
            {
                if (toLowerCaseCache.ContainsKey(str))
                    return toLowerCaseCache[str];

                var lowerStr = str.ToLowerInvariant();
                toLowerCaseCache.Add(str, lowerStr);

                return lowerStr;
            }
        }

        public static object ToLowerCaseObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(ToLowerCase);
            else
                return ToLowerCase(StiValueHelper.TryToString(value));
        }

        public static string ToProperCase(string str)
        {
            if (str == null) return null;

            lock (toProperCaseCache)
            {
                if (toProperCaseCache.ContainsKey(str))
                    return toProperCaseCache[str];

                var properStr = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
                toProperCaseCache.Add(str, properStr);

                return properStr;
            }
        }

        public static object ToProperCaseObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(ToProperCase);
            else
                return ToProperCase(StiValueHelper.TryToString(value));
        }

        public static string ToString(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static object ToStringObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToList(value).Select(ToString);
            else
                return ToString(value);
        }

        public static string ToUpperCase(string str)
        {
            if (str == null)
                return null;

            lock (toUpperCaseCache)
            {
                if (toUpperCaseCache.ContainsKey(str))
                    return toUpperCaseCache[str];

                var lowerStr = str.ToUpperInvariant();
                toUpperCaseCache.Add(str, lowerStr);

                return lowerStr;
            }
        }

        public static object ToUpperCaseObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(ToUpperCase);
            else
                return ToUpperCase(StiValueHelper.TryToString(value));
        }

        public static string Trim(string str)
        {
            return str?.Trim();
        }

        public static object TrimObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(Trim);
            else
                return Trim(StiValueHelper.TryToString(value));
        }

        public static string TrimStart(string str)
        {
            return str?.TrimStart();
        }

        public static object TrimStartObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(TrimStart);
            else
                return TrimStart(StiValueHelper.TryToString(value));
        }

        public static string TrimEnd(string str)
        {
            return str?.TrimEnd();
        }

        public static object TrimEndObject(object value)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(TrimEnd);
            else
                return TrimEnd(StiValueHelper.TryToString(value));
        }

        public static string Substring(string str, int startIndex, int length = -1)
        {
            if (str == null)
                return null;

            if (startIndex < -1)
                return "";

            if (length == -1)
                return startIndex >= str.Length ? "" : str.Substring(startIndex);

            if (startIndex >= str.Length)
                return "";

            return startIndex + length > str.Length
                ? str.Substring(startIndex)
                : str.Substring(startIndex, length);
        }

        public static object SubstringObject(object value, int startIndex, int length = -1)
        {
            if (ListExt.IsList(value))
                return ListExt.ToStringList(value).Select(v => Substring(v, startIndex, length));
            else
                return Substring(StiValueHelper.TryToString(value), startIndex, length);
        }

        public static object GetSystemVariable(StiSystemVariableObject variable, int line)
        {
            switch (variable)
            {
                case StiSystemVariableObject.Line:
                    return line;

                case StiSystemVariableObject.LineABC:
                    return Funcs.ToABC(line);

                case StiSystemVariableObject.LineRoman:
                    return Funcs.ToRoman(line);

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }
}