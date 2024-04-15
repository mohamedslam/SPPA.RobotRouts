#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Stimulsoft.System.Web;
using Stimulsoft.Report.Web;
using System.Net.Http;
using System.Net;
using System.ComponentModel;

namespace Stimulsoft.Report.Mvc
{
    public class StiNetCoreActionResult : ActionResult
    {
        #region Properties
        public byte[] Data { get; private set; }

        public string ContentType { get; private set; }

        public bool EnableBrowserCache { get; private set; }

        public string FileName { get; private set; }

        public bool ShowSaveFileDialog { get; private set; }

        [Obsolete("This property is obsolete, pleasu use the ShowSaveFileDialog property instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SaveFileDialog
        {
            get
            {
                return ShowSaveFileDialog;
            }
            private set
            {
                ShowSaveFileDialog = value;
            }
        }
        #endregion

        #region Methods
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (Data != null && Data.Length > 0)
            {
                var response = new HttpResponseBase(context.HttpContext.Response);
                
                if (!string.IsNullOrEmpty(FileName))
                {
                    var type = ShowSaveFileDialog ? "attachment" : "inline";
                    var value = string.Format("{0};filename=\"{1}\";filename*=UTF-8''{2}", type, FileName, HttpUtility.UrlEncode(FileName).Replace("+", "%20"));
                    response.AppendHeader("Content-Disposition", value);
                }

                if (EnableBrowserCache)
                {
                    response.Cache.SetExpires(DateTime.Now.AddYears(1));
                    response.Cache.SetCacheability(HttpCacheability.Public);
                }
                else
                {
                    response.Cache.SetExpires(DateTime.MinValue);
                    response.Cache.SetCacheability(HttpCacheability.NoCache);
                }

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = ContentType != null ? ContentType + "; charset=utf-8" : ContentType;
                response.AddHeader("Content-Length", Data.Length.ToString());
                response.BinaryWrite(Data);
            }
        }

        public HttpResponseMessage ToHttpResponseMessage()
        {
            var httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new ByteArrayContent(Data);

            if (!string.IsNullOrEmpty(FileName))
            {
                var type = ShowSaveFileDialog ? "attachment" : "inline";
                var value = string.Format("{0};filename=\"{1}\";filename*=UTF-8''{2}", type, FileName, HttpUtility.UrlEncode(FileName).Replace("+", "%20"));
                httpResponseMessage.Content.Headers.Add("Content-Disposition", value);
            }

            httpResponseMessage.Headers.Add("Cache-Control", EnableBrowserCache ? "public, max-age=31536000" : "no-cache, no-store, max-age=0, must-revalidate");
            httpResponseMessage.Content.Headers.Add("Content-Type", ContentType + "; charset=utf-8");
            httpResponseMessage.Content.Headers.Add("Content-Length", Data.Length.ToString());

            return httpResponseMessage;
        }
        #endregion

        #region Static
        public static StiNetCoreActionResult EmptyResult()
        {
            return new StiNetCoreActionResult();
        }

        public static StiNetCoreActionResult FromWebActionResult(StiWebActionResult result)
        {
            if (result == null)
                return EmptyResult();

            return new StiNetCoreActionResult(result.Data, result.ContentType, result.FileName, result.ShowSaveFileDialog, false);
        }

        public static HttpResponseMessage ToHttpResponseMessage(ActionResult result)
        {
            var mvcActionResult = result as StiNetCoreActionResult;
            if (mvcActionResult != null)
                return mvcActionResult.ToHttpResponseMessage();

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion

        #region Constructors
        public StiNetCoreActionResult() :
            this(string.Empty)
        {
        }

        public StiNetCoreActionResult(string data, string contentType = "text/plain", string fileName = null, bool showSaveFileDialog = true, bool enableBrowserCache = false) :
            this(Encoding.UTF8.GetBytes(data), contentType, fileName, showSaveFileDialog, enableBrowserCache)
        {
        }

        public StiNetCoreActionResult(Stream data, string contentType = "text/plain", string fileName = null, bool showSaveFileDialog = true, bool enableBrowserCache = false) :
            this(((MemoryStream)data).ToArray(), contentType, fileName, showSaveFileDialog, enableBrowserCache)
        {
        }

        public StiNetCoreActionResult(byte[] data, string contentType = "text/plain", string fileName = null, bool showSaveFileDialog = true, bool enableBrowserCache = false)
        {
            this.Data = data;
            this.ContentType = contentType;
            this.FileName = fileName;
            this.ShowSaveFileDialog = !string.IsNullOrEmpty(fileName) && showSaveFileDialog;
            this.EnableBrowserCache = enableBrowserCache;
        }
        #endregion
    }
}
