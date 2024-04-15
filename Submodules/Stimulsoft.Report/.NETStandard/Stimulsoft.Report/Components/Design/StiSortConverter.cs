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
using System.ComponentModel;
using System.Globalization;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Provides a type converter to convert sort rules to string.
	/// </summary>
	public class StiSortConverter : TypeConverter
	{
	    public string SortToString(string []sorts, StiDataSource dataSource)
		{
            if (dataSource == null || sorts == null || sorts.Length == 0)
                return $"[{Loc.Get("FormBand", "NoSort")}]";

			sorts = (string[])sorts.Clone();
			
			var s = $"{Loc.Get("FormBand", "SortBy")} ";
			var index = 0;
			while (index < sorts.Length)
			{
				var data = dataSource;
				var startIndex = index;

			    if (sorts[index] == "DESC")
				    s += sorts[index] + ' ';

				index++;
				while (index < sorts.Length - 1 && sorts[index + 1] != "ASC" && sorts[index + 1] != "DESC")
				{
					var relations = data.GetParentRelations();
				    foreach (StiDataRelation relation in relations)
				    {
				        if (relation.NameInSource != sorts[index]) continue;

				        sorts[index] = relation.Alias;
				        data = relation.ParentSource;
				        break;
				    }

				    s += sorts[index++];
					if (index - startIndex > 0)
					    s += '.';
				}
				s += sorts[index++];
				if (index < sorts.Length - 1)
				    s += ", ";
			}
			return s;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(string))return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			if (destinationType == typeof(string))
			{
				if (context.Instance is Array)
				{
					foreach (IStiDataSource data in (Array)context.Instance)
					{
						SortToString((string[])value, data.DataSource);
					}
				}
				else
				    return SortToString((string[])value, ((IStiDataSource)context.Instance).DataSource);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
