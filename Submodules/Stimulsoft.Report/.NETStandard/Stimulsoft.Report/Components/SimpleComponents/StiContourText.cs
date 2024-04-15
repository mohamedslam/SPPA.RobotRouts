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
using System.Drawing.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components
{
	/// <summary>
	/// The class describes the Contour Text.
	/// </summary>
    [StiServiceBitmap(typeof(StiText), "Stimulsoft.Report.Images.Components.StiContourText.png")]
	[StiToolbox(false)]
	[StiDesigner("Stimulsoft.Report.Components.Design.StiTextDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfTextDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiContourTextGdiPainter))]
	[StiContextTool(typeof(IStiCanGrow))]
	[StiContextTool(typeof(IStiCanShrink))]
	[StiContextTool(typeof(IStiText))]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiGrowToHeight))]
	[StiContextTool(typeof(IStiComponentDesigner))]
	public class StiContourText : StiText
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("LinesOfUnderline");

            // StiContourText
            jObject.AddPropertyColor("ContourColor", ContourColor, Color.Black);
            jObject.AddPropertyDouble("Size", Size, 1d);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "ContourColor":
                        this.ContourColor = property.DeserializeColor();
                        break;

                    case "Size":
                        this.Size = property.DeserializeDouble();
                        break;
                }
            }
        }
        #endregion

        #region StiComponent.Properties
        public override StiComponentId ComponentId => StiComponentId.StiContourText;
        #endregion

		#region IStiText override
		[Browsable(false)]
		[StiNonSerialized]
		public override StiPenStyle LinesOfUnderline
		{
			get
			{
				return base.LinesOfUnderline;
			}
			set
			{
			}
		}
		#endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.ContourText;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

        /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiContourText");
        #endregion

		#region Properties
        /// <summary>
		/// Gets or sets a contour color.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable()]
		[TypeConverter(typeof(StiExpressionColorConverter))]
		[Editor(StiEditors.ExpressionColor, typeof(UITypeEditor))]
		[Description("Gets or sets a contour color.")]
		[StiExpressionAllowed]
		public Color ContourColor { get; set; } = Color.Black;

		private bool ShouldSerializeContourColor()
		{
			return ContourColor != Color.Black;
		}

		/// <summary>
		/// Gets or sets a contour size.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable()]
		[DefaultValue(1d)]
		[Description("Gets or sets a contour size.")]
		public double Size { get; set; } = 1d;
        #endregion

        /// <summary>
		/// Creates a new contour text.
		/// </summary>
		public StiContourText() : this(RectangleD.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new contour text.
		/// </summary>
		/// <param name="rect">Rectangle describes size and position of the component.</param>
		public StiContourText(RectangleD rect) : this(rect, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new contour text.
		/// </summary>
		/// <param name="rect">Rectangle describes size and position of the component.</param>
		/// <param name="text">Text.</param>
		public StiContourText(RectangleD rect, string text) : base(rect, text)
		{
			this.TextBrush = new StiSolidBrush(Color.White);
			PlaceOnToolbox = false;
		}
	}
}
