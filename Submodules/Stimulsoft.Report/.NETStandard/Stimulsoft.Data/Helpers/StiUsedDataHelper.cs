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
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Data.Helpers
{
    public class StiUsedDataHelper
    {
        public static List<string> GetMany(params IStiMeter[] meters)
        {
            if (meters == null)
                return new List<string>();

            return meters
                .Where(m => m != null)
                .SelectMany(GetSingle)
                .Distinct()
                .ToList();
        }

        public static List<string> GetMany(IEnumerable<IStiMeter> meters)
        {
            if (meters == null)
                return new List<string>();

            return meters
                .Where(m => m != null)
                .SelectMany(GetSingle)
                .Distinct()
                .ToList();
        }

        public static List<string> GetSingle(IStiMeter meter)
        {
            return GetSingle(meter.Expression);
        }

        public static List<string> GetSingle(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return new List<string>();

            try
            {
                return StiExpressionHelper.GetArguments(expression);
            }
            catch
            {
            }

            return new List<string>();
        }
    }
}