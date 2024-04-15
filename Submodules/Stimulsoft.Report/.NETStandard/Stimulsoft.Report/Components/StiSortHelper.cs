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

namespace Stimulsoft.Report.Components
{
	public sealed class StiSortHelper
	{
		public static int GetColumnIndexInSorting(string[] sorts, string columnName)
		{
			int sortIndex = 0;
			string sortStr = "";
			int index = 0;
			foreach (string str in sorts)
			{
				#region Add sorting str
				if (str != "ASC" && str != "DESC")
				{
					if (sortStr.Length == 0) sortStr = str;
					else sortStr += "." + str;
				}
				#endregion

				if (str == "ASC" || str == "DESC" || index == sorts.Length - 1)
				{
					#region If Sorting string is not empty then process it
					if (sortStr.Length > 0)
					{
						if (columnName == sortStr)return sortIndex;

						sortStr = "";
						sortIndex++;
					}
					#endregion
				}
				index++;
			}
			return -1;
		}

		public static StiInteractionSortDirection GetColumnSortDirection(string[] sorts, string columnName)
		{
			int sortIndex = GetColumnIndexInSorting(sorts, columnName);
			if (sortIndex == -1) return StiInteractionSortDirection.None;
			foreach (string str in sorts)
			{
				if (str == "ASC" || str == "DESC")
				{
					if (sortIndex == 0)
					{
						if (str == "ASC") return StiInteractionSortDirection.Ascending;
						if (str == "DESC") return StiInteractionSortDirection.Descending;
					}

					sortIndex--;
				}
			}
			return StiInteractionSortDirection.None;
		}

		public static string[] ChangeColumnSortDirection(string[] sorts, string columnName)
		{
			int sortIndex = GetColumnIndexInSorting(sorts, columnName);
			if (sortIndex == -1) return sorts;
			int index = 0;
			foreach (string str in sorts)
			{
				if (str == "ASC" || str == "DESC")
				{
					if (sortIndex == 0)
					{
						if (str == "ASC") sorts[index] = "DESC";
						if (str == "DESC") sorts[index] = "ASC";
						return sorts;
					}

					sortIndex--;
				}
				index++;
			}
			return sorts;
		}

		public static bool IsColumnExistInSorting(string[] sorts, string columnName)
		{
			return GetColumnIndexInSorting(sorts, columnName) != -1;
		}

		public static string[] AddColumnToSorting(string[] sorts, string columnName, bool isAscending)
		{
			string[] strs = columnName.Split(new char[]{'.'});
            if (columnName.StartsWith("{", StringComparison.InvariantCulture) && columnName.EndsWith("}", StringComparison.InvariantCulture))
            {
                strs = new string[1] { columnName };
            }

			string []newSorts = new string[sorts.Length + strs.Length + 1];
			for (int index = 0; index < sorts.Length; index++)
			{
				newSorts[index] = sorts[index];
			}
			
			newSorts[sorts.Length] = isAscending ? "ASC" : "DESC";

			for (int index = 0; index < strs.Length; index++)
			{
				newSorts[sorts.Length + index + 1] = strs[index];
			}

			return newSorts;
		}
	}
}
