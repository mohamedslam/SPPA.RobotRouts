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
    public class StiHorzScrollBarGeom : StiCellGeom
    {
        #region IStiGeomInteraction override
        public override void InvokeMouseDown(StiInteractionOptions options)
        {
            if (!this.Axis.Interaction.RangeScrollEnabled)
                return;

            StiAxisAreaCoreXF axisCore = this.Axis.Area.Core as StiAxisAreaCoreXF;

            axisCore.BlockScrollValueX = true;

            float buttonSize = this.ClientRectangle.Height;
            float mousePointX = options.MousePoint.X - buttonSize;
            float width = this.ClientRectangle.Width - buttonSize * 2;

            axisCore.ScrollValueX = mousePointX / width * axisCore.ScrollRangeX;

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
            #endregion

            context.DrawRectangle(penLine, rect.X, rect.Y, rect.Width, rect.Height);
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

        public StiHorzScrollBarGeom(IStiXAxis axis, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.axis = axis;
        }
    }
}
