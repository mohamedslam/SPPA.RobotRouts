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
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dictionary
{
    public class StiDataPathFinder
    {
        #region Methods
        public static StiDataColumn GetColumnFromPath(string path, StiDictionary dictionary)
        {
            var strs = path.Split('.');

            var dataSourceStr = strs[0];
            foreach (StiDataSource dataSource in dictionary.DataSources)
            {
                if (StiNameValidator.CorrectName(dataSource.Name, dictionary.Report) == dataSourceStr && path.Length > dataSource.Name.Length + 1)
                {
                    var newPath = path.Substring(dataSource.Name.Length + 1);
                    return GetColumnFromPath(newPath, dataSource);
                }
            }
            return null;
        }

        public static StiDataColumn GetColumnFromPath(string path, StiDataSource dataSource)
        {
            var strs = path.Split('.');

            var index = 0;
            var str = strs[index];
            while (true)
            {
                foreach (StiDataColumn dataColumn in dataSource.Columns)
                {
                    if (StiNameValidator.CorrectName(dataColumn.Name, dataSource.Dictionary?.Report) == str && index == strs.Length - 1)
                    {
                        return dataColumn;
                    }
                }

                var relations = dataSource.GetParentRelations();
                foreach (StiDataRelation relation in relations)
                {
                    if (StiNameValidator.CorrectName(relation.Name, dataSource.Dictionary?.Report) == str)
                    {
                        var newPath = path.Substring(relation.Name.Length + 1);
                        return GetColumnFromPath(newPath, relation.ParentSource);
                    }
                }

                if (index == strs.Length - 1) return null;

                index++;
                str += "." + strs[index];
            }
        }
        #endregion
    }
}