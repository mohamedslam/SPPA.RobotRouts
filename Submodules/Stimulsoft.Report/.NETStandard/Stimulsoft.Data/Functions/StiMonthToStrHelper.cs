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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Data.Functions
{
    /// <summary>
    /// MonthToStr helper.
    /// </summary>
    public sealed class StiMonthToStrHelper
    {
        #region Fields
        private static List<string[]> months;
        private static List<bool> defaultUpperCaseList;
        private static Hashtable cultureIndexes;
        #endregion

        #region Methods
        public static string MonthName(DateTime dateTime)
        {
            return dateTime.ToString("MMMM");
        }

        public static string MonthName(DateTime dateTime, bool localized)
        {
            if (!localized)
                return MonthName(dateTime);

            switch (dateTime.Month)
            {
                case 1:
                    return Loc.Get("A_WebViewer", "MonthJanuary");

                case 2:
                    return Loc.Get("A_WebViewer", "MonthFebruary");

                case 3:
                    return Loc.Get("A_WebViewer", "MonthMarch");

                case 4:
                    return Loc.Get("A_WebViewer", "MonthApril");

                case 5:
                    return Loc.Get("A_WebViewer", "MonthMay");

                case 6:
                    return Loc.Get("A_WebViewer", "MonthJune");

                case 7:
                    return Loc.Get("A_WebViewer", "MonthJuly");

                case 8:
                    return Loc.Get("A_WebViewer", "MonthAugust");

                case 9:
                    return Loc.Get("A_WebViewer", "MonthSeptember");

                case 10:
                    return Loc.Get("A_WebViewer", "MonthOctober");

                case 11:
                    return Loc.Get("A_WebViewer", "MonthNovember");

                case 12:
                    return Loc.Get("A_WebViewer", "MonthDecember");
            }

            return MonthName(dateTime);
        }

        public static string MonthName(DateTime dateTime, string culture)
        {
            return MonthName(dateTime.Month, culture);
        }

        public static string MonthName(int month, string culture)
        {
            var monthName = string.Empty;

            try
            {
                return new CultureInfo(culture).DateTimeFormat.GetMonthName(month);
            }
            catch
            {
                var cultureName = culture.ToLowerInvariant();
                if (!cultureIndexes.ContainsKey(cultureName))
                    return Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetMonthName(month);

                var index = (int)cultureIndexes[cultureName];

                if (month < months[index].Length)
                    monthName = months[index][month - 1];

                if (defaultUpperCaseList[index])
                    monthName = monthName.Substring(0, 1).ToUpperInvariant() + monthName.Substring(1);
            }

            return monthName;
        }

        public static string MonthName(DateTime dateTime, string culture, bool upperCase)
        {
            var monthName = MonthName(dateTime, culture).ToLowerInvariant();
            return upperCase 
                ? monthName.Substring(0, 1).ToUpperInvariant() + monthName.Substring(1) 
                : monthName;
        }

        public static void AddCulture(string[] monthNames, string[] cultureNames, bool defaultUpperCase)
        {
            int index = months.Count;
            months.Add(monthNames);
            defaultUpperCaseList.Add(defaultUpperCase);
            foreach (string culture in cultureNames)
            {
                cultureIndexes[culture.ToLowerInvariant()] = index;
            }
        }

        public static StiMonth? Month(int value)
        {
            if (value >= 1 || value <= 12)
                return (StiMonth)value;

            else 
                return null;
        }

        public static StiMonth? Month(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            str = str.ToLowerInvariant().Trim();

            var result = 0;
            if (int.TryParse(str, out result))
                return Month(result);

            foreach (string[] monthArray in months)
            {
                for (var index = 0; index < monthArray.Length; index++)
                {
                    var monthName = monthArray[index].ToLowerInvariant();
                    if (monthName == str)
                        return (StiMonth)(index + 1);

                    if (monthName.Length >= 3 && monthName.Substring(0, 3) == str)
                        return (StiMonth)(index + 1);
                }
            }

            return null;
        }
        #endregion

        static StiMonthToStrHelper()
        {
            months = new List<string[]>();
            defaultUpperCaseList = new List<bool>();
            cultureIndexes = new Hashtable();

            AddCulture(new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" }, new[] { "en" }, false);
            AddCulture(new[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" }, new[] { "ru" }, false);
            AddCulture(new[] { "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" }, new[] { "es" }, false);
            AddCulture(new[] { "gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno", "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre" }, new[] { "it" }, false);
            AddCulture(new[] { "ianuarie", "februarie", "martie", "aprilie", "mai", "iunie", "iulie", "august", "septembrie", "octombrie", "noiembrie", "decembrie" }, new[] { "ro" }, false);
            AddCulture(new[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" }, new[] { "pt" }, false);
            AddCulture(new[] { "Jannar", "Frar", "Marzu", "April", "Mejju", "Ġunju", "Lulju", "Awissu", "Settembru", "Ottubru", "Novembru", "Diċembru" }, new[] { "mt" }, false);
            AddCulture(new[] { "január", "február", "március", "április", "május", "június", "július", "augusztus", "szeptember", "október", "november", "december" }, new[] { "hu" }, false);
            AddCulture(new[] { "január", "február", "marec", "apríl", "máj", "jún", "júl", "august", "september", "október", "november", "december" }, new[] { "sk" }, false);
            AddCulture(new[] { "janúar", "febrúar", "mars", "apríl", "maí", "júní", "júlí", "ágúst", "september", "október", "nóvember", "desember" }, new[] { "is" }, false);
            AddCulture(new[] { "januar", "februar", "mars", "apríl", "mai", "juni", "juli", "august", "september", "oktober", "november", "desember" }, new[] { "fo" }, false);
            AddCulture(new[] { "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember" }, new[] { "de" }, false);
            AddCulture(new[] { "januari", "februari", "maart", "april", "mei", "juni", "juli", "augustus", "september", "oktober", "november", "december" }, new[] { "nl" }, false);
            AddCulture(new[] { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "Nopember", "Desember" }, new[] { "id" }, false);
            AddCulture(new[] { "janvāris", "februāris", "marts", "aprīlis", "maijs", "jūnijs", "jūlijs", "augusts", "septembris", "oktobris", "novembris", "decembris" }, new[] { "lv" }, false);
            AddCulture(new[] { "janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre" }, new[] { "fr" }, false);
            AddCulture(new[] { "leden", "únor", "březen", "duben", "květen", "červen", "červenec", "srpen", "září", "říjen", "listopad", "prosinec" }, new[] { "cs" }, false);
            AddCulture(new[] { "Mutarama", "Gashyantare", "Werurwe", "Mata", "Gicurasi", "Kamena", "Nyakanga", "Kanama", "Nzeli", "Ukwakira", "Ugushyingo", "Ukuboza" }, new[] { "rw" }, false);
            AddCulture(new[] { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" }, new[] { "tr" }, false);
            AddCulture(new[] { "ođđajagemánnu", "guovvamánnu", "njukčamánnu", "cuoŋománnu", "miessemánnu", "geassemánnu", "suoidnemánnu", "borgemánnu", "čakčamánnu", "golggotmánnu", "skábmamánnu", "juovlamánnu" }, new[] { "se" }, false);
            AddCulture(new[] { "sausis", "vasaris", "kovas", "balandis", "gegužė", "birželis", "liepa", "rugpjūtis", "rugsėjis", "spalis", "lapkritis", "gruodis" }, new[] { "lt" }, false);
            AddCulture(new[] { "siječanj", "veljača", "ožujak", "travanj", "svibanj", "lipanj", "srpanj", "kolovoz", "rujan", "listopad", "studeni", "prosinac" }, new[] { "hr" }, false);
            AddCulture(new[] { "styczeń", "luty", "marzec", "kwiecień", "maj", "czerwiec", "lipiec", "sierpień", "wrzesień", "październik", "listopad", "grudzień" }, new[] { "pl" }, false);
            AddCulture(new[] { "tammikuu", "helmikuu", "maaliskuu", "huhtikuu", "toukokuu", "kesäkuu", "heinäkuu", "elokuu", "syyskuu", "lokakuu", "marraskuu", "joulukuu" }, new[] { "fi" }, false);
            AddCulture(new[] { "јануари", "февруари", "март", "април", "мај", "јуни", "јули", "август", "септември", "октомври", "ноември", "декември" }, new[] { "mk" }, false);
            AddCulture(new[] { "Януари", "Февруари", "Март", "Април", "Май", "Юни", "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември" }, new[] { "bg" }, false);
            AddCulture(new[] { "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน", "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม" }, new[] { "th" }, false);

        }
    }
}