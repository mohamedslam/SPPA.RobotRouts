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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security;

namespace Stimulsoft.Report.Viewer
{
	/// <summary>
	/// Class helps print in RAW mode. 
	/// </summary>
	[SuppressUnmanagedCodeSecurity]
	public sealed class StiRawPrinterHelper
	{
        #region struct DOC_INFO_1
        [StructLayout(LayoutKind.Sequential)]
	    private struct DOC_INFO_1
	    {
	        [MarshalAs(UnmanagedType.LPTStr)]
	        public string pDocName;

	        [MarshalAs(UnmanagedType.LPTStr)]
	        public string pOutputFile;

	        [MarshalAs(UnmanagedType.LPTStr)]
	        public string pDataType;
	    }
	    #endregion

        #region Methods.Imports
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool OpenPrinter(string pPrinterName, ref IntPtr phPrinter, int pDefault);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern int StartDocPrinter(IntPtr hPrinter, int Level, ref DOC_INFO_1 pDocInfo);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool StartPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool WritePrinter(IntPtr hPrinter, byte[] data, int buf, ref int pcWritten);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool EndPagePrinter(IntPtr hPrinter);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool EndDocPrinter(IntPtr hPrinter);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern bool ClosePrinter(IntPtr hPrinter);
        #endregion

        #region Methods
		/// <summary>
		/// Sends string to printer in RAW mode.
		/// </summary>
		/// <param name="printerName">Name of printer.</param>
		/// <param name="docName">Name of document.</param>
		/// <param name="text">Text to print.</param>
		public static void SendStringToPrinter(string printerName, string docName, string text)
		{
			if (!string.IsNullOrEmpty(StiOptions.Print.DotMatrixFormatSequence))
				SendStringToPrinter(printerName, docName, StiOptions.Print.DotMatrixFormatSequence, Encoding.ASCII);

			SendStringToPrinter(printerName, docName, text, Encoding.ASCII);
		}

		/// <summary>
		/// Sends string to printer in RAW mode.
		/// </summary>
		/// <param name="printerName">Name of printer.</param>
		/// <param name="docName">Name of document.</param>
		/// <param name="text">Text to print.</param>
		/// <param name="encoding">Encoding of text.</param>
		public static void SendStringToPrinter(string printerName, string docName, string text, Encoding encoding)
		{
			var printerHandle = IntPtr.Zero;

			var di = new DOC_INFO_1();
			di.pDataType = "RAW";

		    if (!OpenPrinter(printerName, ref printerHandle, 0))
		        throw new Win32Exception(Marshal.GetLastWin32Error());

		    di.pDocName = docName == null ? "" : docName;
			if (StartDocPrinter(printerHandle, 1, ref di) == 0)
			{
				ClosePrinter(printerHandle);
			    throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			if (!StartPagePrinter(printerHandle))
			{
				EndDocPrinter(printerHandle);
				ClosePrinter(printerHandle);
			    throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			var pcWritten = 0;
			var data = encoding.GetBytes(text);
			if (!WritePrinter(printerHandle, data, data.Length, ref pcWritten))
			    throw new Win32Exception(Marshal.GetLastWin32Error());

			EndPagePrinter(printerHandle);
			EndDocPrinter(printerHandle);
			ClosePrinter(printerHandle);
		}
		#endregion
	}
}
