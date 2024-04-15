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

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
	/// <summary>
	/// Represents a Windows text box control.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiTextBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiTextBoxControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiTextBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiTextBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiTextBoxControl : 
		StiReportControl,
		IStiTextBoxControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiTextBoxControl
            jObject.AddPropertyStringNullOrEmpty("TextBinding", TextBinding);
            jObject.AddPropertyBool("Multiline", Multiline);
            jObject.AddPropertyBool("WordWrap", WordWrap);
            jObject.AddPropertyBool("AcceptsTab", AcceptsTab);
            jObject.AddPropertyBool("AcceptsReturn", AcceptsReturn);
            jObject.AddPropertyInt("MaxLength", MaxLength, 32767);
            jObject.AddPropertyStringNullOrEmpty("PasswordChar", PasswordChar.ToString());
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
                    case "TextBinding":
                        this.textBinding = property.DeserializeString();
                        break;

                    case "Multiline":
                        this.multiline = property.DeserializeBool();
                        break;

                    case "WordWrap":
                        this.wordWrap = property.DeserializeBool();
                        break;

                    case "AcceptsTab":
                        this.acceptsTab = property.DeserializeBool();
                        break;

                    case "AcceptsReturn":
                        this.acceptsReturn = property.DeserializeBool();
                        break;

                    case "MaxLength":
                        this.maxLength = property.DeserializeInt();
                        break;

                    case "PasswordChar":
                        {
                            var pass = property.DeserializeString();
                            this.passwordChar = pass.ToCharArray()[0];
                        }
                        break;

                    case "Text":
                        this.text = property.DeserializeString();
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
                return StiComponentId.StiTextBoxControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.TextBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiTextBoxControl");
			}
		}
		#endregion

		#region Controls Property
		private TextBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public TextBox Control
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


		private string textBinding = "";
		/// <summary>
		/// Gets the data bindings for the text.
		/// </summary>
		[StiSerializable]
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[DefaultValue("")]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[Description("Gets the data bindings for the text.")]
        [StiOrder(StiPropertyOrder.DialogTextBinding)]
		public string TextBinding
		{
			get
			{
				return textBinding;
			}
			set
			{
				textBinding = value;
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
				if (!Multiline)return 20;
				return base.Height;
			}
			set
			{
				if (!Multiline)base.Height = 20;
				base.Height = value;
			}
		}


		private bool multiline = false;
		/// <summary>
		/// Gets or sets a value indicating whether this is a multiline text box control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether this is a multiline text box control.")]
        [StiOrder(StiPropertyOrder.DialogMultiline)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool Multiline
		{
			get
			{
				return multiline;
			}
			set
			{
				multiline = value;
				UpdateReportControl("Multiline");
			}
		}


		private bool wordWrap = false;
		/// <summary>
		/// Indicates whether a multiline text box control automatically wraps words to the beginning of the next line when necessary.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Indicates whether a multiline text box control automatically wraps words to the beginning of the next line when necessary.")]
        [StiOrder(StiPropertyOrder.DialogWordWrap)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool WordWrap
		{
			get
			{
				return wordWrap;
			}
			set
			{
				wordWrap = value;
				UpdateReportControl("WordWrap");
			}
		}


		private bool acceptsTab = false;
		/// <summary>
		/// Gets or sets a value indicating whether pressing the TAB key in a multiline text box control types a TAB character in the control instead of moving the focus to the next control in the tab order.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether pressing the TAB key in a multiline text box control types a TAB character in the control instead of moving the focus to the next control in the tab order.")]
        [StiOrder(StiPropertyOrder.DialogAcceptsTab)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool AcceptsTab
		{
			get
			{
				return acceptsTab;
			}
			set
			{
				acceptsTab = value;
				UpdateReportControl("AcceptsTab");
			}
		}


		private bool acceptsReturn = false;
		/// <summary>
		/// Gets or sets a value indicating whether pressing ENTER in a multiline TextBox control creates a new line of text in the control or activates the default button for the form.
		/// </summary>
		[StiSerializable]
		[DefaultValue(false)]
		[StiCategory("Behavior")]
		[TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[Description("Gets or sets a value indicating whether pressing ENTER in a multiline TextBox control creates a new line of text in the control or activates the default button for the form.")]
        [StiOrder(StiPropertyOrder.DialogAcceptsReturn)]
        [StiPropertyLevel(StiLevel.Standard)]
		public virtual bool AcceptsReturn
		{
			get
			{
				return acceptsReturn;
			}
			set
			{
				acceptsReturn = value;
				UpdateReportControl("AcceptsReturn");
			}
		}


		private int maxLength = 32767;
		/// <summary>
		/// Gets or sets the maximum number of characters the user can type into the text box control.
		/// </summary>
		[StiSerializable]
		[DefaultValue(32767)]
		[StiCategory("Appearance")]
		[Description("Gets or sets the maximum number of characters the user can type into the text box control.")]
        [StiOrder(StiPropertyOrder.DialogMaxLength)]
        [StiPropertyLevel(StiLevel.Professional)]
		public int MaxLength
		{
			get 
			{
				return maxLength;
			}
			set 
			{
				maxLength = value;
				UpdateReportControl("MaxLength");
			}
		}


		private char passwordChar = (char)0;
		/// <summary>
		/// Gets or sets the character used to mask characters of a password in a single-line TextBox control.
		/// </summary>
		[StiSerializable]
		[DefaultValue((char)0)]
		[StiCategory("Behavior")]
		[Description("Gets or sets the character used to mask characters of a password in a single-line TextBox control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogPasswordChar)]
        [StiPropertyLevel(StiLevel.Professional)]
		public char PasswordChar
		{
			get 
			{
				return passwordChar;
			}
			set 
			{
				passwordChar = value;
				UpdateReportControl("PasswordChar");
			}
		}


		private string text = "TextBox";
		/// <summary>
		/// Gets or sets the current text in the text box.
		/// </summary>
		[StiSerializable]
		[StiCategory("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the current text in the text box.")]
		[Editor("Stimulsoft.Report.Components.Design.StiExpressionEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
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
            return new StiTextBoxControl();
        }
        #endregion

		#region this
		/// <summary>
		/// Creates a new StiTextBoxControl.
		/// </summary>
		public StiTextBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiTextBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiTextBoxControl(RectangleD rect) : base(rect)
		{
			base.BackColor = SystemColors.Window;
			PlaceOnToolbox = true;
			Text = this.LocalizedName;
		}
		#endregion
	}
}