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
using Stimulsoft.Report.BarCodes;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
	/// Converts image to string description. 
    /// </summary>
    public class StiSimpeImageConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                var str = ConvertForImage(context);
                if (str != null) return str;

                str = ConvertForSignatureImage(context);
                if (str != null) return str;

                str = ConvertForReport(context);
                if (str != null) return str;

                str = ConvertToBarCode(context);
                if (str != null) return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static string ConvertForImage(ITypeDescriptorContext context)
	    {
            var image = context.Instance as StiImage;
	        if (image == null) return null;

	        if (image.ExistImage() || !string.IsNullOrEmpty(image.DataColumn) ||
	            !string.IsNullOrEmpty(image.ImageData.Value) ||
	            !string.IsNullOrEmpty(image.ImageURL.Value) ||
	            !string.IsNullOrEmpty(image.File))
	        {
	            return $"({Loc.Get("PropertyMain", "Image")})";
	        }

	        return $"[{Loc.Get("Report", "NotAssigned")}]";
	    }

        private static string ConvertForReport(ITypeDescriptorContext context)
        {
            var report = context.Instance as StiReport;
            if (report == null) return null;

            return report.ReportImage != null 
                ? $"({Loc.Get("PropertyMain", "Image")})"
                : $"[{Loc.Get("Report", "NotAssigned")}]";
        }

        private static string ConvertForSignatureImage(ITypeDescriptorContext context)
        {
            var signatureImage = context.Instance as StiSignatureImage;
            if (signatureImage == null) return null;

            return signatureImage.Image != null
                ? $"({Loc.Get("PropertyMain", "Image")})"
                : $"[{Loc.Get("Report", "NotAssigned")}]";
        }

        private static string ConvertToBarCode(ITypeDescriptorContext context)
        {
            var qrCode = context.Instance as StiQRCodeBarCodeType;
            if (qrCode == null) return null;

            return qrCode.Image != null 
                ? $"({Loc.Get("PropertyMain", "Image")})"
                : $"[{Loc.Get("Report", "NotAssigned")}]";
        }
    }
}
