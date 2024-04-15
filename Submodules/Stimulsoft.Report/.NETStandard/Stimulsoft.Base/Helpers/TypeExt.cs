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

namespace Stimulsoft.Base.Helpers
{
    public static class TypeExt
    {
        #region Fields
        private static Hashtable numerics = new Hashtable();
        private static Hashtable integers = new Hashtable();
        #endregion

        #region Methods
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
                return false;

            return numerics.ContainsKey(type);
        }

        public static bool IsIntegerType(this Type type)
        {
            if (type == null)
                return false;

            return integers.ContainsKey(type);
        }

        public static bool IsDateType(this Type type)
        {
            if (type == null)
                return false;

            return 
                type == typeof(DateTime) || 
                type == typeof(DateTimeOffset) || 
                type == typeof(TimeSpan);
        }

        public static bool IsStringType(this Type type)
        {
            if (type == null)
                return false;

            return type == typeof(string);
        }

        public static bool IsBooleanType(this Type type)
        {
            if (type == null)
                return false;

            return type == typeof(bool);
        }
        #endregion

        static TypeExt()
        {
            numerics.Add(typeof(sbyte), true);
            numerics.Add(typeof(byte), true);
            numerics.Add(typeof(short), true);
            numerics.Add(typeof(ushort), true);
            numerics.Add(typeof(int), true);
            numerics.Add(typeof(uint), true);
            numerics.Add(typeof(long), true);
            numerics.Add(typeof(ulong), true);
            numerics.Add(typeof(float), true);
            numerics.Add(typeof(double), true);
            numerics.Add(typeof(decimal), true);

            integers.Add(typeof(sbyte), true);
            integers.Add(typeof(byte), true);
            integers.Add(typeof(short), true);
            integers.Add(typeof(ushort), true);
            integers.Add(typeof(int), true);
            integers.Add(typeof(uint), true);
            integers.Add(typeof(long), true);
            integers.Add(typeof(ulong), true);
        }
    }
}