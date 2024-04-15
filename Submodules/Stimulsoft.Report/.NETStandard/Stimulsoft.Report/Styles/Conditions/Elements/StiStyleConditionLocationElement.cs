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
    public class StiStyleConditionLocationElement : StiStyleConditionElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component locations.
        /// </summary>
        public StiStyleConditionOperation OperationLocation { get; set; }
        
        /// <summary>
        /// Gets or sets variant of component location on parent component area.
        /// </summary>
        public StiStyleLocation Location { get; set; }
        #endregion

        public StiStyleConditionLocationElement(StiStyleLocation location)
            : this(location, StiStyleConditionOperation.EqualTo)
        {
        }

        public StiStyleConditionLocationElement(StiStyleLocation location, StiStyleConditionOperation operationLocation)
        {
            this.Location = location;
            this.OperationLocation = operationLocation;
        }
    }
}
