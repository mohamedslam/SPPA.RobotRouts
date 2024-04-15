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
	/// Class describes a class for store type with name.
	/// </summary>
	public struct StiNameTypeEntry
	{
	    /// <summary>
		/// Gets or sets the Type.
		/// </summary>
		public Type Type { get; }

	    /// <summary>
		/// Gets or sets the Name.
		/// </summary>
		public string Name { get; }

	    /// <summary>
		/// Creates a new instance of the StiNameTypeEntry class.
		/// </summary>
		public StiNameTypeEntry(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}
	}
}
