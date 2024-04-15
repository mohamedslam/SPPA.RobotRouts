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

using Stimulsoft.Base.Context;
using System.Xml;
using Stimulsoft.Report.Chart;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export.Services.Helpers;
using System;
using Stimulsoft.Base.Localization;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Report.Export
{
    public static class StiChartSvgHelper
    {
        #region Methods
        public static void WriteChart(XmlTextWriter writer, StiSvgData svgData, bool needAnimation)
        {
            WriteChart(writer, svgData, 1f, needAnimation);
        }

        public static void WriteChart(XmlTextWriter writer, StiSvgData svgData, float zoom, bool needAnimation)
        {
            var chart = svgData.Component as StiChart;
            chart.IsAnimation = needAnimation;
            StiContext context = null;

            if (chart.Series.Count == 0) 
            {
                WriteEmptyDataMessage(writer, svgData);
                chart.IsAnimation = false;
                return;
            }

            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    var painter = new Stimulsoft.Report.Painters.StiGdiContextPainter(g);
                    context = new StiContext(painter, true, false, false, zoom);

                    bool storeIsAnimation = chart.IsAnimation;
                    chart.IsAnimationChangingValues = needAnimation;
                    
                    var report = chart.Report;
                    var svgWidth = (float)svgData.Width;
                    var svgHeight = (float)svgData.Height;
                    if (chart.Rotation == StiImageRotation.Rotate90CCW || chart.Rotation == StiImageRotation.Rotate90CW)
                    {
                        svgWidth = (float)svgData.Height;
                        svgHeight = (float)svgData.Width;
                    }
                    var chartGeom = chart.Core.Render(context,new RectangleF(0, 0, svgWidth, svgHeight),true);

                    chartGeom.DrawGeom(context);
                    chart.PreviousAnimations = context.Animations;

                    chart.IsAnimation = storeIsAnimation;

                    writer.WriteStartElement("g");
                    var translateX = svgData.X + 0.5;
                    var translateY = svgData.Y + 0.5;
                    var rotation = "";
                    switch (chart.Rotation)
                    {
                        case StiImageRotation.Rotate90CW:
                            rotation = " rotate(90)";
                            translateX += svgHeight;
                            break;
                        case StiImageRotation.Rotate90CCW:
                            rotation = " rotate(-90)";
                            translateY += svgWidth;
                            break;
                        case StiImageRotation.Rotate180:
                            rotation = " rotate(180)";
                            translateY += svgHeight;
                            translateX += svgWidth;
                            break;
                        case StiImageRotation.FlipHorizontal:
                            rotation = " scale(-1, 1)";
                            translateX += svgWidth;
                            break;
                        case StiImageRotation.FlipVertical :
                            rotation = " scale(1, -1)";
                            translateY += svgHeight;
                            break;
                    }
                    
                    writer.WriteAttributeString("transform",
                        string.Format("translate({0},{1}){2}", StiContextSvgHelper.DoubleToString(translateX), StiContextSvgHelper.DoubleToString(translateY), rotation));

                    StiContextSvgHelper.WriteGeoms(writer, context, needAnimation);

                    writer.WriteEndElement();
                }
            }
            chart.IsAnimation = false;
        }

        private static void WriteEmptyDataMessage(XmlTextWriter writer, StiSvgData svgData)
        {
            if (svgData.Width > 50)
            {
                writer.WriteStartElement("image");
                writer.WriteAttributeString("x", StiContextSvgHelper.DoubleToString(svgData.Width / 2 - 16));
                writer.WriteAttributeString("y", StiContextSvgHelper.DoubleToString(svgData.Height / 2 - 16));
                writer.WriteAttributeString("width", "32");
                writer.WriteAttributeString("height", "32");
                writer.WriteStartAttribute("href");
                writer.WriteString($"data:image/png;base64,{Convert.ToBase64String(GetEmptyDataImage())}");
                writer.WriteRaw("\r\n");
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
            
            var font = new Font("Arial", 8);
            var text = Loc.Get("PropertyMain", "NoElements");
            var textSize = MeasureTextWidth(text, font);

            if (svgData.Width > textSize + 30)
            {
                writer.WriteStartElement("text");
                writer.WriteAttributeString("x", StiContextSvgHelper.DoubleToString(svgData.Width / 2 - textSize / 2));
                writer.WriteAttributeString("y", StiContextSvgHelper.DoubleToString(svgData.Height / 2 + 30));
                writer.WriteAttributeString("style", "font-size:12px; font-family:'Arial'; fill: #a9a9a9;");
                writer.WriteRaw(text);
                writer.WriteEndElement();
            }
        }

        private static byte[] GetEmptyDataImage()
        {
            var assembly = typeof(StiChartSvgHelper).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"Stimulsoft.Report.Images.NotDefined.StiChart_x2.png"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        private static double MeasureTextWidth(string text, Font baseFont)
        {
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            {
                return g.MeasureString(text, baseFont).Width;
            }
        }
        #endregion
    }
}