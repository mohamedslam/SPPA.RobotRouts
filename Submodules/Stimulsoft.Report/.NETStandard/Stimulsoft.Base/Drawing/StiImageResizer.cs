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
using Stimulsoft.Base;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    public static class StiImageResizer
    {
        #region Methods
        public static Image GetImage(Image image, bool allowDispose = true, double? scaleFactor = null)
        {
            if (image == null)
                return null;

            if (scaleFactor != null)
            {
                var newImage = StiImageUtils.ResizeImage(image, 
                    Scale(image.Width, scaleFactor.Value), 
                    Scale(image.Height, scaleFactor.Value));

                if (allowDispose)
                    image.Dispose();

                return newImage;
            }
            else
            {
                if (StiScale.IsNoScaling)
                    return image;

                var newImage = StiImageUtils.ResizeImage(image, StiScale.XXI(image.Width), StiScale.YYI(image.Height));
                if (allowDispose)
                    image.Dispose();

                return newImage;
            }
        }

        public static Image GetImageWithSampling(Image image)
        {
            if (image == null)
                return null;

            if (StiScale.IsNoScaling)
                return image;

            var canvasWidth = StiScale.XXI(image.Width);
            var canvasHeight = StiScale.YYI(image.Height);
            var newImage = StiImageUtils.ResizeImage(image, canvasWidth, canvasHeight, canvasWidth, canvasHeight, true);
            image.Dispose();

            return newImage;
        }

        private static int Scale(double value, double scaleFactor)
        {
            return (int)Math.Ceiling(scaleFactor * value);
        }
        #endregion
    }
}