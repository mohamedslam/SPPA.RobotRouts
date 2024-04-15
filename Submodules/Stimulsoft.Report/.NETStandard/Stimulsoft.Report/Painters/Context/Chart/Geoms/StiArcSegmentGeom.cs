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
using Stimulsoft.Base.Json.Linq;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiArcSegmentGeom : StiSegmentGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Rect", SaveRectangleFToJsonObject(Rect)));
            jObject.Add(new JProperty("StartAngle", StartAngle));
            jObject.Add(new JProperty("SweepAngle", SweepAngle));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public RectangleF Rect { get; set; }

        public float StartAngle { get; set; }

        public float SweepAngle { get; set; }

        public float? RealStartAngle { get; set; }

        public float? RealSweepAngle { get; set; }

        public StiAnimation Animation { get; set; }

        public bool CrossElipseDraw { get; set; }
        #endregion

        public StiArcSegmentGeom(RectangleF rect, float startAngle, float sweepAngle)
        {
            this.Rect = rect;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }

        public StiArcSegmentGeom(RectangleF rect, float startAngle, float sweepAngle, float realStartAngle, float realSweepAngle)
        {
            this.Rect = rect;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
            this.RealStartAngle = realStartAngle;
            this.RealSweepAngle = realSweepAngle;
            this.CrossElipseDraw = true;
        }
    }
}
