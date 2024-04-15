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

using System.Linq;

namespace Stimulsoft.Data.Engine
{
    public class StiDataColumnRuleHelper
    {
        #region Methods.Static
        public static bool IsGoodColumnName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                return false;

            if (columnName.StartsWith("["))
                return true;

            if (columnName.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            if (char.IsDigit(columnName[0]))
                return false;

            return !keywords.Contains(columnName.ToLowerInvariant());
        }

        public static string GetGoodColumnName(string columnName, bool normalize = false)
        {
            if ((columnName.Contains(",") || columnName.Contains("'")) && normalize)
                columnName = columnName.Replace(",", "").Replace("'", "");

            return IsGoodColumnName(columnName) ? columnName : $"[{columnName}]";
        }
        #endregion

        #region Field.Static
        private static string[] keywords = 
        {
            "and",
            "between",
            "child",
            "false",
            "in",
            "is",
            "like",
            "not",
            "null",
            "or",
            "parent",
            "true"
        };
        #endregion
    }
}