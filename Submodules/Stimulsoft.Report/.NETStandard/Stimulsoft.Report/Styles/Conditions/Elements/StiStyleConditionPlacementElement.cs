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

namespace Stimulsoft.Report
{
    public class StiStyleConditionPlacementElement : StiStyleConditionElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets type of bands on which component can be placed.
        /// </summary>
        public StiStyleComponentPlacement Placement { get; set; }

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component placements.
        /// </summary>
        public StiStyleConditionOperation OperationPlacement { get; set; }
        #endregion

        public StiStyleConditionPlacementElement(StiStyleComponentPlacement placement)
            : this(placement, StiStyleConditionOperation.EqualTo)
        {
        }

        public StiStyleConditionPlacementElement(StiStyleComponentPlacement placement, StiStyleConditionOperation operationPlacement)
        {
            this.Placement = placement;
            this.OperationPlacement = operationPlacement;
        }
    }
}
