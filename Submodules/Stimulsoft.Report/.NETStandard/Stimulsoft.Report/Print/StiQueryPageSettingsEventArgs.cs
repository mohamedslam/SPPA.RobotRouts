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
using System.Drawing.Printing;

namespace Stimulsoft.Report.Print
{
	public delegate void StiQueryPageSettingsEventHandler(object sender, StiQueryPageSettingsEventArgs e);

	public class StiQueryPageSettingsEventArgs : EventArgs
	{
	    public object PaperSize { get; set; }

	    public PaperSource PaperSource { get; set; }

	    public PageSettings PageSettings { get; set; }

	    internal StiQueryPageSettingsEventArgs(
			object paperSize, PaperSource paperSource, PageSettings pageSettings)
		{
			this.PaperSize = paperSize;
			this.PaperSource = paperSource;
			this.PageSettings = pageSettings;
		}
	}
}
