#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Stimulsoft.Base.Drawing;
using System.Drawing.Imaging;
using System.Drawing;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Export
{
    public class StiMetafileParser
    {
        #region struct SIZEL
        internal struct SIZEL
        {
            public int cX;	//LONG == signed int
            public int cY;	//LONG
        }
        #endregion

        #region struct POINTL
        internal struct POINTL
        {
            public int X; //LONG
            public int Y; //LONG
        }
        #endregion

        #region struct RECTL
        internal struct RECTL
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #endregion

        #region struct LOGFONT
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/fontext_1wmq.htm
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName; //tchar[LF_FACESIZE]
        }
        #endregion

        #region struct tagENHMETAHEADER
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_5t2q.htm
        internal struct tagENHMETAHEADER
        {
            //public uint iType; 
            //public uint nSize; 
            public RECTL rclBounds; //Specifies the dimensions, in device units, of the smallest rectangle that can be drawn around 
            //the picture stored in the metafile. This rectangle is supplied by graphics device interface (GDI).
            //Its dimensions include the right and bottom edges. 
            public RECTL rclFrame;	//Specifies the dimensions, in .01 millimeter units, of a rectangle that surrounds the picture stored
            //in the metafile. This rectangle must be supplied by the application that creates the metafile.
            //Its dimensions include the right and bottom edges. 
            public uint dSignature;
            public uint nVersion;
            public uint nBytes;
            public uint nRecords;
            public ushort nHandles;
            public ushort sReserved;
            public uint nDescription;
            public uint offDescription;
            public uint nPalEntries;
            public SIZEL szlDevice;		//Specifies the resolution of the reference device, in pixels.
            public SIZEL szlMillimeters;	//Specifies the resolution of the reference device, in millimeters.
            public uint cbPixelFormat;
            public uint offPixelFormat;
            public uint bOpenGL;
            //public SIZEL szlMicrometers;	//Windows 98/Me, Windows 2000/XP: Size of the reference device in micrometers.
        }
        #endregion

        #region struct XFORM
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/cordspac_4tma.htm
        internal struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;

            public XFORM(float eM11, float eM12, float eM21, float eM22, float eDx, float eDy)
            {
                this.eM11 = eM11;
                this.eM12 = eM12;
                this.eM21 = eM21;
                this.eM22 = eM22;
                this.eDx = eDx;
                this.eDy = eDy;
            }
        }
        #endregion

        #region struct POINTD
        internal struct POINTD
        {
            public double X;
            public double Y;
        }
        #endregion

        #region class EmfFontInfo
        internal class EmfFontInfo
		{
			public int internalFontNumber;
			public LOGFONT elfLogFont;
		}
        #endregion

        #region class EmfBrushInfo
        internal class EmfBrushInfo
		{
			public uint lbStyle;	
			public uint lbColor;
			public uint lbHatch;
		}
        #endregion

        #region class EmfPenInfo
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/pens_0k8i.htm
        internal class EmfPenInfo
		{
			public uint lopnStyle;
			public int lopnWidth;
			public uint lopnColor;
		}
        #endregion

        #region class GDIobjectInfo
        internal class GDIobjectInfo
		{
			public uint ihNumber;
			public EmfFontInfo Font;
			public EmfBrushInfo Brush;
			public EmfPenInfo Pen;
		}
        #endregion
        
		#region Internal GDI objects methods
        private void SelectObject(uint ihNumber)
        {
            if (ihNumber == 0x80000005)
            {
                gdiSelectedBrush = 0xFFFFFFFF;	//NULL_BRUSH
                return;
            }
            if (ihNumber == 0x80000008)
            {
                gdiSelectedPen = 0xFFFFFFFF;	//NULL_PEN
                return;
            }

            //check array, if element with ihNumber exists
            for (int index = GDIobjects.Count - 1; index >= 0; index--)
            {
                GDIobjectInfo gdiObject = GDIobjects[index];
                if (gdiObject.ihNumber == ihNumber)
                {
                    if (gdiObject.Pen != null) gdiSelectedPen = ihNumber;
                    if (gdiObject.Brush != null) gdiSelectedBrush = ihNumber;
                    if (gdiObject.Font != null) gdiSelectedFont = ihNumber;
                    return;
                }
            }
        }

        private void RemoveObject(uint ihNumber)
		{
			//check array, if element with ihNumber already exists
			for (int index = GDIobjects.Count - 1; index >= 0; index --)
			{
                if (GDIobjects[index].ihNumber == ihNumber)
				{
					GDIobjects.RemoveAt(index);
				}
			}
		}

		private int GetGdiObjectIndex(uint ihNumber)
		{
			//check array, if element with ihNumber exists
			for (int index = GDIobjects.Count - 1; index >= 0; index --)
			{
                if (GDIobjects[index].ihNumber == ihNumber)
				{
					return index;
				}
			}
			return 0;
		}

        //		/* Stock Logical Objects */
        //		#define WHITE_BRUSH         0
        //		#define LTGRAY_BRUSH        1
        //		#define GRAY_BRUSH          2
        //		#define DKGRAY_BRUSH        3
        //		#define BLACK_BRUSH         4
        //		#define NULL_BRUSH          5
        //		#define HOLLOW_BRUSH        NULL_BRUSH
        //		#define WHITE_PEN           6
        //		#define BLACK_PEN           7
        //		#define NULL_PEN            8
        //		#define OEM_FIXED_FONT      10
        //		#define ANSI_FIXED_FONT     11
        //		#define ANSI_VAR_FONT       12
        //		#define SYSTEM_FONT         13
        //		#define DEVICE_DEFAULT_FONT 14
        //		#define DEFAULT_PALETTE     15
        //		#define SYSTEM_FIXED_FONT   16

		private void AddStockGDIobjects()
		{
			//empty object with index 0
			GDIobjectInfo gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0xFFFFFFFF;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//WHITE_BRUSH
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000000;
			EmfBrushInfo emfb = new EmfBrushInfo();
			emfb.lbStyle = 0;
			emfb.lbColor = 0x00FFFFFF;
			emfb.lbHatch = 0;
			gdiObject.Brush = emfb;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//LTGRAY_BRUSH
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000001;
			emfb = new EmfBrushInfo();
			emfb.lbStyle = 0;
			emfb.lbColor = 0x00C0C0C0;
			emfb.lbHatch = 0;
			gdiObject.Brush = emfb;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//GRAY_BRUSH
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000002;
			emfb = new EmfBrushInfo();
			emfb.lbStyle = 0;
			emfb.lbColor = 0x00808080;
			emfb.lbHatch = 0;
			gdiObject.Brush = emfb;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//DKGRAY_BRUSH
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000003;
			emfb = new EmfBrushInfo();
			emfb.lbStyle = 0;
			emfb.lbColor = 0x00404040;
			emfb.lbHatch = 0;
			gdiObject.Brush = emfb;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//BLACK_BRUSH
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000004;
			emfb = new EmfBrushInfo();
			emfb.lbStyle = 0;
			emfb.lbColor = 0x00000000;
			emfb.lbHatch = 0;
			gdiObject.Brush = emfb;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//WHITE_PEN
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000006;
			EmfPenInfo emfp = new EmfPenInfo();
			emfp.lopnStyle = 0;
			emfp.lopnWidth = 0;
			emfp.lopnColor = 0x00FFFFFF;
			gdiObject.Pen = emfp;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

			//BLACK_PEN
			gdiObject = new GDIobjectInfo();
			gdiObject.ihNumber = 0x80000007;
			emfp = new EmfPenInfo();
			emfp.lopnStyle = 0;
			emfp.lopnWidth = 0;
			emfp.lopnColor = 0x00000000;
			gdiObject.Pen = emfp;
			RemoveObject(gdiObject.ihNumber);
			GDIobjects.Add(gdiObject);

            SelectObject(0x80000000);   //white brush
            SelectObject(0x80000007);   //black pen
        }
        #endregion

        #region Internal methods
        private Color ConvertColorFromGdi(uint gdiColor)
        {
            Color newColor = Color.FromArgb(
                (int)((gdiColor & 0x0000FFu)),
                (int)((gdiColor & 0x00FF00u) >> 8),
                (int)((gdiColor & 0xFF0000u) >> 16));
            return newColor;
        }

        private double GetTransformedCX(int x)
        {
            double newPointX = (x * baseXForm.eM11);
            return newPointX;
        }
        private double GetTransformedCY(int y)
        {
            double newPointY = (y * baseXForm.eM22);
            return newPointY;
        }
        private double GetTransformedX(int x)
        {
            double newPointX = (x * baseXForm.eM11 - emfOffsetX);
            return newPointX;
        }
        private double GetTransformedY(int y)
        {
            double newPointY = (y * baseXForm.eM22 - emfOffsetY);
            return newPointY;
        }
        private POINTD GetTransformedXY(int x, int y)
        {
            POINTD newPoint = new POINTD();
            newPoint.X = (x * baseXForm.eM11 - emfOffsetX);
            newPoint.Y = (y * baseXForm.eM22 - emfOffsetY);
            return newPoint;
        }
        private double GetTransformedW(int w)
        {
            double newW = w * ((baseXForm.eM11 + baseXForm.eM22) / 2);
            return newW;
            //return (double)w;
        }

        private void SetViewport()
        {
            if ((viewportFlags == 0x0F) && ((emfMapMode == 7) || (emfMapMode == 8)))
            {
                float scaleX = (float)(viewportExt.X / windowExt.X);
                float scaleY = (float)(viewportExt.Y / windowExt.Y);

                float em11 = scaleX;
                //float em12 = 0;
                //float em21 = 0;
                float em22 = scaleY;
                float edx = (float)(emfX * (1 - scaleX) + (viewportOrg.X - windowOrg.X * scaleX) * emfScaleX);
                float edy = (float)(emfY * (1 - scaleY) - (viewportOrg.Y - windowOrg.Y * scaleY) * emfScaleY);

                //pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                //    ConvertToString(em11, 7),
                //    ConvertToString(0),
                //    ConvertToString(0),
                //    ConvertToString(em22, 7),
                //    ConvertToString(edx, 6),
                //    ConvertToString(edy, 6));
                //
                ////viewportBack = new XFORM(em11, em12, em21, em22, edx, edy);

                viewportFlags = 0;
            }
        }

		#endregion

        #region Data read procedures
        private byte[] dataArray = null;
        private int dataOffset;

        private Int32 ReadInt32()
        {
            Int32 value = BitConverter.ToInt32(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private Int16 ReadInt16()
        {
            Int16 value = BitConverter.ToInt16(dataArray, dataOffset);
            dataOffset += 2;
            return value;
        }
        private UInt32 ReadUInt32()
        {
            UInt32 value = BitConverter.ToUInt32(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private UInt16 ReadUInt16()
        {
            UInt16 value = BitConverter.ToUInt16(dataArray, dataOffset);
            dataOffset += 2;
            return value;
        }
        private byte ReadUInt8()
        {
            byte value = dataArray[dataOffset];
            dataOffset++;
            return value;
        }
        private float ReadSingle()
        {
            float value = BitConverter.ToSingle(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private string ReadStringTchar(int bufSize)
        {
            StringBuilder value = new StringBuilder();
            int offset = 0;
            while ((BitConverter.ToUInt16(dataArray, dataOffset + offset * 2) != 0) && (offset < bufSize))
            {
                value.Append(BitConverter.ToChar(dataArray, dataOffset + offset * 2));
                offset++;
            }
            dataOffset += bufSize * 2;
            return value.ToString();
        }
        #endregion

        #region Constants
        private const double penWidthScale = 1.2;
        private const double penWidthDefault = 0.1;
        #endregion

        #region Variables

        internal List<GDIobjectInfo> GDIobjects = new List<GDIobjectInfo>();

        private uint gdiSelectedPen = 0;
		private uint gdiSelectedBrush = 0;
		private uint gdiSelectedFont = 0;

        //private POINTD emfCurrentPoint;
		//private uint emfBackColor = 0;
		//private uint emfTextColor = 0;
		//private uint emfBkMode = 0;	//TRANSPARENT 1, OPAQUE 2
		//private uint emfPolyFillMode = 0;	//ALTERNATE 1, WINDING 2
		private uint emfMapMode = 0;	//MM_ISOTROPIC 7, MM_ANISOTROPIC 8
		//private uint emfTextAlign = 0;

		private PointD viewportOrg = PointD.Empty;
		private PointD viewportExt = PointD.Empty;
		private PointD windowOrg = PointD.Empty;
		private PointD windowExt = PointD.Empty;
		private uint viewportFlags = 0;
		//private object viewportBack = null;
		//private Stack viewportStack = null;

        private XFORM baseXForm = new XFORM();

    	private double emfX = 0;
		private double emfY = 0;
		private int emfOffsetX = 0;
		private int emfOffsetY = 0;
		//private double emfScale = 0;
        private double emfScaleX = 0;
        private double emfScaleY = 0;

		//private bool makePath = false;
		//private ArrayList emfPath = null;

        private Stack dcStack = null;
        //private DCData dcData = null;

        private class DCData
        {
            public uint emfBackColor = 0;
            public uint emfTextColor = 0;
            public uint emfBkMode = 0;
            public uint emfPolyFillMode = 0;
            public uint emfMapMode = 0;
            public uint emfTextAlign = 0;
        }

        #endregion

        #region MetafileCallback
        private bool MetafileCallback(
            EmfPlusRecordType recordType,
            int flags,
            int dataSize,
            IntPtr data,
            PlayRecordCallback callbackData)
        {
            dataArray = null;
            dataOffset = 0;
            if (data != IntPtr.Zero)
            {
                // Copy the unmanaged record to a managed byte buffer 
                // that can be used by PlayRecord.
                dataArray = new byte[dataSize];
                Marshal.Copy(data, dataArray, 0, dataSize);
            }


            #region Prepare data

            //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/winprog/winprog/windows_data_types.htm
            //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_5hkj.htm
            //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/metafile_5hkj.htm
            //ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/gdi/metafile_5hkj.htm

            if ((viewportFlags == 0x0F) &&
                (recordType != EmfPlusRecordType.EmfSetViewportOrgEx) &&
                (recordType != EmfPlusRecordType.EmfSetViewportExtEx) &&
                (recordType != EmfPlusRecordType.EmfSetWindowOrgEx) &&
                (recordType != EmfPlusRecordType.EmfSetWindowExtEx))
            {
                SetViewport();
            }

            switch (recordType)
            {

                case EmfPlusRecordType.EmfHeader:
                    #region read data
                    tagENHMETAHEADER header;

                    header.rclBounds.Left = ReadInt32();
                    header.rclBounds.Top = ReadInt32();
                    header.rclBounds.Right = ReadInt32();
                    header.rclBounds.Bottom = ReadInt32();
                    header.rclFrame.Left = ReadInt32();
                    header.rclFrame.Top = ReadInt32();
                    header.rclFrame.Right = ReadInt32();
                    header.rclFrame.Bottom = ReadInt32();
                    header.dSignature = ReadUInt32();
                    header.nVersion = ReadUInt32();
                    header.nBytes = ReadUInt32();
                    header.nRecords = ReadUInt32();
                    header.nHandles = ReadUInt16();
                    header.sReserved = ReadUInt16();
                    header.nDescription = ReadUInt32();
                    header.offDescription = ReadUInt32();
                    header.nPalEntries = ReadUInt32();
                    header.szlDevice.cX = ReadInt32();
                    header.szlDevice.cY = ReadInt32();
                    header.szlMillimeters.cX = ReadInt32();
                    header.szlMillimeters.cY = ReadInt32();
                    header.cbPixelFormat = ReadUInt32();
                    header.offPixelFormat = ReadUInt32();
                    header.bOpenGL = ReadUInt32();
                    //						#if (WINVER >= 0x0500)
                    //						header.szlMicrometers.cX = ReadInt32(); 
                    //						header.szlMicrometers.cY = ReadInt32(); 

                    emfOffsetX = header.rclBounds.Left;
                    emfOffsetY = header.rclBounds.Top;
                    int emfWidth = header.rclBounds.Right - header.rclBounds.Left;
                    int emfHeight = header.rclBounds.Bottom - header.rclBounds.Top;
                    if (emfOffsetX != 0)
                    {
                        emfWidth += emfOffsetX * 2;
                        emfOffsetX = 0;
                    }
                    if (emfOffsetY != 0)
                    {
                        emfHeight += emfOffsetY * 2;
                        emfOffsetY = 0;
                    }
                    //double emfScaleX = emfComponent.Width / emfWidth;
                    //double emfScaleY = emfComponent.Height / emfHeight;
                    //emfScale = emfScaleX;
                    //if (emfScaleY < emfScaleX) emfScale = emfScaleY;

                    //emfScale = hiToTwips;
                    //emfScaleX = hiToTwips * 0.996;
                    //emfScaleY = hiToTwips;

                    //double dpiX = (header.szlDevice.cX / (double)header.szlMillimeters.cX * 25.4);
                    //double dpiY = (header.szlDevice.cY / (double)header.szlMillimeters.cY * 25.4);
                    //double dpi = (dpiX + dpiY) / 2f;
                    //if (dpi > 110)
                    //{
                    //    emfScale *= 96f / dpi;
                    //    emfScaleX *= 96f / dpiX;
                    //    emfScaleY *= 96f / dpiY;
                    //}

                    //RectangleD tempRectD = new RectangleD();
                    //if (emfComponent.Component is StiRichText)
                    //{
                    //    tempRectD = (emfComponent.Component as StiRichText).ConvertTextMargins(tempRectD, false);
                    //}
                    //RectangleD rectRichD = new RectangleD(
                    //    emfComponent.X + tempRectD.X * hiToTwips,
                    //    emfComponent.Y + (-tempRectD.Bottom) * hiToTwips,
                    //    emfComponent.Width + tempRectD.Width * hiToTwips,
                    //    emfComponent.Height + tempRectD.Height * hiToTwips);

                    ////emfX = emfComponent.X + (centerEmf ? (emfComponent.Width - emfWidth * emfScale) / 2f : 0);
                    ////emfY = emfComponent.Y + emfComponent.Height - (centerEmf ? (emfComponent.Height - emfHeight * emfScale) / 2f : 0);
                    //emfX = rectRichD.X + (centerEmf ? (rectRichD.Width - emfWidth * emfScale) / 2f : 0);
                    //emfY = rectRichD.Y + rectRichD.Height - (centerEmf ? (rectRichD.Height - emfHeight * emfScale) / 2f : 0);

                    baseXForm = new XFORM(1, 0, 0, 1, 0, 0);
                    viewportFlags = 0;
                    //viewportBack = null;
                    //viewportStack = new Stack();
                    emfMapMode = 0;
                    dcStack = new Stack();
                    #endregion
                    break;



                default:
                    //empty
                    break;
            }
            #endregion

            return true;
        }
        #endregion

        #region Main
        private static object lockMetafileFlag = new object();

        private void RenderRtf(Metafile metafile)
        {
            Bitmap emfBmp = null;
            Graphics emfGr = null;
            Graphics.EnumerateMetafileProc m_delegate = null;
            Point m_destPoint = new Point(0, 0);

            lock (lockMetafileFlag)
            {
                if (emfBmp == null) emfBmp = new Bitmap(25, 25);
                if (emfGr == null) emfGr = Graphics.FromImage(emfBmp);
                if (m_delegate == null) m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                try
                {
                    GDIobjects.Clear();
                    AddStockGDIobjects();
                    emfGr.EnumerateMetafile(metafile, m_destPoint, m_delegate);
                }
                catch
                {
                }
                finally
                {
                    GDIobjects.Clear();
                }
            }
        }
        #endregion
    }
}
