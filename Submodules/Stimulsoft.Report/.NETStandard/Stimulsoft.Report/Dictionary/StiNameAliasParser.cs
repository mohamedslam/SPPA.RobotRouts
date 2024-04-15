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

namespace Stimulsoft.Report
{
    public static class StiNameAliasParser
    {
        public static string GetNameFromText(string text)
        {
            var index = text.IndexOf("[");
            if (index == -1)
                return text;

            return text.Substring(0, index).TrimEnd();
        }

        public static string GetAliasFromText(string text)
        {
            var index = text.IndexOf("[");
            if (index == -1)
                return text;

            var second = text.Substring(index + 1);
            var secondIndex = second.IndexOf("]");
            if (secondIndex != -1)
                second = second.Substring(0, secondIndex).TrimEnd();

            return second;
        }
    }
}