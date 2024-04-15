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

using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Text;

#if STIDRAWING
using PrivateFontCollection = Stimulsoft.Drawing.Text.PrivateFontCollection;
using Graphics = Stimulsoft.Drawing.Graphics;
using FontFamily = Stimulsoft.Drawing.FontFamily;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using StringFormat = Stimulsoft.Drawing.StringFormat;
#endif

namespace Stimulsoft.Report.Helpers
{
    public static class StiFontIconsHelper
    {
        #region Static
        private static StiFont? FONT;
        #endregion

        #region Fields
        private static PrivateFontCollection pfc = new PrivateFontCollection();
        #endregion

        #region Methods
        internal static Image ConvertFontIconToImage(StiFontIcons icon, Color color, int width, int height)
        {
            if (width < 1 || height < 1) return null;

            var fontFamilyIcons = GetFontFamilyIcons();
            var minSize = Math.Min(width, height);
            var image = new Bitmap(minSize, minSize);
            using (var g = Graphics.FromImage(image))
            {
                var fontSize = 1;
                using (var fontTemp = new Font(fontFamilyIcons, fontSize))
                {
                    var iconContent = GetContent(icon);
                    var sizeTemp = g.MeasureString(iconContent, fontTemp);

                    var maxSideSizeTemp = Math.Max(sizeTemp.Height, sizeTemp.Width);

                    fontSize = (int)(minSize / maxSideSizeTemp);

                    using (var font = new Font(fontFamilyIcons, fontSize))
                    using (var sf = StringFormat.GenericDefault.Clone() as StringFormat)
                    using (var brush = new SolidBrush(color))
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        g.DrawString(iconContent, font, brush, new Rectangle(0, 0, minSize, minSize), sf);
                    }
                }
            }

            return image;
        }

        public static FontFamily GetFontFamilyIcons()
        {
            if (pfc.Families.Length == 0)
            {
                using (var fontStream = typeof(StiFontIconsHelper).Assembly.GetManifestResourceStream("Stimulsoft.Base.FontIcons.Stimulsoft.ttf"))
                {
                    if (null == fontStream)
                        return null;

                    pfc.AddFontStream(fontStream);
                }
            }

            if (pfc.Families.Length == 0)
                return null;

            return pfc.Families[0];
        }

        public static float[] GetIconPadding(StiFontIcons fontIcons)
        {
            var isGdi = true;
#if STIDRAWING
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif
            if (!isGdi)
                return new float[] { 1f, 1f, 1f, 1f };
            
            switch (fontIcons)
            {
                case StiFontIcons.Fire: return new float[] { 14.67236f, 0.1445087f, 16.66666f, 11.12717f };//[left, top, right, bottom]
                case StiFontIcons.MarsStroke: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Latin5: return new float[] { 21.84397f, 1.878613f, 18.29787f, 15.17341f };
                case StiFontIcons.HackerNews: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Foursquare: return new float[] { 17.96043f, 0f, 19.93912f, 17.48555f };
                case StiFontIcons.Pinterest: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Superscript: return new float[] { 14.05622f, 7.514451f, 15.79652f, 23.84393f };
                case StiFontIcons.Download: return new float[] { 12.98865f, 0f, 15.13241f, 23.84393f };
                case StiFontIcons.MarsStrokeV: return new float[] { 15.67732f, 0f, 24.35312f, 11.27168f };
                case StiFontIcons.Latin4: return new float[] { 13.32561f, 1.878613f, 15.52723f, 15.17341f };
                case StiFontIcons.TencentWeibo: return new float[] { 18.72146f, 0f, 20.54794f, 11.12717f };
                case StiFontIcons.BlackTie: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Eye: return new float[] { 12.29117f, 19.07514f, 14.43914f, 23.84393f };
                case StiFontIcons.Trello: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.PinterestSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ArrowCircleODown: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Qq: return new float[] { 13.00716f, 0f, 15.15513f, 11.12717f };
                case StiFontIcons.Latin3: return new float[] { 13.42593f, 1.734104f, 15.50926f, 15.89595f };
                case StiFontIcons.Subscript: return new float[] { 14.05622f, 31.6474f, 15.79652f, 11.12717f };
                case StiFontIcons.Fonticons: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Female: return new float[] { 15.67732f, 1.589595f, 17.65601f, 11.12717f };
                case StiFontIcons.MarsStrokeH: return new float[] { 11.08719f, 19.07514f, 18.08396f, 23.84393f };
                case StiFontIcons.GooglePlusSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ArrowCircleOUp: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Weixin: return new float[] { 11.08719f, 3.179191f, 13.34769f, 14.30636f };
                case StiFontIcons.Latin2: return new float[] { 20.15209f, 1.878613f, 20.02534f, 15.17341f };
                case StiFontIcons.EyeSlash: return new float[] { 12.29117f, 14.30636f, 14.43914f, 19.07514f };
                case StiFontIcons.Male: return new float[] { 18.19788f, 1.589595f, 19.96466f, 11.12717f };
                case StiFontIcons.Neuter: return new float[] { 15.67732f, 0f, 24.35312f, 11.12717f };
                case StiFontIcons.QuarterHalf: return new float[] { 14.25261f, 0.5780347f, 15.52723f, 11.7052f };
                case StiFontIcons.PaperPlane: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.PaperPlaneO: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Eraser: return new float[] { 11.66478f, 12.71676f, 13.81654f, 23.84393f };
                case StiFontIcons.ExclamationTriangle: return new float[] { 12.29117f, 0f, 14.43914f, 17.48555f };
                case StiFontIcons.Inbox: return new float[] { 13.78849f, 12.71676f, 15.79652f, 23.84393f };
                case StiFontIcons.GooglePlus: return new float[] { 10.10795f, 8.092485f, 12.36507f, 19.21965f };
                case StiFontIcons.Gratipay: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Latin1: return new float[] { 28.61272f, 1.878613f, 30.49133f, 15.0289f };
                case StiFontIcons.QuarterQuarter: return new float[] { 14.71611f, 1.156069f, 15.52723f, 11.99422f };
                case StiFontIcons.PuzzlePiece: return new float[] { 12.98865f, 0f, 15.13241f, 22.10983f };
                case StiFontIcons.Genderless: return new float[] { 15.67732f, 19.07514f, 24.35312f, 23.84393f };
                case StiFontIcons.Plane: return new float[] { 14.67236f, 6.791907f, 16.95157f, 23.84393f };
                case StiFontIcons.SunO: return new float[] { 14.08115f, 0.1445087f, 16.10979f, 11.12717f };
                case StiFontIcons.PlayCircleO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.QuarterFull: return new float[] { 14.83198f, 1.300578f, 15.52723f, 11.84971f };
                case StiFontIcons.History: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Microphone: return new float[] { 16.83006f, 0f, 18.79085f, 17.48555f };
                case StiFontIcons.QuarterNone: return new float[] { 14.71611f, 1.156069f, 15.52723f, 11.99422f };
                case StiFontIcons.FacebookOfficial: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Money: return new float[] { 11.66478f, 12.71676f, 13.81654f, 23.84393f };
                case StiFontIcons.Calendar: return new float[] { 12.98865f, 0f, 15.13241f, 11.12717f };
                case StiFontIcons.QuarterThreeFourth: return new float[] { 14.94786f, 1.156069f, 15.52723f, 12.13873f };
                case StiFontIcons.Repeat: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.MoonO: return new float[] { 13.78849f, 6.50289f, 18.74163f, 17.48555f };
                case StiFontIcons.MicrophoneSlash: return new float[] { 15.24217f, 0f, 17.23647f, 17.48555f };
                case StiFontIcons.Rating4: return new float[] { 13.53383f, 0.867052f, 15.41354f, 11.7052f };
                case StiFontIcons.PinterestP: return new float[] { 15.67732f, 0f, 17.65601f, 17.48555f };
                case StiFontIcons.CircleThin: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Random: return new float[] { 12.29117f, 1.589595f, 14.43914f, 19.07514f };
                case StiFontIcons.Rating3: return new float[] { 13.90977f, 0.867052f, 15.41354f, 12.13873f };
                case StiFontIcons.Square4: return new float[] { 14.71611f, 0.867052f, 15.52723f, 12.13873f };
                case StiFontIcons.CaretDown: return new float[] { 18.19788f, 31.79191f, 19.96466f, 39.73988f };
                case StiFontIcons.Refresh: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Shield: return new float[] { 15.67732f, 6.358381f, 17.65601f, 17.48555f };
                case StiFontIcons.Whatsapp: return new float[] { 13.78849f, 6.069364f, 15.79652f, 17.19653f };
                case StiFontIcons.Archive: return new float[] { 14.91647f, 6.358381f, 17.06444f, 17.48555f };
                case StiFontIcons.Header: return new float[] { 14.79714f, 6.358381f, 16.94511f, 17.48555f };
                case StiFontIcons.Comment: return new float[] { 12.29117f, 12.71676f, 14.43914f, 11.12717f };
                case StiFontIcons.Rating2: return new float[] { 13.90977f, 0.867052f, 15.41354f, 12.13873f };
                case StiFontIcons.StarFull: return new float[] { 13.32561f, 1.300578f, 15.52723f, 12.86127f };
                case StiFontIcons.CalendarO: return new float[] { 12.98865f, 0f, 15.13241f, 11.12717f };
                case StiFontIcons.ListAlt: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Paragraph: return new float[] { 16.89498f, 6.358381f, 17.65601f, 17.48555f };
                case StiFontIcons.Server: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Bug: return new float[] { 14.37579f, 3.179191f, 16.51955f, 19.07514f };
                case StiFontIcons.Rating1: return new float[] { 14.1604f, 1.300578f, 15.41354f, 11.99422f };
                case StiFontIcons.CaretUp: return new float[] { 18.19788f, 34.9711f, 19.96466f, 36.56069f };
                case StiFontIcons.StarThreeFourth: return new float[] { 13.32561f, 1.300578f, 15.52723f, 12.86127f };
                case StiFontIcons.Magnet: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.FireExtinguisher: return new float[] { 14.67236f, 1.589595f, 16.66666f, 17.48555f };
                case StiFontIcons.Lock: return new float[] { 16.83006f, 6.358381f, 18.79085f, 23.84393f };
                case StiFontIcons.Sliders: return new float[] { 13.78849f, 12.71676f, 15.79652f, 17.48555f };
                case StiFontIcons.UserPlus: return new float[] { 11.08719f, 6.358381f, 13.34769f, 17.48555f };
                case StiFontIcons.Rating0: return new float[] { 14.28571f, 1.300578f, 15.41354f, 12.13873f };
                case StiFontIcons.StarHalf: return new float[] { 13.32561f, 1.300578f, 15.52723f, 12.86127f };
                case StiFontIcons.CaretLeft: return new float[] { 29.00232f, 19.07514f, 25.29002f, 30.20231f };
                case StiFontIcons.Rocket: return new float[] { 14.37579f, 6.358381f, 15.13241f, 12.71677f };
                case StiFontIcons.Vk: return new float[] { 11.66478f, 21.96532f, 13.81654f, 23.84393f };
                case StiFontIcons.ShareAlt: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Square0: return new float[] { 14.25261f, 0.867052f, 15.52723f, 11.56069f };
                case StiFontIcons.UserTimes: return new float[] { 11.08719f, 6.358381f, 13.67062f, 17.48555f };
                case StiFontIcons.FAFlag: return new float[] { 14.91647f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.StarQuarter: return new float[] { 13.32561f, 1.300578f, 15.52723f, 12.86127f };
                case StiFontIcons.ChevronUp: return new float[] { 15.99045f, 23.84393f, 18.13843f, 25f };
                case StiFontIcons.CaretRight: return new float[] { 23.89791f, 19.07514f, 30.39443f, 30.20231f };
                case StiFontIcons.Square1: return new float[] { 14.48436f, 0.867052f, 15.52723f, 11.84971f };
                case StiFontIcons.ShareAltSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Maxcdn: return new float[] { 12.29117f, 12.71676f, 15.87112f, 23.84393f };
                case StiFontIcons.Weibo: return new float[] { 12.29117f, 8.526011f, 14.43914f, 19.65318f };
                case StiFontIcons.StarNone: return new float[] { 13.32561f, 1.300578f, 15.52723f, 12.86127f };
                case StiFontIcons.Bed: return new float[] { 11.08719f, 12.71676f, 13.34769f, 23.84393f };
                case StiFontIcons.Headphones: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.Square2: return new float[] { 14.48436f, 0.867052f, 15.52723f, 11.84971f };
                case StiFontIcons.Bomb: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Columns: return new float[] { 12.98865f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.Viacoin: return new float[] { 13.78849f, 0f, 15.79652f, 11.27168f };
                case StiFontIcons.ArrowDown: return new float[] { 14.25261f, 0.2890173f, 15.6431f, 12.13873f };
                case StiFontIcons.Renren: return new float[] { 13.78849f, 7.080925f, 15.79652f, 17.48555f };
                case StiFontIcons.ChevronCircleLeft: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ChevronDown: return new float[] { 15.99045f, 26.44509f, 18.13843f, 22.25433f };
                case StiFontIcons.FutbolO: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Square3: return new float[] { 14.48436f, 0.867052f, 15.52723f, 11.84971f };
                case StiFontIcons.VolumeOff: return new float[] { 21.63865f, 14.30636f, 23.10925f, 25.43353f };
                case StiFontIcons.Train: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.ArrowRight: return new float[] { 14.60023f, 0.5780347f, 15.52723f, 12.28323f };
                case StiFontIcons.Sort: return new float[] { 18.19788f, 9.537572f, 19.96466f, 20.66474f };
                case StiFontIcons.Pagelines: return new float[] { 14.67236f, 0.1445087f, 16.95157f, 11.12717f };
                case StiFontIcons.ChevronCircleRight: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ArrowRightDown: return new float[] { 14.60023f, 1.011561f, 15.52723f, 11.84971f };
                case StiFontIcons.Subway: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Tty: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.VolumeDown: return new float[] { 16.83006f, 14.30636f, 18.79085f, 25.43353f };
                case StiFontIcons.Home: return new float[] { 13.32561f, 1.300578f, 15.52723f, 23.84393f };
                case StiFontIcons.Retweet: return new float[] { 11.66478f, 19.07514f, 13.81654f, 23.84393f };
                case StiFontIcons.ArrowRightUp: return new float[] { 14.60023f, 0.7225434f, 15.52723f, 12.13873f };
                case StiFontIcons.StackExchange: return new float[] { 16.74277f, 12.71676f, 18.72146f, 11.27168f };
                case StiFontIcons.Shirtsinbulk: return new float[] { 16.74277f, 2f, 18.72146f, 14.27168f };
                case StiFontIcons.Html5: return new float[] { 16.74277f, 12.71676f, 18.72146f, 14.27168f };
                case StiFontIcons.Binoculars: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.ChevronCircleUp: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SortDesc: return new float[] { 18.19788f, 50.86705f, 19.96466f, 20.66474f };
                case StiFontIcons.Medium: return new float[] { 12.29117f, 6.936416f, 14.43914f, 18.06358f };
                case StiFontIcons.VolumeUp: return new float[] { 12.98865f, 9.248555f, 15.13241f, 20.37572f };
                case StiFontIcons.Cart: return new float[] { 13.32561f, 1.878613f, 15.52723f, 14.88439f };
                case StiFontIcons.Plug: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.ArrowUp: return new float[] { 15.06373f, 1.156069f, 15.52723f, 12.28323f };
                case StiFontIcons.ShoppingCart: return new float[] { 12.98865f, 12.71676f, 15.13241f, 17.48555f };
                case StiFontIcons.ArrowCircleORight: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Qrcode: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.Phone: return new float[] { 13.97327f, 1.445087f, 16.03888f, 15.31792f };
                case StiFontIcons.YCombinator: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ChevronCircleDown: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Slideshare: return new float[] { 12.88783f, 0f, 15.0358f, 11.12717f };
                case StiFontIcons.SortAsc: return new float[] { 18.19788f, 9.537572f, 19.96466f, 61.99422f };
                case StiFontIcons.Check: return new float[] { 13.77315f, 2.456647f, 15.50926f, 22.39884f };
                case StiFontIcons.ArrowCircleOLeft: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Mobile: return new float[] { 24.46237f, 0.867052f, 25.40323f, 12.71677f };
                case StiFontIcons.Folder: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.Barcode: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Twitch: return new float[] { 16.34845f, 0f, 18.49642f, 17.48555f };
                case StiFontIcons.OptinMonster: return new float[] { 12.8937f, 0.1445087f, 15.15748f, 11.56069f };
                case StiFontIcons.Circle: return new float[] { 14.60023f, 0.867052f, 15.52723f, 11.99422f };
                case StiFontIcons.GgCircle: return new float[] { 14.60023f, 0.867052f, 15.52723f, 11.99422f };
                case StiFontIcons.Envelope: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.CaretSquareOLeft: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Tag: return new float[] { 13.78849f, 6.358381f, 16.7336f, 18.64162f };
                case StiFontIcons.Yelp: return new float[] { 17.67068f, 0f, 19.67872f, 11.12717f };
                case StiFontIcons.Mug: return new float[] { 13.32561f, 1.878613f, 15.52723f, 14.88439f };
                case StiFontIcons.Css3: return new float[] { 12.7685f, 6.358381f, 14.91647f, 14.45087f };
                case StiFontIcons.CircleCheck: return new float[] { 14.48436f, 0.7225434f, 15.52723f, 12.13873f };
                case StiFontIcons.Opencart: return new float[] { 10.20608f, 3.612717f, 12.36507f, 14.45087f };
                case StiFontIcons.FolderOpen: return new float[] { 11.66478f, 6.358381f, 15.40204f, 23.84393f };
                case StiFontIcons.Airplane: return new float[] { 14.13673f, 0.2890173f, 15.52723f, 11.84971f };
                case StiFontIcons.Tags: return new float[] { 11.66478f, 6.358381f, 14.60928f, 18.64162f };
                case StiFontIcons.DotCircleO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Linkedin: return new float[] { 13.78849f, 7.369942f, 15.79652f, 19.9422f };
                case StiFontIcons.NewspaperO: return new float[] { 11.08719f, 6.358381f, 13.34769f, 23.84393f };
                case StiFontIcons.CircleCross: return new float[] { 14.71611f, 0.7225434f, 15.52723f, 12.28323f };
                case StiFontIcons.Anchor: return new float[] { 12.29117f, 0f, 14.43914f, 11.99422f };
                case StiFontIcons.Expeditedssl: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.ArrowsV: return new float[] { 26.2605f, 0f, 27.73109f, 11.12717f };
                case StiFontIcons.CircleExclamation: return new float[] { 14.71611f, 0.867052f, 15.52723f, 12.13873f };
                case StiFontIcons.Book: return new float[] { 12.98865f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.Man: return new float[] { 36f, 0.867052f, 33.92f, 12.13873f };
                case StiFontIcons.StreetView: return new float[] { 36f, 0.867052f, 33.92f, 12.13873f };
                case StiFontIcons.Wheelchair: return new float[] { 12.98865f, 1.589595f, 16.64565f, 11.12717f };
                case StiFontIcons.Undo: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.UnlockAlt: return new float[] { 16.83006f, 0f, 18.79085f, 23.84393f };
                case StiFontIcons.Wifi: return new float[] { 12.59419f, 6.358381f, 14.85468f, 24.56647f };
                case StiFontIcons.Bookmark: return new float[] { 15.67732f, 6.358381f, 17.65601f, 18.64162f };
                case StiFontIcons.Cross: return new float[] { 14.83198f, 0.867052f, 15.52723f, 12.13873f };
                case StiFontIcons.Woman: return new float[] { 25.56818f, 0.2890173f, 26.13636f, 11.99422f };
                case StiFontIcons.BatteryFull: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.VimeoSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Bullseye: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Gavel: return new float[] { 13.96181f, 2.023121f, 15.27446f, 12.28323f };
                case StiFontIcons.Calculator: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.Rhomb: return new float[] { 13.32561f, 0.7225434f, 15.52723f, 10.54913f };
                case StiFontIcons.ArrowsH: return new float[] { 12.29117f, 28.61272f, 14.43914f, 39.73988f };
                case StiFontIcons.Print: return new float[] { 12.98865f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.Paypal: return new float[] { 14.19009f, 0f, 16.19813f, 11.12717f };
                case StiFontIcons.UserTie: return new float[] { 24.52107f, 1.300578f, 16.73052f, 15.46243f };
                case StiFontIcons.Exclamation: return new float[] { 30.11364f, 2.023121f, 31.53409f, 11.56069f };
                case StiFontIcons.Tachometer: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.Try: return new float[] { 16.83006f, 6.358381f, 18.79085f, 23.84393f };
                case StiFontIcons.GoogleWallet: return new float[] { 13.72315f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.BatteryThreeQuarters: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.BarChart: return new float[] { 11.08719f, 6.358381f, 13.34769f, 17.48555f };
                case StiFontIcons.Camera: return new float[] { 11.66478f, 0f, 13.81654f, 17.48555f };
                case StiFontIcons.Truck: return new float[] { 13.32561f, 4.046243f, 15.52723f, 23.84393f };
                case StiFontIcons.Flag: return new float[] { 21.7016f, 1.156069f, 16.2762f, 12.13873f };
                case StiFontIcons.CommentO: return new float[] { 12.29117f, 12.71676f, 14.43914f, 11.12717f };
                case StiFontIcons.EllipsisH: return new float[] { 14.67236f, 31.79191f, 16.66666f, 49.27746f };
                case StiFontIcons.PlusSquareO: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.CcVisa: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.Earth: return new float[] { 14.25261f, 0.5780347f, 15.52723f, 11.84971f };
                case StiFontIcons.TwitterSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.EllipsisV: return new float[] { 30.29412f, 6.358381f, 31.17647f, 23.84393f };
                case StiFontIcons.Font: return new float[] { 12.98865f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.BatteryHalf: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.CommentsO: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.CcMastercard: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.ManWoman: return new float[] { 14.48436f, 0.867052f, 15.52723f, 11.84971f };
                case StiFontIcons.Bolt: return new float[] { 19.76967f, 6.358381f, 21.49712f, 11.12717f };
                case StiFontIcons.SpaceShuttle: return new float[] { 10.57495f, 12.71676f, 12.83368f, 17.48555f };
                case StiFontIcons.RssSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Minus: return new float[] { 13.32561f, 32.08092f, 15.52723f, 42.63006f };
                case StiFontIcons.FacebookSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Bold: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.Underline: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.Appleinc: return new float[] { 22.26368f, 0.867052f, 16.41791f, 11.56069f };
                case StiFontIcons.CcDiscover: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.BatteryQuarter: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.Sitemap: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Slack: return new float[] { 12.98865f, 0f, 15.13241f, 17.48555f };
                case StiFontIcons.PlayCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Italic: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.Triangle: return new float[] { 13.44148f, 1.300578f, 15.52723f, 26.7341f };
                case StiFontIcons.CameraRetro: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Windows8: return new float[] { 13.97327f, 1.011561f, 16.03888f, 15.89595f };
                case StiFontIcons.CcAmex: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.Umbrella: return new float[] { 12.98865f, 3.179191f, 15.13241f, 17.48555f };
                case StiFontIcons.EnvelopeSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Ticket: return new float[] { 14.55847f, 2.745665f, 16.58711f, 13.87283f };
                case StiFontIcons.BatteryEmpty: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.Glass: return new float[] { 16.10979f, 6.358381f, 18.25776f, 11.12717f };
                case StiFontIcons.Key: return new float[] { 12.29117f, 6.358381f, 18.85442f, 14.73988f };
                case StiFontIcons.TextHeight: return new float[] { 12.29117f, 6.358381f, 14.55847f, 17.48555f };
                case StiFontIcons.Clipboard: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.TriangleDown: return new float[] { 13.32561f, 11.84971f, 15.52723f, 37.28324f };
                case StiFontIcons.CcPaypal: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.Wordpress: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Music: return new float[] { 13.78849f, 6.358381f, 15.79652f, 11.12717f };
                case StiFontIcons.TextWidth: return new float[] { 13.78849f, 6.358381f, 15.79652f, 11.41618f };
                case StiFontIcons.LightbulbO: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.MinusSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Cogs: return new float[] { 11.66478f, 0.867052f, 13.81654f, 11.99422f };
                case StiFontIcons.MousePointer: return new float[] { 15.67732f, 0f, 24.20091f, 11.12717f };
                case StiFontIcons.CcStripe: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.Openid: return new float[] { 12.29117f, 0f, 14.43914f, 17.48555f };
                case StiFontIcons.Search: return new float[] { 12.98865f, 6.358381f, 15.13241f, 11.12717f };
                case StiFontIcons.TriangleUp: return new float[] { 13.32561f, 11.99422f, 15.52723f, 37.28324f };
                case StiFontIcons.ICursor: return new float[] { 18.19788f, 0f, 27.56184f, 11.12717f };
                case StiFontIcons.InusSquareO: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.Exchange: return new float[] { 12.29117f, 14.30636f, 14.43914f, 19.07514f };
                case StiFontIcons.BellSlash: return new float[] { 11.6254f, 0f, 13.8859f, 11.12717f };
                case StiFontIcons.Comments: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.University: return new float[] { 11.08719f, 0f, 18.08396f, 11.12717f };
                case StiFontIcons.AlignLeft: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.ObjectGroup: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.Trophy: return new float[] { 15.26104f, 6.358381f, 17.26907f, 17.48555f };
                case StiFontIcons.TrashO: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.EnvelopeO: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.BellSlashO: return new float[] { 11.6254f, 0f, 13.8859f, 11.12717f };
                case StiFontIcons.LevelUp: return new float[] { 18.19788f, 6.50289f, 19.78799f, 23.84393f };
                case StiFontIcons.ThumbsOUp: return new float[] { 13.78849f, 0f, 15.79652f, 17.48555f };
                case StiFontIcons.Cloud: return new float[] { 11.66478f, 6.358381f, 13.81654f, 23.84393f };
                case StiFontIcons.CloudDownload: return new float[] { 11.66478f, 6.358381f, 13.81654f, 23.84393f };
                case StiFontIcons.AlignCenter: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.FileO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.ObjectUngroup: return new float[] { 10.10795f, 0f, 12.36507f, 11.12717f };
                case StiFontIcons.GraduationCap: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.Heart: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Trash: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.LevelDown: return new float[] { 18.19788f, 12.71676f, 19.78799f, 17.48555f };
                case StiFontIcons.ThumbsODown: return new float[] { 13.78849f, 6.358381f, 15.79652f, 11.12717f };
                case StiFontIcons.CloudUpload: return new float[] { 11.66478f, 6.358381f, 13.81654f, 23.84393f };
                case StiFontIcons.ClockO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.StickyNote: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Star: return new float[] { 12.98865f, 1.589595f, 15.13241f, 19.79769f };
                case StiFontIcons.Yahoo: return new float[] { 18.74163f, 0f, 20.74966f, 17.48555f };
                case StiFontIcons.Copyright: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.AlignRight: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.CheckSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.HeartO: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.UserMd: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.StarO: return new float[] { 12.98865f, 1.589595f, 15.13241f, 19.79769f };
                case StiFontIcons.StickyNoteO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.At: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Google: return new float[] { 13.78849f, 6.358381f, 17.13521f, 17.48555f };
                case StiFontIcons.Road: return new float[] { 13.59003f, 12.71676f, 15.74179f, 23.84393f };
                case StiFontIcons.Stethoscope: return new float[] { 14.67236f, 6.358381f, 16.66666f, 17.48555f };
                case StiFontIcons.PencilSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.AlignJustify: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.User: return new float[] { 15.67732f, 6.358381f, 17.65601f, 17.48555f };
                case StiFontIcons.Eyedropper: return new float[] { 12.29117f, 0f, 14.43914f, 11.27168f };
                case StiFontIcons.SignOut: return new float[] { 12.98865f, 12.71676f, 19.29382f, 23.84393f };
                case StiFontIcons.Reddit: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.CcJcb: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.List: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Suitcase: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.ExternalLinkSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Play: return new float[] { 14.67236f, 6.069364f, 16.66666f, 17.19653f };
                case StiFontIcons.PaintBrush: return new float[] { 12.29117f, 0f, 14.55847f, 11.12717f };
                case StiFontIcons.Film: return new float[] { 11.66478f, 6.358381f, 13.81654f, 11.12717f };
                case StiFontIcons.LinkedinSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.RedditSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.BellO: return new float[] { 14.91647f, 0f, 17.06444f, 11.12717f };
                case StiFontIcons.BirthdayCake: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Outdent: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.ThLarge: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.CcDinersClub: return new float[] { 10.10795f, 6.358381f, 12.36507f, 17.48555f };
                case StiFontIcons.ShareSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Pause: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ThumbTack: return new float[] { 16.83006f, 6.358381f, 18.79085f, 11.12717f };
                case StiFontIcons.StumbleuponCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.AreaChart: return new float[] { 11.08719f, 6.358381f, 13.34769f, 17.48555f };
                case StiFontIcons.Coffee: return new float[] { 11.66478f, 12.71676f, 16.30804f, 17.48555f };
                case StiFontIcons.Indent: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Clone: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Th: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Compass: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ExternalLink: return new float[] { 12.29117f, 0f, 14.43914f, 23.84393f };
                case StiFontIcons.Stop: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Cutlery: return new float[] { 14.67236f, 0f, 16.66666f, 11.12717f };
                case StiFontIcons.PieChart: return new float[] { 12.29117f, 0f, 17.06444f, 17.48555f };
                case StiFontIcons.VideoCamera: return new float[] { 12.29117f, 12.71676f, 14.43914f, 23.84393f };
                case StiFontIcons.FileTextO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.BalanceScale: return new float[] { 10.10795f, 0f, 16.68302f, 11.12717f };
                case StiFontIcons.Stumbleupon: return new float[] { 11.66478f, 8.092485f, 13.81654f, 19.21965f };
                case StiFontIcons.ThList: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.CaretSquareODown: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.LineChart: return new float[] { 11.08719f, 6.358381f, 13.34769f, 17.48555f };
                case StiFontIcons.Forward: return new float[] { 12.98865f, 6.358381f, 20.42875f, 17.48555f };
                case StiFontIcons.SignIn: return new float[] { 13.78849f, 12.71676f, 15.79652f, 23.84393f };
                case StiFontIcons.BuildingO: return new float[] { 14.67236f, 0f, 16.66666f, 11.12717f };
                case StiFontIcons.HourglassO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.PictureO: return new float[] { 11.66478f, 6.358381f, 13.81654f, 17.48555f };
                case StiFontIcons.Delicious: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.CaretSquareOUp: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.HospitalO: return new float[] { 14.67236f, 0f, 16.66666f, 11.12717f };
                case StiFontIcons.Times: return new float[] { 20.08547f, 18.20809f, 22.07977f, 22.97688f };
                case StiFontIcons.HourglassStart: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.FastForward: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Lastfm: return new float[] { 12.29117f, 21.09827f, 14.43914f, 23.84393f };
                case StiFontIcons.Pencil: return new float[] { 13.78849f, 7.514451f, 16.7336f, 17.48555f };
                case StiFontIcons.SearchPlus: return new float[] { 12.98865f, 6.358381f, 15.13241f, 11.12717f };
                case StiFontIcons.Ambulance: return new float[] { 14.15629f, 6.358381f, 13.81654f, 17.48555f };
                case StiFontIcons.HourglassHalf: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.CaretSquareORight: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.LastfmSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.StepForward: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.MapMarker: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.Digg: return new float[] { 11.08719f, 14.01734f, 13.34769f, 25.14451f };
                case StiFontIcons.GithubSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SearchMinus: return new float[] { 12.98865f, 6.358381f, 15.13241f, 11.12717f };
                case StiFontIcons.HourglassEnd: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Eur: return new float[] { 18.19788f, 6.358381f, 20.67138f, 23.84393f };
                case StiFontIcons.Medkit: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Adjust: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ToggleOff: return new float[] { 11.08719f, 12.71676f, 13.34769f, 23.84393f };
                case StiFontIcons.Upload: return new float[] { 12.98865f, 3.179191f, 15.13241f, 17.48555f };
                case StiFontIcons.PiedPiper: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.PowerOff: return new float[] { 13.78849f, 0f, 15.79652f, 17.48555f };
                case StiFontIcons.Eject: return new float[] { 13.77005f, 12.42775f, 15.7754f, 23.84393f };
                case StiFontIcons.Hourglass: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Gbp: return new float[] { 18.19788f, 6.358381f, 20.14134f, 23.84393f };
                case StiFontIcons.Tint: return new float[] { 18.19788f, 3.179191f, 19.96466f, 23.84393f };
                case StiFontIcons.FighterJet: return new float[] { 11.66478f, 19.07514f, 13.81654f, 23.84393f };
                case StiFontIcons.LemonO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Signal: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.PiedPiperAlt: return new float[] { 12.64865f, 0.5780347f, 14.91892f, 11.7052f };
                case StiFontIcons.ToggleOn: return new float[] { 11.08719f, 12.71676f, 13.34769f, 23.84393f };
                case StiFontIcons.Usd: return new float[] { 20.84806f, 0f, 22.61484f, 11.12717f };
                case StiFontIcons.ChevronLeft: return new float[] { 23.74429f, 1.300578f, 22.37443f, 18.78613f };
                case StiFontIcons.PencilSquareO: return new float[] { 12.29117f, 6.358381f, 14.79713f, 23.84393f };
                case StiFontIcons.HandRockO: return new float[] { 13.78849f, 12.71676f, 15.79652f, 17.48555f };
                case StiFontIcons.Beer: return new float[] { 15.76293f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.Cog: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Drupal: return new float[] { 13.78849f, 1.011561f, 15.79652f, 12.13873f };
                case StiFontIcons.SquareO: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.ShareSquareO: return new float[] { 12.98865f, 0f, 15.13241f, 23.84393f };
                case StiFontIcons.Inr: return new float[] { 19.7318f, 6.358381f, 21.45594f, 23.84393f };
                case StiFontIcons.ChevronRight: return new float[] { 20.39574f, 1.300578f, 25.72298f, 18.78613f };
                case StiFontIcons.HandPaperO: return new float[] { 12.29117f, 0f, 21.00239f, 11.12717f };
                case StiFontIcons.Bicycle: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.HSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.TimesCircleO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Joomla: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Bus: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Jpy: return new float[] { 18.16578f, 6.358381f, 19.92945f, 23.84393f };
                case StiFontIcons.BookmarkO: return new float[] { 15.67732f, 6.358381f, 17.65601f, 18.64162f };
                case StiFontIcons.CheckSquareO: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.HandScissorsO: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.PlusCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.PlusSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.CheckCircleO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Arrows: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Ioxhost: return new float[] { 11.08719f, 0.433526f, 13.34769f, 17.91908f };
                case StiFontIcons.Language: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Rub: return new float[] { 15.67732f, 6.358381f, 17.65601f, 23.84393f };
                case StiFontIcons.PhoneSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.HandLizardO: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.Ban: return new float[] { 13.78849f, 6.213873f, 15.79652f, 17.48555f };
                case StiFontIcons.Angellist: return new float[] { 16.28615f, 0f, 18.11263f, 11.12717f };
                case StiFontIcons.AngleDoubleLeft: return new float[] { 20.84806f, 22.97688f, 20.67138f, 27.74567f };
                case StiFontIcons.MinusCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.StepBackward: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.Fax: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.HandSpockO: return new float[] { 11.08719f, 0f, 18.08396f, 11.12717f };
                case StiFontIcons.FAArrowLeft: return new float[] { 16.7336f, 9.104046f, 15.79652f, 13.87283f };
                case StiFontIcons.Cc: return new float[] { 11.08719f, 6.358381f, 13.34769f, 17.48555f };
                case StiFontIcons.FastBackward: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Twitter: return new float[] { 15.0063f, 12.71676f, 17.15006f, 23.84393f };
                case StiFontIcons.Krw: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Building: return new float[] { 13.78849f, 0f, 21.68674f, 11.12717f };
                case StiFontIcons.AngleDoubleRight: return new float[] { 18.90459f, 22.97688f, 22.61484f, 27.74567f };
                case StiFontIcons.TimesCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.HandPointerO: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.FAArrowRight: return new float[] { 13.78849f, 9.104046f, 18.74163f, 13.87283f };
                case StiFontIcons.Btc: return new float[] { 18.56925f, 6.358381f, 18.72146f, 11.12717f };
                case StiFontIcons.Backward: return new float[] { 18.28499f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.Ils: return new float[] { 13.78849f, 6.358381f, 23.1593f, 23.84393f };
                case StiFontIcons.Facebook: return new float[] { 24.02827f, 0f, 23.85159f, 17.48555f };
                case StiFontIcons.Child: return new float[] { 19.02588f, 7.947977f, 21.00457f, 11.99422f };
                case StiFontIcons.HandPeaceO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.CheckCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.AngleDoubleUp: return new float[] { 21.07843f, 21.38728f, 23.03922f, 30.92486f };
                case StiFontIcons.FAArrowUp: return new float[] { 15.25851f, 9.537572f, 17.40227f, 17.48555f };
                case StiFontIcons.Meanpath: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.File: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Bell: return new float[] { 14.91647f, 0f, 17.06444f, 11.12717f };
                case StiFontIcons.Paw: return new float[] { 12.98865f, 6.358381f, 15.13241f, 17.48555f };
                case StiFontIcons.Github: return new float[] { 13.78849f, 6.358381f, 15.79652f, 19.36416f };
                case StiFontIcons.QuestionCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Buysellads: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.FAArrowDown: return new float[] { 15.25851f, 6.358381f, 17.40227f, 20.23122f };
                case StiFontIcons.FileImageO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.FileText: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.AngleDoubleDown: return new float[] { 21.07843f, 19.79769f, 23.03922f, 32.51445f };
                case StiFontIcons.Certificate: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Spoon: return new float[] { 26.2605f, 0f, 27.73109f, 11.12717f };
                case StiFontIcons.Trademark: return new float[] { 11.41907f, 25.43353f, 13.63636f, 23.84393f };
                case StiFontIcons.Connectdevelop: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.InfoCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SortAlphaAsc: return new float[] { 14.37579f, 0f, 15.25851f, 11.12717f };
                case StiFontIcons.Share: return new float[] { 12.29117f, 3.179191f, 14.43914f, 17.48555f };
                case StiFontIcons.Unlock: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.HandORight: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.AngleLeft: return new float[] { 27.37819f, 22.97688f, 26.45012f, 27.74567f };
                case StiFontIcons.Cube: return new float[] { 12.29117f, 6.358381f, 19.68974f, 11.12717f };
                case StiFontIcons.Cubes: return new float[] { 12.29117f, 0f, 15.79f, 11.12717f };
                case StiFontIcons.Dashcube: return new float[] { 13.78849f, 0.1445087f, 21.68674f, 11.12717f };
                case StiFontIcons.Registered: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Crosshairs: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Expand: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SortAlphaDesc: return new float[] { 14.37579f, 0f, 15.25851f, 11.12717f };
                case StiFontIcons.HandOLeft: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.CreditCard: return new float[] { 11.66478f, 6.358381f, 13.81654f, 17.48555f };
                case StiFontIcons.AngleRight: return new float[] { 24.82599f, 22.97688f, 29.00232f, 27.74567f };
                case StiFontIcons.CreativeCommons: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Forumbee: return new float[] { 13.78849f, 6.50289f, 15.79652f, 17.77457f };
                case StiFontIcons.SortAmountAsc: return new float[] { 13.60382f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.HandOUp: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Users: return new float[] { 11.66478f, 0f, 13.81654f, 11.12717f };
                case StiFontIcons.Compress: return new float[] { 14.32396f, 7.080925f, 16.33199f, 18.20809f };
                case StiFontIcons.Rss: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.HandODown: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.SortAmountDesc: return new float[] { 13.60382f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Leanpub: return new float[] { 11.08719f, 13.15029f, 13.34769f, 17.48555f };
                case StiFontIcons.AngleUp: return new float[] { 21.07843f, 34.10405f, 23.03922f, 37.28324f };
                case StiFontIcons.AngleDown: return new float[] { 21.07843f, 34.10405f, 23.03922f, 37.28324f };
                case StiFontIcons.Link: return new float[] { 13.61917f, 0.867052f, 15.76292f, 18.3526f };
                case StiFontIcons.Behance: return new float[] { 11.08719f, 12.71676f, 13.34769f, 23.84393f };
                case StiFontIcons.FAPlus: return new float[] { 14.67236f, 6.358381f, 16.66666f, 23.84393f };
                case StiFontIcons.Gg: return new float[] { 13.45533f, 11.12717f, 15.82346f, 22.25433f };
                case StiFontIcons.ArrowCircleLeft: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SortNumericAsc: return new float[] { 15.26104f, 0f, 18.07229f, 11.12717f };
                case StiFontIcons.SortNumericDesc: return new float[] { 15.26104f, 0f, 18.07229f, 11.12717f };
                case StiFontIcons.HddO: return new float[] { 13.78849f, 12.71676f, 15.79652f, 23.84393f };
                case StiFontIcons.Sellsy: return new float[] { 11.08719f, 8.236994f, 13.34769f, 19.36416f };
                case StiFontIcons.BehanceSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ArrowCircleRight: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.FAMinus: return new float[] { 14.67236f, 31.79191f, 16.66666f, 49.27746f };
                case StiFontIcons.Bullhorn: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Desktop: return new float[] { 11.66478f, 0f, 13.81654f, 17.48555f };
                case StiFontIcons.ArrowCircleUp: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ThumbsUp: return new float[] { 12.98865f, 6.358381f, 17.90668f, 17.48555f };
                case StiFontIcons.Simplybuilt: return new float[] { 11.08719f, 5.780347f, 13.34769f, 16.90752f };
                case StiFontIcons.Steam: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Flask: return new float[] { 17.40227f, 6.358381f, 19.54603f, 17.48555f };
                case StiFontIcons.Asterisk: return new float[] { 18.03279f, 6.358381f, 20.17654f, 17.48555f };
                case StiFontIcons.Tripadvisor: return new float[] { 10.10795f, 10.98266f, 12.36507f, 22.10983f };
                case StiFontIcons.FrownO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ArrowCircleDown: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Laptop: return new float[] { 11.66478f, 12.71676f, 13.81654f, 23.84393f };
                case StiFontIcons.ExclamationCircle: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.SteamSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.ThumbsDown: return new float[] { 12.98865f, 12.71676f, 17.90668f, 11.12717f };
                case StiFontIcons.Skyatlas: return new float[] { 11.08719f, 14.73988f, 13.34769f, 25.86705f };
                case StiFontIcons.Odnoklassniki: return new float[] { 21.00457f, 0f, 22.98325f, 11.12717f };
                case StiFontIcons.Scissors: return new float[] { 12.29117f, 12.71676f, 14.43914f, 17.48555f };
                case StiFontIcons.Globe: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.MehO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Tablet: return new float[] { 16.83006f, 6.358381f, 18.79085f, 23.84393f };
                case StiFontIcons.Gift: return new float[] { 13.78849f, 9.537572f, 15.79652f, 23.84393f };
                case StiFontIcons.Recycle: return new float[] { 13.12649f, 0f, 15.0358f, 15.60693f };
                case StiFontIcons.CartPlus: return new float[] { 12.98865f, 12.71676f, 15.13241f, 17.48555f };
                case StiFontIcons.YoutubeSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Wrench: return new float[] { 13.87137f, 6.358381f, 15.25851f, 12.28323f };
                case StiFontIcons.FilesO: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.OdnoklassnikiSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.CircleO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Leaf: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Gamepad: return new float[] { 11.66478f, 25.43353f, 13.81654f, 23.84393f };
                case StiFontIcons.Youtube: return new float[] { 14.99331f, 0f, 17.00134f, 11.12717f };
                case StiFontIcons.Car: return new float[] { 11.08719f, 6.358381f, 13.34769f, 14.30636f };
                case StiFontIcons.CartArrowDown: return new float[] { 12.98865f, 12.71676f, 15.13241f, 17.48555f };
                case StiFontIcons.Tasks: return new float[] { 12.29117f, 6.358381f, 14.43914f, 23.84393f };
                case StiFontIcons.Paperclip: return new float[] { 14.81481f, 6.647399f, 16.80912f, 17.77457f };
                case StiFontIcons.QuoteLeft: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.GetPocket: return new float[] { 12.66913f, 6.358381f, 14.88315f, 17.48555f };
                case StiFontIcons.Flickr: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Diamond: return new float[] { 11.08719f, 6.358381f, 13.34769f, 11.12717f };
                case StiFontIcons.Xing: return new float[] { 14.67236f, 0.5780347f, 16.66666f, 17.48555f };
                case StiFontIcons.Filter: return new float[] { 14.67236f, 12.71676f, 16.52422f, 17.48555f };
                case StiFontIcons.Taxi: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.QuoteRight: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.FloppyO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Ship: return new float[] { 11.84069f, 0f, 14.10118f, 11.12717f };
                case StiFontIcons.Adn: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.KeyboardO: return new float[] { 11.66478f, 19.07514f, 13.81654f, 23.84393f };
                case StiFontIcons.WikipediaW: return new float[] { 10.10795f, 7.658959f, 12.36507f, 18.78613f };
                case StiFontIcons.Briefcase: return new float[] { 12.29117f, 0f, 14.43914f, 23.84393f };
                case StiFontIcons.Tree: return new float[] { 15.26104f, 0f, 17.26907f, 11.12717f };
                case StiFontIcons.XingSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Spinner: return new float[] { 14.91647f, 0f, 17.06444f, 14.30636f };
                case StiFontIcons.Bitbucket: return new float[] { 14.67236f, 4.190752f, 16.66666f, 15.17341f };
                case StiFontIcons.UserSecret: return new float[] { 13.78849f, 0f, 21.68674f, 17.48555f };
                case StiFontIcons.Square: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Safari: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.ArrowsAlt: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.FlagO: return new float[] { 14.91647f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Spotify: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Reply: return new float[] { 12.29117f, 3.179191f, 14.43914f, 17.48555f };
                case StiFontIcons.BitbucketSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.YoutubePlay: return new float[] { 12.29117f, 13.2948f, 14.43914f, 24.42197f };
                case StiFontIcons.FileExcelO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Chrome: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Bars: return new float[] { 13.78849f, 12.71676f, 15.79652f, 23.84393f };
                case StiFontIcons.Deviantart: return new float[] { 18.19788f, 0f, 19.96466f, 17.48555f };
                case StiFontIcons.Motorcycle: return new float[] { 10.10795f, 12.71676f, 12.36507f, 23.84393f };
                case StiFontIcons.Tumblr: return new float[] { 22.26148f, 0f, 19.96466f, 17.48555f };
                case StiFontIcons.GithubAlt: return new float[] { 12.98865f, 12.71676f, 15.13241f, 17.48555f };
                case StiFontIcons.FilePowerpointO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.FlagCheckered: return new float[] { 14.91647f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Dropbox: return new float[] { 14.91647f, 5.924856f, 17.06444f, 17.63006f };
                case StiFontIcons.Firefox: return new float[] { 12.29117f, 2.890173f, 14.43914f, 11.12717f };
                case StiFontIcons.TumblrSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.FolderO: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.ListUl: return new float[] { 12.29117f, 9.537572f, 14.43914f, 20.66474f };
                case StiFontIcons.StackOverflow: return new float[] { 14.32396f, 0.1445087f, 16.33199f, 11.12717f };
                case StiFontIcons.FileArchiveO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Heartbeat: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.LongArrowDown: return new float[] { 21.63865f, 0f, 23.31933f, 14.30636f };
                case StiFontIcons.Opera: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Terminal: return new float[] { 13.49306f, 22.97688f, 15.13241f, 23.84393f };
                case StiFontIcons.Soundcloud: return new float[] { 10.10795f, 19.65318f, 12.36507f, 30.78035f };
                case StiFontIcons.FileAudioO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.FolderOpenO: return new float[] { 11.66478f, 6.358381f, 14.26954f, 23.84393f };
                case StiFontIcons.ListOl: return new float[] { 12.88783f, 0.433526f, 14.43914f, 11.12717f };
                case StiFontIcons.LongArrowUp: return new float[] { 21.63865f, 3.179191f, 23.31933f, 11.12717f };
                case StiFontIcons.Venus: return new float[] { 15.67732f, 0.1445087f, 24.35312f, 11.12717f };
                case StiFontIcons.Instagram: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.InternetExplorer: return new float[] { 12.29117f, 0.867052f, 14.43914f, 11.99422f };
                case StiFontIcons.FileVideoO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Database: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.SmileO: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Strikethrough: return new float[] { 12.29117f, 6.358381f, 14.43914f, 17.48555f };
                case StiFontIcons.Mars: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Code: return new float[] { 13.36353f, 13.58381f, 15.51529f, 18.3526f };
                case StiFontIcons.FilePdfO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.FileCodeO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.Contao: return new float[] { 13.12649f, 6.358381f, 15.27446f, 17.48555f };
                case StiFontIcons.Television: return new float[] { 11.08719f, 6.358381f, 18.08396f, 17.48555f };
                case StiFontIcons.LongArrowLeft: return new float[] { 14.91647f, 25.43353f, 14.43914f, 36.56069f };
                case StiFontIcons.Mercury: return new float[] { 15.67732f, 0f, 24.35312f, 11.12717f };
                case StiFontIcons.FileWordO: return new float[] { 13.78849f, 0f, 15.79652f, 11.12717f };
                case StiFontIcons.ReplyAll: return new float[] { 12.29117f, 3.179191f, 14.43914f, 17.48555f };
                case StiFontIcons.Vine: return new float[] { 15.52878f, 0f, 17.53681f, 17.63006f };
                case StiFontIcons.Px500: return new float[] { 14.45783f, 0f, 21.68674f, 11.12717f };
                case StiFontIcons.Transgender: return new float[] { 13.78849f, 0f, 21.68674f, 11.12717f };
                case StiFontIcons.Table: return new float[] { 12.98865f, 6.358381f, 15.13241f, 23.84393f };
                case StiFontIcons.Codepen: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.StarHalfO: return new float[] { 12.98865f, 1.589595f, 15.13241f, 19.79769f };
                case StiFontIcons.Amazon: return new float[] { 13.48449f, 0f, 15.51313f, 11.12717f };
                case StiFontIcons.TransgenderAlt: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.LongArrowRight: return new float[] { 12.29117f, 25.43353f, 17.06444f, 36.56069f };
                case StiFontIcons.Magic: return new float[] { 14.12358f, 0.1445087f, 15.25851f, 18.93063f };
                case StiFontIcons.CalendarPlusO: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.LocationArrow: return new float[] { 14.67236f, 12.71676f, 16.66666f, 17.48555f };
                case StiFontIcons.Jsfiddle: return new float[] { 11.08719f, 6.791907f, 13.34769f, 23.84393f };
                case StiFontIcons.VenusDouble: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Apple: return new float[] { 14.67236f, 0.1445087f, 17.37891f, 17.48555f };
                case StiFontIcons.LifeRing: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.MarsDouble: return new float[] { 11.66478f, 0f, 13.81654f, 11.27168f };
                case StiFontIcons.CalendarMinusO: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.Crop: return new float[] { 12.98865f, 6.358381f, 15.13241f, 11.12717f };
                case StiFontIcons.Windows: return new float[] { 12.98865f, 6.358381f, 15.13241f, 11.27168f };
                case StiFontIcons.VenusMars: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.CircleONotch: return new float[] { 13.60382f, 2.16763f, 15.75179f, 12.71677f };
                case StiFontIcons.CalendarTimesO: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.CodeFork: return new float[] { 18.19788f, 6.358381f, 19.96466f, 17.48555f };
                case StiFontIcons.Android: return new float[] { 14.67236f, 6.358381f, 16.66666f, 11.12717f };
                case StiFontIcons.Rebel: return new float[] { 13.00716f, 0f, 15.15513f, 11.12717f };
                case StiFontIcons.CalendarCheckO: return new float[] { 12.29117f, 0f, 19.68974f, 11.12717f };
                case StiFontIcons.Linux: return new float[] { 14.19009f, 0f, 16.06425f, 11.27168f };
                case StiFontIcons.ChainBroken: return new float[] { 12.98865f, 0f, 15.13241f, 17.48555f };
                case StiFontIcons.Empire: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Industry: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Dribbble: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Question: return new float[] { 24.02827f, 12.71676f, 20.14134f, 23.84393f };
                case StiFontIcons.MapPin: return new float[] { 18.19788f, 0f, 19.96466f, 11.12717f };
                case StiFontIcons.GitSquare: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.Info: return new float[] { 23.89791f, 6.358381f, 25.29002f, 23.84393f };
                case StiFontIcons.Skype: return new float[] { 13.78849f, 6.358381f, 15.79652f, 17.48555f };
                case StiFontIcons.MapSigns: return new float[] { 13.84248f, 0f, 15.99046f, 11.12717f };
                case StiFontIcons.Git: return new float[] { 15.0358f, 0f, 17.18377f, 11.12717f };
                case StiFontIcons.MapO: return new float[] { 11.08719f, 0f, 13.34769f, 11.12717f };
                case StiFontIcons.Map: return new float[] { 12.29117f, 0f, 14.43914f, 11.12717f };
                case StiFontIcons.Commenting: return new float[] { 12.29117f, 12.71676f, 14.43914f, 12.71677f };
                case StiFontIcons.CommentingO: return new float[] { 12.29117f, 12.71676f, 14.43914f, 11.12717f };
                case StiFontIcons.Houzz: return new float[] { 18.19788f, 0.5780347f, 19.96466f, 11.7052f };
                case StiFontIcons.Vimeo: return new float[] { 15.63246f, 9.537572f, 17.78043f, 20.66474f };

            }

            return null;
        }

        public static string GetContent(StiFontIcons? fontIcons)
        {
            switch (fontIcons)
            {
                case StiFontIcons.ArrowDown: return '\ue900'.ToString();
                case StiFontIcons.ArrowRight: return '\ue901'.ToString();
                case StiFontIcons.ArrowRightDown: return '\ue902'.ToString();
                case StiFontIcons.ArrowRightUp: return '\ue903'.ToString();
                case StiFontIcons.ArrowUp: return '\ue904'.ToString();
                case StiFontIcons.Check: return '\ue905'.ToString();
                case StiFontIcons.Circle: return '\ue906'.ToString();
                case StiFontIcons.CircleCheck: return '\ue907'.ToString();
                case StiFontIcons.CircleCross: return '\ue908'.ToString();
                case StiFontIcons.CircleExclamation: return '\ue909'.ToString();
                case StiFontIcons.Cross: return '\ue90a'.ToString();
                case StiFontIcons.Exclamation: return '\ue90b'.ToString();
                case StiFontIcons.Flag: return '\ue90c'.ToString();
                case StiFontIcons.Latin1: return '\ue90d'.ToString();
                case StiFontIcons.Latin2: return '\ue90e'.ToString();
                case StiFontIcons.Latin3: return '\ue90f'.ToString();
                case StiFontIcons.Latin4: return '\ue910'.ToString();
                case StiFontIcons.Latin5: return '\ue92b'.ToString();
                case StiFontIcons.Minus: return '\ue911'.ToString();
                case StiFontIcons.QuarterFull: return '\ue912'.ToString();
                case StiFontIcons.QuarterHalf: return '\ue913'.ToString();
                case StiFontIcons.QuarterNone: return '\ue914'.ToString();
                case StiFontIcons.QuarterQuarter: return '\ue915'.ToString();
                case StiFontIcons.QuarterThreeFourth: return '\ue916'.ToString();
                case StiFontIcons.Rating0: return '\ue917'.ToString();
                case StiFontIcons.Rating1: return '\ue918'.ToString();
                case StiFontIcons.Rating2: return '\ue919'.ToString();
                case StiFontIcons.Rating3: return '\ue91a'.ToString();
                case StiFontIcons.Rating4: return '\ue91b'.ToString();
                case StiFontIcons.Rhomb: return '\ue91c'.ToString();
                case StiFontIcons.Square0: return '\ue91d'.ToString();
                case StiFontIcons.Square1: return '\ue91e'.ToString();
                case StiFontIcons.Square2: return '\ue91f'.ToString();
                case StiFontIcons.Square3: return '\ue920'.ToString();
                case StiFontIcons.Square4: return '\ue921'.ToString();
                case StiFontIcons.StarFull: return '\ue922'.ToString();
                case StiFontIcons.StarHalf: return '\ue923'.ToString();
                case StiFontIcons.StarNone: return '\ue924'.ToString();
                case StiFontIcons.StarQuarter: return '\ue925'.ToString();
                case StiFontIcons.StarThreeFourth: return '\ue926'.ToString();
                case StiFontIcons.Triangle: return '\ue927'.ToString();
                case StiFontIcons.TriangleDown: return '\ue928'.ToString();
                case StiFontIcons.TriangleUp: return '\ue929'.ToString();
                case StiFontIcons.Home: return '\ue92a'.ToString();
                case StiFontIcons.Cart: return '\ue93a'.ToString();
                case StiFontIcons.Phone: return '\ue942'.ToString();
                case StiFontIcons.UserTie: return '\ue976'.ToString();
                case StiFontIcons.Mobile: return '\ue958'.ToString();
                case StiFontIcons.Mug: return '\ue9a2'.ToString();
                case StiFontIcons.Airplane: return '\ue9af'.ToString();
                case StiFontIcons.Truck: return '\ue9b0'.ToString();
                case StiFontIcons.Earth: return '\ue9ca'.ToString();
                case StiFontIcons.Man: return '\ue9dc'.ToString();
                case StiFontIcons.Woman: return '\ue9dd'.ToString();
                case StiFontIcons.ManWoman: return '\ue9de'.ToString();
                case StiFontIcons.Appleinc: return '\ueabe'.ToString();
                case StiFontIcons.Windows8: return '\ueac2'.ToString();

                case StiFontIcons.Glass: return '\uf000'.ToString();
                case StiFontIcons.Music: return '\uf001'.ToString();
                case StiFontIcons.Search: return '\uf002'.ToString();
                case StiFontIcons.EnvelopeO: return '\uf003'.ToString();
                case StiFontIcons.Heart: return '\uf004'.ToString();
                case StiFontIcons.Star: return '\uf005'.ToString();
                case StiFontIcons.StarO: return '\uf006'.ToString();
                case StiFontIcons.User: return '\uf007'.ToString();
                case StiFontIcons.Film: return '\uf008'.ToString();
                case StiFontIcons.ThLarge: return '\uf009'.ToString();
                case StiFontIcons.Th: return '\uf00a'.ToString();
                case StiFontIcons.ThList: return '\uf00b'.ToString();
                case StiFontIcons.Times: return '\uf00d'.ToString();
                case StiFontIcons.SearchPlus: return '\uf00e'.ToString();
                case StiFontIcons.SearchMinus: return '\uf010'.ToString();
                case StiFontIcons.PowerOff: return '\uf011'.ToString();
                case StiFontIcons.Signal: return '\uf012'.ToString();
                case StiFontIcons.Cog: return '\uf013'.ToString();
                case StiFontIcons.TrashO: return '\uf014'.ToString();
                case StiFontIcons.FileO: return '\uf016'.ToString();
                case StiFontIcons.ClockO: return '\uf017'.ToString();
                case StiFontIcons.Road: return '\uf018'.ToString();
                case StiFontIcons.Download: return '\uf019'.ToString();
                case StiFontIcons.ArrowCircleODown: return '\uf01a'.ToString();
                case StiFontIcons.ArrowCircleOUp: return '\uf01b'.ToString();
                case StiFontIcons.Inbox: return '\uf01c'.ToString();
                case StiFontIcons.PlayCircleO: return '\uf01d'.ToString();
                case StiFontIcons.Repeat: return '\uf01e'.ToString();
                case StiFontIcons.Refresh: return '\uf021'.ToString();
                case StiFontIcons.ListAlt: return '\uf022'.ToString();
                case StiFontIcons.Lock: return '\uf023'.ToString();
                case StiFontIcons.FAFlag: return '\uf024'.ToString();
                case StiFontIcons.Headphones: return '\uf025'.ToString();
                case StiFontIcons.VolumeOff: return '\uf026'.ToString();
                case StiFontIcons.VolumeDown: return '\uf027'.ToString();
                case StiFontIcons.VolumeUp: return '\uf028'.ToString();
                case StiFontIcons.Qrcode: return '\uf029'.ToString();
                case StiFontIcons.Barcode: return '\uf02a'.ToString();
                case StiFontIcons.Tag: return '\uf02b'.ToString();
                case StiFontIcons.Tags: return '\uf02c'.ToString();
                case StiFontIcons.Book: return '\uf02d'.ToString();
                case StiFontIcons.Bookmark: return '\uf02e'.ToString();
                case StiFontIcons.Print: return '\uf02f'.ToString();
                case StiFontIcons.Camera: return '\uf030'.ToString();
                case StiFontIcons.Font: return '\uf031'.ToString();
                case StiFontIcons.Bold: return '\uf032'.ToString();
                case StiFontIcons.Italic: return '\uf033'.ToString();
                case StiFontIcons.TextHeight: return '\uf034'.ToString();
                case StiFontIcons.TextWidth: return '\uf035'.ToString();
                case StiFontIcons.AlignLeft: return '\uf036'.ToString();
                case StiFontIcons.AlignCenter: return '\uf037'.ToString();
                case StiFontIcons.AlignRight: return '\uf038'.ToString();
                case StiFontIcons.AlignJustify: return '\uf039'.ToString();
                case StiFontIcons.List: return '\uf03a'.ToString();
                case StiFontIcons.Outdent: return '\uf03b'.ToString();
                case StiFontIcons.Indent: return '\uf03c'.ToString();
                case StiFontIcons.VideoCamera: return '\uf03d'.ToString();
                case StiFontIcons.PictureO: return '\uf03e'.ToString();
                case StiFontIcons.Pencil: return '\uf040'.ToString();
                case StiFontIcons.MapMarker: return '\uf041'.ToString();
                case StiFontIcons.Adjust: return '\uf042'.ToString();
                case StiFontIcons.Tint: return '\uf043'.ToString();
                case StiFontIcons.PencilSquareO: return '\uf044'.ToString();
                case StiFontIcons.ShareSquareO: return '\uf045'.ToString();
                case StiFontIcons.CheckSquareO: return '\uf046'.ToString();
                case StiFontIcons.Arrows: return '\uf047'.ToString();
                case StiFontIcons.StepBackward: return '\uf048'.ToString();
                case StiFontIcons.FastBackward: return '\uf049'.ToString();
                case StiFontIcons.Backward: return '\uf04a'.ToString();
                case StiFontIcons.Play: return '\uf04b'.ToString();
                case StiFontIcons.Pause: return '\uf04c'.ToString();
                case StiFontIcons.Stop: return '\uf04d'.ToString();
                case StiFontIcons.Forward: return '\uf04e'.ToString();
                case StiFontIcons.FastForward: return '\uf050'.ToString();
                case StiFontIcons.StepForward: return '\uf051'.ToString();
                case StiFontIcons.Eject: return '\uf052'.ToString();
                case StiFontIcons.ChevronLeft: return '\uf053'.ToString();
                case StiFontIcons.ChevronRight: return '\uf054'.ToString();
                case StiFontIcons.PlusCircle: return '\uf055'.ToString();
                case StiFontIcons.MinusCircle: return '\uf056'.ToString();
                case StiFontIcons.TimesCircle: return '\uf057'.ToString();
                case StiFontIcons.CheckCircle: return '\uf058'.ToString();
                case StiFontIcons.QuestionCircle: return '\uf059'.ToString();
                case StiFontIcons.InfoCircle: return '\uf05a'.ToString();
                case StiFontIcons.Crosshairs: return '\uf05b'.ToString();
                case StiFontIcons.TimesCircleO: return '\uf05c'.ToString();
                case StiFontIcons.CheckCircleO: return '\uf05d'.ToString();
                case StiFontIcons.Ban: return '\uf05e'.ToString();
                case StiFontIcons.FAArrowLeft: return '\uf060'.ToString();
                case StiFontIcons.FAArrowRight: return '\uf061'.ToString();
                case StiFontIcons.FAArrowUp: return '\uf062'.ToString();
                case StiFontIcons.FAArrowDown: return '\uf063'.ToString();
                case StiFontIcons.Share: return '\uf064'.ToString();
                case StiFontIcons.Expand: return '\uf065'.ToString();
                case StiFontIcons.Compress: return '\uf066'.ToString();
                case StiFontIcons.FAPlus: return '\uf067'.ToString();
                case StiFontIcons.FAMinus: return '\uf068'.ToString();
                case StiFontIcons.Asterisk: return '\uf069'.ToString();
                case StiFontIcons.ExclamationCircle: return '\uf06a'.ToString();
                case StiFontIcons.Gift: return '\uf06b'.ToString();
                case StiFontIcons.Leaf: return '\uf06c'.ToString();
                case StiFontIcons.Fire: return '\uf06d'.ToString();
                case StiFontIcons.Eye: return '\uf06e'.ToString();
                case StiFontIcons.EyeSlash: return '\uf070'.ToString();
                case StiFontIcons.ExclamationTriangle: return '\uf071'.ToString();
                case StiFontIcons.Plane: return '\uf072'.ToString();
                case StiFontIcons.Calendar: return '\uf073'.ToString();
                case StiFontIcons.Random: return '\uf074'.ToString();
                case StiFontIcons.Comment: return '\uf075'.ToString();
                case StiFontIcons.Magnet: return '\uf076'.ToString();
                case StiFontIcons.ChevronUp: return '\uf077'.ToString();
                case StiFontIcons.ChevronDown: return '\uf078'.ToString();
                case StiFontIcons.Retweet: return '\uf079'.ToString();
                case StiFontIcons.ShoppingCart: return '\uf07a'.ToString();
                case StiFontIcons.Folder: return '\uf07b'.ToString();
                case StiFontIcons.FolderOpen: return '\uf07c'.ToString();
                case StiFontIcons.ArrowsV: return '\uf07d'.ToString();
                case StiFontIcons.ArrowsH: return '\uf07e'.ToString();
                case StiFontIcons.BarChart: return '\uf080'.ToString();
                case StiFontIcons.TwitterSquare: return '\uf081'.ToString();
                case StiFontIcons.FacebookSquare: return '\uf082'.ToString();
                case StiFontIcons.CameraRetro: return '\uf083'.ToString();
                case StiFontIcons.Key: return '\uf084'.ToString();
                case StiFontIcons.Cogs: return '\uf085'.ToString();
                case StiFontIcons.Comments: return '\uf086'.ToString();
                case StiFontIcons.ThumbsOUp: return '\uf087'.ToString();
                case StiFontIcons.ThumbsODown: return '\uf088'.ToString();
                case StiFontIcons.HeartO: return '\uf08a'.ToString();
                case StiFontIcons.SignOut: return '\uf08b'.ToString();
                case StiFontIcons.LinkedinSquare: return '\uf08c'.ToString();
                case StiFontIcons.ThumbTack: return '\uf08d'.ToString();
                case StiFontIcons.ExternalLink: return '\uf08e'.ToString();
                case StiFontIcons.SignIn: return '\uf090'.ToString();
                case StiFontIcons.Trophy: return '\uf091'.ToString();
                case StiFontIcons.GithubSquare: return '\uf092'.ToString();
                case StiFontIcons.Upload: return '\uf093'.ToString();
                case StiFontIcons.LemonO: return '\uf094'.ToString();
                case StiFontIcons.SquareO: return '\uf096'.ToString();
                case StiFontIcons.BookmarkO: return '\uf097'.ToString();
                case StiFontIcons.PhoneSquare: return '\uf098'.ToString();
                case StiFontIcons.Twitter: return '\uf099'.ToString();
                case StiFontIcons.Facebook: return '\uf09a'.ToString();
                case StiFontIcons.Github: return '\uf09b'.ToString();
                case StiFontIcons.Unlock: return '\uf09c'.ToString();
                case StiFontIcons.CreditCard: return '\uf09d'.ToString();
                case StiFontIcons.Rss: return '\uf09e'.ToString();
                case StiFontIcons.HddO: return '\uf0a0'.ToString();
                case StiFontIcons.Bullhorn: return '\uf0a1'.ToString();
                case StiFontIcons.Bell: return '\uf0f3'.ToString();
                case StiFontIcons.Certificate: return '\uf0a3'.ToString();
                case StiFontIcons.HandORight: return '\uf0a4'.ToString();
                case StiFontIcons.HandOLeft: return '\uf0a5'.ToString();
                case StiFontIcons.HandOUp: return '\uf0a6'.ToString();
                case StiFontIcons.HandODown: return '\uf0a7'.ToString();
                case StiFontIcons.ArrowCircleLeft: return '\uf0a8'.ToString();
                case StiFontIcons.ArrowCircleRight: return '\uf0a9'.ToString();
                case StiFontIcons.ArrowCircleUp: return '\uf0aa'.ToString();
                case StiFontIcons.ArrowCircleDown: return '\uf0ab'.ToString();
                case StiFontIcons.Globe: return '\uf0ac'.ToString();
                case StiFontIcons.Wrench: return '\uf0ad'.ToString();
                case StiFontIcons.Tasks: return '\uf0ae'.ToString();
                case StiFontIcons.Filter: return '\uf0b0'.ToString();
                case StiFontIcons.Briefcase: return '\uf0b1'.ToString();
                case StiFontIcons.ArrowsAlt: return '\uf0b2'.ToString();
                case StiFontIcons.Users: return '\uf0c0'.ToString();
                case StiFontIcons.Link: return '\uf0c1'.ToString();
                case StiFontIcons.Cloud: return '\uf0c2'.ToString();
                case StiFontIcons.Flask: return '\uf0c3'.ToString();
                case StiFontIcons.Scissors: return '\uf0c4'.ToString();
                case StiFontIcons.FilesO: return '\uf0c5'.ToString();
                case StiFontIcons.Paperclip: return '\uf0c6'.ToString();
                case StiFontIcons.FloppyO: return '\uf0c7'.ToString();
                case StiFontIcons.Square: return '\uf0c8'.ToString();
                case StiFontIcons.Bars: return '\uf0c9'.ToString();
                case StiFontIcons.ListUl: return '\uf0ca'.ToString();
                case StiFontIcons.ListOl: return '\uf0cb'.ToString();
                case StiFontIcons.Strikethrough: return '\uf0cc'.ToString();
                case StiFontIcons.Underline: return '\uf0cd'.ToString();
                case StiFontIcons.Table: return '\uf0ce'.ToString();
                case StiFontIcons.Magic: return '\uf0d0'.ToString();
                case StiFontIcons.Pinterest: return '\uf0d2'.ToString();
                case StiFontIcons.PinterestSquare: return '\uf0d3'.ToString();
                case StiFontIcons.GooglePlusSquare: return '\uf0d4'.ToString();
                case StiFontIcons.GooglePlus: return '\uf0d5'.ToString();
                case StiFontIcons.Money: return '\uf0d6'.ToString();
                case StiFontIcons.CaretDown: return '\uf0d7'.ToString();
                case StiFontIcons.CaretUp: return '\uf0d8'.ToString();
                case StiFontIcons.CaretLeft: return '\uf0d9'.ToString();
                case StiFontIcons.CaretRight: return '\uf0da'.ToString();
                case StiFontIcons.Columns: return '\uf0db'.ToString();
                case StiFontIcons.Sort: return '\uf0dc'.ToString();
                case StiFontIcons.SortDesc: return '\uf0dd'.ToString();
                case StiFontIcons.SortAsc: return '\uf0de'.ToString();
                case StiFontIcons.Envelope: return '\uf0e0'.ToString();
                case StiFontIcons.Linkedin: return '\uf0e1'.ToString();
                case StiFontIcons.Undo: return '\uf0e2'.ToString();
                case StiFontIcons.Gavel: return '\uf0e3'.ToString();
                case StiFontIcons.Tachometer: return '\uf0e4'.ToString();
                case StiFontIcons.CommentO: return '\uf0e5'.ToString();
                case StiFontIcons.CommentsO: return '\uf0e6'.ToString();
                case StiFontIcons.Bolt: return '\uf0e7'.ToString();
                case StiFontIcons.Sitemap: return '\uf0e8'.ToString();
                case StiFontIcons.Umbrella: return '\uf0e9'.ToString();
                case StiFontIcons.Clipboard: return '\uf0ea'.ToString();
                case StiFontIcons.LightbulbO: return '\uf0eb'.ToString();
                case StiFontIcons.Exchange: return '\uf0ec'.ToString();
                case StiFontIcons.CloudDownload: return '\uf0ed'.ToString();
                case StiFontIcons.CloudUpload: return '\uf0ee'.ToString();
                case StiFontIcons.UserMd: return '\uf0f0'.ToString();
                case StiFontIcons.Stethoscope: return '\uf0f1'.ToString();
                case StiFontIcons.Suitcase: return '\uf0f2'.ToString();
                case StiFontIcons.BellO: return '\uf0a2'.ToString();
                case StiFontIcons.Coffee: return '\uf0f4'.ToString();
                case StiFontIcons.Cutlery: return '\uf0f5'.ToString();
                case StiFontIcons.FileTextO: return '\uf0f6'.ToString();
                case StiFontIcons.BuildingO: return '\uf0f7'.ToString();
                case StiFontIcons.HospitalO: return '\uf0f8'.ToString();
                case StiFontIcons.Ambulance: return '\uf0f9'.ToString();
                case StiFontIcons.Medkit: return '\uf0fa'.ToString();
                case StiFontIcons.FighterJet: return '\uf0fb'.ToString();
                case StiFontIcons.Beer: return '\uf0fc'.ToString();
                case StiFontIcons.HSquare: return '\uf0fd'.ToString();
                case StiFontIcons.PlusSquare: return '\uf0fe'.ToString();
                case StiFontIcons.AngleDoubleLeft: return '\uf100'.ToString();
                case StiFontIcons.AngleDoubleRight: return '\uf101'.ToString();
                case StiFontIcons.AngleDoubleUp: return '\uf102'.ToString();
                case StiFontIcons.AngleDoubleDown: return '\uf103'.ToString();
                case StiFontIcons.AngleLeft: return '\uf104'.ToString();
                case StiFontIcons.AngleRight: return '\uf105'.ToString();
                case StiFontIcons.AngleUp: return '\uf106'.ToString();
                case StiFontIcons.AngleDown: return '\uf107'.ToString();
                case StiFontIcons.Desktop: return '\uf108'.ToString();
                case StiFontIcons.Laptop: return '\uf109'.ToString();
                case StiFontIcons.Tablet: return '\uf10a'.ToString();
                case StiFontIcons.CircleO: return '\uf10c'.ToString();
                case StiFontIcons.QuoteLeft: return '\uf10d'.ToString();
                case StiFontIcons.QuoteRight: return '\uf10e'.ToString();
                case StiFontIcons.Spinner: return '\uf110'.ToString();
                case StiFontIcons.Reply: return '\uf112'.ToString();
                case StiFontIcons.GithubAlt: return '\uf113'.ToString();
                case StiFontIcons.FolderO: return '\uf114'.ToString();
                case StiFontIcons.FolderOpenO: return '\uf115'.ToString();
                case StiFontIcons.SmileO: return '\uf118'.ToString();
                case StiFontIcons.FrownO: return '\uf119'.ToString();
                case StiFontIcons.MehO: return '\uf11a'.ToString();
                case StiFontIcons.Gamepad: return '\uf11b'.ToString();
                case StiFontIcons.KeyboardO: return '\uf11c'.ToString();
                case StiFontIcons.FlagO: return '\uf11d'.ToString();
                case StiFontIcons.FlagCheckered: return '\uf11e'.ToString();
                case StiFontIcons.Terminal: return '\uf120'.ToString();
                case StiFontIcons.Code: return '\uf121'.ToString();
                case StiFontIcons.ReplyAll: return '\uf122'.ToString();
                case StiFontIcons.StarHalfO: return '\uf123'.ToString();
                case StiFontIcons.LocationArrow: return '\uf124'.ToString();
                case StiFontIcons.Crop: return '\uf125'.ToString();
                case StiFontIcons.CodeFork: return '\uf126'.ToString();
                case StiFontIcons.ChainBroken: return '\uf127'.ToString();
                case StiFontIcons.Question: return '\uf128'.ToString();
                case StiFontIcons.Info: return '\uf129'.ToString();
                case StiFontIcons.Superscript: return '\uf12b'.ToString();
                case StiFontIcons.Subscript: return '\uf12c'.ToString();
                case StiFontIcons.Eraser: return '\uf12d'.ToString();
                case StiFontIcons.PuzzlePiece: return '\uf12e'.ToString();
                case StiFontIcons.Microphone: return '\uf130'.ToString();
                case StiFontIcons.MicrophoneSlash: return '\uf131'.ToString();
                case StiFontIcons.Shield: return '\uf132'.ToString();
                case StiFontIcons.CalendarO: return '\uf133'.ToString();
                case StiFontIcons.FireExtinguisher: return '\uf134'.ToString();
                case StiFontIcons.Rocket: return '\uf135'.ToString();
                case StiFontIcons.Maxcdn: return '\uf136'.ToString();
                case StiFontIcons.ChevronCircleLeft: return '\uf137'.ToString();
                case StiFontIcons.ChevronCircleRight: return '\uf138'.ToString();
                case StiFontIcons.ChevronCircleUp: return '\uf139'.ToString();
                case StiFontIcons.ChevronCircleDown: return '\uf13a'.ToString();
                case StiFontIcons.Html5: return '\uf13b'.ToString();
                case StiFontIcons.Css3: return '\uf13c'.ToString();
                case StiFontIcons.Anchor: return '\uf13d'.ToString();
                case StiFontIcons.UnlockAlt: return '\uf13e'.ToString();
                case StiFontIcons.Bullseye: return '\uf140'.ToString();
                case StiFontIcons.EllipsisH: return '\uf141'.ToString();
                case StiFontIcons.EllipsisV: return '\uf142'.ToString();
                case StiFontIcons.RssSquare: return '\uf143'.ToString();
                case StiFontIcons.PlayCircle: return '\uf144'.ToString();
                case StiFontIcons.Ticket: return '\uf145'.ToString();
                case StiFontIcons.MinusSquare: return '\uf146'.ToString();
                case StiFontIcons.InusSquareO: return '\uf147'.ToString();
                case StiFontIcons.LevelUp: return '\uf148'.ToString();
                case StiFontIcons.LevelDown: return '\uf149'.ToString();
                case StiFontIcons.CheckSquare: return '\uf14a'.ToString();
                case StiFontIcons.PencilSquare: return '\uf14b'.ToString();
                case StiFontIcons.ExternalLinkSquare: return '\uf14c'.ToString();
                case StiFontIcons.ShareSquare: return '\uf14d'.ToString();
                case StiFontIcons.Compass: return '\uf14e'.ToString();
                case StiFontIcons.CaretSquareODown: return '\uf150'.ToString();
                case StiFontIcons.CaretSquareOUp: return '\uf151'.ToString();
                case StiFontIcons.CaretSquareORight: return '\uf152'.ToString();
                case StiFontIcons.Eur: return '\uf153'.ToString();
                case StiFontIcons.Gbp: return '\uf154'.ToString();
                case StiFontIcons.Usd: return '\uf155'.ToString();
                case StiFontIcons.Inr: return '\uf156'.ToString();
                case StiFontIcons.Jpy: return '\uf157'.ToString();
                case StiFontIcons.Rub: return '\uf158'.ToString();
                case StiFontIcons.Krw: return '\uf159'.ToString();
                case StiFontIcons.Btc: return '\uf15a'.ToString();
                case StiFontIcons.File: return '\uf15b'.ToString();
                case StiFontIcons.FileText: return '\uf15c'.ToString();
                case StiFontIcons.SortAlphaAsc: return '\uf15d'.ToString();
                case StiFontIcons.SortAlphaDesc: return '\uf15e'.ToString();
                case StiFontIcons.SortAmountAsc: return '\uf160'.ToString();
                case StiFontIcons.SortAmountDesc: return '\uf161'.ToString();
                case StiFontIcons.SortNumericAsc: return '\uf162'.ToString();
                case StiFontIcons.SortNumericDesc: return '\uf163'.ToString();
                case StiFontIcons.ThumbsUp: return '\uf164'.ToString();
                case StiFontIcons.ThumbsDown: return '\uf165'.ToString();
                case StiFontIcons.YoutubeSquare: return '\uf166'.ToString();
                case StiFontIcons.Youtube: return '\uf167'.ToString();
                case StiFontIcons.Xing: return '\uf168'.ToString();
                case StiFontIcons.XingSquare: return '\uf169'.ToString();
                case StiFontIcons.YoutubePlay: return '\uf16a'.ToString();
                case StiFontIcons.Dropbox: return '\uf16b'.ToString();
                case StiFontIcons.StackOverflow: return '\uf16c'.ToString();
                case StiFontIcons.Instagram: return '\uf16d'.ToString();
                case StiFontIcons.Flickr: return '\uf16e'.ToString();
                case StiFontIcons.Adn: return '\uf170'.ToString();
                case StiFontIcons.Bitbucket: return '\uf171'.ToString();
                case StiFontIcons.BitbucketSquare: return '\uf172'.ToString();
                case StiFontIcons.Tumblr: return '\uf173'.ToString();
                case StiFontIcons.TumblrSquare: return '\uf174'.ToString();
                case StiFontIcons.LongArrowDown: return '\uf175'.ToString();
                case StiFontIcons.LongArrowUp: return '\uf176'.ToString();
                case StiFontIcons.LongArrowLeft: return '\uf177'.ToString();
                case StiFontIcons.LongArrowRight: return '\uf178'.ToString();
                case StiFontIcons.Apple: return '\uf179'.ToString();
                case StiFontIcons.Windows: return '\uf17a'.ToString();
                case StiFontIcons.Android: return '\uf17b'.ToString();
                case StiFontIcons.Linux: return '\uf17c'.ToString();
                case StiFontIcons.Dribbble: return '\uf17d'.ToString();
                case StiFontIcons.Skype: return '\uf17e'.ToString();
                case StiFontIcons.Foursquare: return '\uf180'.ToString();
                case StiFontIcons.Trello: return '\uf181'.ToString();
                case StiFontIcons.Female: return '\uf182'.ToString();
                case StiFontIcons.Male: return '\uf183'.ToString();
                case StiFontIcons.Gratipay: return '\uf184'.ToString();
                case StiFontIcons.SunO: return '\uf185'.ToString();
                case StiFontIcons.MoonO: return '\uf186'.ToString();
                case StiFontIcons.Archive: return '\uf187'.ToString();
                case StiFontIcons.Bug: return '\uf188'.ToString();
                case StiFontIcons.Vk: return '\uf189'.ToString();
                case StiFontIcons.Weibo: return '\uf18a'.ToString();
                case StiFontIcons.Renren: return '\uf18b'.ToString();
                case StiFontIcons.Pagelines: return '\uf18c'.ToString();
                case StiFontIcons.StackExchange: return '\uf18d'.ToString();
                case StiFontIcons.ArrowCircleORight: return '\uf18e'.ToString();
                case StiFontIcons.ArrowCircleOLeft: return '\uf190'.ToString();
                case StiFontIcons.CaretSquareOLeft: return '\uf191'.ToString();
                case StiFontIcons.DotCircleO: return '\uf192'.ToString();
                case StiFontIcons.Wheelchair: return '\uf193'.ToString();
                case StiFontIcons.VimeoSquare: return '\uf194'.ToString();
                case StiFontIcons.Try: return '\uf195'.ToString();
                case StiFontIcons.PlusSquareO: return '\uf196'.ToString();
                case StiFontIcons.SpaceShuttle: return '\uf197'.ToString();
                case StiFontIcons.Slack: return '\uf198'.ToString();
                case StiFontIcons.EnvelopeSquare: return '\uf199'.ToString();
                case StiFontIcons.Wordpress: return '\uf19a'.ToString();
                case StiFontIcons.Openid: return '\uf19b'.ToString();
                case StiFontIcons.University: return '\uf19c'.ToString();
                case StiFontIcons.GraduationCap: return '\uf19d'.ToString();
                case StiFontIcons.Yahoo: return '\uf19e'.ToString();
                case StiFontIcons.Google: return '\uf1a0'.ToString();
                case StiFontIcons.Reddit: return '\uf1a1'.ToString();
                case StiFontIcons.RedditSquare: return '\uf1a2'.ToString();
                case StiFontIcons.StumbleuponCircle: return '\uf1a3'.ToString();
                case StiFontIcons.Stumbleupon: return '\uf1a4'.ToString();
                case StiFontIcons.Delicious: return '\uf1a5'.ToString();
                case StiFontIcons.Digg: return '\uf1a6'.ToString();
                case StiFontIcons.PiedPiper: return '\uf1a7'.ToString();
                case StiFontIcons.PiedPiperAlt: return '\uf1a8'.ToString();
                case StiFontIcons.Drupal: return '\uf1a9'.ToString();
                case StiFontIcons.Joomla: return '\uf1aa'.ToString();
                case StiFontIcons.Language: return '\uf1ab'.ToString();
                case StiFontIcons.Fax: return '\uf1ac'.ToString();
                case StiFontIcons.Building: return '\uf1ad'.ToString();
                case StiFontIcons.Child: return '\uf1ae'.ToString();
                case StiFontIcons.Paw: return '\uf1b0'.ToString();
                case StiFontIcons.Spoon: return '\uf1b1'.ToString();
                case StiFontIcons.Cube: return '\uf1b2'.ToString();
                case StiFontIcons.Cubes: return '\uf1b3'.ToString();
                case StiFontIcons.Behance: return '\uf1b4'.ToString();
                case StiFontIcons.BehanceSquare: return '\uf1b5'.ToString();
                case StiFontIcons.Steam: return '\uf1b6'.ToString();
                case StiFontIcons.SteamSquare: return '\uf1b7'.ToString();
                case StiFontIcons.Recycle: return '\uf1b8'.ToString();
                case StiFontIcons.Car: return '\uf1b9'.ToString();
                case StiFontIcons.Taxi: return '\uf1ba'.ToString();
                case StiFontIcons.Tree: return '\uf1bb'.ToString();
                case StiFontIcons.Spotify: return '\uf1bc'.ToString();
                case StiFontIcons.Deviantart: return '\uf1bd'.ToString();
                case StiFontIcons.Soundcloud: return '\uf1be'.ToString();
                case StiFontIcons.Database: return '\uf1c0'.ToString();
                case StiFontIcons.FilePdfO: return '\uf1c1'.ToString();
                case StiFontIcons.FileWordO: return '\uf1c2'.ToString();
                case StiFontIcons.FileExcelO: return '\uf1c3'.ToString();
                case StiFontIcons.FilePowerpointO: return '\uf1c4'.ToString();
                case StiFontIcons.FileImageO: return '\uf1c5'.ToString();
                case StiFontIcons.FileArchiveO: return '\uf1c6'.ToString();
                case StiFontIcons.FileAudioO: return '\uf1c7'.ToString();
                case StiFontIcons.FileVideoO: return '\uf1c8'.ToString();
                case StiFontIcons.FileCodeO: return '\uf1c9'.ToString();
                case StiFontIcons.Vine: return '\uf1ca'.ToString();
                case StiFontIcons.Codepen: return '\uf1cb'.ToString();
                case StiFontIcons.Jsfiddle: return '\uf1cc'.ToString();
                case StiFontIcons.LifeRing: return '\uf1cd'.ToString();
                case StiFontIcons.CircleONotch: return '\uf1ce'.ToString();
                case StiFontIcons.Rebel: return '\uf1d0'.ToString();
                case StiFontIcons.Empire: return '\uf1d1'.ToString();
                case StiFontIcons.GitSquare: return '\uf1d2'.ToString();
                case StiFontIcons.Git: return '\uf1d3'.ToString();
                case StiFontIcons.HackerNews: return '\uf1d4'.ToString();
                case StiFontIcons.TencentWeibo: return '\uf1d5'.ToString();
                case StiFontIcons.Qq: return '\uf1d6'.ToString();
                case StiFontIcons.Weixin: return '\uf1d7'.ToString();
                case StiFontIcons.PaperPlane: return '\uf1d8'.ToString();
                case StiFontIcons.PaperPlaneO: return '\uf1d9'.ToString();
                case StiFontIcons.History: return '\uf1da'.ToString();
                case StiFontIcons.CircleThin: return '\uf1db'.ToString();
                case StiFontIcons.Header: return '\uf1dc'.ToString();
                case StiFontIcons.Paragraph: return '\uf1dd'.ToString();
                case StiFontIcons.Sliders: return '\uf1de'.ToString();
                case StiFontIcons.ShareAlt: return '\uf1e0'.ToString();
                case StiFontIcons.ShareAltSquare: return '\uf1e1'.ToString();
                case StiFontIcons.Bomb: return '\uf1e2'.ToString();
                case StiFontIcons.FutbolO: return '\uf1e3'.ToString();
                case StiFontIcons.Tty: return '\uf1e4'.ToString();
                case StiFontIcons.Binoculars: return '\uf1e5'.ToString();
                case StiFontIcons.Plug: return '\uf1e6'.ToString();
                case StiFontIcons.Slideshare: return '\uf1e7'.ToString();
                case StiFontIcons.Twitch: return '\uf1e8'.ToString();
                case StiFontIcons.Yelp: return '\uf1e9'.ToString();
                case StiFontIcons.NewspaperO: return '\uf1ea'.ToString();
                case StiFontIcons.Wifi: return '\uf1eb'.ToString();
                case StiFontIcons.Calculator: return '\uf1ec'.ToString();
                case StiFontIcons.Paypal: return '\uf1ed'.ToString();
                case StiFontIcons.GoogleWallet: return '\uf1ee'.ToString();
                case StiFontIcons.CcVisa: return '\uf1f0'.ToString();
                case StiFontIcons.CcMastercard: return '\uf1f1'.ToString();
                case StiFontIcons.CcDiscover: return '\uf1f2'.ToString();
                case StiFontIcons.CcAmex: return '\uf1f3'.ToString();
                case StiFontIcons.CcPaypal: return '\uf1f4'.ToString();
                case StiFontIcons.CcStripe: return '\uf1f5'.ToString();
                case StiFontIcons.BellSlash: return '\uf1f6'.ToString();
                case StiFontIcons.BellSlashO: return '\uf1f7'.ToString();
                case StiFontIcons.Trash: return '\uf1f8'.ToString();
                case StiFontIcons.Copyright: return '\uf1f9'.ToString();
                case StiFontIcons.At: return '\uf1fa'.ToString();
                case StiFontIcons.Eyedropper: return '\uf1fb'.ToString();
                case StiFontIcons.PaintBrush: return '\uf1fc'.ToString();
                case StiFontIcons.BirthdayCake: return '\uf1fd'.ToString();
                case StiFontIcons.AreaChart: return '\uf1fe'.ToString();
                case StiFontIcons.PieChart: return '\uf200'.ToString();
                case StiFontIcons.LineChart: return '\uf201'.ToString();
                case StiFontIcons.Lastfm: return '\uf202'.ToString();
                case StiFontIcons.LastfmSquare: return '\uf203'.ToString();
                case StiFontIcons.ToggleOff: return '\uf204'.ToString();
                case StiFontIcons.ToggleOn: return '\uf205'.ToString();
                case StiFontIcons.Bicycle: return '\uf206'.ToString();
                case StiFontIcons.Bus: return '\uf207'.ToString();
                case StiFontIcons.Ioxhost: return '\uf208'.ToString();
                case StiFontIcons.Angellist: return '\uf209'.ToString();
                case StiFontIcons.Cc: return '\uf20a'.ToString();
                case StiFontIcons.Ils: return '\uf20b'.ToString();
                case StiFontIcons.Meanpath: return '\uf20c'.ToString();
                case StiFontIcons.Buysellads: return '\uf20d'.ToString();
                case StiFontIcons.Connectdevelop: return '\uf20e'.ToString();
                case StiFontIcons.Dashcube: return '\uf210'.ToString();
                case StiFontIcons.Forumbee: return '\uf211'.ToString();
                case StiFontIcons.Leanpub: return '\uf212'.ToString();
                case StiFontIcons.Sellsy: return '\uf213'.ToString();
                case StiFontIcons.Shirtsinbulk: return '\uf214'.ToString();
                case StiFontIcons.Simplybuilt: return '\uf215'.ToString();
                case StiFontIcons.Skyatlas: return '\uf216'.ToString();
                case StiFontIcons.CartPlus: return '\uf217'.ToString();
                case StiFontIcons.CartArrowDown: return '\uf218'.ToString();
                case StiFontIcons.Diamond: return '\uf219'.ToString();
                case StiFontIcons.Ship: return '\uf21a'.ToString();
                case StiFontIcons.UserSecret: return '\uf21b'.ToString();
                case StiFontIcons.Motorcycle: return '\uf21c'.ToString();
                case StiFontIcons.StreetView: return '\uf21d'.ToString();
                case StiFontIcons.Heartbeat: return '\uf21e'.ToString();
                case StiFontIcons.Venus: return '\uf221'.ToString();
                case StiFontIcons.Mars: return '\uf222'.ToString();
                case StiFontIcons.Mercury: return '\uf223'.ToString();
                case StiFontIcons.Transgender: return '\uf224'.ToString();
                case StiFontIcons.TransgenderAlt: return '\uf225'.ToString();
                case StiFontIcons.VenusDouble: return '\uf226'.ToString();
                case StiFontIcons.MarsDouble: return '\uf227'.ToString();
                case StiFontIcons.VenusMars: return '\uf228'.ToString();
                case StiFontIcons.MarsStroke: return '\uf229'.ToString();
                case StiFontIcons.MarsStrokeV: return '\uf22a'.ToString();
                case StiFontIcons.MarsStrokeH: return '\uf22b'.ToString();
                case StiFontIcons.Neuter: return '\uf22c'.ToString();
                case StiFontIcons.Genderless: return '\uf22d'.ToString();
                case StiFontIcons.FacebookOfficial: return '\uf230'.ToString();
                case StiFontIcons.PinterestP: return '\uf231'.ToString();
                case StiFontIcons.Whatsapp: return '\uf232'.ToString();
                case StiFontIcons.Server: return '\uf233'.ToString();
                case StiFontIcons.UserPlus: return '\uf234'.ToString();
                case StiFontIcons.UserTimes: return '\uf235'.ToString();
                case StiFontIcons.Bed: return '\uf236'.ToString();
                case StiFontIcons.Viacoin: return '\uf237'.ToString();
                case StiFontIcons.Train: return '\uf238'.ToString();
                case StiFontIcons.Subway: return '\uf239'.ToString();
                case StiFontIcons.Medium: return '\uf23a'.ToString();
                case StiFontIcons.YCombinator: return '\uf23b'.ToString();
                case StiFontIcons.OptinMonster: return '\uf23c'.ToString();
                case StiFontIcons.Opencart: return '\uf23d'.ToString();
                case StiFontIcons.Expeditedssl: return '\uf23e'.ToString();
                case StiFontIcons.BatteryFull: return '\uf240'.ToString();
                case StiFontIcons.BatteryThreeQuarters: return '\uf241'.ToString();
                case StiFontIcons.BatteryHalf: return '\uf242'.ToString();
                case StiFontIcons.BatteryQuarter: return '\uf243'.ToString();
                case StiFontIcons.BatteryEmpty: return '\uf244'.ToString();
                case StiFontIcons.MousePointer: return '\uf245'.ToString();
                case StiFontIcons.ICursor: return '\uf246'.ToString();
                case StiFontIcons.ObjectGroup: return '\uf247'.ToString();
                case StiFontIcons.ObjectUngroup: return '\uf248'.ToString();
                case StiFontIcons.StickyNote: return '\uf249'.ToString();
                case StiFontIcons.StickyNoteO: return '\uf24a'.ToString();
                case StiFontIcons.CcJcb: return '\uf24b'.ToString();
                case StiFontIcons.CcDinersClub: return '\uf24c'.ToString();
                case StiFontIcons.Clone: return '\uf24d'.ToString();
                case StiFontIcons.BalanceScale: return '\uf24e'.ToString();
                case StiFontIcons.HourglassO: return '\uf250'.ToString();
                case StiFontIcons.HourglassStart: return '\uf251'.ToString();
                case StiFontIcons.HourglassHalf: return '\uf252'.ToString();
                case StiFontIcons.HourglassEnd: return '\uf253'.ToString();
                case StiFontIcons.Hourglass: return '\uf254'.ToString();
                case StiFontIcons.HandRockO: return '\uf255'.ToString();
                case StiFontIcons.HandPaperO: return '\uf256'.ToString();
                case StiFontIcons.HandScissorsO: return '\uf257'.ToString();
                case StiFontIcons.HandLizardO: return '\uf258'.ToString();
                case StiFontIcons.HandSpockO: return '\uf259'.ToString();
                case StiFontIcons.HandPointerO: return '\uf25a'.ToString();
                case StiFontIcons.HandPeaceO: return '\uf25b'.ToString();
                case StiFontIcons.Trademark: return '\uf25c'.ToString();
                case StiFontIcons.Registered: return '\uf25d'.ToString();
                case StiFontIcons.CreativeCommons: return '\uf25e'.ToString();
                case StiFontIcons.Gg: return '\uf260'.ToString();
                case StiFontIcons.GgCircle: return '\uf261'.ToString();
                case StiFontIcons.Tripadvisor: return '\uf262'.ToString();
                case StiFontIcons.Odnoklassniki: return '\uf263'.ToString();
                case StiFontIcons.OdnoklassnikiSquare: return '\uf264'.ToString();
                case StiFontIcons.GetPocket: return '\uf265'.ToString();
                case StiFontIcons.WikipediaW: return '\uf266'.ToString();
                case StiFontIcons.Safari: return '\uf267'.ToString();
                case StiFontIcons.Chrome: return '\uf268'.ToString();
                case StiFontIcons.Firefox: return '\uf269'.ToString();
                case StiFontIcons.Opera: return '\uf26a'.ToString();
                case StiFontIcons.InternetExplorer: return '\uf26b'.ToString();
                case StiFontIcons.Television: return '\uf26c'.ToString();
                case StiFontIcons.Contao: return '\uf26d'.ToString();
                case StiFontIcons.Px500: return '\uf26e'.ToString();
                case StiFontIcons.Amazon: return '\uf270'.ToString();
                case StiFontIcons.CalendarPlusO: return '\uf271'.ToString();
                case StiFontIcons.CalendarMinusO: return '\uf272'.ToString();
                case StiFontIcons.CalendarTimesO: return '\uf273'.ToString();
                case StiFontIcons.CalendarCheckO: return '\uf274'.ToString();
                case StiFontIcons.Industry: return '\uf275'.ToString();
                case StiFontIcons.MapPin: return '\uf276'.ToString();
                case StiFontIcons.MapSigns: return '\uf277'.ToString();
                case StiFontIcons.MapO: return '\uf278'.ToString();
                case StiFontIcons.Map: return '\uf279'.ToString();
                case StiFontIcons.Commenting: return '\uf27a'.ToString();
                case StiFontIcons.CommentingO: return '\uf27b'.ToString();
                case StiFontIcons.Houzz: return '\uf27c'.ToString();
                case StiFontIcons.Vimeo: return '\uf27d'.ToString();
                case StiFontIcons.BlackTie: return '\uf27e'.ToString();
                case StiFontIcons.Fonticons: return '\uf280'.ToString();

            }

            return null;
        }

        internal static string GetGroupIconsName(StiFontIconGroup iconGroup)
        {
            switch (iconGroup)
            {
                case StiFontIconGroup.AccessibilityIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupAccessibilityIcons");

                case StiFontIconGroup.BrandIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupBrandIcons");

                case StiFontIconGroup.ChartIcons:
                    return StiLocalization.Get("Components", "StiChart");

                case StiFontIconGroup.CurrencyIcons:
                    return StiLocalization.Get("FormFormatEditor", "Currency");

                case StiFontIconGroup.DirectionalIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupDirectionalIcons");

                case StiFontIconGroup.FileTypeIcons:
                    return StiLocalization.Get("PropertyMain", "File");

                case StiFontIconGroup.FormControlIcons:
                    return StiLocalization.Get("PropertySystemColors", "Control");

                case StiFontIconGroup.GenderIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupGenderIcons");

                case StiFontIconGroup.HandIcons:
                    return StiLocalization.Get("Toolbox", "Hand");

                case StiFontIconGroup.MedicalIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupMedicalIcons");

                case StiFontIconGroup.OtherIcons:
                    return StiLocalization.Get("FormDictionaryDesigner", "CsvSeparatorOther");

                case StiFontIconGroup.PaymentIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupPaymentIcons");

                case StiFontIconGroup.SpinnerIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupSpinnerIcons");

                case StiFontIconGroup.TextEditorIcons:
                    return StiLocalization.Get("Toolbox", "TextEditor");

                case StiFontIconGroup.TransportationIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupTransportationIcons");

                case StiFontIconGroup.VideoPlayerIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupVideoPlayerIcons");

                case StiFontIconGroup.WebApplicationIcons:
                    return StiLocalization.Get("PropertyEnum", "StiFontIconGroupWebApplicationIcons");
            }

            return iconGroup.ToString();
        }

        internal static string GetIsonSetContent(StiFontIconSet iconSet)
        {
            var builtString = new StringBuilder();
            foreach (var icon in StiFontIconsHelper.GetFontIcons(iconSet))
            {
                builtString.Append(StiFontIconsHelper.GetContent(icon));
                builtString.Append(' ');
            }

            return builtString.ToString();
        }

        internal static List<StiFontIcons> GetFontIcons(StiFontIconSet iconSet)
        {
            switch (iconSet)
            {
                case StiFontIconSet.Rating:
                    return new List<StiFontIcons> { StiFontIcons.Rating0, StiFontIcons.Rating1, StiFontIcons.Rating2, StiFontIcons.Rating3, StiFontIcons.Rating4 };

                case StiFontIconSet.Quarter:
                    return new List<StiFontIcons> { StiFontIcons.QuarterNone, StiFontIcons.QuarterQuarter, StiFontIcons.QuarterHalf, StiFontIcons.QuarterThreeFourth, StiFontIcons.QuarterFull };

                case StiFontIconSet.Square:
                    return new List<StiFontIcons> { StiFontIcons.Square4, StiFontIcons.Square3, StiFontIcons.Square2, StiFontIcons.Square1, StiFontIcons.Square0 };

                case StiFontIconSet.Star:
                    return new List<StiFontIcons> { StiFontIcons.StarNone, StiFontIcons.StarQuarter, StiFontIcons.StarHalf, StiFontIcons.StarThreeFourth, StiFontIcons.StarFull };

                case StiFontIconSet.Latin:
                    return new List<StiFontIcons> { StiFontIcons.Latin1, StiFontIcons.Latin2, StiFontIcons.Latin3, StiFontIcons.Latin4, StiFontIcons.Latin5 };
            }

            return new List<StiFontIcons> { StiFontIcons.Rating0, StiFontIcons.Rating1, StiFontIcons.Rating2, StiFontIcons.Rating3, StiFontIcons.Rating4 };
        }

        internal static List<StiFontIcons> GetFontIcons(StiFontIconGroup iconGroup)
        {
            switch (iconGroup)
            {
                case StiFontIconGroup.WebApplicationIcons:
                    return new List<StiFontIcons> {
                        StiFontIcons.Adjust,
                        StiFontIcons.Anchor,
                        StiFontIcons.Archive,
                        StiFontIcons.AreaChart,
                        StiFontIcons.Arrows,
                        StiFontIcons.ArrowsH,
                        StiFontIcons.ArrowsV,
                        StiFontIcons.Asterisk,
                        StiFontIcons.At,
                        StiFontIcons.BalanceScale,
                        StiFontIcons.Ban,
                        StiFontIcons.BarChart,
                        StiFontIcons.Barcode,
                        StiFontIcons.Bars,
                        StiFontIcons.BatteryEmpty,
                        StiFontIcons.BatteryFull,
                        StiFontIcons.BatteryHalf,
                        StiFontIcons.BatteryQuarter,
                        StiFontIcons.BatteryThreeQuarters,
                        StiFontIcons.Bed,
                        StiFontIcons.Beer,
                        StiFontIcons.Bell,
                        StiFontIcons.BellO,
                        StiFontIcons.BellSlash,
                        StiFontIcons.BellSlashO,
                        StiFontIcons.Bicycle,
                        StiFontIcons.Binoculars,
                        StiFontIcons.BirthdayCake,
                        StiFontIcons.Bolt,
                        StiFontIcons.Bomb,
                        StiFontIcons.Book,
                        StiFontIcons.Bookmark,
                        StiFontIcons.BookmarkO,
                        StiFontIcons.Briefcase,
                        StiFontIcons.Bug,
                        StiFontIcons.Building,
                        StiFontIcons.BuildingO,
                        StiFontIcons.Bullhorn,
                        StiFontIcons.Bullseye,
                        StiFontIcons.Bus,
                        StiFontIcons.Calculator,
                        StiFontIcons.Calendar,
                        StiFontIcons.CalendarCheckO,
                        StiFontIcons.CalendarMinusO,
                        StiFontIcons.CalendarO,
                        StiFontIcons.CalendarPlusO,
                        StiFontIcons.CalendarTimesO,
                        StiFontIcons.Camera,
                        StiFontIcons.CameraRetro,
                        StiFontIcons.Car,
                        StiFontIcons.CaretSquareODown,
                        StiFontIcons.CaretSquareOLeft,
                        StiFontIcons.CaretSquareORight,
                        StiFontIcons.CaretSquareOUp,
                        StiFontIcons.Cart,
                        StiFontIcons.CartArrowDown,
                        StiFontIcons.CartPlus,
                        StiFontIcons.Cc,
                        StiFontIcons.Certificate,
                        StiFontIcons.Check,
                        StiFontIcons.CheckCircle,
                        StiFontIcons.CheckCircleO,
                        StiFontIcons.CheckSquare,
                        StiFontIcons.CheckSquareO,
                        StiFontIcons.Child,
                        StiFontIcons.Circle,
                        StiFontIcons.CircleO,
                        StiFontIcons.CircleONotch,
                        StiFontIcons.CircleThin,
                        StiFontIcons.ClockO,
                        StiFontIcons.Clone,
                        StiFontIcons.Cloud,
                        StiFontIcons.CloudDownload,
                        StiFontIcons.CloudUpload,
                        StiFontIcons.Code,
                        StiFontIcons.CodeFork,
                        StiFontIcons.Coffee,
                        StiFontIcons.Cog,
                        StiFontIcons.Cogs,
                        StiFontIcons.Comment,
                        StiFontIcons.CommentO,
                        StiFontIcons.Commenting,
                        StiFontIcons.CommentingO,
                        StiFontIcons.Comments,
                        StiFontIcons.CommentsO,
                        StiFontIcons.Compass,
                        StiFontIcons.Copyright,
                        StiFontIcons.CreativeCommons,
                        StiFontIcons.CreditCard,
                        StiFontIcons.Crop,
                        StiFontIcons.Crosshairs,
                        StiFontIcons.Cube,
                        StiFontIcons.Cubes,
                        StiFontIcons.Cutlery,
                        StiFontIcons.Database,
                        StiFontIcons.Desktop,
                        StiFontIcons.Diamond,
                        StiFontIcons.DotCircleO,
                        StiFontIcons.Download,
                        StiFontIcons.EllipsisH,
                        StiFontIcons.EllipsisV,
                        StiFontIcons.Envelope,
                        StiFontIcons.EnvelopeO,
                        StiFontIcons.EnvelopeSquare,
                        StiFontIcons.Eraser,
                        StiFontIcons.Exchange,
                        StiFontIcons.Exclamation,
                        StiFontIcons.ExclamationCircle,
                        StiFontIcons.ExclamationTriangle,
                        StiFontIcons.ExternalLink,
                        StiFontIcons.ExternalLinkSquare,
                        StiFontIcons.Eye,
                        StiFontIcons.EyeSlash,
                        StiFontIcons.Eyedropper,
                        StiFontIcons.Earth,
                        StiFontIcons.Fax,
                        StiFontIcons.Female,
                        StiFontIcons.FighterJet,
                        StiFontIcons.FileArchiveO,
                        StiFontIcons.FileAudioO,
                        StiFontIcons.FileCodeO,
                        StiFontIcons.FileExcelO,
                        StiFontIcons.FileImageO,
                        StiFontIcons.FilePdfO,
                        StiFontIcons.FilePowerpointO,
                        StiFontIcons.FileVideoO,
                        StiFontIcons.FileWordO,
                        StiFontIcons.Film,
                        StiFontIcons.Filter,
                        StiFontIcons.Fire,
                        StiFontIcons.FireExtinguisher,
                        StiFontIcons.Flag,
                        StiFontIcons.FlagCheckered,
                        StiFontIcons.FlagO,
                        StiFontIcons.FAFlag,
                        StiFontIcons.Flask,
                        StiFontIcons.Folder,
                        StiFontIcons.FolderO,
                        StiFontIcons.FolderOpen,
                        StiFontIcons.FolderOpenO,
                        StiFontIcons.FrownO,
                        StiFontIcons.FutbolO,
                        StiFontIcons.Gamepad,
                        StiFontIcons.Gavel,
                        StiFontIcons.Gift,
                        StiFontIcons.Glass,
                        StiFontIcons.Globe,
                        StiFontIcons.GraduationCap,
                        StiFontIcons.HandLizardO,
                        StiFontIcons.HandPaperO,
                        StiFontIcons.HandPeaceO,
                        StiFontIcons.HandPointerO,
                        StiFontIcons.HandRockO,
                        StiFontIcons.HandScissorsO,
                        StiFontIcons.HandSpockO,
                        StiFontIcons.HddO,
                        StiFontIcons.Headphones,
                        StiFontIcons.Heart,
                        StiFontIcons.HeartO,
                        StiFontIcons.Heartbeat,
                        StiFontIcons.History,
                        StiFontIcons.Home,
                        StiFontIcons.Hourglass,
                        StiFontIcons.HourglassEnd,
                        StiFontIcons.HourglassHalf,
                        StiFontIcons.HourglassO,
                        StiFontIcons.HourglassStart,
                        StiFontIcons.ICursor,
                        StiFontIcons.Inbox,
                        StiFontIcons.Industry,
                        StiFontIcons.Info,
                        StiFontIcons.InfoCircle,
                        StiFontIcons.Key,
                        StiFontIcons.KeyboardO,
                        StiFontIcons.Language,
                        StiFontIcons.Laptop,
                        StiFontIcons.Leaf,
                        StiFontIcons.LemonO,
                        StiFontIcons.LevelDown,
                        StiFontIcons.LevelUp,
                        StiFontIcons.LifeRing,
                        StiFontIcons.LightbulbO,
                        StiFontIcons.LineChart,
                        StiFontIcons.LocationArrow,
                        StiFontIcons.Lock,
                        StiFontIcons.Magic,
                        StiFontIcons.Magnet,
                        StiFontIcons.Male,
                        StiFontIcons.Map,
                        StiFontIcons.MapMarker,
                        StiFontIcons.MapO,
                        StiFontIcons.MapPin,
                        StiFontIcons.MapSigns,
                        StiFontIcons.MehO,
                        StiFontIcons.Microphone,
                        StiFontIcons.MicrophoneSlash,
                        StiFontIcons.Minus,
                        StiFontIcons.MinusCircle,
                        StiFontIcons.MinusSquare,
                        StiFontIcons.Mobile,
                        StiFontIcons.Money,
                        StiFontIcons.MoonO,
                        StiFontIcons.Motorcycle,
                        StiFontIcons.MousePointer,
                        StiFontIcons.Mug,
                        StiFontIcons.Music,
                        StiFontIcons.NewspaperO,
                        StiFontIcons.ObjectGroup,
                        StiFontIcons.ObjectUngroup,
                        StiFontIcons.PaintBrush,
                        StiFontIcons.PaperPlane,
                        StiFontIcons.PaperPlaneO,
                        StiFontIcons.Paw,
                        StiFontIcons.Pencil,
                        StiFontIcons.PencilSquare,
                        StiFontIcons.PencilSquareO,
                        StiFontIcons.Phone,
                        StiFontIcons.PhoneSquare,
                        StiFontIcons.PictureO,
                        StiFontIcons.PieChart,
                        StiFontIcons.Plane,
                        StiFontIcons.Plug,
                        StiFontIcons.PlusCircle,
                        StiFontIcons.PlusSquare,
                        StiFontIcons.PlusSquareO,
                        StiFontIcons.PowerOff,
                        StiFontIcons.Print,
                        StiFontIcons.PuzzlePiece,
                        StiFontIcons.Qrcode,
                        StiFontIcons.Question,
                        StiFontIcons.QuestionCircle,
                        StiFontIcons.QuoteLeft,
                        StiFontIcons.QuoteRight,
                        StiFontIcons.Random,
                        StiFontIcons.Recycle,
                        StiFontIcons.Refresh,
                        StiFontIcons.Registered,
                        StiFontIcons.Reply,
                        StiFontIcons.ReplyAll,
                        StiFontIcons.Retweet,
                        StiFontIcons.Road,
                        StiFontIcons.Rocket,
                        StiFontIcons.Rss,
                        StiFontIcons.RssSquare,
                        StiFontIcons.Search,
                        StiFontIcons.SearchMinus,
                        StiFontIcons.SearchPlus,
                        StiFontIcons.Server,
                        StiFontIcons.Share,
                        StiFontIcons.ShareAlt,
                        StiFontIcons.ShareAltSquare,
                        StiFontIcons.ShareSquare,
                        StiFontIcons.ShareSquareO,
                        StiFontIcons.Shield,
                        StiFontIcons.Ship,
                        StiFontIcons.ShoppingCart,
                        StiFontIcons.SignIn,
                        StiFontIcons.SignOut,
                        StiFontIcons.Signal,
                        StiFontIcons.Sitemap,
                        StiFontIcons.Sliders,
                        StiFontIcons.SmileO,
                        StiFontIcons.Sort,
                        StiFontIcons.SortAlphaAsc,
                        StiFontIcons.SortAlphaDesc,
                        StiFontIcons.SortAmountAsc,
                        StiFontIcons.SortAmountDesc,
                        StiFontIcons.SortAsc,
                        StiFontIcons.SortDesc,
                        StiFontIcons.SortNumericAsc,
                        StiFontIcons.SortNumericDesc,
                        StiFontIcons.SpaceShuttle,
                        StiFontIcons.Spinner,
                        StiFontIcons.Spoon,
                        StiFontIcons.Square,
                        StiFontIcons.SquareO,
                        StiFontIcons.Star,
                        StiFontIcons.StarHalf,
                        StiFontIcons.StarHalfO,
                        StiFontIcons.StarO,
                        StiFontIcons.StickyNote,
                        StiFontIcons.StickyNoteO,
                        StiFontIcons.StreetView,
                        StiFontIcons.Suitcase,
                        StiFontIcons.SunO,
                        StiFontIcons.Tablet,
                        StiFontIcons.Tachometer,
                        StiFontIcons.Tag,
                        StiFontIcons.Tags,
                        StiFontIcons.Tasks,
                        StiFontIcons.Taxi,
                        StiFontIcons.Television,
                        StiFontIcons.Terminal,
                        StiFontIcons.ThumbTack,
                        StiFontIcons.ThumbsDown,
                        StiFontIcons.ThumbsODown,
                        StiFontIcons.ThumbsOUp,
                        StiFontIcons.ThumbsUp,
                        StiFontIcons.Ticket,
                        StiFontIcons.Times,
                        StiFontIcons.TimesCircle,
                        StiFontIcons.TimesCircleO,
                        StiFontIcons.Tint,
                        StiFontIcons.ToggleOff,
                        StiFontIcons.ToggleOn,
                        StiFontIcons.Trademark,
                        StiFontIcons.Trash,
                        StiFontIcons.TrashO,
                        StiFontIcons.Tree,
                        StiFontIcons.Trophy,
                        StiFontIcons.Truck,
                        StiFontIcons.Tty,
                        StiFontIcons.Umbrella,
                        StiFontIcons.University,
                        StiFontIcons.Unlock,
                        StiFontIcons.UnlockAlt,
                        StiFontIcons.Upload,
                        StiFontIcons.User,
                        StiFontIcons.UserPlus,
                        StiFontIcons.UserSecret,
                        StiFontIcons.UserTimes,
                        StiFontIcons.Users,
                        StiFontIcons.VideoCamera,
                        StiFontIcons.VolumeDown,
                        StiFontIcons.VolumeOff,
                        StiFontIcons.VolumeUp,
                        StiFontIcons.Wheelchair,
                        StiFontIcons.Wifi,
                        StiFontIcons.Wrench
                    };

                case StiFontIconGroup.AccessibilityIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Cc,
                        StiFontIcons.Tty,
                        StiFontIcons.Wheelchair
                    };

                case StiFontIconGroup.HandIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.HandLizardO,
                        StiFontIcons.HandODown,
                        StiFontIcons.HandOLeft,
                        StiFontIcons.HandORight,
                        StiFontIcons.HandOUp,
                        StiFontIcons.HandPaperO,
                        StiFontIcons.HandPeaceO,
                        StiFontIcons.HandPointerO,
                        StiFontIcons.HandRockO,
                        StiFontIcons.HandScissorsO,
                        StiFontIcons.HandSpockO,
                        StiFontIcons.ThumbsDown,
                        StiFontIcons.ThumbsODown,
                        StiFontIcons.ThumbsOUp,
                        StiFontIcons.ThumbsUp
                    };

                case StiFontIconGroup.TransportationIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Airplane,
                        StiFontIcons.Ambulance,
                        StiFontIcons.Bicycle,
                        StiFontIcons.Bus,
                        StiFontIcons.Car,
                        StiFontIcons.FighterJet,
                        StiFontIcons.Motorcycle,
                        StiFontIcons.Plane,
                        StiFontIcons.Rocket,
                        StiFontIcons.Ship,
                        StiFontIcons.SpaceShuttle,
                        StiFontIcons.Subway,
                        StiFontIcons.Taxi,
                        StiFontIcons.Train,
                        StiFontIcons.Truck,
                        StiFontIcons.Wheelchair
                    };

                case StiFontIconGroup.GenderIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Genderless,
                        StiFontIcons.Mars,
                        StiFontIcons.MarsDouble,
                        StiFontIcons.MarsStroke,
                        StiFontIcons.MarsStrokeH,
                        StiFontIcons.MarsStrokeV,
                        StiFontIcons.Mercury,
                        StiFontIcons.Neuter,
                        StiFontIcons.Transgender,
                        StiFontIcons.TransgenderAlt,
                        StiFontIcons.Venus,
                        StiFontIcons.VenusDouble,
                        StiFontIcons.VenusMars,
                        StiFontIcons.Man,
                        StiFontIcons.Woman,
                        StiFontIcons.UserTie,
                        StiFontIcons.ManWoman,
                    };

                case StiFontIconGroup.FileTypeIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.File,
                        StiFontIcons.FileArchiveO,
                        StiFontIcons.FileAudioO,
                        StiFontIcons.FileCodeO,
                        StiFontIcons.FileExcelO,
                        StiFontIcons.FileImageO,
                        StiFontIcons.FileO,
                        StiFontIcons.FilePdfO,
                        StiFontIcons.FilePowerpointO,
                        StiFontIcons.FileText,
                        StiFontIcons.FileTextO,
                        StiFontIcons.FileVideoO,
                        StiFontIcons.FileWordO
                    };

                case StiFontIconGroup.SpinnerIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.CircleONotch,
                        StiFontIcons.Cog,
                        StiFontIcons.Refresh,
                        StiFontIcons.Spinner
                    };

                case StiFontIconGroup.FormControlIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.CheckSquare,
                        StiFontIcons.CheckSquareO,
                        StiFontIcons.Circle,
                        StiFontIcons.CircleO,
                        StiFontIcons.DotCircleO,
                        StiFontIcons.MinusSquare,
                        StiFontIcons.PlusSquare,
                        StiFontIcons.InusSquareO,
                        StiFontIcons.PlusSquareO,
                        StiFontIcons.Square,
                        StiFontIcons.SquareO,
                        StiFontIcons.FAPlus,
                        StiFontIcons.FAMinus,
                    };

                case StiFontIconGroup.PaymentIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.CcAmex,
                        StiFontIcons.CcDinersClub,
                        StiFontIcons.CcDiscover,
                        StiFontIcons.CcJcb,
                        StiFontIcons.CcMastercard,
                        StiFontIcons.CcPaypal,
                        StiFontIcons.CcStripe,
                        StiFontIcons.CcVisa,
                        StiFontIcons.CreditCard,
                        StiFontIcons.GoogleWallet,
                        StiFontIcons.Paypal
                    };

                case StiFontIconGroup.ChartIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.AreaChart,
                        StiFontIcons.BarChart,
                        StiFontIcons.LineChart,
                        StiFontIcons.PieChart
                    };

                case StiFontIconGroup.CurrencyIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Btc,
                        StiFontIcons.Eur,
                        StiFontIcons.Gbp,
                        StiFontIcons.Gg,
                        StiFontIcons.GgCircle,
                        StiFontIcons.Ils,
                        StiFontIcons.Inr,
                        StiFontIcons.Jpy,
                        StiFontIcons.Krw,
                        StiFontIcons.Money,
                        StiFontIcons.Rub,
                        StiFontIcons.Try,
                        StiFontIcons.Usd,
                        StiFontIcons.Viacoin
                    };

                case StiFontIconGroup.TextEditorIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.AlignCenter,
                        StiFontIcons.AlignJustify,
                        StiFontIcons.AlignLeft,
                        StiFontIcons.AlignRight,
                        StiFontIcons.Bold,
                        StiFontIcons.ChainBroken,
                        StiFontIcons.Clipboard,
                        StiFontIcons.Columns,
                        StiFontIcons.Eraser,
                        StiFontIcons.File,
                        StiFontIcons.FileO,
                        StiFontIcons.FileText,
                        StiFontIcons.FileTextO,
                        StiFontIcons.FilesO,
                        StiFontIcons.FloppyO,
                        StiFontIcons.Font,
                        StiFontIcons.Header,
                        StiFontIcons.Indent,
                        StiFontIcons.Italic,
                        StiFontIcons.Link,
                        StiFontIcons.List,
                        StiFontIcons.ListAlt,
                        StiFontIcons.ListOl,
                        StiFontIcons.ListUl,
                        StiFontIcons.Outdent,
                        StiFontIcons.Paperclip,
                        StiFontIcons.Paragraph,
                        StiFontIcons.Repeat,
                        StiFontIcons.Scissors,
                        StiFontIcons.Strikethrough,
                        StiFontIcons.Subscript,
                        StiFontIcons.Superscript,
                        StiFontIcons.Table,
                        StiFontIcons.TextHeight,
                        StiFontIcons.TextWidth,
                        StiFontIcons.Th,
                        StiFontIcons.ThLarge,
                        StiFontIcons.ThList,
                        StiFontIcons.Underline,
                        StiFontIcons.Undo
                    };

                case StiFontIconGroup.DirectionalIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.AngleDoubleDown,
                        StiFontIcons.AngleDoubleLeft,
                        StiFontIcons.AngleDoubleRight,
                        StiFontIcons.AngleDoubleUp,
                        StiFontIcons.AngleDown,
                        StiFontIcons.AngleLeft,
                        StiFontIcons.AngleRight,
                        StiFontIcons.AngleUp,
                        StiFontIcons.ArrowCircleDown,
                        StiFontIcons.ArrowCircleLeft,
                        StiFontIcons.ArrowCircleODown,
                        StiFontIcons.ArrowCircleOLeft,
                        StiFontIcons.ArrowCircleORight,
                        StiFontIcons.ArrowCircleOUp,
                        StiFontIcons.ArrowCircleRight,
                        StiFontIcons.ArrowCircleUp,
                        StiFontIcons.ArrowDown,
                        StiFontIcons.ArrowRight,
                        StiFontIcons.ArrowUp,
                        StiFontIcons.ArrowRightDown,
                        StiFontIcons.ArrowRightUp,
                        StiFontIcons.Arrows,
                        StiFontIcons.ArrowsAlt,
                        StiFontIcons.ArrowsH,
                        StiFontIcons.ArrowsV,
                        StiFontIcons.CaretDown,
                        StiFontIcons.CaretLeft,
                        StiFontIcons.CaretRight,
                        StiFontIcons.CaretSquareODown,
                        StiFontIcons.CaretSquareOLeft,
                        StiFontIcons.CaretSquareORight,
                        StiFontIcons.CaretSquareOUp,
                        StiFontIcons.CaretUp,
                        StiFontIcons.ChevronCircleDown,
                        StiFontIcons.ChevronCircleLeft,
                        StiFontIcons.ChevronCircleRight,
                        StiFontIcons.ChevronCircleUp,
                        StiFontIcons.ChevronDown,
                        StiFontIcons.ChevronLeft,
                        StiFontIcons.ChevronRight,
                        StiFontIcons.ChevronUp,
                        StiFontIcons.Exchange,
                        StiFontIcons.HandODown,
                        StiFontIcons.HandOLeft,
                        StiFontIcons.HandORight,
                        StiFontIcons.HandOUp,
                        StiFontIcons.LongArrowDown,
                        StiFontIcons.LongArrowLeft,
                        StiFontIcons.LongArrowRight,
                        StiFontIcons.LongArrowUp,
                        StiFontIcons.FAArrowLeft,
                        StiFontIcons.FAArrowRight,
                        StiFontIcons.FAArrowUp,
                        StiFontIcons.FAArrowDown,
                    };

                case StiFontIconGroup.VideoPlayerIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.ArrowsAlt,
                        StiFontIcons.Backward,
                        StiFontIcons.Compress,
                        StiFontIcons.Eject,
                        StiFontIcons.Expand,
                        StiFontIcons.FastBackward,
                        StiFontIcons.FastForward,
                        StiFontIcons.Forward,
                        StiFontIcons.Pause,
                        StiFontIcons.Play,
                        StiFontIcons.PlayCircle,
                        StiFontIcons.PlayCircleO,
                        StiFontIcons.Random,
                        StiFontIcons.StepBackward,
                        StiFontIcons.StepForward,
                        StiFontIcons.Stop,
                        StiFontIcons.YoutubePlay
                    };

                case StiFontIconGroup.BrandIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Adn,
                        StiFontIcons.Amazon,
                        StiFontIcons.Android,
                        StiFontIcons.Angellist,
                        StiFontIcons.Appleinc,
                        StiFontIcons.Behance,
                        StiFontIcons.BehanceSquare,
                        StiFontIcons.Bitbucket,
                        StiFontIcons.BitbucketSquare,
                        StiFontIcons.BlackTie,
                        StiFontIcons.Btc,
                        StiFontIcons.Buysellads,
                        StiFontIcons.CcAmex,
                        StiFontIcons.CcDinersClub,
                        StiFontIcons.CcDiscover,
                        StiFontIcons.CcJcb,
                        StiFontIcons.CcMastercard,
                        StiFontIcons.CcPaypal,
                        StiFontIcons.CcStripe,
                        StiFontIcons.CcVisa,
                        StiFontIcons.Chrome,
                        StiFontIcons.Codepen,
                        StiFontIcons.Connectdevelop,
                        StiFontIcons.Contao,
                        StiFontIcons.Css3,
                        StiFontIcons.Dashcube,
                        StiFontIcons.Delicious,
                        StiFontIcons.Deviantart,
                        StiFontIcons.Digg,
                        StiFontIcons.Dribbble,
                        StiFontIcons.Dropbox,
                        StiFontIcons.Drupal,
                        StiFontIcons.Empire,
                        StiFontIcons.Expeditedssl,
                        StiFontIcons.Facebook,
                        StiFontIcons.FacebookOfficial,
                        StiFontIcons.FacebookSquare,
                        StiFontIcons.Firefox,
                        StiFontIcons.Flickr,
                        StiFontIcons.Fonticons,
                        StiFontIcons.Forumbee,
                        StiFontIcons.Foursquare,
                        StiFontIcons.GetPocket,
                        StiFontIcons.Gg,
                        StiFontIcons.GgCircle,
                        StiFontIcons.Git,
                        StiFontIcons.GitSquare,
                        StiFontIcons.Github,
                        StiFontIcons.GithubAlt,
                        StiFontIcons.GithubSquare,
                        StiFontIcons.Google,
                        StiFontIcons.GooglePlus,
                        StiFontIcons.GooglePlusSquare,
                        StiFontIcons.GoogleWallet,
                        StiFontIcons.Gratipay,
                        StiFontIcons.HackerNews,
                        StiFontIcons.Houzz,
                        StiFontIcons.Html5,
                        StiFontIcons.Instagram,
                        StiFontIcons.InternetExplorer,
                        StiFontIcons.Ioxhost,
                        StiFontIcons.Joomla,
                        StiFontIcons.Jsfiddle,
                        StiFontIcons.Lastfm,
                        StiFontIcons.LastfmSquare,
                        StiFontIcons.Leanpub,
                        StiFontIcons.Linkedin,
                        StiFontIcons.LinkedinSquare,
                        StiFontIcons.Linux,
                        StiFontIcons.Maxcdn,
                        StiFontIcons.Meanpath,
                        StiFontIcons.Medium,
                        StiFontIcons.Odnoklassniki,
                        StiFontIcons.OdnoklassnikiSquare,
                        StiFontIcons.Opencart,
                        StiFontIcons.Openid,
                        StiFontIcons.Opera,
                        StiFontIcons.OptinMonster,
                        StiFontIcons.Pagelines,
                        StiFontIcons.Paypal,
                        StiFontIcons.PiedPiper,
                        StiFontIcons.PiedPiperAlt,
                        StiFontIcons.Pinterest,
                        StiFontIcons.PinterestP,
                        StiFontIcons.PinterestSquare,
                        StiFontIcons.Qq,
                        StiFontIcons.Rebel,
                        StiFontIcons.Reddit,
                        StiFontIcons.RedditSquare,
                        StiFontIcons.Renren,
                        StiFontIcons.Safari,
                        StiFontIcons.Sellsy,
                        StiFontIcons.ShareAlt,
                        StiFontIcons.ShareAltSquare,
                        StiFontIcons.Shirtsinbulk,
                        StiFontIcons.Simplybuilt,
                        StiFontIcons.Skyatlas,
                        StiFontIcons.Skype,
                        StiFontIcons.Slack,
                        StiFontIcons.Slideshare,
                        StiFontIcons.Soundcloud,
                        StiFontIcons.Spotify,
                        StiFontIcons.StackExchange,
                        StiFontIcons.StackOverflow,
                        StiFontIcons.Steam,
                        StiFontIcons.SteamSquare,
                        StiFontIcons.Stumbleupon,
                        StiFontIcons.StumbleuponCircle,
                        StiFontIcons.TencentWeibo,
                        StiFontIcons.Trello,
                        StiFontIcons.Tripadvisor,
                        StiFontIcons.Tumblr,
                        StiFontIcons.TumblrSquare,
                        StiFontIcons.Twitch,
                        StiFontIcons.Twitter,
                        StiFontIcons.TwitterSquare,
                        StiFontIcons.Viacoin,
                        StiFontIcons.Vimeo,
                        StiFontIcons.VimeoSquare,
                        StiFontIcons.Vine,
                        StiFontIcons.Vk,
                        StiFontIcons.Weibo,
                        StiFontIcons.Weixin,
                        StiFontIcons.Whatsapp,
                        StiFontIcons.WikipediaW,
                        StiFontIcons.Windows8,
                        StiFontIcons.Wordpress,
                        StiFontIcons.Xing,
                        StiFontIcons.XingSquare,
                        StiFontIcons.YCombinator,
                        StiFontIcons.Yahoo,
                        StiFontIcons.Yelp,
                        StiFontIcons.Youtube,
                        StiFontIcons.YoutubePlay,
                        StiFontIcons.YoutubeSquare,
                        StiFontIcons.Px500,
                    };

                case StiFontIconGroup.MedicalIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Ambulance,
                        StiFontIcons.HSquare,
                        StiFontIcons.Heart,
                        StiFontIcons.HeartO,
                        StiFontIcons.Heartbeat,
                        StiFontIcons.HospitalO,
                        StiFontIcons.Medkit,
                        StiFontIcons.PlusSquare,
                        StiFontIcons.Stethoscope,
                        StiFontIcons.UserMd,
                        StiFontIcons.Wheelchair
                    };

                case StiFontIconGroup.OtherIcons:
                    return new List<StiFontIcons>
                    {
                        StiFontIcons.Latin5,
                        StiFontIcons.Latin4,
                        StiFontIcons.Latin3,
                        StiFontIcons.Latin2,
                        StiFontIcons.Latin1,
                        StiFontIcons.QuarterFull,
                        StiFontIcons.QuarterThreeFourth,
                        StiFontIcons.QuarterHalf,
                        StiFontIcons.QuarterQuarter,
                        StiFontIcons.QuarterNone,
                        StiFontIcons.Rating4,
                        StiFontIcons.Rating3,
                        StiFontIcons.Rating2,
                        StiFontIcons.Rating1,
                        StiFontIcons.Rating0,
                        StiFontIcons.Square0,
                        StiFontIcons.Square1,
                        StiFontIcons.Square2,
                        StiFontIcons.Square3,
                        StiFontIcons.Square4,
                        StiFontIcons.StarFull,
                        StiFontIcons.StarThreeFourth,
                        StiFontIcons.StarQuarter,
                        StiFontIcons.StarNone,
                        StiFontIcons.CircleCheck,
                        StiFontIcons.CircleCross,
                        StiFontIcons.CircleExclamation,
                        StiFontIcons.Cross,
                        StiFontIcons.Rhomb,
                        StiFontIcons.Triangle,
                        StiFontIcons.TriangleDown,
                        StiFontIcons.TriangleUp,
                    };
            }

            return new List<StiFontIcons> 
            { 
                StiFontIcons.Rating0, 
                StiFontIcons.Rating1, 
                StiFontIcons.Rating2, 
                StiFontIcons.Rating3, 
                StiFontIcons.Rating4 
            };
        }

        public static StiFont GetFont()
        {
            if (FONT == null)
            {
                using (var fontTemp = new Font(GetFontFamilyIcons(), 11))
                {
                    FONT = new StiFont(fontTemp);
                }
            }
            return FONT.Value;
        }
        #endregion
    }
}
