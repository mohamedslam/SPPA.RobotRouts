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
using System.Collections.Generic;

namespace Stimulsoft.Report.Chart
{
    internal class StiDataItemComparer : IComparer<StiDataItem>
    {
        #region IComparer
        public int Compare(StiDataItem x, StiDataItem y)
        {
            try
            {
                IComparable value1;
                IComparable value2;

                if (sortType == StiSeriesSortType.Value)
                {
                    value1 = x.Value as IComparable;
                    value2 = y.Value as IComparable;
                }
                else
                {
                    value1 = x.Argument as IComparable;
                    value2 = y.Argument as IComparable;
                }

                if (value1 == null && value2 == null)
                    return x.Index.CompareTo(y.Index);

                if (value1 == null)
                    return 1;

                if (value2 == null)
                    return -1;

                var value = value1.CompareTo(value2);

                if (value == 0)
                    value = x.Index.CompareTo(y.Index);

                return value * directionFactor;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Fields
        private int directionFactor;
        private StiSeriesSortType sortType;
        #endregion

        internal StiDataItemComparer(StiSeriesSortType sortType, StiSeriesSortDirection sortDirection)
        {
            directionFactor = 1;
            if (sortDirection == StiSeriesSortDirection.Descending)directionFactor = -1;

            this.sortType = sortType;
        }
    }
}