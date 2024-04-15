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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;

namespace Stimulsoft.Base.Context
{
    public class StiEllipseAnimationGeom : StiAnimationGeom
    {
        #region IStiJsonReportObject.Override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            if (Background != null) jObject.Add(new JProperty("Background", SaveBrushToJsonObject(Background, mode)));
            if (BorderPen != null) jObject.Add(new JProperty("BorderPen", BorderPen.SaveToJsonObject(mode)));
            jObject.Add(new JProperty("Rect", SaveRectToJsonObject(Rect)));

            if (Tag is string) jObject.Add(new JProperty("Tag", Tag as string));    // !!!

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
        }
        #endregion

        #region Properties
        public StiBrush WpfBackColor { get; set; }

        public object Background { get; set; }

        public StiPenGeom BorderPen { get; set; }

        public object Rect { get; set; }

        public string ToolTip { get; set; }

        public object Tag { get; set; }
        #endregion

        #region Properties.Override
        public override StiGeomType Type => StiGeomType.AnimationEllipse;
        #endregion

        public StiEllipseAnimationGeom(StiBrush wpfBackColor, object background, StiPenGeom borderPen, object rect, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction)
            : base(animation, interaction)
        {
            this.WpfBackColor = wpfBackColor;
            this.Background = background;
            this.BorderPen = borderPen;
            this.Rect = rect;
            this.ToolTip = toolTip;
            this.Tag = tag;
        }

        public StiEllipseAnimationGeom(object background, StiPenGeom borderPen, object rect, string toolTip, object tag, StiAnimation animation, StiInteractionDataGeom interaction)
            : base(animation, interaction)
        {
            this.Background = background;
            this.BorderPen = borderPen;
            this.Rect = rect;
            this.ToolTip = toolTip;
            this.Tag = tag;
        }
    }
}
