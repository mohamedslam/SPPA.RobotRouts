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
using Stimulsoft.Data.Functions;
using Stimulsoft.Report.CodeDom;

namespace Stimulsoft.Report
{
	/// <summary>
	/// The class serves to serialize into a code of a report.
	/// </summary>
	public class StiNameValidator
	{	
		/// <summary>
		/// Replaces incorrect symbols in the string on the symbol '_'.
		/// </summary>
		/// <param name="str">String to change.</param>
		/// <returns>Changed string.</returns>
		public static string CorrectName(string str, bool isDataExpression = false)
		{
			return CorrectName(str, true, null, isDataExpression);
		}

        /// <summary>
        /// Replaces incorrect symbols in the string on the symbol '_'.
        /// </summary>
        /// <param name="str">String to change.</param>
        /// <param name="checkKeywords">Check string for keywords</param>
        /// <returns>Changed string.</returns>
        public static string CorrectName(string str, bool checkKeywords, bool isDataExpression = false)
        {
            return CorrectName(str, checkKeywords, null, isDataExpression);
        }

        /// <summary>
        /// Replaces incorrect symbols in the string on the symbol '_'.
        /// </summary>
        /// <param name="str">String to change.</param>
        /// <param name="report">Report to get ScriptLanguage info</param>
        /// <returns>Changed string.</returns>
        public static string CorrectName(string str, StiReport report, bool isDataExpression = false)
        {
            return CorrectName(str, true, report, isDataExpression);
        }

        /// <summary>
        /// Replaces incorrect symbols in the string on the symbol '_'.
        /// </summary>
        /// <param name="str">String to change.</param>
        /// <param name="checkKeywords">Check string for keywords</param>
        /// <param name="report">Report to get ScriptLanguage info</param>
        /// <returns>Changed string.</returns>
        public static string CorrectName(string str, bool checkKeywords, StiReport report, 
            bool isDataExpression = false)
		{
            if (isDataExpression)
                return Funcs.ToExpression(str);

            var sb = new StringBuilder(str);

			for (int pos = 0; pos < sb.Length; pos++)
			{
				if (!(char.IsLetterOrDigit(sb[pos]) || sb[pos] == '_'))
                    sb[pos] = '_';
			}

			str = sb.ToString();

			if (str.Length > 0 && char.IsDigit(str[0]))
                str = "n" + str;

			if (StiOptions.Engine.FullTrust && checkKeywords)
                str = CheckKeyword(str, report);            

			return str;
		}

		private static string CheckKeyword(string str, StiReport report)
		{
            var language = StiReportLanguageType.CSharp;
            if (report != null)
                language = report.ScriptLanguage;

			if ((language == StiReportLanguageType.CSharp || language == StiReportLanguageType.JS) && StiCSharpCodeGenerator.IsKeywordExist(str) ||
                language == StiReportLanguageType.VB && StiVBCodeGenerator.IsKeywordExist(str.ToLowerInvariant()))
			{
				if (str == "date")
                    return str;

				return str + "_";
			}
			return str;
		}

        public static string CorrectBusinessObjectName(string str, bool isDataExpression = false)
        {
            return CorrectBusinessObjectName(str, null, isDataExpression);
        }
            
        public static string CorrectBusinessObjectName(string str, StiReport report, bool isDataExpression = false)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            if (isDataExpression)
                return Funcs.ToExpression(str);

            var arr = str.Split(new char[] { '.' });
            var sb = new StringBuilder();
            for (int index = 0; index < arr.Length; index++)
            {
                sb.Append(CorrectName(arr[index], report));
                if (index < arr.Length - 1)
                    sb.Append(".");
            }
            return sb.ToString();
        }
    }
}