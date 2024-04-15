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

using Stimulsoft.Base.Meters;
using System.Collections.Generic;

namespace Stimulsoft.Data.Engine
{
    public class StiDataTable
    {
        #region Properties.Static
        public static StiDataTable NullTable = new StiDataTable();
        #endregion

        #region Properties
        public List<IStiMeter> Meters { get; set; }

        public List<object[]> Rows { get; set; }

        public bool IsNull => this == NullTable;

        public bool IsEmpty => IsNull || Meters == null || Meters.Count == 0 || Rows == null || Rows.Count == 0;
        #endregion

        public StiDataTable() 
            : this(new List<IStiMeter>(), new List<object[]>())
        {
        }

        public StiDataTable(List<IStiMeter> meters, List<object[]> rows)
        {
            this.Meters = meters;
            this.Rows = rows;
        }
    }
}