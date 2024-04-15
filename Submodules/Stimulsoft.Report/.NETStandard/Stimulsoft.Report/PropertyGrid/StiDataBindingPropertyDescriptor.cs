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
using System.ComponentModel;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Base.Design
{
	/// <summary>
	/// Provides an abstraction of a property on a class.
	/// </summary>
	public class StiDataBindingPropertyDescriptor : StiPropertyDescriptor
	{
		/// <summary>
		/// Returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>true if resetting the component changes its value; otherwise, false.</returns>
		public override bool CanResetValue(object component) 
		{
			return PropertyDescriptor.CanResetValue(((StiDataBindingsCollection)component).Control);
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>The value of a property for a given component.</returns>
		public override object GetValue(object component) 
		{
			return PropertyDescriptor.GetValue(((StiDataBindingsCollection)component).Control);
		}

		/// <summary>
		/// Resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component) 
		{
			PropertyDescriptor.ResetValue(((StiDataBindingsCollection)component).Control);
		}

		/// <summary>
		/// Sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value.</param>
		public override void SetValue(object component, object value) 
		{
			PropertyDescriptor.SetValue(((StiDataBindingsCollection)component).Control, value);
		}

		/// <summary>
		/// Determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <param name="component">The component with the property to be examined for persistence.</param>
		/// <returns>true if the property should be persisted; otherwise, false.</returns>
		public override bool ShouldSerializeValue(object component) 
		{
			return PropertyDescriptor.ShouldSerializeValue(((StiDataBindingsCollection)component).Control);
		}

		/// <summary>
		/// Gets the name that can be displayed in a window, such as a Properties window.
		/// </summary>
		public override string DisplayName 
		{ 
			get 
			{
				var displayName = PropertyDescriptor.DisplayName;
                displayName = displayName.Substring(0, displayName.IndexOf("Binding", StringComparison.InvariantCulture));

				if (!IsLocalizableProperties)
				    return displayName;

			    var name = StiLocalization.Get(
			        "PropertyMain", PropertyDescriptor.ComponentType.Name + displayName, false);

			    if (name == null)
			        name = StiLocalization.Get("PropertyMain", displayName, false);

			    return name ?? displayName;
			}
		}

		/// <summary>
		/// Initializes a new instance of the PropertyDescriptor class with the name and attributes in the specified PropertyDescriptor.
		/// </summary>
		/// <param name="propertyDescriptor">Specified PropertyDescriptor.</param>
		public StiDataBindingPropertyDescriptor(PropertyDescriptor propertyDescriptor) : base(propertyDescriptor)
		{
		}
	}

}
