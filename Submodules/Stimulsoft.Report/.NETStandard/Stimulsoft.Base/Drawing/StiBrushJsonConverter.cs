#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Base.Drawing
{
    internal class StiBrushJsonConverter : CustomCreationConverter<StiBrush>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JToken.ReadFrom(reader);
            var ident = jsonObject.Cast<KeyValuePair<string, JToken>>().FirstOrDefault(a => a.Key == "Ident").Value.ToString();
            var brush = StiBrushCreator.New(ident);

            if (brush == null) return null;
            serializer.Populate(jsonObject.CreateReader(), brush);
            return brush;
        }

        public override StiBrush Create(Type objectType)
        {
            return Activator.CreateInstance(objectType, true) as StiBrush;
        }
    }
}