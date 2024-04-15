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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Export;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Imaging;

#if STIDRAWING
using Bitmap = Stimulsoft.Drawing.Bitmap;
using BitmapData = Stimulsoft.Drawing.Imaging.BitmapData;
using Graphics = Stimulsoft.Drawing.Graphics;
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using ImageCodecInfo = Stimulsoft.Drawing.Imaging.ImageCodecInfo;
using Encoder = Stimulsoft.Drawing.Imaging.Encoder;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
#endif

namespace Stimulsoft.Report
{
    public class StiImageCache
    {
        public ArrayList ImageStore = null;
        public ArrayList ImagePackedStore = null;
        public ArrayList ImageMaskStore = null;
        public ArrayList ImageIndex = null;
        public List<ImageFormat> ImageFormatStore = null;
        public ArrayList ImagePaletteStore = null;

        private List<uint> ImageChecksum = null;
        private Hashtable ImageHashTable = null;
        private bool useImageComparer = true;
        private bool useImageCompression = false;
        private bool useImageTransparency = false;

        private ImageCodecInfo jpegCodec = null;
        private ImageFormat imageSaveFormat = ImageFormat.Bmp;
        private float imageQuality = 1;
        private int indexedColorPaletteSize = 0;

        private long usedMemoryCounter = 0;
        internal long UsedMemoryCounter
        {
            get
            {
                return usedMemoryCounter;
            }
            set
            {
                usedMemoryCounter = value;
            }
        }

        private long usedMemoryCounterLimit = 100000000;
        internal long UsedMemoryCounterLimit
        {
            get
            {
                return usedMemoryCounterLimit;
            }
            set
            {
                usedMemoryCounterLimit = value;
            }
        }

        #region Static CRC data
        readonly static uint CrcSeed = 0xFFFFFFFF;
        readonly static uint[] CrcTable = new uint[]
            {
                0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
                0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
                0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
                0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
                0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
                0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
                0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F,
                0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
                0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
                0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
                0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
                0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
                0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
                0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
                0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
                0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
                0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
                0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
                0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
                0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
                0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
                0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79,
                0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F,
                0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
                0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
                0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
                0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
                0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
                0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
                0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
                0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF,
                0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
            };
        #endregion

        #region StiImageCache()
        private void Init(bool useImageComparer, bool useImageCompression, ImageFormat imageFormat)
        {
            ImageStore = new ArrayList();
            ImagePackedStore = new ArrayList();
            ImageMaskStore = new ArrayList();
            ImageIndex = new ArrayList();
            ImageFormatStore = new List<ImageFormat>();
            ImagePaletteStore = new ArrayList();
            ImageChecksum = new List<uint>();
            ImageHashTable = new Hashtable();
            this.useImageComparer = useImageComparer;
            this.useImageCompression = useImageCompression;
            this.imageSaveFormat = imageFormat;
        }

        public StiImageCache(bool useImageComparer)
        {
            Init(useImageComparer, false, ImageFormat.Bmp);
        }

        /// <summary>
        /// ImageCache initialization 
        /// </summary>
        /// <param name="useImageComparer">Allow use image comparer</param>
        /// <param name="imageFormat">Image format to save</param>
        /// <param name="imageQuality">Quality of Jpeg compression; from 0.0 to 1.0</param>
		public StiImageCache(bool useImageComparer, ImageFormat imageFormat, float imageQuality)
        {
            Init(useImageComparer, true, imageFormat);
            this.imageQuality = imageQuality;
            jpegCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");
        }

        /// <summary>
        /// ImageCache initialization 
        /// </summary>
        /// <param name="useImageComparer">Allow use image comparer</param>
        /// <param name="imageFormat">Image format to save</param>
        /// <param name="imageQuality">Quality of Jpeg compression; from 0.0 to 1.0</param>
        /// <param name="useImageTransparency">Allow use image transparency</param>
		public StiImageCache(bool useImageComparer, ImageFormat imageFormat, float imageQuality, bool useImageTransparency, int paletteSize)
        {
            Init(useImageComparer, true, imageFormat);
            this.imageQuality = imageQuality;
            jpegCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");
            this.useImageTransparency = useImageTransparency;
            this.indexedColorPaletteSize = paletteSize;
        }
        #endregion

        #region Clear()
        public void Clear()
        {
            ImageStore.Clear();
            ImageStore = null;
            ImagePackedStore.Clear();
            ImagePackedStore = null;
            ImageMaskStore.Clear();
            ImageMaskStore = null;
            ImageIndex.Clear();
            ImageIndex = null;
            ImageFormatStore.Clear();
            ImageFormatStore = null;
            ImagePaletteStore.Clear();
            ImagePaletteStore = null;
            ImageChecksum.Clear();
            ImageChecksum = null;
            ImageHashTable.Clear();
            ImageHashTable = null;
            jpegCodec = null;
        }
        #endregion

        #region AddImage(Image)
        public int AddImageInt(Image image)
        {
            return AddImageInt(image, imageSaveFormat);
        }

        public int AddImageInt(Image image, float imageQuality)
        {
            float resImageQuality = this.imageQuality;
            this.imageQuality = imageQuality;
            int res = AddImageInt(image, imageSaveFormat);
            this.imageQuality = resImageQuality;
            return res;
        }

        public int AddImageInt(Image image, ImageFormat imageFormat)
        {
            UInt32 checkSum = (uint)ImageStore.Count;
            byte[] bytes = null;
            byte[] mask = null;
            Color[] palette = null;
            Image imageForMask = image;

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            if (imageFormat == null)
            {
                #region Auto-format mode
                imageFormat = ImageFormat.Jpeg;
                if (ImageFormat.MemoryBmp.Equals(image.RawFormat)) imageFormat = ImageFormat.Png;
                if (ImageFormat.Jpeg.Equals(image.RawFormat)) imageFormat = ImageFormat.Jpeg;
                if (ImageFormat.Png.Equals(image.RawFormat)) imageFormat = ImageFormat.Png;
                if (ImageFormat.Gif.Equals(image.RawFormat)) imageFormat = ImageFormat.Gif;
                #endregion
            }

            byte[] pixelsData2 = null;
            var isGdi = true;
#if STIDRAWING
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;

            //ImageSharp removes transparency when saving to Jpeg format, so need to get a mask before saving
            if (useImageCompression && useImageTransparency && (imageForMask is Bitmap) && !isGdi)
            {
                Bitmap bitmap = imageForMask as Bitmap;
                int bitmapWidth = bitmap.Width;
                int bitmapHeight = bitmap.Height;

                pixelsData2 = new byte[bitmapWidth * bitmapHeight];
                bool needMask = false;
                for (int y = 0; y < bitmapHeight; y++)
                {
                    int offs2 = y * bitmapWidth;
                    for (int x = 0; x < bitmapWidth; x++)
                    {
                        byte data = bitmap.GetPixel(x, y).A;
                        pixelsData2[offs2++] = data;
                        if (data != 255)
                        {
                            needMask = true;
                        }
                    }
                }
                usedMemoryCounter += pixelsData2.Length;
                if (!needMask) pixelsData2 = null;
            }
#endif

            if (useImageComparer || useImageCompression)
            {
                #region save image to bytes
                MemoryStream mem = new MemoryStream();
                if (imageFormat == ImageFormat.MemoryBmp)
                {
                    #region Save to FlateDecode format for PDF

                    if (!(image.PixelFormat == PixelFormat.Format32bppArgb ||
                        image.PixelFormat == PixelFormat.Format32bppRgb ||
                        image.PixelFormat == PixelFormat.Format1bppIndexed))
                    {
                        Image imageTemp = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
                        Graphics grTemp = Graphics.FromImage(imageTemp);

                        //grTemp.DrawImageUnscaled(image, 0, 0);
                        grTemp.DrawImage(image, 0, 0, imageWidth, imageHeight);

                        grTemp.Dispose();
                        image = imageTemp;
                    }

                    image.Save(mem, ImageFormat.Bmp);
                    mem.Seek(0, SeekOrigin.End);    //fix
                    mem.WriteByte(0);               //fix
                    byte[] bufBmp = mem.ToArray();
                    mem.Close();
                    mem.Dispose();
                    usedMemoryCounter += bufBmp.Length; //size of memory stream
                    byte[] bufDeflate = null;

                    if (image.PixelFormat == PixelFormat.Format1bppIndexed)
                    {
                        #region Monochrome image
                        bufDeflate = new byte[imageHeight * ((imageWidth - 1) / 8 + 1) + 4];
                        int posDeflate = 0;
                        byte bitDeflate = 0x80;
                        byte val = 0;

                        int offsBits = BitConverter.ToInt32(bufBmp, 10);
                        int stride = imageWidth / 32;
                        if (imageWidth % 32 > 0) stride++;
                        stride *= 4;

                        for (int posY = 0; posY < imageHeight; posY++)
                        {
                            int posBmp = offsBits + (imageHeight - posY - 1) * stride;
                            byte bitPos = 0x80;
                            byte valPos = bufBmp[posBmp];
                            for (int posX = 0; posX < imageWidth; posX++)
                            {
                                if ((valPos & bitPos) != 0)
                                {
                                    val |= bitDeflate;
                                }
                                bitPos >>= 1;
                                if (bitPos == 0)
                                {
                                    bitPos = 0x80;
                                    posBmp++;
                                    valPos = bufBmp[posBmp];
                                }
                                bitDeflate >>= 1;
                                if (bitDeflate == 0)
                                {
                                    bufDeflate[posDeflate] = val;
                                    val = 0;
                                    bitDeflate = 0x80;
                                    posDeflate++;
                                }
                            }
                            if (bitDeflate != 0x80)
                            {
                                bufDeflate[posDeflate] = val;
                                val = 0;
                                bitDeflate = 0x80;
                                posDeflate++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Color image
                        int bitsPerPixel = BitConverter.ToInt16(bufBmp, 28);
                        int bytesPerPixel = (bitsPerPixel == 24) ? 3 : 4;
                        if (!(bitsPerPixel == 24 || bitsPerPixel == 32))
                            throw new Exception("Unsupported number of bits per pixel !");
                        if (BitConverter.ToInt32(bufBmp, 30) != 0)
                            throw new Exception("Unsupported compression method !");

                        bufDeflate = new byte[imageHeight * imageWidth * 3];
                        int offsBits = BitConverter.ToInt32(bufBmp, 10);
                        int stride = imageWidth * bytesPerPixel;
                        if ((stride & 0x03) > 0) stride = ((stride >> 2) + 1) << 2;
                        int posDeflate = 0;
                        for (int posY = 0; posY < imageHeight; posY++)
                        {
                            int posBmp = offsBits + (imageHeight - posY - 1) * stride;
                            for (int posX = 0; posX < imageWidth; posX++)
                            {
                                bufDeflate[posDeflate + 0] = bufBmp[posBmp + 2];
                                bufDeflate[posDeflate + 1] = bufBmp[posBmp + 1];
                                bufDeflate[posDeflate + 2] = bufBmp[posBmp + 0];
                                posDeflate += 3;
                                posBmp += bytesPerPixel;
                            }
                        }
                        #endregion
                    }

                    if ((indexedColorPaletteSize == 0) || (image.PixelFormat == PixelFormat.Format1bppIndexed))
                    {
                        mem = StiExportUtils.MakePdfDeflateStream(bufDeflate);
                    }
                    else
                    {
                        mem = StiExportUtils.MakePdfDeflateStream(StiColorQuantization.ConvertToIndexed(bufDeflate, indexedColorPaletteSize, out palette, StiMonochromeDitheringType.None, ref usedMemoryCounter));
                    }

                    usedMemoryCounter += bufBmp.Length;
                    usedMemoryCounter += bufDeflate.Length;
                    bufBmp = null;
                    bufDeflate = null;
                    #endregion
                }
                else
                {
                    if (image.HorizontalResolution.Equals(1) || image.PixelFormat == (PixelFormat)8207)
                    {
                        Image imageTemp = new Bitmap(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
                        Graphics grTemp = Graphics.FromImage(imageTemp);
                        grTemp.DrawImage(image, 0, 0, imageWidth, imageHeight);
                        grTemp.Dispose();
                        image = imageTemp;
                    }

                    if (ImageFormat.Jpeg.Equals(imageFormat) && (jpegCodec != null))
                    {
                        EncoderParameters imageEncoderParameters = new EncoderParameters(1);
                        imageEncoderParameters.Param[0] =
                            new EncoderParameter(Encoder.Quality, (long)(imageQuality * 100));  // <------------;
                        image.Save(mem, jpegCodec, imageEncoderParameters);
                    }
                    else
                    {
                        image.Save(mem, imageFormat);
                    }
                }
                bytes = mem.ToArray();
                mem.Close();
                mem.Dispose();
                usedMemoryCounter += bytes.Length;  //size of memory stream
                #endregion

                #region Correction of Bmp alignment on Win8
                if ((imageFormat == ImageFormat.Bmp) && (bytes != null) && (BitConverter.ToInt16(bytes, 0) == 0x4d42) && (BitConverter.ToInt32(bytes, 14) == 40) &&
                    (BitConverter.ToInt32(bytes, 28) == 24) && ((image.Width * 3) % 4 > 0))
                {
                    int offset = BitConverter.ToInt32(bytes, 10) + (image.Width * 3);
                    int align = 4 - (image.Width * 3) % 4;
                    int stride = ((image.Width * 3) / 4 + 1) * 4;
                    for (int index = 0; index < image.Height; index++)
                    {
                        for (int index2 = 0; index2 < align; index2++)
                        {
                            bytes[offset + index2] = 0;
                        }
                        offset += stride;
                    }
                }
                #endregion
            }
            if (useImageComparer)
            {
                #region calculate checksum CRC32
                checkSum = CrcSeed;
                int offset = 0;
                int count = bytes.Length;
                while (--count >= 0)
                {
                    checkSum = CrcTable[(checkSum ^ bytes[offset++]) & 0xFF] ^ (checkSum >> 8);
                }
                checkSum ^= CrcSeed;
                #endregion
            }
            if (useImageCompression)
            {
                image = null;
            }
            else
            {
                bytes = null;
            }

            int imageIndex = -1;
            if (ImageHashTable.ContainsKey(checkSum))
            {
                imageIndex = (int)ImageHashTable[checkSum];
            }
            else
            {
                #region make mask
                if (useImageCompression && useImageTransparency && (imageForMask is Bitmap))
                {
                    Bitmap bitmap = imageForMask as Bitmap;
                    int bitmapWidth = bitmap.Width;
                    int bitmapHeight = bitmap.Height;

                    if (isGdi)
                    {
                        #region prepare mask data
                        if (StiOptions.Engine.FullTrust)
                        {
                            #region Full trust, fast
                            pixelsData2 = GetMaskInFullTrust(bitmap, ref usedMemoryCounter);
                            #endregion
                        }
                        else
                        {
                            #region Medium trust, slow
                            pixelsData2 = new byte[bitmapWidth * bitmapHeight];
                            bool needMask = false;
                            for (int y = 0; y < bitmapHeight; y++)
                            {
                                int offs2 = y * bitmapWidth;
                                for (int x = 0; x < bitmapWidth; x++)
                                {
                                    byte data = bitmap.GetPixel(x, y).A;
                                    pixelsData2[offs2++] = data;
                                    if (data != 255)
                                    {
                                        needMask = true;
                                    }
                                }
                            }
                            usedMemoryCounter += pixelsData2.Length;
                            if (!needMask) pixelsData2 = null;
                            #endregion
                        }
                        #endregion
                    }

                    if (pixelsData2 != null)
                    {
                        #region save mask to bytes
                        MemoryStream TmpStream = StiExportUtils.MakePdfDeflateStream(pixelsData2);
                        mask = TmpStream.ToArray();
                        TmpStream.Close();
                        TmpStream.Dispose();

                        usedMemoryCounter += mask.Length;
                        #endregion
                    }

                    pixelsData2 = null;
                }
                #endregion

                imageIndex = ImageStore.Count;
                ImageStore.Add(image);
                ImagePackedStore.Add(bytes);
                ImageMaskStore.Add(mask);
                ImagePaletteStore.Add(palette);
                ImageChecksum.Add(checkSum);
                ImageHashTable.Add(checkSum, imageIndex);
                ImageFormatStore.Add(imageFormat);
            }
            ImageIndex.Add(imageIndex);
            bytes = null;
            mask = null;
            imageForMask = null;

            if (StiOptions.Engine.UseGarbageCollectorForImageCache)
            {
                if (usedMemoryCounter > UsedMemoryCounterLimit)
                {
                    usedMemoryCounter = 0;
                    GC.Collect();

                    if (StiOptions.Engine.AllowWaitForPendingFinalizers)
                        GC.WaitForPendingFinalizers();

                    GC.Collect();
                }
            }
            else
            {
                usedMemoryCounter = 0;
            }

            return imageIndex;
        }

        private byte[] GetMaskInFullTrust(Bitmap bitmap, ref long usedMemoryCounter)
        {
            int pixelsDataCount1 = 0;
            int pixelsDataCount2 = 0;
            byte[] pixelsData1 = null;
            byte[] pixelsData2 = null;

            int bitmapWidth = bitmap.Width;
            int bitmapHeight = bitmap.Height;

            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = Math.Abs(bitmapData.Stride);

            // Declare an array to hold the bytes of the bitmap.
            pixelsDataCount1 = stride * bitmapData.Height;
            pixelsDataCount2 = bitmapData.Width * bitmapData.Height;
            pixelsData1 = new byte[pixelsDataCount1];
            pixelsData2 = new byte[pixelsDataCount2];

            // Copy the bitmap pixel values into the array.
            Marshal.Copy(bitmapData.Scan0, pixelsData1, 0, pixelsDataCount1);

            // Unlock the bits.
            bitmap.UnlockBits(bitmapData);

            //copy data to mask
            bool needMask = false;
            for (int y = 0; y < bitmapHeight; y++)
            {
                int offs1 = y * stride + 3;
                int offs2 = y * bitmapWidth;
                int x = bitmapWidth;
                while (x > 0)
                {
                    byte data = pixelsData1[offs1];
                    pixelsData2[offs2] = data;
                    if (data != 255)
                    {
                        needMask = true;
                    }
                    offs1 += 4;
                    offs2++;
                    x--;
                }
            }

            usedMemoryCounter += pixelsData1.Length;
            usedMemoryCounter += pixelsData2.Length;

            pixelsData1 = null;
            if (!needMask)
            {
                pixelsData2 = null;
            }
            return pixelsData2;
        }
        #endregion

    }
}
