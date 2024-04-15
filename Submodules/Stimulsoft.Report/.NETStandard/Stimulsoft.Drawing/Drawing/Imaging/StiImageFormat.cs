using System;

namespace Stimulsoft.Drawing
{
    internal class StiImageFormat
    {
        #region Static Methods
        private class ImageInfo
        {
            public int width;
            public int height;
            public int horizontalResolution;
            public int verticalResolution;
        }

        private static ImageInfo TiffGetInfo(Byte[] dataBytes)
        {
            var info = new ImageInfo
            {
                width = 0,
                height = 0,
                horizontalResolution = 0,
                verticalResolution = 0
            };

            var offset = dataBytes[7] << 32 | dataBytes[6] << 16 | dataBytes[5] << 8 | dataBytes[4];
            var length = dataBytes[offset + 1] << 8 | dataBytes[offset];
            var pos = offset + 2;
            for (var index = 0; index <= length; index++)
            {
                var type = dataBytes[pos + 1] << 8 | dataBytes[pos];

                if (type == 256)
                    info.width = dataBytes[pos + 11] << 32 | dataBytes[pos + 10] << 16 | dataBytes[pos + 9] << 8 | dataBytes[pos + 8];
                if (type == 257)
                    info.height = dataBytes[pos + 11] << 32 | dataBytes[pos + 10] << 16 | dataBytes[pos + 9] << 8 | dataBytes[pos + 8];
                if (type == 282)
                    info.horizontalResolution = dataBytes[pos + 11] << 32 | dataBytes[pos + 10] << 16 | dataBytes[pos + 9] << 8 | dataBytes[pos + 8];
                if (type == 232)
                    info.verticalResolution = dataBytes[pos + 11] << 32 | dataBytes[pos + 10] << 16 | dataBytes[pos + 9] << 8 | dataBytes[pos + 8];

                pos += 12;
            }

            return info;
        }

        private static ImageInfo PngGetInfo(byte[] dataBytes)
        {
            var info = new ImageInfo
            {
                width = 0,
                height = 0,
                horizontalResolution = 0,
                verticalResolution = 0
            };

            var pos = -1;

            for (pos = 16; pos <= 16 + 3; pos++)
            {
                info.width = dataBytes[pos] | info.width << 8;
            }

            for (pos = 20; pos <= 20 + 3; pos++)
            {
                info.height = dataBytes[pos] | info.height << 8;
            }

            pos = 0;
            while (pos < dataBytes.Length)
            {
                if (dataBytes[pos++] == 0x70)
                    if (dataBytes[pos++] == 0x48)
                        if (dataBytes[pos++] == 0x59)
                            if (dataBytes[pos++] == 0x73)
                            {
                                pos += 4;
                                var resolution = 0;
                                resolution = dataBytes[pos++] | resolution << 8;
                                resolution = dataBytes[pos++] | resolution << 8;
                                resolution = dataBytes[pos++] | resolution << 8;
                                resolution = dataBytes[pos++] | resolution << 8;

                                info.verticalResolution = info.horizontalResolution = (int)Math.Round(resolution * 0.0254);
                            }
            }

            return info;
        }

        private static ImageInfo GifGetInfo(byte[] dataBytes)
        {
            var info = new ImageInfo
            {
                width = 0,
                height = 0,
                horizontalResolution = 300,
                verticalResolution = 300
            };

            info.width = dataBytes[6] | dataBytes[7] << 8;
            info.height = dataBytes[8] | dataBytes[9] << 8;
            return info;
        }

        private static ImageInfo JpegGetInfo(byte[] dataBytes, int flagsMask = 3)
        {
            var result = new ImageInfo
            {
                width = 0,
                height = 0,
                horizontalResolution = 0,
                verticalResolution = 0
            };

            var flags = 0;
            try
            {
                var pos = 0;
                while (pos < dataBytes.Length - 1)
                {
                    if (dataBytes[pos] != 0xff) break;  // "Cannot find next marker"
                    if ((flags & flagsMask) == flagsMask) break;  // all founded

                    var id = dataBytes[pos + 1];
                    if (id == 0xd9) break;  // EOI

                    var len = -1;
                    if (id >= 0xd0 && id <= 0xd9) // RSTn, SOI, EOI
                        len = 0;
                    if (id == 0xdd) // DRI
                        len = 4;
                    if ((len == -1) && (pos < dataBytes.Length - 3))
                    {
                        len = dataBytes[pos + 2] * 256 + dataBytes[pos + 3];
                    }

                    if (id == 0xC0 || id == 0xC1 || id == 0xC2 || id == 0xC3 || id == 0xC5 || id == 0xC6 || id == 0xC7 || id == 0xC9 || id == 0xCA || id == 0xCB || id == 0xCD || id == 0xCE || id == 0xCF)
                    {
                        result.width = dataBytes[pos + 7] * 256 + dataBytes[pos + 8];
                        result.height = dataBytes[pos + 5] * 256 + dataBytes[pos + 6];
                        flags |= 1;
                    }

                    if (id == 0xE0)
                    {
                        var xRes = dataBytes[pos + 14] * 256 + dataBytes[pos + 15];
                        var yRes = dataBytes[pos + 12] * 256 + dataBytes[pos + 13];
                        if (dataBytes[pos + 11] == 0)
                        {
                            xRes = (int)Math.Round(xRes * 96d) | 0;
                            yRes = (int)Math.Round(yRes * 96d) | 0;
                        }
                        if (dataBytes[pos + 11] == 2)
                        {
                            xRes = (int)Math.Round(xRes * 2.54) | 0;
                            yRes = (int)Math.Round(yRes * 2.54) | 0;
                        }
                        result.horizontalResolution = xRes;
                        result.verticalResolution = yRes;
                        flags |= 2;
                    }

                    pos += len + 2;

                    if (id == 0xDA)     // SOS
                    {
                        // find next marker, 0xFF 0x00 is escaped 0xFF
                        while (pos < dataBytes.Length)
                        {
                            if (dataBytes[pos++] != 0xff) continue;
                            if (dataBytes[pos++] != 0)
                            {
                                pos -= 2;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        private static ImageInfo BmpGetInfo(byte[] dataBytes)
        {
            var info = new ImageInfo
            {
                width = 0,
                height = 0,
                horizontalResolution = 0,
                verticalResolution = 0
            };

            info.width = dataBytes[18] | dataBytes[19] << 8;
            info.height = dataBytes[22] | dataBytes[23] << 8;
            info.horizontalResolution = (int)Math.Round((decimal)(dataBytes[38] | dataBytes[39] << 8));
            info.verticalResolution = (int)Math.Round((decimal)(dataBytes[42] | dataBytes[43] << 8));

            return info;
        }

        //private static _svg: ImageFormat;
        //static get Svg(): ImageFormat
        //{
        //    if (this._svg == null)
        //    {
        //        this._svg = new ImageFormat("svg+xml");
        //        this._svg.header = [0x3c, 0x73, 0x76, 0x67];
        //        if (Stimulsoft.System.NodeJs.useWebKit)
        //        {
        //            this._svg.getInfo = (dataBytes: number[], base64 = "", svg: string = null): ImageInfo => {
        //                let info: ImageInfo = { width: 0, height: 0, horizontalResolution: NaN, verticalResolution: NaN, needReconvert: false };

        //                if (svg == null) svg = base64;

        //                let div = document.createElement("div");
        //                div.style.wordWrap = "break-word";
        //                div.style.display = "inline-block";
        //                div.style.lineHeight = "0";
        //                div.innerHTML = svg;
        //                document.body.appendChild(div);

        //                let rect = div.getBoundingClientRect();

        //                info.width = rect.width;
        //                info.height = rect.height;

        //                document.body.removeChild(div);

        //                return info;
        //            };
        //        }
        //        this._svg.checkHeader = (data: number[]): ImageFormat => {
        //            let maxSvgOffset = 1000;

        //            try
        //            {
        //                if (data.length > 5 &&
        //                    String.fromCharCode(data[0]) == "<" &&
        //                    String.fromCharCode(data[1]) == "s" &&
        //                    String.fromCharCode(data[2]) == "v" &&
        //                    String.fromCharCode(data[3]) == "g" &&
        //                    Stimulsoft.System.Char.isWhitespace(String.fromCharCode(data[4]))) return this._svg;

        //                let stack: boolean[] = [];
        //                let flag1 = false;
        //                let level = 0;
        //                let pos = 0;
        //                while (pos < data.length - 5 && pos < maxSvgOffset)
        //                {
        //                    if (String.fromCharCode(data[pos]) == "<")
        //                    {
        //                        if (level == 0 &&
        //                            String.fromCharCode(data[pos + 1]) == "s" &&
        //                            String.fromCharCode(data[pos + 2]) == "v" &&
        //                            String.fromCharCode(data[pos + 3]) == "g" &&
        //                            Stimulsoft.System.Char.isWhitespace(String.fromCharCode(data[pos + 4])))
        //                            return this._svg;

        //                        if (String.fromCharCode(data[pos + 1]) == "/")
        //                        {
        //                            level--;
        //                            flag1 = stack.pop();
        //                        }
        //                        else
        //                        {
        //                            level++;
        //                            stack.push(flag1);
        //                            if (String.fromCharCode(data[pos + 1]) == "!" ||
        //                                String.fromCharCode(data[pos + 1]) == "?")
        //                                flag1 = true;
        //                        }
        //                    }
        //                    else if (String.fromCharCode(data[pos]) == ">")
        //                    {
        //                        if ((pos > 1 && String.fromCharCode(data[pos - 1]) == "/") || flag1)
        //                        {
        //                            level--;
        //                            flag1 = stack.pop();
        //                        }
        //                    }
        //                    pos++;
        //                }
        //            }
        //            catch (e)
        //            {
        //            }

        //            return null;
        //        };
        //    }

        //    return this._svg;
        //}

        private static bool CheckHeader(byte[] dataBytes, byte[] header)
        {
            for (var pos = 0; pos < header.Length; pos++)
            {
                if (header[pos] != dataBytes[pos]) return false;
            }

            return true;
        }

        public static StiImageFormat GetImageFormat(byte[] dataBytes)
        {
            StiImageFormat imageFormat = null;

            if (CheckHeader(dataBytes, new byte[] { 0x42, 0x4D }))
            {
                imageFormat = new StiImageFormat("bmp");
                imageFormat.info = BmpGetInfo(dataBytes);
            }

            if (CheckHeader(dataBytes, new byte[] { 0x47, 0x49, 0x46 }))
            {
                imageFormat = new StiImageFormat("gif");
                imageFormat.info = GifGetInfo(dataBytes);
            }
            if (CheckHeader(dataBytes, new byte[] { 0xFF, 0xD8 }))
            {
                imageFormat = new StiImageFormat("jpeg");
                imageFormat.info = new ImageInfo();

                var sizeInfo = JpegGetInfo(dataBytes, 1);
                var resolutionInfo = JpegGetInfo(dataBytes, 2);

                imageFormat.info.width = sizeInfo.width;
                imageFormat.info.height = sizeInfo.height;
                imageFormat.info.horizontalResolution = resolutionInfo.horizontalResolution;
                imageFormat.info.verticalResolution = resolutionInfo.verticalResolution;
            }
            if (CheckHeader(dataBytes, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            {
                imageFormat = new StiImageFormat("png");
                imageFormat.info = PngGetInfo(dataBytes);
            }
            if (CheckHeader(dataBytes, new byte[] { 0x49, 0x49, 0x2A, 0x00 }))
            {
                imageFormat = new StiImageFormat("tiff");
                imageFormat.info = TiffGetInfo(dataBytes);
            }

            if (imageFormat != null)
                imageFormat.DataBytes = dataBytes;

            return imageFormat;
        }
        #endregion

        #region Fileds
        private ImageInfo info;
        #endregion

        #region Properties
        public byte[] DataBytes { get; set; }

        public string Guid { get; }
        #endregion

        #region Methods
        public int GetWidth()
        {
            return info.width;
        }

        public int GetHeight()
        {
            return info.height;
        }

        public int GetHorizontalResolution()
        {
            return info.horizontalResolution;
        }

        public int GetVerticalResolution()
        {
            return info.verticalResolution;
        }

        public string GetBase64()
        {
            return "image/" + Guid + ";baes64," + Convert.ToBase64String(DataBytes);
        }
        #endregion

        public StiImageFormat(string guid)
        {
            this.Guid = guid;
        }
    }
}

