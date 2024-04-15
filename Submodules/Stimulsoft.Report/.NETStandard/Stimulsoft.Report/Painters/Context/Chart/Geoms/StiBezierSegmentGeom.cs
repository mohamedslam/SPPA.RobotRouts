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

using Stimulsoft.Base.Context.Animation;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiBezierSegmentGeom : StiSegmentGeom
    {
        #region Properties
        public PointF Pt1 { get; private set; }

        public PointF Pt2 { get; private set; }

        public PointF Pt3 { get; private set; }

        public PointF Pt4 { get; private set; }

        public StiAnimation Animation { get; set; }
        #endregion

        public StiBezierSegmentGeom(PointF pt1, PointF pt2, PointF pt3, PointF pt4, StiAnimation animation = null)
        {
            this.Pt1 = pt1;
            this.Pt2 = pt2;
            this.Pt3 = pt3;
            this.Pt4 = pt4;

            this.Animation = animation;
        }
    }
}
