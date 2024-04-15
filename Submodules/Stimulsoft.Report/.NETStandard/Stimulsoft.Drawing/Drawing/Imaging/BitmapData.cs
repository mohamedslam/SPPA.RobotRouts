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
using System.Runtime.InteropServices;

namespace Stimulsoft.Drawing.Imaging
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class BitmapData
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public System.Drawing.Imaging.PixelFormat PixelFormat { get; set; }
        public int Reserved { get; set; }
        public IntPtr Scan0 { get; set; }
        public int Stride { get; set; }

        public static implicit operator BitmapData(System.Drawing.Imaging.BitmapData netBitmapData)
        {
            var bitmapData = new BitmapData();
            bitmapData.Height = netBitmapData.Height;
            bitmapData.Width = netBitmapData.Width;
            bitmapData.PixelFormat = netBitmapData.PixelFormat;
            bitmapData.Reserved = netBitmapData.Reserved;
            bitmapData.Scan0 = netBitmapData.Scan0;
            bitmapData.Stride = netBitmapData.Stride;
            return bitmapData;
        }

        public static implicit operator System.Drawing.Imaging.BitmapData(BitmapData bitmapData)
        {
            var netBitmapData = new System.Drawing.Imaging.BitmapData();
            netBitmapData.Height = bitmapData.Height;
            netBitmapData.Width = bitmapData.Width;
            netBitmapData.PixelFormat = (System.Drawing.Imaging.PixelFormat)bitmapData.PixelFormat;
            netBitmapData.Reserved = bitmapData.Reserved;
            netBitmapData.Scan0 = bitmapData.Scan0;
            netBitmapData.Stride = bitmapData.Stride;
            return netBitmapData;
        }
    }
}
