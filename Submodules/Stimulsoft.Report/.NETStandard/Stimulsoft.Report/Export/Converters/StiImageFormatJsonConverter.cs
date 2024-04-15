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

using Stimulsoft.Base.Json;
using System;
using System.Text;
using System.Drawing.Imaging;

#if STIDRAWING
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    public sealed class StiImageFormatJsonConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var encoding = value as ImageFormat;
            if (encoding != null)
            {
                writer.WriteValue(encoding.Guid.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
        {
            if (reader.Value != null && reader.ValueType == typeof(string))
            {
                try
                {
                    var guid = reader.Value as string;
                    return new ImageFormat(new Guid(guid));
                }
                catch
                {
                }
            }

            return ImageFormat.Jpeg;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ImageFormat);
        }
        #endregion
    }
}
