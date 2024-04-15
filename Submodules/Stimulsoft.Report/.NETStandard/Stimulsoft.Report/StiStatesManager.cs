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
using System.Collections;
using System.Collections.Specialized;

namespace Stimulsoft.Report
{
	/// <summary>
	/// The class describes the manager of the object state.
	/// </summary>
	public sealed class StiStatesManager
	{
        #region IntStorage
		private class IntStorage
		{
			public int value;

			public IntStorage(int value)
			{
				this.value = value;
			}
		}
        #endregion

		#region FloatStorage
		private class FloatStorage
		{
			public float value;

			public FloatStorage(float value)
			{
				this.value = value;
			}
		}
        #endregion

		#region DoubleStorage
		private class DoubleStorage
		{
			public double value;

			public DoubleStorage(double value)
			{
				this.value = value;
			}
		}
        #endregion

		#region Int64Storage
		private class Int64Storage
		{
			public Int64 value;

			public Int64Storage(Int64 value)
			{
				this.value = value;
			}
		}
        #endregion

		#region DecimalStorage
		private class DecimalStorage
		{
			public decimal value;

			public DecimalStorage(decimal value)
			{
				this.value = value;
			}
		}
        #endregion
        
        #region RangeStorage
        private class RangeStorage
        {
            public Type RangeType;
            public object From;
            public object To;

            public RangeStorage(Range range)
            {
                this.RangeType = range.GetType();
                this.From = range.FromObject;
                this.To = range.ToObject;
            }
        }
        #endregion

		#region Fields
		private static object ValueBoolFalse = new object();
		private static object ValueBoolTrue = new object();
		private Hashtable states = new Hashtable();
		#endregion

		#region Methods
		/// <summary>
		///  Saves the specified object state.
		/// </summary>
		public void Push(string stateName, object obj, string property, object value)
		{
			Hashtable objs = states[stateName] as Hashtable;
			if (objs == null)
			{
				objs = new Hashtable();
				states.Add(stateName, objs);
			}

			Hashtable properies = objs[obj] as Hashtable;
			if (properies == null)
			{
				properies = new Hashtable();
				objs.Add(obj, properies);
			}

			properies[property] = value;
		}
		

		/// <summary>
		///  Saves the specified object state.
		/// </summary>
		public void PushBool(string stateName, object obj, string property, bool value)
		{
			if (value)Push(stateName, obj, property, ValueBoolTrue);
			else Push(stateName, obj, property, ValueBoolFalse);
		}

		public void PushInt(string stateName, object obj, string property, int value)
		{
			Push(stateName, obj, property, new IntStorage(value));
		}

		public void PushInt64(string stateName, object obj, string property, Int64 value)
		{
			Push(stateName, obj, property, new Int64Storage(value));
		}

		public void PushFloat(string stateName, object obj, string property, float value)
		{
			Push(stateName, obj, property, new FloatStorage(value));
		}

		public void PushDouble(string stateName, object obj, string property, double value)
		{
			Push(stateName, obj, property, new DoubleStorage(value));
		}

		public void PushDecimal(string stateName, object obj, string property, decimal value)
		{
			Push(stateName, obj, property, new DecimalStorage(value));
		}

        public void PushRange(string stateName, object obj, string property, Range value)
        {
            Push(stateName, obj, property, new RangeStorage(value));
        }

		/// <summary>
		/// Gets the object state.
		/// </summary>
		public object Pop(string stateName, object obj, string property)
		{
			Hashtable objs = states[stateName] as Hashtable;
			if (objs == null)return null;
			Hashtable properties = objs[obj] as Hashtable;
			if (properties == null)return null;
			return properties[property];
		}


		/// <summary>
		/// Gets the object state.
		/// </summary>
		public bool PopBool(string stateName, object obj, string property)
		{
			object value = Pop(stateName, obj, property);
			if (value == ValueBoolFalse) return false;
			return true;
		}


		public int PopInt(string stateName, object obj, string property)
		{
			IntStorage storage = Pop(stateName, obj, property) as IntStorage;
			if (storage == null)return 0;
			return storage.value;
		}

		public Int64 PopInt64(string stateName, object obj, string property)
		{
			Int64Storage storage = Pop(stateName, obj, property) as Int64Storage;
			if (storage == null)return 0;
			return storage.value;
		}

		public double PopDouble(string stateName, object obj, string property)
		{
			DoubleStorage storage = Pop(stateName, obj, property) as DoubleStorage;
			if (storage == null)return 0;
			return storage.value;
		}

		public float PopFloat(string stateName, object obj, string property)
		{
			FloatStorage storage = Pop(stateName, obj, property) as FloatStorage;
			if (storage == null)return 0;
			return storage.value;
		}

		public decimal PopDecimal(string stateName, object obj, string property)
		{
			DecimalStorage storage = Pop(stateName, obj, property) as DecimalStorage;
			if (storage == null)return 0m;
			return storage.value;
		}

        public Range PopRange(string stateName, object obj, string property)
        {
            RangeStorage storage = Pop(stateName, obj, property) as RangeStorage;
            if (storage == null) return null;
            
            Range range = Activator.CreateInstance(storage.RangeType) as Range;
            range.FromObject = storage.From;
            range.ToObject = storage.To;
            return range;
        }

        public bool IsExist(string stateName, object obj)
        {
            if (states[stateName] == null) return false;

            Hashtable objs = states[stateName] as Hashtable;
            if (objs[obj] == null) return false;

            return true;
        }

        public void ClearState(string stateName)
        {
            states.Remove(stateName);
        }

        /// <summary>
        /// Clear all earlier saved object states.
        /// </summary>
        public void Clear()
		{
			states.Clear();
		}
		#endregion
	}
}
