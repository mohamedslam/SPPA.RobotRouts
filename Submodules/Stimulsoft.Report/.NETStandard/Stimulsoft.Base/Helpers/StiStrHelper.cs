#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Text;

namespace Stimulsoft.Base.Helpers
{
    public static class StiStrHelper
	{
        public static string[] Split(string value)
		{
            int count = 1;
            for (int index = 0; index < value.Length; index++)
            {
                if (value[index] == ',' | value[index] == ';')
                    count++;
            }

            string[] strs = new string[count];
            int strIndex = 0;

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < value.Length; index++)
            {
                if (value[index] == ',' | value[index] == ';')
                {
                    strs[strIndex] = sb.ToString();
                    sb = new StringBuilder();
                    strIndex++;
                }
                else
                    sb = sb.Append(value[index]);
            }

            if (sb.Length > 0)
                strs[strIndex] = sb.ToString();

            return strs;

		}
    }
}
