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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components.TextFormats;
using System;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report.Components.Gauge.Primitives
{
    public abstract class StiTickLabelBase : StiTickBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            jObject.AddPropertyStringNullOrEmpty(nameof(TextFormat), TextFormat);
            jObject.AddPropertyBrush(nameof(TextBrush), TextBrush);
            jObject.AddPropertyFontArial10(nameof(Font), Font);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(TextFormat):
                        this.TextFormat = property.DeserializeString();
                        break;

                    case nameof(TextBrush):
                        this.TextBrush = property.DeserializeBrush();
                        break;

                    case nameof(Font):
                        this.Font = property.DeserializeFont(this.Font);
                        break;
                }
            }
        }
        #endregion

        #region ICloneable
        public override object Clone()
        {
            var tickLabel = (StiTickLabelBase)base.Clone();
            tickLabel.TextBrush = (StiBrush)this.TextBrush.Clone();

            return tickLabel;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the format used to convert value to a string.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [StiCategory("TextAdditional")]
        [Description("Gets or sets the format used to convert value to a string.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public string TextFormat { get; set; }

        /// <summary>
        /// Gets or sets the format used to convert value to a string.
        /// </summary>
        [Browsable(false)]
        internal StiFormatService FormatService { get; set; }

        /// <summary>
        /// The brush, which is used to display text.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [Description("The brush, which is used to display text.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public StiBrush TextBrush { get; set; } = new StiSolidBrush(Color.Black);

        /// <summary>
        /// Gets or sets font of component.
        /// </summary>
        [StiSerializable]
        [StiCategory("TextAdditional")]
        [Description("Gets or sets font of component.")]
        [StiPropertyLevel(StiLevel.Basic)]
        public Font Font { get; set; } = new Font("Arial", 10f);
        #endregion

        #region Methods
        protected string GetTextForRender(double value, string format)
        {
            return (string.IsNullOrEmpty(format))
                ? Math.Round(value, 2).ToString()
                : string.Format(format, value);
        }

        protected string GetTextForRender(string value)
        {
            return GetTextForRender(value, this.TextFormat);
        }

        protected string GetTextForRender(string value, string format)
        {
            if (FormatService != null)
            {
                return FormatService.Format(value);
            }
            else
            {
                return (string.IsNullOrEmpty(format))
                    ? value
                    : string.Format(format, value);
            }
        }
        #endregion
    }
}
