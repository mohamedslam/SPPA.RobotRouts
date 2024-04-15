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
using System.Xml;
using System.Drawing.Design;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Controls;
using Stimulsoft.Base.Json.Linq;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows text box control.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiTextBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiRichTextBoxControl.gif")]
	[StiDesigner("Stimulsoft.Report.Dialogs.Design.StiRichTextBoxControlDesigner, Stimulsoft.Report.Design, " + StiVersion.VersionInfo)]
    [StiWpfDesigner("Stimulsoft.Report.WpfDesign.StiWpfRichTextBoxControlDesigner, Stimulsoft.Report.WpfDesign, " + StiVersion.VersionInfo)]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiRichTextBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiRichTextBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiRichTextBoxControl : 
		StiReportControl,
		IStiRichTextBoxControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRichTextBoxControl
            jObject.AddPropertyStringNullOrEmpty("Text", Text);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Text":
                        this.text = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

		#region Paint
		private Image image = null;
		[Browsable(false)]
		public Image Image
		{
			get
			{
				return image;
			}
		}	


		public void RenderMetafile()
		{
			if (Page != null && Report != null)
			{
				bool fail = true;
				int failCount = 0;

				while (fail && failCount < 100)
				{
					fail = false;
					try
					{
                        using (StiRichTextBox richTextBox = new StiRichTextBox(this.BackColor == Color.Transparent))
						{
							string text = GetPreparedText(richTextBox);

							RectangleD rect = GetPaintRectangle(true, false);

							using (Graphics graph = Graphics.FromHwnd(richTextBox.Handle))
							{
								IntPtr ptrGraph = graph.GetHdc();
					
								Metafile newImage = new Metafile(ptrGraph, EmfType.EmfPlusOnly);
								graph.ReleaseHdc(ptrGraph);

								if (BackColor != Color.Transparent)
								    richTextBox.BackColor = BackColor;
					
								if (text.Length > 0)
								{
									using (Graphics imageGraph = Graphics.FromImage(newImage))
									{
										int endChar = 0;
										imageGraph.PageUnit = GraphicsUnit.Pixel;
										StiRichTextHelper.FormatRange(StiRtfFormatType.DrawRtf, richTextBox,
											new Rectangle(0, 0, (int)rect.Width, (int)rect.Height), imageGraph, 0, text.Length, out endChar);
									}
								}

								if (Image != null)Image.Dispose();
								image = newImage;
							}
						}
					}
					catch
					{
						fail = true;
						failCount ++;
					}
				}
			}			
		}


		private string GetPreparedText(RichTextBox richTextBox)
		{
			richTextBox.Rtf = StiRichText.UnpackRtf(XmlConvert.DecodeName(this.Text));
			richTextBox.WordWrap = true;
				
			string text = richTextBox.Rtf;
			if (richTextBox.Text == string.Empty)richTextBox.Text = " ";
			return text;
		}
		#endregion

		#region StiComponent override
        public override StiComponentId ComponentId
        {
            get
            {
                return StiComponentId.StiRichTextBoxControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.RichTextBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiRichTextBoxControl");
			}
		}
		#endregion

		#region Controls Property
		private RichTextBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public RichTextBox Control
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

	
		private string text = "";
		/// <summary>
		/// Gets or sets the current text in the rich text box.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Description("Gets or sets the current text in the rich text box.")]
		[Editor("Stimulsoft.Report.Components.Design.StiRichTextExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[TypeConverter(typeof(Stimulsoft.Report.Components.Design.StiRichTextExpressionConverter))]
		[DefaultValue("")]
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

		/// <summary>
		/// Gets or sets text in rtf format.
		/// </summary>
		[Browsable(false)]
		public string RtfText
		{
			get
			{
				return StiRichText.UnpackRtf(XmlConvert.DecodeName(Text));
			}
			set
			{
				Text = XmlConvert.EncodeName(StiRichText.PackRtf(value));
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
			return this.EnterEvent;
		}
		#endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiRichTextBoxControl();
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

		/// <summary>
		/// Creates a new StiRichTextBoxControl.
		/// </summary>
		public StiRichTextBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiRichTextBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiRichTextBoxControl(RectangleD rect) : base(rect)
		{
			base.BackColor = SystemColors.Window;
			PlaceOnToolbox = true;
		}
		#endregion
	}
}