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

using System.IO;
using Stimulsoft.Report.Export;

namespace Stimulsoft.Report.Events
{
	/// <summary>
	/// Represents the method that handles the ProcessExport event.
	/// </summary>
	public delegate void StiProcessExportEventHandler(object sender, StiProcessExportEventArgs e);

	/// <summary>
	/// Describes an argument for the event ProcessExport.
	/// </summary>
    public class StiProcessExportEventArgs : StiExportEventArgs
	{
	    public virtual StiExportService ExportService { get; set; }

	    public virtual Stream Stream { get; set; }

	    public virtual StiExportSettings ExportSettings { get; set; }

	    public virtual bool Processed { get; set; }

	    public StiProcessExportEventArgs(StiExportFormat format, StiExportService exportService, Stream stream, StiExportSettings settings): base(format)
		{
            this.ExportService = exportService;
            this.Stream = stream;
            this.ExportSettings = settings;
		}
	}
}
