#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports 									            }
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Drawing;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Context.Animation;

namespace Stimulsoft.Base.Context
{
    public class StiLabelAnimationGeom : StiAnimationGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.Add(new JProperty("Text", Text));
            jObject.Add(new JProperty("Font", Font.SaveToJsonObject(mode)));
            jObject.Add(new JProperty("Rectangle", SaveRectangleToJsonObject(Rectangle)));
            jObject.Add(new JProperty("Angle", Angle));
            jObject.Add(new JProperty("DrawBorder", DrawBorder));
            if (TextBrush != null) jObject.Add(new JProperty("TextBrush", SaveBrushToJsonObject(TextBrush, mode)));
            if (LabelBrush != null) jObject.Add(new JProperty("LabelBrush", SaveBrushToJsonObject(LabelBrush, mode)));
            if (PenBorder != null) jObject.Add(new JProperty("PenBorder", PenBorder.SaveToJsonObject(mode)));
            if (StringFormat != null) jObject.Add(new JProperty("StringFormat", StringFormat.SaveToJsonObject(mode)));
            if (RotationMode != null) jObject.Add(new JProperty("RotationMode", RotationMode.Value.ToString()));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public string Text { get; set; }

        public StiFontGeom Font { get; set; }

        public object TextBrush { get; set; }

        public object LabelBrush { get; set; }

        public StiPenGeom PenBorder { get; set; }

        public Rectangle Rectangle { get; set; }

        public StiStringFormatGeom StringFormat { get; set; }

        public float Angle { get; set; }

        public StiRotationMode? RotationMode { get; set; }

        public bool DrawBorder { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.AnimationLabel;
        #endregion

        public StiLabelAnimationGeom(string text, StiFontGeom font, object textBrush, object LabelBrush, StiPenGeom penBorder, Rectangle rect, StiStringFormatGeom sf, StiRotationMode rotationMode,
            float angle, bool drawBorder, StiAnimation animation) : base(animation)
        {
            this.Text = text;
            this.Font = font;
            this.TextBrush = textBrush;
            this.LabelBrush = LabelBrush;
            this.PenBorder = penBorder;
            this.Rectangle = rect;
            this.StringFormat = sf;
            this.RotationMode = rotationMode;
            this.Angle = angle;
            this.DrawBorder = drawBorder;
        }
    }
}
