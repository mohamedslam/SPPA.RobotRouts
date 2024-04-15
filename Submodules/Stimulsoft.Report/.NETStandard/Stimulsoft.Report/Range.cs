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

using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Design;
using System;
using System.Collections;
using System.ComponentModel;

namespace Stimulsoft.Report
{
    #region Range
    /// <summary>
    /// Base class for all Range classes.
    /// </summary>
    [TypeConverter(typeof(RangeConverter))]
    [StiSerializable]
    public abstract class Range
    {
        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public abstract string RangeName { get; }

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public abstract Type RangeType { get; }

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public abstract object FromObject { get; set; }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public abstract object ToObject { get; set; }

        public string FromStrLoc => Loc.Get("PropertyMain", "RangeFrom");

        public string ToStrLoc => Loc.Get("PropertyMain", "RangeTo");
        #endregion

        #region Methods
        public static bool IsRangeType(Type type)
        {
            return typeof(Range).IsAssignableFrom(type);
        }

        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public abstract void Parse(string from, string to);

        public override bool Equals(object obj)
        {
            var range2 = obj as Range;
            if (range2 == null) 
                return false;
            
            return 
                range2 != null &&
                Comparer.Default.Compare(this.FromObject, range2.FromObject) == 0 && 
                Comparer.Default.Compare(this.ToObject, range2.ToObject) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
    #endregion    

    #region CharRange
    public class CharRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as CharRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public char From { get; set; } = 'A';
        public char To { get; set; } = 'Z';
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "CharRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(char);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is char)
                    From = (char)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is char)
                    To = (char)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            if (!string.IsNullOrEmpty(from))
                this.FromObject = from[0];

            if (!string.IsNullOrEmpty(to))
                this.ToObject = to[0];
        }

        public bool Contains(char value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} '{From}' {ToStrLoc.ToLowerInvariant()} '{To}'";
        }
        #endregion

        public CharRange()
        {
        }

        public CharRange(char from, char to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region DateTimeRange
    public class DateTimeRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as DateTimeRange;
            if (range == null || From == null || To == null)
                return -1;

            return (FromDate.Ticks + ToDate.Ticks).CompareTo(range.FromDate.Ticks + range.ToDate.Ticks);
        }
        #endregion

        #region Fields
        public DateTime? From = null;
        public DateTime? To = null;
        #endregion

        #region Properties
        public DateTime FromDate => From == null ? DateTime.MinValue : From.Value;

        public DateTime ToDate => To == null ? DateTime.MaxValue : To.Value;

        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "DateTimeRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(DateTime);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is DateTime)
                    From = (DateTime)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is DateTime)
                    To = (DateTime)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = null;
                else
                    this.FromObject = DateTime.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = null;
                else
                    this.ToObject = DateTime.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(DateTime value)
        {
            if (From == null && To == null)
                return true;

            if (From == null && To != null)
                return To.Value >= value;

            if (From != null && To == null)
                return From.Value <= value;

            return From.Value <= value && To.Value >= value;
        }

        public bool Contains(DateTime? value)
        {
            if (From == null && To == null)
                return true;

            if (value == null)
                return false;

            if (From == null && To != null)
                return To.Value >= value.Value;

            if (From != null && To == null)
                return From.Value <= value.Value;

            return From.Value <= value.Value && To.Value >= value.Value;
        }

        public override string ToString()
        {
            var strFrom = From != null ? string.Format("{0:d}", From) : "-";
            var strTo = To != null ? string.Format("{0:d}", To) : "-";

            return $"{FromStrLoc} {strFrom} {ToStrLoc.ToLowerInvariant()} {strTo}";
        }
        #endregion

        public DateTimeRange()
        {
        }

        public DateTimeRange(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        public DateTimeRange(DateTime? from, DateTime? to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region TimeSpanRange
    public class TimeSpanRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as TimeSpanRange;
            if (range == null || From == null || To == null)
                return -1;

            return (FromTime.Ticks + ToTime.Ticks).CompareTo(range.FromTime.Ticks + range.ToTime.Ticks);
        }
        #endregion

        #region Fields
        public TimeSpan? From = null;
        public TimeSpan? To = null;
        #endregion

        #region Properties
        public TimeSpan FromTime => From == null ? TimeSpan.MinValue : From.Value;

        public TimeSpan ToTime => To == null ? TimeSpan.MaxValue : To.Value;

        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "TimeSpanRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(TimeSpan);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is TimeSpan)
                    From = (TimeSpan)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is TimeSpan)
                    To = (TimeSpan)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = null;
                else
                    this.FromObject = TimeSpan.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = null;
                else
                    this.ToObject = TimeSpan.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(TimeSpan value)
        {
            if (From == null && To == null)
                return true;

            if (From == null && To != null)
                return To.Value >= value;

            if (From != null && To == null)
                return From.Value <= value;

            return From.Value <= value && To.Value >= value;
        }

        public bool Contains(TimeSpan? value)
        {
            if (From == null && To == null)
                return true;

            if (value == null)
                return false;

            if (From == null && To != null)
                return To.Value >= value.Value;

            if (From != null && To == null)
                return From.Value <= value.Value;

            return From.Value <= value.Value && To.Value >= value.Value;
        }

        public override string ToString()
        {
            var strFrom = From != null ? string.Format("{0:t}", From) : "-";
            var strTo = To != null ? string.Format("{0:t}", To) : "-";

            return $"{FromStrLoc} {strFrom} {ToStrLoc.ToLowerInvariant()} {strTo}";
        }
        #endregion

        public TimeSpanRange()
        {
        }

        public TimeSpanRange(TimeSpan from, TimeSpan to)
        {
            this.From = from;
            this.To = to;
        }

        public TimeSpanRange(TimeSpan? from, TimeSpan? to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region DecimalRange
    public class DecimalRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as DecimalRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public decimal From = 0m;
        public decimal To = 0m;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "DecimalRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(decimal);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is decimal)
                    From = (decimal)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is decimal)
                    To = (decimal)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0m;
                else
                    this.FromObject = decimal.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0m;
                else
                    this.ToObject = decimal.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(decimal value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public DecimalRange()
        {
        }

        public DecimalRange(decimal from, decimal to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region FloatRange
    public class FloatRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as FloatRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public float From = 0f;
        public float To = 0f;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "FloatRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(float);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is float)
                    From = (float)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is float)
                    To = (float)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0f;
                else
                    this.FromObject = float.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0f;
                else
                    this.ToObject = float.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(float value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public FloatRange()
        {
        }

        public FloatRange(float from, float to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region DoubleRange
    public class DoubleRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as DoubleRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public double From = 0d;
        public double To = 0d;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "DoubleRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(double);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is double)
                    From = (double)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is double)
                    To = (double)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0d;
                else
                    this.FromObject = double.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0d;
                else
                    this.ToObject = double.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(double value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public DoubleRange()
        {
        }

        public DoubleRange(double from, double to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region ByteRange
    public class ByteRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as ByteRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public byte From = 0;
        public byte To = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "ByteRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(byte);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is byte)
                    From = (byte)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is byte)
                    To = (byte)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0;
                else
                    this.FromObject = byte.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0;
                else
                    this.ToObject = byte.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(byte value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public ByteRange()
        {
        }

        public ByteRange(byte from, byte to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region ShortRange
    public class ShortRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as ShortRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public short From = 0;
        public short To = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "ShortRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(short);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is short)
                    From = (short)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is short)
                    To = (short)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0;
                else
                    this.FromObject = short.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0;
                else
                    this.ToObject = short.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(short value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public ShortRange()
        {
        }

        public ShortRange(short from, short to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion
    
    #region IntRange
    public class IntRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as IntRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public int From = 0;
        public int To = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "IntRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(int);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is int)
                    From = (int)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is int)
                    To = (int)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0;
                else
                    this.FromObject = int.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0;
                else
                    this.ToObject = int.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(int value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public IntRange()
        {
        }

        public IntRange(int from, int to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region LongRange
    public class LongRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as LongRange;
            if (range == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public long From = 0;
        public long To = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "LongRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(long);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is long)
                    From = (long)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is long)
                    To = (long)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(from.Trim()))
                    this.FromObject = 0;
                else
                    this.FromObject = long.Parse(from);

                if (string.IsNullOrEmpty(to.Trim()))
                    this.ToObject = 0;
                else
                    this.ToObject = long.Parse(to);
            }
            catch
            {
            }
        }

        public bool Contains(long value)
        {
            return From <= value && To >= value;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public LongRange()
        {
        }

        public LongRange(long from, long to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region GuidRange
    public class GuidRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as GuidRange;
            if (range == null || From == null || To == null)
                return -1;

            return (From.ToString() + To.ToString()).CompareTo(range.From.ToString() + range.To.ToString());
        }
        #endregion

        #region Fields
        public Guid From = Guid.Empty;
        public Guid To = Guid.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "GuidRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(Guid);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is Guid)
                    From = (Guid)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is Guid)
                    To = (Guid)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            try
            {
                this.FromObject = new Guid(from);
                this.ToObject = new Guid(to);
            }
            catch
            {
            }
        }

        public bool Contains(Guid value)
        {
            return From.CompareTo(value) <= 0 && To.CompareTo(value) >= 0;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public GuidRange()
        {
        }

        public GuidRange(Guid from, Guid to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion

    #region StringRange
    public class StringRange : Range, IComparable
    {
        #region IComparable
        int IComparable.CompareTo(object obj)
        {
            var range = obj as StringRange;
            if (range == null || From == null || To == null)
                return -1;

            return (From + To).CompareTo(range.From + range.To);
        }
        #endregion

        #region Fields
        public string From = string.Empty;
        public string To = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets specified name of range. Range name equal to name of range class.
        /// </summary>
        public override string RangeName => "StringRange";

        /// <summary>
        /// Gets the type of range items. 
        /// </summary>
        public override Type RangeType => typeof(string);

        /// <summary>
        /// Gets or sets From item of range.
        /// </summary>
        public override object FromObject
        {
            get
            {
                return From;
            }
            set
            {
                if (value is string)
                    From = (string)value;
            }
        }

        /// <summary>
        /// Gets or set To item of range.
        /// </summary>
        public override object ToObject
        {
            get
            {
                return To;
            }
            set
            {
                if (value is string)
                    To = (string)value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Fill From and To item of range with it string representation.
        /// </summary>
        public override void Parse(string from, string to)
        {
            this.FromObject = from;
            this.ToObject = to;
        }

        public bool Contains(string value)
        {
            if (From == null && To == null)
                return true;

            if (value == null)
                return true;

            if (From == null && To != null)
                return To.CompareTo(value) >= 0;

            if (From != null && To == null)
                return From.CompareTo(value) <= 0;

            return From.CompareTo(value) <= 0 && To.CompareTo(value) >= 0;
        }

        public override string ToString()
        {
            return $"{FromStrLoc} {From} {ToStrLoc.ToLowerInvariant()} {To}";
        }
        #endregion

        public StringRange()
        {
        }

        public StringRange(string from, string to)
        {
            this.From = from;
            this.To = to;
        }
    }
    #endregion
}
