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

using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;

namespace Stimulsoft.Report
{
    /// <summary>
    /// Class for adjustment all aspects of Stimulsoft Reports.
    /// </summary>
    public sealed partial class StiOptions
	{
        /// <summary>
        /// Class which allows adjustment of the Designer of the report.
        /// </summary>
        public sealed partial class Designer
        {
            /// <summary>
            /// Class for the Property Grid of the Report Designer.
            /// </summary>
            public sealed class PropertyGrid
			{
                #region Fields
                private static bool propertyLevelLoaded;
                #endregion

                #region Properties
                [DefaultValue(true)]
                [StiSerializable]
                public static bool AllowShowConfiguration { get; set; } = true;

                [DefaultValue(false)]
                [StiSerializable]
                public static bool AllowEditStyle { get; set; }

                private static StiLevel? propertyLevel;
                [DefaultValue(null)]
                [StiSerializable]
                public static StiLevel? PropertyLevel
                {
                    get
                    {
                        if (propertyLevelLoaded) return propertyLevel;
                        StiSettings.Load();
                        propertyLevelLoaded = true;
                        propertyLevel = (StiLevel?)StiSettings.Get("StiDesigner", "PropertyLevel", null);
                        StiPropertyGridOptions.PropertyLevel = propertyLevel;
                        return propertyLevel;
                    }
                    set
                    {
                        if (propertyLevelLoaded && propertyLevel == value) return;

                        StiSettings.Load();
                        StiSettings.Set("StiDesigner", "PropertyLevel", value);
                        propertyLevel = value;
                        propertyLevelLoaded = true;
                        StiPropertyGridOptions.PropertyLevel = propertyLevel;
                        StiSettings.Save();
                    }
                }

				/// <summary>
				/// Gets or sets a value, indicating whether to show the StiPropertyGrid description or not.
				/// </summary>
                [DefaultValue(true)]
                [Description("Gets or sets a value, indicating whether to show the StiPropertyGrid description or not.")]
                [StiSerializable]
                public static bool ShowDescription
				{
					get
					{
						return StiPropertyGridOptions.ShowDescription;
					}
					set
					{
						StiPropertyGridOptions.ShowDescription = value;
					}
				}

				/// <summary>
				/// Gets or sets a value, which indicates that the panel of properties is localizable.
				/// </summary>
                [DefaultValue(true)]
                [StiSerializable]
                public static bool Localizable
				{
					get
					{
						return StiPropertyGridOptions.Localizable;
					}
					set
					{
						StiPropertyGridOptions.Localizable = value;
					}
				}
                
                [DefaultValue(true)]
                [StiSerializable]
                public static bool ShowPropertiesWhichUsedFromStyles { get; set; }
                #endregion
            }
		}
    }
}