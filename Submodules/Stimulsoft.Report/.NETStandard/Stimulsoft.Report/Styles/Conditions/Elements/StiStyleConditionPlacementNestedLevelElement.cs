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
    public class StiStyleConditionPlacementNestedLevelElement : StiStyleConditionElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets value which indicates nested level of specified component.
        /// </summary>
        public int PlacementNestedLevel { get; set; }

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component nested level.
        /// </summary>
        public StiStyleConditionOperation OperationPlacementNestedLevel { get; set; }
        #endregion

        public StiStyleConditionPlacementNestedLevelElement(int placementNestedLevel)
            : this(placementNestedLevel, StiStyleConditionOperation.EqualTo)
        {
        }

        public StiStyleConditionPlacementNestedLevelElement(int placementNestedLevel, StiStyleConditionOperation operationPlacementNestedLevel)
        {
            this.PlacementNestedLevel = placementNestedLevel;
            this.OperationPlacementNestedLevel = operationPlacementNestedLevel;
        }
    }
}
