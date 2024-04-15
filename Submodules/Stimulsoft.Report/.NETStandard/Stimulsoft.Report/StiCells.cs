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

namespace Stimulsoft.Report
{
    public class StiCells
    {
        #region Methods
        public void Clear()
        {
            rows.Clear();
        }

        private Hashtable GetRow(int y)
        {
            var row = rows[y] as Hashtable;
            if (row == null)
            {
                row = new Hashtable();
                rows[y] = row;
            }
            return row;
        }
        
        public decimal this[int x, int y]
        {
            get
            {
                var row = GetRow(y + DistY);

                if (row.ContainsKey(x + DistX))
                    return (decimal)row[x + DistX];
                else
                    return 0m;                
            }
        }

        public void SetCell(int x, int y, decimal value)
        {
            var row = GetRow(y);
            row[x] = value;
        }
        #endregion

        #region Fields
        private Hashtable rows = new Hashtable();
        #endregion

        #region Properties
        public int DistX { get; set; }

        public int DistY { get; set; }
        #endregion
    }
}
