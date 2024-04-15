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

using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Represents the method that handles the GetValue event.
	/// </summary>
	public delegate string StiGetValue(StiComponent component);

	/// <summary>
	/// Represents the method that handles the GetValue event.
	/// </summary>
	public delegate string StiGetExcelValue(StiComponent component);

	/// <summary>
	/// Dewscribes text in the component.
	/// </summary>
	public interface IStiText
	{
		/// <summary>
		/// Gets or sets text expression.
		/// </summary>
		StiExpression Text
		{
			get;
			set;
		}

	
		/// <summary>
		/// Gets or sets text value. If the text is not null thet it is necessary to use this text.
		/// </summary>
		string TextValue
		{
			get;
			set;
		}

						
		/// <summary>
		/// Sets the text value in all printed objects.
		/// </summary>
        /// <param name="getValue">Value for setting.</param>
		void SetText(StiGetValue getValue);

		/// <summary>
		/// Gets or sets value indicates that it is necessary to lines of underline.
		/// </summary>
		StiPenStyle LinesOfUnderline
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets value indicates that no need show zeroes.
		/// </summary>
		bool HideZeros
		{
			get;
			set;
		}


		
		StiProcessingDuplicatesType ProcessingDuplicates
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets value indicates that the text expression contains a text only.
		/// </summary>
		bool OnlyText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets maximum number of lines which specify the limit of the height stretch.
		/// </summary>
		int MaxNumberOfLines
		{
			get;
			set;
		}

        /// <summary>
        /// Internal use only.
        /// </summary>
        string GetTextInternal();

        /// <summary>
        /// Internal use only.
        /// </summary>
        void SetTextInternal(string value);
	}
}