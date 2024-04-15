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

namespace Stimulsoft.Report.Chart
{
    public class StiGridLinesCoreXF : 
        IStiApplyStyle, 
        ICloneable
    {
        #region ICloneable
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region IStiApplyStyle
        public virtual void ApplyStyle(IStiChartStyle style)
        {
            if (GridLines.AllowApplyStyle)
            {
                if (this.GridLines is IStiGridLinesVert)
                {
                    this.GridLines.Color = style.Core.GridLinesVertColor;
                    this.GridLines.MinorColor = style.Core.GridLinesVertColor;
                }
                else
                {
                    this.GridLines.Color = style.Core.GridLinesHorColor;
                    this.GridLines.MinorColor = style.Core.GridLinesHorColor;
                }
            }
        }
        #endregion

        #region Properties
        private IStiGridLines gridLines;
        public IStiGridLines GridLines
        {
            get
            {
                return gridLines;
            }
            set
            {
                gridLines = value;
            }
        }
        #endregion

        public StiGridLinesCoreXF(IStiGridLines gridLines)
        {
            this.gridLines = gridLines;
        }
	}
}
