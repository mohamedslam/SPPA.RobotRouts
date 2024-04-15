#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Globalization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Export
{
    public partial class StiPdfExportService
    {
        #region CheckShape
        private bool CheckShape(StiShape shape)
        {
            if ((shape.ShapeType is StiVerticalLineShapeType) ||
                (shape.ShapeType is StiHorizontalLineShapeType) ||
                (shape.ShapeType is StiTopAndBottomLineShapeType) ||
                (shape.ShapeType is StiLeftAndRightLineShapeType) ||
                (shape.ShapeType is StiRectangleShapeType) ||
                (shape.ShapeType is StiRoundedRectangleShapeType) ||
                (shape.ShapeType is StiDiagonalDownLineShapeType) ||
                (shape.ShapeType is StiDiagonalUpLineShapeType) ||
                (shape.ShapeType is StiTriangleShapeType) ||
                (shape.ShapeType is StiOvalShapeType) ||
                (shape.ShapeType is StiArrowShapeType) ||
                (shape.ShapeType is StiOctagonShapeType) ||
                (shape.ShapeType is StiComplexArrowShapeType) ||
                (shape.ShapeType is StiBentArrowShapeType) ||
                (shape.ShapeType is StiChevronShapeType) ||
                (shape.ShapeType is StiDivisionShapeType) ||
                (shape.ShapeType is StiEqualShapeType) ||
                (shape.ShapeType is StiFlowchartCardShapeType) ||
                (shape.ShapeType is StiFlowchartCollateShapeType) ||
                (shape.ShapeType is StiFlowchartDecisionShapeType) ||
                (shape.ShapeType is StiFlowchartManualInputShapeType) ||
                (shape.ShapeType is StiFlowchartOffPageConnectorShapeType) ||
                (shape.ShapeType is StiFlowchartPreparationShapeType) ||
                (shape.ShapeType is StiFlowchartSortShapeType) ||
                (shape.ShapeType is StiFrameShapeType) ||
                (shape.ShapeType is StiMinusShapeType) ||
                (shape.ShapeType is StiMultiplyShapeType) ||
                (shape.ShapeType is StiParallelogramShapeType) ||
                (shape.ShapeType is StiPlusShapeType) ||
                (shape.ShapeType is StiRegularPentagonShapeType) ||
                (shape.ShapeType is StiTrapezoidShapeType) ||
                (shape.ShapeType is StiSnipSameSideCornerRectangleShapeType) ||
                (shape.ShapeType is StiSnipDiagonalSideCornerRectangleShapeType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region RenderShape
        private void RenderShape(StiPdfData pp, float imageResolution)
        {
            StiShape shape = pp.Component as StiShape;
            if (shape != null)
            {
                if (CheckShape(shape) == true)
                {
                    #region Render primitive
                    IStiBrush mBrush = pp.Component as IStiBrush;

                    StiPdfGeomWriter geomWriter = new StiPdfGeomWriter(pageStream, this, false);

                    #region Fillcolor
                    Color tempColor = Color.Transparent;
                    if (mBrush != null) tempColor = StiBrush.ToColor(mBrush.Brush);
                    if (tempColor.A != 0)
                    {
                        SetNonStrokeColor(tempColor);
                    }

                    if (mBrush != null)
                    {
                        if (mBrush.Brush is StiGradientBrush || mBrush.Brush is StiGlareBrush)
                        {
                            StoreShadingData2(pp.X, pp.Y, pp.Width, pp.Height, mBrush.Brush);
                            pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
                        }
                        if (mBrush.Brush is StiHatchBrush)
                        {
                            StiHatchBrush hBrush = mBrush.Brush as StiHatchBrush;
                            pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(hBrush) + 1);
                        }
                    }
                    #endregion

                    //stroke color
                    Color tempColor2 = shape.BorderColor;
                    SetStrokeColor(tempColor2);

                    bool needFill = tempColor.A != 0;
                    bool needStroke = shape.Style != StiPenStyle.None;

                    if (!needFill && !needStroke) return;

                    string st = needFill ? (needStroke ? "B" : "f") : (needStroke ? "S" : "n");

                    pageStream.WriteLine("{0} w", ConvertToString(shape.Size * hiToTwips));

                    pageStream.WriteLine("q");

                    #region set line style
                    double step = shape.Size * hiToTwips * 0.04;
                    switch (shape.Style)
                    {
                        case StiPenStyle.Dot:
                            pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 55));
                            break;

                        case StiPenStyle.Dash:
                            pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55));
                            break;

                        case StiPenStyle.DashDot:
                            pageStream.WriteLine("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                            break;

                        case StiPenStyle.DashDotDot:
                            pageStream.WriteLine("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                            break;
                    }
                    #endregion

                    #region VerticalLine
                    if (shape.ShapeType is StiVerticalLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height));
                        }
                    }
                    #endregion

                    #region HorizontalLine
                    if (shape.ShapeType is StiHorizontalLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height / 2));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height / 2));
                        }
                    }
                    #endregion

                    #region TopAndBottomLine
                    if (shape.ShapeType is StiTopAndBottomLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                        }
                    }
                    #endregion

                    #region LeftAndRightLine
                    if (shape.ShapeType is StiLeftAndRightLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                        }
                    }
                    #endregion

                    #region Rectangle
                    if (shape.ShapeType is StiRectangleShapeType)
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X), ConvertToString(pp.Y));
                    }
                    #endregion

                    #region DiagonalDownLine
                    if (shape.ShapeType is StiDiagonalDownLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                        }
                    }
                    #endregion

                    #region DiagonalUpLine
                    if (shape.ShapeType is StiDiagonalUpLineShapeType)
                    {
                        if (needFill)
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                ConvertToString(pp.X),
                                ConvertToString(pp.Y),
                                ConvertToString(pp.Width),
                                ConvertToString(pp.Height));
                        }
                        if (needStroke)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                        }
                    }
                    #endregion

                    #region Triangle
                    if (shape.ShapeType is StiTriangleShapeType)
                    {
                        StiShapeDirection ssd = (shape.ShapeType as StiTriangleShapeType).Direction;
                        if (ssd == StiShapeDirection.Up)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X), ConvertToString(pp.Y));
                        }
                        if (ssd == StiShapeDirection.Down)
                        {
                            if (needFill) st = needStroke ? "B*" : "f*"; else st = needStroke ? "S" : "n";
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                        }
                        if (ssd == StiShapeDirection.Left)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height / 2));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y));
                        }
                        if (ssd == StiShapeDirection.Right)
                        {
                            if (needFill) st = needStroke ? "B*" : "f*"; else st = needStroke ? "S" : "n";
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height / 2));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X), ConvertToString(pp.Y));
                        }
                    }
                    #endregion

                    #region Oval
                    if (shape.ShapeType is StiOvalShapeType)
                    {
                        pageStream.WriteLine(geomWriter.GetEllipseString(new RectangleF((float)pp.X, (float)pp.Y, (float)pp.Width, (float)pp.Height)) + st);
                    }
                    #endregion

                    #region RoundedRectangle
                    if (shape.ShapeType is StiRoundedRectangleShapeType)
                    {
                        float rnd = (shape.ShapeType as StiRoundedRectangleShapeType).Round;
                        double side = pp.Width;
                        if (side > pp.Height) side = pp.Height;
                        //double offs = Math.Min(side, 100 * shape.Page.Zoom) * rnd;
                        double offs = Math.Min(side, 70) * rnd;
                        double tmp = offs * (1 - pdfCKT);

                        pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + offs));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - offs));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - tmp),
                            ConvertToString(pp.X + tmp), ConvertToString(pp.Y + pp.Height),
                            ConvertToString(pp.X + offs), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - offs), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(pp.X + pp.Width - tmp), ConvertToString(pp.Y + pp.Height),
                            ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height - tmp),
                            ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height - offs));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + offs));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + tmp),
                            ConvertToString(pp.X + pp.Width - tmp), ConvertToString(pp.Y),
                            ConvertToString(pp.X + pp.Width - offs), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + offs), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c " + st,
                            ConvertToString(pp.X + tmp), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y + tmp),
                            ConvertToString(pp.X), ConvertToString(pp.Y + offs));
                    }
                    #endregion

                    #region Arrow
                    if (shape.ShapeType is StiArrowShapeType)
                    {
                        StiShapeDirection ssd = (shape.ShapeType as StiArrowShapeType).Direction;
                        float arrowW = (shape.ShapeType as StiArrowShapeType).ArrowWidth;
                        float arrowH = (shape.ShapeType as StiArrowShapeType).ArrowHeight;
                        double arw = pp.Width * arrowW;
                        double arh = pp.Height * arrowH;
                        if ((ssd == StiShapeDirection.Left) || (ssd == StiShapeDirection.Right))
                        {
                            arw = pp.Height * arrowW;
                            arh = pp.Width * arrowH;
                        }
                        if (arrowH == 0) arh = Math.Min(pp.Width / 2, pp.Height / 2);

                        if (ssd == StiShapeDirection.Up)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + arw), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arw), ConvertToString(pp.Y + pp.Height - arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height - arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arw), ConvertToString(pp.Y + pp.Height - arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arw), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X + arw), ConvertToString(pp.Y));
                        }
                        if (ssd == StiShapeDirection.Down)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width - arw), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arw), ConvertToString(pp.Y + arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arw), ConvertToString(pp.Y + arh));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arw), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X + pp.Width - arw), ConvertToString(pp.Y + pp.Height));
                        }
                        if (ssd == StiShapeDirection.Left)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arh), ConvertToString(pp.Y + arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arh), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height / 2));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arh), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arh), ConvertToString(pp.Y + pp.Height - arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height - arw));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + arw));
                        }
                        if (ssd == StiShapeDirection.Right)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arh), ConvertToString(pp.Y + pp.Height - arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arh), ConvertToString(pp.Y + pp.Height));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height / 2));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arh), ConvertToString(pp.Y));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - arh), ConvertToString(pp.Y + arw));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + arw));
                            pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - arw));
                        }
                    }
                    #endregion

                    #region Octagon
                    if (shape.ShapeType is StiOctagonShapeType)
                    {
                        var octagonShape = (StiOctagonShapeType)shape.ShapeType;
                        double bevelx = (shape.Report != null ? (float)shape.Report.Unit.ConvertToHInches(octagonShape.Bevel) : octagonShape.Bevel) * hiToTwips;
                        double bevely = bevelx;
                        if (octagonShape.AutoSize)
                        {
                            bevelx = pp.Width / (2.414f * 1.414f);
                            bevely = pp.Height / (2.414f * 1.414f);
                        }
                        if (bevelx > pp.Width / 2) bevelx = pp.Width / 2;
                        if (bevely > pp.Height / 2) bevely = pp.Height / 2;

                        pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X + bevelx), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - bevelx), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + bevely));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width), ConvertToString(pp.Y + pp.Height - bevely));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width - bevelx), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + bevelx), ConvertToString(pp.Y + pp.Height));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height - bevely));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + bevely));
                        pageStream.WriteLine("{0} {1} l " + st, ConvertToString(pp.X + bevelx), ConvertToString(pp.Y));
                    }
                    #endregion

                    #region ComplexArrow
                    if (shape.ShapeType is StiComplexArrowShapeType)
                    {
                        double restHeight = (pp.Width < pp.Height) ? pp.Width / 2 : pp.Height / 2;
                        double topBottomSpace = (pp.Height / 3.8f);
                        double leftRightSpace = (pp.Width / 3.8f);
                        double restWidth = (pp.Height < pp.Width) ? pp.Height / 2 : pp.Width / 2;

                        switch ((shape.ShapeType as StiComplexArrowShapeType).Direction)
                        {
                            case StiShapeDirection.Left:
                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + restHeight), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + restHeight), ConvertToString(pp.Y + topBottomSpace));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - restHeight), ConvertToString(pp.Y + topBottomSpace));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - restHeight), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y + pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - restHeight), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - restHeight), ConvertToString(pp.Top - topBottomSpace));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + restHeight), ConvertToString(pp.Top - topBottomSpace));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + restHeight), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Y + pp.Height / 2));
                                break;

                            case StiShapeDirection.Down:
                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y + restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y + restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - leftRightSpace), ConvertToString(pp.Y + restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - leftRightSpace), ConvertToString(pp.Top - restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Top - restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + leftRightSpace), ConvertToString(pp.Top - restWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + leftRightSpace), ConvertToString(pp.Y + restWidth));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Y + restWidth));
                                break;
                        }
                    }
                    #endregion

                    #region BentArrow
                    if (shape.ShapeType is StiBentArrowShapeType)
                    {
                        double lineHeight = 0;
                        double arrowWidth = 0;
                        double space = 0;
                        if (pp.Height > pp.Width)
                        {
                            arrowWidth = pp.Width / 4;
                            lineHeight = arrowWidth;
                            space = arrowWidth / 2;
                        }
                        else
                        {
                            lineHeight = (int)(pp.Height / 4);
                            arrowWidth = lineHeight;
                            space = arrowWidth / 2;
                        }

                        switch ((shape.ShapeType as StiBentArrowShapeType).Direction)
                        {
                            #region Up
                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + lineHeight));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - (space + lineHeight)), ConvertToString(pp.Y + lineHeight));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - (space + lineHeight)), ConvertToString(pp.Top - arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth * 2), ConvertToString(pp.Top - arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - space), ConvertToString(pp.Top - arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - space), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Y));
                                break;
                            #endregion

                            #region Left
                            case StiShapeDirection.Left:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.Right), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth), ConvertToString(pp.Top - space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Top - arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth), ConvertToString(pp.Top - arrowWidth * 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth), ConvertToString(pp.Top - arrowWidth - space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - lineHeight), ConvertToString(pp.Top - arrowWidth - space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - lineHeight), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.Right), ConvertToString(pp.Y));
                                break;
                            #endregion

                            #region Down
                            case StiShapeDirection.Down:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.Right), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + space), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + space), ConvertToString(pp.Y + arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth * 2), ConvertToString(pp.Y + arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth + space), ConvertToString(pp.Y + arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + arrowWidth + space), ConvertToString(pp.Top - lineHeight));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - lineHeight));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.Right), ConvertToString(pp.Top));
                                break;
                            #endregion

                            #region Right
                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth), ConvertToString(pp.Y + space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y + arrowWidth));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth), ConvertToString(pp.Y + arrowWidth * 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - arrowWidth), ConvertToString(pp.Y + arrowWidth + space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + lineHeight), ConvertToString(pp.Y + arrowWidth + space));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + lineHeight), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Top));
                                break;
                                #endregion
                        }
                    }
                    #endregion

                    #region Chevron
                    if (shape.ShapeType is StiChevronShapeType)
                    {
                        double rest = (pp.Width > pp.Height) ? (pp.Height / 2) : (pp.Width / 2);

                        switch ((shape.ShapeType as StiChevronShapeType).Direction)
                        {
                            #region Right
                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + rest), ConvertToString(pp.Top - pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - rest), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - rest), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Top));
                                break;
                            #endregion

                            #region Left
                            case StiShapeDirection.Left:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.Right), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + rest), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + rest), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right - rest), ConvertToString(pp.Top - pp.Height / 2));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.Right), ConvertToString(pp.Top));
                                break;
                            #endregion

                            #region Up
                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Top - rest));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top - rest));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + rest));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Top - rest));
                                break;
                            #endregion

                            #region Down
                            case StiShapeDirection.Down:
                                pageStream.WriteLine("{0} {1} m", ConvertToString(pp.X), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top - rest));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.Right), ConvertToString(pp.Y + rest));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y));
                                pageStream.WriteLine("{0} {1} l", ConvertToString(pp.X), ConvertToString(pp.Y + rest));
                                pageStream.WriteLine("{0} {1} l h " + st, ConvertToString(pp.X), ConvertToString(pp.Top));
                                break;
                                #endregion
                        }
                    }
                    #endregion

                    #region Division
                    if (shape.ShapeType is StiDivisionShapeType)
                    {
                        double restHeight = pp.Height / 3;

                        float offset = (float)(4 * hiToTwips);
                        pageStream.WriteLine(geomWriter.GetRectString(pp.X, pp.Y + restHeight + offset, pp.Width, restHeight - offset * 2) + st);
                        pageStream.WriteLine(geomWriter.GetEllipseString(pp.X + pp.Width / 2 - restHeight / 2, pp.Top - hiToTwips - restHeight, restHeight, restHeight) + st);
                        pageStream.WriteLine(geomWriter.GetEllipseString(pp.X + pp.Width / 2 - restHeight / 2, pp.Y + 2 * hiToTwips, restHeight, restHeight) + st);
                    }
                    #endregion

                    #region Equal
                    if (shape.ShapeType is StiEqualShapeType)
                    {
                        double height = (pp.Height - (pp.Height / 6)) / 2;

                        pageStream.WriteLine(geomWriter.GetRectString(pp.X, pp.Top - height, pp.Width, height) + st);
                        pageStream.WriteLine(geomWriter.GetRectString(pp.X, pp.Y, pp.Width, height) + st);
                    }
                    #endregion

                    #region Flowchart: Card
                    if (shape.ShapeType is StiFlowchartCardShapeType)
                    {
                        pageStream.WriteLine("{0} {1} m {2} {3} l",
                            ConvertToString(pp.Right), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 5),
                            ConvertToString(pp.X + pp.Width / 5), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top));
                    }
                    #endregion

                    #region Flowchart: Collate
                    if (shape.ShapeType is StiFlowchartCollateShapeType)
                    {
                        switch ((shape.ShapeType as StiFlowchartCollateShapeType).Direction)
                        {
                            case StiShapeDirection.Down:
                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l h " + st,
                                    ConvertToString(pp.X), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top),
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height / 2));
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l h " + st,
                                    ConvertToString(pp.X), ConvertToString(pp.Y),
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height / 2),
                                    ConvertToString(pp.Right), ConvertToString(pp.Y));
                                break;

                            case StiShapeDirection.Left:
                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l h " + st,
                                    ConvertToString(pp.X), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Top),
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height / 2));
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l h " + st,
                                    ConvertToString(pp.Right), ConvertToString(pp.Y),
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height / 2),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top));
                                break;
                        }
                    }
                    #endregion

                    #region Flowchart: Decision
                    if (shape.ShapeType is StiFlowchartDecisionShapeType)
                    {
                        pageStream.WriteLine("{0} {1} m",
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top - pp.Height / 2),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2));
                    }
                    #endregion

                    #region Flowchart: Manual Input
                    if (shape.ShapeType is StiFlowchartManualInputShapeType)
                    {
                        pageStream.WriteLine("{0} {1} m",
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 5));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.Right), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 5));
                    }
                    #endregion

                    #region Flowchart: Off Page Connector
                    if (shape.ShapeType is StiFlowchartOffPageConnectorShapeType)
                    {
                        double restHeight = pp.Height / 5;
                        double restWidth = pp.Width / 5;
                        switch ((shape.ShapeType as StiFlowchartOffPageConnectorShapeType).Direction)
                        {
                            case StiShapeDirection.Down:
                                pageStream.WriteLine("{0} {1} m {2} {3} l",
                                    ConvertToString(pp.X), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.Right), ConvertToString(pp.Y + restHeight),
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Y + restHeight),
                                    ConvertToString(pp.X), ConvertToString(pp.Top));
                                break;

                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m {2} {3} l",
                                    ConvertToString(pp.X), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Top - restHeight));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight),
                                    ConvertToString(pp.Right), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Y));
                                break;

                            case StiShapeDirection.Left:
                                pageStream.WriteLine("{0} {1} m {2} {3} l",
                                    ConvertToString(pp.X + restWidth), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.Right), ConvertToString(pp.Y),
                                    ConvertToString(pp.X + restWidth), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2),
                                    ConvertToString(pp.X + restWidth), ConvertToString(pp.Top));
                                break;

                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m {2} {3} l",
                                    ConvertToString(pp.X), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.Right), ConvertToString(pp.Top - pp.Height / 2),
                                    ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Top));
                                break;
                        }
                    }
                    #endregion

                    #region Flowchart: Preparation
                    if (shape.ShapeType is StiFlowchartPreparationShapeType)
                    {
                        double restWidth = pp.Width / 5;
                        double restHeight = pp.Height / 5;
                        double xCenter = pp.Width / 2;
                        double yCenter = pp.Height / 2;

                        switch ((shape.ShapeType as StiFlowchartPreparationShapeType).Direction)
                        {
                            case StiShapeDirection.Left:
                            case StiShapeDirection.Right:
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l",
                                    ConvertToString(pp.X), ConvertToString(pp.Top - yCenter),
                                    ConvertToString(pp.X + restWidth), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.Right), ConvertToString(pp.Top - yCenter),
                                    ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y),
                                    ConvertToString(pp.X + restWidth), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Top - yCenter));
                                break;

                            case StiShapeDirection.Down:
                            case StiShapeDirection.Up:
                                pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l",
                                    ConvertToString(pp.X + xCenter), ConvertToString(pp.Top),
                                    ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight),
                                    ConvertToString(pp.Right), ConvertToString(pp.Y + restHeight));
                                pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                                    ConvertToString(pp.X + xCenter), ConvertToString(pp.Y),
                                    ConvertToString(pp.X), ConvertToString(pp.Y + restHeight),
                                    ConvertToString(pp.X), ConvertToString(pp.Top - restHeight),
                                    ConvertToString(pp.X + xCenter), ConvertToString(pp.Top));
                                break;
                        }
                    }
                    #endregion

                    #region Flowchart: Sort
                    if (shape.ShapeType is StiFlowchartSortShapeType)
                    {
                        pageStream.WriteLine("{0} {1} m {2} {3} l",
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.Right), ConvertToString(pp.Top - pp.Height / 2),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Top - pp.Height / 2),
                            ConvertToString(pp.Right), ConvertToString(pp.Top - pp.Height / 2));
                    }
                    #endregion

                    #region Frame
                    if (shape.ShapeType is StiFrameShapeType)
                    {
                        double restWidth = pp.Width / 7;
                        double restHeight = pp.Height / 7;

                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                            ConvertToString(pp.X), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y));
                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top - restHeight));
                    }
                    #endregion

                    #region Minus
                    if (shape.ShapeType is StiMinusShapeType)
                    {
                        double restHeight = pp.Height / 3;

                        pageStream.WriteLine(geomWriter.GetRectString(pp.X, pp.Y + restHeight, pp.Width, restHeight) + st);
                    }
                    #endregion

                    #region Multiply
                    if (shape.ShapeType is StiMultiplyShapeType)
                    {
                        double restWidth = pp.Width / 4;
                        double restHeight = pp.Height / 4;

                        pageStream.WriteLine("{0} {1} m",
                            ConvertToString(pp.X), ConvertToString(pp.Top - restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l",
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l",
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top - pp.Height / 2),
                            ConvertToString(pp.Right), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l h " + st,
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top - pp.Height / 2));
                    }
                    #endregion

                    #region Parallelogram
                    if (shape.ShapeType is StiParallelogramShapeType)
                    {
                        double restWidth = pp.Width / 7;
                        double restHeight = pp.Height / 7;

                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X), ConvertToString(pp.Y),
                            ConvertToString(pp.X + pp.Width / 5), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top),
                            ConvertToString(pp.Right - pp.Width / 5), ConvertToString(pp.Y));
                    }
                    #endregion

                    #region Plus
                    if (shape.ShapeType is StiPlusShapeType)
                    {
                        double restWidth = pp.Width / 3;
                        double restHeight = pp.Height / 3;

                        pageStream.WriteLine("{0} {1} m",
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l",
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.Right), ConvertToString(pp.Y + restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l",
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Y),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Y),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Y + restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X), ConvertToString(pp.Y + restHeight),
                            ConvertToString(pp.X), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top));
                    }
                    #endregion

                    #region Regular: Pentagon
                    if (shape.ShapeType is StiRegularPentagonShapeType)
                    {
                        double restTop = pp.Height / 2.6f;
                        double restLeft = pp.Width / 5.5f;

                        pageStream.WriteLine("{0} {1} m {2} {3} l",
                            ConvertToString(pp.X), ConvertToString(pp.Top - restTop),
                            ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Top));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.Right), ConvertToString(pp.Top - restTop),
                            ConvertToString(pp.Right - restLeft), ConvertToString(pp.Y),
                            ConvertToString(pp.X + restLeft), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Top - restTop));
                    }
                    #endregion

                    #region Trapezoid
                    if (shape.ShapeType is StiTrapezoidShapeType)
                    {
                        double rest = pp.Width / 4.75f;
                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h " + st,
                            ConvertToString(pp.X), ConvertToString(pp.Y),
                            ConvertToString(pp.X + rest), ConvertToString(pp.Top),
                            ConvertToString(pp.Right - rest), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Y));
                    }
                    #endregion

                    #region Snip Same Side Corner Rectangle
                    if (shape.ShapeType is StiSnipSameSideCornerRectangleShapeType)
                    {
                        double restWidth = pp.Width / 7.2f;
                        double restHeight = pp.Height / 4.6f;

                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(pp.X), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Top),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l h " + st,
                            ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight),
                            ConvertToString(pp.Right), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y));
                    }
                    #endregion

                    #region Snip Diagonal Side Corner Rectangle
                    if (shape.ShapeType is StiSnipDiagonalSideCornerRectangleShapeType)
                    {
                        double restWidth = pp.Width / 7.2f;
                        double restHeight = pp.Height / 4.6f;

                        pageStream.WriteLine("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(pp.X), ConvertToString(pp.Top),
                            ConvertToString(pp.Right - restWidth), ConvertToString(pp.Top),
                            ConvertToString(pp.Right), ConvertToString(pp.Top - restHeight));
                        pageStream.WriteLine("{0} {1} l {2} {3} l {4} {5} l h " + st,
                            ConvertToString(pp.Right), ConvertToString(pp.Y),
                            ConvertToString(pp.X + restWidth), ConvertToString(pp.Y),
                            ConvertToString(pp.X), ConvertToString(pp.Y + restHeight));
                    }
                    #endregion

                    pageStream.WriteLine("Q");

                    #endregion

                    var parserText = shape.GetParsedText();
                    if (!string.IsNullOrWhiteSpace(parserText))
                    {
                        StiText txt = new StiText(shape.ClientRectangle, parserText)
                        {
                            Font = shape.Font,
                            TextBrush = new StiSolidBrush(shape.ForeColor),
                            HorAlignment = shape.HorAlignment,
                            VertAlignment = shape.VertAlignment,
                            Margins = shape.Margins,
                            WordWrap = true,
                            TextQuality = StiTextQuality.Typographic,
                            Page = shape.Page
                        };
                        StiPdfData pp2 = pp.Clone();
                        pp2.Component = txt;
                        RenderTextFont(pp2);
                        RenderText(pp2);
                    }
                }
                else
                {
                    float ir = imageResolution;
                    using (Image image = shape.GetImage(ref imageResolution))
                    {
                        WriteImageInfo(pp, ir);
                    }
                }
            }
        }
        #endregion

        #region RenderPrimitives
        private void RenderPrimitives(StiPdfData pp)
        {
            if (pp.Component is StiRoundedRectanglePrimitive)
            {
                RenderRoundedRectanglePrimitive(pp);
            }
            else
            {
                RenderLinePrimitive(pp);
            }
        }

        private void RenderRoundedRectanglePrimitive(StiPdfData pp)
        {
            IStiBorder mBorder = pp.Component as IStiBorder;
            if (mBorder != null)
            {
                StiRoundedRectanglePrimitive primitive = pp.Component as StiRoundedRectanglePrimitive;
                if (primitive.Style == StiPenStyle.None) return;

                StiBorderSide border = new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style);

                bool needPush = (mBorder.Border.Style != StiPenStyle.None) && (mBorder.Border.Style != StiPenStyle.Solid);
                if (needPush)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                }

                double offset = StoreBorderSideData(border);
                double roundOffset = Math.Min((pp.Width < pp.Height ? pp.Width : pp.Height), 100 * pp.Component.Page.Zoom) * primitive.Round;
                double roundOffset2 = roundOffset * (1 - pdfCKT);

                double x1 = pp.X - offset;
                double x2 = pp.X + pp.Width + offset;
                double xc = pp.X + pp.Width / 2d;
                double y1 = pp.Y - offset;
                double y2 = pp.Y + pp.Height + offset;

                #region Draw single lines
                if (primitive.LeftSide)
                {
                    if (primitive.BottomSide)
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString((primitive.RightSide ? xc : x2)), ConvertToString(y1));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(x1 + roundOffset), ConvertToString(y1));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(x1 + roundOffset2), ConvertToString(y1),
                            ConvertToString(x1), ConvertToString(y1 + roundOffset2),
                            ConvertToString(x1), ConvertToString(y1 + roundOffset));
                    }
                    else
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString(x1), ConvertToString(y1));
                    }
                    if (primitive.TopSide)
                    {
                        pageStream.WriteLine("{0} {1} l", ConvertToString(x1), ConvertToString(y2 - roundOffset));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(x1), ConvertToString(y2 - roundOffset2),
                            ConvertToString(x1 + roundOffset2), ConvertToString(y2),
                            ConvertToString(x1 + roundOffset), ConvertToString(y2));
                        pageStream.WriteLine("{0} {1} l S", ConvertToString((primitive.RightSide ? xc : x2)), ConvertToString(y2));
                    }
                    else
                    {
                        pageStream.WriteLine("{0} {1} l S", ConvertToString(x1), ConvertToString(y2));
                    }
                }
                if (primitive.RightSide)
                {
                    if (primitive.BottomSide)
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString((primitive.LeftSide ? xc : x1)), ConvertToString(y1));
                        pageStream.WriteLine("{0} {1} l", ConvertToString(x2 - roundOffset), ConvertToString(y1));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(x2 - roundOffset2), ConvertToString(y1),
                            ConvertToString(x2), ConvertToString(y1 + roundOffset2),
                            ConvertToString(x2), ConvertToString(y1 + roundOffset));
                    }
                    else
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString(x2), ConvertToString(y1));
                    }
                    if (primitive.TopSide)
                    {
                        pageStream.WriteLine("{0} {1} l", ConvertToString(x2), ConvertToString(y2 - roundOffset));
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                            ConvertToString(x2), ConvertToString(y2 - roundOffset2),
                            ConvertToString(x2 - roundOffset2), ConvertToString(y2),
                            ConvertToString(x2 - roundOffset), ConvertToString(y2));
                        pageStream.WriteLine("{0} {1} l S", ConvertToString((primitive.LeftSide ? xc : x1)), ConvertToString(y2));
                    }
                    else
                    {
                        pageStream.WriteLine("{0} {1} l S", ConvertToString(x2), ConvertToString(y2));
                    }
                }
                if (primitive.TopSide && !primitive.LeftSide && !primitive.RightSide)
                {
                    pageStream.WriteLine("{0} {1} m", ConvertToString(x1), ConvertToString(y2));
                    pageStream.WriteLine("{0} {1} l S", ConvertToString(x2), ConvertToString(y2));
                }
                if (primitive.BottomSide && !primitive.LeftSide && !primitive.RightSide)
                {
                    pageStream.WriteLine("{0} {1} m", ConvertToString(x1), ConvertToString(y1));
                    pageStream.WriteLine("{0} {1} l S", ConvertToString(x2), ConvertToString(y1));
                }
                #endregion

                if (border.Style == StiPenStyle.Double)
                {
                    #region Draw second lines
                    roundOffset -= offset * 2;
                    roundOffset2 = roundOffset * (1 - pdfCKT);

                    double x3 = pp.X + offset;
                    double x4 = pp.X + pp.Width - offset;
                    double y3 = pp.Y + offset;
                    double y4 = pp.Y + pp.Height - offset;

                    if (primitive.LeftSide)
                    {
                        if (primitive.BottomSide)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString((primitive.RightSide ? xc : x2)), ConvertToString(y3));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(x3 + roundOffset), ConvertToString(y3));
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                                ConvertToString(x3 + roundOffset2), ConvertToString(y3),
                                ConvertToString(x3), ConvertToString(y3 + roundOffset2),
                                ConvertToString(x3), ConvertToString(y3 + roundOffset));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(x3), ConvertToString(y1));
                        }
                        if (primitive.TopSide)
                        {
                            pageStream.WriteLine("{0} {1} l", ConvertToString(x3), ConvertToString(y4 - roundOffset));
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                                ConvertToString(x3), ConvertToString(y4 - roundOffset2),
                                ConvertToString(x3 + roundOffset2), ConvertToString(y4),
                                ConvertToString(x3 + roundOffset), ConvertToString(y4));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString((primitive.RightSide ? xc : x2)), ConvertToString(y4));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(x3), ConvertToString(y2));
                        }
                    }
                    if (primitive.RightSide)
                    {
                        if (primitive.BottomSide)
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString((primitive.LeftSide ? xc : x1)), ConvertToString(y3));
                            pageStream.WriteLine("{0} {1} l", ConvertToString(x4 - roundOffset), ConvertToString(y3));
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                                ConvertToString(x4 - roundOffset2), ConvertToString(y3),
                                ConvertToString(x4), ConvertToString(y3 + roundOffset2),
                                ConvertToString(x4), ConvertToString(y3 + roundOffset));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} m", ConvertToString(x4), ConvertToString(y1));
                        }
                        if (primitive.TopSide)
                        {
                            pageStream.WriteLine("{0} {1} l", ConvertToString(x4), ConvertToString(y4 - roundOffset));
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} c",
                                ConvertToString(x4), ConvertToString(y4 - roundOffset2),
                                ConvertToString(x4 - roundOffset2), ConvertToString(y4),
                                ConvertToString(x4 - roundOffset), ConvertToString(y4));
                            pageStream.WriteLine("{0} {1} l S", ConvertToString((primitive.LeftSide ? xc : x1)), ConvertToString(y4));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} l S", ConvertToString(x4), ConvertToString(y2));
                        }
                    }
                    if (primitive.TopSide && !primitive.LeftSide && !primitive.RightSide)
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString(x1), ConvertToString(y4));
                        pageStream.WriteLine("{0} {1} l S", ConvertToString(x2), ConvertToString(y4));
                    }
                    if (primitive.BottomSide && !primitive.LeftSide && !primitive.RightSide)
                    {
                        pageStream.WriteLine("{0} {1} m", ConvertToString(x1), ConvertToString(y3));
                        pageStream.WriteLine("{0} {1} l S", ConvertToString(x2), ConvertToString(y3));
                    }
                    #endregion
                }

                if (needPush)
                {
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
            }
        }

        private void RenderLinePrimitive(StiPdfData pp)
        {
            StiLinePrimitive primitive = pp.Component as StiLinePrimitive;
            StiHorizontalLinePrimitive horizontalLine = pp.Component as StiHorizontalLinePrimitive;
            StiVerticalLinePrimitive verticalLine = pp.Component as StiVerticalLinePrimitive;
            double x1 = pp.X;
            double y1 = pp.Top;
            double x2 = pp.X + (horizontalLine != null ? pp.Width : 0);
            double y2 = pp.Top - (verticalLine != null ? pp.Height : 0);

            #region Render line
            if (primitive.Style != StiPenStyle.None)
            {
                StiBorderSide border = new StiBorderSide(primitive.Color, primitive.Size, primitive.Style);

                bool needPush = (border.Style != StiPenStyle.None) && (border.Style != StiPenStyle.Solid);
                if (needPush)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                }

                double offset = StoreBorderSideData(border);

                if (horizontalLine != null)
                {
                    pageStream.WriteLine("{0} {1} m {2} {1} l S", ConvertToString(x1), ConvertToString(y1 - offset), ConvertToString(x2));
                    if (border.Style == StiPenStyle.Double)
                    {
                        pageStream.WriteLine("{0} {1} m {2} {1} l S", ConvertToString(x1), ConvertToString(y1 + offset), ConvertToString(x2));
                    }
                }
                if (verticalLine != null)
                {
                    pageStream.WriteLine("{0} {1} m {0} {2} l S", ConvertToString(x1 - offset), ConvertToString(y1), ConvertToString(y2));
                    if (border.Style == StiPenStyle.Double)
                    {
                        pageStream.WriteLine("{0} {1} m {0} {2} l S", ConvertToString(x1 + offset), ConvertToString(y1), ConvertToString(y2));
                    }
                }

                if (needPush)
                {
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
            }
            #endregion

            #region Render caps
            double lineSize = primitive.Size * hiToTwips * 0.9;
            if (lineSize < 0.32) lineSize = 0.32;
            pageStream.WriteLine("{0} w", ConvertToString(lineSize));

            if (horizontalLine != null)
            {
                RenderCap(horizontalLine.StartCap, x1, y1, StiAngle.Angle0, lineSize);
                RenderCap(horizontalLine.EndCap, x2, y2, StiAngle.Angle180, lineSize);
            }
            if (verticalLine != null)
            {
                RenderCap(verticalLine.StartCap, x1, y1, StiAngle.Angle270, lineSize);
                RenderCap(verticalLine.EndCap, x2, y2, StiAngle.Angle90, lineSize);
            }
            #endregion
        }

        private void RenderCap(StiCap cap, double x, double y, StiAngle angle, double lineSize)
        {
            if (cap.Style == StiCapStyle.None) return;

            SetStrokeColor(cap.Color);
            if (cap.Fill)
            {
                SetNonStrokeColor(cap.Color);
            }

            double vertOffset = cap.Height / 2f * hiToTwips;
            double horOffset = cap.Width / 2f * hiToTwips;

            switch (cap.Style)
            {
                case StiCapStyle.Arrow:
                    if (angle == StiAngle.Angle0)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l h",
                            ConvertToString(x - lineSize), ConvertToString(y),
                            ConvertToString(x + horOffset * 2), ConvertToString(y + vertOffset),
                            ConvertToString(x + horOffset * 2), ConvertToString(y - vertOffset));
                    }
                    if (angle == StiAngle.Angle90)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l h",
                            ConvertToString(x), ConvertToString(y - lineSize),
                            ConvertToString(x - horOffset), ConvertToString(y + vertOffset * 2),
                            ConvertToString(x + horOffset), ConvertToString(y + vertOffset * 2));
                    }
                    if (angle == StiAngle.Angle180)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l h",
                            ConvertToString(x + lineSize), ConvertToString(y),
                            ConvertToString(x - horOffset * 2), ConvertToString(y - vertOffset),
                            ConvertToString(x - horOffset * 2), ConvertToString(y + vertOffset));
                    }
                    if (angle == StiAngle.Angle270)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l h",
                            ConvertToString(x), ConvertToString(y + lineSize),
                            ConvertToString(x + horOffset), ConvertToString(y - vertOffset * 2),
                            ConvertToString(x - horOffset), ConvertToString(y - vertOffset * 2));
                    }
                    break;

                case StiCapStyle.Open:
                    if (angle == StiAngle.Angle0)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(x + horOffset * 2), ConvertToString(y + vertOffset),
                            ConvertToString(x - lineSize), ConvertToString(y),
                            ConvertToString(x + horOffset * 2), ConvertToString(y - vertOffset));
                    }
                    if (angle == StiAngle.Angle90)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(x - horOffset), ConvertToString(y + vertOffset * 2),
                            ConvertToString(x), ConvertToString(y - lineSize),
                            ConvertToString(x + horOffset), ConvertToString(y + vertOffset * 2));
                    }
                    if (angle == StiAngle.Angle180)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(x - horOffset * 2), ConvertToString(y - vertOffset),
                            ConvertToString(x + lineSize), ConvertToString(y),
                            ConvertToString(x - horOffset * 2), ConvertToString(y + vertOffset));
                    }
                    if (angle == StiAngle.Angle270)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l",
                            ConvertToString(x + horOffset), ConvertToString(y - vertOffset * 2),
                            ConvertToString(x), ConvertToString(y + lineSize),
                            ConvertToString(x - horOffset), ConvertToString(y - vertOffset * 2));
                    }
                    break;

                case StiCapStyle.Stealth:
                    if (angle == StiAngle.Angle0)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                            ConvertToString(x + horOffset * 2), ConvertToString(y + vertOffset),
                            ConvertToString(x - lineSize), ConvertToString(y),
                            ConvertToString(x + horOffset * 2), ConvertToString(y - vertOffset),
                            ConvertToString(x + horOffset * 1.3), ConvertToString(y));
                    }
                    if (angle == StiAngle.Angle90)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                            ConvertToString(x - horOffset), ConvertToString(y + vertOffset * 2),
                            ConvertToString(x), ConvertToString(y - lineSize),
                            ConvertToString(x + horOffset), ConvertToString(y + vertOffset * 2),
                            ConvertToString(x), ConvertToString(y + vertOffset * 1.3));
                    }
                    if (angle == StiAngle.Angle180)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                            ConvertToString(x - horOffset * 2), ConvertToString(y - vertOffset),
                            ConvertToString(x + lineSize), ConvertToString(y),
                            ConvertToString(x - horOffset * 2), ConvertToString(y + vertOffset),
                            ConvertToString(x - horOffset * 1.3), ConvertToString(y));
                    }
                    if (angle == StiAngle.Angle270)
                    {
                        pageStream.Write("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                            ConvertToString(x + horOffset), ConvertToString(y - vertOffset * 2),
                            ConvertToString(x), ConvertToString(y + lineSize),
                            ConvertToString(x - horOffset), ConvertToString(y - vertOffset * 2),
                            ConvertToString(x), ConvertToString(y - vertOffset * 1.3));
                    }
                    break;

                case StiCapStyle.Square:
                    pageStream.Write("{0} {1} {2} {3} re",
                        ConvertToString(x - horOffset), ConvertToString(y - vertOffset),
                        ConvertToString(horOffset * 2), ConvertToString(vertOffset * 2));
                    break;

                case StiCapStyle.Diamond:
                    pageStream.Write("{0} {1} m {2} {3} l {4} {5} l {6} {7} l h",
                        ConvertToString(x), ConvertToString(y + vertOffset),
                        ConvertToString(x + horOffset), ConvertToString(y),
                        ConvertToString(x), ConvertToString(y - vertOffset),
                        ConvertToString(x - horOffset), ConvertToString(y));
                    break;

                case StiCapStyle.Oval:
                    StiPdfGeomWriter pdfGeomWriter = new StiPdfGeomWriter(null, this);
                    pageStream.Write(pdfGeomWriter.GetEllipseString(x - horOffset, y - vertOffset, horOffset * 2, vertOffset * 2));
                    break;

            }

            if (cap.Fill && cap.Style != StiCapStyle.Open)
            {
                pageStream.WriteLine(" B");
            }
            else
            {
                pageStream.WriteLine(" S");
            }
        }
        #endregion

        #region RenderCheckbox
        private void RenderCheckbox(StiPdfData pp, bool checkBoxValue, bool storeShading = true)
        {
            StiCheckBox checkbox = pp.Component as StiCheckBox;
            if (checkbox != null)
            {
                #region Get shape path
                string shape = string.Empty;
                switch (checkBoxValue ? checkbox.CheckStyleForTrue : checkbox.CheckStyleForFalse)
                {
                    case StiCheckStyle.Cross:
                        shape = "62.568 52.024 m 62.018 52.166 60.405 52.537 58.984 52.848 c 55.336 53.645 49.313 58.685 44.741 64.767 c 40.839 69.958 l 45.919 71.092 l \r\n" +
                                "53.272 72.735 59.559 76.81 67.746 85.239 c 74.954 92.661 l 68.543 100.174 l 56.77 113.972 54.196 123.193 59.915 131.088 c 62.809 135.083 71.734 143.458 73.097 143.458 c \r\n" +
                                "73.509 143.458 74.16 141.77 74.546 139.708 c 75.526 134.457 81.002 122.942 85.482 116.708 c 87.557 113.82 89.473 111.458 89.74 111.458 c \r\n" +
                                "90.006 111.458 93.515 114.945 97.537 119.208 c 113.934 136.584 127.211 138.972 135.818 126.095 c 139.973 119.877 140.004 118.024 135.958 117.739 c \r\n" +
                                "130.11 117.329 118.795 109.205 110.443 99.42 c 105.812 93.994 l 110.69 89.679 l 117.241 83.884 129.589 77.786 136.531 76.919 c \r\n" +
                                "139.576 76.539 142.068 75.813 142.068 75.307 c 142.068 72.526 132.802 60.889 129.038 58.942 c 121.077 54.825 112.668 58.23 96.273 72.209 c \r\n" +
                                "91.287 76.46 l 84.2 67.488 l 80.303 62.554 75.379 57.368 73.259 55.965 c 69.353 53.38 64.393 51.552 62.568 52.024 c h";
                        break;

                    case StiCheckStyle.Check:
                        shape = "60.972 37.503 m 51.173 63.277 43.562 76.623 35.37 82.397 c 30.912 85.54 l 33.664 88.435 l 37.539 92.513 43.698 95.935 48.566 96.713 c \r\n" +
                                "52.426 97.33 53.024 97.093 57.102 93.334 c 59.763 90.882 63.368 85.726 66.269 80.223 c 68.899 75.234 71.18 71.153 71.337 71.153 c \r\n" +
                                "71.493 71.153 73.65 74.19 76.13 77.903 c 96.259 108.044 129.683 141.214 157.565 158.718 c 166.414 164.274 l 168.677 161.643 l \r\n" +
                                "170.941 159.012 l 163.178 152.717 l 139.859 133.81 108.017 94.486 89.043 61.164 c 82.362 49.432 81.87 48.851 73.952 43.345 c \r\n" +
                                "69.45 40.214 64.908 37.04 63.858 36.292 c 62.149 35.074 61.848 35.2 60.972 37.503 c h";
                        break;

                    case StiCheckStyle.CrossRectangle:
                        shape = "24.153 97.958 m 24.153 170.458 l 98.653 170.458 l 173.153 170.458 l 173.153 97.958 l 173.153 25.458 l 98.653 25.458 l 24.153 25.458 l 24.153 97.958 l h \r\n" +
                                "157.911 97.708 m 157.653 154.958 l 98.653 154.958 l 39.653 154.958 l 39.393 98.958 l 39.25 68.158 39.348 42.395 39.611 41.708 c \r\n" +
                                "39.987 40.727 52.819 40.458 99.129 40.458 c 158.169 40.458 l 157.911 97.708 l h \r\n" +
                                "67.337 54.521 m 65.513 54.912 62.41 56.378 60.442 57.778 c 57.123 60.14 48.153 70.186 48.153 71.541 c 48.153 71.87 50.57 72.68 53.525 73.342 c \r\n" +
                                "60.71 74.95 67.272 79.277 75.328 87.718 c 82.003 94.713 l 75.624 102.027 l 64.931 114.288 61.644 123.705 65.472 131.108 c \r\n" +
                                "67.054 134.168 78.562 145.458 80.098 145.458 c 80.556 145.458 81.245 143.77 81.63 141.708 c 82.611 136.457 88.086 124.942 92.567 118.708 c \r\n" +
                                "94.642 115.82 96.558 113.458 96.824 113.458 c 97.091 113.458 100.6 116.945 104.622 121.208 c 121.019 138.584 134.296 140.972 142.903 128.095 c \r\n" +
                                "147.058 121.877 147.089 120.024 143.043 119.739 c 137.213 119.33 124.806 110.39 117.127 101.066 c 113.226 96.33 113.155 96.112 114.876 94.198 c \r\n" +
                                "118.066 90.648 128.579 83.654 133.847 81.578 c 136.682 80.461 141.285 79.244 144.077 78.873 c 146.868 78.503 149.153 77.878 149.153 77.484 c \r\n" +
                                "149.153 75.37 140.777 64.275 137.501 62.048 c 129.107 56.344 120.869 59.278 103.358 74.209 c 98.372 78.46 l 91.285 69.488 l \r\n" +
                                "81.563 57.18 74.76 52.928 67.337 54.521 c h";
                        break;

                    case StiCheckStyle.CheckRectangle:
                        shape = "19.915 96.5 m 19.915 169 l 91.857 169 l 163.8 169 l 170.357 173.111 l 176.914 177.223 l 178.882 174.861 l 179.963 173.563 180.864 172.217 180.882 171.872 c \r\n" +
                                "180.9 171.526 178.44 169.334 175.415 167 c 169.915 162.757 l 169.915 93.378 l 169.915 24 l 94.915 24 l 19.915 24 l 19.915 96.5 l h \r\n" +
                                "153.915 92.622 m 153.915 141.962 153.786 146.137 152.294 144.899 c 149.513 142.592 136.609 126.998 127.965 115.5 c 117.473 101.544 104.486 81.963 98.451 71 c \r\n" +
                                "93.993 62.903 93.316 62.192 84.16 56 c 78.873 52.425 74.256 49.375 73.9 49.223 c 73.544 49.07 71.988 52.22 70.441 56.223 c \r\n" +
                                "68.895 60.225 65.183 68.635 62.192 74.911 c 57.906 83.903 55.515 87.56 50.914 92.161 c 47.703 95.372 44.364 98 43.495 98 c \r\n" +
                                "40.697 98 41.79 99.66 47.479 104.049 c 53.073 108.365 60.662 111.14 64.28 110.194 c 67.84 109.263 73.689 102.039 78.2 93.002 c \r\n" +
                                "82.663 84.062 l 87.207 90.895 l 95.518 103.394 108.214 118.311 125.807 136.25 c 143.215 154 l 89.565 154 l 35.915 154 l 35.915 96.5 l \r\n" +
                                "35.915 39 l 94.915 39 l 153.915 39 l 153.915 92.622 l h";
                        break;

                    case StiCheckStyle.CrossCircle:
                        shape = "83.347 26.864 m 61.07 31.95 42.193 47.128 32.202 67.986 c 23.401 86.36 23.68 110.034 32.919 128.958 c 41.882 147.315 60.868 162.86 80.847 168.201 c \r\n" +
                                "91.083 170.936 112.112 170.628 121.812 167.6 c 147.999 159.425 167.881 138.673 173.432 113.721 c 175.869 102.768 175 85.662 171.452 74.743 c \r\n" +
                                "164.795 54.256 145.804 35.792 124.126 28.729 c 117.735 26.647 113.94 26.133 102.847 25.845 c 93.814 25.61 87.363 25.947 83.347 26.864 c h \r\n" +
                                "112.414 41.542 m 129.545 44.672 146.131 57.503 153.827 73.579 c 168.725 104.698 152.719 141.239 119.425 152.119 c 112.712 154.313 109.49 154.763 100.347 154.781 c \r\n" +
                                "90.993 154.8 88.185 154.404 81.579 152.131 c 64.423 146.231 51.91 134.6 45.14 118.265 c 42.988 113.072 42.446 109.911 42.069 100.368 c \r\n" +
                                "41.551 87.229 42.811 81.166 48.181 70.958 c 52.288 63.15 63.613 51.864 71.549 47.67 c 83.611 41.295 98.688 39.034 112.414 41.542 c h \r\n" +
                                "69.097 66.583 m 66.21 69.342 63.847 71.942 63.847 72.361 c 63.847 72.78 69.506 78.671 76.422 85.451 c 88.996 97.78 l 76.198 110.607 l 63.4 123.434 l \r\n" +
                                "68.336 128.446 l 71.051 131.202 73.641 133.458 74.091 133.458 c 74.542 133.458 80.666 127.846 87.7 120.988 c 100.49 108.517 l 104.919 113.071 l \r\n" +
                                "107.354 115.575 113.31 121.259 118.154 125.701 c 126.961 133.777 l 132.308 128.496 l 137.656 123.215 l 124.694 110.658 l 111.733 98.1 l 124.866 84.939 l \r\n" +
                                "137.999 71.779 l 132.815 67.118 l 129.964 64.555 127.11 62.458 126.474 62.458 c 125.837 62.458 119.93 67.858 113.347 74.458 c \r\n" +
                                "106.765 81.058 100.96 86.458 100.449 86.458 c 99.938 86.458 93.856 80.857 86.933 74.013 c 74.347 61.567 l 69.097 66.583 l h";
                        break;

                    case StiCheckStyle.DotCircle:
                        shape = "81.653 29.406 m 59.375 34.493 40.499 49.67 30.507 70.529 c 21.706 88.903 21.985 112.576 31.224 131.5 c 40.187 149.857 59.173 165.402 79.153 170.743 c \r\n" +
                                "89.388 173.479 110.417 173.17 120.117 170.142 c 146.304 161.968 166.186 141.215 171.737 116.263 c 174.174 105.311 173.305 88.205 169.757 77.285 c \r\n" +
                                "163.1 56.798 144.109 38.334 122.431 31.271 c 116.04 29.189 112.245 28.675 101.153 28.387 c 92.119 28.152 85.668 28.49 81.653 29.406 c h \r\n" +
                                "111.653 44.504 m 132.341 48.848 149.671 64.959 155.751 85.5 c 158.113 93.481 158.113 107.519 155.751 115.5 c 150.089 134.629 134.635 149.703 114.653 155.588 c \r\n" +
                                "106.553 157.973 90.741 157.974 82.695 155.589 c 62.46 149.592 46.687 133.961 41.605 114.869 c 39.656 107.547 39.74 91.753 41.764 84.932 c \r\n" +
                                "50.494 55.507 80.736 38.013 111.653 44.504 c h \r\n" +
                                "90.005 77.33 m 76.55 82.362 69.825 98.176 75.898 110.5 c 78.035 114.836 83.045 119.856 87.653 122.277 c 93.231 125.208 104.066 125.204 109.705 122.27 c \r\n" +
                                "127.735 112.887 128.781 89.485 111.62 79.428 c 106.047 76.162 95.789 75.166 90.005 77.33 c h";
                        break;

                    case StiCheckStyle.DotRectangle:
                        shape = "23.847 98.805 m 23.847 171.305 l 98.347 171.305 l 172.847 171.305 l 172.847 98.805 l 172.847 26.305 l 98.347 26.305 l 23.847 26.305 l 23.847 98.805 l h \r\n" +
                                "157.847 98.813 m 157.847 156.321 l 98.597 156.063 l 39.347 155.805 l 39.089 98.555 l 38.831 41.305 l 98.339 41.305 l 157.847 41.305 l 157.847 98.813 l h \r\n" +
                                "63.527 64.959 m 63.153 65.333 62.847 80.638 62.847 98.972 c 62.847 132.305 l 98.361 132.305 l 133.874 132.305 l 133.611 98.555 l 133.347 64.805 l \r\n" +
                                "98.777 64.542 l 79.763 64.398 63.901 64.585 63.527 64.959 c h";
                        break;

                    case StiCheckStyle.NoneCircle:
                        shape = "83.5 29.406 m 61.222 34.493 42.346 49.67 32.355 70.529 c 23.554 88.903 23.832 112.576 33.071 131.5 c 42.034 149.857 61.02 165.402 81 170.743 c \r\n" +
                                "91.235 173.479 112.265 173.17 121.965 170.142 c 148.151 161.968 168.034 141.215 173.585 116.263 c 176.022 105.311 175.152 88.205 171.605 77.285 c \r\n" +
                                "164.948 56.798 145.957 38.334 124.278 31.271 c 117.887 29.189 114.092 28.675 103 28.387 c 93.966 28.152 87.515 28.49 83.5 29.406 c h \r\n" +
                                "113.5 44.504 m 134.189 48.848 151.519 64.959 157.598 85.5 c 159.961 93.481 159.961 107.519 157.598 115.5 c 151.937 134.629 136.483 149.703 116.5 155.588 c \r\n" +
                                "108.401 157.973 92.589 157.974 84.543 155.589 c 64.308 149.592 48.534 133.961 43.453 114.869 c 41.504 107.547 41.588 91.753 43.612 84.932 c \r\n" +
                                "52.342 55.507 82.583 38.013 113.5 44.504 c h";
                        break;

                    case StiCheckStyle.NoneRectangle:
                        shape = "24.153 97.958 m 24.153 170.458 l 98.653 170.458 l 173.153 170.458 l 173.153 97.958 l 173.153 25.458 l 98.653 25.458 l 24.153 25.458 l 24.153 97.958 l h \r\n" +
                                "157.911 97.708 m 157.653 154.958 l 98.653 154.958 l 39.653 154.958 l 39.393 98.958 l 39.25 68.158 39.348 42.395 39.611 41.708 c \r\n" +
                                "39.987 40.727 52.819 40.458 99.129 40.458 c 158.169 40.458 l 157.911 97.708 l h";
                        break;
                }
                #endregion

                pageStream.WriteLine("q");
                PushColorToStack();

                #region Set style
                //Fill color
                Color tempColor = StiBrush.ToColor(checkbox.TextBrush);
                if (tempColor.A != 0)
                {
                    SetNonStrokeColor(tempColor);
                }
                if (checkbox.TextBrush is StiGradientBrush || checkbox.TextBrush is StiGlareBrush)
                {
                    if (storeShading)   //only for editable, second shape use the same data
                    {
                        StoreShadingData2(pp.X, pp.Y, pp.Width, pp.Height, checkbox.TextBrush);
                    }
                    pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
                }
                if (checkbox.TextBrush is StiHatchBrush)
                {
                    StiHatchBrush hBrush = checkbox.TextBrush as StiHatchBrush;
                    pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(hBrush) + 1);
                }

                //stroke color
                SetStrokeColor(checkbox.ContourColor);

                pageStream.WriteLine("{0} w", ConvertToString(checkbox.Size));
                #endregion

                pageStream.WriteLine("1 0 0 1 {0} {1} cm", ConvertToString(pp.X + pp.Width / 2), ConvertToString(pp.Y + pp.Height / 2));
                double size = Math.Min(pp.Width, pp.Height);
                double scale = size / 200;
                pageStream.WriteLine("{0} 0 0 {0} 0 0 cm", ConvertToString(scale));
                pageStream.WriteLine("1 0 0 1 -98 -98 cm");

                pageStream.WriteLine(shape);
                pageStream.WriteLine("B");

                pageStream.WriteLine("Q");
                PopColorFromStack();
            }
        }

        private bool? GetCheckBoxValue(StiCheckBox checkbox)
        {
            bool? checkBoxValue = null;
            if ((checkbox != null) && (checkbox.CheckedValue != null))
            {
                checkBoxValue = false;
                string checkedValueStr = checkbox.CheckedValue.ToString().Trim().ToLower(CultureInfo.InvariantCulture);
                string[] strs = checkbox.Values.Split(new char[] { '/', ';', ',' });
                if (strs != null && strs.Length > 0)
                {
                    string firstValue = strs[0].Trim().ToLower(CultureInfo.InvariantCulture);
                    checkBoxValue = checkedValueStr == firstValue;
                }
            }
            return checkBoxValue;
        }
        #endregion

    }
}
