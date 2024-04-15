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
    public class StiAxisTitleGeom : StiCellGeom
    {
        #region Properties
        public IStiAxis Axis { get; }

        public float Angle { get; }

        private StiFontGeom Font { get; }
        #endregion

        #region Methods
        public override void Draw(StiContext context)
        {
            var brush = new StiSolidBrush(Axis.Title.Color);            
            var sf = context.GetDefaultStringFormat();
            
            context.DrawRotatedString(Axis.Title.Text, this.Font, brush, ClientRectangle, sf, StiRotationMode.CenterCenter, Angle, Axis.Title.Antialiasing);
        }
        #endregion

        public StiAxisTitleGeom(IStiAxis axis, RectangleF clientRectangle, float angle, StiFontGeom font)
            : base(clientRectangle)
        {
            this.Axis = axis;
            this.Angle = angle;
            this.Font = font;
        }
    }
}
