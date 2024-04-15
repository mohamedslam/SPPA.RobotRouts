#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
{	                         										}
{																	}
{	Copyright (C) 2003-2022 Stimulsoft   							}
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
{	TRADE SECRETS OF STIMULSOFT										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2022 Stimulsoft

using System;
using System.Text;
using Stimulsoft.Report;

namespace Stimulsoft.Report.Export
{
    public sealed class StiBidirectionalConvert
    {
		#region enum Mode
		public enum Mode
		{
			Pdf, 
			Xps 
		}
		#endregion

		private const int arabicTableSize = 76 + 21;
		private const int ligaturesTableSize = 8 + 32;
		private Mode mode = Mode.Pdf;		
		private static string[,] ligaturesTable = null;
		private static ushort[,] arabicTable = null;
		private static ushort[,] arabicTableArray = null;
        private const string stSeparator = "./:\\,-";

		private bool useStaticArrays = true;

		#region Method Convert()
		public StringBuilder Convert(StringBuilder inputString, bool useRightToLeft)
		{
			StringBuilder outputString = new StringBuilder();
			if (inputString.Length > 0)
			{

				if (useRightToLeft)
				{
					#region RightToLeft
					int index = 0;
					StringBuilder sbTemp = new StringBuilder();

					#region List
					if (char.IsDigit(inputString[0]))
					{
						//collect string
						StringBuilder sbb = new StringBuilder();
						int digIndex = 0;
						while ((digIndex < inputString.Length) &&
                            ((char.IsDigit(inputString[digIndex])) || (stSeparator.IndexOf(inputString[digIndex]) != -1)))
						{
							sbb.Append(inputString[digIndex]);
							digIndex ++;
						}
						//move last dot
						if (inputString[digIndex - 1] == '.')
						{
							sbb.Length --;
							sbb.Insert(0, '.');
						}
						//move white space
						while ((digIndex < inputString.Length) &&
							(((ushort)inputString[digIndex] == 0x0020) || ((ushort)inputString[digIndex] == 0x00a0)))
						{
							digIndex ++;
							outputString.Insert(0, ' ');;
						}
						outputString.Append(sbb);
						index = digIndex;
					}
					#endregion

					while (index < inputString.Length)
					{
						ushort num = (ushort)inputString[index];
						if (SymbolIsArabicOrHebrew(num) || SymbolIsRTLMark(num))
						{
                            if (!SymbolIsRTLMark(num))
                            {
                                //find the beginning of the sentence
                                int sbTempLength = sbTemp.Length;
                                int lastSpacePos = -1;
                                int lastSpaceLen = -1;
                                while ((index > 0) && (sbTempLength > 0) &&
                                    ((((ushort)inputString[index - 1] >= 0x0020) && ((ushort)inputString[index - 1] <= 0x0040)) ||	//basic and numbers
                                    (((ushort)inputString[index - 1] >= 0x005b) && ((ushort)inputString[index - 1] <= 0x0060)) ||	// [ \ ] ^ _ `
                                    (((ushort)inputString[index - 1] >= 0x007b) && ((ushort)inputString[index - 1] <= 0x007e)) ||   // { | } ~
                                    ((ushort)inputString[index - 1] == 0x00a0)))
                                {
                                    index--;
                                    sbTempLength--;
                                    if ((ushort)inputString[index] == 0x0020 || (ushort)inputString[index] == 0x00A0)
                                    {
                                        lastSpacePos = index;
                                        lastSpaceLen = sbTempLength;
                                    }
                                }
                                if (lastSpacePos != -1)
                                {
                                    index = lastSpacePos;
                                    sbTempLength = lastSpaceLen;
                                }
                                sbTemp.Length = sbTempLength;
                            }

							//search end of arabic or hebrew string
                            int startIndex = index;
                            int lastSpace = startIndex;
                            int lastArabicHebrew = startIndex;
                            num = (ushort)inputString[index];
                            while (!SymbolIsLTRMark(num) && (SymbolIsArabicOrHebrew(num) ||
                                ((num >= 0x0020) && (num <= 0x0040)) ||		//basic and numbers
                                ((num >= 0x005b) && (num <= 0x0060)) ||		// [ \ ] ^ _ `
                                ((num >= 0x007b) && (num <= 0x007e)) ||     // { | } ~
                                (num == 0x00a0) ||							//non-breaking space
                                ((num >= 0x2000) && (num <= 0x206F))))		//general punctuation
                            {
                                if (SymbolIsArabicOrHebrew(num)) lastArabicHebrew = index;
                                if (char.IsWhiteSpace((char)num)) lastSpace = index;
                                index++;
                                if (index < inputString.Length)
                                {
                                    num = (ushort)inputString[index];
                                }
                                else
                                {
                                    num = 0;
                                }
                            }

                            //check for last space
                            int endIndex = index;
                            if (lastSpace > lastArabicHebrew) endIndex = lastSpace + 1;

                            //collect all symbols for arabic or hebrew string
							StringBuilder sb = new StringBuilder();
                            for (index = startIndex; index < endIndex; index++)
                            {
                                num = (ushort)inputString[index];
                                if (!SymbolIsBidiMark(num))
                                {
                                    sb.Append(inputString[index]);
                                }
							}
							index--;
							outputString.Insert(0, sbTemp);
							outputString.Insert(0, ConvertArabic(sb));
							sbTemp = new StringBuilder();
						}
						else
						{
                            if (!SymbolIsBidiMark(num))
                            {
                                sbTemp.Append((char)num);
                            }
						}
						index++;
					}
					outputString.Insert(0, sbTemp);

					#region Fast fix for TOC
                    if ((outputString.Length > 0) && (outputString[outputString.Length - 1] == '.'))
                    {
						string st = outputString.ToString();
						int pos = st.Length - 1;
						while ((pos > 0) && (st[pos - 1] == '.')) pos--;
						outputString.Clear();
						outputString.Append(st.Substring(pos));
						outputString.Append(st.Substring(0, pos));
                    }
					#endregion

					#endregion
				}
				else
				{
					#region LeftToRight

					int index = 0;
					while (index < inputString.Length)
					{
						ushort num = (ushort)inputString[index];
						if (SymbolIsArabicOrHebrew(num) && !char.IsDigit((char)num))
						{
							//collect all symbols for arabic or hebrew string
							StringBuilder sb = new StringBuilder();
							while (	SymbolIsArabicOrHebrew(num) ||
								((num >= 0x0020) && (num <= 0x0040)) ||		//basic and numbers
                                ((num >= 0x005b) && (num <= 0x0060)) ||		// [ \ ] ^ _ `
                                ((num >= 0x007b) && (num <= 0x007e)) ||     // { | } ~
                                (num == 0x00a0) ||							//non-breaking space
								((num >= 0x2000) && (num <= 0x206F)))		//general punctuation
							{
                                if (!SymbolIsBidiMark(num))
                                {
                                    sb.Append(inputString[index]);
                                }
								index++;
								if (index < inputString.Length)
								{
									num = (ushort)inputString[index];
								}
								else
								{
									num = 0;
								}
							}
							index--;
							//remove white space
							while (((ushort)inputString[index] == 0x0020) || ((ushort)inputString[index] == 0x00a0))
							{
								//trim right
								index--;
								sb.Length--;
							}
							outputString.Append(ConvertArabic(sb));
						}
						else
						{
                            if (!SymbolIsBidiMark(num))
                            {
                                outputString.Append((char)num);
                            }
						}
						index++;
					}
			
					#endregion
				}

				if (mode == Mode.Pdf)
				{
					if ((StiOptions.Export.Pdf.ConvertDigitsToArabic) && (useRightToLeft))
					{
						return StiExportUtils.ConvertDigitsToArabic(outputString, StiOptions.Export.Pdf.ArabicDigitsType); 
					}
				}
				if (mode == Mode.Xps)
				{
//					if ((StiOptions.Export.Xps.ConvertDigitsToArabic) && (useRightToLeft))
//					{
//						return StiExportUtils.ConvertDigitsToArabic(outputString, StiOptions.Export.Xps.ArabicDigitsType); 
//					}
				}
			}
			return outputString;
		}
		#endregion

		#region Method ConvertArabic()
		private string ConvertArabic(StringBuilder inputSB)
		{
			StringBuilder tempSB = new StringBuilder();

			int index = 0;
			while (index < inputSB.Length)
			{
				ushort tempInt = arabicTableArray[(ushort)inputSB[index], 0];
				switch (tempInt)
				{
					case 3:
					{
						//get sequence
						StringBuilder sb = new StringBuilder();
						sb.Append(inputSB[index]);
						while ((index + 1 < inputSB.Length) &&
							(arabicTableArray[(ushort)inputSB[index + 1], 0] == 3))
						{
							index++;
							sb.Append(inputSB[index]);
						}
						if ((index + 1 < inputSB.Length) &&
							(arabicTableArray[(ushort)inputSB[index + 1], 0] == 2))
						{
							index++;
							sb.Append(inputSB[index]);
						}
						//replace symbols
						if (sb.Length == 1)	
						{
							tempSB.Append((char)arabicTableArray[(ushort)sb[0], 1]);	//isolated
						}
						else
						{
							tempSB.Append((char)arabicTableArray[(ushort)sb[0], 3]);	//initial
							if (sb.Length > 2)
							{
								for (int tempIndex = 1; tempIndex < sb.Length - 1; tempIndex++)
								{
									tempSB.Append((char)arabicTableArray[(ushort)sb[tempIndex], 4]);	//medial
								}
							}
							tempSB.Append((char)arabicTableArray[(ushort)sb[sb.Length - 1], 2]);	//final
						}
					}
						break;
					case 2:
					case 1:
					{
						tempSB.Append((char)arabicTableArray[(ushort)inputSB[index], 1]);	//isolated
					}
						break;
					default:
						tempSB.Append(inputSB[index]);
						break;
				}
				index++;
			}

			StringBuilder tempSB2 = new StringBuilder();
			tempSB2 = tempSB;
			for (int index3 = 0; index3 < ligaturesTableSize; index3++)
			{
				tempSB2 = tempSB2.Replace(ligaturesTable[index3, 0], ligaturesTable[index3, 1]); 
			}

			//reverse string (numbers not reverse)
			StringBuilder outputSB = new StringBuilder();
			int index2 = tempSB2.Length - 1;
			while (index2 >= 0)
			{
				ushort num = (ushort)tempSB2[index2];
				switch (num)
				{
					case '(': num = ')'; break;
					case ')': num = '('; break;
					case '[': num = ']'; break;
					case ']': num = '['; break;
					case '{': num = '}'; break;
					case '}': num = '{'; break;
				}
				if (char.IsDigit((char)num))
				{
					int index5 = index2;
					while ((index5 > 0) && SymbolIsDigitOrDelimiter((ushort)tempSB2[index5 - 1]))
					{
						index5--;
					}
					for (int index4 = index5; index4 <= index2; index4++)
					{
						outputSB.Append(tempSB2[index4]);
					}
					index2 = index5;
				}
				else
				{
					//outputSB.Append(tempSB2[index2]);
					outputSB.Append((char)num);
				}
				index2--;
			}

			return outputSB.ToString();
		}
		#endregion

        #region Method SymbolIsDigitOrDelimiter()
        private bool SymbolIsDigitOrDelimiter(ushort num)
        {
            return (char.IsDigit((char)num)) ||
                (num == (ushort)'.') ||
                (num == (ushort)'/') ||
                (num == (ushort)':') ||
                (num == (ushort)'\\') ||
                (num == (ushort)',') ||
                (num == (ushort)'-');
        }
        #endregion

		#region Method SymbolIsArabicOrHebrew()
		private static bool SymbolIsArabicOrHebrew(ushort num)
		{
			return	((num >= 0x0600) && (num <= 0x06FF)) ||		//arabic
				((num >= 0x0590) && (num <= 0x05ff)) ||		//hebrew
				((num >= 0xfb1d) && (num <= 0xfb4f)) ||		//hebrew - form
				((num >= 0xFB50) && (num <= 0xFDFF)) ||		//arabic - form a
				((num >= 0xFE70) && (num <= 0xFEFF));		//arabic - form b
		}

        public static bool StringContainArabicOrHebrew(string st)
		{
            if (string.IsNullOrWhiteSpace(st)) return false;

		    foreach (char ch in st)
		    {
		        if (SymbolIsArabicOrHebrew(ch)) return true;
		    }
		    return false;
		}
		#endregion

        #region Method SymbolIsBidiMark()
        private bool SymbolIsBidiMark(ushort num)
        {
            return (num == 0x200E) || (num == 0x200F) ||
                ((num >= 0x202A) && (num <= 0x202E));
        }
        private bool SymbolIsLTRMark(ushort num)
        {
            return (num == 0x200E) || (num == 0x202A) || (num == 0x202D);
        }
        private bool SymbolIsRTLMark(ushort num)
        {
            return (num == 0x200F) || (num == 0x202B) || (num == 0x202E);
        }
        #endregion

		#region Method Clear()
		public void Clear()
		{
			if (!useStaticArrays)
			{
				arabicTable = null;
				ligaturesTable = null;
				arabicTableArray = null;
			}
		}
		#endregion

		#region Constructor
		public StiBidirectionalConvert(Mode mode)
		{
			this.mode = mode;

			#region define Arabic tables
			if (arabicTable == null)
			{
				ushort[,] tempArabicTable = new ushort[arabicTableSize, 6]
				{
					// presentation form 
					// code, mode, isolated, final, initial, medial
                    {0x0621,1,0xFE80,0x0000,0x0000,0x0000},
					{0x0622,2,0xFE81,0xFE82,0x0000,0x0000},
					{0x0623,2,0xFE83,0xFE84,0x0000,0x0000},
					{0x0624,2,0xFE85,0xFE86,0x0000,0x0000},
					{0x0625,2,0xFE87,0xFE88,0x0000,0x0000},
					{0x0626,3,0xFE89,0xFE8A,0xFE8B,0xFE8C},
					{0x0627,2,0xFE8D,0xFE8E,0x0000,0x0000},
					{0x0628,3,0xFE8F,0xFE90,0xFE91,0xFE92},
					{0x0629,2,0xFE93,0xFE94,0x0000,0x0000},
					{0x062A,3,0xFE95,0xFE96,0xFE97,0xFE98},
					{0x062B,3,0xFE99,0xFE9A,0xFE9B,0xFE9C},
					{0x062C,3,0xFE9D,0xFE9E,0xFE9F,0xFEA0},
					{0x062D,3,0xFEA1,0xFEA2,0xFEA3,0xFEA4},
					{0x062E,3,0xFEA5,0xFEA6,0xFEA7,0xFEA8},
					{0x062F,2,0xFEA9,0xFEAA,0x0000,0x0000},
					{0x0630,2,0xFEAB,0xFEAC,0x0000,0x0000},
					{0x0631,2,0xFEAD,0xFEAE,0x0000,0x0000},
					{0x0632,2,0xFEAF,0xFEB0,0x0000,0x0000},
					{0x0633,3,0xFEB1,0xFEB2,0xFEB3,0xFEB4},
					{0x0634,3,0xFEB5,0xFEB6,0xFEB7,0xFEB8},
					{0x0635,3,0xFEB9,0xFEBA,0xFEBB,0xFEBC},
					{0x0636,3,0xFEBD,0xFEBE,0xFEBF,0xFEC0},
					{0x0637,3,0xFEC1,0xFEC2,0xFEC3,0xFEC4},
					{0x0638,3,0xFEC5,0xFEC6,0xFEC7,0xFEC8},
					{0x0639,3,0xFEC9,0xFECA,0xFECB,0xFECC},
					{0x063A,3,0xFECD,0xFECE,0xFECF,0xFED0},
					{0x0641,3,0xFED1,0xFED2,0xFED3,0xFED4},
					{0x0642,3,0xFED5,0xFED6,0xFED7,0xFED8},
					{0x0643,3,0xFED9,0xFEDA,0xFEDB,0xFEDC},
					{0x0644,3,0xFEDD,0xFEDE,0xFEDF,0xFEE0},
					{0x0645,3,0xFEE1,0xFEE2,0xFEE3,0xFEE4},
					{0x0646,3,0xFEE5,0xFEE6,0xFEE7,0xFEE8},
					{0x0647,3,0xFEE9,0xFEEA,0xFEEB,0xFEEC},
					{0x0648,2,0xFEED,0xFEEE,0x0000,0x0000},
					{0x0649,3,0xFEEF,0xFEF0,0xFBE8,0xFBE9},
					{0x064A,3,0xFEF1,0xFEF2,0xFEF3,0xFEF4},
					{0x0671,2,0xFB50,0xFB51,0x0000,0x0000},
					{0x0677,2,0xFBDD,0x0677,0x0000,0x0000},
					{0x0679,3,0xFB66,0xFB67,0xFB68,0xFB69},
					{0x067A,3,0xFB5E,0xFB5F,0xFB60,0xFB61},
					{0x067B,3,0xFB52,0xFB53,0xFB54,0xFB55},
					{0x067E,3,0xFB56,0xFB57,0xFB58,0xFB59},
					{0x067F,3,0xFB62,0xFB63,0xFB64,0xFB65},
					{0x0680,3,0xFB5A,0xFB5B,0xFB5C,0xFB5D},
					{0x0683,3,0xFB76,0xFB77,0xFB78,0xFB79},
					{0x0684,3,0xFB72,0xFB73,0xFB74,0xFB75},
					{0x0686,3,0xFB7A,0xFB7B,0xFB7C,0xFB7D},
					{0x0687,3,0xFB7E,0xFB7F,0xFB80,0xFB81},
					{0x0688,2,0xFB88,0xFB89,0x0000,0x0000},
					{0x068C,2,0xFB84,0xFB85,0x0000,0x0000},
					{0x068D,2,0xFB82,0xFB83,0x0000,0x0000},
					{0x068E,2,0xFB86,0xFB87,0x0000,0x0000},
					{0x0691,2,0xFB8C,0xFB8D,0x0000,0x0000},
					{0x0698,2,0xFB8A,0xFB8B,0x0000,0x0000},
					{0x06A4,3,0xFB6A,0xFB6B,0xFB6C,0xFB6D},
					{0x06A6,3,0xFB6E,0xFB6F,0xFB70,0xFB71},
					{0x06A9,3,0xFB8E,0xFB8F,0xFB90,0xFB91},
					{0x06AD,3,0xFBD3,0xFBD4,0xFBD5,0xFBD6},
					{0x06AF,3,0xFB92,0xFB93,0xFB94,0xFB95},
					{0x06B1,3,0xFB9A,0xFB9B,0xFB9C,0xFB9D},
					{0x06B3,3,0xFB96,0xFB97,0xFB98,0xFB99},
					{0x06BA,2,0xFB9E,0xFB9F,0x0000,0x0000},
					{0x06BB,3,0xFBA0,0xFBA1,0xFBA2,0xFBA3},
					{0x06BE,3,0xFBAA,0xFBAB,0xFBAC,0xFBAD},
					{0x06C0,2,0xFBA4,0xFBA5,0x0000,0x0000},
					{0x06C1,3,0xFBA6,0xFBA7,0xFBA8,0xFBA9},
					{0x06C5,2,0xFBE0,0xFBE1,0x0000,0x0000},
					{0x06C6,2,0xFBD9,0xFBDA,0x0000,0x0000},
					{0x06C7,2,0xFBD7,0xFBD8,0x0000,0x0000},
					{0x06C8,2,0xFBDB,0xFBDC,0x0000,0x0000},
					{0x06C9,2,0xFBE2,0xFBE3,0x0000,0x0000},
					{0x06CB,2,0xFBDE,0xFBDF,0x0000,0x0000},
					{0x06CC,3,0xFBFC,0xFBFD,0xFBFE,0xFBFF},
					{0x06D0,3,0xFBE4,0xFBE5,0xFBE6,0xFBE7},
					{0x06D2,2,0xFBAE,0xFBAF,0x0000,0x0000},
					{0x06D3,2,0xFBB0,0xFBB1,0x0000,0x0000},

                    //diacritics
                    {0x064B,3,0x064B,0x064B,0x064B,0x064B},
                    {0x064C,3,0x064C,0x064C,0x064C,0x064C},
                    {0x064D,3,0x064D,0x064D,0x064D,0x064D},
                    {0x064E,3,0x064E,0x064E,0x064E,0x064E},
                    {0x064F,3,0x064F,0x064F,0x064F,0x064F},
                    {0x0650,3,0x0650,0x0650,0x0650,0x0650},
                    {0x0651,3,0x0651,0x0651,0x0651,0x0651},
                    {0x0652,3,0x0652,0x0652,0x0652,0x0652},
                    {0x0653,3,0x0653,0x0653,0x0653,0x0653},
                    {0x0654,3,0x0654,0x0654,0x0654,0x0654},
                    {0x0655,3,0x0655,0x0655,0x0655,0x0655},
                    {0x0656,3,0x0656,0x0656,0x0656,0x0656},
                    {0x0657,3,0x0657,0x0657,0x0657,0x0657},
                    {0x0658,3,0x0658,0x0658,0x0658,0x0658},
                    {0x0659,3,0x0659,0x0659,0x0659,0x0659},
                    {0x065A,3,0x065A,0x065A,0x065A,0x065A},
                    {0x065B,3,0x065B,0x065B,0x065B,0x065B},
                    {0x065C,3,0x065C,0x065C,0x065C,0x065C},
                    {0x065D,3,0x065D,0x065D,0x065D,0x065D},
                    {0x065E,3,0x065E,0x065E,0x065E,0x065E},
                    {0x065F,3,0x065F,0x065F,0x065F,0x065F}
                };
				arabicTable = tempArabicTable;
			}

			if (ligaturesTable == null)
			{
				string[,] tempLigaturesTable = new string[ligaturesTableSize, 2]
				{
					// ligatures Arabic
					{	"\uFEDF\uFE82",	"\uFEF5"	},
					{	"\uFEE0\uFE82",	"\uFEF6"	},
					{	"\uFEDF\uFE84",	"\uFEF7"	},
					{	"\uFEE0\uFE84",	"\uFEF8"	},
					{	"\uFEDF\uFE88",	"\uFEF9"	},
					{	"\uFEE0\uFE88",	"\uFEFA"	},
					{	"\uFEDF\uFE8E",	"\uFEFB"	},
					{	"\uFEE0\uFE8E",	"\uFEFC"	},

					// ligatures Hebrew
					{	"\u05E9\u05C1",	"\uFB2A"	},
					{	"\u05E9\u05C2",	"\uFB2B"	},
					{	"\uFB49\u05C1",	"\uFB2C"	},
					{	"\uFB49\u05C2",	"\uFB2D"	},
					{	"\u05D0\u05B7",	"\uFB2E"	},
					{	"\u05D0\u05B8",	"\uFB2F"	},
					{	"\u05D0\u05BC",	"\uFB30"	},
					{	"\u05D1\u05BC",	"\uFB31"	},
					{	"\u05D2\u05BC",	"\uFB32"	},
					{	"\u05D3\u05BC",	"\uFB33"	},
					{	"\u05D4\u05BC",	"\uFB34"	},
					{	"\u05D5\u05BC",	"\uFB35"	},
					{	"\u05D6\u05BC",	"\uFB36"	},
					{	"\u05D8\u05BC",	"\uFB38"	},
					{	"\u05D9\u05BC",	"\uFB39"	},
					{	"\u05DA\u05BC",	"\uFB3A"	},
					{	"\u05DB\u05BC",	"\uFB3B"	},
					{	"\u05DC\u05BC",	"\uFB3C"	},
					{	"\u05DE\u05BC",	"\uFB3E"	},
					{	"\u05E0\u05BC",	"\uFB40"	},
					{	"\u05E1\u05BC",	"\uFB41"	},
					{	"\u05E3\u05BC",	"\uFB43"	},
					{	"\u05E4\u05BC",	"\uFB44"	},
					{	"\u05E6\u05BC",	"\uFB46"	},
					{	"\u05E7\u05BC",	"\uFB47"	},
					{	"\u05E8\u05BC",	"\uFB48"	},
					{	"\u05E9\u05BC",	"\uFB49"	},
					{	"\u05EA\u05BC",	"\uFB4A"	},
					{	"\u05D5\u05B9",	"\uFB4B"	},
					{	"\u05D1\u05BF",	"\uFB4C"	},
					{	"\u05DB\u05BF",	"\uFB4D"	},
					{	"\u05E4\u05BF",	"\uFB4E"	}
					//{	"\u05D0\u05DC",	"\uFB4F"	}	//unicode standard, but has approximate value
				};
				ligaturesTable = tempLigaturesTable;
			}
			#endregion

			#region prepare Arabic table array
			if (arabicTableArray == null)
			{
				ushort[,] tempArray = new ushort[65536,5];
				for (int index = 0; index < arabicTableSize; index++)
				{
					ushort tempInt = arabicTable[index,0];
					tempArray[tempInt, 0] = arabicTable[index, 1];
					tempArray[tempInt, 1] = arabicTable[index, 2];
					tempArray[tempInt, 2] = arabicTable[index, 3];
					tempArray[tempInt, 3] = arabicTable[index, 4];
					tempArray[tempInt, 4] = arabicTable[index, 5];
				}
				for (ushort index = 0x0590; index <= 0x05ff; index++)
				{
					tempArray[index, 0] = 1;
					tempArray[index, 1] = index;
				}
				for (ushort index = 0xfb1d; index <= 0xfb4f; index++)
				{
					tempArray[index, 0] = 1;
					tempArray[index, 1] = index;
				}
				arabicTableArray = tempArray;
			}
			#endregion
		}
		#endregion
    }    
}
