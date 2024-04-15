#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Dashboards											}
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
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
#endif

namespace Stimulsoft.Base.Dashboard
{
    public class StiElementConsts
    {
        #region class Font
        public class FontConst
        {
            #region Properties
            public string Name { get; }

            public float Size { get; }

            public Color Color { get; }

            public Color SelectedColor = Color.White;

            public bool IsBold { get; }
            #endregion

            #region Methods
            public Font GetGdiFont(double zoom = 1f, float? fontSize = null, Font baseFont = null)
            {
                if (baseFont != null)
                {
                    return new Font(
                        baseFont.FontFamily.Name, fontSize != null ? fontSize.Value * (float)zoom : baseFont.Size * (float)zoom,
                        baseFont.Style);
                }
                else
                {
                    var style = IsBold ? FontStyle.Bold : FontStyle.Regular;
                    return new Font(Name, fontSize != null ? fontSize.Value * (float)zoom : Size * (float)zoom, style);
                }
            }

            public Font GetCachedGdiFont()
            {
                return cachedFont ?? (cachedFont = GetGdiFont());
            }
            #endregion

            #region Fields
            private Font cachedFont;
            #endregion

            public FontConst(string name, float size, Color color, bool isBold = false)
            {
                this.Name = name;
                this.Size = size;
                this.Color = color;
                this.IsBold = isBold;
            }
        }
        #endregion

        public static FontConst TitleFont = new FontConst("Arial", 12, Color.Gray);

        public static Color ForegroundColor = Color.DimGray;

        public static Color BackgroundColor = Color.White;

        public static class Table
        {
            public static FontConst Font = new FontConst("Arial", 10, Color.Black);

            public static Color BorderColor = Color.Gainsboro;

            public static int Height = 28;

            public static int GetHeight(Font font, double scale = 1d)
            {
                return (int)Math.Max(Height * scale, font.GetHeight() * scale * 0.8);
            }

            public static class CheckBoxCell
            {
                public static Color CheckColor = Color.Gray;

                public static Color IndeterminateCheckColor = Color.LightGray;

                public static Color SelectedBorderColor = Color.White;

                public static Color SelectedIndeterminateCheckColor = Color.White;
            }
        }

        public static class Highlight
        {
            public static Color Color = Color.FromArgb(0xff, 0x2b, 0x57, 0x9a);

            public static Color DarkColor = Color.FromArgb(0xff, 0x5b, 0x87, 0xca);
        }

        public static class DragDrop
        {
            public static FontConst Font = new FontConst("Arial", 8, Color.DimGray);
        }

        public static class ListBox
        {
            public static int ItemHeight = 32;

            public static int CheckBoxWidth = 20;
        }

        public static class ComboBox
        {
            public static int ItemHeight = 32;
        }

        public static class TreeView
        {
            public static int ItemHeight = 32;
        }
    }
}
