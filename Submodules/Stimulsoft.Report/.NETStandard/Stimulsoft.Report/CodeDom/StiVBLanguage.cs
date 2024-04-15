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

using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.CodeDom
{
	/// <summary>
	/// Language VB.
	/// </summary>
	[StiServiceBitmap(typeof(StiLanguage), "Stimulsoft.Report.Bmp.Languages.LanguageVB.bmp")]
	public class StiVBLanguage : StiLanguage
	{
		private StiVBCodeProvider provider;
		/// <summary>
		/// Returns provider of this language.
		/// </summary>
		/// <returns>Provider.</returns>
		public override StiCodeDomProvider GetProvider()
		{
		    return provider ?? (provider = new StiVBCodeProvider());
		}

		/// <summary>
		/// Returns language name.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "VB";
		}
	}
}