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

namespace Stimulsoft.Drawing.Imaging
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct EnhMetafileHeader
    {
        public int type;
        public int size;
        public Rectangle bounds;
        public Rectangle frame;
        public int signature;
        public int version;
        public int bytes;
        public int records;
        public short handles;
        public short reserved;
        public int description;
        public int off_description;
        public int palette_entires;
        public Size device;
        public Size millimeters;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct MonoMetafileHeader
    {
        [FieldOffset(0)]
        public System.Drawing.Imaging.MetafileType type;
        [FieldOffset(4)]
        public int size;
        [FieldOffset(8)]
        public int version;
        [FieldOffset(12)]
        public int emf_plus_flags;
        [FieldOffset(16)]
        public float dpi_x;
        [FieldOffset(20)]
        public float dpi_y;
        [FieldOffset(24)]
        public int x;
        [FieldOffset(28)]
        public int y;
        [FieldOffset(32)]
        public int width;
        [FieldOffset(36)]
        public int height;
        [FieldOffset(40)]
        public EnhMetafileHeader emf_header;
        [FieldOffset(128)]
        public int emfplus_header_size;
        [FieldOffset(132)]
        public int logical_dpi_x;
        [FieldOffset(136)]
        public int logical_dpi_y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public sealed class MetafileHeader
    {

        private MonoMetafileHeader header;

        //constructor

        internal MetafileHeader(IntPtr henhmetafile)
        {
            Marshal.PtrToStructure(henhmetafile, this);
        }

        // methods

        public bool IsDisplay()
        {
            return false;
        }

        public bool IsEmf()
        {
            return (Type == System.Drawing.Imaging.MetafileType.Emf);
        }

        public bool IsEmfOrEmfPlus()
        {
            return (Type >= System.Drawing.Imaging.MetafileType.Emf);
        }

        public bool IsEmfPlus()
        {
            return (Type >= System.Drawing.Imaging.MetafileType.EmfPlusOnly);
        }

        public bool IsEmfPlusDual()
        {
            return (Type == System.Drawing.Imaging.MetafileType.EmfPlusDual);
        }

        public bool IsEmfPlusOnly()
        {
            return (Type == System.Drawing.Imaging.MetafileType.EmfPlusOnly);
        }

        public bool IsWmf()
        {
            return (Type <= System.Drawing.Imaging.MetafileType.WmfPlaceable);
        }

        public bool IsWmfPlaceable()
        {
            return (Type == System.Drawing.Imaging.MetafileType.WmfPlaceable);
        }

        // properties

        public Rectangle Bounds
        {
            get { return new Rectangle(header.x, header.y, header.width, header.height); }
        }

        public float DpiX
        {
            get { return header.dpi_x; }
        }

        public float DpiY
        {
            get { return header.dpi_y; }
        }

        public int EmfPlusHeaderSize
        {
            get { return header.emfplus_header_size; }
        }

        public int LogicalDpiX
        {
            get { return header.logical_dpi_x; }
        }

        public int LogicalDpiY
        {
            get { return header.logical_dpi_y; }
        }

        public int MetafileSize
        {
            get { return header.size; }
        }

        public System.Drawing.Imaging.MetafileType Type
        {
            get { return header.type; }
        }

        public int Version
        {
            get { return header.version; }
        }
    }
}
