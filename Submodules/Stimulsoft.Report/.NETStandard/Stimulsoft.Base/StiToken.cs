#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

namespace Stimulsoft.Base
{	
	/// <summary>
	/// Class describes Token.
	/// </summary>
	public class StiToken
	{
        #region Properties
        /// <summary>
        /// Gets or sets value indicates the beginning of token in text.
        /// </summary>
        public int Index { get; set; }

	    /// <summary>
		/// Gets or sets value indicates the length of token.
		/// </summary>
		public int Length { get; set; }
        
	    /// <summary>
		/// Gets or sets value indicates the type of token.
		/// </summary>
		public StiTokenType	Type { get; set; }


	    /// <summary>
		/// Gets or sets Value of the identifier.
		/// </summary>
		public object Data { get; set; }
        #endregion

        #region Methods
        public override string ToString()
	    {
	        switch (Type)
	        {
	            case StiTokenType.Value:
	                return $"{Type}={Data}";

	            case StiTokenType.Ident:
	                return $"{Type}({Data})";

	            default:
	                return Type.ToString();
	        }
	    }
        #endregion

        /// <summary>
        /// Create a new instance StiToken.
        /// </summary>
        /// <param name="type">Type Token.</param>
        public StiToken(StiTokenType type):this(type, 0, 0)
		{
		}

		/// <summary>
		/// Creates a new object of the type StiToken.
		/// </summary>
		/// <param name="type">Type Token.</param>
		/// <param name="index">The Beginning Token in text.</param>
		/// <param name="length">The Length Token.</param>
		public StiToken(StiTokenType type, int index, int length)
		{
			this.Type = type;
			this.Index = index;
			this.Length = length;
		}

		/// <summary>
		/// Creates a new object of the type StiToken.
		/// </summary>
		/// <param name="type">Type Token.</param>
		/// <param name="index">The Beginning Token in text.</param>
		/// <param name="length">The Length Token.</param>
		/// <param name="charValue">Char for initializing</param>
		public StiToken(StiTokenType type, int index, int length, char charValue) : this(type, index, length)
		{
			this.Data = charValue;
		}
		
		/// <summary>
		/// Creates an object of the type StiToken that contains the value of the string.
		/// </summary>
		/// <param name="type">Type Token.</param>
		/// <param name="index">The Beginning Token in text.</param>
		/// <param name="length">The Length Token.</param>
		/// <param name="stringValue">String for initializing.</param>
		public StiToken(StiTokenType type, int index, int length, string stringValue) : this(type, index, length)
		{
			this.Data = stringValue;
		}

		/// <summary>
		/// Creates an object of the type StiToken that contains an object.
		/// </summary>
		/// <param name="type">Type Token</param>
		/// <param name="index">The Beginning Token in text.</param>
		/// <param name="length">The Length Token.</param>
		/// <param name="obj">Object for initializing.</param>
		public StiToken(StiTokenType type, int index, int length, object obj) : this(type, index, length)
		{
			this.Data = obj;
		}
	}
}
