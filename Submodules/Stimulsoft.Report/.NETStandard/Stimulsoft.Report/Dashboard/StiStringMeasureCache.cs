#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using System.Collections;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Dashboard
{
    public static class StiStringMeasureCache
    {
        #region Fields
        private static Hashtable stringToSize = new Hashtable();
        #endregion

        #region Methods
        public static SizeF? GetSize(Font font, string str)
        {
            if (str == null)
                return null;

            var hashCode = GetHashCode(font, str);

            if (stringToSize[hashCode] != null)
                return (SizeF)stringToSize[hashCode];
            else
                return null;
        }

        public static void PutSize(Font font, string str, SizeF size)
        {
            if (str == null)return;

            var hashCode = GetHashCode(font, str);

            stringToSize[hashCode] = size;
        }

        private static string GetHashCode(Font font, string str)
        {
            return font.GetHashCode() + str;
        }
        #endregion
    }
}