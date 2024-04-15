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

using Stimulsoft.Base.Drawing;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
#endif

namespace Stimulsoft.Base.Exceptions
{
    public static class StiExceptionsImages
    {
        public static Image Copy(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("Copy", size, allowCache);

        public static Image Save(StiImageSize size = StiImageSize.Normal, bool allowCache = true) => GetImage("SaveFile", size, allowCache);

        #region Consts
        private const string RootPath = "Stimulsoft.Base.Exceptions.Images";
        #endregion

        #region Fields
        private static Hashtable mainCache = new Hashtable();
        #endregion

        #region Methods        
        public static Image GetImage(string path, StiImageSize size = StiImageSize.Normal, bool allowCache = true)
        {
            var cachedImage = allowCache ? mainCache[path + size.ToString()] as Bitmap : null;
            if (cachedImage != null && !IsDisposedImage(cachedImage))
                return cachedImage;

            var assembly = typeof(StiExceptionsImages).Assembly;
            var image = StiScaledImagesHelper.GetImage(assembly, $"{RootPath}.{path}", size);

            if (allowCache)
                mainCache[path + size.ToString()] = image;

            return image;
        }

        private static bool IsDisposedImage(Image image)
        {
            try
            {
                if (image.PixelFormat == PixelFormat.DontCare) return true;  //most disposed images have this value
                return image.Width == 0;//Access to the property of the image to check its disposed state.
            }
            catch
            {
                return true;
            }
        }
        #endregion
    }
}