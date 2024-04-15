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
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Stimulsoft.Report;
using Stimulsoft.Report.CodeDom;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Units;
using Stimulsoft.Report.Events;
using Stimulsoft.Report.Components.TextFormats;
using Stimulsoft.Report.Components.Design;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Dictionary;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.Components
{	
	public class StiRichTextHelper
	{
		#region STRUCT_CHARRANGE
		[StructLayout(LayoutKind.Sequential)]
			internal struct STRUCT_CHARRANGE
		{
			public Int32 cpMin;
			public Int32 cpMax;
		}
		#endregion

		#region STRUCT_FORMATRANGE
		[StructLayout(LayoutKind.Sequential)]
			internal struct STRUCT_FORMATRANGE
		{
			public IntPtr			hdc; 
			public IntPtr			hdcTarget; 
			public Win32.RECT		rc; 
			public Win32.RECT		rcPage; 
			public STRUCT_CHARRANGE chrg; 
		}
		#endregion

		#region CHARFORMATSTRUCT
		//[StructLayout( LayoutKind.Sequential)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CHARFORMATSTRUCT
		{
			public int      cbSize; 
			public UInt32   dwMask; 
			public UInt32   dwEffects; 
			public Int32    yHeight; 
			public Int32    yOffset; 
			public Int32	crTextColor; 
			public byte     bCharSet; 
			public byte     bPitchAndFamily; 
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
			public char[]   szFaceName; 
		}
		#endregion

		public static int FormatRange(StiRtfFormatType formatType, RichTextBox richTextBox, Rectangle rect, Graphics g, 
			int charFrom, int charTo, out int charEnd)
		{
			int rtfHeight = 0;

			STRUCT_CHARRANGE cr;
			cr.cpMin = charFrom;
			cr.cpMax = (((charTo >= 0) && (charTo > charFrom)) && (charTo <= richTextBox.TextLength)) ? charTo : richTextBox.TextLength;

			Win32.RECT rc;
			if (formatType == StiRtfFormatType.TotalRtfHeight)
			{
				rc.top		= HundredthInchToTwips(0);
				rc.left		= HundredthInchToTwips(0);
				rc.bottom	= HundredthInchToTwips(1000000);
				rc.right	= HundredthInchToTwips(rect.Width);
			}
			else if (formatType == StiRtfFormatType.MeasureRtf)
			{
				rc.top		= HundredthInchToTwips(0);
				rc.left		= HundredthInchToTwips(0);
				rc.bottom	= HundredthInchToTwips(rect.Height);
				rc.right	= HundredthInchToTwips(rect.Width);
			}
			else
			{
				rc.top		= HundredthInchToTwips(rect.Top);
				rc.left		= HundredthInchToTwips(rect.Left);
				rc.bottom	= HundredthInchToTwips(rect.Bottom);
				rc.right	= HundredthInchToTwips(rect.Right);
			}

			Win32.RECT rcPage;
			rcPage.top =	0;
			rcPage.left =	0;
			rcPage.bottom = rc.bottom;
			rcPage.right =	rc.right;

			IntPtr hdc = g.GetHdc();

			try
			{
				STRUCT_FORMATRANGE fr;
				fr.chrg      = cr;
				fr.hdc       = hdc;
				fr.hdcTarget = hdc;
				fr.rc        = rc;
				fr.rcPage    = rcPage;

				Int32 measurePtr = formatType != StiRtfFormatType.DrawRtf  ? 0 : 1;
			
				IntPtr formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr)); 
				Marshal.StructureToPtr(fr, formatPtr, false);

				charEnd = Win32.SendMessage(richTextBox.Handle, (int)Win32.Msg.EM_FORMATRANGE, measurePtr, formatPtr);
							
				if (formatType != StiRtfFormatType.DrawRtf)
				{
					fr = (STRUCT_FORMATRANGE)Marshal.PtrToStructure(formatPtr, typeof(STRUCT_FORMATRANGE));
					rtfHeight = TwipsToHundredthInch(fr.rc.bottom);
				}

				Marshal.FreeCoTaskMem(formatPtr);
			
				IntPtr lParam = new IntPtr(0);
				Win32.SendMessage(richTextBox.Handle, (int)Win32.Msg.EM_FORMATRANGE, 0, lParam);
			}
			finally
			{
				g.ReleaseHdc(hdc);
			}

			return rtfHeight;
		}


		private static int HundredthInchToTwips(int n)
		{
			return (int)(n * 1440 / StiOptions.Engine.RichTextScale);
		}

		
		private static int TwipsToHundredthInch(int n)
		{
			return (int)(n * StiOptions.Engine.RichTextScale / 1440);
		}
	}
}
