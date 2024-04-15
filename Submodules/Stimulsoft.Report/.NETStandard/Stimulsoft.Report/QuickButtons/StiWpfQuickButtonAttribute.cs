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
using System.Collections;

namespace Stimulsoft.Report.QuickButtons
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class StiWpfQuickButtonAttribute : Attribute
	{
        #region class Sorter
        public class Sorter : IComparer
		{
			int IComparer.Compare(object x, object y)
			{
                var attr1 = x as StiWpfQuickButtonAttribute;
                var attr2 = y as StiWpfQuickButtonAttribute;

				return -attr1.Order.CompareTo(attr2.Order);
			}
		}
		#endregion

		#region Properties
	    public string QuickButtonTypeName { get; }

	    public int Order { get; }
	    #endregion
		
		public StiWpfQuickButtonAttribute(Type type) : this(type, -1)
		{
		}

		public StiWpfQuickButtonAttribute(Type type, int order) : this(type.AssemblyQualifiedName, order)
		{
		} 

		public StiWpfQuickButtonAttribute(string quickButtonTypeName) : this(quickButtonTypeName, -1)
		{
		}

        public StiWpfQuickButtonAttribute(string quickButtonTypeName, int order)
		{
			this.QuickButtonTypeName = quickButtonTypeName;
			this.Order = order;				 
		}
	}
}
