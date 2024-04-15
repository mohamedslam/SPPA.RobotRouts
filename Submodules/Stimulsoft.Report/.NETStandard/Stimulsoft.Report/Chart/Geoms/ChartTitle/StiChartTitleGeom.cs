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
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Context;
using System.Drawing;

namespace Stimulsoft.Report.Chart
{
    public class StiChartTitleGeom : StiCellGeom
    {
        #region Properties
        private IStiChartTitle title;
        public IStiChartTitle Title
        {
            get
            {
                return title;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws area geom object on spefied context.
        /// </summary>
        public override void Draw(StiContext context)
        {
            if (Title.Visible)
            {
                float angle = (float)Title.Dock;
                if (Title.Dock == StiChartTitleDock.Bottom) angle = 0;

                StiFontGeom font = StiFontGeom.ChangeFontSize(Title.Font, Title.Font.Size * context.Options.Zoom);
                StiStringFormatGeom sf = context.GetDefaultStringFormat();

                sf.Alignment = Title.Alignment;

                sf.Trimming = StringTrimming.None;
                sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
                
                context.DrawRotatedString(Title.Text, font, Title.Brush, ClientRectangle, sf, StiRotationMode.CenterCenter, angle, Title.Antialiasing);
            }
        }
        #endregion

        public StiChartTitleGeom(IStiChartTitle title, RectangleF clientRectangle)
            : base(clientRectangle)
        {
            this.title = title;
        }
    }
}
