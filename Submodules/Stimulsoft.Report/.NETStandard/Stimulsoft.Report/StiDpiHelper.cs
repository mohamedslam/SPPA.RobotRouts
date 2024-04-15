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
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Pens = Stimulsoft.Drawing.Pens;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Report
{

    [SuppressUnmanagedCodeSecurity]
    public sealed class StiDpiHelper
    {
        #region Win32
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern int GetDeviceCaps(IntPtr hDc, int nIndex);

        private const short LOGPIXELSX = 88;
        private const short LOGPIXELSY = 90;
        #endregion

        #region Fields
        private static int deviceCapsDpi = 0;
        private static int graphicsDpi = 0;
        private static int graphicsRichTextDpi = 0;
        private static bool? fontScaling = null;
        private static bool? isWindows = null;
        private static bool? isLinux = null;
        private static bool? isMacOsX = null;
        #endregion

        #region Methods.Public
        public static int DeviceCapsDpi
        {
            get
            {
                if (deviceCapsDpi == 0)
                {
                    if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                    {
                        GetDpi();
                    }
                    else
                    {
                        deviceCapsDpi = 96;
                        graphicsDpi = 96;
                        graphicsRichTextDpi = 96;
                        fontScaling = false;
                    }
                }
                return deviceCapsDpi;
            }
        }

        public static int GraphicsDpi
        {
            get
            {
                if (graphicsDpi == 0)
                {
                    if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                    {
                        GetDpi();
                    }
                    else
                    {
                        deviceCapsDpi = 96;
                        graphicsDpi = 96;
                        graphicsRichTextDpi = 96;
                        fontScaling = false;
                    }
                }
                return graphicsDpi;
            }
        }

        public static int GraphicsRichTextDpi
        {
            get
            {
                if (graphicsRichTextDpi == 0)
                {
                    if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                    {
                        GetDpi();
                    }
                    else
                    {
                        deviceCapsDpi = 96;
                        graphicsDpi = 96;
                        graphicsRichTextDpi = 96;
                        fontScaling = false;
                    }
                }
                return graphicsRichTextDpi;
            }
        }

        public static double DeviceCapsScale
        {
            get
            {
                if (StiOptions.Engine.DpiAware)
                {
                    int dpi = DeviceCapsDpi;
                    if (dpi != 96) return 96 / (double)dpi;
                }
                return 1;
            }
        }

        public static double GraphicsScale
        {
            get
            {
                if (StiOptions.Engine.DpiAware)
                {
                    int dpi = GraphicsDpi;
                    if (dpi != 96) return 96 / (double)dpi;
                }
                return 1;
            }
        }

        public static double GraphicsRichTextScale
        {
            get
            {
                if (StiOptions.Engine.DpiAware)
                {
                    var dpi = GraphicsRichTextDpi;
                    if (dpi != 96) return 96 / (double)dpi;
                }
                return 1;
            }
        }

        public static bool NeedDeviceCapsScale
        {
            get
            {
                return DeviceCapsDpi != 96;
            }
        }

        public static bool NeedGraphicsScale
        {
            get
            {
                return GraphicsDpi != 96;
            }
        }

        public static bool NeedGraphicsRichTextScale
        {
            get
            {
                return GraphicsRichTextDpi != 96;
            }
        }

        public static bool NeedFontScaling
        {
            get
            {
                if (!StiOptions.Engine.DpiAware) return false;

                if (fontScaling == null)
                {
                    if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                    {
                        GetDpi();
                    }
                    else
                    {
                        deviceCapsDpi = 96;
                        graphicsDpi = 96;
                        graphicsRichTextDpi = 96;
                        fontScaling = false;
                    }
                }
                return fontScaling.GetValueOrDefault(false);
            }
        }

        public static void Reset(int deviceCapsDpi = 0, int graphicsDpi = 0, int graphicsRichTextDpi = 0, bool? fontScaling = null)
        {
            StiDpiHelper.deviceCapsDpi = deviceCapsDpi;
            StiDpiHelper.graphicsDpi = graphicsDpi;
            StiDpiHelper.graphicsRichTextDpi = graphicsRichTextDpi;
            StiDpiHelper.fontScaling = fontScaling;
        }

        public static bool IsWindows
        {
            get
            {
                if (!isWindows.HasValue)
                {
                    try
                    {
                        var isGdi = true;
#if STIDRAWING
                        isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif
                        if (isGdi)
                        {
                            using (Graphics gr = Graphics.FromImage(new Bitmap(1, 1)))
                            {
                                IntPtr graphicsPtr = gr.GetHdc();
                                var test = GetDeviceCaps(graphicsPtr, LOGPIXELSX);
                                gr.ReleaseHdc();
                                isWindows = true;
                            }
                        }

                        // * alternate method
                        //string windir = Environment.GetEnvironmentVariable("windir");
                        //if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
                        //{
                        //    isWindows = true;
                        //}
                    }
                    catch
                    {
                        isWindows = false;
                    }
                }
                return isWindows.GetValueOrDefault();
            }
        }

        public static bool IsLinux
        {
            get
            {
                if (!isLinux.HasValue)
                {
                    try
                    {
                        if (File.Exists(@"/proc/sys/kernel/ostype"))
                        {
                            string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                            if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                            {
                                // Note: Android gets here too
                                isLinux = true;
                            }
                        }
                    }
                    catch
                    {
                        isLinux = false;
                    }
                }
                return isLinux.GetValueOrDefault();
            }
        }

        public static bool IsMacOsX
        {
            get
            {
                if (!isMacOsX.HasValue)
                {
                    try
                    {
                        if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
                        {
                            // Note: iOS gets here too
                            isMacOsX = true;
                        }
                    }
                    catch
                    {
                        isMacOsX = false;
                    }
                }
                return isMacOsX.GetValueOrDefault();
            }
        }

        public static void CheckWysiwygScaling()
        {
            Base.Drawing.StiTextRenderer.StiDpiHelperGraphicsScale = NeedFontScaling ? GraphicsScale : 1;
        }
        #endregion

        #region Methods.Private
        private static void GetDpi()
        {
            try
            {
                using (Graphics gr = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    gr.PageUnit = GraphicsUnit.Pixel;
                    gr.PageScale = 1f;
                    IntPtr graphicsPtr = gr.GetHdc();
                    deviceCapsDpi = GetDeviceCaps(graphicsPtr, LOGPIXELSX);
                    //int dpiY = GetDeviceCaps(graphicsPtr, LOGPIXELSY);
                    gr.ReleaseHdc();
                    graphicsDpi = (int)gr.DpiX;
                    //int dpiY = (int)gr.DpiY;
                }
            }
            catch
            {
                deviceCapsDpi = 96;
                graphicsDpi = 96;
            }
            finally
            {
                if (deviceCapsDpi < 70 || deviceCapsDpi > 600) deviceCapsDpi = 96;
                if (graphicsDpi < 70 || graphicsDpi > 600) graphicsDpi = 96;
                graphicsRichTextDpi = graphicsDpi;
            }
            if ((deviceCapsDpi == 96) && (graphicsDpi == 96))
            {
                //Not WinXP scaling style
                int dpi = GetRegistryValue();
                if ((dpi != 96) && (dpi > 32) && (dpi < 600))
                {
                    //deviceCapsDpi = dpi;
                    //graphicsDpi = dpi;
                    graphicsRichTextDpi = dpi;
                }
            }
            if ((graphicsRichTextDpi == 96))
            {
                //Win10 only font scaling style
                int dpi = GetMetafileDpi();
                if ((dpi != 96) && (dpi > 32) && (dpi < 600))
                {
                    graphicsRichTextDpi = dpi;
                }
            }
            fontScaling = false;
            if (graphicsDpi != 96)
            {
                Font fnt = new Font("Arial", 1000, GraphicsUnit.Pixel);
                fontScaling = Math.Abs((fnt.SizeInPoints - 750) / fnt.SizeInPoints) > 0.01;
            }
        }

        private static Int32 GetMetafileDpi()
        {
            try
            {
                using (Graphics graph = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    IntPtr ptrGraph = graph.GetHdc();
                    try
                    {
                        GraphicsUnit grUnit = GraphicsUnit.Pixel;

                        Metafile mf1 = new Metafile(ptrGraph, StiOptions.Engine.RichTextDrawingMetafileType);
                        using (Graphics grfx = Graphics.FromImage(mf1))
                        {
                            grfx.DrawLine(Pens.Black, 0, 0, 10, 10);
                        }
                        var rect1 = mf1.GetBounds(ref grUnit);
                        MemoryStream ms = new MemoryStream();
                        Stimulsoft.Base.Drawing.StiMetafileSaver.Save(ms, mf1);
                        mf1.Dispose();

                        ms.Seek(0, SeekOrigin.Begin);
                        Metafile mf2 = Image.FromStream(ms) as Metafile;
                        var rect2 = mf2.GetBounds(ref grUnit);
                        mf2.Dispose();

                        return (int)Math.Round(rect2.Width / rect1.Width * 4, 0) * 24;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        graph.ReleaseHdc(ptrGraph);
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        private static Int32 GetRegistryValue()
        {
#if !BLAZOR
            try
            {
                //for Win8 and newer
                //if Win8DpiScaling==1 then LogPixels contains "System-wide scale factor"
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop");
                object result = null;
                if (key != null)
                {
                    result = key.GetValue("Win8DpiScaling");
                    if (result != null)
                    {
                        int tval = global::System.Convert.ToInt32(result);
                        if (tval == 1)
                        {
                            result = key.GetValue("LogPixels");
                            if (result != null)
                            {
                                int tdpi = global::System.Convert.ToInt32(result);
                                if (tdpi != 96) return tdpi;
                            }
                        }
                    }
                }

                #region Check for Win10 or newer
                //bool isWin10 = false;
                //var osVer = System.Environment.OSVersion;
                //if (osVer.Version.Major > 6) isWin10 = true;
                //if (osVer.Version.Major == 6 && osVer.Version.Minor >= 2)
                //{
                //    key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion");
                //    if (key != null)
                //    {
                //        result = key.GetValue("CurrentMajorVersionNumber");
                //        int tver = global::System.Convert.ToInt32(result);
                //        if (tver >= 10) isWin10 = true;
                //    }
                //}
                #endregion

                var subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
                if (subKey != null)
                {
                    result = subKey.GetValue("AppliedDPI");
                    if (result != null)
                    {
                        int tdpi = global::System.Convert.ToInt32(result);
                        if (tdpi != 96) return tdpi;
                    }
                }

                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\FontDPI");
                if (key != null)
                {
                    result = key.GetValue("LogPixels");
                    if (result != null)
                    {
                        int tdpi = global::System.Convert.ToInt32(result);
                        if (tdpi != 96) return tdpi;
                    }
                }

            }
            catch
            {
            }
#endif
            return 0;
        }
        #endregion
    }
}
