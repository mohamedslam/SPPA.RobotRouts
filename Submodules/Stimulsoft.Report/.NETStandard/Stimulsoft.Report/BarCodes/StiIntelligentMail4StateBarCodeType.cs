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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - IntelligentMail4State.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiBarCodeTypeServiceConverter))]
	public class StiIntelligentMail4StateBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, 20f);
            jObject.AddPropertyFloat("Height", Height, 1f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Module":
                        this.module = property.DeserializeFloat();
                        break;

                    case "Height":
                        this.height = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiIntelligentMail4StateBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.Module()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Intelligent Mail 4-State";
        #endregion

        #region IntelligentMail4State
        public sealed class IntelligentMail4State
        {
            #region Static fields
            private static Int32 table2Of13Size = 78;
            private static Int32 table5Of13Size = 1287;
            private static Int32 entries2Of13;
            private static Int32 entries5Of13;
            private static Int32[] table2Of13;
            private static Int32[] table5Of13;
            private static Int32[][] codewordArray;
            private static Int32[] barTopCharIndexArray = new Int32[] { 4, 0, 2, 6, 3, 5, 1, 9, 8, 7, 1, 2, 0, 6, 4, 8, 2, 9, 5, 3, 0, 1, 3, 7, 4, 6, 8, 9, 2, 0, 5, 1, 9, 4, 3, 8, 6, 7, 1, 2, 4, 3, 9, 5, 7, 8, 3, 0, 2, 1, 4, 0, 9, 1, 7, 0, 2, 4, 6, 3, 7, 1, 9, 5, 8 };
            private static Int32[] barBottomCharIndexArray = new Int32[] { 7, 1, 9, 5, 8, 0, 2, 4, 6, 3, 5, 8, 9, 7, 3, 0, 6, 1, 7, 4, 6, 8, 9, 2, 5, 1, 7, 5, 4, 3, 8, 7, 6, 0, 2, 5, 4, 9, 3, 0, 1, 6, 8, 2, 0, 4, 5, 9, 6, 7, 5, 2, 6, 3, 8, 5, 1, 9, 8, 7, 4, 0, 2, 6, 3 };
            private static Int32[] barTopCharShiftArray = new Int32[] { 3, 0, 8, 11, 1, 12, 8, 11, 10, 6, 4, 12, 2, 7, 9, 6, 7, 9, 2, 8, 4, 0, 12, 7, 10, 9, 0, 7, 10, 5, 7, 9, 6, 8, 2, 12, 1, 4, 2, 0, 1, 5, 4, 6, 12, 1, 0, 9, 4, 7, 5, 10, 2, 6, 9, 11, 2, 12, 6, 7, 5, 11, 0, 3, 2 };
            private static Int32[] barBottomCharShiftArray = new Int32[] { 2, 10, 12, 5, 9, 1, 5, 4, 3, 9, 11, 5, 10, 1, 6, 3, 4, 1, 10, 0, 2, 11, 8, 6, 1, 12, 3, 8, 6, 4, 4, 11, 0, 6, 1, 9, 11, 5, 3, 7, 3, 10, 7, 11, 8, 2, 10, 3, 5, 8, 0, 3, 12, 11, 8, 4, 5, 1, 3, 0, 7, 12, 9, 8, 10 };
            #endregion

            #region Encode
            public static string Encode(string source, ref string bars)
            {
                if (string.IsNullOrEmpty(source)) return "No input data.";
                if (source.Length < 20) return "Invalid tracking code length.";
                if (source.Length > 31) return "Input data too long.";
                if (!Regex.IsMatch(source.Substring(0, 2), "[0-9][0-4]")) return "Invalid 'Barcode Identifier'";

                var zip = source.Substring(20);
                if (zip.Length != 11 && zip.Length != 9 && zip.Length != 5 && zip.Length != 0) return "Invalid ZIP code.";

                Int32 fcs = 0;
                Int64 l = 0;
                Int64 v = 0;
                var ds = String.Empty;
                Int32[] byteArray = new Int32[14];
                Int32[] ai = new Int32[66];
                Int32[] ai1 = new Int32[66];
                int[][] ad = new int[11][];
                if (zip.Length > 0)
                {
                    l = Int64.Parse(zip, CultureInfo.InvariantCulture) + ((zip.Length == 5) ? 1 : ((zip.Length == 9) ? 100001 : (zip.Length == 11 ? 1000100001 : 0)));
                }
                v = l * 10 + Int32.Parse(source.Substring(0, 1), CultureInfo.InvariantCulture);
                v = v * 5 + Int32.Parse(source.Substring(1, 1), CultureInfo.InvariantCulture);
                ds = v.ToString(CultureInfo.InvariantCulture) + source.Substring(2, 18);
                byteArray[12] = (Int32)(l & 255);
                byteArray[11] = (Int32)(l >> 8 & 255);
                byteArray[10] = (Int32)(l >> 16 & 255);
                byteArray[9] = (Int32)(l >> 24 & 255);
                byteArray[8] = (Int32)(l >> 32 & 255);
                MathMultiply(ref byteArray, 13, 10);
                MathAdd(ref byteArray, 13, Int32.Parse(source.Substring(0, 1), CultureInfo.InvariantCulture));
                MathMultiply(ref byteArray, 13, 5);
                MathAdd(ref byteArray, 13, Int32.Parse(source.Substring(1, 1), CultureInfo.InvariantCulture));
                for (Int16 i = 2; i <= 19; i++)
                {
                    MathMultiply(ref byteArray, 13, 10);
                    MathAdd(ref byteArray, 13, Int32.Parse(source.Substring(i, 1), CultureInfo.InvariantCulture));
                }
                fcs = MathFcs(byteArray);
                for (Int16 i = 0; i <= 9; i++)
                {
                    codewordArray[i][0] = entries2Of13 + entries5Of13;
                    codewordArray[i][1] = 0;
                }
                codewordArray[0][0] = 659;
                codewordArray[9][0] = 636;
                MathDivide(ds);
                codewordArray[9][1] *= 2;
                if (fcs >> 10 != 0) codewordArray[0][1] += 659;
                for (Int16 i = 0; i <= 9; i++) ad[i] = new int[3];
                for (Int16 i = 0; i <= 9; i++)
                {
                    if (codewordArray[i][1] >= (Decimal)(entries2Of13 + entries5Of13)) return "Something went wrong. Calculation error!";
                    ad[i][0] = 8192;
                    ad[i][1] = (codewordArray[i][1] >= (Decimal)entries2Of13) ? ad[i][1] = table2Of13[(Int32)(codewordArray[i][1] - entries2Of13)] : ad[i][1] = table5Of13[(Int32)codewordArray[i][1]];
                }
                for (Int16 i = 0; i <= 9; i++) if ((fcs & 1 << i) != 0) ad[i][1] = ~(Int32)ad[i][1] & 8191;
                for (Int16 i = 0; i <= 64; i++)
                {
                    ai[i] = (Int32)ad[barTopCharIndexArray[i]][1] >> barTopCharShiftArray[i] & 1;
                    ai1[i] = (Int32)ad[barBottomCharIndexArray[i]][1] >> barBottomCharShiftArray[i] & 1;
                }
                string encoded = string.Empty;
                for (int i = 0; i <= 64; i++)
                {
                    //if (ai[i] == 0) encoded += (ai1[i] == 0) ? "T" : "D";
                    //else encoded += (ai1[i] == 0) ? "A" : "F";

                    if (ai[i] == 0)
                        encoded += (ai1[i] == 0) ? "f" : "e";   // T D
                    else
                        encoded += (ai1[i] == 0) ? "d" : "c";   // A F

                    encoded += "1"; //space * WideToNarrowRatio
                }
                bars = encoded;
                return null;
            }
            #endregion

            #region BigIntMath
            private static Boolean MathAdd(ref Int32[] bytearray, Int32 i, Int32 j)
            {
                if (bytearray == null) return false;
                if (i < 1) return false;
                Int32 x = (bytearray[i - 1] | (bytearray[i - 2] << 8)) + j;
                Int32 l = x | 65535;
                Int32 k = i - 3;
                bytearray[i - 1] = x & 255;
                bytearray[i - 2] = x >> 8 & 255;
                while (l == 1 && k > 0)
                {
                    x = l + bytearray[k];
                    bytearray[k] = x & 255;
                    l = x | 255;
                    k -= 1;
                }
                return true;
            }

            private static Boolean MathDivide(String v)
            {
                Int32 j = 10;
                String n = v;
                for (Int32 k = j - 1; k >= 1; k += -1)
                {
                    String r = string.Empty;
                    Int32 divider = (Int32)codewordArray[k][0];
                    String copy = n;
                    String left = "0";
                    Int32 l = copy.Length;
                    for (Int16 i = 1; i <= l; i++)
                    {
                        Int32 divident = Int32.Parse(copy.Substring(0, i), CultureInfo.InvariantCulture);
                        while (divident < divider & i < l - 1)
                        {
                            r = r + "0";
                            i += 1;
                            divident = Int32.Parse(copy.Substring(0, i), CultureInfo.InvariantCulture);
                        }
                        r = r + (divident / divider).ToString(CultureInfo.InvariantCulture);
                        left = (divident % divider).ToString(CultureInfo.InvariantCulture).PadLeft(i, '0');
                        copy = left + copy.Substring(i);
                    }
                    n = r.TrimStart('0');
                    if (String.IsNullOrEmpty(n)) n = "0";
                    codewordArray[k][1] = Int32.Parse(left, CultureInfo.InvariantCulture);
                    if (k == 1) codewordArray[0][1] = Int32.Parse(r, CultureInfo.InvariantCulture);
                }
                return true;
            }

            private static Int32 MathFcs(Int32[] bytearray)
            {
                Int32 c = 3893;
                Int32 i = 2047;
                Int32 j = bytearray[0] << 5;
                for (Int16 b = 2; b <= 7; b++)
                {
                    if (((i ^ j) & 1024) != 0) i = i << 1 ^ c;
                    else i <<= 1;
                    i = i & 2047;
                    j <<= 1;
                }
                for (Int32 l = 1; l <= 12; l++)
                {
                    Int32 k = bytearray[l] << 3;
                    for (Int16 b = 0; b <= 7; b++)
                    {
                        if (((i ^ k) & 1024) != 0) i = i << 1 ^ c;
                        else i <<= 1;
                        i = i & 2047;
                        k <<= 1;
                    }
                }
                return i;
            }

            private static Boolean MathMultiply(ref Int32[] bytearray, Int32 i, Int32 j)
            {
                if (bytearray == null) return false;
                if (i < 1) return false;
                Int32 l = 0;
                Int32 k = 0;
                for (k = i - 1; k >= 1; k += -2)
                {
                    Int32 x = (bytearray[k] | (bytearray[k - 1] << 8)) * j + l;
                    bytearray[k] = x & 255;
                    bytearray[k - 1] = x >> 8 & 255;
                    l = x >> 16;
                }
                if (k == 0) bytearray[0] = (bytearray[0] * j + l) & 255;
                return true;
            }

            private static Int32 MathReverse(Int32 i)
            {
                Int32 j = 0;
                for (Int16 k = 0; k <= 15; k++)
                {
                    j <<= 1;
                    j = j | i & 1;
                    i >>= 1;
                }
                return j;
            }
            #endregion

            #region Utils
            private static Boolean InitializeNof13Table(Int32[] ai, Int32 i, Int32 j)
            {
                Int32 i1 = 0;
                Int32 j1 = j - 1;
                for (Int16 k = 0; k <= 8191; k++)
                {
                    Int32 k1 = 0;
                    for (Int32 l1 = 0; l1 <= 12; l1++) if ((k & 1 << l1) != 0) k1 += 1;
                    if (k1 == i)
                    {
                        Int32 l = MathReverse(k) >> 3;
                        Boolean flag = k == l;
                        if (l >= k)
                        {
                            if (flag)
                            {
                                ai[j1] = k;
                                j1 -= 1;
                            }
                            else
                            {
                                ai[i1] = k;
                                i1 += 1;
                                ai[i1] = l;
                                i1 += 1;
                            }
                        }
                    }
                }
                return i1 == j1 + 1;
            }
            #endregion

            static IntelligentMail4State()
            {
                #region Init static arrays
                table2Of13 = new Int32[table2Of13Size + 1];
                InitializeNof13Table(table2Of13, 2, table2Of13Size);
                entries5Of13 = table2Of13Size;
                table5Of13 = new Int32[table5Of13Size + 1];
                InitializeNof13Table(table5Of13, 5, table5Of13Size);
                entries2Of13 = table5Of13Size;

                codewordArray = new Int32[11][];
                for (Int16 i = 0; i <= 9; i++) codewordArray[i] = new Int32[3];
                #endregion
            }
        }
        #endregion

        #region Properties
        public override string DefaultCodeValue => "01 234 567094 987654321 01234 5678 91";

        private float module = 20f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
		[DefaultValue(20f)]
        [Description("Gets or sets width of the most fine element of the bar code.")]
        [StiCategory("BarCode")]
        public override float Module
		{
			get
			{
				return module;
			}
			set
			{
				module = value;
				if (value < 20f)	module = 20f;
				if (value > 20f)	module = 20f;
			}
		}

		private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>
		[DefaultValue(1f)]
		[Browsable(false)]
        [Description("Gets os sets height factor of the bar code.")]
        [StiCategory("BarCode")]
        public override float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				if (value < 1f)	height = 1f;
				if (value > 1f)	height = 1f;
			}
		}

        internal override float LabelFontHeight => IntelligentMail4StateTextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[13] = true;

                return props;
            }
        }
		#endregion

        #region Consts
        protected const float IntelligentMail4StateSpaceLeft = 7f;
		protected const float IntelligentMail4StateSpaceRight = 7f;
		protected const float IntelligentMail4StateSpaceTop = 1.5f + 7f;
		protected const float IntelligentMail4StateSpaceBottom = 1.5f;
		protected const float IntelligentMail4StateLineHeightShort = IntelligentMail4StateLineHeightLong * 0.655f;
		protected const float IntelligentMail4StateLineHeightLong = 7.25f;
		protected const float IntelligentMail4StateTextPosition = 1f;
		protected const float IntelligentMail4StateTextHeight = 5.5f;
		protected const float IntelligentMail4StateMainHeight = IntelligentMail4StateSpaceTop + IntelligentMail4StateLineHeightLong + IntelligentMail4StateSpaceBottom;
		protected const float IntelligentMail4StateLineHeightForCut = IntelligentMail4StateLineHeightLong;
        protected const float IntelligentMail4StateWideToNarrowRatio = 1.25f;

        protected override StringAlignment TextAlignment { get { return StringAlignment.Near; } }

        protected override bool PreserveAspectRatio => true;
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
            string text = CheckCodeSymbols(code, "0123456789 ");
            code = CheckCodeSymbols(code, "0123456789");

            string barsArray = null;
            string errorString = IntelligentMail4State.Encode(code, ref barsArray);

			if (!string.IsNullOrEmpty(barsArray))
			{
				CalculateSizeFull(
					IntelligentMail4StateSpaceLeft,
					IntelligentMail4StateSpaceRight,
					IntelligentMail4StateSpaceTop,
					IntelligentMail4StateSpaceBottom,
					IntelligentMail4StateLineHeightShort,
					IntelligentMail4StateLineHeightLong,
					IntelligentMail4StateTextPosition,
					IntelligentMail4StateTextHeight,
					IntelligentMail4StateMainHeight,
					IntelligentMail4StateLineHeightForCut,
                    IntelligentMail4StateWideToNarrowRatio,
					zoom,
					code,
					text,
					barsArray,
					rect,
					barCode);

                DrawBarCode(context, rect, barCode); 
			}
			else
			{
				if (!string.IsNullOrEmpty(errorString))
				{
                    DrawBarCodeError(context, rect, barCode, errorString);
				}
				else
				{
                    DrawBarCodeError(context, rect, barCode);
				}
			}
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiIntelligentMail4StateBarCodeType();
        #endregion

		public StiIntelligentMail4StateBarCodeType() : this(20f, 1f)
		{
		}

		public StiIntelligentMail4StateBarCodeType(float module, float height)
		{
			this.module = module;
			this.height = height;
		}
	}
}