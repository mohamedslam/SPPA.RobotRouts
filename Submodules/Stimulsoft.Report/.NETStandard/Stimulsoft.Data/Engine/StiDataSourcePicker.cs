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

using Stimulsoft.Base;
using Stimulsoft.Data.Functions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public class StiDataSourcePicker
    {
        #region Methods
        public static IEnumerable<IStiAppDataSource> Fetch(IStiQueryObject query, string group, IEnumerable<string> dataNames, 
            IEnumerable<IStiAppDataSource> dataSources)
        {
            dataNames = dataNames ?? query.RetrieveUsedDataNames(group);
            dataNames = dataNames
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(Funcs.ToDataName);

            var allColumns = dataSources?.ToList().SelectMany(d => d.FetchColumns().Select(c => new
            {
                DataSource = d,
                Name = Funcs.ToDataName(c.GetName())
            })).ToList();

            return dataNames
                .SelectMany(dataName => allColumns
                    .Where(pair => Funcs.IsDataEqual(pair.DataSource, pair.Name, dataName))
                    .Select(c => c.DataSource))
                .Distinct()
                .ToList();
        }
        #endregion
    }
}