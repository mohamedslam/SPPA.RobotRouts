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
using Stimulsoft.Base.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stimulsoft.Report
{
    #region List
    /// <summary>
    /// Base class for all List classes.
    /// </summary>
    public interface IStiList : IEnumerable
    {
        #region Properties
        /// <summary>
        /// Gets specified name of list. List name equal to name of list class.
        /// </summary>
        string ListName { get; }

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        Type ListType { get; }

        int Count { get; }
        #endregion

        #region Methods
        void AddElement(object value);

        string ToQueryString();

        string ToQueryString(string quotationMark);

        object[] ToObjectArray();

        void Clear();
        #endregion
    }
    #endregion    

    #region BoolList
    public class BoolList : List<bool>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "BoolList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(bool);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((bool)StiConvert.ChangeType(value, typeof(bool)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as BoolList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public BoolList()
        {
        }

        public BoolList(params bool[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region CharList
    public class CharList : List<char>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "CharList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(char);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            if (value is char[])
                Add(' ');
            else
                Add((char)StiConvert.ChangeType(value, typeof(char)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as CharList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public CharList()
        {
        }

        public CharList(params char[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region DateTimeList
    public class DateTimeList : List<DateTime>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "DateTimeList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(DateTime);
        #endregion

        #region Methods
        public bool Contains(DateTime? value)
        {
            return base.Contains(value.GetValueOrDefault());
        }

        public void AddElement(object value)
        {
            this.Add((DateTime)StiConvert.ChangeType(value, typeof(DateTime)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }
        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, string dateTimeFormat)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, dateTimeFormat);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public string ToQueryString(string quotationMark, string dateTimeFormat, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, dateTimeFormat, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as DateTimeList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToDateTime(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public DateTimeList()
        {
        }

        public DateTimeList(params DateTime[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region TimeSpanList
    public class TimeSpanList : List<TimeSpan>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "TimeSpanList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(TimeSpan);
        #endregion

        #region Methods
        public bool Contains(TimeSpan? value)
        {
            return base.Contains(value.GetValueOrDefault());
        }

        public void AddElement(object value)
        {
            this.Add((TimeSpan)StiConvert.ChangeType(value, typeof(TimeSpan)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }
        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, string dateTimeFormat)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, dateTimeFormat);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public string ToQueryString(string quotationMark, string dateTimeFormat, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, dateTimeFormat, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as TimeSpanList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToTimeSpan(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public TimeSpanList()
        {
        }

        public TimeSpanList(params TimeSpan[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region DecimalList
    public class DecimalList : List<decimal>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "DecimalList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(decimal);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((decimal)StiConvert.ChangeType(value, typeof(decimal)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as DecimalList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(long value)
        {
            return base.Contains(StiValueHelper.TryToDecimal(value));
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToDecimal(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public DecimalList()
        {
        }

        public DecimalList(params decimal[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region FloatList
    public class FloatList : List<float>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "FloatList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(float);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((float)StiConvert.ChangeType(value, typeof(float)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as FloatList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(long value)
        {
            return base.Contains(StiValueHelper.TryToFloat(value));
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToFloat(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public FloatList()
        {
        }

        public FloatList(params float[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region DoubleList
    public class DoubleList : List<double>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "DoubleList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(double);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((double)StiConvert.ChangeType(value, typeof(double)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as DoubleList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(long value)
        {
            return base.Contains(StiValueHelper.TryToDouble(value));
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToDouble(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public DoubleList()
        {
        }

        public DoubleList(params double[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region ByteList
    public class ByteList : List<byte>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "ByteList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(byte);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((byte)StiConvert.ChangeType(value, typeof(byte)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as ByteList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(decimal value)
        {
            return base.Contains((byte)StiValueHelper.TryToInt(value));
        }

        public bool Contains(string value)
        {
            return base.Contains((byte)StiValueHelper.TryToInt(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public ByteList()
        {
        }

        public ByteList(params byte[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region ShortList
    public class ShortList : List<short>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "ShortList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(short);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((short)StiConvert.ChangeType(value, typeof(short)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as ShortList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(decimal value)
        {
            return base.Contains((short)StiValueHelper.TryToLong(value));
        }

        public bool Contains(string value)
        {
            return base.Contains((short)StiValueHelper.TryToLong(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public ShortList()
        {
        }

        public ShortList(params short[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region IntList
    public class IntList : List<int>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "IntList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(int);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((int)StiConvert.ChangeType(value, typeof(int)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as IntList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(decimal value)
        {
            return base.Contains(StiValueHelper.TryToInt(value));
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToInt(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public IntList()
        {
        }

        public IntList(params int[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region LongList
    public class LongList : List<long>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "LongList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(long);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((long)StiConvert.ChangeType(value, typeof(long)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as LongList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(decimal value)
        {
            return base.Contains(StiValueHelper.TryToLong(value));
        }

        public bool Contains(string value)
        {
            return base.Contains(StiValueHelper.TryToLong(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public LongList()
        {
        }

        public LongList(params long[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region GuidList
    public class GuidList : List<Guid>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "GuidList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(Guid);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((Guid)StiConvert.ChangeType(value, typeof(Guid)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as GuidList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public GuidList()
        {
        }

        public GuidList(params Guid[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion

    #region StringList
    public class StringList : List<string>, IStiList
    {
        #region Properties
        /// <summary>
        /// Gets specified name of List. List name equal to name of List class.
        /// </summary>
        public string ListName => "StringList";

        /// <summary>
        /// Gets the type of List items. 
        /// </summary>
        public Type ListType => typeof(string);
        #endregion

        #region Methods
        public void AddElement(object value)
        {
            this.Add((string)StiConvert.ChangeType(value, typeof(string)));
        }

        public override string ToString()
        {
            return ToQueryString();
        }

        public string ToQueryString()
        {
            return Func.EngineHelper.ToQueryString(this, null, null);
        }

        public string ToQueryString(string quotationMark)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null);
        }

        public string ToQueryString(string quotationMark, bool needEscape)
        {
            return Func.EngineHelper.ToQueryString(this, quotationMark, null, needEscape);
        }

        public object[] ToObjectArray()
        {
            return this.Cast<object>().ToArray();
        }

        public override bool Equals(object obj)
        {
            var list2 = obj as StringList;
            if (list2 == null || this.Count != list2.Count) 
                return false;

            for (var index = 0; index < this.Count; index++)
            {
                if (Comparer.Default.Compare(this[index], list2[index]) != 0) 
                    return false;
            }

            return true;
        }

        public bool Contains(long value)
        {
            return base.Contains(StiValueHelper.TryToString(value));
        }

        public bool Contains(double value)
        {
            return base.Contains(StiValueHelper.TryToString(value));
        }

        public bool Contains(decimal value)
        {
            return base.Contains(StiValueHelper.TryToString(value));
        }

        public override int GetHashCode() => base.GetHashCode();
        #endregion

        public StringList()
        {
        }

        public StringList(params string[] values)
        {
            if (values == null) return;

            foreach (var value in values)
                Add(value);
        }
    }
    #endregion
}
