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

namespace Stimulsoft.Report.Dictionary
{
	public class StiRowsCollection : IEnumerable, IEnumerator
	{
		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
		#endregion

		#region IEnumerator
		object IEnumerator.Current
		{
			get
			{
				return new StiRow(dataSource, dataSource.Position);
			}
		}
		
		bool IEnumerator.MoveNext()
		{
			dataSource.Next();
			return !dataSource.IsEof;
		}

		void IEnumerator.Reset()
		{
			dataSource.First();
		}
        #endregion

        #region Properties
        public StiRow this[int rowIndex] => new StiRow(dataSource, rowIndex);
        #endregion

        #region Fields
        public int Count => dataSource.Count;

	    private StiDataSource dataSource;
        #endregion

        public StiRowsCollection(StiDataSource dataSource)
		{
			this.dataSource = dataSource;
		}
	}
}
