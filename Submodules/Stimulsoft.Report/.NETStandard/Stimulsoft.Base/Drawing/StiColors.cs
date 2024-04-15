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

using System.Drawing;

namespace Stimulsoft.Base.Drawing
{
#if NETSTANDARD
    using System = global::System;
#endif

    /// <summary>
    /// Colors of StiControls.
    /// </summary>
    public static class StiColors
    {
        #region Properties
        public static Color[] CustomColors { get; } =
        {
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 192, 192),
            Color.FromArgb(255, 224, 192),
            Color.FromArgb(255, 255, 192),
            Color.FromArgb(192, 255, 192),
            Color.FromArgb(192, 255, 255),
            Color.FromArgb(192, 192, 255),
            Color.FromArgb(255, 192, 255),
            Color.FromArgb(224, 224, 224),
            Color.FromArgb(255, 128, 128),
            Color.FromArgb(255, 192, 128),
            Color.FromArgb(255, 255, 128),
            Color.FromArgb(128, 255, 128),
            Color.FromArgb(128, 255, 255),
            Color.FromArgb(128, 128, 255),
            Color.FromArgb(255, 128, 255),
            Color.FromArgb(192, 192, 192),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(255, 128, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(192, 0, 0),
            Color.FromArgb(192, 64, 0),
            Color.FromArgb(192, 192, 0),
            Color.FromArgb(0, 192, 0),
            Color.FromArgb(0, 192, 192),
            Color.FromArgb(0, 0, 192),
            Color.FromArgb(192, 0, 192),
            Color.FromArgb(64, 64, 64),
            Color.FromArgb(128, 0, 0),
            Color.FromArgb(128, 64, 0),
            Color.FromArgb(128, 128, 0),
            Color.FromArgb(0, 128, 0),
            Color.FromArgb(0, 128, 128),
            Color.FromArgb(0, 0, 128),
            Color.FromArgb(128, 0, 128),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(64, 0, 0),
            Color.FromArgb(128, 64, 64),
            Color.FromArgb(64, 64, 0),
            Color.FromArgb(0, 64, 0),
            Color.FromArgb(0, 64, 64),
            Color.FromArgb(0, 0, 64),
            Color.FromArgb(64, 0, 64),

            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(255, 255, 255)
        };

        public static Color[] SystemColors { get; } =
        {
            System.Drawing.SystemColors.ActiveBorder,
            System.Drawing.SystemColors.ActiveCaption,
            System.Drawing.SystemColors.ActiveCaptionText,
            System.Drawing.SystemColors.AppWorkspace,
            System.Drawing.SystemColors.Control,
            System.Drawing.SystemColors.ControlDark,
            System.Drawing.SystemColors.ControlDarkDark,
            System.Drawing.SystemColors.ControlLight,
            System.Drawing.SystemColors.ControlLightLight,
            System.Drawing.SystemColors.ControlText,
            System.Drawing.SystemColors.Desktop,
            System.Drawing.SystemColors.GrayText,
            System.Drawing.SystemColors.Highlight,
            System.Drawing.SystemColors.HighlightText,
            System.Drawing.SystemColors.HotTrack,
            System.Drawing.SystemColors.InactiveBorder,
            System.Drawing.SystemColors.InactiveCaption,
            System.Drawing.SystemColors.InactiveCaptionText,
            System.Drawing.SystemColors.Info,
            System.Drawing.SystemColors.InfoText,
            System.Drawing.SystemColors.Menu,
            System.Drawing.SystemColors.MenuText,
            System.Drawing.SystemColors.ScrollBar,
            System.Drawing.SystemColors.Window,
            System.Drawing.SystemColors.WindowFrame,
            System.Drawing.SystemColors.WindowText
        };

        public static Color[] Colors { get; } =
        {
            Color.Transparent,
            Color.Black,
            Color.DimGray,
            Color.Gray,
            Color.DarkGray,
            Color.Silver,
            Color.LightGray,
            Color.Gainsboro,
            Color.WhiteSmoke,
            Color.White,
            Color.RosyBrown,
            Color.IndianRed,
            Color.Brown,
            Color.Firebrick,
            Color.LightCoral,
            Color.Maroon,
            Color.DarkRed,
            Color.Red,
            Color.Snow,
            Color.MistyRose,
            Color.Salmon,
            Color.Tomato,
            Color.DarkSalmon,
            Color.Coral,
            Color.OrangeRed,
            Color.LightSalmon,
            Color.Sienna,
            Color.SeaShell,
            Color.Chocolate,
            Color.SaddleBrown,
            Color.SandyBrown,
            Color.PeachPuff,
            Color.Peru,
            Color.Linen,
            Color.Bisque,
            Color.DarkOrange,
            Color.BurlyWood,
            Color.Tan,
            Color.AntiqueWhite,
            Color.NavajoWhite,
            Color.BlanchedAlmond,
            Color.PapayaWhip,
            Color.Moccasin,
            Color.Orange,
            Color.Wheat,
            Color.OldLace,
            Color.FloralWhite,
            Color.DarkGoldenrod,
            Color.Goldenrod,
            Color.Cornsilk,
            Color.Gold,
            Color.Khaki,
            Color.LemonChiffon,
            Color.PaleGoldenrod,
            Color.DarkKhaki,
            Color.Beige,
            Color.LightGoldenrodYellow,
            Color.Olive,
            Color.Yellow,
            Color.LightYellow,
            Color.Ivory,
            Color.OliveDrab,
            Color.YellowGreen,
            Color.DarkOliveGreen,
            Color.GreenYellow,
            Color.Chartreuse,
            Color.LawnGreen,
            Color.DarkSeaGreen,
            Color.ForestGreen,
            Color.LimeGreen,
            Color.LightGreen,
            Color.PaleGreen,
            Color.DarkGreen,
            Color.Green,
            Color.Lime,
            Color.Honeydew,
            Color.SeaGreen,
            Color.MediumSeaGreen,
            Color.SpringGreen,
            Color.MintCream,
            Color.MediumSpringGreen,
            Color.MediumAquamarine,
            Color.Aquamarine,
            Color.Turquoise,
            Color.LightSeaGreen,
            Color.MediumTurquoise,
            Color.DarkSlateGray,
            Color.PaleTurquoise,
            Color.Teal,
            Color.DarkCyan,
            Color.Aqua,
            Color.Cyan,
            Color.LightCyan,
            Color.Azure,
            Color.DarkTurquoise,
            Color.CadetBlue,
            Color.PowderBlue,
            Color.LightBlue,
            Color.DeepSkyBlue,
            Color.SkyBlue,
            Color.LightSkyBlue,
            Color.SteelBlue,
            Color.AliceBlue,
            Color.DodgerBlue,
            Color.SlateGray,
            Color.LightSlateGray,
            Color.LightSteelBlue,
            Color.CornflowerBlue,
            Color.RoyalBlue,
            Color.MidnightBlue,
            Color.Lavender,
            Color.Navy,
            Color.DarkBlue,
            Color.MediumBlue,
            Color.Blue,
            Color.GhostWhite,
            Color.SlateBlue,
            Color.DarkSlateBlue,
            Color.MediumSlateBlue,
            Color.MediumPurple,
            Color.BlueViolet,
            Color.Indigo,
            Color.DarkOrchid,
            Color.DarkViolet,
            Color.MediumOrchid,
            Color.Thistle,
            Color.Plum,
            Color.Violet,
            Color.Purple,
            Color.DarkMagenta,
            Color.Magenta,
            Color.Fuchsia,
            Color.Orchid,
            Color.MediumVioletRed,
            Color.DeepPink,
            Color.HotPink,
            Color.LavenderBlush,
            Color.PaleVioletRed,
            Color.Crimson,
            Color.Pink,
            Color.LightPink
        };

        public static string[] NameColors { get; } =
        {
            "Transparent",
            "Black",
            "DimGray",
            "Gray",
            "DarkGray",
            "Silver",
            "LightGray",
            "Gainsboro",
            "WhiteSmoke",
            "White",
            "RosyBrown",
            "IndianRed",
            "Brown",
            "Firebrick",
            "LightCoral",
            "Maroon",
            "DarkRed",
            "Red",
            "Snow",
            "MistyRose",
            "Salmon",
            "Tomato",
            "DarkSalmon",
            "Coral",
            "OrangeRed",
            "LightSalmon",
            "Sienna",
            "SeaShell",
            "Chocolate",
            "SaddleBrown",
            "SandyBrown",
            "PeachPuff",
            "Peru",
            "Linen",
            "Bisque",
            "DarkOrange",
            "BurlyWood",
            "Tan",
            "AntiqueWhite",
            "NavajoWhite",
            "BlanchedAlmond",
            "PapayaWhip",
            "Moccasin",
            "Orange",
            "Wheat",
            "OldLace",
            "FloralWhite",
            "DarkGoldenrod",
            "Goldenrod",
            "Cornsilk",
            "Gold",
            "Khaki",
            "LemonChiffon",
            "PaleGoldenrod",
            "DarkKhaki",
            "Beige",
            "LightGoldenrodYellow",
            "Olive",
            "Yellow",
            "LightYellow",
            "Ivory",
            "OliveDrab",
            "YellowGreen",
            "DarkOliveGreen",
            "GreenYellow",
            "Chartreuse",
            "LawnGreen",
            "DarkSeaGreen",
            "ForestGreen",
            "LimeGreen",
            "LightGreen",
            "PaleGreen",
            "DarkGreen",
            "Green",
            "Lime",
            "Honeydew",
            "SeaGreen",
            "MediumSeaGreen",
            "SpringGreen",
            "MintCream",
            "MediumSpringGreen",
            "MediumAquamarine",
            "Aquamarine",
            "Turquoise",
            "LightSeaGreen",
            "MediumTurquoise",
            "DarkSlateGray",
            "PaleTurquoise",
            "Teal",
            "DarkCyan",
            "Aqua",
            "Cyan",
            "LightCyan",
            "Azure",
            "DarkTurquoise",
            "CadetBlue",
            "PowderBlue",
            "LightBlue",
            "DeepSkyBlue",
            "SkyBlue",
            "LightSkyBlue",
            "SteelBlue",
            "AliceBlue",
            "DodgerBlue",
            "SlateGray",
            "LightSlateGray",
            "LightSteelBlue",
            "CornflowerBlue",
            "RoyalBlue",
            "MidnightBlue",
            "Lavender",
            "Navy",
            "DarkBlue",
            "MediumBlue",
            "Blue",
            "GhostWhite",
            "SlateBlue",
            "DarkSlateBlue",
            "MediumSlateBlue",
            "MediumPurple",
            "BlueViolet",
            "Indigo",
            "DarkOrchid",
            "DarkViolet",
            "MediumOrchid",
            "Thistle",
            "Plum",
            "Violet",
            "Purple",
            "DarkMagenta",
            "Magenta",
            "Fuchsia",
            "Orchid",
            "MediumVioletRed",
            "DeepPink",
            "HotPink",
            "LavenderBlush",
            "PaleVioletRed",
            "Crimson",
            "Pink",
            "LightPink"
        };

        /// <summary>
        /// Gets a Focus color.
        /// </summary>
        public static Color Focus { get; set; }

        /// <summary>
        /// Gets a ControlStart color.
        /// </summary>
        public static Color ControlStart { get; set; }

        /// <summary>
		/// Gets a ControlEnd color.
		/// </summary>
		public static Color ControlEnd { get; set; }

        /// <summary>
        /// Gets a Content color.
        /// </summary>
        public static Color Content { get; set; }

        /// <summary>
        /// Gets a Dark Content color.
        /// </summary>
        public static Color ContentDark { get; set; }

        /// <summary>
        /// Gets a ControlText color.
        /// </summary>
        public static Color ControlText { get; set; }

        /// <summary>
        /// Gets a Selected color.
        /// </summary>
        public static Color Selected { get; set; }

        /// <summary>
        /// Gets a SelectedText color.
        /// </summary>
        public static Color SelectedText { get; set; }

        /// <summary>
        /// Gets a ControlStartLight color.
        /// </summary>
        public static Color ControlStartLight { get; set; }

        /// <summary>
        /// Gets a ControlEndLight color.
        /// </summary>
        public static Color ControlEndLight { get; set; }

        /// <summary>
        /// Gets a ControlStartDark color.
        /// </summary>
        public static Color ControlStartDark { get; set; }

        /// <summary>
        /// Gets a ControlEndDark color.
        /// </summary>
        public static Color ControlEndDark { get; set; }

        /// <summary>
        /// Gets an ActiveCaptionStart color.
        /// </summary>
        public static Color ActiveCaptionStart { get; set; }

        /// <summary>
        /// Gets an ActiveCaptionEnd color.
        /// </summary>
        public static Color ActiveCaptionEnd { get; set; }

        public static Color InactiveCaptionStart { get; set; }

        public static Color InactiveCaptionEnd { get; set; }
        #endregion

        #region Methods
        public static void InitColors()
        {
            ActiveCaptionEnd = System.Drawing.SystemColors.ActiveCaption;
            InactiveCaptionEnd = System.Drawing.SystemColors.InactiveCaption;

            Focus = CalcColor(System.Drawing.SystemColors.Highlight, System.Drawing.SystemColors.Window, 70);
            Selected = CalcColor(System.Drawing.SystemColors.Highlight, System.Drawing.SystemColors.Window, 30);
            SelectedText = CalcColor(System.Drawing.SystemColors.Highlight, System.Drawing.SystemColors.Window, 220);
            Content = CalcColor(System.Drawing.SystemColors.Window, System.Drawing.SystemColors.Control, 200);
            ContentDark = StiColorUtils.Dark(StiColors.Content, 10);
            ControlStart = StiColorUtils.Light(System.Drawing.SystemColors.Control, 30);
            ControlEnd = StiColorUtils.Dark(System.Drawing.SystemColors.Control, 10);
            ControlText = System.Drawing.SystemColors.ControlText;

            ControlStartLight = StiColorUtils.Light(ControlStart, 20);
            ControlEndLight = StiColorUtils.Light(ControlEnd, 20);
            ControlStartDark = StiColorUtils.Dark(ControlStart, 20);
            ControlEndDark = StiColorUtils.Dark(ControlEnd, 20);

#if !NETSTANDARD
            ActiveCaptionStart = StiColorUtils.GetSysColor(Win32.ColorType.COLOR_GRADIENTACTIVECAPTION);
            InactiveCaptionStart = StiColorUtils.GetSysColor(Win32.ColorType.COLOR_GRADIENTINACTIVECAPTION);
#else
            ActiveCaptionStart = Color.FromArgb(255, 185, 209, 234);
            InactiveCaptionStart = Color.FromArgb(255, 215, 228, 242);
#endif
        }

        private static Color CalcColor(Color front, Color back, int alpha)
        {
            var frontColor = Color.FromArgb(255, front);
            var backColor = Color.FromArgb(255, back);

            var frontRed = frontColor.R;
            var frontGreen = frontColor.G;
            var frontBlue = frontColor.B;
            var backRed = backColor.R;
            var backGreen = backColor.G;
            var backBlue = backColor.B;

            var fRed = frontRed * alpha / 255 + backRed * ((float)(255 - alpha) / 255);
            var newRed = (byte)fRed;

            var fGreen = frontGreen * alpha / 255 + backGreen * ((float)(255 - alpha) / 255);
            var newGreen = (byte)fGreen;

            var fBlue = frontBlue * alpha / 255 + backBlue * ((float)(255 - alpha) / 255);
            var newBlue = (byte)fBlue;

            return Color.FromArgb(255, newRed, newGreen, newBlue);
        }
        #endregion

        static StiColors()
        {
            InitColors();
        }
    }
}
