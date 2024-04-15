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

using System.Collections.Generic;

namespace Stimulsoft.Report.Maps.Helpers
{
    public static class StiGssMapHelper
    {
        #region Fields
        //https://de.wikipedia.org/wiki/Office_for_National_Statistics
        private static Dictionary<string, Dictionary<string, string>> hash;
        #endregion

        #region Methods
        public static bool AllowGss(string id)
        {
            return (id == StiMapID.UKCountries.ToString());
        }

        public static Dictionary<string, string> Get(string id)
        {
            if (!AllowGss(id)) return null;

            if (hash == null)
                hash = new Dictionary<string, Dictionary<string, string>>();

            if (!hash.ContainsKey(id))
            {
                Init(id);
            }

            return hash[id];
        }

        private static void Init(string id)
        {
            if (id == StiMapID.UKCountries.ToString())
            {
                InitUKCountries();
            }
        }
        public static bool IsGssValue(string value)
        {
            if (value == null) return false;

            string value1 = value.ToLowerInvariant();
            return (value1.StartsWith("s12") ||
                value1.StartsWith("e06") ||
                value1.StartsWith("e10") ||
                value1.StartsWith("e07") ||
                value1.StartsWith("e09") ||
                value1.StartsWith("n09") ||
                value1.StartsWith("w06") ||
                value1.StartsWith("s12") ||
                value1.StartsWith("e08") ||
                value1.StartsWith("e11"));
        }
        #endregion

        #region Methods.Init
        private static void Add(Dictionary<string, string> data, string state, string gss)
        {
            if (gss != null)
                data.Add(gss.ToLowerInvariant(), state);
        }

        #region UKCountries

        private static void InitUKCountries()
        {
            var data = new Dictionary<string, string>();

            Add(data, "EastAyrshire", "S12000008");
            Add(data, "EastDunbartonshire", "S12000045");
            Add(data, "EastRenfrewshire", "S12000011");
            Add(data, "Glasgow", "S12000046");
            Add(data, "Inverclyde", "S12000018");
            Add(data, "NorthAyshire", "S12000021");
            Add(data, "NorthLanarkshire", "S12000044");
            Add(data, "Renfrewshire", "S12000038");
            Add(data, "SouthAyrshire", "S12000028");
            Add(data, "SouthLanarkshire", "S12000029");
            Add(data, "WestDunbartonshire", "S12000039");
            Add(data, "Aberdeen", "S12000033");
            Add(data, "Aberdeenshire", "S12000034");
            Add(data, "Moray", "S12000020");
            Add(data, "Falkirk", "S12000014");
            Add(data, "Stirling", "S12000030");
            Add(data, "Clackmannanshire", "S12000005");
            Add(data, "PerthshireAndKinross", "S12000024");
            Add(data, "Angus", "S12000041");
            Add(data, "Dundee", "S12000042");
            Add(data, "Fife", "S12000015");
            Add(data, "EastLothian", "S12000010");
            Add(data, "Edinburgh", "S12000036");
            Add(data, "Midlothian", "S12000019");
            Add(data, "WestLothian", "S12000040");
            Add(data, "ScottishBorders", "S12000026");
            Add(data, "Darlington", "E06000005");
            Add(data, "Durham", "E06000047");
            Add(data, "Hartlepool", "E06000001");
            Add(data, "Middlesbrough", "E06000002");
            Add(data, "RedcarAndCleveland", "E06000003");
            Add(data, "StocktonOnTees", "E06000004");
            Add(data, "Northumberland", "E06000057");
            Add(data, "Hampshire", "E10000014");
            Add(data, "Southampton", "E06000045");
            Add(data, "MiltonKeynes", "E06000042");
            Add(data, "Gloucestershire", "E07000081");
            Add(data, "Buckinghamshire", "E10000002");
            Add(data, "Hertfordshire", "E10000015");
            Add(data, "BathAndNorthEastSomerset", "E06000022");
            Add(data, "Bristol", "E06000023");
            Add(data, "NorthSomerset", "E06000024");
            Add(data, "SouthGloucestershire", "E06000025");
            Add(data, "Somerset", "E10000027");
            Add(data, "Devon", "E10000008");
            Add(data, "Bournemouth", "E06000028");
            Add(data, "Dorset", "E10000009");
            Add(data, "Poole", "E06000029");
            Add(data, "Cambridgeshire", "E07000008");
            Add(data, "Leicestershire", "E07000134");
            Add(data, "KingstonUponHull", "E06000010");
            Add(data, "NorthEastLincolnshire", "E06000012");
            Add(data, "NorthLincolnshire", "E06000013");
            Add(data, "Derby", "E06000015");
            Add(data, "Derbyshire", "E07000035");
            Add(data, "BarkingAndDagenham", "E09000002");
            Add(data, "Bexley", "E09000004");
            Add(data, "Brent", "E09000005");
            Add(data, "Bromley", "E09000006");
            Add(data, "Camden", "E09000007");
            Add(data, "Croydon", "E09000008");
            Add(data, "Ealing", "E09000009");
            Add(data, "Enfield", "E09000010");
            Add(data, "Greenwich", "E09000011");
            Add(data, "HammersmithAndFulham", "E09000013");
            Add(data, "Hounslow", "E09000018");
            Add(data, "Islington", "E09000019");
            Add(data, "KensingtonAndChelsea", "E09000020");
            Add(data, "Merton", "E09000024");
            Add(data, "Redbridge", "E09000026");
            Add(data, "RichmondUponThames", "E09000027");
            Add(data, "Sutton", "E09000029");
            Add(data, "TowerHamlets", "E09000030");
            Add(data, "WalthamForest", "E09000031");
            Add(data, "Wandsworth", "E09000032");
            Add(data, "Westminster", "E09000033");
            Add(data, "Lincolnshire", "E07000138");
            Add(data, "Belfast", "N09000003");
            Add(data, "Derry", null);
            Add(data, "Omagh", null);
            Add(data, "Armagh", null);
            Add(data, "NewryAndMourne", null);
            Add(data, "Banbridge", null);
            Add(data, "Craigavon", null);
            Add(data, "Dungannon", null);
            Add(data, "Lisburn", null);
            Add(data, "Cookstown", null);
            Add(data, "Antrim", null);
            Add(data, "Magherafelt", null);
            Add(data, "Ballymena", null);
            Add(data, "Larne", null);
            Add(data, "Carrickfergus", null);
            Add(data, "Newtownabbey", null);
            Add(data, "NorthDown", null);
            Add(data, "Down", null);
            Add(data, "Coleraine", null);
            Add(data, "Ballymoney", null);
            Add(data, "Limavady", null);
            Add(data, "Castlereagh", null);
            Add(data, "Carmarthenshire", "W06000010");
            Add(data, "Ceredigion", "W06000008");
            Add(data, "Pembrokeshire", "W06000009");
            Add(data, "Cornwall", "E06000052");
            Add(data, "Powys", "W06000023");
            Add(data, "Bridgend", "W06000013");
            Add(data, "Caerphilly", "W06000018");
            Add(data, "MerthyrTydfil", "W06000024");
            Add(data, "RhonddaCynonTaf", "W06000016");
            Add(data, "Cardiff", "W06000015");
            Add(data, "ValeOfGlamorgan", "W06000014");
            Add(data, "NeathPortTalbot", "W06000012");
            Add(data, "Swansea", "W06000011");
            Add(data, "York", "E06000014");
            Add(data, "TelfordAndWrekin", "E06000020");
            Add(data, "BlackburnWithDarwen", "E06000008");
            Add(data, "Lancashire", "E10000017");
            Add(data, "EastRidingOfYorkshire", "E06000011");
            Add(data, "Denbighshire", "W06000004");
            Add(data, "Flintshire", "W06000005");
            Add(data, "Wrexham", "W06000006");
            Add(data, "Anglesey", "W06000001");
            Add(data, "Conwy", "W06000003");
            Add(data, "Gwynedd", "W06000002");
            Add(data, "BlaenauGwent", "W06000019");
            Add(data, "Monmouthshire", "W06000021");
            Add(data, "Newport", "W06000022");
            Add(data, "Torfaen", "W06000020");
            Add(data, "Strabane", null);
            Add(data, "Fermanagh", null);
            Add(data, "Ards", null);
            Add(data, "DumfriesAndGalloway", "S12000006");
            Add(data, "Cumbria", "E10000006");
            Add(data, "NorthYorkshire", "E10000023");
            Add(data, "Plymouth", "E06000026");
            Add(data, "Torbay", "E06000027");
            Add(data, "Essex", "E10000012");
            Add(data, "Suffolk", "E10000029");
            Add(data, "Norfolk", "E10000020");
            Add(data, "BrightonAndHove", "E06000043");
            Add(data, "Havering", "E09000016");
            Add(data, "Thurrock", "E06000034");
            Add(data, "EastSussex", "E10000011");
            Add(data, "Medway", "E06000035");
            Add(data, "SouthendOnSea", "E06000033");
            Add(data, "Orkney", "S12000023");
            Add(data, "Highland", "S12000017");
            Add(data, "ArgyllAndBute", "S12000035");
            Add(data, "ShetlandIslands", "S12000027");
            Add(data, "WestSussex", "E10000032");
            Add(data, "Northamptonshire", "E07000152");
            Add(data, "Warwickshire", "E07000218");
            Add(data, "Oxfordshire", "E07000178");
            Add(data, "Luton", "E06000032");
            Add(data, "Hillingdon", "E09000017");
            Add(data, "KingstonUponThames", "E09000021");
            Add(data, "Surrey", "E10000030");
            Add(data, "Swindon", "E06000030");
            Add(data, "Wiltshire", "E06000054");
            Add(data, "IsleOfWight", "E06000046");
            Add(data, "Portsmouth", "E06000044");
            Add(data, "Peterborough", "E06000031");
            Add(data, "Leicester", "E06000016");
            Add(data, "Rutland", "E06000017");
            Add(data, "Nottingham", "E06000018");
            Add(data, "Nottinghamshire", "E10000024");
            Add(data, "Hackney", "E09000012");
            Add(data, "Haringey", "E09000014");
            Add(data, "Harrow", "E09000015");
            Add(data, "Lambeth", "E09000022");
            Add(data, "Lewisham", "E09000023");
            Add(data, "Newham", "E09000025");
            Add(data, "Southwark", "E09000028");
            Add(data, "EileanSiar", "S12000013");
            Add(data, "Moyle", null);
            Add(data, "Warrington", "E06000007");
            Add(data, "Herefordshire", "E06000019");
            Add(data, "Worcestershire", "E07000237");
            Add(data, "Staffordshire", "E07000193");
            Add(data, "StokeOnTrent", "E06000021");
            Add(data, "Shropshire", "E06000051");
            Add(data, "Kent", "E10000016");
            Add(data, "City", "E09000001");
            Add(data, "NewcastleUponTyne", "E08000021");
            Add(data, "NorthTyneside", "E08000022");
            Add(data, "SouthTyneside", "E08000023");
            Add(data, "Sunderland", "E08000024");
            Add(data, "Gateshead", "E08000037");
            Add(data, "Knowsley", "E08000011");
            Add(data, "Sefton", "E08000014");
            Add(data, "Liverpool", "E08000012");
            Add(data, "Merseyside", "E11000002");
            Add(data, "Blackpool", "E06000009");
            Add(data, "Kirklees", "E08000034");
            Add(data, "Calderdale", "E08000033");
            Add(data, "Bradford", "E08000032");
            Add(data, "Leeds", "E08000035");
            Add(data, "Wakefield", "E08000036");
            Add(data, "Salford", "E08000006");
            Add(data, "Wigan", "E08000010");
            Add(data, "Bolton", "E08000001");
            Add(data, "Bury", "E08000002");
            Add(data, "Rochdale", "E08000005");
            Add(data, "Oldham", "E08000004");
            Add(data, "Tameside", "E08000008");
            Add(data, "Stockport", "E08000007");
            Add(data, "Manchester", "E08000003");
            Add(data, "Trafford", "E08000009");
            Add(data, "Rotherham", "E08000018");
            Add(data, "Sheffield", "E08000019");
            Add(data, "Barnsley", "E08000016");
            Add(data, "Doncaster", "E08000017");
            Add(data, "Birmingham", "E08000025");
            Add(data, "Sandwell", "E08000028");
            Add(data, "Dudley", "E08000027");
            Add(data, "Wolverhampton", "E08000031");
            Add(data, "Walsall", "E08000030");
            Add(data, "Solihull", "E08000029");
            Add(data, "Coventry", "E08000026");
            Add(data, "CentralBedfordshire", "E06000056");
            Add(data, "Bedford", "E06000055");
            Add(data, "Reading", "E06000038");
            Add(data, "WestBerkshire", "E06000037");
            Add(data, "Wokingham", "E06000041");
            Add(data, "BracknellForest", "E06000036");
            Add(data, "RoyalBoroughOfWindsorAndMaidenhead", "E06000040");
            Add(data, "Slough", "E06000039");
            Add(data, "Barnet", "E09000003");
            Add(data, "CheshireEast", "E06000049");
            Add(data, "CheshireWestAndChester", "E06000050");
            Add(data, "Halton", "E06000006");
            Add(data, "IslesOfScilly", "E06000053");

            hash.Add(StiMapID.UKCountries.ToString(), data);
        }
        #endregion
        #endregion
    }
}