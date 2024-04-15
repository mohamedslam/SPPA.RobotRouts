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
    public class StiXmlDecodeFastHelper
    {
        public static string XmlDecodeFast(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (str.IndexOf('_') == -1) return str;
            unchecked
            {
                StringBuilder sbo = new StringBuilder();
                for (int index = 0; index < str.Length; index++)
                {
                    char ch = str[index];
                    if (ch == '_')
                    {
                        if ((index + 6 < str.Length) && (str[index + 1] == 'x') && (str[index + 6] == '_'))
                        {
                            int c1 = GetValueOfHexChar(str[index + 2]);
                            int c2 = GetValueOfHexChar(str[index + 3]);
                            int c3 = GetValueOfHexChar(str[index + 4]);
                            int c4 = GetValueOfHexChar(str[index + 5]);
                            if (c1 >= 0 && c2 >= 0 && c3 >= 0 && c4 >= 0)
                            {
                                int vv = (c1 << 12) | (c2 << 8) | (c3 << 4) | c4;
                                sbo.Append((char)vv);
                                index += 6;
                                continue;
                            }
                        }
                    }
                    sbo.Append(ch);
                }
                return sbo.ToString();
            }
        }

        private static int GetValueOfHexChar(int ch)
        {
            unchecked
            {
                if (ch >= 48 && ch <= 57) return ch - 48;
                if (ch >= 65 && ch <= 70) return ch - 55;
                if (ch >= 97 && ch <= 102) return ch - 87;
                return -1;
            }
        }
    }
}
