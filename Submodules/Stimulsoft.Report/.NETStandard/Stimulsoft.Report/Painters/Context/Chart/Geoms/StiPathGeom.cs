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
using System.Collections.Generic;

namespace Stimulsoft.Base.Context
{
    public class StiPathGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Rect", SaveRectToJsonObject(Rect)));
            if (Background != null) jObject.Add(new JProperty("Background", SaveBrushToJsonObject(Background, mode)));
            if (Pen != null) jObject.Add(new JProperty("Pen", Pen.SaveToJsonObject(mode)));
            if (Geoms != null) jObject.Add(new JProperty("Geoms", SaveGeomListToJsonObject(Geoms, mode)));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Static.Properies
        public static object GetBoundsState { get; set; } = new object();
        #endregion

        #region Properties
        public object Background { get; set; }

        public StiPenGeom Pen { get; set; }

        public object Rect { get; set; }

        public List<StiSegmentGeom> Geoms { get; set; }

        public StiInteractionDataGeom Interaction { get; set; }

        public int ElementIndex { get; set; }

        public string ToolTip { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Path;
        #endregion

        public StiPathGeom(object background, StiPenGeom pen, List<StiSegmentGeom> geoms, object rect, StiInteractionDataGeom interaction, int elementIndex)
        {
            this.Background = background;
            this.Pen = pen;
            this.Geoms = geoms;
            this.Rect = rect;
            this.Interaction = interaction;
            this.ElementIndex = elementIndex;
        }

        public StiPathGeom(object background, StiPenGeom pen, List<StiSegmentGeom> geoms, object rect, StiInteractionDataGeom interaction, int elementIndex, string toolTip)
        {
            this.Background = background;
            this.Pen = pen;
            this.Geoms = geoms;
            this.Rect = rect;
            this.Interaction = interaction;
            this.ElementIndex = elementIndex;
            this.ToolTip = toolTip;
        }
    }
}