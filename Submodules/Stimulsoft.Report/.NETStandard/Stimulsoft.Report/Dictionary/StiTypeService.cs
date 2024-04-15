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
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Describes the base service for registration of the types.
	/// </summary>
	[StiServiceBitmap(typeof(StiTypeService), "Stimulsoft.Report.Bmp.Type.bmp")]
	public abstract class StiTypeService : StiService
	{
		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		public sealed override string ServiceCategory => StiLocalization.Get("Services", "categoryDictionary");

	    /// <summary>
		/// Gets a service type.
		/// </summary>
		public sealed override Type ServiceType => typeof(StiTypeService);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets collection of the registered types.
        /// </summary>
        public StiTypesCollection Types { get; set; } = new StiTypesCollection();
        #endregion
    }
}
