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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Painters;
using Stimulsoft.Base.Design;
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
	/// Represents a Windows group box.
	/// </summary>
	[StiToolbox(true)]
	[StiServiceBitmap(typeof(StiGroupBoxControl), "Stimulsoft.Report.Dialogs.Bmp.StiGroupBoxControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiGroupBoxControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiGroupBoxControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiGroupBoxControl : StiReportControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiGroupBoxControl
            jObject.AddPropertyStringNullOrEmpty("TextBinding", TextBinding);
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
                return StiComponentId.StiGroupBoxControl;
            }
        }

		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				return (int)StiReportControlToolboxPosition.GroupBoxControl;
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
				return StiLocalization.Get("Dialogs", "StiGroupBoxControl");
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

		#region Controls Property
		private GroupBox control = null;
		/// <summary>
		/// Gets ot sets Windows Forms Control for this Report Control.
		/// </summary>
		[Browsable(false)]
		public GroupBox Control
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
		[Editor("Stimulsoft.Report.Components.Design.StiDataColumnEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiSerializable]
		[Browsable(false)]
		[Bindable(BindableSupport.Yes)]
		[DefaultValue("")]
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


		private string text = "GroupBox";
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
            return new StiGroupBoxControl();
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

		[Browsable(false)]
		public override bool IsReportContainer
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Creates a new StiGroupBoxControl.
		/// </summary>
		public StiGroupBoxControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
		/// Creates a new StiGroupBoxControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
		public StiGroupBoxControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = true;
			Text = this.LocalizedName;
		}
		#endregion
	}
}