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
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;

namespace Stimulsoft.Report.Components.Design
{
	/// <summary>
    /// Converts StiIndicator from one data type to another. 
	/// </summary>
    public class StiIndicatorConverter : TypeConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false; 
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
			object value, Type destinationType) 
		{
			#region Convert to string
			if (destinationType == typeof(string))
            {
                #region StiDataBarIndicator
                var dataBar = value as StiDataBarIndicator;
                if (dataBar != null)
                {
                    return string.Format(
                                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                "DataBar",
                                XmlConvert.EncodeName(dataBar.BrushType.ToString()),
                                StiReportObjectStringConverter.ConvertColorToString(dataBar.PositiveColor),
                                StiReportObjectStringConverter.ConvertColorToString(dataBar.NegativeColor),
                                dataBar.ShowBorder,
                                StiReportObjectStringConverter.ConvertColorToString(dataBar.PositiveBorderColor),
                                StiReportObjectStringConverter.ConvertColorToString(dataBar.NegativeBorderColor),
                                XmlConvert.EncodeName(dataBar.Direction.ToString()),
                                dataBar.Value,
                                dataBar.Minimum,
                                dataBar.Maximum
                        );
                }
                #endregion

                #region StiIconSetIndicator
                var iconSet = value as StiIconSetIndicator;
                if (iconSet != null)
                {
                    return string.Format(
                                "{0},{1},{2}",
                                "IconSet",
                                XmlConvert.EncodeName(iconSet.Icon.ToString()),
                                XmlConvert.EncodeName(iconSet.Alignment.ToString())
                        );
                }
                #endregion
            }
			#endregion

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))return true; 
			return base.CanConvertFrom(context, sourceType); 
		} 

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType); 
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
		    var text = value as string;
            if (text != null)
			{
				var splits = new[]{ ',' };
				var words = text.Split(splits);

                #region StiDataBarIndicator
                if (words[0] == "DataBar")
                {
                    return 
                        new StiDataBarIndicator(
                            (StiBrushType)Enum.Parse(typeof(StiBrushType), XmlConvert.DecodeName(words[1])),
                            StiReportObjectStringConverter.ConvertStringToColor(words[2]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[3]),
                            words[4].ToLower(CultureInfo.InvariantCulture) == "true",
                            StiReportObjectStringConverter.ConvertStringToColor(words[5]),
                            StiReportObjectStringConverter.ConvertStringToColor(words[6]),
                            (StiDataBarDirection)Enum.Parse(typeof(StiDataBarDirection), XmlConvert.DecodeName(words[7])),
                            float.Parse(words[8]),
                            float.Parse(words[9]),
                            float.Parse(words[10]));
                }
                #endregion

                #region StiIconSetIndicator
                if (words[0] == "IconSet")
                {
                    return
                        new StiIconSetIndicator(
                            (StiIcon)Enum.Parse(typeof(StiIcon), XmlConvert.DecodeName(words[1])),
                            (ContentAlignment)Enum.Parse(typeof(ContentAlignment), XmlConvert.DecodeName(words[2])));
                }
                #endregion
            }
			return base.ConvertFrom(context, culture, value); 
		}
	}
}
