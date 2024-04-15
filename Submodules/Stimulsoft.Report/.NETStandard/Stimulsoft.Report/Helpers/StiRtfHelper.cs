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
using System.Text;

namespace Stimulsoft.Report.Helpers
{
    public class StiRtfHelper
    {
        /* From specification:
             "The control word \rtf<N> must follow the opening brace. 
             The numeric parameter N identifies the major version of the RTF Specification used. 
             The RTF standard described in this Application Note, although titled as version 1.9, continues to correspond syntactically to RTF Specification version 1." 
           But in practice N can be omitted, so \rtf, \rtf0, \rtf1 variants should work.
        */

        public static bool IsRtfText(string str)
        {
            return str != null && str.StartsWith("{\\rtf");
        }

        public static bool IsRtfBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 6) return false;

            return 
                bytes[0] == '{' || 
                bytes[1] == '\\' || 
                bytes[2] == 'r' || 
                bytes[3] == 't' || 
                bytes[4] == 'f';
        }

        public static string XmlDecodeFast(string str)
        {
                return StiXmlDecodeFastHelper.XmlDecodeFast(str);
        }

    }
}
