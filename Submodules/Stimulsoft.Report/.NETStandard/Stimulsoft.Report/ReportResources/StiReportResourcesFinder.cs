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

using Stimulsoft.Report.Components;
using Stimulsoft.Report.Dictionary;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report
{
    public static class StiReportResourcesFinder
    {
        #region Consts
        public const string IdentResourceWatermark = "Watermark";
        public const string IdentResourceImage = "Image";
        public const string IdentResourceVariable = "ValueObject";
        public const string IdentResourceReport = "Resource";
        #endregion

        #region Methods
        public static List<StiPage> GetResourcePages(StiReport report)
	    {
            return report.Pages.ToList()
                .Where(p => p.Watermark.ExistImage()).ToList();
	    }

        public static List<StiImage> GetResourceImageComponents(StiReport report)
        {
            return report.GetComponents().ToList()
                .Where(c => c is StiImage).Cast<StiImage>()
                .Where(c => c.ExistImage()).ToList();
        }

        public static List<StiVariable> GetResourceVariables(StiReport report)
        {
            return report.Dictionary.Variables.ToList()
                .Where(v => !string.IsNullOrEmpty(v.Value) && v.Value.Length > 1000 && (v.ValueObject is Image || v.ValueObject is byte[])).ToList();
        }

        public static List<StiResource> GetResourceReport(StiReport report)
        {
            return report.Dictionary.Resources.ToList()
                .Where(r => r.Content != null).ToList();
        }

        public static bool ResourcesExits(StiReport report)
        {
            if (GetResourcePages(report).Any()) return true;
            if (GetResourceImageComponents(report).Any())return true;
            if (GetResourceVariables(report).Any())return true;
            if (GetResourceReport(report).Any()) return true;

            return false;
        }
        #endregion
    }
}
