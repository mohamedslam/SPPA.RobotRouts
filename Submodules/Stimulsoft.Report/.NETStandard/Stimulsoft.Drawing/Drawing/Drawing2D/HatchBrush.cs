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
using System.Linq;

namespace Stimulsoft.Drawing.Drawing2D
{
    public sealed class HatchBrush : Brush
    {
        private SixLabors.ImageSharp.Drawing.Processing.IBrush sixBrush;
        internal override SixLabors.ImageSharp.Drawing.Processing.IBrush SixBrush => sixBrush;

        private System.Drawing.Drawing2D.HatchBrush netBrush;
        internal override System.Drawing.Brush NetBrush => netBrush;

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netBrush.BackgroundColor;
                else
                    return backgroundColor;
            }
        }

        private Color foregroundColor;
        public Color ForegroundColor
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netBrush.ForegroundColor;
                else
                    return foregroundColor;
            }
        }

        private System.Drawing.Drawing2D.HatchStyle hatchStyle;
        public System.Drawing.Drawing2D.HatchStyle HatchStyle
        {
            get
            {
                if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                    return netBrush.HatchStyle;
                else
                    return hatchStyle;
            }
        }

        public HatchBrush(System.Drawing.Drawing2D.HatchStyle hatchStyle, Color foregroundColor, Color backgroundColor)
        {
            if (Graphics.GraphicsEngine == GraphicsEngine.Gdi)
                netBrush = new System.Drawing.Drawing2D.HatchBrush((System.Drawing.Drawing2D.HatchStyle)hatchStyle, foregroundColor, backgroundColor);
            else
            {
                this.foregroundColor = foregroundColor;
                this.backgroundColor = backgroundColor;
                this.hatchStyle = hatchStyle;

                string[] patternString;
                switch (hatchStyle)
                {
                    case System.Drawing.Drawing2D.HatchStyle.Horizontal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "11111111"}; break;
                    // case System.Drawing.Drawing2D.HatchStyle.Min: patternString = new string[] {
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Vertical:
                        patternString = new string[] {
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal:
                        patternString = new string[] {
                        "10000000",
                        "01000000",
                        "00100000",
                        "00010000",
                        "00001000",
                        "00000100",
                        "00000010",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal:
                        patternString = new string[] {
                        "00000001",
                        "00000010",
                        "00000100",
                        "00001000",
                        "00010000",
                        "00100000",
                        "01000000",
                        "10000000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Cross:
                        patternString = new string[] {
                        "11111111",
                        "10000001",
                        "10000001",
                        "10000001",
                        "10000001",
                        "10000001",
                        "10000001",
                        "11111111"}; break;
                    // case System.Drawing.Drawing2D.HatchStyle.LargeGrid: patternString = new string[] {
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000"}; break;
                    // case System.Drawing.Drawing2D.HatchStyle.Max: patternString = new string[] {
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000",
                    // "00000000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DiagonalCross: patternString = new string[] {
                        "10000001",
                        "01000010",
                        "00100100",
                        "00011000",
                        "00011000",
                        "00100100",
                        "01000010",
                        "10000001" }; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent05:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "00010000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent10:
                        patternString = new string[] {
                        "00000000",
                        "00010000",
                        "00000000",
                        "00000001",
                        "00000000",
                        "00010000",
                        "00000000",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent20:
                        patternString = new string[] {
                        "00000000",
                        "01000100",
                        "00000000",
                        "00010001",
                        "00000000",
                        "01000100",
                        "00000000",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent25:
                        patternString = new string[] {
                        "01000100",
                        "00010001",
                        "01000100",
                        "00010001",
                        "01000100",
                        "00010001",
                        "01000100",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent30:
                        patternString = new string[] {
                        "10101010",
                        "01000100",
                        "10101010",
                        "01010001",
                        "10101010",
                        "01000100",
                        "10101010",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent40:
                        patternString = new string[] {
                        "10101010",
                        "01010101",
                        "10101010",
                        "00010101",
                        "10101010",
                        "01010101",
                        "10101010",
                        "01010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent50:
                        patternString = new string[] {
                        "10101010",
                        "01010101",
                        "10101010",
                        "01010101",
                        "10101010",
                        "01010101",
                        "10101010",
                        "01010101"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent60:
                        patternString = new string[] {
                        "11011101",
                        "10101010",
                        "01110111",
                        "10101010",
                        "11011101",
                        "10101010",
                        "01110111",
                        "10101010"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent70:
                        patternString = new string[] {
                        "11011101",
                        "01110111",
                        "11011101",
                        "01110111",
                        "11011101",
                        "01110111",
                        "11011101",
                        "01110111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent75:
                        patternString = new string[] {
                        "01110111",
                        "11111111",
                        "11011101",
                        "11111111",
                        "01110111",
                        "11111111",
                        "11011101",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent80:
                        patternString = new string[] {
                        "11110111",
                        "11111111",
                        "01111111",
                        "11111111",
                        "11110111",
                        "11111111",
                        "01111111",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Percent90:
                        patternString = new string[] {
                        "11110111",
                        "11111111",
                        "11111111",
                        "11111111",
                        "01111111",
                        "11111111",
                        "11111111",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal:
                        patternString = new string[] {
                        "10001000",
                        "01000100",
                        "00100010",
                        "00010001",
                        "10001000",
                        "01000100",
                        "00100010",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LightUpwardDiagonal:
                        patternString = new string[] {
                        "00010001",
                        "00100010",
                        "01000100",
                        "10001000",
                        "00010001",
                        "00100010",
                        "01000100",
                        "10001000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DarkDownwardDiagonal:
                        patternString = new string[] {
                        "10011000",
                        "11001100",
                        "01100110",
                        "00110011",
                        "10011001",
                        "11001100",
                        "01100110",
                        "00110011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DarkUpwardDiagonal:
                        patternString = new string[] {
                        "00011001",
                        "00110011",
                        "01100110",
                        "11001100",
                        "10011001",
                        "00110011",
                        "01100110",
                        "11001100"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.WideDownwardDiagonal:
                        patternString = new string[] {
                        "10000000",
                        "11000000",
                        "11100000",
                        "01110000",
                        "00111000",
                        "00011100",
                        "00001110",
                        "00000111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal:
                        patternString = new string[] {
                        "00000001",
                        "00000011",
                        "00000111",
                        "00001110",
                        "00011100",
                        "00111000",
                        "01110000",
                        "11100000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LightVertical:
                        patternString = new string[] {
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LightHorizontal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "11111111",
                        "00000000",
                        "00000000",
                        "00000000",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.NarrowVertical:
                        patternString = new string[] {
                        "01010101",
                        "01010101",
                        "01010101",
                        "01010101",
                        "01010101",
                        "01010101",
                        "01010101",
                        "01010101"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.NarrowHorizontal:
                        patternString = new string[] {
                        "00000000",
                        "11111111",
                        "00000000",
                        "11111111",
                        "00000000",
                        "11111111",
                        "00000000",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DarkVertical:
                        patternString = new string[] {
                        "00110011",
                        "00110011",
                        "00110011",
                        "00110011",
                        "00110011",
                        "00110011",
                        "00110011",
                        "00110011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DarkHorizontal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "11111111",
                        "11111111",
                        "00000000",
                        "00000000",
                        "11111111",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DashedDownwardDiagonal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "10001000",
                        "01000100",
                        "00100010",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DashedUpwardDiagonal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00010001",
                        "00100010",
                        "01000100",
                        "10001000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DashedHorizontal:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "11110000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00001111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DashedVertical:
                        patternString = new string[] {
                        "00010000",
                        "00010000",
                        "00010000",
                        "00010000",
                        "00000001",
                        "00000001",
                        "00000001",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.SmallConfetti:
                        patternString = new string[] {
                        "00001000",
                        "01000000",
                        "00000010",
                        "00100000",
                        "00000100",
                        "10000000",
                        "00010000",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LargeConfetti:
                        patternString = new string[] {
                        "00110000",
                        "00110110",
                        "11000110",
                        "11000000",
                        "00001100",
                        "01101100",
                        "01100011",
                        "00000011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.ZigZag:
                        patternString = new string[] {
                        "00110000",
                        "01001000",
                        "10000100",
                        "00000011",
                        "00110000",
                        "01001000",
                        "10000100",
                        "00000011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Wave:
                        patternString = new string[] {
                        "00000000",
                        "00000110",
                        "01001001",
                        "00110000",
                        "00000000",
                        "00000110",
                        "01001001",
                        "00110000"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DiagonalBrick:
                        patternString = new string[] {
                        "00000001",
                        "00000010",
                        "00000100",
                        "00001000",
                        "00011000",
                        "00100100",
                        "01000010",
                        "10000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.HorizontalBrick:
                        patternString = new string[] {
                        "00010000",
                        "00010000",
                        "00010000",
                        "11111111",
                        "00000001",
                        "00000001",
                        "00000001",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Weave:
                        patternString = new string[] {
                        "10001000",
                        "01010100",
                        "00100010",
                        "01000101",
                        "10001000",
                        "00010100",
                        "00100010",
                        "01000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Plaid:
                        patternString = new string[] {
                        "10101010",
                        "01010101",
                        "10101010",
                        "01010101",
                        "00001111",
                        "00001111",
                        "00001111",
                        "00001111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Divot:
                        patternString = new string[] {
                        "00000000",
                        "00100000",
                        "00010000",
                        "00100000",
                        "00000000",
                        "00000001",
                        "00000010",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DottedGrid:
                        patternString = new string[] {
                        "00000000",
                        "00000001",
                        "00000000",
                        "00000001",
                        "00000000",
                        "00000001",
                        "00000000",
                        "01010101"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.DottedDiamond:
                        patternString = new string[] {
                        "00000000",
                        "01000000",
                        "00000000",
                        "00010001",
                        "00000000",
                        "00000100",
                        "00000000",
                        "00010001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Shingle:
                        patternString = new string[] {
                        "10000100",
                        "01001000",
                        "00110000",
                        "00001100",
                        "00000010",
                        "00000001",
                        "00000001",
                        "00000011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Trellis:
                        patternString = new string[] {
                        "11001100",
                        "11111111",
                        "00110011",
                        "11111111",
                        "11001100",
                        "11111111",
                        "00110011",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.Sphere:
                        patternString = new string[] {
                        "01110000",
                        "10011000",
                        "11111000",
                        "11111000",
                        "01110111",
                        "00001001",
                        "00001111",
                        "00001111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.SmallGrid:
                        patternString = new string[] {
                        "00010001",
                        "00010001",
                        "00010001",
                        "00010001",
                        "11111111",
                        "00010001",
                        "00010001",
                        "11111111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard:
                        patternString = new string[] {
                        "11001100",
                        "11001100",
                        "00110011",
                        "00110011",
                        "11001100",
                        "11001100",
                        "00110011",
                        "00110011"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.LargeCheckerBoard:
                        patternString = new string[] {
                        "11110000",
                        "11110000",
                        "11110000",
                        "11110000",
                        "00001111",
                        "00001111",
                        "00001111",
                        "00001111"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.OutlinedDiamond:
                        patternString = new string[] {
                        "10000010",
                        "01000100",
                        "00101000",
                        "00010000",
                        "00101000",
                        "01000100",
                        "10000010",
                        "00000001"}; break;
                    case System.Drawing.Drawing2D.HatchStyle.SolidDiamond:
                        patternString = new string[] {
                        "00000000",
                        "00001000",
                        "00011100",
                        "00111110",
                        "01111111",
                        "00111110",
                        "00011100",
                        "00001000"}; break;
                    default:
                        patternString = new string[] {
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000",
                        "00000000"}; break;
                }

                bool[,] pattern = new bool[8, 8];
                for (var y = 0; y < 8; y++)
                {
                    for (var x = 0; x < 8; x++)
                    {
                        pattern[x, y] = patternString[x][y] == '0' ? false : true;
                    }
                }

                sixBrush = new SixLabors.ImageSharp.Drawing.Processing.PatternBrush(ColorExt.ToSixColor(foregroundColor), ColorExt.ToSixColor(backgroundColor), pattern);
            }
        }
    }
}
