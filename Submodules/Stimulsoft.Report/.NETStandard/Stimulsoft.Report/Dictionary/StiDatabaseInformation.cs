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

using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report.Dictionary
{
    public class StiDatabaseInformation
    {
        #region Properties
        public List<DataRelation> Relations { get; set; } = new List<DataRelation>();

        public List<DataTable> Tables { get; } = new List<DataTable>();

        public List<DataTable> Views { get; } = new List<DataTable>();

        public List<DataTable> StoredProcedures { get; } = new List<DataTable>();
        #endregion

        public StiDatabaseInformation()
        {
        }

        public StiDatabaseInformation(List<DataTable> tables)
        {
            tables.ForEach(Tables.Add);
        }

        public StiDatabaseInformation(DataTableCollection tables) : this(tables.Cast<DataTable>().ToList())
        {
        }
    }
}
