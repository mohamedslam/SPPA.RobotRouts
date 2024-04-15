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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiTextGeom : StiAnimationGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Text", Text));
            jObject.Add(new JProperty("Font", Font.SaveToJsonObject(mode)));
            jObject.Add(new JProperty("IsRounded", IsRounded));
            jObject.Add(new JProperty("IsRotatedText", IsRotatedText));
            jObject.Add(new JProperty("Angle", Angle));
            jObject.Add(new JProperty("Antialiasing", Antialiasing));
            jObject.Add(new JProperty("MaximalWidth", MaximalWidth));
            if (Brush != null) jObject.Add(new JProperty("Brush", SaveBrushToJsonObject(Brush, mode)));
            if (StringFormat != null) jObject.Add(new JProperty("StringFormat", StringFormat.SaveToJsonObject(mode)));
            if (RotationMode != null) jObject.Add(new JProperty("RotationMode", RotationMode.Value.ToString()));

            if (Location is PointF) jObject.Add(new JProperty("Location", SavePointFToJsonObject((PointF)Location)));
            if (Location is Rectangle) jObject.Add(new JProperty("Location", SaveRectangleToJsonObject((Rectangle)Location)));
            if (Location is RectangleF) jObject.Add(new JProperty("Location", SaveRectangleFToJsonObject((RectangleF)Location)));
            if (Location is RectangleD) jObject.Add(new JProperty("Location", SaveRectangleDToJsonObject((RectangleD)Location)));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public string Text { get; set; }

        public StiFontGeom Font { get; set; }

        public bool IsRounded { get; set; } = false;  //Special flag which simulate drawing as for Rectangle

        public bool IsRotatedText { get; set; } = false;

        public object Brush { get; set; }

        public object Location { get; set; }

        public StiStringFormatGeom StringFormat { get; set; }

        public float Angle { get; set; }

        public bool Antialiasing { get; set; }

        public int? MaximalWidth { get; set; }

        public StiRotationMode? RotationMode { get; set; }

        public string ToolTip { get; set; }

        public int ElementIndex { get; set; } = -1;
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.Text;
        #endregion

        public StiTextGeom(string text, StiFontGeom font, object brush, object location, StiStringFormatGeom stringFormat, bool isRotatedText, int elementIndex = -1)
            :
            this(text, font, brush, location, stringFormat, 0f, false, null, null, isRotatedText)
        {
            this.ElementIndex = elementIndex;
        }

        public StiTextGeom(string text, StiFontGeom font, object brush, object location, StiStringFormatGeom stringFormat, float angle, bool antialiasing, bool isRotatedText)
            :
            this(text, font, brush, location, stringFormat, angle, antialiasing, null, null, isRotatedText)
        {

        }

        public StiTextGeom(string text, StiFontGeom font, object brush, object location, StiStringFormatGeom stringFormat, float angle, bool antialiasing, StiRotationMode? rotationMode, bool isRotatedText)
            :
            this(text, font, brush, location, stringFormat, angle, antialiasing, null, rotationMode, isRotatedText)
        {
        }

        public StiTextGeom(string text, StiFontGeom font, object brush, object location, StiStringFormatGeom stringFormat, float angle, bool antialiasing, int? maximalWidth, StiRotationMode? rotationMode, bool isRotatedText)
            :
            this(text, font, brush, location, stringFormat, angle, antialiasing, maximalWidth, rotationMode, isRotatedText, null)
        {
        }

        public StiTextGeom(string text, StiFontGeom font, object brush, object location, StiStringFormatGeom stringFormat, float angle,
            bool antialiasing, int? maximalWidth, StiRotationMode? rotationMode, bool isRotatedText, string toolTip, StiAnimation animation = null)
            :base(animation)
        {
            this.IsRotatedText = isRotatedText;
            this.Text = text;
            this.Font = font;
            this.Brush = brush;
            this.Location = location;
            this.StringFormat = stringFormat;
            this.Angle = angle;
            this.Antialiasing = antialiasing;
            this.MaximalWidth = maximalWidth;
            this.RotationMode = rotationMode;
            this.ToolTip = toolTip;
        }
    }
}
