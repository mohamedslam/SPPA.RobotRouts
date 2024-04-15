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
using Stimulsoft.Report.Dictionary;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// A class which controls settings of a dictionary in the report.
        /// </summary>
        public sealed partial class Dictionary
		{
            /// <summary>
			/// A Class which controls Enumerable DataSources of the report.
			/// </summary>
			public sealed class Enumerables
			{
                /// <summary>
                /// Gets or sets a value, which controls processing of properties in business objects.
                /// </summary>
                [Obsolete("Please use property StiOptions.Dictionary.BusinessObjects.PropertiesProcessingType")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static StiPropertiesProcessingType PropertiesProcessingType
				{
					get
					{
						return BusinessObjects.PropertiesProcessingType;
					}
					set
					{
                        BusinessObjects.PropertiesProcessingType = value;
					}
				}

                /// <summary>
                /// Gets or sets a delimiter between a name of the enumerable data source
                /// and its column names for the child tables in the master-detail relation.
                /// </summary>
                [Obsolete("Please use property StiOptions.Dictionary.BusinessObjects.Delimeter")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static char Delimeter
				{
					get
					{
                        return BusinessObjects.Delimeter;
					}
					set
					{
                        BusinessObjects.Delimeter = value;
					}
				}

                /// <summary>
                /// Gets or sets the maximal nested level of master-detail relations in enumerable sources.
                /// </summary>
                [Obsolete("Please use property StiOptions.Dictionary.BusinessObjects.MaxLevel")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static int MaxLevel
				{
					get
					{
                        return BusinessObjects.MaxLevel;
					}
					set
					{
                        BusinessObjects.MaxLevel = value;
					}
				}	

                /// <summary>
                /// Gets or sets a value, which controls that enumerable sources with the same names.
                /// </summary>
                [Obsolete("Please use property StiOptions.Dictionary.BusinessObjects.CheckTableDuplication")]
                [EditorBrowsable(EditorBrowsableState.Never)]
                public static bool CheckTableDuplication
				{
					get
					{
                        return BusinessObjects.CheckTableDuplication;
					}
					set
					{
                        BusinessObjects.CheckTableDuplication = value;
					}
				}
			}
		}		
    }
}