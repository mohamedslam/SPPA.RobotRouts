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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dictionary
{
    /// <summary>
    /// Describes the adapter for access to Csv files.
    /// </summary>
    public abstract class StiFileAdapterService : StiDataStoreAdapterService
	{
        #region Properties
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => string.Format(StiLocalization.Get("Adapters", "AdapterConnection"), CreateConnector().Name);
        
        /// <summary>
        /// Returns name of category for data.
        /// </summary>
        public override string GetDataCategoryName(StiData data)
		{
			return data.Name;
		}
        #endregion

        #region Methods
        /// <summary>
	    /// Returns new data connector for this type of the database.
	    /// </summary>
	    /// <returns>Created connector.</returns>
	    public abstract StiFileDataConnector CreateConnector();
        #endregion
	}
}
