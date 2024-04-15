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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Controls;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Drawing.Design;
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Dialogs
{
    [StiToolbox(true)]
    [StiServiceBitmap(typeof(StiPanelControl), "Stimulsoft.Report.Dialogs.Bmp.StiPanelControl.gif")]
    [StiGdiPainter(typeof(Stimulsoft.Report.Painters.StiCustomControlGdiPainter))]
    [StiWpfPainter("Stimulsoft.Report.Painters.StiPanelControlWpfPainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
    public class StiCustomControl : StiReportControl,
        IStiSerializableCustomControl
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("ForeColor");

            // StiCustomControl
            jObject.AddPropertyEnum("BorderStyle", BorderStyle, StiBorderStyle.None);
            //jObject.Add("Control", Control);      // Anton

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "BorderStyle":
                        this.borderStyle = property.DeserializeEnum<StiBorderStyle>();
                        break;
                }
            }
        }
        #endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition
		{
			get
			{
				//return (int)StiReportControlToolboxPosition.PanelControl;
                return 19;
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
                return "StiCustomControl";//StiLocalization.Get("Dialogs", "StiPanelControl");
			}
		}
		#endregion

        #region Properties override
        public override RectangleD ClientRectangle
        {
            get
            {
                return base.ClientRectangle;
            }
            set
            {
                base.ClientRectangle = value;
                if (control != null && control is Control)
                {
                    ((Control)control).Width = (int)this.Width;
                    ((Control)control).Height = (int)this.Height;
                }
            }
        }
        #endregion

        #region Controls Property
        [StiNonSerialized]
		[Browsable(false)]
		public override Color ForeColor
		{
			get 
			{
				return base.ForeColor;
			}
			set 
			{
				base.ForeColor = value;
			}
		}


		private StiBorderStyle borderStyle = StiBorderStyle.None;
		/// <summary>
		/// Indicates the border style for the control.
		/// </summary>
		[StiSerializable]
		[StiCategory("Appearance")]
		[Browsable(true)]
		[DefaultValue(StiBorderStyle.None)]
		[TypeConverter(typeof(Stimulsoft.Base.Localization.StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Indicates the border style for the control.")]
        [StiGuiMode(StiGuiMode.Gdi)]
        [StiOrder(StiPropertyOrder.DialogBorderStyle)]
		public StiBorderStyle BorderStyle
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

        #region IStiSerializableCustomControl
        private object control = null;
        [Browsable(true)]
        [StiSerializable(StiSerializationVisibility.Control)]
        public object Control
        {
            get
            {
                return control;
            }
            set
            {
                control = value;
                if (value != null && value is Control)
                {
                    ((Control)value).Width = (int)this.Width;
                    ((Control)value).Height = (int)this.Height;
                }
            }
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
        /// Creates a new StiCustomControl.
		/// </summary>
		public StiCustomControl() : this(RectangleD.Empty)
		{
		}

		/// <summary>
        /// Creates a new StiCustomControl.
		/// </summary>
		/// <param name="rect">The rectangle describes sizes and position of the control.</param>
        public StiCustomControl(RectangleD rect)
            : base(rect)
		{
			PlaceOnToolbox = true;
		}
		#endregion
    }
}