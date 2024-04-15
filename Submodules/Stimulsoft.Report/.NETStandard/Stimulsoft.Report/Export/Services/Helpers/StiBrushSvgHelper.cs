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
using Stimulsoft.Base.Drawing;
using System;
using System.Text;
using System.Xml;
using System.Drawing;

#if STIDRAWING
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
#endif

namespace Stimulsoft.Report.Export
{
    public static class StiBrushSvgHelper
    {
        #region Fields
        private static string[] hatchData =
            {
                "000000FF00000000",	//HatchStyleHorizontal = 0
				"1010101010101010",	//HatchStyleVertical = 1,			
				"8040201008040201",	//HatchStyleForwardDiagonal = 2,	
				"0102040810204080",	//HatchStyleBackwardDiagonal = 3,	
				"101010FF10101010",	//HatchStyleCross = 4,			
				"8142241818244281",	//HatchStyleDiagonalCross = 5,	
				"8000000008000000",	//HatchStyle05Percent = 6,		
				"0010000100100001",	//HatchStyle10Percent = 7,		
				"2200880022008800",	//HatchStyle20Percent = 8,		
				"2288228822882288",	//HatchStyle25Percent = 9,		
				"2255885522558855",	//HatchStyle30Percent = 10,		
				"AA558A55AA55A855",	//HatchStyle40Percent = 11,		
				"AA55AA55AA55AA55",	//HatchStyle50Percent = 12,		
				"BB55EE55BB55EE55",	//HatchStyle60Percent = 13,		
				"DD77DD77DD77DD77",	//HatchStyle70Percent = 14,		
				"FFDDFF77FFDDFF77",	//HatchStyle75Percent = 15,		
				"FF7FFFF7FF7FFFF7",	//HatchStyle80Percent = 16,		
				"FF7FFFFFFFF7FFFF",	//HatchStyle90Percent = 17,		
				"8844221188442211",	//HatchStyleLightDownwardDiagonal = 18,	
				"1122448811224488",	//HatchStyleLightUpwardDiagonal = 19,	
				"CC663399CC663399",	//HatchStyleDarkDownwardDiagonal = 20,	
				"993366CC993366CC",	//HatchStyleDarkUpwardDiagonal = 21,	
				"E070381C0E0783C1",	//HatchStyleWideDownwardDiagonal = 22,	
				"C183070E1C3870E0",	//HatchStyleWideUpwardDiagonal = 23,	
				"4040404040404040",	//HatchStyleLightVertical = 24,			
				"00FF000000FF0000",	//HatchStyleLightHorizontal = 25,		
				"AAAAAAAAAAAAAAAA",	//HatchStyleNarrowVertical = 26,		
				"FF00FF00FF00FF00",	//HatchStyleNarrowHorizontal = 27,		
				"CCCCCCCCCCCCCCCC",	//HatchStyleDarkVertical = 28,			
				"FFFF0000FFFF0000",	//HatchStyleDarkHorizontal = 29,		
				"8844221100000000",	//HatchStyleDashedDownwardDiagonal = 30,
				"1122448800000000",	//HatchStyleDashedUpwardDiagonal = 311,	
				"F00000000F000000",	//HatchStyleDashedHorizontal = 32,		
				"8080808008080808",	//HatchStyleDashedVertical = 33,		
				"0240088004200110",	//HatchStyleSmallConfetti = 34,			
				"0C8DB130031BD8C0",	//HatchStyleLargeConfetti = 35,		
				"8403304884033048",	//HatchStyleZigZag = 36,			
				"00304A8100304A81",	//HatchStyleWave = 37,				
				"0102040818244281",	//HatchStyleDiagonalBrick = 38,		
				"202020FF020202FF",	//HatchStyleHorizontalBrick = 39,	
				"1422518854224588",	//HatchStyleWeave = 40,				
				"F0F0F0F0AA55AA55",	//HatchStylePlaid = 41,				
				"0100201020000102",	//HatchStyleDivot = 42,				
				"AA00800080008000",	//HatchStyleDottedGrid = 43,		
				"0020008800020088",	//HatchStyleDottedDiamond = 44,		
				"8448300C02010103",	//HatchStyleShingle = 45,			
				"33FFCCFF33FFCCFF",	//HatchStyleTrellis = 46,			
				"98F8F877898F8F77",	//HatchStyleSphere = 47,			
				"111111FF111111FF",	//HatchStyleSmallGrid = 48,			
				"3333CCCC3333CCCC",	//HatchStyleSmallCheckerBoard = 49,	
				"0F0F0F0FF0F0F0F0",	//HatchStyleLargeCheckerBoard = 50,	
				"0502058850205088",	//HatchStyleOutlinedDiamond = 51,	
				"10387CFE7C381000",	//HatchStyleSolidDiamond = 52,
				"0000000000000000"	//HatchStyleTotal = 53
			};
        #endregion

        #region Methods
        public static string WriteGlareBrush(XmlTextWriter writer, object brush, RectangleF rect)
        {
            var gb = brush as StiGlareBrush;

            #region Calculate gradient info
            double xs = 1;
            double ys = 1;
            double angle = gb.Angle;
            if (angle < 0) angle += 360;
            if ((angle >= 270) && (angle < 360))
            {
                angle = 360 - angle;
                ys = -1;
            };
            if ((angle >= 180) && (angle < 270))
            {
                angle = angle - 180;
                ys = -1;
                xs = -1;
            };
            if ((angle >= 90) && (angle < 180))
            {
                angle = 180 - angle;
                xs = -1;
            };
            angle = angle * Math.PI / 180f;

            double x0 = rect.X + rect.Width / 2f;
            double y0 = rect.Y + rect.Height / 2f;
            double r = Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height) / 2f;
            double a2 = Math.Atan2(rect.Height, rect.Width);
            double st = Math.PI / 2f - angle + a2;
            double b = r * Math.Sin(st);
            double xr = b * Math.Cos(angle) * xs;
            double yr = b * Math.Sin(angle) * (-ys);

            double x1 = x0 - xr;
            double x2 = x0 + xr;
            double y1 = y0 + yr;
            double y2 = y0 - yr;
            #endregion

            #region Create gradient
            string gradientId = string.Format("gradient{0}", StiGuidUtils.NewGuid());
            writer.WriteStartElement("linearGradient");
            writer.WriteAttributeString("id", gradientId);
            writer.WriteAttributeString("gradientUnits", "userSpaceOnUse");
            writer.WriteAttributeString("x1", x1.ToString().Replace(",", "."));
            writer.WriteAttributeString("y1", y1.ToString().Replace(",", "."));
            writer.WriteAttributeString("x2", x2.ToString().Replace(",", "."));
            writer.WriteAttributeString("y2", y2.ToString().Replace(",", "."));

            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "0%");
            writer.WriteAttributeString("stop-color", string.Format("#{0:X2}{1:X2}{2:X2}", gb.StartColor.R, gb.StartColor.G, gb.StartColor.B));
            if (gb.StartColor.A != 0xFF)
            {
                writer.WriteAttributeString("stop-opacity", Math.Round(gb.StartColor.A / 255f, 3).ToString().Replace(",", "."));
            }
            writer.WriteEndElement();

            writer.WriteStartElement("stop");
            var focus = gb.Focus * 100;
            writer.WriteAttributeString("offset", $"{focus}%");
            writer.WriteAttributeString("stop-color", string.Format("#{0:X2}{1:X2}{2:X2}", gb.EndColor.R, gb.EndColor.G, gb.EndColor.B));
            if (gb.EndColor.A != 0xFF)
            {
                writer.WriteAttributeString("stop-opacity", Math.Round(gb.EndColor.A / 255f, 3).ToString().Replace(",", "."));
            }
            writer.WriteEndElement();

            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "100%");
            writer.WriteAttributeString("stop-color", string.Format("#{0:X2}{1:X2}{2:X2}", gb.StartColor.R, gb.StartColor.G, gb.StartColor.B));
            if (gb.StartColor.A != 0xFF)
            {
                writer.WriteAttributeString("stop-opacity", Math.Round(gb.StartColor.A / 255f, 3).ToString().Replace(",", "."));
            }
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            #endregion

            return gradientId;
        }

        public static string WriteGradientBrush(XmlTextWriter writer, object brush, RectangleF rect)
        {
            var gb = brush as StiGradientBrush;

            #region Calculate gradient info
            double xs = 1;
            double ys = 1;
            double angle = gb.Angle;
            if (angle < 0) angle += 360;
            if ((angle >= 270) && (angle < 360))
            {
                angle = 360 - angle;
                ys = -1;
            };
            if ((angle >= 180) && (angle < 270))
            {
                angle = angle - 180;
                ys = -1;
                xs = -1;
            };
            if ((angle >= 90) && (angle < 180))
            {
                angle = 180 - angle;
                xs = -1;
            };
            angle = angle * Math.PI / 180f;

            double x0 = rect.X + rect.Width / 2f;
            double y0 = rect.Y + rect.Height / 2f;
            double r = Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height) / 2f;
            double a2 = Math.Atan2(rect.Height, rect.Width);
            double st = Math.PI / 2f - angle + a2;
            double b = r * Math.Sin(st);
            double xr = b * Math.Cos(angle) * xs;
            double yr = b * Math.Sin(angle) * (-ys);

            double x1 = x0 - xr;
            double x2 = x0 + xr;
            double y1 = y0 + yr;
            double y2 = y0 - yr;
            #endregion

            #region Create gradient
            string gradientId = string.Format("gradient{0}", StiGuidUtils.NewGuid());

            writer.WriteStartElement("linearGradient");
            writer.WriteAttributeString("id", gradientId);
            writer.WriteAttributeString("gradientUnits", "userSpaceOnUse");
            writer.WriteAttributeString("x1", x1.ToString().Replace(",", "."));
            writer.WriteAttributeString("y1", y1.ToString().Replace(",", "."));
            writer.WriteAttributeString("x2", x2.ToString().Replace(",", "."));
            writer.WriteAttributeString("y2", y2.ToString().Replace(",", "."));
            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "0%");
            writer.WriteAttributeString("stop-color", string.Format("#{0:X2}{1:X2}{2:X2}", gb.StartColor.R, gb.StartColor.G, gb.StartColor.B));
            if (gb.StartColor.A != 0xFF)
            {
                writer.WriteAttributeString("stop-opacity", Math.Round(gb.StartColor.A / 255f, 3).ToString().Replace(",", "."));
            }
            writer.WriteEndElement();
            writer.WriteStartElement("stop");
            writer.WriteAttributeString("offset", "100%");
            writer.WriteAttributeString("stop-color", string.Format("#{0:X2}{1:X2}{2:X2}", gb.EndColor.R, gb.EndColor.G, gb.EndColor.B));
            if (gb.EndColor.A != 0xFF)
            {
                writer.WriteAttributeString("stop-opacity", Math.Round(gb.EndColor.A / 255f, 3).ToString().Replace(",", "."));
            }
            writer.WriteEndElement();
            writer.WriteFullEndElement();
            #endregion

            return gradientId;
        }

        public static string WriteGlassBrush(XmlTextWriter writer, object brush, RectangleF rect)
        {
            var gb = brush as StiGlassBrush;

            var topColor = gb.GetTopColor();
            var topRect = gb.GetTopRectangle(rect);

            var bottomColor = gb.GetBottomColor();
            var bottomRectangle = gb.GetBottomRectangle(rect);

            var brushId = String.Format("glass{0}", StiGuidUtils.NewGuid());

            writer.WriteStartElement("pattern");
            writer.WriteAttributeString("id", brushId);
            writer.WriteAttributeString("x", rect.X.ToString().Replace(",", "."));
            writer.WriteAttributeString("y", rect.Y.ToString().Replace(",", "."));
            writer.WriteAttributeString("width", rect.Width.ToString().Replace(",", "."));
            writer.WriteAttributeString("height", rect.Height.ToString().Replace(",", "."));
            writer.WriteAttributeString("patternUnits", "userSpaceOnUse");
            
            writer.WriteStartElement("rect");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteAttributeString("width", rect.Width.ToString().Replace(",", "."));
            writer.WriteAttributeString("height", rect.Height.ToString().Replace(",", "."));
            writer.WriteAttributeString("style", string.Format("fill:rgb({0},{1},{2});fill-opacity:{3};", bottomColor.R, bottomColor.G, bottomColor.B, Math.Round(bottomColor.A / 255f, 3).ToString().Replace(",", ".")));
            writer.WriteEndElement();

            writer.WriteStartElement("rect");
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteAttributeString("width", topRect.Width.ToString().Replace(",", "."));
            writer.WriteAttributeString("height", topRect.Height.ToString().Replace(",", "."));
            writer.WriteAttributeString("style", string.Format("fill:rgb({0},{1},{2});fill-opacity:{3};", topColor.R, topColor.G, topColor.B, Math.Round(topColor.A / 255f, 3).ToString().Replace(",", ".")));
            writer.WriteEndElement();

            writer.WriteEndElement();

            return brushId;
        }

        public static string WriteHatchBrush(XmlTextWriter writer, object brush)
        {
            var hatch = brush as StiHatchBrush;

            var foreColor = hatch.ForeColor;
            var backColor = hatch.BackColor;

            var hatchNumber = (int)hatch.Style;
            if (hatchNumber > 53) hatchNumber = 53;

            var brushId = String.Format("hatch{0}", StiGuidUtils.NewGuid());

            writer.WriteStartElement("pattern");
            writer.WriteAttributeString("id", brushId);
            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteAttributeString("width", "8");
            writer.WriteAttributeString("height", "8");
            writer.WriteAttributeString("patternUnits", "userSpaceOnUse");

            var sb = new StringBuilder();

            var hatchHex = hatchData[hatchNumber];

            for (var index = 0; index < 16; index++)
            {
                sb.Append(HexToByteString(hatchHex.Substring(index, 1)));
            }

            writer.WriteStartElement("rect");

            writer.WriteAttributeString("x", "0");
            writer.WriteAttributeString("y", "0");
            writer.WriteAttributeString("width", "8");
            writer.WriteAttributeString("height", "8");
            writer.WriteAttributeString("fill", string.Format("#{0:X2}{1:X2}{2:X2}", backColor.R, backColor.G, backColor.B));

            writer.WriteEndElement();

            for (var indexRow = 0; indexRow < 8; indexRow++)
            {
                for (var indexColumn = 0; indexColumn < 8; indexColumn++)
                {

                    var indexChar = sb[indexRow * 8 + indexColumn];

                    if (indexChar.ToString() == "1")
                    {
                        writer.WriteStartElement("rect");

                        writer.WriteAttributeString("x", indexColumn.ToString());
                        writer.WriteAttributeString("y", indexRow.ToString());
                        writer.WriteAttributeString("width", "1");
                        writer.WriteAttributeString("height", "1");
                        writer.WriteAttributeString("fill", string.Format("#{0:X2}{1:X2}{2:X2}", foreColor.R, foreColor.G, foreColor.B));

                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();

            return brushId;
        }

        private static string HexToByteString(string hex)
        {
            var result = "0000";
            switch (hex)
            {
                case "1":
                    result = "0001";
                    break;

                case "2":
                    result = "0010";
                    break;

                case "3":
                    result = "0011";
                    break;

                case "4":
                    result = "0100";
                    break;

                case "5":
                    result = "0101";
                    break;

                case "6":
                    result = "0110";
                    break;

                case "7":
                    result = "0111";
                    break;

                case "8":
                    result = "1000";
                    break;

                case "9":
                    result = "1001";
                    break;

                case "A":
                    result = "1010";
                    break;

                case "B":
                    result = "1011";
                    break;

                case "C":
                    result = "1100";
                    break;

                case "D":
                    result = "1101";
                    break;

                case "E":
                    result = "1110";
                    break;

                case "F":
                    result = "1111";
                    break;
            }

            return result;
        }

        public static string GetFillBrush(XmlTextWriter writer, object brush, RectangleF sRect)
        {
            if (brush is Color)
            {
                var color = (Color)brush;

                if (color.A == 0)
                    return "transparent";

                return string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, Math.Round(color.A / 255f, 3).ToString().Replace(",", "."));
            }
            else if (brush is StiGradientBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGradientBrush(writer, brush, sRect);

                return string.Format("url(#{0})", gradientId);
            }
            else if (brush is StiGlareBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlareBrush(writer, brush, sRect);

                return string.Format("url(#{0})", gradientId);
            }
            else if (brush is StiGlassBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteGlassBrush(writer, brush, sRect);

                return string.Format("url(#{0})", gradientId);
            }
            else if (brush is StiHatchBrush)
            {
                var gradientId = StiBrushSvgHelper.WriteHatchBrush(writer, brush);

                return string.Format("url(#{0})", gradientId);
            }
            else if (brush is StiBrush || brush is SolidBrush)
            {
                var color = brush is StiBrush ? StiBrush.ToColor(brush as StiBrush) : (brush as SolidBrush).Color;
                return string.Format("rgba({0},{1},{2},{3})", color.R, color.G, color.B, Math.Round(color.A / 255f, 3).ToString().Replace(",", "."));
            }

            return "none";
        }
        #endregion
    }
}
