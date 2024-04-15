﻿#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using System.Globalization;
using Stimulsoft.Base.Json;

namespace Stimulsoft.Base
{
    public static class StiJsonHelper
    {
        #region Properties
        private static JsonSerializerSettings defaultSerializerSettings;
        public static JsonSerializerSettings DefaultSerializerSettings
        {
            get
            {
                return defaultSerializerSettings ?? (defaultSerializerSettings = new JsonSerializerSettings
                {
                    Culture = new CultureInfo("en-US", false),
                    DefaultValueHandling = DefaultValueHandling.Populate | DefaultValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    NullValueHandling = NullValueHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                });
            }
        }
        #endregion

        #region Methods
        public static string SaveToJsonString(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, DefaultSerializerSettings);
        }

        public static void LoadFromJsonString(string str, object value)
        {
            JsonConvert.PopulateObject(str, value, DefaultSerializerSettings);
        }
        #endregion
    }
}
