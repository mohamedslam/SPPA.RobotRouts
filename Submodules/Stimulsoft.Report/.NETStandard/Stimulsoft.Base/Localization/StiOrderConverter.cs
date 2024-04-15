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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Stimulsoft.Base.Localization
{
	public class StiOrderConverter : ExpandableObjectConverter
	{
		#region class OrderData
		private class OrderData : IComparable
		{
			#region IComparable
			public int CompareTo(object obj)
			{
			    var secondValue = obj as OrderData;

			    return secondValue.Position == Position 
			        ? string.Compare(Name, secondValue.Name) 
			        : -secondValue.Position.CompareTo(Position);
			}
            #endregion

            #region Properties
		    public int Position { get; }

		    public string Name { get; }
            #endregion

            public OrderData(string name, int position)
			{
				this.Position = position;
				this.Name = name;
			}
		}
		#endregion

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			var properties = TypeDescriptor.GetProperties(value, attributes);
            var orderedProperties = new List<OrderData>();

			foreach (PropertyDescriptor property in properties)
			{
				var attribute = property.Attributes[typeof(StiOrderAttribute)] as StiOrderAttribute;

			    if (attribute != null)
			        orderedProperties.Add(new OrderData(property.Name, attribute.Position));

			    else
			        orderedProperties.Add(new OrderData(property.Name, 0));
			}

			orderedProperties.Sort();
            return properties.Sort(orderedProperties.Select(orderData => orderData.Name).ToArray());
		}
	}
}
