#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiWaterfallConnectorLine :
        IStiWaterfallConnectorLine,
        IStiSerializeToCodeAsClass
    {
        #region Properties
        [StiSerializable]
        [TypeConverter(typeof(StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        public Color LineColor { get; set; } = Color.LightGray;

        [StiSerializable]
        [DefaultValue(StiPenStyle.Solid)]
        public StiPenStyle LineStyle { get; set; } = StiPenStyle.Solid;


        [DefaultValue(1f)]
        [StiSerializable]
        public float LineWidth { get; set; } = 1f;

        [StiSerializable]
        [DefaultValue(true)]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool Visible { get; set; } = true;
        #endregion

        #region Methods
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyColor("LineColor", LineColor, Color.LightGray);
            jObject.AddPropertyFloat("LineWidth", LineWidth, 1f);
            jObject.AddPropertyEnum("LineStyle", LineStyle, StiPenStyle.Solid);
            jObject.AddPropertyBool("ShowShadow", Visible);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "LineColor":
                        this.LineColor = property.DeserializeColor();
                        break;

                    case "LineWidth":
                        this.LineWidth = property.DeserializeFloat();
                        break;

                    case "LineStyle":
                        this.LineStyle = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Visible":
                        this.Visible = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        public StiWaterfallConnectorLine()
        {
        }

        [StiUniversalConstructor("Line")]
        public StiWaterfallConnectorLine(Color lineColor, StiPenStyle lineStyle, float lineWidth, bool visible)
        {
            this.LineColor = lineColor;
            this.LineStyle = lineStyle;
            this.LineWidth = lineWidth;
            this.Visible = visible;
        }
    }
}
