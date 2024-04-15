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
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Data;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Base.Design
{
	/// <summary>
	/// Describes a collection of StiDataBinding.
	/// </summary>
	[ParenthesizePropertyName(true)]
	[TypeConverter(typeof(Stimulsoft.Base.Design.StiDataBindingsCollectionConverter))]
	[Editor("Stimulsoft.Base.Design.StiDataBindingsCollectionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
	public class StiDataBindingsCollection : CollectionBase
	{
		#region Collection
		public void Add(StiDataBinding binding)
		{
			List.Add(binding);
		}

		public void Add(string propertyName, string displayName)
		{
			List.Add(new StiDataBinding(this, propertyName, displayName));
		}

		public void Remove(StiDataBinding binding)
		{
			List.Remove(binding);
		}
		#endregion

		#region Properties
	    public object Control { get; }
        #endregion

        public StiDataBindingsCollection() : this(null)
		{
		}

		public StiDataBindingsCollection(object control)
		{
			this.Control = control;
		}
	}
}
