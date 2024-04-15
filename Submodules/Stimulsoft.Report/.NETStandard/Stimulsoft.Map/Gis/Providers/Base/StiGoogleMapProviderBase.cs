#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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

using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Projections;
using Stimulsoft.Map.Gis.Providers.Providers;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiGoogleMapProviderBase : 
        StiGisMapProvider
    {
        public StiGoogleMapProviderBase()
        {
            RefererUrl = string.Format("https://maps.{0}/", Server);
            Copyright = string.Format("©{0} Google - Map data ©{0} Tele Atlas, Imagery ©{0} TerraMetrics", DateTime.Today.Year);
        }

        #region consts
        private const string ServerAPIs = "googleapis.com";
        internal const string Server = "google.com";
        internal const string ServerChina = "google.cn";
        private const string SecureWord = "Galileo";
        #endregion

        #region Fields
        private static bool init = false;
        private static readonly string Sec1 = "&s=";
        #endregion

        #region Properties
        public string ClientId { get; private set; } = string.Empty;
        #endregion

        #region Properties.override
        internal override StiGisProjection Projection => StiMercatorGisProjection.Instance;
        #endregion

        #region Methods.override
        public override void OnInitialized()
        {
            if (!init)
            {
                var url = string.Format("https://maps.{0}/maps/api/js?client=google-maps-lite&amp;libraries=search&amp;language=en&amp;region=", ServerAPIs);

                try
                {
                    var html = GetContentUsingHttp(url);
                    if (!string.IsNullOrEmpty(html))
                    {
                        #region match versions
                        var reg = new Regex(string.Format(@"https?://mts?\d.{0}/maps/vt\?lyrs=m@(\d*)", Server), RegexOptions.IgnoreCase);
                        var mat = reg.Match(html);
                        if (mat.Success)
                        {
                            var gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 0)
                            {
                                var ver = string.Format("m@{0}", gc[1].Value);
                                var old = StiGisGoogleProviderHelper.GoogleMapVersion;

                                StiGisGoogleProviderHelper.GoogleMapVersion = ver;
                                StiGisGoogleProviderHelper.GoogleChinaMapVersion = ver;
                            }
                        }
                        
                        reg = new Regex(string.Format(@"https?://khms?\d.{0}/kh\?v=(\d*)", Server), RegexOptions.IgnoreCase);
                        mat = reg.Match(html);
                        if (mat.Success)
                        {
                            var gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 0)
                            {
                                var ver = gc[1].Value;
                                var old = StiGisGoogleProviderHelper.GoogleSatelliteMapVersion;

                                StiGisGoogleProviderHelper.GoogleSatelliteMapVersion = ver;
                                StiGisGoogleProviderHelper.GoogleChinaSatelliteMapVersion = "s@" + ver;
                            }
                        }
                        
                        reg = new Regex(string.Format(@"https?://mts?\d.{0}/maps/vt\?lyrs=t@(\d*),r@(\d*)", Server), RegexOptions.IgnoreCase);
                        mat = reg.Match(html);
                        if (mat.Success)
                        {
                            var gc = mat.Groups;
                            int count = gc.Count;
                            if (count > 1)
                            {
                                var ver = string.Format("t@{0},r@{1}", gc[1].Value, gc[2].Value);
                                var old = StiGisGoogleProviderHelper.GoogleTerrainMapVersion;

                                StiGisGoogleProviderHelper.GoogleTerrainMapVersion = ver;
                                StiGisGoogleProviderHelper.GoogleChinaTerrainMapVersion = ver;
                            }
                        }
                        #endregion
                    }

                    init = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TryCorrectGoogleVersions failed: " + ex.ToString());
                }
            }
        }
        #endregion

        #region Methods
        internal void GetSecureWords(StiGisPoint pos, out string sec1, out string sec2)
        {
            sec1 = string.Empty; // after &x=...

            int seclen = (int)((pos.X * 3) + pos.Y) % 8;
            sec2 = SecureWord.Substring(0, seclen);
            if (pos.Y >= 10000 && pos.Y < 100000)
            {
                sec1 = Sec1;
            }
        }
        #endregion
    }
}