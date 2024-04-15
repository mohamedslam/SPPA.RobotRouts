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
    public class StiUpButtonGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseEnter(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (!this.Axis.Core.IsMouseOverDecreaseButton)
            {
                this.Axis.Core.IsMouseOverDecreaseButton = true;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseLeave(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            if (this.Axis.Core.IsMouseOverDecreaseButton)
            {
                this.Axis.Core.IsMouseOverDecreaseButton = false;
                options.UpdateContext = true;
            }
        }

        public override void InvokeMouseDown(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            options.UpdateContext = MoveUp();

            if (options.UpdateContext)
            {
                if (!options.IsRecalled)
                    options.RecallTime = TimeSpan.FromSeconds(StiAxisCoreXF.DefaultScrollBarFirstRecallTime);
                else
                    options.RecallTime = TimeSpan.FromSeconds(StiAxisCoreXF.DefaultScrollBarOtherRecallTime);

                options.RecallEvent = true;
            }
        }

        private bool MoveUp()
        {
            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;

            if (axisCore.ScrollValueY == 0)
                return false;
            else
            {
                axisCore.ScrollValueY -=
                    axisCore.ScrollViewY * StiAxisCoreXF.DefaultScrollBarSmallFactor;

                if (axisCore.ScrollValueY < 0)
                    axisCore.ScrollValueY = 0;

                axisCore.BlockScrollValueY = true;
                return true;
            }
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
        #endregion

        #region Methods
        /// <summary>
        /// Draws cell geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            float zoom = context.Options.Zoom;
            RectangleF rect = this.ClientRectangle;
            float centerX = rect.X + rect.Width / 2;

            Color colorArrow = Color.White;

            #region Fill Button
            if (this.Axis.Core.IsMouseOverDecreaseButton)
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
            float arrowWidth = rect.Width / 3;
            float arrowHeight = rect.Height / 4;
            PointF arrowStart = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            context.PushSmoothingModeToAntiAlias();

            List<StiSegmentGeom> path = new List<StiSegmentGeom>();
            path.Add(new StiLineSegmentGeom(arrowStart.X - arrowWidth, arrowStart.Y + arrowHeight, arrowStart.X, arrowStart.Y - arrowHeight));
            path.Add(new StiLineSegmentGeom(arrowStart.X, arrowStart.Y - arrowHeight, arrowStart.X + arrowWidth, arrowStart.Y + arrowHeight));

            context.FillPath(colorArrow, path, rect, null);

            context.PopSmoothingMode();
            #endregion

            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiUpButtonGeom(IStiYAxis axis, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.axis = axis;
        }
    }
}
