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

using Stimulsoft.Base.Json;
using System;
using System.IO;

namespace Stimulsoft.Base.TaskScheduler
{
    public sealed class StiSmtpConfig
    {
        private StiSmtpConfig()
        {

        }

        #region Fields
        private static StiSmtpConfig defaultConfig;
        #endregion

        #region Properties
        /// <summary>
        /// The Email of the Server Email agent.
        /// </summary>
        public string SenderEmail { get; set; }

        public string Host { get; set; }

        /// <summary>
        /// Port used for SMTP transactions.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The user name associated with the credentials.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password for the user name associated with the credentials.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Encrypt or not the connections to the SMTP server with help of the Secure Sockets Layer protocol.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Value that specifies the amount of time after which a synchronous System.Net.Mail.SmtpClient.SendEmail call times out.
        /// </summary>
        public int Timeout = 100000;
        #endregion

        #region Methods
        public static StiSmtpConfig Get()
        {
            if (defaultConfig == null)
                defaultConfig = TryLoad();

            return defaultConfig;
        }

        internal static void Clear()
        {
            defaultConfig = null;
        }

        public static StiSmtpConfig GetEmpty()
        {
            defaultConfig = null;
            return new StiSmtpConfig();
        }

        internal static string GetSettingsPath()
        {
            string path = $"Stimulsoft{Path.DirectorySeparatorChar}Scheduler{Path.DirectorySeparatorChar}Settings{Path.DirectorySeparatorChar}SmtpConfig.json";
            string folder;

            try
            {
                if (StiBaseOptions.FullTrust)
                {
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (folder.Length != 0)
                        path = Path.Combine(folder, path);
                }

                folder = Path.GetDirectoryName(path);

                if (StiBaseOptions.FullTrust && !string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch
            {
                try
                {
                    path = $"Stimulsoft{Path.DirectorySeparatorChar}Scheduler{Path.DirectorySeparatorChar}Settings{Path.DirectorySeparatorChar}SmtpConfig.json";
                    folder = $"Stimulsoft{Path.DirectorySeparatorChar}Scheduler{Path.DirectorySeparatorChar}Settings";

                    if (StiBaseOptions.FullTrust && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch
                {
                    path = $"SmtpConfig.json";
                }
            }

            return path;
        }

        public void TrySave()
        {
            try
            {
                var path = GetSettingsPath();
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);

                StiFileUtils.ProcessReadOnly(path);
                File.WriteAllText(path, json);
            }
            catch
            {

            }
        }

        private static StiSmtpConfig TryLoad()
        {
            try
            {
                var path = GetSettingsPath();
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<StiSmtpConfig>(text);
                }
            }
            catch
            {
            }

            return new StiSmtpConfig();
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(SenderEmail))
                return false;

            if (string.IsNullOrWhiteSpace(Host))
                return false;

            if (Port == 0)
                return false;

            return true;
        }

        #endregion
    }
}