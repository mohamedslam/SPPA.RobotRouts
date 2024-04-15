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
using System.Linq;
using System.Collections;
using System.IO;
using Stimulsoft.Base.Localization;
using System.Collections.Generic;

namespace Stimulsoft.Report
{
	internal static class StiSystemVariableLocHelper
    {
        #region Methods
        public static string GetPageNofM(StiReport report)
        {
            return string.Format(GetPageNofMIdent(report), report.PageNumber, report.TotalPageCount);
        }

	    public static string GetPageNofMThrough(StiReport report)
        {
            return string.Format(GetPageNofMIdent(report), report.PageNumberThrough, report.TotalPageCountThrough);
	    }

        private static string GetPageNofMIdent(StiReport report)
        {
            if (report.PageNofMLocalizationString != null) 
                return report.PageNofMLocalizationString;

            string str = null;

            var culture = report.GetParsedCulture();
            if (!string.IsNullOrWhiteSpace(culture))
                str = GetIdent(culture);

            if (str != null) 
                return str;
            
            str = GetIdent(StiLocalization.CultureName);

            if (str != null) 
                return str;

            return StiLocalization.Get("Report", "PageNofM");
        }

	    private static string GetIdent(string culture)
	    {
	        culture = culture.ToLowerInvariant();
	        if (locs.ContainsKey(culture)) return locs[culture];

	        if (culture.Contains("-"))
	        {
	            culture = culture.Substring(0, culture.IndexOf("-"));

	            return locs.Where(p => culture.StartsWith(p.Key)).Select(p => p.Value).FirstOrDefault();
	        }
	        return null;
	    }
	    #endregion

        #region Fields
        private static Dictionary<string, string> locs = new Dictionary<string, string>
        {
            { "ar", "صفحة {0} من {1}" },
            { "en", "Page {0} of {1}" },
            { "be", "Старонка {0} з {1}" },
            { "bg", "Страница {0} от {1}" },
            { "cz", "{0} z {1}" },
            { "de", "Seite {0} von {1}" },
            { "el-gr", "Σελίδα {0} από {1}" },
            
            { "es", "Pág.{0} de {1}" },
            { "ca-es", "P.{0} de {1}" },
            { "eu-es", "{0}. or. {1}-tik" },
            { "gl-es", "Páx.{0} de {1}" },

            { "fa", "صفحه {0} از {1}" },
            { "fr", "{0} sur {1}" },
            { "hr", "Strana {0} od {1}" },
            { "hu", "Oldal {0} a {1}-ból" },
            { "id", "Halaman {0} dari {1}" },
            { "it", "{0} di {1}" },
            { "ka", "{0}, {1}-დან" },
            { "lt", "{0} iš {1}" },
            { "lt-sr-sp", "Strana {0} od {1}" },
            { "nb-no", "Side {0} av {1}" },
            { "nl", "{0} op {1}" },
            { "pl", "Strona {0} z {1}" },
            { "pt", "{0} de {1}" },
            { "pt-br", "{0} de {1}" },
            { "ro", "{0} din {1}" },
            { "ru", "Страница {0} из {1}" },
            { "sk", "{0} z {1}" },
            { "sv", "{0} av {1}" },
            { "tr", "Sayfa {0}/{1}" },
            { "ua", "Сторінка {0} із {1}" }
        };
        #endregion
    }
}
