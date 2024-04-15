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
	/// Specifies a localization key in the stimulsoft localization which should be used for localizing 
    /// a property for which this attribute is applied to.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class StiLocalizationKeyAttribute : Attribute
	{
        #region Properties
        /// <summary>
        /// Gets or sets a localization category for the property that this attribute is applied to.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets a localization key for the property that this attribute is applied to.
        /// </summary>
        public string Key { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the StiCategoryAttribute class using the category name Default.
        /// </summary>
        /// <param name="category">A localization category for the property that this attribute is applied to.</param>
        /// <param name="key">A localization key for the property that this attribute is applied to.</param>
        public StiLocalizationKeyAttribute(string category, string key)
		{
			this.Category = category;
			this.Key = key;
		}
	}
}
