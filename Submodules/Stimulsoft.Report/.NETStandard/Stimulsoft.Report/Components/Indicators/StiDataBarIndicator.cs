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

using System.Drawing;
using System.ComponentModel;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base;

namespace Stimulsoft.Report.Components
{
    [StiGdiIndicatorTypePainter(typeof(Stimulsoft.Report.Painters.StiDataBarGdiIndicatorTypePainter))]
    [StiWpfIndicatorTypePainter("Stimulsoft.Report.Painters.StiDataBarWpfIndicatorTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiDataBarIndicator : 
        StiIndicator,
        IStiDataBarIndicator
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataBarIndicator
            jObject.AddPropertyEnum("BrushType", BrushType, StiBrushType.Gradient);
            jObject.AddPropertyColor("PositiveColor", PositiveColor, Color.Green);
            jObject.AddPropertyColor("NegativeColor", NegativeColor, Color.Red);
            jObject.AddPropertyColor("PositiveBorderColor", PositiveBorderColor, Color.DarkGreen);
            jObject.AddPropertyColor("NegativeBorderColor", NegativeBorderColor, Color.DarkRed);
            jObject.AddPropertyBool("ShowBorder", ShowBorder);
            jObject.AddPropertyFloat("Value", Value);
            jObject.AddPropertyFloat("Minimum", Minimum);
            jObject.AddPropertyFloat("Maximum", Maximum, 100f);
            jObject.AddPropertyEnum("Direction", Direction, StiDataBarDirection.Default);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BrushType":
                        this.BrushType = property.DeserializeEnum<StiBrushType>();
                        break;

                    case "PositiveColor":
                        this.PositiveColor = property.DeserializeColor();
                        break;

                    case "NegativeColor":
                        this.NegativeColor = property.DeserializeColor();
                        break;

                    case "PositiveBorderColor":
                        this.PositiveBorderColor = property.DeserializeColor();
                        break;

                    case "NegativeBorderColor":
                        this.NegativeBorderColor = property.DeserializeColor();
                        break;

                    case "ShowBorder":
                        this.ShowBorder = property.DeserializeBool();
                        break;

                    case "Value":
                        this.Value = property.DeserializeFloat();
                        break;

                    case "Minimum":
                        this.Minimum = property.DeserializeFloat();
                        break;

                    case "Maximum":
                        this.Maximum = property.DeserializeFloat();
                        break;

                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiDataBarDirection>();
                        break;
                }
            }
        }
        #endregion

        #region IStiDataBarIndicator
        /// <summary>
        /// Gets or sets value which indicates which type of brush will be used for Data Bar indicator drawing.
        /// </summary>    
        [StiSerializable]
        [DefaultValue(StiBrushType.Gradient)]
        public StiBrushType BrushType { get; set; } = StiBrushType.Gradient;

        /// <summary>
        /// Gets or sets a color of positive values for data bar indicator.
        /// </summary>
        [StiSerializable]
        public Color PositiveColor { get; set; } = Color.Green;

        /// <summary>
        /// Gets or sets a color of negative values for data bar indicator.
        /// </summary>
        [StiSerializable]
        public Color NegativeColor { get; set; } = Color.Red;

        /// <summary>
        /// Gets or sets a border color of positive values for Data Bar indicator.
        /// </summary>
        [StiSerializable]
        public Color PositiveBorderColor { get; set; } = Color.DarkGreen;

        /// <summary>
        /// Gets or sets a border color of negative values for Data Bar indicator.
        /// </summary>
        [StiSerializable]
        public Color NegativeBorderColor { get; set; } = Color.DarkRed;

        /// <summary>
        /// Gets or sets value which indicates that border is drawing.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        public bool ShowBorder { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets value from maximum or minimum amount.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets minimum amount.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0f)]
        public float Minimum { get; set; }

        /// <summary>
        /// Gets or sets minimum amount.
        /// </summary>
        [StiSerializable]
        [DefaultValue(100f)]
        public float Maximum { get; set; } = 100f;

        /// <summary>
        /// Gets or sets value which direction data bar will be filled by brush, from left to right or from right to left.
        /// </summary>        
        [StiSerializable]
        [DefaultValue(StiDataBarDirection.Default)]
        public StiDataBarDirection Direction { get; set; } = StiDataBarDirection.Default;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new object of the type StiDataBarIndicator.
		/// </summary>
        public StiDataBarIndicator()
        {
        }

        /// <summary>
        /// Creates a new object of the type StiDataBarIndicator.
		/// </summary>
        public StiDataBarIndicator(StiBrushType brushType, Color positiveColor, Color negativeColor,
            bool showBorder, Color positiveBorderColor, Color negativeBorderColor, 
            StiDataBarDirection direction,
            float value, float minimum, float maximum)
		{
            this.BrushType = brushType;
            this.PositiveColor = positiveColor;
            this.NegativeColor = negativeColor;
            this.ShowBorder = showBorder;
            this.PositiveBorderColor = positiveBorderColor;
            this.NegativeBorderColor = negativeBorderColor;
            this.Direction = direction;
            this.Value = value;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }
        #endregion
    }
}