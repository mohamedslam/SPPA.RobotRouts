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

using System.Collections;

namespace Stimulsoft.Report.Web
{
    public class StiEmailOptions
    {
        /// <summary>
        /// A System.String that contains the addresses of the recipients of the Email message.
        /// </summary>
        public string AddressFrom { get; set; } = string.Empty;

        /// <summary>
        /// A System.String that contains the address of the sender of the Email message.
        /// </summary>
        public string AddressTo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the subject line for the Email message.
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the replyTo for the Email message.
        /// </summary>
        public string ReplyTo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// A System.String that contains the name or IP address of the host used for SMTP transactions.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// An System.Int32 greater than zero that contains the port to be used on host.
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// The user name associated with the credentials.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// The password for the user name associated with the credentials.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Specify whether the System.Net.Mail.SmtpClient uses Secure Sockets Layer (SSL) to encrypt the connection.
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Sets the address collection that contains the carbon copy (CC) recipients for the Email message.
        /// </summary>
        public ArrayList CC { get; set; } = new ArrayList();

        /// <summary>
        /// Sets the address collection that contains the blind carbon copy (BCC) recipients for the Email message.
        /// </summary>
        public ArrayList BCC { get; set; } = new ArrayList();
    }
}
