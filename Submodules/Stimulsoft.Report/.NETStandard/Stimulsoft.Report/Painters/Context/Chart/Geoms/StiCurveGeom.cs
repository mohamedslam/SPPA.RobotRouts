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

using Stimulsoft.Base.Json.Linq;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiCurveGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            if (Pen != null) jObject.Add(new JProperty("Pen", Pen.SaveToJsonObject(mode)));
            jObject.Add(new JProperty("Tension", Tension));
            jObject.Add(new JProperty("Points", SavePointFArrayToJsonObject(Points)));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public StiPenGeom Pen { get; set; }

        public float Tension { get; set; }

        public PointF[] Points { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Curve;
        #endregion

        public StiCurveGeom(StiPenGeom pen, PointF[] points, float tension)
        {
            this.Pen = pen;
            this.Tension = tension;
            this.Points = points;
        }
    }
}
