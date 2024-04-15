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
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Controls
{	
	[ToolboxItem(false)]
	public class StiDialogPanel : Panel
	{
		#region Properties
		private StiBorderStyle borderStyle = StiBorderStyle.None;
		[Category("Appearance")]
		[DefaultValue(StiBorderStyle.None)]
		public new virtual StiBorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}
			set
			{				
				borderStyle = value;
				Invalidate();
			}
		}
		#endregion

		#region Handlers
		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			StiColors.InitColors();
		}

		protected override void OnPaint(PaintEventArgs p)
		{
			Graphics g = p.Graphics;
			Rectangle rect = new Rectangle(0, 0, Width, Height);
			using (SolidBrush brush = new SolidBrush(BackColor))
			{
				g.FillRectangle(brush, rect);
			}
			
			if (BorderStyle != StiBorderStyle.None)
			{
				ControlPaint.DrawBorder3D(g, rect, (Border3DStyle)BorderStyle);
			}
		}

		#endregion

		public StiDialogPanel()
		{
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.Selectable, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, false);
		}		
	}
}
