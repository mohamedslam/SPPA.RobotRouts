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

using System.Collections.Generic;
using System.Data;

namespace Stimulsoft.Data.Comparers
{
    public class StiDataRowComparer : IComparer<DataRow>
    {
        #region Methods
        public int Compare(DataRow x, DataRow y)
        {
            if (x == null || y == null)
                return 0;

            if (x.ItemArray.Length != y.ItemArray.Length)
                return -1;

            for (var index = 0; index < x.ItemArray.Length; index++)
            {
                var result = StiObjectComparer.Compare(x, y);
                if (result != 0)
                    return result;
            }
            return 0;
        }
        #endregion
    }
}