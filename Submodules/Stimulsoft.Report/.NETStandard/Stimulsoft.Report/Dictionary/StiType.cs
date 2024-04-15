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
using Stimulsoft.Base;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.Dictionary
{
	/// <summary>
	/// Class describes wrapper for type registered in dictionary data.
	/// </summary>
	public class StiType
	{
        #region Properties
        /// <summary>
        /// Gets or sets name of type.
        /// </summary>
        public string Name { get; set; }

	    /// <summary>
		/// Gets or sets the registered type.
		/// </summary>
		public Type Type { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the collection of all registered types in dictionary data.
        /// </summary>
        /// <returns>Collection of types.</returns>
        public static StiTypesCollection GetTypes()
		{
			var retTypes = new StiTypesCollection();

			foreach (var type in StiTypeWrapper.SimpleTypes)
			{
				retTypes.Add(new StiType(StiTypeConverter.ToString(type), type));
			}

			return retTypes;
		}

        /// <summary>
        /// Returns the collection of all base registered types in dictionary data.
        /// </summary>
        /// <returns>Collection of types.</returns>
        public static StiTypesCollection GetBaseTypes()
        {
            var retTypes = new StiTypesCollection();

            foreach (var type in StiTypeWrapper.SimpleBaseTypes)
            {
                retTypes.Add(new StiType(StiTypeConverter.ToString(type), type));
            }

            return retTypes;
        }

        public static Type GetTypeModeFromType(Type type, out StiTypeMode typeMode)
        {
            #region Single Value
            typeMode = StiTypeMode.Value;

            if (type == null) return type;
            if (type == typeof(string)) return type;
            if (type == typeof(bool)) return type;
            if (type == typeof(byte)) return type;
            if (type == typeof(sbyte)) return type;
            if (type == typeof(ushort)) return type;
            if (type == typeof(short)) return type;
            if (type == typeof(uint)) return type;
            if (type == typeof(int)) return type;
            if (type == typeof(ulong)) return type;
            if (type == typeof(long)) return type;
            if (type == typeof(float)) return type;
            if (type == typeof(double)) return type;
            if (type == typeof(decimal)) return type;
            if (type == typeof(char)) return type;
            if (type == typeof(TimeSpan)) return type;
            if (type == typeof(DateTime)) return type;
            if (type == typeof(DateTimeOffset)) return type;
            if (type == typeof(Guid)) return type;
            if (type == typeof(Image)) return type;
            if (type == typeof(object)) return type;
            #endregion

            #region Single Nullable Value
            typeMode = StiTypeMode.NullableValue;

            if (type == typeof(bool?)) return typeof(bool);
            if (type == typeof(byte?)) return typeof(byte);
            if (type == typeof(sbyte?)) return typeof(sbyte);
            if (type == typeof(ushort?)) return typeof(ushort);
            if (type == typeof(short?)) return typeof(short);
            if (type == typeof(uint?)) return typeof(uint);
            if (type == typeof(int?)) return typeof(int);
            if (type == typeof(ulong?)) return typeof(ulong);
            if (type == typeof(long?)) return typeof(long);
            if (type == typeof(float?)) return typeof(float);
            if (type == typeof(double?)) return typeof(double);
            if (type == typeof(decimal?)) return typeof(decimal);
            if (type == typeof(char?)) return typeof(char);
            if (type == typeof(TimeSpan?)) return typeof(TimeSpan);
            if (type == typeof(DateTime?)) return typeof(DateTime);
            if (type == typeof(DateTimeOffset?)) return typeof(DateTimeOffset);
            if (type == typeof(Guid?)) return typeof(Guid);
            #endregion

            #region List of Values
            typeMode = StiTypeMode.List;

            if (type == typeof(StringList)) return typeof(string);
            if (type == typeof(BoolList)) return typeof(bool);
            if (type == typeof(ByteList)) return typeof(byte);
            if (type == typeof(ShortList)) return typeof(short);
            if (type == typeof(IntList)) return typeof(int);
            if (type == typeof(LongList)) return typeof(long);
            if (type == typeof(FloatList)) return typeof(float);
            if (type == typeof(DoubleList)) return typeof(double);
            if (type == typeof(DecimalList)) return typeof(decimal);
            if (type == typeof(CharList)) return typeof(char);
            if (type == typeof(TimeSpanList)) return typeof(TimeSpan);
            if (type == typeof(DateTimeList)) return typeof(DateTime);
            if (type == typeof(GuidList)) return typeof(Guid);
            #endregion

            #region Range of Values
            typeMode = StiTypeMode.Range;

            if (type == typeof(StringRange)) return typeof(string);
            if (type == typeof(ByteRange)) return typeof(byte);
            if (type == typeof(ShortRange)) return typeof(short);
            if (type == typeof(IntRange)) return typeof(int);
            if (type == typeof(LongRange)) return typeof(long);
            if (type == typeof(FloatRange)) return typeof(float);
            if (type == typeof(DoubleRange)) return typeof(double);
            if (type == typeof(DecimalRange)) return typeof(decimal);
            if (type == typeof(CharRange)) return typeof(char);
            if (type == typeof(TimeSpanRange)) return typeof(TimeSpan);
            if (type == typeof(DateTimeRange)) return typeof(DateTime);
            if (type == typeof(GuidRange)) return typeof(Guid);
            #endregion

            typeMode = StiTypeMode.Value;
            return type;
        }

        public static Type GetTypeFromTypeMode(Type type, StiTypeMode typeMode)
        {
            if (type == null)
                return null;

            switch (typeMode)
            {
                #region StiTypeMode.Value
                case StiTypeMode.Value:
                    return type;
                #endregion

                #region StiTypeMode.NullableValue
                case StiTypeMode.NullableValue:
                    if (type == typeof(bool)) return typeof(bool?);
                    else if (type == typeof(byte)) return typeof(byte?);
                    else if (type == typeof(sbyte)) return typeof(sbyte?);
                    else if (type == typeof(ushort)) return typeof(ushort?);
                    else if (type == typeof(short)) return typeof(short?);
                    else if (type == typeof(uint)) return typeof(uint?);
                    else if (type == typeof(int)) return typeof(int?);
                    else if (type == typeof(ulong)) return typeof(ulong?);
                    else if (type == typeof(long)) return typeof(long?);
                    else if (type == typeof(float)) return typeof(float?);
                    else if (type == typeof(double)) return typeof(double?);
                    else if (type == typeof(decimal)) return typeof(decimal?);
                    else if (type == typeof(char)) return typeof(char?);
                    else if (type == typeof(TimeSpan)) return typeof(TimeSpan?);
                    else if (type == typeof(DateTime)) return typeof(DateTime?);
                    else if (type == typeof(DateTimeOffset)) return typeof(DateTimeOffset?);
                    else if (type == typeof(Guid)) return typeof(Guid?);
                    break;
                #endregion

                #region StiTypeMode.List
                case StiTypeMode.List:
                    if (type == typeof(string)) return typeof(StringList);
                    else if (type == typeof(bool)) return typeof(BoolList);
                    else if (type == typeof(byte)) return typeof(ByteList);
                    else if (type == typeof(short)) return typeof(ShortList);
                    else if (type == typeof(int)) return typeof(IntList);
                    else if (type == typeof(long)) return typeof(LongList);
                    else if (type == typeof(float)) return typeof(FloatList);
                    else if (type == typeof(double)) return typeof(DoubleList);
                    else if (type == typeof(decimal)) return typeof(DecimalList);
                    else if (type == typeof(char)) return typeof(CharList);
                    else if (type == typeof(TimeSpan)) return typeof(TimeSpanList);
                    else if (type == typeof(DateTime)) return typeof(DateTimeList);
                    else if (type == typeof(Guid)) return typeof(GuidList);
                    break;
                #endregion

                #region StiTypeMode.Range
                case StiTypeMode.Range:
                    if (type == typeof(string)) return typeof(StringRange);
                    else if (type == typeof(byte)) return typeof(ByteRange);
                    else if (type == typeof(short)) return typeof(ShortRange);
                    else if (type == typeof(int)) return typeof(IntRange);
                    else if (type == typeof(long)) return typeof(LongRange);
                    else if (type == typeof(float)) return typeof(FloatRange);
                    else if (type == typeof(double)) return typeof(DoubleRange);
                    else if (type == typeof(decimal)) return typeof(DecimalRange);
                    else if (type == typeof(char)) return typeof(CharRange);
                    else if (type == typeof(TimeSpan)) return typeof(TimeSpanRange);
                    else if (type == typeof(DateTime)) return typeof(DateTimeRange);
                    else if (type == typeof(Guid)) return typeof(GuidRange);
                    break;
                #endregion
            }
            
            return type;
        }

		public override string ToString()
		{
			return Name;
		}
        #endregion

        /// <summary>
        /// Creates a new object of the type StiType.
        /// </summary>
        /// <param name="name">Name of type.</param>
        /// <param name="type">Registereg type.</param>
        public StiType(string name, Type type)
		{
			this.Name = name;
			this.Type = type;
		}
	}
}
