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
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Drawing;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base
{
	/// <summary>
	/// Class describes a wrapper for the type.
	/// </summary>
	public class StiTypeWrapper
	{
        #region Properties
        /// <summary>
        /// Gets or sets the Type for which is described wrapper.
        /// </summary>
        public Type Type { get; }
        #endregion

        #region Methods
        public override string ToString()
		{
			return StiTypeConverter.ToString(Type);
		}

        public static string ToString(Type type)
		{
		    return StiTypeConverter.ToString(type);
		}

        /// <summary>
        /// Gets the array of simple types.
        /// </summary>
        /// <returns>Array of simple types.</returns>
        public static StiTypeWrapper[] GetTypeWrappers()
	    {
	        return SimpleTypes
                .ToList()
                .Select(t => new StiTypeWrapper(t))
                .ToArray();
	    }

	    /// <summary>
	    /// Gets the array of base simple types.
	    /// </summary>
	    /// <returns>Array of base simple types.</returns>
	    public static StiTypeWrapper[] GetBaseTypeWrappers()
	    {
	        return SimpleBaseTypes
	            .ToList()
	            .Select(t => new StiTypeWrapper(t))
	            .ToArray();
	    }

        /// <summary>
        /// Returns true if specified type is a simple type for Stimulsoft Reports.
        /// </summary>
	    public static bool IsAllowedType(Type type)
	    {
	        return SimpleTypes.Any(t => t == type);
	    }
        #endregion

        #region Fields
	    public static List<Type> SimpleTypes;

	    public static List<Type> SimpleBaseTypes = new List<Type>
	    {
	        typeof(string),
	        typeof(float),
	        typeof(double),
	        typeof(decimal),
	        typeof(DateTime),
	        typeof(DateTimeOffset),
	        typeof(TimeSpan),
	        typeof(sbyte),
	        typeof(byte),
	        typeof(byte[]),
	        typeof(short),
	        typeof(ushort),
	        typeof(int),
	        typeof(uint),
	        typeof(long),
	        typeof(ulong),
	        typeof(bool),
	        typeof(char),
	        typeof(Guid),
	        typeof(object),
	        typeof(Image),
	    };

	    public static List<Type> SimpleNullableTypes = new List<Type>
	    {
	        typeof(float?),
	        typeof(double?),
	        typeof(decimal?),
	        typeof(DateTime?),
	        typeof(DateTimeOffset?),
	        typeof(TimeSpan?),
	        typeof(sbyte?),
	        typeof(byte?),
	        typeof(short?),
	        typeof(ushort?),
	        typeof(int?),
	        typeof(uint?),
	        typeof(long?),
	        typeof(ulong?),
	        typeof(bool?),
	        typeof(char?),
	        typeof(Guid?)
	    };
        #endregion


        /// <summary>
        /// Creates a new instance of the StiTypeWrapper class.
        /// </summary>
        /// <param name="type">Type for which wrapper is described.</param>
        public StiTypeWrapper(Type type)
		{
			this.Type = type;
		}

		static StiTypeWrapper()
		{
		    SimpleTypes = SimpleBaseTypes.Concat(SimpleNullableTypes).ToList();
        }
	}
}
