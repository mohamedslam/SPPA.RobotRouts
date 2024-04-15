#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	Stimulsoft.Report Library										}
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
using Stimulsoft.Base.Services;

namespace Stimulsoft.Report.SaveLoad
{
	/// <summary>
	/// Describes the abstract class that allows to save / load reports.
	/// </summary>
	[StiServiceBitmap(typeof(StiSLService), "Stimulsoft.Report.Bmp.SL.SLReport.bmp")]
	public abstract class StiReportSLService : StiSLService
	{
		/// <summary>
		/// Saves report in the stream.
		/// </summary>
		/// <param name="report">Report for saving.</param>
		/// <param name="stream">Stream to save report.</param>
		public abstract void Save(StiReport report, Stream stream);

		/// <summary>
		/// Loads a report from the stream.
		/// </summary>
		/// <param name="report">The report in which loading will be done.</param>
		/// <param name="stream">Stream to save report.</param>
		public abstract void Load(StiReport report, Stream stream);
	}
}
