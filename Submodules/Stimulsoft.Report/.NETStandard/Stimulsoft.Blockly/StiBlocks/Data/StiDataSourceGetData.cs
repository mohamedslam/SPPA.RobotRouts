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

using Stimulsoft.Blockly.Blocks;
using Stimulsoft.Blockly.Model;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Dictionary.Databases.Azure;
using Stimulsoft.Report.Engine;
using System;
using System.Data;
using System.Linq;

namespace Stimulsoft.Blockly.StiBlocks.Data
{
    internal class StiDataSourceGetData : IronBlock
    {
        #region Methods
        public override object Evaluate(Context context)
        {
            var obj = this.Values.Evaluate("DATA", context) as StiDataSource;
            if (obj != null)
            {
                if (obj.IsConnected == false)
                {
                    var databaseName = obj.GetNameInSource().ToLowerInvariant();
                    databaseName = databaseName.Substring(0, databaseName.IndexOf("."));
                    var connection = obj.Dictionary.Databases.Cast<StiDatabase>().FirstOrDefault(d => d.Name.ToLowerInvariant() == databaseName) ;
                    if (connection != null) StiDataLeader.RegData(connection, obj.Dictionary, true);
                }


                var columnName = this.Values.Evaluate("COLUMN", context).ToString();
                var rowIndex = this.Values.Evaluate("ROW", context).ToString();
                var index = 0;

                if (int.TryParse(rowIndex, out index))
                {
                    var method = obj.GetType().GetMethod("GetData", new Type[] { columnName.GetType(), index.GetType() });
                    return method.Invoke(obj, new object[] { columnName, index });
                }
            }

            return base.Evaluate(context);
        }
        #endregion
    }
}
