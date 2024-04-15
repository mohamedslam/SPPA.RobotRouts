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
    public class StiRightButtonGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (!this.Axis.Core.IsMouseOverIncreaseButton)
            {
                this.Axis.Core.IsMouseOverIncreaseButton = true;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (this.Axis.Core.IsMouseOverIncreaseButton)
            {
                this.Axis.Core.IsMouseOverIncreaseButton = false;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseDown(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            options.UpdateContext = MoveRight();

            if (options.UpdateContext)
            {
                if (!options.IsRecalled)
                    options.RecallTime = TimeSpan.FromSeconds(StiAxisCoreXF.DefaultScrollBarFirstRecallTime);
                else                    
                    options.RecallTime = TimeSpan.FromSeconds(StiAxisCoreXF.DefaultScrollBarOtherRecallTime);

                options.RecallEvent = true;
            }
        }

        private bool MoveRight()
        {
            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;

            if (axisCore.ScrollValueX >= (axisCore.ScrollRangeX - axisCore.ScrollViewX))
                return false;
            else
            {
                axisCore.ScrollValueX +=
                    axisCore.ScrollViewX * StiAxisCoreXF.DefaultScrollBarSmallFactor;

                if (axisCore.ScrollValueX >= (axisCore.ScrollRangeX - axisCore.ScrollViewX))
                    axisCore.ScrollValueX = axisCore.ScrollRangeX - axisCore.ScrollViewX;

                axisCore.BlockScrollValueX = true;
                return true;
            }
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
        #endregion

        #region Methods
        /// <summary>
        /// Draws cell geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            float zoom = context.Options.Zoom;
            RectangleF rect = this.ClientRectangle;
            float centerY = rect.Y + rect.Height / 2;

            Color colorArrow = Color.White;

            #region Fill Button
            if (this.Axis.Core.IsMouseOverIncreaseButton)
            {
                context.FillRectangle(StiColorUtils.Light(Axis.LineColor, 50), rect.X, rect.Y, rect.Width, rect.Height, null);
                context.DrawRectangle(new StiPenGeom(this.Axis.LineColor), rect.X, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                colorArrow = Axis.LineColor;
            }
            #endregion

            #region Draw Arrow
            float arrowWidth = rect.Width / 4;
            float arrowHeight = rect.Height / 3;
            PointF arrowStart = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            context.PushSmoothingModeToAntiAlias();

            List<StiSegmentGeom> path = new List<StiSegmentGeom>();
            path.Add(new StiLineSegmentGeom(arrowStart.X - arrowWidth, arrowStart.Y - arrowHeight, arrowStart.X + arrowWidth, arrowStart.Y));
            path.Add(new StiLineSegmentGeom(arrowStart.X + arrowWidth, arrowStart.Y, arrowStart.X - arrowWidth, arrowStart.Y + arrowHeight));

            context.FillPath(colorArrow, path, rect, null);

            context.PopSmoothingMode();
            #endregion

            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiRightButtonGeom(IStiXAxis axis, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.axis = axis;
        }
    }
}
