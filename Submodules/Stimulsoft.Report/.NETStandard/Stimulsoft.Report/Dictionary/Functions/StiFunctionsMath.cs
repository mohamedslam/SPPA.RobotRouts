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

namespace Stimulsoft.Report.Dictionary
{
    public class StiFunctionsMath
    {
        #region Maximum
        public static decimal Maximum(decimal value1, decimal value2)
        {
            return Math.Max(value1, value2);
        }

		public static decimal? Maximum(decimal? value1, decimal? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Max(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;
			
			return null;
		}

        public static double Maximum(double value1, double value2)
        {
            return Math.Max(value1, value2);
        }

		public static double? Maximum(double? value1, double? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Max(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;

			return null;
		}

        public static long Maximum(long value1, long value2)
        {
            return Math.Max(value1, value2);
        }

		public static long? Maximum(long? value1, long? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Max(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;

			return null;
		}
        #endregion

        #region Minimum
        public static decimal Minimum(decimal value1, decimal value2)
        {
            return Math.Min(value1, value2);
        }

		public static decimal? Minimum(decimal? value1, decimal? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Min(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;

			return null;
		}

        public static double Minimum(double value1, double value2)
        {
            return Math.Min(value1, value2);
        }

		public static double? Minimum(double? value1, double? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Min(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;

			return null;
		}

        public static long Minimum(long value1, long value2)
        {
            return Math.Min(value1, value2);
        }

		public static long? Minimum(long? value1, long? value2)
		{
			if (value1.HasValue && value2.HasValue)
				return Math.Min(value1.Value, value2.Value);

			if (value1.HasValue) return value1;
			if (value2.HasValue) return value2;

			return null;
		}
        #endregion

        #region Div
        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        public static long Div(long value1, long value2)
        {
            return Div(value1, value2, 0);
        }


        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        public static long Div(long value1, long value2, long zeroResult)
        {
            if (value2 == 0) return zeroResult;
            return value1 / value2;
        }


        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        public static double Div(double value1, double value2)
        {
            return Div(value1, value2, 0);
        }


        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        public static double Div(double value1, double value2, double zeroResult)
        {
            if (value2 == 0) return zeroResult;
            return value1 / value2;
        }


        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
        /// </summary>
        public static decimal Div(decimal value1, decimal value2)
        {
            return Div(value1, value2, 0);
        }


        /// <summary>
        /// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
        /// </summary>
        public static decimal Div(decimal value1, decimal value2, decimal zeroResult)
        {
            if (value2 == 0) return zeroResult;
            return value1 / value2;
        }




		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
		/// </summary>
		public static long? Div(long? value1, long? value2)
		{
			return Div(value1, value2, 0);
		}


		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
		/// </summary>
		public static long? Div(long? value1, long? value2, long? zeroResult)
		{
			if (!(value1.HasValue && value2.HasValue))
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			if (value2 == 0)
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			return value1 / value2;
		}


		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
		/// </summary>
		public static double? Div(double? value1, double? value2)
		{
			return Div(value1, value2, 0);
		}


		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
		/// </summary>
		public static double? Div(double? value1, double? value2, double? zeroResult)
		{
			if (!(value1.HasValue && value2.HasValue))
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			if (value2 == 0)
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			return value1 / value2;
		}


		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result equal to 0.
		/// </summary>
		public static decimal? Div(decimal? value1, decimal? value2)
		{
			return Div(value1, value2, 0);
		}


		/// <summary>
		/// Returns result of dividing value1 and value2. If value2 is zero, then result is zeroResult (third argument).
		/// </summary>
		public static decimal? Div(decimal? value1, decimal? value2, decimal? zeroResult)
		{
			if (!(value1.HasValue && value2.HasValue))
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			if (value2 == 0)
			{
				if (zeroResult.HasValue) return zeroResult;
				else return null;
			}
			return value1 / value2;
		}
        #endregion

        #region Round
        /// <summary>
        /// Returns result of rounding.
        /// </summary>
        public static double Round(double value)
        {
            return Math.Round(value, StiOptions.Engine.MidpointRounding);
        }
        public static decimal Round(decimal value)
        {
            return Math.Round(value, StiOptions.Engine.MidpointRounding);
        }
        public static double Round(double value, int decimals)
        {
            return Math.Round(value, decimals, StiOptions.Engine.MidpointRounding);
        }
        public static decimal Round(decimal value, int decimals)
        {
            return Math.Round(value, decimals, StiOptions.Engine.MidpointRounding);
        }

        public static double Round(double value, MidpointRounding midpointRounding)
        {
            return Math.Round(value, midpointRounding);
        }
        public static decimal Round(decimal value, MidpointRounding midpointRounding)
        {
            return Math.Round(value, midpointRounding);
        }
        public static double Round(double value, int decimals, MidpointRounding midpointRounding)
        {
            return Math.Round(value, decimals, midpointRounding);
        }
        public static decimal Round(decimal value, int decimals, MidpointRounding midpointRounding)
        {
            return Math.Round(value, decimals, midpointRounding);
        }

        public static double? Round(double? value)
        {
            if (value.HasValue)
                return Math.Round(value.Value, StiOptions.Engine.MidpointRounding);
            else 
                return null;
        }
        public static decimal? Round(decimal? value)
        {
            if (value.HasValue)
                return Math.Round(value.Value, StiOptions.Engine.MidpointRounding);
            else
                return null;
        }
        public static double? Round(double? value, int decimals)
        {
            if (value.HasValue)
                return Math.Round(value.Value, decimals, StiOptions.Engine.MidpointRounding);
            else
                return null;
        }
        public static decimal? Round(decimal? value, int decimals)
        {
            if (value.HasValue)
                return Math.Round(value.Value, decimals, StiOptions.Engine.MidpointRounding);
            else
                return null;
        }

        public static double? Round(double? value, MidpointRounding midpointRounding)
        {
            if (value.HasValue)
                return Math.Round(value.Value, midpointRounding);
            else
                return null;
        }
        public static decimal? Round(decimal? value, MidpointRounding midpointRounding)
        {
            if (value.HasValue)
                return Math.Round(value.Value, midpointRounding);
            else
                return null;
        }
        public static double? Round(double? value, int decimals, MidpointRounding midpointRounding)
        {
            if (value.HasValue)
                return Math.Round(value.Value, decimals, midpointRounding);
            else
                return null;
        }
        public static decimal? Round(decimal? value, int decimals, MidpointRounding midpointRounding)
        {
            if (value.HasValue)
                return Math.Round(value.Value, decimals, midpointRounding);
            else
                return null;
        }
        #endregion
    }
}
