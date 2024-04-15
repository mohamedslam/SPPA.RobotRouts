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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.BarCodes
{
    [TypeConverter(typeof(StiBarCodeTypeServiceConverter))]
    public abstract class StiBarCodeTypeService : 
        StiService, 
        IStiPropertyGridObject,
        IStiJsonReportObject
	{
        #region IStiJsonReportObject.override
        internal static StiBarCodeTypeService CreateFromJsonObject(JObject jObject)
        {
            var ident = jObject.Properties().FirstOrDefault(x => x.Name == "Ident").Value.ToObject<string>();

            var service = StiOptions.Services.BarCodes.FirstOrDefault(x => x.GetType().Name == ident);

            if (service == null)
                throw new Exception($"Type {ident} is not found!");

            var barCodeTypeService = service.CreateNew();
            barCodeTypeService.LoadFromJsonObject(jObject);
            return barCodeTypeService;
        }

        public virtual JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = new JObject();

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            return jObject;
        }

        public abstract void LoadFromJsonObject(JObject jObject);
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public abstract StiComponentId ComponentId { get; }

        [Browsable(false)]
        public string PropName => string.Empty;

        public virtual StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level) => null;

        public virtual StiEventCollection GetEvents(IStiPropertyGrid propertyGrid) => null;
        #endregion

		#region StiService override
		/// <summary>
		/// Gets a service category.
		/// </summary>
		[Browsable(false)]
		public sealed override string ServiceCategory => "BarCodeTypes";

		/// <summary>
		/// Gets a service type.
		/// </summary>
		[Browsable(false)]
		public sealed override Type ServiceType => typeof(StiBarCodeTypeService);
		#endregion

		#region Class BarCodeData
		protected class StiBarCodeData
		{
			//for 1D barcodes
			public float SpaceLeft;
			public float SpaceRight;
			public float SpaceTop;
			public float SpaceBottom;
			public float LineHeightShort;
			public float LineHeightLong;
			public float LineWidth;
			public float TextPosition;
			public float TextHeight;
			public float MainHeight;
			public float MainWidth;
			public float WideToNarrowRatio;
			public string Code;
			public string TextString;
			public string BarsArray;
			public float FullZoomY;
			//for EAN13-based barcodes
			public float SpaceBeforeAdd;
			public float SpaceTextTop;
			public float TextPositionTop;
			public float TextPositionBottom;
            public List<StiEAN13BarCodeType.EanBarInfo> EanBarsArray;
			public float OffsetY;
			//for 2D barcodes
			public byte[] MatrixGrid;
			public int MatrixWidth;
            public int MatrixHeight;
			public int MatrixRatioY;
		}
		#endregion

        #region Properties.abstract
        public const int visiblePropertiesCount = 27;

        [Browsable(false)]
        public abstract bool[] VisibleProperties { get; }

        [Browsable(false)]
        public abstract string DefaultCodeValue { get; }
        #endregion

        #region Properties
        [Browsable(false)]
        public IStiBarCodePainter CustomPainter { get; set; }

        [Browsable(false)]
        public float MainWidth => barCodeData.MainWidth;

        [Browsable(false)]
        public float MainHeight => barCodeData.MainHeight;

		private StiBarCodeData barCodeData = new StiBarCodeData();
		protected StiBarCodeData BarCodeData => barCodeData;

		[StiSerializable]
        public abstract float Module { get; set; }

		[StiSerializable]
		public abstract float Height { get; set; }

		protected virtual StringAlignment TextAlignment => StringAlignment.Center;

        protected virtual bool TextSpacing => true;

        protected virtual bool PreserveAspectRatio => false;

        protected RectangleF RectWindow = new RectangleF(0, 0, 0, 0);

        internal abstract float LabelFontHeight { get; }

        protected const float DefaultLabelFontHeight = 8;
        #endregion

		#region Methods
		/// <summary>
		/// Remove from string all undefined symbols.
		/// </summary>
		protected string CheckCodeSymbols(string inputCode, string tolerantSymbols)
		{
            var outputCode = new StringBuilder();
			if (inputCode != null)
			{
				for (int index = 0; index < inputCode.Length; index++)
				{
					char currentChar = inputCode[index];
                    if (tolerantSymbols.IndexOf(currentChar) != -1)
					{
						outputCode.Append(currentChar);
					}
				}
			}
			return outputCode.ToString();
		}

		/// <summary>
		/// Returns string of char - input data for calculation of bar code.
		/// </summary>
		/// <param name="barCode"></param>
		/// <returns></returns>
		public virtual string GetCode(IStiBarCode barCode) => barCode.GetBarCodeString();

        public virtual string GetCombinedCode() => null;

        internal enum BarcodeCommandCode
        {
            Fnc1 = 0xFFB1,
            Fnc2 = 0xFFB2,
            Fnc3 = 0xFFB3,
            Fnc4 = 0xFFB4
        }

        internal static int[] UnpackTilde(int[] input, bool processTilde)
        {
            int index = 0;
            var output = new List<int>();
            while (index < input.Length)
            {
                int ch = input[index++];
                if (processTilde && (ch == '~'))
                {
                    bool flag = false;
                    if ((index < input.Length) && (input[index] == '~'))
                    {
                        output.Add('~');
                        flag = true;
                        index++;
                    }
                    if (!flag && (index + 2 < input.Length))
                    {
                        if ((input[index] == 'F') && (input[index + 1] == 'N') && (input[index + 2] == 'C') && (index + 3 < input.Length))
                        {
                            int num = input[index + 3] - 48;
                            if (num == 1 || num == 2 || num == 3 || num == 4)
                            {
                                if (num == 1) output.Add((int)BarcodeCommandCode.Fnc1);
                                if (num == 2) output.Add((int)BarcodeCommandCode.Fnc2);
                                if (num == 3) output.Add((int)BarcodeCommandCode.Fnc3);
                                if (num == 4) output.Add((int)BarcodeCommandCode.Fnc4);
                                flag = true;
                                index += 4;
                            }
                        }
                        if (!flag && char.IsDigit((char)input[index]) && char.IsDigit((char)input[index + 1]) && char.IsDigit((char)input[index + 2]))
                        {
                            int num = (input[index] - 48) * 100 + (input[index + 1] - 48) * 10 + (input[index + 2] - 48);
                            if (num >= 0 && num <= 255)
                            {
                                output.Add(num);
                                flag = true;
                                index += 3;
                            }
                        }
                    }
                    if (flag) continue;
                }
                output.Add(ch);
            }
            return output.ToArray();
        }

        internal static string UnpackTilde(string input, bool processTilde)
        {
            if (!processTilde) return input;
            int[] ints = new int[input.Length];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = (int)input[i];
            }
            var ints2 = UnpackTilde(ints, true);
            StringBuilder sb = new StringBuilder(ints2.Length);
            for (int i = 0; i < ints2.Length; i++)
            {
                sb.Append((char)ints2[i]);
            }
            return sb.ToString();
        }

        #region Get Symbol Data
        /// <summary>
        /// Returns width of one symbol in module.
        /// </summary>
        protected virtual float GetSymbolWidth(char symbol)
		{
			float symbolWidth;
			switch (symbol)
			{
				case '0':
				case '4':
				case '8':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					symbolWidth = 1f;
					break;
				case '1':
				case '5':
				case '9':
					symbolWidth = 1f * BarCodeData.WideToNarrowRatio;
					break;
				case '2':
				case '6':
				case 'a':
					symbolWidth = 1.5f * BarCodeData.WideToNarrowRatio;
					break;
				case '3':
				case '7':
				case 'b':
					symbolWidth = 2f * BarCodeData.WideToNarrowRatio;
					break;
				default:
					symbolWidth = 1f;
					break;
			}
			return symbolWidth;
		}

		/// <summary>
		/// Returns true if one line is long.
		/// </summary>
		protected bool IsSymbolLong(char symbol)
		{
			bool isLong;
			switch (symbol)
			{
				case '8':
				case '9':
				case 'a':
				case 'b':
				case 'c':
				//case 'd':
				case 'e':
				//case 'f':
					isLong = true;
					break;
				default:
					isLong = false;
					break;
			}
			return isLong;
		}

		/// <summary>
		/// Returns true is char is space.
		/// </summary>
		protected virtual bool IsSymbolSpace(char symbol)
		{
			bool isSpace;
			switch (symbol)
			{
				case '0':
				case '1':
				case '2':
				case '3':
					isSpace = true;
					break;
				default:
					isSpace = false;
					break;
			}
			return isSpace;
		}

		/// <summary>
		/// Returns true if short line is post-descend
		/// </summary>
		protected bool IsSymbolPostDescend(char symbol)
		{
			bool isPostDescend;
			switch (symbol)
			{
				//case 'c':
				//case 'd':
				case 'e':
				case 'f':
					isPostDescend = true;
					break;
				default:
					isPostDescend = false;
					break;
			}
			return isPostDescend;
		}
		#endregion

		/// <summary>
		/// Returns width of string in modules.
		/// </summary>
		/// <param name="symbolsString"></param>
		/// <returns></returns>
		protected float GetSymbolsStringWidth(string symbolsString)
		{
			float symbolsStringLength = 0;
			for (int index = 0; index < symbolsString.Length; index++)
			{
				symbolsStringLength += GetSymbolWidth(symbolsString[index]);
			}
			return symbolsStringLength;
		}


		/// <summary>
		/// Draws content of bar code.
		/// </summary>
        protected void DrawBars(object context, string sym, StiBrush foreBrush)
		{
			float binOffsetX = BarCodeData.SpaceLeft;
			float binOffsetY = BarCodeData.SpaceTop;
			float inSymPosition = 0;
			for (int index = 0; index < sym.Length; index++)
			{
				char currentChar = sym[index];
				if (IsSymbolSpace(currentChar) == false)
				{
					float binHeight = BarCodeData.LineHeightShort;
					if (IsSymbolLong(currentChar) == true)
					{
						binHeight = BarCodeData.LineHeightLong;
					}
					float binPostnetOffset = 0;
					if (IsSymbolPostDescend(currentChar) == true) 
					{
						binPostnetOffset = BarCodeData.LineHeightLong - BarCodeData.LineHeightShort;
					}
					BaseFillRectangle(context, foreBrush,
						(binOffsetX + BarCodeData.LineWidth * inSymPosition),
						(binOffsetY + binPostnetOffset),
						(BarCodeData.LineWidth * GetSymbolWidth(currentChar)),
						(binHeight - binPostnetOffset));
				}
				inSymPosition += GetSymbolWidth(currentChar);
			}
		}

        protected delegate void DrawBaseLinesDelegate(object context, StiBrush foreBrush);

        /// <summary>
        /// Draws bar code.
        /// </summary>
        protected void DrawBarCode(object context, RectangleF rect, StiBarCode barCode, DrawBaseLinesDelegate drawMethod = null)
		{
            bool needPixel = context is Graphics;
            float fontScale = needPixel ? 1 : 0.72f / 0.96f;
            using (var barCodeFont = new Font(
                       StiFontCollection.GetFontFamily(barCode.Font.Name),
                       barCode.Font.Size * (barCode.BarCodeType.LabelFontHeight / 8f) * BarCodeData.FullZoomY * fontScale,
                       barCode.Font.Style,
                       needPixel ? GraphicsUnit.Pixel : GraphicsUnit.Point))
			{
                TranslateRect(context, rect, barCode);

                var foreBrush = new StiSolidBrush(barCode.ForeColor);
                var backBrush = new StiSolidBrush(barCode.BackColor);
				
                BaseFillRectangle(context, backBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.MainHeight);
				

                DrawBars(context, BarCodeData.BarsArray, foreBrush);

				if ((barCode.ShowLabelText) && (BarCodeData.TextString.Length > 0)) 
				{
                    var tempString = new StringBuilder();
                    if (TextSpacing)
                    {
                        for (int index = 0; index < BarCodeData.TextString.Length; index++)
                        {
                            tempString.Append(BarCodeData.TextString[index]);
                            tempString.Append(' ');
                        }
                        tempString.Length = tempString.Length - 1;
                    }
                    else
                    {
                        tempString.Append(BarCodeData.TextString);
                    }
                    var outputString = tempString.ToString();

                    var stringSizeF = BaseMeasureString(context, outputString, barCodeFont);
					float barsWidth = GetSymbolsStringWidth(BarCodeData.BarsArray);

					if (stringSizeF.Width > barsWidth * BarCodeData.LineWidth + BarCodeData.SpaceLeft + BarCodeData.SpaceRight)
					{
						outputString = BarCodeData.TextString;
					}

//					float binOffsetX = BarCodeData.SpaceLeft;
					float binOffsetX = 0;
					float binOffsetY = BarCodeData.TextPosition;

					using (var sf = new StringFormat())
					{
						sf.Alignment = TextAlignment;
						sf.FormatFlags = 0;
						if (TextAlignment == StringAlignment.Center)
						{
                            BaseDrawString(context, outputString, barCodeFont, foreBrush, 
								new RectangleF(	binOffsetX,
								binOffsetY, 
								barsWidth * BarCodeData.LineWidth + BarCodeData.SpaceLeft + BarCodeData.SpaceRight,
								BarCodeData.TextHeight * 2),
								sf);
						}
						else
						{
                            BaseDrawString(context, BarCodeData.TextString, barCodeFont, foreBrush, 
								new RectangleF(	BarCodeData.SpaceLeft,
								binOffsetY, 
								barsWidth * BarCodeData.LineWidth,
								BarCodeData.TextHeight * 2),
								sf);
						}
					}
				}

                if (drawMethod != null)
                    drawMethod(context, foreBrush);

                RollbackTransform(context);
			}
		}


		protected void CalculateSizeFull(
										float SpaceLeft,
										float SpaceRight,
										float SpaceTop,
										float SpaceBottom,
										float LineHeightShort,
										float LineHeightLong,
										float TextPosition,
										float TextHeight,
										float MainHeight,
										float LineHeightForCut,
										float WideToNarrowRatio,
										float Zoom,
										string Code,
										string TextString,
										string BarsArray,
										RectangleF rect,
										StiBarCode barCode)
		{
			BarCodeData.WideToNarrowRatio = WideToNarrowRatio;
			BarCodeData.Code = Code;
			BarCodeData.TextString = TextString;
			BarCodeData.BarsArray = BarsArray;

            if (!barCode.ShowQuietZones &&
                !(this is StiAustraliaPost4StateBarCodeType || this is StiIntelligentMail4StateBarCodeType || this is StiITF14BarCodeType))
            {
                SpaceLeft = 0;
                SpaceRight = 0;
                SpaceTop = 0;
                SpaceBottom = 0;
            }

			RectWindow = new RectangleF(0, 0, rect.Width, rect.Height);
			if ((barCode.Angle == StiAngle.Angle90) || (barCode.Angle == StiAngle.Angle270))
			{
				RectWindow = new RectangleF(0, 0, rect.Height, rect.Width);
			}

            float fontAddSize = barCode.Font.Size - 8 + 0.5f;
            MainHeight += fontAddSize;
            TextHeight += fontAddSize;

            float fullZoomX = (Module / 10) * Zoom;
			float fullZoomY = fullZoomX;
			float cutHeight = LineHeightForCut * (1f - Height);

			if (barCode.AutoScale)
			{
                if (PreserveAspectRatio)
                {
                    fullZoomX = (float)RectWindow.Width / (GetSymbolsStringWidth(BarsArray) + SpaceLeft + SpaceRight);
                    fullZoomY = (float)RectWindow.Height / MainHeight;
                    fullZoomX = Math.Min(fullZoomX, fullZoomY);
                    fullZoomY = fullZoomX;
                }
                else
                {
                    fullZoomX = (float)RectWindow.Width / (GetSymbolsStringWidth(BarsArray) + SpaceLeft + SpaceRight);
                    cutHeight = -(float)(RectWindow.Height / fullZoomY - MainHeight);
                    if ((!barCode.ShowLabelText) && (TextPosition > LineHeightForCut))
                    {
                        cutHeight -= TextHeight;
                    }
                }
			}

			BarCodeData.FullZoomY = fullZoomY;

			BarCodeData.SpaceLeft = SpaceLeft * fullZoomX;
			BarCodeData.SpaceRight = SpaceRight * fullZoomX;
			BarCodeData.LineWidth = 1f * fullZoomX;
			BarCodeData.MainWidth = (GetSymbolsStringWidth(BarsArray) + SpaceLeft + SpaceRight) * fullZoomX;
			BarCodeData.SpaceTop = SpaceTop * fullZoomY;
			BarCodeData.SpaceBottom = SpaceBottom * fullZoomY;
			BarCodeData.LineHeightShort = (LineHeightShort - cutHeight) * fullZoomY;
			BarCodeData.LineHeightLong = (LineHeightLong - cutHeight) * fullZoomY;
			if (TextPosition > SpaceTop) TextPosition -= cutHeight;
			BarCodeData.TextPosition = TextPosition * fullZoomY;
			BarCodeData.MainHeight = (MainHeight - cutHeight) * fullZoomY;
			BarCodeData.TextHeight = TextHeight * fullZoomY;
		}


		protected void CalculateSize2(float SpaceLeft, float SpaceRight, float SpaceTop, float SpaceBottom,
			float LineHeightShort, float LineHeightLong, float TextPosition, float TextHeight, float MainHeight,
			float WideToNarrowRatio, float Zoom, string BarsArray, RectangleF rect, StiBarCode barCode)
		{
			CalculateSizeFull(SpaceLeft, SpaceRight, SpaceTop, SpaceBottom,
			LineHeightShort, LineHeightLong, TextPosition, TextHeight, MainHeight, LineHeightShort,
			WideToNarrowRatio, Zoom, "", "", BarsArray, rect, barCode);
		}


        protected void Draw2DBarCode(object context, RectangleF rect, StiBarCode barCode, float zoom)
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
                fullZoomX = (float)RectWindow.Width / (barCodeData.MatrixWidth + quietZone * 2);
                fullZoomY = (float)RectWindow.Height / (barCodeData.MatrixHeight * barCodeData.MatrixRatioY + quietZone * 2);
                fullZoomX = Math.Min(fullZoomX, fullZoomY);
                fullZoomY = fullZoomX;
            }

            BarCodeData.MainWidth = (barCodeData.MatrixWidth + quietZone * 2) * fullZoomX;
            BarCodeData.MainHeight = (barCodeData.MatrixHeight * barCodeData.MatrixRatioY + quietZone * 2) * fullZoomY;
            BarCodeData.SpaceLeft = quietZone * fullZoomX;
            BarCodeData.SpaceTop = quietZone * fullZoomY;

            TranslateRect(context, rect, barCode);

            var foreBrush = new StiSolidBrush(barCode.ForeColor);
            var backBrush = new StiSolidBrush(barCode.BackColor);
            
            BaseFillRectangle(context, backBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.MainHeight);

            float binOffsetX = BarCodeData.SpaceLeft;
            float binOffsetY = BarCodeData.SpaceTop;
            for (int indexY = 0; indexY < barCodeData.MatrixHeight; indexY++)
            {
                for (int indexX = 0; indexX < barCodeData.MatrixWidth; indexX++)
                {
                    if (barCodeData.MatrixGrid[indexX + indexY * barCodeData.MatrixWidth] != 0)
                    {
                        BaseFillRectangle2D(context, foreBrush,
                            (binOffsetX + indexX * fullZoomX),
                            (binOffsetY + indexY * fullZoomY * barCodeData.MatrixRatioY),
                            (fullZoomX),
                            (fullZoomY * barCodeData.MatrixRatioY));
                    }
                }
            }

            var qrCode = this as StiQRCodeBarCodeType;
            if ((qrCode != null) && (qrCode.Image != null))
            {
                float imageWidth = qrCode.Image.Width * zoom * (float)qrCode.ImageMultipleFactor;
                float imageHeight = qrCode.Image.Height * zoom * (float)qrCode.ImageMultipleFactor;
                float x = binOffsetX + (barCodeData.MatrixWidth * fullZoomX - imageWidth) / 2;
                float y = binOffsetY + (barCodeData.MatrixHeight * fullZoomY - imageHeight) / 2;

                BaseDrawImage(context, qrCode.Image, barCode.Report, x, y, imageWidth, imageHeight);
            }

            RollbackTransform(context);
        }        

        protected void DrawMaxicode(object context, RectangleF rect, StiBarCode barCode, float zoom)
        {
            RectWindow = new RectangleF(0, 0, rect.Width, rect.Height);
            if ((barCode.Angle == StiAngle.Angle90) || (barCode.Angle == StiAngle.Angle270))
            {
                RectWindow = new RectangleF(0, 0, rect.Height, rect.Width);
            }

            float size_L = 100f;  // 1"
            float size_W = size_L / 29;
            float size_V = size_W * 1.1547f;
            float size_Y = size_W * 0.866f;
            float offset_X = size_W * 1.5f;
            float offset_Y = size_Y + size_V / 2;

            float scale = zoom;
            float fullZoomX = size_W * 32;
            float fullZoomY = size_Y * 34 + size_V;
            if (barCode.AutoScale)
            {
                scale = Math.Min(RectWindow.Height, RectWindow.Width) / fullZoomX;
            }

            BarCodeData.MainWidth = fullZoomX * scale;
            BarCodeData.MainHeight = fullZoomY * scale;

            TranslateRect(context, rect, barCode);

            var foreBrush = new StiSolidBrush(barCode.ForeColor);
            var backBrush = new StiSolidBrush(barCode.BackColor.A < 64 ? Color.White : barCode.BackColor);

            BaseFillRectangle(context, backBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.MainHeight);

            float[] Hex_Offset_X = { 0, size_W/2, size_W/2, 0, -size_W/2, -size_W/2 };
            float[] Hex_Offset_Y = { size_V/2, size_V/4, -size_V/4, -size_V/2, -size_V/4, size_V/4 };
            float Hex_InkSpread = 0.87f;

            for (int row = 0; row < 33; row++)
            {
                for (int col = 0; col < 30; col++)
                {
                    if (barCodeData.MatrixGrid[row * 30 + col] == 1)
                    {
                        float x = offset_X + col * size_W;
                        if ((row & 1) != 0)
                        {
                            x += size_W / 2;
                        }
                        float y = offset_Y + size_Y * row;

                        var points = new PointF[6];
                        for (int i = 0; i < 6; i++)
                        {
                            points[i] = new PointF((x  + Hex_Offset_X[i] * Hex_InkSpread) * scale, (y + Hex_Offset_Y[i] * Hex_InkSpread) * scale);
                        }

                        BaseFillPolygon(context, foreBrush, points);
                    }
                }
            }

            // circles
            float centreX = offset_X + size_W * 14;
            float centreY = offset_Y + size_Y * 16;
            float[] radii = { 15.236f, 12.598f, 9.96f, 7.32f, 4.646f, 2.008f };
            for (int i = 0; i < radii.Length; i++)
            {
                var rectf = new RectangleF((centreX - radii[i]) * scale, (centreY - radii[i]) * scale, radii[i] * 2 * scale, radii[i] * 2 * scale);
                BaseFillEllipse(context, (i & 1) > 0 ? backBrush : foreBrush, rectf.X, rectf.Y, rectf.Width, rectf.Height);
            }

            RollbackTransform(context);
        }

        protected void DrawBarCodeError(object context, RectangleF rect, StiBarCode barCode)
		{
            DrawBarCodeError(context, rect, barCode, null);
		}

        protected void DrawBarCodeError(object context, RectangleF rect, StiBarCode barCode, string message)
		{
            var backBrush = new StiSolidBrush(barCode.BackColor);			
            BaseFillRectangle(context, backBrush, rect.X, rect.Y, rect.Width, rect.Height);
		    BaseDrawRectangle(context, Color.Red, 4, rect.X, rect.Y, rect.Width, rect.Height);

			using (var foreFont = new Font("Arial", 8))
			{
				if (string.IsNullOrEmpty(message))
				{
                    BaseDrawString(context, "Not valid data", foreFont, new StiSolidBrush(Color.Red), new RectangleF(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2), new StringFormat());
				}
				else
				{
                    BaseDrawString(context, message, foreFont, new StiSolidBrush(Color.Red), new RectangleF(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2), new StringFormat());
				}
			}
		}

        public abstract void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom);

		/// <summary>
		/// Translate coordinates using vertical and horizontal justify
		/// </summary>
        protected void TranslateRect(object context, RectangleF rect, StiBarCode barCode)
		{
			float transformAngle;
			float transformX;
			float transformY;
			float transformdX;
			float transformdY;

			switch (barCode.HorAlignment)
			{
				case StiHorAlignment.Right:
					transformdX = (float)RectWindow.Width - BarCodeData.MainWidth;
					break;
				case StiHorAlignment.Center:
					transformdX = ((float)RectWindow.Width - BarCodeData.MainWidth) / 2;
					break;
				default:
					transformdX = 0;
					break;
			}
			switch (barCode.VertAlignment)
			{
				case StiVertAlignment.Bottom:
					transformdY = (float)RectWindow.Height - BarCodeData.MainHeight;
					break;
				case StiVertAlignment.Center:
					transformdY = ((float)RectWindow.Height - BarCodeData.MainHeight) / 2;
					break;
				default:
					transformdY = 0;
					break;
			}

			switch (barCode.Angle)
			{
				case StiAngle.Angle90:
					transformAngle = -90f;
					transformX = (float)(rect.X);
					transformY = (float)(rect.Y + rect.Height);
					break;

				case StiAngle.Angle180:
					transformAngle = -180f;
					transformX = (float)(rect.X + rect.Width);
					transformY = (float)(rect.Y + rect.Height);
					break;

				case StiAngle.Angle270:
					transformAngle = -270f;
					transformX = (float)(rect.X + rect.Width);
					transformY = (float)(rect.Y);
					break;

				default:
					transformAngle = 0f;
					transformX = (float)rect.X;
					transformY = (float)rect.Y;
					break;
			}

            BaseTransform(context, transformX, transformY, transformAngle, transformdX, transformdY);
		}

        protected void RollbackTransform(object context)
        {
            BaseRollbackTransform(context);
        }

        protected void BaseDrawString(object context, string st, Font font, StiBrush brush, float x, float y)
        {
            BaseDrawString(context, st, font, brush, new RectangleF(x, y, 0, 0), null);
        }

        protected virtual void BaseTransform(object context, float x, float y, float angle, float dx, float dy)
        {
            IStiBarCodePainter painter = null;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            } else 
            if (CustomPainter != null)
            {
                painter = CustomPainter;
            }
            else
            {
                painter = StiPainter.GetPainter(typeof(StiBarCode),
                    context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
            }
            painter.BaseTransform(context, x, y, angle, dx, dy); 
        }

        protected virtual void BaseRollbackTransform(object context)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseRollbackTransform(context);
        }

        protected virtual void BaseFillRectangle(object context, StiBrush brush, float x, float y, float width, float height)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseFillRectangle(context, brush, x, y, width, height);
        }

        protected virtual void BaseFillRectangle2D(object context, StiBrush brush, float x, float y, float width, float height)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseFillRectangle2D(context, brush, x, y, width, height);
        }

        protected virtual void BaseFillPolygon(object context, StiBrush brush, PointF[] points)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseFillPolygon(context, brush, points);
        }

        protected virtual void BaseFillPolygons(object context, StiBrush brush, List<List<PointF>> points)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseFillPolygons(context, brush, points);
        }

        protected virtual void BaseFillEllipse(object context, StiBrush brush, float x, float y, float width, float height)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseFillEllipse(context, brush, x, y, width, height);
        }

        protected virtual void BaseDrawRectangle(object context, Color penColor, float penSize, float x, float y, float width, float height)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseDrawRectangle(context, penColor, penSize, x, y, width, height); 
        }

        protected virtual void BaseDrawImage(object context, Image image, StiReport report, float x, float y, float width, float height)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseDrawImage(context, image, report, x, y, width, height);
        }

        protected virtual void BaseDrawString(object context, string st, Font font, StiBrush brush, RectangleF rect, StringFormat sf)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            painter.BaseDrawString(context, st, font, brush, rect, sf); 
        }

        protected virtual SizeF BaseMeasureString(object context, string st, Font font)
        {
            IStiBarCodePainter painter;
            if (context is Stimulsoft.Report.Export.StiBarCodeExportPainter)
            {
                painter = context as Stimulsoft.Report.Export.StiBarCodeExportPainter;
            }
            else
            {
                if (CustomPainter != null)
                {
                    painter = CustomPainter;
                }
                else
                {
                    painter = StiPainter.GetPainter(typeof(StiBarCode),
                        context is Graphics ? StiGuiMode.Gdi : StiGuiMode.Wpf) as IStiBarCodePainter;
                }
            }
            return painter.BaseMeasureString(context, st, font); 
        }

		public override string ToString() => ServiceName;
		#endregion

        #region Methods.virtual
        public virtual StiBarCodeTypeService CreateNew() => throw new NotImplementedException();
        #endregion
	}
}