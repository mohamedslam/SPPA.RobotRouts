#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Controls
{
	public class Empty
	{
	}
}

namespace Stimulsoft.Report.Controls
{
	/// <summary>
	/// Allows users to select a value from a drop-down list. 
	/// </summary>
	[ToolboxItem(false)]
	public class StiDialogComboBox : ComboBox
	{
		#region Fields
		protected bool isMouseOver = false;
		protected bool lastIsMouseOverButton = false;
		#endregion

		#region Browsable(false)
		/// <summary>
		/// Do not use this property.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new System.Windows.Forms.DrawMode DrawMode
		{
			get
			{
				return base.DrawMode;
			}
			set
			{
				base.DrawMode = value;
			}
		}
		#endregion

		#region Properties
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor
		{
			get
			{
                return StiUX.InputForeground;
            }
			set
			{
				base.ForeColor = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor
		{
			get
			{
                return StiUX.InputBackground;
            }
			set
			{
				base.BackColor = value;
			}
		}

		private MeasureItemEventHandler GetMeasureItemHandler()
		{
			FieldInfo info = typeof(ComboBox).GetField("EVENT_MEASUREITEM", 
				BindingFlags.Static | 
				BindingFlags.NonPublic | 
				BindingFlags.GetField);
            
			MeasureItemEventHandler handler = (MeasureItemEventHandler)base.Events[info.GetValue(null)];

			return handler;
		}

		private DrawItemEventHandler GetDrawItemHandler()
		{
			FieldInfo info = typeof(ComboBox).GetField("EVENT_DRAWITEM", 
				BindingFlags.Static | 
				BindingFlags.NonPublic | 
				BindingFlags.GetField);
            
			DrawItemEventHandler handler = (DrawItemEventHandler)base.Events[info.GetValue(null)];

			return handler;
		}


		private bool flat = true;
		[DefaultValue(true)]
		[Category("Appearance")]
		public virtual bool Flat
		{
			get
			{
				return flat;
			}
			set
			{
				flat = value;
				Invalidate();
			}
		}


		private bool useFirstImage = false;
		/// <summary>
		/// Use always first Image from ImageList to Draw item.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool UseFirstImage
		{
			get
			{
				return useFirstImage;
			}
			set
			{
				useFirstImage = value;
				Invalidate();
			}
		}


		private bool autoDropDownWidth = true;
		/// <summary>
		/// Gets or sets a value indicating whether the size of popup is auto calculated.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public virtual bool AutoDropDownWidth
		{
			get
			{
				return autoDropDownWidth;
			}
			set
			{
				autoDropDownWidth = value;
				Invalidate();
			}
		}


		private ImageList imageList = null;
		/// <summary>
		/// Gets or sets the collection of images available to the StiListBoxBase items.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(null)]
		[Browsable(true)]
		public virtual ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;
				Invalidate();
			}
		}


		[Browsable(false)]
		public bool IsMouseOverButton
		{
			get
			{
				Rectangle rect = StiControlPaint.GetButtonRect(new Rectangle(0, 0, Width - 1, Height - 1), 
					Flat, RightToLeft);

                return rect.Contains(this.PointToClient(Cursor.Position));
            }
        }
		#endregion

		#region Methods
		public static void DrawComboBox(Graphics g, Rectangle rect, string text, Font font, Color foreColor, Color backColor,
			RightToLeft rightToLeft, bool isEnabled, bool isMouseOver, bool isMouseOverButton, bool isDroppedDown, 
			bool isFocused, bool fillContent, ComboBoxStyle comboBoxStyle, Image buttonBitmap, bool flat)
		{
			if (fillContent)
			{
				using (SolidBrush brush = new SolidBrush(backColor))
				{
					g.FillRectangle(brush, rect);
				}
			}

			StiControlPaint.DrawBorder(g, rect, isMouseOver, isFocused, isEnabled);

			if (buttonBitmap != null)
			{
				if (comboBoxStyle != ComboBoxStyle.Simple)
				{
					Rectangle buttonRect = StiControlPaint.GetButtonRect(rect, flat, rightToLeft);
					StiControlPaint.DrawButton(g, buttonRect, buttonBitmap, isDroppedDown, isFocused | isMouseOver, 
						isMouseOverButton, isEnabled, flat);
				}
			}

			if (text != null)
			{
				using (StringFormat sf = new StringFormat())
				using (SolidBrush brush = new SolidBrush(foreColor))
				{
					sf.FormatFlags = StringFormatFlags.NoWrap;
					sf.LineAlignment = StringAlignment.Center;
					g.DrawString(text, font, brush, rect, sf); 
				}
			}
		}


		protected virtual void Draw(bool fillContent)
		{			
			using (Graphics g = Graphics.FromHwnd(this.Handle))
			{	
				Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
				
				if (flat)
				{
					Color color = BackColor;
					if (!Enabled)color = SystemColors.Control;
					
					int borderWidth = (int)SystemInformation.Border3DSize.Width;
					int borderHeight = (int)SystemInformation.Border3DSize.Height;

					Rectangle buttonRect = rect;
					if (DropDownStyle != ComboBoxStyle.Simple)
						buttonRect.Width -= SystemInformation.HorizontalScrollBarArrowWidth;

					using (SolidBrush brush = new SolidBrush(color))
					{
						if (RightToLeft == RightToLeft.Yes)
						{
							buttonRect.X += SystemInformation.HorizontalScrollBarArrowWidth;

							g.FillRectangle(brush, buttonRect);
						}
						else
						{					
							g.FillRectangle(brush, 1, 1, buttonRect.Width - 1, borderHeight - 1);
							g.FillRectangle(brush, 1, buttonRect.Bottom - borderHeight + 1, buttonRect.Width - 1, borderHeight - 1);
							g.FillRectangle(brush, 1, 1, borderWidth - 1, buttonRect.Height - borderHeight);
							g.FillRectangle(brush, buttonRect.Right - borderWidth + 1, 1, borderWidth - 1, buttonRect.Height - borderHeight);
						}
					}
				}

				DrawComboBox(g, rect, null, Font, ForeColor, BackColor, 
					RightToLeft, Enabled, isMouseOver, IsMouseOverButton,
					DroppedDown, Focused, false, DropDownStyle, buttonBitmap, flat);

				lastIsMouseOverButton = IsMouseOverButton;
			}
		}
 
		#endregion

		#region Handlers
		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			StiColors.InitColors();
		}

		protected override void OnDropDown(EventArgs e)
		{
			base.OnDropDown(e);

			if (AutoDropDownWidth)
			{
				using (Graphics g = Graphics.FromHwnd(this.Handle))
				{
					int width = 0;
					for (int index = 0; index < Items.Count; index ++)
					{
						MeasureItemEventArgs me = new MeasureItemEventArgs(g, index ++, ItemHeight);
						OnMeasureItem(me);
						width = Math.Max(me.ItemWidth, width);
					}
					DropDownWidth = Math.Max(width, Width);
				}
			}
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			MeasureItemEventHandler handler = GetMeasureItemHandler();
			if (handler == null)
			{
				base.OnMeasureItem(e);
				if (e.Index != -1)
				{
					Graphics g = e.Graphics;
					string str = Items[e.Index].ToString();
					e.ItemWidth = (int)g.MeasureString(str, this.Font).Width + 
						SystemInformation.HorizontalScrollBarThumbWidth + 10;
				}
			}
			else
			{
				handler(this, e);
			}
		}
		
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			DrawItemEventHandler handler = GetDrawItemHandler();
			if (handler == null)
			{
				Graphics g = e.Graphics;
				Rectangle itemRect = e.Bounds;
				if ((e.State & DrawItemState.Selected) > 0)itemRect.Width--;
				DrawItemState state = e.State;

				int imageIndex = UseFirstImage ? 0 : e.Index;
				string text = Text;
				if (e.Index != -1)text = this.GetItemText(Items[e.Index]);

				StiControlPaint.DrawItem(g, itemRect, state, text, ImageList, imageIndex, Font, BackColor, ForeColor, RightToLeft);
			}
			else
			{
				handler(this, e);
			}
		}
		
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (Flat)Draw(false);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (Flat)Draw(false);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			isMouseOver = true;
			if (Flat)Draw(false);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			isMouseOver = false;
			if (Flat)Draw(false);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (lastIsMouseOverButton != IsMouseOverButton)
			{
				if (Flat)Draw(false);
			}
		}

		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == (int)Win32.Msg.WM_PAINT)
			{
				base.DefWndProc(ref msg);
				if (Flat)Draw(true);
			}				
			else base.WndProc(ref msg);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && buttonBitmap != null)
			{
				buttonBitmap.Dispose();
			}
			base.Dispose(disposing); 
		}

		#endregion

		#region Constructors
		private Image buttonBitmap;
		public StiDialogComboBox()
		{
			try
			{
				buttonBitmap = StiImageUtils.GetImage("Stimulsoft.Controls", "Stimulsoft.Controls.Bmp.ComboDown.bmp");
			}
			catch
			{
			}

			SetStyle(ControlStyles.ResizeRedraw, true);
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
		}
		#endregion
	}
}
