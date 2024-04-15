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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Stimulsoft.Base
{
    public static class StiScaleUI
    {
        #region Fields
        private static bool isInitialized;
        private static int lockCount = 0;
        #endregion

        #region Properties
        public static bool IsNoScaling => Id == SystemScaleID.x1;

        public static bool IsPrinting
        {
            set
            {
                if (lockCount > 0) return;

                if (value)
                {
                    isInitialized = true;
                    x = 1;
                    y = 1;
                }
                else
                {
                    isInitialized = false;
                    Initialize();
                }
            }
        }

        private static SystemScaleID id;
        public static SystemScaleID Id
        {
            get
            {
                Initialize();

                return id;
            }
        }

        private static SystemScaleIconID iconId;
        public static SystemScaleIconID IconId
        {
            get
            {
                Initialize();

                return iconId;
            }
        }

        private static double x;
        public static double X
        {
            get
            {
                Initialize();

                return x;
            }
        }

        private static double y;
        public static double Y
        {
            get
            {
                Initialize();

                return y;
            }
        }

        public static int I1 => (int)Math.Ceiling(Factor);

        public static int I2 => (int)Math.Ceiling(Factor * 2);

        public static int I3 => (int)Math.Ceiling(Factor * 3);

        public static int I4 => (int)Math.Ceiling(Factor * 4);

        public static int I5 => (int)Math.Ceiling(Factor * 5);

        public static int I6 => (int)Math.Ceiling(Factor * 6);

        public static int I7 => (int)Math.Ceiling(Factor * 7);

        public static int I8 => (int)Math.Ceiling(Factor * 8);

        public static int I9 => (int)Math.Ceiling(Factor * 9);

        public static int I10 => (int)Math.Ceiling(Factor * 10);

        public static int I11 => (int)Math.Ceiling(Factor * 11);

        public static int I12 => (int)Math.Ceiling(Factor * 12);

        public static int I13 => (int)Math.Ceiling(Factor * 13);

        public static int I14 => (int)Math.Ceiling(Factor * 14);

        public static int I15 => (int)Math.Ceiling(Factor * 15);

        public static int I16 => (int)Math.Ceiling(Factor * 16);

        public static int I17 => (int)Math.Ceiling(Factor * 17);

        public static int I18 => (int)Math.Ceiling(Factor * 18);

        public static int I19 => (int)Math.Ceiling(Factor * 19);

        public static int I20 => (int)Math.Ceiling(Factor * 20);

        public static double Factor => X;

        public static double System => 1.0 / X;

        public static double Step
        {
            get
            {
                if (Factor < 1.5)
                    return 1;

                else if (Factor < 2)
                    return 1.5;

                else
                    return 2;
            }
        }

        public static string StepName => GetStepName(Step);

        public static string BigStepName => GetStepName(Step * 2);
        #endregion

        #region Methods
        public static string ToIconName(SystemScaleIconID id)
        {
            switch (id)
            {
                case SystemScaleIconID.x100:
                    return string.Empty;

                case SystemScaleIconID.x125:
                    return "_x1_25";

                case SystemScaleIconID.x150:
                    return "_x1_5";

                case SystemScaleIconID.x175:
                    return "_x1_75";

                case SystemScaleIconID.x200:
                    return "_x2";

                case SystemScaleIconID.x225:
                    return "_x2_25";

                case SystemScaleIconID.x250:
                    return "_x2_5";

                case SystemScaleIconID.x275:
                    return "_x2_75";

                case SystemScaleIconID.x300:
                    return "_x3";

                case SystemScaleIconID.x325:
                    return "_x3_25";

                case SystemScaleIconID.x350:
                    return "_x3_5";

                case SystemScaleIconID.x375:
                    return "_x3_75";

                case SystemScaleIconID.x400:
                    return "_x4";
            }

            return string.Empty;
        }

        public static int I(int value)
        {
            return XXI(value);
        }

        public static Rectangle I(Rectangle rect)
        {
            return new Rectangle(XXI(rect.X), YYI(rect.Y), XXI(rect.Width), YYI(rect.Height));
        }

        public static Point I(Point point)
        {
            return new Point(XXI(point.X), YYI(point.Y));
        }

        public static Size I(Size size)
        {
            return SizeI(size.Width, size.Height);
        }

        public static SizeF I(SizeF size)
        {
            return new SizeF((float)XX(size.Width), (float)YY(size.Height));
        }

        public static Size SizeI(int width, int height)
        {
            return new Size(XXI(width), YYI(height));
        }

        public static Point PointI(int left, int top)
        {
            return new Point(XXI(left), YYI(top));
        }

        public static double XX(double value)
        {
            return X * value;
        }

        public static double YY(double value)
        {
            return Y * value;
        }

        public static int XXI(double value)
        {
            return (int)Math.Ceiling(X * value);
        }

        public static int YYI(double value)
        {
            return (int)Math.Ceiling(Y * value);
        }

        private static string GetStepName(double scale)
        {
            if (scale == 1.5)
                return "_x1_5";

            if (scale == 2)
                return "_x2";

            if (scale == 3)
                return "_x3";

            if (scale == 4)
                return "_x4";

            return "";
        }

        internal static void Lock()
        {
            lockCount++;

            x = 1;
            y = 1;
            id = SystemScaleID.x1;

            isInitialized = true;
        }

        internal static void Unlock()
        {
            lockCount--;
            if (lockCount == 0)
            {
                isInitialized = false;
                Initialize();
            }
        }

        private static void Initialize()
        {
            if (isInitialized) return;

            int deviceDpiX = 96;
            int deviceDpiY = 96;

            try
            {
                var dc = GetDC(IntPtr.Zero);
                if (dc != IntPtr.Zero)
                {
                    deviceDpiX = GetDeviceCaps(dc, 88);
                    deviceDpiY = GetDeviceCaps(dc, 90);

                    ReleaseDC(IntPtr.Zero, dc);
                }
            }
            catch
            {
            }

            x = deviceDpiX / 96d;
            y = deviceDpiY / 96d;

            var dpi = Math.Max(deviceDpiX, deviceDpiY);
            if (dpi <= 96)
            {
                id = SystemScaleID.x1;
                iconId = SystemScaleIconID.x100;
            }
            else if (dpi <= 192)
            {
                id = SystemScaleID.x2;
                if (dpi <= 120)
                    iconId = SystemScaleIconID.x125;
                else if (dpi <= 144)
                    iconId = SystemScaleIconID.x150;
                else if (dpi <= 168)
                    iconId = SystemScaleIconID.x175;
                else
                    iconId = SystemScaleIconID.x200;
            }
            else if (dpi <= 288)
            {
                id = SystemScaleID.x3;
                if (dpi <= 216)
                    iconId = SystemScaleIconID.x225;
                else if (dpi <= 240)
                    iconId = SystemScaleIconID.x250;
                else if (dpi <= 264)
                    iconId = SystemScaleIconID.x275;
                else
                    iconId = SystemScaleIconID.x300;
            }
            else
            {
                id = SystemScaleID.x4;
                if (dpi <= 312)
                    iconId = SystemScaleIconID.x325;
                else if (dpi <= 336)
                    iconId = SystemScaleIconID.x350;
                else if (dpi <= 360)
                    iconId = SystemScaleIconID.x375;
                else
                    iconId = SystemScaleIconID.x400;
            }

            isInitialized = true;
        }
        #endregion

        #region Methods.Imports
        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        #endregion
    }
}
