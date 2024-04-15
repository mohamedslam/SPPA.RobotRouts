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

using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows picture box control for displaying an image.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiPictureBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiPictureBoxControl.gif")]
	[StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiPictureBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiPictureBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiPictureBoxControl : 
		StiReportControl,
		IStiPictureBoxControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiPictureBoxControl
            jObject.AddPropertyEnum("SizeMode", SizeMode, PictureBoxSizeMode.Normal);
            jObject.AddPropertyColor("TransparentColor", TransparentColor, Color.Transparent);
            jObject.AddPropertyEnum("BorderStyle", BorderStyle, BorderStyle.None);
            jObject.Add("Image", StiImageConverter.ImageToString(Image));

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "SizeMode":
                        this.sizeMode = property.DeserializeEnum<PictureBoxSizeMode>();
                        break;

                    case "TransparentColor":
                        this.transparentColor = property.DeserializeColor();
                        break;

                    case "BorderStyle":
                        this.borderStyle = property.DeserializeEnum<BorderStyle>(); 
                        break;

                    case "Image":
						this.image = property.DeserializeImage();
                        break;
                }
            }
        }
        #endregion

		#region StiComponent override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiPictureBoxControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.PictureBoxControl;
			}
		}

        public override StiToolboxCategory ToolboxCategory
        {
            get
            {
                return StiToolboxCategory.Controls;
            }
        }


		/// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName
		{
			get 
			{
				return StiLocalization.Get("Dialogs", "StiPictureBoxControl");
			}
		}
		#endregion
        
		#region Controls Property
		private PictureBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public PictureBox Control
		{
			get
			{
				return control;
			}
			set
			{
				control = value;
			}
		}


		/// <summary>
		/// Gets or sets the width of the control.
		/// </summary>
		[StiCategory("Position")]
		public override double Width
		{
			get
			{
				if ((SizeMode == PictureBoxSizeMode.AutoSize) && (image != null))
				{
					return image.Width + GetBorderWidth() * 2;
				}
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}


		/// <summary>
		/// Gets or sets the height of the control.
		/// </summary>
		[StiCategory("Position")]
		public override double Height
		{
			get
			{
                if ((SizeMode == PictureBoxSizeMode.AutoSize) && (image != null))
				{
					return image.Height + GetBorderHeight() * 2;
				}
				return base.Height;
			}
			set
			{
				base.Height = value;
			}
		}



		private PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;
		/// <summary>
		/// Indicates how the image is displayed.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Browsable(true)]
		[DefaultValue(PictureBoxSizeMode.Normal)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Indicates how the image is displayed.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogSizeMode)]
        [StiPropertyLevel(StiLevel.Standard)]
		public PictureBoxSizeMode SizeMode
		{
			get
			{
				return sizeMode;
			}
			set
			{				
				sizeMode = value;
				UpdateReportControl("SizeMode");
			}
		}


		private Color transparentColor = Color.Transparent;
		/// <summary>
		/// Gets or sets the transparent color for the image.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("Appearance")]
		[Description("Gets or sets the transparent color for the image.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogTransparentColor)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual Color TransparentColor
		{
			get 
			{
				return transparentColor;
			}
			set 
			{
				transparentColor = value;
				MakeTransparent();
				UpdateReportControl("TransparentColor");
			}
		}



		private BorderStyle borderStyle = BorderStyle.None;
		/// <summary>
		/// Indicates the border style for the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Browsable(true)]
		[DefaultValue(BorderStyle.None)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Indicates the border style for the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBorderStyle)]
        [StiPropertyLevel(StiLevel.Professional)]
		public BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{				
				borderStyle = value;
				UpdateReportControl("BorderStyle");
			}
		}


		private Image image = null;
		/// <summary>
		/// Gets or sets the image that the PictureBox displays.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets the image that the PictureBox displays.")]
		[Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.DialogImage)]
        [StiPropertyLevel(StiLevel.Basic)]
		public Image Image
		{
			get 
			{
				return image;
			}
			set 
			{
				image = value;
				MakeTransparent();
				UpdateReportControl("Image");
			}
		}
		#endregion

        #region Report Control Off
        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }
        #endregion

		#region Report Control Override
		/// <summary>
		/// Gets default event for this report control.
		/// </summary>
		/// <returns>Default event.</returns>
		public override StiEvent GetDefaultEvent()
		{
			return this.ClickEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiPictureBoxControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Gets or sets the default client area of a component.
		/// </summary>
		[Browsable(false)]
		public override RectangleD DefaultClientRectangle
		{
			get
			{
				return new RectangleD(0, 0, 96, 96);
			}
		}

		private void MakeTransparent()
		{
			Bitmap bmp = Image as Bitmap;
			if (bmp != null && TransparentColor != Color.Transparent)bmp.MakeTransparent(TransparentColor);
		}

		public int GetBorderWidth()
		{
			if (BorderStyle == BorderStyle.Fixed3D)return (int)SystemInformation.Border3DSize.Width;
			if (BorderStyle == BorderStyle.FixedSingle)return 1;
			return 0;
		}

        public int GetBorderHeight()
		{
			if (BorderStyle == BorderStyle.Fixed3D)return (int)SystemInformation.Border3DSize.Height;
			if (BorderStyle == BorderStyle.FixedSingle)return 1;
			return 0;
		}

		/// <summary>
		/// Creates a new StiPictureBoxControl.
		/// </summary>
		public StiPictureBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiPictureBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiPictureBoxControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;			
		}
		#endregion
	}
}