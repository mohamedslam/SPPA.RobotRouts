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

using System;

namespace Stimulsoft.Report.Events
{
    /// <summary>
    /// Represents the method that handles the AllowClipboardOperation event.
    /// </summary>
    public delegate void StiSendEMailEventHandler(object sender, StiSendEMailEventArgs e);

    /// <summary>
    /// Describes an argument for the event StiSendEMailEvent.
    /// </summary>
    public class StiSendEMailEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a recipient of sending EMail.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Gets or sets a subject of sending EMail.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets a body of sending EMail.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets a file path of sending EMail.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a report of sending EMail.
        /// </summary>
        public StiReport Report { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to call standard handler.
        /// </summary>
        public bool CallStandardHandler { get; set; } = true;

        public StiSendEMailEventArgs(StiReport report, string recipient, string subject, string body, string filePath)
        {
            this.Report = report;
            this.Recipient = recipient;
            this.Subject = subject;
            this.Body = body;
            this.FilePath = filePath;
        }
    }
}