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

using Stimulsoft.Base.Json;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Stimulsoft.Base
{
    public static class StiExceptionProviderHelper
    {
        #region classes
        private sealed class ReportErrorCommand
        {
            #region Properties
            public string Ident { get; } = "SystemReportError";
            public string UserName { get; set; }
            public string ErrorSubject { get; set; }
            public string ErrorBody { get; set; }
            public string ErrorNumber { get; set; }
            public bool ResultSuccess { get; set; }
            #endregion

            #region Methods
            public byte[] SaveToString()
            {
                return StiGZipHelper.ConvertStringToByteArray(JsonConvert.SerializeObject(this, Formatting.Indented, StiJsonHelper.DefaultSerializerSettings));
            }

            public void Load(string str)
            {
                JsonConvert.PopulateObject(str, this);
            }
            #endregion
        }
        #endregion

        #region Methods.Send
        public static Task<bool> SendAsync(string userName, string errorSubject, string errorBody, string errorNumber)
        {
#if NET45
            return Task.Run(() => Send(userName, errorSubject, errorBody, errorNumber));
#else
            return Task.Factory.StartNew(() => Send(userName, errorSubject, errorBody, errorNumber));
#endif
        }

        private static bool Send(string userName, string errorSubject, string errorBody, string errorNumber)
        {
            try
            {
                var url = string.IsNullOrEmpty(StiExceptionProvider.ServerUrl)
                    ? "https://reports.stimulsoft.com/1/runcommand"
                    : StiExceptionProvider.ServerUrl;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var request = WebRequest.Create(url);
                request.Proxy = StiProxy.GetProxy();
                request.Method = "POST";

                var command = new ReportErrorCommand
                {
                    UserName = userName,
                    ErrorSubject = errorSubject,
                    ErrorBody = errorBody.Replace("\r\n", "<br>"),
                    ErrorNumber = errorNumber
                };

                var commandBytes = command.SaveToString();

                request.ContentLength = commandBytes.Length;
                request.ContentType = "application/json";
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(commandBytes, 0, commandBytes.Length);
                }

                request.Timeout = 30000;
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var output = stream.ReadToEnd();

                        var resCommand = new ReportErrorCommand();
                        resCommand.Load(output);

                        return resCommand.ResultSuccess;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}