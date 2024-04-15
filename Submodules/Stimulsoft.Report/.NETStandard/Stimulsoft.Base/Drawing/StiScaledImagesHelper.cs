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

using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using System;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiScaledImagesHelper
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
        public static Image GetImage(Assembly assembly, string path, StiImageSize size = StiImageSize.Normal, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            switch (size)
            {
                case StiImageSize.Normal:
                    return GetNormalImage(assembly, path, throwError);

                case StiImageSize.OneHalf:
                    return GetOneHalfImage(assembly, path, throwError);

                default:
                    return GetDoubleImage(assembly, path, throwError);
            }
        }

        private static Image GetNormalImage(Assembly assembly, string path, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            var factor = (decimal)StiScaleUI.Factor;

            if (factor <= 1m)
                return GetImageFromResources(assembly, path, throwError);

            if (factor <= 1.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX1_25, "", 20, 16, throwError);

            if (factor <= 1.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX1_5, null, 0, 0, throwError);

            if (factor <= 1.75m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX1_75, ScaleX1_5, 28, 24, throwError);

            if (factor <= 2m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX2, null, 0, 0, throwError);

            if (factor <= 2.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX2_25, ScaleX2, 36, 32, throwError);

            if (factor <= 2.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX2_5, ScaleX2, 40, 32, throwError);

            if (factor <= 3m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3, ScaleX1_5, 48, 48, throwError);

            if (factor <= 3.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3_5, ScaleX1_5, 56, 48, throwError);

            if (factor <= 4m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX4, ScaleX2, 64, 64, throwError);

            return GetImageFromResourcesAndResize(assembly, path, throwError);
        }

        private static Image GetOneHalfImage(Assembly assembly, string path, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            var factor = (decimal)StiScaleUI.Factor;
            if (factor <= 1m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX1_5, null, 0, 0, throwError);

            if (factor <= 1.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX1_75, ScaleX1_5, 30, 24, throwError);

            if (factor <= 1.5m)
                return GetIfExistOrResizeImage(assembly, path, null, ScaleX2, 36, 32, throwError);

            if (factor <= 1.75m)
                return GetIfExistOrResizeImage(assembly, path, null, ScaleX2, 42, 32, throwError);

            if (factor <= 2m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3, ScaleX1_5, 48, 48, throwError);

            if (factor <= 2.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3, ScaleX1_5, 54, 48, throwError);

            if (factor <= 2.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3, ScaleX1_5, 60, 48, throwError);

            if (factor <= 3m)
                return GetIfExistOrResizeImage(assembly, path, null, ScaleX4, 72, 64, throwError);

            return GetImageFromResourcesAndResize(assembly, path, throwError);
        }        

        private static Image GetDoubleImage(Assembly assembly, string path, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            var factor = (decimal)StiScaleUI.Factor;

            if (factor <= 1m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX2, null, 0, 0, throwError);

            if (factor <= 1.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX2_5, ScaleX2, 40, 32, throwError);

            if (factor <= 1.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3, ScaleX1_5, 48, 48, throwError);

            if (factor <= 1.75m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX3_5, ScaleX1_5, 56, 48, throwError);

            if (factor <= 2m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX4, ScaleX2, 64, 64, throwError);

            if (factor <= 2.25m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX4_5, ScaleX2, 72, 64, throwError);

            if (factor <= 2.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX5, ScaleX2, 80, 64, throwError);

            if (factor <= 3m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX6, ScaleX2, 96, 64, throwError);

            if (factor <= 3.5m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX7, null, 0, 0, throwError);

            if (factor <= 4m)
                return GetIfExistOrResizeImage(assembly, path, ScaleX8, null, 0, 0, throwError);

            return GetImageFromResourcesAndResize(assembly, path, throwError);            
        }

        private static Image GetIfExistOrResizeImage(Assembly assembly, string path, string originalScale, string scale2 = null, 
            int canvasSize = 0, int imageSize = 0, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            if (originalScale != null && ExistImage(assembly, $"{path}{originalScale}"))
            {
                return GetImageFromResources(assembly, $"{path}{originalScale}", throwError);
            }
            else if (scale2 != null && ExistImage(assembly, $"{path}{scale2}"))
            {
                return GetImageFromResourcesAndResize(assembly, $"{path}{scale2}", canvasSize, imageSize, false, throwError);
            }
            else if (canvasSize > 0)
            {
                var image = GetNearestImageFromResourcesAndResize(assembly, path, canvasSize);
                if (image != null)
                    return image;
            }

            if (ExistImage(assembly, $"{path}"))
                return GetImageFromResourcesAndResize(assembly, path, throwError);

            else if (ExistImage(assembly, $"{path}_x2"))
                return GetImageFromResourcesAndResize(assembly, $"{path}_x2", throwError);

            else if (throwError == StiErrorProcessing.Exception)
                throw new ArgumentNullException($"The image '{path}' is not found!");
            
            else
                return null;
        }

        public static Bitmap GetNearestImageFromResourcesAndResize(Assembly assembly, string path, int canvasSize)
        {
            var scales = new List<string>() { "x1_25", "x1_5", "x1_75", "x2", "x2_25", "x2_5", "x3", "x3_5", "x4" };
            var index = scales.Count - 1;

            while (index >= 0)
            {
                var scale = scales[index];
                if (ExistImage(assembly, $"{path}_{scale}"))
                    return GetImageFromResourcesAndResize(assembly, $"{path}_{scale}", canvasSize, canvasSize, true) as Bitmap;

                index--;
            }
            return null;
        }

        public static Bitmap GetImageFromResources(Assembly assembly, string path, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            return StiImageUtils.GetImage(assembly, AddImageExtToPath(path), false, throwError);
        }

        private static Image GetImageFromResourcesAndResize(Assembly assembly, string path, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            using (var image = GetImageFromResources(assembly, path, throwError))
            {
                if (image == null)
                    return null;

                return StiImageUtils.ResizeImage(image, StiScaleUI.XXI(image.Width), StiScaleUI.YYI(image.Height));
            }
        }

        private static Image GetImageFromResourcesAndResize(Assembly assembly, string path, int canvasSize, int imageSize, 
            bool allowSampling = false, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            using (var image = GetImageFromResources(assembly, path, throwError))
            {
                if (image == null)
                    return null;

                return StiImageUtils.ResizeImage(image, canvasSize, canvasSize, imageSize, imageSize, allowSampling);
            }
        }

        private static bool ExistImage(Assembly assembly, string path)
        {
            return StiImageUtils.ExistsImage(assembly, AddImageExtToPath(path));
        }

        private static string AddImageExtToPath(string path)
        {
            return $"{path}.png";
        }
        #endregion
    }
}