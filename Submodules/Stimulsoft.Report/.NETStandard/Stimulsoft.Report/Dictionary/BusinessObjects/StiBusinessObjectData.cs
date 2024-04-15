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

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Class which contains information about business object which registered in report.
    /// </summary>
    public class StiBusinessObjectData
    {
        /// <summary>
        /// Gets or sets category name of business object.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets name of business object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets alias of business object.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets business object.
        /// </summary>
        public object BusinessObjectValue { get; set; }

        /// <summary>
        /// Creates new instance of StiBusinessObject class.
        /// </summary>
        /// <param name="category">Category of business object.</param>
        /// <param name="name">Name of business object.</param>
        /// <param name="value">Business object.</param>
        public StiBusinessObjectData(string category, string name, object value)
            : this(category, name, name, value)
        {
        }

        /// <summary>
        /// Creates new instance of StiBusinessObject class.
        /// </summary>
        /// <param name="category">Category of business object.</param>
        /// <param name="name">Name of business object.</param>
        /// <param name="alias">Alias of business object.</param>
        /// <param name="value">Business object.</param>
        public StiBusinessObjectData(string category, string name, string alias, object value)
        {
            this.Category = category;
            this.Name = name;
            this.Alias = alias;
            this.BusinessObjectValue = value;
        }
    }
}
