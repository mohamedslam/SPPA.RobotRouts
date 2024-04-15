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

using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart
{
    public class StiAxisLabelGeom : StiCellGeom
    {
        #region Properties
        public StiRotationMode RotationMode { get; }

        public PointF TextPoint { get; }

        public float Angle { get; }

        public float Width { get; }

        public IStiAxis Axis { get; }

        public string Text { get; }

        public StiStripLineXF StripLine { get; }

        public bool WordWrap { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            var sf = Axis.Core.GetStringFormatGeom(context, this.WordWrap);
            var font = Axis.Core.GetFontGeom(context);
            var labelBrush = new StiSolidBrush(Axis.Labels.Color);

            context.DrawRotatedString(Text, font, labelBrush,this.TextPoint, sf,
                        this.RotationMode, this.Angle, Axis.Labels.Antialiasing,
                        (int)(this.Width * context.Options.Zoom));
        }
        #endregion

        public StiAxisLabelGeom(IStiAxis axis, RectangleF clientRectangle, PointF textPoint, string text, StiStripLineXF stripLine, 
            float angle, float width, StiRotationMode rotationMode, bool wordWrap)
            : base(clientRectangle)
        {
            this.Axis = axis;
            this.Text = text;
            this.StripLine = stripLine;
            this.TextPoint = textPoint;
            this.Angle = angle;
            this.Width = width;
            this.WordWrap = wordWrap;
            this.RotationMode = rotationMode;
        }
    }
}
