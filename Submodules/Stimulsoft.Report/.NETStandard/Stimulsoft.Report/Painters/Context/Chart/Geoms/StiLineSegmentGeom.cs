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
    public class StiLineSegmentGeom : StiSegmentGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("X1", X1));
            jObject.Add(new JProperty("Y1", Y1));
            jObject.Add(new JProperty("X2", X2));
            jObject.Add(new JProperty("Y2", Y2));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public float X1 { get; set; }

        public float Y1 { get; set; }

        public float X2 { get; set; }

        public float Y2 { get; set; }

        public StiAnimation Animation { get; set; }
        #endregion

        public StiLineSegmentGeom(PointF start, PointF end, StiAnimation animation) : this(start, end)
        {
            this.Animation = animation;
        }

        public StiLineSegmentGeom(PointF start, PointF end)
        {
            this.X1 = start.X;
            this.Y1 = start.Y;
            this.X2 = end.X;
            this.Y2 = end.Y;
        }

        public StiLineSegmentGeom(float x1, float y1, float x2, float y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
    }
}
