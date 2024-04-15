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

using System;
using System.Text;

namespace Stimulsoft.Report.Web
{
    internal static class StiEncodingHelper
    {
        #region Methods.Encode/Decode
        public static string DecodeString(string xml)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(xml))
                    result = Encoding.UTF8.GetString(Convert.FromBase64String(xml));
            }
            catch { }

            return result;
        }

        public static string Encode(string xml)
        {
            return xml != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)) : null;
        }
        #endregion
    }
}