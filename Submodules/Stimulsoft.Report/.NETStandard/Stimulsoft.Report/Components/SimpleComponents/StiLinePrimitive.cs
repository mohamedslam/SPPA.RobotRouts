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
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base.Drawing.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// Describes class that realizes base component for all primitives with lines.
	/// </summary>
	[StiToolbox(false)]
	public abstract class StiLinePrimitive : StiPrimitive
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiLinePrimitive
            jObject.AddPropertyEnum("Style", Style, StiPenStyle.Solid);
            jObject.AddPropertyColor("Color", Color, Color.Black);
            jObject.AddPropertyFloat("Size", Size, 1f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Style":
                        this.Style = property.DeserializeEnum<StiPenStyle>();
                        break;

                    case "Color":
                        this.Color = property.DeserializeColor();
                        break;

                    case "Size":
                        this.Size = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        public override bool InvalidateOnMouseOver => true;

        /// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle => new RectangleD(0, 0, 60, 60);

        /// <summary>
		/// Gets or sets a line style.
		/// </summary>
		[Editor(StiEditors.PenStyle, typeof(UITypeEditor))]
        [StiSerializable]
		[DefaultValue(StiPenStyle.Solid)]
		[StiCategory("Primitive")]
		[StiOrder(StiPropertyOrder.PrimitiveStyle)]
		[TypeConverter(typeof(StiEnumConverter))]
		[Description("Gets or sets a pen style.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public StiPenStyle Style { get; set; } = StiPenStyle.Solid;

        /// <summary>
		/// Gets or sets line color.
		/// </summary>
		[StiCategory("Primitive")]
		[StiOrder(StiPropertyOrder.PrimitiveColor)]
		[StiSerializable]
        [TypeConverter(typeof(StiExpressionColorConverter))]
        [Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
        [Description("Gets or sets line color.")]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiExpressionAllowed]
        public Color Color { get; set; } = Color.Black;

        private bool ShouldSerializeColor()
        {
            return Color != Color.Black;
        }

        /// <summary>
		/// Gets or sets size of the line.
		/// </summary>
		[StiCategory("Primitive")]
		[StiOrder(StiPropertyOrder.PrimitiveSize)]
		[StiSerializable]
		[DefaultValue(1f)]
        [Description("Gets or sets size of the line.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public float Size { get; set; } = 1f;
        #endregion

        /// <summary>
        /// Creates a new StiLinePrimitive.
        /// </summary>
        public StiLinePrimitive() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiLinePrimitive.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiLinePrimitive(RectangleD rect): base(rect)
		{
			PlaceOnToolbox = true;
		}
	}
}