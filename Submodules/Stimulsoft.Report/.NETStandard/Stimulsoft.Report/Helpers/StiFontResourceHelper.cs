#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports         										}
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

using Stimulsoft.Report.Dictionary;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public static class StiFontResourceHelper
    {
        #region Methods
        public static bool IsResourceFont(string fontName, StiReport report)
        {
            return report.Dictionary.Resources.ToList()
                .Where(r => FontVHelper.IsFont(r.Type) && r.Content != null)
                .Any(f => f.Name == fontName);
        }

        public static List<StiResource> GetAllFonts(StiReport report)
        {
            return report.Dictionary.Resources.ToList()
                .Where(r => FontVHelper.IsFont(r.Type) && r.Content != null)
                .ToList();
        } 
        #endregion
    }
}
