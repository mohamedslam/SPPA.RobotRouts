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
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Stimulsoft.Report.Dictionary.Design
{
    /// <summary>
    /// Converts StiResource from one data type to another. 
    /// </summary>
    public class StiResourceFileConverter : StiExtConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            #region InstanceDescriptor
            if (destinationType == typeof(InstanceDescriptor) && value != null)
            {
                var resource = value as StiResource;
                var types = new[]
                {
                    typeof(string),
                    typeof(string),
                    typeof(StiResourceType),
                    typeof(byte[]),
                    typeof(bool)
                };

                var info = typeof(StiResource).GetConstructor(types);
                if (info == null)
                    return base.ConvertTo(context, culture, value, destinationType);

                var objs = new object[]
                {
                    resource.Name,
                    resource.Alias,
                    resource.Type,
                    resource.Content,
                    resource.AvailableInTheViewer
                };
                return CreateNewInstanceDescriptor(info, objs);
            }
            #endregion

            #region String
            if (destinationType == typeof(string))
            {
                var resource = value as StiResource;
                if (resource != null)
                {
                    var fileName = Encode("Name", resource.Name);
                    var fileAlias = Encode("Alias", resource.Alias);
                    var fileKey = Encode("Key", resource.Key);
                    var fileInherited = EncodeBool("Inherited", resource.Inherited, false);
                    var fileType = EncodeEnum("Type", resource.Type, StiResourceType.Image);
                    var fileAvailableInTheViewer = EncodeBool("AvailableInTheViewer", resource.AvailableInTheViewer, false);
                    var fileContent = EncodeBytes("Content", resource.Content);

                    return Encode(fileName, fileAlias, fileAvailableInTheViewer, fileKey, fileInherited, fileType, fileContent);
                }
            }
            #endregion

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var resource = new StiResource();

                var elements = DecodeElements(value as string);
                if (elements != null)
                {
                    foreach (var element in elements)
                    {
                        if (element.Key == "name") resource.Name = element.Value;
                        else if (element.Key == "alias") resource.Alias = element.Value;
                        else if (element.Key == "key") resource.Key = element.Value;
                        else if (element.Key == "inherited") resource.Inherited = element.Value == "True";
                        else if (element.Key == "type") resource.Type = DecodeEnum<StiResourceType>(element.Value, StiResourceType.Image);
                        else if (element.Key == "availableintheviewer") resource.AvailableInTheViewer = element.Value == "True";
                        else if (element.Key == "content") resource.Content = DecodeBytes(element.Value);
                    }
                }

                return resource;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
