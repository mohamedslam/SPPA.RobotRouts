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


using System.Text;

namespace Stimulsoft.Data.Functions
{
    public partial class Funcs
    {
        #region Fields
        private static int[] Arabics = 
            { 1, 5, 10, 50, 100, 1000 };

        private static char[] Romans = 
            { 'I', 'V', 'X', 'L', 'C', 'M' };

        private static int[] Subs = 
            { 0, 0, 0, 2, 2, 4 };

        private static char[] ABC = 
            { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
              'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static char[] ABCRu = 
            { 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'К', 'Л', 'М', 'Н',
              'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Э',
              'Ю', 'Я' };
        #endregion

        #region Methods
        /// <summary>
        /// Converts Arabic numerals to Roman.
        /// </summary>
        /// <param name="value">Arabic numerals for converting to the Roman format.</param>
        /// <returns>Returns Arabics numerals in Roman.</returns>
        public static string ToRoman(int value)
        {
            var str = new StringBuilder();

            while (value > 0)
            {
                for (int i = 5; i >= 0; i--)
                {
                    if (value >= Arabics[i])
                    {
                        str.Append(Romans[i]);
                        value -= Arabics[i];
                        break;
                    }

                    var flag = false;
                    for (var j = Subs[i]; j < i; j++)
                    {
                        if (Arabics[j] == Arabics[i] - Arabics[j]) continue;
                        
                        if (value >= Arabics[i] - Arabics[j])
                        {
                            str.Append(Romans[j]);
                            str.Append(Romans[i]);
                            value -= Arabics[i] - Arabics[j];
                            flag = true;
                            break;
                        }
                    }
                    if (flag) break;
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// Converts the number to A B C D representation for numbering of the list.
        /// </summary>
        /// <param name="value">A number for converting into the A B C representation.</param>
        /// <returns>String representation of the value in A B C D format.</returns>
        public static string ToABC(int value)
        {
            if (value < 1) 
                return string.Empty;

            int count = 0;
            while (value > 26)
            {
                count++;
                value -= 26;
            }
            return new string(ABC[value - 1], count + 1);
        }

        /// <summary>
        /// Converts the number to A B C D representation for numbering of the list.
        /// </summary>
        /// <param name="value">A number for converting into the A B C representation.</param>
        /// <returns>String representation of the value in A B C D format.</returns>
        public static string ToABCNumeric(int value)
        {
            int count = 0;
            while (value > 26)
            {
                count++;
                value -= 26;
            }

            if (count == 0)
                return ((char)(value + 64)).ToString();
            else
                return ((char)(value + 64)).ToString() + count.ToString();
        }


        /// <summary>
        /// Converts the number to А Б В representation for numbering of the list in russian.
        /// </summary>
        /// <param name="value">A number for converting into the А Б В representation.</param>
        /// <returns>String representation of the value in А Б В format.</returns>
        public static string ToABCRu(int value)
        {
            if (value < 1) 
                return string.Empty;

            var count = 0;
            while (value > 26)
            {
                count++;
                value -= 26;
            }

            return new string(ABCRu[value - 1], count + 1);
        }

        /// <summary>
        /// Converts the number to the arabic representation.
        /// </summary>
        /// <param name="value">A number for converting into the arabic representation.</param>
        /// <param name="useEasternDigits">Use eastern or standard arabic digits.</param>
        /// <returns>String representation of the value with arabic digits.</returns>
        public static string ToArabic(int value, bool useEasternDigits)
        {
            return ToArabic(value.ToString(), useEasternDigits);
        }

        /// <summary>
        /// Converts all digits in the string to the arabic representation.
        /// </summary>
        /// <param name="value">A string for converting into the arabic representation.</param>
        /// <param name="useEasternDigits">Use eastern or standard arabic digits.</param>
        /// <returns>String with arabic digits.</returns>
        public static string ToArabic(string value, bool useEasternDigits)
        {
            var sb = new StringBuilder();
            for (var index = 0; index < value.Length; index++)
            {
                var num = (int)value[index];
                if ((num >= 0x0030) && (num <= 0x0039))
                {
                    num += 0x0660 - 0x0030;
                    if (useEasternDigits)
                        num += 0x06f0 - 0x0660;
                }

                sb.Append((char)num);
            }

            return sb.ToString();
        }
        #endregion
    }
}