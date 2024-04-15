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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Base.Context
{
    public class StiPenGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            if (Brush != null) jObject.Add(new JProperty("Brush", SaveBrushToJsonObject(Brush, mode)));
            jObject.Add(new JProperty("Thickness", Thickness));
            jObject.Add(new JProperty("PenStyle", PenStyle.ToString()));
            jObject.Add(new JProperty("Alignment", Alignment.ToString()));
            jObject.Add(new JProperty("StartCap", StartCap.ToString()));
            jObject.Add(new JProperty("EndCap", EndCap.ToString()));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public object Brush { get; set; }

        public float Thickness { get; set; } = 1f;

        public StiPenStyle PenStyle { get; set; } = StiPenStyle.Solid;

        public StiPenAlignment Alignment { get; set; } = StiPenAlignment.Center;

        public StiPenLineCap StartCap { get; set; } = StiPenLineCap.Flat;

        public StiPenLineCap EndCap { get; set; } = StiPenLineCap.Flat;
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Pen;
        #endregion

        public StiPenGeom(object brush)
            :
            this(brush, 1f)
        {
        }

        public StiPenGeom(object brush, float thickness)
        {
            this.Brush = brush;
            this.Thickness = thickness;
        }
    }
}
