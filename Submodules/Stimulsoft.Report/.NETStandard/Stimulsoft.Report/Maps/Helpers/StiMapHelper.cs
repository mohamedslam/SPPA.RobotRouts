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

using Stimulsoft.Base.Drawing;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Maps.Helpers
{
    public static class StiMapHelper
    {
        #region Fields
        private static StiReport globalReport;
        private static StiMap globalMap;
        #endregion

        #region Properties
        private static string defaultMapId;
        public static string DefaultMapId
        {
            get
            {
                if (defaultMapId == null)
                {
                    StiSettings.Load();
                    defaultMapId = StiSettings.GetStr("DbsMapEditor", "LastMapID", "USA");
                }

                return defaultMapId;
            }
            set
            {
                defaultMapId = value;

                StiSettings.Load();
                StiSettings.Set("DbsMapEditor", "LastMapID", value);
                StiSettings.Save();
            }
        }

        public static bool IsWorld(StiMapID id)
        {
            return id == StiMapID.World;
        }

        public static bool IsAfrica(StiMapID id)
        {
            return id == StiMapID.SouthAfrica;
        }

        public static bool IsNorthAmerica(StiMapID id)
        {
            switch (id)
            {
                case StiMapID.USA:
                case StiMapID.Canada:
                case StiMapID.Mexico:
                    return true;
            }

            return false;
        }

        public static bool IsSouthAmerica(StiMapID id)
        {
            switch (id)
            {
                case StiMapID.Argentina:
                case StiMapID.Bolivia:
                case StiMapID.Brazil:
                case StiMapID.Chile:
                case StiMapID.Colombia:
                case StiMapID.Ecuador:
                case StiMapID.FalklandIslands:
                case StiMapID.Guyana:
                case StiMapID.Paraguay:
                case StiMapID.Peru:
                case StiMapID.Suriname:
                case StiMapID.Uruguay:
                case StiMapID.Venezuela:
                    return true;
            }

            return false;
        }

        public static bool IsEU(StiMapID id)
        {
            switch (id)
            {
                case StiMapID.Albania:
                case StiMapID.Andorra:
                case StiMapID.Austria:
                case StiMapID.Belarus:
                case StiMapID.Belgium:
                case StiMapID.BosniaAndHerzegovina:
                case StiMapID.Bulgaria:
                case StiMapID.Croatia:
                case StiMapID.CzechRepublic:
                case StiMapID.Denmark:
                case StiMapID.Estonia:
                case StiMapID.EU:
                case StiMapID.EUWithUnitedKingdom:
                case StiMapID.Finland:
                case StiMapID.France:
                case StiMapID.Georgia:
                case StiMapID.Germany:
                case StiMapID.Greece:
                case StiMapID.Hungary:
                case StiMapID.Iceland:
                case StiMapID.Ireland:
                case StiMapID.Italy:
                case StiMapID.Latvia:
                case StiMapID.Liechtenstein:
                case StiMapID.Lithuania:
                case StiMapID.Luxembourg:
                case StiMapID.Macedonia:
                case StiMapID.Malta:
                case StiMapID.Moldova:
                case StiMapID.Monaco:
                case StiMapID.Montenegro:
                case StiMapID.Netherlands:
                case StiMapID.Norway:
                case StiMapID.Poland:
                case StiMapID.Portugal:
                case StiMapID.Romania:
                case StiMapID.Russia:
                case StiMapID.SanMarino:
                case StiMapID.Serbia:
                case StiMapID.Slovakia:
                case StiMapID.Slovenia:
                case StiMapID.Spain:
                case StiMapID.Sweden:
                case StiMapID.Switzerland:
                case StiMapID.Turkey:
                case StiMapID.UK:
                case StiMapID.UKCountries:
                case StiMapID.Ukraine:
                case StiMapID.Vatican:
                    return true;
            }

            return false;
        }

        public static bool IsOceania(StiMapID id)
        {
            switch (id)
            {
                case StiMapID.Australia:
                case StiMapID.Indonesia:
                case StiMapID.NewZealand:
                case StiMapID.Oceania:
                    return true;
            }

            return false;
        }

        public static bool IsAsia(StiMapID id)
        {
            switch (id)
            {
                case StiMapID.Armenia:
                case StiMapID.Azerbaijan:
                case StiMapID.China:
                case StiMapID.Cyprus:
                case StiMapID.India:
                case StiMapID.Israel:
                case StiMapID.Japan:
                case StiMapID.Kazakhstan:
                case StiMapID.Malaysia:
                case StiMapID.Philippines:
                case StiMapID.SaudiArabia:
                case StiMapID.SouthKorea:
                case StiMapID.Thailand:
                case StiMapID.Vietnam:
                case StiMapID.MiddleEast:
                case StiMapID.Oman:
                case StiMapID.Qatar:
                case StiMapID.Afghanistan:
                    return true;
            }

            return false;
        }
        #endregion

        #region Methods
        public static string[] GetStates(StiReport report, string id, string lang)
        {
            var container = StiMapLoader.LoadResource(report, id, lang);
            if (container == null) return new string[0];

            return container.HashPaths.Keys.ToArray();
        }

        public static StiMap GetMapSample(StiReport baseReport, string mapID = "USA")
        {
            if (globalReport == null)
                globalReport = new StiReport();

            globalReport.Styles.Clear();
            if (baseReport != null && baseReport.Styles.Count > 0)
                globalReport.Styles.AddRange(baseReport.Styles);

            if (globalMap == null)
            {
                globalMap = new StiMap();
                globalMap.MapData = "[{\"Key\":\"Alabama\",\"Group\":\"3\"},{\"Key\":\"Alaska\",\"Group\":\"1\"},{\"Key\":\"Arizona\",\"Group\":\"1\"},{\"Key\":\"Arkansas\",\"Group\":\"2\"},{\"Key\":\"California\",\"Group\":\"1\"},{\"Key\":\"Colorado\",\"Group\":\"1\"},{\"Key\":\"Connecticut\",\"Group\":\"3\"},{\"Key\":\"Delaware\",\"Group\":\"3\"},{\"Key\":\"Florida\",\"Group\":\"3\"},{\"Key\":\"Georgia\",\"Group\":\"3\"},{\"Key\":\"Hawaii\",\"Group\":\"2\"},{\"Key\":\"Idaho\",\"Group\":\"1\"},{\"Key\":\"Illinois\",\"Group\":\"3\"},{\"Key\":\"Indiana\",\"Group\":\"3\"},{\"Key\":\"Iowa\",\"Group\":\"2\"},{\"Key\":\"Kansas\",\"Group\":\"2\"},{\"Key\":\"Kentucky\",\"Group\":\"3\"},{\"Key\":\"Louisiana\",\"Group\":\"2\"},{\"Key\":\"Maine\",\"Group\":\"3\"},{\"Key\":\"Maryland\",\"Group\":\"3\"},{\"Key\":\"Massachusetts\",\"Group\":\"3\"},{\"Key\":\"Michigan\",\"Group\":\"3\"},{\"Key\":\"Minnesota\",\"Group\":\"2\"},{\"Key\":\"Mississippi\",\"Group\":\"3\"},{\"Key\":\"Missouri\",\"Group\":\"2\"},{\"Key\":\"Montana\",\"Group\":\"1\"},{\"Key\":\"Nebraska\",\"Group\":\"2\"},{\"Key\":\"Nevada\",\"Group\":\"1\"},{\"Key\":\"NewHampshire\",\"Group\":\"3\"},{\"Key\":\"NewJersey\",\"Group\":\"3\"},{\"Key\":\"NewMexico\",\"Group\":\"1\"},{\"Key\":\"NewYork\",\"Group\":\"3\"},{\"Key\":\"NorthCarolina\",\"Group\":\"3\"},{\"Key\":\"NorthDakota\",\"Group\":\"2\"},{\"Key\":\"Ohio\",\"Group\":\"3\"},{\"Key\":\"Oklahoma\",\"Group\":\"2\"},{\"Key\":\"Oregon\",\"Group\":\"1\"},{\"Key\":\"Pennsylvania\",\"Group\":\"3\"},{\"Key\":\"RhodeIsland\",\"Group\":\"3\"},{\"Key\":\"SouthCarolina\",\"Group\":\"3\"},{\"Key\":\"SouthDakota\",\"Group\":\"2\"},{\"Key\":\"Tennessee\",\"Group\":\"3\"},{\"Key\":\"Texas\",\"Group\":\"2\"},{\"Key\":\"Utah\",\"Group\":\"1\"},{\"Key\":\"Vermont\",\"Group\":\"3\"},{\"Key\":\"Virginia\",\"Group\":\"3\"},{\"Key\":\"Washington\",\"Group\":\"1\"},{\"Key\":\"WestVirginia\",\"Group\":\"3\"},{\"Key\":\"Wisconsin\",\"Group\":\"3\"},{\"Key\":\"Wyoming\",\"Group\":\"1\"}]";

                globalReport.Pages[0].Components.Add(globalMap);
            }

            globalMap.Stretch = true;
            globalMap.MapType = StiMapType.Individual;
            globalMap.DisplayNameType = StiDisplayNameType.None;
            globalMap.ShowLegend = false;
            globalMap.MapIdent = mapID;
            globalMap.ShowValue = false;
            globalMap.Stretch = true;

            return globalMap;
        }

        public static Color[] GetColors()
        {
            return new[]
            {
                ColorTranslator.FromHtml("#90c2a8"),
                ColorTranslator.FromHtml("#da5459"),
                ColorTranslator.FromHtml("#efca70"),
                ColorTranslator.FromHtml("#63b8e3"),
                ColorTranslator.FromHtml("#ab92c4"),
                ColorTranslator.FromHtml("#6d58d9"),
                ColorTranslator.FromHtml("#fb6b40"),
                ColorTranslator.FromHtml("#e34e2e"),
            };
        }

        public static string PrepareIsoCode(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            int index = text.IndexOf("-");
            if (index != -1)
                return text.Substring(index + 1);

            return text;
        }
        #endregion
    }
}