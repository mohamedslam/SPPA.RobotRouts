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
using System.Text;
using System.Threading;

namespace Stimulsoft.Base
{
	/// <summary>
	/// Helps a converts and works with objects.
	/// </summary>
	public class StiObjectConverter 
	{
		/// <summary>
		/// Convert object to Decimal.
		/// </summary>
		/// <param name="value">Object for converting.</param>
		/// <returns>Converted Decimal value.</returns>
		public static decimal ConvertToDecimal(object value)
		{
			try
			{
			    if (value == null)
			        return 0M;

			    if (value is string)
			    {
			        if (((string) value).Length == 0)
			            return 0M;

			        var str = NormalizeFloatingPointValue(value);

			        decimal decimalResult;
                    if (decimal.TryParse(str, out decimalResult))
                        return decimalResult;

			        double doubleResult;//Try to convert to double, may be this value with exponent
			        if (double.TryParse(str, out doubleResult))
			            return (decimal)doubleResult;
			    }

			    return (decimal) Convert.ChangeType(value, typeof(decimal));
			}
			catch
			{
				return 0;
			}
		}

	    /// <summary>
		/// Convert object to Double.
		/// </summary>
		/// <param name="value">Object for converting.</param>
		/// <returns>Converted Double value.</returns>
		public static double ConvertToDouble(object value)
		{
			try
			{
			    if (value == null)
			        return 0d;

			    if (value is string)
			    {
			        if (string.IsNullOrWhiteSpace((string) value))
			            return 0;

			        var str = NormalizeFloatingPointValue(value);

			        double result;
                    if (double.TryParse(str, out result))
                        return result;
			    }

			    return (double) Convert.ChangeType(value, typeof(double));
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		/// Convert object to Int64.
		/// </summary>
		/// <param name="value">Object for converting.</param>
		/// <returns>Converted Int64 value.</returns>
		public static long ConvertToInt64(object value)
		{
			try
			{
			    if (value == null)
			        return 0;

			    if (value is string)
			    {
			        long result;
                    if (long.TryParse(value as string, out result))
                        return result;
			    }

			    return (long) Convert.ChangeType(value, typeof(long));
			}
			catch
			{
				return 0;
			}
		}		

		/// <summary>
		/// Convert array of bytes to string.
		/// </summary>
        /// <param name="bytes">Array of bytes for converting.</param>
		/// <returns>Converted string.</returns>
		public static string ConvertToString(byte[] bytes)
		{
		    if (bytes == null)
		        return string.Empty;

		    var sb = new StringBuilder();

		    foreach (var b in bytes)
		    {
		        sb.Append(b.ToString("x2"));
		    }

		    return sb.ToString();
		}

        private static string NormalizeFloatingPointValue(object value)
        {
            return ((string)value).Replace(".", ",").Replace(",", Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }
    }
}