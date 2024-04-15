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
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Components
{
    [StiServiceBitmap(typeof(StiWinControl), "Stimulsoft.Report.Images.Components.StiWinControl.png")]
    [StiGdiPainter(typeof(StiWinControlGdiPainter))]
	[StiV1Builder(typeof(StiWinControlV1Builder))]
	[StiV2Builder(typeof(StiWinControlV2Builder))]
	[StiToolbox(true)]
	[StiContextTool(typeof(IStiShift))]
	[StiContextTool(typeof(IStiGrowToHeight))]
    [StiGuiMode(StiGuiMode.Gdi)]
	public class StiWinControl : 
		StiComponent,
		IStiExportImageExtended		
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // Old
            jObject.RemoveProperty("CanShrink");
            jObject.RemoveProperty("CanGrow");

            // StiWinControl
            jObject.AddPropertyBorder("Border", Border);
            jObject.AddPropertyStringNullOrEmpty("TypeName", TypeName);
            jObject.AddPropertyString("Text", Text);
            jObject.AddPropertyColor("ForeColor", ForeColor, Color.Black);
            jObject.AddPropertyFont("Font", Font, "Microsoft Sans Serif", 8);
            jObject.AddPropertyColor("BackColor", BackColor, SystemColors.Control);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Border":
                        this.Border = property.DeserializeBorder();
                        break;

                    case "TypeName":
                        this.typeName = property.DeserializeString();
                        break;

                    case "Text":
                        this.Text = property.DeserializeString();
                        break;

                    case "ForeColor":
                        this.ForeColor = property.DeserializeColor();
                        break;

                    case "Font":
                        this.font = property.DeserializeFont(this.font);
                        break;

                    case "BackColor":
                        this.backColor = property.DeserializeColor();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        public override StiComponentId ComponentId => StiComponentId.StiWinControl;

		public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level) => null;

        public override StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
		#endregion

		#region IStiBorder
		/// <summary>
		/// The appearance and behavior of the component border.
		/// </summary>
		[StiCategory("Appearance")]
		[StiSerializable]
		[Description("The appearance and behavior of the component border.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public StiBorder Border { get; set; } = new StiBorder();
	    #endregion

		#region IStiExportImageExtended
		public virtual Image GetImage(ref float zoom)
		{
			return GetImage(ref zoom, StiExportFormat.None);
		}

		public virtual Image GetImage(ref float zoom, StiExportFormat format)
		{
            var painter = StiPainter.GetPainter(this.GetType(), StiGuiMode.Gdi);
            return painter.GetImage(this, ref zoom, format);
		}

		[Browsable(false)]
		public override bool IsExportAsImage(StiExportFormat format)
		{
			return true;
		}
		#endregion

		#region IStiCanShrink override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanShrink
		{
			get
			{
				return base.CanShrink;
			}
			set
			{
			}
		}
		#endregion

		#region IStiCanGrow override
		[Browsable(false)]
		[StiNonSerialized]
		public override bool CanGrow
		{
			get
			{
				return base.CanGrow;
			}
			set
			{
			}
		}
		#endregion

		#region IStiGetFonts
		public override List<StiFont> GetFonts()
		{
			var result = base.GetFonts();
			result.Add(new StiFont(Font));
			return result.Distinct().ToList();
		}
		#endregion

		#region StiComponent override
		/// <summary>
		/// Gets value to sort a position in the toolbox.
		/// </summary>
		public override int ToolboxPosition => (int)StiComponentToolboxPosition.WinControl;

	    public override StiToolboxCategory ToolboxCategory => StiToolboxCategory.Components;

	    /// <summary>
		/// Gets a localized name of the component category.
		/// </summary>
		public override string LocalizedCategory => StiLocalization.Get("Report", "Components");
        
	    /// <summary>
		/// Gets a localized component name.
		/// </summary>
		public override string LocalizedName => StiLocalization.Get("Components", "StiWinControl");
	    #endregion

		#region Methods
		public Image GetControlImage()
		{
		    if (Page == null || Report == null || Control == null)
		        return null;

		    var rect = Report.Unit.ConvertToHInches(ClientRectangle);
		    if (!(rect.Width > 0) || !(rect.Height > 0))
		        return null;

		    Control.Left = 0;
		    Control.Top = 0;
		    Control.Width = (int)rect.Width;
		    Control.Height = (int)rect.Height;
		    Control.Text = Text;
		    Control.ForeColor = ForeColor;
		    Control.BackColor = BackColor;
		    Control.Font = Font;

		    var stream = new MemoryStream();
		    Image mf = null;
		    using (var bmp = new Bitmap(1, 1))
		    using (var grfx = Graphics.FromImage(bmp))
		    {
		        var ipHdc = grfx.GetHdc();
		        mf = new Metafile(stream, ipHdc);
		        grfx.ReleaseHdc(ipHdc);
		    }

		    using (var g = Graphics.FromImage(mf))
		    {
		        g.Clear(Color.White);
						
		        var controlType = this.Control.GetType();
				
		        var getStyleInfo = controlType.GetMethod("GetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
		        var args = new object[] { ControlStyles.DoubleBuffer };
		        var doubleBuffer = (bool) getStyleInfo.Invoke(this.Control, args);

		        var setStyleInfo = controlType.GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
		        args = new object[] { ControlStyles.DoubleBuffer, false };
		        setStyleInfo.Invoke(this.Control, args);

		        var ptr = g.GetHdc();
		        var message = Message.Create(this.Control.Handle, 15, ptr, IntPtr.Zero);
				
		        var wndProcInfo = controlType.GetMethod("WndProc", BindingFlags.NonPublic | BindingFlags.Instance);
		        args = new object[] { message };
		        wndProcInfo.Invoke(this.Control, args);
		        g.ReleaseHdc(ptr);

		        args = new object[] { ControlStyles.DoubleBuffer, doubleBuffer};
		        setStyleInfo.Invoke(this.Control, args);
		    }
		    return mf;
		}

		private void UpdateControl()
		{
			if (this.Control != null)
			{
				this.Control.Dispose();
				this.Control = null;
			}

			if (!string.IsNullOrEmpty(TypeName))
			{
				var type = StiTypeFinder.GetType(this.TypeName);
			    if (type != null)
			        this.Control = StiActivator.CreateObject(type) as Control;
			}
		} 
		#endregion

		#region Properties
		private string typeName = "";
		/// <summary>
		/// Gets or sets type name.
		/// </summary>
		[Browsable(true)]
		[DefaultValue("")]
		[StiSerializable]
		[StiCategory("WinControl")]
		[StiOrder(StiPropertyOrder.WinControlTypeName)]
		[Description("Gets or sets type name.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public string TypeName
		{
			get
			{
				return typeName;
			}
			set
			{
				typeName = value;
				UpdateControl();
			}
		}
        
	    /// <summary>
		/// Gets or sets control.
		/// </summary>
		[Browsable(false)]
		public Control Control { get; set; }

	    /// <summary>
		/// Gets or sets the text associated with this control.
		/// </summary>
		[StiSerializable]
		[Browsable(true)]
		[Description("Gets or sets the text associated with this control.")]
		[StiCategory("WinControl")]
		[StiOrder(StiPropertyOrder.WinControlText)]
        [StiPropertyLevel(StiLevel.Basic)]
		public string Text { get; set; } = "";

	    /// <summary>
		/// Gets or sets the foreground color of the control.
		/// </summary>
		[StiSerializable]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[StiCategory("WinControl")]
		[StiOrder(StiPropertyOrder.WinControlForeColor)]
		[Description("Gets or sets the foreground color of the control.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Color ForeColor { get; set; } = Color.Black;

	    private Font font = new Font("Microsoft Sans Serif", 8);
		/// <summary>
		/// The font used to display text in the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("WinControl")]
		[StiOrder(StiPropertyOrder.WinControlFont)]
		[Description("The font used to display text in the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				UpdateControl();
			}
		}

		private Color backColor = SystemColors.Control;
		/// <summary>
		/// The background color of the component.
		/// </summary>
		[StiSerializable]
		[StiCategory("WinControl")]
		[StiOrder(StiPropertyOrder.WinControlBackColor)]
		[TypeConverter(typeof(Stimulsoft.Base.Drawing.Design.StiColorConverter))]
		[Editor("Stimulsoft.Base.Drawing.Design.StiColorEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
		[Description("The background color of the component.")]
        [StiPropertyLevel(StiLevel.Basic)]
		public virtual Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				backColor = value;
				UpdateControl();
			}
		}
        #endregion

        #region Methods.override
        public override StiComponent CreateNew()
        {
            return new StiWinControl();
        }
        #endregion

	    /// <summary>
	    /// Creates a new component of the type StiWinControl.
	    /// </summary>
	    public StiWinControl() : this(RectangleD.Empty)
	    {
	    }

		/// <summary>
		/// Creates a new component of the type StiWinControl.
		/// </summary>
		/// <param name="rect">The rectangle describes size and position of the component.</param>
		public StiWinControl(RectangleD rect) : base(rect)
		{
			PlaceOnToolbox = false;
		}
	}
}