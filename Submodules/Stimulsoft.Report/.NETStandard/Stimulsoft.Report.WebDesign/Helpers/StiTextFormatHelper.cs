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

using System;
using System.Collections;
using Stimulsoft.Report.Components;
using Stimulsoft.Base.Localization;
using System.Linq;
using Stimulsoft.Report.Components.TextFormats;
using System.Globalization;
using System.Threading;

namespace Stimulsoft.Report.Web
{
    internal class StiTextFormatHelper
    {                
        //General
        public static Hashtable GeneralTextFormatItem(StiGeneralFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
           
            return textFormatItem;
        }

        //Number
        public static Hashtable NumberTextFormatItem(StiNumberFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["useGroupSeparator"] = service.UseGroupSeparator;
            textFormatItem["useLocalSetting"] = service.UseLocalSetting;
            textFormatItem["useAbbreviation"] = (service.State & StiTextFormatState.Abbreviation) > 0;
            textFormatItem["negativeInRed"] = (service.State & StiTextFormatState.NegativeInRed) > 0;
            textFormatItem["decimalDigits"] = service.DecimalDigits;
            textFormatItem["decimalSeparator"] = service.DecimalSeparator;
            textFormatItem["groupSeparator"] = service.GroupSeparator;
            textFormatItem["groupSize"] = service.GroupSize;
            textFormatItem["numberNegativePattern"] = service.NegativePattern;
            textFormatItem["state"] = service.State.ToString();

            return textFormatItem;
        }

        //Currency
        public static Hashtable CurrencyTextFormatItem(StiCurrencyFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["useGroupSeparator"] = service.UseGroupSeparator;
            textFormatItem["useLocalSetting"] = service.UseLocalSetting;
            textFormatItem["useAbbreviation"] = (service.State & StiTextFormatState.Abbreviation) > 0;
            textFormatItem["negativeInRed"] = (service.State & StiTextFormatState.NegativeInRed) > 0;
            textFormatItem["decimalDigits"] = service.DecimalDigits;
            textFormatItem["decimalSeparator"] = service.DecimalSeparator;
            textFormatItem["groupSeparator"] = service.GroupSeparator;
            textFormatItem["groupSize"] = service.GroupSize;
            textFormatItem["currencyPositivePattern"] = service.PositivePattern;
            textFormatItem["currencyNegativePattern"] = service.NegativePattern;
            textFormatItem["currencySymbol"] = service.Symbol;
            textFormatItem["state"] = service.State.ToString();

            return textFormatItem;
        }

        //Date
        public static Hashtable DateTextFormatItem(StiDateFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["dateFormat"] = service.StringFormat;

            return textFormatItem;
        }

        //Time
        public static Hashtable TimeTextFormatItem(StiTimeFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["timeFormat"] = service.StringFormat;

            return textFormatItem;
        }

        //Percentage
        public static Hashtable PercentageTextFormatItem(StiPercentageFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["useGroupSeparator"] = service.UseGroupSeparator;
            textFormatItem["useLocalSetting"] = service.UseLocalSetting;
            textFormatItem["negativeInRed"] = (service.State & StiTextFormatState.NegativeInRed) > 0;
            textFormatItem["decimalDigits"] = service.DecimalDigits;
            textFormatItem["decimalSeparator"] = service.DecimalSeparator;
            textFormatItem["groupSeparator"] = service.GroupSeparator;
            textFormatItem["groupSize"] = service.GroupSize;
            textFormatItem["percentagePositivePattern"] = service.PositivePattern;
            textFormatItem["percentageNegativePattern"] = service.NegativePattern;
            textFormatItem["percentageSymbol"] = service.Symbol;
            textFormatItem["state"] = service.State.ToString();

            return textFormatItem;
        }

        //Boolean
        public static Hashtable BooleanTextFormatItem(StiBooleanFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["falseValue"] = service.FalseValue;
            textFormatItem["falseDisplay"] = service.FalseDisplay;
            textFormatItem["trueValue"] = service.TrueValue;
            textFormatItem["trueDisplay"] = service.TrueDisplay;

            return textFormatItem;
        }

        //Custom
        public static Hashtable CustomTextFormatItem(StiCustomFormatService service)
        {
            Hashtable textFormatItem = CommonTextFormatItem(service);
            textFormatItem["customFormat"] = service.StringFormat;

            return textFormatItem;
        }

        //Common
        public static Hashtable CommonTextFormatItem(StiFormatService service)
        {
            Hashtable textFormatItem = new Hashtable();
            textFormatItem["type"] = service.GetType().Name;
            textFormatItem["sample"] = service.Sample.ToString();

            return textFormatItem;
        }

        private static StiTextFormatState GetStateProperty(string propertyValue)
        {            
            int value = 0;
            if (propertyValue.IndexOf("DecimalDigits") >= 0) value += 1;
            if (propertyValue.IndexOf("DecimalSeparator") >= 0) value += 2;
            if (propertyValue.IndexOf("GroupSeparator") >= 0) value += 4;
            if (propertyValue.IndexOf("GroupSize") >= 0) value += 8;
            if (propertyValue.IndexOf("PositivePattern") >= 0) value += 16;
            if (propertyValue.IndexOf("NegativePattern") >= 0) value += 32;
            if (propertyValue.IndexOf("CurrencySymbol") >= 0) value += 64;
            if (propertyValue.IndexOf("PercentageSymbol") >= 0) value += 128;

            return (StiTextFormatState)value;
        }

        public static ArrayList GetCurrencySymbols()
        {
            ArrayList currencySymbols = new ArrayList();
            string cr = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
            currencySymbols.Add(cr);
            currencySymbols.Add(((char)0x0024).ToString());//Dollar
            currencySymbols.Add(((char)0x20AC).ToString());//Euro
            currencySymbols.Add(((char)0x00A2).ToString());//Cent
            currencySymbols.Add(((char)0x00A3).ToString());//Pound
            currencySymbols.Add(((char)0x00A4).ToString());//Currency
            currencySymbols.Add(((char)0x00A5).ToString());//Yen
            currencySymbols.Add(((char)0x20A3).ToString());//FrenchFranc
            currencySymbols.Add(((char)0x20A4).ToString());//Lira
            currencySymbols.Add(((char)0x20A7).ToString());//Peseta
            currencySymbols.Add(((char)0x20AA).ToString());//Sheqel
            currencySymbols.Add(((char)0x20AB).ToString());//Dong
            currencySymbols.Add(((char)0x0E3F).ToString());//Thai
            currencySymbols.Add(((char)0x20A0).ToString());//Euro Currency
            currencySymbols.Add(((char)0x20A1).ToString());//Colon
            currencySymbols.Add(((char)0x20A2).ToString());//Cruzeiro
            currencySymbols.Add(((char)0x20A5).ToString());//Mill
            currencySymbols.Add(((char)0x20A6).ToString());//Naira
            currencySymbols.Add(((char)0x20A8).ToString());//Rupee
            currencySymbols.Add(((char)0x20B9).ToString());//Rupee2
            currencySymbols.Add(((char)0x20A9).ToString());//Won
            currencySymbols.Add(((char)0x20AB).ToString());//Dong
            currencySymbols.Add("R$");//Brazilian Real

            return currencySymbols;
        }

        public static StiFormatService GetFormatService(Hashtable properties)
        {
            switch ((string)properties["type"]) 
            {
                case "StiNumberFormatService":
                    {
                        StiNumberFormatService format = new StiNumberFormatService();
                        format.UseGroupSeparator = Convert.ToBoolean(properties["useGroupSeparator"]);
                        format.UseLocalSetting = Convert.ToBoolean(properties["useLocalSetting"]);
                        format.DecimalDigits = Convert.ToInt32(properties["decimalDigits"]);
                        format.DecimalSeparator = Convert.ToString(properties["decimalSeparator"]);
                        format.GroupSeparator = Convert.ToString(properties["groupSeparator"]);
                        format.GroupSize = Convert.ToInt32(properties["groupSize"]);
                        format.NegativePattern = Convert.ToInt32(properties["numberNegativePattern"]);
                        format.State = GetStateProperty(Convert.ToString(properties["state"]));
                        if (Convert.ToBoolean(properties["useAbbreviation"])) format.State |= StiTextFormatState.Abbreviation;
                        if (Convert.ToBoolean(properties["negativeInRed"])) format.State |= StiTextFormatState.NegativeInRed;

                        return format;
                    }
                case "StiCurrencyFormatService":
                    {
                        StiCurrencyFormatService format = new StiCurrencyFormatService();
                        format.UseGroupSeparator = Convert.ToBoolean(properties["useGroupSeparator"]);
                        format.UseLocalSetting = Convert.ToBoolean(properties["useLocalSetting"]);
                        format.DecimalDigits = Convert.ToInt32(properties["decimalDigits"]);
                        format.DecimalSeparator = Convert.ToString(properties["decimalSeparator"]);
                        format.GroupSeparator = Convert.ToString(properties["groupSeparator"]);
                        format.GroupSize = Convert.ToInt32(properties["groupSize"]);
                        format.PositivePattern = Convert.ToInt32(properties["currencyPositivePattern"]);
                        format.NegativePattern = Convert.ToInt32(properties["currencyNegativePattern"]);
                        format.Symbol = Convert.ToString(properties["currencySymbol"]);
                        format.State = GetStateProperty(Convert.ToString(properties["state"]));
                        if (Convert.ToBoolean(properties["useAbbreviation"])) format.State |= StiTextFormatState.Abbreviation;
                        if (Convert.ToBoolean(properties["negativeInRed"])) format.State |= StiTextFormatState.NegativeInRed;

                        return format;
                    }
                case "StiDateFormatService":
                    {
                        StiDateFormatService format = new StiDateFormatService();
                        format.StringFormat = Convert.ToString(properties["dateFormat"]);

                        return format;
                    }
                case "StiTimeFormatService":
                    {
                        StiTimeFormatService format = new StiTimeFormatService();
                        format.StringFormat = Convert.ToString(properties["timeFormat"]);

                        return format;
                    }
                case "StiPercentageFormatService":
                    {
                        StiPercentageFormatService format = new StiPercentageFormatService();
                        format.UseGroupSeparator = Convert.ToBoolean(properties["useGroupSeparator"]);
                        format.UseLocalSetting = Convert.ToBoolean(properties["useLocalSetting"]);
                        format.DecimalDigits = Convert.ToInt32(properties["decimalDigits"]);
                        format.DecimalSeparator = Convert.ToString(properties["decimalSeparator"]);
                        format.GroupSeparator = Convert.ToString(properties["groupSeparator"]);
                        format.GroupSize = Convert.ToInt32(properties["groupSize"]);
                        format.PositivePattern = Convert.ToInt32(properties["percentagePositivePattern"]);
                        format.NegativePattern = Convert.ToInt32(properties["percentageNegativePattern"]);
                        format.Symbol = Convert.ToString(properties["percentageSymbol"]);
                        format.State = GetStateProperty(Convert.ToString(properties["state"]));
                        if (Convert.ToBoolean(properties["negativeInRed"])) format.State |= StiTextFormatState.NegativeInRed;

                        return format;
                    }
                case "StiBooleanFormatService":
                    {
                        StiBooleanFormatService format = new StiBooleanFormatService();
                        format.FalseValue = Convert.ToString(properties["falseValue"]);
                        format.FalseDisplay = Convert.ToString(properties["falseDisplay"]);
                        format.TrueValue = Convert.ToString(properties["trueValue"]);
                        format.TrueDisplay = Convert.ToString(properties["trueDisplay"]);

                        return format;
                    }
                case "StiCustomFormatService":
                    {
                        StiCustomFormatService format = new StiCustomFormatService();
                        format.StringFormat = Convert.ToString(properties["customFormat"]);

                        return format;
                    }
            }

            return new StiGeneralFormatService();
        }

        public static ArrayList GetDateAndTimeFormats(string category, StiFormatService service)
        {
            ArrayList items = new ArrayList();
            string[] keys = StiLocalization.GetKeys("Formats");

            foreach (string key in keys)
            {
                if (key.IndexOf(category, StringComparison.InvariantCulture) <= -1) continue;

                string value = StiLocalization.Get("Formats", key);
                if (value != null)
                {
                    Hashtable item = new Hashtable();
                    if (value.Length > 0 && value[0] == '*')
                    {
                        item["key"] = value.Substring(1);
                        item["value"] = '*' + service.Format(value.Substring(1), service.Sample);
                    }
                    else
                    {
                        item["key"] = value;
                        item["value"] = service.Format(value, service.Sample);
                    }
                    
                    items.Add(item);
                }
            }

            return items;
        }

        public static Hashtable GetTextFormatItem(StiFormatService service)
        {
            if (service.GetType() == typeof(StiGeneralFormatService)) return GeneralTextFormatItem(service as StiGeneralFormatService);
            else if (service.GetType() == typeof(StiNumberFormatService)) return NumberTextFormatItem(service as StiNumberFormatService);
            else if (service.GetType() == typeof(StiCurrencyFormatService)) return CurrencyTextFormatItem(service as StiCurrencyFormatService);
            else if (service.GetType() == typeof(StiDateFormatService)) return DateTextFormatItem(service as StiDateFormatService);
            else if (service.GetType() == typeof(StiTimeFormatService)) return TimeTextFormatItem(service as StiTimeFormatService);
            else if (service.GetType() == typeof(StiPercentageFormatService)) return PercentageTextFormatItem(service as StiPercentageFormatService);
            else if (service.GetType() == typeof(StiBooleanFormatService)) return BooleanTextFormatItem(service as StiBooleanFormatService);
            else if (service.GetType() == typeof(StiCustomFormatService)) return CustomTextFormatItem(service as StiCustomFormatService);

            return CommonTextFormatItem(service);
        }

        public static Hashtable GetTextFormatItems()
        {
            Hashtable items = new Hashtable();
            foreach (var service in StiOptions.Services.Formats.Where(s => s.ServiceEnabled))
            {
                items[service.GetType().Name] = GetTextFormatItem(service);
            }

            return items;
        }

        public static void UpdateTextFormatItemsByReportCulture(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            CultureInfo storedCulture = null;
            try
            {
                var culture = report.GetParsedCulture();
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }
            }
            finally
            {
                callbackResult["textFormats"] = GetTextFormatItems();
                callbackResult["dateFormats"] = GetDateAndTimeFormats("date", new StiDateFormatService());
                callbackResult["timeFormats"] = GetDateAndTimeFormats("time", new StiTimeFormatService());
                callbackResult["customFormats"] = GetDateAndTimeFormats("custom", new StiCustomFormatService());

                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
        }

        public static void UpdateSampleTextFormat(StiReport report, Hashtable param, Hashtable callbackResult)
        {
            CultureInfo storedCulture = null;
            try
            {
                var culture = report.GetParsedCulture();
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    storedCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                }
            }
            finally
            {
                StiFormatService service = GetFormatService((Hashtable)param["textFormat"]);
                if (!service.IsFormatStringFromVariable) callbackResult["sampleText"] = service.Format(service.Sample);
                service = null;

                if (storedCulture != null)
                    Thread.CurrentThread.CurrentCulture = storedCulture;
            }
        }
    }
}