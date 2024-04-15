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

using Stimulsoft.Report.Engine;

namespace Stimulsoft.Report.Components
{	
	public class StiGroupHeaderBandInfoV1 : StiComponentInfo
	{
		/// <summary>
		/// Used for calculate system variable Line 
		/// </summary>
		public int LastPositionLineRendering { get; set; } = -1;

		public int LastPositionRendering { get; set; } = -1;

		/// <summary>
		/// Gets or sets value, indicates that it is necessary to reprint the group header.
		/// </summary>
		public bool Rerender { get; set; }

        public bool ForceCanBreak { get; set; }

        public bool IsFirstPassOfBreak { get; set; } = true;

		public StiComponentsCollection BreakableComps { get; set; }

        /// <summary>
        /// Gets or sets the group of footers which fits to this header.
        /// </summary>
        public StiGroupFooterBand GroupFooter { get; set; }
    }
}
