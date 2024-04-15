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

using Stimulsoft.Base.Localization;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Stimulsoft.Report.Components.Design
{
    /// <summary>
    /// Converts icon to string description. 
    /// </summary>
    public class StiReportIconConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var report = context.Instance as StiReport;
                if (report != null)
                {
                    return report.ReportIcon != null
                        ? $"({Loc.Get("PropertyMain", "Icon")})"
                        : $"[{Loc.Get("Report", "NotAssigned")}]";
                }

                var page = context.Instance as StiPage;
                if (page != null)
                {
                    return page.Icon != null
                        ? $"({Loc.Get("PropertyMain", "Icon")})"
                        : $"[{Loc.Get("Report", "NotAssigned")}]";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
