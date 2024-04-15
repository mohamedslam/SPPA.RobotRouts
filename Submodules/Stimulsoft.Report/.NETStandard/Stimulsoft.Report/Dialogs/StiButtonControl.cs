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
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Drawing;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Dialogs
{
    /// <summary>
    /// Represents a Windows button control.
    /// </summary>
    [StiToolbox(true)]
	[StiServiceBitmap(typeof(StiButtonControl), "Stimulsoft.Report.Dialogs.Bmp.StiButtonControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiButtonControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiButtonControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiButtonControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("BackColor");

            // StiButtonControl
            jObject.AddPropertyImage("Image", Image);
            jObject.AddPropertyBool("Default", Default);
            jObject.AddPropertyEnum("DialogResult", DialogResult, DialogResult.None);
            jObject.AddPropertyBool("Cancel", Cancel);
            jObject.AddPropertyEnum("ImageAlign", ImageAlign, ContentAlignment.MiddleCenter);
            jObject.AddPropertyEnum("TextAlign", TextAlign, ContentAlignment.MiddleCenter);
            jObject.AddPropertyString("Text", Text);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Image":
						this.Image = property.DeserializeImage();
                        break;

                    case "Default":
                        this.defaultValue = property.DeserializeBool();
                        break;

                    case "DialogResult":
                        this.dialogResult = property.DeserializeEnum<DialogResult>();
                        break;

                    case "Cancel":
                        this.cancel = property.DeserializeBool();
                        break;

                    case "ImageAlign":
                        this.ImageAlign = property.DeserializeEnum<ContentAlignment>(); 
                        break;

                    case "TextAlign":
                        this.TextAlign = property.DeserializeEnum<ContentAlignment>(); 
                        break;

                    case "Text":
                        this.text = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region StiComponent override
        public override StiComponentId ComponentId => StiComponentId.StiButtonControl;

        /// <summary>
        /// Gets value to sort a position in the toolbox.
        /// </summary>
        public override int ToolboxPosition => (int)StiReportControlToolboxPosition.ButtonControl;

        public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Controls;

        /// <summary>
        /// Gets a localized component name.
        /// </summary>
        public override string LocalizedName => StiLocalization.Get("Dialogs", "StiButtonControl");
        #endregion

        #region Off
        [StiNonSerialized]
		[Browsable(false)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
        #endregion

        #region Controls Property
        /// <summary>
        /// Gets ot sets Windows Forms Control for this Report Control.
        /// </summary>
        [Browsable(false)]
        public Button Control { get; set; }

        private Image image = null;
		/// <summary>
		/// Gets or sets the image that is displayed on a button control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Description("Gets or sets the image that is displayed on a button control.")]
		[Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [StiOrder(StiPropertyOrder.DialogImage)]
        [StiPropertyLevel(StiLevel.Standard)]
		public Image Image
		{
			get 
			{
				return image;
			}
			set 
			{
				image = value;

                if (value != null && value is Bitmap)				
					StiImageUtils.MakeImageBackgroundAlphaZero((Bitmap)value);				
				
				UpdateReportControl("Image");
			}
		}

		private bool defaultValue = false;
		/// <summary>
		/// Gets or sets the value which indicates which button is clicked when the user presses the ENTER key.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the value which indicates which button is clicked when the user presses the ENTER key.")]
        [StiOrder(StiPropertyOrder.DialogDefault)]
        [StiPropertyLevel(StiLevel.Basic)]
		public bool Default
		{
			get 
			{
				return defaultValue;
			}
			set 
			{
				defaultValue = value;
				UpdateReportControl("Default");
			}
		}

		private DialogResult dialogResult = DialogResult.None;
		/// <summary>
		/// Gets or sets a value that is returned to the parent form when the button is clicked.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[DefaultValue(DialogResult.None)]
		[StiCategory("Behavior")]
		[Description("Gets or sets a value that is returned to the parent form when the button is clicked.")]
        [StiOrder(StiPropertyOrder.DialogDialogResult)]
        [StiPropertyLevel(StiLevel.Basic)]
		public DialogResult DialogResult
		{
			get 
			{
				return dialogResult;
			}
			set 
			{
				dialogResult = value;
				UpdateReportControl("DialogResult");
			}
		}

		private bool cancel = false;
		/// <summary>
		/// Gets or sets the value which indicates which button is clicked when the user presses the ESCAPE key.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the value which indicates which button is clicked when the user presses the ESCAPE key.")]
        [StiOrder(StiPropertyOrder.DialogCancel)]
        [StiPropertyLevel(StiLevel.Basic)]
		public bool Cancel
		{
			get 
			{
				return cancel;
			}
			set 
			{
				cancel = value;
				UpdateReportControl("Cancel");
			}
		}

        /// <summary>
        /// Gets or sets the alignment of the image on the button control.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("Behavior")]
        [Description("Gets or sets the alignment of the image on the button control.")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogImageAlign)]
        [StiPropertyLevel(StiLevel.Standard)]
        public ContentAlignment ImageAlign { get; set; } = ContentAlignment.MiddleCenter;

        /// <summary>
        /// Gets or sets the alignment of the text on the button control.
        /// </summary>
        [StiSerializable]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("Behavior")]
        [Description("Gets or sets the alignment of the text on the button control.")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        [StiOrder(StiPropertyOrder.DialogTextAlign)]
        [StiPropertyLevel(StiLevel.Standard)]
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;

        private string text = "Button";
		/// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the text associated with this control.")]
        [StiOrder(StiPropertyOrder.DialogText)]
        [StiPropertyLevel(StiLevel.Basic)]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{				
				text = value;
				UpdateReportControl("Text");
			}
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiButtonControl();
        }
        #endregion

		/// <summary>
		/// Creates a new StiButtonControl.
		/// </summary>
		public StiButtonControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiButtonControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiButtonControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;	
			Text = this.LocalizedName;
		}
	}
}
