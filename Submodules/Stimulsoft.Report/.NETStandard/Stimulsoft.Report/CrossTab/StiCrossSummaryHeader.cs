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

using System.ComponentModel;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;

namespace Stimulsoft.Report.CrossTab
{
	public class StiCrossSummaryHeader : StiCrossField
	{
	    #region IStiPropertyGridObject
	    public override StiComponentId ComponentId => StiComponentId.StiCrossSummaryHeader;
        #endregion

        #region Properties Browsable(true)
        [Browsable(true)]
		public sealed override bool Enabled
		{
			get 
			{
				return base.Enabled;
			}
			set 
			{
				base.Enabled = value;
			}
		}

        public override string CellText => this.GetTextInternal();

	    /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiCrossSummaryHeader");
        #endregion

	    #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiCrossSummaryHeader();
        }
        #endregion
    }
}