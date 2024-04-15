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
    public class StiYRadarAxisLabelGeom : StiCellGeom
    {
        #region Properties
        private StiRotationMode rotationMode;
        public StiRotationMode RotationMode
        {
            get
            {
                return rotationMode;
            }
        }

        private PointF textPoint;
        public PointF TextPoint
        {
            get
            {
                return textPoint;
            }
        }

        private float angle;
        public float Angle
        {
            get
            {
                return angle;
            }
        }

        private IStiYRadarAxis axis;
        public IStiYRadarAxis Axis
        {
            get
            {
                return axis;
            }
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
        }

        private StiStripLineXF stripLine;
        public StiStripLineXF StripLine
        {
            get
            {
                return stripLine;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF rect = this.ClientRectangle;

            StiStringFormatGeom sf = Axis.YCore.GetStringFormatGeom(context);
            StiFontGeom font = Axis.YCore.GetFontGeom(context);
            StiBrush labelBrush = new StiSolidBrush(Axis.Labels.Color);

            context.DrawRotatedString(
                        Text, font, labelBrush,
                        this.TextPoint, sf,
                        this.RotationMode, this.Angle, Axis.Labels.Antialiasing,
                        (int)(Axis.Labels.Width * context.Options.Zoom));

            //Red line
            //context.DrawRectangle(new StiPenGeom(Color.Blue), rect.X, rect.Y, rect.Width, rect.Height);
        }
        #endregion

        public StiYRadarAxisLabelGeom(IStiYRadarAxis axis, RectangleF clientRectangle, PointF textPoint, string text, StiStripLineXF stripLine, 
            float angle, StiRotationMode rotationMode)
            : base(clientRectangle)
        {
            this.axis = axis;
            this.text = text;
            this.stripLine = stripLine;
            this.textPoint = textPoint;
            this.angle = angle;
            this.rotationMode = rotationMode;
        }
    }
}
