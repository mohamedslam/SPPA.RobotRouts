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
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiCachedShadowGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Rect", SaveRectangleFToJsonObject(Rect)));
            jObject.Add(new JProperty("Sides", Sides.ToString()));
            jObject.Add(new JProperty("IsPrinting", IsPrinting));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public RectangleF Rect { get; set; }

        public RectangleF ClipRect { get; set; }

        public StiShadowSides Sides { get; set; }

        public bool IsPrinting { get; set; }

        public StiCornerRadius CornerRadius { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.CachedShadow;
        #endregion

        public StiCachedShadowGeom(RectangleF rect, StiShadowSides sides, bool isPrinting)
            :this(rect, sides, isPrinting, rect)
        {
        }

        public StiCachedShadowGeom(RectangleF rect, StiShadowSides sides, bool isPrinting, RectangleF clipRect)
            : this(rect, sides, isPrinting, clipRect, null)
        {
        }

        public StiCachedShadowGeom(RectangleF rect, StiShadowSides sides, bool isPrinting, RectangleF clipRect, StiCornerRadius cornerRadius)
        {
            this.Rect = rect;
            this.ClipRect = clipRect;
            this.Sides = sides;
            this.IsPrinting = isPrinting;
            this.CornerRadius = cornerRadius;
        }
    }
}