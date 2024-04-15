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
    public delegate void StiSaveDocumentEventHandler(object sender, StiSaveDocumentEventArgs e);

    /// <summary>
    /// Describes an argument for the event StiSaveDocumentEvent.
    /// </summary>
    public class StiSaveDocumentEventArgs : EventArgs
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }

        public StiReport Report { get; }

        public StiSaveDocumentEventArgs(StiReport report, string filePath, string fileName)
        {
            this.FilePath = filePath;
            this.FileName = fileName;
            this.Report = report;
        }
    }
}