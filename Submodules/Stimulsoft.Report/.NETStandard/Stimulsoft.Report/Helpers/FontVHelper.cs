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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Dictionary;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public static class FontVHelper
    {
        public static bool LoadFontsFromResources(StiReport report)
        {
            bool haveFonts = false;

            report.Dictionary.Resources
                .ToList()
                .Where(r => IsFont(r.Type) && r.Content != null)
                .ToList()
                .ForEach(r =>
                {
                    StiFontCollection.AddResourceFont(r.Name, r.Content, r.Type.ToString().Substring(4), r.Alias);
                    haveFonts = true;
                });

            return haveFonts;
        }

        public static void RemoveFontsFromResources(StiReport report)
        {
            report.Dictionary.Resources
                .ToList()
                .Where(r => IsFont(r.Type) && r.Content != null)
                .ToList()
                .ForEach(r =>
                {
                    StiFontCollection.RemoveResourceFont(r.Name, r.Content);
                });
        }

        public static bool IsFont(StiResourceType type)
        {
            return type == StiResourceType.FontOtf ||
                   type == StiResourceType.FontTtc ||
                   type == StiResourceType.FontTtf;
        }
    }
}