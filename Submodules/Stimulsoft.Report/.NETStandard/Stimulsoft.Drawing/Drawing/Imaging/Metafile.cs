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

using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Drawing;

namespace Stimulsoft.Drawing.Imaging
{
    [Serializable]
    public sealed class Metafile : Image
    {
        public IntPtr GetHenhmetafile()
        {
            return IntPtr.Zero;
        }

        public MetafileHeader GetMetafileHeader()
        {
            return null;
        }

        public RectangleF GetBounds(ref GraphicsUnit unit)
        {
            throw new NotImplementedException();
        }

        internal Metafile(Stream stream)
        {
        }

        public Metafile(IntPtr referenceHdc, System.Drawing.Imaging.EmfType emfType) :
            this(referenceHdc, new RectangleF(), System.Drawing.Imaging.MetafileFrameUnit.GdiCompatible, emfType, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc) :
            this(stream, referenceHdc, new RectangleF(), System.Drawing.Imaging.MetafileFrameUnit.GdiCompatible, System.Drawing.Imaging.EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type) :
            this(referenceHdc, frameRect, frameUnit, type, null)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit) :
            this(stream, referenceHdc, frameRect, frameUnit, System.Drawing.Imaging.EmfType.EmfPlusDual, null)
        {
        }

        public Metafile(IntPtr referenceHdc, RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string description)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, RectangleF frameRect, System.Drawing.Imaging.MetafileFrameUnit frameUnit, System.Drawing.Imaging.EmfType type, string description)
        {
        }

        public Metafile(Stream stream, IntPtr referenceHdc, object p, System.Drawing.Imaging.MetafileFrameUnit pixel) : this(stream, referenceHdc)
        {
        }
    }
}
