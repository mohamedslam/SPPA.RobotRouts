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

using System.Collections;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Globalization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{
    public static class StiViewerFitTextHelper
    {
        #region class StiFitTextInfo
        public class StiFitTextInfo
        {
            public Hashtable HashText = new Hashtable();
            public Hashtable HashComponent = new Hashtable();
            private Hashtable HashFontString = new Hashtable();

            public string GetFontString(Font font)
            {
                var obj = HashFontString[font];
                if (obj != null)
                    return (string)obj;

                var st = $"{font.Name}*{font.Style}*";
                HashFontString[font] = st;
                return st;
            }

            public object GetFontSizeObject(StiText textBox, RectangleD rect, string text, ref float fontSize, ref string hashSt)
            {
                var hashObj = HashComponent[textBox];
                if (hashObj == null)
                {
                    hashSt = text + "*" +
                             rect.Width.ToString(CultureInfo.InvariantCulture) + "*" +
                             rect.Height.ToString(CultureInfo.InvariantCulture) + "*" +
                             GetFontString(textBox.Font) + "*" +
                             fontSize.ToString(CultureInfo.InvariantCulture);

                    hashObj = HashText[hashSt];
                    if (hashObj != null)
                        HashComponent[textBox] = hashObj;
                }

                if (hashObj is float)
                    fontSize = (float) hashObj;

                return hashObj;
            }

            public void Clear()
            {
                HashText.Clear();
                HashComponent.Clear();
                HashFontString.Clear();
            }
        }
        #endregion

        #region Fields
        private static Hashtable hashes = new Hashtable();
        #endregion

        #region Properties
        public static bool Enabled { get; set; }
        #endregion

        #region Methods
        public static void AddReport(StiReport report)
        {
            if (Enabled && report != null)
                hashes[report.CompiledReport ?? report] = new StiFitTextInfo();
        }

        public static void RemoveReport(StiReport report)
        {
            if (report != null)
            {
                var obj = (StiFitTextInfo)hashes[report];
                if (obj != null) obj.Clear();
                hashes.Remove(report);
            }
        }

        public static void ClearReportInfo(StiReport report)
        {
            if (report != null)
            {
                var obj = (StiFitTextInfo)hashes[report];
                if (obj != null) obj.Clear();
            }
        }

        public static StiFitTextInfo GetReportInfo(StiReport report)
        {
            if (Enabled && report != null)
            {
                var obj = hashes[report];
                if (obj != null) return (StiFitTextInfo)obj;
            }
            return null;
        }

        public static void Clear()
        {
            hashes.Clear();
        }
        #endregion

    }
}
