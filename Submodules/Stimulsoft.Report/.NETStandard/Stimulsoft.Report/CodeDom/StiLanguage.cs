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

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Base class for script languages description.
	/// </summary>
	public abstract class StiLanguage
	{
		/// <summary>
		/// Returns a provider for this language.
		/// </summary>
		/// <returns>Provider.</returns>
		public abstract StiCodeDomProvider GetProvider();
	
		/// <summary>
		/// Returns a position of the automatically generated script.
		/// </summary>
		/// <param name="text">Text contains script.</param>
		/// <param name="index">Script position.</param>
		/// <param name="length">Script length.</param>
		/// <param name="language">Script language.</param>
		public static void GetGeneratedCodePos(string text, out int index, out int length, StiLanguage language)
		{
			var generator = language.GetProvider().CreateGenerator() as StiCodeGenerator;

			if (generator == null)
			    throw new Exception("Provider must be inherited from StiCodeGenerator");
			
			var start = generator.GetRegionStart(StiCodeDomSerializator.StrGenCode);
			var end = generator.GetRegionEnd(StiCodeDomSerializator.StrGenCode);
            
            var tempIndex = text.IndexOf(start, StringComparison.InvariantCulture);
            index = tempIndex + start.Length;
            
            var indexEnd = text.IndexOf(end, StringComparison.InvariantCulture);
            if (tempIndex == -1 || indexEnd == -1)
            {
                index = 0;
                indexEnd = 0;
            }
            length = indexEnd - index;
		}

		/// <summary>
		/// Removes the automatically generated script from the text.
		/// </summary>
		/// <param name="text">Text contains script.</param>
		/// <param name="language">Script language.</param>
		/// <returns>Text without script.</returns>
		public static string RemoveGeneratedCode(string text, StiLanguage language)
		{
			int index;
			int length;
			GetGeneratedCodePos(text, out index, out length, language);
            return length > 0 ? text.Remove(index, length) : text;
		}

		/// <summary>
		/// Returns the automatically generated script from the text.
		/// </summary>
		/// <param name="text">Text contains script.</param>
		/// <param name="language">Script language.</param>
		/// <returns>Script.</returns>
		public static string GetGeneratedCode(string text, StiLanguage language)
		{
			int index;
			int length;
			GetGeneratedCodePos(text, out index, out length, language);
			return length > 0 ? text.Substring(index, length) : string.Empty;
		}

		/// <summary>
		/// Inserts the automatically generated script from the text.
		/// </summary>
		/// <param name="text">The text which will be inserted into the script.</param>
		/// <param name="script">Script for embedding.</param>
		/// <param name="language">Script language.</param>
		/// <returns>Text with inserted script.</returns>
		public static string InsertGeneratedCode(string text, string script, StiLanguage language)
		{
			text = RemoveGeneratedCode(text, language);
			int index;
			int length;
			GetGeneratedCodePos(text, out index, out length, language);
		    return index < 0 || index > text.Length ? text : text.Insert(index, script);
		}

		/// <summary>
		///  Replace the automatically generated script for a new one.
		/// </summary>
		/// <param name="text">Text contains script.</param>
		/// <param name="script">Script.</param>
		/// <param name="language">Script language.</param>
		/// <returns>Text changed with script.</returns>
		public static string ReplaceGeneratedCode(string text, string script, StiLanguage language)
		{
			var newScript = GetGeneratedCode(script, language);

			if (string.IsNullOrEmpty(newScript))
				newScript = " \r\n\t ";

			if (string.IsNullOrEmpty(text) || text.Length < 10 && text.Trim().Length == 0)
		        return script;

		    text = RemoveGeneratedCode(text, language);
			return InsertGeneratedCode(text, newScript, language);
		}
	}
}
