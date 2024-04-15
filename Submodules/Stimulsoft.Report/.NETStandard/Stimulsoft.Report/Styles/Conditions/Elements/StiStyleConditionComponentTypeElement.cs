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
    public class StiStyleConditionComponentTypeElement : StiStyleConditionElement
    {
        #region Properties
        /// <summary>
        /// Gets or sets component type which can be detected by style condition.
        /// </summary>
        public StiStyleComponentType ComponentType { get; set; }

        /// <summary>
        /// Gets or sets type of operation which will be used for comparison of component types.
        /// </summary>
        public StiStyleConditionOperation OperationComponentType { get; set; }
        #endregion

        public StiStyleConditionComponentTypeElement(StiStyleComponentType componentType)
            : this(componentType, StiStyleConditionOperation.EqualTo)
        {
        }

        public StiStyleConditionComponentTypeElement(StiStyleComponentType componentType, StiStyleConditionOperation operationComponentType)
        {
            this.ComponentType = componentType;
            this.OperationComponentType = operationComponentType;
        }
    }
}
