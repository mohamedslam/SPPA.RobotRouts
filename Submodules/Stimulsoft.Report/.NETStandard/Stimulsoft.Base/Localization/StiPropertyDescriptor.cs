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
using System.Drawing;
using Stimulsoft.Base.Design;

namespace Stimulsoft.Base.Localization
{
	/// <summary>
	/// Provides an abstraction of a property on a class.
	/// </summary>
	public class StiPropertyDescriptor : PropertyDescriptor
    {
        #region Events
        public static event StiProcessDescriptionEventHandler ProcessDescription;

        public static void InvokeProcessDescription(object sender, StiProcessDescriptionEventArgs e)
        {
            ProcessDescription?.Invoke(sender, e);
        }
        #endregion

        #region Properties
        protected PropertyDescriptor PropertyDescriptor { get; set; }

        private bool IsEventDescriptor { get; set; }

        protected bool IsLocalizableProperties { get; set; }

        internal int Level { get; set; }

        internal int MaxLevel { get; set; }

        public override string Description
        {
            get
            {                
                var e = new StiProcessDescriptionEventArgs(this.ComponentType, this.Name, base.Description);

                InvokeProcessDescription(this, e);
                return e.Description;
            }
        }

        /// <summary>
		/// Gets the name of the category to which the member belongs, as specified in the StiCategoryAttribute.
		/// </summary>
		public override string Category
		{ 
			get
			{
				var category = PropertyDescriptor.Attributes[typeof(StiCategoryAttribute)] as StiCategoryAttribute;
                var categorySource = category == null ? StiCategorySource.PropertyCategory : category.Source;
			    var categoryName = category == null ? "Misc" : category.Category;

				if (!IsLocalizableProperties)
				{
					categoryName = UpdateCategoryName(categoryName);

				    if (Level != 0)
				    {
				        return MaxLevel != 0 && MaxLevel > 9 && Level < 10 
				            ? $"0{Level}. {categoryName}"
				            : $"{Level}. {categoryName}";
				    }

				    return categoryName;
				}

                switch (categorySource)
                {
                    case StiCategorySource.PropertyCategory:
                        if (!categoryName.EndsWithInvariant("Category"))
                            categoryName += "Category";

                        categoryName = StiLocalization.Get("PropertyCategory", categoryName);
                        break;

                    case StiCategorySource.Components:
                        if (!categoryName.StartsWithInvariant("Sti"))
                            categoryName = "Sti" + categoryName;

                        categoryName = StiLocalization.Get("Components", categoryName);
                        break;

                    default:
                        categoryName = StiLocalization.Get("PropertyMain", categoryName);
                        break;
                }

                if (categoryName != null)
				{
					categoryName = UpdateCategoryName(categoryName);

					if (Level != 0)
					{
					    return MaxLevel != 0 && MaxLevel > 9 && Level < 10 
					        ? $"0{Level}. {categoryName}" 
					        : $"{Level}. {categoryName}";
					}
					return categoryName;
				}

                return categoryName;
			} 
		}

        /// <summary>
        /// Gets the name of the category to which the member belongs, as specified in the StiCategoryAttribute without localization.
        /// </summary>
        public string OriginalCategory
        {
            get
            {
                var category = PropertyDescriptor.Attributes[typeof(StiCategoryAttribute)] as StiCategoryAttribute;

                return category == null ? "Misc" : category.Category;
            }
        }
        
        /// <summary>
		/// Gets the name that can be displayed in a window, such as a Properties window.
		/// </summary>
		public override string DisplayName
		{ 
			get
			{
			    return !IsLocalizableProperties 
			        ? PropertyDescriptor.DisplayName 
			        : LocalizedName;
			}
		}

        public string LocalizedName
        {
            get
            {
                if (IsLocalizableProperties)
                {
                    var localizationKey = PropertyDescriptor.Attributes[typeof(StiLocalizationKeyAttribute)] as StiLocalizationKeyAttribute;
                    if (localizationKey != null)
                        return StiLocalization.Get(localizationKey.Category, localizationKey.Key);
                }
                
                if (!IsEventDescriptor)
                {
                    var key = PropertyDescriptor.ComponentType.Name + PropertyDescriptor.Name;

                    var name = PropertyDescriptor.Name == "Category" ? null : StiLocalization.Get("PropertyMain", key, false);

                    if (name == null)
                        name = StiLocalization.Get("PropertyMain", PropertyDescriptor.Name, false);

                    return name ?? PropertyDescriptor.DisplayName;
                }
                else
                {
                    var name = StiLocalization.Get("PropertyEvents", PropertyDescriptor.Name, false);
                    return name ?? PropertyDescriptor.DisplayName;
                }
            }
        }

        public override TypeConverter Converter
        {
            get
            {
                if (PropertyType == typeof(bool) && !(base.Converter is StiExpressionBoolConverter))
                    return new StiBoolConverter();

                return base.Converter;
            }
        }

        public override object GetEditor(Type editorBaseType)
        {
            var editor = GetFontBoolEditor();
            if (editor != null)
                return editor;

            editor = GetFontNameEditor();
            if (editor != null)
                return editor;

            return base.GetEditor(editorBaseType);
        }

        private object GetFontBoolEditor()
        {
            if (PropertyType != typeof(bool) || ComponentType != typeof(Font))
                return null;

            var typeEditor = StiTypeFinder.GetType(StiEditors.Bool);
            if (typeEditor == null)            
                return null;
            
            return StiActivator.CreateObject(typeEditor);
        }

        private object GetFontNameEditor()
        {
            if (PropertyType != typeof(string) || Name != "Name" || ComponentType != typeof(Font))
                return null;

            var typeEditor = StiTypeFinder.GetType(StiEditors.FontName);
            if (typeEditor == null)            
                return null;
            
            return StiActivator.CreateObject(typeEditor);
        }

        /// <summary>
        /// Gets a type of the component.
        /// </summary>
        public override Type ComponentType => PropertyDescriptor.ComponentType;

        /// <summary>
        /// Gets a value indicating whether this property is read-only.
        /// </summary>
        public override bool IsReadOnly => PropertyDescriptor.IsReadOnly;

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType => PropertyDescriptor.PropertyType;
        #endregion

        #region Methods
        public override string ToString()
        {
            return DisplayName;
        }

        private string UpdateCategoryName(string name)
        {
            int index = 1;
            while (index < name.Length)
            {
                if (char.IsUpper(name[index]))
                {
                    name = name.Insert(index, " ");
                    index++;
                }
                index++;
            }

            return name;
        }
        
        /// <summary>
		/// Returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>true if resetting the component changes its value; otherwise, false.</returns>
		public override bool CanResetValue(object component) 
		{
			return PropertyDescriptor.CanResetValue(component);
		}

		/// <summary>
		/// Gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>The value of a property for a given component.</returns>
		public override object GetValue(object component) 
		{
			return PropertyDescriptor.GetValue(component);
		}

		/// <summary>
		/// Resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component) 
		{
			PropertyDescriptor.ResetValue(component);
		}

		/// <summary>
		/// Sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value.</param>
		public override void SetValue(object component, object value) 
		{
			PropertyDescriptor.SetValue(component, value);
		}

		/// <summary>
		/// Determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <param name="component">The component with the property to be examined for persistence.</param>
		/// <returns>true if the property should be persisted; otherwise, false.</returns>
		public override bool ShouldSerializeValue(object component) 
		{
			return PropertyDescriptor.ShouldSerializeValue(component);
        }
        #endregion

        /// <summary>
		/// Initializes a new instance of the StiPropertyDescriptor class with the name and attributes in the specified PropertyDescriptor.
		/// </summary>
		/// <param name="propertyDescriptor">Specified PropertyDescriptor.</param>
        public StiPropertyDescriptor(PropertyDescriptor propertyDescriptor)
            : this(propertyDescriptor, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StiPropertyDescriptor class with the name and attributes in the specified StiPropertyDescriptor.
        /// </summary>
        /// <param name="propertyDescriptor">Specified SPropertyDescriptor.</param>
        public StiPropertyDescriptor(PropertyDescriptor propertyDescriptor, bool isEventDescriptor) 
            : base(propertyDescriptor)
		{
			this.IsLocalizableProperties = StiPropertyGridOptions.Localizable;
			this.PropertyDescriptor = propertyDescriptor;
            this.IsEventDescriptor = isEventDescriptor;
		}
	}

}
