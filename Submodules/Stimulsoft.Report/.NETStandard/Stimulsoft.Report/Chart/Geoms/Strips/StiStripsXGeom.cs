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
using Stimulsoft.Report.Dashboard;

namespace Stimulsoft.Report.Chart
{
    public class StiStripsXGeom : StiCellGeom
    {
        #region Properties
        private IStiStrips strip;
        public IStiStrips Strip
        {
            get
            {
                return strip;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            RectangleF stripRect = this.ClientRectangle;
            
            context.FillRectangle(strip.StripBrush, stripRect.X, stripRect.Y, stripRect.Width, stripRect.Height, null);

            if (strip.TitleVisible)
            {
                StiBrush brush = new StiSolidBrush(strip.TitleColor);
                StiFontGeom font = StiFontGeom.ChangeFontSize(strip.Font, (float)(strip.Font.Size * context.Options.Zoom));

                StiStringFormatGeom sf = context.GetGenericStringFormat();
                var text = StiReportParser.Parse(strip.Text, strip.Chart as StiChart);
                context.DrawRotatedString(strip.Text, font, brush, stripRect, sf, StiRotationMode.CenterCenter, 90, strip.Antialiasing, 0);
            }
        }
        #endregion

        public StiStripsXGeom(IStiStrips strip, RectangleF clientRectangle)
            :
            base(clientRectangle)
        {
            this.strip = strip;
        }
    }
}
