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

using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - QR Code.
    /// </summary>
	public partial class StiQRCodeBarCodeType : StiBarCodeTypeService
	{
        protected void DrawQRCode(object context, RectangleF rect, StiBarCode barCode, float zoom,
            StiQRCodeBodyShapeType bodyShape, StiQRCodeEyeFrameShapeType eyeFrameShape, StiQRCodeEyeBallShapeType eyeBallShape)
        {
            RectWindow = new RectangleF(0, 0, rect.Width, rect.Height);
            if ((barCode.Angle == StiAngle.Angle90) || (barCode.Angle == StiAngle.Angle270))
            {
                RectWindow = new RectangleF(0, 0, rect.Height, rect.Width);
            }

            int quietZone = 2;
            if (!barCode.ShowQuietZones) quietZone = 0;

            float fullZoomX = (Module / 10) * zoom;
            float fullZoomY = fullZoomX;
            if (barCode.AutoScale)
            {
                fullZoomX = (float)RectWindow.Width / (BarCodeData.MatrixWidth + quietZone * 2);
                fullZoomY = (float)RectWindow.Height / (BarCodeData.MatrixHeight * BarCodeData.MatrixRatioY + quietZone * 2);
                fullZoomX = Math.Min(fullZoomX, fullZoomY);
                fullZoomY = fullZoomX;
            }

            BarCodeData.MainWidth = (BarCodeData.MatrixWidth + quietZone * 2) * fullZoomX;
            BarCodeData.MainHeight = (BarCodeData.MatrixHeight * BarCodeData.MatrixRatioY + quietZone * 2) * fullZoomY;
            BarCodeData.SpaceLeft = quietZone * fullZoomX;
            BarCodeData.SpaceTop = quietZone * fullZoomY;

            TranslateRect(context, rect, barCode);

            List<List<PointF>> listPoints = new List<List<PointF>>();

            var backBrush = new StiSolidBrush(barCode.BackColor);
            BaseFillRectangle(context, backBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.MainHeight);

            StiBrush foreBrush = new StiSolidBrush(barCode.ForeColor);
            if (!(BodyBrush == null || BodyBrush is StiEmptyBrush))
            {
                foreBrush = BodyBrush;
            }

            var frames = DrawEyeFrames(context, eyeFrameShape, fullZoomX, foreBrush);
            if (EyeFrameBrush == null || EyeFrameBrush is StiEmptyBrush)
            {
                listPoints.AddRange(frames);
            }
            else
            {
                BaseFillPolygons(context, EyeFrameBrush, frames);
            }

            var balls = DrawEyeBalls(context, eyeBallShape, fullZoomX, foreBrush);
            if (EyeBallBrush == null || EyeBallBrush is StiEmptyBrush)
            {
                listPoints.AddRange(balls);
            }
            else
            {
                BaseFillPolygons(context, EyeBallBrush, balls);
            }

            DrawBodyShapes(context, bodyShape, fullZoomX, foreBrush, listPoints);

            if (listPoints.Count > 0)
                BaseFillPolygons(context, foreBrush, listPoints);

            var qrCode = this as StiQRCodeBarCodeType;
            if ((qrCode != null) && (qrCode.Image != null))
            {
                float imageWidth = qrCode.Image.Width * zoom * (float)qrCode.ImageMultipleFactor;
                float imageHeight = qrCode.Image.Height * zoom * (float)qrCode.ImageMultipleFactor;
                float x = BarCodeData.SpaceLeft + (BarCodeData.MatrixWidth * fullZoomX - imageWidth) / 2;
                float y = BarCodeData.SpaceTop + (BarCodeData.MatrixHeight * fullZoomY - imageHeight) / 2;

                BaseDrawImage(context, qrCode.Image, barCode.Report, x, y, imageWidth, imageHeight);
            }

            RollbackTransform(context);
        }

        #region Utils
        private List<PointF> AddArcPoints(float x, float y, float width, float height, float startAngle, float sweepAngle, int steps, List<PointF> path)
        {
            if (path == null) path = new List<PointF>();
            for (int i = 0; i <= steps; i++)
            {
                double angle = (startAngle + sweepAngle * ((float)i / steps)) * Math.PI / 180;
                double dx = x + width * (1 + Math.Cos(angle)) / 2;
                double dy = y + height * (1 + Math.Sin(angle)) / 2;
                path.Add(new PointF((float)dx, (float)dy));
            }
            return path;
        }
        #endregion

        #region
        private void DrawBodyShapes(object context, StiQRCodeBodyShapeType bodyShape, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            if (bodyShape == StiQRCodeBodyShapeType.ZebraHorizontal)
            {
                DrawBodyShapeCircleZebraHorizontal(context, foreBrush, step, listPoints);
            }
            else if (bodyShape == StiQRCodeBodyShapeType.ZebraVertical)
            {
                DrawBodyShapeCircleZebraVertical(context, foreBrush, step, listPoints);
            }
            else if (bodyShape == StiQRCodeBodyShapeType.ZebraCross1)
            {
                DrawBodyShapeCircleZebraCross1(context, foreBrush, step, listPoints);
            }
            else if (bodyShape == StiQRCodeBodyShapeType.ZebraCross2)
            {
                DrawBodyShapeCircleZebraCross2(context, foreBrush, step, listPoints);
            }
            else
            {
                int matrixW = BarCodeData.MatrixWidth;
                int matrixH = BarCodeData.MatrixHeight;
                for (int indexY = 0; indexY < matrixH; indexY++)
                {
                    for (int indexX = 0; indexX < matrixW; indexX++)
                    {
                        int offs = indexX + indexY * matrixW;
                        byte matrixValue = BarCodeData.MatrixGrid[offs];
                        if (matrixValue == 1)
                        {
                            float x = BarCodeData.SpaceLeft + indexX * step;
                            float y = BarCodeData.SpaceTop + indexY * step;

                            bool hasLeft = indexX > 0 && BarCodeData.MatrixGrid[offs - 1] == 1;
                            bool hasRight = indexX < matrixW - 1 && BarCodeData.MatrixGrid[offs + 1] == 1;
                            bool hasUp = indexY > 0 && BarCodeData.MatrixGrid[offs - matrixW] == 1;
                            bool hasDown = indexY < matrixH - 1 && BarCodeData.MatrixGrid[offs + matrixW] == 1;
                            float offsPL = hasLeft ? -step * 0.01f : 0;
                            float offsPR = hasRight ? step * 0.01f : 0;
                            float offsPU = hasUp ? -step * 0.01f : 0;
                            float offsPD = hasDown ? step * 0.01f : 0;

                            List<PointF> points = new List<PointF>();

                            switch (bodyShape)
                            {
                                case StiQRCodeBodyShapeType.Square:
                                    points.Add(new PointF(x + offsPL, y + offsPU));
                                    points.Add(new PointF(x + step + offsPR, y + offsPU));
                                    points.Add(new PointF(x + step + offsPL, y + step + offsPD));
                                    points.Add(new PointF(x + offsPL, y + step + offsPD));
                                    break;

                                case StiQRCodeBodyShapeType.Dot:
                                    float offsE = step * 0.2f;
                                    AddArcPoints(x + offsE, y + offsE, step - offsE * 2, step - offsE * 2, 0, 360, 12, points);
                                    break;

                                case StiQRCodeBodyShapeType.Circle:
                                    AddArcPoints(x, y, step, step, 0, 360, 16, points);
                                    break;

                                case StiQRCodeBodyShapeType.Diamond:
                                    float offsD = step * 0.5f;
                                    points.Add(new PointF(x, y + offsD));
                                    points.Add(new PointF(x + offsD, y));
                                    points.Add(new PointF(x + step, y + offsD));
                                    points.Add(new PointF(x + offsD, y + step));
                                    break;

                                case StiQRCodeBodyShapeType.Star:
                                    #region Star
                                    points.Add(new PointF(x, y + step * 0.395f));
                                    points.Add(new PointF(x + step * 0.045f, y + step * 0.34f));
                                    points.Add(new PointF(x + step * 0.31f, y + step * 0.28f));
                                    points.Add(new PointF(x + step * 0.43f, y + step * 0.01f));
                                    points.Add(new PointF(x + step * 0.485f, y));
                                    points.Add(new PointF(x + step * 0.53f, y + step * 0.025f));
                                    points.Add(new PointF(x + step * 0.66f, y + step * 0.265f));
                                    points.Add(new PointF(x + step * 0.92f, y + step * 0.32f));
                                    points.Add(new PointF(x + step, y + step * 0.385f));
                                    points.Add(new PointF(x + step * 0.97f, y + step * 0.45f));
                                    points.Add(new PointF(x + step * 0.755f, y + step * 0.59f));
                                    points.Add(new PointF(x + step * 0.83f, y + step * 0.84f));
                                    points.Add(new PointF(x + step * 0.82f, y + step * 0.925f));
                                    points.Add(new PointF(x + step * 0.765f, y + step * 0.945f));
                                    points.Add(new PointF(x + step * 0.485f, y + step * 0.765f));
                                    points.Add(new PointF(x + step * 0.28f, y + step * 0.94f));
                                    points.Add(new PointF(x + step * 0.22f, y + step * 0.945f));
                                    points.Add(new PointF(x + step * 0.18f, y + step * 0.895f));
                                    points.Add(new PointF(x + step * 0.23f, y + step * 0.595f));
                                    points.Add(new PointF(x + step * 0.025f, y + step * 0.45f));
                                    #endregion
                                    break;

                                case StiQRCodeBodyShapeType.RoundedSquare:
                                    #region Round
                                    float offsR = step * 0.35f * 2;
                                    if (hasLeft && hasRight || hasUp && hasDown)
                                    {
                                        points.Add(new PointF(x + offsPL, y + step / 2));
                                    }
                                    if (hasLeft || hasUp)
                                    {
                                        points.Add(new PointF(x + offsPL, y + offsPU));
                                    }
                                    else
                                    {
                                        AddArcPoints(x, y, offsR, offsR, 180, 90, 4, points);
                                    }
                                    if (hasRight || hasUp)
                                    {
                                        points.Add(new PointF(x + step + offsPR, y + offsPU));
                                    }
                                    else
                                    {
                                        AddArcPoints(x + step - offsR, y, offsR, offsR, 270, 90, 4, points);
                                    }
                                    if (hasRight || hasDown)
                                    {
                                        points.Add(new PointF(x + step + offsPR, y + step + offsPD));
                                    }
                                    else
                                    {
                                        AddArcPoints(x + step - offsR, y + step - offsR, offsR, offsR, 0, 90, 4, points);
                                    }
                                    if (hasLeft || hasDown)
                                    {
                                        points.Add(new PointF(x + offsPL, y + step + offsPD));
                                    }
                                    else
                                    {
                                        AddArcPoints(x, y + step - offsR, offsR, offsR, 90, 90, 4, points);
                                    }
                                    #endregion
                                    break;

                                case StiQRCodeBodyShapeType.Circular:
                                    #region Circular
                                    int vers = (hasUp ? 1 : 0) + (hasLeft ? 2 : 0) + (hasRight ? 4 : 0) + (hasDown ? 8 : 0);
                                    switch (vers)
                                    {
                                        case 0:
                                            AddArcPoints(x, y, step, step, 0, 360, 16, points);
                                            break;

                                        case 1:
                                            points.Add(new PointF(x, y + offsPU));
                                            points.Add(new PointF(x + step, y + offsPU));
                                            AddArcPoints(x, y, step, step, 0, 180, 8, points);
                                            break;
                                        case 2:
                                            points.Add(new PointF(x + offsPL, y + step));
                                            points.Add(new PointF(x + offsPL, y));
                                            AddArcPoints(x, y, step, step, 270, 180, 8, points);
                                            break;
                                        case 4:
                                            points.Add(new PointF(x + step + offsPL, y));
                                            points.Add(new PointF(x + step + offsPL, y + step));
                                            AddArcPoints(x, y, step, step, 90, 180, 8, points);
                                            break;
                                        case 8:
                                            points.Add(new PointF(x + step, y + step + offsPD));
                                            points.Add(new PointF(x, y + step + offsPD));
                                            AddArcPoints(x, y, step, step, 180, 180, 8, points);
                                            break;

                                        case 3:
                                            points.Add(new PointF(x + offsPL, y + offsPU));
                                            points.Add(new PointF(x + step, y + offsPU));
                                            AddArcPoints(x - step, y - step, step * 2, step * 2, 0, 90, 8, points);
                                            points.Add(new PointF(x + offsPL, y + step));
                                            break;
                                        case 5:
                                            points.Add(new PointF(x, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + step));
                                            AddArcPoints(x, y - step, step * 2, step * 2, 90, 90, 8, points);
                                            break;
                                        case 10:
                                            points.Add(new PointF(x + offsPL, y));
                                            AddArcPoints(x - step, y, step * 2, step * 2, 270, 90, 8, points);
                                            points.Add(new PointF(x + step, y + step + offsPD));
                                            points.Add(new PointF(x + offsPL, y + step + offsPD));
                                            break;
                                        case 12:
                                            points.Add(new PointF(x, y + step + offsPD));
                                            AddArcPoints(x, y, step * 2, step * 2, 180, 90, 8, points);
                                            points.Add(new PointF(x + step + offsPR, y));
                                            points.Add(new PointF(x + step + offsPR, y + step + offsPD));
                                            break;

                                        default:
                                            points.Add(new PointF(x + offsPL, y + step / 2));
                                            points.Add(new PointF(x + offsPL, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + step + offsPD));
                                            points.Add(new PointF(x + offsPL, y + step + offsPD));
                                            break;
                                    }
                                    #endregion
                                    break;

                                case StiQRCodeBodyShapeType.DockedDiamonds:
                                    #region PointedEdgeCut
                                    int vers2 = (hasUp ? 1 : 0) + (hasLeft ? 2 : 0) + (hasRight ? 4 : 0) + (hasDown ? 8 : 0);
                                    float offsPEC = step * 0.4f;
                                    switch (vers2)
                                    {
                                        case 0:
                                            points.Add(new PointF(x, y + step / 2));
                                            points.Add(new PointF(x + step / 2, y));
                                            points.Add(new PointF(x + step, y + step / 2));
                                            points.Add(new PointF(x + step / 2, y + step));
                                            break;

                                        case 1:
                                            points.Add(new PointF(x, y + offsPU));
                                            points.Add(new PointF(x + step, y + offsPU));
                                            points.Add(new PointF(x + step, y + step - offsPEC));
                                            points.Add(new PointF(x + step / 2, y + step));
                                            points.Add(new PointF(x, y + step - offsPEC));
                                            break;
                                        case 2:
                                            points.Add(new PointF(x + offsPL, y + step));
                                            points.Add(new PointF(x + offsPL, y));
                                            points.Add(new PointF(x + step - offsPEC, y));
                                            points.Add(new PointF(x + step, y + step / 2));
                                            points.Add(new PointF(x + step - offsPEC, y + step));
                                            break;
                                        case 4:
                                            points.Add(new PointF(x + step + offsPL, y));
                                            points.Add(new PointF(x + step + offsPL, y + step));
                                            points.Add(new PointF(x + offsPEC, y + step));
                                            points.Add(new PointF(x, y + step / 2));
                                            points.Add(new PointF(x + offsPEC, y));
                                            break;
                                        case 8:
                                            points.Add(new PointF(x + step, y + step + offsPD));
                                            points.Add(new PointF(x, y + step + offsPD));
                                            points.Add(new PointF(x, y + offsPEC));
                                            points.Add(new PointF(x + step / 2, y));
                                            points.Add(new PointF(x + step, y + offsPEC));
                                            break;

                                        case 3:
                                            points.Add(new PointF(x + offsPL, y + offsPU));
                                            points.Add(new PointF(x + step, y + offsPU));
                                            points.Add(new PointF(x + step, y + step - offsPEC));
                                            points.Add(new PointF(x + step - offsPEC, y + step));
                                            points.Add(new PointF(x + offsPL, y + step));
                                            break;
                                        case 5:
                                            points.Add(new PointF(x, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + step));
                                            points.Add(new PointF(x + offsPEC, y + step));
                                            points.Add(new PointF(x, y + step - offsPEC));
                                            break;
                                        case 10:
                                            points.Add(new PointF(x + offsPL, y));
                                            points.Add(new PointF(x + step - offsPEC, y));
                                            points.Add(new PointF(x + step, y + offsPEC));
                                            points.Add(new PointF(x + step, y + step + offsPD));
                                            points.Add(new PointF(x + offsPL, y + step + offsPD));
                                            break;
                                        case 12:
                                            points.Add(new PointF(x, y + step + offsPD));
                                            points.Add(new PointF(x, y + offsPEC));
                                            points.Add(new PointF(x + offsPEC, y));
                                            points.Add(new PointF(x + step + offsPR, y));
                                            points.Add(new PointF(x + step + offsPR, y + step + offsPD));
                                            break;

                                        default:
                                            points.Add(new PointF(x + offsPL, y + step / 2));
                                            points.Add(new PointF(x + offsPL, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + offsPU));
                                            points.Add(new PointF(x + step + offsPR, y + step + offsPD));
                                            points.Add(new PointF(x + offsPL, y + step + offsPD));
                                            break;
                                    }
                                    #endregion
                                    break;

                                default:
                                    break;
                            }

                            if (points.Count > 0)
                                listPoints.Add(points);
                        }
                    }
                }
            }
        }

        private void DrawBodyShapeCircleZebraHorizontal(object context, StiBrush foreBrush, float step, List<List<PointF>> listPoints)
        {
            float cf = 0.8f;
            for (int indexY = 0; indexY < BarCodeData.MatrixHeight; indexY++)
            {
                for (int indexX = 0; indexX < BarCodeData.MatrixWidth; indexX++)
                {
                    int offs = indexX + indexY * BarCodeData.MatrixWidth;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float x = BarCodeData.SpaceLeft + indexX * step;
                        float y = BarCodeData.SpaceTop + indexY * step + (1 - cf) / 2 * step;

                        int w = 0;
                        while ((indexX + 1 < BarCodeData.MatrixWidth) && (BarCodeData.MatrixGrid[offs + 1] == 1))
                        {
                            offs++;
                            indexX++;
                            w++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 90, 180, 8, path);
                        AddArcPoints(x + step * w, y, step * cf, step * cf, 270, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }
        }

        private void DrawBodyShapeCircleZebraVertical(object context, StiBrush foreBrush, float step, List<List<PointF>> listPoints)
        {
            float cf = 0.8f;
            int maxw = BarCodeData.MatrixWidth;
            for (int indexX = 0; indexX < maxw; indexX++)
            {
                for (int indexY = 0; indexY < BarCodeData.MatrixHeight; indexY++)
                {
                    int offs = indexX + indexY * maxw;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float x = BarCodeData.SpaceLeft + indexX * step + (1 - cf) / 2 * step;
                        float y = BarCodeData.SpaceTop + indexY * step;

                        int h = 0;
                        while ((indexY + 1 < BarCodeData.MatrixHeight) && (BarCodeData.MatrixGrid[offs + maxw] == 1))
                        {
                            offs += BarCodeData.MatrixWidth;
                            indexY++;
                            h++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 180, 180, 8, path);
                        AddArcPoints(x, y + step * h, step * cf, step * cf, 0, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }
        }

        private void DrawBodyShapeCircleZebraCross1(object context, StiBrush foreBrush, float step, List<List<PointF>> listPoints)
        {
            float cf = 0.8f;
            float cf2 = (1 - cf) / 2;
            int center = BarCodeData.MatrixWidth / 2;

            for (int indexY = 0; indexY < BarCodeData.MatrixHeight; indexY++)
            {
                float y = BarCodeData.SpaceTop + (indexY + cf2) * step;
                int maxx = indexY <= center ? indexY : BarCodeData.MatrixHeight - indexY - 1;
                for (int indexX = 0; indexX <= maxx; indexX++)
                {
                    int offs = indexX + indexY * BarCodeData.MatrixWidth;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float x = BarCodeData.SpaceLeft + (indexX + cf2) * step;

                        int w = 0;
                        while ((indexX + 1 <= maxx) && (BarCodeData.MatrixGrid[offs + 1] == 1))
                        {
                            offs++;
                            indexX++;
                            w++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 90, 180, 8, path);
                        AddArcPoints(x + step * w, y, step * cf, step * cf, 270, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
                int minx = indexY <= center ? BarCodeData.MatrixHeight - indexY - 1 : indexY;
                for (int indexX = minx; indexX < BarCodeData.MatrixWidth; indexX++)
                {
                    int offs = indexX + indexY * BarCodeData.MatrixWidth;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float x = BarCodeData.SpaceLeft + (indexX + cf2) * step;

                        int w = 0;
                        while ((indexX + 1 < BarCodeData.MatrixWidth) && (BarCodeData.MatrixGrid[offs + 1] == 1))
                        {
                            offs++;
                            indexX++;
                            w++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 90, 180, 8, path);
                        AddArcPoints(x + step * w, y, step * cf, step * cf, 270, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }

            int maxw = BarCodeData.MatrixWidth;
            for (int indexX = 7; indexX < maxw; indexX++)
            {
                float x = BarCodeData.SpaceLeft + (indexX + cf2) * step;
                int maxy = indexX <= center ? indexX : BarCodeData.MatrixHeight - indexX - 1;
                for (int indexY = 0; indexY < maxy; indexY++)
                {
                    int offs = indexX + indexY * maxw;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float y = BarCodeData.SpaceTop + (indexY + cf2) * step;

                        int h = 0;
                        while ((indexY + 1 < maxy) && (BarCodeData.MatrixGrid[offs + maxw] == 1))
                        {
                            offs += BarCodeData.MatrixWidth;
                            indexY++;
                            h++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 180, 180, 8, path);
                        AddArcPoints(x, y + step * h, step * cf, step * cf, 0, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
                int miny = indexX <= center ? BarCodeData.MatrixWidth - indexX - 1 : indexX;
                for (int indexY = miny; indexY < BarCodeData.MatrixHeight; indexY++)
                {
                    int offs = indexX + indexY * maxw;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float y = BarCodeData.SpaceTop + (indexY + cf2) * step;

                        int h = 0;
                        while ((indexY + 1 < BarCodeData.MatrixHeight) && (BarCodeData.MatrixGrid[offs + maxw] == 1))
                        {
                            offs += BarCodeData.MatrixWidth;
                            indexY++;
                            h++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 180, 180, 8, path);
                        AddArcPoints(x, y + step * h, step * cf, step * cf, 0, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }
        }

        private void DrawBodyShapeCircleZebraCross2(object context, StiBrush foreBrush, float step, List<List<PointF>> listPoints)
        {
            float cf = 0.8f;
            float cf2 = (1 - cf) / 2;
            int center = BarCodeData.MatrixWidth / 2;

            for (int indexY = 0; indexY < BarCodeData.MatrixHeight; indexY++)
            {
                float y = BarCodeData.SpaceTop + (indexY + cf2) * step;
                int minx = indexY <= center ? indexY : BarCodeData.MatrixHeight - indexY - 1;
                int maxx = indexY <= center ? BarCodeData.MatrixHeight - indexY - 1 : indexY;
                for (int indexX = minx; indexX <= maxx; indexX++)
                {
                    int offs = indexX + indexY * BarCodeData.MatrixWidth;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float x = BarCodeData.SpaceLeft + (indexX + cf2) * step;

                        int w = 0;
                        while ((indexX + 1 <= maxx) && (BarCodeData.MatrixGrid[offs + 1] == 1))
                        {
                            offs++;
                            indexX++;
                            w++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 90, 180, 8, path);
                        AddArcPoints(x + step * w, y, step * cf, step * cf, 270, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }

            int maxw = BarCodeData.MatrixWidth;
            for (int indexX = 0; indexX < maxw; indexX++)
            {
                float x = BarCodeData.SpaceLeft + (indexX + cf2) * step;
                int miny = indexX <= center ? indexX : BarCodeData.MatrixHeight - indexX - 1;
                int maxy = indexX <= center ? BarCodeData.MatrixWidth - indexX - 1 : indexX;
                for (int indexY = miny; indexY < maxy; indexY++)
                {
                    int offs = indexX + indexY * maxw;
                    byte matrixValue = BarCodeData.MatrixGrid[offs];
                    if (matrixValue == 1)
                    {
                        float y = BarCodeData.SpaceTop + (indexY + cf2) * step;

                        int h = 0;
                        while ((indexY + 1 < maxy) && (BarCodeData.MatrixGrid[offs + maxw] == 1))
                        {
                            offs += BarCodeData.MatrixWidth;
                            indexY++;
                            h++;
                        }

                        List<PointF> path = new List<PointF>();
                        AddArcPoints(x, y, step * cf, step * cf, 180, 180, 8, path);
                        AddArcPoints(x, y + step * h, step * cf, step * cf, 0, 180, 8, path);
                        listPoints.Add(path);
                    }
                }
            }
        }
        #endregion

        #region DrawEyeFrames
        private List<List<PointF>> DrawEyeFrames(object context, StiQRCodeEyeFrameShapeType eyeFrameShape, float step, StiBrush foreBrush)
        {
            List<List<PointF>> listPoints = new List<List<PointF>>();

            float x1 = BarCodeData.SpaceLeft;
            float y1 = BarCodeData.SpaceTop;
            float x2 = BarCodeData.SpaceLeft + (BarCodeData.MatrixWidth - 7) * step;
            float y2 = BarCodeData.SpaceTop + (BarCodeData.MatrixHeight - 7) * step;

            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Square)
            {
                DrawEyeFrameSquare(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeFrameSquare(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeFrameSquare(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Dots)
            {
                DrawEyeFrameDots(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeFrameDots(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeFrameDots(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Circle)
            {
                DrawEyeFrameCircle(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeFrameCircle(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeFrameCircle(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Round)
            {
                DrawEyeFrameRound(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeFrameRound(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeFrameRound(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Round1)
            {
                DrawEyeFrameRound1(context, x1, y1, step, foreBrush, 1, listPoints);
                DrawEyeFrameRound1(context, x1, y2, step, foreBrush, 3, listPoints);
                DrawEyeFrameRound1(context, x2, y1, step, foreBrush, 2, listPoints);
            }
            if (eyeFrameShape == StiQRCodeEyeFrameShapeType.Round3)
            {
                DrawEyeFrameRound3(context, x1, y1, step, foreBrush, 1, listPoints);
                DrawEyeFrameRound3(context, x1, y2, step, foreBrush, 3, listPoints);
                DrawEyeFrameRound3(context, x2, y1, step, foreBrush, 2, listPoints);
            }

            return listPoints;
        }

        private void DrawEyeFrameSquare(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>
            {
                new PointF(x, y),
                new PointF(x + step * 7, y),
                new PointF(x + step * 7, y + step * 7),
                new PointF(x, y + step * 7),
                new PointF(x, y + step * 0.99f),
                new PointF(x + step, y + step * 0.99f),
                new PointF(x + step, y + step * 6),
                new PointF(x + step * 6, y + step * 6),
                new PointF(x + step * 6, y + step),
                new PointF(x, y + step)
            };
            listPoints.Add(path);
        }

        private void DrawEyeFrameDots(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            for (int index = 0; index < 7; index++)
            {
                listPoints.Add(AddArcPoints(x + index * step, y, step, step, 0, 360, 16, null));
                listPoints.Add(AddArcPoints(x + index * step, y + 6 * step, step, step, 0, 360, 16, null));
            }
            for (int index = 1; index < 6; index++)
            {
                listPoints.Add(AddArcPoints(x, y + index * step, step, step, 0, 360, 16, null));
                listPoints.Add(AddArcPoints(x + 6 * step, y + index * step, step, step, 0, 360, 16, null));
            }
        }

        private void DrawEyeFrameCircle(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();
            AddArcPoints(x, y, step * 7, step * 7, 0, 360, 32, path);
            AddArcPoints(x + step, y + step, step * 5, step * 5, 0, -360, 32, path);
            listPoints.Add(path);
        }

        private void DrawEyeFrameRound(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf1 = 3f;
            float cf2 = 5f;

            AddArcPoints(x, y, step * cf2, step * cf2, 180, 90, 8, path);
            AddArcPoints(x + step * (7 - cf2), y, step * cf2, step * cf2, 270, 90, 8, path);
            AddArcPoints(x + step * (7 - cf2), y + step * (7 - cf2), step * cf2, step * cf2, 0, 90, 8, path);
            AddArcPoints(x, y + step * (7 - cf2), step * cf2, step * cf2, 90, 90, 8, path);
            path.Add(new PointF(x, y + step * cf2 / 2));
            path.Add(new PointF(x + step, y + step * cf2 / 2));
            AddArcPoints(x + step, y + step * (6 - cf1), step * cf1, step * cf1, 180, -90, 6, path);
            AddArcPoints(x + step * (6 - cf1), y + step * (6 - cf1), step * cf1, step * cf1, 90, -90, 6, path);
            AddArcPoints(x + step * (6 - cf1), y + step, step * cf1, step * cf1, 0, -90, 6, path);
            AddArcPoints(x + step, y + step, step * cf1, step * cf1, 270, -90, 6, path);

            listPoints.Add(path);
        }

        private void DrawEyeFrameRound1(object context, float x, float y, float step, StiBrush foreBrush, int corner, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf1 = 3f;
            float cf2 = 5f;

            if (corner == 1)
            {
                AddArcPoints(x, y, step * cf2, step * cf2, 180, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x, y + step * cf2 / 2));
                path.Add(new PointF(x, y));
            }
            if (corner == 2)
            {
                AddArcPoints(x + step * (7 - cf2), y, step * cf2, step * cf2, 270, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x + step * 7, y));
            }
            path.Add(new PointF(x + step * 7, y + step * 7));
            if (corner == 3)
            {
                AddArcPoints(x, y + step * (7 - cf2), step * cf2, step * cf2, 90, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x, y + step * 7));
            }
            path.Add(new PointF(x, y + step * cf2 / 2));
            path.Add(new PointF(x + step, y + step * cf2 / 2));
            if (corner == 3)
            {
                AddArcPoints(x + step, y + step * (6 - cf1), step * cf1, step * cf1, 180, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step, y + step * 6));
            }
            path.Add(new PointF(x + step * 6, y + step * 6));
            if (corner == 2)
            {
                AddArcPoints(x + step * (6 - cf1), y + step, step * cf1, step * cf1, 0, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 6, y + step));
            }
            if (corner == 1)
            {
                AddArcPoints(x + step, y + step, step * cf1, step * cf1, 270, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step, y + step));
                path.Add(new PointF(x + step, y + step * cf2 / 2));
            }

            listPoints.Add(path);
        }

        private void DrawEyeFrameRound3(object context, float x, float y, float step, StiBrush foreBrush, int corner, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf1 = 3f;
            float cf2 = 5f;

            AddArcPoints(x, y, step * cf2, step * cf2, 180, 90, 8, path);
            if (corner != 3)
            {
                AddArcPoints(x + step * (7 - cf2), y, step * cf2, step * cf2, 270, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x + step * 7, y));
            }
            if (corner != 1)
            {
                AddArcPoints(x + step * (7 - cf2), y + step * (7 - cf2), step * cf2, step * cf2, 0, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x + step * 7, y + step * 7));
            }
            if (corner != 2)
            {
                AddArcPoints(x, y + step * (7 - cf2), step * cf2, step * cf2, 90, 90, 8, path);
            }
            else
            {
                path.Add(new PointF(x, y + step * 7));
            }
            path.Add(new PointF(x, y + step * cf2 / 2));
            path.Add(new PointF(x + step, y + step * cf2 / 2));
            if (corner != 2)
            {
                AddArcPoints(x + step, y + step * (6 - cf1), step * cf1, step * cf1, 180, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step, y + step * 6));
            }
            if (corner != 1)
            {
                AddArcPoints(x + step * (6 - cf1), y + step * (6 - cf1), step * cf1, step * cf1, 90, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 6, y + step * 6));
            }
            if (corner != 3)
            {
                AddArcPoints(x + step * (6 - cf1), y + step, step * cf1, step * cf1, 0, -90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 6, y + step));
            }
            AddArcPoints(x + step, y + step, step * cf1, step * cf1, 270, -90, 6, path);

            listPoints.Add(path);
        }
        #endregion

        #region DrawEyeBalls
        private List<List<PointF>> DrawEyeBalls(object context, StiQRCodeEyeBallShapeType eyeBallShape, float step, StiBrush foreBrush)
        {
            List<List<PointF>> listPoints = new List<List<PointF>>();

            float x1 = BarCodeData.SpaceLeft + step * 2;
            float y1 = BarCodeData.SpaceTop + step * 2;
            float x2 = BarCodeData.SpaceLeft + (BarCodeData.MatrixWidth - 5) * step;
            float y2 = BarCodeData.SpaceTop + (BarCodeData.MatrixHeight - 5) * step;

            if (eyeBallShape == StiQRCodeEyeBallShapeType.Square)
            {
                DrawEyeBallSquare(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallSquare(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallSquare(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Dots)
            {
                DrawEyeBallDots(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallDots(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallDots(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Circle)
            {
                DrawEyeBallCircle(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallCircle(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallCircle(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Round)
            {
                DrawEyeBallRound(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallRound(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallRound(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Round1)
            {
                DrawEyeBallRound1(context, x1, y1, step, foreBrush, 1, listPoints);
                DrawEyeBallRound1(context, x1, y2, step, foreBrush, 3, listPoints);
                DrawEyeBallRound1(context, x2, y1, step, foreBrush, 2, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Round3)
            {
                DrawEyeBallRound3(context, x1, y1, step, foreBrush, 1, listPoints);
                DrawEyeBallRound3(context, x1, y2, step, foreBrush, 3, listPoints);
                DrawEyeBallRound3(context, x2, y1, step, foreBrush, 2, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.Star)
            {
                DrawEyeBallStar(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallStar(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallStar(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.ZebraHorizontal)
            {
                DrawEyeBallCircleZebraHorizontal(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallCircleZebraHorizontal(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallCircleZebraHorizontal(context, x2, y1, step, foreBrush, listPoints);
            }
            if (eyeBallShape == StiQRCodeEyeBallShapeType.ZebraVertical)
            {
                DrawEyeBallCircleZebraVertical(context, x1, y1, step, foreBrush, listPoints);
                DrawEyeBallCircleZebraVertical(context, x1, y2, step, foreBrush, listPoints);
                DrawEyeBallCircleZebraVertical(context, x2, y1, step, foreBrush, listPoints);
            }

            return listPoints;
        }

        private void DrawEyeBallSquare(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>
            {
                new PointF(x, y),
                new PointF(x + step * 3, y),
                new PointF(x + step * 3, y + step * 3),
                new PointF(x, y + step * 3)
            };
            listPoints.Add(path);
        }

        private void DrawEyeBallDots(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            for (int index = 0; index < 3; index++)
            {
                listPoints.Add(AddArcPoints(x, y + index * step, step, step, 0, 360, 16, null));
                listPoints.Add(AddArcPoints(x + step, y + index * step, step, step, 0, 360, 16, null));
                listPoints.Add(AddArcPoints(x + step * 2, y + index * step, step, step, 0, 360, 16, null));
            }
        }

        private void DrawEyeBallCircle(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            listPoints.Add(AddArcPoints(x, y, step * 3, step * 3, 0, 360, 32, null));
        }

        private void DrawEyeBallRound(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf = 1.4f;

            AddArcPoints(x, y, step * cf, step * cf, 180, 90, 6, path);
            AddArcPoints(x + step * (3 - cf), y, step * cf, step * cf, 270, 90, 6, path);
            AddArcPoints(x + step * (3 - cf), y + step * (3 - cf), step * cf, step * cf, 0, 90, 6, path);
            AddArcPoints(x, y + step * (3 - cf), step * cf, step * cf, 90, 90, 6, path);

            listPoints.Add(path);
        }

        private void DrawEyeBallRound1(object context, float x, float y, float step, StiBrush foreBrush, int corner, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf = 1.4f;

            if (corner == 1)
            {
                AddArcPoints(x, y, step * cf, step * cf, 180, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x, y));
            }
            if (corner == 2)
            {
                AddArcPoints(x + step * (3 - cf), y, step * cf, step * cf, 270, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 3, y));
            }
            path.Add(new PointF(x + step * 3, y + step * 3));
            if (corner == 3)
            {
                AddArcPoints(x, y + step * (3 - cf), step * cf, step * cf, 90, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x, y + step * 3));
            }

            listPoints.Add(path);
        }

        private void DrawEyeBallRound3(object context, float x, float y, float step, StiBrush foreBrush, int corner, List<List<PointF>> listPoints)
        {
            List<PointF> path = new List<PointF>();

            float cf = 1.4f;

            AddArcPoints(x, y, step * cf, step * cf, 180, 90, 6, path);
            if (corner != 3)
            {
                AddArcPoints(x + step * (3 - cf), y, step * cf, step * cf, 270, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 3, y));
            }
            if (corner != 1)
            {
                AddArcPoints(x + step * (3 - cf), y + step * (3 - cf), step * cf, step * cf, 0, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x + step * 3, y + step * 3));
            }
            if (corner != 2)
            {
                AddArcPoints(x, y + step * (3 - cf), step * cf, step * cf, 90, 90, 6, path);
            }
            else
            {
                path.Add(new PointF(x, y + step * 3));
            }

            listPoints.Add(path);
        }

        private void DrawEyeBallStar(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            float step3 = step * 3.5f;
            x -= step / 4;
            y -= step / 4;

            List<PointF> path = new List<PointF>();

            path.Add(new PointF(x, y + step3 * 0.395f));
            path.Add(new PointF(x + step3 * 0.015f, y + step3 * 0.36f));
            path.Add(new PointF(x + step3 * 0.075f, y + step3 * 0.325f));
            path.Add(new PointF(x + step3 * 0.31f, y + step3 * 0.28f));
            path.Add(new PointF(x + step3 * 0.415f, y + step3 * 0.05f));
            path.Add(new PointF(x + step3 * 0.445f, y + step3 * 0.015f));
            path.Add(new PointF(x + step3 * 0.485f, y));
            path.Add(new PointF(x + step3 * 0.515f, y + step3 * 0.01f));
            path.Add(new PointF(x + step3 * 0.55f, y + step3 * 0.045f));
            path.Add(new PointF(x + step3 * 0.66f, y + step3 * 0.265f));
            path.Add(new PointF(x + step3 * 0.885f, y + step3 * 0.3f));
            path.Add(new PointF(x + step3 * 0.965f, y + step3 * 0.335f));
            path.Add(new PointF(x + step3, y + step3 * 0.385f));
            path.Add(new PointF(x + step3 * 0.99f, y + step3 * 0.43f));
            path.Add(new PointF(x + step3 * 0.95f, y + step3 * 0.475f));
            path.Add(new PointF(x + step3 * 0.755f, y + step3 * 0.59f));
            path.Add(new PointF(x + step3 * 0.83f, y + step3 * 0.815f));
            path.Add(new PointF(x + step3 * 0.835f, y + step3 * 0.880f));
            path.Add(new PointF(x + step3 * 0.82f, y + step3 * 0.925f));
            path.Add(new PointF(x + step3 * 0.785f, y + step3 * 0.945f));
            path.Add(new PointF(x + step3 * 0.745f, y + step3 * 0.94f));
            path.Add(new PointF(x + step3 * 0.485f, y + step3 * 0.765f));
            path.Add(new PointF(x + step3 * 0.305f, y + step3 * 0.930f));
            path.Add(new PointF(x + step3 * 0.26f, y + step3 * 0.95f));
            path.Add(new PointF(x + step3 * 0.22f, y + step3 * 0.945f));
            path.Add(new PointF(x + step3 * 0.19f, y + step3 * 0.915f));
            path.Add(new PointF(x + step3 * 0.18f, y + step3 * 0.87f));
            path.Add(new PointF(x + step3 * 0.23f, y + step3 * 0.595f));
            path.Add(new PointF(x + step3 * 0.04f, y + step3 * 0.465f));
            path.Add(new PointF(x + step3 * 0.01f, y + step3 * 0.435f));

            listPoints.Add(path);
        }

        private void DrawEyeBallCircleZebraHorizontal(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            float cf = 0.9f;

            List<PointF> path = new List<PointF>();
            AddArcPoints(x, y + step * (1 - cf) / 2, step * cf, step * cf, 90, 180, 8, path);
            AddArcPoints(x + step * (3 - cf), y + step * (1 - cf) / 2, step * cf, step * cf, 270, 180, 8, path);
            listPoints.Add(path);

            y += step;
            path = new List<PointF>();
            AddArcPoints(x, y + step * (1 - cf) / 2, step * cf, step * cf, 90, 180, 8, path);
            AddArcPoints(x + step * (3 - cf), y + step * (1 - cf) / 2, step * cf, step * cf, 270, 180, 8, path);
            listPoints.Add(path);

            y += step;
            path = new List<PointF>();
            AddArcPoints(x, y + step * (1 - cf) / 2, step * cf, step * cf, 90, 180, 8, path);
            AddArcPoints(x + step * (3 - cf), y + step * (1 - cf) / 2, step * cf, step * cf, 270, 180, 8, path);
            listPoints.Add(path);
        }

        private void DrawEyeBallCircleZebraVertical(object context, float x, float y, float step, StiBrush foreBrush, List<List<PointF>> listPoints)
        {
            float cf = 0.9f;
            List<PointF> path = new List<PointF>();
            AddArcPoints(x + step * (1 - cf) / 2, y, step * cf, step * cf, 180, 180, 8, path);
            AddArcPoints(x + step * (1 - cf) / 2, y + step * (3 - cf), step * cf, step * cf, 0, 180, 8, path);
            listPoints.Add(path);

            x += step;
            path = new List<PointF>();
            AddArcPoints(x + step * (1 - cf) / 2, y, step * cf, step * cf, 180, 180, 8, path);
            AddArcPoints(x + step * (1 - cf) / 2, y + step * (3 - cf), step * cf, step * cf, 0, 180, 8, path);
            listPoints.Add(path);

            x += step;
            path = new List<PointF>();
            AddArcPoints(x + step * (1 - cf) / 2, y, step * cf, step * cf, 180, 180, 8, path);
            AddArcPoints(x + step * (1 - cf) / 2, y + step * (3 - cf), step * cf, step * cf, 0, 180, 8, path);
            listPoints.Add(path);
        }
        #endregion
    }
}