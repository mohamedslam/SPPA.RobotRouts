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

namespace Stimulsoft.Base.Localization
{
	/// <summary>
	/// Specifies the name of the category in which to group the property or event when displayed in a StiPropertyGrid control.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class StiCategoryAttribute : Attribute
	{
        #region Consts
        public const string Action = nameof(Action);
        public const string Appearance = nameof(Appearance);
        public const string Behavior = nameof(Behavior);
        public const string Button = nameof(Button);
        public const string CheckBox = "CheckBox";
        public const string Layout = nameof(Layout);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the category for the property or event that this attribute is applied to.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the source of the localization keys from which that category should be taken.
        /// </summary>
        public StiCategorySource Source { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the StiCategoryAttribute class using the category name Default.
        /// </summary>
        /// <param name="category">The name of the category for the property or event that this attribute is applied to.</param>
        /// <param name="source">The source of the localization keys from which that category should be taken.</param>
        public StiCategoryAttribute(string category, StiCategorySource source = StiCategorySource.PropertyCategory)
		{
			this.Category = category;
			this.Source = source;
		}
	}
}
