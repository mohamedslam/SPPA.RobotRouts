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
using System.Net;

namespace Stimulsoft.Base
{
    public static class StiProxy
    {
        #region class Settings
        public class Settings : StiObject
        {
            public string Address { get; set; }

            public int Port { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public Settings()
            {
            }

            public Settings(string address, int port, string userName, string password)
            {
                this.Address = address;
                this.Port = port;
                this.UserName = userName;
                this.Password = password;
            }
        }
        #endregion

        #region Methods.Static
        public static IWebProxy GetProxy()
        {
            try
            {
                var settings = LoadProxySettings();
                if (settings == null || string.IsNullOrWhiteSpace(settings.Address))
                    return WebRequest.GetSystemWebProxy();

                var proxy = new WebProxy(settings.Address, settings.Port);
                proxy.Credentials = new NetworkCredential(settings.UserName, settings.Password);
                return proxy;
            }
            catch
            {
                return WebRequest.GetSystemWebProxy();
            }
        }

        private static string GetProxySettingsPath()
        {
            string directory = GetStimulsoftFolder();
            return Path.Combine(directory, "Proxy.json");
        }

        private static string GetStimulsoftFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Stimulsoft");
        }

        public static Settings LoadProxySettings()
        {
            var file = GetProxySettingsPath();
            if (!File.Exists(file))
                return null;

            var settings = new Settings();
            settings.LoadFromString(File.ReadAllText(file));
            return settings;
        }

        public static void SaveProxySettings(Settings settings)
        {
            var file = GetProxySettingsPath();
            var directory = GetStimulsoftFolder();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(file, settings.SaveToString());
        }
        #endregion
    }
}
