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

using Stimulsoft.Report.Dictionary.Adapters.Google;
using System;

namespace Stimulsoft.Report.Dictionary.DataSources.Google
{
    public class StiGoogleSheetsSource : StiDataStoreSource
    {
        #region Methods
        protected override Type GetDataAdapterType()
        {
            return typeof(StiGoogleSheetsAdapterService);
        }

        public override StiDataSource CreateNew()
        {
            return new StiGoogleSheetsSource();
        }
        #endregion

        /// <summary>
		/// Creates a new object of the type StiGoogleSheetsSource.
		/// </summary>
        public StiGoogleSheetsSource() : base("", "", "", null)
        {
        }

        public StiGoogleSheetsSource(string nameInSource, string name)
            : base(nameInSource, name)
        {
        }

        public StiGoogleSheetsSource(string nameInSource, string name, string alias, string key) :
            base(nameInSource, name, alias, key)
        {
        }
    }
}
