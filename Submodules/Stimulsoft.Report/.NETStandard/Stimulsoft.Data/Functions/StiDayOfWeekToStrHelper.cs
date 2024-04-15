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

using Stimulsoft.Base.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Data.Functions
{
    public sealed class StiDayOfWeekToStrHelper
    {
        #region Fields
        private static List<string[]> days;
        private static List<bool> defaultUpperCaseList;
        private static Hashtable cultureIndexes;
        #endregion

        #region Methods
        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date)
        {
            return DateTimeFormatInfo.CurrentInfo.GetDayName(date.DayOfWeek);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date)
        {
            return DateTimeFormatInfo.CurrentInfo.GetDayName(date.DayOfWeek);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTime date, bool localized)
        {
            if (localized)
            {
                var str = DayOfWeekToStr(date.DayOfWeek);
                if (str != null)
                    return str;
            }

            return DayOfWeek(date);
        }

        /// <summary>
        /// Returns the day of the week.
        /// </summary>
        public static string DayOfWeek(DateTimeOffset date, bool localized)
        {
            if (localized)
            {
                var str = DayOfWeekToStr(date.DayOfWeek);
                if (str != null)
                    return str;
            }

            return DayOfWeek(date);
        }

        private static string DayOfWeekToStr(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                // global::System need for NetCore
                case global::System.DayOfWeek.Sunday:
                    return Loc.Get("A_WebViewer", "DaySunday");

                case global::System.DayOfWeek.Monday:
                    return Loc.Get("A_WebViewer", "DayMonday");

                case global::System.DayOfWeek.Tuesday:
                    return Loc.Get("A_WebViewer", "DayTuesday");

                case global::System.DayOfWeek.Wednesday:
                    return Loc.Get("A_WebViewer", "DayWednesday");

                case global::System.DayOfWeek.Thursday:
                    return Loc.Get("A_WebViewer", "DayThursday");

                case global::System.DayOfWeek.Friday:
                    return Loc.Get("A_WebViewer", "DayFriday");

                case global::System.DayOfWeek.Saturday:
                    return Loc.Get("A_WebViewer", "DaySaturday");

                default:
                    return null;
            }
        }

        public static string DayOfWeek(DateTime dateTime, string culture)
        {
            return DayOfWeek((int)dateTime.DayOfWeek, culture);
        }

        public static string DayOfWeek(DateTimeOffset dateTime, string culture)
        {
            return DayOfWeek((int)dateTime.DayOfWeek, culture);
        }

        public static string DayOfWeek(int dayOfWeek, string culture)
        {
            var dayName = string.Empty;

            try
            {
                return new CultureInfo(culture).DateTimeFormat.GetDayName((DayOfWeek)dayOfWeek);
            }
            catch
            {
                var cultureName = culture.ToLowerInvariant();

                if (!cultureIndexes.ContainsKey(cultureName))
                    return Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)dayOfWeek);

                var index = (int)cultureIndexes[cultureName];

                if (dayOfWeek < days[index].Length)
                    dayName = days[index][dayOfWeek];

                if (defaultUpperCaseList[index])
                    dayName = dayName.Substring(0, 1).ToUpperInvariant() + dayName.Substring(1);
            }

            return dayName;
        }

        public static string DayOfWeek(DateTime dateTime, string culture, bool upperCase)
        {
            var dayName = DayOfWeek(dateTime, culture).ToLowerInvariant();
            return upperCase 
                ? dayName.Substring(0, 1).ToUpperInvariant() + dayName.Substring(1) 
                : dayName;
        }

        public static string DayOfWeek(DateTimeOffset dateTime, string culture, bool upperCase)
        {
            var dayName = DayOfWeek(dateTime, culture).ToLowerInvariant();
            return upperCase
                ? dayName.Substring(0, 1).ToUpperInvariant() + dayName.Substring(1)
                : dayName;
        }

        public static void AddCulture(string[] monthNames, string[] cultureNames, bool defaultUpperCase)
        {
            var index = days.Count;
            days.Add(monthNames);
            defaultUpperCaseList.Add(defaultUpperCase);
            foreach (var culture in cultureNames)
            {
                cultureIndexes[culture.ToLowerInvariant()] = index;
            }
        }

        public static DayOfWeek? DayOfWeek(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            str = str.ToLowerInvariant().Trim();

            foreach (string[] dayArray in days)
            {
                for (var index = 0; index < dayArray.Length; index++)
                {
                    var dayName = dayArray[index].ToLowerInvariant();
                    if (dayName == str)
                        return (DayOfWeek)index;
                }
            }

            return null;
        }
        #endregion

        static StiDayOfWeekToStrHelper()
        {
            days = new List<string[]>();
            defaultUpperCaseList = new List<bool>();
            cultureIndexes = new Hashtable();

            AddCulture(new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }, new[] { "en" }, false);
            AddCulture(new[] { "dimanche", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi" }, new[] { "fr" }, false);
            AddCulture(new[] { "domenica", "lunedì", "martedì", "mercoledì", "giovedì", "venerdì", "sabato" }, new[] { "it" }, false);
            AddCulture(new[] { "domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado" }, new[] { "es" }, false);
            AddCulture(new[] { "domingo", "segunda-feira", "terça-feira", "quarta-feira", "quinta-feira", "sexta-feira", "sábado" }, new[] { "pt" }, false);
            AddCulture(new[] { "duminică", "luni", "marţi", "miercuri", "joi", "vineri", "sâmbătă" }, new[] { "ro" }, false);
            AddCulture(new[] { "Il-Ħadd", "It-Tnejn", "It-Tlieta", "L-Erbgħa", "Il-Ħamis", "Il-Ġimgħa", "Is-Sibt" }, new[] { "mt" }, false);
            AddCulture(new[] { "Ku wa mbere", "Ku wa kabiri", "Ku wa gatatu", "Ku wa kane", "Ku wa gatanu", "Ku wa gatandatu", "Ku cyumweru" }, new[] { "rw" }, false);
            AddCulture(new[] { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" }, new[] { "id" }, false);
            AddCulture(new[] { "nedeľa", "pondelok", "utorok", "streda", "štvrtok", "piatok", "sobota" }, new[] { "sk" }, false);
            AddCulture(new[] { "nedjelja", "ponedjeljak", "utorak", "srijeda", "četvrtak", "petak", "subota" }, new[] { "hr" }, false);
            AddCulture(new[] { "niedziela", "poniedziałek", "wtorek", "środa", "czwartek", "piątek", "sobota" }, new[] { "pl" }, false);
            AddCulture(new[] { "Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" }, new[] { "tr" }, false);
            AddCulture(new[] { "sekmadienis", "pirmadienis", "antradienis", "trečiadienis", "ketvirtadienis", "penktadienis", "šeštadienis" }, new[] { "lt" }, false);
            AddCulture(new[] { "Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag" }, new[] { "de" }, false);
            AddCulture(new[] { "sotnabeaivi", "mánnodat", "disdat", "gaskavahkku", "duorastat", "bearjadat", "lávvardat" }, new[] { "se" }, false);
            AddCulture(new[] { "sunnudagur", "mánadagur", "týsdagur", "mikudagur", "hósdagur", "fríggjadagur", "leygardagur" }, new[] { "fo" }, false);
            AddCulture(new[] { "sunnudagur", "mánudagur", "þriðjudagur", "miðvikudagur", "fimmtudagur", "föstudagur", "laugardagur" }, new[] { "is" }, false);
            AddCulture(new[] { "sunnuntai", "maanantai", "tiistai", "keskiviikko", "torstai", "perjantai", "lauantai" }, new[] { "fi" }, false);
            AddCulture(new[] { "svētdiena", "pirmdiena", "otrdiena", "trešdiena", "ceturtdiena", "piektdiena", "sestdiena" }, new[] { "lv" }, false);
            AddCulture(new[] { "vasárnap", "hétfő", "kedd", "szerda", "csütörtök", "péntek", "szombat" }, new[] { "hu" }, false);
            AddCulture(new[] { "zondag", "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag" }, new[] { "nl" }, false);
            AddCulture(new[] { "воскресенье", "понедельник", "вторник", "среда", "четверг", "пятница", "суббота" }, new[] { "ru" }, false);
            AddCulture(new[] { "недела", "понеделник", "вторник", "среда", "четврток", "петок", "сабота" }, new[] { "mk" }, false);
            AddCulture(new[] { "неделя", "понеделник", "вторник", "сряда", "четвъртък", "петък", "събота" }, new[] { "bg" }, false);
            AddCulture(new[] { "Ням", "Даваа", "Мягмар", "Лхагва", "Пүрэв", "Баасан", "Бямба" }, new[] { "mn" }, false);
            AddCulture(new[] { "อาทิตย์", "จันทร์", "อังคาร", "พุธ", "พฤหัสบดี", "ศุกร์", "เสาร์" }, new[] { "th" }, false);
        }
    }
}
