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


using Stimulsoft.Base.Localization;
using System;
using System.Data;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to Business Objects.
    /// </summary>
    public class StiBusinessObjectAdapterService : StiDataTableAdapterService
	{
        #region StiService override
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => StiLocalization.Get("Adapters", "AdapterBusinessObjects");
	    #endregion

        #region Properties
        public override bool IsObjectAdapter => true;
	    #endregion

		#region StiDataAdapterService override
		/// <summary>
		/// Returns name of category for data.
		/// </summary>
		public override string GetDataCategoryName(StiData data)
		{
			var table = data.ViewData as DataTable;
			if (table != null && table.DataSet != null)
			    return base.GetDataCategoryName(data);

			return ServiceName;
		}

		/// <summary>
		/// Returns the type of the Data Source.
		/// </summary>
		/// <returns>The type of Data Source.</returns>
		public override Type GetDataSourceType()
		{
			return typeof(StiBusinessObjectSource);
		}

		/// <summary>
		/// Returns the array of data types to which the Data Source may refer.
		/// </summary>
		/// <returns>Array of data types.</returns>
		public override Type[] GetDataTypes()
		{
			return new Type[] {typeof(object)};
		}
		#endregion
    }
}
