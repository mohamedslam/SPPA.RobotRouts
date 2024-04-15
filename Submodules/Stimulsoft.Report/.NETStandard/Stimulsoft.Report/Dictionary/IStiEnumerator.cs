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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes inteface that is used for moving in the data list.
	/// </summary>
	public interface IStiEnumerator
	{
		/// <summary>
		/// Sets a position at the beginning.
		/// </summary>
		void First();

		/// <summary>
		/// Sets a position on the previous element.
		/// </summary>
		void Prior();

		/// <summary>
		/// Sets a position on the next element.
		/// </summary>
		void Next();

		/// <summary>
		/// Sets a position on the last element.
		/// </summary>
		void Last();

		/// <summary>
		/// Gets the current position.
		/// </summary>
		int Position { get; set; }

		/// <summary>
		/// Gets count of elements.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets or sets value indicates that this position specifies to the data end.
		/// </summary>
		bool IsEof { get; set; }

		/// <summary>
		/// Gets or sets value indicates that this position specifies to the beginning of data.
		/// </summary>
		bool IsBof  { get; set; }

		/// <summary>
		/// Gets value indicates that no data.
		/// </summary>
		bool IsEmpty { get; }
	}
}
