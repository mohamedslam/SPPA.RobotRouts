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
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
using ImageAttributes = Stimulsoft.Drawing.Imaging.ImageAttributes;
#endif

namespace Stimulsoft.Base.Drawing
{
	public class StiImageUtils
	{
        public static Bitmap GetImage(Type type, string imageName, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{
			return GetImage(type, imageName, true, throwError);
		}

        public static byte[] GetByteArray(Type type, string imageName, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            return GetByteArray(type, imageName, true, throwError);
        }

        /// <summary>
        /// Gets the Image object associated with Type.
        /// </summary>
        /// <param name="type">The type with which Image object is associated.</param>
        /// <param name="imageName">The name of the image file to look for.</param>
        /// <returns>The Image object.</returns>
        public static Bitmap GetImage(Type type, string imageName, bool makeTransparent, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{
			return GetImage(type.Module.Assembly, imageName, makeTransparent, throwError);
		}

        public static byte[] GetByteArray(Type type, string imageName, bool makeTransparent, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            return GetByteArray(type.Module.Assembly, imageName, makeTransparent, throwError);
        }

        public static Bitmap GetImage(string assemblyName, string imageName, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{
			return GetImage(assemblyName, imageName, true, throwError);
		}

        public static byte[] GetByteArray(string assemblyName, string imageName, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            return GetByteArray(assemblyName, imageName, true, throwError);
        }

        /// <summary>
        /// Gets the Image object placed in assembly.
        /// </summary>
        /// <param name="assemblyName">The name of assembly in which the Cursor object is placed.</param>
        /// <param name="imageName">The name of the image file to look for.</param>
        /// <returns>The Image object.</returns>
        public static Bitmap GetImage(string assemblyName, string imageName, bool makeTransparent, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{			
			var assemblys = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in assemblys)
			{
                if (StiBaseOptions.FullTrust)
                {
                    if (assembly.GetName().Name == assemblyName)
                        return GetImage(assembly, imageName, makeTransparent, throwError);
                }
                else
                {
                    var str = assembly.FullName;
                    var pos = str.IndexOf(',');
                    if (pos > -1) str = str.Substring(0, pos);

                    if (str == assemblyName)
                        return GetImage(assembly, imageName, makeTransparent, throwError);
                }
			}
			
            if (throwError == StiErrorProcessing.Exception)
			    throw new Exception($"Can't find assembly '{assemblyName}'");

            return null;
		}

        public static byte[] GetByteArray(string assemblyName, string imageName, bool makeTransparent, StiErrorProcessing throwError = StiErrorProcessing.Exception)
        {
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                if (StiBaseOptions.FullTrust)
                {
                    if (assembly.GetName().Name == assemblyName)
                        return GetByteArray(assembly, imageName, makeTransparent, throwError);
                }
                else
                {
                    var str = assembly.FullName;
                    var pos = str.IndexOf(',');
                    if (pos > -1) str = str.Substring(0, pos);

                    if (str == assemblyName)
                        return GetByteArray(assembly, imageName, makeTransparent, throwError);
                }
            }

            if (throwError == StiErrorProcessing.Exception)
                throw new Exception($"Can't find assembly '{assemblyName}'");

            return null;
        }

        public static Bitmap GetImage(Assembly imageAssembly, string imageName, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{
			return GetImage(imageAssembly, imageName, true, throwError);
		}

		/// <summary>
		/// Gets the Image object placed in assembly.
		/// </summary>
		/// <param name="imageAssembly">Assembly in which is the Image object is placed.</param>
		/// <param name="imageName">The name of the image file to look for.</param>
		/// <returns>The Image object.</returns>
		public static Bitmap GetImage(Assembly imageAssembly, string imageName, bool makeTransparent, StiErrorProcessing throwError = StiErrorProcessing.Exception)
		{
			var stream = imageAssembly.GetManifestResourceStream(imageName);
			if (stream != null)
			{
				var image = new Bitmap(stream);

				if (makeTransparent)
				    MakeImageBackgroundAlphaZero(image);

				return image;			
			}

            if (throwError == StiErrorProcessing.Exception)
		        throw new Exception($"Can't find image '{imageName}' in resources of '{imageAssembly}' assembly");

            return null;
		}

        /// <summary>
        /// Returns true if specified image exists.
        /// </summary>
        /// <param name="imageAssembly">Assembly in which is the Image object is placed.</param>
        /// <param name="imageName">The name of the image file to look for.</param>
        /// <returns>True if specified image exists.</returns>
        public static bool ExistsImage(Assembly imageAssembly, string imageName)
        {
            return imageAssembly.GetManifestResourceInfo(imageName) != null;
        }
        
        /// <summary>
        /// Returns true if specified image exists.
        /// </summary>
        /// <param name="imageAssembly">Assembly in which is the Image object is placed.</param>
        /// <param name="imageName">The name of the image file to look for.</param>
        /// <returns>True if specified image exists.</returns>
        public static bool ExistsImage(string imageAssembly, string imageName)
        {
            var assembly = StiAssemblyFinder.GetAssembly(imageAssembly);
            return assembly.GetManifestResourceInfo(imageName) != null;
        }

        public static byte[] GetByteArray(Assembly imageAssembly, string imageName, bool makeTransparent, StiErrorProcessing throwError)
        {
            var stream = imageAssembly.GetManifestResourceStream(imageName);
            if (stream != null)
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                return buffer;
            }

            if (throwError == StiErrorProcessing.Exception)
                throw new Exception($"Can't find image '{imageName}' in resources of '{imageAssembly}' assembly");

            return null;
        }

        public static void MakeImageBackgroundAlphaZero(Bitmap image)
		{
			var color1 = image.GetPixel(0, image.Height - 1);
			image.MakeTransparent();

			var color2 = Color.FromArgb(0, color1);
			image.SetPixel(0, image.Height - 1, color2);
		}
		
		public static Bitmap ReplaceImageColor(Bitmap bmp, Color colorForReplace, Color replacedColor)
		{
			var bufferImage = new Bitmap(bmp.Width, bmp.Height);
			var g = Graphics.FromImage(bufferImage);

			var imageAttr = new ImageAttributes();
		    imageAttr.SetRemapTable(new[]
		    {
		        new ColorMap
		        {
		            OldColor = replacedColor,
		            NewColor = colorForReplace
		        }
            });

			g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 
				0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttr);

			g.Dispose();

			return bufferImage;
		}
		
		public static Bitmap ConvertToDisabled(Bitmap bmp)
		{
			var bufferImage = new Bitmap(bmp.Width, bmp.Height);
            using (var g = Graphics.FromImage(bufferImage))
            {
                var imageAttr = new ImageAttributes();
                var disableMatrix = new ColorMatrix(new[]
                {
                    new[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                    new[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                    new[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                    new[] { 0, 0, 0, 0.4f, 0, 0 },
                    new[] { 0, 0, 0, 0, 0.4f, 0 },
                    new[] { 0, 0, 0, 0, 0, 0.4f }
                });

                imageAttr.SetColorMatrix(disableMatrix);

                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttr);
            }

			return bufferImage;
		}
		
		public static Bitmap ConvertToGrayscale(Bitmap bmp)
		{
			var bufferImage = new Bitmap(bmp.Width, bmp.Height);
			var g = Graphics.FromImage(bufferImage);

			var grayscaleMatrix = new ColorMatrix(new[]
			{
				new[]{0.3f,0.3f,0.3f,0,0},
				new[]{0.59f,0.59f,0.59f,0,0},
				new[]{0.11f,0.11f,0.11f,0,0},
				new[]{0f,0,0,1,0, 0 },
				new[]{0f,0,0,0,1, 0 },
				new[]{0f,0,0,0,0, 1 }
			});			


			var imageAttr = new ImageAttributes();
			imageAttr.SetColorMatrix(grayscaleMatrix);
			g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 
				0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttr);

			g.Dispose();
			return bufferImage;
		}

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var canvasWidth = width;
            var canvasHeight = height;
            var imageWidth = width;
            var imageHeight = height;

            if (image.Width == 16 && image.Height == 16)
            {
                switch ((decimal)StiScale.Factor)
                {
                    case 1.25m:
                        imageWidth = 16;
                        imageHeight = 16;
                        break;

                    case 2.25m:
                        imageWidth = 32;
                        imageHeight = 32;
                        break;

                    case 2.5m:
                        imageWidth = 32;
                        imageHeight = 32;
                        break;
                }
            }

            if (image.Width == 32 && image.Height == 32)
            {
                switch ((decimal)StiScale.Factor)
                {
                    case 1.25m:
                        imageWidth = 32;
                        imageHeight = 32;
                        break;

                    case 2.25m:
                        imageWidth = 64;
                        imageHeight = 64;
                        break;

                    case 2.5m:
                        imageWidth = 64;
                        imageHeight = 64;
                        break;
                }
            }

            return ResizeImage(image, canvasWidth, canvasHeight, imageWidth, imageHeight);
        }

        public static Bitmap ResizeImage(Image image, int canvasWidth, int canvasHeight, int imageWidth, int imageHeight, bool allowSampling = false)
        {
            if (allowSampling)
            {
                var destRect = new Rectangle(0, 0, canvasWidth, canvasHeight);
                var destImage = new Bitmap(canvasWidth, canvasHeight);
                using (var graphics = Graphics.FromImage(destImage))

                using (var wrapMode = new ImageAttributes())
                {
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }                

                return destImage;
            }
            else
            {
                var destRect = new Rectangle((canvasWidth - imageWidth) / 2, (canvasHeight - imageHeight) / 2, imageWidth, imageHeight);
                var destImage = new Bitmap(canvasWidth, canvasHeight);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
        }

        public static Icon IconFromBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return new Icon(ms);
            }
        }
    }
}
