#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports  											}
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

using Stimulsoft.Base.Helpers;
using System.Text;

namespace Stimulsoft.Report.Web.Helpers.Dashboards
{
    public class StiManuallyDataHelper
    {
        internal static string ConvertJSDataToPackedString(string jsData)
        {
            if (string.IsNullOrEmpty(jsData)) return null;

            var bytes = Encoding.UTF8.GetBytes(jsData);
            return StiPacker.PackToString(bytes);
        }

        internal static string ConvertPackedStringToJSData(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var bytes = StiPacker.UnpackFromString(content);
            if (bytes == null || bytes.Length == 0)
                return null;

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
