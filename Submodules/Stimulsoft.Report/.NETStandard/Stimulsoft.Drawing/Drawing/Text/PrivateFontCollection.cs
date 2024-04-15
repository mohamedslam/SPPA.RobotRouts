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
using System.IO;
using System.Runtime.InteropServices;

namespace Stimulsoft.Drawing.Text
{
    public sealed class PrivateFontCollection : FontCollection
    {
        internal SixLabors.Fonts.FontCollection sixFontCollection => (SixLabors.Fonts.FontCollection)sixFontCollectionBase;
        internal System.Drawing.Text.PrivateFontCollection netFontCollection => (System.Drawing.Text.PrivateFontCollection)netFontCollectionBase;

        public void AddFontFile(string filename)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFontCollection.AddFontFile(filename);
            else
                sixFontCollection.Add(filename);
        }

        public void AddMemoryFont(IntPtr memory, int length)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFontCollection.AddMemoryFont(memory, length);
            else
                throw new NotImplementedException();
        }

        public void AddFontBytesInternal(byte[] fontData)
        {
            var stream = new MemoryStream(fontData);
            sixFontCollection.Add(stream);
        }

        public PrivateFontCollection()
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netFontCollectionBase = new System.Drawing.Text.PrivateFontCollection();
            else
                sixFontCollectionBase = new SixLabors.Fonts.FontCollection();
        }
    }
}
