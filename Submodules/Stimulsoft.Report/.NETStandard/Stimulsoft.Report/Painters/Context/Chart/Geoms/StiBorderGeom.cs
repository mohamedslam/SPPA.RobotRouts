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
    public class StiBorderGeom : StiGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            if (Background != null) jObject.Add(new JProperty("Background", SaveBrushToJsonObject(Background, mode)));
            if (BorderPen != null) jObject.Add(new JProperty("BorderPen", BorderPen.SaveToJsonObject(mode)));
            jObject.Add(new JProperty("Rect", SaveRectToJsonObject(Rect)));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public object Background { get; set; }

        public object BackgroundMouseOver { get; set; }

        public StiPenGeom BorderPen { get; set; }

        public object Rect { get; set; }

        public StiInteractionDataGeom Interaction { get; set; }

        public int ElementIndex { get; set; }

        public float Angle { get; set; }

        public StiRotationMode RotationMode { get; set; }

        public StiCornerRadius CornerRadius { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Border;
        #endregion

        public StiBorderGeom(object background, object backgroundMouseOver, StiPenGeom borderPen, object rect, StiInteractionDataGeom interaction, int elementIndex, float angle, StiRotationMode rotationMode) :
            this(background, backgroundMouseOver, borderPen, rect, interaction, elementIndex)
        {
            this.Angle = angle;
            this.RotationMode = rotationMode;
        }

        public StiBorderGeom(object background, object backgroundMouseOver, StiPenGeom borderPen, object rect, StiInteractionDataGeom interaction, int elementIndex) :
            this(background, backgroundMouseOver, borderPen, rect, null, interaction, elementIndex)
        {
        }

        public StiBorderGeom(object background, object backgroundMouseOver, StiPenGeom borderPen, object rect, StiCornerRadius cornerRadius, StiInteractionDataGeom interaction, int elementIndex)
        {
            this.Background = background;
            this.BackgroundMouseOver = backgroundMouseOver;
            this.BorderPen = borderPen;
            this.Rect = rect;
            this.CornerRadius = cornerRadius;
            this.Interaction = interaction;
            this.ElementIndex = elementIndex;
        }
    }
}