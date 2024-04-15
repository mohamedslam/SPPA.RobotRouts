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
    public class StiYAxisViewGeom : StiYAxisGeom        
    {
        #region Methods
        /// <summary>
        /// Draws area geom object with all child geom objects on spefied context.
        /// </summary>
        public override void DrawChildGeoms(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;

            #region Center Axis Processing
            if (this.IsCenterAxis)
            {
                rect.X +=
                    ((StiAxisAreaCoreXF)this.Axis.Area.Core).GetDividerX() -
                    (float)((StiAxisAreaCoreXF)Axis.Area.Core).ScrollDistanceX;
            }
            #endregion
                        

            if (this.ChildGeoms != null)
            {                
                foreach (StiCellGeom childGeom in ChildGeoms)
                {
                    if (AllowChildDrawing(childGeom))
                    {
                        if (!(childGeom is StiVertScrollBarGeom))
                            context.PushTranslateTransform(0, -(float)((StiAxisAreaCoreXF)Axis.Area.Core).ScrollDistanceY);

                        context.PushTranslateTransform(rect.X, rect.Y);
                        childGeom.DrawGeom(context);
                        context.PopTransform();
                        
                        if (!(childGeom is StiVertScrollBarGeom))
                            context.PopTransform();
                    }
                }                
            }
        }

        /// <summary>
        /// Draws cell geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;

            if (this.IsCenterAxis)
            {
                rect.X +=
                    ((StiAxisAreaCoreXF)this.Axis.Area.Core).GetDividerX() -
                    (float)((StiAxisAreaCoreXF)Axis.Area.Core).ScrollDistanceX;
            }

            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (Axis.ArrowStyle != StiArrowStyle.None && (!IsCenterAxis))
                DrawArrow(context, rect);

            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Red), rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiYAxisViewGeom(IStiYAxis axis, RectangleF clientRectangle, bool isCenterAxis)
            : base(axis, clientRectangle, isCenterAxis)
        {
        }
    }
}
