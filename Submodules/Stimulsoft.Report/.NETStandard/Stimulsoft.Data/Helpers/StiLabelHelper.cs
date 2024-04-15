#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Base.Meters;
using Stimulsoft.Data.Functions;
using System.Collections.Generic;
using System.Linq;

using Stimulsoft.Data.Engine;

namespace Stimulsoft.Data.Helpers
{
    public static class StiLabelHelper
    {
        #region Fields
        private static Dictionary<string, string> cache = new Dictionary<string, string>();
        #endregion

        #region Methods
        public static string GetLabel(IStiMeter meter)
        {
            if (meter == null)
                return "Unknown";

            if (!string.IsNullOrEmpty(meter.Label))
                return meter.Label;

            var localizedName = (meter as IStiLocalizedMeter)?.LocalizedName;

            try
            {
                lock (cache)
                {
                    if (!string.IsNullOrWhiteSpace(meter.Expression) && cache.ContainsKey(meter.Expression))
                        return cache[meter.Expression];

                    var argument = StiExpressionHelper.GetFirstArgumentFromExpression(meter.Expression);

                    if (argument == null)
                        argument = StiExpressionHelper.GetFunction(meter.Expression);

                    if (argument == null && localizedName != null)
                        argument = localizedName;

                    if (argument.Contains("."))
                        argument = argument.Split('.').LastOrDefault();

                    if (!(argument.Length > 0 && char.IsUpper(argument[0])))
                        argument = Funcs.ToProperCase(argument);

                    if (!string.IsNullOrWhiteSpace(meter.Expression))
                        cache.Add(meter.Expression, argument);

                    return argument;
                }
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(meter.Expression))
                    cache.Add(meter.Expression, localizedName);

                return localizedName;
            }
        }
        #endregion
    }
}