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
using System.Collections.Generic;
using Stimulsoft.Base.Context;

namespace Stimulsoft.Report.Chart.Geoms.Series.Funnel
{
    public class StiFunnelEmptySeriesElementGeom : StiCellGeom
    {
        #region Properties
        public List<StiSegmentGeom> Path { get; }
        #endregion

        public override void Draw(StiContext context)
        {
            context.PushSmoothingModeToAntiAlias();
            context.FillPath(Color.FromArgb(50, Color.LightGray), Path, this.ClientRectangle);
            context.DrawPath(new StiPenGeom(Color.LightGray), Path, this.ClientRectangle);
            context.PopSmoothingMode();
        }

        public StiFunnelEmptySeriesElementGeom(RectangleF clientRectangle, List<StiSegmentGeom> path) : base(clientRectangle)
        {
            this.Path = path;
        }
    }
}
