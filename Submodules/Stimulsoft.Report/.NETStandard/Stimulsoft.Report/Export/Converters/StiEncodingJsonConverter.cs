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

namespace Stimulsoft.Report.Export
{
    public sealed class StiEncodingJsonConverter : JsonConverter
    {
        #region Methods
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var encoding = value as Encoding;
            if (encoding != null)
            {
                writer.WriteValue(encoding.CodePage);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object value, JsonSerializer serializer)
        {
            if (reader.Value != null && reader.ValueType == typeof(Int64))
            {
                try
                {
                    var codePage = Convert.ToInt32(reader.Value);
                    return Encoding.GetEncoding(codePage);
                }
                catch
                {
                }
            }

            return Encoding.Default;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Encoding);
        }
        #endregion
    }
}
