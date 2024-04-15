﻿#region Copyright (C) 2003-2022 Stimulsoft
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
using Stimulsoft.Base.Json.Linq;

namespace Stimulsoft.Base.Context
{
    public class StiShadowGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Rect", SaveRectangleFToJsonObject(Rect)));
            jObject.Add(new JProperty("Radius", Radius));
            //jObject.Add(new JProperty("ShadowContext", shadowContext.SaveToJsonObject()));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public RectangleF Rect { get; set; }

        public float Radius { get; set; }

        public StiContext ShadowContext { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Shadow;
        #endregion

        public StiShadowGeom(StiContext shadowContext, RectangleF rect, float radius)
        {
            this.ShadowContext = shadowContext;
            this.Rect = rect;
            this.Radius = radius;
        }
    }
}
