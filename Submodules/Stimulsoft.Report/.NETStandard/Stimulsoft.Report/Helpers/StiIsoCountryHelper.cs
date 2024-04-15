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

using System.Collections.ObjectModel;
using System.Linq;

namespace Stimulsoft.Report.Helpers
{
    public static class StiIsoElementHelper
    {
        #region Properties.Static
        private static object lockObject = new object();

        private static Collection<StiIsoCountry> countries;
        public static Collection<StiIsoCountry> Countries
        {
            get
            {
                lock (lockObject)
                {
                    if (countries == null)
                    {
                        countries = new Collection<StiIsoCountry>();
                        InitializeCountries();
                    }

                    return countries;
                }
            }
        }

        private static Collection<StiIsoCountry> usStates;
        public static Collection<StiIsoCountry> UsStates
        {
            get
            {
                lock (lockObject)
                {
                    if (usStates == null)
                    {
                        usStates = new Collection<StiIsoCountry>();
                        InitializeUsStates();
                    }

                    return usStates;
                }
            }
        }

        private static Collection<StiIsoCountry> canadaProvinces;
        public static Collection<StiIsoCountry> CanadaProvinces
        {
            get
            {
                lock (lockObject)
                {
                    if (canadaProvinces == null)
                    {
                        canadaProvinces = new Collection<StiIsoCountry>();
                        InitializeCanadaProvinces();
                    }

                    return canadaProvinces;
                }
            }
        }

        private static Collection<StiIsoCountry> brazilProvinces;
        public static Collection<StiIsoCountry> BrazilProvinces
        {
            get
            {
                lock (lockObject)
                {
                    if (brazilProvinces == null)
                    {
                        brazilProvinces = new Collection<StiIsoCountry>();
                        InitializeBrazilProvinces();
                    }

                    return brazilProvinces;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets ISO3166-1 Alpha2 code based on country name. Returns null, if country is not recognized.
        /// </summary>
        public static string GetIsoAlpha2FromName(string name, string mapId = null)
        {
            var country = GetCountryFromName(name, mapId);
            return country != null ? country.Alpha2 : null;
        }

        /// <summary>
        /// Gets ISO3166-1 Alpha3 code based on country name. Returns null, if country is not recognized.
        /// </summary>
        public static string GetIsoAlpha3FromName(string name, string mapId = null)
        {
            var country = GetCountryFromName(name, mapId);
            return country != null ? country.Alpha3 : null;
        }

        /// <summary>
        /// Gets ISO3166-1 Country based on its alpha3 code. Returns null, if country is not recognized.
        /// </summary>
        public static StiIsoCountry GetCountryFromName(string name, string mapId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = StiMapKeyHelper.Simplify(name);

            lock (lockObject)
            {
                return GetCountries(mapId)
                    .FirstOrDefault(c => IsEqual(c, name));
            }
        }

        private static Collection<StiIsoCountry> GetCountries(string mapId = null)
        {
            if (string.IsNullOrWhiteSpace(mapId))
                return Countries;

            switch (mapId.ToLowerInvariant())
            {
                case "world":
                    return Countries;

                case "usa":
                    return UsStates;

                case "canada":
                    return CanadaProvinces;

                case "brazil":
                    return BrazilProvinces;

                default:
                    return Countries;
            }
        }

        private static bool IsEqual(StiIsoCountry country, string id)
        {
            if (country.Names.Any(name => StiMapKeyHelper.Simplify(name) == id) ||
                   StiMapKeyHelper.Simplify(country.Alpha2) == id ||
                   StiMapKeyHelper.Simplify(country.Alpha3) == id)
                return true;

            if (country.RuNames != null && country.RuNames.Any(name => StiMapKeyHelper.Simplify(name) == id))
                return true;

            if (country.FrNames != null && country.FrNames.Any(name => StiMapKeyHelper.Simplify(name) == id))
                return true;

            return false;
        }

        /// <summary>
        /// Gets ISO3166-1 Country based on its alpha3 code.
        /// </summary>
        public static StiIsoCountry GetCountryFromAlpha3(string alpha3, string mapId = null)
        {
            if (string.IsNullOrWhiteSpace(alpha3))
                return null;

            alpha3 = StiMapKeyHelper.Simplify(alpha3);

            lock (lockObject)
            {
                return GetCountries(mapId)
                    .FirstOrDefault(p => StiMapKeyHelper.Simplify(p.Alpha3) == alpha3);
            }
        }

        /// <summary>
        /// Obtain ISO3166-1 Country based on its alpha2 code.
        /// </summary>
        public static StiIsoCountry GetCountryFromAlpha2(string alpha2, string mapId = null)
        {
            if (string.IsNullOrWhiteSpace(alpha2))
                return null;

            alpha2 = StiMapKeyHelper.Simplify(alpha2);

            lock (lockObject)
            {
                return GetCountries(mapId)
                    .FirstOrDefault(p => StiMapKeyHelper.Simplify(p.Alpha2) == alpha2);
            }
        }

        private static void InitializeCountries()
        {
            countries.Add(new StiIsoCountry("Afghanistan").Ru("Афганистан").Iso("AF", "AFG"));
            countries.Add(new StiIsoCountry("Åland Islands").Ru("Аландские острова").Iso("AX", "ALA"));
            countries.Add(new StiIsoCountry("Albania").Ru("Албания").Iso("AL", "ALB"));
            countries.Add(new StiIsoCountry("Algeria").Ru("Алжир").Iso("DZ", "DZA"));
            countries.Add(new StiIsoCountry("American Samoa").Ru("Американское Самоа").Iso("AS", "ASM"));
            countries.Add(new StiIsoCountry("Andorra").Ru("Андорра").Iso("AD", "AND"));
            countries.Add(new StiIsoCountry("Angola").Ru("Ангола").Iso("AO", "AGO"));
            countries.Add(new StiIsoCountry("Anguilla").Ru("Ангилья").Iso("AI", "AIA"));
            countries.Add(new StiIsoCountry("Antarctica").Ru("Антарктида").Iso("AQ", "ATA"));
            countries.Add(new StiIsoCountry("Antigua and Barbuda").Ru("Антигуа и Барбуда").Iso("AG", "ATG"));
            countries.Add(new StiIsoCountry("Argentina").Ru("Аргентина").Iso("AR", "ARG"));
            countries.Add(new StiIsoCountry("Armenia").Ru("Армения").Iso("AM", "ARM"));
            countries.Add(new StiIsoCountry("Aruba").Ru("Аруба").Iso("AW", "ABW"));
            countries.Add(new StiIsoCountry("Australia").Ru("Австралия").Iso("AU", "AUS"));
            countries.Add(new StiIsoCountry("Austria").Ru("Австрия").Iso("AT", "AUT"));
            countries.Add(new StiIsoCountry("Azerbaijan").Ru("Азербайджан").Iso("AZ", "AZE"));
            countries.Add(new StiIsoCountry("Bahamas").Ru("Багамские Острова").Iso("BS", "BHS"));
            countries.Add(new StiIsoCountry("Bahrain").Ru("Бахрейн").Iso("BH", "BHR"));
            countries.Add(new StiIsoCountry("Bangladesh").Ru("Бангладеш").Iso("BD", "BGD"));
            countries.Add(new StiIsoCountry("Barbados").Ru("Барбадос").Iso("BB", "BRB"));
            countries.Add(new StiIsoCountry("Belarus").Ru("Беларусь", "Белоруссия").Iso("BY", "BLR"));
            countries.Add(new StiIsoCountry("Belgium").Ru("Бельгия").Iso("BE", "BEL"));
            countries.Add(new StiIsoCountry("Belize").Ru("Белиз").Iso("BZ", "BLZ"));
            countries.Add(new StiIsoCountry("Benin").Ru("Бенин").Iso("BJ", "BEN"));
            countries.Add(new StiIsoCountry("Bermuda").Ru("Бермуды").Iso("BM", "BMU"));
            countries.Add(new StiIsoCountry("Bhutan").Ru("Бутан").Iso("BT", "BTN"));
            countries.Add(new StiIsoCountry("Bolivia", "Bolivia (Plurinational State of)").Ru("Боливия").Iso("BO", "BOL"));
            countries.Add(new StiIsoCountry("Bonaire", "Bonaire, Sint Eustatius and Saba").Ru("Бонэйр").Iso("BQ", "BES"));
            countries.Add(new StiIsoCountry("Bosnia and Herzegovina").Ru("Босния и Герцеговина ").Iso("BA", "BIH"));
            countries.Add(new StiIsoCountry("Botswana").Ru("Ботсвана").Iso("BW", "BWA"));
            countries.Add(new StiIsoCountry("Bouvet Island").Ru("Остров Буве").Iso("BV", "BVT"));
            countries.Add(new StiIsoCountry("Brazil").Ru("Бразилия").Iso("BR", "BRA"));
            countries.Add(new StiIsoCountry("British Indian Ocean Territory").Ru("Британская Территория в Индийском Океане").Iso("IO", "IOT"));
            countries.Add(new StiIsoCountry("Brunei Darussalam").Ru("Бруней").Iso("BN", "BRN"));
            countries.Add(new StiIsoCountry("Bulgaria").Ru("Болгария").Iso("BG", "BGR"));
            countries.Add(new StiIsoCountry("Burkina Faso").Ru("Буркина - Фасо").Iso("BF", "BFA"));
            countries.Add(new StiIsoCountry("Burundi").Ru("Бурунди").Iso("BI", "BDI"));     
            countries.Add(new StiIsoCountry("Cabo Verde").Ru("Кабо - Верде").Iso("CV", "CPV"));
            countries.Add(new StiIsoCountry("Cambodia").Ru("Камбоджа").Iso("KH", "KHM"));           
            countries.Add(new StiIsoCountry("Cameroon").Ru("Камерун").Iso("CM", "CMR"));
            countries.Add(new StiIsoCountry("Canada").Ru("Канада").Iso("CA", "CAN"));
            countries.Add(new StiIsoCountry("Cayman Islands").Ru("Острова Кайман").Iso("KY", "CYM"));
            countries.Add(new StiIsoCountry("Central African Republic").Ru("ЦАР").Iso("CF", "CAF"));
            countries.Add(new StiIsoCountry("Chad").Ru("Чад").Iso("TD", "TCD"));
            countries.Add(new StiIsoCountry("Chile").Ru("Чили").Iso("CL", "CHL"));
            countries.Add(new StiIsoCountry("China").Ru("Китай", "КНР", "Китайская Народная Республика").Iso("CN", "CHN"));
            countries.Add(new StiIsoCountry("Christmas Island").Ru("Остров Рождества").Iso("CX", "CXR"));
            countries.Add(new StiIsoCountry("Cocos (Keeling) Islands").Ru("Кокосовые острова").Iso("CC", "CCK"));
            countries.Add(new StiIsoCountry("Colombia").Ru("Колумбия").Iso("CO", "COL"));
            countries.Add(new StiIsoCountry("Comoros").Ru("Коморы").Iso("KM", "COM"));
            countries.Add(new StiIsoCountry("Congo").Ru("Республика Конго").Iso("CG", "COG"));
            countries.Add(new StiIsoCountry("Congo (Democratic Republic of the)").Ru("Демократическая Республика Конго").Iso("CD", "COD"));
            countries.Add(new StiIsoCountry("Cook Islands").Ru("Острова Кука").Iso("CK", "COK"));
            countries.Add(new StiIsoCountry("Costa Rica").Ru("Коста - Рика").Iso("CR", "CRI"));
            countries.Add(new StiIsoCountry("Côte d'Ivoire").Ru("Кот - д’Ивуар").Iso("CI", "CIV"));
            countries.Add(new StiIsoCountry("Croatia").Ru("Хорватия").Iso("HR", "HRV"));      
            countries.Add(new StiIsoCountry("Cuba").Ru("Куба").Iso("CU", "CUB"));
            countries.Add(new StiIsoCountry("Curaçao").Ru("Кюрасао").Iso("CW", "CUW"));
            countries.Add(new StiIsoCountry("Cyprus").Ru("Кипр").Iso("CY", "CYP"));
            countries.Add(new StiIsoCountry("Czech Republic", "Czech Republic", "Czech").Ru("Чехия").Iso("CZ", "CZE"));
            countries.Add(new StiIsoCountry("Denmark").Ru("Дания").Iso("DK", "DNK"));
            countries.Add(new StiIsoCountry("Djibouti").Ru("Джибути").Iso("DJ", "DJI"));
            countries.Add(new StiIsoCountry("Dominica").Ru("Доминикана").Iso("DM", "DMA"));
            countries.Add(new StiIsoCountry("Dominican Republic").Ru("Доминиканская Республика").Iso("DO", "DOM"));
            countries.Add(new StiIsoCountry("Ecuador").Ru("Эквадор").Iso("EC", "ECU"));
            countries.Add(new StiIsoCountry("Egypt").Ru("Египет").Iso("EG", "EGY"));
            countries.Add(new StiIsoCountry("El Salvador").Ru("Сальвадор").Iso("SV", "SLV"));
            countries.Add(new StiIsoCountry("Equatorial Guinea").Ru("Экваториальная Гвинея").Iso("GQ", "GNQ"));
            countries.Add(new StiIsoCountry("Eritrea").Ru("Эритрея").Iso("ER", "ERI"));
            countries.Add(new StiIsoCountry("Estonia").Ru("Эстония").Iso("EE", "EST"));
            countries.Add(new StiIsoCountry("Ethiopia").Ru("Эфиопия").Iso("ET", "ETH"));
            countries.Add(new StiIsoCountry("European Union").Ru("Европейский союз").Iso("EU", "EUE"));
            countries.Add(new StiIsoCountry("Falkland Islands (Malvinas)").Ru("Фолклендские острова").Iso("FK", "FLK"));
            countries.Add(new StiIsoCountry("Faroe Islands").Ru("Фареры").Iso("FO", "FRO"));
            countries.Add(new StiIsoCountry("Fiji").Ru("Фиджи").Iso("FJ", "FJI"));
            countries.Add(new StiIsoCountry("Finland").Ru("Финляндия").Iso("FI", "FIN"));
            countries.Add(new StiIsoCountry("France").Ru("Франция").Iso("FR", "FRA"));
            countries.Add(new StiIsoCountry("French Guiana").Ru("Французская Гвиана").Iso("GF", "GUF"));
            countries.Add(new StiIsoCountry("French Polynesia").Ru("Французская Полинезия").Iso("PF", "PYF"));
            countries.Add(new StiIsoCountry("French Southern Territories").Ru("Французские Южные и Антарктические территории").Iso("TF", "ATF"));
            countries.Add(new StiIsoCountry("Gabon").Ru("Габон").Iso("GA", "GAB"));
            countries.Add(new StiIsoCountry("Gambia").Ru("Гамбия").Iso("GM", "GMB"));
            countries.Add(new StiIsoCountry("Georgia").Ru("Грузия").Iso("GE", "GEO"));
            countries.Add(new StiIsoCountry("Germany").Ru("Германия").Iso("DE", "DEU"));
            countries.Add(new StiIsoCountry("Ghana").Ru("Гана").Iso("GH", "GHA"));
            countries.Add(new StiIsoCountry("Gibraltar").Ru("Гибралтар").Iso("GI", "GIB"));
            countries.Add(new StiIsoCountry("Greece").Ru("Греция").Iso("GR", "GRC"));
            countries.Add(new StiIsoCountry("Greenland").Ru("Гренландия").Iso("GL", "GRL"));
            countries.Add(new StiIsoCountry("Grenada").Ru("Гренада").Iso("GD", "GRD"));
            countries.Add(new StiIsoCountry("Guadeloupe").Ru("Гваделупа").Iso("GP", "GLP"));
            countries.Add(new StiIsoCountry("Guam").Ru("Гуам").Iso("GU", "GUM"));
            countries.Add(new StiIsoCountry("Guatemala").Ru("Гватемала").Iso("GT", "GTM"));
            countries.Add(new StiIsoCountry("Guernsey").Ru("Гернси").Iso("GG", "GGY"));
            countries.Add(new StiIsoCountry("Guinea").Ru("Гвинея").Iso("GN", "GIN"));
            countries.Add(new StiIsoCountry("Guinea - Bissau").Ru("Гвинея - Бисау").Iso("GW", "GNB"));
            countries.Add(new StiIsoCountry("Guyana").Ru("Гайана").Iso("GY", "GUY"));
            countries.Add(new StiIsoCountry("Haiti").Ru("Гаити").Iso("HT", "HTI"));
            countries.Add(new StiIsoCountry("Heard Island and McDonald Islands").Ru("Остров Херд и острова Макдональд").Iso("HM", "HMD"));
            countries.Add(new StiIsoCountry("Holy See").Ru("Ватикан").Iso("VA", "VAT"));
            countries.Add(new StiIsoCountry("Honduras").Ru("Гондурас").Iso("HN", "HND"));
            countries.Add(new StiIsoCountry("Hong Kong").Ru("Гонконг").Iso("HK", "HKG"));
            countries.Add(new StiIsoCountry("Hungary").Ru("Венгрия").Iso("HU", "HUN"));
            countries.Add(new StiIsoCountry("Iceland").Ru("Исландия").Iso("IS", "ISL"));
            countries.Add(new StiIsoCountry("India").Ru("Индия").Iso("IN", "IND"));
            countries.Add(new StiIsoCountry("Indonesia").Ru("Индонейзия").Iso("ID", "IDN"));
            countries.Add(new StiIsoCountry("Iran").Ru("Иран", "Iran (Islamic Republic of)").Iso("IR", "IRN"));
            countries.Add(new StiIsoCountry("Iraq").Ru("Ирак", "IQ").Iso("IRQ"));
            countries.Add(new StiIsoCountry("Ireland").Ru("Ирландия").Iso("IE", "IRL"));
            countries.Add(new StiIsoCountry("Isle of Man").Ru("Остров Мэн").Iso("IM", "IMN"));
            countries.Add(new StiIsoCountry("Israel").Ru("Израиль").Iso("IL", "ISR"));
            countries.Add(new StiIsoCountry("Italy").Ru("Италия").Iso("IT", "ITA"));
            countries.Add(new StiIsoCountry("Jamaica").Ru("Ямайка").Iso("JM", "JAM"));
            countries.Add(new StiIsoCountry("Japan").Ru("Япония").Iso("JP", "JPN"));
            countries.Add(new StiIsoCountry("Jersey").Ru("Джерси").Iso("JE", "JEY"));
            countries.Add(new StiIsoCountry("Jordan").Ru("Иордания").Iso("JO", "JOR"));
            countries.Add(new StiIsoCountry("Kazakhstan").Ru("Казахстан").Iso("KZ", "KAZ"));
            countries.Add(new StiIsoCountry("Kenya").Ru("Кения").Iso("KE", "KEN"));
            countries.Add(new StiIsoCountry("Kiribati").Ru("Кирибати").Iso("KI", "KIR"));
            countries.Add(new StiIsoCountry("Korea (Democratic People's Republic of)", "North Korea").Ru("КНДР(Корейская Народно - Демократическая Республика)").Iso("KP", "PRK"));
            countries.Add(new StiIsoCountry("Korea (Republic of)", "South Korea").Ru("Республика Корея", "Южная Корея").Iso("KR", "KOR"));
            countries.Add(new StiIsoCountry("Kuwait").Ru("Кувейт").Iso("KW", "KWT"));
            countries.Add(new StiIsoCountry("Kyrgyzstan").Ru("Киргизия").Iso("KG", "KGZ"));
            countries.Add(new StiIsoCountry("Lao People's Democratic Republic").Ru("Лаос").Iso("LA", "LAO"));
            countries.Add(new StiIsoCountry("Latvia").Ru("Латвия").Iso("LV", "LVA"));
            countries.Add(new StiIsoCountry("Lebanon").Ru("Ливия").Iso("LB", "LBN"));
            countries.Add(new StiIsoCountry("Lesotho").Ru("Лесото").Iso("LS", "LSO"));
            countries.Add(new StiIsoCountry("Liberia").Ru("Либерия").Iso("LR", "LBR"));
            countries.Add(new StiIsoCountry("Libya").Ru("Либия").Iso("LY", "LBY"));
            countries.Add(new StiIsoCountry("Liechtenstein").Ru("Лихтенштейн").Iso("LI", "LIE"));
            countries.Add(new StiIsoCountry("Lithuania").Ru("Литва").Iso("LT", "LTU"));
            countries.Add(new StiIsoCountry("Luxembourg").Ru("Люксембург").Iso("LU", "LUX"));
            countries.Add(new StiIsoCountry("Macao").Ru("Макао").Iso("MO", "MAC"));
            countries.Add(new StiIsoCountry("Macedonia", "Macedonia (the former Yugoslav Republic of)").Ru("Македония").Iso("MK", "MKD"));
            countries.Add(new StiIsoCountry("Madagascar").Ru("Мадагаскар").Iso("MG", "MDG"));
            countries.Add(new StiIsoCountry("Malawi").Ru("Малави").Iso("MW", "MWI"));
            countries.Add(new StiIsoCountry("Malaysia").Ru("Малайзия").Iso("MY", "MYS"));
            countries.Add(new StiIsoCountry("Maldives").Ru("Мальдивы").Iso("MV", "MDV"));
            countries.Add(new StiIsoCountry("Mali").Ru("Мали").Iso("ML", "MLI"));
            countries.Add(new StiIsoCountry("Malta").Ru("Мальта").Iso("MT", "MLT"));
            countries.Add(new StiIsoCountry("Marshall Islands").Ru("Маршалловы Острова").Iso("MH", "MHL"));
            countries.Add(new StiIsoCountry("Martinique").Ru("Мартиника").Iso("MQ", "MTQ"));
            countries.Add(new StiIsoCountry("Mauritania").Ru("Мавритания").Iso("MR", "MRT"));
            countries.Add(new StiIsoCountry("Mauritius").Ru("Маврикий").Iso("MU", "MUS"));
            countries.Add(new StiIsoCountry("Mayotte").Ru("Майотта").Iso("YT", "MYT"));
            countries.Add(new StiIsoCountry("Mexico").Ru("Мексика").Iso("MX", "MEX"));
            countries.Add(new StiIsoCountry("Micronesia", "Micronesia (Federated States of)").Ru("Микронезии", "Федеративные Штаты Микронезии").Iso("FM", "FSM"));
            countries.Add(new StiIsoCountry("Moldova", "Moldova (Republic of)").Ru("Молдова", "Молдавия").Iso("MD", "MDA"));
            countries.Add(new StiIsoCountry("Monaco").Ru("Монако").Iso("MC", "MCO"));
            countries.Add(new StiIsoCountry("Mongolia").Ru("Монголия").Iso("MN", "MNG"));
            countries.Add(new StiIsoCountry("Montenegro").Ru("Черногория").Iso("ME", "MNE"));
            countries.Add(new StiIsoCountry("Montserrat").Ru("Монтсеррат").Iso("MS", "MSR"));
            countries.Add(new StiIsoCountry("Morocco").Ru("Марокко").Iso("MA", "MAR"));
            countries.Add(new StiIsoCountry("Mozambique").Ru("Мозамбик").Iso("MZ", "MOZ"));
            countries.Add(new StiIsoCountry("Myanmar").Ru("Мьянма").Iso("MM", "MMR"));
            countries.Add(new StiIsoCountry("Namibia").Ru("Намибия").Iso("NA", "NAM"));
            countries.Add(new StiIsoCountry("Nauru").Ru("Науру").Iso("NR", "NRU"));
            countries.Add(new StiIsoCountry("Nepal").Ru("Непал").Iso("NP", "NPL"));
            countries.Add(new StiIsoCountry("Netherlands").Ru("Нидерланды").Iso("NL", "NLD"));
            countries.Add(new StiIsoCountry("New Caledonia").Ru("Новая Каледония").Iso("NC", "NCL"));
            countries.Add(new StiIsoCountry("New Zealand").Ru("Новая Зеландия").Iso("NZ", "NZL"));
            countries.Add(new StiIsoCountry("Nicaragua").Ru("Никарагуа").Iso("NI", "NIC"));
            countries.Add(new StiIsoCountry("Niger").Ru("Нигер").Iso("NE", "NER"));
            countries.Add(new StiIsoCountry("Nigeria").Ru("Нигерия").Iso("NG", "NGA"));
            countries.Add(new StiIsoCountry("Niue").Ru("Ниуэ").Iso("NU", "NIU"));
            countries.Add(new StiIsoCountry("Norfolk Island").Ru("Остров Норфолк").Iso("NF", "NFK"));
            countries.Add(new StiIsoCountry("Northern Mariana Islands").Ru("Северные Марианские Острова").Iso("MP", "MNP"));
            countries.Add(new StiIsoCountry("Norway").Ru("Норвегия").Iso("NO", "NOR"));
            countries.Add(new StiIsoCountry("Oman").Ru("Оман").Iso("OM", "OMN"));
            countries.Add(new StiIsoCountry("Pakistan").Ru("Пакистан").Iso("PK", "PAK"));
            countries.Add(new StiIsoCountry("Palau").Ru("Палау").Iso("PW", "PLW"));
            countries.Add(new StiIsoCountry("Palestine", "Palestine, State of").Ru("Палестина", "Государство Палестина").Iso("PS", "PSE"));
            countries.Add(new StiIsoCountry("Panama").Ru("Панама").Iso("PA", "PAN"));
            countries.Add(new StiIsoCountry("Papua New Guinea").Ru("Папуа — Новая Гвинея").Iso("PG", "PNG"));
            countries.Add(new StiIsoCountry("Paraguay").Ru("Парагвай").Iso("PY", "PRY"));
            countries.Add(new StiIsoCountry("Peru").Ru("Перу").Iso("PE", "PER"));
            countries.Add(new StiIsoCountry("Philippines").Ru("Филиппины").Iso("PH", "PHL"));
            countries.Add(new StiIsoCountry("Pitcairn").Ru("Острова Питкэрн").Iso("PN", "PCN"));
            countries.Add(new StiIsoCountry("Poland").Ru("Польша").Iso("PL", "POL"));
            countries.Add(new StiIsoCountry("Portugal").Ru("Португалия").Iso("PT", "PRT"));
            countries.Add(new StiIsoCountry("Puerto Rico").Ru("Пуэрто - Рико").Iso("PR", "PRI"));
            countries.Add(new StiIsoCountry("Qatar").Ru("Катар").Iso("QA", "QAT"));
            countries.Add(new StiIsoCountry("Réunion").Ru("Реюньон").Iso("RE", "REU"));
            countries.Add(new StiIsoCountry("Romania").Ru("Румыния").Iso("RO", "ROU"));
            countries.Add(new StiIsoCountry("Russia", "Russian Federation").Ru("Россия", "Российская Федерация").Iso("RU", "RUS"));
            countries.Add(new StiIsoCountry("Rwanda").Ru("Руанда").Iso("RW", "RWA"));
            countries.Add(new StiIsoCountry("Saint Barthélemy").Ru("Сен-Бартелеми").Iso("BL", "BLM"));
            countries.Add(new StiIsoCountry("Saint Helena, Ascension and Tristan da Cunha").Ru("Острова Святой Елены, Вознесения и Тристан-да-Кунья").Iso("SH", "SHN"));
            countries.Add(new StiIsoCountry("Saint Kitts and Nevis").Ru("Сент-Китс и Невис").Iso("KN", "KNA"));
            countries.Add(new StiIsoCountry("Saint Lucia").Ru("Сент-Люсия").Iso("LC", "LCA"));
            countries.Add(new StiIsoCountry("Saint Martin (French part)").Ru("Сен-Мартен (владение Франции)").Iso("MF", "MAF"));
            countries.Add(new StiIsoCountry("Saint Pierre and Miquelon").Ru("Сен-Пьер и Микелон").Iso("PM", "SPM"));
            countries.Add(new StiIsoCountry("Saint Vincent and the Grenadines", "Сент - Винсент и Гренадины").Iso("VC", "VCT"));
            countries.Add(new StiIsoCountry("Samoa").Ru("Самоа").Iso("WS", "WSM"));
            countries.Add(new StiIsoCountry("San Marino").Ru("Сан - Марино").Iso("SM", "SMR"));
            countries.Add(new StiIsoCountry("Sao Tome and Principe").Ru("Сан - Томе и Принсипи").Iso("ST", "STP"));
            countries.Add(new StiIsoCountry("Saudi Arabia", "Arabia").Ru("Саудовская Аравия").Iso("SA", "SAU"));
            countries.Add(new StiIsoCountry("Senegal").Ru("Сенегал").Iso("SN", "SEN"));
            countries.Add(new StiIsoCountry("Serbia").Ru("Сербия").Iso("RS", "SRB"));
            countries.Add(new StiIsoCountry("Seychelles").Ru("Сейшельские Острова").Iso("SC", "SYC"));
            countries.Add(new StiIsoCountry("Sierra Leone").Ru("Сирия").Iso("SL", "SLE"));
            countries.Add(new StiIsoCountry("Singapore").Ru("Сингапур").Iso("SG", "SGP"));
            countries.Add(new StiIsoCountry("Sint Maarten").Ru("Синт-Мартен").Iso("SX", "SXM"));
            countries.Add(new StiIsoCountry("Slovakia").Ru("Словакия").Iso("SK", "SVK"));
            countries.Add(new StiIsoCountry("Slovenia").Ru("Словения").Iso("SI", "SVN"));
            countries.Add(new StiIsoCountry("Solomon Islands").Ru("Соломоновы Острова").Iso("SB", "SLB"));
            countries.Add(new StiIsoCountry("Somalia").Ru("Сомали").Iso("SO", "SOM"));
            countries.Add(new StiIsoCountry("South Africa").Ru("ЮАР").Iso("ZA", "ZAF"));
            countries.Add(new StiIsoCountry("South Georgia and the South Sandwich Islands").Ru("Южная Георгия и Южные Сандвичевы Острова").Iso("GS", "SGS"));
            countries.Add(new StiIsoCountry("South Sudan").Ru("Южный Судан").Iso("SS", "SSD"));
            countries.Add(new StiIsoCountry("Spain").Ru("Испания").Iso("ES", "ESP"));
            countries.Add(new StiIsoCountry("Sri Lanka").Ru("Шри - Ланка").Iso("LK", "LKA"));
            countries.Add(new StiIsoCountry("Sudan").Ru("Судан").Iso("SD", "SDN"));
            countries.Add(new StiIsoCountry("Suriname").Ru("Суринам").Iso("SR", "SUR"));
            countries.Add(new StiIsoCountry("Svalbard and Jan Mayen").Ru("Шпицберген и Ян-Майен").Iso("SJ", "SJM"));
            countries.Add(new StiIsoCountry("Swaziland").Ru("Свазиленд").Iso("SZ", "SWZ"));
            countries.Add(new StiIsoCountry("Sweden").Ru("Швеция").Iso("SE", "SWE"));
            countries.Add(new StiIsoCountry("Switzerland").Ru("Швейцария").Iso("CH", "CHE"));
            countries.Add(new StiIsoCountry("Syrian Arab Republic").Ru("Сирия").Iso("SY", "SYR"));
            countries.Add(new StiIsoCountry("Taiwan", "Taiwan, Province of China[a]").Ru("Китайская Республика").Iso("TW", "TWN"));
            countries.Add(new StiIsoCountry("Tajikistan").Ru("Таджикистан").Iso("TJ", "TJK"));
            countries.Add(new StiIsoCountry("Tanzania", "Tanzania, United Republic of").Ru("Танзания").Iso("TZ", "TZA"));
            countries.Add(new StiIsoCountry("Thailand").Ru("Таиланд").Iso("TH", "THA"));
            countries.Add(new StiIsoCountry("Timor-Leste").Ru("Восточный Тимор").Iso("TL", "TLS"));
            countries.Add(new StiIsoCountry("Togo").Ru("Того").Iso("TG", "TGO"));
            countries.Add(new StiIsoCountry("Tokelau").Ru("Токелау").Iso("TK", "TKL"));
            countries.Add(new StiIsoCountry("Tonga").Ru("Тонга").Iso("TO", "TON"));
            countries.Add(new StiIsoCountry("Trinidad and Tobago").Ru("Тринидад и Тобаго").Iso("TT", "TTO"));
            countries.Add(new StiIsoCountry("Tunisia").Ru("Тунис").Iso("TN", "TUN"));
            countries.Add(new StiIsoCountry("Turkey").Ru("Турция").Iso("TR", "TUR"));
            countries.Add(new StiIsoCountry("Turkmenistan").Ru("Туркмения").Iso("TM", "TKM"));
            countries.Add(new StiIsoCountry("Turks and Caicos Islands").Ru("Тёркс и Кайкос").Iso("TC", "TCA"));
            countries.Add(new StiIsoCountry("Tuvalu").Ru("Тувалу").Iso("TV", "TUV"));
            countries.Add(new StiIsoCountry("Uganda").Ru("Уганда").Iso("UG", "UGA"));
            countries.Add(new StiIsoCountry("Ukraine").Ru("Украина").Iso("UA", "UKR"));
            countries.Add(new StiIsoCountry("United Arab Emirates").Ru("Объединённые Арабские Эмираты").Iso("AE", "ARE"));
            countries.Add(new StiIsoCountry("United Kingdom", "United Kingdom of Great Britain and Northern Ireland", "Great Britain").Ru("Великобритания", "UK").Iso("GB", "GBR"));
            countries.Add(new StiIsoCountry("United States of America", "United States", "U.S.", "U.S. of A", "U.S. of America", "America").Ru("США").Iso("US", "USA"));
            countries.Add(new StiIsoCountry("United States Minor Outlying Islands").Ru("Внешние малые острова (США)").Iso("UM", "UMI"));
            countries.Add(new StiIsoCountry("Uruguay").Ru("Уругвай").Iso("UY", "URY"));
            countries.Add(new StiIsoCountry("Uzbekistan").Ru("Узбекистан").Iso("UZ", "UZB"));
            countries.Add(new StiIsoCountry("Vanuatu").Ru("Вануату").Iso("VU", "VUT"));
            countries.Add(new StiIsoCountry("Venezuela", "Venezuela (Bolivarian Republic of)").Ru("Венесуэла").Iso("VE", "VEN"));
            countries.Add(new StiIsoCountry("Viet Nam").Ru("Вьетнам").Iso("VN", "VNM"));
            countries.Add(new StiIsoCountry("Virgin Islands (British)").Ru("Виргинские Острова (Великобритания)").Iso("VG", "VGB"));
            countries.Add(new StiIsoCountry("Virgin Islands (U.S.)").Ru("Виргинские Острова (США)").Iso("VI", "VIR"));
            countries.Add(new StiIsoCountry("Wallis and Futuna").Ru("Уоллис и Футуна").Iso("WF", "WLF"));
            countries.Add(new StiIsoCountry("Western Sahara").Ru("Западная Сахара").Iso("EH", "ESH"));
            countries.Add(new StiIsoCountry("Yemen").Ru("Йемен").Iso("YE", "YEM"));
            countries.Add(new StiIsoCountry("Zambia").Ru("Замбия").Iso("ZM", "ZMB"));
            countries.Add(new StiIsoCountry("Zimbabwe").Ru("Зимбабве").Iso("ZW", "ZWE"));
        }

        private static void InitializeUsStates()
        {
            usStates.Add(new StiIsoCountry("Alabama", "Ala.").Iso("AL").Ru("Алабама"));
            usStates.Add(new StiIsoCountry("Alaska", "Alas.").Iso("AK").Ru("Аляска"));
            usStates.Add(new StiIsoCountry("American Samoa", "A.S").Iso("AS").Ru("Американское Самоа"));
            usStates.Add(new StiIsoCountry("Arizona", "Ariz.").Iso("AZ").Ru("Аризона"));
            usStates.Add(new StiIsoCountry("Arkansas", "Ark.").Iso("AR").Ru("Арканзас"));
            usStates.Add(new StiIsoCountry("California", "Calif.", "Ca.", "Cal.").Iso("CA").Ru("Калифорния"));
            usStates.Add(new StiIsoCountry("Colorado", "Colo.", "Col.").Iso("CO").Ru("Колорадо"));
            usStates.Add(new StiIsoCountry("Connecticut", "Conn.", "Ct.").Iso("CT").Ru("Коннектикут"));
            usStates.Add(new StiIsoCountry("Delaware", "Del.", "Ct.").Iso("DE").Ru("Делавэр"));
            usStates.Add(new StiIsoCountry("District of Columbia", "D.C.", "Wash D.C.").Iso("DC").Ru("Округ Колумбия"));
            usStates.Add(new StiIsoCountry("Florida", "Fla.", "Fl.", "Flor.").Iso("FL").Ru("Флорида"));
            usStates.Add(new StiIsoCountry("Georgia", "Ga.", "Geo.").Iso("GA").Ru("Джорджия"));
            usStates.Add(new StiIsoCountry("Guam", "GUM").Iso("GU").Ru("Гуам"));
            usStates.Add(new StiIsoCountry("Hawaii", "H.I.").Iso("HI").Ru("Гавайи"));
            usStates.Add(new StiIsoCountry("Idaho", "Id.", "Ida.").Iso("ID").Ru("Айдахо"));
            usStates.Add(new StiIsoCountry("Illinois", "Ill.").Iso("IL").Ru("Иллинойс"));
            usStates.Add(new StiIsoCountry("Indiana", "Ind.", "In.").Iso("IN").Ru("Индиана"));
            usStates.Add(new StiIsoCountry("Iowa", "Ia.", "Ioa.").Iso("IA").Ru("Айова"));
            usStates.Add(new StiIsoCountry("Kansas", "Kans.", "Kan.", "Ks", "Ka").Iso("KS").Ru("Канзас"));
            usStates.Add(new StiIsoCountry("Kentucky", "Ky.", "Ken.", "Kent.").Iso("KY").Ru("Кентукки"));
            usStates.Add(new StiIsoCountry("Louisiana", "La.").Iso("LA").Ru("Луизиана"));
            usStates.Add(new StiIsoCountry("Maine", "Me.").Iso("ME").Ru("Мэн"));
            usStates.Add(new StiIsoCountry("Maryland", "Md.").Iso("MD").Ru("Мэриленд"));
            usStates.Add(new StiIsoCountry("Massachusetts", "Mass.").Iso("MA").Ru("Массачусетс"));
            usStates.Add(new StiIsoCountry("Michigan", "Mich.").Iso("MI").Ru("Мичиган"));
            usStates.Add(new StiIsoCountry("Minnesota", "Minn.", "Mn.").Iso("MN").Ru("Миннесота"));
            usStates.Add(new StiIsoCountry("Mississippi", "Miss.").Iso("MS").Ru("Миссисипи"));
            usStates.Add(new StiIsoCountry("Missouri", "Mo.").Iso("MO").Ru("Миссури"));
            usStates.Add(new StiIsoCountry("Montana", "Mont.").Iso("MT").Ru("Монтана"));
            usStates.Add(new StiIsoCountry("Nebraska", "Nebr.", "Neb.").Iso("NE").Ru("Небраска"));
            usStates.Add(new StiIsoCountry("Nevada", "Nev.", "Nv.").Iso("NV").Ru("Невада"));
            usStates.Add(new StiIsoCountry("New Hampshire", "N.H.").Iso("NH").Ru("Нью-Гемпшир"));
            usStates.Add(new StiIsoCountry("New Jersey", "N.J.", "N. Jersey").Iso("NJ").Ru("Нью-Джерси"));
            usStates.Add(new StiIsoCountry("New Mexico", "N. Mex.", "N.M.", "New M.").Iso("NM").Ru("Нью-Мексико"));
            usStates.Add(new StiIsoCountry("New York", "N.Y.", "N. York").Iso("NY").Ru("Нью-Йорк"));
            usStates.Add(new StiIsoCountry("North Carolina", "N.C.", "N. Car.").Iso("NC").Ru("Северная Каролина"));
            usStates.Add(new StiIsoCountry("North Dakota", "N. Dak.", "N.D.", "NoDak").Iso("ND").Ru("Северная Дакота"));
            usStates.Add(new StiIsoCountry("Northern Mariana Islands", "M.P.", "CNMI").Iso("MP", "MNP").Ru("Северные Марианские острова"));
            usStates.Add(new StiIsoCountry("Ohio", "O.", "Oh.").Iso("OH").Ru("Огайо"));
            usStates.Add(new StiIsoCountry("Oklahoma", "Okla.", "Ok.").Iso("OK").Ru("Оклахома"));
            usStates.Add(new StiIsoCountry("Oregon", "Oreg.", "Ore.", "Or.").Iso("OR").Ru("Орегон"));
            usStates.Add(new StiIsoCountry("Pennsylvania", "Pa.", "Penn.", "Penna.").Iso("PA").Ru("Пенсильвания"));
            usStates.Add(new StiIsoCountry("Puerto Rico", "P.R.").Iso("PR", "PRI").Ru("Пуэрто - Рико"));
            usStates.Add(new StiIsoCountry("Rhode Island", "R.I.", "P.P.", "R. Isl.").Iso("RI").Ru("Род-Айленд"));
            usStates.Add(new StiIsoCountry("South Carolina", "S.C.", "S. Car.").Iso("SC").Ru("Южная Каролина"));
            usStates.Add(new StiIsoCountry("South Dakota", "S. Dak.", "S.D.", "SoDak").Iso("SD").Ru("Южная Дакота"));
            usStates.Add(new StiIsoCountry("Tennessee", "Tenn.").Iso("TN").Ru("Теннесси"));
            usStates.Add(new StiIsoCountry("Texas", "Tex.", "Tx.").Iso("TX").Ru("Техас"));
            usStates.Add(new StiIsoCountry("US Minor Outlying Islands").Iso("WY", "UMI").Ru("Внешние малые острова"));
            usStates.Add(new StiIsoCountry("US Virgin Islands", "V.I.", "U.S.V.I.").Iso("VI", "ASM").Ru("Американские Виргинские острова"));
            usStates.Add(new StiIsoCountry("Utah", "Ut.").Iso("UT").Ru("Юта"));
            usStates.Add(new StiIsoCountry("Vermont", "Vt.").Iso("VT").Ru("Вермонт"));
            usStates.Add(new StiIsoCountry("Virginia", "Va.", "Virg.").Iso("VA", "VIR").Ru("Виргиния"));
            usStates.Add(new StiIsoCountry("Washington", "Wash.", "Wa.", "Wn.").Iso("WA").Ru("Вашингтон"));
            usStates.Add(new StiIsoCountry("West Virginia", "W. Va.", "W.V.", "W. Virg.").Iso("WV").Ru("Западная Виргиния"));
            usStates.Add(new StiIsoCountry("Wisconsin", "Wis.", "Wi.", "Wisc.").Iso("WI").Ru("Висконсин"));
            usStates.Add(new StiIsoCountry("Wyoming", "Wyo.", "Wy.").Iso("WY").Ru("Вайоминг"));
        }

        private static void InitializeCanadaProvinces()
        {
            canadaProvinces.Add(new StiIsoCountry("Alberta").Iso("AB").Fr("Alberta").Ru("Альберта"));
            canadaProvinces.Add(new StiIsoCountry("British Columbia").Iso("BC").Fr("Colombie-Britannique").Ru("Британская Колумбия"));
            canadaProvinces.Add(new StiIsoCountry("Manitoba").Iso("MB").Fr("Manitoba").Ru("Манитоба"));
            canadaProvinces.Add(new StiIsoCountry("New Brunswick").Iso("NB").Fr("Nouveau-Brunswick").Ru("Нью-Брансуик"));
            canadaProvinces.Add(new StiIsoCountry("Newfoundland and Labrador").Iso("NL").Fr("Terre-Neuve-et-Labrador").Ru("Ньюфаундленд и Лабрадор"));
            canadaProvinces.Add(new StiIsoCountry("Nova Scotia").Iso("NS").Fr("Nouvelle-Écosse").Ru("Новая Шотландия"));
            canadaProvinces.Add(new StiIsoCountry("Northwest Territories").Iso("NT").Fr("Territoires du Nord-Ouest").Ru("Северо-западные территории"));
            canadaProvinces.Add(new StiIsoCountry("Nunavut").Iso("NU").Fr("Nunavut").Ru("Нунавут"));
            canadaProvinces.Add(new StiIsoCountry("Ontario").Iso("ON").Fr("Ontario").Ru("Онтарио"));
            canadaProvinces.Add(new StiIsoCountry("Prince Edward Island").Iso("PE").Fr("Île-du-Prince-Édouard").Ru("Остров Принца Эдуарда"));
            canadaProvinces.Add(new StiIsoCountry("Quebec").Iso("QC").Fr("Québec").Ru("Квебек"));
            canadaProvinces.Add(new StiIsoCountry("Saskatchewan").Iso("SK").Fr("Saskatchewan").Ru("Саскачеван"));
            canadaProvinces.Add(new StiIsoCountry("Yukon").Iso("YT").Fr("Yukon").Ru("Юкон"));
        }

        private static void InitializeBrazilProvinces()
        {
            brazilProvinces.Add(new StiIsoCountry("Roraima").Iso("AB").Fr("Roraima").Ru("Рорайма"));
            brazilProvinces.Add(new StiIsoCountry("Amazonas").Iso("AM").Fr("Amazonas").Ru("Амазонас"));
            brazilProvinces.Add(new StiIsoCountry("Para").Iso("PA").Fr("Pará").Ru("Пара"));
            brazilProvinces.Add(new StiIsoCountry("Amapa").Iso("AP").Fr("Amapá").Ru("Амапа"));
            brazilProvinces.Add(new StiIsoCountry("Acre").Iso("AC").Fr("Acre").Ru("Акри"));
            brazilProvinces.Add(new StiIsoCountry("Rondonia").Iso("RO").Fr("Rondônia").Ru("Рондония"));
            brazilProvinces.Add(new StiIsoCountry("Mato Grosso").Iso("MT").Fr("Mato Grosso").Ru("Мату-Гросу"));
            brazilProvinces.Add(new StiIsoCountry("Maranhao").Iso("MA").Fr("Maranhão").Ru("Мараньян"));
            brazilProvinces.Add(new StiIsoCountry("Piaui").Iso("PI").Fr("Piauí").Ru("Пиауи"));
            brazilProvinces.Add(new StiIsoCountry("Ceara").Iso("CE").Fr("Ceará").Ru("Сеара"));
            brazilProvinces.Add(new StiIsoCountry("Rio Grande do Norte").Iso("RN").Fr("Rio Grande do Norte").Ru("Риу-Гранди-ду-Норти"));
            brazilProvinces.Add(new StiIsoCountry("Paraiba").Iso("PB").Fr("Paraïba").Ru("Параиба"));
            brazilProvinces.Add(new StiIsoCountry("Pernambuco").Iso("PE").Fr("Pernambouc").Ru("Пернамбуку"));
            brazilProvinces.Add(new StiIsoCountry("Alagoas").Iso("AL").Fr("Alagoas").Ru("Алагоас"));
            brazilProvinces.Add(new StiIsoCountry("Sergipe").Iso("SE").Fr("Sergipe").Ru("Сержипи"));
            brazilProvinces.Add(new StiIsoCountry("Bahia").Iso("BA").Fr("Bahia").Ru("Баия"));
            brazilProvinces.Add(new StiIsoCountry("Tocantins").Iso("TO").Fr("Tocantins").Ru("Токантинс"));
            brazilProvinces.Add(new StiIsoCountry("Goias").Iso("GO").Fr("Goiás").Ru("Гояс"));
            brazilProvinces.Add(new StiIsoCountry("Minas Gerais").Iso("MG").Fr("Minas Gerais").Ru("Минас-Жерайс"));
            brazilProvinces.Add(new StiIsoCountry("Espirito Santo").Iso("ES").Fr("Minas Gerais").Ru("Минас-Жерайс"));
            brazilProvinces.Add(new StiIsoCountry("Rio de Janeiro").Iso("RJ").Fr("Rio de Janeiro").Ru("Рио-де-Жанейро"));
            brazilProvinces.Add(new StiIsoCountry("Sao Paulo").Iso("SP").Fr("São Paulo").Ru("Сан-Паулу"));
            brazilProvinces.Add(new StiIsoCountry("Parana").Iso("PR").Fr("Paraná").Ru("Парана"));
            brazilProvinces.Add(new StiIsoCountry("Santa Catarina").Iso("SC").Fr("Santa Catarina").Ru("Санта-Катарина"));
            brazilProvinces.Add(new StiIsoCountry("Rio Grande do Sul").Iso("RS").Fr("Rio Grande do Sul").Ru("Риу-Гранди-ду-Сул"));
            brazilProvinces.Add(new StiIsoCountry("Mato Grosso do Sul").Iso("MS").Fr("Mato Grosso do Sul").Ru("Мату-Гросу-ду-Сул"));
        }
        #endregion
    }
}
