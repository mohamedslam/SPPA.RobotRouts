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

using Stimulsoft.Map.Gis.Projections;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiBingMapProviderBase : 
        StiGisMapProvider
    {
        public StiBingMapProviderBase()
        {
            RefererUrl = "http://www.bing.com/maps/";
            Copyright = $"©{DateTime.Today.Year} Microsoft Corporation, ©{DateTime.Today.Year} NAVTEQ, ©{DateTime.Today.Year} Image courtesy of NASA";
        }

        #region Fields
        private string sessionId = string.Empty;
        private static bool init;
        #endregion

        #region Properties
        public string Version => "4810";

        // http://msdn.microsoft.com/en-us/library/ff428642.aspx
        private string ClientKey => "AjGy60ciMrcB7Acfl0kqPEAS2zNzuISiRVr7GUmKCFUTELF9fIj7tGshe6oJVmbS";
        #endregion

        #region Properties.override
        internal sealed override StiGisProjection Projection => StiMercatorGisProjection.Instance;
        #endregion

        #region Methods.override
        public override void OnInitialized()
        {
            if (!init)
            {
                try
                {
                    var key = ClientKey;

                    // to avoid registration stuff, default key
                    if (string.IsNullOrEmpty(key))
                        throw new NotSupportedException();

                    #region try get sesion key --
                    var keyResponse = GetContentUsingHttp(string.Format("http://dev.virtualearth.net/webservices/v1/LoggingService/LoggingService.svc/Log?entry=0&fmt=1&type=3&group=MapControl&name=AJAX&mkt=en-us&auth={0}&jsonp=microsoftMapsNetworkCallback", key));
                    if (!string.IsNullOrEmpty(keyResponse) && keyResponse.Contains("sessionId") && keyResponse.Contains("ValidCredentials"))
                    {
                        sessionId = keyResponse.Split(',')[0].Split(':')[1].Replace("\"", string.Empty).Replace(" ", string.Empty);
                    }
                    #endregion

                    init = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TryCorrectBingVersions failed: {ex.Message}");
                }
            }
        }

        protected override bool CheckTileImageHttpResponse(WebResponse response)
        {
            var pass = base.CheckTileImageHttpResponse(response);
            if (pass)
            {
                var tileInfo = response.Headers.Get("X-VE-Tile-Info");
                if (tileInfo != null)
                    return !tileInfo.Equals("no-tile");
            }

            return pass;
        }
        #endregion
        
        #region Methods
        internal string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            var quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                    digit++;

                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }

        internal string GetTileUrl(string imageryType)
        {
            //Retrieve map tile URL from the Imagery Metadata service: http://msdn.microsoft.com/en-us/library/ff701716.aspx
            //This ensures that the current tile URL is always used. 
            //This will prevent the app from breaking when the map tiles change.

            var ret = string.Empty;
            if (!string.IsNullOrEmpty(sessionId))
            {
                try
                {
                    string url = "http://dev.virtualearth.net/REST/V1/Imagery/Metadata/" + imageryType + "?output=xml&key=" + sessionId;
                    var r = GetContentUsingHttp(url);

                    if (!string.IsNullOrEmpty(r))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(r);

                        var xn = doc["Response"];
                        var statuscode = xn["StatusCode"].InnerText;
                        if (string.Compare(statuscode, "200", true) == 0)
                        {
                            xn = xn["ResourceSets"]["ResourceSet"]["Resources"];
                            var xnl = xn.ChildNodes;
                            foreach (XmlNode xno in xnl)
                            {
                                var imageUrl = xno["ImageUrl"];

                                if (imageUrl != null && !string.IsNullOrEmpty(imageUrl.InnerText))
                                {
                                    var baseTileUrl = imageUrl.InnerText;

                                    if (baseTileUrl.Contains("{key}") || baseTileUrl.Contains("{token}"))
                                    {
                                        baseTileUrl.Replace("{key}", sessionId).Replace("{token}", sessionId);
                                    }

                                    ret = baseTileUrl;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetTileUrl: Error getting Bing Maps tile URL - " + ex);
                }
            }

            return ret;
        }
        #endregion
    }
}