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
    public class StiVertTrackBarGeom : StiCellGeom
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
            axisCore.ScrollDragStartValue = axisCore.ScrollValueY;
        }

        public override void InvokeDrag(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;
            axisCore.BlockScrollValueY = true;

            float buttonSize = ScrollBar.ClientRectangle.Height;
            float height = ScrollBar.ClientRectangle.Height - buttonSize * 2;

            axisCore.ScrollValueY = -options.DragDelta.Height / height * axisCore.ScrollRangeY + axisCore.ScrollDragStartValue;

            #region Check Range
            if (axisCore.ScrollValueY < 0)
                axisCore.ScrollValueY = 0;

            if (axisCore.ScrollValueY >= (axisCore.ScrollRangeY - axisCore.ScrollViewY))
                axisCore.ScrollValueY = axisCore.ScrollRangeY - axisCore.ScrollViewY;
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
            float zoom = context.Options.Zoom;
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

            if (rect.Height > 10 && rect.Width > 4)
            {
                float rollerCenterY = rect.Y + rect.Height / 2;

                context.DrawLine(penWhite, rect.X + 2, rollerCenterY - 2, rect.Right - 2, rollerCenterY - 2);
                context.DrawLine(penWhite, rect.X + 2, rollerCenterY, rect.Right - 2, rollerCenterY);
                context.DrawLine(penWhite, rect.X + 2, rollerCenterY + 2, rect.Right - 2, rollerCenterY + 2);
            }
            #endregion
        }
        #endregion

        #region Properties
        private IStiYAxis axis;
        public IStiYAxis Axis
        {
            get
            {
                return axis;
            }
        }

        private StiVertScrollBarGeom scrollBar;
        public StiVertScrollBarGeom ScrollBar
        {
            get
            {
                return scrollBar;
            }
        }
        #endregion

        public StiVertTrackBarGeom(IStiYAxis axis, RectangleF clientRectangle, StiVertScrollBarGeom scrollBar)
            : base(clientRectangle)
        {
            this.axis = axis;
            this.scrollBar = scrollBar;
        }
    }
}
