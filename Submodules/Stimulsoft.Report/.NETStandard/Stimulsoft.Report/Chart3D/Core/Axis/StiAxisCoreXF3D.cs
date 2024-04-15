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

using Stimulsoft.Base.Context;
using System;

namespace Stimulsoft.Report.Chart
{
    public abstract class StiAxisCoreXF3D :
        ICloneable,
        IStiApplyStyle
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
            if (this.Axis.AllowApplyStyle)
            {
                this.Axis.LineColor = style.Core.AxisLineColor;

                this.Axis.Labels.Core.ApplyStyle(style);
            }
        }

        protected internal StiFontGeom GetFontGeom(StiContext context)
        {
            return StiFontGeom.ChangeFontSize(this.Axis.Labels.Font, this.Axis.Labels.Font.Size * context.Options.Zoom); ;
        }
        #endregion

        #region Properties
        private IStiAxis3D axis;
        public IStiAxis3D Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        } 
        #endregion

        #region Methods
        protected internal void CalculateStripPositions(double topPosition, double bottomPosition)
        {
            bottomPosition -= topPosition;
            topPosition = 0;

            if (this.Axis.Info.StripLines == null || this.Axis.Info.StripLines.Count < 2)
            {
                this.Axis.Info.StripPositions = new float[0];
            }
            else
            {
                this.Axis.Info.StripPositions = new float[this.Axis.Info.StripLines.Count];
                this.Axis.Info.StripPositions[0] = (float)topPosition;
                this.Axis.Info.StripPositions[this.Axis.Info.StripPositions.Length - 1] = (float)bottomPosition;

                for (int index = 1; index < this.Axis.Info.StripPositions.Length - 1; index++)
                {
                    this.Axis.Info.StripPositions[index] = (float)(topPosition + index * this.Axis.Info.Step);
                }
            }
        }

        public abstract StiCellGeom Render3D(StiContext context, StiRectangle3D rect3D, StiRender3D render);
        #endregion

        public StiAxisCoreXF3D(IStiAxis3D axis)
        {
            this.axis = axis;
        }
    }
}
