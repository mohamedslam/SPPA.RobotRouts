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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Chart
{
    [TypeConverter(typeof(StiUniversalConverter))]
    public class StiChartTableDataCells :
        IStiChartTableDataCells,
        IStiFont
    {   
        #region IStiJsonReportObject.override
        public JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyColor("TextColor", TextColor, Color.DarkGray);
            jObject.AddPropertyFontArial8("Font", Font);
            jObject.AddPropertyFloat("ShrinkFontToFitMinimumSize", ShrinkFontToFitMinimumSize, 1);
            jObject.AddPropertyBool("ShrinkFontToFit", ShrinkFontToFit);

            return jObject;
        }

        public void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Font":
                        this.Font = property.DeserializeFont(Font);
                        break;

                    case "TextColor":
                        this.TextColor = property.DeserializeColor();
                        break;

                    case "ShrinkFontToFitMinimumSize":
                        this.ShrinkFontToFitMinimumSize = property.DeserializeFloat();
                        break;

                    case "ShrinkFontToFit":
                        this.ShrinkFontToFit = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region ICloneable override
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            var dataCells = this.MemberwiseClone() as IStiChartTableDataCells;            
            dataCells.Font = this.Font.Clone() as Font;

            return dataCells;
        }
        #endregion

        #region IStiDefault
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                return
                    !ShrinkFontToFit
                    && ShrinkFontToFitMinimumSize == 1f
                    && !ShouldSerializeFont()
                    && !ShouldSerializeTextColor();
            }
        }

        [Browsable(false)]
        public bool IsDefaultExceptTextColor
        {
            get
            {
                return
                    !ShrinkFontToFit
                    && ShrinkFontToFitMinimumSize == 1f
                    && !ShouldSerializeFont();
            }
        }
        #endregion

        #region Properties
        [StiSerializable]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Professional)]
        [DefaultValue(false)]
        public bool ShrinkFontToFit { get; set; }

        [StiSerializable]
        [DefaultValue(1f)]
        [Description("Gets or sets value that indicates minimum font size for ShrinkFontToFit operation.")]
        [StiPropertyLevel(StiLevel.Professional)]
        public float ShrinkFontToFitMinimumSize { get; set; } = 1f;

        /// <summary>
        /// Gets or sets font which will be used to draw chart table header.
        /// </summary>
        [StiSerializable]
        [StiOrder(StiSeriesLabelsPropertyOrder.Font)]
        [Description("Gets or sets font which will be used to draw chart table cells.")]
        [Editor(StiEditors.Font, typeof(UITypeEditor))]
        public Font Font { get; set; } = new Font("Arial", 8);

        private bool ShouldSerializeFont()
        {
            return !(Font != null && Font.Name == "Arial" && Font.SizeInPoints == 8 && Font.Style == FontStyle.Regular);
        }

        /// <summary>
        /// Gets or sets text color.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
        [Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [Description("Gets or sets text color.")]
        [StiCategory("Appearance")]
        public Color TextColor { get; set; } = Color.DarkGray;

        private bool ShouldSerializeTextColor()
        {
            return TextColor != Color.DarkGray;
        }
        #endregion

        public StiChartTableDataCells()
        {
        }

        [StiUniversalConstructor("DataCells")]
        public StiChartTableDataCells(
            bool shrinkFontToFit,
            float shrinkFontToFitMinimumSize,
            Font font,
            Color textColor)
        {
            this.Font = font;
            this.ShrinkFontToFit = shrinkFontToFit;
            this.ShrinkFontToFitMinimumSize = shrinkFontToFitMinimumSize;
            this.TextColor = textColor;
        }
    }
}
