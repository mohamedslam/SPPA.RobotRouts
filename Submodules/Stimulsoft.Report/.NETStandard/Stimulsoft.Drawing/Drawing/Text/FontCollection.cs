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
using System.Linq;

namespace Stimulsoft.Drawing.Text
{
    public class FontCollection : IDisposable
    {
        internal SixLabors.Fonts.IReadOnlyFontCollection sixFontCollectionBase;
        internal System.Drawing.Text.FontCollection netFontCollectionBase;

        public FontFamily[] Families
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netFontCollectionBase.Families.Select(netFontFamily => new FontFamily(netFontFamily)).ToArray();
                else
                    return sixFontCollectionBase.Families.Select(sixFontFamily => new FontFamily(sixFontFamily)).ToArray();

            }
        }

        public void Dispose()
        {
        }
    }
}
