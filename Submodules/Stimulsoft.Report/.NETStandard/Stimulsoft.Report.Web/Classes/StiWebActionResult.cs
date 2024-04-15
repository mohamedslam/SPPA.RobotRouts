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

using Stimulsoft.Base;
using Stimulsoft.Base.Json;
using Stimulsoft.Base.Json.Converters;
using Stimulsoft.Base.Localization;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stimulsoft.Report.Web
{
    public class StiWebActionResult
    {
        #region Properties 
        public byte[] Data { get; set; }
        
        public string ContentType { get; set; }
        
        public string FileName { get; set; }

        public bool ShowSaveFileDialog { get; set; }
        #endregion

        #region Methods.Results
        private static StiWebActionResult CompressedResult(string data)
        {
            var base64 = StiGZipHelper.Pack(data);
            return new StiWebActionResult(base64);
        }

        public static StiWebActionResult StringResult(StiRequestParams requestParams, string data)
        {
            if (requestParams.Server.UseCompression)
                return CompressedResult(data);

            return new StiWebActionResult(data);
        }

        public static StiWebActionResult JsonResult(StiRequestParams requestParams, Hashtable data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None, new StringEnumConverter());

            if (requestParams.Server.UseCompression)
                return CompressedResult(json);

            return new StiWebActionResult(json, "application/json");
        }

        public static StiWebActionResult DashboardNotSupportedResult(StiRequestParams requestParams)
        {
            var messageText = string.Format("{0}<br><br>{1}", StiLocalization.Get("Dashboard", "CannotLoadDashboard"), "You should use the Stimulsoft Dashboards.WEB product.");

            if (requestParams.ComponentType == StiComponentType.Designer)
            {
                var callbackResult = new Hashtable();
                callbackResult["infoMessage"] = messageText;

                return JsonResult(requestParams, callbackResult);
            }
            else
            {
                return StiWebActionResult.ErrorResult(requestParams, messageText);
            }
        }

        public static StiWebActionResult ErrorResult(StiRequestParams requestParams, string data)
        {
            if (requestParams.ComponentType == StiComponentType.Designer)
            {
                var result = new Hashtable();
                result["error"] = data;

                // If an error occurs, clear file name for the new report
                if (requestParams.Designer.Command == StiDesignerCommand.SaveReport && requestParams.Designer.IsNewReport)
                    result["isNewReport"] = true;

                return JsonResult(requestParams, result);
            }

            return StringResult(requestParams, "ServerError:" + data);
        }

        public static StiWebActionResult EmptyResult()
        {
            return new StiWebActionResult();
        }

        public static async Task<StiWebActionResult> EmptyResultAsync()
        {
            return await Task.Run(() => EmptyResult());
        }

        public static StiWebActionResult EmptyJsonResult()
        {
            return new StiWebActionResult("{}", "application/json");
        }

        public static StiWebActionResult EmptyReportResult()
        {
            return new StiWebActionResult("ServerError:The report is not specified.");
        }

        public static string GetErrorMessageText(Exception e)
        {
            if (e == null) return string.Empty;

            return (!string.IsNullOrEmpty(e.Message) ? e.Message : string.Empty) +
                (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message) ? $" {e.InnerException.Message}" : string.Empty);
        }
        #endregion

        public StiWebActionResult() :
            this(string.Empty)
        {
        }

        public StiWebActionResult(string data) :
            this(data, "text/plain")
        {
        }

        public StiWebActionResult(string data, string contentType, string fileName = null, bool showSaveFileDialog = true) :
            this(Encoding.UTF8.GetBytes(data), contentType, fileName, showSaveFileDialog)
        {
        }

        public StiWebActionResult(Stream data, string contentType, string fileName = null, bool showSaveFileDialog = true) :
            this(((MemoryStream)data).ToArray(), contentType, fileName, showSaveFileDialog)
        {
        }

        public StiWebActionResult(byte[] data, string contentType, string fileName = null, bool showSaveFileDialog = true)
        {
            Data = data;
            ContentType = contentType;
            FileName = fileName;
            ShowSaveFileDialog = !string.IsNullOrEmpty(fileName) && showSaveFileDialog;
        }
    }
}
