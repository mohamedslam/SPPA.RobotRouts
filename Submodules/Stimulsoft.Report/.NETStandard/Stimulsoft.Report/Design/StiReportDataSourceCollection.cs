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
using System.Collections;
using System.Globalization;

namespace Stimulsoft.Report.Design
{	
	public class StiReportDataSourceCollection : CollectionBase
	{
		#region Methods
		public void Add(string name, object value)
		{
			Add(new StiReportDataSource(name, value));
		}
		
		public void Add(StiReportDataSource item)
		{
			this.List.Add(item);
		}
		
		public void Remove(string name)
		{
			foreach (StiReportDataSource dataSource in this.List)
			{
				if (dataSource.Name == name)
				{
					this.List.Remove(dataSource);
					break;
				}
			}
		}
		
		public StiReportDataSource this[string name]
		{ 
			get
			{
				name = name.ToLower(CultureInfo.InvariantCulture);
				foreach (StiReportDataSource dataSource in this.List)
				{
					if (dataSource.Name.ToLower(CultureInfo.InvariantCulture) == name)
					{
						return dataSource;
					}
				}
				return null;
			}
			set
			{
				name = name.ToLower(CultureInfo.InvariantCulture);
				for (int index = 0; index < List.Count; index++)				
				{
					StiReportDataSource dataSource = List[index] as StiReportDataSource;
					
					if (dataSource.Name.ToLower(CultureInfo.InvariantCulture) == name)
					{
						List[index] = value;
						return;
					}
				}
				Add(value);
			}
		}
		#endregion

		#region Properties
	    public StiReport Report { get; }
	    #endregion

		public StiReportDataSourceCollection(StiReport report)
		{
			this.Report = report;
		}
	}	
}
