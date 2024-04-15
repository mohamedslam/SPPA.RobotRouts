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
	/// Class describes convertor of types.
	/// </summary>
	public sealed class StiTypeConvert
	{
		/// <summary>
		/// Returns base string presentation for the specified type.
		/// </summary>
		/// <param name="type">Specified type.</param>
		/// <param name="language">Script language.</param>
		/// <returns>String presentation of the type.</returns>
		public static string GetBaseType(Type type, StiLanguage language)
		{
			StiCodeGenerator generator = null;
			if (language != null)
			    generator = language.GetProvider().CreateGenerator() as StiCodeGenerator;

			var fullName = type.FullName;

			if (fullName[fullName.Length - 1] == ']')
			{
			    var name = language != null ? generator.GetBaseTypeOutput(fullName.Substring(0, fullName.Length - 2)) : fullName;
			    return name == fullName ? type.Name : name + "[]";
			}
			else
			{
			    var name = language != null ? generator.GetBaseTypeOutput(type.FullName) : type.FullName;
				return name == fullName ? type.Name : name;
			}
		}
	}
}
