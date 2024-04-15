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

using System.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiYRightAxisCoreXF : StiYAxisCoreXF
    {
        #region Properties
        public override StiYAxisDock Dock
        {
            get
            {
                return StiYAxisDock.Right;
            }
        }
        #endregion

        #region Methods
        public override bool GetStartFromZero()
        {
            if (this.Axis.Area.AxisCore.ValuesCount == 1)
                return true;
            if (this.Axis != null && this.Axis.Range != null && (!this.Axis.Range.Auto)) return false;
            return this.Axis != null ? this.Axis.StartFromZero : true;
        }
        #endregion

        public StiYRightAxisCoreXF(IStiAxis axis)
            : base(axis)
        {
        }

	}
}
