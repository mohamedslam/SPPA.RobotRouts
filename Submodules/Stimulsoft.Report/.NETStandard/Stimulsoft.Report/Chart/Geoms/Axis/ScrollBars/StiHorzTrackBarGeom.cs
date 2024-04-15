#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiHorzTrackBarGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (!this.Axis.Core.IsMouseOverTrackBar)
            {
                this.Axis.Core.IsMouseOverTrackBar = true;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (this.Axis.Core.IsMouseOverTrackBar)
            {
                this.Axis.Core.IsMouseOverTrackBar = false;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseDown(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;

            options.DragEnabled = true;
            axisCore.ScrollDragStartValue = axisCore.ScrollValueX;
        }

        public override void InvokeDrag(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;
            axisCore.BlockScrollValueX = true;

            float buttonSize = ScrollBar.ClientRectangle.Height;
            float width = ScrollBar.ClientRectangle.Width - buttonSize * 2;

            axisCore.ScrollValueX = options.DragDelta.Width / width * axisCore.ScrollRangeX + axisCore.ScrollDragStartValue;

            #region Check Range
            if (axisCore.ScrollValueX < 0)
                axisCore.ScrollValueX = 0;

            if (axisCore.ScrollValueX >= (axisCore.ScrollRangeX - axisCore.ScrollViewX))
                axisCore.ScrollValueX = axisCore.ScrollRangeX - axisCore.ScrollViewX;
            #endregion    
         
            options.UpdateContext = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws cell geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;

            #region Create Pens
            StiPenGeom penLine = new StiPenGeom(Axis.LineColor);
            StiPenGeom penWhite = new StiPenGeom(Color.White);
            #endregion

            #region Draw Roller
            StiAxisAreaCoreXF axisCore = Axis.Area.Core as StiAxisAreaCoreXF;
            IStiXAxis rollAxis = Axis.Area.XAxis;

            #region Draw Roll Bar
            if (Axis.Core.IsMouseOverTrackBar)
                context.FillRectangle(StiColorUtils.Light(Axis.LineColor, 50), rect.X, rect.Y, rect.Width, rect.Height, null);
            else
                context.FillRectangle(Axis.LineColor, rect.X, rect.Y, rect.Width, rect.Height, null);

            context.DrawRectangle(penLine, rect.X, rect.Y, rect.Width, rect.Height);
            #endregion

            if (rect.Width > 10 && rect.Height > 4)
            {
                float rollerCenterX = rect.X + rect.Width / 2;

                context.DrawLine(penWhite, rollerCenterX - 2, rect.Y + 2, rollerCenterX - 2, rect.Bottom - 2);
                context.DrawLine(penWhite, rollerCenterX, rect.Y + 2, rollerCenterX, rect.Bottom - 2);
                context.DrawLine(penWhite, rollerCenterX + 2, rect.Y + 2, rollerCenterX + 2, rect.Bottom - 2);
            }
            #endregion
        }
        #endregion

        #region Properties
        private IStiXAxis axis;
        public IStiXAxis Axis
        {
            get
            {
                return axis;
            }
        }

        private StiHorzScrollBarGeom scrollBar;
        public StiHorzScrollBarGeom ScrollBar
        {
            get
            {
                return scrollBar;
            }
        }
        #endregion

        public StiHorzTrackBarGeom(IStiXAxis axis, RectangleF clientRectangle, StiHorzScrollBarGeom scrollBar)
            : base(clientRectangle)
        {
            this.axis = axis;
            this.scrollBar = scrollBar;
        }
    }
}
