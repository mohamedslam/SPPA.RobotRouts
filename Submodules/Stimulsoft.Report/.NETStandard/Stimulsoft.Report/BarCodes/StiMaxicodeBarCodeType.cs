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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

#if NETSTANDARD
using UITypeEditor = Stimulsoft.System.Drawing.Design.UITypeEditor;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Maxicode.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiMaxicodeBarCodeTypeConverter))]
	public class StiMaxicodeBarCodeType : StiBarCodeTypeService
	{
        #region class StiMaxicodeException
        protected class StiMaxicodeException : Exception
        {
            public StiMaxicodeException()
            {
            }
            public StiMaxicodeException(string message)
                : base(message)
            {
            }
            public StiMaxicodeException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        #endregion

        #region class StiMaxicode
        protected class StiMaxicode
        {
            #region Constants
            private const char com_RS = (char)30;
            private const char com_GS = (char)29;

            private static readonly string FormatHeader = "[)>" + com_RS + "01" + com_GS;
            private static readonly string FormatTrailer = "" + com_RS + (char)4;

            // MaxiCode module sequence, 30*33
            private static readonly int[] Grid_Map = {
                122, 121, 128, 127, 134, 133, 140, 139, 146, 145, 152, 151, 158, 157, 164, 163, 170, 169, 176, 175, 182, 181, 188, 187, 194, 193, 200, 199, 0,   0,
                124, 123, 130, 129, 136, 135, 142, 141, 148, 147, 154, 153, 160, 159, 166, 165, 172, 171, 178, 177, 184, 183, 190, 189, 196, 195, 202, 201, 817, 0,
                126, 125, 132, 131, 138, 137, 144, 143, 150, 149, 156, 155, 162, 161, 168, 167, 174, 173, 180, 179, 186, 185, 192, 191, 198, 197, 204, 203, 819, 818,
                284, 283, 278, 277, 272, 271, 266, 265, 260, 259, 254, 253, 248, 247, 242, 241, 236, 235, 230, 229, 224, 223, 218, 217, 212, 211, 206, 205, 820, 0,
                286, 285, 280, 279, 274, 273, 268, 267, 262, 261, 256, 255, 250, 249, 244, 243, 238, 237, 232, 231, 226, 225, 220, 219, 214, 213, 208, 207, 822, 821,
                288, 287, 282, 281, 276, 275, 270, 269, 264, 263, 258, 257, 252, 251, 246, 245, 240, 239, 234, 233, 228, 227, 222, 221, 216, 215, 210, 209, 823, 0,
                290, 289, 296, 295, 302, 301, 308, 307, 314, 313, 320, 319, 326, 325, 332, 331, 338, 337, 344, 343, 350, 349, 356, 355, 362, 361, 368, 367, 825, 824,
                292, 291, 298, 297, 304, 303, 310, 309, 316, 315, 322, 321, 328, 327, 334, 333, 340, 339, 346, 345, 352, 351, 358, 357, 364, 363, 370, 369, 826, 0,
                294, 293, 300, 299, 306, 305, 312, 311, 318, 317, 324, 323, 330, 329, 336, 335, 342, 341, 348, 347, 354, 353, 360, 359, 366, 365, 372, 371, 828, 827,
                410, 409, 404, 403, 398, 397, 392, 391, 80,  79,  0,   0,   14,  13,  38,  37,  3,   0,   45,  44,  110, 109, 386, 385, 380, 379, 374, 373, 829, 0,
                412, 411, 406, 405, 400, 399, 394, 393, 82,  81,  41,  0,   16,  15,  40,  39,  4,   0,   0,   46,  112, 111, 388, 387, 382, 381, 376, 375, 831, 830,
                414, 413, 408, 407, 402, 401, 396, 395, 84,  83,  42,  0,   0,   0,   0,   0,   6,   5,   48,  47,  114, 113, 390, 389, 384, 383, 378, 377, 832, 0,
                416, 415, 422, 421, 428, 427, 104, 103, 56,  55,  17,  0,   0,   0,   0,   0,   0,   0,   21,  20,  86,  85,  434, 433, 440, 439, 446, 445, 834, 833,
                418, 417, 424, 423, 430, 429, 106, 105, 58,  57,  0,   0,   0,   0,   0,   0,   0,   0,   23,  22,  88,  87,  436, 435, 442, 441, 448, 447, 835, 0,
                420, 419, 426, 425, 432, 431, 108, 107, 60,  59,  0,   0,   0,   0,   0,   0,   0,   0,   0,   24,  90,  89,  438, 437, 444, 443, 450, 449, 837, 836,
                482, 481, 476, 475, 470, 469, 49,  0,   31,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   1,   54,  53,  464, 463, 458, 457, 452, 451, 838, 0,
                484, 483, 478, 477, 472, 471, 50,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   466, 465, 460, 459, 454, 453, 840, 839,
                486, 485, 480, 479, 474, 473, 52,  51,  32,  0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   2,   0,   43,  468, 467, 462, 461, 456, 455, 841, 0,
                488, 487, 494, 493, 500, 499, 98,  97,  62,  61,  0,   0,   0,   0,   0,   0,   0,   0,   0,   27,  92,  91,  506, 505, 512, 511, 518, 517, 843, 842,
                490, 489, 496, 495, 502, 501, 100, 99,  64,  63,  0,   0,   0,   0,   0,   0,   0,   0,   29,  28,  94,  93,  508, 507, 514, 513, 520, 519, 844, 0,
                492, 491, 498, 497, 504, 503, 102, 101, 66,  65,  18,  0,   0,   0,   0,   0,   0,   0,   19,  30,  96,  95,  510, 509, 516, 515, 522, 521, 846, 845,
                560, 559, 554, 553, 548, 547, 542, 541, 74,  73,  33,  0,   0,   0,   0,   0,   0,   11,  68,  67,  116, 115, 536, 535, 530, 529, 524, 523, 847, 0,
                562, 561, 556, 555, 550, 549, 544, 543, 76,  75,  0,   0,   8,   7,   36,  35,  12,  0,   70,  69,  118, 117, 538, 537, 532, 531, 526, 525, 849, 848,
                564, 563, 558, 557, 552, 551, 546, 545, 78,  77,  0,   34,  10,  9,   26,  25,  0,   0,   72,  71,  120, 119, 540, 539, 534, 533, 528, 527, 850, 0,
                566, 565, 572, 571, 578, 577, 584, 583, 590, 589, 596, 595, 602, 601, 608, 607, 614, 613, 620, 619, 626, 625, 632, 631, 638, 637, 644, 643, 852, 851,
                568, 567, 574, 573, 580, 579, 586, 585, 592, 591, 598, 597, 604, 603, 610, 609, 616, 615, 622, 621, 628, 627, 634, 633, 640, 639, 646, 645, 853, 0,
                570, 569, 576, 575, 582, 581, 588, 587, 594, 593, 600, 599, 606, 605, 612, 611, 618, 617, 624, 623, 630, 629, 636, 635, 642, 641, 648, 647, 855, 854,
                728, 727, 722, 721, 716, 715, 710, 709, 704, 703, 698, 697, 692, 691, 686, 685, 680, 679, 674, 673, 668, 667, 662, 661, 656, 655, 650, 649, 856, 0,
                730, 729, 724, 723, 718, 717, 712, 711, 706, 705, 700, 699, 694, 693, 688, 687, 682, 681, 676, 675, 670, 669, 664, 663, 658, 657, 652, 651, 858, 857,
                732, 731, 726, 725, 720, 719, 714, 713, 708, 707, 702, 701, 696, 695, 690, 689, 684, 683, 678, 677, 672, 671, 666, 665, 660, 659, 654, 653, 859, 0,
                734, 733, 740, 739, 746, 745, 752, 751, 758, 757, 764, 763, 770, 769, 776, 775, 782, 781, 788, 787, 794, 793, 800, 799, 806, 805, 812, 811, 861, 860,
                736, 735, 742, 741, 748, 747, 754, 753, 760, 759, 766, 765, 772, 771, 778, 777, 784, 783, 790, 789, 796, 795, 802, 801, 808, 807, 814, 813, 862, 0,
                738, 737, 744, 743, 750, 749, 756, 755, 762, 761, 768, 767, 774, 773, 780, 779, 786, 785, 792, 791, 798, 797, 804, 803, 810, 809, 816, 815, 864, 863
            };

            // ASCII character to CodeSet mapping
            // 1 = Set A, 2 = Set B, 3 = Set C, 4 = Set D, 5 = Set E, 0 = any
            private static readonly int[] CodeSet_Map = {
                5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 5, 5,
                5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 5,
                0, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2,
                2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4,
                4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
                5, 4, 5, 5, 5, 5, 5, 5, 4, 5, 3, 4, 3, 5, 5, 4,
                4, 3, 3, 3, 4, 3, 5, 4, 4, 3, 3, 4, 3, 3, 3, 4,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
                4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
            };

            // ASCII character to symbol value
            private static readonly byte[] CharToSym_Map = {
                0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 30, 28, 29, 30, 35,
                32, 53, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
                48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 37, 38, 39, 40, 41,
                52, 1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 42, 43, 44, 45, 46,
                0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32, 54, 34, 35, 36,
                48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 47, 48, 49, 50, 51, 52,
                53, 54, 55, 56, 57, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 36,
                37, 37, 38, 39, 40, 41, 42, 43, 38, 44, 37, 39, 38, 45, 46, 40,
                41, 39, 40, 41, 42, 42, 47, 43, 44, 43, 44, 45, 45, 46, 47, 46,
                0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32, 33, 34, 35, 36,
                0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12, 13, 14, 15,
                16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32, 33, 34, 35, 36
            };
            #endregion

            #region Fields
            private StiMaxicodeMode mode;
            private int structuredAppendPosition = 1;
            private int structuredAppendTotal = 1;
            private bool trimExcessData = true;
            private string data_Postcode = null;
            private int data_Country = 0;
            private int data_Service = 0;
            private string content = null;

            private int eciMode = 3;
            private List<byte> codewords = null;
            private byte[] sourceBytes;
            private int[] set = new int[144];
            private byte[] character = new byte[144];
            private bool[,] grid = new bool[33, 30];
            private string errorMessage = null;
            private int offsetEndOfData = 0;
            #endregion

            #region Properties
            public byte[] Matrix => GridToMatrix();

            public string ErrorMessage => errorMessage;
            #endregion

            #region Utils

            protected int GetEciEncoding()
            {
                if (TryCodePage("ISO-8859-1")) return 3;
                if (TryCodePage("ISO-8859-2")) return 4;
                if (TryCodePage("ISO-8859-3")) return 5;
                if (TryCodePage("ISO-8859-4")) return 6;
                if (TryCodePage("ISO-8859-5")) return 7;
                if (TryCodePage("ISO-8859-6")) return 8;
                if (TryCodePage("ISO-8859-7")) return 9;
                if (TryCodePage("ISO-8859-8")) return 10;
                if (TryCodePage("ISO-8859-9")) return 11;
                //if (TryCodePage("ISO-8859-10")) return 12;
                if (TryCodePage("ISO-8859-11")) return 13;
                if (TryCodePage("ISO-8859-13")) return 15;
                //if (TryCodePage("ISO-8859-14")) return 16;
                if (TryCodePage("ISO-8859-15")) return 17;
                //if (TryCodePage("ISO-8859-16")) return 18;
                if (TryCodePage("Windows-1250")) return 21;
                if (TryCodePage("Windows-1251")) return 22;
                if (TryCodePage("Windows-1252")) return 23;
                if (TryCodePage("Windows-1256")) return 24;
                if (TryCodePage("SJIS")) return 20;
                TryCodePage("UTF-8");
                return 26;
            }

            private bool TryCodePage(string charset)
            {
                try
                {
                    var enc = Encoding.GetEncoding(charset);
                    sourceBytes = enc.GetBytes(content);
                    string back = enc.GetString(sourceBytes);
                    if (content == back) return true;
                }
                catch
                {
                }
                return false;
            }

            private string UnpackTilde(string input)
            {
                int index = 0;
                var output = new StringBuilder();
                while (index < input.Length)
                {
                    char ch = input[index++];
                    bool flag = false;
                    if ((ch == '~') && (index + 2 < input.Length))
                    {
                        string stNum = input.Substring(index, 3);
                        int num = 0;
                        if (int.TryParse(stNum, out num))
                        {
                            if (num >= 0 && num < 255)
                            {
                                output.Append((char)num);
                                flag = true;
                                index += 3;
                            }
                        }
                    }
                    if (!flag)
                    {
                        output.Append(ch);
                    }
                }
                return output.ToString();
            }

            private List<string> ExtractPrimaryParts(string input)
            {
                var output = new List<string>();
                int index = 0;
                var sb = new StringBuilder();
                while (index < input.Length)
                {
                    char ch = input[index++];
                    if (ch == com_GS)
                    {
                        output.Add(sb.ToString());
                        if (output.Count == 3)
                        {
                            output.Add(input.Substring(index));
                            sb.Clear();
                            break;
                        }
                        sb = new StringBuilder();
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                if (sb.Length > 0)
                {
                    output.Add(sb.ToString());
                }
                return output;
            }

            private byte[] GridToMatrix()
            {
                var matrix = new byte[33 * 30];
                for (int row = 0; row < 33; row++)
                {
                    for (int col = 0; col < 30; col++)
                    {
                        if (grid[row, col])
                        {
                            matrix[row * 30 + col] = 1;
                        }
                    }
                }
                return matrix;
            }
            #endregion

            #region Class ReedSolomon
            public class ReedSolomon
            {
                private int logmod;
                private int rlen;

                private int[] log = null;
                private int[] alog = null;
                private int[] rspoly = null;

                public ReedSolomon(int poly, int nsym, int index)
                {
                    // Find the range
                    int m = 0;
                    int b = 0;
                    for (b = 1; b <= poly; b <<= 1)
                    {
                        m++;
                    }
                    b >>= 1;
                    m--;

                    // Calculate the log/alog tables
                    logmod = (1 << m) - 1;
                    log = new int[logmod + 1];
                    alog = new int[logmod];

                    int p = 1;
                    for (int v = 0; v < logmod; v++)
                    {
                        alog[v] = p;
                        log[p] = v;
                        p <<= 1;
                        if ((p & b) != 0)
                        {
                            p ^= poly;
                        }
                    }

                    //Calculate rspoly table
                    rlen = nsym;
                    rspoly = new int[nsym + 1];
                    rspoly[0] = 1;
                    for (int i = 1; i <= nsym; i++)
                    {
                        rspoly[i] = 1;
                        for (int k = i - 1; k > 0; k--)
                        {
                            if (rspoly[k] != 0)
                            {
                                rspoly[k] = alog[(log[rspoly[k]] + index) % logmod];
                            }
                            rspoly[k] ^= rspoly[k - 1];
                        }
                        rspoly[0] = alog[(log[rspoly[0]] + index) % logmod];
                        index++;
                    }
                }

                public byte[] Encode(int len, byte[] data)
                {
                    var res = new byte[rlen];
                    for (int i = 0; i < len; i++)
                    {
                        int m = res[rlen - 1] ^ data[i];
                        for (int k = rlen - 1; k > 0; k--)
                        {
                            if ((m != 0) && (rspoly[k] != 0))
                            {
                                res[k] = (byte)(res[k - 1] ^ alog[(log[m] + log[rspoly[k]]) % logmod]);
                            }
                            else
                            {
                                res[k] = res[k - 1];
                            }
                        }
                        if ((m != 0) && (rspoly[0] != 0))
                        {
                            res[0] = (byte)alog[(log[m] + log[rspoly[0]]) % logmod];
                        }
                        else
                        {
                            res[0] = 0;
                        }
                    }
                    return res;
                }
            }
            #endregion

            #region Methods

            // Find the best surrounding set at the specified index by looking at the surrounding sets.
            private int FindBestSet(int index, int length, int set1, int set2, int set3 = -1, int set4 = -1, int set5 = -1)
            {
                // check previous symbol
                int option1 = set[index - 1];
                bool contain1 = option1 == set1 || option1 == set2 || option1 == set3 || option1 == set4 || option1 == set5;
                if (index + 1 < length)
                {
                    // check also next symbol
                    int option2 = set[index + 1];
                    bool contain2 = option2 == set1 || option2 == set2 || option2 == set3 || option2 == set4 || option2 == set5;

                    if (contain1 && contain2)
                    {
                        //characters in lower-numbered sets are more common
                        return Math.Min(option1, option2);
                    }
                    else if (contain1)
                    {
                        return option1;
                    }
                    else if (contain2)
                    {
                        return option2;
                    }
                }
                else
                {
                    // check only previous symbol
                    if (contain1)
                    {
                        return option1;
                    }
                }
                //no good surrounding sets, returned the value of the first valid set
                return set1;
            }

            //Shift content of 'set' and 'character' arrays and insert byte at the position.
            private void InsertSymbol(int position, byte c)
            {
                for (int i = 143; i > position; i--)
                {
                    set[i] = set[i - 1];
                    character[i] = character[i - 1];
                }
                character[position] = c;
            }

            #region ProcessTextToSymbols
            private bool ProcessTextToSymbols()
            {
                int length = sourceBytes.Length;
                int i, j;

                if (length > 138)
                {
                    return false;
                }

                for (int index = 0; index < 144; index++)
                {
                    set[index] = -1;
                    character[index] = 0;
                }

                // Get value and codeset for characters
                for (int index = 0; index < length; index++)
                {
                    set[index] = CodeSet_Map[sourceBytes[index]];
                    character[index] = CharToSym_Map[sourceBytes[index]];
                }

                #region Process set 'any'
                if (set[0] == 0)
                {
                    if (character[0] == 13)
                    {
                        character[0] = 0;
                    }
                    set[0] = 1;
                }

                for (i = 1; i < length; i++)
                {
                    if (set[i] == 0)
                    {
                        /* Special character that can be represented in more than one code set. */
                        if (character[i] == 13)
                        {
                            /* Carriage Return */
                            set[i] = FindBestSet(i, length, 1, 5);
                            if (set[i] == 5)
                            {
                                character[i] = 13;
                            }
                            else
                            {
                                character[i] = 0;
                            }
                        }
                        else if (character[i] == 28)
                        {
                            /* FS */
                            set[i] = FindBestSet(i, length, 1, 2, 3, 4, 5);
                            if (set[i] == 5)
                            {
                                character[i] = 32;
                            }
                        }
                        else if (character[i] == 29)
                        {
                            /* GS */
                            set[i] = FindBestSet(i, length, 1, 2, 3, 4, 5);
                            if (set[i] == 5)
                            {
                                character[i] = 33;
                            }
                        }
                        else if (character[i] == 30)
                        {
                            /* RS */
                            set[i] = FindBestSet(i, length, 1, 2, 3, 4, 5);
                            if (set[i] == 5)
                            {
                                character[i] = 34;
                            }
                        }
                        else if (character[i] == 32)
                        {
                            /* Space */
                            set[i] = FindBestSet(i, length, 1, 2, 3, 4, 5);
                            if (set[i] == 1)
                            {
                                character[i] = 32;
                            }
                            else if (set[i] == 2)
                            {
                                character[i] = 47;
                            }
                            else
                            {
                                character[i] = 59;
                            }
                        }
                        else if (character[i] == 44)
                        {
                            /* Comma */
                            set[i] = FindBestSet(i, length, 1, 2);
                            if (set[i] == 2)
                            {
                                character[i] = 48;
                            }
                        }
                        else if (character[i] == 46)
                        {
                            /* Full Stop */
                            set[i] = FindBestSet(i, length, 1, 2);
                            if (set[i] == 2)
                            {
                                character[i] = 49;
                            }
                        }
                        else if (character[i] == 47)
                        {
                            /* Slash */
                            set[i] = FindBestSet(i, length, 1, 2);
                            if (set[i] == 2)
                            {
                                character[i] = 50;
                            }
                        }
                        else if (character[i] == 58)
                        {
                            /* Colon */
                            set[i] = FindBestSet(i, length, 1, 2);
                            if (set[i] == 2)
                            {
                                character[i] = 51;
                            }
                        }
                    }
                }
                #endregion

                #region Add the padding
                //Check for EOT
                if (set[length - 1] == 5 && character[length - 1] == 4)
                {
                    //Latch B/A
                    set[length] = 0;
                    character[length] = 63;
                    //length++;

                    for (i = length + 1; i < set.Length; i++)
                    {
                        set[i] = 0;
                        character[i] = 33;
                    }
                }
                else
                {
                    for (i = length; i < set.Length; i++)
                    {
                        if (set[length - 1] == 2)
                        {
                            set[i] = 2;
                        }
                        else
                        {
                            set[i] = 1;
                        }
                        character[i] = 33;
                    }
                }
                #endregion

                #region Number compression, part1 - prepare
                if (mode == StiMaxicodeMode.Mode2 || mode == StiMaxicodeMode.Mode3)
                {
                    //not allowed in primary message in modes 2 and 3
                    j = 9;
                }
                else
                {
                    j = 0;
                }
                int count = 0;
                for (i = j; i < 143; i++)
                {
                    if ((set[i] == 1) && (character[i] >= 48 && character[i] <= 57))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }
                    if (count == 9)
                    {
                        // Nine digits in a row can be compressed
                        set[i] = 6;
                        set[i - 1] = 6;
                        set[i - 2] = 6;
                        set[i - 3] = 6;
                        set[i - 4] = 6;
                        set[i - 5] = 6;
                        set[i - 6] = 6;
                        set[i - 7] = 6;
                        set[i - 8] = 6;
                        count = 0;
                    }
                }
                #endregion

                #region Add shift and latch characters
                int current_set = 1;
                i = 0;
                do
                {
                    if ((set[i] != current_set) && (set[i] != 6))
                    {
                        switch (set[i])
                        {
                            case 1:
                                if (i + 1 < set.Length && set[i + 1] == 1)
                                {
                                    if (i + 2 < set.Length && set[i + 2] == 1)
                                    {
                                        if (i + 3 < set.Length && set[i + 3] == 1)
                                        {
                                            /* Latch A */
                                            InsertSymbol(i, 63);
                                            current_set = 1;
                                            length++;
                                            i += 3;
                                        }
                                        else
                                        {
                                            /* 3 Shift A */
                                            InsertSymbol(i, 57);
                                            length++;
                                            i += 2;
                                        }
                                    }
                                    else
                                    {
                                        /* 2 Shift A */
                                        InsertSymbol(i, 56);
                                        length++;
                                        i++;
                                    }
                                }
                                else
                                {
                                    /* Shift A */
                                    InsertSymbol(i, 59);
                                    length++;
                                }
                                break;
                            case 2:
                                if (i + 1 < set.Length && set[i + 1] == 2)
                                {
                                    /* Latch B */
                                    InsertSymbol(i, 63);
                                    current_set = 2;
                                    length++;
                                    i++;
                                }
                                else
                                {
                                    /* Shift B */
                                    InsertSymbol(i, 59);
                                    length++;
                                }
                                break;
                            case 3:
                                if (i + 3 < set.Length && set[i + 1] == 3 && set[i + 2] == 3 && set[i + 3] == 3)
                                {
                                    /* Lock In C */
                                    InsertSymbol(i, 60);
                                    InsertSymbol(i, 60);
                                    current_set = 3;
                                    length++;
                                    i += 3;
                                }
                                else
                                {
                                    /* Shift C */
                                    InsertSymbol(i, 60);
                                    length++;
                                }
                                break;
                            case 4:
                                if (i + 3 < set.Length && set[i + 1] == 4 && set[i + 2] == 4 && set[i + 3] == 4)
                                {
                                    /* Lock In D */
                                    InsertSymbol(i, 61);
                                    InsertSymbol(i, 61);
                                    current_set = 4;
                                    length++;
                                    i += 3;
                                }
                                else
                                {
                                    /* Shift D */
                                    InsertSymbol(i, 61);
                                    length++;
                                }
                                break;
                            case 5:
                                if (i + 3 < set.Length && set[i + 1] == 5 && set[i + 2] == 5 && set[i + 3] == 5)
                                {
                                    /* Lock In E */
                                    InsertSymbol(i, 62);
                                    InsertSymbol(i, 62);
                                    current_set = 5;
                                    length++;
                                    i += 3;
                                }
                                else
                                {
                                    /* Shift E */
                                    InsertSymbol(i, 62);
                                    length++;
                                }
                                break;
                            default:
                                if (set[i] == 0 && (character[i] == 63 || character[i] == 33))  //Latch B/A or Padding
                                {
                                    if (character[i] == 63)
                                    {
                                        if (current_set == 2)
                                            current_set = 1;
                                        else
                                            current_set = 2;
                                    }
                                }
                                else
                                {
                                    throw new StiMaxicodeException("Encoding: unexpected set " + set[i] + " at index " + i + ".");
                                }
                                break;
                        }
                        i++;
                    }
                    i++;
                } while (i < set.Length);
                #endregion

                #region Number compression, part2
                i = 0;
                do
                {
                    if (set[i] == 6)
                    {
                        int value = 0;
                        for (j = 0; j < 9; j++)
                        {
                            value *= 10;
                            value += (character[i + j] - '0');
                        }
                        character[i] = 31; /* NS */
                        character[i + 1] = (byte)((value & 0x3f000000) >> 24);
                        character[i + 2] = (byte)((value & 0xfc0000) >> 18);
                        character[i + 3] = (byte)((value & 0x3f000) >> 12);
                        character[i + 4] = (byte)((value & 0xfc0) >> 6);
                        character[i + 5] = (byte)(value & 0x3f);
                        i += 6;
                        for (j = i; j < 140; j++)
                        {
                            set[j] = set[j + 3];
                            character[j] = character[j + 3];
                        }
                        length -= 3;
                    }
                    else
                    {
                        i++;
                    }
                } while (i < set.Length);
                #endregion

                // Inject ECI codes to beginning of data
                if (eciMode != 3)
                {
                    InsertSymbol(0, 27); // ECI code
                    InsertSymbol(1, (byte)(eciMode & 0x1F));
                    length += 2;
                }

                if (!trimExcessData)
                {
                    // Check maximum data length
                    if ((mode == StiMaxicodeMode.Mode2 || mode == StiMaxicodeMode.Mode3) && length > 84)
                    {
                        return false;
                    }
                    if ((mode == StiMaxicodeMode.Mode4 || mode == StiMaxicodeMode.Mode6) && length > 93)
                    {
                        return false;
                    }
                    if (mode == StiMaxicodeMode.Mode5 && length > 77)
                    {
                        return false;
                    }
                }

                offsetEndOfData = length;
                return true;
            }
            #endregion

            private static byte[] GetErrorCorrectionCodewords(byte[] dataCodewords, int ecclen)
            {
                var rs = new ReedSolomon(0x43, ecclen, 1);
                var res = rs.Encode(dataCodewords.Length, dataCodewords);

                var results = new byte[ecclen];
                for (int i = 0; i < ecclen; i++)
                {
                    results[i] = res[ecclen - 1 - i];
                }

                return results;
            }

            #region GetPrimaryCodewords
            // Extracts the postal code, country code and service code from the primary data and returns the corresponding primary message codewords.
            private byte[] GetPrimaryCodewords()
            {
                if (mode == StiMaxicodeMode.Mode2)
                {
                    return getMode2PrimaryCodewords(data_Postcode, data_Country, data_Service);
                }
                else
                {
                    return getMode3PrimaryCodewords(data_Postcode, data_Country, data_Service);
                }
            }

            private static byte[] getMode2PrimaryCodewords(string postcode, int country, int service)
            {
                int postcodeNum;
                int.TryParse(postcode, out postcodeNum);

                byte[] primary = new byte[10];
                primary[0] = (byte)(((postcodeNum & 0x03) << 4) | 2);
                primary[1] = (byte)((postcodeNum & 0xfc) >> 2);
                primary[2] = (byte)((postcodeNum & 0x3f00) >> 8);
                primary[3] = (byte)((postcodeNum & 0xfc000) >> 14);
                primary[4] = (byte)((postcodeNum & 0x3f00000) >> 20);
                primary[5] = (byte)(((postcodeNum & 0x3c000000) >> 26) | ((postcode.Length & 0x3) << 4));
                primary[6] = (byte)(((postcode.Length & 0x3c) >> 2) | ((country & 0x3) << 4));
                primary[7] = (byte)((country & 0xfc) >> 2);
                primary[8] = (byte)(((country & 0x300) >> 8) | ((service & 0xf) << 2));
                primary[9] = (byte)((service & 0x3f0) >> 4);

                return primary;
            }

            private static byte[] getMode3PrimaryCodewords(string postcode, int country, int service)
            {
                if (postcode.Length < 6)
                {
                    postcode += new string(' ', 6 - postcode.Length);
                }

                var postcodeNums = new int[postcode.Length];

                postcode = postcode.ToUpperInvariant();
                for (int i = 0; i < postcodeNums.Length; i++)
                {
                    postcodeNums[i] = postcode[i];
                    if (postcode[i] >= 'A' && postcode[i] <= 'Z')
                    {
                        // (Capital) letters shifted to Code Set A values
                        postcodeNums[i] -= 64;
                    }
                    if (postcodeNums[i] == 27 || postcodeNums[i] == 31 || postcodeNums[i] == 33 || postcodeNums[i] >= 59)
                    {
                        // Not a valid postal code character, use space instead
                        postcodeNums[i] = 32;
                    }
                    // Input characters lower than 27 in postal code are interpreted as capital letters in Code Set A
                }

                var primary = new byte[10];
                primary[0] = (byte)(((postcodeNums[5] & 0x03) << 4) | 3);
                primary[1] = (byte)(((postcodeNums[4] & 0x03) << 4) | ((postcodeNums[5] & 0x3c) >> 2));
                primary[2] = (byte)(((postcodeNums[3] & 0x03) << 4) | ((postcodeNums[4] & 0x3c) >> 2));
                primary[3] = (byte)(((postcodeNums[2] & 0x03) << 4) | ((postcodeNums[3] & 0x3c) >> 2));
                primary[4] = (byte)(((postcodeNums[1] & 0x03) << 4) | ((postcodeNums[2] & 0x3c) >> 2));
                primary[5] = (byte)(((postcodeNums[0] & 0x03) << 4) | ((postcodeNums[1] & 0x3c) >> 2));
                primary[6] = (byte)(((postcodeNums[0] & 0x3c) >> 2) | ((country & 0x3) << 4));
                primary[7] = (byte)((country & 0xfc) >> 2);
                primary[8] = (byte)(((country & 0x300) >> 8) | ((service & 0xf) << 2));
                primary[9] = (byte)((service & 0x3f0) >> 4);

                return primary;
            }
            #endregion

            #region Encode grid
            private void Encode()
            {
                eciMode = GetEciEncoding();

                // prepare set and character
                if (!ProcessTextToSymbols())
                {
                    throw new StiMaxicodeException("The input data is too long");
                }

                // init codeword array
                codewords = new List<byte>();
                codewords.AddRange(character);

                // insert primary message if this is a structured carrier message; insert mode otherwise
                if (mode == StiMaxicodeMode.Mode2 || mode == StiMaxicodeMode.Mode3)
                {
                    byte[] primaryCodes = GetPrimaryCodewords();
                    codewords.InsertRange(0, primaryCodes);
                    offsetEndOfData += primaryCodes.Length;
                }
                else
                {
                    codewords.Insert(0, (byte)mode);
                    offsetEndOfData++;
                }

                // insert structured append flag if necessary
                if (structuredAppendTotal > 1)
                {

                    var flag = new byte[2];
                    flag[0] = 33; // padding
                    flag[1] = (byte)(((structuredAppendPosition - 1) << 3) | (structuredAppendTotal - 1)); // position + total

                    int index;
                    if (mode == StiMaxicodeMode.Mode2 || mode == StiMaxicodeMode.Mode3)
                    {
                        index = 10; // first two data symbols in the secondary message
                    }
                    else
                    {
                        index = 1; // first two data symbols in the primary message (first symbol at index 0 isn't a data symbol)
                    }

                    codewords.InsertRange(index, flag);
                    offsetEndOfData += flag.Length;
                }

                int secondaryMax, secondaryECMax;
                if (mode == StiMaxicodeMode.Mode5)
                {
                    // 68 data codewords, 56 error corrections in secondary message
                    secondaryMax = 68;
                    secondaryECMax = 56;
                }
                else
                {
                    // 84 data codewords, 40 error corrections in secondary message
                    secondaryMax = 84;
                    secondaryECMax = 40;
                }

                // truncate data codewords to maximum data space available
                int totalMax = secondaryMax + 10;
                if (codewords.Count > totalMax)
                {
                    if ((offsetEndOfData > totalMax) && content.EndsWith("\u0004"))
                    {
                        //move format trailer
                        codewords[totalMax - 1] = codewords[offsetEndOfData - 1];
                        codewords[totalMax - 2] = codewords[offsetEndOfData - 2];
                        if (codewords[offsetEndOfData - 3] == 30)
                        {
                            codewords[totalMax - 3] = codewords[offsetEndOfData - 3];
                            if (codewords[offsetEndOfData - 4] == 29)
                            {
                                codewords[totalMax - 4] = codewords[offsetEndOfData - 4];
                            }
                        }
                    }
                    codewords.RemoveRange(totalMax, codewords.Count - totalMax);
                }

                // insert primary error correction between primary message and secondary message (always EEC)
                var primary = new byte[10];
                Array.Copy(codewords.ToArray(), 0, primary, 0, 10);
                codewords.InsertRange(10, GetErrorCorrectionCodewords(primary, 10));

                // calculate secondary error correction
                var secondary = new byte[codewords.Count - 20];
                Array.Copy(codewords.ToArray(), 20, secondary, 0, codewords.Count - 20);
                var secondaryOdd = new byte[secondary.Length / 2];
                var secondaryEven = new byte[secondary.Length / 2];
                for (int i = 0; i < secondary.Length; i++)
                {
                    if ((i & 1) != 0)
                    {
                        secondaryOdd[(i - 1) / 2] = secondary[i];
                    }
                    else
                    {
                        secondaryEven[i / 2] = secondary[i];
                    }
                }
                var secondaryECOdd = GetErrorCorrectionCodewords(secondaryOdd, secondaryECMax / 2);
                var secondaryECEven = GetErrorCorrectionCodewords(secondaryEven, secondaryECMax / 2);

                // add secondary error correction after secondary message
                for (int i = 0; i < secondaryECEven.Length; i++)
                {
                    codewords.Add(secondaryECEven[i]);
                    codewords.Add(secondaryECOdd[i]);
                }

                // copy data into symbol grid
                var bit_pattern = new int[7];
                for (int i = 0; i < 33; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        int block = (Grid_Map[(i * 30) + j] + 5) / 6;
                        int bit = (Grid_Map[(i * 30) + j] + 5) % 6;

                        if (block != 0)
                        {
                            bit_pattern[0] = (codewords[block - 1] & 0x20) >> 5;
                            bit_pattern[1] = (codewords[block - 1] & 0x10) >> 4;
                            bit_pattern[2] = (codewords[block - 1] & 0x8) >> 3;
                            bit_pattern[3] = (codewords[block - 1] & 0x4) >> 2;
                            bit_pattern[4] = (codewords[block - 1] & 0x2) >> 1;
                            bit_pattern[5] = (codewords[block - 1] & 0x1);

                            if (bit_pattern[bit] != 0)
                            {
                                grid[i, j] = true;
                            }
                            else
                            {
                                grid[i, j] = false;
                            }
                        }
                    }
                }

                // add orientation markings
                grid[0, 28] = true;  // top right filler
                grid[0, 29] = true;
                grid[9, 10] = true;  // top left marker
                grid[9, 11] = true;
                grid[10, 11] = true;
                grid[15, 7] = true;  // left hand marker
                grid[16, 8] = true;
                grid[16, 20] = true; // right hand marker
                grid[17, 20] = true;
                grid[22, 10] = true; // bottom left marker
                grid[23, 10] = true;
                grid[22, 17] = true; // bottom right marker
                grid[23, 17] = true;
            }
            #endregion

            #endregion

            #region this
            public StiMaxicode(string data, StiMaxicodeMode mode, int position, int total, bool processTilde, bool trimExcessData)
            {
                try
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        throw new StiMaxicodeException("Empty content!");
                    }
                    if (position < 1 || position > 8)
                    {
                        throw new StiMaxicodeException("Invalid structured append position: " + position);
                    }
                    if (total < 1 || total > 8)
                    {
                        throw new StiMaxicodeException("Invalid structured append total: " + total);
                    }

                    this.mode = mode;
                    this.structuredAppendPosition = position;
                    this.structuredAppendTotal = total;
                    this.trimExcessData = trimExcessData;

                    var sourceCode = processTilde ? UnpackTilde(data) : data;

                    if (mode == StiMaxicodeMode.Mode2 || mode == StiMaxicodeMode.Mode3)
                    {
                        #region Prepare primary data
                        string messageYear = null;
                        if (sourceCode.StartsWith(FormatHeader))
                        {
                            if (!sourceCode.EndsWith(FormatTrailer))
                            {
                                throw new StiMaxicodeException("Unexpected end of input data");
                            }
                            messageYear = sourceCode.Substring(7, 2);
                            sourceCode = sourceCode.Substring(9, sourceCode.Length - 11);
                        }

                        var parts = ExtractPrimaryParts(sourceCode);
                        if (parts.Count < 4)
                        {
                            throw new StiMaxicodeException("Invalid message format");
                        }

                        data_Postcode = parts[0].Trim();

                        if (!int.TryParse(parts[1].Trim(), out data_Country))
                        {
                            throw new StiMaxicodeException("Invalid country data");
                        }
                        if (!int.TryParse(parts[2].Trim(), out data_Service))
                        {
                            throw new StiMaxicodeException("Invalid service data");
                        }

                        content = parts[3].Trim();
                        if (messageYear != null)
                        {
                            content = FormatHeader + messageYear + content + FormatTrailer;
                        }

                        // if postal code not numeric - change to mode 3
                        if (mode == StiMaxicodeMode.Mode2)
                        {
                            for (int index = 0; index < data_Postcode.Length; index++)
                            {
                                if ((data_Postcode[index] < '0') || (data_Postcode[index] > '9'))
                                {
                                    this.mode = StiMaxicodeMode.Mode3;
                                    break;
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        content = sourceCode;
                    }

                    Encode();
                }
                catch (StiMaxicodeException mce)
                {
                    errorMessage = mce.Message;
                }
            }
            #endregion
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiMaxicodeBarCodeType
            jObject.AddPropertyEnum("Mode", Mode, StiMaxicodeMode.Mode4);
            jObject.AddPropertyInt("StructuredAppendPosition", StructuredAppendPosition, 1);
            jObject.AddPropertyInt("StructuredAppendTotal", StructuredAppendTotal, 1);
            jObject.AddPropertyBool("ProcessTilde", ProcessTilde);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Mode":
                        this.Mode = property.DeserializeEnum<StiMaxicodeMode>();
                        break;

                    case "StructuredAppendPosition":
                        this.StructuredAppendPosition = property.DeserializeInt();
                        break;

                    case "StructuredAppendTotal":
                        this.StructuredAppendTotal = property.DeserializeInt();
                        break;

                    case "ProcessTilde":
                        this.ProcessTilde = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiMaxicodeBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.Mode(),
                propHelper.ProcessTilde(),
                propHelper.TrimExcessData(),
                propHelper.StructuredAppendPosition(),
                propHelper.StructuredAppendTotal()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Maxicode";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "ABC abc 123";

        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
        [DefaultValue(10f)]
        [Browsable(false)]
        public override float Module
        {
            get
            {
                return 10f;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>		
        [Description("Gets os sets height factor of the bar code.")]
        [DefaultValue(1f)]
        [Browsable(false)]
        public override float Height
        {
            get
            {
                return 1f;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the mode of the Maxicode.
        /// </summary>
        [Description("Gets or sets the mode of the Maxicode.")]
		[DefaultValue(StiMaxicodeMode.Mode4)]
        [StiOrder(100)]
        [StiSerializable]
        [StiCategory("BarCode")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiMaxicodeMode Mode { get; set; } = StiMaxicodeMode.Mode4;

        /// <summary>
        /// Gets or sets the flag that indicates whether the data message supports character '~' as the escape.
        /// Escape sequence must have format '~ddd', where number from 0 to 255 (eg. "~000", "~029" etc.)
        /// </summary>
        [Description("Gets or sets the flag that indicates whether the data message supports character '~' as the escape.")]
        [DefaultValue(true)]
        [StiOrder(110)]
        [StiSerializable]
        [StiCategory("BarCode")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool ProcessTilde { get; set; } = true;

        /// <summary>
        /// Gets or sets the structured append Position.
        /// </summary>
        [Description("Gets or sets the structured append Position.")]
        [DefaultValue(1)]
        [StiOrder(140)]
        [StiSerializable]
        [StiCategory("BarCode")]
        public int StructuredAppendPosition { get; set; } = 1;

        /// <summary>
        /// Gets or sets the structured append Total.
        /// </summary>
        [Description("Gets or sets the structured append Total.")]
        [DefaultValue(1)]
        [StiOrder(150)]
        [StiSerializable]
        [StiCategory("BarCode")]
        public int StructuredAppendTotal { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether it is necessary to trim excess data if the data length exceeds the capacity of the barcode.
        /// </summary>
        [Description("Gets or sets a value indicating whether it is necessary to trim excess data if the data length exceeds the capacity of the barcode.")]
        [DefaultValue(true)]
        [StiOrder(120)]
        [StiSerializable]
        [StiCategory("BarCode")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool TrimExcessData { get; set; } = true;


        internal override float LabelFontHeight => DefaultLabelFontHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[9] = true;
                props[12] = true;
                props[13] = true;
                props[21] = true;

                return props;
            }
        }
        #endregion        

        #region Methods

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
            var code = GetCode(barCode);
			BarCodeData.Code = code;

            var mc = new StiMaxicode(code, Mode, StructuredAppendPosition, StructuredAppendTotal, ProcessTilde, TrimExcessData);

			BarCodeData.MatrixGrid = mc.Matrix;

			if (mc.ErrorMessage == null)
			{
                DrawMaxicode(context, rect, barCode, zoom);
			}
			else
			{
                DrawBarCodeError(context, rect, barCode, mc.ErrorMessage);
			}
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiMaxicodeBarCodeType();
        #endregion

		public StiMaxicodeBarCodeType() : this(StiMaxicodeMode.Mode4, 1, 1, true, true)
		{
		}

		public StiMaxicodeBarCodeType(StiMaxicodeMode mode, int structuredAppendPosition, int structuredAppendTotal, bool processTilde) : this(mode, structuredAppendPosition, structuredAppendTotal, processTilde, true)
        {
        }

        public StiMaxicodeBarCodeType(StiMaxicodeMode mode, int structuredAppendPosition, int structuredAppendTotal, bool processTilde, bool trimExcessData)
        {
            this.Mode = mode;
            this.StructuredAppendPosition = structuredAppendPosition;
            this.StructuredAppendTotal = structuredAppendTotal;
            this.ProcessTilde = processTilde;
            this.TrimExcessData = trimExcessData;
        }
    }
}