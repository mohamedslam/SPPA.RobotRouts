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

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the class that is used to set names of the relation when this relation is created.
	/// </summary>
	public class StiDataRelationSetName
	{
		/// <summary>
		/// Sets a name and alias of relation.
		/// </summary>
		/// <param name="dataRelation">StiDataRelation that is used to set parameters.</param>
		/// <param name="report">The report in the dictionary of data from which relation is registered.</param>
		/// <param name="dataSet">DataSet in which the master of relation is located.</param>
		/// <param name="name">Relation name.</param>
		public static void SetName(StiDataRelation dataRelation, StiReport report,
			DataSet dataSet, string name)
		{
			dataRelation.NameInSource = name;

		    if (dataRelation.ParentSource == null) return;

			var relationName = StiNameCreation.CreateRelationName(report, dataRelation, dataRelation.ParentSource.Name);

			dataRelation.Name = relationName;
		    dataRelation.Alias = relationName;
		}
	}
}
