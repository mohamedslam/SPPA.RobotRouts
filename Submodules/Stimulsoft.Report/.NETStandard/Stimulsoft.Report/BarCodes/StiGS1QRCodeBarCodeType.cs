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

using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
#else
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - QR Code.
    /// </summary>
    [TypeConverter(typeof(Design.StiQRCodeBarCodeTypeConverter))]
	public partial class StiGS1QRCodeBarCodeType : StiQRCodeBarCodeType
    {
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGS1QRCodeBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Main,
                    new[]
                    {
                        propHelper.ErrorCorrectionLevel(),
                        propHelper.MatrixSize(),
                        propHelper.Module(),
                        propHelper.ProcessTilde()
                    }
                },
                {
                    StiPropertyCategories.Appearance,
                    new[]
                    {
                        propHelper.BodyBrush(),
                        propHelper.BodyShape(),
                        propHelper.EyeBallBrush(),
                        propHelper.EyeBallShape(),
                        propHelper.EyeFrameBrush(),
                        propHelper.EyeFrameShape()
                    }
                }
            };
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "GS1 QR Code";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "[21]012345[3103]000123";

        [Browsable(false)]
        [StiBrowsable(false)]
        public override Image Image { get; set; }

        [Browsable(false)]
        [StiBrowsable(false)]
        public override double ImageMultipleFactor { get; set; } = 1d;
        
        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[10] = true;
                props[12] = true;
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
            if (code == "System.Byte[]") code = String.Empty;
            code = CheckCodeSymbols(code, "!\"%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz[]");

            var sbCode = new StringBuilder();
            var sbText = new StringBuilder();
            string errorMessage = StiGS1ApplicationIdentifiers.ParseCode(code, sbCode, sbText, (char)BarcodeCommandCode.Fnc1, false);

            if (errorMessage != null && (barCode.CodeValue == null) && barCode.Code.Value.Contains("{"))
            {
                errorMessage = null;
                sbCode = new StringBuilder(barCode.Code.Value);
                sbText = new StringBuilder(barCode.Code.Value);
            }

            BarCodeData.Code = sbCode.ToString();

            var errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.L;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level2) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.M;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level3) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.Q;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level4) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.H;

            try
            {
                var qrCode = new StiQRCode();
                bool assembleData = (context is StiBarCodeExportPainter) && (context as StiBarCodeExportPainter).OnlyAssembleData;
                if (!assembleData)  //speed optimization for pdf
                {
                    StiQRCode.QREncoder.Encode(sbCode.ToString(), errorCorrectionLevel2, qrCode, MatrixSize, ProcessTilde, true);
                }
                else
                {
                    var matrix1 = new StiQRCode.ByteMatrix(1, 1);
                    matrix1.Set(0, 0, 1);
                    qrCode.SetMatrix(matrix1);
                }
                var bm = qrCode.GetMatrix();

                byte[] matrix = new byte[bm.GetWidth() * bm.GetHeight()];

                for (int y = 0; y < bm.GetHeight(); y++)
                {
                    int offset = y * bm.GetWidth();
                    for (int x = 0; x < bm.GetWidth(); x++)
                    {
                        matrix[offset + x] = (byte)bm.Get(x, y);
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.GetWidth();
                BarCodeData.MatrixHeight = bm.GetHeight();
                BarCodeData.MatrixRatioY = 1;

                if (errorMessage == null)
                {
                    DrawQRCode(context, rect, barCode, zoom, BodyShape, EyeFrameShape, EyeBallShape);
                }
                else
                {
                    DrawBarCodeError(context, rect, barCode, errorMessage);
                }
            }
            catch
            {
                DrawBarCodeError(context, rect, barCode);
            }
		}

        public static new Image GetBarcodeImage(string code, int zoom)
        {
            var qr = new StiBarCode();
            qr.BarCodeType = new StiQRCodeBarCodeType(10, StiQRCodeErrorCorrectionLevel.Level1, StiQRCodeSize.Automatic);
            var BarCodeData = (qr.BarCodeType as StiGS1QRCodeBarCodeType).BarCodeData;
            BarCodeData.Code = code;

            Image resultImage = null;

            var errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.L;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level2) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.M;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level3) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.Q;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level4) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.H;

            try
            {
                var qrCode = new StiQRCode();
                StiQRCode.QREncoder.Encode(code, errorCorrectionLevel2, qrCode, StiQRCodeSize.Automatic, false);
                var bm = qrCode.GetMatrix();

                byte[] matrix = new byte[bm.GetWidth() * bm.GetHeight()];

                for (int y = 0; y < bm.GetHeight(); y++)
                {
                    int offset = y * bm.GetWidth();
                    for (int x = 0; x < bm.GetWidth(); x++)
                    {
                        matrix[offset + x] = (byte)bm.Get(x, y);
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.GetWidth();
                BarCodeData.MatrixHeight = bm.GetHeight();
                BarCodeData.MatrixRatioY = 1;

                int quietZone = 2;
                if (!qr.ShowQuietZones) quietZone = 0;
                int imageWidth = (BarCodeData.MatrixWidth + quietZone * 2) * zoom;
                int imageHeight = (BarCodeData.MatrixHeight + quietZone * 2) * zoom;
                resultImage = new Bitmap(imageWidth, imageHeight);
                using (var gr = Graphics.FromImage(resultImage))
                {
                    (qr.BarCodeType as StiGS1QRCodeBarCodeType).Draw2DBarCode(gr, new Rectangle(0, 0, imageWidth, imageHeight), qr, zoom);
                }
            }
            catch
            {
            }

            return resultImage;
        }        
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiGS1QRCodeBarCodeType();
        #endregion

        public StiGS1QRCodeBarCodeType()
            : this(40f, StiQRCodeErrorCorrectionLevel.Level1, StiQRCodeSize.Automatic)
		{
		}

        public StiGS1QRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize)
		{
			this.Module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
			this.MatrixSize = matrixSize;
		}

        public StiGS1QRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize, Image image, double imageMultipleFactor,
            StiQRCodeBodyShapeType bodyShape, StiQRCodeEyeFrameShapeType eyeFrameShape, StiQRCodeEyeBallShapeType eyeBallShape,
            StiBrush bodyBrush, StiBrush eyeFrameBrush, StiBrush eyeBallBrush, bool processTilde)
        {
            this.Module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
            this.MatrixSize = matrixSize;
            this.Image = image;
            this.ImageMultipleFactor = imageMultipleFactor;
            this.BodyShape = bodyShape;
            this.EyeFrameShape = eyeFrameShape;
            this.EyeBallShape = eyeBallShape;
            this.BodyBrush = bodyBrush;
            this.EyeFrameBrush = eyeFrameBrush;
            this.EyeBallBrush = eyeBallBrush;
            this.ProcessTilde = processTilde;
        }
    }
}