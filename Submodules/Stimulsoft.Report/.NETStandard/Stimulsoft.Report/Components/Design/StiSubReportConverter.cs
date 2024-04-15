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
using System.Globalization;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Helpers;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Provides a type converter to convert SubReport to string.
	/// </summary>
	public class StiSubReportConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, 
			Type destinationType)
		{
			if (destinationType == typeof(string))
                return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
            var subReport = context.Instance as StiSubReport;
            if (subReport == null)
                return Loc.Get("FormStyleDesigner", "NotSpecified");

            else if (subReport.SubReportPage != null)
                return $"{Loc.Get("FormViewer", "LabelPageN")} {subReport.SubReportPage.Name}";

            else if (!string.IsNullOrWhiteSpace(subReport.SubReportUrl))
            {
                var fileName = StiHyperlinkProcessor.GetFileNameFromHyperlink(subReport.SubReportUrl);
                if (fileName != null)
                    return $"{Loc.GetMain("File")}: {fileName}";

                else
                    return $"{Loc.GetMain("Hyperlink")}: {subReport.SubReportUrl}";
            }

            else
                return Loc.Get("FormStyleDesigner", "NotSpecified");
        }
	}
}
