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

using System.Reflection;
using System;
using Stimulsoft.Base.Drawing;
using System.Collections;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Web
{
    internal class StiScalingImagesHelper
    {
        #region Consts
        private const string ScaleX1_25 = "_x1_25";
        private const string ScaleX1_5 = "_x1_5";
        private const string ScaleX1_75 = "_x1_75";
        private const string ScaleX2 = "_x2";
        private const string ScaleX2_25 = "_x2_25";
        private const string ScaleX2_5 = "_x2_5";
        private const string ScaleX3 = "_x3";
        private const string ScaleX3_5 = "_x3_5";
        private const string ScaleX4 = "_x4";
        private const string ScaleX4_5 = "_x4_5";
        private const string ScaleX5 = "_x5";
        private const string ScaleX6 = "_x6";
        private const string ScaleX7 = "_x7";
        private const string ScaleX8 = "_x8";
        #endregion

        #region Methods
        public static Image GetIfExistOrResizeImage(Assembly assembly, string imagesPath, string imageName, string imageExt, double sourceScale, double needScale)
        {
            var pathToSourceImage = $"{imagesPath}{imageName}{GetScaleSuffix(sourceScale)}.{imageExt}";
            var pathToDefaultImage = $"{imagesPath}{imageName}.{imageExt}";

            if (StiImageUtils.ExistsImage(assembly, pathToSourceImage))
            {
                var image = StiImageUtils.GetImage(assembly, pathToSourceImage, false);
                var imageScaleFactor = 1;

                if (needScale / sourceScale >= 2)
                {
                    imageScaleFactor = (int)(needScale / sourceScale);
                    sourceScale *= imageScaleFactor;
                }

                var imageWidth = image.Width * imageScaleFactor;
                var imageHeight = image.Height * imageScaleFactor;
                var canvasWidth = (needScale - sourceScale) * (imageWidth / sourceScale) + imageWidth;
                var canvasHeight = (needScale - sourceScale) * (imageHeight / sourceScale) + imageHeight;

                if (canvasWidth / (int)canvasWidth != 1 || canvasHeight / (int)canvasHeight != 1)
                {
                    return StiImageUtils.GetImage(assembly, pathToDefaultImage, false); // TO DO
                }

                return StiImageUtils.ResizeImage(image, (int)Math.Ceiling(canvasWidth), (int)Math.Ceiling(canvasHeight), imageWidth, imageHeight);
            }
            else
            {
                var image = StiImageUtils.GetImage(assembly, pathToDefaultImage, false);
                return StiImageUtils.ResizeImage(image, (int)Math.Ceiling(image.Width * needScale), (int)Math.Ceiling(image.Height * needScale));
            }
        }

        public static Image GetScalingImage(Assembly assembly, string imagesPath, string imageName, string imageExt, double scaleFactor)
        {
            var sourceScale = 1d;

            switch (scaleFactor)
            {
                case 1.25:
                    sourceScale = 1;
                    break;

                case 1.5:
                    sourceScale = 1.5;
                    break;

                case 1.75:
                    sourceScale = 1.5;
                    break;

                case 2:
                    sourceScale = 2;
                    break;

                case 2.25:
                    sourceScale = 2;
                    break;

                case 2.5:
                    sourceScale = 2;
                    break;

                case 3:
                    sourceScale = 1.5;
                    break;

                case 3.5:
                    sourceScale = 1.5;
                    break;

                case 4:
                    sourceScale = 2;
                    break;
            }

            return GetIfExistOrResizeImage(assembly, imagesPath, imageName, imageExt, sourceScale, scaleFactor);
        }

        public static string GetScaleSuffix(double scaleFactor)
        {
            if (scaleFactor == 1.25) return ScaleX1_25;
            else if (scaleFactor == 1.5) return ScaleX1_5;
            else if (scaleFactor == 1.75) return ScaleX1_75;
            else if (scaleFactor == 2) return ScaleX2;
            else if (scaleFactor == 2.25) return ScaleX2_25;
            else if (scaleFactor == 2.5) return ScaleX2_5;
            else if (scaleFactor == 3) return ScaleX3;
            else if (scaleFactor == 3.5) return ScaleX3_5;
            else if (scaleFactor == 4) return ScaleX4;
            else if (scaleFactor == 4.5) return ScaleX4_5;
            else if (scaleFactor == 5) return ScaleX5;
            else if (scaleFactor == 6) return ScaleX6;
            else if (scaleFactor == 7) return ScaleX7;
            else if (scaleFactor == 8) return ScaleX8;
            else return string.Empty;
        }

        public static double GetScaleFactorFromName(string name)
        {
            if (name.Contains($"{ScaleX1_25}.")) return 1.25;
            else if (name.Contains($"{ScaleX1_5}.")) return 1.5;
            else if (name.Contains($"{ScaleX1_75}.")) return 1.75;
            else if (name.Contains($"{ScaleX2}.")) return 2;
            else if (name.Contains($"{ScaleX2_25}.")) return 2.25;
            else if (name.Contains($"{ScaleX2_5}.")) return 2.5;
            else if (name.Contains($"{ScaleX3}.")) return 3;
            else if (name.Contains($"{ScaleX3_5}.")) return 3.5;
            else if (name.Contains($"{ScaleX4}.")) return 4;
            else if (name.Contains($"{ScaleX4_5}.")) return 4.5;
            else if (name.Contains($"{ScaleX5}.")) return 5;
            else if (name.Contains($"{ScaleX6}.")) return 6;
            else if (name.Contains($"{ScaleX7}.")) return 7;
            else if (name.Contains($"{ScaleX8}.")) return 8;
            else return 1;
        }

        public static string GetScaleSuffixFromName(string name)
        {
            return GetScaleSuffix(GetScaleFactorFromName(name));
        }
        #endregion;
    }
}
