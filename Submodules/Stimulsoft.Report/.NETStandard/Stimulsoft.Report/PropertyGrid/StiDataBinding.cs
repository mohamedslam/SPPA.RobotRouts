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

namespace Stimulsoft.Base.Design
{	
	/// <summary>
	/// Class describes databinding properties.
	/// </summary>
	public class StiDataBinding
	{
	    /// <summary>
		/// Gets collection of DataBinding.
		/// </summary>
		public StiDataBindingsCollection Collection { get; }

	    /// <summary>
		/// Gets property name.
		/// </summary>
		public string PropertyName { get; }

	    /// <summary>
		/// Gets display name.
		/// </summary>
		public string DisplayName { get; }

	    /// <summary>
		/// Creates an object of the type StiDataBinding that contains an object.
		/// </summary>
		/// <param name="collection">Collection of DataBinding.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="displayName">Display name.</param>
		public StiDataBinding(StiDataBindingsCollection collection, string propertyName, string displayName)
		{
			this.Collection = collection;
			this.PropertyName = propertyName;
			this.DisplayName = displayName;
		}
	}
}
