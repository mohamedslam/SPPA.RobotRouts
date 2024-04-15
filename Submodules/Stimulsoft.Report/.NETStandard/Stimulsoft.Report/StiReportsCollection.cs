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

using System.Collections;

namespace Stimulsoft.Report
{
	/// <summary>
	/// Class describes a collection of reports. This collection used in SubReports property of StiReport.
	/// </summary>
	public class StiReportsCollection : CollectionBase
	{
		#region Collection
		public void Add(StiReport report)
		{
			Add(report, false, false);
		}

		public void Add(StiReport report, bool resetPageNumber, bool printOnPreviousPage)
		{
            if (report == owner) return;

			report.SubReportsPrintOnPreviousPage = printOnPreviousPage;
			report.SubReportsResetPageNumber = resetPageNumber;

            report.Unit = this.owner.Unit;
			if (report.CompiledReport != null) report.CompiledReport.Unit = this.owner.Unit;
			List.Add(report);
		}

		public void AddRange(StiReportsCollection reports)
		{
			foreach (StiReport report in reports)Add(report);
		}

		public void AddRange(StiReport[] reports)
		{
			foreach (StiReport report in reports)Add(report);
		}

		public bool Contains(StiReport report)
		{
			return List.Contains(report);
		}
		
		public int IndexOf(StiReport report)
		{
			return List.IndexOf(report);
		}

		public void Insert(int index, StiReport report)
		{
			List.Insert(index, report);
		}

		public void Remove(StiReport report)
		{
			List.Remove(report);
		}
		
		public StiReport this[int index]
		{
			get
			{
				return (StiReport)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		#endregion

        private StiReport owner;

		internal StiReportsCollection(StiReport owner)
		{
            this.owner = owner;
		}
	}
}