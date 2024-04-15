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

using Stimulsoft.Base.Gis;
using Stimulsoft.Map.Gis.Core;
using Stimulsoft.Map.Gis.Projections;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Stimulsoft.Map.Gis.Providers
{
    public abstract class StiGisMapProvider
    {
        static StiGisMapProvider()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public StiGisMapProvider()
        {
            var random = new Random();
            userAgent = string.Format("Mozilla/5.0 (Windows NT {1}.0; {2}rv:{0}.0) Gecko/20100101 Firefox/{0}.0",
                    random.Next(DateTime.Today.Year - 1969 - 5, DateTime.Today.Year - 1969),
                    random.Next(0, 10) % 2 == 0 ? 10 : 6,
                    random.Next(0, 10) % 2 == 1 ? string.Empty : "WOW64; ");
        }

        #region Fields
        private const string requestAccept = "*/*";
        private const string responseContentType = "image";
        internal string LanguageStr = "en";
        private readonly IWebProxy webProxy = new StiGisEmptyWebProxy();
        // Gets or sets the value of the User-agent HTTP header. It's pseudo-randomized to avoid blockages...
        private readonly string userAgent;

        public StiGisRectLatLng? Area;
        public static int TimeoutMs = 5 * 1000;
        public string RefererUrl = string.Empty;
        public string Copyright = string.Empty;
        private string Authorization = string.Empty;
        #endregion

        #region Properties.abstract
        public virtual int MaxZoom => 18;

        internal abstract StiGisProjection Projection { get; }

        public abstract StiGeoMapProviderType ProviderType { get; }
        #endregion

        #region Properties
        public int MinZoom => 1;

        public string Name => StiGisMapHelper.GetProviderName(this.ProviderType);

        public bool IsInitialized { get; internal set; }

        #endregion

        #region Methods.abstract
        public abstract StiGisMapImage GetTileImage(StiGisPoint pos, int zoom, StiGeoRenderMode mode);
        #endregion

        #region Methods.virtual
        public virtual void OnInitialized() { }

        protected virtual bool CheckTileImageHttpResponse(WebResponse response)
        {
            return response.ContentType.Contains(responseContentType);
        }
        #endregion

        #region Methods.override
        public override int GetHashCode() => (int)this.ProviderType;

        public override bool Equals(object obj)
        {
            var provider = obj as StiGisMapProvider;
            return (provider != null)
                ? this.ProviderType == provider.ProviderType
                : false;
        }

        public override string ToString() => Name;
        #endregion

        #region Methods
        public void ForceBasicHttpAuthentication(string userName, string userPassword)
        {
            Authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + userPassword));
        }

        protected StiGisMapImage GetTileImageUsingHttp(string url, StiGeoRenderMode mode)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = WebRequest.Create(url);
            if (webProxy != null)
            {
                request.Proxy = webProxy;
            }

            if (!string.IsNullOrEmpty(Authorization))
            {
                request.Headers.Set("Authorization", Authorization);
            }

            if (request is HttpWebRequest)
            {
                var r = request as HttpWebRequest;
                r.UserAgent = userAgent;
                r.ReadWriteTimeout = TimeoutMs * 6;
                r.Accept = requestAccept;
                r.Referer = RefererUrl;
                r.Timeout = TimeoutMs;
            }

            StiGisMapImage ret = null;
            using (var response = request.GetResponse())
            {
                if (CheckTileImageHttpResponse(response))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var data = StiGisCoreHelper.CopyStream(responseStream);
                        if (data.Length > 0)
                        {
                            ret = StiGisMapImage.FromByteArray(data, mode);
                            if (ret != null)
                                ret.Data = data;
                        }
                    }
                }

                response.Close();
            }

            return ret;
        }

        protected string GetContentUsingHttp(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = WebRequest.Create(url);

            if (webProxy != null)
            {
                request.Proxy = webProxy;
            }

            if (!string.IsNullOrEmpty(Authorization))
            {
                request.Headers.Set("Authorization", Authorization);
            }

            if (request is HttpWebRequest)
            {
                var r = request as HttpWebRequest;
                r.UserAgent = userAgent;
                r.ReadWriteTimeout = TimeoutMs * 6;
                r.Accept = requestAccept;
                r.Referer = RefererUrl;
                r.Timeout = TimeoutMs;
            }

            var ret = string.Empty;
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                using (var read = new StreamReader(responseStream, Encoding.UTF8))
                {
                    ret = read.ReadToEnd();
                }

                response.Close();
            }

            return ret;
        }

        protected static int GetServerNum(StiGisPoint pos, int max)
        {
            return (int)(pos.X + 2 * pos.Y) % max;
        }
        #endregion
    }
}