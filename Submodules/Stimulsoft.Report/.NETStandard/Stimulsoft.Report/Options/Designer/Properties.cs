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
using Stimulsoft.Base.Design;

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
            public class Properties
            {
                #region Methods
                public static void Hide(Type type, string propertyName)
                {
                    StiPropertyGrid.HideProperty(type, propertyName);
                }

                public static void Hide(string propertyName)
                {
                    StiPropertyGrid.HideProperty("All", propertyName);
                }

                public static void Show(Type type, string propertyName)
                {
                    StiPropertyGrid.ShowProperty(type, propertyName);
                }

                public static void Show(string propertyName)
                {
                    StiPropertyGrid.ShowProperty("All", propertyName);
                }

                public static bool IsAllowed(Type type, string propertyName)
                {
                    return StiPropertyGrid.IsAllowedProperty(type, propertyName);
                }
                #endregion
            }
        }
    }
}