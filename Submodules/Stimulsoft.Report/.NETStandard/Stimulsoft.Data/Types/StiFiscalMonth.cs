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

using Stimulsoft.Data.Comparers;
using Stimulsoft.Data.Functions;
using System;

namespace Stimulsoft.Data.Types
{
    public class StiFiscalMonth : IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            return StiObjectComparer.Compare(this, obj);
        }
        #endregion

        #region Properties
        public StiMonth Month { get; }

        public StiMonth StartMonth { get; }

        public int ActualMonthIndex => Month < StartMonth ? (int)Month + 12 : (int)Month;
        #endregion

        #region Methods
        public override string ToString()
        {
            return Month.ToString();
        }

        public override int GetHashCode()
        {
            return Month.GetHashCode() + StartMonth.GetHashCode();
        }
        #endregion

        public StiFiscalMonth(StiMonth month, StiMonth startMonth)
        {
            Month = month;
            StartMonth = startMonth;
        }
    }
}