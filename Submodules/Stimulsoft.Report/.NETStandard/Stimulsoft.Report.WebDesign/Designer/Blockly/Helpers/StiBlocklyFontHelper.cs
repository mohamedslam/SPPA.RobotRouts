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

using Stimulsoft.Base;
using Stimulsoft.Report;
using Stimulsoft.Report.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using FontFamily = Stimulsoft.Drawing.FontFamily;
#endif

namespace Stimulsoft.Report.Web
{
    public static class StiBlocklyFontHelper
    {
        public static List<string> LoadFonts(StiReport report)
        {
            var fontNames = new List<string>();

            if (report == null)
            {
                var fontResource = report.Dictionary.Resources.ToList()
                    .Where(r => FontVHelper.IsFont(r.Type) && r.Content != null)
                    .ToList();

                foreach (var resource in fontResource)
                {
                    var name = report.GetResourceFontName(resource.Name);
                    fontNames.Add(name);
                }

                foreach (var family in StiFontCollection.Instance.Families)
                {
                    fontNames.Add(family.Name);
                }
            }

            foreach (var currentFont in FontFamily.Families)
            {
                if (string.IsNullOrWhiteSpace(currentFont?.Name)) continue;

                fontNames.Add(currentFont.Name);
            }

            return fontNames;
        }
    }
}
