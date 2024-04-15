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

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
	public class StiHtmlImageHost
	{
        #region Methods
        public virtual string GetImageString(Bitmap bmp)
		{
            var imageFormat = ForcePng ? ImageFormat.Png : HtmlExport.imageFormat;
			var imageCount = ImageCache.ImagePackedStore.Count;
			var imageNumber = ImageCache.AddImageInt(bmp, imageFormat);

            if (imageFormat == null) imageFormat = (ImageFormat)ImageCache.ImageFormatStore[imageNumber];

            if (HtmlExport.zip != null && !HtmlExport.useEmbeddedImages)
            {
                var fileName = HtmlExport.fileName;
                var imageStr = $"{fileName}.files/{fileName}{imageNumber + 1}.{imageFormat.ToString()}";

                if (ImageCache.ImagePackedStore.Count <= imageCount)
                    return imageStr;

                var bytes = (byte[])ImageCache.ImagePackedStore[imageNumber];
                var ms = new MemoryStream(bytes);
                HtmlExport.zip.AddFile(imageStr, ms);

                return imageStr;
            }

            string baseDir = null;
            if (HtmlExport.isFileStreamMode)
            {
                try
                {
                    baseDir = Path.GetDirectoryName(HtmlExport.fileName).Replace("\\", "/");
                }
                catch
                {
                    HtmlExport.isFileStreamMode = false;
                }
            }

            if (HtmlExport.isFileStreamMode)
            {
                if (baseDir.Length > 0 && baseDir[baseDir.Length - 1] != '/')
                    baseDir += '/';

                var dir = Path.GetFileNameWithoutExtension(HtmlExport.fileName) + ".files/";

                if (!Directory.Exists(baseDir + dir))
                    Directory.CreateDirectory(baseDir + dir);

                var imageStr = Path.GetFileNameWithoutExtension(HtmlExport.fileName) + (imageNumber + 1) + "." + imageFormat.ToString();

                if (ImageCache.ImagePackedStore.Count > imageCount)	//new image
                {
                    try
                    {
                        using (var stream = File.OpenWrite(baseDir + dir + imageStr))
                        {
                            var bytes = (byte[])ImageCache.ImagePackedStore[imageNumber];
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                    catch
                    {
                        HtmlExport.isFileStreamMode = false;
                    }
                }

                if (HtmlExport.isFileStreamMode)
                    return dir + imageStr;
            }

            var imageData = (byte[])ImageCache.ImagePackedStore[imageNumber];
            return $"data:image/{imageFormat.ToString().ToLower()};base64,{Convert.ToBase64String(imageData)}";
		}
        #endregion

        #region Properties
        protected StiHtmlExportService HtmlExport { get; set; }

	    public StiImageCache ImageCache { get; set; }

	    internal bool IsMhtExport { get; set; }

		internal bool ForcePng { get; set; }
        #endregion

        public StiHtmlImageHost(StiHtmlExportService htmlExport)
		{
			this.HtmlExport = htmlExport;
			this.IsMhtExport = false;
            this.ForcePng = false;
        }
    }	
}
